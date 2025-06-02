using System;
using System.Drawing;
using System.Windows.Forms;
using SiparisYonetimUygulamasi.DAL; // DbHelper sınıfı için

namespace SiparisYonetimUygulamasi 
{
    public partial class MainForm : Form 
    {
        public MainForm()
        {
            InitializeComponent();

            this.Text = "Sipariş Yönetim Sistemi - Ana Menü"; // Formun başlık çubuğu metni
            this.StartPosition = FormStartPosition.CenterScreen; // Formu ekranın ortasında açar
            this.FormBorderStyle = FormBorderStyle.FixedSingle; // Kullanıcının formu yeniden boyutlandırmasını engeller
            this.Size = new Size(800, 600); // Formun başlangıç boyutu

            // Pastel yeşil arka plan rengi (RGB: 224, 255, 230)
            this.BackColor = Color.FromArgb(224, 255, 230);
            // Eğer pastel pembe istersen: this.BackColor = Color.FromArgb(255, 224, 230);

            Label welcomeLabel = new Label();
            welcomeLabel.Text = "Hoş Geldiniz!";
            welcomeLabel.Font = new Font("Segoe UI", 36, FontStyle.Bold); // Büyük ve kalın yazı tipi
            welcomeLabel.ForeColor = Color.DimGray; // Koyu gri yazı rengi
            welcomeLabel.TextAlign = ContentAlignment.MiddleCenter; // Yazıyı ortala
            welcomeLabel.AutoSize = false; // Boyutu manuel ayarlayacağız

            // Label'ın boyutunu ve konumunu formun üst kısmına ortalayalım
            welcomeLabel.Width = this.ClientSize.Width; // Formun istemci alanının genişliği kadar
            welcomeLabel.Height = 80; // Yükseklik
            welcomeLabel.Location = new Point(0, 30); // Formun üst kenarından 30 piksel aşağıda
            this.Controls.Add(welcomeLabel); // Label'ı forma ekle

            // *******************************************************************
            // BUTON STİL AYARLARI İÇİN YARDIMCI METOT
            // *******************************************************************
            Button CreateStyledButton(string text, Color backColor, int yPosition)
            {
                Button btn = new Button();
                btn.Text = text;
                btn.Font = new Font("Segoe UI", 14, FontStyle.Bold);
                btn.BackColor = backColor;
                btn.ForeColor = Color.Black; // Buton yazı rengi
                btn.FlatStyle = FlatStyle.Flat; // Düz buton stili
                btn.FlatAppearance.BorderSize = 0; // Buton kenarlığını kaldırır
                btn.Size = new Size(280, 70); // Buton boyutu

                // Butonu formun yatayda ortasına yerleştirme hesaplaması
                int xPosition = (this.ClientSize.Width - btn.Width) / 2;
                btn.Location = new Point(xPosition, yPosition);

                return btn;
            }

            // *******************************************************************
            // PASTEL RENK TANIMLARI
            // *******************************************************************
            Color pastelPink = Color.FromArgb(255, 224, 230);
            Color pastelGreen = Color.FromArgb(224, 255, 230);

            // *******************************************************************
            // BUTONLARI KODLA OLUŞTURMA VE FORMA EKLEME
            // Her butona farklı bir Name ve Click olayı atıyoruz.
            // *******************************************************************

            // 1. Yeni Sipariş Oluştur butonu
            Button btnNewOrder = CreateStyledButton("Yeni Sipariş Oluştur", pastelPink, 150);
            btnNewOrder.Name = "btnNewOrder"; // Name özelliği, kodda butona referans vermek için kullanılır
            btnNewOrder.Click += BtnNewOrder_Click; // Click olayına metod ataması
            this.Controls.Add(btnNewOrder); // Butonu forma ekle

            // 2. Mevcut Siparişler butonu
            Button btnViewOrders = CreateStyledButton("Mevcut Siparişler", pastelGreen, 230);
            btnViewOrders.Name = "btnViewOrders";
            btnViewOrders.Click += BtnViewOrders_Click;
            this.Controls.Add(btnViewOrders);

            // 3. Ürün Yönetimi butonu
            Button btnProductManagement = CreateStyledButton("Ürün Yönetimi", pastelPink, 310);
            btnProductManagement.Name = "btnProductManagement";
            btnProductManagement.Click += BtnProductManagement_Click;
            this.Controls.Add(btnProductManagement);

            // 4. Masa Durumları butonu
            Button btnTableStatus = CreateStyledButton("Masa Durumları", pastelGreen, 390);
            btnTableStatus.Name = "btnTableStatus";
            btnTableStatus.Click += BtnTableStatus_Click;
            this.Controls.Add(btnTableStatus);
        }

        // *******************************************************************
        // BUTONLARIN CLICK OLAY METOTLARI
        // *******************************************************************

        private void BtnNewOrder_Click(object sender, EventArgs e)
        {
            NewOrderForm newOrderForm = new NewOrderForm();
            this.Hide(); // MainForm'u gizle
            newOrderForm.FormClosed += (s, args) => this.Show(); // Açılan form kapandığında MainForm'u tekrar göster
            newOrderForm.Show(); // Formu modal olmayan (non-modal) olarak aç
        }

        private void BtnViewOrders_Click(object sender, EventArgs e)
        {
            ViewOrdersForm viewOrdersForm = new ViewOrdersForm();
            this.Hide();
            viewOrdersForm.FormClosed += (s, args) => this.Show();
            viewOrdersForm.Show();
        }

        private void BtnProductManagement_Click(object sender, EventArgs e)
        {
            ProductManagementForm productManagementForm = new ProductManagementForm();
            this.Hide();
            productManagementForm.FormClosed += (s, args) => this.Show();
            productManagementForm.Show();
        }

        private void BtnTableStatus_Click(object sender, EventArgs e)
        {
            TableStatusForm tableStatusForm = new TableStatusForm();
            this.Hide();
            tableStatusForm.FormClosed += (s, args) => this.Show();
            tableStatusForm.Show();
        }

        // Form yüklendiğinde çalışacak ek kodlar buraya gelebilir.
        private void MainForm_Load(object sender, EventArgs e)
        {
            // Eğer form yüklendiğinde yapılması gereken bir şey varsa buraya yazılır.
        }
    } // MainForm sınıfının kapanış parantezi
} // namespace'in kapanış parantezi
