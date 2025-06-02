USE SiparisYonetim; -- Do�ru veritaban�nda �al��t���m�zdan emin olal�m
GO

-- Bill tablosuna payment_date s�tununu ekle
-- E�er s�tun zaten varsa hata vermemesi i�in kontrol ekleyelim
IF NOT EXISTS (SELECT * FROM sys.columns 
               WHERE Name = N'payment_date' 
               AND Object_ID = Object_ID(N'Bill'))
BEGIN
    ALTER TABLE Bill
    ADD payment_date DATETIME NULL; -- DATETIME tipinde ve NULL olabilen bir s�tun
END;
GO

-- Tablo yap�s�n� kontrol etmek i�in:
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Bill';