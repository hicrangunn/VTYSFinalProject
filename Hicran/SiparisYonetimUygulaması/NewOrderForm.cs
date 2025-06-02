using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data; // DataTable için gerekli
using System.Data.SqlClient; // SqlParameter için gerekli
using System.Linq; // LINQ metotları (AsEnumerable, FirstOrDefault, Sum) için gerekli
using SiparisYonetimUygulamasi.DAL; // Veritabanı yardımcı sınıfımız DbHelper için gerekli

namespace SiparisYonetimUygulamasi
{
    public partial class NewOrderForm : Form
    {
        // Kontrol referansları: Form üzerindeki bileşenlere erişmek için kullanılır.
        private ComboBox cmbTables;
        private DataGridView dgvProducts; // Menüdeki ürünleri listeler
        private NumericUpDown numQuantity; // Ürün miktarı girişi için
        private DataGridView dgvOrderDetails; // Siparişe eklenen ürünlerin detaylarını gösterir
        private Label lblTotalPrice; // Toplam sipariş tutarını gösterir
        private Button btnAddOrderItem; // Siparişe ürün ekleme butonu
        private Button btnCompleteOrder; // Siparişi tamamlama/fatura oluşturma butonu
        private Button btnRemoveOrderItem; // Siparişten ürün çıkarma butonu
        private Button btnPay; // Ödeme yapma butonu (btnPayBill yerine btnPay kullanıldı)

        // Sipariş yönetimi için dahili değişkenler
        private DataTable currentOrderItems; // Geçici sipariş listesini tutar (DataGridView'e bağlı)
        private int currentBillId = -1; // Oluşturulan faturanın ID'sini tutar
        private int selectedTableId = -1; // Seçili masanın ID'sini tutar
        private string selectedTableStatus = ""; // Seçili masanın mevcut durumunu tutar (YENİ)
        private int activeBillIdForPayment = -1; // Seçili masanın ödenmemiş faturası varsa ID'si

