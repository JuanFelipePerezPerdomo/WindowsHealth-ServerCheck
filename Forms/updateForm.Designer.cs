namespace WindowsHealth_ServerCheck.Forms
{
    partial class updateForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            pb_Process = new ProgressBar();
            txt_Logs = new RichTextBox();
            SuspendLayout();
            // 
            // pb_Process
            // 
            pb_Process.Location = new Point(14, 129);
            pb_Process.Margin = new Padding(4, 3, 4, 3);
            pb_Process.Name = "pb_Process";
            pb_Process.Size = new Size(342, 32);
            pb_Process.TabIndex = 0;
            // 
            // txt_Logs
            // 
            txt_Logs.Location = new Point(14, 14);
            txt_Logs.Margin = new Padding(4, 3, 4, 3);
            txt_Logs.Name = "txt_Logs";
            txt_Logs.Size = new Size(341, 95);
            txt_Logs.TabIndex = 1;
            txt_Logs.Text = "";
            // 
            // updateForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.MintCream;
            ClientSize = new Size(370, 175);
            ControlBox = false;
            Controls.Add(txt_Logs);
            Controls.Add(pb_Process);
            Cursor = Cursors.AppStarting;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Margin = new Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "updateForm";
            ShowInTaskbar = false;
            Text = "Actualizando...";
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ProgressBar pb_Process;
        private System.Windows.Forms.RichTextBox txt_Logs;
    }
}