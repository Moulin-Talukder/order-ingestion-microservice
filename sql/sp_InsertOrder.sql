CREATE PROCEDURE sp_InsertOrder
    @RequestId NVARCHAR(100),
    @OrderId UNIQUEIDENTIFIER,
    @CustomerEmail NVARCHAR(320),
    @CustomerName NVARCHAR(200),
    @Currency NVARCHAR(10),
    @TotalAmount DECIMAL(18,2),
    @OrderItems OrderItemType READONLY,
    @OutStatus INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRAN;

        IF EXISTS(SELECT 1 FROM IdempotencyRequests WHERE RequestId = @RequestId)
        BEGIN
            SET @OutStatus = 1;
            ROLLBACK TRAN;
            RETURN;
        END

        DECLARE @CustomerId UNIQUEIDENTIFIER;
        SELECT @CustomerId = CustomerId FROM Customers WHERE Email = @CustomerEmail;
        IF @CustomerId IS NULL
        BEGIN
            SET @CustomerId = NEWID();
            INSERT INTO Customers (CustomerId, Email, Name) VALUES (@CustomerId, @CustomerEmail, @CustomerName);
        END

        INSERT INTO Orders (OrderId, CustomerId, TotalAmount, Currency)
        VALUES (@OrderId, @CustomerId, @TotalAmount, @Currency);

        INSERT INTO OrderItems (OrderId, SKU, ProductName, Quantity, UnitPrice)
        SELECT @OrderId, SKU, ProductName, Quantity, UnitPrice
        FROM @OrderItems;

        INSERT INTO IdempotencyRequests (RequestId, OrderId) VALUES (@RequestId, @OrderId);

        COMMIT TRAN;
        SET @OutStatus = 0;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0
            ROLLBACK TRAN;
        SET @OutStatus = -1;
        RETURN;
    END CATCH
END
