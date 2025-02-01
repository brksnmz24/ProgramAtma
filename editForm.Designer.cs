namespace ProgramAtmaAparatı
{
    partial class editForm
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
            comboBox = new ComboBox();
            infoTextBox = new TextBox();
            deleteButton = new Button();
            panel1 = new Panel();
            SuspendLayout();
            // 
            // comboBox
            // 
            comboBox.Font = new Font("Consolas", 11.25F, FontStyle.Bold, GraphicsUnit.Point);
            comboBox.FormattingEnabled = true;
            comboBox.Location = new Point(121, 70);
            comboBox.Name = "comboBox";
            comboBox.Size = new Size(208, 26);
            comboBox.TabIndex = 0;
            // 
            // infoTextBox
            // 
            infoTextBox.Font = new Font("Consolas", 11.25F, FontStyle.Bold, GraphicsUnit.Point);
            infoTextBox.Location = new Point(121, 115);
            infoTextBox.Name = "infoTextBox";
            infoTextBox.Size = new Size(208, 25);
            infoTextBox.TabIndex = 1;
            // 
            // deleteButton
            // 
            deleteButton.Font = new Font("Consolas", 11.25F, FontStyle.Bold, GraphicsUnit.Point);
            deleteButton.Location = new Point(121, 171);
            deleteButton.Name = "deleteButton";
            deleteButton.Size = new Size(120, 26);
            deleteButton.TabIndex = 2;
            deleteButton.Text = "Sil";
            deleteButton.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            panel1.BackColor = Color.White;
            panel1.Location = new Point(35, 36);
            panel1.Name = "panel1";
            panel1.Size = new Size(400, 240);
            panel1.TabIndex = 3;
            // 
            // editForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(252, 209, 35);
            ClientSize = new Size(463, 317);
            Controls.Add(deleteButton);
            Controls.Add(infoTextBox);
            Controls.Add(comboBox);
            Controls.Add(panel1);
            Name = "editForm";
            Text = "editForm";
            Load += editForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel panel1;
    }
}