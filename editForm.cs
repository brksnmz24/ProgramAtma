using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using ProgramAtmaAparatı;
using System.Drawing; // BackColor özelliği için gerekli



namespace ProgramAtmaAparatı
{
    public partial class editForm : Form
    {
        private ComboBox comboBox;
        private TextBox infoTextBox;
        private Button deleteButton;
        private string filePath = "yeniürün.txt"; // Dosya yolu
        //private string filePath1 = "KartBoyutu.txt"; // Dosya yolu

        private Form1 mainForm; // Ana form referansı

        public editForm(Form1 parentForm, ComboBox mainComboBox)
        {
            mainForm = parentForm; // Ana form referansını al
            comboBox = new ComboBox();
            comboBox.Location = new System.Drawing.Point(15, 15);
            comboBox.Width = 200;

            // mainComboBox öğelerini bir diziye kopyala ve yeni ComboBox'a ekle
            object[] items = new object[mainComboBox.Items.Count];
            mainComboBox.Items.CopyTo(items, 0);
            comboBox.Items.AddRange(items);

            Controls.Add(comboBox);

            // Ürün bilgilerini gösterecek TextBox
            infoTextBox = new TextBox();
            infoTextBox.Location = new System.Drawing.Point(15, 45);
            infoTextBox.Width = 200;
            infoTextBox.Multiline = true;
            infoTextBox.Height = 100;
            Controls.Add(infoTextBox);

            // Silme butonu
            deleteButton = new Button();
            deleteButton.Text = "Sil";
            deleteButton.Location = new System.Drawing.Point(15, 150);
            deleteButton.Click += new EventHandler(DeleteButton_Click);
            Controls.Add(deleteButton);

            comboBox.SelectedIndexChanged += ComboBox_SelectedIndexChanged;
            this.Text = "Düzenleme Formu";
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            this.BackColor = Color.FromArgb(252, 209, 35);
            this.Font = new Font("Consolas", 11.00F, FontStyle.Bold); // Formun varsayılan yazı stilini ayarla

        }

        private void ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Seçilen ürünün bilgilerini göster
            if (comboBox.SelectedItem != null)
            {
                string selectedItem = comboBox.SelectedItem.ToString();
                infoTextBox.Text = $"Seçilen Ürün: {selectedItem}\nÜrün Bilgileri: Burada gösterilecek.";
            }
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            // Seçilen ürünü sil
            if (comboBox.SelectedItem != null)
            {
                string selectedItem = comboBox.SelectedItem.ToString();
                DeleteProduct(selectedItem);
                comboBox.Items.Remove(selectedItem); // EditForm'dan seçilen ürünü kaldır
                mainForm.RemoveProductFromComboBox(selectedItem); // Ana formdan seçilen ürünü kaldır
                infoTextBox.Clear();
                MessageBox.Show($"{selectedItem} silindi.");
            }
            else
            {
                MessageBox.Show("Silinecek ürün seçilmedi.");
            }
        }

