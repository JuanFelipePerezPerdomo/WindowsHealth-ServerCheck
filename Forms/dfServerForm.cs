using WindowsHealth_ServerCheck.Models;

namespace WindowsHealth_ServerCheck.Forms
{
    public partial class dfServerForm : Form
    {
        public DfServerData DfData { get; private set; } = null;

        private readonly List<(TextBox txtName, DateTimePicker dtp, CheckBox chkNotified, Label labStatus)> _certRows
            = new List<(TextBox, DateTimePicker, CheckBox, Label)>();

        private const int CertRowHeight = 38;

        // Panel interno sin scroll que crece con las filas.
        // pnl_certRows (externo, AutoScroll=true) actúa solo como ventana.
        // Esto evita el bug de WinForms donde AutoScroll desplaza el origen
        // de coordenadas y las filas nuevas aparecen fuera del área visible.
        private Panel _innerPanel;

        public dfServerForm()
        {
            InitializeComponent();
            BuildInnerPanel();
            WireEvents();
            RefreshSectionVisibility();
        }

        // Crea el panel interno y lo añade a pnl_certRows
        private void BuildInnerPanel()
        {
            _innerPanel = new Panel
            {
                Location = new System.Drawing.Point(0, 0),
                Width = pnl_certRows.ClientSize.Width,
                Height = 0,
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top,
            };
            pnl_certRows.Controls.Add(_innerPanel);
            pnl_certRows.AutoScroll = true;
        }

        // IMPORTANTE: Los eventos se suscriben ÚNICAMENTE aquí.
        // NO agregue controladores en el Diseñador para estos controles.
        private void WireEvents()
        {
            rad_digYes.CheckedChanged += (s, e) => RefreshSectionVisibility();
            rad_sigYes.CheckedChanged += (s, e) => RefreshSectionVisibility();
            rad_certYes.CheckedChanged += (s, e) => RefreshSectionVisibility();
            nud_signatureNumber.ValueChanged += (s, e) => RefreshSignatureWarning();
            nud_certsNum.ValueChanged += (s, e) => RebuildCertRows();
            btn_generateForm.Click += btn_generateForm_Click;
            btn_cancel.Click += btn_cancel_Click;
        }

        // Visibilidad de secciones 
        private void RefreshSectionVisibility()
        {
            pnl_digitization.Visible = rad_digYes.Checked;
            pnl_dfSignature.Visible = rad_sigYes.Checked;
            pnl_certificates.Visible = rad_certYes.Checked;

            if (rad_certYes.Checked && _certRows.Count == 0)
                RebuildCertRows();

            RefreshSignatureWarning();
        }

        private void RefreshSignatureWarning()
        {
            pnl_sigWarning.Visible = rad_sigYes.Checked && nud_signatureNumber.Value <= 100;
        }

        // Filas dinámicas de certificados
        private void RebuildCertRows()
        {
            _innerPanel.Controls.Clear();
            _certRows.Clear();

            int count = (int)nud_certsNum.Value;
            for (int i = 0; i < count; i++)
                AddCertRow(i + 1, i * CertRowHeight);

            // Ajustar la altura del inner panel al contenido total
            // AutoScroll del panel externo detecta esto y muestra el scroll si hace falta
            _innerPanel.Height = count * CertRowHeight;

            // Forzar scroll al inicio al reconstruir
            pnl_certRows.AutoScrollPosition = new System.Drawing.Point(0, 0);
        }

        private void AddCertRow(int index, int top)
        {
            Panel row = new Panel
            {
                Left = 0,
                Top = top,
                Width = _innerPanel.Width,
                Height = CertRowHeight - 2,
                BackColor = index % 2 == 0
                    ? System.Drawing.Color.WhiteSmoke
                    : System.Drawing.Color.Transparent,
            };

            int x = 4;

            Label labIdx = new Label
            {
                Text = $"{index}.",
                Left = x,
                Top = 8,
                Width = 22,
                Height = 20,
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
                ForeColor = System.Drawing.SystemColors.GrayText,
            };
            x += 26;

            TextBox txt = new TextBox
            {
                Left = x,
                Top = 6,
                Width = 160,
                Height = 23,
                PlaceholderText = "Nombre del certificado",
            };
            x += 165;

            DateTimePicker dtp = new DateTimePicker
            {
                Left = x,
                Top = 6,
                Width = 110,
                Height = 23,
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Today.AddYears(1),
            };
            x += 115;

            Label labStatus = new Label
            {
                Left = x,
                Top = 8,
                Width = 88,
                Height = 20,
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
                Font = new System.Drawing.Font("Segoe UI", 9f, System.Drawing.FontStyle.Bold),
            };
            x += 93;

            CheckBox chk = new CheckBox
            {
                Text = "Avisado",
                Left = x,
                Top = 8,
                Width = 85,
                Height = 20,
                Visible = false,
            };

            dtp.ValueChanged += (s, e) => UpdateCertRowStatus(dtp, labStatus, chk);
            UpdateCertRowStatus(dtp, labStatus, chk);

            row.Controls.AddRange(new Control[] { labIdx, txt, dtp, labStatus, chk });
            _innerPanel.Controls.Add(row);
            _certRows.Add((txt, dtp, chk, labStatus));
        }

