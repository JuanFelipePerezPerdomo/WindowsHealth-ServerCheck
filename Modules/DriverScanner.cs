using System.Management;
using WindowsHealth_ServerCheck.Helpers;
using WindowsHealth_ServerCheck.Models;
using WUApiLib;

namespace WindowsHealth_ServerCheck.Modules
{
    public class DriverScanner
    {
        // Fecha de referencia para drivers genéricos de Microsoft,
        // aunque el driver este actualizado su fecha siempre sera estatica por lo que puede dar problemas
        // a la hora de comparar, por lo que se usara como referencia para no marcar drivers
        // genéricos como desactualizados
        private static readonly string GenericWindowsDriverDate = "21/06/2006";

        // Lista de fabricantes genéricos que suelen tener drivers con fechas antiguas
        // pero que no necesariamente indican que el driver este desactualizado
        private static readonly HashSet<string> GenericManufacturers = new HashSet<string>(
            StringComparer.OrdinalIgnoreCase)
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
            List<string> log = new List<string>();
            DriversResult result = new DriversResult { Date = DateTime.Now };
            try    
            {
                log.Add("Consultando Actualizaciones Pendientes...");

                HashSet<string> pendingUpdateTitles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                Dictionary<string, string> pendingUpdateMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                try
                {
                    UpdateSession updateSession = new UpdateSession();
                    IUpdateSearcher updateSearcher = updateSession.CreateUpdateSearcher();
                    ISearchResult searchResult = updateSearcher.Search("IsInstalled=0 AND Type='Driver'");

                    foreach (IUpdate update in searchResult.Updates)
                    {
                        pendingUpdateTitles.Add(update.Title);
                        foreach (string word in update.Title.Split(' '))
                        {
                            string key = word.Trim().ToLowerInvariant();
                            if (key.Length > 4 && !pendingUpdateMap.ContainsKey(key))
                            {
                                pendingUpdateMap[key] = update.Title;
                            }
                        }
                        log.Add($"Actualizaciones de driver pendientes encontradas: {pendingUpdateTitles.Count}");
                    }
                } catch (Exception ex) 
                {
                    log.Add($"[AVISO] No se pudo consultar Windows Update: {ex.Message}");
                    log.Add("El escaneo continuará solo con datos WMI.");
                }

                log.Add("Consultando drivers instalados...");

                var drives = WmiHelper.Query(@"root\CIMV2", 
                    "SELECT * FROM Win32_PnPSignedDriver WHERE DeviceName IS NOT NULL");

                foreach (ManagementBaseObject drive in drives)
                {
                    string deviceName = drive["DeviceName"]?.ToString() ?? "Unknown Device";
                    string driverVersion = drive["DriverVersion"]?.ToString() ?? "Unknown Version";
                    string driverDate = drive["DriverDate"]?.ToString() ?? "Unknown Date";
                    string manufacturer = drive["Manufacturer"]?.ToString() ?? "Unknown Manufacturer";

                    // Parsear la fecha a un formato legible
                    string driverDateReadable = WmiHelper.ParseWmiDate(driverDate);

                    DriverInfo info = new DriverInfo
                    {
                        DeviceName = deviceName,
                        DriverVersion = driverVersion,
                        DriverDate = driverDateReadable,
                        Manufacturer = manufacturer,
                        IsOutdated = false,
                        UpdatedTitle = string.Empty
                    };

                    if(IsGenericDriver(manufacturer, driverDateReadable))
                    {
                        log.Add($"[GENÉRICO] {deviceName} — v{driverVersion} ({driverDateReadable})");
                        result.Drivers.Add(info);
                        result.TotalDrivers++;
                        continue; // no marcara como desactualizado aunque tenga una fecha antigua al ser un driver genérico de Microsoft
                    }

                    /* Para cruzar esta informacion con las actualizacion pendientes lo que haremos
                     sera que buscaremos una palabra clave del nombre del dispositivo que aparece como
                     clave del map de actualizaciones pendientes
                    */
                    string matchedTitle = FindMatchingUpdate(deviceName, pendingUpdateMap);
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
                log.Add("Escaneo de drivers completado.");
            } catch (Exception ex)
            {
                log.Add("Error crítico en DriverScanner: " + ex.Message);
            }
            return (log, result);
        }
        // Busca si alguna palabra significativa del deviceName tiene coincidencia
        // en el mapa de títulos de actualizaciones pendientes
        private static string? FindMatchingUpdate(string deviceName, Dictionary<string, string> updateMap)
        {
            if (updateMap.Count == 0) return null;

            foreach (string word in deviceName.Split(' '))
            {
                string key = word.Trim().ToLowerInvariant();
                if (key.Length > 4 && updateMap.TryGetValue(key, out string title))
                    return title;
            }
            return null;
        }

        // Método para determinar si un driver es genérico basado en el fabricante y la fecha del driver
        private static bool IsGenericDriver(string manufacturer, string driverDateReadable)
        {
            if (driverDateReadable != GenericWindowsDriverDate)
                return false;

            // coincidencia exacta con fabricantes genéricos conocidos
            if (GenericManufacturers.Contains(manufacturer))
                return true;

            // También se puede hacer una comprobación más flexible para fabricantes genéricos
            if (manufacturer.StartsWith("(Standard", StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        }
    }
}