        private void DeleteProduct(string productName)
        {
            var lines = File.ReadAllLines(filePath); // Dosyanızdaki ürün bilgilerini okuduğunuz yer
            //var lines1 = File.ReadAllLines(filePath1); // Eğer başka bir dosya ile ilişkili ürün bilgisi varsa buradan okunur

            var updatedList = new List<string>();
            bool isInProductSection = false;

            foreach (var line in lines)
            {
                if (line.StartsWith($"{productName}"))
                {
                    // Seçilen ürünün başlığını gördük, bu bölümden sonraki tüm satırları atla
                    isInProductSection = true;
                    continue;
                }

                if (isInProductSection)
                {
                    if (line.StartsWith("") && !line.StartsWith($"{productName}"))
                    {
                        // Bir sonraki ürünün başlığına ulaştık, bu yüzden mevcut ürünün altındaki bölümü bitir
                        isInProductSection = false;
                        updatedList.Add(line); // Yeni ürün başlığını ekle
                    }
                    // Seçilen ürünün altındaki satırları geç
                    continue;
                }

                // Eğer mevcut bölüm silinmiyorsa, listeye ekle
                updatedList.Add(line);
            }

            // Güncellenmiş listeyi dosyaya yaz
            File.WriteAllLines(filePath, updatedList);
            //File.WriteAllLines(filePath1, updatedList);

            // Ürüne ait koordinatlar klasörünü bul ve sil
            DeleteProductCoordinates(productName);
            DeleteCycloneInfineonFile(productName);
            DeleteProductCoordinates1(productName);
            DeleteProductCoordinates12(productName);
            //DeleteKartBoyutuEntry(productName); // KartBoyutu.txt dosyasından da sil
        }
        private void DeleteProductCoordinates12(string productName)
        {
            try
            {
                // comboBox'taki format: "MCode_UrunAdi" şeklinde, bunu ayırıyoruz
                string[] tokens = productName.Split('_');
                if (tokens.Length < 2)
                {
                    MessageBox.Show("Geçersiz ürün formatı.");
                    return;
                }

                string mCode = tokens[0];  // M kodunu alıyoruz

                // CycloneInfineon klasöründeki MCode.txt dosyasının yolu
                string filePath = Path.Combine("KartBoyutu", $"{mCode}.txt");

                // Eğer dosya varsa sil
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    MessageBox.Show($"{mCode} KartBoyutu dosyası silindi.");
                }
                else
                {
                    MessageBox.Show("KartBoyutu dosyası bulunamadı.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"KartBoyutu dosyası silinirken bir hata oluştu: {ex.Message}");
            }
        }


        private void DeleteProductCoordinates(string productName)
        {
            try
            {
                // "KoordinatlarListesi" klasöründeki tüm dosyaları al
                string folderPath = "KoordinatlarListesi";

                if (!Directory.Exists(folderPath))
                {
                    MessageBox.Show("KoordinatlarListesi klasörü bulunamadı.");
                    return;
                }

                // Klasördeki tüm .txt dosyalarını al
                var allFiles = Directory.GetFiles(folderPath, "*.txt");

                // Ürün adını içeren dosyayı bul
                foreach (var file in allFiles)
                {
                    string fileName = Path.GetFileNameWithoutExtension(file); // Dosya adını uzantısız al

                    // Eğer dosya adı ürün adını içeriyorsa sil
                    if (fileName.Contains(productName))
                    {
                        File.Delete(file); // Dosyayı sil
                        MessageBox.Show($"{fileName}.txt dosyası silindi.");
                    }
                }

                // Eğer ürün adına göre bir klasör de varsa, onu da silelim
                string productFolderPath = Path.Combine(folderPath, productName);

                if (Directory.Exists(productFolderPath))
                {
                    Directory.Delete(productFolderPath, true); // Klasörü ve içeriğini sil
                    MessageBox.Show($"{productName} klasörü silindi.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Koordinatlar silinirken bir hata oluştu: {ex.Message}");
            }
        }
        private void DeleteCycloneInfineonFile(string productName)
        {
            try
            {
                // comboBox'taki format: "MCode_UrunAdi" şeklinde, bunu ayırıyoruz
                string[] tokens = productName.Split('_');
                if (tokens.Length < 2)
                {
                    MessageBox.Show("Geçersiz ürün formatı.");
                    return;
                }

                string mCode = tokens[0];  // M kodunu alıyoruz

                // CycloneInfineon klasöründeki MCode.txt dosyasının yolu
                string filePath = Path.Combine("CycloneInfineon", $"{mCode}.txt");

                // Eğer dosya varsa sil
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    MessageBox.Show($"{mCode} CycloneInfineon dosyası silindi.");
                }
                else
                {
                    MessageBox.Show("CycloneInfineon dosyası bulunamadı.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"CycloneInfineon dosyası silinirken bir hata oluştu: {ex.Message}");
            }
        }
        private void DeleteProductCoordinates1(string mCode)
        {
            try
            {
                // Ana Koordinatlar klasörü yolu
                string mainDirectoryPath = "ÜrünBaşlangıçKoordinatları";

                // Koordinatlar klasörü altındaki tüm klasörleri al
                var directories = Directory.GetDirectories(mainDirectoryPath);

                foreach (var dir in directories)
                {
                    // Alt klasör adını al (örneğin M4116)
                    string dirName = Path.GetFileName(dir);

                    // Eğer klasör adı mCode ile eşleşiyorsa
                    if (dirName.Length >= 5 && mCode.Contains(dirName))
                    {
                        // Klasör içindeki tüm dosyaları sil
                        var files = Directory.GetFiles(dir);
                        foreach (var file in files)
                        {
                            File.Delete(file); // Dosyaları sil
                        }

                        // Şimdi klasörü sil
                        Directory.Delete(dir, true); // true -> alt klasördeki tüm dosyaları da siler

                        // Kontrol: klasör hala mevcut mu?
                        if (!Directory.Exists(dir))
                        {
                            MessageBox.Show($"ÜrünBaşlangıçKoordinatları klasörü {dirName} ve içeriği başarıyla silindi.");
                        }
                        else
                        {
                            MessageBox.Show($"Klasör {dirName} silinemedi.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ÜrünBaşlangıçKoordinatları klasörü silinirken bir hata oluştu: {ex.Message}");
            }
        }
     
        private void editForm_Load(object sender, EventArgs e)
        {

        }
    }
}