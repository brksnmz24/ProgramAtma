using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProgramAtmaAparatı
{
    public partial class Password : Form
    {
        public Password()
        {
            InitializeComponent();
            //// Şifre TextBox'u
            //txtsifre = new TextBox();
            //txtsifre.PasswordChar = '*'; // Şifreyi gizlemek için
            ////txtsifre.Width = 100;
            //Controls.Add(txtsifre);

            //// OK Butonu
            //btnok = new Button();
            //btnok.Text = "Tamam";
            ////Controls.Add(btnok);




            //// Form özellikleri
            //this.Text = "Şifre Girişi";
            //this.StartPosition = FormStartPosition.CenterParent;
            //this.FormBorderStyle = FormBorderStyle.FixedDialog;
            //this.MaximizeBox = false;
            //this.MinimizeBox = false;
            //this.AcceptButton = btnok;
        }

        private void Password_Load(object sender, EventArgs e)
        {

        }



        private void btnclose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnok_Click(object sender, EventArgs e)
        {
            // Şifreyi kontrol et (örnek şifre "1234")
            if (txtsifre.Text == "1234")
            {
                this.DialogResult = DialogResult.OK; // Şifre doğru
                this.Close();
            }
            else
            {
                MessageBox.Show("Yanlış şifre. Lütfen tekrar deneyin.");
                txtsifre.Clear();
                txtsifre.Focus();
            }
        }

        private void txtsifre_TextChanged_1(object sender, EventArgs e)
        {
            txtsifre.PasswordChar = '*'; // Şifreyi gizlemek için
        }
    }
}
