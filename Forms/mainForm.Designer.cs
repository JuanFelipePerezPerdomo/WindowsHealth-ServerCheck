namespace WindowsHealth_ServerCheck.Forms
{
    partial class mainForm
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(mainForm));
            btn_TempCleaner = new Button();
            btn_smartProtocol = new Button();
            btn_winUpdate = new Button();
            btn_startEveryone = new Button();
            btn_GenerateAudit = new Button();
            label1 = new Label();
            panel1 = new Panel();
            chkBox_EnableSystemUpdate = new CheckBox();
            btn_startDriverComp = new Button();
            txt_healtInformation = new RichTextBox();
            btn_HealtHistory = new Button();
            groupBox1 = new GroupBox();
            tableLayoutPanel2 = new TableLayoutPanel();
            lab_totalSize = new Label();
            label2 = new Label();
            label4 = new Label();
            lab_deleteDirs = new Label();
            lab_deleteFiles = new Label();
            label3 = new Label();
            groupBox2 = new GroupBox();
            btn_viewAllDisk = new Button();
            tableLayoutPanel1 = new TableLayoutPanel();
            label7 = new Label();
            label5 = new Label();
            label6 = new Label();
            label8 = new Label();
            lab_diskHealth = new Label();
            lab_temperature = new Label();
            lab_hours = new Label();
            lab_failed = new Label();
            comB_diskName = new ComboBox();
            groupBox3 = new GroupBox();
            btn_openWinUpdPanel = new Button();
            tableLayoutPanel3 = new TableLayoutPanel();
            label9 = new Label();
            label10 = new Label();
            label11 = new Label();
            lab_foundUpd = new Label();
            lab_succesUpd = new Label();
            lab_failedUpd = new Label();
            label12 = new Label();
            label13 = new Label();
            tableLayoutPanel4 = new TableLayoutPanel();
            label14 = new Label();
            pictureBox1 = new PictureBox();
            groupBox4 = new GroupBox();
            tableLayoutPanel5 = new TableLayoutPanel();
            label16 = new Label();
            label15 = new Label();
            lab_driverNotUpdated = new Label();
            lab_TotalDriversScan = new Label();
            btn_openDeviceManager = new Button();
            btn_temp_diagnostic = new Button();
            chkbox_dfserver = new CheckBox();
            pictureBox2 = new PictureBox();
            label17 = new Label();
            comB_techicianName = new ComboBox();
            panel1.SuspendLayout();
            groupBox1.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            groupBox2.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            groupBox3.SuspendLayout();
            tableLayoutPanel3.SuspendLayout();
            tableLayoutPanel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            groupBox4.SuspendLayout();
            tableLayoutPanel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            SuspendLayout();
            // 
            // btn_TempCleaner
            // 
            btn_TempCleaner.BackColor = Color.DodgerBlue;
            btn_TempCleaner.Cursor = Cursors.Hand;
            btn_TempCleaner.FlatStyle = FlatStyle.Flat;
            btn_TempCleaner.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btn_TempCleaner.ForeColor = Color.Transparent;
            btn_TempCleaner.Location = new Point(45, 40);
            btn_TempCleaner.Margin = new Padding(4, 3, 4, 3);
            btn_TempCleaner.Name = "btn_TempCleaner";
            btn_TempCleaner.Size = new Size(231, 35);
            btn_TempCleaner.TabIndex = 0;
            btn_TempCleaner.Text = "Eliminar Archivos Temporales";
            btn_TempCleaner.UseVisualStyleBackColor = false;
            btn_TempCleaner.Click += btn_TempCleaner_Click;
            // 
            // btn_smartProtocol
            // 
            btn_smartProtocol.BackColor = Color.DodgerBlue;
            btn_smartProtocol.Cursor = Cursors.Hand;
            btn_smartProtocol.FlatStyle = FlatStyle.Flat;
            btn_smartProtocol.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btn_smartProtocol.ForeColor = Color.Transparent;
            btn_smartProtocol.Location = new Point(45, 91);
            btn_smartProtocol.Margin = new Padding(4, 3, 4, 3);
            btn_smartProtocol.Name = "btn_smartProtocol";
            btn_smartProtocol.Size = new Size(231, 35);
            btn_smartProtocol.TabIndex = 1;
            btn_smartProtocol.Text = "Iniciar Protocolo S.M.A.R.T";
            btn_smartProtocol.UseVisualStyleBackColor = false;
            btn_smartProtocol.Click += btn_smartProtocol_Click;
            // 
            // btn_winUpdate
            // 
            btn_winUpdate.BackColor = Color.DodgerBlue;
            btn_winUpdate.Cursor = Cursors.Hand;
            btn_winUpdate.FlatStyle = FlatStyle.Flat;
            btn_winUpdate.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btn_winUpdate.ForeColor = Color.Transparent;
            btn_winUpdate.Location = new Point(45, 143);
            btn_winUpdate.Margin = new Padding(4, 3, 4, 3);
            btn_winUpdate.Name = "btn_winUpdate";
            btn_winUpdate.Size = new Size(231, 35);
            btn_winUpdate.TabIndex = 2;
            btn_winUpdate.Text = "Iniciar Scan de Actualizaciones";
            btn_winUpdate.UseVisualStyleBackColor = false;
            btn_winUpdate.Click += btn_winUpdate_Click;
            // 
            // btn_startEveryone
            // 
            btn_startEveryone.BackColor = Color.FromArgb(202, 11, 11);
            btn_startEveryone.BackgroundImageLayout = ImageLayout.None;
            btn_startEveryone.Cursor = Cursors.Hand;
            btn_startEveryone.FlatAppearance.BorderColor = Color.FromArgb(244, 15, 2);
            btn_startEveryone.FlatAppearance.BorderSize = 0;
            btn_startEveryone.FlatStyle = FlatStyle.Flat;
            btn_startEveryone.Font = new Font("Maiandra GD", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btn_startEveryone.ForeColor = Color.White;
            btn_startEveryone.Location = new Point(12, 483);
            btn_startEveryone.Margin = new Padding(4, 3, 4, 3);
            btn_startEveryone.Name = "btn_startEveryone";
            btn_startEveryone.Size = new Size(331, 59);
            btn_startEveryone.TabIndex = 3;
            btn_startEveryone.Text = "Realizar Todos los procesos y Generar el Informe";
            btn_startEveryone.UseVisualStyleBackColor = false;
            btn_startEveryone.Click += btn_startEveryone_Click;
            // 
            // btn_GenerateAudit
            // 
            btn_GenerateAudit.BackColor = Color.MidnightBlue;
            btn_GenerateAudit.Cursor = Cursors.Hand;
            btn_GenerateAudit.FlatStyle = FlatStyle.Flat;
            btn_GenerateAudit.Font = new Font("Maiandra GD", 9F, FontStyle.Bold);
            btn_GenerateAudit.ForeColor = SystemColors.Control;
            btn_GenerateAudit.Location = new Point(4, 262);
            btn_GenerateAudit.Margin = new Padding(4, 3, 4, 3);
            btn_GenerateAudit.Name = "btn_GenerateAudit";
            btn_GenerateAudit.Size = new Size(318, 53);
            btn_GenerateAudit.TabIndex = 4;
            btn_GenerateAudit.Text = "Generar Informe";
            btn_GenerateAudit.UseVisualStyleBackColor = false;
            btn_GenerateAudit.Click += btn_GenerateAudit_ClickAsync;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Noto Serif Georgian", 20.25F, FontStyle.Bold | FontStyle.Underline);
            label1.ForeColor = Color.MidnightBlue;
            label1.Location = new Point(29, 0);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(262, 37);
            label1.TabIndex = 5;
            label1.Text = "Protocolo Manual";
            // 
            // panel1
            // 
            panel1.BackColor = Color.Honeydew;
            panel1.BorderStyle = BorderStyle.Fixed3D;
            panel1.Controls.Add(chkBox_EnableSystemUpdate);
            panel1.Controls.Add(btn_startDriverComp);
            panel1.Controls.Add(btn_winUpdate);
            panel1.Controls.Add(label1);
            panel1.Controls.Add(btn_TempCleaner);
            panel1.Controls.Add(btn_GenerateAudit);
            panel1.Controls.Add(btn_smartProtocol);
            panel1.Location = new Point(12, 151);
            panel1.Margin = new Padding(4, 3, 4, 3);
            panel1.Name = "panel1";
            panel1.Size = new Size(331, 326);
            panel1.TabIndex = 6;
            // 
            // chkBox_EnableSystemUpdate
            // 
            chkBox_EnableSystemUpdate.AutoSize = true;
            chkBox_EnableSystemUpdate.Cursor = Cursors.Hand;
            chkBox_EnableSystemUpdate.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            chkBox_EnableSystemUpdate.Location = new Point(45, 177);
            chkBox_EnableSystemUpdate.Name = "chkBox_EnableSystemUpdate";
            chkBox_EnableSystemUpdate.Size = new Size(223, 19);
            chkBox_EnableSystemUpdate.TabIndex = 7;
            chkBox_EnableSystemUpdate.Text = "Actualizar Sistema Automaticamente";
            chkBox_EnableSystemUpdate.UseVisualStyleBackColor = true;
            chkBox_EnableSystemUpdate.CheckedChanged += chkBox_EnableSystemUpdate_CheckedChanged;
            // 
            // btn_startDriverComp
            // 
            btn_startDriverComp.BackColor = Color.DodgerBlue;
            btn_startDriverComp.Cursor = Cursors.Hand;
            btn_startDriverComp.FlatStyle = FlatStyle.Flat;
            btn_startDriverComp.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btn_startDriverComp.ForeColor = Color.Transparent;
            btn_startDriverComp.Location = new Point(45, 210);
            btn_startDriverComp.Margin = new Padding(4, 3, 4, 3);
            btn_startDriverComp.Name = "btn_startDriverComp";
            btn_startDriverComp.Size = new Size(231, 35);
            btn_startDriverComp.TabIndex = 6;
            btn_startDriverComp.Text = "Iniciar comprobador de drivers";
            btn_startDriverComp.UseVisualStyleBackColor = false;
            btn_startDriverComp.Click += btn_startDriverComp_Click;
            // 
            // txt_healtInformation
            // 
            txt_healtInformation.Location = new Point(91, 604);
            txt_healtInformation.Margin = new Padding(4, 3, 4, 3);
            txt_healtInformation.Name = "txt_healtInformation";
            txt_healtInformation.Size = new Size(252, 32);
            txt_healtInformation.TabIndex = 7;
            txt_healtInformation.Text = "";
            // 
            // btn_HealtHistory
            // 
            btn_HealtHistory.Anchor = AnchorStyles.None;
            btn_HealtHistory.BackColor = SystemColors.Control;
            btn_HealtHistory.Cursor = Cursors.Hand;
            btn_HealtHistory.FlatStyle = FlatStyle.System;
            btn_HealtHistory.Location = new Point(250, 8);
            btn_HealtHistory.Margin = new Padding(4, 3, 4, 3);
            btn_HealtHistory.Name = "btn_HealtHistory";
            btn_HealtHistory.Size = new Size(81, 24);
            btn_HealtHistory.TabIndex = 8;
            btn_HealtHistory.Text = "Ver Historico";
            btn_HealtHistory.UseVisualStyleBackColor = false;
            btn_HealtHistory.Click += btn_HealtHistory_Click;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(tableLayoutPanel2);
            groupBox1.Location = new Point(380, 130);
            groupBox1.Margin = new Padding(4, 3, 4, 3);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new Padding(4, 3, 4, 3);
            groupBox1.Size = new Size(255, 160);
            groupBox1.TabIndex = 9;
            groupBox1.TabStop = false;
            groupBox1.Text = "Espacio Liberado";
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 2;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 62F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 38F));
            tableLayoutPanel2.Controls.Add(lab_totalSize, 1, 2);
            tableLayoutPanel2.Controls.Add(label2, 0, 0);
            tableLayoutPanel2.Controls.Add(label4, 0, 2);
            tableLayoutPanel2.Controls.Add(lab_deleteDirs, 1, 1);
            tableLayoutPanel2.Controls.Add(lab_deleteFiles, 1, 0);
            tableLayoutPanel2.Controls.Add(label3, 0, 1);
            tableLayoutPanel2.Location = new Point(8, 38);
            tableLayoutPanel2.Margin = new Padding(4, 3, 4, 3);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 3;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Absolute, 23F));
            tableLayoutPanel2.Size = new Size(240, 98);
            tableLayoutPanel2.TabIndex = 12;
            // 
            // lab_totalSize
            // 
            lab_totalSize.AutoSize = true;
            lab_totalSize.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lab_totalSize.Location = new Point(152, 74);
            lab_totalSize.Margin = new Padding(4, 0, 4, 0);
            lab_totalSize.Name = "lab_totalSize";
            lab_totalSize.Size = new Size(26, 13);
            lab_totalSize.TabIndex = 5;
            lab_totalSize.Text = "0 B";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(4, 0);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(117, 15);
            label2.TabIndex = 0;
            label2.Text = "Archivos Eliminados:";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(4, 74);
            label4.Margin = new Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new Size(125, 15);
            label4.TabIndex = 2;
            label4.Text = "Espacio Total liberado:";
            // 
            // lab_deleteDirs
            // 
            lab_deleteDirs.AutoSize = true;
            lab_deleteDirs.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lab_deleteDirs.Location = new Point(152, 37);
            lab_deleteDirs.Margin = new Padding(4, 0, 4, 0);
            lab_deleteDirs.Name = "lab_deleteDirs";
            lab_deleteDirs.Size = new Size(14, 13);
            lab_deleteDirs.TabIndex = 4;
            lab_deleteDirs.Text = "0";
            // 
            // lab_deleteFiles
            // 
            lab_deleteFiles.AutoSize = true;
            lab_deleteFiles.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lab_deleteFiles.Location = new Point(152, 0);
            lab_deleteFiles.Margin = new Padding(4, 0, 4, 0);
            lab_deleteFiles.Name = "lab_deleteFiles";
            lab_deleteFiles.Size = new Size(14, 13);
            lab_deleteFiles.TabIndex = 3;
            lab_deleteFiles.Text = "0";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(4, 37);
            label3.Margin = new Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new Size(116, 15);
            label3.TabIndex = 1;
            label3.Text = "Carpetas Eliminadas:";
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(btn_viewAllDisk);
            groupBox2.Controls.Add(tableLayoutPanel1);
            groupBox2.Controls.Add(comB_diskName);
            groupBox2.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            groupBox2.Location = new Point(380, 296);
            groupBox2.Margin = new Padding(4, 3, 4, 3);
            groupBox2.Name = "groupBox2";
            groupBox2.Padding = new Padding(4, 3, 4, 3);
            groupBox2.Size = new Size(528, 181);
            groupBox2.TabIndex = 10;
            groupBox2.TabStop = false;
            groupBox2.Text = "S.M.A.R.T";
            // 
            // btn_viewAllDisk
            // 
            btn_viewAllDisk.Cursor = Cursors.Hand;
            btn_viewAllDisk.Enabled = false;
            btn_viewAllDisk.Location = new Point(368, 73);
            btn_viewAllDisk.Margin = new Padding(4, 3, 4, 3);
            btn_viewAllDisk.Name = "btn_viewAllDisk";
            btn_viewAllDisk.Size = new Size(142, 85);
            btn_viewAllDisk.TabIndex = 3;
            btn_viewAllDisk.Text = "Ver todos los Discos";
            btn_viewAllDisk.UseVisualStyleBackColor = true;
            btn_viewAllDisk.Click += btn_viewAllDisk_Click;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65.44118F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34.55882F));
            tableLayoutPanel1.Controls.Add(label7, 0, 2);
            tableLayoutPanel1.Controls.Add(label5, 0, 0);
            tableLayoutPanel1.Controls.Add(label6, 0, 1);
            tableLayoutPanel1.Controls.Add(label8, 0, 3);
            tableLayoutPanel1.Controls.Add(lab_diskHealth, 1, 0);
            tableLayoutPanel1.Controls.Add(lab_temperature, 1, 1);
            tableLayoutPanel1.Controls.Add(lab_hours, 1, 2);
            tableLayoutPanel1.Controls.Add(lab_failed, 1, 3);
            tableLayoutPanel1.Location = new Point(13, 59);
            tableLayoutPanel1.Margin = new Padding(4, 3, 4, 3);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 4;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 23F));
            tableLayoutPanel1.Size = new Size(317, 115);
            tableLayoutPanel1.TabIndex = 2;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(4, 60);
            label7.Margin = new Padding(4, 0, 4, 0);
            label7.Name = "label7";
            label7.Size = new Size(91, 13);
            label7.TabIndex = 12;
            label7.Text = "Horas encendido:";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(4, 0);
            label5.Margin = new Padding(4, 0, 4, 0);
            label5.Name = "label5";
            label5.Size = new Size(80, 13);
            label5.TabIndex = 0;
            label5.Text = "Salud de disco:";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(4, 30);
            label6.Margin = new Padding(4, 0, 4, 0);
            label6.Name = "label6";
            label6.Size = new Size(70, 13);
            label6.TabIndex = 1;
            label6.Text = "Temperatura:";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(4, 92);
            label8.Margin = new Padding(4, 0, 4, 0);
            label8.Name = "label8";
            label8.Size = new Size(123, 13);
            label8.TabIndex = 13;
            label8.Text = "¿Hay un fallo inminente?";
            // 
            // lab_diskHealth
            // 
            lab_diskHealth.AutoSize = true;
            lab_diskHealth.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lab_diskHealth.ForeColor = SystemColors.InactiveCaption;
            lab_diskHealth.Location = new Point(211, 0);
            lab_diskHealth.Margin = new Padding(4, 0, 4, 0);
            lab_diskHealth.Name = "lab_diskHealth";
            lab_diskHealth.Size = new Size(23, 13);
            lab_diskHealth.TabIndex = 14;
            lab_diskHealth.Text = "0%";
            // 
            // lab_temperature
            // 
            lab_temperature.AutoSize = true;
            lab_temperature.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lab_temperature.ForeColor = SystemColors.InactiveCaption;
            lab_temperature.Location = new Point(211, 30);
            lab_temperature.Margin = new Padding(4, 0, 4, 0);
            lab_temperature.Name = "lab_temperature";
            lab_temperature.Size = new Size(19, 13);
            lab_temperature.TabIndex = 15;
            lab_temperature.Text = "0º";
            // 
            // lab_hours
            // 
            lab_hours.AutoSize = true;
            lab_hours.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lab_hours.ForeColor = SystemColors.InactiveCaption;
            lab_hours.Location = new Point(211, 60);
            lab_hours.Margin = new Padding(4, 0, 4, 0);
            lab_hours.Name = "lab_hours";
            lab_hours.Size = new Size(25, 13);
            lab_hours.TabIndex = 16;
            lab_hours.Text = "0 h";
            // 
            // lab_failed
            // 
            lab_failed.AutoSize = true;
            lab_failed.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lab_failed.ForeColor = SystemColors.InactiveCaption;
            lab_failed.Location = new Point(211, 92);
            lab_failed.Margin = new Padding(4, 0, 4, 0);
            lab_failed.Name = "lab_failed";
            lab_failed.Size = new Size(30, 13);
            lab_failed.TabIndex = 17;
            lab_failed.Text = "N/A";
            // 
            // comB_diskName
            // 
            comB_diskName.Cursor = Cursors.Hand;
            comB_diskName.DropDownStyle = ComboBoxStyle.DropDownList;
            comB_diskName.FormattingEnabled = true;
            comB_diskName.Location = new Point(10, 24);
            comB_diskName.Margin = new Padding(4, 3, 4, 3);
            comB_diskName.Name = "comB_diskName";
            comB_diskName.Size = new Size(510, 21);
            comB_diskName.TabIndex = 0;
            comB_diskName.SelectedIndexChanged += comB_diskName_SelectedIndexChanged;
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(btn_openWinUpdPanel);
            groupBox3.Controls.Add(tableLayoutPanel3);
            groupBox3.Location = new Point(380, 483);
            groupBox3.Margin = new Padding(4, 3, 4, 3);
            groupBox3.Name = "groupBox3";
            groupBox3.Padding = new Padding(4, 3, 4, 3);
            groupBox3.Size = new Size(526, 153);
            groupBox3.TabIndex = 11;
            groupBox3.TabStop = false;
            groupBox3.Text = "Actualizaciones";
            // 
            // btn_openWinUpdPanel
            // 
            btn_openWinUpdPanel.Cursor = Cursors.Hand;
            btn_openWinUpdPanel.Location = new Point(185, 104);
            btn_openWinUpdPanel.Margin = new Padding(4, 3, 4, 3);
            btn_openWinUpdPanel.Name = "btn_openWinUpdPanel";
            btn_openWinUpdPanel.Size = new Size(168, 27);
            btn_openWinUpdPanel.TabIndex = 1;
            btn_openWinUpdPanel.Text = "Abrir Windows Update";
            btn_openWinUpdPanel.UseVisualStyleBackColor = true;
            btn_openWinUpdPanel.Click += btn_openWinUpdPanel_Click;
            // 
            // tableLayoutPanel3
            // 
            tableLayoutPanel3.Anchor = AnchorStyles.Top | AnchorStyles.Bottom;
            tableLayoutPanel3.ColumnCount = 3;
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 51.63044F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 48.36956F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 104F));
            tableLayoutPanel3.Controls.Add(label9, 0, 0);
            tableLayoutPanel3.Controls.Add(label10, 1, 0);
            tableLayoutPanel3.Controls.Add(label11, 2, 0);
            tableLayoutPanel3.Controls.Add(lab_foundUpd, 0, 1);
            tableLayoutPanel3.Controls.Add(lab_succesUpd, 1, 1);
            tableLayoutPanel3.Controls.Add(lab_failedUpd, 2, 1);
            tableLayoutPanel3.Location = new Point(95, 39);
            tableLayoutPanel3.Margin = new Padding(4, 3, 4, 3);
            tableLayoutPanel3.Name = "tableLayoutPanel3";
            tableLayoutPanel3.RowCount = 2;
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Absolute, 28F));
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel3.Size = new Size(328, 54);
            tableLayoutPanel3.TabIndex = 0;
            // 
            // label9
            // 
            label9.Anchor = AnchorStyles.None;
            label9.AutoSize = true;
            label9.BackColor = Color.Transparent;
            label9.Location = new Point(11, 6);
            label9.Margin = new Padding(4, 0, 4, 0);
            label9.Name = "label9";
            label9.Size = new Size(92, 15);
            label9.TabIndex = 0;
            label9.Text = "ENCONTRADAS";
            label9.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label10
            // 
            label10.Anchor = AnchorStyles.None;
            label10.AutoSize = true;
            label10.Location = new Point(140, 6);
            label10.Margin = new Padding(4, 0, 4, 0);
            label10.Name = "label10";
            label10.Size = new Size(58, 15);
            label10.TabIndex = 1;
            label10.Text = "EXITOSAS";
            label10.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label11
            // 
            label11.Anchor = AnchorStyles.None;
            label11.AutoSize = true;
            label11.Location = new Point(247, 6);
            label11.Margin = new Padding(4, 0, 4, 0);
            label11.Name = "label11";
            label11.Size = new Size(57, 15);
            label11.TabIndex = 2;
            label11.Text = "FALLIDAS";
            label11.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lab_foundUpd
            // 
            lab_foundUpd.Anchor = AnchorStyles.None;
            lab_foundUpd.AutoSize = true;
            lab_foundUpd.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lab_foundUpd.ForeColor = SystemColors.InactiveCaption;
            lab_foundUpd.Location = new Point(42, 34);
            lab_foundUpd.Margin = new Padding(4, 0, 4, 0);
            lab_foundUpd.Name = "lab_foundUpd";
            lab_foundUpd.Size = new Size(30, 13);
            lab_foundUpd.TabIndex = 3;
            lab_foundUpd.Text = "N/A";
            // 
            // lab_succesUpd
            // 
            lab_succesUpd.Anchor = AnchorStyles.None;
            lab_succesUpd.AutoSize = true;
            lab_succesUpd.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lab_succesUpd.ForeColor = SystemColors.InactiveCaption;
            lab_succesUpd.Location = new Point(154, 34);
            lab_succesUpd.Margin = new Padding(4, 0, 4, 0);
            lab_succesUpd.Name = "lab_succesUpd";
            lab_succesUpd.Size = new Size(30, 13);
            lab_succesUpd.TabIndex = 4;
            lab_succesUpd.Text = "N/A";
            // 
            // lab_failedUpd
            // 
            lab_failedUpd.Anchor = AnchorStyles.None;
            lab_failedUpd.AutoSize = true;
            lab_failedUpd.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lab_failedUpd.ForeColor = SystemColors.InactiveCaption;
            lab_failedUpd.Location = new Point(260, 34);
            lab_failedUpd.Margin = new Padding(4, 0, 4, 0);
            lab_failedUpd.Name = "lab_failedUpd";
            lab_failedUpd.Size = new Size(30, 13);
            lab_failedUpd.TabIndex = 5;
            lab_failedUpd.Text = "N/A";
            // 
            // label12
            // 
            label12.BorderStyle = BorderStyle.Fixed3D;
            label12.Location = new Point(13, 74);
            label12.Margin = new Padding(4, 0, 4, 0);
            label12.Name = "label12";
            label12.Size = new Size(897, 2);
            label12.TabIndex = 12;
            label12.Text = "label12";
            // 
            // label13
            // 
            label13.Anchor = AnchorStyles.None;
            label13.AutoSize = true;
            label13.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label13.ForeColor = Color.MidnightBlue;
            label13.Location = new Point(4, 13);
            label13.Margin = new Padding(4, 0, 4, 0);
            label13.Name = "label13";
            label13.Size = new Size(238, 15);
            label13.TabIndex = 13;
            label13.Text = "¿Necesitas los registros anteriores?";
            // 
            // tableLayoutPanel4
            // 
            tableLayoutPanel4.ColumnCount = 2;
            tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 246F));
            tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel4.Controls.Add(label13, 0, 0);
            tableLayoutPanel4.Controls.Add(btn_HealtHistory, 1, 0);
            tableLayoutPanel4.Location = new Point(12, 556);
            tableLayoutPanel4.Margin = new Padding(4, 3, 4, 3);
            tableLayoutPanel4.Name = "tableLayoutPanel4";
            tableLayoutPanel4.RowCount = 1;
            tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel4.Size = new Size(335, 41);
            tableLayoutPanel4.TabIndex = 14;
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Font = new Font("Noto Serif Georgian", 26.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label14.ForeColor = Color.MidnightBlue;
            label14.Location = new Point(12, 9);
            label14.Margin = new Padding(4, 0, 4, 0);
            label14.Name = "label14";
            label14.Size = new Size(640, 48);
            label14.TabIndex = 15;
            label14.Text = "Health Server Check - Copicanarias";
            // 
            // pictureBox1
            // 
            pictureBox1.Image = Properties.Resources.copican_logo;
            pictureBox1.Location = new Point(693, 2);
            pictureBox1.Margin = new Padding(4, 3, 4, 3);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(72, 69);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 16;
            pictureBox1.TabStop = false;
            // 
            // groupBox4
            // 
            groupBox4.Controls.Add(tableLayoutPanel5);
            groupBox4.Controls.Add(btn_openDeviceManager);
            groupBox4.Location = new Point(645, 130);
            groupBox4.Margin = new Padding(4, 3, 4, 3);
            groupBox4.Name = "groupBox4";
            groupBox4.Padding = new Padding(4, 3, 4, 3);
            groupBox4.Size = new Size(261, 160);
            groupBox4.TabIndex = 17;
            groupBox4.TabStop = false;
            groupBox4.Text = "Drivers";
            // 
            // tableLayoutPanel5
            // 
            tableLayoutPanel5.ColumnCount = 2;
            tableLayoutPanel5.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 62.05357F));
            tableLayoutPanel5.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 37.94643F));
            tableLayoutPanel5.Controls.Add(label16, 0, 1);
            tableLayoutPanel5.Controls.Add(label15, 0, 0);
            tableLayoutPanel5.Controls.Add(lab_driverNotUpdated, 1, 1);
            tableLayoutPanel5.Controls.Add(lab_TotalDriversScan, 1, 0);
            tableLayoutPanel5.Location = new Point(4, 22);
            tableLayoutPanel5.Margin = new Padding(4, 3, 4, 3);
            tableLayoutPanel5.Name = "tableLayoutPanel5";
            tableLayoutPanel5.RowCount = 2;
            tableLayoutPanel5.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel5.RowStyles.Add(new RowStyle(SizeType.Absolute, 37F));
            tableLayoutPanel5.Size = new Size(251, 80);
            tableLayoutPanel5.TabIndex = 3;
            // 
            // label16
            // 
            label16.Anchor = AnchorStyles.None;
            label16.AutoSize = true;
            label16.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label16.Location = new Point(17, 54);
            label16.Margin = new Padding(4, 0, 4, 0);
            label16.Name = "label16";
            label16.Size = new Size(120, 15);
            label16.TabIndex = 2;
            label16.Text = "Drivers sin Actualizar";
            // 
            // label15
            // 
            label15.Anchor = AnchorStyles.None;
            label15.AutoSize = true;
            label15.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label15.Location = new Point(19, 14);
            label15.Margin = new Padding(4, 0, 4, 0);
            label15.Name = "label15";
            label15.Size = new Size(116, 15);
            label15.TabIndex = 0;
            label15.Text = "Drivers Escaneados";
            // 
            // lab_driverNotUpdated
            // 
            lab_driverNotUpdated.Anchor = AnchorStyles.None;
            lab_driverNotUpdated.AutoSize = true;
            lab_driverNotUpdated.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lab_driverNotUpdated.Location = new Point(190, 54);
            lab_driverNotUpdated.Margin = new Padding(4, 0, 4, 0);
            lab_driverNotUpdated.Name = "lab_driverNotUpdated";
            lab_driverNotUpdated.Size = new Size(26, 15);
            lab_driverNotUpdated.TabIndex = 1;
            lab_driverNotUpdated.Text = "N/A";
            // 
            // lab_TotalDriversScan
            // 
            lab_TotalDriversScan.Anchor = AnchorStyles.None;
            lab_TotalDriversScan.AutoSize = true;
            lab_TotalDriversScan.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lab_TotalDriversScan.Location = new Point(190, 14);
            lab_TotalDriversScan.Name = "lab_TotalDriversScan";
            lab_TotalDriversScan.Size = new Size(26, 15);
            lab_TotalDriversScan.TabIndex = 3;
            lab_TotalDriversScan.Text = "N/A";
            // 
            // btn_openDeviceManager
            // 
            btn_openDeviceManager.BackColor = SystemColors.HighlightText;
            btn_openDeviceManager.Cursor = Cursors.Hand;
            btn_openDeviceManager.Location = new Point(16, 108);
            btn_openDeviceManager.Margin = new Padding(4, 3, 4, 3);
            btn_openDeviceManager.Name = "btn_openDeviceManager";
            btn_openDeviceManager.Size = new Size(227, 46);
            btn_openDeviceManager.TabIndex = 2;
            btn_openDeviceManager.Text = "Abrir Administrador de Dispositivo";
            btn_openDeviceManager.UseVisualStyleBackColor = false;
            btn_openDeviceManager.Click += btn_openDeviceManager_Click;
            // 
            // btn_temp_diagnostic
            // 
            btn_temp_diagnostic.Location = new Point(12, 615);
            btn_temp_diagnostic.Name = "btn_temp_diagnostic";
            btn_temp_diagnostic.Size = new Size(52, 21);
            btn_temp_diagnostic.TabIndex = 18;
            btn_temp_diagnostic.Text = "button1";
            btn_temp_diagnostic.UseVisualStyleBackColor = true;
            btn_temp_diagnostic.Click += btn_temp_diagnostic_Click;
            // 
            // chkbox_dfserver
            // 
            chkbox_dfserver.AutoSize = true;
            chkbox_dfserver.Cursor = Cursors.Hand;
            chkbox_dfserver.Font = new Font("Noto Serif Georgian", 11.9999981F, FontStyle.Regular, GraphicsUnit.Point, 0);
            chkbox_dfserver.Location = new Point(91, 98);
            chkbox_dfserver.Name = "chkbox_dfserver";
            chkbox_dfserver.Size = new Size(233, 26);
            chkbox_dfserver.TabIndex = 19;
            chkbox_dfserver.Text = "Soy Tecnico de DF-SERVER";
            chkbox_dfserver.UseVisualStyleBackColor = true;
            chkbox_dfserver.CheckedChanged += chkbox_dfserver_CheckedChanged;
            // 
            // pictureBox2
            // 
            pictureBox2.BackgroundImageLayout = ImageLayout.None;
            pictureBox2.Image = Properties.Resources.dfserver_logo;
            pictureBox2.Location = new Point(19, 79);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(66, 66);
            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox2.TabIndex = 20;
            pictureBox2.TabStop = false;
            // 
            // label17
            // 
            label17.AutoSize = true;
            label17.Font = new Font("Noto Serif Georgian", 11.9999981F);
            label17.Location = new Point(388, 98);
            label17.Name = "label17";
            label17.Size = new Size(73, 22);
            label17.TabIndex = 21;
            label17.Text = "Tecnico:";
            // 
            // comB_techicianName
            // 
            comB_techicianName.FormattingEnabled = true;
            comB_techicianName.Items.AddRange(new object[] { "Francisco Muñoz", "Himar Bautista", "Mencey Medina", "Aaron Ojeda" });
            comB_techicianName.Location = new Point(464, 97);
            comB_techicianName.Name = "comB_techicianName";
            comB_techicianName.Size = new Size(320, 23);
            comB_techicianName.TabIndex = 22;
            // 
            // mainForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.Honeydew;
            BackgroundImageLayout = ImageLayout.None;
            ClientSize = new Size(924, 648);
            Controls.Add(comB_techicianName);
            Controls.Add(label17);
            Controls.Add(pictureBox2);
            Controls.Add(chkbox_dfserver);
            Controls.Add(btn_temp_diagnostic);
            Controls.Add(groupBox4);
            Controls.Add(pictureBox1);
            Controls.Add(label14);
            Controls.Add(tableLayoutPanel4);
            Controls.Add(label12);
            Controls.Add(groupBox3);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Controls.Add(txt_healtInformation);
            Controls.Add(panel1);
            Controls.Add(btn_startEveryone);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(4, 3, 4, 3);
            MaximizeBox = false;
            Name = "mainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Health Check";
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            groupBox1.ResumeLayout(false);
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanel2.PerformLayout();
            groupBox2.ResumeLayout(false);
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            groupBox3.ResumeLayout(false);
            tableLayoutPanel3.ResumeLayout(false);
            tableLayoutPanel3.PerformLayout();
            tableLayoutPanel4.ResumeLayout(false);
            tableLayoutPanel4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            groupBox4.ResumeLayout(false);
            tableLayoutPanel5.ResumeLayout(false);
            tableLayoutPanel5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private Button btn_TempCleaner;
        private Button btn_smartProtocol;
        private Button btn_winUpdate;
        private Button btn_startEveryone;
        private Button btn_GenerateAudit;
        private Label label1;
        private Panel panel1;
        private RichTextBox txt_healtInformation;
        private Button btn_HealtHistory;
        private GroupBox groupBox1;
        private Label lab_totalSize;
        private Label lab_deleteDirs;
        private Label lab_deleteFiles;
        private Label label4;
        private Label label3;
        private Label label2;
        private GroupBox groupBox2;
        private ComboBox comB_diskName;
        private Label label5;
        private GroupBox groupBox3;
        private Label label6;
        private TableLayoutPanel tableLayoutPanel1;
        private Label label7;
        private Label label8;
        private Label lab_diskHealth;
        private Label lab_temperature;
        private Label lab_hours;
        private Label lab_failed;
        private TableLayoutPanel tableLayoutPanel2;
        private TableLayoutPanel tableLayoutPanel3;
        private Label label9;
        private Label label10;
        private Label label11;
        private Label lab_foundUpd;
        private Label lab_succesUpd;
        private Label lab_failedUpd;
        private Button btn_openWinUpdPanel;
        private Label label12;
        private Button btn_startDriverComp;
        private Label label13;
        private TableLayoutPanel tableLayoutPanel4;
        private Label label14;
        private PictureBox pictureBox1;
        private GroupBox groupBox4;
        private Button btn_viewAllDisk;
        private Button btn_openDeviceManager;
        private Label lab_driverNotUpdated;
        private Label label15;
        private TableLayoutPanel tableLayoutPanel5;
        private Label label16;
        private Label lab_TotalDriversScan;
        private Button btn_temp_diagnostic;
        private CheckBox chkbox_dfserver;
        private PictureBox pictureBox2;
        private Label label17;
        private ComboBox comB_techicianName;
        private CheckBox chkBox_EnableSystemUpdate;
    }
}