        public NewOrderForm()
        {
            InitializeComponent(); // Tasarımcı tarafından oluşturulan bileşenleri başlatır

            // Kontrol nesnelerini bellekte oluşturma (NullReferenceException hatasını önler)
            cmbTables = new ComboBox();
            dgvProducts = new DataGridView();
            numQuantity = new NumericUpDown();
            dgvOrderDetails = new DataGridView();
            lblTotalPrice = new Label();
            btnAddOrderItem = new Button();
            btnCompleteOrder = new Button(); // Siparişi Tamamlama butonu
            btnRemoveOrderItem = new Button();
            btnPay = new Button(); // Ödeme butonu oluşturuldu

            // Sipariş detayları DataGridView'ini başlatır
            InitializeOrderGrid();

            // Formun temel ayarları
            this.Text = "Yeni Sipariş Oluştur / Fatura Öde"; // Başlık güncellendi
            this.StartPosition = FormStartPosition.CenterScreen; // Formu ekranın ortasında açar
            this.Size = new Size(1000, 750); // Formun boyutunu ayarlar
            this.BackColor = Color.FromArgb(255, 224, 230); // Pastel Pembe arka plan rengi

            // Başlık Label'ı oluşturma ve ayarlama
            Label titleLabel = new Label();
            titleLabel.Text = "Yeni Sipariş Oluştur / Fatura Öde"; // Başlık güncellendi
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
            backButton.Location = new Point(this.ClientSize.Width - backButton.Width - 20, 20); // Sağ üst köşe
            backButton.Click += (sender, e) => this.Close(); // Tıklandığında formu kapat
            this.Controls.Add(backButton); // Forma ekle

            // Masa Seçimi Label'ı oluşturma ve ayarlama
            Label lblSelectTable = new Label();
            lblSelectTable.Text = "Masa Seçin:";
            lblSelectTable.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblSelectTable.AutoSize = true;
            lblSelectTable.Location = new Point(50, titleLabel.Bottom + 20);
            this.Controls.Add(lblSelectTable);

            // Masa Seçimi ComboBox'ı ayarlama ve forma ekleme
            cmbTables.Font = new Font("Segoe UI", 12, FontStyle.Regular);
            cmbTables.Size = new Size(200, 30); // Genişletildi
            cmbTables.Location = new Point(lblSelectTable.Right + 10, titleLabel.Bottom + 20);
            cmbTables.DropDownStyle = ComboBoxStyle.DropDownList; // Sadece listeden seçim yapılabilir
            cmbTables.SelectedIndexChanged += CmbTables_SelectedIndexChanged; // Seçim değiştiğinde olay tetikle
            this.Controls.Add(cmbTables); // Forma ekle

            // Menüdeki Ürünler Label'ı oluşturma ve ayarlama
            Label lblProductsMenu = new Label();
            lblProductsMenu.Text = "Menüdeki Ürünler:";
            lblProductsMenu.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblProductsMenu.AutoSize = true;
            lblProductsMenu.Location = new Point(50, cmbTables.Bottom + 20);
            this.Controls.Add(lblProductsMenu);

            // Ürünler DataGridView'ini ayarlama ve forma ekleme
            dgvProducts.Location = new Point(50, lblProductsMenu.Bottom + 10);
            dgvProducts.Size = new Size(400, 300);
            dgvProducts.BackgroundColor = Color.White;
            dgvProducts.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(224, 255, 230); // Pastel Yeşil başlık
            dgvProducts.EnableHeadersVisualStyles = false; // Başlıkların görsel stilini kapat
            dgvProducts.RowHeadersVisible = false; // Sol baştaki boş satır başlıklarını gizle
            dgvProducts.SelectionMode = DataGridViewSelectionMode.FullRowSelect; // Tüm satırı seçilebilir yap
            dgvProducts.ReadOnly = true; // Sadece okunabilir
            dgvProducts.AllowUserToAddRows = false; // Kullanıcının satır eklemesini engelle
            dgvProducts.AllowUserToDeleteRows = false; // Kullanıcının satır silmesini engelle
            dgvProducts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; // Sütunları otomatik doldur
            this.Controls.Add(dgvProducts); // Forma ekle

            // Miktar Label'ı oluşturma ve ayarlama
            Label lblQuantity = new Label();
            lblQuantity.Text = "Miktar:";
            lblQuantity.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblQuantity.AutoSize = true;
            lblQuantity.Location = new Point(dgvProducts.Right + 30, dgvProducts.Top + 100);
            this.Controls.Add(lblQuantity);

            // Miktar NumericUpDown'ı ayarlama ve forma ekleme
            numQuantity.Font = new Font("Segoe UI", 12, FontStyle.Regular);
            numQuantity.Size = new Size(80, 30);
            numQuantity.Location = new Point(lblQuantity.Right + 10, lblQuantity.Top);
            numQuantity.Minimum = 1; // Minimum miktar 1
            numQuantity.Value = 1; // Başlangıç değeri 1
            this.Controls.Add(numQuantity); // Forma ekle

            // Siparişe Ekle Butonu ayarlama ve forma ekleme
            btnAddOrderItem.Text = "Siparişe Ekle >>";
            btnAddOrderItem.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            btnAddOrderItem.BackColor = Color.FromArgb(193, 230, 193); // Koyu pastel yeşil
            btnAddOrderItem.ForeColor = Color.Black;
            btnAddOrderItem.FlatStyle = FlatStyle.Flat;
            btnAddOrderItem.FlatAppearance.BorderSize = 0;
            btnAddOrderItem.Size = new Size(150, 40);
            btnAddOrderItem.Location = new Point(dgvProducts.Right + 30, numQuantity.Bottom + 20);
            btnAddOrderItem.Click += BtnAddOrderItem_Click; // Tıklandığında olay tetikle
            this.Controls.Add(btnAddOrderItem); // Forma ekle

            // Siparişten Çıkar Butonu ayarlama ve forma ekleme
            btnRemoveOrderItem.Text = "<< Siparişten Çıkar";
            btnRemoveOrderItem.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            btnRemoveOrderItem.BackColor = Color.FromArgb(255, 128, 128); // Kırmızı
            btnRemoveOrderItem.ForeColor = Color.White;
            btnRemoveOrderItem.FlatStyle = FlatStyle.Flat;
            btnRemoveOrderItem.FlatAppearance.BorderSize = 0;
            btnRemoveOrderItem.Size = new Size(180, 40);
            btnRemoveOrderItem.Location = new Point(dgvProducts.Right + 30, btnAddOrderItem.Bottom + 10);
            btnRemoveOrderItem.Click += BtnRemoveOrderItem_Click; // Tıklandığında olay tetikle
            this.Controls.Add(btnRemoveOrderItem); // Forma ekle

            // Mevcut Sipariş Label'ı oluşturma ve ayarlama
            Label lblOrderDetails = new Label();
            lblOrderDetails.Text = "Mevcut Sipariş:";
            lblOrderDetails.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblOrderDetails.AutoSize = true;
            lblOrderDetails.Location = new Point(btnAddOrderItem.Right + 30, cmbTables.Bottom + 20);
            this.Controls.Add(lblOrderDetails);

            // Sipariş Detayları DataGridView'ini ayarlama ve forma ekleme
            dgvOrderDetails.Location = new Point(btnAddOrderItem.Right + 30, lblOrderDetails.Bottom + 10);
            dgvOrderDetails.Size = new Size(400, 300);
            dgvOrderDetails.BackgroundColor = Color.White;
            dgvOrderDetails.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(255, 224, 230); // Pastel Pembe başlık
            dgvOrderDetails.EnableHeadersVisualStyles = false;
            dgvOrderDetails.RowHeadersVisible = false;
            dgvOrderDetails.SelectionMode = DataGridViewSelectionMode.FullRowSelect; // Tüm satırı seçilebilir yap
            dgvOrderDetails.ReadOnly = true; // Sadece okunabilir
            dgvOrderDetails.AllowUserToAddRows = false; // Kullanıcının satır eklemesini engelle
            dgvOrderDetails.AllowUserToDeleteRows = false; // Kullanıcının satır silmesini engelle
            dgvOrderDetails.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; // Sütunları otomatik doldur
            this.Controls.Add(dgvOrderDetails); // Forma ekle

            // Toplam Tutar Label'ı ayarlama ve forma ekleme
            lblTotalPrice.Text = "Toplam: 0.00 TL";
            lblTotalPrice.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            lblTotalPrice.ForeColor = Color.DarkRed;
            lblTotalPrice.AutoSize = true;
            this.Controls.Add(lblTotalPrice); // Forma ekle

            // Siparişi Tamamla Butonu ayarlama ve forma ekleme
            btnCompleteOrder.Text = "Siparişi Oluştur"; // Metin güncellendi
            btnCompleteOrder.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            btnCompleteOrder.BackColor = Color.FromArgb(193, 230, 193); // Koyu pastel yeşil
            btnCompleteOrder.ForeColor = Color.Black;
            btnCompleteOrder.FlatStyle = FlatStyle.Flat;
            btnCompleteOrder.FlatAppearance.BorderSize = 0;
            btnCompleteOrder.Size = new Size(250, 50); // Boyut güncellendi
            btnCompleteOrder.Location = new Point((this.ClientSize.Width / 2) - (btnCompleteOrder.Width / 2) - 130, this.ClientSize.Height - 80); // Konum güncellendi
            btnCompleteOrder.Click += BtnCreateOrder_Click; // Olay metodu adı güncellendi
            this.Controls.Add(btnCompleteOrder); // Forma ekle

            // Fatura Ödeme Butonu ayarlama ve forma ekleme
            btnPay.Text = "Fatura Öde"; // Metin güncellendi
            btnPay.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            btnPay.BackColor = Color.FromArgb(128, 193, 255); // Mavi tonu
            btnPay.ForeColor = Color.White;
            btnPay.FlatStyle = FlatStyle.Flat;
            btnPay.FlatAppearance.BorderSize = 0;
            btnPay.Size = new Size(250, 50); // Boyut güncellendi
            btnPay.Location = new Point((this.ClientSize.Width / 2) - (btnPay.Width / 2) + 130, this.ClientSize.Height - 80); // Konum güncellendi
            btnPay.Click += BtnPay_Click; // Tıklandığında olay tetikle
            btnPay.Enabled = false; // Başlangıçta devre dışı bırak
            this.Controls.Add(btnPay); // Forma ekle

            // Form yüklendiğinde çalışacak olay metodunu ata
            this.Load += NewOrderForm_Load;
        }

