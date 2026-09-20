SELECT [id]
      ,[debit_id] AS [DebitId]
      ,[request]
      ,[response]
      ,[endpoint]
      ,[success]
      ,[created_on] AS [createdOn]
  FROM [integration_log_debit]
  WHERE [debit_id] = @id