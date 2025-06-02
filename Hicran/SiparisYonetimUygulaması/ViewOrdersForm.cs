using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data; // DataTable için gerekli
using System.Data.SqlClient; // SqlParameter için gerekli
using System.Linq; // LINQ metotları (örn: Sum) için gerekli
using SiparisYonetimUygulamasi.DAL; // Veritabanı yardımcı sınıfımız DbHelper için gerekli

namespace SiparisYonetimUygulamasi
{
    public partial class ViewOrdersForm : Form
    {
        // Kontrol referansları
        private DataGridView dgvBills; // Faturaları listeleyecek DataGridView
        private DataGridView dgvOrderDetails; // Seçilen faturanın detaylarını gösterecek DataGridView
        private Label lblTotalBills; // Toplam fatura adedini gösterir

        public ViewOrdersForm()
        {
            InitializeComponent(); // Tasarımcı tarafından oluşturulan bileşenleri başlatır

            // Kontrol nesnelerini bellekte oluşturma (NullReferenceException hatasını önler)
            dgvBills = new DataGridView();
            dgvOrderDetails = new DataGridView();
            lblTotalBills = new Label();

            // Formun temel ayarları
            this.Text = "Mevcut Siparişler ve Faturalar";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(1200, 800); // Form boyutunu büyüttüm
            this.BackColor = Color.FromArgb(224, 255, 230); // Pastel Yeşil arka plan rengi

            // Başlık Label'ı oluşturma ve ayarlama
            Label titleLabel = new Label();
            titleLabel.Text = "Mevcut Siparişler";
            titleLabel.Font = new Font("Segoe UI", 30, FontStyle.Bold);
            titleLabel.ForeColor = Color.Black;
            titleLabel.TextAlign = ContentAlignment.TopCenter;
            titleLabel.AutoSize = false;
            titleLabel.Width = this.ClientSize.Width;
            titleLabel.Height = 70;
            titleLabel.Location = new Point(0, 20);
            this.Controls.Add(titleLabel); // Forma ekle

            // Geri Butonu oluşturma ve ayarlama
            Button backButton = new Button();
            backButton.Text = "Ana Menüye Dön";
            backButton.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            backButton.BackColor = Color.LightGray;
            backButton.ForeColor = Color.Black;
            backButton.FlatStyle = FlatStyle.Flat;
            backButton.FlatAppearance.BorderSize = 0;
            backButton.Size = new Size(150, 40);
            backButton.Location = new Point(this.ClientSize.Width - backButton.Width - 20, 20);
            backButton.Click += (sender, e) => this.Close(); // Tıklandığında formu kapat
            this.Controls.Add(backButton); // Forma ekle

            // Faturalar DataGridView'i (Üst Kısım) ayarlama ve forma ekleme
            Label lblBills = new Label();
            lblBills.Text = "Faturalar:";
            lblBills.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblBills.AutoSize = true;
            lblBills.Location = new Point(50, titleLabel.Bottom + 20);
            this.Controls.Add(lblBills);

            dgvBills.Location = new Point(50, lblBills.Bottom + 10);
            dgvBills.Size = new Size(this.ClientSize.Width - 100, 300); // Genişliği forma göre ayarla
            dgvBills.BackgroundColor = Color.White;
            dgvBills.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(224, 255, 230); // Pastel Yeşil
            dgvBills.EnableHeadersVisualStyles = false;
            dgvBills.RowHeadersVisible = false;
            dgvBills.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvBills.ReadOnly = true;
            dgvBills.AllowUserToAddRows = false;
            dgvBills.AllowUserToDeleteRows = false;
            dgvBills.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvBills.CellClick += DgvBills_CellClick; // Fatura seçildiğinde detayları yüklemek için
            this.Controls.Add(dgvBills);

            // Toplam Fatura Adedi Label'ı ayarlama ve forma ekleme
            lblTotalBills.Text = "Toplam Fatura: 0";
            lblTotalBills.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblTotalBills.ForeColor = Color.DarkBlue;
            lblTotalBills.AutoSize = true;
            lblTotalBills.Location = new Point(50, dgvBills.Bottom + 10);
            this.Controls.Add(lblTotalBills);

            // Sipariş Detayları DataGridView'i (Alt Kısım) ayarlama ve forma ekleme
            Label lblOrderDetails = new Label();
            lblOrderDetails.Text = "Seçilen Fatura Detayları:";
            lblOrderDetails.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblOrderDetails.AutoSize = true;
            lblOrderDetails.Location = new Point(50, lblTotalBills.Bottom + 30);
            this.Controls.Add(lblOrderDetails);

            dgvOrderDetails.Location = new Point(50, lblOrderDetails.Bottom + 10);
            dgvOrderDetails.Size = new Size(this.ClientSize.Width - 100, this.ClientSize.Height - lblOrderDetails.Bottom - 60); // Kalan alanı doldur
            dgvOrderDetails.BackgroundColor = Color.White;
            dgvOrderDetails.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(255, 224, 230); // Pastel Pembe
            dgvOrderDetails.EnableHeadersVisualStyles = false;
            dgvOrderDetails.RowHeadersVisible = false;
            dgvOrderDetails.ReadOnly = true;
            dgvOrderDetails.AllowUserToAddRows = false;
            dgvOrderDetails.AllowUserToDeleteRows = false;
            dgvOrderDetails.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.Controls.Add(dgvOrderDetails);

            // Form yüklendiğinde faturaları yükle
            this.Load += ViewOrdersForm_Load;
        }

