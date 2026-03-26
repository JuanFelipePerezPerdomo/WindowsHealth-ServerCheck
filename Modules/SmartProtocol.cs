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

                    // Temperatura
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

                    // Vida útil / salud
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

                    // Horas de uso
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

                    // PredictFailure
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

            // 1. Identificador interno de LHM
            // LHM construye el Identifier según el tipo de bus que detectó:
            //   NVMe  →  /nvme/0, /nvme/1, …
            //   SATA  →  /hdd/0,  /hdd/1,  …
            // Esta es la fuente más fiable porque proviene del propio LHM,
            // independientemente de cómo el fabricante haya puesto el nombre.
            string identifier = disk.Identifier.ToString().ToLowerInvariant();

            if (identifier.Contains("/nvme"))
                return "NVMe";

            // 2. Nombre del dispositivo
            // Fallback cuando el Identifier no es concluyente (discos USB que
            // LHM enumera con /hdd/ si logra acceder vía SAT-passthrough).
            if (upper.Contains("NVME") || upper.Contains("NVM EXPRESS"))
                return "NVMe";

            if (upper.Contains("USB"))
                return "USB / Externo";

            // 3. WMI MSFT_PhysicalDisk.BusType
            // Fuente autoritativa del SO. BusType conocidos:
            //   3 = SATA,  7 = USB,  8 = RAID,  11 = NVMe,  17 = SCM, etc.
            // Cruzamos por modelo (primeras palabras del nombre) porque
            // MSFT_PhysicalDisk no expone un ID compartido con LHM.
            string wmiBusResult = TryGetBusTypeFromWmi(diskName);
            if (wmiBusResult != null)
                return wmiBusResult;

            // 4. Ausencia de sensores
            // Si LHM no pudo leer ningún sensor es señal de que el acceso SMART
            // está bloqueado, lo que ocurre típicamente en discos USB sin
            // SAT-passthrough.
            bool hasAnyValue = disk.Sensors.Any(s => s.Value.HasValue);
            if (!hasAnyValue)
                return "USB / Externo";

            return "SATA / ATA";
        }

        // Consulta MSFT_PhysicalDisk para obtener el BusType del disco.
        // Devuelve la cadena de interfaz o null si no se puede determinar.
        private static string TryGetBusTypeFromWmi(string diskName)
        {
            try
            {
                var collection = WmiHelper.Query(
                    @"root\Microsoft\Windows\Storage",
                    "SELECT FriendlyName, BusType FROM MSFT_PhysicalDisk");

                // Tomamos las primeras dos palabras del nombre como token de búsqueda.
                // Ejemplo: "Samsung SSD 970 EVO Plus" → buscamos "Samsung SSD" en FriendlyName.
                string[] words = diskName.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                string searchToken = words.Length >= 2
                    ? $"{words[0]} {words[1]}"
                    : diskName;

                foreach (ManagementBaseObject obj in collection)
                {
                    string friendly = obj["FriendlyName"]?.ToString() ?? string.Empty;
                    if (!friendly.StartsWith(searchToken, StringComparison.OrdinalIgnoreCase))
                        continue;

                    int busType = Convert.ToInt32(obj["BusType"]);
                    return busType switch
                    {
                        17 => "NVMe",
                        11 => "SATA",
                        8 => "RAID", // aunque RAID puede incluir discos SATA o NVMe, lo etiquetamos aparte porque el acceso SMART suele ser distinto
                        7 => "USB / Externo",
                        3 => "ATA",
                        _ => null          // desconocido → continúa con el fallback
                    };
                }
            }
            catch
            {
                // WMI no disponible — el caller continúa con el paso 4
            }
            return null;
        }
    }
}