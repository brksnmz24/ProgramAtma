namespace ProgramAtmaAparatı
{
    partial class frmWait
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
            button1 = new Button();
            button2 = new Button();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Consolas", 11.25F, FontStyle.Bold, GraphicsUnit.Point);
            label1.Location = new Point(37, 69);
            label1.Name = "label1";
            label1.Size = new Size(280, 18);
            label1.TabIndex = 0;
            label1.Text = "Program Atılıyor Lütfen Bekleyiniz";
            // 
            // progressBar1
            // 
            progressBar1.Location = new Point(37, 116);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(312, 23);
            progressBar1.TabIndex = 1;
            progressBar1.Click += progressBar1_Click;
            // 
            // button1
            // 
            button1.Font = new Font("Consolas", 11.25F, FontStyle.Bold, GraphicsUnit.Point);
            button1.Location = new Point(37, 183);
            button1.Name = "button1";
            button1.Size = new Size(129, 23);
            button1.TabIndex = 2;
            button1.Text = "Cancel";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.Font = new Font("Consolas", 11.25F, FontStyle.Bold, GraphicsUnit.Point);
            button2.Location = new Point(220, 183);
            button2.Name = "button2";
            button2.Size = new Size(129, 23);
            button2.TabIndex = 3;
            button2.Text = "Testi Durdur";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // frmWait
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(252, 209, 35);
            ClientSize = new Size(390, 256);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(progressBar1);
            Controls.Add(label1);
            Name = "frmWait";
            Text = "frmWait";
            Load += frmWait_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private ProgressBar progressBar1;
        private Button button1;
        private Button button2;
    }
}