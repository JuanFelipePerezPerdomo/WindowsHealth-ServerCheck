using System.Management;
using WindowsHealth_ServerCheck.Helpers;
using WindowsHealth_ServerCheck.Models;
using WUApiLib;

namespace WindowsHealth_ServerCheck.Modules
{
    public class DriverScanner
    {
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
    }
}
