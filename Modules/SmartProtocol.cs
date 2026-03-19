using System.Management;
using LibreHardwareMonitor.Hardware;
using WindowsHealth_ServerCheck.Helpers;
using WindowsHealth_ServerCheck.Models;

namespace WindowsHealth_ServerCheck.Modules
{
    public class SmartProtocol
    {
        private class UpdateVisitor : IVisitor
        {
            public void VisitComputer(IComputer computer) { computer.Traverse(this); }
            public void VisitHardware(IHardware hardware)
            {
                hardware.Update();
                foreach (IHardware sub in hardware.SubHardware)
                    sub.Accept(this);
            }
            public void VisitSensor(ISensor sensor) { }
            public void VisitParameter(IParameter parameter) { }
        }

        public static (List<string> log, List<SmartResult> results) smart()
        {
            List<string> log = new List<string>();
            List<SmartResult> results = new List<SmartResult>();
            Computer computer = null;

            try
            {
                computer = new Computer { IsStorageEnabled = true };
                computer.Open();
                computer.Accept(new UpdateVisitor());

                var storageDevices = computer.Hardware
                    .Where(h => h.HardwareType == HardwareType.Storage)
                    .ToList();

                if (storageDevices.Count == 0)
                {
                    log.Add("[AVISO] No se detectaron dispositivos de almacenamiento.");
                    return (log, results);
                }

                log.Add($"Dispositivos de almacenamiento detectados: {storageDevices.Count}");
                log.Add("---");

                foreach (IHardware disk in storageDevices)
                {
                    disk.Update();

                    string diskName = disk.Name ?? "Disco desconocido";
                    string interfaceType = DetectInterface(diskName, disk);

                    log.Add($"[ Disco ] {diskName}  ({interfaceType})");

                    SmartResult result = new SmartResult
                    {
                        Date = DateTime.Now,
                        DiskName = diskName,
                        InterfaceType = interfaceType,
                        HasSmartData = false,
                        HasHealthData = false,
                    };

                    // ── Temperatura ───────────────────────────────────────────────
                    // Primer sensor Temperature con valor disponible
                    ISensor tempSensor = disk.Sensors
                        .Where(s => s.SensorType == SensorType.Temperature && s.Value.HasValue)
                        .OrderBy(s => s.Name.Equals("Temperature", StringComparison.OrdinalIgnoreCase) ? 0 : 1)
                        .FirstOrDefault();

                    if (tempSensor != null)
                    {
                        result.Temperature = (int)Math.Round(tempSensor.Value.Value);
                        result.HasSmartData = true;
                        log.Add($"  > Temperatura ({tempSensor.Name}): {result.Temperature}°C");
                    }

                    // ── Vida útil / salud ─────────────────────────────────────────
                    // LHM expone siempre SensorType.Level para vida útil en SSDs.
                    // Nombres conocidos por fabricante:
                    //   "Life"             → Micron, Crucial
                    //   "Remaining Life"   → Samsung, WD, SanDisk, ADATA, SK Hynix, Intel
                    //   "SSD Life Left"    → Kingston
                    //   "Percentage Used"  → Seagate SSD, algunos NVMe — valor INVERSO (% consumido)
                    // Los HDD mecánicos no exponen ninguno de estos sensores Level.
                    ISensor healthSensor = disk.Sensors
                        .Where(s => s.SensorType == SensorType.Level && s.Value.HasValue &&
                               (s.Name.Equals("Life", StringComparison.OrdinalIgnoreCase) ||
                                s.Name.Equals("Remaining Life", StringComparison.OrdinalIgnoreCase) ||
                                s.Name.Equals("SSD Life Left", StringComparison.OrdinalIgnoreCase) ||
                                s.Name.Equals("Percentage Used", StringComparison.OrdinalIgnoreCase) ||
                                s.Name.IndexOf("life", StringComparison.OrdinalIgnoreCase) >= 0 ||
                                s.Name.IndexOf("health", StringComparison.OrdinalIgnoreCase) >= 0 ||
                                s.Name.IndexOf("wear", StringComparison.OrdinalIgnoreCase) >= 0 ||
                                s.Name.IndexOf("endurance", StringComparison.OrdinalIgnoreCase) >= 0))
                        .FirstOrDefault();

                    if (healthSensor != null)
                    {
                        float rawValue = healthSensor.Value.Value;

                        // "Percentage Used" es el % consumido → invertimos para obtener vida restante
                        bool isInverted = healthSensor.Name.Equals("Percentage Used",
                            StringComparison.OrdinalIgnoreCase);

                        result.HealthPercent = isInverted
                            ? (int)Math.Round(100f - rawValue)
                            : (int)Math.Round(rawValue);

                        result.HasHealthData = true;
                        result.HasSmartData = true;
                        log.Add($"  > Vida útil ({healthSensor.Name}{(isInverted ? ", invertido" : "")}): {result.HealthPercent}%");
                        if (result.HealthPercent < 10)
                            log.Add("  [WARNING] El disco esta llegando al final de su vida util");
                    }

                    // ── Horas de uso ──────────────────────────────────────────────
                    // LHM expone las horas como SensorType.Factor (no Data), Name="Power On Hours".
                    // Buscamos en Factor primero; como fallback también miramos Data por si
                    // algún fabricante lo expone ahí, pero filtrando siempre por nombre
                    // para no confundir con GB escritos u otros contadores.
                    ISensor hourSensor =
                        disk.Sensors
                            .Where(s => s.SensorType == SensorType.Factor && s.Value.HasValue &&
                                   s.Name.IndexOf("power on hour", StringComparison.OrdinalIgnoreCase) >= 0)
                            .FirstOrDefault()
                        ?? disk.Sensors
                            .Where(s => s.SensorType == SensorType.Data && s.Value.HasValue &&
                                   s.Name.IndexOf("power on hour", StringComparison.OrdinalIgnoreCase) >= 0)
                            .FirstOrDefault();

                    if (hourSensor != null)
                    {
                        result.HoursUsed = (int)Math.Round(hourSensor.Value.Value);
                        result.HasSmartData = true;
                        log.Add($"  > Horas de uso ({hourSensor.Name}): {result.HoursUsed} h");
                    }

                    // ── PredictFailure ────────────────────────────────────────────
                    // NVMe: WMI no lo soporta → inferimos de vida útil si disponible.
                    // SATA/ATA: intentamos WMI (fuente más fiable); si falla, inferimos.
                    // En ambos casos si tenemos HasHealthData siempre podemos dar un valor.
                    bool predictResolved = false;

                    // USB/Externo y NVMe no pasan por WMI:
                    // USB porque los datos que devuelve WMI via SAT no son fiables,
                    // NVMe porque WMI no soporta ese protocolo.
                    if (interfaceType == "SATA / ATA")
                    {
                        predictResolved = TryGetPredictFailureFromWmi(diskName, out bool wmiPredict, log);
                        if (predictResolved)
                            result.PredictFailure = wmiPredict;
                    }

                    if (!predictResolved)
                    {
                        // Inferencia por vida útil: si tenemos el dato, podemos dar una respuesta
                        if (result.HasHealthData)
                        {
                            result.PredictFailure = result.HealthPercent < 10;
                            predictResolved = true;
                            log.Add($"  > ¿Falla inminente? (inferido): {(result.PredictFailure ? "SÍ" : "No")}");
                        }
                    }

                    // predictResolved indica si lab_failed puede mostrar Sí/No
                    // Si sigue false: HasSmartData puede ser true pero PredictFailure queda indeterminado
                    // → en la UI se mostrará "N/D", no "N/A"
                    result.PredictFailureResolved = predictResolved;

                    if (!result.HasSmartData)
                        log.Add("  [INFO] SMART no disponible para este dispositivo (USB u otro bus externo)");

                    log.Add(string.Empty);
                    results.Add(result);
                }

                log.Add("---");
                log.Add($"S.M.A.R.T finalizado — discos con datos: " +
                        $"{results.Count(r => r.HasSmartData)} / {results.Count}");
            }
            catch (Exception ex)
            {
                log.Add("Error en SmartProtocol: " + ex.Message);
            }
            finally
            {
                computer?.Close();
            }

            return (log, results);
        }

        // Consulta WMI para obtener PredictFailure en discos SATA/ATA.
        // Devuelve true si pudo resolverlo, false si WMI no tiene datos para ese disco.
        private static bool TryGetPredictFailureFromWmi(string diskName, out bool predictFailure, List<string> log)
        {
            predictFailure = false;
            try
            {
                var collection = WmiHelper.Query(@"root\wmi",
                    "SELECT * FROM MSStorageDriver_FailurePredictStatus");

                // El InstanceName WMI (ej. "SCSI\Disk&Ven_MTFDDAV2&Prod_56TDL...\...")
                // no coincide directamente con el nombre LHM (ej. "MTFDDAV256TDL-1AW15ABFA").
                // Usamos los primeros 8 caracteres del nombre como token de vendor,
                // que sí aparece en el fragmento "Ven_XXXXXXXX" del InstanceName.
                string vendorToken = diskName.Length >= 8
                    ? diskName.Substring(0, 8)
                    : diskName;

                foreach (ManagementObject obj in collection)
                {
                    string instance = obj["InstanceName"]?.ToString() ?? string.Empty;

                    if (instance.IndexOf(vendorToken, StringComparison.OrdinalIgnoreCase) < 0)
                        continue;

                    predictFailure = (bool)obj["PredictFailure"];
                    log.Add($"  > ¿Falla inminente? (WMI): {(predictFailure ? "SÍ" : "No")}");
                    return true;
                }
            }
            catch
            {
                // WMI no disponible o acceso denegado — el caller usa inferencia
            }
            return false;
        }

        private static string DetectInterface(string diskName, IHardware disk)
        {
            string upper = diskName.ToUpperInvariant();

            if (upper.Contains("NVME") || upper.Contains("NVM EXPRESS"))
                return "NVMe";

            // LHM suele incluir "USB" en el nombre de discos externos conectados por USB
            if (upper.Contains("USB"))
                return "USB / Externo";

            // Si ningún sensor tiene valor, tampoco hay comunicación SMART directa
            bool hasAnyValue = disk.Sensors.Any(s => s.Value.HasValue);
            if (!hasAnyValue)
                return "USB / Externo";

            return "SATA / ATA";
        }
    }
}