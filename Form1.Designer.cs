namespace ProgramAtmaAparatı
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            panel1 = new Panel();
            label1 = new Label();
            btnmin = new Button();
            btnclose = new Button();
            pictureBox1 = new PictureBox();
            panel2 = new Panel();
            panel4 = new Panel();
            chkınfıneon = new CheckBox();
            chkcyclone = new CheckBox();
            button3 = new Button();
            button2 = new Button();
            button1 = new Button();
            btnkalibrasyon = new Button();
            btnduzenle = new Button();
            cmbhatadurumu = new ComboBox();
            btnDown = new Button();
            btnRight = new Button();
            btnUp = new Button();
            btnLeft = new Button();
            chkkontrol = new CheckBox();
            btnhome = new Button();
            btnstop = new Button();
            btnpiston = new Button();
            btnbaslangıckaydet = new Button();
            btnyeniurunekle = new Button();
            cmburunsecim = new ComboBox();
            label2 = new Label();
            lblyeniurunekle = new Label();
            lblurunsecim = new Label();
            chkhatadurumu = new CheckBox();
            panel3 = new Panel();
            btntemizle = new Button();
            rxtLog = new RichTextBox();
            btnStart = new Button();
            label6 = new Label();
            readTimer = new System.Windows.Forms.Timer(components);
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            panel2.SuspendLayout();
            panel3.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(label1);
            panel1.Controls.Add(btnmin);
            panel1.Controls.Add(btnclose);
            panel1.Controls.Add(pictureBox1);
            panel1.Location = new Point(-3, -3);
            panel1.Name = "panel1";
            panel1.Size = new Size(1012, 62);
            panel1.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.FromArgb(252, 209, 35);
            label1.Font = new Font("Consolas", 18F, FontStyle.Bold, GraphicsUnit.Point);
            label1.ForeColor = SystemColors.ButtonHighlight;
            label1.Location = new Point(400, 23);
            label1.Name = "label1";
            label1.Size = new Size(233, 28);
            label1.TabIndex = 3;
            label1.Text = "CodeInjector V2.0";
            // 
            // btnmin
            // 
            btnmin.Font = new Font("Consolas", 11.25F, FontStyle.Bold, GraphicsUnit.Point);
            btnmin.Location = new Point(913, 15);
            btnmin.Name = "btnmin";
            btnmin.Size = new Size(37, 30);
            btnmin.TabIndex = 2;
            btnmin.Text = "-";
            btnmin.UseVisualStyleBackColor = true;
            btnmin.Click += btnmin_Click;
            // 
            // btnclose
            // 
            btnclose.BackColor = Color.Red;
            btnclose.Font = new Font("Consolas", 11.25F, FontStyle.Bold, GraphicsUnit.Point);
            btnclose.Location = new Point(956, 15);
            btnclose.Name = "btnclose";
            btnclose.Size = new Size(40, 30);
            btnclose.TabIndex = 1;
            btnclose.Text = "x";
            btnclose.UseVisualStyleBackColor = false;
            btnclose.Click += btnclose_Click;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(3, 12);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(1000, 50);
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            pictureBox1.Click += pictureBox1_Click;
            // 
            // panel2
            // 
            panel2.BackColor = Color.White;
            panel2.Controls.Add(panel4);
            panel2.Controls.Add(chkınfıneon);
            panel2.Controls.Add(chkcyclone);
            panel2.Controls.Add(button3);
            panel2.Controls.Add(button2);
            panel2.Controls.Add(button1);
            panel2.Controls.Add(btnkalibrasyon);
            panel2.Controls.Add(btnduzenle);
            panel2.Controls.Add(cmbhatadurumu);
            panel2.Controls.Add(btnDown);
            panel2.Controls.Add(btnRight);
            panel2.Controls.Add(btnUp);
            panel2.Controls.Add(btnLeft);
            panel2.Controls.Add(chkkontrol);
            panel2.Controls.Add(btnhome);
            panel2.Controls.Add(btnstop);
            panel2.Controls.Add(btnpiston);
            panel2.Controls.Add(btnbaslangıckaydet);
            panel2.Controls.Add(btnyeniurunekle);
            panel2.Controls.Add(cmburunsecim);
            panel2.Controls.Add(label2);
            panel2.Controls.Add(lblyeniurunekle);
            panel2.Controls.Add(lblurunsecim);
            panel2.Controls.Add(chkhatadurumu);
            panel2.Location = new Point(13, 65);
            panel2.Name = "panel2";
            panel2.Size = new Size(791, 602);
            panel2.TabIndex = 1;
            // 
            // panel4
            // 
            panel4.Location = new Point(59, 352);
            panel4.Name = "panel4";
            panel4.Size = new Size(472, 237);
            panel4.TabIndex = 24;
            panel4.Paint += panel4_Paint;
            // 
            // chkınfıneon
            // 
            chkınfıneon.AutoSize = true;
            chkınfıneon.Location = new Point(89, 414);
            chkınfıneon.Name = "chkınfıneon";
            chkınfıneon.Size = new Size(111, 17);
            chkınfıneon.TabIndex = 18;
            chkınfıneon.Text = "Infineon bağlantısı";
            chkınfıneon.UseVisualStyleBackColor = true;
            chkınfıneon.Visible = false;
            chkınfıneon.CheckedChanged += chkınfıneon_CheckedChanged;
            // 
            // chkcyclone
            // 
            chkcyclone.AutoSize = true;
            chkcyclone.Location = new Point(89, 380);
            chkcyclone.Name = "chkcyclone";
            chkcyclone.Size = new Size(111, 17);
            chkcyclone.TabIndex = 17;
            chkcyclone.Text = "Cyclone bağlantısı";
            chkcyclone.UseVisualStyleBackColor = true;
            chkcyclone.Visible = false;
            chkcyclone.CheckedChanged += chkcyclone_CheckedChanged;
            // 
            // button3
            // 
            button3.Location = new Point(89, 476);
            button3.Name = "button3";
            button3.Size = new Size(94, 23);
            button3.TabIndex = 21;
            button3.Text = "cyclone veri";
            button3.UseVisualStyleBackColor = true;
            button3.Visible = false;
            button3.Click += button3_Click_1;
            // 
            // button2
            // 
            button2.Location = new Point(191, 447);
            button2.Name = "button2";
            button2.Size = new Size(94, 23);
            button2.TabIndex = 20;
            button2.Text = "cyclone program";
            button2.UseVisualStyleBackColor = true;
            button2.Visible = false;
            button2.Click += button2_Click_1;
            // 
            // button1
            // 
            button1.Location = new Point(89, 447);
            button1.Name = "button1";
            button1.Size = new Size(84, 23);
            button1.TabIndex = 19;
            button1.Text = "cyclone ad";
            button1.UseVisualStyleBackColor = true;
            button1.Visible = false;
            button1.Click += button1_Click_2;
            // 
            // btnkalibrasyon
            // 
            btnkalibrasyon.Font = new Font("Consolas", 11.25F, FontStyle.Bold, GraphicsUnit.Point);
            btnkalibrasyon.Location = new Point(590, 556);
            btnkalibrasyon.Name = "btnkalibrasyon";
            btnkalibrasyon.Size = new Size(112, 28);
            btnkalibrasyon.TabIndex = 25;
            btnkalibrasyon.Text = "Kalibrasyon";
            btnkalibrasyon.UseVisualStyleBackColor = true;
            btnkalibrasyon.Click += btnkalibrasyon_Click;
            // 
            // btnduzenle
            // 
            btnduzenle.Font = new Font("Consolas", 11.25F, FontStyle.Bold, GraphicsUnit.Point);
            btnduzenle.Location = new Point(590, 522);
            btnduzenle.Name = "btnduzenle";
            btnduzenle.Size = new Size(112, 28);
            btnduzenle.TabIndex = 22;
            btnduzenle.Text = "Düzenle";
            btnduzenle.UseVisualStyleBackColor = true;
            btnduzenle.Click += btnduzenle_Click;
            // 
            // cmbhatadurumu
            // 
            cmbhatadurumu.Font = new Font("Consolas", 9.75F, FontStyle.Bold, GraphicsUnit.Point);
            cmbhatadurumu.FormattingEnabled = true;
            cmbhatadurumu.Location = new Point(342, 324);
            cmbhatadurumu.Name = "cmbhatadurumu";
            cmbhatadurumu.Size = new Size(189, 23);
            cmbhatadurumu.TabIndex = 16;
            cmbhatadurumu.SelectedIndexChanged += cmbhatadurumu_SelectedIndexChanged;
            // 
            // btnDown
            // 
            btnDown.Font = new Font("Consolas", 11.25F, FontStyle.Bold, GraphicsUnit.Point);
            btnDown.Location = new Point(586, 271);
            btnDown.Name = "btnDown";
            btnDown.Size = new Size(49, 38);
            btnDown.TabIndex = 14;
            btnDown.Text = "-y";
            btnDown.UseVisualStyleBackColor = true;
            btnDown.Click += btnDown_Click;
            // 
            // btnRight
            // 
            btnRight.Font = new Font("Consolas", 11.25F, FontStyle.Bold, GraphicsUnit.Point);
            btnRight.Location = new Point(631, 233);
            btnRight.Name = "btnRight";
            btnRight.Size = new Size(49, 38);
            btnRight.TabIndex = 13;
            btnRight.Text = "+x";
            btnRight.UseVisualStyleBackColor = true;
            btnRight.Click += btnRight_Click;
            // 
            // btnUp
            // 
            btnUp.Font = new Font("Consolas", 11.25F, FontStyle.Bold, GraphicsUnit.Point);
            btnUp.Location = new Point(586, 202);
            btnUp.Name = "btnUp";
            btnUp.Size = new Size(49, 38);
            btnUp.TabIndex = 12;
            btnUp.Text = "+y";
            btnUp.UseVisualStyleBackColor = true;
            btnUp.Click += btnUp_Click_1;
            // 
            // btnLeft
            // 
            btnLeft.Font = new Font("Consolas", 11.25F, FontStyle.Bold, GraphicsUnit.Point);
            btnLeft.Location = new Point(539, 233);
            btnLeft.Name = "btnLeft";
            btnLeft.Size = new Size(51, 38);
            btnLeft.TabIndex = 11;
            btnLeft.Text = "-x";
            btnLeft.UseVisualStyleBackColor = true;
            btnLeft.Click += btnLeft_Click;
            // 
            // chkkontrol
            // 
            chkkontrol.AutoSize = true;
            chkkontrol.Font = new Font("Consolas", 11.25F, FontStyle.Bold, GraphicsUnit.Point);
            chkkontrol.Location = new Point(539, 174);
            chkkontrol.Name = "chkkontrol";
            chkkontrol.Size = new Size(139, 22);
            chkkontrol.TabIndex = 10;
            chkkontrol.Text = "Manuel Kontrol";
            chkkontrol.UseVisualStyleBackColor = true;
            // 
            // btnhome
            // 
            btnhome.Font = new Font("Consolas", 11.25F, FontStyle.Bold, GraphicsUnit.Point);
            btnhome.Location = new Point(141, 233);
            btnhome.Name = "btnhome";
            btnhome.Size = new Size(89, 55);
            btnhome.TabIndex = 8;
            btnhome.Text = "Home";
            btnhome.UseVisualStyleBackColor = true;
            btnhome.Click += btnhome_Click;
            // 
            // btnstop
            // 
            btnstop.Font = new Font("Consolas", 11.25F, FontStyle.Bold, GraphicsUnit.Point);
            btnstop.Location = new Point(191, 169);
            btnstop.Name = "btnstop";
            btnstop.Size = new Size(89, 53);
            btnstop.TabIndex = 7;
            btnstop.Text = "Stop";
            btnstop.UseVisualStyleBackColor = true;
            btnstop.Click += btnstop_Click;
            // 
            // btnpiston
            // 
            btnpiston.Font = new Font("Consolas", 11.25F, FontStyle.Bold, GraphicsUnit.Point);
            btnpiston.Location = new Point(84, 169);
            btnpiston.Name = "btnpiston";
            btnpiston.Size = new Size(89, 53);
            btnpiston.TabIndex = 6;
            btnpiston.Text = "Piston";
            btnpiston.UseVisualStyleBackColor = true;
            btnpiston.Click += btnpiston_Click;
            // 
            // btnbaslangıckaydet
            // 
            btnbaslangıckaydet.Font = new Font("Consolas", 11.25F, FontStyle.Bold, GraphicsUnit.Point);
            btnbaslangıckaydet.Location = new Point(539, 110);
            btnbaslangıckaydet.Name = "btnbaslangıckaydet";
            btnbaslangıckaydet.Size = new Size(163, 47);
            btnbaslangıckaydet.TabIndex = 5;
            btnbaslangıckaydet.Text = "Başlangıç noktası olarak kaydet";
            btnbaslangıckaydet.UseVisualStyleBackColor = true;
            btnbaslangıckaydet.Click += btnbaslangıckaydet_Click;
            // 
            // btnyeniurunekle
            // 
            btnyeniurunekle.Font = new Font("Consolas", 11.25F, FontStyle.Bold, GraphicsUnit.Point);
            btnyeniurunekle.Location = new Point(539, 65);
            btnyeniurunekle.Name = "btnyeniurunekle";
            btnyeniurunekle.Size = new Size(163, 39);
            btnyeniurunekle.TabIndex = 4;
            btnyeniurunekle.Text = "Yeni Ürün Ekle";
            btnyeniurunekle.UseVisualStyleBackColor = true;
            btnyeniurunekle.Click += btnyeniurunekle_Click;
            // 
            // cmburunsecim
            // 
            cmburunsecim.Font = new Font("Consolas", 9.75F, FontStyle.Bold, GraphicsUnit.Point);
            cmburunsecim.FormattingEnabled = true;
            cmburunsecim.Location = new Point(465, 38);
            cmburunsecim.Name = "cmburunsecim";
            cmburunsecim.Size = new Size(237, 23);
            cmburunsecim.TabIndex = 3;
            cmburunsecim.SelectedIndexChanged += cmburunsecim_SelectedIndexChanged;
            cmburunsecim.KeyPress += cmburunsecim_KeyPress_1;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Consolas", 11.25F, FontStyle.Bold, GraphicsUnit.Point);
            label2.Location = new Point(61, 122);
            label2.Name = "label2";
            label2.Size = new Size(472, 18);
            label2.TabIndex = 2;
            label2.Text = "Yeni eklediğiniz ürün için başlangıç noktası belirleyiniz:";
            // 
            // lblyeniurunekle
            // 
            lblyeniurunekle.AutoSize = true;
            lblyeniurunekle.Font = new Font("Consolas", 11.25F, FontStyle.Bold, GraphicsUnit.Point);
            lblyeniurunekle.Location = new Point(61, 77);
            lblyeniurunekle.Name = "lblyeniurunekle";
            lblyeniurunekle.Size = new Size(336, 18);
            lblyeniurunekle.TabIndex = 1;
            lblyeniurunekle.Text = "Listede olmayan ürün için butona basınız:";
            // 
            // lblurunsecim
            // 
            lblurunsecim.AutoSize = true;
            lblurunsecim.Font = new Font("Consolas", 11.25F, FontStyle.Bold, GraphicsUnit.Point);
            lblurunsecim.Location = new Point(61, 38);
            lblurunsecim.Name = "lblurunsecim";
            lblurunsecim.Size = new Size(112, 18);
            lblurunsecim.TabIndex = 0;
            lblurunsecim.Text = "Ürün Seçiniz:";
            // 
            // chkhatadurumu
            // 
            chkhatadurumu.AutoSize = true;
            chkhatadurumu.Font = new Font("Consolas", 11.25F, FontStyle.Bold, GraphicsUnit.Point);
            chkhatadurumu.Location = new Point(61, 324);
            chkhatadurumu.Name = "chkhatadurumu";
            chkhatadurumu.Size = new Size(275, 22);
            chkhatadurumu.TabIndex = 15;
            chkhatadurumu.Text = "Hata Durumu İçin İşaretleyiniz:";
            chkhatadurumu.UseVisualStyleBackColor = true;
            // 
            // panel3
            // 
            panel3.Controls.Add(btntemizle);
            panel3.Controls.Add(rxtLog);
            panel3.Location = new Point(810, 65);
            panel3.Name = "panel3";
            panel3.Size = new Size(199, 602);
            panel3.TabIndex = 2;
            // 
            // btntemizle
            // 
            btntemizle.Font = new Font("Consolas", 9.75F, FontStyle.Bold, GraphicsUnit.Point);
            btntemizle.Location = new Point(3, 3);
            btntemizle.Name = "btntemizle";
            btntemizle.Size = new Size(80, 29);
            btntemizle.TabIndex = 24;
            btntemizle.Text = "Temizle";
            btntemizle.UseVisualStyleBackColor = true;
            btntemizle.Click += btntemizle_Click;
            // 
            // rxtLog
            // 
            rxtLog.BackColor = Color.White;
            rxtLog.Location = new Point(3, 39);
            rxtLog.Name = "rxtLog";
            rxtLog.ReadOnly = true;
            rxtLog.Size = new Size(196, 563);
            rxtLog.TabIndex = 0;
            rxtLog.Text = "";
            rxtLog.TextChanged += rxtLog_TextChanged;
            // 
            // btnStart
            // 
            btnStart.Font = new Font("Consolas", 11.25F, FontStyle.Bold, GraphicsUnit.Point);
            btnStart.Location = new Point(853, 626);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(112, 28);
            btnStart.TabIndex = 25;
            btnStart.Text = "Start";
            btnStart.UseVisualStyleBackColor = true;
            btnStart.Click += btnStart_Click;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.ForeColor = Color.FromArgb(252, 209, 35);
            label6.Location = new Point(13, 670);
            label6.Name = "label6";
            label6.Size = new Size(35, 13);
            label6.TabIndex = 3;
            label6.Text = "label1";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(252, 209, 35);
            ClientSize = new Size(1020, 679);
            ControlBox = false;
            Controls.Add(btnStart);
            Controls.Add(label6);
            Controls.Add(panel3);
            Controls.Add(panel2);
            Controls.Add(panel1);
            Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Form1";
            Text = "Program Atma";
            FormClosing += Form1_FormClosing;
            Load += Form1_Load;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            panel3.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel panel1;
        private PictureBox pictureBox1;
        private Panel panel2;
        private Label lblyeniurunekle;
        private Label lblurunsecim;
        private Panel panel3;
        private RichTextBox rxtLog;
        private Label label2;
        private Button btnbaslangıckaydet;
        private Button btnyeniurunekle;
        private ComboBox cmburunsecim;
        private Button btnhome;
        private Button btnstop;
        private Button btnpiston;
        private Button btnDown;
        private Button btnRight;
        private Button btnUp;
        private Button btnLeft;
        private CheckBox chkkontrol;
        private Button button3;
        private Button button2;
        private Button button1;
        private ComboBox cmbhatadurumu;
        private CheckBox chkhatadurumu;
        private CheckBox chkınfıneon;
        private CheckBox chkcyclone;
        private Button btnduzenle;
        private Button btntemizle;
        private Label label6;
        private Button btnmin;
        private Button btnclose;
        private Button btnStart;
        private System.Windows.Forms.Timer readTimer;
        public Panel panel4;
        private Button btnkalibrasyon;
        private Label label1;
    }
}
