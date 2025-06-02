USE SiparisYonetim; -- Doðru veritabanýnda çalýþtýðýmýzdan emin olalým
GO

-- Bill tablosuna payment_date sütununu ekle
-- Eðer sütun zaten varsa hata vermemesi için kontrol ekleyelim
IF NOT EXISTS (SELECT * FROM sys.columns 
               WHERE Name = N'payment_date' 
               AND Object_ID = Object_ID(N'Bill'))
BEGIN
    ALTER TABLE Bill
    ADD payment_date DATETIME NULL; -- DATETIME tipinde ve NULL olabilen bir sütun
END;
GO

-- Tablo yapýsýný kontrol etmek için:
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Bill';