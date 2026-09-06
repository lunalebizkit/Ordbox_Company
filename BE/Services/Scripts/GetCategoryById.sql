SELECT [c].[id]
      ,[c].[description]
  FROM [category] [c]
  WHERE [c].[id] = @id;