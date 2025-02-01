using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Ini.Net;  // Settings için
using ProgramAtmaAparatı;
// Diğer gerekli using direktifleri eklenebilir.

namespace ProgramAtmaAparatı
{
    public partial class Form1 : Form
    {
        // Seri portlar
        private SerialPort portArduino;
        private SerialPort serialPort;

        // Loglama ve mesaj yönetimi
        private readonly List<string> outputMessages = new List<string>();
        private enum MessageType { Info = 1, Warning = 2, Error = 3, Question = 4 }

        // Ürün ve pozisyon bilgileri
        private string currentProductKeyword = "";
        private bool isProductSelected = false;
        private bool isMoving = false;
        private readonly string coordinatesFileName = "koordinatlistesi.txt";
        private bool isPistonActivated = false;
        private double currentXPosition = 0;
        private double currentYPosition = 0;
        private readonly Dictionary<string, Tuple<double, double>> productStartPositions = new Dictionary<string, Tuple<double, double>>();

        // Cyclone ve ilgili değişkenler
        private string cycloneName;
        private UInt32 handle = 0;

        // Timer ve veri tamponu
        private System.Windows.Forms.Timer readTimer;
        private readonly StringBuilder dataBuffer = new StringBuilder();

        // Piston ve sensör durumları
        private bool isPistonDown = false;
        private bool pistonIsUp = false;
        private int pistonState = -1; // 0: Piston yukarıda, 1: Piston aşağıda, -1: Bilinmiyor

        // Diğer değişkenler ve flag'ler
        private int previousX = 0;
        private int previousY = 0;
        private bool isOperationPerformed = false;
        private bool isPistonUp = false;
        private bool lowerSensorActive = false;
        private DateTime lowerSensorActivationTime;
        private bool checkBoxChecked = false;
        private bool limitSwitch1Triggered = false;
        private bool limitSwitch2Triggered = false;
        private readonly Dictionary<string, (int Rows, int Columns)> kartBoyutlari = new Dictionary<string, (int Rows, int Columns)>();
        private bool lazerAcik = false;
        private bool durdur = false;
        private double tempXPosition = 0;
        private double tempYPosition = 0;
        private bool isRunning = false;
        private Thread emergencyCheckThread;
        private volatile bool shouldStopEmergencyCheck = false;
        private bool emergencyActive = false;
        private double lastX = 0;
        private double lastY = 0;
        private frmWait waitForm;
        private double startX = 0;
        private double startY = 0;
        public bool shouldClearComboBox = true; // Gerekirse dışarıdan kontrol edilebilir.
        public int berkco = 0;
        private string logDirectoryPath = "";
        private readonly List<Control> rectangles = new List<Control>();
        private bool isCalibrationActive = false;
        private bool isYLimitHandled = false;
        private bool isXLimitHandled = false;
        public bool pressedStartButton = false;
        private ControlForm controlForm;

