using Microsoft.VisualBasic.Logging;
using WindowsHealth_ServerCheck.Models;

namespace WindowsHealth_ServerCheck.Modules
{
    public class TempCleaner
    {

        // perfil de sistema, no se debe limpiar, no son usuarios reales
        private static readonly HashSet<string> _systemProfiles = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Default", "Default User", "Public", "All Users", 
            "systemprofile", "LocalService", "NetworkService"
        };

        public static (List<string> log, CleanUpResult result) Clean()
        {
            List<string> log = new List<string>();
            CleanUpResult result = new CleanUpResult { Date = DateTime.Now };

            List<string> tempPaths = GetAllTempPaths(log);

            if (tempPaths.Count == 0)
            {
                log.Add("No se encontraron carpetas temporales accesibles.");
                return (log, result);
            }

            foreach (string tempPath in tempPaths)
            {
                CleanFolder(tempPath, result, log);
            }

            log.Add("---");
            log.Add($"Archivos eliminados: {result.DeleteFiles} | Omitidos: {result.SkippedFiles}");
            log.Add($"Carpetas eliminadas: {result.DeleteDirs} | Omitidas: {result.SkippedDirs}");
            log.Add("Limpieza finalizada.");

            return (log, result);
        }

        private static List<string> GetAllTempPaths(List<string> log)
        {
            List<string> paths = new List<string>();
            string userRoot = Path.GetDirectoryName(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));

            if (!Directory.Exists(userRoot))
            {
                log.Add($"[AVISO] no se pudo acceder a la carpeta de perfiles {userRoot}");

                paths.Add(Path.GetTempPath());
                return paths;
            }

            foreach (string profileDir in Directory.GetDirectories(userRoot))
            {
                string profileName = Path.GetFileName(profileDir);

                if (_systemProfiles.Contains(profileName))
                    continue;

                string tempPath = Path.Combine(profileDir, "AppData", "Local", "Temp");

                if (Directory.Exists(tempPath))
                {
                    log.Add($"[ Usuario ] {profileName}  →  {tempPath}");
                    paths.Add(tempPath);
                }
            }

            // Si no se encontro ningun perfil, caer al usuario actual
            if (paths.Count == 0)
            {
                string fallback = Path.GetTempPath();
                log.Add($"[ Usuario ] (actual)  →  {fallback}");
                paths.Add(fallback);
            }

            return paths;
        }

        // Limpia una carpeta temp concreta y acumula resultados en CleanUpResult
        private static void CleanFolder(string folderPath, CleanUpResult result, List<string> log)
        {
            try
            {
                string[] files = Directory.GetFiles(folderPath);
                string[] dirs = Directory.GetDirectories(folderPath);

                foreach (string file in files)
                {
                    try
                    {
                        long size = new FileInfo(file).Length;
                        File.Delete(file);
                        result.DeleteFiles++;
                        result.FreedBytes += size;
                        result.DeletedFilesNames.Add(Path.GetFileName(file));
                        log.Add($"[OK]         {Path.GetFileName(file)}");
                    }
                    catch
                    {
                        result.SkippedFiles++;
                        log.Add($"[EN USO]     {Path.GetFileName(file)}");
                    }
                }

                foreach (string dir in dirs)
                {
                    try
                    {
                        Directory.Delete(dir, true);
                        result.DeleteDirs++;
                        log.Add($"[OK DIR]     {Path.GetFileName(dir)}");
                    }
                    catch
                    {
                        result.SkippedDirs++;
                        log.Add($"[EN USO DIR] {Path.GetFileName(dir)}");
                    }
                }
            }
            catch (Exception ex)
            {
                log.Add($"[ERROR] {folderPath}: {ex.Message}");
            }
        }
    }
}