        /// <summary>
        /// Form yüklendiğinde çalışacak olay metodu.
        /// Masaları ve ürünleri yükler, sipariş gridini başlatır ve toplam tutarı hesaplar.
        /// </summary>
        private void NewOrderForm_Load(object sender, EventArgs e)
        {
            LoadTables(); // Masaları veritabanından yükle
            LoadProducts(); // Ürünleri veritabanından yükle
            CalculateTotalPrice(); // Başlangıçta toplam tutarı hesapla (genellikle 0.00 TL)
            UpdateControlStates(); // Kontrol durumlarını başlangıçta güncelle
        }

        /// <summary>
        /// Veritabanından masa bilgilerini yükler ve ComboBox'a bağlar.
        /// Şimdi tüm masaları (Boş, Rezervasyonlu, Dolu) listeler, çünkü dolu masaların da faturası ödenebilir.
        /// </summary>
        private void LoadTables()
        {
            try
            {
                // SQL sorgusu: Tüm masaları seç
                string query = "SELECT table_id, status FROM TableInfo";
                DataTable dt = DbHelper.GetData(query); // DbHelper ile veriyi al

                // ComboBox'ın görünen ve arka plandaki değerlerini ayarla
                cmbTables.DisplayMember = "DisplayMember"; // ComboBox'ta görünecek metin
                cmbTables.ValueMember = "table_id"; // Arka plandaki değer (masa ID'si)

                // Masa ID ve Durumu birleştirerek ComboBox'ta daha okunabilir bir metin oluştur
                DataTable displayDt = new DataTable();
                displayDt.Columns.Add("table_id", typeof(int));
                displayDt.Columns.Add("status", typeof(string)); // Masa durumu için sütun ekle
                displayDt.Columns.Add("DisplayMember", typeof(string));

                foreach (DataRow row in dt.Rows)
                {
                    DataRow newRow = displayDt.NewRow();
                    newRow["table_id"] = row["table_id"];
                    newRow["status"] = row["status"].ToString(); // Durumu da sakla
                    newRow["DisplayMember"] = $"Masa {row["table_id"]} ({row["status"]})";
                    displayDt.Rows.Add(newRow);
                }

                cmbTables.DataSource = displayDt; // ComboBox'a veriyi bağla

                // Eğer listede masa varsa ilkini varsayılan olarak seç
                if (cmbTables.Items.Count > 0)
                {
                    cmbTables.SelectedIndex = 0;
                }
                else
                {
                    MessageBox.Show("Masa bulunmamaktadır. Lütfen Masa Yönetimi ekranından masa ekleyiniz.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Masa bilgileri yüklenirken bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Veritabanından ürün bilgilerini yükler ve ürünler DataGridView'ine bağlar.
        /// </summary>
        private void LoadProducts()
        {
            try
            {
                string query = "SELECT product_id, product_name, price FROM Product";
                DataTable dt = DbHelper.GetData(query); // DbHelper ile veriyi al
                dgvProducts.DataSource = dt; // DataGridView'e bağla

                // DataGridView sütun başlıklarını ayarla ve ID sütununu gizle
                dgvProducts.Columns["product_name"].HeaderText = "Ürün Adı";
                dgvProducts.Columns["price"].HeaderText = "Fiyat (TL)";
                dgvProducts.Columns["product_id"].Visible = false; // ID'yi gizle

                // Fiyat sütununu para birimi olarak formatla
                if (dgvProducts.Columns.Contains("price"))
                {
                    dgvProducts.Columns["price"].DefaultCellStyle.Format = "C2"; // C2 para birimi formatı (örn: ₺12,34)
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ürünler yüklenirken bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Sipariş detayları DataGridView'i için sütunları tanımlar ve başlatır.
        /// </summary>
        private void InitializeOrderGrid()
        {
            currentOrderItems = new DataTable(); // Yeni bir DataTable oluştur
            // DataTable'a sütunları ekle
            currentOrderItems.Columns.Add("product_id", typeof(int));
            currentOrderItems.Columns.Add("Ürün Adı", typeof(string));
            currentOrderItems.Columns.Add("Adet", typeof(int));
            currentOrderItems.Columns.Add("Birim Fiyat", typeof(decimal));
            currentOrderItems.Columns.Add("Toplam", typeof(decimal)); // Ürün bazında toplam tutar

            // DataGridView'in otomatik sütun oluşturmasını kapat
            dgvOrderDetails.AutoGenerateColumns = false;

            // DataGridView'e manuel olarak sütunları ekle
            // Bu, sütunların her zaman var olmasını garanti eder.
            dgvOrderDetails.Columns.Clear(); // Önceki sütunları temizle (eğer varsa)

            dgvOrderDetails.Columns.Add(new DataGridViewTextBoxColumn() { Name = "product_id", DataPropertyName = "product_id", Visible = false });
            dgvOrderDetails.Columns.Add(new DataGridViewTextBoxColumn() { Name = "Ürün Adı", DataPropertyName = "Ürün Adı", HeaderText = "Ürün Adı" });
            dgvOrderDetails.Columns.Add(new DataGridViewTextBoxColumn() { Name = "Adet", DataPropertyName = "Adet", HeaderText = "Adet" });
            dgvOrderDetails.Columns.Add(new DataGridViewTextBoxColumn() { Name = "Birim Fiyat", DataPropertyName = "Birim Fiyat", HeaderText = "Birim Fiyat", DefaultCellStyle = { Format = "C2" } });
            dgvOrderDetails.Columns.Add(new DataGridViewTextBoxColumn() { Name = "Toplam", DataPropertyName = "Toplam", HeaderText = "Toplam", DefaultCellStyle = { Format = "C2" } });

            // DataGridView'e DataTable'ı bağla
            dgvOrderDetails.DataSource = currentOrderItems;

            // Toplam tutar label'ının konumunu sağa doğru dinamik olarak ayarlar.
            // Label'ın genişliği metin içeriğine göre değiştiği için bu önemlidir.
            lblTotalPrice.Location = new Point(dgvOrderDetails.Right - lblTotalPrice.PreferredWidth, dgvOrderDetails.Bottom + 20);
        }

        /// <summary>
        /// Masa seçimi değiştiğinde çalışır. Seçili masayı günceller ve siparişi sıfırlar.
        /// </summary>
        private void CmbTables_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Seçili değerin null olmadığını ve int'e dönüştürülebilir olduğunu kontrol et
            if (cmbTables.SelectedValue != null && cmbTables.SelectedValue is int)
            {
                selectedTableId = Convert.ToInt32(cmbTables.SelectedValue);
                // Seçili masanın durumunu da al
                DataRowView selectedRow = cmbTables.SelectedItem as DataRowView;
                if (selectedRow != null)
                {
                    selectedTableStatus = selectedRow["status"].ToString();
                }

                ResetOrder(); // Masa değişince mevcut siparişi sıfırla
                LoadCurrentOrderForTable(selectedTableId); // Seçili masanın bekleyen siparişini yükle
                UpdateControlStates(); // Kontrol durumlarını güncelle
            }
            else if (cmbTables.SelectedValue == null || cmbTables.Items.Count == 0)
            {
                selectedTableId = -1;
                selectedTableStatus = "";
                ResetOrder();
                UpdateControlStates(); // Kontrol durumlarını güncelle
            }
        }

        /// <summary>
        /// Seçili masanın varsa açık (ödeme bekleyen) siparişini yükler.
        /// </summary>
        /// <param name="tableId">Siparişin yükleneceği masanın ID'si.</param>
        private void LoadCurrentOrderForTable(int tableId)
        {
            currentOrderItems.Clear(); // Önceki siparişleri temizle
            currentBillId = -1; // Fatura ID'sini sıfırla

            try
            {
                // Ödeme durumu 'Ödenmedi' olan bir fatura var mı kontrol et
                string billQuery = "SELECT bill_id, total_amount FROM Bill WHERE table_id = @tableId AND payment_status = 'Ödenmedi'";
                SqlParameter[] billParams = { new SqlParameter("@tableId", tableId) };
                DataTable billDt = DbHelper.GetData(billQuery, billParams);

                if (billDt != null && billDt.Rows.Count > 0)
                {
                    currentBillId = Convert.ToInt32(billDt.Rows[0]["bill_id"]);
                    // Sipariş kalemlerini yükle
                    string orderItemsQuery = @"
                        SELECT oi.product_id, p.product_name, oi.quantity, p.price 
                        FROM OrderInfo oi 
                        JOIN Product p ON oi.product_id = p.product_id 
                        WHERE oi.bill_id = @billId";
                    SqlParameter[] orderItemsParams = { new SqlParameter("@billId", currentBillId) };
                    DataTable orderItemsDt = DbHelper.GetData(orderItemsQuery, orderItemsParams);

                    foreach (DataRow itemRow in orderItemsDt.Rows)
                    {
                        DataRow newRow = currentOrderItems.NewRow();
                        newRow["product_id"] = itemRow.Field<int>("product_id");
                        newRow["Ürün Adı"] = itemRow.Field<string>("product_name");
                        newRow["Adet"] = itemRow.Field<int>("quantity");
                        newRow["Birim Fiyat"] = itemRow.Field<decimal>("price");
                        newRow["Toplam"] = itemRow.Field<int>("quantity") * itemRow.Field<decimal>("price");
                        currentOrderItems.Rows.Add(newRow);
                    }
                    MessageBox.Show($"Masa {tableId} için bekleyen bir sipariş yüklendi. Fatura ID: {currentBillId}", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // Yeni sipariş oluşturulacak, bekleyen fatura yok
                    currentBillId = -1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Mevcut sipariş yüklenirken bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            CalculateTotalPrice(); // Toplam tutarı yeniden hesapla
        }

        /// <summary>
        /// Seçili ürünü ve miktarı sipariş listesine ekler.
        /// </summary>
        private void BtnAddOrderItem_Click(object sender, EventArgs e)
        {
            if (selectedTableId == -1)
            {
                MessageBox.Show("Lütfen önce bir masa seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (dgvProducts.SelectedRows.Count == 0)
            {
                MessageBox.Show("Lütfen siparişe eklemek için bir ürün seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Seçili ürünün bilgilerini al
            DataGridViewRow selectedProductRow = dgvProducts.SelectedRows[0];
            int productId = Convert.ToInt32(selectedProductRow.Cells["product_id"].Value);
            string productName = selectedProductRow.Cells["product_name"].Value.ToString();
            decimal price = Convert.ToDecimal(selectedProductRow.Cells["price"].Value);
            int quantity = Convert.ToInt32(numQuantity.Value);

            // Sipariş listesinde bu ürün daha önce eklenmiş mi kontrol et
            DataRow existingItem = currentOrderItems.AsEnumerable()
                                    .FirstOrDefault(row => row.Field<int>("product_id") == productId);

            if (existingItem != null)
            {
                // Ürün zaten varsa miktarını ve toplamını güncelle
                existingItem.SetField("Adet", existingItem.Field<int>("Adet") + quantity);
                existingItem.SetField("Toplam", existingItem.Field<int>("Adet") * existingItem.Field<decimal>("Birim Fiyat"));
            }
            else
            {
                // Ürün yoksa yeni satır olarak ekle
                DataRow newRow = currentOrderItems.NewRow();
                newRow["product_id"] = productId;
                newRow["Ürün Adı"] = productName;
                newRow["Adet"] = quantity;
                newRow["Birim Fiyat"] = price;
                newRow["Toplam"] = quantity * price;
                currentOrderItems.Rows.Add(newRow);
            }

            CalculateTotalPrice(); // Toplam tutarı yeniden hesapla
            numQuantity.Value = 1; // Miktarı varsayılana geri getir
            UpdateControlStates(); // Kontrol durumlarını güncelle
        }

        /// <summary>
        /// Seçili ürünü sipariş listesinden çıkarır.
        /// </summary>
        private void BtnRemoveOrderItem_Click(object sender, EventArgs e)
        {
            if (dgvOrderDetails.SelectedRows.Count == 0)
            {
                MessageBox.Show("Lütfen siparişten çıkarmak için bir ürün seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int selectedRowIndex = dgvOrderDetails.SelectedRows[0].Index;
            currentOrderItems.Rows.RemoveAt(selectedRowIndex); // Seçili satırı listeden kaldır

            CalculateTotalPrice(); // Toplam tutarı yeniden hesapla
            UpdateControlStates(); // Kontrol durumlarını güncelle
        }

        /// <summary>
        /// Siparişi veritabanına kaydeder (Bill ve OrderInfo tablolarına).
        /// Masa durumunu 'Dolu' olarak günceller ve payment_status'ı 'Ödenmedi' olarak ayarlar.
        /// </summary>
        private void BtnCreateOrder_Click(object sender, EventArgs e) // Metot adı BtnCompleteOrder_Click'ten BtnCreateOrder_Click'e değişti
        {
            if (selectedTableId == -1)
            {
                MessageBox.Show("Lütfen sipariş oluşturmadan önce bir masa seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (currentOrderItems.Rows.Count == 0)
            {
                MessageBox.Show("Sipariş listesi boş, oluşturulacak bir şey yok.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Eğer zaten bekleyen bir fatura varsa, yeni sipariş oluşturmaya izin verme
            if (currentBillId != -1)
            {
                MessageBox.Show("Bu masa için zaten bekleyen bir sipariş var. Ürün eklemeye devam edebilir veya faturayı ödeyebilirsiniz.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                // Siparişin toplam tutarını hesapla
                decimal finalTotal = currentOrderItems.AsEnumerable().Sum(row => row.Field<decimal>("Toplam"));

                // 1. Yeni Bill (Fatura) kaydı oluştur
                // payment_status sütununu ekledik ve başlangıç değeri 'Ödenmedi' olarak belirliyoruz.
                string billQuery = "INSERT INTO Bill (table_id, total_amount, date, payment_status) VALUES (@tableId, @totalAmount, GETDATE(), 'Ödenmedi'); SELECT SCOPE_IDENTITY();";
                SqlParameter[] billParams = new SqlParameter[]
                {
                    new SqlParameter("@tableId", selectedTableId),
                    new SqlParameter("@totalAmount", finalTotal)
                };

                // Fatura ID'sini alırken hata kontrolü ekleyelim
                int newGeneratedBillId = DbHelper.ExecuteScalarReturnId(billQuery, billParams);
                if (newGeneratedBillId <= 0)
                {
                    MessageBox.Show("Fatura ana kaydı oluşturulamadı. Veritabanı işlemi başarısız.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return; // Hata oluştuysa devam etme
                }
                currentBillId = newGeneratedBillId; // Başarılıysa ID'yi ata

                // 2. Her bir sipariş kalemini OrderInfo tablosuna kaydet
                foreach (DataRow row in currentOrderItems.Rows)
                {
                    string orderInfoQuery = "INSERT INTO OrderInfo (bill_id, product_id, quantity) VALUES (@billId, @productId, @quantity)";
                    SqlParameter[] orderInfoParams = new SqlParameter[]
                    {
                        new SqlParameter("@billId", currentBillId),
                        new SqlParameter("@productId", row.Field<int>("product_id")),
                        new SqlParameter("@quantity", row.Field<int>("Adet"))
                    };
                    int orderItemAffectedRows = DbHelper.ExecuteNonQuery(orderInfoQuery, orderInfoParams);
                    if (orderItemAffectedRows <= 0)
                    {
                        // Bu noktada işlem geri alınmalı veya loglanmalı (daha ileri seviye)
                        MessageBox.Show($"Ürün '{row.Field<string>("Ürün Adı")}' sipariş detaylarına eklenirken hata oluştu. Fatura kısmen kaydedilmiş olabilir.", "Kısmi Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        // Kısmi kayıt durumunda ne yapılacağına karar verilebilir (örneğin faturayı silmek)
                        return;
                    }
                }

                // 3. Masa durumunu 'Dolu' yap (sipariş alındığı için)
                string updateTableStatusQuery = "UPDATE TableInfo SET status = 'Dolu' WHERE table_id = @tableId";
                SqlParameter[] updateTableParams = new SqlParameter[]
                {
                    new SqlParameter("@tableId", selectedTableId)
                };
                int tableStatusAffectedRows = DbHelper.ExecuteNonQuery(updateTableStatusQuery, updateTableParams);
                if (tableStatusAffectedRows <= 0)
                {
                    MessageBox.Show($"Masa {selectedTableId} durumu 'Dolu' olarak güncellenirken bir sorun oluştu.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                MessageBox.Show($"Sipariş başarıyla oluşturuldu. Fatura ID: {currentBillId}\nMasa Durumu: Dolu", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Sipariş oluşturulduktan sonra DataGridView'i temizle ama masa seçimini koru
                currentOrderItems.Clear();
                CalculateTotalPrice();
                LoadTables(); // Masa listesini yenile (durum güncellendiği için)
                // cmbTables.SelectedValue = selectedTableId; // Mevcut masayı seçili bırak
                UpdateControlStates(); // Kontrol durumlarını güncelle
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sipariş oluşturulurken beklenmeyen bir hata oluştu: " + ex.Message + "\nDetay: " + (ex.InnerException != null ? ex.InnerException.Message : "Yok"), "Kritik Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Faturayı ödeme işlemini gerçekleştirir ve masa durumunu 'Boş' olarak günceller.
        /// </summary>
        private void BtnPay_Click(object sender, EventArgs e) // Metot adı BtnPayBill_Click'ten BtnPay_Click'e değişti
        {
            if (selectedTableId == -1)
            {
                MessageBox.Show("Lütfen fatura ödemeden önce bir masa seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Ödeme bekleyen fatura olup olmadığını kontrol et
            if (currentBillId == -1) // currentBillId, LoadCurrentOrderForTable'dan gelmeli
            {
                MessageBox.Show("Bu masa için ödenecek bir fatura bulunamadı.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Ödeme onayı al
            DialogResult dialogResult = MessageBox.Show($"Masa {selectedTableId} için {lblTotalPrice.Text} tutarındaki faturayı ödemek istediğinizden emin misiniz?", "Fatura Ödeme Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    // 1. Bill tablosundaki ödeme durumunu 'Ödendi' ve ödeme tarihini güncelleyin
                    string updateBillQuery = "UPDATE Bill SET payment_status = 'Ödendi', payment_date = GETDATE() WHERE bill_id = @billId";
                    SqlParameter[] updateBillParams = { new SqlParameter("@billId", currentBillId) };
                    int billAffectedRows = DbHelper.ExecuteNonQuery(updateBillQuery, updateBillParams);

                    if (billAffectedRows <= 0)
                    {
                        MessageBox.Show("Fatura ödeme durumu güncellenirken bir hata oluştu.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // 2. Masa durumunu 'Boş' olarak güncelleyin
                    string updateTableStatusQuery = "UPDATE TableInfo SET status = 'Boş' WHERE table_id = @tableId";
                    SqlParameter[] updateTableParams = { new SqlParameter("@tableId", selectedTableId) };
                    int tableStatusAffectedRows = DbHelper.ExecuteNonQuery(updateTableStatusQuery, updateTableParams);

                    if (tableStatusAffectedRows <= 0)
                    {
                        MessageBox.Show($"Masa {selectedTableId} durumu 'Boş' olarak güncellenirken bir sorun oluştu.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    MessageBox.Show($"Fatura başarıyla ödendi. Masa {selectedTableId} şimdi 'Boş'.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ResetOrder(); // Siparişi ve fatura ID'sini sıfırla
                    LoadTables(); // Masa listesini yenile (durum güncellendiği için)
                    UpdateControlStates(); // Kontrol durumlarını güncelle
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Fatura ödenirken beklenmeyen bir hata oluştu: " + ex.Message + "\nDetay: " + (ex.InnerException != null ? ex.InnerException.Message : "Yok"), "Kritik Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Siparişin toplam tutarını hesaplar ve Label'da gösterir.
        /// </summary>
        private void CalculateTotalPrice()
        {
            decimal total = currentOrderItems.AsEnumerable().Sum(row => row.Field<decimal>("Toplam"));
            lblTotalPrice.Text = $"Toplam: {total:C2} TL"; // Para birimi formatında gösterim (örn: ₺12,34)

            // Toplam tutar label'ının konumunu sağa doğru dinamik olarak ayarlar.
            // Label'ın genişliği metin içeriğine göre değiştiği için bu önemlidir.
            lblTotalPrice.Location = new Point(dgvOrderDetails.Right - lblTotalPrice.PreferredWidth, dgvOrderDetails.Bottom + 20);
        }

        /// <summary>
        /// Mevcut siparişi sıfırlar, DataGridView'i temizler ve başlangıç durumuna getirir.
        /// </summary>
        private void ResetOrder()
        {
            currentOrderItems.Clear(); // Sipariş listesini temizle
            CalculateTotalPrice(); // Toplamı sıfırla (0.00 TL gösterecek)
            currentBillId = -1; // Fatura ID'sini sıfırla
            activeBillIdForPayment = -1; // Ödeme için aktif fatura ID'sini sıfırla
            numQuantity.Value = 1; // Miktarı varsayılana getir
        }

        /// <summary>
        /// Seçili masa durumuna ve mevcut siparişin varlığına göre butonların ve diğer kontrollerin
        /// (Siparişe Ekle, Siparişi Oluştur, Fatura Öde, Siparişten Çıkar) görünürlüğünü/etkinliğini ayarlar.
        /// </summary>
        private void UpdateControlStates()
        {
            // Masa seçili değilse her şeyi devre dışı bırak
            if (selectedTableId == -1)
            {
                btnAddOrderItem.Enabled = false;
                btnRemoveOrderItem.Enabled = false;
                btnCompleteOrder.Enabled = false;
                btnPay.Enabled = false;
                dgvProducts.Enabled = false;
                dgvOrderDetails.Enabled = false;
                numQuantity.Enabled = false;
                return;
            }

            // Sipariş DataGridView'inde öğe olup olmadığını kontrol et
            bool hasOrderItems = currentOrderItems.Rows.Count > 0;

            // Masanın mevcut durumunu kontrol et
            // Eğer masa 'Boş' veya 'Rezervasyonlu' ise, yeni sipariş oluşturulabilir.
            if (selectedTableStatus == "Boş" || selectedTableStatus == "Rezervasyonlu")
            {
                // Eğer bu masada bekleyen bir fatura yoksa, yeni sipariş alabiliriz.
                // LoadCurrentOrderForTable metodu currentBillId'yi güncellediği için burayı kontrol etmeliyiz.
                if (currentBillId == -1) // Bekleyen fatura yoksa
                {
                    btnAddOrderItem.Enabled = true; // Ürün eklenebilir
                    btnRemoveOrderItem.Enabled = hasOrderItems; // Ürün varsa çıkarılabilir
                    btnCompleteOrder.Enabled = hasOrderItems; // Ürün varsa sipariş oluşturulabilir
                    btnPay.Enabled = false; // Ödeme butonu pasif
                    dgvProducts.Enabled = true;
                    dgvOrderDetails.Enabled = true;
                    numQuantity.Enabled = true;
                }
                else // Bekleyen fatura varsa (bu durum normalde olmamalı ama önlem olarak)
                {
                    btnAddOrderItem.Enabled = false; // Yeni ürün eklenemez
                    btnRemoveOrderItem.Enabled = false; // Ürün çıkarılamaz
                    btnCompleteOrder.Enabled = false; // Yeni sipariş oluşturulamaz
                    btnPay.Enabled = true; // Ödeme butonu aktif (çünkü bekleyen fatura var)
                    dgvProducts.Enabled = false;
                    dgvOrderDetails.Enabled = true; // Detaylar görülebilir
                    numQuantity.Enabled = false;
                }
            }
            // Eğer masa 'Dolu' ise, sadece ödeme yapılabilir veya mevcut siparişe devam edilebilir.
            else if (selectedTableStatus == "Dolu")
            {
                // Masa doluysa, ödenmemiş bir fatura olup olmadığını kontrol et
                // LoadCurrentOrderForTable metodu currentBillId'yi güncellediği için burayı kontrol etmeliyiz.
                if (currentBillId != -1) // Bekleyen fatura varsa
                {
                    btnAddOrderItem.Enabled = false; // Yeni ürün eklenemez (sadece mevcut siparişe devam)
                    btnRemoveOrderItem.Enabled = hasOrderItems; // Ürün varsa çıkarılabilir
                    btnCompleteOrder.Enabled = false; // Yeni sipariş oluşturulamaz
                    btnPay.Enabled = true; // Ödeme butonu aktif
                    dgvProducts.Enabled = true; // Ürünler görülebilir
                    dgvOrderDetails.Enabled = true;
                    numQuantity.Enabled = false; // Miktar değiştirilemez
                }
                else // Masa dolu ama bekleyen fatura yoksa (tutarsız durum)
                {
                    btnAddOrderItem.Enabled = false;
                    btnRemoveOrderItem.Enabled = false;
                    btnCompleteOrder.Enabled = false;
                    btnPay.Enabled = false;
                    dgvProducts.Enabled = false;
                    dgvOrderDetails.Enabled = false;
                    numQuantity.Enabled = false;
                    MessageBox.Show($"Masa {selectedTableId} dolu görünüyor ancak bekleyen bir fatura bulunamadı. Lütfen Masa Yönetimi ekranından durumu kontrol ediniz.", "Veri Tutarsızlığı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            // Diğer durumlar (örn: 'Ödendi' - ki bu ComboBox'ta listelenmemeli ama yine de önlem)
            else
            {
                btnAddOrderItem.Enabled = false;
                btnRemoveOrderItem.Enabled = false;
                btnCompleteOrder.Enabled = false;
                btnPay.Enabled = false;
                dgvProducts.Enabled = false;
                dgvOrderDetails.Enabled = false;
                numQuantity.Enabled = false;
            }

            // Eğer hiç masa yoksa veya masa seçimi kalkarsa tüm butonları devre dışı bırak
            if (cmbTables.Items.Count == 0 || cmbTables.SelectedValue == null)
            {
                btnAddOrderItem.Enabled = false;
                btnRemoveOrderItem.Enabled = false;
                btnCompleteOrder.Enabled = false;
                btnPay.Enabled = false;
                dgvProducts.Enabled = false;
                dgvOrderDetails.Enabled = false;
                numQuantity.Enabled = false;
            }
        }
    }
}