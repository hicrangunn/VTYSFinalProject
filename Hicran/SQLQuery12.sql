USE SiparisYonetim; -- Doðru veritabanýnda çalýþtýðýmýzdan emin olalým
GO

-- Bill tablosuna payment_status sütununu ekle
-- Eðer sütun zaten varsa hata vermemesi için kontrol ekleyelim
IF NOT EXISTS (SELECT * FROM sys.columns 
               WHERE Name = N'payment_status' 
               AND Object_ID = Object_ID(N'Bill'))
BEGIN
    ALTER TABLE Bill
    ADD payment_status VARCHAR(20) DEFAULT 'Ödenmedi';
END;
GO

-- Mevcut faturalarýn payment_status'unu 'Ödenmedi' olarak güncelle (eðer boþsa)
UPDATE Bill
SET payment_status = 'Ödenmedi'
WHERE payment_status IS NULL;
GO

-- Tablo yapýsýný kontrol etmek için:
SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Bill';