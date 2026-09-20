SELECT [b].[id] AS [value]
      ,[b].[description] AS [label]
  FROM [brand] [b]
  INNER JOIN [product] [p]
  ON [p].[brand_id] = [b].[id]
  WHERE [p].[id] = @productid;