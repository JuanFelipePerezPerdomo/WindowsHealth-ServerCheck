using WindowsHealth_ServerCheck.Models;

namespace WindowsHealth_ServerCheck.Modules
{
    public class TempCleaner
    {
        public static (List<string> log, CleanUpResult result) Clean()
        {
            List<string> log = new List<string>();
            CleanUpResult result = new CleanUpResult { Date = DateTime.Now };
            string filePath = Path.GetTempPath();
            try
            {
                string[] files = Directory.GetFiles(filePath);
                string[] dirs = Directory.GetDirectories(filePath);
                if (files.Length == 0 && dirs.Length == 0)
                {
                    log.Add("No hay archivos temporales para borrar.");
                    return (log, result);
                }

                foreach (string file in files)
                {
                    try
                    {
                        long size = new FileInfo(file).Length;

                        File.Delete(file);
                        result.DeleteFiles++;
                        result.FreedBytes += size;
                        result.DeletedFilesNames.Add(Path.GetFileName(file));
                        log.Add($"[OK]      {Path.GetFileName(file)}");
                    }
                    catch
                    {
                        /* Se ignoraran los archivos que estan en uso */
                        result.SkippedFiles++;
                        log.Add($"[EN USO]  {Path.GetFileName(file)}");
                    }
                }

                foreach (string dir in dirs)
                {
                    try
                    {
                        Directory.Delete(dir, true);
                        result.DeleteDirs++;
                        log.Add($"[OK DIR]  {Path.GetDirectoryName(dir)}");
                    }
                    catch
                    {
                        result.SkippedDirs++;
                        log.Add($"[EN USO DIR] {Path.GetDirectoryName(dir)}");
                    }
                }

                log.Add("---");
                log.Add($"Archivos eliminados: {result.DeleteFiles} | Omitidos: {result.SkippedFiles}");
                log.Add($"Carpetas eliminadas: {result.DeleteDirs} | Omitidas: {result.SkippedDirs}");
                log.Add("Limpieza finalizada.");
            }
            catch (IOException ex)
            {
                log.Add("Error general: " + ex.Message);
            }
            return (log, result);
        }
    }
}
