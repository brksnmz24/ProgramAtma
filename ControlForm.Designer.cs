namespace ProgramAtmaAparatı
{
    partial class ControlForm
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
            button1 = new Button();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Consolas", 14.25F, FontStyle.Bold, GraphicsUnit.Point);
            label1.Location = new Point(153, 86);
            label1.Name = "label1";
            label1.Size = new Size(70, 22);
            label1.TabIndex = 0;
            label1.Text = "label1";
            label1.Click += label1_Click;
            // 
            // button1
            // 
            button1.Font = new Font("Consolas", 14.25F, FontStyle.Bold, GraphicsUnit.Point);
            button1.Location = new Point(60, 145);
            button1.Name = "button1";
            button1.Size = new Size(256, 31);
            button1.TabIndex = 1;
            button1.Text = "Eski Noktaya Geri Dön";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click_1;
            // 
            // ControlForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(252, 209, 35);
            ClientSize = new Size(387, 278);
            Controls.Add(button1);
            Controls.Add(label1);
            Name = "ControlForm";
            Text = "ControlForm";
            Load += ControlForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Button button1;
    }
}