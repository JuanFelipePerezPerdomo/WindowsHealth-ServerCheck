// Modules/DriverScanner.cs

using System.Management;
using WindowsHealth_ServerCheck.Helpers;
using WindowsHealth_ServerCheck.Interop;
using WindowsHealth_ServerCheck.Models;
using WUApiLib;

namespace WindowsHealth_ServerCheck.Modules
{
    public class DriverScanner
    {
        private static readonly string GenericWindowsDriverDate = "21/06/2006";

        private static readonly HashSet<string> GenericManufacturers = new(StringComparer.OrdinalIgnoreCase)
        {
            "Microsoft",
            "(Standard system devices)",
            "(Standard display types)",
            "(Standard port types)",
            "(Standard USB Host Controller)",
            "(Standard keyboards)",
            "(Standard mice)",
            "(Standard floppy disk controllers)",
            "(Standard CD-ROM drives)",
            "(Standard AHCI 1.0 Serial ATA Controller)",
            "(Standard IDE ATA/ATAPI controllers)",
        };

        public static (List<string> log, DriversResult result) Scan()
        {
            List<string> log = new();
            DriversResult result = new() { Date = DateTime.Now };

            try
            {
                // ── 1. Actualizaciones pendientes — extraer HardwareID real del objeto ──
                log.Add("Consultando actualizaciones de driver pendientes...");

                // Clave: HardwareId del dispositivo → título del update
                // IUpdate expone IWindowsDriverUpdate que tiene DriverHardwareID
                var pendingByHardwareId = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                try
                {
                    UpdateSession updateSession = new();
                    IUpdateSearcher searcher = updateSession.CreateUpdateSearcher();
                    ISearchResult searchResult = searcher.Search("IsInstalled=0 AND Type='Driver'");

                    foreach (IUpdate update in searchResult.Updates)
                    {
                        // Castear a IWindowsDriverUpdate para acceder al HardwareID real
                        if (update is IWindowsDriverUpdate driverUpdate)
                        {
                            string hwid = driverUpdate.DriverHardwareID ?? string.Empty;
                            if (!string.IsNullOrWhiteSpace(hwid) && !pendingByHardwareId.ContainsKey(hwid))
                            {
                                pendingByHardwareId[hwid] = update.Title;
                                log.Add($"  [WU] HardwareID encontrado: {hwid}");
                            }
                        }
                    }

                    log.Add($"Actualizaciones pendientes: {searchResult.Updates.Count}");
                    log.Add($"Con Hardware ID real extraído: {pendingByHardwareId.Count}");
                }
                catch (Exception ex)
                {
                    log.Add($"[AVISO] No se pudo consultar Windows Update: {ex.Message}");
                }

                // ── 2. Dispositivos con Hardware IDs via DLL nativa ──────────────
                log.Add("Enumerando dispositivos con Hardware IDs via SetupAPI...");

                // Mapa: InstanceId (DeviceID de WMI) → DeviceRecord completo
                var devicesByInstanceId = new Dictionary<string, DeviceRecord>(StringComparer.OrdinalIgnoreCase);

                try
                {
                    var allDevices = HardwareIdScanner.GetAllDevices();
                    foreach (var dev in allDevices)
                        if (!string.IsNullOrWhiteSpace(dev.InstanceId))
                            devicesByInstanceId[dev.InstanceId] = dev;

                    log.Add($"Dispositivos enumerados: {devicesByInstanceId.Count}");
                }
                catch (Exception ex)
                {
                    log.Add($"[AVISO] Error al cargar HardwareIdScanner.dll: {ex.Message}");
                }

                // ── 3. Drivers WMI — cruce por Hardware ID exacto ───────────────
                log.Add("Consultando drivers instalados (WMI)...");

                var wmiDrivers = WmiHelper.Query(@"root\CIMV2",
                    "SELECT * FROM Win32_PnPSignedDriver WHERE DeviceName IS NOT NULL");

                foreach (ManagementBaseObject drive in wmiDrivers)
                {
                    string deviceName = drive["DeviceName"]?.ToString() ?? "Unknown Device";
                    string driverVersion = drive["DriverVersion"]?.ToString() ?? "Unknown Version";
                    string driverDate = drive["DriverDate"]?.ToString() ?? "Unknown Date";
                    string manufacturer = drive["Manufacturer"]?.ToString() ?? "Unknown Manufacturer";
                    string pnpDeviceId = drive["DeviceID"]?.ToString() ?? string.Empty;

                    string driverDateReadable = WmiHelper.ParseWmiDate(driverDate);

                    DriverInfo info = new()
                    {
                        DeviceName = deviceName,
                        DriverVersion = driverVersion,
                        DriverDate = driverDateReadable,
                        Manufacturer = manufacturer,
                        IsOutdated = false,
                        UpdatedTitle = string.Empty
                    };

                    // Drivers genéricos → nunca desactualizado
                    if (IsGenericDriver(manufacturer, driverDateReadable))
                    {
                        log.Add($"[GENÉRICO] {deviceName} — v{driverVersion} ({driverDateReadable})");
                        result.Drivers.Add(info);
                        result.TotalDrivers++;
                        continue;
                    }

                    // Buscar los Hardware IDs reales del dispositivo via SetupAPI
                    string? matchedTitle = null;

                    if (devicesByInstanceId.TryGetValue(pnpDeviceId, out DeviceRecord rec)
                        && rec.HardwareIds != null
                        && pendingByHardwareId.Count > 0)
                    {
                        for (int i = 0; i < rec.HardwareIdCount && i < rec.HardwareIds.Length; i++)
                        {
                            string hwid = rec.HardwareIds[i].HardwareId ?? string.Empty;
                            if (string.IsNullOrWhiteSpace(hwid)) continue;

                            if (pendingByHardwareId.TryGetValue(hwid, out string? title))
                            {
                                matchedTitle = title;
                                break; // match exacto encontrado, detener
                            }
                        }
                    }

                    // SIN fallback por keyword — solo match exacto por Hardware ID
                    if (matchedTitle != null)
                    {
                        info.IsOutdated = true;
                        info.UpdatedTitle = matchedTitle;
                        result.OutdatedDrivers++;
                        log.Add($"[DESACTUALIZADO] {deviceName} — v{driverVersion} ({driverDateReadable})");
                        log.Add($"  → Actualización disponible: {matchedTitle}");
                    }
                    else
                    {
                        log.Add($"[OK] {deviceName} — v{driverVersion} ({driverDateReadable})");
                    }

                    result.Drivers.Add(info);
                    result.TotalDrivers++;
                }

                log.Add("--------");
                log.Add($"Drivers escaneados: {result.TotalDrivers}");
                log.Add($"Drivers desactualizados: {result.OutdatedDrivers}");
                log.Add("Escaneo completado.");
            }
            catch (Exception ex)
            {
                log.Add("Error crítico en DriverScanner: " + ex.Message);
            }

            return (log, result);
        }

        private static bool IsGenericDriver(string manufacturer, string driverDateReadable)
        {
            if (driverDateReadable != GenericWindowsDriverDate) return false;
            if (GenericManufacturers.Contains(manufacturer)) return true;
            if (manufacturer.StartsWith("(Standard", StringComparison.OrdinalIgnoreCase)) return true;
            return false;
        }
    }
}