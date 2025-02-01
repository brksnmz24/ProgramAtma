namespace ProgramAtmaAparatı
{
    partial class Password
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
            txtsifre = new TextBox();
            lblsifre = new Label();
            btnok = new Button();
            panel1 = new Panel();
            btnclose = new Button();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // txtsifre
            // 
            txtsifre.Location = new Point(45, 63);
            txtsifre.Name = "txtsifre";
            txtsifre.Size = new Size(214, 23);
            txtsifre.TabIndex = 0;
            txtsifre.TextChanged += txtsifre_TextChanged_1;
            // 
            // lblsifre
            // 
            lblsifre.AutoSize = true;
            lblsifre.Font = new Font("Consolas", 11.25F, FontStyle.Bold, GraphicsUnit.Point);
            lblsifre.Location = new Point(45, 19);
            lblsifre.Name = "lblsifre";
            lblsifre.Size = new Size(120, 18);
            lblsifre.TabIndex = 1;
            lblsifre.Text = "Şifre Giriniz:";
            // 
            // btnok
            // 
            btnok.Font = new Font("Consolas", 11.25F, FontStyle.Bold, GraphicsUnit.Point);
            btnok.Location = new Point(45, 118);
            btnok.Name = "btnok";
            btnok.Size = new Size(93, 31);
            btnok.TabIndex = 2;
            btnok.Text = "Tamam";
            btnok.UseVisualStyleBackColor = true;
            btnok.Click += btnok_Click;
            // 
            // panel1
            // 
            panel1.BackColor = Color.White;
            panel1.Controls.Add(lblsifre);
            panel1.Controls.Add(btnok);
            panel1.Controls.Add(txtsifre);
            panel1.Location = new Point(12, 39);
            panel1.Name = "panel1";
            panel1.Size = new Size(346, 187);
            panel1.TabIndex = 3;
            // 
            // btnclose
            // 
            btnclose.BackColor = Color.Red;
            btnclose.Location = new Point(365, 3);
            btnclose.Name = "btnclose";
            btnclose.Size = new Size(35, 30);
            btnclose.TabIndex = 4;
            btnclose.Text = "X";
            btnclose.UseVisualStyleBackColor = false;
            btnclose.Click += btnclose_Click;
            // 
            // Password
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(252, 209, 35);
            ClientSize = new Size(404, 238);
            ControlBox = false;
            Controls.Add(btnclose);
            Controls.Add(panel1);
            FormBorderStyle = FormBorderStyle.None;
            Name = "Password";
            Text = "Password";
            Load += Password_Load;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TextBox txtsifre;
        private Label lblsifre;
        private Button btnok;
        private Panel panel1;
        private Button btnclose;
    }
}