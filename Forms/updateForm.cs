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

        public class DownloadProgressWatcher : IDownloadProgressChangedCallback
        {
            private ProgressBar _pb;
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

        public updateForm()
        {
            InitializeComponent();
            this.Shown += UpdateForm_Shown;
        }

        private void UpdateForm_Shown(object sender, EventArgs e)
        {
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

            var (finalLog, updateResult) = WindowsUpdater.updateBrowser(updateLog, watcher, downloadCompleted, null);

            UpdateResult = updateResult;
            FinalLog = finalLog;

            string header = $"Windows Update - {DateTime.Now:dd/MM/yyyy HH:mm:ss}";
            HistoryLogger.Save(header, finalLog);

            this.Invoke((MethodInvoker)delegate
            {
                pb_Process.Value = 100;
                MessageBox.Show("Proceso finalizado.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            });
        }
    }
}
