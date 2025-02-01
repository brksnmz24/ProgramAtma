namespace ProgramAtmaAparatı
{
    partial class YeniUrunEkleForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(YeniUrunEkleForm));
            lblurunadi = new Label();
            lblmcode = new Label();
            lblxılerleme = new Label();
            lblyılerleme = new Label();
            lblsatır = new Label();
            lblsütun = new Label();
            pictureBox1 = new PictureBox();
            panel1 = new Panel();
            cmb3v5 = new ComboBox();
            btnKaydet = new Button();
            cmbCyclone = new ComboBox();
            chkInfıneonSecim = new CheckBox();
            chkCyloneSecim = new CheckBox();
            txtSütun = new TextBox();
            txtSatir = new TextBox();
            txtYilerleme = new TextBox();
            txtXilerleme = new TextBox();
            txtMcode = new TextBox();
            txtUrunAdi = new TextBox();
            button1 = new Button();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // lblurunadi
            // 
            lblurunadi.AutoSize = true;
            lblurunadi.Font = new Font("Consolas", 12F, FontStyle.Bold, GraphicsUnit.Point);
            lblurunadi.Location = new Point(22, 117);
            lblurunadi.Name = "lblurunadi";
            lblurunadi.Size = new Size(270, 19);
            lblurunadi.TabIndex = 0;
            lblurunadi.Text = "Ürün Y kodu ve Adını Giriniz:";
            // 
            // lblmcode
            // 
            lblmcode.AutoSize = true;
            lblmcode.Font = new Font("Consolas", 12F, FontStyle.Bold, GraphicsUnit.Point);
            lblmcode.Location = new Point(22, 177);
            lblmcode.Name = "lblmcode";
            lblmcode.Size = new Size(207, 19);
            lblmcode.TabIndex = 1;
            lblmcode.Text = "Ürün M Kodunu Giriniz:";
            // 
            // lblxılerleme
            // 
            lblxılerleme.AutoSize = true;
            lblxılerleme.Font = new Font("Consolas", 12F, FontStyle.Bold, GraphicsUnit.Point);
            lblxılerleme.Location = new Point(22, 240);
            lblxılerleme.Name = "lblxılerleme";
            lblxılerleme.Size = new Size(387, 19);
            lblxılerleme.TabIndex = 2;
            lblxılerleme.Text = "X Ekseni İki Nokta Arası Mesafeyi giriniz:";
            // 
            // lblyılerleme
            // 
            lblyılerleme.AutoSize = true;
            lblyılerleme.Font = new Font("Consolas", 12F, FontStyle.Bold, GraphicsUnit.Point);
            lblyılerleme.Location = new Point(22, 305);
            lblyılerleme.Name = "lblyılerleme";
            lblyılerleme.Size = new Size(387, 19);
            lblyılerleme.TabIndex = 3;
            lblyılerleme.Text = "Y Ekseni İki Nokta Arası Mesafeyi Giriniz:";
            // 
            // lblsatır
            // 
            lblsatır.AutoSize = true;
            lblsatır.Font = new Font("Consolas", 12F, FontStyle.Bold, GraphicsUnit.Point);
            lblsatır.Location = new Point(22, 375);
            lblsatır.Name = "lblsatır";
            lblsatır.Size = new Size(216, 19);
            lblsatır.TabIndex = 4;
            lblsatır.Text = "Satır Sayısını Giriniz:";
            // 
            // lblsütun
            // 
            lblsütun.AutoSize = true;
            lblsütun.Font = new Font("Consolas", 12F, FontStyle.Bold, GraphicsUnit.Point);
            lblsütun.Location = new Point(22, 439);
            lblsütun.Name = "lblsütun";
            lblsütun.Size = new Size(216, 19);
            lblsütun.TabIndex = 5;
            lblsütun.Text = "Sütun Sayısını Giriniz:";
            // 
            // pictureBox1
            // 
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(-2, -2);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(689, 62);
            pictureBox1.TabIndex = 6;
            pictureBox1.TabStop = false;
            // 
            // panel1
            // 
            panel1.BackColor = Color.White;
            panel1.Controls.Add(cmb3v5);
            panel1.Controls.Add(btnKaydet);
            panel1.Controls.Add(cmbCyclone);
            panel1.Controls.Add(chkInfıneonSecim);
            panel1.Controls.Add(chkCyloneSecim);
            panel1.Controls.Add(txtSütun);
            panel1.Controls.Add(txtSatir);
            panel1.Controls.Add(txtYilerleme);
            panel1.Controls.Add(txtXilerleme);
            panel1.Controls.Add(txtMcode);
            panel1.Controls.Add(txtUrunAdi);
            panel1.Location = new Point(12, 81);
            panel1.Name = "panel1";
            panel1.Size = new Size(662, 577);
            panel1.TabIndex = 7;
            panel1.Paint += panel1_Paint;
            // 
            // cmb3v5
            // 
            cmb3v5.Font = new Font("Consolas", 12F, FontStyle.Bold, GraphicsUnit.Point);
            cmb3v5.FormattingEnabled = true;
            cmb3v5.Location = new Point(413, 443);
            cmb3v5.Name = "cmb3v5";
            cmb3v5.Size = new Size(229, 27);
            cmb3v5.TabIndex = 10;
            // 
            // btnKaydet
            // 
            btnKaydet.BackColor = Color.FromArgb(252, 209, 35);
            btnKaydet.Font = new Font("Consolas", 12F, FontStyle.Bold, GraphicsUnit.Point);
            btnKaydet.Location = new Point(218, 524);
            btnKaydet.Name = "btnKaydet";
            btnKaydet.Size = new Size(197, 37);
            btnKaydet.TabIndex = 9;
            btnKaydet.Text = "Kaydet";
            btnKaydet.UseVisualStyleBackColor = false;
            btnKaydet.Click += btnKaydet_Click;
            // 
            // cmbCyclone
            // 
            cmbCyclone.Font = new Font("Consolas", 12F, FontStyle.Bold, GraphicsUnit.Point);
            cmbCyclone.FormattingEnabled = true;
            cmbCyclone.Location = new Point(413, 443);
            cmbCyclone.Name = "cmbCyclone";
            cmbCyclone.Size = new Size(229, 27);
            cmbCyclone.TabIndex = 8;
            cmbCyclone.SelectedIndexChanged += cmbCyclone_SelectedIndexChanged;
            // 
            // chkInfıneonSecim
            // 
            chkInfıneonSecim.AutoSize = true;
            chkInfıneonSecim.Font = new Font("Consolas", 12F, FontStyle.Bold, GraphicsUnit.Point);
            chkInfıneonSecim.Location = new Point(135, 443);
            chkInfıneonSecim.Name = "chkInfıneonSecim";
            chkInfıneonSecim.Size = new Size(100, 23);
            chkInfıneonSecim.TabIndex = 7;
            chkInfıneonSecim.Text = "Infineon";
            chkInfıneonSecim.UseVisualStyleBackColor = true;
            chkInfıneonSecim.CheckedChanged += chkInfıneonSecim_CheckedChanged;
            // 
            // chkCyloneSecim
            // 
            chkCyloneSecim.AutoSize = true;
            chkCyloneSecim.Font = new Font("Consolas", 12F, FontStyle.Bold, GraphicsUnit.Point);
            chkCyloneSecim.Location = new Point(20, 443);
            chkCyloneSecim.Name = "chkCyloneSecim";
            chkCyloneSecim.Size = new Size(91, 23);
            chkCyloneSecim.TabIndex = 6;
            chkCyloneSecim.Text = "Cyclone";
            chkCyloneSecim.UseVisualStyleBackColor = true;
            chkCyloneSecim.CheckedChanged += chkCyloneSecim_CheckedChanged;
            // 
            // txtSütun
            // 
            txtSütun.Font = new Font("Consolas", 11.25F, FontStyle.Bold, GraphicsUnit.Point);
            txtSütun.Location = new Point(413, 358);
            txtSütun.Name = "txtSütun";
            txtSütun.Size = new Size(229, 25);
            txtSütun.TabIndex = 5;
            txtSütun.TextChanged += txtSütun_TextChanged;
            // 
            // txtSatir
            // 
            txtSatir.Font = new Font("Consolas", 11.25F, FontStyle.Bold, GraphicsUnit.Point);
            txtSatir.Location = new Point(413, 294);
            txtSatir.Name = "txtSatir";
            txtSatir.Size = new Size(229, 25);
            txtSatir.TabIndex = 4;
            txtSatir.TextChanged += txtSatir_TextChanged;
            // 
            // txtYilerleme
            // 
            txtYilerleme.Font = new Font("Consolas", 11.25F, FontStyle.Bold, GraphicsUnit.Point);
            txtYilerleme.Location = new Point(413, 224);
            txtYilerleme.Name = "txtYilerleme";
            txtYilerleme.Size = new Size(229, 25);
            txtYilerleme.TabIndex = 3;
            txtYilerleme.TextChanged += txtYilerleme_TextChanged;
            // 
            // txtXilerleme
            // 
            txtXilerleme.Font = new Font("Consolas", 11.25F, FontStyle.Bold, GraphicsUnit.Point);
            txtXilerleme.Location = new Point(413, 159);
            txtXilerleme.Name = "txtXilerleme";
            txtXilerleme.Size = new Size(229, 25);
            txtXilerleme.TabIndex = 2;
            txtXilerleme.TextChanged += txtXilerleme_TextChanged;
            // 
            // txtMcode
            // 
            txtMcode.Font = new Font("Consolas", 11.25F, FontStyle.Bold, GraphicsUnit.Point);
            txtMcode.Location = new Point(413, 96);
            txtMcode.Name = "txtMcode";
            txtMcode.Size = new Size(229, 25);
            txtMcode.TabIndex = 1;
            txtMcode.TextChanged += txtMcode_TextChanged;
            // 
            // txtUrunAdi
            // 
            txtUrunAdi.Font = new Font("Consolas", 11.25F, FontStyle.Bold, GraphicsUnit.Point);
            txtUrunAdi.Location = new Point(413, 32);
            txtUrunAdi.Name = "txtUrunAdi";
            txtUrunAdi.Size = new Size(229, 25);
            txtUrunAdi.TabIndex = 0;
            txtUrunAdi.TextChanged += txtUrunAdi_TextChanged;
            // 
            // button1
            // 
            button1.BackColor = Color.Red;
            button1.Location = new Point(641, 12);
            button1.Name = "button1";
            button1.Size = new Size(33, 30);
            button1.TabIndex = 8;
            button1.Text = "X";
            button1.UseVisualStyleBackColor = false;
            button1.Click += button1_Click;
            // 
            // YeniUrunEkleForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoValidate = AutoValidate.EnablePreventFocusChange;
            BackColor = Color.FromArgb(252, 209, 35);
            ClientSize = new Size(686, 670);
            Controls.Add(button1);
            Controls.Add(pictureBox1);
            Controls.Add(lblsütun);
            Controls.Add(lblsatır);
            Controls.Add(lblyılerleme);
            Controls.Add(lblxılerleme);
            Controls.Add(lblmcode);
            Controls.Add(lblurunadi);
            Controls.Add(panel1);
            FormBorderStyle = FormBorderStyle.None;
            Name = "YeniUrunEkleForm";
            Text = "YeniUrunEkleForm";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblurunadi;
        private Label lblmcode;
        private Label lblxılerleme;
        private Label lblyılerleme;
        private Label lblsatır;
        private Label lblsütun;
        private PictureBox pictureBox1;
        private Panel panel1;
        private CheckBox chkInfıneonSecim;
        private CheckBox chkCyloneSecim;
        private TextBox txtSütun;
        private TextBox txtSatir;
        private TextBox txtYilerleme;
        private TextBox txtXilerleme;
        private TextBox txtMcode;
        private TextBox txtUrunAdi;
        private Button btnKaydet;
        private ComboBox cmbCyclone;
        private ComboBox cmb3v5;
        private Button button1;
    }
}