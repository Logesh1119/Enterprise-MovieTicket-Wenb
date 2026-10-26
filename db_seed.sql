
DECLARE @AdminId INT = 5;
DECLARE @ShowId INT = 1;
DECLARE @ScreenSeatId INT = 799;

IF @AdminId IS NOT NULL AND @ShowId IS NOT NULL AND @ScreenSeatId IS NOT NULL
BEGIN
    INSERT INTO ActivityLogs (TheaterAdminId, ActionType, ActionDescription, CreatedAt)
    VALUES 
    (@AdminId, 'Movie Created', 'Added new movie Inception to the catalog', GETUTCDATE()),
    (@AdminId, 'Show Scheduled', 'Scheduled Inception for Screen 1', GETUTCDATE()),
    (@AdminId, 'Theater Updated', 'Updated PVR MALL amenities', GETUTCDATE());

    -- Create a dummy Customer if none exists
    DECLARE @CustomerId INT = (SELECT TOP 1 Id FROM Users WHERE RoleId = (SELECT RoleId FROM Roles WHERE RoleName = 'Customer'));
    IF @CustomerId IS NULL
    BEGIN
        DECLARE @CustomerRoleId UNIQUEIDENTIFIER = (SELECT RoleId FROM Roles WHERE RoleName = 'Customer');
        IF @CustomerRoleId IS NULL
        BEGIN
            SET @CustomerRoleId = NEWID();
            INSERT INTO Roles (RoleId, RoleName) VALUES (@CustomerRoleId, 'Customer');
        END
        INSERT INTO Users (Username, Email, PasswordHash, RoleId) VALUES ('TestCustomer', 'customer@test.com', 'hash', @CustomerRoleId);
        SET @CustomerId = SCOPE_IDENTITY();
    END

    -- Add some bookings over the last 12 months for the chart
    DECLARE @i INT = 0;
    WHILE @i < 100
    BEGIN
        DECLARE @RandomMonth INT = ABS(CHECKSUM(NEWID())) % 12;
        DECLARE @BookingDate DATETIME2 = DATEADD(MONTH, -@RandomMonth, GETUTCDATE());
        DECLARE @BookingType NVARCHAR(50) = CASE WHEN @i % 3 = 0 THEN 'On-site' ELSE 'Online' END;

        INSERT INTO Bookings (UserId, ShowId, BookingDate, BookingType, TotalAmount, Status)
        VALUES (@CustomerId, @ShowId, @BookingDate, @BookingType, 150.00, 'Confirmed');
        
        DECLARE @BookingId INT = SCOPE_IDENTITY();

        INSERT INTO BookingSeats (BookingId, ScreenSeatId, Price)
        VALUES (@BookingId, @ScreenSeatId, 150.00);

        SET @i = @i + 1;
    END

    -- Add a few bookings for TODAY
    SET @i = 0;
    WHILE @i < 15
    BEGIN
        DECLARE @BookingTypeToday NVARCHAR(50) = CASE WHEN @i % 3 = 0 THEN 'On-site' ELSE 'Online' END;
        INSERT INTO Bookings (UserId, ShowId, BookingDate, BookingType, TotalAmount, Status)
        VALUES (@CustomerId, @ShowId, GETUTCDATE(), @BookingTypeToday, 150.00, 'Confirmed');
        
        DECLARE @BookingIdToday INT = SCOPE_IDENTITY();
        INSERT INTO BookingSeats (BookingId, ScreenSeatId, Price)
        VALUES (@BookingIdToday, @ScreenSeatId, 150.00);
        SET @i = @i + 1;
    END
END

