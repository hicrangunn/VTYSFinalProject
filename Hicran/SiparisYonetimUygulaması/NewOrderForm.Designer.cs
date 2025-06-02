namespace SiparisYonetimUygulamasi // Bu namespace projenizin ana namespace'i olmalı
{
    partial class NewOrderForm
    {
        /// <summary>
        /// Gerekli tasarımcı değişkeni.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Kullanılmakta olan tüm kaynakları temizleyin.
        /// </summary>
        /// <param name="disposing">Yönetilen kaynaklar atılmalıysa doğru; aksi takdirde yanlış.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer üretilen kod

        /// <summary>
        /// Tasarımcı desteği için gerekli metot - kod düzenleyiciyi
        /// değiştirmeyin.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1000, 750); // NewOrderForm için belirlediğimiz boyut
            this.Text = "NewOrderForm"; // Formun varsayılan başlığı

            // NOT: Bu dosya genellikle Visual Studio tarafından otomatik olarak güncellenir.
            // Manuel olarak eklediğimiz kontrollerin (ComboBox, DataGridView'ler, Button'lar vb.)
            // InitializeComponent() içinde tanımlanması gerekir. Ancak biz bu kontrolleri
            // NewOrderForm.cs'nin constructor'ı içinde manuel olarak oluşturduğumuz için,
            // bu Designer dosyasının içeriği sadece formun temel boyutlarını ve Text özelliğini
            // tanımlayan minimal bir yapıda kalabilir.
            // Önemli olan, NewOrderForm.cs'deki InitializeComponent() çağrısının altında
            // kırmızı çizgi olmamasıdır. Bu minimal yapı, o hatayı gidermek için yeterlidir.
            // Eğer daha fazla kontrolü Designer üzerinden ekleseydik, burada daha fazla kod olurdu.
        }

        #endregion
    }
}