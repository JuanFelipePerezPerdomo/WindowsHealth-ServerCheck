using WindowsHealth_ServerCheck.Helpers;
using WindowsHealth_ServerCheck.Models;
using WindowsHealth_ServerCheck.Modules;
using WUApiLib;

namespace WindowsHealth_ServerCheck.Forms
{
    public partial class updateForm : Form
    {
        public List<string> FinalLog { get; private set; } = new List<string>();
        public UpdateResult UpdateResult { get; private set; }

        // Controla si se instalan las actualizaciones o solo se consultan
        private readonly bool _installUpdates;

        public class DownloadProgressWatcher : IDownloadProgressChangedCallback
        {
            private readonly ProgressBar _pb;
            public DownloadProgressWatcher(ProgressBar pb) => _pb = pb;

            public void Invoke(IDownloadJob downloadJob, IDownloadProgressChangedCallbackArgs callbackArgs)
            {
                _pb.Invoke((MethodInvoker)delegate
                {
                    _pb.Value = callbackArgs.Progress.PercentComplete;
                });
            }
        }

        public class DownloadCompletedWatcher : IDownloadCompletedCallback
        {
            public void Invoke(IDownloadJob downloadJob, IDownloadCompletedCallbackArgs callbackArgs) { }
        }

        public class InstallCompletedWatcher : IInstallationCompletedCallback
        {
            public void Invoke(IInstallationJob installationJob, IInstallationCompletedCallbackArgs callbackArgs) { }
        }

        /// <param name="installUpdates">
        /// Viene del estado del checkbox en mainForm.
        /// false → solo consulta. true → descarga e instala.
        /// </param>
        public updateForm(bool installUpdates = false)
        {
            InitializeComponent();
            _installUpdates = installUpdates;
            this.Shown += UpdateForm_Shown;
        }

        private void UpdateForm_Shown(object sender, EventArgs e)
        {
            // Ajustar el título del formulario según el modo
            this.Text = _installUpdates
                ? "Windows Update — Instalando actualizaciones"
                : "Windows Update — Consultando actualizaciones";

            Task.Run(() => StartUpdateProccess());
        }

        private void StartUpdateProccess()
        {
            DownloadProgressWatcher watcher = new DownloadProgressWatcher(pb_Process);
            DownloadCompletedWatcher downloadCompleted = new DownloadCompletedWatcher();

            Action<string> updateLog = (message) =>
            {
                txt_Logs.Invoke((MethodInvoker)delegate
                {
                    txt_Logs.AppendText(message + Environment.NewLine);
                });
            };

            var (finalLog, updateResult) = WindowsUpdater.updateBrowser(
                updateLog,
                watcher,
                downloadCompleted,
                null,
                _installUpdates);

            UpdateResult = updateResult;
            FinalLog = finalLog;

            string header = _installUpdates
                ? $"Windows Update (instalación) - {DateTime.Now:dd/MM/yyyy HH:mm:ss}"
                : $"Windows Update (consulta)    - {DateTime.Now:dd/MM/yyyy HH:mm:ss}";

            HistoryLogger.Save(header, finalLog);

            this.Invoke((MethodInvoker)delegate
            {
                pb_Process.Value = 100;
                string msg = _installUpdates
                    ? "Proceso de actualización finalizado."
                    : "Consulta de actualizaciones finalizada.";
                MessageBox.Show(msg, "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            });
        }
    }
}