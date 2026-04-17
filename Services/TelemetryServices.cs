using Microsoft.Win32;
using System.Diagnostics;
using System.Management;
using System.Text.RegularExpressions;
using WindowsHealth_ServerCheck.Helpers;
using WindowsHealth_ServerCheck.Models;

namespace WindowsHealth_ServerCheck.Services
{
    public class TelemetryServices
    {
        public static void FetchData(DfServerData DfData, OtherData OtherData)
        {
            FetchRAM(OtherData);
            FetchAntivirus(OtherData);
            FetchNetwork(OtherData);
            FetchNetworkUnits(OtherData);
            FetchBackUps(OtherData);
            FetchDFServerVersion(DfData);
        }
        private static void FetchRAM(OtherData Data)
        {
            try
            {
                foreach (ManagementObject cs in WmiHelper.Query("root\\cimv2",
                    "SELECT TotalPhysicalMemory FROM Win32_ComputerSystem"))
                {
                    using (cs)
                    {
                        Data.RAM = $"{Math.Round(
                            Convert.ToInt64(cs["TotalPhysicalMemory"]) /
                            (1024.0 * 1024.0 * 1024.0))} GB";
                        break;
                    }
                }
            }
            catch (EndOfStreamException ex)
            {
                Console.WriteLine($"Failed to fetch RAM data. Details: {ex.Message}");
            }
        }
        private static void FetchAntivirus(OtherData Data)
        {
            Data.AntiVirusName = "";
            Data.AntiVirusState = "";
            Data.AntiVirusDir = "";
            bool FoundThirdParty = false;

            try
            {
                foreach (ManagementObject av in WmiHelper.Query("root\\SecurityCenter2",
                    "SELECT displayName, productState FROM AntivirusProduct"))
                {
                    using (av)
                    {
                        string name = av["displayName"]?.ToString() ?? "Unknown";
                        if (name.IndexOf("Windows Defender", StringComparison.OrdinalIgnoreCase) >= 0)
                            continue; // Skip Windows Defender

                        Data.AntiVirusName = name;
                        try
                        {
                            uint state = Convert.ToUInt32(av["productState"] ?? 0u);
                            bool active = ((state >> 12) & 0xF) == 1; // Check if enabled
                            bool updated = ((state >> 4) & 0xF) != 10; // Check if up to date (not 10)
                            Data.AntiVirusState = active
                                ? (updated ? "Activo - Actualizado" : "Activo - Desactualizado")
                                : "Desactivado ⚠️";
                        }
                        catch { Data.AntiVirusState = "Estado desconocido"; }
                        FoundThirdParty = true;
                        break; // Stop after finding the first third-party antivirus
                    }
                }
            }
            catch (ManagementException ex)
            {
                Console.WriteLine($"Failed to fetch antivirus data. Details: {ex.Message}");
            }

            if (!FoundThirdParty)
                FoundThirdParty = SearchThirdPartyAv(Data);
            if (!FoundThirdParty)
            {
                try
                {
                    using var s = new ManagementObjectSearcher(
                        "root\\Microsoft\\Windows\\Defender",
                        "SELECT AMProductVersion, AMRunningMode, AntivirusEnabled FROM MSFT_MpComputerStatus");
                    bool defenderFound = false;
                    foreach (ManagementObject mp in s.Get())
                    {
                        using (mp)
                        {
                            string version = mp["AMProductVersion"]?.ToString() ?? "Unknown";
                            string mode = mp["AMRunningMode"]?.ToString() ?? "Unknown";
                            bool avActive = false;
                            try { avActive = Convert.ToBoolean(mp["AntivirusEnabled"] ?? false); }
                            catch { avActive = false; }

                            Data.AntiVirusName = $"Windows Defender (v{version})";
                            Data.AntiVirusState = (!avActive || mode.Contains("Passive"))
                                ? "Pasivo / Deshabilitado ⚠️"
                                : "Activo y actualizado";
                            defenderFound = true;
                            break;
                        }
                    }
                    if (!defenderFound)
                    {
                        Data.AntiVirusName = "No detectado";
                        Data.AntiVirusState = "No fue posible consultar el estado del antivirus";
                    }
                }
                catch (ManagementException ex)
                {
                    Console.WriteLine($"Failed to fetch Windows Defender data. Details: {ex.Message}");
                    Data.AntiVirusName = "No detectado";
                    Data.AntiVirusState = "No fue posible consultar el estado del antivirus";
                }
            }
        }
        private static bool SearchThirdPartyAv(OtherData Data)
        {
            // Nivel 1: Drivers FSFilter Anti-Virus (EDRs con drivers en Kernel)
            try
            {
                foreach (ManagementObject driver in WmiHelper.Query("root\\cimv2",
                    "SELECT DisplayName, State FROM Win32_SystemDriver WHERE Group = 'FSFilter Anti-Virus'"))
                {
                    using (driver)
                    {
                        string displayName = driver["DisplayName"]?.ToString() ?? "Unknown";
                        if (string.IsNullOrEmpty(displayName)) continue;
                        if (displayName.IndexOf("Windows Defender", StringComparison.OrdinalIgnoreCase) >= 0 ||
                            displayName.Equals("wdFilter", StringComparison.OrdinalIgnoreCase)) continue;

                        string cleanName = displayName
                            .Replace("Mini-Filter Driver", "").Replace(" Minifilter Driver", "")
                            .Replace(" File System Filter", "").Replace(" Filter Driver", "").Trim();

                        Data.AntiVirusName = $"{cleanName} (Motor Core EDR)";
                        Data.AntiVirusState = (driver["State"]?.ToString() ?? "Unknow")
                            .Equals("Running", StringComparison.OrdinalIgnoreCase)
                            ? "Activo (Driver en ejecución)" : "Instalado (Driver detenido) ⚠️";
                        return true;
                    }
                }
            }
            catch (ManagementException ex)
            {
                Console.WriteLine($"Failed to search for third-party antivirus drivers. Details: {ex.Message}");
            }
            // Nivel 2: Servicios por DisplayName (agentes userSpace de EDRs)
            string[] edrBrands = {"threatdown", "malwarebytes", "crowdstrike", "sentinel", "sophos",
                                   "cylance", "carbon black", "trellix", "bitdefender", "eset",
                                   "kaspersky", "symantec", "mcafee", "watchguard", "fortinet",
                                   "forticlient", "trend micro", "cybereason", "cortex",
                                   "check point", "cisco", "panda", "avast", "avg", "avira" };
            string[] ignore = { "webadvisor", "vpn", "updater", "installer", "safeconnect",
                                "management agent", "network", "firewall", "identity", "update service" };
            try
            {
                foreach (ManagementObject svc in WmiHelper.Query("root\\cimv2",
                    "SELECT DisplayName, State FROM Win32_Service"))
                {
                    using (svc)
                    {
                        string displayName = svc["DisplayName"]?.ToString() ?? "Unknown";
                        if (string.IsNullOrEmpty(displayName)) continue;
                        string lower = displayName.ToLower();
                        if (ignore.Any(ig => lower.Contains(ig))) continue;
                        if (edrBrands.Any(kw => lower.Contains(kw)))
                        {
                            string state = svc["State"]?.ToString() ?? "";
                            Data.AntiVirusName = displayName;
                            Data.AntiVirusState = state.Equals("Running", StringComparison.OrdinalIgnoreCase)
                                ? "Activo (Servicio en ejecución)" : "Instalado (Servicio detenido) ⚠️";
                            return true;
                        }
                    }
                }
            }
            catch (ManagementException ex)
            {
                Console.WriteLine($"Failed to search for third-party antivirus services. Details: {ex.Message}");
            }
            return false;
        }
        private static void FetchNetwork(OtherData Data)
        {
            Data.RedInterface.Clear();
            try
            {
                foreach (ManagementObject net in WmiHelper.Query("root\\cimv2",
                    "SELECT Name, NetConnectionID, Speed, AdapterType FROM Win32_NetworkAdapter WHERE " +
                    "NetConnectionStatus = 2"))
                {
                    using (net)
                    {
                        string name = net["Name"]?.ToString() ?? "Unknown";
                        string connId = net["NetConnectionID"]?.ToString() ?? "Unknown";
                        long speedBps = 0;
                        try { speedBps = Convert.ToInt64(net["Speed"] ?? 0L); } catch { }
                        string speedStr = speedBps > 0 ? $"{speedBps / 1_000_000} Mbps" : "N/A";
                        string adapterType = net["AdapterType"]?.ToString() ?? "";
                        string typeStr = (adapterType.Contains("Wireless")
                            || connId.ToLower().Contains("wi-fi")
                            || connId.ToLower().Contains("wifi"))
                            ? "Wi-Fi" : adapterType.Contains("Ethernet") ? "Ethernet" : "Otro";

                        Data.RedInterface.Add(new RedInfo
                        {
                            RedName = name,
                            Mbps = speedStr,
                            RedType = typeStr,
                            RedState = "Conectada"
                        });
                    }
                }
            }
            catch
            {
                Data.RedInterface.Clear();
            }
        }
        // Unidades de Red Mapeadas
        [System.Runtime.InteropServices.DllImport("kernel32.dll",
            SetLastError = true, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
        private static extern bool GetDiskFreeSpaceEx(string lpDirectoryName, out ulong lpFreeBytesAvailable,
            out ulong lpTotalNumberOfBytes, out ulong lpTotalNumberOfFreeBytes);

        private static void FetchNetworkUnits(OtherData Data)
        {
            Data.RedUnits.Clear();
            try
            {
                using var usersKey = Registry.Users;
                foreach (string sid in usersKey.GetSubKeyNames())
                {
                    if (sid.EndsWith("_Classes") || sid.Length < 15) continue; // Skip non-user SIDs
                    using var networkKey = usersKey.OpenSubKey($@"{sid}\Network");
                    if (networkKey == null) continue;

                    foreach (string letter in networkKey.GetSubKeyNames())
                    {
                        using var driveKey = networkKey.OpenSubKey(letter);
                        if (driveKey == null) continue;

                        string remotePath = driveKey.GetValue("RemotePath")?.ToString() ?? "";
                        if (string.IsNullOrEmpty(remotePath)) continue;

                        string upperLetter = letter.ToUpper() + ":";
                        if (Data.RedUnits.Any(u => u.Letter == upperLetter && u.Path == remotePath)) continue;

                        var unitInfo = new UnitRedInfo { Letter = upperLetter, Path = remotePath };

                        if (GetDiskFreeSpaceEx(remotePath, out ulong freeBytesAvail, out ulong totalBytes, out _))
                        {
                            unitInfo.TotalGB = totalBytes / 1073741824.0;
                            unitInfo.FreeGB = freeBytesAvail / 1073741824.0;
                            if (unitInfo.TotalGB > 0)
                            {
                                unitInfo.FreePercentage = (unitInfo.FreeGB / unitInfo.TotalGB) * 100;
                                int filledBars = Math.Max(0, Math.Min(10, (int)Math.Round((100 - unitInfo.FreePercentage) / 10.0)));
                                unitInfo.VisualUsage = $"[{new string('█', filledBars)}{new string('░', 10 - filledBars)}]";
                            }
                        }
                        else
                        {
                            unitInfo.VisualUsage = "[No accesible]";
                        }
                        Data.RedUnits.Add(unitInfo);
                    }
                }
            }
            catch
            {
                Data.RedUnits.Clear();
            }
        }
        // BackUps de Windows
        private static void FetchBackUps(OtherData Data)
        {
            Data.BackUpState = "No configurado";
            Data.LastBackUp = "--/--/----";

            // Intento A: Wbadmin (Windows Server Backup)
            try
            {
                string cmdPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "cmd.exe");
                if (Environment.Is64BitOperatingSystem && !Environment.Is64BitProcess)
                    cmdPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows),
                        "sysnative", "cmd.exe");

                var psi = new ProcessStartInfo
                {
                    FileName = cmdPath,
                    Arguments = "/c wbadmin get versions",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var proc = Process.Start(psi);
                string output = proc.StandardOutput.ReadToEnd();
                proc.WaitForExit();

                string lastDate = "";
                foreach (string line in output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    string lower = line.ToLower();
                    if (lower.Contains("backup time:") || lower.Contains("hora de copia")
                        || lower.Contains("hora de la copia"))
                    {
                        var parts = line.Split(new[] { ':' }, 2);
                        if (parts.Length == 2) lastDate = parts[1].Trim();
                    }
                }
                if (!string.IsNullOrEmpty(lastDate))
                {
                    Data.LastBackUp = lastDate;
                    Data.BackUpState = "OK (vía wbadmin)";
                    return;
                }
            }
            catch { }

            // Intento B: WMI MSFT_WBSummary
            try
            {
                foreach (ManagementObject summary in WmiHelper.Query("root\\Microsoft\\Windows\\Backup",
                    "SELECT LastSuccessfulBackupTime, LastBackupResultHR FROM MSFT_WBSummary"))
                {
                    using (summary)
                    {
                        string dateStr = summary["LastSuccessfulBackupTime"]?.ToString() ?? "";
                        if (!string.IsNullOrEmpty(dateStr))
                        {
                            Data.LastBackUp = WmiHelper.ParseWmiDate(dateStr);
                            uint hr = 0;
                            try {  hr = Convert.ToUInt32(summary["LastBackupResultHR"] ?? 0u); } catch { }
                            Data.BackUpState = hr == 0 ? "OK" : $"Error (0x{hr:X8})";
                        }
                        return;
                    }
                }
            } catch
            {
                // Ignorar errores de WMI, ya que este método no está disponible en todas las ediciones de Windows
            }

            // Intento C: Registros de Windows
            try
            {
                using var baseKey = Registry.LocalMachine.OpenSubKey(
                    @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\WindowsBackup\ActionHistory");
                if (baseKey != null)
                {
                    DateTime bestDate = DateTime.MinValue;
                    foreach (string subName in baseKey.GetSubKeyNames())
                    {
                        using var sub = baseKey.OpenSubKey(subName);
                        byte[] ftBytes = sub?.GetValue("ActionItemDate") as byte[];
                        if (ftBytes != null && ftBytes.Length == 8)
                        {
                            DateTime dt = DateTime.FromFileTime(BitConverter.ToInt64(ftBytes, 0));
                            if (dt > bestDate) bestDate = dt;
                        }
                    }
                    if (bestDate > DateTime.MinValue)
                    {
                        Data.LastBackUp = bestDate.ToString("dd/MM/yyyy HH:mm");
                        Data.BackUpState = "OK";
                    }
                }
            }
            catch {/* https://pbs.twimg.com/media/GGym6mYXkAEAt72.jpg */}
        }
        // Version de DF-Server
        private static void FetchDFServerVersion(DfServerData DfData)
        {
            DfData.SoftwareVersion = "No Dectectada";

            string[] registryPaths = {
                @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall",
                @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall"
            };

            foreach (var keyPath in registryPaths)
            {
                try
                {
                    using var baseKey = Registry.LocalMachine.OpenSubKey(keyPath);
                    if (baseKey == null) continue;

                    foreach (string subKeyName in baseKey.GetSubKeyNames())
                    {
                        using var subKey = baseKey.OpenSubKey(subKeyName);
                        string displayName = subKey?.GetValue("DisplayName")?.ToString() ?? "";

                        if (displayName.IndexOf("DF-Server", StringComparison.OrdinalIgnoreCase) < 0 &&
                            displayName.IndexOf("DFServer", StringComparison.OrdinalIgnoreCase) < 0)
                            continue;

                        // intento 1: DisplayVersion en el registro
                        string version = subKey?.GetValue("DisplayVersion")?.ToString();
                        if (!string.IsNullOrWhiteSpace(version))
                        {
                            DfData.SoftwareVersion = version;
                            return;
                        }

                        // Intento 2: Ejecutable en InstallLocation
                        string installLocation = subKey?.GetValue("InstallLocation")?.ToString() ?? "";
                        if (string.IsNullOrWhiteSpace(installLocation))
                            installLocation = @"C:\Program Files (x86)\SIT\DF-SERVER_EVO(Server)";

                        string exeVersion = ExtractVersionFromFile(
                            Path.Combine(installLocation, "DFServer_kernel.exe"));
                        if (!string.IsNullOrWhiteSpace(exeVersion))
                        {
                            DfData.SoftwareVersion = exeVersion;
                            return;
                        }
                        break;
                    }
                }
                catch
                {
                    // Ignorar errores de acceso al registro y continuar con la siguiente ruta  
                }           
            }
            // Intento 3: Ruta estándar garantizada (insertar enlace de broma de arriba si no funciona)
            string fallbackExe = @"C:\Program Files (x86)\SIT\DF-SERVER_EVO(Server)\DFServer_kernel.exe";
            string fallbackVersion = ExtractVersionFromFile(fallbackExe);

            DfData.SoftwareVersion = !string.IsNullOrWhiteSpace(fallbackVersion)
                ? fallbackVersion
                : "Versión oculta (Registro y archivo sin datos)";
        }
        private static string ExtractVersionFromFile(string path)
        {
            if(!File.Exists(path)) return null;
            try
            {
                var info = FileVersionInfo.GetVersionInfo(path);
                string version = info.FileVersion ?? info.ProductVersion;
                if (!string.IsNullOrWhiteSpace(version) && version.Trim() != "0.0.0.0")
                    return version;

                return $"Desconocida (Compilado: {File.GetLastWriteTime(path):dd/MM/yyyy})";
            }
            catch { return null; }
        }
        // Java 
        public static async Task FetchJavaVersion(OtherData Data, HttpClient http)
        {
            Data.JavaVersion = "No detectado";
            Data.JavaUpToDate = false;
            Data.OnlineJavaVersion = "";
            string installedVersion = "";
            int localUpdate = 0;

            string[] registryPaths = {
                @"SOFTWARE\JavaSoft\Java Runtime Environment",
                @"SOFTWARE\WOW6432Node\JavaSoft\Java Runtime Environment"
            };

            foreach (var keyPath in registryPaths)
            {
                try
                {
                    using var key = Registry.LocalMachine.OpenSubKey(keyPath);
                    if (key == null) continue;
                    foreach (string subName in key.GetSubKeyNames())
                    {
                        var match = Regex.Match(subName, @"1\.8\.0_(\d+)");
                        if (match.Success)
                        {
                            int update = int.Parse(match.Groups[1].Value);
                            if (update > localUpdate)
                            {
                                localUpdate = update;
                                installedVersion = subName;
                            }                        
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Failed to fetch local Java version. Details: {e.Message}");
                }
            }

            if (localUpdate == 0) return; // Java no encontrado

            Data.JavaVersion = $"Java 8 Update {localUpdate}";

            try
            {
                string html = await http.GetStringAsync("https://www.java.com/es/download/");
                var matchOnline = Regex.Match(html, @"Versi(?:ó|o)n 8 Update (\d+)");

                if (matchOnline.Success)
                {
                    int onlineUpdate = int.Parse(matchOnline.Groups[1].Value);
                    Data.OnlineJavaVersion = $"Java 8 Update {onlineUpdate} (Fuente: java.com)";

                    if (localUpdate >= onlineUpdate)
                    {
                        Data.JavaUpToDate = true;
                        Data.JavaVersion += " ✅";
                    }
                    else
                    {
                        Data.JavaUpToDate = false;
                        Data.JavaVersion += $" ⚠️ (disponible Update {onlineUpdate})";
                    }
                }
                else
                {
                    Data.OnlineJavaVersion = "No se pudo encontrar la versión en java.com";
                }
            }
            catch
            {
                Data.OnlineJavaVersion = "Error al consultar java.com";
            }
        }
    }
}
