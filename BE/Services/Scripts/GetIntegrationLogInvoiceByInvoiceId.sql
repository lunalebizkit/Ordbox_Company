SELECT [id]
      ,[invoice_id] AS [invoiceId]
      ,[request]
      ,[response]
      ,[endpoint]
      ,[success]
      ,[created_on] AS [createdOn]
  FROM [integration_log_invoice]
  WHERE [invoice_id] = @invoiceid