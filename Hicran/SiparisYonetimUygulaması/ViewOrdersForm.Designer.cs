﻿namespace SiparisYonetimUygulamasi // Bu namespace projenizin ana namespace'i olmalı
{
    partial class ViewOrdersForm
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
            this.ClientSize = new System.Drawing.Size(1000, 700); // ViewOrdersForm için belirlediğimiz boyut
            this.Text = "ViewOrdersForm"; // Formun varsayılan başlığı
        }

        #endregion
    }
}