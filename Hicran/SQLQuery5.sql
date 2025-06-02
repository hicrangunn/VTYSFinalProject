-- Veritabaný oluþtur (eðer yoksa)
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'RestoranVeritabani')
BEGIN
    CREATE DATABASE RestoranVeritabani;
END
GO

USE RestoranVeritabani;
GO

-- TableInfo Tablosu
CREATE TABLE TableInfo (
    table_id INT PRIMARY KEY IDENTITY(1,1),
    status VARCHAR(20) NOT NULL -- 'Boþ', 'Dolu', 'Rezerv' gibi durumlar
);

-- Product Tablosu
CREATE TABLE Product (
    product_id INT PRIMARY KEY IDENTITY(1,1),
    product_name VARCHAR(100) NOT NULL,
    price DECIMAL(10,2) NOT NULL
);

-- Bill Tablosu
CREATE TABLE Bill (
    bill_id INT PRIMARY KEY IDENTITY(1,1),
    table_id INT NOT NULL,
    date DATETIME NOT NULL DEFAULT GETDATE(),
    total_amount DECIMAL(10,2) NOT NULL,
    FOREIGN KEY (table_id) REFERENCES TableInfo(table_id)
);

-- OrderInfo Tablosu
CREATE TABLE OrderInfo (
    order_id INT PRIMARY KEY IDENTITY(1,1),
    bill_id INT NOT NULL,
    product_id INT NOT NULL,
    quantity INT NOT NULL,
    FOREIGN KEY (bill_id) REFERENCES Bill(bill_id),
    FOREIGN KEY (product_id) REFERENCES Product(product_id)
);

-- Örnek Veri Ekleme (Ýsteðe Baðlý, test için faydalýdýr)
INSERT INTO TableInfo (status) VALUES ('Boþ'), ('Dolu'), ('Boþ');
INSERT INTO Product (product_name, price) VALUES ('Pizza Margherita', 45.00), ('Ayran', 7.50), ('Salata', 25.00);