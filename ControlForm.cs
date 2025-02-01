using ProgramAtmaAparatı;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO.Ports;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.AxHost;

namespace ProgramAtmaAparatı
{
    public partial class ControlForm : Form
    {

        private Form1 _koordinatForm; // Ana form referansı
        private Dikdörtgen _dikdörtgenForm; // Dikdörtgen form referansı
        private bool _isProcessing = false;
        private readonly Func<string, Task> sendGCodeWithWaitMethod;
        private readonly double startXCoordinate;
        private readonly double startYCoordinate;
        private readonly Func<bool> sendFlashMethod;
        private readonly double startX;
        private readonly double startY;
        private bool _isButton2Clicked = false; // button2'nin tıklanıp tıklanmadığını izleyen bayrak
        private bool _isPistonMoving = false; // Pistonun hareket edip etmediğini takip etmek için
        private int _deviceIndex = -1;


        // ControlForm yapıcısında startX ve startY'yi parametre olarak al
        public ControlForm(Form1 form, double startX, double startY, Func<string, Task> sendGCodeWithWait, int deviceIndex)
        {

            InitializeComponent();
            _koordinatForm = form;
            sendGCodeWithWaitMethod = sendGCodeWithWait;
            this.startX = startX;
            this.startY = startY;
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            sendGCodeWithWaitMethod = sendGCodeWithWait;
            startXCoordinate = startX;
            startYCoordinate = startY;
            _deviceIndex = deviceIndex;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
        public async void Arduino_DataReceived(string data)
        {
            // Eğer button2 tıklanmışsa, verilerin işlenmesine izin verme
            if (_isButton2Clicked)
            {
                return; // button2_Click tetiklenmişse, veri alımını işleme
            }
            //await Task.Delay(1000); // Komutun işlenmesi için kısa bir bekleme süresi

            try
            {
                // Eğer veri boş değil ve 'start:1' içeriyorsa
                if (!string.IsNullOrEmpty(data) && data.Contains("start:1"))
                {
                    _koordinatForm.pressedStartButton = true;
                    await _koordinatForm.SendPistonControlCommandAsync("PistonUp");

                    this.Invoke(new Action(async () =>
                    {
                        bool flashResult = await _koordinatForm.SendFlashAsync(); // SendFlashAsync fonksiyonunu await ile çağır

                        if (flashResult)
                        {
                            _koordinatForm.Invoke(new Action(() => _koordinatForm.panel4.Controls[_deviceIndex].BackColor = Color.Green));
                            MessageBox.Show("Program Atıldı");
                        }
                        else
                        {
                            _koordinatForm.Invoke(new Action(() => _koordinatForm.panel4.Controls[_deviceIndex].BackColor = Color.Red));
                            MessageBox.Show("Program Atılamadı");
                        }
                        await _koordinatForm.SendPistonControlCommandAsync("PistonDown");
                        _koordinatForm.pressedStartButton = false;
                    }));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Data read error: " + ex.Message);
                _koordinatForm.pressedStartButton = false;
            }
        }

        private void ControlForm_Load_1(object sender, EventArgs e)
        {
            label1.Text = "Starta basınız";
        }



        private void button1_Click(object sender, EventArgs e)
        {
            Close();
            _koordinatForm.Enabled = true;

        }

        private void ControlForm_FormClosing(object sender, FormClosingEventArgs e)
        {


        }

        private async void button1_Click_1(object sender, EventArgs e)
        {
            // Verilerin işlenmesini geçici olarak durdur
            _isButton2Clicked = true; // button2'ye tıklanmış bayrağını true yap

            using (Wait waitForm = new Wait())
            {
                // Wait form'u ana forma göre konumlandır
                waitForm.Location = new Point(this.Location.X, this.Location.Y);
                waitForm.Show();
                waitForm.Refresh(); // Formun güncellenmesini sağlar.

                // İşlemler
                double negativeStartX = -startX;
                double negativeStartY = -startY;

                // Piston'u aşağı indir
                await _koordinatForm.SendPistonControlCommandAsync("PistonDown");

                // Geri dönüş komutunu oluştur
                string returnGcode = $"G91G0X{negativeStartX.ToString(CultureInfo.InvariantCulture)}Y{negativeStartY.ToString(CultureInfo.InvariantCulture)}";

                // GCode'u gönder ve işlenmesini bekle
                if (!await _koordinatForm.SendGCodeWithWait(returnGcode))
                {
                    // Eğer komut başarısız olursa form'u kapatmadan çık
                    return;
                }

                await Task.Delay(500); // Komutun işlenmesi için kısa bir bekleme süresi
            }

            // İşlemler tamamlandıktan sonra bayrağı false yaparak veri almayı tekrar açabilirsin (eğer istersen)
            _isButton2Clicked = false;

            this.Close(); // veya this.Hide();
            _koordinatForm.Enabled = true;
        }

        private void ControlForm_Load(object sender, EventArgs e)
        {

        }
    }
}