        // Estado de certificado
        private static void UpdateCertRowStatus(DateTimePicker dtp, Label labStatus, CheckBox chk)
        {
            DateTime today = DateTime.Today;
            DateTime expiration = dtp.Value.Date;

            if (expiration < today)
            {
                labStatus.Text = "Caducado";
                labStatus.ForeColor = System.Drawing.Color.Crimson;
                chk.Visible = true;
            }
            else if (expiration <= today.AddMonths(3))
            {
                labStatus.Text = "Caduca pronto";
                labStatus.ForeColor = System.Drawing.Color.DarkOrange;
                chk.Visible = true;
            }
            else
            {
                labStatus.Text = "Vigente";
                labStatus.ForeColor = System.Drawing.Color.SeaGreen;
                chk.Visible = false;
                chk.Checked = false;
            }
        }

        // Validación
        private void btn_generateForm_Click(object sender, EventArgs e)
        {
            if (!ValidateForm()) return;
            DfData = BuildModel();
            DialogResult = DialogResult.OK;
            Close();
        }

        private bool ValidateForm()
        {
            // Sección 1 — Digitalización
            if (rad_digYes.Checked && !chk_digConfigurate.Checked)
            {
                ShowBlocker("Digitalización",
                    "Debes confirmar que la digitalización está configurada correctamente.");
                return false;
            }

            // Sección 2 — DfSignature
            if (rad_sigYes.Checked && nud_signatureNumber.Value <= 100 && !chk_clientNotificateSignatures.Checked)
            {
                ShowBlocker("DfSignature",
                    "El cliente debe ser notificado cuando hay 100 o menos firmas activas.");
                return false;
            }

            // Sección 3 — Certificados
            if (rad_certYes.Checked)
            {
                for (int i = 0; i < _certRows.Count; i++)
                {
                    var (txt, _, chk, _) = _certRows[i];

                    if (string.IsNullOrWhiteSpace(txt.Text))
                    {
                        ShowBlocker("Certificados",
                            $"El certificado #{i + 1} no tiene nombre.");
                        return false;
                    }

                    if (chk.Visible && !chk.Checked)
                    {
                        ShowBlocker("Certificados",
                            $"El certificado \"{txt.Text}\" está caducado o próximo a caducar.\n" +
                            "Debes confirmar que el cliente ha sido avisado.");
                        return false;
                    }
                }
            }

            return true;
        }

        private static void ShowBlocker(string section, string message) =>
            MessageBox.Show(message,
                $"Requisito pendiente — {section}",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);

        // Construcción del modelo
        private DfServerData BuildModel()
        {
            var data = new DfServerData
            {
                HasCertifiedDigitization = rad_digYes.Checked,
                HasConfigureDigitization = rad_digYes.Checked && chk_digConfigurate.Checked,
                HasDfSignature = rad_sigYes.Checked,
                DfSignatureCount = rad_sigYes.Checked ? (int)nud_signatureNumber.Value : 0,
                ClientNotificateSignature = rad_sigYes.Checked && chk_clientNotificateSignatures.Checked,
                HasCertificates = rad_certYes.Checked,
            };

            if (rad_certYes.Checked)
            {
                foreach (var (txt, dtp, chk, _) in _certRows)
                {
                    DateTime today = DateTime.Today;
                    DateTime expiration = dtp.Value.Date;

                    CertificateStatus status =
                        expiration < today ? CertificateStatus.Expired :
                        expiration <= today.AddMonths(3) ? CertificateStatus.ExpiringSoon :
                                                           CertificateStatus.Valid;

                    data.Certificate.Add(new CertificateInfo
                    {
                        Name = txt.Text.Trim(),
                        ExpirationDate = expiration,
                        ClientNotificade = chk.Checked,
                        Status = status,
                    });
                }
            }

            return data;
        }

        // Cierre
        private void btn_cancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
        }
    }
}