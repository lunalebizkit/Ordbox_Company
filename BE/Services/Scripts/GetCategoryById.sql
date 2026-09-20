SELECT [c].[id]
      ,[c].[description]
      ,[c].[company_id]
  FROM [category] [c]
  WHERE [c].[id] = @id
  AND [c].[company_id] = @companyid;