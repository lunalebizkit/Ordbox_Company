SELECT [s].[id] AS [value]
      ,[s].[name] AS [label]
  FROM [entity] [s]
  INNER JOIN [product] [p]
  ON [p].[supplier_id] = [s].[id]
  WHERE [p].[id] = @productid;