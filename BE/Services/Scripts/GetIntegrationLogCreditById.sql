SELECT [id]
      ,[credit_id] AS [CreditId]
      ,[request]
      ,[response]
      ,[endpoint]
      ,[success]
      ,[created_on] AS [createdOn]
  FROM [integration_log_credit]
  WHERE [credit_id] = @id