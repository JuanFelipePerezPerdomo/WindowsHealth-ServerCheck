namespace WindowsHealth_ServerCheck.Forms
{
    partial class dfServerForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            grp_digitization = new GroupBox();
            pnl_digitization = new Panel();
            chk_digConfigurate = new CheckBox();
            lab_digReq = new Label();
            rad_digNo = new RadioButton();
            rad_digYes = new RadioButton();
            grp_dfSignature = new GroupBox();
            pnl_dfSignature = new Panel();
            pnl_sigWarning = new Panel();
            chk_clientNotificateSignatures = new CheckBox();
            lab_sigReq = new Label();
            nud_signatureNumber = new NumericUpDown();
            lab_sigCount = new Label();
            rad_sigNo = new RadioButton();
            rad_sigYes = new RadioButton();
            grp_certificates = new GroupBox();
            pnl_certificates = new Panel();
            pnl_certRows = new Panel();
            lab_certDetail = new Label();
            nud_certsNum = new NumericUpDown();
            lab_certCount = new Label();
            rad_certNo = new RadioButton();
            rad_certYes = new RadioButton();
            pnl_footer = new Panel();
            btn_generateForm = new Button();
            btn_cancel = new Button();
            pictureBox1 = new PictureBox();
            label1 = new Label();
            grp_digitization.SuspendLayout();
            pnl_digitization.SuspendLayout();
            grp_dfSignature.SuspendLayout();
            pnl_dfSignature.SuspendLayout();
            pnl_sigWarning.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nud_signatureNumber).BeginInit();
            grp_certificates.SuspendLayout();
            pnl_certificates.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nud_certsNum).BeginInit();
            pnl_footer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // grp_digitization
            // 
            grp_digitization.BackColor = Color.MintCream;
            grp_digitization.Controls.Add(pnl_digitization);
            grp_digitization.Controls.Add(rad_digNo);
            grp_digitization.Controls.Add(rad_digYes);
            grp_digitization.FlatStyle = FlatStyle.Flat;
            grp_digitization.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            grp_digitization.ForeColor = SystemColors.ControlText;
            grp_digitization.Location = new Point(12, 60);
            grp_digitization.Name = "grp_digitization";
            grp_digitization.RightToLeft = RightToLeft.No;
            grp_digitization.Size = new Size(556, 96);
            grp_digitization.TabIndex = 0;
            grp_digitization.TabStop = false;
            grp_digitization.Text = "Digitalización certificada";
            // 
            // pnl_digitization
            // 
            pnl_digitization.BackColor = Color.Khaki;
            pnl_digitization.Controls.Add(chk_digConfigurate);
            pnl_digitization.Controls.Add(lab_digReq);
            pnl_digitization.Location = new Point(12, 52);
            pnl_digitization.Name = "pnl_digitization";
            pnl_digitization.Size = new Size(530, 32);
            pnl_digitization.TabIndex = 2;
            pnl_digitization.Visible = false;
            // 
            // chk_digConfigurate
            // 
            chk_digConfigurate.Location = new Point(0, 5);
            chk_digConfigurate.Name = "chk_digConfigurate";
            chk_digConfigurate.Size = new Size(260, 22);
            chk_digConfigurate.TabIndex = 0;
            chk_digConfigurate.Text = "¿Esta Configurada Correctamente?";
            // 
            // lab_digReq
            // 
            lab_digReq.ForeColor = Color.Crimson;
            lab_digReq.Location = new Point(424, 5);
            lab_digReq.Name = "lab_digReq";
            lab_digReq.Size = new Size(80, 18);
            lab_digReq.TabIndex = 1;
            lab_digReq.Text = "Obligatorio";
            // 
            // rad_digNo
            // 
            rad_digNo.Checked = true;
            rad_digNo.Location = new Point(280, 24);
            rad_digNo.Name = "rad_digNo";
            rad_digNo.Size = new Size(60, 22);
            rad_digNo.TabIndex = 1;
            rad_digNo.TabStop = true;
            rad_digNo.Text = "No";
            // 
            // rad_digYes
            // 
            rad_digYes.Location = new Point(12, 24);
            rad_digYes.Name = "rad_digYes";
            rad_digYes.Size = new Size(260, 22);
            rad_digYes.TabIndex = 0;
            rad_digYes.Text = "Sí, tiene digitalización certificada";
            // 
            // grp_dfSignature
            // 
            grp_dfSignature.Controls.Add(pnl_dfSignature);
            grp_dfSignature.Controls.Add(rad_sigNo);
            grp_dfSignature.Controls.Add(rad_sigYes);
            grp_dfSignature.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            grp_dfSignature.Location = new Point(12, 162);
            grp_dfSignature.Name = "grp_dfSignature";
            grp_dfSignature.Size = new Size(556, 128);
            grp_dfSignature.TabIndex = 1;
            grp_dfSignature.TabStop = false;
            grp_dfSignature.Text = "DF-Signature";
            // 
            // pnl_dfSignature
            // 
            pnl_dfSignature.Controls.Add(pnl_sigWarning);
            pnl_dfSignature.Controls.Add(nud_signatureNumber);
            pnl_dfSignature.Controls.Add(lab_sigCount);
            pnl_dfSignature.Location = new Point(12, 52);
            pnl_dfSignature.Name = "pnl_dfSignature";
            pnl_dfSignature.Size = new Size(530, 66);
            pnl_dfSignature.TabIndex = 2;
            pnl_dfSignature.Visible = false;
            // 
            // pnl_sigWarning
            // 
            pnl_sigWarning.BackColor = Color.Khaki;
            pnl_sigWarning.Controls.Add(chk_clientNotificateSignatures);
            pnl_sigWarning.Controls.Add(lab_sigReq);
            pnl_sigWarning.Location = new Point(0, 36);
            pnl_sigWarning.Name = "pnl_sigWarning";
            pnl_sigWarning.Size = new Size(530, 26);
            pnl_sigWarning.TabIndex = 2;
            pnl_sigWarning.Visible = false;
            // 
            // chk_clientNotificateSignatures
            // 
            chk_clientNotificateSignatures.Location = new Point(6, 3);
            chk_clientNotificateSignatures.Name = "chk_clientNotificateSignatures";
            chk_clientNotificateSignatures.Size = new Size(410, 20);
            chk_clientNotificateSignatures.TabIndex = 0;
            chk_clientNotificateSignatures.Text = "¿El Cliente ha sido notificado de que quedan pocas firmas?";
            // 
            // lab_sigReq
            // 
            lab_sigReq.ForeColor = Color.Crimson;
            lab_sigReq.Location = new Point(424, 5);
            lab_sigReq.Name = "lab_sigReq";
            lab_sigReq.Size = new Size(89, 18);
            lab_sigReq.TabIndex = 1;
            lab_sigReq.Text = "Obligatorio";
            // 
            // nud_signatureNumber
            // 
            nud_signatureNumber.Location = new Point(248, 7);
            nud_signatureNumber.Maximum = new decimal(new int[] { 9999, 0, 0, 0 });
            nud_signatureNumber.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            nud_signatureNumber.Name = "nud_signatureNumber";
            nud_signatureNumber.Size = new Size(80, 25);
            nud_signatureNumber.TabIndex = 1;
            nud_signatureNumber.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // lab_sigCount
            // 
            lab_sigCount.Location = new Point(0, 6);
            lab_sigCount.Name = "lab_sigCount";
            lab_sigCount.Size = new Size(240, 22);
            lab_sigCount.TabIndex = 0;
            lab_sigCount.Text = "¿Con Cuántas firmas Cuenta?";
            lab_sigCount.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // rad_sigNo
            // 
            rad_sigNo.Checked = true;
            rad_sigNo.Location = new Point(260, 24);
            rad_sigNo.Name = "rad_sigNo";
            rad_sigNo.Size = new Size(60, 22);
            rad_sigNo.TabIndex = 1;
            rad_sigNo.TabStop = true;
            rad_sigNo.Text = "No";
            // 
            // rad_sigYes
            // 
            rad_sigYes.Location = new Point(12, 24);
            rad_sigYes.Name = "rad_sigYes";
            rad_sigYes.Size = new Size(240, 22);
            rad_sigYes.TabIndex = 0;
            rad_sigYes.Text = "Sí, tiene firmas de DF-Signature";
            // 
            // grp_certificates
            // 
            grp_certificates.Controls.Add(pnl_certificates);
            grp_certificates.Controls.Add(rad_certNo);
            grp_certificates.Controls.Add(rad_certYes);
            grp_certificates.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            grp_certificates.Location = new Point(10, 296);
            grp_certificates.Name = "grp_certificates";
            grp_certificates.Size = new Size(556, 316);
            grp_certificates.TabIndex = 2;
            grp_certificates.TabStop = false;
            grp_certificates.Text = "Certificados digitales";
            // 
            // pnl_certificates
            // 
            pnl_certificates.Controls.Add(pnl_certRows);
            pnl_certificates.Controls.Add(lab_certDetail);
            pnl_certificates.Controls.Add(nud_certsNum);
            pnl_certificates.Controls.Add(lab_certCount);
            pnl_certificates.Location = new Point(0, 50);
            pnl_certificates.Name = "pnl_certificates";
            pnl_certificates.Size = new Size(548, 260);
            pnl_certificates.TabIndex = 2;
            pnl_certificates.Visible = false;
            // 
            // pnl_certRows
            // 
            pnl_certRows.AutoScroll = true;
            pnl_certRows.BorderStyle = BorderStyle.FixedSingle;
            pnl_certRows.Location = new Point(0, 56);
            pnl_certRows.Name = "pnl_certRows";
            pnl_certRows.Size = new Size(542, 201);
            pnl_certRows.TabIndex = 3;
            // 
            // lab_certDetail
            // 
            lab_certDetail.ForeColor = SystemColors.GrayText;
            lab_certDetail.Location = new Point(0, 34);
            lab_certDetail.Name = "lab_certDetail";
            lab_certDetail.Size = new Size(340, 18);
            lab_certDetail.TabIndex = 2;
            lab_certDetail.Text = "Nombre · Fecha caducidad · Estado · Notificado";
            // 
            // nud_certsNum
            // 
            nud_certsNum.Location = new Point(260, 8);
            nud_certsNum.Maximum = new decimal(new int[] { 20, 0, 0, 0 });
            nud_certsNum.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            nud_certsNum.Name = "nud_certsNum";
            nud_certsNum.Size = new Size(80, 25);
            nud_certsNum.TabIndex = 1;
            nud_certsNum.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // lab_certCount
            // 
            lab_certCount.Location = new Point(0, 6);
            lab_certCount.Name = "lab_certCount";
            lab_certCount.Size = new Size(237, 22);
            lab_certCount.TabIndex = 0;
            lab_certCount.Text = "¿Con Cuántos certificados Cuenta?";
            lab_certCount.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // rad_certNo
            // 
            rad_certNo.Checked = true;
            rad_certNo.Location = new Point(260, 24);
            rad_certNo.Name = "rad_certNo";
            rad_certNo.Size = new Size(60, 22);
            rad_certNo.TabIndex = 1;
            rad_certNo.TabStop = true;
            rad_certNo.Text = "No";
            // 
            // rad_certYes
            // 
            rad_certYes.Location = new Point(12, 24);
            rad_certYes.Name = "rad_certYes";
            rad_certYes.Size = new Size(240, 22);
            rad_certYes.TabIndex = 0;
            rad_certYes.Text = "Sí, tiene certificados digitales";
            // 
            // pnl_footer
            // 
            pnl_footer.Controls.Add(btn_generateForm);
            pnl_footer.Controls.Add(btn_cancel);
            pnl_footer.Dock = DockStyle.Bottom;
            pnl_footer.Location = new Point(0, 618);
            pnl_footer.Name = "pnl_footer";
            pnl_footer.Size = new Size(578, 48);
            pnl_footer.TabIndex = 3;
            // 
            // btn_generateForm
            // 
            btn_generateForm.BackColor = Color.MidnightBlue;
            btn_generateForm.FlatStyle = FlatStyle.Flat;
            btn_generateForm.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btn_generateForm.ForeColor = SystemColors.ControlLight;
            btn_generateForm.Location = new Point(448, 9);
            btn_generateForm.Name = "btn_generateForm";
            btn_generateForm.Size = new Size(120, 30);
            btn_generateForm.TabIndex = 1;
            btn_generateForm.Text = "Generar informe";
            btn_generateForm.UseVisualStyleBackColor = false;
            // 
            // btn_cancel
            // 
            btn_cancel.BackColor = Color.Red;
            btn_cancel.FlatStyle = FlatStyle.Flat;
            btn_cancel.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btn_cancel.ForeColor = SystemColors.ControlLightLight;
            btn_cancel.Location = new Point(340, 9);
            btn_cancel.Name = "btn_cancel";
            btn_cancel.Size = new Size(100, 30);
            btn_cancel.TabIndex = 0;
            btn_cancel.Text = "Cancelar";
            btn_cancel.UseVisualStyleBackColor = false;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = Properties.Resources.DF_SERVER_Smart_Digital_ProcessesAzulojo;
            pictureBox1.Location = new Point(319, 17);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(249, 45);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 4;
            pictureBox1.TabStop = false;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Noto Serif Georgian", 18.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.ForeColor = Color.MidnightBlue;
            label1.Location = new Point(12, 23);
            label1.Name = "label1";
            label1.Size = new Size(289, 34);
            label1.TabIndex = 5;
            label1.Text = "Panel Mantenimiento";
            // 
            // dfServerForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.MintCream;
            ClientSize = new Size(578, 666);
            Controls.Add(label1);
            Controls.Add(pictureBox1);
            Controls.Add(grp_digitization);
            Controls.Add(grp_dfSignature);
            Controls.Add(grp_certificates);
            Controls.Add(pnl_footer);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "dfServerForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Datos DfServer";
            grp_digitization.ResumeLayout(false);
            pnl_digitization.ResumeLayout(false);
            grp_dfSignature.ResumeLayout(false);
            pnl_dfSignature.ResumeLayout(false);
            pnl_sigWarning.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)nud_signatureNumber).EndInit();
            grp_certificates.ResumeLayout(false);
            pnl_certificates.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)nud_certsNum).EndInit();
            pnl_footer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }
        #endregion

        private GroupBox grp_digitization;
        private RadioButton rad_digYes;
        private RadioButton rad_digNo;
        private Panel pnl_digitization;
        private CheckBox chk_digConfigurate;
        private Label lab_digReq;

        private GroupBox grp_dfSignature;
        private RadioButton rad_sigYes;
        private RadioButton rad_sigNo;
        private Panel pnl_dfSignature;
        private Label lab_sigCount;
        private NumericUpDown nud_signatureNumber;
        private Panel pnl_sigWarning;
        private CheckBox chk_clientNotificateSignatures;
        private Label lab_sigReq;

        private GroupBox grp_certificates;
        private RadioButton rad_certYes;
        private RadioButton rad_certNo;
        private Panel pnl_certificates;
        private Label lab_certCount;
        private NumericUpDown nud_certsNum;
        private Label lab_certDetail;
        private Panel pnl_certRows;

        private Panel pnl_footer;
        private Button btn_cancel;
        private Button btn_generateForm;
        private PictureBox pictureBox1;
        private Label label1;
    }
}