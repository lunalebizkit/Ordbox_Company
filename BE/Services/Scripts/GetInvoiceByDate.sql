DECLARE @DesiredOffSet VARCHAR(6) = '+00:00';

SELECT [i].[id]
      ,[i].[invoice_number] AS [InvoiceNumber]
      ,[i].[customer_name] AS [CustomerName]
      ,[i].[customer_cuit] AS [CustomerCuit]
      ,[i].[dateTime]
      ,[i].[total]
      ,[i].[iva_total] AS [IvaTotal]
      ,[i].[type]
	  ,[id].[id]
	  ,[id].[invoice_id] AS [InvocieId]
      ,[id].[product_id] AS [ProductId]
      ,[id].[product_code]
      ,[id].[quantity]
      ,[id].[price]
      ,[id].[iva]
      ,[id].[cae]
      ,[id].[cae_expiration_date] AS [CaeExpirationDate]
      ,[id].[integration_success] AS [IntegrationSuccess]

  FROM [invoice] [i]
  INNER JOIN [invoice_detail] [id] ON [id].[invoice_id] = [i].[id]
  WHERE 
  ((LEN(@textdatefrom)=0 OR CONVERT(datetime, SWITCHOfFSET([i].[dateTime], @DesiredOffSet)) >= CONVERT(datetime, @textdatefrom, 105))
   AND (LEN(@textdateto)=0 OR CONVERT(DATETIME, SWITCHOFFSET([i].[dateTime], @DesiredOffSet)) < DATEADD(DAY, 1, CONVERT(datetime, @textdateto, 105))))