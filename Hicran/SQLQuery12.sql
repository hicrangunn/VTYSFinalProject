USE SiparisYonetim; -- Do�ru veritaban�nda �al��t���m�zdan emin olal�m
GO

-- Bill tablosuna payment_status s�tununu ekle
-- E�er s�tun zaten varsa hata vermemesi i�in kontrol ekleyelim
IF NOT EXISTS (SELECT * FROM sys.columns 
               WHERE Name = N'payment_status' 
               AND Object_ID = Object_ID(N'Bill'))
BEGIN
    ALTER TABLE Bill
    ADD payment_status VARCHAR(20) DEFAULT '�denmedi';
END;
GO

-- Mevcut faturalar�n payment_status'unu '�denmedi' olarak g�ncelle (e�er bo�sa)
UPDATE Bill
SET payment_status = '�denmedi'
WHERE payment_status IS NULL;
GO

-- Tablo yap�s�n� kontrol etmek i�in:
SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Bill';