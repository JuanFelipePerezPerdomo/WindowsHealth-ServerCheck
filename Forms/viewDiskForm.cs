using WindowsHealth_ServerCheck.Models;

namespace WindowsHealth_ServerCheck.Forms
{
    public partial class viewDiskForm : Form
    {
        private FlowLayoutPanel flpnl_containerPanel;
        private List<SmartResult> _disk;
        public viewDiskForm(List<SmartResult> disks)
        {
            InitializeComponent();
            _disk = disks;
            UIConfig();
            loadDiskInfo();
        }

        private void UIConfig()
        {
            Text = "Informacion de discos";
            Size = new Size(500, 600);
            StartPosition = FormStartPosition.CenterScreen;

            flpnl_containerPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Padding = new Padding(15)
            };
            Label title = lab_addTag(flpnl_containerPanel, "Discos de almacenamiento detectados", 15, 15, new Font("Noto Serif Georgian", 16, FontStyle.Bold));
            title .ForeColor = Color.MidnightBlue;

            Label separator = lab_addTag(flpnl_containerPanel, "", 13, 74, new Font("Arial", 1));
            separator.AutoSize = false;              // IMPORTANTE: Para poder darle un Size manual
            separator.BorderStyle = BorderStyle.Fixed3D;
            separator.Size = new Size(440, 2);       // Aquí definimos el largo y grosor
            separator.Text = "";                     // No necesita texto
            separator.Margin = new Padding(4, 0, 4, 0);


            Controls.Add(flpnl_containerPanel);
        }

        private void loadDiskInfo()
        {
            if (_disk == null || _disk.Count == 0)
            {
                lab_addTag(flpnl_containerPanel, "No se detectaron discos de almacenamiento.", 15, 15, new Font(this.Font.FontFamily, 12, FontStyle.Bold));
                return;
            }

            foreach ( var disk in _disk)
            {
                GroupBox diskGroup = new GroupBox
                {
                    Text = $"{disk.DiskName} [{disk.InterfaceType ?? "Unknown"}]",
                    Width = 430,
                    Height = 170,
                    Margin = new Padding(0, 15, 0, 0),
                    Font = new Font(this.Font, FontStyle.Bold)
                };

                Font defaultFont = new Font(this.Font, FontStyle.Regular);
                int y = 30;
                int dy = 25;

                if(!disk.HasSmartData)
                {
                    Label lab_noSmart = lab_addTag(diskGroup, "No se pudo obtener datos SMART para este disco.", 15, y, defaultFont);
                    lab_noSmart.ForeColor = Color.Gray;
                    flpnl_containerPanel.Controls.Add(diskGroup);
                    continue;
                }

                Label lab_temp = lab_addTag(diskGroup, $"Temperatura: {disk.Temperature}°C", 15, y, defaultFont);
                lab_temp.ForeColor = disk.Temperature < 45 ? Color.Green : disk.Temperature <= 55 ? Color.Goldenrod : Color.Red;
                y += dy;

                if (disk.HasHealthData)
                {
                    Label lab_health = lab_addTag(diskGroup, $"Salud: {disk.HealthPercent}%", 15, y, defaultFont);
                    lab_health.ForeColor = disk.HealthPercent >= 90 ? Color.Green : disk.HealthPercent >= 60 ? Color.Goldenrod : Color.Red;
                   
                } else
                {
                    Label lab_healthNA = lab_addTag(diskGroup, "Información de salud no disponible.", 15, y, defaultFont);
                    lab_healthNA.ForeColor = Color.Gray;
                }
                y += dy;

                if (disk.HoursUsed > 0)
                {
                    Label lab_hours = lab_addTag(diskGroup, $"Horas de uso: {disk.HoursUsed}h", 15, y, defaultFont);
                    lab_hours.ForeColor = disk.HoursUsed < 20000 ? Color.Green : disk.HoursUsed < 40000 ? Color.Goldenrod : Color.Red;
                }
                else
                {
                    Label lab_hoursNA = lab_addTag(diskGroup, "Horas de uso no disponibles.", 15, y, defaultFont);
                    lab_hoursNA.ForeColor = Color.Gray;
                }
                y += dy;
                
                if (disk.PredictFailureResolved)
                {
                    Label lab_Failure = lab_addTag(diskGroup, $"Falla inminente: {(disk.PredictFailure ? "Sí" : "No")}", 15, y, defaultFont);
                    lab_Failure.ForeColor = disk.PredictFailure ? Color.Red : Color.Green;
                    if (disk.PredictFailure) lab_Failure.Font = new Font(defaultFont, FontStyle.Bold); // Resalta si hay peligro
                } else
                {
                    Label lblFalla = lab_addTag(diskGroup, "Falla inminente: N/D", 15, y, defaultFont);
                    lblFalla.ForeColor = Color.Gray;
                }
                
                flpnl_containerPanel.Controls.Add(diskGroup);
            }        
        }

        private Label lab_addTag(Control container, string text, int x, int y, Font? font = null)
        {
            Label tag = new Label
            {
                Text = text,
                Location = new Point(x, y),
                AutoSize = true,
                Font = font ?? this.Font
            };
            container.Controls.Add(tag);
            return tag;
        }
    }
}
