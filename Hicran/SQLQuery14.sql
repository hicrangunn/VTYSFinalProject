-- Veritabanı oluştur
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'SiparisYonetim')
BEGIN
    CREATE DATABASE SiparisYonetim;
END
GO

USE SiparisYonetim;
GO

-- Tablo: TableInfo
CREATE TABLE TableInfo (
    table_id INT PRIMARY KEY IDENTITY(1,1),
    status VARCHAR(20) NOT NULL
);
GO

-- Tablo: Product
CREATE TABLE Product (
    product_id INT PRIMARY KEY IDENTITY(1,1),
    product_name VARCHAR(100) NOT NULL,
    price DECIMAL(10,2) NOT NULL
);
GO

-- Tablo: Bill
CREATE TABLE Bill (
    bill_id INT PRIMARY KEY IDENTITY(1,1),
    table_id INT NOT NULL,
    date DATETIME NOT NULL DEFAULT GETDATE(),
    total_amount DECIMAL(10,2) NOT NULL,
    FOREIGN KEY (table_id) REFERENCES TableInfo(table_id)
);
GO

-- Tablo: OrderInfo
CREATE TABLE OrderInfo (
    order_id INT PRIMARY KEY IDENTITY(1,1),
    bill_id INT NOT NULL,
    product_id INT NOT NULL,
    quantity INT NOT NULL,
    FOREIGN KEY (bill_id) REFERENCES Bill(bill_id),
    FOREIGN KEY (product_id) REFERENCES Product(product_id)
);
GO

-- Kolon ekleme: Bill.payment_status
IF NOT EXISTS (
    SELECT * FROM sys.columns 
    WHERE Name = N'payment_status' AND Object_ID = Object_ID(N'Bill'))
BEGIN
    ALTER TABLE Bill
    ADD payment_status VARCHAR(20) DEFAULT 'Ödenmedi';
END;
GO

UPDATE Bill
SET payment_status = 'Ödenmedi'
WHERE payment_status IS NULL;
GO

-- Kolon ekleme: Bill.payment_date
IF NOT EXISTS (
    SELECT * FROM sys.columns 
    WHERE Name = N'payment_date' AND Object_ID = Object_ID(N'Bill'))
BEGIN
    ALTER TABLE Bill
    ADD payment_date DATETIME NULL;
END;
GO

-- Örnek veriler
DELETE FROM TableInfo;
DBCC CHECKIDENT ('TableInfo', RESEED, 0);
GO

INSERT INTO TableInfo (status)
VALUES ('Boş'), ('Boş'), ('Boş'), ('Boş'), ('Boş'),
       ('Boş'), ('Boş'), ('Boş'), ('Boş'), ('Boş');
GO

INSERT INTO Product (product_name, price)
VALUES ('Pizza Margherita', 45.00), 
       ('Ayran', 7.50), 
       ('Salata', 25.00);
GO

-- Kontrol amaçlı sorgular
SELECT * FROM TableInfo;
SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Bill';
