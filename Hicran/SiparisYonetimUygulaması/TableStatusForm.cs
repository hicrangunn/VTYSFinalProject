using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data; // DataTable için gerekli
using System.Data.SqlClient; // SqlParameter için gerekli
using SiparisYonetimUygulamasi.DAL; // Veritabanı yardımcı sınıfımız DbHelper için gerekli

namespace SiparisYonetimUygulamasi
{
    public partial class TableStatusForm : Form
    {
        // Kontrol referansları
        private FlowLayoutPanel flowLayoutPanelTables; // Masaları düzenli bir şekilde gösterecek panel
        private Label lblTableSummary; // Masa özeti (örn: Toplam: X, Boş: Y, Dolu: Z)

        public TableStatusForm()
        {
            InitializeComponent(); // Tasarımcı tarafından oluşturulan bileşenleri başlatır

            // Formun temel ayarları
            this.Text = "Masa Durumları";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(1000, 750); // Form boyutunu ayarladım
            this.BackColor = Color.FromArgb(255, 224, 230); // Pastel Pembe arka plan rengi

            // Başlık Label'ı oluşturma ve ayarlama
            Label titleLabel = new Label();
            titleLabel.Text = "Masa Durumları";
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

            // Masa Özeti Label'ı
            lblTableSummary = new Label();
            lblTableSummary.Text = "Masa Özeti: Yükleniyor...";
            lblTableSummary.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblTableSummary.AutoSize = true;
            lblTableSummary.Location = new Point(50, titleLabel.Bottom + 20);
            this.Controls.Add(lblTableSummary);

            // FlowLayoutPanel oluşturma (Masaları dinamik olarak içine ekleyeceğiz)
            flowLayoutPanelTables = new FlowLayoutPanel();
            flowLayoutPanelTables.FlowDirection = FlowDirection.LeftToRight; // Soldan sağa dizilim
            flowLayoutPanelTables.WrapContents = true; // İçerik sığmazsa alt satıra geç
            flowLayoutPanelTables.AutoScroll = true; // İçerik taşarsa kaydırma çubuğu göster
            flowLayoutPanelTables.Location = new Point(50, lblTableSummary.Bottom + 20);
            flowLayoutPanelTables.Size = new Size(this.ClientSize.Width - 100, this.ClientSize.Height - lblTableSummary.Bottom - 70); // Genişliği ve yüksekliği ayarla
            flowLayoutPanelTables.BackColor = Color.White; // Arka planı beyaz yapabiliriz
            flowLayoutPanelTables.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(flowLayoutPanelTables);

            // Form yüklendiğinde masaları yükle
            this.Load += TableStatusForm_Load;
        }

        /// <summary>
        /// Form yüklendiğinde masaları veritabanından çeker ve görselleştirir.
        /// </summary>
        private void TableStatusForm_Load(object sender, EventArgs e)
        {
            LoadTableStatus();
        }

