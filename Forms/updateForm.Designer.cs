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
            this.pb_Process = new System.Windows.Forms.ProgressBar();
            this.txt_Logs = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // pb_Process
            // 
            this.pb_Process.Location = new System.Drawing.Point(12, 112);
            this.pb_Process.Name = "pb_Process";
            this.pb_Process.Size = new System.Drawing.Size(293, 28);
            this.pb_Process.TabIndex = 0;
            // 
            // txt_Logs
            // 
            this.txt_Logs.Location = new System.Drawing.Point(12, 12);
            this.txt_Logs.Name = "txt_Logs";
            this.txt_Logs.Size = new System.Drawing.Size(293, 83);
            this.txt_Logs.TabIndex = 1;
            this.txt_Logs.Text = "";
            // 
            // updateForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(317, 152);
            this.ControlBox = false;
            this.Controls.Add(this.txt_Logs);
            this.Controls.Add(this.pb_Process);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "updateForm";
            this.ShowInTaskbar = false;
            this.Text = "Actualizando...";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ProgressBar pb_Process;
        private System.Windows.Forms.RichTextBox txt_Logs;
    }
}