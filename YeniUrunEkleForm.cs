using System;
using System.Windows.Forms;
using System.IO;
using System.Globalization;
using static System.Windows.Forms.DataFormats;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Reflection.Metadata;
using ProgramAtmaAparatı;
using ProgramAtmaAparatı.Properties;
using ProgramAtmaAparatı;
//using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;
using System.Reflection.Emit;

namespace ProgramAtmaAparatı
{
    public partial class YeniUrunEkleForm : Form
    {

        private Form1 _mainForm; // Ana formun referansı
        private string cycloneName;
        UInt32 handle = 0;


        public YeniUrunEkleForm(Form1 mainForm)
        {
            Ini.Net.IniFile settings = new Ini.Net.IniFile("Settings.ini");
            InitializeComponent();
            _mainForm = mainForm;
            InitializeCheckBoxes();
            cycloneName = settings.ReadString("Settings", "CycloneName");
            cyclone_control_api.enumerateAllPorts();
            handle = cyclone_control_api.connectToCyclone(cycloneName);
            var filename = cyclone_control_api.DLL_FILENAME;
            cyclone_control_api.enumerateAllPorts();
            var dsds = cyclone_control_api.queryNumberOfAutodetectedCyclones();
            cyclone_control_api.enumerateAllPorts();
            handle = cyclone_control_api.connectToCyclone(cycloneName);

            //textBox1.Text = "Universal_PEMC95119";
        }
        private void InitializeCheckBoxes()
        {
            // Set event handlers
            chkCyloneSecim.CheckedChanged += CheckBox_CheckedChanged;
            chkInfıneonSecim.CheckedChanged += CheckBox_CheckedChanged;

            // Set ComboBox visibility based on CheckBox1
            cmbCyclone.Visible = chkCyloneSecim.Checked;
            cmb3v5.Visible = chkInfıneonSecim.Checked;

        }
        private void CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox changedCheckBox = sender as CheckBox;

            if (changedCheckBox != null)
            {
                // Eğer checkBox1 işaretlendiyse, checkBox2'yi temizle ve comboBox1'i göster, comboBox2'yi gizle
                if (changedCheckBox == chkCyloneSecim && chkCyloneSecim.Checked)
                {
                    chkInfıneonSecim.Checked = false;
                    cmbCyclone.Visible = true;
                    cmb3v5.Visible = false;
                }
                // Eğer checkBox2 işaretlendiyse, checkBox1'i temizle ve comboBox2'yi göster, comboBox1'i gizle
                else if (changedCheckBox == chkInfıneonSecim && chkInfıneonSecim.Checked)
                {
                    chkCyloneSecim.Checked = false;
                    cmbCyclone.Visible = false;
                    cmb3v5.Visible = true;
                }
                // Eğer her iki checkbox da işaretsiz hale geldiyse, her iki ComboBox'ı da gizle
                else if (!chkCyloneSecim.Checked && !chkInfıneonSecim.Checked)
                {
                    cmbCyclone.Visible = false;
                    cmb3v5.Visible = false;
                }
            }
        }

        private void YeniUrunEkleForm_Load(object sender, EventArgs e)
        {

        }