        /// <summary>
        /// Veritabanından masaların durumunu çeker ve FlowLayoutPanel'a masa kartları ekler.
        /// </summary>
        public void LoadTableStatus()
        {
            try
            {
                // Mevcut masa kartlarını temizle
                flowLayoutPanelTables.Controls.Clear();

                string query = "SELECT table_id, status FROM TableInfo ORDER BY table_id";
                DataTable dt = DbHelper.GetData(query); // DbHelper ile veriyi al

                int totalTables = dt.Rows.Count;
                int emptyTables = 0;
                int occupiedTables = 0;
                int reservedTables = 0;

                // Her masa için bir görsel kontrol oluştur
                foreach (DataRow row in dt.Rows)
                {
                    int tableId = Convert.ToInt32(row["table_id"]);
                    string status = row["status"].ToString();

                    // Masa kartı için Panel oluşturma
                    Panel tablePanel = new Panel();
                    tablePanel.Size = new Size(150, 150); // Masa kartı boyutu
                    tablePanel.Margin = new Padding(10); // Kartlar arası boşluk
                    tablePanel.BorderStyle = BorderStyle.FixedSingle;
                    tablePanel.Cursor = Cursors.Hand; // Üzerine gelince el işareti

                    // Duruma göre renk ataması
                    switch (status)
                    {
                        case "Boş":
                            tablePanel.BackColor = Color.FromArgb(200, 255, 200); // Açık yeşil
                            emptyTables++;
                            break;
                        case "Dolu":
                            tablePanel.BackColor = Color.FromArgb(255, 200, 200); // Açık kırmızı
                            occupiedTables++;
                            break;
                        case "Rezervasyonlu":
                            tablePanel.BackColor = Color.FromArgb(255, 255, 200); // Açık sarı
                            reservedTables++;
                            break;
                        default:
                            tablePanel.BackColor = Color.LightGray; // Bilinmeyen durum
                            break;
                    }

                    // Masa Numarası Label'ı
                    Label lblTableNo = new Label();
                    lblTableNo.Text = $"Masa {tableId}";
                    lblTableNo.Font = new Font("Segoe UI", 18, FontStyle.Bold);
                    lblTableNo.AutoSize = false;
                    lblTableNo.TextAlign = ContentAlignment.MiddleCenter;
                    lblTableNo.Dock = DockStyle.Top; // Üste hizala
                    lblTableNo.Height = 50;
                    tablePanel.Controls.Add(lblTableNo);

                    // Masa Durumu Label'ı
                    Label lblStatus = new Label();
                    lblStatus.Text = status;
                    lblStatus.Font = new Font("Segoe UI", 14, FontStyle.Regular);
                    lblStatus.AutoSize = false;
                    lblStatus.TextAlign = ContentAlignment.MiddleCenter;
                    lblStatus.Dock = DockStyle.Fill; // Kalan alanı doldur
                    tablePanel.Controls.Add(lblStatus);
                    lblStatus.BringToFront(); // Durum yazısının önde olmasını sağla

                    // Masa Paneline tıklama olayı (lambda expression ile tableId ve status'ı yakala)
                    tablePanel.Click += (sender, e) => TablePanel_Click(tableId, status);

                    // FlowLayoutPanel'a masa panelini ekle
                    flowLayoutPanelTables.Controls.Add(tablePanel);
                }

                // Masa özetini güncelle
                lblTableSummary.Text = $"Masa Özeti: Toplam: {totalTables}, Boş: {emptyTables}, Dolu: {occupiedTables}, Rezervasyonlu: {reservedTables}";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Masa durumları yüklenirken bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Masa paneline tıklandığında çalışacak olay metodu.
        /// Masa durumunu güncelleme veya yeni sipariş başlatma seçenekleri sunar.
        /// </summary>
        /// <param name="tableId">Tıklanan masanın ID'si.</param>
        /// <param name="currentStatus">Tıklanan masanın mevcut durumu.</param>
        private void TablePanel_Click(int tableId, string currentStatus)
        {
            ContextMenuStrip contextMenu = new ContextMenuStrip();

            // Yeni Sipariş Başlat seçeneği
            ToolStripMenuItem newOrderMenuItem = new ToolStripMenuItem($"Masa {tableId} İçin Yeni Sipariş");
            newOrderMenuItem.Click += (s, ev) => StartNewOrderForTable(tableId);
            contextMenu.Items.Add(newOrderMenuItem);

            // Durum Güncelleme seçenekleri (Mevcut duruma göre farklı seçenekler)
            if (currentStatus == "Dolu")
            {
                ToolStripMenuItem makeEmptyMenuItem = new ToolStripMenuItem("Boş Yap");
                makeEmptyMenuItem.Click += (s, ev) => UpdateTableStatus(tableId, "Boş", currentStatus); // currentStatus'ı da gönder
                contextMenu.Items.Add(makeEmptyMenuItem);
            }
            else // Boş veya Rezervasyonlu ise
            {
                ToolStripMenuItem makeOccupiedMenuItem = new ToolStripMenuItem("Dolu Yap");
                makeOccupiedMenuItem.Click += (s, ev) => UpdateTableStatus(tableId, "Dolu", currentStatus);
                contextMenu.Items.Add(makeOccupiedMenuItem);
            }
            // Rezervasyonlu seçeneği (her zaman olabilir)
            ToolStripMenuItem makeReservedMenuItem = new ToolStripMenuItem("Rezervasyonlu Yap");
            makeReservedMenuItem.Click += (s, ev) => UpdateTableStatus(tableId, "Rezervasyonlu", currentStatus);
            contextMenu.Items.Add(makeReservedMenuItem);


            // Menüyü aç
            contextMenu.Show(Cursor.Position); // Farenin olduğu yerde menüyü göster
        }


        /// <summary>
        /// Belirtilen masanın durumunu veritabanında günceller ve masa durumlarını yeniden yükler.
        /// Eğer masa 'Dolu'dan 'Boş'a çekiliyorsa, ilişkili faturaları silme onayı ister.
        /// </summary>
        /// <param name="tableId">Durumu güncellenecek masanın ID'si.</param>
        /// <param name="newStatus">Masanın yeni durumu (örn: 'Boş', 'Dolu', 'Rezervasyonlu').</param>
        /// <param name="oldStatus">Masanın mevcut durumu.</param>
        private void UpdateTableStatus(int tableId, string newStatus, string oldStatus)
        {
            try
            {
                // Eğer masa 'Dolu'dan 'Boş'a çekiliyorsa, ilişkili faturaları kontrol et
                if (oldStatus == "Dolu" && newStatus == "Boş")
                {
                    // Masaya ait açık fatura var mı kontrol et (Bill tablosunda table_id'si bu masaya ait olan)
                    string checkBillQuery = "SELECT COUNT(*) FROM Bill WHERE table_id = @tableId";
                    SqlParameter[] checkBillParams = new SqlParameter[]
                    {
                        new SqlParameter("@tableId", tableId)
                    };
                    int billCount = Convert.ToInt32(DbHelper.GetData(checkBillQuery, checkBillParams).Rows[0][0]);

                    if (billCount > 0)
                    {
                        // Kullanıcıdan onay iste
                        DialogResult confirmResult = MessageBox.Show(
                            $"Masa {tableId} için {billCount} adet fatura bulunmaktadır. Masayı 'Boş' yaparsanız, bu faturalar ve ilişkili sipariş detayları silinecektir. Emin misiniz?",
                            "Fatura Silme Onayı",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Warning
                        );

                        if (confirmResult == DialogResult.Yes)
                        {
                            // İlişkili OrderInfo kayıtlarını sil
                            string deleteOrderInfoQuery = "DELETE FROM OrderInfo WHERE bill_id IN (SELECT bill_id FROM Bill WHERE table_id = @tableId)";
                            SqlParameter[] deleteOrderInfoParams = new SqlParameter[] { new SqlParameter("@tableId", tableId) };
                            DbHelper.ExecuteNonQuery(deleteOrderInfoQuery, deleteOrderInfoParams);

                            // İlişkili Bill kayıtlarını sil
                            string deleteBillQuery = "DELETE FROM Bill WHERE table_id = @tableId";
                            SqlParameter[] deleteBillParams = new SqlParameter[] { new SqlParameter("@tableId", tableId) };
                            DbHelper.ExecuteNonQuery(deleteBillQuery, deleteBillParams);

                            MessageBox.Show($"Masa {tableId} için tüm faturalar ve sipariş detayları silindi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            // Kullanıcı vazgeçtiyse işlemi iptal et
                            MessageBox.Show("Masa durumu güncelleme işlemi iptal edildi.", "İptal Edildi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                    }
                }

                // SQL sorgusu: TableInfo tablosunda masanın durumunu güncelle
                string query = "UPDATE TableInfo SET status = @status WHERE table_id = @tableId";
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@status", newStatus), // Yeni durumu parametre olarak ata
                    new SqlParameter("@tableId", tableId) // Masa ID'sini parametre olarak ata
                };

                int affectedRows = DbHelper.ExecuteNonQuery(query, parameters); // DbHelper ile sorguyu çalıştır
                if (affectedRows > 0)
                {
                    MessageBox.Show($"Masa {tableId} durumu '{newStatus}' olarak başarıyla güncellendi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadTableStatus(); // Masa durumlarını başarıyla güncellendikten sonra yeniden yükle
                }
                else
                {
                    MessageBox.Show($"Masa {tableId} durumu güncellenirken bir sorun oluştu veya değişiklik yapılmadı. Etkilenen satır yok.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Masa durumu güncellenirken beklenmeyen bir hata oluştu: {ex.Message}\nDetay: {ex.InnerException?.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Belirtilen masa için yeni bir sipariş formu açar.
        /// </summary>
        /// <param name="tableId">Sipariş başlatılacak masanın ID'si.</param>
        private void StartNewOrderForTable(int tableId)
        {
            // NewOrderForm'u açıp masayı otomatik seçtirebiliriz.
            // Bunun için NewOrderForm'a bir constructor overload eklememiz gerekir.
            // Önce NewOrderForm'u bu constructor'ı kabul edecek şekilde güncelleyelim.
            MessageBox.Show($"Masa {tableId} için yeni sipariş başlatılıyor...", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Şimdilik sadece formu açıyoruz, masa seçimi NewOrderForm içinde manuel yapılacak.
            // Eğer istersen, NewOrderForm'a 'tableId' parametresi göndererek otomatik seçim yapabiliriz.
            // NewOrderForm newOrderForm = new NewOrderForm(tableId); // Bu constructor'ı sonra ekleyeceğiz
            // this.Hide();
            // newOrderForm.FormClosed += (s, args) => this.Show();
            // newOrderForm.Show();
        }
    }
}