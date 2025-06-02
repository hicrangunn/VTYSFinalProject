using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data; // DataTable için gerekli
using System.Data.SqlClient; // SqlParameter için gerekli
using System.Linq; // LINQ metotları (AsEnumerable, FirstOrDefault) için gerekli
using SiparisYonetimUygulamasi.DAL; // DbHelper için gerekli

namespace SiparisYonetimUygulamasi
{
    public partial class ProductManagementForm : Form
    {
        // Kontrol referanslarını sınıf seviyesinde tanımlayalım ki metotlar arası erişebilelim
        private TextBox txtProductName;
        private TextBox txtPrice;
        private DataGridView dgvProducts;
        private Button btnAddProduct;
        private Button btnUpdateProduct;
        private Button btnDeleteProduct;

        // Seçili ürünün ID'sini tutmak için (güncelleme/silme işlemlerinde kullanılacak)
        private int selectedProductId = -1;

        public ProductManagementForm()
        {
            InitializeComponent(); // Tasarımcı tarafından oluşturulan bileşenleri başlatır

            // Kontrol nesnelerini bellekte oluşturma
            txtProductName = new TextBox();
            txtPrice = new TextBox();
            dgvProducts = new DataGridView();
            btnAddProduct = new Button();
            btnUpdateProduct = new Button();
            btnDeleteProduct = new Button();

            // Form Ayarları
            this.Text = "Ürün Yönetimi";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(800, 600);
            this.BackColor = Color.FromArgb(224, 255, 230); // Pastel Yeşil

            // Başlık Label'ı oluşturma ve ayarlama
            Label titleLabel = new Label();
            titleLabel.Text = "Ürün Yönetimi";
            titleLabel.Font = new Font("Segoe UI", 30, FontStyle.Bold);
            titleLabel.ForeColor = Color.Black;
            titleLabel.TextAlign = ContentAlignment.TopCenter;
            titleLabel.AutoSize = false;
            titleLabel.Width = this.ClientSize.Width;
            titleLabel.Height = 70;
            titleLabel.Location = new Point(0, 20);
            this.Controls.Add(titleLabel);

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
            backButton.Click += (sender, e) => this.Close();
            this.Controls.Add(backButton);

            // *******************************************************************
            // Ürün Giriş Alanları ve Butonlar
            // *******************************************************************

            int currentY = titleLabel.Bottom + 20; // Başlığın altından başla

            // Ürün Adı Label ve TextBox
            Label lblProductName = new Label();
            lblProductName.Text = "Ürün Adı:";
            lblProductName.Font = new Font("Segoe UI", 12, FontStyle.Regular);
            lblProductName.AutoSize = true;
            lblProductName.Location = new Point(50, currentY);
            this.Controls.Add(lblProductName);

            txtProductName.Font = new Font("Segoe UI", 12, FontStyle.Regular);
            txtProductName.Size = new Size(250, 30);
            txtProductName.Location = new Point(lblProductName.Right + 10, currentY);
            this.Controls.Add(txtProductName);

            currentY += 40; // Bir sonraki satır için Y konumunu artır

            // Fiyat Label ve TextBox
            Label lblPrice = new Label();
            lblPrice.Text = "Fiyat:";
            lblPrice.Font = new Font("Segoe UI", 12, FontStyle.Regular);
            lblPrice.AutoSize = true;
            lblPrice.Location = new Point(50, currentY);
            this.Controls.Add(lblPrice);

            txtPrice.Font = new Font("Segoe UI", 12, FontStyle.Regular);
            txtPrice.Size = new Size(150, 30);
            txtPrice.Location = new Point(lblPrice.Right + 47, currentY); // Hizalama için biraz daha boşluk
            this.Controls.Add(txtPrice);

            currentY += 60; // Butonlar için daha fazla boşluk

            // Butonlar
            btnAddProduct.Text = "Ürün Ekle";
            btnAddProduct.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            btnAddProduct.BackColor = Color.FromArgb(193, 230, 193); // Biraz daha koyu pastel yeşil
            btnAddProduct.ForeColor = Color.Black;
            btnAddProduct.FlatStyle = FlatStyle.Flat;
            btnAddProduct.FlatAppearance.BorderSize = 0;
            btnAddProduct.Size = new Size(120, 40);
            btnAddProduct.Location = new Point(50, currentY);
            btnAddProduct.Click += BtnAddProduct_Click; // Click olayı ataması
            this.Controls.Add(btnAddProduct);

            btnUpdateProduct.Text = "Ürün Güncelle";
            btnUpdateProduct.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            btnUpdateProduct.BackColor = Color.FromArgb(255, 192, 203); // Açık Pembe
            btnUpdateProduct.ForeColor = Color.Black;
            btnUpdateProduct.FlatStyle = FlatStyle.Flat;
            btnUpdateProduct.FlatAppearance.BorderSize = 0;
            btnUpdateProduct.Size = new Size(150, 40);
            btnUpdateProduct.Location = new Point(btnAddProduct.Right + 20, currentY);
            btnUpdateProduct.Click += BtnUpdateProduct_Click; // Click olayı ataması
            this.Controls.Add(btnUpdateProduct);

            btnDeleteProduct.Text = "Ürün Sil";
            btnDeleteProduct.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            btnDeleteProduct.BackColor = Color.FromArgb(255, 128, 128); // Daha koyu kırmızı
            btnDeleteProduct.ForeColor = Color.White;
            btnDeleteProduct.FlatStyle = FlatStyle.Flat;
            btnDeleteProduct.FlatAppearance.BorderSize = 0;
            btnDeleteProduct.Size = new Size(120, 40);
            btnDeleteProduct.Location = new Point(btnUpdateProduct.Right + 20, currentY);
            btnDeleteProduct.Click += BtnDeleteProduct_Click; // Click olayı ataması
            this.Controls.Add(btnDeleteProduct);

            currentY += 70; // DataGridView için boşluk

            // *******************************************************************
            // DataGridView (Ürün Listesi)
            // *******************************************************************
            dgvProducts.Location = new Point(50, currentY);
            dgvProducts.Size = new Size(700, 300);
            dgvProducts.BackgroundColor = Color.White;
            dgvProducts.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(193, 230, 193); // Başlık rengi
            dgvProducts.EnableHeadersVisualStyles = false;
            dgvProducts.RowHeadersVisible = false; // Sol baştaki boş sütunu gizle
            dgvProducts.SelectionMode = DataGridViewSelectionMode.FullRowSelect; // Tüm satırı seçsin
            dgvProducts.ReadOnly = true; // Sadece okunabilir
            dgvProducts.AllowUserToAddRows = false; // Kullanıcının satır eklemesini engelle
            dgvProducts.AllowUserToDeleteRows = false; // Kullanıcının satır silmesini engelle
            dgvProducts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; // Sütunları doldur
            dgvProducts.CellClick += DgvProducts_CellClick; // Satır seçimi için olay ataması
            this.Controls.Add(dgvProducts);

            // Form yüklendiğinde ürünleri yükle
            this.Load += (sender, e) => LoadProducts();
        }

