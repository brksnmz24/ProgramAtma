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
    public partial class Wait : Form
    {
        public Wait()
        {
            InitializeComponent();
        }

        private void Wait_Load(object sender, EventArgs e)
        {
            label1.Text = "Hareket devam ediyor lütfen bekleyiniz...";
            progressBar1.Style = ProgressBarStyle.Marquee;
            progressBar1.MarqueeAnimationSpeed = 30;
            label1.Visible = true;
            progressBar1.Visible = true;
            this.StartPosition = FormStartPosition.CenterScreen; // Formu ekranın ortasında başlat        }
        }
    }
}
