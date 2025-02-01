namespace ProgramAtmaAparatı
{
    partial class Dikdörtgen
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing && (components != null))
        //    {
        //        components.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        public void InitializeComponent()
        {
            pnlkartekranı = new Panel();
            SuspendLayout();
            // 
            // pnlkartekranı
            // 
            pnlkartekranı.BackColor = Color.White;
            pnlkartekranı.Location = new Point(22, 12);
            pnlkartekranı.Name = "pnlkartekranı";
            pnlkartekranı.Size = new Size(763, 482);
            pnlkartekranı.TabIndex = 0;
            pnlkartekranı.Paint += pnlkartekranı_Paint;
            // 
            // Dikdörtgen
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(252, 209, 35);
            ClientSize = new Size(797, 506);
            Controls.Add(pnlkartekranı);
            Name = "Dikdörtgen";
            Text = "dikdörtgenForm";
            FormClosing += Dikdörtgen_FormClosing_1;
            Load += Dikdörtgen_Load_1;
            ResumeLayout(false);
        }

        #endregion

        public Panel pnlkartekranı;
    }
}