        /// <summary>
        /// Form yüklendiğinde faturaları veritabanından çeker ve listeler.
        /// </summary>
        private void ViewOrdersForm_Load(object sender, EventArgs e)
        {
            LoadBills();
        }

        /// <summary>
        /// Veritabanından tüm faturaları çeker ve dgvBills DataGridView'ine yükler.
        /// </summary>
        private void LoadBills()
        {
            try
            {
                // Fatura bilgilerini, masa durumunu ve toplam tutarı getir
                string query = @"
            SELECT 
                b.bill_id, 
                b.table_id, 
                ti.status AS MasaDurumu, 
                b.date AS FaturaTarihi, 
                b.total_amount AS ToplamTutar
            FROM Bill b
            JOIN TableInfo ti ON b.table_id = ti.table_id
            ORDER BY b.date DESC"; // En son faturaları en üstte göster

                DataTable dtBills = DbHelper.GetData(query);
                dgvBills.DataSource = dtBills;

                // Sütun başlıklarını ve formatlarını ayarla
                if (dgvBills.Columns.Contains("bill_id"))
                    dgvBills.Columns["bill_id"].HeaderText = "Fatura ID";
                if (dgvBills.Columns.Contains("table_id"))
                    dgvBills.Columns["table_id"].HeaderText = "Masa ID";
                if (dgvBills.Columns.Contains("MasaDurumu"))
                    dgvBills.Columns["MasaDurumu"].HeaderText = "Masa Durumu";
                if (dgvBills.Columns.Contains("FaturaTarihi"))
                {
                    dgvBills.Columns["FaturaTarihi"].HeaderText = "Fatura Tarihi";
                    dgvBills.Columns["FaturaTarihi"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm"; // Tarih formatı
                }
                if (dgvBills.Columns.Contains("ToplamTutar"))
                {
                    dgvBills.Columns["ToplamTutar"].HeaderText = "Toplam Tutar (TL)";
                    dgvBills.Columns["ToplamTutar"].DefaultCellStyle.Format = "C2"; // Para birimi formatı
                }

                // Toplam fatura adedini güncelle
                lblTotalBills.Text = $"Toplam Fatura: {dtBills.Rows.Count}";

                // Detay DataGridView'ini temizle
                dgvOrderDetails.DataSource = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Faturalar yüklenirken bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// dgvBills DataGridView'de bir fatura seçildiğinde detaylarını dgvOrderDetails'a yükler.
        /// </summary>
        private void DgvBills_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) // Başlık satırına tıklanmadığından emin ol
            {
                DataGridViewRow selectedRow = dgvBills.Rows[e.RowIndex];
                int billId = Convert.ToInt32(selectedRow.Cells["bill_id"].Value);
                LoadBillDetails(billId);
            }
        }

        /// <summary>
        /// Belirtilen fatura ID'sine ait sipariş detaylarını dgvOrderDetails'a yükler.
        /// </summary>
        /// <param name="billId">Detayları yüklenecek faturanın ID'si.</param>
        private void LoadBillDetails(int billId)
        {
            try
            {
                // Sipariş detaylarını ürün adıyla birlikte getir
                string query = @"
            SELECT 
                p.product_name AS ÜrünAdı,
                oi.quantity AS Adet,
                p.price AS BirimFiyat,
                (oi.quantity * p.price) AS Toplam
            FROM OrderInfo oi
            JOIN Product p ON oi.product_id = p.product_id
            WHERE oi.bill_id = @billId";

                SqlParameter[] parameters = new SqlParameter[]
                {
            new SqlParameter("@billId", billId)
                }; // Bu parantez ve noktalı virgül hataya neden oluyordu, şimdi doğru yerde.

                DataTable dtDetails = DbHelper.GetData(query, parameters);
                dgvOrderDetails.DataSource = dtDetails;

                // Sütun başlıklarını ve formatlarını ayarla
                if (dgvOrderDetails.Columns.Contains("ÜrünAdı"))
                    dgvOrderDetails.Columns["ÜrünAdı"].HeaderText = "Ürün Adı";
                if (dgvOrderDetails.Columns.Contains("Adet"))
                    dgvOrderDetails.Columns["Adet"].HeaderText = "Adet";
                if (dgvOrderDetails.Columns.Contains("BirimFiyat"))
                {
                    dgvOrderDetails.Columns["BirimFiyat"].HeaderText = "Birim Fiyat (TL)";
                    dgvOrderDetails.Columns["BirimFiyat"].DefaultCellStyle.Format = "C2";
                }
                if (dgvOrderDetails.Columns.Contains("Toplam"))
                {
                    dgvOrderDetails.Columns["Toplam"].HeaderText = "Ürün Toplam (TL)";
                    dgvOrderDetails.Columns["Toplam"].DefaultCellStyle.Format = "C2";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fatura detayları yüklenirken bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }


}