USE SiparisYonetim; -- Do�ru veritaban�nda �al��t���m�zdan emin olal�m
GO

-- Mevcut t�m masa kay�tlar�n� sil (�nceki ad�mda yapt�ysan tekrar yapmana gerek yok)
DELETE FROM TableInfo; 
-- IDENTITY_INSERT'i a� (E�er table_id'ye manuel de�er atayacaksak, ancak IDENTITY kulland���m�z i�in buna gerek yok)
-- SET IDENTITY_INSERT TableInfo ON;

-- 10 adet yeni masa ekle
INSERT INTO TableInfo (status) VALUES ('Bo�'); -- Masa 1
INSERT INTO TableInfo (status) VALUES ('Bo�'); -- Masa 2
INSERT INTO TableInfo (status) VALUES ('Bo�'); -- Masa 3
INSERT INTO TableInfo (status) VALUES ('Bo�'); -- Masa 4
INSERT INTO TableInfo (status) VALUES ('Bo�'); -- Masa 5
INSERT INTO TableInfo (status) VALUES ('Bo�'); -- Masa 6
INSERT INTO TableInfo (status) VALUES ('Bo�'); -- Masa 7
INSERT INTO TableInfo (status) VALUES ('Bo�'); -- Masa 8
INSERT INTO TableInfo (status) VALUES ('Bo�'); -- Masa 9
INSERT INTO TableInfo (status) VALUES ('Bo�'); -- Masa 10

-- SET IDENTITY_INSERT TableInfo OFF;
GO

-- Masalar� kontrol etmek i�in:
