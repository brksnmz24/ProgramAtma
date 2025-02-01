namespace ProgramAtmaAparatı
{
    partial class Wait
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
            label1 = new Label();
            progressBar1 = new ProgressBar();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Consolas", 11.25F, FontStyle.Bold, GraphicsUnit.Point);
            label1.Location = new Point(49, 89);
            label1.Name = "label1";
            label1.Size = new Size(320, 18);
            label1.TabIndex = 0;
            label1.Text = "Hareket Devam Ediyor Lütfen Bekleyiniz!";
            // 
            // progressBar1
            // 
            progressBar1.Location = new Point(49, 138);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(320, 23);
            progressBar1.TabIndex = 1;
            // 
            // Wait
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(252, 209, 35);
            ClientSize = new Size(419, 278);
            Controls.Add(progressBar1);
            Controls.Add(label1);
            Name = "Wait";
            Text = "Wait";
            Load += Wait_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private ProgressBar progressBar1;
    }
}