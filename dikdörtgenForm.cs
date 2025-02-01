using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ProgramAtmaAparatı;

namespace ProgramAtmaAparatı
{
    public partial class Dikdörtgen : Form
    {
        private int rows;
        private int columns;
        private int counter = 1; // Sayıları takip etmek için bir sayaç
        private Form1 _koordinatForm; // Ana form referansı

        // İki parametreli yapıcı metod
        public Dikdörtgen(int rows, int columns, Form1 form)
        {
            InitializeComponent();
            this.rows = rows;
            this.columns = columns;
            this.FormClosing += Dikdörtgen_FormClosing;
            _koordinatForm = form; // Ana form referansını al
            // Dikdörtgenleri oluştur ve panel içine ekle
            CreateRectangles();
            frmWait waitForm = new frmWait(this);

        }

        private void CreateRectangles()
        {
            // Sıfıra bölme hatasını önlemek için minimum 1 değerini kontrol edin
            if (columns <= 0 || rows <= 0)
            {
                MessageBox.Show("Geçersiz sütun veya satır değeri.");
                return;
            }

            // Dikdörtgen boyutları
            int rectangleWidth = pnlkartekranı.Width / columns;
            int rectangleHeight = pnlkartekranı.Height / rows;
            bool leftToRight = true;

            // Dikdörtgenlerin konumunu hesapla ve oluştur
            for (int i = 0; i < rows; i++)
            {
                if (leftToRight)
                {
                    for (int j = 0; j < columns; j++)
                    {
                        CreateRectanglePanel(j * rectangleWidth, i * rectangleHeight, rectangleWidth, rectangleHeight);
                    }
                }
                else
                {
                    for (int j = columns - 1; j >= 0; j--)
                    {
                        CreateRectanglePanel(j * rectangleWidth, i * rectangleHeight, rectangleWidth, rectangleHeight);
                    }
                }

                leftToRight = !leftToRight;
            }
        }

        private void CreateRectanglePanel(int x, int y, int width, int height)
        {
            // Panel oluştur
            Panel rectanglePanel = new Panel
            {
                Location = new Point(x, y),
                Size = new Size(width, height),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Label oluştur
            Label label = new Label
            {
                Text = counter.ToString(),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Arial", 12, FontStyle.Bold),
                ForeColor = Color.White // Label yazı rengi beyaz olarak ayarlandı
            };

            // Panelin üzerine label'ı ekle
            rectanglePanel.Controls.Add(label);

            // Paneli ana panelin içine ekle
            pnlkartekranı.Controls.Add(rectanglePanel);

            // Sayacı artır
            counter++;
        }

        private void Dikdörtgen_Load(object sender, EventArgs e)
        {

        }

        public void button1_Click(object sender, EventArgs e)
        {
            // Tüm dikdörtgenlerin rengini beyaz yapmak için panel içindeki tüm panelleri gez
            foreach (Control control in pnlkartekranı.Controls)
            {
                if (control is Panel)
                {
                    control.BackColor = Color.White;
                }
            }
        }

      
        private void Dikdörtgen_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_koordinatForm != null)
            {
                if (_koordinatForm.berkco == 0)
                {
                    _koordinatForm.ClearComboBoxSelection();
                }
            }

        }

        public void pnlkartekranı_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Dikdörtgen_Load_1(object sender, EventArgs e)
        {

        }

        private void Dikdörtgen_FormClosing_1(object sender, FormClosingEventArgs e)
        {
            if (_koordinatForm != null)
            {
                if (_koordinatForm.berkco == 0)
                {
                    _koordinatForm.ClearComboBoxSelection();
                }
            }
        }
    }
}