        public Form1()
        {
            // Ayarların okunması
            var settings = new IniFile("Settings.ini");
            cycloneName = settings.ReadString("Settings", "CycloneName");
            string arduinoPortName = settings.ReadString("Settings", "ArduinoCom");
            int arduinoBaudrate = settings.ReadInteger("Settings", "ArduinoBaudrate");
            string printerPortName = settings.ReadString("Settings", "PrinterCom");

            // Seri portların başlatılması
            portArduino = new SerialPort(arduinoPortName, arduinoBaudrate);
            serialPort = new SerialPort(printerPortName);

            // Cyclone portlarının enumerate edilip bağlantı kurulması
            cyclone_control_api.enumerateAllPorts();
            handle = cyclone_control_api.connectToCyclone(cycloneName);

            // Form ayarları
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(Form1_KeyDown);

            InitializeComponent();

            // Checkbox değişim eventi
            chkkontrol.CheckedChanged += new EventHandler(chkkontrol_CheckedChanged_1);

            // Başlangıç loglama ve ürün yükleme işlemleri
            label6.Text = "";
            LogToOutput("Cyclone version: " + cyclone_control_api.version());
            cyclone_control_api.enumerateAllPorts();
            handle = cyclone_control_api.connectToCyclone(cycloneName);

            LoadProducts();
            InitializeSerialPort();

            cmburunsecim.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        /// <summary>
        /// Arduino ve CNC seri portlarının başlatılması ve ilk veri alınması.
        /// </summary>
        private void InitializeSerialPort()
        {
            try
            {
                // Arduino portunun ayarlanması
                portArduino.BaudRate = 115200;
                if (!portArduino.IsOpen)
                {
                    portArduino.Open();
                    LogToOutput("Arduino Seri Port Bağlandı");
                }

                if (portArduino.IsOpen)
                {
                    string incomingData = portArduino.ReadLine();

                    // Veri formatı kontrolü: yalnızca '<' ve '>' arasında ise işlenir.
                    if (incomingData.StartsWith("<") && incomingData.EndsWith(">"))
                    {
                        incomingData = incomingData.Trim('<', '>');

                        // Piston durumunu kontrol et ve düzelt (metot içeriğini eklemeniz gerekiyor)
                        CheckAndFixPistonState();

                        // Senkron bekleme (gerekirse metodun async hale getirilmesi düşünülebilir)
                        Task.Delay(250).Wait();

                        bool pistonStateFlag = CheckAndFixPistonStateRead();

                        if (pistonStateFlag)
                        {
                            Home();
                        }
                        else if (incomingData.Contains("EMERGENCY:1"))
                        {
                            if (!serialPort.IsOpen)
                            {
                                serialPort.BaudRate = 115200;
                                serialPort.Open();
                                serialPort.WriteLine("$X");
                                LogToOutput("CNC Seri Port Bağlandı");
                                serialPort.WriteLine("$X"); // CNC komutu gönder
                            }
                        }
                        else
                        {
                            LogToOutput("Bilinmeyen veri alındı: " + incomingData);
                        }
                    }
                    else
                    {
                        LogToOutput("Geçersiz veri formatı alındı: " + incomingData);
                    }
                }

                // CNC'ye "$X" komutunu gönderiyoruz.
                serialPort.WriteLine("$X");
                Thread.Sleep(100);
                shouldStopEmergencyCheck = false;
            }
            catch (UnauthorizedAccessException uaEx)
            {
                LogToOutput("Erişim hatası: " + uaEx.Message);
            }
            catch (IOException ioEx)
            {
                LogToOutput("Bağlantı hatası: " + ioEx.Message);
            }
            catch (InvalidOperationException invOpEx)
            {
                LogToOutput("Geçersiz işlem hatası: " + invOpEx.Message);
            }
            catch (Exception ex)
            {
                LogToOutput("Seri port açılamadı: " + ex.Message);
            }
        }

        /// <summary>
        /// CNC'nin ev konumuna dönmesi.
        /// </summary>
        public void Home()
        {
            try
            {
                SendGCode("G53X0Y0");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        /// <summary>
        /// CNC'ye G-Code komutlarının gönderilmesi.
        /// </summary>
        /// <param name="gcode">Gönderilecek G-Code komutu</param>
        public void SendGCode(string gcode)
        {
            try
            {
                if (serialPort != null && serialPort.IsOpen)
                {
                    serialPort.WriteLine(gcode);
                }
            }
            catch (Exception ex)
            {
                LogToOutput("G kodu gönderme hatası: " + ex.Message);
            }
        }

        /// <summary>
        /// Klavye tuşlarına basıldığında ilgili yön tuşlarının simüle edilmesi.
        /// (Örneğin chkkontrol işaretliyse ok tuşları ilgili butonlara tıklanır.)
        /// </summary>
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (chkkontrol.Checked)
            {
                switch (e.KeyCode)
                {
                    case Keys.Up:
                        btnUp.PerformClick();
                        break;
                    case Keys.Left:
                        btnLeft.PerformClick();
                        break;
                    case Keys.Down:
                        btnDown.PerformClick();
                        break;
                    case Keys.Right:
                        btnRight.PerformClick();
                        break;
                }
            }
        }

        // Aşağıdaki metodlar, projede henüz detaylandırılması gereken işlevler için yer tutucudur.

        /// <summary>
        /// chkkontrol değiştiğinde çalışacak event handler.
        /// </summary>
        private void chkkontrol_CheckedChanged_1(object sender, EventArgs e)
        {
            // İhtiyaca göre checkbox değişimine özel işlemler eklenebilir.
        }

        /// <summary>
        /// Ürün listesi yükleniyor.
        /// </summary>
        private void LoadProductList()
        {
            // Ürün listesi yükleme işlemleri burada yapılacak.
        }

        /// <summary>
        /// Ürünlerle ilgili detayların yüklenmesi.
        /// </summary>
        private void LoadProducts()
        {
            // Ürün detaylarının yüklenmesi işlemleri burada yapılacak.
        }

        /// <summary>
        /// Gelen mesajların loglanması (örneğin, konsola veya bir TextBox'a yazdırılabilir).
        /// </summary>
        /// <param name="message">Loglanacak mesaj</param>
        private void LogToOutput(string message)
        {
            // Loglama işlemlerini uygulamanıza göre düzenleyin.
            Debug.WriteLine(message);
            // Örneğin: outputTextBox.AppendText(message + Environment.NewLine);
        }

        /// <summary>
        /// Piston durumunu kontrol edip, gerekiyorsa düzeltme işlemleri yapar.
        /// </summary>
        private void CheckAndFixPistonState()
        {
            // Piston durumunun kontrolü ve gerekirse düzeltme işlemleri burada yapılmalıdır.
        }

        /// <summary>
        /// Piston durumunu okur. İstenen duruma ulaşıldıysa true döner.
        /// </summary>
        /// <returns>True ise piston istenen durumda demektir.</returns>
        private bool CheckAndFixPistonStateRead()
        {
            // Piston durumunu okuyup, istenen duruma (örneğin down sensör aktif) ulaşıldıysa true döndürün.
            return true; // Geçici olarak true döndürüyoruz.
        }
    }
}


public async void cmburunsecim_SelectedIndexChanged(object sender, EventArgs e)
{
    // Eğer bir ürün seçildiyse
    if (cmburunsecim.SelectedItem != null)
    {
        // Eğer daha önce oluşturulmuş dikdörtgen form varsa kapat ve bellekten at
        if (dikdörtgenForm != null && !dikdörtgenForm.IsDisposed)
        {
            berkco = 1;
            dikdörtgenForm.Close();
            dikdörtgenForm.Dispose();
            dikdörtgenForm = null;
            berkco = 0;
        }

        shouldClearComboBox = false;

        // Kullanıcıya başlangıç noktasına gitmek isteyip istemediğini sor
        DialogResult dialogResult = MessageBox.Show(
            "Yeni bir ürün seçildi. Önce başlangıç noktasına gitmek ister misiniz?",
            "Başlangıç Noktası Uyarısı",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning);

        // Piston kontrolünü yap ve gerekiyorsa düzelt
        CheckAndFixPistonState();

        // Kısa bekleme: işlem sırasındaki senkronizasyon için
        await Task.Delay(500);

        // Piston durumunu oku (true ise istenen durumda demektir)
        bool pistonState = CheckAndFixPistonStateRead();

        if (pistonState)
        {
            // Eğer kullanıcı başlangıç noktasına gitmek istiyorsa
            if (dialogResult == DialogResult.Yes)
            {
                string gcodeStart = "G53X0Y0";
                SendGCode(gcodeStart);
                await Task.Delay(1000);
            }

            // Seçilen üründen M kodu kısmını (ilk alt çizgiden önceki kısmı) al
            currentProductKeyword = cmburunsecim.SelectedItem.ToString().Split('_')[0];

            // Kombobox'ı güncellemek için gerekli metodu çağır
            await UpdateComboBoxWithCoordinates();

            // Son konum bilgisini dosyadan yükle
            LoadLastProductPositionFromFile();

            // Seçilen ürünün ön eki olarak ilk 5 karakteri kullan (örn. "M1234")
            string selectedProductPrefix = cmburunsecim.SelectedItem.ToString().Substring(0, 5);

            // Ürünle ilgili dosya yolunu oluştur
            string productFilePath = Path.Combine("CycloneInfineon", $"{selectedProductPrefix}.txt");

            if (File.Exists(productFilePath))
            {
                string[] productFileLines = File.ReadAllLines(productFilePath);

                // "Programmer:" satırını bulup işlemci türünü belirle
                string programmerLine = productFileLines.FirstOrDefault(line => line.StartsWith("Programmer:"));
                if (!string.IsNullOrEmpty(programmerLine))
                {
                    if (programmerLine.Contains("Cyclone"))
                    {
                        chkcyclone.Checked = true;
                        chkınfıneon.Checked = false;
                    }
                    else if (programmerLine.Contains("Infineon"))
                    {
                        chkcyclone.Checked = false;
                        chkınfıneon.Checked = true;
                    }
                }

                // Eğer dosya varsa ve işlemci türü Cyclone ise,
                // dosyadaki program sırası bilgisini oku ve Edit1'e yaz
                if (chkcyclone.Checked)
                {
                    label6.Text = "";
                    string txtFilePath = Path.Combine("CycloneInfineon", $"{selectedProductPrefix}.txt");
                    if (File.Exists(txtFilePath))
                    {
                        string[] lines1 = File.ReadAllLines(txtFilePath);
                        string programSequenceLine = lines1.FirstOrDefault(line1 => line1.StartsWith("Program Sırası:"));
                        if (!string.IsNullOrEmpty(programSequenceLine))
                        {
                            string[] parts = programSequenceLine.Split(':');
                            if (parts.Length == 2 && int.TryParse(parts[1].Trim(), out int programSequence))
                            {
                                Edit1.Text = programSequence.ToString();
                            }
                        }
                    }


                    // Cyclone bağlantısı ve işlem başlatma işlemleri
                    UInt32 connectionType = cyclone_control_api.CyclonePortType_USB;
                    UInt32 handle = 0;

                    var settings = new Ini.Net.IniFile("Settings.ini");
                    string cycloneName = settings.ReadString("Settings", "CycloneName");

                    handle = cyclone_control_api.connectToCyclone(cycloneName);
                    Application.DoEvents();

                    if (cyclone_control_api.checkCycloneExecutionStatus(handle) == 0)
                    {
                        if (cyclone_control_api.getNumberOfErro


                            label6.Visible = true;

                            // Kart boyutu dosyasını oku ve ürünün boyutlarını ayarla
                            string kartBoyutuKlasorYolu = "KartBoyutu";
                            int rows = 0;
                            int columns = 0;

                            // Ürünün MCode'unun ilk 5 karakterini al (örn. M0088)
                            string productPrefix = currentProductKeyword.Substring(0, 5);

                            // MCode'a göre dosya adı belirle (ilk 5 karakteri kullanarak)
                            string kartBoyutuDosyaYolu = Path.Combine(kartBoyutuKlasorYolu, $"{productPrefix}.txt");

                            if (File.Exists(kartBoyutuDosyaYolu))
                            {
                                // Dosyanın içeriğini oku
                                string[] kartBoyutuLines = File.ReadAllLines(kartBoyutuDosyaYolu);

                                // İlk satırı al ve boyutları ayrıştır (örneğin: M0088: 2x3)
                                if (kartBoyutuLines.Length > 0)
                                {
                                    string firstLine = kartBoyutuLines[0]; // İlk satır örneği: "M0088: 2x3"

                                    // Boyutları ayrıştır
                                    string[] parts = firstLine.Split(':')[1].Trim().Split('x');
                                    if (parts.Length == 2)
                                    {
                                        // Boyutları integer olarak ayarla
                                        if (int.TryParse(parts[0], out rows) && int.TryParse(parts[1], out columns))
                                        {
                                            // Başarıyla okunan boyutlar burada kullanılabilir
                                            Console.WriteLine($"Rows: {rows}, Columns: {columns}");
                                        }
                                    }
                                }
                            }
                            else
                            {
                                // Dosya bulunamadıysa hata işleme yapılabilir
                                Console.WriteLine($"Dosya bulunamadı: {kartBoyutuDosyaYolu}");
                            }
                            // Bu kısımda 'rows' ve 'columns' değerlerini doğru şekilde geçirmeniz gerekecek
                            CreateRectangles(rows, columns);

                            //// Dikdörtgen formunu oluştur ve göster
                            dikdörtgenForm = new Dikdörtgen(rows, columns, this);
                           dikdörtgenForm.Show();

                            string selectedMCode = currentProductKeyword.Substring(0, 5);

                            // KoordinatlarListesi klasöründeki tüm .txt dosyalarını al
                            string koordinatlarKlasoru = "KoordinatlarListesi";
                            string[] koordinatDosyalari = Directory.GetFiles(koordinatlarKlasoru, "*.txt");

                            // Uygun dosyayı bulmak için dosya adlarını kontrol et
                            string koordinatDosyasi = koordinatDosyalari.FirstOrDefault(dosya =>
                           Path.GetFileNameWithoutExtension(dosya).StartsWith(selectedMCode));

                            if (!string.IsNullOrEmpty(koordinatDosyasi) && File.Exists(koordinatDosyasi))
                            {
                                string[] koordinatLines = File.ReadAllLines(koordinatDosyasi);
                                double cumulativeX = 0;
                                double cumulativeY = 0;
                                int pointNumber = 1;

                                cmbhatadurumu.Items.Clear();
                                cmbhatadurumu.Items.Add($"Nokta {pointNumber}: 0.00 0.00");
                                pointNumber++;

                                foreach (string line in koordinatLines)
                                {
                                    string[] coords = line.Trim().Split(' ');
                                    if (coords.Length == 2)
                                    {
                                        string xCoord = coords[0].Replace(',', '.');
                                        string yCoord = coords[1].Replace(',', '.');

                                        double x = Convert.ToDouble(xCoord, CultureInfo.InvariantCulture);
                                        double y = Convert.ToDouble(yCoord, CultureInfo.InvariantCulture);

                                        cumulativeX += x;
                                        cumulativeY += y;

                                        cmbhatadurumu.Items.Add($"Nokta {pointNumber}: {cumulativeX.ToString("F2", CultureInfo.InvariantCulture)} {cumulativeY.ToString("F2", CultureInfo.InvariantCulture)}");
                                        pointNumber++;
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show($"Ürün için koordinat dosyası bulunamadı: {koordinatDosyasi}");
                            }
                        }
                        else
                        {

                            button3.Visible = false;
                            label6.Visible = false;
                        }

                        LogToOutput($"Ürün Seçildi: {cmburunsecim.SelectedItem}");
                        lblurunsecim.Text = "Ürün Seçildi: " + cmburunsecim.SelectedItem;

                        btnStart.Enabled = true;

                        if (cmburunsecim.SelectedItem.ToString().EndsWith("Yeni Ürün Ekle"))
                        {
                            YeniUrunEkleForm yeniUrunForm = new YeniUrunEkleForm(this);
                            yeniUrunForm.SetYeniUrunEklendiHandler(new Action<string>((urunKey) =>
                            {
                                // Ürünün başarıyla eklendiğinde yapılacak işlemler
                                cmburunsecim.Items.Add(urunKey);
                            }));
                            DialogResult result = yeniUrunForm.ShowDialog();

                            if (result == DialogResult.OK)
                            {
                                // Yeni ürün ekleme işlemleri
                            }
                        }
                    }
                }

                else
                {
                    LogToOutput("Piston Hatası!!!");
                }
            }
        }



        private UInt32 convert_dropboxindex_to_connectiontype(int index)
        {
            UInt32 result = 0;
            switch (index)
            {
                case 0:
                    result = cyclone_control_api.CyclonePortType_USB;
                    break;
                    //case 1:
                    //    result = cyclone_control_api.CyclonePortType_Ethernet;
                    //    break;
                    //case 2:
                    //    result = cyclone_control_api.CyclonePortType_Serial;
                    //    break;
                    //default:
                    //    result = cyclone_control_api.CyclonePortType_USB;
                    //    break;
            }
            return result;
        }
        public bool CheckAndFixPistonState()
        {
            if (pistonState == 1) //piston aşağıda ise yukarı kaldır.
            {
                portArduino.WriteLine("TogglePiston");
                Thread.Sleep(1000);

                if (pistonState == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (pistonState == -1)
            {
                portArduino.WriteLine("PistonDown");
                Thread.Sleep(1000);

                if (pistonState == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        public bool CheckAndFixPistonStateRead()
        {
            return pistonState == 0; // Piston yukarıda mı?
        }


        private async Task UpdateComboBoxWithCoordinates()
        {

            try
            {
                LoadLastProductPositionFromFile();

                // currentProductKeyword'in ilk 5 hanesini al
                string selectedMCode = currentProductKeyword.Substring(0, 5);

                // KoordinatlarListesi klasöründeki tüm .txt dosyalarını al
                string koordinatlarKlasoru = "KoordinatlarListesi";
                string[] koordinatDosyalari = Directory.GetFiles(koordinatlarKlasoru, "*.txt");

                // Uygun dosyayı bulmak için dosya adlarını kontrol et
                string koordinatDosyasi = koordinatDosyalari.FirstOrDefault(dosya =>
                    Path.GetFileNameWithoutExtension(dosya).StartsWith(selectedMCode));

                if (!string.IsNullOrEmpty(koordinatDosyasi) && File.Exists(koordinatDosyasi))
                {
                    string[] lines = File.ReadAllLines(koordinatDosyasi);
                    bool isTargetProduct = false;
                    int pointNumber = 1;
                    double cumulativeX = lastX;  // Son X pozisyonunu eklemeye başla
                    double cumulativeY = lastY;  // Son Y pozisyonunu eklemeye başla

                    cmbhatadurumu.Items.Clear();
                    cmbhatadurumu.Items.Add($"Nokta {pointNumber}: {cumulativeX.ToString(CultureInfo.InvariantCulture)} {cumulativeY.ToString(CultureInfo.InvariantCulture)}");
                    pointNumber++;

                    foreach (string line in lines)
                    {
                        if (line.StartsWith("[")) // Ürün başlangıcını kontrol et
                        {
                            if (line.Contains(currentProductKeyword))
                            {
                                isTargetProduct = true;
                                cumulativeX = lastX; // Son X pozisyonunu sıfırla
                                cumulativeY = lastY; // Son Y pozisyonunu sıfırla
                                pointNumber = 1;     // Nokta numarasını sıfırla
                                cmbhatadurumu.Items.Clear(); // Daha önceki noktaları temizle
                            }
                            else
                            {
                                isTargetProduct = false;
                            }
                        }
                        else if (isTargetProduct)
                        {
                            string[] coords = line.Trim().Split(' ');
                            if (coords.Length == 2)
                            {
                                string xCoord = coords[0].Replace(',', '.');
                                string yCoord = coords[1].Replace(',', '.');

                                double x = Convert.ToDouble(xCoord, CultureInfo.InvariantCulture);
                                double y = Convert.ToDouble(yCoord, CultureInfo.InvariantCulture);

                                cumulativeX += x;
                                cumulativeY += y;

                                cmbhatadurumu.Items.Add($"Nokta {pointNumber}: {cumulativeX.ToString("F2", CultureInfo.InvariantCulture)} {cumulativeY.ToString("F2", CultureInfo.InvariantCulture)}");
                                pointNumber++;
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show($"Ürün için koordinat dosyası bulunamadı: {selectedMCode}");
                }
            }
            catch (Exception ex)
            {
                // Hata durumunda kullanıcıya bildirimde bulun
                MessageBox.Show($"Hata oluştu: {ex.Message}");
            }
        }

        private void LoadLastProductPositionFromFile()
        {
            try
            {
                // comboBox1'deki format: "MCode_UrunAdi" şeklinde
                string[] tokens = cmburunsecim.SelectedItem.ToString().Split('_');
                if (tokens.Length < 2)
                {
                    MessageBox.Show("Geçersiz ürün formatı.");
                    return;
                }

                string mCode = tokens[0];  // M kodunu al
                string urunAdi = tokens[1]; // Ürün adını al

                // Klasör ve dosya yolu: "ÜrünBaşlangıçKoordinatları/MCode_UrunAdi/MCode.txt"
                string coordinatesFolderPath = Path.Combine(Application.StartupPath, "ÜrünBaşlangıçKoordinatları");
                string folderPath = Path.Combine(coordinatesFolderPath, $"{mCode}");
                string filePath = Path.Combine(folderPath, $"{mCode}.txt");

                if (!File.Exists(filePath))
                {
                    //MessageBox.Show("Başlangıç noktası bulunamadı.");
                    return;
                }

                // Dosyadaki satırları oku
                string[] lines = File.ReadAllLines(filePath);
                if (lines.Length == 0)
                {
                    MessageBox.Show("Dosyada veri yok.");
                    return;
                }

                // En son satırdaki X ve Y koordinatlarını al
                string lastLine = lines[lines.Length - 1];
                string[] tokensLine = lastLine.Trim().Split(' ');

                if (tokensLine.Length >= 3)
                {
                    string lastX = tokensLine[1];
                    string lastY = tokensLine[2];

                    // G kodunu gönder
                    string gcode = $"G53 G0 X{lastX} Y{lastY}";
                    SendGCode(gcode);
                }
                else
                {
                    MessageBox.Show("Koordinatlar geçerli değil.");
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show($"Bir hata oluştu: {ex.Message}");
            }
        }
        private async Task<bool> GoToProductStartPosition(string productName)
        {
            try
            {
                string productKey = productName.Length >= 5 ? productName.Substring(0, 5) : productName;

                string folderPath = Path.Combine("ÜrünBaşlangıçKoordinatları", productKey);
                string filePath = Path.Combine(folderPath, $"{productKey}.txt");

                if (File.Exists(filePath))
                {
                    string[] lines = File.ReadAllLines(filePath);
                    string lastX = "";
                    string lastY = "";

                    for (int i = lines.Length - 1; i >= 0; i--)
                    {
                        string[] tokens = lines[i].Split(' ');
                        if (tokens.Length >= 3 && tokens[0].StartsWith(productKey))
                        {
                            lastX = tokens[1];
                            lastY = tokens[2];
                            break;
                        }
                    }

                    if (!string.IsNullOrEmpty(lastX) && !string.IsNullOrEmpty(lastY))
                    {
                        if (!CheckAndFixPistonState())
                        {
                            throw new Exception("Piston durumu doğrulanamadı. Lütfen kontrol ediniz. İşlem iptal edildi.");
                        }
                        string gcode = $"G53G0X{lastX}Y{lastY}";


                        SendGCode(gcode);
                        chkkontrol.Visible = false; // Replace someControl with the actual control you want to hide
                        btnDown.Visible = false;
                        btnUp.Visible = false;
                        btnRight.Visible = false;
                        btnLeft.Visible = false;
                        return true; // Başarılı olarak geri dön

                    }
                    else
                    {
                        LogToOutput("Son X ve Y koordinatları bulunamadı.");
                        chkkontrol.Visible = true; // Replace someControl with the actual control you want to hide
                        btnDown.Visible = true;
                        btnUp.Visible = true;
                        btnRight.Visible = true;
                        btnLeft.Visible = true;
                        return false;

                    }
                }
                else
                {
                    LogToOutput("Ürün başlangıç pozisyonu bulunamadı.");
                    chkkontrol.Visible = true; // Replace someControl with the actual control you want to hide
                    btnDown.Visible = true;
                    btnUp.Visible = true;
                    btnRight.Visible = true;
                    btnLeft.Visible = true;

                    return false;

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata oluştu: {ex.Message}");
                return false;
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {



            if (!Directory.Exists("Logs"))
            {
                _logDirectoryPath = Directory.CreateDirectory("Logs").FullName;
            }
            else
            {
                _logDirectoryPath = Application.StartupPath + "\\Logs";
            }
            portArduino.DataReceived += PortArduino_DataReceived;  // Veri alındığında tetiklenecek event handler
            LoadComboBoxItems();
            btnDown.Visible = true;
            btnLeft.Visible = true;
            btnUp.Visible = true;
            btnRight.Visible = true;

            chkkontrol.Visible = false;
            cmbhatadurumu.Enabled = false;
            //var result = MessageBox.Show("Başlangıç noktasına dönmek ister misiniz?", "Uyarı", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            //if (result == DialogResult.Yes)
            //{
            //    // Eğer kullanıcı 'Evet' dediyse, başlangıç noktasına dönmek için G53 komutunu gönder
            //    string gcode = "G53 X0 Y0";
            //    SendGCode(gcode);
            //}
        }
        private void LoadComboBoxItems()
        {

            // "Koordinatlar" klasörünü oku
            string coordinatesFolderPath = "KoordinatlarListesi";

            if (Directory.Exists(coordinatesFolderPath))
            {
                // Koordinatlar klasöründeki tüm alt klasörleri ve dosyaları al
                var directories = Directory.GetDirectories(coordinatesFolderPath);

                foreach (var directory in directories)
                {
                    // Her klasör için, içinde bulunan koordinat dosyasını bul
                    var txtFiles = Directory.GetFiles(directory, "*.txt");

                    foreach (var file in txtFiles)
                    {
                        // Dosyayı oku ve mcode ile ürün adını al
                        string[] lines = File.ReadAllLines(file);
                        string mcode = string.Empty;
                        string urunAdi = string.Empty;

                        foreach (string line in lines)
                        {
                            // mcode'u bul
                            if (line.StartsWith("MCode:"))
                            {
                                mcode = line.Substring("MCode:".Length).Trim();
                            }

                            // Urun adını bul, dosya ismine bakarak bulacağız
                            if (Path.GetFileNameWithoutExtension(file).Contains("_"))
                            {
                                urunAdi = Path.GetFileNameWithoutExtension(file).Split('_')[1];
                            }

                            // Eğer hem mcode hem de ürün adı bulunduysa ComboBox'a ekle
                            if (!string.IsNullOrEmpty(mcode) && !string.IsNullOrEmpty(urunAdi))
                            {
                                cmburunsecim.Items.Add($"{mcode}_{urunAdi}");
                                break; // mcode ve urunAdı bulduğumuz için daha fazla satıra bakmaya gerek yok
                            }
                        }
                    }
                }
            }
        }
        private async void PortArduino_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                // Arduino'dan gelen veriyi oku
                string data = portArduino.ReadLine().Trim();



                HandlePistonState(data);

                if (isCalibrationActive)
                {
                    if (data.StartsWith("<") && data.EndsWith(">"))
                    {
                        data = data.Trim('<', '>'); // Sadece <> içindeki kısmı al

                        if (data.Contains("XLimit") && !isXLimitHandled)
                        {
                            await HandleXLimit(data);
                            isXLimitHandled = true;

                            await Task.Delay(1500);

                            if (data.Contains("YLimit") && !isYLimitHandled)
                            {
                                await HandleYLimit(data, true);
                                isYLimitHandled = true;
                            }
                        }

                        // Kalibrasyon tamamlandıktan sonra değişkenleri sıfırla
                        isCalibrationActive = false;
                        isXLimitHandled = false;
                        isYLimitHandled = false;
                    }
                }

                // Kontrol formuna veri ilet
                if (controlForm != null)
                {
                    if (!pressedStartButton)
                    {
                        Task.Run(() => controlForm.Arduino_DataReceived(data));
                    }
                }

                // Başlangıç durumu kontrolü
                if (data.Contains("start:1"))
                {
                    if (!pressedStartButton)
                    {
                        // Start button click eventini tetikle
                        Task.Run(() => this.Invoke(new Action(() =>
                        {
                            // Burada startbutton.PerformClick() ya da gerekli başka işlemler yapılabilir
                            //btnStart.PerformClick(); // Start butonuna tıklama işlemi
                            btnStart_Click(btnStart, EventArgs.Empty);  // Start butonunu tıklamak
                            LogToOutput("Start butonuna basıldı.");
                        })));
                    }
                }
                // Acil durum kontrolü
                else if (data.Contains("EMERGENCY:1"))
                {
                    portArduino.Close();
                    emergencyActive = true;
                    serialPort.WriteLine(((char)0x18).ToString()); // Acil durumu aktif et
                    serialPort.Close();
                    MessageBox.Show("Acil durdurma aktif! Seri port kapatıldı.");
                    LogToOutput("Acil durdurma aktif, seri port kapatıldı.");

                    if (data.Contains("S1:1") && data.Contains("S2:0")) // Piston aşağıdaysa, pistonu toggle et
                    {
                        portArduino.WriteLine("TogglePiston");
                        LogToOutput("Piston durumu değiştiriliyor.");
                    }
                }
                else if (data.Contains("EMERGENCY:0"))
                {
                    emergencyActive = false;

                    if (!serialPort.IsOpen)
                    {
                        serialPort.BaudRate = 115200;
                        serialPort.Open();
                        serialPort.WriteLine("$X");
                        LogToOutput("CNC Seri Port Bağlandı.");
                    }
                }
            }
            catch (Exception ex)
            {
                // Hata loglama
                LogToOutput("Hata: " + ex.Message);
            }
        }
        private void HandlePistonState(string data)
        {
            if (data.Contains("S1:1") && data.Contains("S2:0")) // Piston aşağıda
            {
                pistonState = 1;
            }
            else if (data.Contains("S1:0") && data.Contains("S2:1")) // Piston yukarıda
            {
                pistonState = 0;
            }
            else
            {
                pistonState = -1; // Belirsiz durum
            }
        }
        private async Task HandleXLimit(string data)
        {
            string xLimitValue = ExtractValue(data, "XLimit:");


            if (xLimitValue == "Pasif")
            {
                // Adım 1: X ekseninde hareket
                string returnGcode = $"G91G0X{-200}";
                SendGCode(returnGcode);
            }
            LogToOutput("X ekseni hareketi başlatıldı.");

            Thread.Sleep(250);

            if (xLimitValue == "Pasif")
            {
                LogToOutput("XLimit durumu: Aktif");

                serialPort.WriteLine("$X"); // CNC komutu gönder
                Thread.Sleep(250);
                //SendGCode("G91G0Y200"); // Y eksenine hareket

            }
        }

        private async Task HandleYLimit(string data, bool isPin7)
        {
            await Task.Delay(1500);
            string yLimitValue = ExtractValue(data, "YLimit:");

            serialPort.WriteLine("$X"); // CNC komutu gönder
            if (yLimitValue == "Pasif")
            {
                SendGCode("G91G0Y200");
                Thread.Sleep(500);
            }
            Thread.Sleep(1500);

            if (yLimitValue == "Pasif")
            {
                LogToOutput("YLimit durumu: Aktif");

                await Task.Delay(1750);

                serialPort.WriteLine("$X"); // CNC komutu gönder
                await Task.Delay(1750);

                SendGCode("G91X30Y-30"); // G91X10Y-10 komutu gönderiliyor

                Thread.Sleep(1750);

                // XLimit ve YLimit pasif durumunu kontrol et
                if (!data.Contains("XLimit:Aktif") && !data.Contains("YLimit:Aktif"))
                {
                    string command = "Pin7High:0"; // Komut oluştur
                    portArduino.WriteLine(command); // Arduino'ya komutu gönder
                    await Task.Delay(1500);

                    LogToOutput("Kalibrasyon tamamlandı.");

                    serialPort.WriteLine("$X"); // CNC komutu gönder
                }
                else
                {
                    LogToOutput("Kalibrasyon tamamlanamadı: XLimit veya YLimit aktif.");
                }
                await Task.Delay(1500);

                serialPort.WriteLine("$X"); // CNC komutu gönder
                await Task.Delay(1500);
                //SendGCode("G91G0Y200"); // Y eksenine hareket

            }


        }

        /// <summary>
        /// Veriyi belirli bir anahtara göre ayıklar.
        /// </summary>
        private string ExtractValue(string data, string key)
        {
            foreach (string part in data.Split(' '))
            {
                if (part.StartsWith(key))
                {
                    return part.Substring(key.Length);
                }
            }
            return string.Empty;
        }
        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void btnclose_Click(object sender, EventArgs e)
        {
            Process.GetCurrentProcess().Kill();

        }

        private void btnmin_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;

        }
        private void LogToOutput(string message, bool isErrorMessage = false)
        {
            string logMessage = /*DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss") +*/ " -> " + message + Environment.NewLine;

            Action a;

            a = () =>
            {
                rxtLog.AppendText(logMessage);

                if (isErrorMessage)
                {
                    int startIndex = (rxtLog.Text.Length + 1) - logMessage.Length;
                    rxtLog.Select(startIndex < 0 ? 0 : startIndex, logMessage.Length);
                    rxtLog.SelectionColor = Color.Red;
                    rxtLog.SelectionFont = new Font(rxtLog.Font, FontStyle.Bold);
                    rxtLog.SelectionStart = rxtLog.Text.Length;
                    rxtLog.SelectionLength = 0;
                    rxtLog.SelectionColor = rxtLog.ForeColor; // Rengi geri al
                    rxtLog.SelectionFont = new Font(rxtLog.Font, FontStyle.Regular);
                }

                rxtLog.ForeColor = Color.Black;

            };

            if (rxtLog.IsHandleCreated)
            {
                rxtLog.Invoke(a);
            }

            _outputMessages.Add($"{DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")}{logMessage}");
        }
        private DialogResult ShowMessageBox(string message, MessageType messageType, MessageBoxButtons mbb = MessageBoxButtons.OK)
        {
            Action a;
            DialogResult dr = new DialogResult();
            if (messageType == MessageType.Info)
            {
                a = () => dr = MessageBox.Show(this, message, "Bilgi", mbb, MessageBoxIcon.Information);
                Invoke(a);
                LogToOutput(message);
            }
            else if (messageType == MessageType.Warning)
            {
                a = () => dr = MessageBox.Show(this, message, "Uyarı", mbb, MessageBoxIcon.Warning);
                Invoke(a);
                LogToOutput(message, true);
            }
            else if (messageType == MessageType.Error)
            {
                a = () => dr = MessageBox.Show(this, message, "Hata", mbb, MessageBoxIcon.Error);
                Invoke(a);
                LogToOutput(message, true);
            }
            else if (messageType == MessageType.Question)
            {
                a = () => dr = MessageBox.Show(this, message, "Soru", mbb, MessageBoxIcon.Question);
                Invoke(a);
                LogToOutput(message);
            }

            return dr;
        }

        private void btntemizle_Click(object sender, EventArgs e)
        {

            if (DialogResult.Yes == ShowMessageBox("Temizlensin mi?", MessageType.Question, MessageBoxButtons.YesNo))
            {
                rxtLog.Clear();
                _outputMessages.Clear();
            }
        }

        private void chkkontrol_CheckedChanged_1(object sender, EventArgs e)
        {
            if (chkkontrol.Checked)
            {
                btnpiston.Enabled = false;
                this.KeyPreview = true;
                cmburunsecim.Enabled = false;
                btnyeniurunekle.Enabled = false;
                btnbaslangıckaydet.Enabled = false;
                btnstop.Enabled = false;
                btnhome.Enabled = false;
            }
            else
            {
                btnpiston.Enabled = true;
                this.KeyPreview = false;
                cmburunsecim.Enabled = true;
                btnyeniurunekle.Enabled = true;
                btnbaslangıckaydet.Enabled = true;
                btnstop.Enabled = true;
                btnhome.Enabled = true;
            }
        }

        //wait formu ekleyince kullanıcam.
        public void TriggerStopButton()
        {
            char stopCharacter = (char)0x18;
            serialPort.WriteLine(stopCharacter.ToString());
            portArduino.Close();
            serialPort.Close();
            MessageBox.Show("Bağlantı kesildi.");
            this.Enabled = true;
            //if (waitForm != null && !waitForm.IsDisposed)
            //{
            //    waitForm.Close();
            //}
        }
        private void LoadProductList()
        {
            if (File.Exists(coordinatesFileName))
            {
                string[] lines = File.ReadAllLines(coordinatesFileName);
                foreach (string line in lines)
                {
                    if (line.StartsWith("["))
                    {
                        string productName = line.Trim('[', ']');
                        cmburunsecim.Items.Add(productName);
                    }
                }
            }
        }
        private void LoadProducts()
        {
            string fileName = "yeniürün.txt";

            if (File.Exists(fileName))
            {
                cmburunsecim.Items.Clear();
                string[] lines = File.ReadAllLines(fileName);
                foreach (string line in lines)
                {
                    cmburunsecim.Items.Add(line.Trim());
                }
            }

        }

        private void btnyeniurunekle_Click(object sender, EventArgs e)
        {
            YeniUrunEkleForm yeniUrunForm = new YeniUrunEkleForm(this);
            yeniUrunForm.StartPosition = FormStartPosition.CenterParent;

            yeniUrunForm.SetYeniUrunEklendiHandler((yeniUrunAdi) =>
            {
                cmburunsecim.Items.Add(yeniUrunAdi);
                SaveProductList();
            });


            DialogResult result = yeniUrunForm.ShowDialog();
        }



        private void btnbaslangıckaydet_Click(object sender, EventArgs e)
        {
            // Retrieve the selected product name from comboBox1
            string selectedProduct = cmburunsecim.SelectedItem?.ToString();

            if (string.IsNullOrEmpty(selectedProduct) || selectedProduct.Length < 5)
            {
                MessageBox.Show("Geçerli bir ürün seçilmedi veya ürün adı yeterince uzun değil.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Get the first 5 characters of the product name
            string productPrefix = selectedProduct.Substring(0, 5);

            // Construct the path to the product's folder and text file
            string productFolderPath = Path.Combine("ÜrünBaşlangıçKoordinatları", productPrefix);
            string productFilePath = Path.Combine(productFolderPath, $"{productPrefix}.txt");

            try
            {
                // Ensure the folder exists
                if (!Directory.Exists(productFolderPath))
                {
                    Directory.CreateDirectory(productFolderPath);
                }

                // Read the existing lines from the file
                string[] lines = File.Exists(productFilePath) ? File.ReadAllLines(productFilePath) : new string[0];

                if (lines.Length > 0)
                {
                    // Get the last line
                    string lastLine = lines[lines.Length - 1];
                    string[] tokens = lastLine.Trim().Split(' ');

                    if (tokens.Length >= 3)
                    {
                        string lastX = tokens[1];
                        string lastY = tokens[2];

                        // Clear the file
                        File.WriteAllText(productFilePath, string.Empty);

                        // Write the last line back to the file
                        File.AppendAllText(productFilePath, lastLine + Environment.NewLine);

                        // Log message
                        LogToOutput($"Mevcut konum G28 olarak ayarlandı. X: {lastX}, Y: {lastY}");

                        // Show saved message
                        MessageBox.Show("Kaydedildi", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        chkkontrol.Visible = false; // Replace someControl with the actual control you want to hide
                        btnDown.Visible = false;
                        btnUp.Visible = false;
                        btnRight.Visible = false;
                        btnLeft.Visible = false;
                        //dikdörtgenForm.Close(); // Close the form
                        //dikdörtgenForm.Dispose();

                        Task.Delay(250);

                        bool pistonState = CheckAndFixPistonStateRead();

                        if (pistonState) // int data = 1 durumuna eşdeğer
                        {
                            SendGCode("G53X0Y0");
                        }

                    }
                    else
                    {
                        MessageBox.Show("Son satırdaki koordinatlar geçerli değil.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Dosya boş.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnduzenle_Click(object sender, EventArgs e)
        {
            using (Password passwordForm = new Password())
            {
                if (passwordForm.ShowDialog() == DialogResult.OK)
                {
                    // Şifre doğrulandı, düzenleme formunu aç
                    using (editForm editForm = new editForm(this, cmburunsecim))
                    {
                        editForm.ShowDialog();
                    }
                }
                else
                {
                    MessageBox.Show("Şifre yanlış veya giriş iptal edildi.");
                }
            }
        }

        private async void btnStart_Click(object sender, EventArgs e)
        {

            pressedStartButton = true;

            if (chkhatadurumu.Checked && cmbhatadurumu.SelectedItem == null)
            {
                MessageBox.Show("Lütfen bir koordinat seçiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (chkhatadurumu.Checked)
            {
                pressedStartButton = false;
                return;
            }

            if (!btnStart.Enabled)
            {
                pressedStartButton = false;
                return;
            }

            if (!CheckAndFixPistonState())
            {
                throw new Exception("Piston durumu doğrulanamadı. Lütfen kontrol ediniz. İşlem iptal edildi.");
            }

            frmWait waitForm = new frmWait(this);
            waitForm.Show();
            this.Enabled = false;

            bool isCycloneChecked = chkcyclone.Checked;
            bool isProgramChecked = chkınfıneon.Checked;

            if ((isCycloneChecked && isProgramChecked) || (!isCycloneChecked && !isProgramChecked))
            {
                MessageBox.Show("Lütfen sadece bir kontrol seçin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Enabled = true;
                pressedStartButton = false;
                return;
            }

            if (!string.IsNullOrEmpty(currentProductKeyword) && currentProductKeyword != "Yeni Ürün Ekle")
            {
                bool goToStartSuccess = await GoToProductStartPosition(currentProductKeyword);

                if (!goToStartSuccess)
                {
                    MessageBox.Show("Başlangıç pozisyonu bulunamadı. Lütfen bir başlangıç belirleyin ve kaydedin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.Enabled = true;
                    pressedStartButton = false;
                    return;
                }

                foreach (var rectangle in rectangles)
                {
                    rectangle.BackColor = Color.White; // Rengi beyaz yapıyoruz
                }

                if (isCycloneChecked)
                {
                    // Cyclone kontrolü burada yapılabilir
                }

                Thread.Sleep(100);

                await SendPistonControlCommandAsync("PistonDown");

                LogToOutput("Start işlemi başlatıldı.");

                bool flashSuccess = false;

                // Flash işlemini 3 kez dene
                
                    flashSuccess = await SendFlashAsync(); // SendFlashAsync kullanıldı


                Invoke(new Action(() =>
                {
                    var rectangleControl = panel4.Controls[0];
                    rectangleControl.BackColor = flashSuccess ? Color.Green : Color.Red;
                }));

                // Eğer flash başarılı olmadıysa, burada piston tekrar indirilecek
                // Piston hareketi ve diğer işlemler başlatılıyor ancak thread'de yapılması sağlanıyor
                Thread thread = new Thread(() => ProcessCoordinatesFile(waitForm));
                thread.Start();

            }
            else
            {
                MessageBox.Show("Lütfen bir ürün seçin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Enabled = true;
                pressedStartButton = false;
            }
        }


        //////////////////////////////////////////////////////////////////////////////////////////



        public async Task<bool> SendFlashAsync()
        {
            void UpdateLabel(string text)
            {
                if (label6.InvokeRequired)
                {
                    label6.Invoke(new Action(() => label6.Text = text));
                }
                else
                {
                    label6.Text = text;
                }
            }

            int retryCount = 0;
            bool flashSuccessful = false;

            while (retryCount < 3 && !flashSuccessful)
            {
                if (chkcyclone.Checked)
                {
                    Thread.Sleep(5500); // Cyclone işlemi bekleniyor...

                    if (cyclone_control_api.checkCycloneExecutionStatus(handle) == 0)
                    {
                        uint getNumberOfErrors = cyclone_control_api.getNumberOfErrors(handle);
                        if (getNumberOfErrors == 0)
                        {
                            UpdateLabel("Programming was successful.");
                            LogToOutput("Programming was successful.");
                            flashSuccessful = true;
                        }
                        else
                        {
                            UpdateLabel("Error Code = " + cyclone_control_api.getErrorCode(handle, 1).ToString());
                            LogToOutput("Error Code = " + cyclone_control_api.getErrorCode(handle, 1).ToString());
                        }
                    }
                    else
                    {
                        UpdateLabel("Cyclone execution status check failed.");
                        return false;
                    }
                }
                else if (chkınfıneon.Checked)
                {
                    processOutput = "";
                    LogToOutput($"Çalıştırılan bat dosyası: {batFile}");

                    Process p = new Process();
                    p.StartInfo = new ProcessStartInfo
                    {
                        FileName = batFile,
                        WorkingDirectory = Path.GetDirectoryName(batFile),
                        CreateNoWindow = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false
                    };

                    p.OutputDataReceived += LiveOutputHandler;
                    p.ErrorDataReceived += ErrorDataReceived;

                    p.Start();
                    p.BeginOutputReadLine();
                    p.BeginErrorReadLine();

                    Thread.Sleep(6000);

                    p.Close();
                    p.Dispose();

                    if (processOutput.Contains("HATA!!!") || processOutput.Contains("Error setting BMI value"))
                    {
                        retryCount++;
                        if (retryCount < 3)
                        {
                            // Piston tekrar indir
                            await SendPistonControlCommandAsync("PistonUp");
                            await Task.Delay(1000); // Pistonun inmesi için bekle
                            await SendPistonControlCommandAsync("PistonDown");

                            continue; // Tekrar dene
                        }
                        else
                        {
                            LogToOutput("Program hatalı, 3 denemede başarısız oldu.");
                            //await SendPistonControlCommandAsync("PistonUp");

                            return false;
                        }
                    }
                    else
                    {
                        flashSuccessful = true;
                        return true;
                    }
                }

                retryCount++;
            }

            return flashSuccessful;
        }

        private void LiveOutputHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            Process p = sendingProcess as Process;
            processOutput += outLine.Data + Environment.NewLine;
        }



        private void ErrorDataReceived(object sender, DataReceivedEventArgs errorLine)
        {
            Process p = sender as Process;
            processOutput += errorLine.Data + "\r\n";

        }




        public void RemoveProductFromComboBox(string productName)
        {
            if (cmburunsecim.Items.Contains(productName))
            {
                cmburunsecim.Items.Remove(productName);
            }
        }




        private async void ProcessCoordinatesFile(frmWait waitForm)
        {
            string selectedProduct = string.Empty;
            if (cmburunsecim.InvokeRequired)
            {
                selectedProduct = (string)cmburunsecim.Invoke(new Func<string>(() => cmburunsecim.SelectedItem?.ToString()));
            }
            else
            {
                selectedProduct = cmburunsecim.SelectedItem?.ToString();
            }

            string selectedProductPrefix = selectedProduct != null && selectedProduct.Length >= 5 ? selectedProduct.Substring(0, 5) : string.Empty;

            if (string.IsNullOrEmpty(selectedProductPrefix))
            {
                MessageBox.Show("Seçilen ürün adı geçersiz.");
                pressedStartButton = false;
                return;
            }

            string koordinatlarKlasoru = "KoordinatlarListesi";
            string[] koordinatDosyalari = Directory.GetFiles(koordinatlarKlasoru, "*.txt");

            string koordinatDosyasi = koordinatDosyalari.FirstOrDefault(dosya =>
                Path.GetFileNameWithoutExtension(dosya).StartsWith(selectedProductPrefix));

            if (!string.IsNullOrEmpty(koordinatDosyasi) && File.Exists(koordinatDosyasi))
            {
                bool isHeader = true;
                bool shouldRead = true;
                int coordinateIndex = 1;
                double cumulativeX = 0;
                double cumulativeY = 0;
                int retryCount = 0;

                foreach (string line in File.ReadLines(koordinatDosyasi))
                {
                    string lineTrimmed = line.Trim();

                    if (isHeader)
                    {
                        isHeader = false;
                        continue;
                    }

                    if (lineTrimmed.StartsWith("[") && lineTrimmed.Length >= 7 &&
                        lineTrimmed.Substring(1, 5).Equals(selectedProductPrefix, StringComparison.OrdinalIgnoreCase))
                    {
                        shouldRead = true;
                        continue;
                    }

                    if (shouldRead)
                    {
                        if (lineTrimmed.StartsWith("[") && !lineTrimmed.Substring(1, 5).Equals(selectedProductPrefix, StringComparison.OrdinalIgnoreCase))
                        {
                            break;
                        }

                        string[] coordinates = lineTrimmed.Split(' ');

                        if (coordinates.Length >= 2)
                        {
                            if (double.TryParse(coordinates[0], NumberStyles.Float, CultureInfo.InvariantCulture, out double x) &&
                                double.TryParse(coordinates[1], NumberStyles.Float, CultureInfo.InvariantCulture, out double y))
                            {
                                cumulativeX += x;
                                cumulativeY += y;

                                CheckAndFixPistonState();
                                await Task.Delay(500);

                                bool pistonState = CheckAndFixPistonStateRead();

                                if (pistonState) // Piston yukarıdaysa
                                {
                                    string gcode = $"G91G0X{x.ToString(CultureInfo.InvariantCulture)}Y{y.ToString(CultureInfo.InvariantCulture)}";

                                    await Task.Delay(500);
                                    await SendPistonControlCommandAsync("PistonUp");

                                    if (!await SendGCodeWithWait(gcode))
                                        return;

                                    await Task.Delay(1000);

                                    await SendPistonControlCommandAsync("PistonDown");

                                    bool flashSuccess = await SendFlashAsync(); // SendFlashAsync kullanıldı

                                    // Koordinat için renk değişikliğini güncelle
                                    Invoke(new Action(() =>
                                    {
                                        if (coordinateIndex >= 0 && coordinateIndex < panel4.Controls.Count)
                                        {
                                            var rectangleControl = panel4.Controls[coordinateIndex];
                                            rectangleControl.BackColor = flashSuccess ? Color.Green : Color.Red;
                                        }
                                    }));

                                    // Eğer kırmızı yandı, işlemi 3 kez tekrarla
                                    if (!flashSuccess)
                                    {

                                        // Kırmızı yandığında, piston hareketlerini tekrar yap
                                        
                                        Invoke(new Action(() =>
                                        {
                                            var rectangleControl = panel4.Controls[coordinateIndex];
                                            rectangleControl.BackColor = flashSuccess ? Color.Green : Color.Red;
                                        }));

                                        if (!flashSuccess)
                                        {
                                            await SendPistonControlCommandAsync("PistonUp");

                                            MessageBox.Show("Flash başarısız oldu, ancak işlem devam ediyor.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                                            retryCount = 0; // Eğer başarılıysa, retry sayacını sıfırla
                                            coordinateIndex++; // Başarıyla tamamlanmışsa, koordinat index'ini arttır
                                        }

                                    }
                                    //else
                                    //{
                                    //    retryCount = 0; // Eğer başarılıysa, retry sayacını sıfırla
                                    //    coordinateIndex++; // Başarıyla tamamlanmışsa, koordinat index'ini arttır
                                    //}
                                }
                                else
                                {
                                    LogToOutput("Piston durumu hatalı.");
                                    MessageBox.Show("Piston hatası algılandı. İşlem durduruldu.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    pressedStartButton = false; // İşlemi durdur
                                }
                            }
                            else
                            {
                                MessageBox.Show($"Geçersiz koordinat formatı: {lineTrimmed}");
                                pressedStartButton = false;
                            }
                        }
                        else if (lineTrimmed.StartsWith("["))
                        {
                            break;
                        }
                    }
                }

                await SendPistonControlCommandAsync("PistonDown");
                double negativeCumulativeX = -cumulativeX;
                double negativeCumulativeY = -cumulativeY;
                string returnGcode = $"G91G0X{negativeCumulativeX.ToString(CultureInfo.InvariantCulture)}Y{negativeCumulativeY.ToString(CultureInfo.InvariantCulture)}";
                //SendGCode(returnGcode);

            }
            else
            {
                MessageBox.Show("Koordinat dosyası bulunamadı.");
                pressedStartButton = false;
            }

            waitForm.Invoke(new Action(() =>
            {
                waitForm.Close();
                this.Enabled = true;
            }));

            Invoke(new Action(() => LoadLastProductPositionFromFile()));
            pressedStartButton = false;
        }

        public async Task<bool> SendGCodeWithWait(string gcode)
        {
            try
            {
                if (!CheckAndFixPistonState())
                {
                    throw new Exception("Piston durumu doğrulanamadı. Lütfen kontrol ediniz. İşlem iptal edildi.");
                }

                double x = 0;
                double y = 0;

                var xMatch = Regex.Match(gcode, @"X(-?\d+([.,]\d+)?)");
                var yMatch = Regex.Match(gcode, @"Y(-?\d+([.,]\d+)?)");

                if (xMatch.Success)
                {
                    x = double.Parse(xMatch.Groups[1].Value.Replace(',', '.'), CultureInfo.InvariantCulture);
                }
                if (yMatch.Success)
                {
                    y = double.Parse(yMatch.Groups[1].Value.Replace(',', '.'), CultureInfo.InvariantCulture);
                }
                double moveLength = Math.Sqrt(x * x + y * y);
                double waitTime = CalculateWaitTime(moveLength);
                SendGCode(gcode);
                await Task.Delay((int)waitTime);

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("G kodu gönderme hatası: " + ex.Message);
                return false;
            }
        }


        public double CalculateWaitTime(double moveLength)
        {
            return moveLength * 100;
        }

        public async Task SendCycloneSelectedCommandAsync(bool isCycloneSelected)
        {
            try
            {
                string command = isCycloneSelected ? "cycloneselected:1" : "cycloneselected:0";
                portArduino.WriteLine(command);
                await Task.Delay(500);
            }
            catch (Exception ex)
            {
                LogToOutput("Cyclone seçimi hatası: " + ex.Message);
            }
        }
        public async Task SendInfineonSelectedCommandAsync(bool isInfineon)
        {


            await Task.Delay(750);


            try
            {
                string command = isInfineon ? "infineonselected:1" : "infineonselected:0";
                LogToOutput("Gönderilen komut: " + command);

                portArduino.WriteLine(command);
                await Task.Delay(500);
            }
            catch (Exception ex)
            {
                LogToOutput("Infineon seçimi hatası: " + ex.Message);
            }
        }
        public async Task SendInfineonVoltageCommandAsync(bool mcode)
        {
            try
            {
                // Ürün seçiminden ilk 5 karakteri al
                string selectedProductPrefix = cmburunsecim.SelectedItem.ToString().Substring(0, 5);

                // Dosya yolunu oluştur
                string productFilePath = Path.Combine("CycloneInfineon", $"{selectedProductPrefix}.txt");

                // Dosyanın var olup olmadığını kontrol et
                if (File.Exists(productFilePath))
                {
                    // Dosyadan Voltage satırını oku
                    string voltageLine = File.ReadLines(productFilePath).FirstOrDefault(line => line.StartsWith("Voltage:"));

                    if (voltageLine != null)
                    {
                        string voltage = voltageLine.Split(':')[1].Trim();

                        // Voltage değerine göre komut gönder
                        if (voltage == "3.3V")
                        {
                            await SendVoltageCommandAsync(true, false);
                        }
                        else if (voltage == "5V")
                        {
                            await SendVoltageCommandAsync(false, true);
                        }
                    }
                }
                else
                {
                    LogToOutput($"Dosya bulunamadı: {productFilePath}");
                }
            }
            catch (Exception ex)
            {
                LogToOutput("Infineon voltaj gönderme hatası: " + ex.Message);
            }
        }

        public async Task SendVoltageCommandAsync(bool is3V3Selected, bool is5VSelected)
        {
            try
            {
                string command3V3 = is3V3Selected ? "ucnoktaucselected:1" : "ucnoktaucselected:0";
                portArduino.WriteLine(command3V3);
                await Task.Delay(500);

                string command5V = is5VSelected ? "besselected:1" : "besselected:0";
                portArduino.WriteLine(command5V);
                await Task.Delay(500);
            }
            catch (Exception ex)
            {
                LogToOutput("Voltaj komutu gönderme hatası: " + ex.Message);
            }
        }

        public async Task SendPistonControlCommandAsync(string command)
        {
            try
            {
                portArduino.WriteLine(command);
                await Task.Delay(500);
            }
            catch (Exception ex)
            {
                LogToOutput("Piston kontrol hatası: " + ex.Message);
            }
        }
        private void SaveProductList()
        {
            string[] productNames = cmburunsecim.Items.OfType<string>().Select(name => $"[{name}]").ToArray();
        }


        ///////////////////////////////////////////////////////////////////////////////////


        private void yeniUrunEkleButton_Click(object sender, EventArgs e)
        {
            YeniUrunEkleForm yeniUrunForm = new YeniUrunEkleForm(this);
            yeniUrunForm.StartPosition = FormStartPosition.CenterParent;

            yeniUrunForm.SetYeniUrunEklendiHandler((yeniUrunAdi) =>
            {
                cmburunsecim.Items.Add(yeniUrunAdi);
                SaveProductList();
            });


            DialogResult result = yeniUrunForm.ShowDialog();
        }
        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            panel1.BackColor = System.Drawing.Color.FromArgb(255, 214, 0);
        }
        private void panel2_Paint(object sender, PaintEventArgs e)
        {
            panel2.BackColor = System.Drawing.Color.FromArgb(255, 214, 0);
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }
        private void btnUp_Click(object sender, EventArgs e)
        {

            if (!CheckAndFixPistonState())
            {
                throw new Exception("Piston durumu doğrulanamadı. Lütfen kontrol ediniz. İşlem iptal edildi.");
            }

            if (!isProductSelected)
            {
                tempYPosition += 0.1;
                SendGCode($"G91G0Y0.1");
            }
            else
            {
                currentYPosition += 0.1;
                SendGCode($"G91G0Y0.1");

                string urunAdi = cmburunsecim.SelectedItem.ToString();
                LogAndSaveProgress(urunAdi, currentXPosition, currentYPosition);
            }

        }
        private void btnRight_Click_1(object sender, EventArgs e)
        {
            if (!CheckAndFixPistonState())
            {
                throw new Exception("Piston durumu doğrulanamadı. Lütfen kontrol ediniz. İşlem iptal edildi.");
            }
            if (!isProductSelected)
            {
                tempXPosition += 0.1;
                SendGCode($"G91G0X+0.1");
            }
            else
            {
                currentXPosition += 0.1;
                SendGCode($"G91G0X+0.1");
                string urunAdi = cmburunsecim.SelectedItem.ToString();
                LogAndSaveProgress(urunAdi, currentXPosition, currentYPosition);
            }
        }
        private void btnLeft_Click_1(object sender, EventArgs e)
        {
            if (!CheckAndFixPistonState())
            {
                throw new Exception("Piston durumu doğrulanamadı. Lütfen kontrol ediniz. İşlem iptal edildi.");
            }
            if (!isProductSelected)
            {
                tempXPosition -= 0.1;
                SendGCode($"G91G0X-0.1");
            }
            else
            {
                currentXPosition -= 0.1;
                SendGCode($"G91G0X-0.1");
                string urunAdi = cmburunsecim.SelectedItem.ToString();
                LogAndSaveProgress(urunAdi, currentXPosition, currentYPosition);
            }
        }
        private void btnDown_Click_1(object sender, EventArgs e)
        {
            if (!CheckAndFixPistonState())
            {
                throw new Exception("Piston durumu doğrulanamadı. Lütfen kontrol ediniz. İşlem iptal edildi.");
            }
            if (!isProductSelected)
            {
                tempYPosition -= 0.1;
                SendGCode($"G91G0Y-0.1");
            }
            else
            {
                currentYPosition -= 0.1;
                SendGCode($"G91G0Y-0.1");
                string urunAdi = cmburunsecim.SelectedItem.ToString();
                LogAndSaveProgress(urunAdi, currentXPosition, currentYPosition);
            }
        }
        private void LogAndSaveProgress(string urunAdi, double xPosition, double yPosition)
        {
            try
            {
                if (!CheckAndFixPistonState())
                {
                    throw new Exception("Piston durumu doğrulanamadı. Lütfen kontrol ediniz. İşlem iptal edildi.");
                }
                else
                {
                    // Ürün adının ilk 5 hanesini al
                    string shortUrunAdi = urunAdi.Length >= 5 ? urunAdi.Substring(0, 5) : urunAdi;

                    // Klasör oluşturma
                    string folderPath = Path.Combine("ÜrünBaşlangıçKoordinatları", shortUrunAdi);
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    // M kodlu dosya oluşturma
                    string filePath = Path.Combine(folderPath, $"{shortUrunAdi}.txt");

                    // Konum verilerini dosyaya yaz
                    using (StreamWriter writer = new StreamWriter(filePath, true))
                    {
                        writer.WriteLine($"{shortUrunAdi} {xPosition.ToString("F3", CultureInfo.InvariantCulture)} {yPosition.ToString("F3", CultureInfo.InvariantCulture)}");
                    }
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show($"İlerleme kaydedilirken hata oluştu: {ex.Message}");
            }
        }
        private void button1_Click_1(object sender, EventArgs e)
        {
            // Retrieve the selected product name from comboBox1
            string selectedProduct = cmburunsecim.SelectedItem?.ToString();

            if (string.IsNullOrEmpty(selectedProduct) || selectedProduct.Length < 5)
            {
                MessageBox.Show("Geçerli bir ürün seçilmedi veya ürün adı yeterince uzun değil.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Get the first 5 characters of the product name
            string productPrefix = selectedProduct.Substring(0, 5);

            // Construct the path to the product's folder and text file
            string productFolderPath = Path.Combine("ÜrünBaşlangıçKoordinatları", productPrefix);
            string productFilePath = Path.Combine(productFolderPath, $"{productPrefix}.txt");

            try
            {
                // Ensure the folder exists
                if (!Directory.Exists(productFolderPath))
                {
                    Directory.CreateDirectory(productFolderPath);
                }

                // Read the existing lines from the file
                string[] lines = File.Exists(productFilePath) ? File.ReadAllLines(productFilePath) : new string[0];

                if (lines.Length > 0)
                {
                    // Get the last line
                    string lastLine = lines[lines.Length - 1];
                    string[] tokens = lastLine.Trim().Split(' ');

                    if (tokens.Length >= 3)
                    {
                        string lastX = tokens[1];
                        string lastY = tokens[2];

                        // Clear the file
                        File.WriteAllText(productFilePath, string.Empty);

                        // Write the last line back to the file
                        File.AppendAllText(productFilePath, lastLine + Environment.NewLine);

                        // Log message
                        LogToOutput($"Mevcut konum G28 olarak ayarlandı. X: {lastX}, Y: {lastY}");

                        // Show saved message
                        MessageBox.Show("Kaydedildi", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        chkkontrol.Visible = false; // Replace someControl with the actual control you want to hide
                        btnDown.Visible = false;
                        btnUp.Visible = false;
                        btnRight.Visible = false;
                        btnLeft.Visible = false;
                        dikdörtgenForm.Close(); // Close the form
                        dikdörtgenForm.Dispose();
                        SendGCode("G53X0Y0");

                    }
                    else
                    {
                        MessageBox.Show("Son satırdaki koordinatlar geçerli değil.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Dosya boş.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void SetCheckBox1Checked(bool isChecked)
        {
            chkcyclone.Checked = isChecked;
        }

        public void SetCheckBox2Checked(bool isChecked)
        {
            chkınfıneon.Checked = isChecked;
        }
        private void label1_Click(object sender, EventArgs e)
        {

        }
        private void label6_Click(object sender, EventArgs e)
        {

        }
        public void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            UInt32 image_count = 1;
            UInt32 connection_type = cyclone_control_api.CyclonePortType_USB;
            UInt32 handle = 0;
            if (chkcyclone.Checked == true)
            {
                if (portArduino.IsOpen)
                {
                    // CheckBox'ın durumuna göre komut gönder
                    SendCycloneSelectedCommandAsync(chkcyclone.Checked);

                    string arduinoResponse = portArduino.ReadLine(); // Arduino'dan gelen cevabı oku

                    // ListBox'a "Cyclone Seçildi" yazdır
                    //listBox1.Items.Add("Cyclone Seçildi: " + arduinoResponse); // Arduino cevabını ekleyebilirsiniz
                }
                else
                {
                    MessageBox.Show("Seri port açık değil.");
                }

                label6.Text = "Contacting IP1 ...";
                Application.DoEvents();
                //handle = cyclone_control_api.connectToCyclone(textBox1.Text);
                if (handle == 0)

                    label6.Text = "Error Opening Device";
                else
                {
                    image_count = cyclone_control_api.countCycloneImages(handle);
                    label6.Text = "Total Images = " + image_count.ToString();
                    for (UInt32 i = 1; i < image_count + 1; i++)
                    {
                        // Assuming `getImageDescription` returns the image name or description
                        string imageName = cyclone_control_api.getImageDescription(handle, i);
                        //comboBox3.Items.Add(imageName);
                    }

                }
            }
            else
                SendCycloneSelectedCommandAsync(chkcyclone.Checked);
        }




        private async Task MovePistonToCoordinate(string gcode, int deviceIndex)
        {
            using (Wait waitForm = new Wait())
            {
                // Wait formunu ana forma göre konumlandır ve göster
                waitForm.Location = new Point(this.Location.X, this.Location.Y);
                waitForm.Show();
                waitForm.Refresh(); // Formun güncellenmesini sağlar.


                // CNC'ye G-code'u gönder ve işlemi bekle
                if (!await SendGCodeWithWait(gcode))
                    return;

                // Geri dönüş koordinatlarını hesapla
                double negativeStartX = -startX;
                double negativeStartY = -startY;


                // ControlForm'u başlat ve göster
                controlForm = new ControlForm(this, startX, startY, SendGCodeWithWait, deviceIndex);

                waitForm.Close(); // ControlForm açılmadan önce Wait formunu kapat

                controlForm.Show();

                this.Enabled = false; // Ana formu devre dışı bırak
            }

            chkhatadurumu.Checked = false; // CheckBox tikini kaldır

            cmbhatadurumu.SelectedIndex = -1;
        }



        private void pictureBox6_Click(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (pistonIsUp)
            {
                button2.PerformClick();
            }
        }




        private void WriteLog(string logMessage)
        {
            // Log dosyasının adı (Günlük tarihine göre isimlendirilmiş olabilir)
            string logFilePath = Path.Combine(_logDirectoryPath, "log_" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt");

            // Log dosyasına işlem zamanını ve mesajı yaz
            using (StreamWriter writer = new StreamWriter(logFilePath, true))
            {
                writer.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} - {logMessage}");
            }
        }

        private string GetProductNameForMCode(string mcode, HashSet<string> validProductNames)
        {
            // Burada geçerli ürün adını bulmak için mcode'ye göre arama yapılır
            // Dummy implementasyon, gerçek iş mantığını buraya ekleyin

            // Örneğin, ürün adları bir dosya veya veritabanından alınabilir.
            // Bu örnekte, sadece mcode'ye uygun geçerli bir ürün adı bulunup bulunmadığını kontrol ediyoruz.

            foreach (string productName in validProductNames)
            {
                if (productName.StartsWith(mcode))
                {
                    return productName;
                }
            }

            return null;
        }
        private void koordinat_Load(object sender, EventArgs e)
        {
            if (!Directory.Exists("Logs"))
            {
                _logDirectoryPath = Directory.CreateDirectory("Logs").FullName;
            }
            else
            {
                _logDirectoryPath = Application.StartupPath + "\\Logs";
            }
            portArduino.DataReceived += PortArduino_DataReceived;  // Veri alındığında tetiklenecek event handler
            LoadComboBoxItems();
            btnDown.Visible = false;
            btnLeft.Visible = false;
            btnUp.Visible = false;
            btnRight.Visible = false;
            chkkontrol.Visible = false;


            cmbhatadurumu.Enabled = false;
            //var result = MessageBox.Show("Başlangıç noktasına dönmek ister misiniz?", "Uyarı", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            //if (result == DialogResult.Yes)
            //{
            //    // Eğer kullanıcı 'Evet' dediyse, başlangıç noktasına dönmek için G53 komutunu gönder
            //    string gcode = "G53 X0 Y0";
            //    SendGCode(gcode);
            //}
        }





        private void button6_Click(object sender, EventArgs e)
        {
            if (portArduino.IsOpen)
            {
                try
                {
                    portArduino.WriteLine("TogglePiston");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Seri port açık değil.");
            }
        }


        public void StopTest()
        {

            char stopCharacter = (char)0x18;

            if (isRunning)
            {
                serialPort.WriteLine(stopCharacter.ToString());
                portArduino.Close();
                Thread.Sleep(1000);
            }
            else
            {
            }

            isRunning = !isRunning;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (emergencyActive)
            {
                MessageBox.Show("Emergency stop is active. The operation cannot be started or stopped.");
                return;
            }

            char stopCharacter = (char)0x18;
            serialPort.WriteLine(stopCharacter.ToString());
            serialPort.Close();

            portArduino.Close();
            Thread.Sleep(1000);

            //if (isRunning)
            //{

            //    serialPort.WriteLine(stopCharacter.ToString());
            //    serialPort.Close();
            //    portArduino.Close();
            //    Thread.Sleep(1000);


            //}
            //else
            //{


            //}

            //isRunning = !isRunning;

        }

        //private string HexToAsciiSymbol(string hexString)
        //{
        //    // Hexadecimal stringi ASCII sembolüne dönüştürme
        //    int charValue = Convert.ToInt32(hexString, 16);
        //    return ((char)charValue).ToString();
        //}




        private void button9_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;

        }

        private void button8_Click(object sender, EventArgs e)
        {

            Close();
        }

        private void checkBoxRetryCoordinate_CheckedChanged(object sender, EventArgs e)
        {
            int sevo;
            if (chkhatadurumu.Checked)
            {
                cmbhatadurumu.Visible = true;
                cmbhatadurumu.Enabled = true;
                cmbhatadurumu.BringToFront();
                btnStart.Enabled = false;
                btnbaslangıckaydet.Enabled = false;
                btnyeniurunekle.Enabled = false;
                cmburunsecim.Enabled = false;
                btnpiston.Enabled = false;
                btnhome.Enabled = false;
                LogToOutput("Hata durumu aktif edildi.");


            }
            else
            {
                cmbhatadurumu.Visible = false;
                cmbhatadurumu.Enabled = false;
                btnStart.Enabled = true;
                btnbaslangıckaydet.Enabled = true;
                btnyeniurunekle.Enabled = true;
                cmburunsecim.Enabled = true;
                btnpiston.Enabled = true;
                btnhome.Enabled = true;
                LogToOutput("Hata durumu pasif edildi.");

            }

        }




        private void ConnectArduino()
        {
            try
            {
                if (!portArduino.IsOpen)
                {
                    portArduino.BaudRate = 115200;

                    portArduino.Open();
                    LogToOutput("Arduino bağlantısı başarılı.");
                }
            }
            catch (Exception ex)
            {
                LogToOutput("Arduino bağlantısı başarısız.");
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }


        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (portArduino.IsOpen)
            {
                portArduino.Close();
            }
            if (readTimer != null)
            {
                readTimer.Stop();
                readTimer.Dispose();
            }

            Process.GetCurrentProcess().Kill();
        }

        private void chkkalibrasyon_CheckedChanged(object sender, EventArgs e)
        {
            //// Check if the checkbox is checked
            //if (chkkalibrasyon.Checked)
            //{
            //    // Checkbox işaretlendiğinde düğmeleri göster
            //    btnUp.Visible = true;
            //    btnDown.Visible = true;
            //    btnLeft.Visible = true;
            //    btnRight.Visible = true;
            //    chkkontrol.Visible = true;
            //}
            //else
            //{
            //    // Checkbox kaldırıldığında düğmeleri gizle
            //    btnUp.Visible = false;
            //    btnDown.Visible = false;
            //    btnLeft.Visible = false;
            //    btnRight.Visible = false;
            //    chkkontrol.Visible = false;
            //}
        }

        private void button1_Click_2(object sender, EventArgs e)
        {
            readTimer.Start();
            UInt32 connection_type = cyclone_control_api.CyclonePortType_USB;
            UInt32 handle = 0;
            label6.Text = "";
            if (chkcyclone.Checked == true)
            {
                label6.Text = "Contacting IP1 ... ";
                Application.DoEvents();
                //handle = cyclone_control_api.connectToCyclone(textBox1.Text);
                if (handle == 0)
                {

                    label6.ForeColor = Color.Red;
                    label6.Font = new Font(label6.Font.FontFamily, 14, FontStyle.Bold);
                    label6.Text = "Error Opening Device";
                }
                else
                {
                    label6.Text = cyclone_control_api.getPropertyValue(handle, 0, cyclone_control_api.CycloneProperties, cyclone_control_api.selectCyclonePropertyName);
                }
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            readTimer.Start();
            UInt32 connection_type = cyclone_control_api.CyclonePortType_USB;
            bool cyclone1done = false;
            bool cyclone1enabled = chkcyclone.Checked;
            label6.Text = "";
            if (cyclone1enabled == true)
            {
                label6.Text = "";
                //handle = cyclone_control_api.connectToCyclone(textBox1.Text);
                if (handle == 0)
                {
                    label6.ForeColor = Color.Red;
                    label6.Font = new Font(label6.Font.FontFamily, 14, FontStyle.Bold);
                    label6.Text = "Error Opening Device";
                    cyclone1enabled = false;
                }
                else
                {
                    label6.Text = "Programming Image on IP1 ... ";
                    //    cyclone_control_api.startImageExecution(handle, Convert.ToByte(Edit1.Text));
                }
            }


            Application.DoEvents();

            do
            {
                if (cyclone1enabled == true)
                {
                    if (cyclone_control_api.checkCycloneExecutionStatus(handle) == 0)
                    {
                        if (cyclone_control_api.getNumberOfErrors(handle) == 0)
                        {
                            label6.Text = "Programming was successful.";
                        }
                        else
                        {
                            label6.ForeColor = Color.Red;
                            label6.Font = new Font(label6.Font.FontFamily, 14, FontStyle.Bold);
                            label6.Text = "Error Code = " + cyclone_control_api.getErrorCode(handle, 1).ToString();
                        }
                        cyclone1done = true;
                    }
                }
                else cyclone1done = true;
            } while (!(cyclone1done));
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            UInt32 connection_type = cyclone_control_api.CyclonePortType_USB;
            UInt32 handle = 0;
            bool cyclone1done = false;
            bool cyclone1enabled = chkcyclone.Checked;
            label6.Text = "";
            if (cyclone1enabled == true)
            {
                label6.Text = "";
                //handle = cyclone_control_api.connectToCyclone(textBox1.Text);
                if (handle == 0)
                {
                    label6.ForeColor = Color.Red;
                    label6.Font = new Font(label6.Font.FontFamily, 14, FontStyle.Bold);
                    cyclone1enabled = false;
                }
                else
                {
                    label6.Text = "Programming Image on IP1 ... ";
                    //cyclone_control_api.startImageExecution(handle, Convert.ToByte(Edit1.Text));
                }
            }
        }

        private void btnhome_Click(object sender, EventArgs e)
        {
            if (!CheckAndFixPistonState())
            {
                MessageBox.Show("Piston durumu doğrulanamadı. Lütfen kontrol ediniz. İşlem iptal edildi.");
                //portArduino.WriteLine("TogglePiston");

            }
            SendGCode("G53X0Y0");
            cmburunsecim.SelectedIndex = -1; // Clear the selected index
            chkcyclone.Checked = false;
            chkınfıneon.Checked = false;

            // Check if dikdörtgenForm is not null and still open
            if (dikdörtgenForm != null && !dikdörtgenForm.IsDisposed)
            {
                dikdörtgenForm.Close(); // Close the form if it's open
            }

            cmbhatadurumu.Items.Clear();
            // **Şevo burayı ben düzelttim (Berkcho)**
        }



        private void chkınfıneon_CheckedChanged(object sender, EventArgs e)
        {
           
            if (chkınfıneon.Checked)
            {
                if (portArduino.IsOpen)
                {
                    // CheckBox'ın durumuna göre komut gönder
                    SendInfineonSelectedCommandAsync(chkınfıneon.Checked);
                    SendInfineonVoltageCommandAsync(chkınfıneon.Checked);

                    // ListBox'a "Cyclone Seçildi" yazdır
                    //listBox1.Items.Add("Infineon Seçildi: " + arduinoResponse); // Arduino cevabını ekleyebilirsiniz
                    LogToOutput("Infineon Seçildi: \" + arduinoResponse");
                }
                else
                {
                    MessageBox.Show("Seri port açık değil.");
                }
                batFile = "";
                string[] subDirectories = Directory.GetDirectories(Application.StartupPath + "//Programs");

                // currentProductKeyword'dan ilk 5 karakteri alıyoruz
                string mcode = currentProductKeyword.Substring(0, Math.Min(5, currentProductKeyword.Length)); // Eğer currentProductKeyword 5 karakterden kısa ise

                string dir = subDirectories.Where(d => d.Contains(mcode)).FirstOrDefault();

                if (string.IsNullOrEmpty(dir))
                {
                    MessageBox.Show("İlgili klasör bulunamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    LogToOutput("İlgili klasör bulunamadı.");

                    return;
                }

                string[] files = Directory.GetFiles(dir);
                batFile = files.Where(f => f.EndsWith(".bat")).FirstOrDefault();

                if (string.IsNullOrEmpty(batFile))
                {
                    MessageBox.Show("Bat dosyası bulunamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                LogToOutput($"Bat dosyası bulundu ve seçildi: {batFile}");

            }
            else

                SendInfineonSelectedCommandAsync(chkınfıneon.Checked);

        }

        private async void cmbhatadurumu_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!chkhatadurumu.Checked)
            {
                return;
            }

            if (cmbhatadurumu.SelectedItem == null)
            {
                return;
            }

            pressedStartButton = true;

            await Task.Delay(500);

            string selectedCoordinate = cmbhatadurumu.SelectedItem.ToString();
            string[] parts = selectedCoordinate.Split(new char[] { ' ', ':' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length < 4 || !parts[0].StartsWith("Nokta"))
            {
                MessageBox.Show("Koordinat formatı hatalı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }





            if (!double.TryParse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture, out double x) ||
                !double.TryParse(parts[3], NumberStyles.Float, CultureInfo.InvariantCulture, out double y))
            {
                MessageBox.Show("Koordinatlar sayısal değil.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            startX = x;
            startY = y;

            string gcode = $"G91G0X{x.ToString(CultureInfo.InvariantCulture)}Y{y.ToString(CultureInfo.InvariantCulture)}";

            await MovePistonToCoordinate(gcode, Convert.ToInt32(parts[1]) - 1);

            pressedStartButton = false;
        }

        private void cmburunsecim_KeyPress_1(object sender, KeyPressEventArgs e)
        {

            if (e.KeyChar == '-')
            {
                // Olayın işlenmesini durdur ve '-' karakterinin girilmesini engelle
                e.Handled = true;
            }
        }

        private void btnUp_Click_1(object sender, EventArgs e)
        {
            if (!CheckAndFixPistonState())
            {
                throw new Exception("Piston durumu doğrulanamadı. Lütfen kontrol ediniz. İşlem iptal edildi.");
            }

            if (!isProductSelected)
            {
                tempYPosition += 0.1;
                SendGCode($"G91G0Y0.1");
            }
            else
            {
                currentYPosition += 0.1;
                SendGCode($"G91G0Y0.1");

                string urunAdi = cmburunsecim.SelectedItem.ToString();
                LogAndSaveProgress(urunAdi, currentXPosition, currentYPosition);
            }
        }

        private void btnRight_Click(object sender, EventArgs e)
        {
            if (!CheckAndFixPistonState())
            {
                throw new Exception("Piston durumu doğrulanamadı. Lütfen kontrol ediniz. İşlem iptal edildi.");
            }
            if (!isProductSelected)
            {
                tempXPosition = 0.1;
                SendGCode($"G91G0X0.1");
            }
            else
            {
                currentXPosition = 0.1;
                SendGCode($"G91G0X0.1");
                string urunAdi = cmburunsecim.SelectedItem.ToString();
                LogAndSaveProgress(urunAdi, currentXPosition, currentYPosition);
            }
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            if (!CheckAndFixPistonState())
            {
                throw new Exception("Piston durumu doğrulanamadı. Lütfen kontrol ediniz. İşlem iptal edildi.");
            }
            if (!isProductSelected)
            {
                tempYPosition -= 0.1;
                SendGCode($"G91G0Y-0.1");
            }
            else
            {
                currentYPosition -= 0.1;
                SendGCode($"G91G0Y-0.1");
                string urunAdi = cmburunsecim.SelectedItem.ToString();
                LogAndSaveProgress(urunAdi, currentXPosition, currentYPosition);
            }
        }

        private void btnLeft_Click(object sender, EventArgs e)
        {
            if (!CheckAndFixPistonState())
            {
                throw new Exception("Piston durumu doğrulanamadı. Lütfen kontrol ediniz. İşlem iptal edildi.");
            }
            if (!isProductSelected)
            {
                tempXPosition -= 0.1;
                SendGCode($"G91G0X-0.1");
            }
            else
            {
                currentXPosition -= 0.1;
                SendGCode($"G91G0X-0.1");
                string urunAdi = cmburunsecim.SelectedItem.ToString();
                LogAndSaveProgress(urunAdi, currentXPosition, currentYPosition);
            }
        }

        private void btnpiston_Click(object sender, EventArgs e)
        {
            if (portArduino.IsOpen)
            {
                try
                {
                    portArduino.WriteLine("TogglePiston");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Seri port açık değil.");
            }
        }

        private void rxtLog_TextChanged(object sender, EventArgs e)
        {

        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void CreateRectanglesBasedOnProductSize()
        {
            string kartBoyutuKlasorYolu = "KartBoyutu";
            int rows = 0;
            int columns = 0;

            // Ürünün MCode'unun ilk 5 karakterini al (örn. M0088)
            string productPrefix = currentProductKeyword.Substring(0, 5);

            // MCode'a göre dosya adı belirle (ilk 5 karakteri kullanarak)
            string kartBoyutuDosyaYolu = Path.Combine(kartBoyutuKlasorYolu, $"{productPrefix}.txt");

            if (File.Exists(kartBoyutuDosyaYolu))
            {
                // Dosyanın içeriğini oku
                string[] kartBoyutuLines = File.ReadAllLines(kartBoyutuDosyaYolu);

                // İlk satırı al ve boyutları ayrıştır (örneğin: M0088: 2x3)
                if (kartBoyutuLines.Length > 0)
                {
                    string firstLine = kartBoyutuLines[0]; // İlk satır örneği: "M0088: 2x3"

                    // Boyutları ayrıştır
                    string[] parts = firstLine.Split(':')[1].Trim().Split('x');
                    if (parts.Length == 2)
                    {
                        // Boyutları integer olarak ayarla
                        if (int.TryParse(parts[0], out rows) && int.TryParse(parts[1], out columns))
                        {
                            // Başarıyla okunan boyutlar burada kullanılabilir
                            Console.WriteLine($"Rows: {rows}, Columns: {columns}");

                            // Kart boyutlarına göre dikdörtgenleri oluştur
                            CreateRectangles(rows, columns);
                        }
                    }
                }
            }
            else
            {
                // Dosya bulunamadıysa hata işleme yapılabilir
                Console.WriteLine($"Dosya bulunamadı: {kartBoyutuDosyaYolu}");
            }
        }

        private void CreateRectangles(int rows, int columns)
        {
            // Panelin boyutları
            int panelWidth = panel4.Width;
            int panelHeight = panel4.Height;

            // Her dikdörtgenin boyutları
            int rectangleWidth = panelWidth / columns;
            int rectangleHeight = panelHeight / rows;

            // Paneli temizle
            panel4.Controls.Clear();

            // Dikdörtgenlerin konumunu hesapla ve oluştur
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    // Her bir dikdörtgen için panel oluştur
                    Panel rectangle = new Panel
                    {
                        Location = new Point(j * rectangleWidth, i * rectangleHeight),
                        Size = new Size(rectangleWidth, rectangleHeight),
                        BackColor = Color.White,
                        BorderStyle = BorderStyle.FixedSingle
                    };

                    // Panelin üzerine bir label ekleyelim (isteğe bağlı)
                    System.Windows.Forms.Label label = new System.Windows.Forms.Label
                    {
                        Text = $"{i * columns + j + 1}", // Dikdörtgen numarası
                        Dock = DockStyle.Fill,
                        TextAlign = ContentAlignment.MiddleCenter,
                        Font = new Font("Arial", 12, FontStyle.Bold),
                        ForeColor = Color.Black // Label yazı rengi siyah
                    };

                    rectangle.Controls.Add(label);

                    // Paneli ana panelin içine ekle
                    panel4.Controls.Add(rectangle);
                }
            }
        }

        private async void btnkalibrasyon_Click(object sender, EventArgs e)
        {
            try
            {
                // Kalibrasyon işlemini başlat
                isCalibrationActive = true;


            }
            catch (Exception ex)
            {
                LogToOutput($"Kalibrasyon hatası: {ex.Message}");
            }
        }

        private void chkcyclone_CheckedChanged(object sender, EventArgs e)
        {
            UInt32 image_count = 1;
            UInt32 connection_type = cyclone_control_api.CyclonePortType_USB;
            UInt32 handle = 0;
            if (chkcyclone.Checked == true)
            {
                if (portArduino.IsOpen)
                {
                    // CheckBox'ın durumuna göre komut gönder
                    SendCycloneSelectedCommandAsync(chkcyclone.Checked);

                    string arduinoResponse = portArduino.ReadLine(); // Arduino'dan gelen cevabı oku

                    // ListBox'a "Cyclone Seçildi" yazdır
                    //listBox1.Items.Add("Cyclone Seçildi: " + arduinoResponse); // Arduino cevabını ekleyebilirsiniz
                }
                else
                {
                    MessageBox.Show("Seri port açık değil.");
                }

                label6.Text = "Contacting IP1 ...";
                Application.DoEvents();
                //handle = cyclone_control_api.connectToCyclone(textBox1.Text);
                if (handle == 0)

                    label6.Text = "Error Opening Device";
                else
                {
                    image_count = cyclone_control_api.countCycloneImages(handle);
                    label6.Text = "Total Images = " + image_count.ToString();
                    for (UInt32 i = 1; i < image_count + 1; i++)
                    {
                        // Assuming `getImageDescription` returns the image name or description
                        string imageName = cyclone_control_api.getImageDescription(handle, i);
                        //comboBox3.Items.Add(imageName);
                    }

                }
            }
            else
                SendCycloneSelectedCommandAsync(chkcyclone.Checked);
        }

        private async void btnstop_Click(object sender, EventArgs e)
        {
            if (emergencyActive)
            {
                MessageBox.Show("Emergency stop is active. The operation cannot be started or stopped.");
                return;
            }

            char stopCharacter = (char)0x18;
            serialPort.WriteLine(stopCharacter.ToString());
            serialPort.Close();

            await Task.Delay(500);

            serialPort.WriteLine("$X");

            portArduino.Close();
            Thread.Sleep(1000);
        }


        //private void DisplayImage(UInt32 handle, string imageName)
        //{
        //    try
        //    {
        //        // Assuming `getImageData` fetches the image data

        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"Error displaying image: {ex.Message}");
        //    }
        //}
    }
}


    