        /// <summary>
        /// Ürünleri veritabanından yükler ve DataGridView'e bağlar.
        /// </summary>
        private void LoadProducts()
        {
            try
            {
                string query = "SELECT product_id, product_name, price FROM Product";
                DataTable dt = DbHelper.GetData(query);
                dgvProducts.DataSource = dt;

                // DataGridView sütun başlıklarını ve formatlarını ayarlar
                if (dgvProducts.Columns.Contains("product_name"))
                    dgvProducts.Columns["product_name"].HeaderText = "Ürün Adı";
                if (dgvProducts.Columns.Contains("price"))
                {
                    dgvProducts.Columns["price"].HeaderText = "Fiyat (TL)";
                    dgvProducts.Columns["price"].DefaultCellStyle.Format = "C2"; // Para birimi formatı
                }
                if (dgvProducts.Columns.Contains("product_id"))
                    dgvProducts.Columns["product_id"].Visible = false; // ID'yi gizle

                ClearProductInputs(); // Giriş alanlarını temizle
                selectedProductId = -1; // Seçili ID'yi sıfırla
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ürünler yüklenirken bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Yeni ürün ekleme işlemini gerçekleştirir.
        /// </summary>
        private void BtnAddProduct_Click(object sender, EventArgs e)
        {
            string productName = txtProductName.Text.Trim();
            decimal price;

            if (string.IsNullOrEmpty(productName))
            {
                MessageBox.Show("Ürün adı boş bırakılamaz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!decimal.TryParse(txtPrice.Text, out price))
            {
                MessageBox.Show("Geçerli bir fiyat giriniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string query = "INSERT INTO Product (product_name, price) VALUES (@productName, @price)";
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@productName", productName),
                    new SqlParameter("@price", price)
                };

                int affectedRows = DbHelper.ExecuteNonQuery(query, parameters);
                if (affectedRows > 0)
                {
                    MessageBox.Show("Ürün başarıyla eklendi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadProducts(); // Listeyi yenile
                }
                else
                {
                    MessageBox.Show("Ürün eklenirken bir sorun oluştu.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ürün eklenirken bir hata oluştu: " + ex.Message + "\nDetay: " + (ex.InnerException != null ? ex.InnerException.Message : "Yok"), "Kritik Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Seçili ürünü günceller.
        /// </summary>
        private void BtnUpdateProduct_Click(object sender, EventArgs e)
        {
            if (selectedProductId == -1)
            {
                MessageBox.Show("Lütfen güncellemek için bir ürün seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string productName = txtProductName.Text.Trim();
            decimal price;

            if (string.IsNullOrEmpty(productName))
            {
                MessageBox.Show("Ürün adı boş bırakılamaz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!decimal.TryParse(txtPrice.Text, out price))
            {
                MessageBox.Show("Geçerli bir fiyat giriniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string query = "UPDATE Product SET product_name = @productName, price = @price WHERE product_id = @productId";
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@productName", productName),
                    new SqlParameter("@price", price),
                    new SqlParameter("@productId", selectedProductId)
                };

                int affectedRows = DbHelper.ExecuteNonQuery(query, parameters);
                if (affectedRows > 0)
                {
                    MessageBox.Show("Ürün başarıyla güncellendi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadProducts(); // Listeyi yenile
                }
                else
                {
                    MessageBox.Show("Ürün güncellenirken bir sorun oluştu veya değişiklik yapılmadı.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ürün güncellenirken bir hata oluştu: " + ex.Message + "\nDetay: " + (ex.InnerException != null ? ex.InnerException.Message : "Yok"), "Kritik Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Seçili ürünü veritabanından siler.
        /// </summary>
        private void BtnDeleteProduct_Click(object sender, EventArgs e)
        {
            if (selectedProductId == -1)
            {
                MessageBox.Show("Lütfen silmek için bir ürün seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult dialogResult = MessageBox.Show("Seçili ürünü silmek istediğinizden emin misiniz? Bu işlem geri alınamaz.", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    // Ürünün siparişlerde kullanılıp kullanılmadığını kontrol et
                    string checkQuery = "SELECT COUNT(*) FROM OrderInfo WHERE product_id = @productId";
                    SqlParameter[] checkParams = new SqlParameter[]
                    {
                        new SqlParameter("@productId", selectedProductId)
                    };
                    // ExecuteScalar tek bir değer döndürür, burada int'e dönüştürüyoruz
                    // GetData metodu DataTable döndürdüğü için Rows[0][0] ile ilk hücrenin değerini alıyoruz.
                    int usageCount = Convert.ToInt32(DbHelper.GetData(checkQuery, checkParams).Rows[0][0]);

                    if (usageCount > 0)
                    {
                        MessageBox.Show("Bu ürün mevcut siparişlerde kullanıldığı için silinemez. Önce ilgili siparişleri silmelisiniz.", "Silme Hatası", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    string deleteQuery = "DELETE FROM Product WHERE product_id = @productId";
                    SqlParameter[] deleteParams = new SqlParameter[]
                    {
                        new SqlParameter("@productId", selectedProductId)
                    };

                    int affectedRows = DbHelper.ExecuteNonQuery(deleteQuery, deleteParams);
                    if (affectedRows > 0)
                    {
                        MessageBox.Show("Ürün başarıyla silindi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadProducts(); // Listeyi yenile
                    }
                    else
                    {
                        MessageBox.Show("Ürün silinirken bir sorun oluştu veya ürün bulunamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ürün silinirken beklenmeyen bir hata oluştu: " + ex.Message + "\nDetay: " + (ex.InnerException != null ? ex.InnerException.Message : "Yok"), "Kritik Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// DataGridView'de bir hücreye tıklandığında, seçilen ürünün bilgilerini giriş alanlarına doldurur.
        /// </summary>
        private void DgvProducts_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) // Başlık satırına tıklanmadığından emin ol
            {
                DataGridViewRow row = this.dgvProducts.Rows[e.RowIndex];
                selectedProductId = Convert.ToInt32(row.Cells["product_id"].Value);
                txtProductName.Text = row.Cells["product_name"].Value.ToString();
                txtPrice.Text = row.Cells["price"].Value.ToString();
            }
        }

        /// <summary>
        /// Ürün giriş alanlarını temizler ve seçili ürün ID'sini sıfırlar.
        /// </summary>
        private void ClearProductInputs()
        {
            txtProductName.Clear();
            txtPrice.Clear();
            selectedProductId = -1; // Seçili ID'yi sıfırla
        }
    }
}