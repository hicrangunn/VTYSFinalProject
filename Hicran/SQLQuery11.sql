USE SiparisYonetim; -- Do�ru veritaban�nda �al��t���m�zdan emin olal�m
GO

-- Mevcut t�m masa kay�tlar�n� sil
DELETE FROM TableInfo; 
GO

-- TableInfo tablosunun IDENTITY sayac�n� s�f�rla
-- NOT: �kinci parametre (0) ile s�f�rl�yoruz. Bir sonraki eklenen kay�t 1'den ba�layacak.
DBCC CHECKIDENT ('TableInfo', RESEED, 0); 
GO

-- 10 adet yeni masa ekle (Hepsi 'Bo�' durumda ba�layacak)
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
GO

-- Masalar� kontrol etmek i�in:
SELECT * FROM TableInfo;