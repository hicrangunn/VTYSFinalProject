USE SiparisYonetim; -- Doðru veritabanýnda çalýþtýðýmýzdan emin olalým
GO

-- Mevcut tüm masa kayýtlarýný sil
DELETE FROM TableInfo; 
GO

-- TableInfo tablosunun IDENTITY sayacýný sýfýrla
-- NOT: Ýkinci parametre (0) ile sýfýrlýyoruz. Bir sonraki eklenen kayýt 1'den baþlayacak.
DBCC CHECKIDENT ('TableInfo', RESEED, 0); 
GO

-- 10 adet yeni masa ekle (Hepsi 'Boþ' durumda baþlayacak)
INSERT INTO TableInfo (status) VALUES ('Boþ'); -- Masa 1
INSERT INTO TableInfo (status) VALUES ('Boþ'); -- Masa 2
INSERT INTO TableInfo (status) VALUES ('Boþ'); -- Masa 3
INSERT INTO TableInfo (status) VALUES ('Boþ'); -- Masa 4
INSERT INTO TableInfo (status) VALUES ('Boþ'); -- Masa 5
INSERT INTO TableInfo (status) VALUES ('Boþ'); -- Masa 6
INSERT INTO TableInfo (status) VALUES ('Boþ'); -- Masa 7
INSERT INTO TableInfo (status) VALUES ('Boþ'); -- Masa 8
INSERT INTO TableInfo (status) VALUES ('Boþ'); -- Masa 9
INSERT INTO TableInfo (status) VALUES ('Boþ'); -- Masa 10
GO

-- Masalarý kontrol etmek için:
SELECT * FROM TableInfo;