        private void btnKaydet_Click(object sender, EventArgs e)
        {

            string urunAdi = txtUrunAdi.Text;
            string mcode = txtMcode.Text;

            double xIlerleme = double.Parse(txtXilerleme.Text) / 2; // x değerini yarıya böl
            double yIlerleme = double.Parse(txtYilerleme.Text) / 2; // y değerini yarıya böl

            int satirSayisi = int.Parse(txtSatir.Text);
            int sutunSayisi = int.Parse(txtSütun.Text);

            if (cmbCyclone.Visible)
            {
                string selectedItem = cmbCyclone.SelectedItem?.ToString();
                // Save the selected ComboBox value as needed
            }
            if (mcode.Length < 5)
            {
                MessageBox.Show("M kodu hatası. Lütfen M kodunu kontrol ediniz."); // Şevo burayı da ben düzelttim (Berkcho) **
                return;
            }

            if (string.IsNullOrEmpty(urunAdi) || string.IsNullOrEmpty(mcode) || satirSayisi <= 0 || sutunSayisi <= 0 || xIlerleme == 0 || yIlerleme == 0)
            {
                MessageBox.Show("Lütfen tüm alanları doldurun ve x ve y ilerleme değerlerini sıfırdan farklı girin.");
                return;
            }

            // Ana klasör oluştur: "Koordinatlar"
            string coordinatesMainFolder = Path.Combine("KoordinatlarListesi");

            // Eğer ana klasör yoksa oluştur
            if (!Directory.Exists(coordinatesMainFolder))
            {
                Directory.CreateDirectory(coordinatesMainFolder);
            }

            // Ürün koordinatlarını kaydetmek için dosya oluştur
            string coordinatesFilePath = Path.Combine(coordinatesMainFolder, $"{mcode}_{urunAdi}.txt");

            using (StreamWriter writer = new StreamWriter(coordinatesFilePath))
            {
                writer.WriteLine($"[{mcode}_{urunAdi}]");

                for (int i = 0; i < satirSayisi; i++)
                {
                    for (int j = 0; j < sutunSayisi - 1; j++) // x art arda sutunSayisi kez tekrar edecek
                    {
                        writer.WriteLine($"{(-xIlerleme).ToString(CultureInfo.InvariantCulture)} 0");
                    }

                    if (i != satirSayisi - 1) // Son satırın sonunda yIlerleme değerini yazdırmamak için kontrol ekledik
                    {
                        writer.WriteLine($"0 {yIlerleme.ToString(CultureInfo.InvariantCulture)}");
                    }

                    // xIlerleme değerinin işaretini değiştir
                    xIlerleme = +xIlerleme; // Negatif yapma işlemini düzeltmek için
                }
            }

            // Yeni ürün adını ve mcode'u birleştirip dosyaya ekle
            string urunKey = $"{mcode}_{urunAdi}";

            // Yeni ürünleri dosyaya kaydet
            string yeniUrunFileName = "yeniürün.txt";
            using (StreamWriter writer = File.AppendText(yeniUrunFileName))
            {
                writer.WriteLine(urunKey);
            }

            //// Kart boyutları dosyasına ekle
            //string kartBoyutlariFileName = "KartBoyutu.txt";
            //using (StreamWriter writer = File.AppendText(kartBoyutlariFileName))
            //{
            //    writer.WriteLine($"{mcode}: {satirSayisi}x{sutunSayisi}");
            //}

            // Ana klasör ve ürün klasörünü oluştur

            string mainFolderPath1 = Path.Combine("KartBoyutu");

            // Eğer ana klasör yoksa oluştur
            if (!Directory.Exists(mainFolderPath1))
            {
                Directory.CreateDirectory(mainFolderPath1);
            }

            // txt dosyasının yolunu belirle
            string txtFilePath1 = Path.Combine(mainFolderPath1, $"{mcode}.txt");

            using (StreamWriter writer = new StreamWriter(txtFilePath1))
            {
                writer.WriteLine($"{mcode}: {satirSayisi}x{sutunSayisi}");

            }




            //////
            string mainFolderPath = Path.Combine("CycloneInfineon");

            // Eğer ana klasör yoksa oluştur
            if (!Directory.Exists(mainFolderPath))
            {
                Directory.CreateDirectory(mainFolderPath);
            }

            // txt dosyasının yolunu belirle
            string txtFilePath = Path.Combine(mainFolderPath, $"{mcode}.txt");

            using (StreamWriter writer = new StreamWriter(txtFilePath))
            {
                writer.WriteLine($"MCode:{mcode}");

                if (chkCyloneSecim.Checked)
                {
                    writer.WriteLine("Programmer:Cyclone");

                    if (cmbCyclone.SelectedItem != null)
                    {
                        writer.WriteLine($"Program: {cmbCyclone.SelectedItem.ToString()}");
                        // Program sırası için başlık ekle
                        writer.WriteLine($"Program Sırası: {cmbCyclone.SelectedIndex + 1}");
                    }
                }
                else if (chkInfıneonSecim.Checked)
                {
                    writer.WriteLine("Programmer:Infineon");

                    // Eğer Infineon seçildiyse, combobox2'deki seçilen voltajı da kaydet
                    if (cmb3v5.SelectedItem != null)
                    {
                        writer.WriteLine($"Voltage: {cmb3v5.SelectedItem.ToString()}");
                    }
                }

                MessageBox.Show("Ürün başarıyla kaydedildi.");
            }
            string yeniUrunAdi = txtUrunAdi.Text;
            string yeniMCode = txtMcode.Text;
            string yeniMCode_yeniUrunAdi = $"{yeniMCode}_{yeniUrunAdi}";

            yeniUrunEklendiHandler?.Invoke(yeniMCode_yeniUrunAdi);

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void txtYilerleme_TextChanged(object sender, EventArgs e)
        {
            System.Windows.Forms.TextBox tb = sender as System.Windows.Forms.TextBox;
            if (tb != null)
            {
                // Geçerli metni al
                string text = tb.Text;

                // Yeni bir StringBuilder nesnesi oluştur
                var result = new System.Text.StringBuilder();

                // Karakterleri tek tek kontrol et
                foreach (char c in text)
                {
                    // Nokta karakterini virgül ile değiştir
                    if (c == '.')
                    {
                        result.Append(',');
                    }
                    else
                    {
                        // Diğer tüm karakterleri olduğu gibi ekle
                        result.Append(c);
                    }
                }

                // Metni TextBox'a yeniden ata
                // TextBox'un TextChanged olayını geçici olarak devre dışı bırak
                tb.TextChanged -= txtYilerleme_TextChanged;
                tb.Text = result.ToString();
                tb.TextChanged += txtYilerleme_TextChanged;
                // İmleci metnin sonuna getir
                tb.SelectionStart = tb.Text.Length;
            }
        }

        private void txtUrunAdi_TextChanged(object sender, EventArgs e)
        {
            System.Windows.Forms.TextBox tb = sender as System.Windows.Forms.TextBox;
            if (tb != null)
            {
                // Geçerli metni al
                string text = tb.Text;
                // Küçük harfleri büyük harflere dönüştür
                text = text.ToUpper();

                // Geçerli metni `_` ile değiştirmek için karakter dizisi
                text = text.Replace(' ', '_'); // Boşluğu _ ile değiştir
                text = text.Replace('-', '_'); // - karakterini _ ile değiştir
                text = text.Replace('.', '_'); // . karakterini _ ile değiştir
                text = text.Replace('(', '_'); // ( karakterini _ ile değiştir
                text = text.Replace(')', '_'); // ) karakterini _ ile değiştir
                text = text.Replace('/', '_'); // / karakterini _ ile değiştir
                text = text.Replace('=', '_'); // = karakterini _ ile değiştir
                text = text.Replace(',', '_'); // , karakterini _ ile değiştir
                text = text.Replace('!', '_'); // ! karakterini _ ile değiştir

                // Metni TextBox'a yeniden ata
                tb.Text = text;
                // TextChanged olayının yeniden tetiklenmesini engelle
                tb.SelectionStart = tb.Text.Length; // İmleci metnin sonuna getir
                                                    // TextChanged olayının yeniden tetiklenmesini engelle
                tb.TextChanged -= txtUrunAdi_TextChanged;
                tb.Text = text;
                tb.TextChanged += txtUrunAdi_TextChanged;
            }
        }

        private void txtSatir_TextChanged(object sender, EventArgs e)
        {
            System.Windows.Forms.TextBox tb = sender as System.Windows.Forms.TextBox;
            if (tb != null)
            {
                // TextBox'un mevcut metnini al
                string text = tb.Text;

                // Yalnızca tek haneli rakamları içeren yeni bir StringBuilder oluştur
                var result = new System.Text.StringBuilder();

                // Karakterleri tek tek kontrol et
                foreach (char c in text)
                {
                    // Eğer karakter bir rakam (0-9) ise ve tek haneli ise
                    if (char.IsDigit(c) && text.Length <= 1)
                    {
                        result.Append(c);
                    }
                }

                // Yeni metni TextBox'a ata
                tb.Text = result.ToString();
                tb.SelectionStart = tb.Text.Length; // İmleci metnin sonuna getir

                // Olayın yeniden tetiklenmesini engellemek için olay işleyicisinin çalışmasını engelle
                tb.TextChanged -= txtSatir_TextChanged;
                tb.Text = result.ToString();
                tb.TextChanged += txtSatir_TextChanged;
            }
        }

        private void txtXilerleme_TextChanged(object sender, EventArgs e)
        {
            System.Windows.Forms.TextBox tb = sender as System.Windows.Forms.TextBox;
            if (tb != null)
            {
                // Geçerli metni al
                string text = tb.Text;

                // Yeni bir StringBuilder nesnesi oluştur
                var result = new System.Text.StringBuilder();

                // Karakterleri tek tek kontrol et
                foreach (char c in text)
                {
                    // Nokta karakterini virgül ile değiştir
                    if (c == '.')
                    {
                        result.Append(',');
                    }
                    else
                    {
                        // Diğer tüm karakterleri olduğu gibi ekle
                        result.Append(c);
                    }
                }

                // Metni TextBox'a yeniden ata
                // TextBox'un TextChanged olayını geçici olarak devre dışı bırak
                tb.TextChanged -= txtXilerleme_TextChanged;
                tb.Text = result.ToString();
                tb.TextChanged += txtXilerleme_TextChanged;

                // İmleci metnin sonuna getir
                tb.SelectionStart = tb.Text.Length;
            }
        }

        private void txtSütun_TextChanged(object sender, EventArgs e)
        {
            System.Windows.Forms.TextBox tb = sender as System.Windows.Forms.TextBox;
            if (tb != null)
            {
                // TextBox'un mevcut metnini al
                string text = tb.Text;

                // Yalnızca tek haneli rakamları içeren yeni bir StringBuilder oluştur
                var result = new System.Text.StringBuilder();

                // Karakterleri tek tek kontrol et
                foreach (char c in text)
                {
                    // Eğer karakter bir rakam (0-9) ise ve tek haneli ise
                    if (char.IsDigit(c) && text.Length <= 1)
                    {
                        result.Append(c);
                    }
                }

                // Yeni metni TextBox'a ata
                tb.Text = result.ToString();
                tb.SelectionStart = tb.Text.Length; // İmleci metnin sonuna getir

                // Olayın yeniden tetiklenmesini engellemek için olay işleyicisinin çalışmasını engelle
                tb.TextChanged -= txtSütun_TextChanged;
                tb.Text = result.ToString();
                tb.TextChanged += txtSütun_TextChanged;
            }
        }

        private void txtMcode_TextChanged(object sender, EventArgs e)
        {
            System.Windows.Forms.TextBox tb = sender as System.Windows.Forms.TextBox;
            if (tb != null)
            {
                // Geçerli metni al
                string text = tb.Text;

                // Temizlenmiş metni oluştur
                string result = string.Empty;

                // Eğer metin 'M' ile başlıyorsa ve toplamda beş karakter olacaksa
                if (text.Length > 0)
                {
                    // İlk karakteri kontrol et ve 'm' varsa 'M' olarak değiştir
                    if (text[0] == 'm' || text[0] == 'M')
                    {
                        result += 'M'; // 'M' harfini ekle
                    }

                    // 'M' harfini takip eden dört karakteri al
                    for (int i = 1; i < text.Length; i++)
                    {
                        char c = text[i];

                        // Eğer karakter bir rakam ise ve toplamda dört rakam olacaksa
                        if (char.IsDigit(c) && result.Length < 5)
                        {
                            result += c;
                        }
                    }

                    // Sonuç metnini beş karakterle sınırla
                    if (result.Length > 5)
                    {
                        result = result.Substring(0, 5);
                    }
                }

                // Metni TextBox'a ata
                tb.Text = result;

                // İmleci metnin sonuna getir
                tb.SelectionStart = tb.Text.Length;

                // TextChanged olayının yeniden tetiklenmesini engelle
                tb.TextChanged -= txtMcode_TextChanged;
                tb.Text = result;
                tb.TextChanged += txtMcode_TextChanged;
            }
        }

        private void cmbCyclone_SelectedIndexChanged(object sender, EventArgs e)
        {
            //UInt32 image_count = 1;
            //UInt32 connection_type = cyclone_control_api.CyclonePortType_USB;
            //UInt32 handle = 0;
            //if (chkCyloneSecim.Checked == true)
            //{


            //    //label6.Text = "Contacting IP1 ...";
            //    Application.DoEvents();
            //    if (handle == 0)

            //        //label8.Text = "Error Opening Device";
            //    else
            //            {
            //                image_count = cyclone_control_api.countCycloneImages(handle);
            //                //label6.Text = "Total Images = " + image_count.ToString();
            //                for (UInt32 i = 1; i < image_count + 1; i++)
            //                {
            //                    // Assuming `getImageDescription` returns the image name or description
            //                    string imageName = cyclone_control_api.getImageDescription(handle, i);
            //                    cmbCyclone.Items.Add(imageName);
            //                }

            //            }
            //}
            //UInt32 imageCount = cyclone_control_api.countCycloneImages(handle);

            //for (UInt32 i = 0; i < imageCount; i++)
            //{
            //    // Assuming `getImageDescription` returns the image name or description
            //    string imageName = cyclone_control_api.getImageDescription(handle, i);
            //    comboBox1.Items.Add(imageName);
            //}
        }

        private void chkCyloneSecim_CheckedChanged(object sender, EventArgs e)
        {
            UInt32 image_count = 1;
            UInt32 connection_type = cyclone_control_api.CyclonePortType_USB;
            UInt32 handle = 0;

            if (chkCyloneSecim.Checked == true)
            {

                ////label8.Text = "Contacting IP1 ...";
                //Application.DoEvents();
                ////handle = cyclone_control_api.connectToCyclone(textBox1.Text);
                //if (handle == 0)

                //    //label6.Text = "Error Opening Device";
                //else
                //{
                //    image_count = cyclone_control_api.countCycloneImages(handle);
                //    //label8.Text = "Total Images = " + image_count.ToString();
                //    for (UInt32 i = 1; i < image_count + 1; i++)
                //    {
                //        // Assuming `getImageDescription` returns the image name or description
                //        string imageName = cyclone_control_api.getImageDescription(handle, i);
                //        cmbCyclone.Items.Add(imageName);
                //    }
                //}

                //if (comboBox1.Items.Count == 0)
                //{
                //    MessageBox.Show(this, "Bağlı bir Cyclone cihazı bulunamadı. Lütfen bağlantıları kontrol ediniz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //    Close();
                //    Dispose();
                //}
            }
        }

        private void chkInfıneonSecim_CheckedChanged(object sender, EventArgs e)
        {
            // Eğer checkBox2 işaretlendiyse
            if (chkInfıneonSecim.Checked)
            {
                // comboBox1 içeriğini temizleyin
                cmb3v5.Items.Clear();

                // 3.3V ve 5V değerlerini ekleyin
                cmb3v5.Items.Add("3.3V");
                cmb3v5.Items.Add("5V");

                // İlk değer seçili olacak şekilde ayarlayın
                cmb3v5.SelectedIndex = -1;
            }
            else
            {

            }
        }

        private Action<string> yeniUrunEklendiHandler; // Yöntem tanımlama

        public void SetYeniUrunEklendiHandler(Action<string> handler)
        {
            yeniUrunEklendiHandler = handler;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
 






//        private void checkBox2_CheckedChanged(object sender, EventArgs e)
//        {
//            // Eğer checkBox2 işaretlendiyse
//            if (checkBox2.Checked)
//            {
//                // comboBox1 içeriğini temizleyin
//                comboBox2.Items.Clear();

//                // 3.3V ve 5V değerlerini ekleyin
//                comboBox2.Items.Add("3.3V");
//                comboBox2.Items.Add("5V");

//                // İlk değer seçili olacak şekilde ayarlayın
//                comboBox2.SelectedIndex = -1;
//            }
//            else
//            {

//            }
//        }
//    }
//}


