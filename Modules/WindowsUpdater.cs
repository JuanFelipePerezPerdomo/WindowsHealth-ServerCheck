using WindowsHealth_ServerCheck.Models;
using WUApiLib;

namespace WindowsHealth_ServerCheck.Modules
{
    public class WindowsUpdater
    {
        public static (List<string> log, UpdateResult result) updateBrowser(
            Action<string> reportLog,
            IDownloadProgressChangedCallback progressWatcher,
            IDownloadCompletedCallback downloadCompleted,
            IInstallationCompletedCallback installCompleted
            )
        {
            List<string> log = new List<string>();
            UpdateResult result = new UpdateResult { Date = DateTime.Now };

            void Logger(string message)
            {
                log.Add(message);
                reportLog(message);
            }

            try
            {
                UpdateSession updateSession = new UpdateSession();
                IUpdateSearcher updateSearcher = updateSession.CreateUpdateSearcher();

                Logger("Buscando Actualizaciones... espere un momento");
                ISearchResult searchResult = updateSearcher.Search("IsInstalled=0 and Type='Software'");

                result.UpdatesFound = searchResult.Updates.Count;

                if (searchResult.Updates.Count == 0)
                {
                    Logger("No hay Actualizaciones Pendientes");
                    result.Success = true;
                    return (log, result);
                }

                Logger($"Se encontraron {searchResult.Updates.Count} actualizaciones disponibles");

                UpdateCollection updateToDownload = new UpdateCollection();
                foreach (IUpdate update in searchResult.Updates)
                {
                    Logger($"- {update.Title}");
                    result.UpdateTitles.Add(update.Title);
                    updateToDownload.Add(update);
                }

                UpdateDownloader downloader = updateSession.CreateUpdateDownloader();
                downloader.Updates = updateToDownload;

                Logger("Descargando...");

                IDownloadJob job = downloader.BeginDownload(progressWatcher, downloadCompleted, null);
                while (!job.IsCompleted)
                {
                    System.Threading.Thread.Sleep(500);
                }
                downloader.EndDownload(job);
                Logger("Descarga Completada");

                UpdateCollection updatesToInstall = new UpdateCollection();
                foreach (IUpdate update in updateToDownload)
                {
                    if (update.IsDownloaded)
                    {
                        updatesToInstall.Add(update);
                    }
                }

                UpdateInstaller installer = (UpdateInstaller)updateSession.CreateUpdateInstaller();
                installer.Updates = updatesToInstall;
                Logger("Instalando... (Esto puede tardar unos minutos)");

                IInstallationResult installationResult = installer.Install();
                result.UpdatesInstalled = updatesToInstall.Count;
                result.Success = installationResult.ResultCode == OperationResultCode.orcSucceeded;

                Logger($"Resultado: {installationResult.ResultCode}");

            }
            catch (Exception ex)
            {
                Logger("Error CRÍTICO: " + ex.Message);
                result.Success = false;
            }
            return (log, result);
        }
    }
}
