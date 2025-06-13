IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'SimpQDemo')
BEGIN
    CREATE DATABASE [SimpQDemo];
END
GO


USE [SimpQDemo];
GO


IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InvoiceHeader]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[InvoiceHeader] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [Date] DATETIME NOT NULL,
        [CashierName] NVARCHAR(100) NOT NULL,
        [TotalAmount] DECIMAL(18, 2) NOT NULL
    );
END
GO


IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InvoiceDetail]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[InvoiceDetail] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [InvoiceHeaderId] INT NOT NULL,
        [ProductName] NVARCHAR(200) NOT NULL,
        [Price] DECIMAL(18, 2) NOT NULL,
        CONSTRAINT [FK_InvoiceDetail_InvoiceHeader] FOREIGN KEY ([InvoiceHeaderId])
            REFERENCES [dbo].[InvoiceHeader] ([Id])
            ON DELETE CASCADE
    );
END
GO


IF NOT EXISTS (SELECT 1 FROM [dbo].[InvoiceHeader])
BEGIN
    INSERT INTO [dbo].[InvoiceHeader] ([Date], [CashierName], [TotalAmount]) VALUES
    ('2024-01-01', 'Alice Johnson', 120.00),
    ('2024-01-02', 'Bob Smith', 230.50),
    ('2024-01-03', 'Carol Evans', 89.99),
    ('2024-01-04', 'Daniel Lee', 199.99),
    ('2024-01-05', 'Eva Green', 150.75),
    ('2024-01-06', 'Frank Moore', 300.20),
    ('2024-01-07', 'Grace Kim', 178.60),
    ('2024-01-08', 'Hank Davis', 250.00),
    ('2024-01-09', 'Ivy Clark', 90.45),
    ('2024-01-10', 'Jake Hall', 135.00);
END
GO


IF NOT EXISTS (SELECT 1 FROM [dbo].[InvoiceDetail])
BEGIN
    INSERT INTO [dbo].[InvoiceDetail] ([InvoiceHeaderId], [ProductName], [Price]) VALUES
    (3, 'Keyboard', 40.00),
    (7, 'Mouse', 20.00),
    (2, 'USB Cable', 10.00),
    (8, 'Mouse Pad', 15.00),
    (1, 'Screen Cleaner', 35.00),
    (9, 'Monitor', 150.00),
    (4, 'HDMI Cable', 25.00),
    (3, 'Power Adapter', 20.00),
    (10, 'Desk Lamp', 15.50),
    (1, 'Chair Mat', 20.00),
    (8, 'Notebook', 30.00),
    (6, 'Pen Set', 10.00),
    (3, 'Desk Organizer', 20.00),
    (5, 'Stapler', 9.99),
    (2, 'Paper Clips', 20.00),
    (7, 'Webcam', 80.00),
    (10, 'Tripod', 30.00),
    (5, 'Microphone', 50.00),
    (1, 'Headphones', 25.00),
    (6, 'Cable Organizer', 14.99),
    (2, 'External Drive', 90.00),
    (9, 'SSD', 30.00),
    (4, 'HDD Enclosure', 10.75),
    (10, 'Cooling Pad', 15.00),
    (6, 'Router', 5.00),
    (7, 'Tablet', 120.00),
    (3, 'Stylus Pen', 50.00),
    (5, 'Case', 20.00),
    (8, 'Screen Protector', 5.20),
    (2, 'Charger', 105.00),
    (9, 'Laptop', 100.00),
    (6, 'Laptop Stand', 30.00),
    (10, 'Docking Station', 20.00),
    (1, 'USB Hub', 15.60),
    (8, 'Bluetooth Mouse', 13.00),
    (5, 'Smartphone', 130.00),
    (3, 'Phone Case', 20.00),
    (6, 'Wireless Charger', 30.00),
    (4, 'Earbuds', 25.00),
    (7, 'Power Bank', 45.00),
    (10, 'Printer', 40.00),
    (9, 'Ink Cartridge', 20.45),
    (8, 'Paper Pack', 10.00),
    (7, 'Label Maker', 10.00),
    (5, 'USB Cable', 10.00),
    (4, 'Camera', 50.00),
    (2, 'Lens', 40.00),
    (6, 'Tripod', 15.00),
    (3, 'SD Card', 10.00),
    (1, 'Cleaning Kit', 20.00);
END
GO