using ProgramAtmaAparatı;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.DataFormats;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ProgramAtmaAparatı
{
    public partial class frmWait : Form
    {
        private Form1 _koordinatForm; // Ana form referansı
        private Dikdörtgen _dikdortgenForm;


        public frmWait(Form1 form)
        {
            InitializeComponent();
            _koordinatForm = form; // Ana form referansını al
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
        }


        public frmWait(Dikdörtgen form)
        {
            InitializeComponent();
            _dikdortgenForm = form;
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;

            // Eğer koordinat formunu kullanmak istiyorsanız:
            // _koordinatForm = form;
        }


        private void frmWait_Load(object sender, EventArgs e)
        {
            button2.Enabled = false;
            progressBar1.Style = ProgressBarStyle.Marquee;
            progressBar1.MarqueeAnimationSpeed = 30; // Animasyon hızını ayarlayın (isteğe bağlı)
        }

        private void progressBar1_Click(object sender, EventArgs e)
        {
            // ProgressBar'a tıklama işlemi
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _koordinatForm.TriggerStopButton(); // Ana formdaki durdur butonunu tetikle
            button2.Enabled = true;



        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close(); // Formu kapat
            Process.GetCurrentProcess().Kill();

        }

        private void frmWait_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Form kapatıldığında yapılacak işlemler
        }
    }
}
