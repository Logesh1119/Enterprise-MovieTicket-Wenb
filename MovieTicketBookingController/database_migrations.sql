-- database_migrations.sql
-- Add payment tracking columns to Bookings table

USE MovieTicketDb;
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Bookings]') AND name = 'PaymentMethod')
BEGIN
    ALTER TABLE [dbo].[Bookings] ADD [PaymentMethod] NVARCHAR(50) NULL;
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Bookings]') AND name = 'PaymentStatus')
BEGIN
    ALTER TABLE [dbo].[Bookings] ADD [PaymentStatus] NVARCHAR(50) NULL;
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Bookings]') AND name = 'TransactionId')
BEGIN
    ALTER TABLE [dbo].[Bookings] ADD [TransactionId] NVARCHAR(100) NULL;
END
GO

-- Set default values for existing confirmed bookings
UPDATE [dbo].[Bookings]
SET [PaymentStatus] = 'Paid', [PaymentMethod] = 'Cash'
WHERE [Status] = 'Confirmed' AND [PaymentStatus] IS NULL;
GO