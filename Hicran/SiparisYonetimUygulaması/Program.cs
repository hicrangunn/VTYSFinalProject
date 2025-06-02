using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

// BURADAKİ 'SiparisYonetimUygulamasi' KISMI, SİZİN PROJENİZİN ADI VE AYNI ZAMANDA
// MainForm.cs gibi diğer form dosyalarındaki 'namespace' adı ile BİREBİR AYNI OLMALIDIR.
using SiparisYonetimUygulamasi;

namespace SiparisYonetimUygulamasi // Bu da yukarıdaki 'using' ile aynı namespace olmalı
{
    static class Program
    {
        /// <summary>
        /// Uygulamanın ana girdi noktası.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm()); // Bu satırda hata alıyorsak, MainForm tanımında sorun vardır.
        }
    }
}