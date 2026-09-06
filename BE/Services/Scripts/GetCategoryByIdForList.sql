SELECT [c].[id] AS [value]
      ,[c].[description] AS [label]
  FROM [category] [c]
  INNER JOIN [product] [p]
  ON [p].[category_id] = [c].[id]
  WHERE [p].[id] = @productid;