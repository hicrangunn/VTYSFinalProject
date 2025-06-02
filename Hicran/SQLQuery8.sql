USE SiparisYonetim; -- Doðru veritabanýnda çalýþtýðýmýzdan emin olalým
GO

-- Mevcut tüm masa kayýtlarýný sil (önceki adýmda yaptýysan tekrar yapmana gerek yok)
DELETE FROM TableInfo; 
-- IDENTITY_INSERT'i aç (Eðer table_id'ye manuel deðer atayacaksak, ancak IDENTITY kullandýðýmýz için buna gerek yok)
-- SET IDENTITY_INSERT TableInfo ON;

-- 10 adet yeni masa ekle
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

-- SET IDENTITY_INSERT TableInfo OFF;
GO

-- Masalarý kontrol etmek için:
