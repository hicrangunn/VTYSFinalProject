using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration; // App.config'dan bağlantı dizesi okumak için gerekli

namespace SiparisYonetimUygulamasi.DAL
{
    public class DbHelper
    {
        // App.config dosyasından bağlantı dizesini alıyoruz.
        private static string connectionString = ConfigurationManager.ConnectionStrings["SiparisYonetimDB"].ConnectionString;

        // ***********************************************************************************
        // METOT 1: GetData (Veri okuma - SELECT işlemleri için)
        // Parametre: SQL sorgusu (örneğin "SELECT * FROM Product")
        // Geri Dönüş: Sorgu sonucunu içeren bir DataTable
        // ***********************************************************************************
        public static DataTable GetData(string query)
        {
            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    try
                    {
                        connection.Open();
                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        adapter.Fill(dt);
                    }
                    catch (SqlException ex)
                    {
                        // SQL ile ilgili hataları yakala
                        Console.WriteLine("SQL Hatası: " + ex.Message);
                        throw new Exception("Veritabanından veri alınırken hata oluştu.", ex);
                    }
                    catch (Exception ex)
                    {
                        // Diğer genel hataları yakala
                        Console.WriteLine("Genel Hata: " + ex.Message);
                        throw new Exception("Veri alınırken beklenmeyen bir hata oluştu.", ex);
                    }
                }
            }
            return dt;
        }

        // ***********************************************************************************
        // METOT 1.1: GetData (Parametreli veri okuma - SELECT işlemleri için) - YENİ EKLENDİ
        // Parametreler: SQL sorgusu, SqlParameter dizisi
        // Geri Dönüş: Sorgu sonucunu içeren bir DataTable
        // ***********************************************************************************
        public static DataTable GetData(string query, SqlParameter[] parameters = null)
        {
            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters); // Parametreleri komuta ekle
                    }
                    try
                    {
                        connection.Open();
                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        adapter.Fill(dt);
                    }
                    catch (SqlException ex)
                    {
                        Console.WriteLine("SQL Hatası (Parametreli GetData): " + ex.Message);
                        throw new Exception("Veritabanından veri alınırken hata oluştu.", ex);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Genel Hata (Parametreli GetData): " + ex.Message);
                        throw new Exception("Veri alınırken beklenmeyen bir hata oluştu.", ex);
                    }
                }
            }
            return dt;
        }

        // ***********************************************************************************
        // METOT 2: ExecuteNonQuery (Veri ekleme, güncelleme, silme - INSERT, UPDATE, DELETE işlemleri için)
        // Parametreler: SQL sorgusu, SqlParameter dizisi (güvenli sorgular için)
        // Geri Dönüş: Etkilenen satır sayısı
        // ***********************************************************************************
        public static int ExecuteNonQuery(string query, SqlParameter[] parameters = null)
        {
            int affectedRows = 0;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters); // Parametreleri komuta ekle (SQL Injection'dan korunmak için)
                    }
                    try
                    {
                        connection.Open();
                        affectedRows = command.ExecuteNonQuery();
                    }
                    catch (SqlException ex)
                    {
                        Console.WriteLine("SQL Hatası (NonQuery): " + ex.Message);
                        throw new Exception("Veritabanı işlemi sırasında hata oluştu.", ex);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Genel Hata (NonQuery): " + ex.Message);
                        throw new Exception("Beklenmeyen bir hata oluştu.", ex);
                    }
                }
            }
            return affectedRows;
        }

        // ***********************************************************************************
        // METOT 3: ExecuteScalarReturnId (Yeni eklenen bir kaydın IDENTITY ID'sini almak için)
        // Örneğin: Yeni bir Bill (fatura) eklediğimizde, o faturanın bill_id'sini alıp OrderInfo'ya kaydetmek için.
        // Parametreler: SQL sorgusu (genellikle INSERT...; SELECT SCOPE_IDENTITY();), SqlParameter dizisi
        // Geri Dönüş: Yeni eklenen kaydın ID'si veya 0 (hata durumunda)
        // ***********************************************************************************
        public static int ExecuteScalarReturnId(string query, SqlParameter[] parameters = null)
        {
            int newId = 0;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }
                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar(); // Tek bir değer döndürür (genellikle ID)
                        if (result != null && result != DBNull.Value)
                        {
                            newId = Convert.ToInt32(result);
                        }
                    }
                    catch (SqlException ex)
                    {
                        Console.WriteLine("SQL Hatası (Scalar): " + ex.Message);
                        throw new Exception("Veritabanından ID alınırken hata oluştu.", ex);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Genel Hata (Scalar): " + ex.Message);
                        throw new Exception("ID alınırken beklenmeyen bir hata oluştu.", ex);
                    }
                }
            }
            return newId;
        }
    }
}