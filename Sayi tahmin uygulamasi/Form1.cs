using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Sayi_tahmin_uygulamasi
{
    public partial class Form1 : Form
    {
        private readonly Random rnd = new Random();
        private readonly string skorDosyaYolu = Path.Combine(Application.StartupPath, "en_iyi_skor.txt");
        private int sayi;
        private int sayac;
        private int altSinir;
        private int ustSinir;
        private int denemeHakki;
        private int enIyiSkor;
        private ComboBox zorlukComboBox;
        private Label skorLabel;
        private Label hakLabel;
        private Button yeniOyunButton;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(textBox1.Text, out int tahmin))
            {
                MessageBox.Show("Lütfen sadece sayı giriniz.", "Geçersiz Giriş", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox1.Clear();
                textBox1.Focus();
                return;
            }

            if (tahmin < altSinir || tahmin > ustSinir)
            {
                MessageBox.Show($"Tahmin {altSinir} ile {ustSinir} arasında olmalıdır.", "Aralık Dışı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox1.SelectAll();
                textBox1.Focus();
                return;
            }

            sayac++;

            if (tahmin > sayi)
            {
                label2.Text = $"Aşağı inmelisin. Deneme: {sayac}";
            }
            else if (tahmin < sayi)
            {
                label2.Text = $"Yukarı çıkmalısın. Deneme: {sayac}";
            }
            else
            {
                int puan = PuaniHesapla();
                if (puan > enIyiSkor)
                {
                    enIyiSkor = puan;
                    EnIyiSkoruKaydet();
                }

                label2.Text = $"Doğru tahmin! {sayac}. denemede bildin. Puan: {puan}";
                SkorBilgisiniGuncelle();
                MessageBox.Show("Tebrikler! Yeni oyun başlatılıyor.", "Oyun Bitti", MessageBoxButtons.OK, MessageBoxIcon.Information);
                YeniOyunBaslat();
                return;
            }

            if (sayac >= denemeHakki)
            {
                MessageBox.Show($"Deneme hakkınız bitti. Tutulan sayı: {sayi}", "Oyun Bitti", MessageBoxButtons.OK, MessageBoxIcon.Information);
                YeniOyunBaslat();
                return;
            }

            HakBilgisiniGuncelle();
            textBox1.Clear();
            textBox1.Focus();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Text = "Sayı Tahmin Oyunu";
            ClientSize = new Size(800, 450);
            ArayuzuGelistir();
            enIyiSkor = EnIyiSkoruOku();
            YeniOyunBaslat();
        }

        private void ArayuzuGelistir()
        {
            label1.Location = new Point(265, 75);
            textBox1.Location = new Point(305, 145);
            button1.Location = new Point(305, 190);
            label2.Location = new Point(250, 250);
            label2.AutoSize = true;

            Label zorlukLabel = new Label
            {
                Location = new Point(305, 110),
                Size = new Size(190, 20),
                Text = "Zorluk seviyesi"
            };
            Controls.Add(zorlukLabel);

            zorlukComboBox = new ComboBox
            {
                Location = new Point(430, 107),
                Size = new Size(170, 24),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            zorlukComboBox.Items.AddRange(new object[]
            {
                "Kolay (0-50)",
                "Orta (0-100)",
                "Zor (0-500)"
            });
            zorlukComboBox.SelectedIndex = 1;
            zorlukComboBox.SelectedIndexChanged += ZorlukComboBox_SelectedIndexChanged;
            Controls.Add(zorlukComboBox);

            hakLabel = new Label
            {
                Location = new Point(250, 290),
                Size = new Size(330, 24)
            };
            Controls.Add(hakLabel);

            skorLabel = new Label
            {
                Location = new Point(250, 325),
                Size = new Size(330, 24)
            };
            Controls.Add(skorLabel);

            yeniOyunButton = new Button
            {
                Location = new Point(305, 360),
                Size = new Size(191, 34),
                Text = "Yeni Oyun"
            };
            yeniOyunButton.Click += YeniOyunButton_Click;
            Controls.Add(yeniOyunButton);
        }

        private void YeniOyunBaslat()
        {
            ZorlukAyarlariniBelirle();
            sayi = rnd.Next(altSinir, ustSinir + 1);
            sayac = 0;
            label1.Text = $"{altSinir}-{ustSinir} arasında bir sayı giriniz";
            label2.Text = "Yeni oyun başladı. Tahminini gir.";
            HakBilgisiniGuncelle();
            SkorBilgisiniGuncelle();
            textBox1.Clear();
            textBox1.Focus();
        }

        private void ZorlukAyarlariniBelirle()
        {
            switch (zorlukComboBox.SelectedIndex)
            {
                case 0:
                    altSinir = 0;
                    ustSinir = 50;
                    denemeHakki = 10;
                    break;
                case 2:
                    altSinir = 0;
                    ustSinir = 500;
                    denemeHakki = 7;
                    break;
                default:
                    altSinir = 0;
                    ustSinir = 100;
                    denemeHakki = 8;
                    break;
            }
        }

        private int PuaniHesapla()
        {
            int kalanHak = Math.Max(0, denemeHakki - sayac);
            int zorlukCarpani = zorlukComboBox.SelectedIndex == 2 ? 3 : zorlukComboBox.SelectedIndex == 1 ? 2 : 1;
            return Math.Max(10, (kalanHak + 1) * 10 * zorlukCarpani);
        }

        private void HakBilgisiniGuncelle()
        {
            hakLabel.Text = $"Kalan hak: {Math.Max(0, denemeHakki - sayac)} / {denemeHakki}";
        }

        private void SkorBilgisiniGuncelle()
        {
            skorLabel.Text = $"En iyi skor: {enIyiSkor}";
        }

        private int EnIyiSkoruOku()
        {
            if (!File.Exists(skorDosyaYolu))
                return 0;

            return int.TryParse(File.ReadAllText(skorDosyaYolu), out int skor) ? skor : 0;
        }

        private void EnIyiSkoruKaydet()
        {
            File.WriteAllText(skorDosyaYolu, enIyiSkor.ToString());
        }

        private void ZorlukComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (hakLabel != null)
                YeniOyunBaslat();
        }

        private void YeniOyunButton_Click(object sender, EventArgs e)
        {
            YeniOyunBaslat();
        }
    }
}
