SELECT [b].[id]
      ,[b].[description]
      ,[b].[company_id] AS [companyId]
  FROM [brand] [b]
  WHERE
  [b].[id] = @id
  AND [b].[company_id] = @companyid