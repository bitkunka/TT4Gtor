 


	 

CREATE PROCEDURE [Suppliers].SP_EliminarSuppliers @SupplierID INT
AS
	DELETE
	FROM [Suppliers].[Suppliers]
	WHERE SupplierID = @SupplierID
	 

CREATE PROCEDURE [Suppliers].SP_InsertarSuppliers
		@CompanyName NVARCHAR(40),
        @ContactName NVARCHAR(40),
        @ContactTitle NVARCHAR(40),
        @Address NVARCHAR(60),
        @City NVARCHAR(15),
        @Region NVARCHAR(15),
        @PostalCode NVARCHAR(10),
        @Country NVARCHAR(15),
        @Phone NVARCHAR(24),
        @Fax NVARCHAR(24),
        @HomePage NTEXT 
AS
	BEGIN
		SET NOCOUNT ON;
		BEGIN TRY        
			SET @ErrorNum = -1;
			INSERT INTO [Suppliers].Suppliers 
			(				
				[CompanyName],
[ContactName],
[ContactTitle],
[Address],
[City],
[Region],
[PostalCode],
[Country],
[Phone],
[Fax],
[HomePage]			) 
			VALUES 
			(
				@CompanyName,
@ContactName,
@ContactTitle,
@Address,
@City,
@Region,
@PostalCode,
@Country,
@Phone,
@Fax,
@HomePage			)
			SET NOCOUNT ON
			-- RETORNA EL VALOR DE LA NUEVA FILA SupplierID 
			SET @SupplierID = SCOPE_IDENTITY()
		END TRY
		BEGIN CATCH
			 SET @ErrorNum =(SELECT ERROR_NUMBER());        
			 SET @ErrorMes=(SELECT ERROR_MESSAGE());
		END CATCH
	END

 
