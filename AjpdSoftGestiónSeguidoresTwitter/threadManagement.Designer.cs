namespace GestiónSeguidoresTwitter
{
    partial class threadManagement
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(threadManagement));
            this.textBoxNroThread = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.initManagement = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBoxNroThread
            // 
            this.textBoxNroThread.Location = new System.Drawing.Point(146, 39);
            this.textBoxNroThread.Name = "textBoxNroThread";
            this.textBoxNroThread.Size = new System.Drawing.Size(287, 20);
            this.textBoxNroThread.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(63, 42);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "URL Access";
            // 
            // initManagement
            // 
            this.initManagement.Location = new System.Drawing.Point(66, 89);
            this.initManagement.Name = "initManagement";
            this.initManagement.Size = new System.Drawing.Size(367, 41);
            this.initManagement.TabIndex = 3;
            this.initManagement.Text = "Iniciar";
            this.initManagement.UseVisualStyleBackColor = true;
            this.initManagement.Click += new System.EventHandler(this.initManagement_Click);
            // 
            // threadManagement
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(498, 161);
            this.Controls.Add(this.initManagement);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxNroThread);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "threadManagement";
            this.Text = "Administrador de Cuentas";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox textBoxNroThread;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button initManagement;
    }
}