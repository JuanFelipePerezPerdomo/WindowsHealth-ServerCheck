using System.Diagnostics;
using WindowsHealth_ServerCheck.Diagnostic;
using WindowsHealth_ServerCheck.Helpers;
using WindowsHealth_ServerCheck.Models;
using WindowsHealth_ServerCheck.Modules;
using WindowsHealth_ServerCheck.Reports;
using WindowsHealth_ServerCheck.Services;

namespace WindowsHealth_ServerCheck.Forms
{
    public partial class mainForm : Form
    {
        private AuditResult _auditResult = new AuditResult();

        public mainForm()
        {
            InitializeComponent();
        }

        private async void btn_TempCleaner_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("¿Desea Borrar los archivos Temporales?",
                "Purgar Archivos temporales",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );
            if (result == DialogResult.Yes)
            {

                try
                {
                    btn_TempCleaner.Enabled = false;
                    var (log, cleanupResult) = await Task.Run(() => TempCleaner.Clean());

                    _auditResult.CleanUp = cleanupResult;
                    _auditResult.CleanupExecuted = true;

                    lab_deleteFiles.Text = cleanupResult.DeleteFiles.ToString();
                    lab_deleteDirs.Text = cleanupResult.DeleteDirs.ToString();
                    lab_totalSize.Text = FormatBytesHelper.FormatBytes(cleanupResult.FreedBytes);

                    string header = $"Limpieza Temporal - {DateTime.Now:dd/MM/yyyy HH:mm:ss} \n";
                    txt_healtInformation.Clear();
                    txt_healtInformation.AppendText(header + Environment.NewLine);
                    foreach (string line in log)
                        txt_healtInformation.AppendText(line + Environment.NewLine);

                    HistoryLogger.Save(header, log);

                    if (cleanupResult.DeleteFiles == 0 && cleanupResult.DeleteDirs == 0 && cleanupResult.SkippedFiles == 0)
                    {
                        MessageBox.Show("No hay archivos para borrar.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else if (log.Any(l => l.StartsWith("Error general:")))
                    {
                        MessageBox.Show("La limpieza finalizó, pero ocurrió un error grave. Revisa el registro.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        MessageBox.Show("Limpieza Finalizada con éxito.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ocurrió un error durante la limpieza: {ex.Message}",
                        "Error en Limpieza",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }
                finally
                {
                    btn_TempCleaner.Enabled = true;
                }
            }
        }

        private async void btn_smartProtocol_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("¿Desea Empezar con el Protocolo Smart?",
                "Iniciar S.M.A.R.T",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                try
                {
                    btn_smartProtocol.Enabled = false;

                    var (log, smartResults) = await Task.Run(() => SmartProtocol.smart());

                    _auditResult.Disks = smartResults;
                    _auditResult.SmartExecuted = true;

                    comB_diskName.Items.Clear();
                    bool inminentFailureDetected = false;

                    foreach (SmartResult disk in smartResults)
                    {
                        comB_diskName.Items.Add($"{disk.DiskName}  [{disk.InterfaceType ?? "Unknown"}]");

                        // Evaluamos si algún disco reportó peligro
                        if (disk.PredictFailure)
                            inminentFailureDetected = true;
                    }

                    if (comB_diskName.Items.Count > 0)
                        comB_diskName.SelectedIndex = 0;

                    btn_viewAllDisk.Enabled = true;

                    string header = $"Protocolo S.M.A.R.T - {DateTime.Now:dd/MM/yyyy HH:mm:ss} \n";
                    txt_healtInformation.Clear();
                    txt_healtInformation.AppendText(header + Environment.NewLine);
                    foreach (string line in log)
                        txt_healtInformation.AppendText(line + Environment.NewLine);

                    HistoryLogger.Save(header, log);

                    if (inminentFailureDetected)
                    {
                        MessageBox.Show("Análisis S.M.A.R.T finalizado.\n\n" +
                            "¡ADVERTENCIA! Se ha detectado que uno o más discos reportan falla inminente. " +
                            "Revise el registro.", "Peligro en Disco",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else if (smartResults.Count == 0)
                    {
                        MessageBox.Show("Análisis finalizado, pero no se detectaron discos compatibles con S.M.A.R.T.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Análisis S.M.A.R.T finalizado con éxito. Todos los discos parecen estar en orden.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ocurrió un error durante el análisis SMART: {ex.Message}",
                        "Error en SMART",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }
                finally
                {
                    btn_smartProtocol.Enabled = true;
                }
            }
        }
        private void btn_winUpdate_Click(object sender, EventArgs e)
        {
            updateForm upForm = new updateForm(chkBox_EnableSystemUpdate.Checked);
            upForm.FormClosed += (s, args) => { btn_winUpdate.Enabled = true; };

            upForm.ShowDialog();

            if (upForm.UpdateResult != null)
            {
                _auditResult.Updates = upForm.UpdateResult;
                _auditResult.UpdatesExecuted = true;

                UpdateResult upResult = upForm.UpdateResult;
                bool installing = chkBox_EnableSystemUpdate.Checked;

                lab_foundUpd.Text = upResult.UpdatesFound.ToString();
                lab_foundUpd.ForeColor = Color.Black;

                if (installing)
                {
                    lab_succesUpd.Text = upResult.UpdatesInstalled.ToString();
                    lab_succesUpd.ForeColor = upResult.UpdatesInstalled > 0 ? Color.Green : Color.Black;
                    lab_failedUpd.Text = (upResult.UpdatesFound - upResult.UpdatesInstalled).ToString();
                    lab_failedUpd.ForeColor = (upResult.UpdatesFound - upResult.UpdatesInstalled) > 0
                        ? Color.Red : Color.Black;
                }
                else
                {
                    lab_succesUpd.Text = "N/A";
                    lab_succesUpd.ForeColor = Color.Gray;
                    lab_failedUpd.Text = "N/A";
                    lab_failedUpd.ForeColor = Color.Gray;
                }
            }
        }

        private async void btn_startDriverComp_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("¿Desea Empezar con el Escaneo de Drivers?",
                "Iniciar Escaneo de Drivers",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                btn_startDriverComp.Enabled = false;
                lab_driverNotUpdated.Text = "-";
                txt_healtInformation.Clear();
                txt_healtInformation.AppendText("Escaneando drivers... esto puede tardar un momento." + Environment.NewLine);

                var (log, driversResult) = await Task.Run(() => DriverScanner.Scan());

                _auditResult.Drivers = driversResult;
                _auditResult.DriversExecuted = true;

                lab_TotalDriversScan.Text = driversResult.TotalDrivers.ToString();
                lab_driverNotUpdated.Text = driversResult.OutdatedDrivers.ToString();
                lab_driverNotUpdated.ForeColor = driversResult.OutdatedDrivers > 0 ? Color.Red : Color.Green;

                string header = $"Escaneo de Drivers - {DateTime.Now:yyyy/MM/dd HH:mm:ss} \n";
                txt_healtInformation.Clear();
                txt_healtInformation.AppendText(header + Environment.NewLine);
                foreach (string line in log)
                    txt_healtInformation.AppendText(line + Environment.NewLine);

                HistoryLogger.Save(header, log);

                btn_startDriverComp.Enabled = true;

            }
        }

        private void btn_GenerateAudit_Click(object sender, EventArgs e)
        {
            _auditResult.Date = DateTime.Now;

            if (!_auditResult.CleanupExecuted && !_auditResult.SmartExecuted
                && !_auditResult.UpdatesExecuted && !_auditResult.DriversExecuted)
            {
                MessageBox.Show("No se ha ejecutado ningún módulo todavía.",
                    "Sin datos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (chkbox_dfserver.Checked)
            {
                if (!ShowDfServerForm()) return;
            }
            ReportBuilder.Generate(_auditResult);
        }

        private async void btn_startEveryone_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("¿Estas Seguro de que quieres ejecutar todos los modulos?",
                "Ejecutar Auditoria Completa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                if (chkbox_dfserver.Checked)
                {
                    if (!ShowDfServerForm()) return;
                }

                btn_viewAllDisk.Enabled = true;
                btn_startEveryone.Enabled = false;
                btn_startDriverComp.Enabled = false;
                txt_healtInformation.Clear();

                var (audit, logs) = await AuditService.RunAll(log =>
                {
                    txt_healtInformation.AppendText(log + Environment.NewLine);
                });

                // — Cleanup UI
                _auditResult = audit;
                lab_deleteFiles.Text = audit.CleanUp.DeleteFiles.ToString();
                lab_deleteDirs.Text = audit.CleanUp.DeleteDirs.ToString();
                lab_totalSize.Text = FormatBytesHelper.FormatBytes(audit.CleanUp.FreedBytes);

                // — SMART UI
                comB_diskName.Items.Clear();
                foreach (SmartResult disk in audit.Disks)
                    comB_diskName.Items.Add($"{disk.DiskName}  [{disk.InterfaceType ?? "Unknown"}]");
                if (comB_diskName.Items.Count > 0)
                    comB_diskName.SelectedIndex = 0;

                // — Drivers UI
                lab_TotalDriversScan.Text = audit.Drivers.TotalDrivers.ToString();
                lab_driverNotUpdated.Text = audit.Drivers.OutdatedDrivers.ToString();
                lab_driverNotUpdated.ForeColor = audit.Drivers.OutdatedDrivers > 0 ? Color.Red : Color.Green;

                // — Windows Update UI
                List<string> updateLog = new List<string>();
                updateForm upForm = new updateForm(chkBox_EnableSystemUpdate.Checked);
                upForm.ShowDialog();

                if (upForm.UpdateResult != null)
                {
                    _auditResult.Updates = upForm.UpdateResult;
                    _auditResult.UpdatesExecuted = true;
                    updateLog = upForm.FinalLog;
                    bool installing = chkBox_EnableSystemUpdate.Checked;

                    lab_foundUpd.Text = upForm.UpdateResult.UpdatesFound.ToString();
                    lab_foundUpd.ForeColor = Color.Black;

                    if (installing)
                    {
                        lab_succesUpd.Text = upForm.UpdateResult.UpdatesInstalled.ToString();
                        lab_succesUpd.ForeColor = upForm.UpdateResult.UpdatesInstalled > 0 ? Color.Green : Color.Black;
                        lab_failedUpd.Text = (upForm.UpdateResult.UpdatesFound -
                                                  upForm.UpdateResult.UpdatesInstalled).ToString();
                        lab_failedUpd.ForeColor = (upForm.UpdateResult.UpdatesFound -
                                                  upForm.UpdateResult.UpdatesInstalled) > 0 ? Color.Red : Color.Black;
                    }
                    else
                    {
                        lab_succesUpd.Text = "N/A";
                        lab_succesUpd.ForeColor = Color.Gray;
                        lab_failedUpd.Text = "N/A";
                        lab_failedUpd.ForeColor = Color.Gray;
                    }
                }

                // Histórico Completo
                string header = $"Auditoría Completa - {DateTime.Now:dd/MM/yyyy HH:mm:ss} \n";
                List<string> fullLog = new List<string>();
                foreach (var entry in logs)
                    fullLog.AddRange(entry.Value);
                fullLog.AddRange(updateLog);
                HistoryLogger.Save(header, fullLog);

                // Generar PDF solo si no hubo errores críticos
                bool hasErrors = fullLog.Exists(l => l.StartsWith("Error CRÍTICO"));
                if (hasErrors)
                {
                    MessageBox.Show(
                        "Se detectaron errores durante la auditoría. Revisa el histórico antes de generar el informe.",
                        "Errores detectados",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
                else
                {
                    _auditResult.Date = DateTime.Now;
                    ReportBuilder.Generate(_auditResult);
                }
                btn_startEveryone.Enabled = true;
                btn_startDriverComp.Enabled = true;
            }
        }
        private void btn_HealtHistory_Click(object sender, EventArgs e)
        {
            if (File.Exists(HistoryLogger.HistoryPath))
            {
                Process.Start("notepad.exe", HistoryLogger.HistoryPath);
            }
            else
            {
                MessageBox.Show(
                    "Aún no existe un histórico guardado.",
                    "Sin histórico",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
        }

        // ── DfServer: abre el formulario, asigna los datos y devuelve si se completó ──
        // Devuelve true si el usuario completó y confirmó el formulario.
        // Devuelve false si canceló o cerró sin completarlo, y muestra aviso.
        private bool ShowDfServerForm()
        {
            using dfServerForm form = new dfServerForm();
            DialogResult dr = form.ShowDialog();

            if (dr == DialogResult.OK && form.DfData != null)
            {
                _auditResult.DfServer = form.DfData;
                _auditResult.DfServerExecuted = true;
                return true;
            }

            // El usuario canceló o cerró sin completar
            MessageBox.Show(
                "El informe no se ha generado porque el formulario de DF-Server no fue completado.\n\n" +
                "Puedes desmarcar la opción 'DF-Server' si no necesitas incluir esta sección.",
                "Informe no generado",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            return false;
        }

        private void chkbox_dfserver_CheckedChanged(object sender, EventArgs e) { }

        private void chkBox_EnableSystemUpdate_CheckedChanged(object sender, EventArgs e)
        {
            if (chkBox_EnableSystemUpdate.Checked)
            {
                MessageBox.Show("¡Recuerda que al activar esta opción estás habilitando " +
                    "la actualización automática del sistema operativo!",
                    "Advertencia",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }
        }

        private void btn_openWinUpdPanel_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "ms-settings:windowsupdate",
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"No se pudo abrir el panel de Windows Update: {ex.Message}",
                    "Error al abrir Windows Update",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btn_openDeviceManager_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "devmgmt.msc",
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"No se pudo abrir el Administrador de Dispositivos: {ex.Message}",
                    "Error al abrir el Administrador de Dispositivos",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // Selección de disco
        private void comB_diskName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_auditResult?.Disks == null || comB_diskName.SelectedIndex < 0) return;

            SmartResult disk = _auditResult.Disks[comB_diskName.SelectedIndex];

            // Disco sin ningún dato SMART (USB, pendrive, bus externo sin soporte)
            if (!disk.HasSmartData)
            {
                SetLabel(lab_temperature, "N/A", Color.Gray);
                SetLabel(lab_diskHealth, "N/A", Color.Gray);
                SetLabel(lab_hours, "N/A", Color.Gray);
                SetLabel(lab_failed, "N/A", Color.Gray);
                return;
            }

            // Temperatura 
            lab_temperature.Text = $"{disk.Temperature}°C";
            lab_temperature.ForeColor = disk.Temperature < 45 ? Color.Green
                : disk.Temperature <= 55 ? Color.Goldenrod : Color.Red;

            // Vida útil (solo SSDs con atributo de salud) 
            if (disk.HasHealthData)
            {
                lab_diskHealth.Text = $"{disk.HealthPercent}%";
                lab_diskHealth.ForeColor = disk.HealthPercent >= 90 ? Color.Green
                    : disk.HealthPercent >= 60 ? Color.Goldenrod : Color.Red;
            }
            else
            {
                SetLabel(lab_diskHealth, "N/D", Color.Gray);
            }

            // Horas de uso 
            if (disk.HoursUsed > 0)
            {
                lab_hours.Text = $"{disk.HoursUsed} h";
                lab_hours.ForeColor = disk.HoursUsed < 20000 ? Color.Green
                    : disk.HoursUsed < 40000 ? Color.Goldenrod : Color.Red;
            }
            else
            {
                SetLabel(lab_hours, "N/D", Color.Gray);
            }

            // Falla inminente
            // PredictFailureResolved = true  → tenemos una respuesta definitiva (Sí / No)
            // PredictFailureResolved = false → no hubo fuente disponible para determinarlo (N/D)
            // HasSmartData = false           → ya manejado arriba con N/A
            if (disk.PredictFailureResolved)
            {
                lab_failed.Text = disk.PredictFailure ? "Sí" : "No";
                lab_failed.ForeColor = disk.PredictFailure ? Color.Red : Color.Green;
            }
            else
            {
                SetLabel(lab_failed, "N/D", Color.Gray);
            }
        }

        private static void SetLabel(Label label, string text, Color color)
        {
            label.Text = text;
            label.ForeColor = color;
        }

        private void btn_temp_diagnostic_Click(object sender, EventArgs e)
        {
            var log = SmartDiagnostic.Run();
            txt_healtInformation.Clear();
            foreach (string line in log)
                txt_healtInformation.AppendText(line + Environment.NewLine);
        }
    }
}
