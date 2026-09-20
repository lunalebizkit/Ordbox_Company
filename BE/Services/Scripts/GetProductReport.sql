SELECT	[p].[id], 
		[p].[description], 
		[p].[code], 
		[B].[description] AS [brandName], 
		[p].[quantity],
		[p].[purchase_price],
		CONVERT (DECIMAL(18,2), ([p].[quantity] * [p].[purchase_price])) AS [subTotal]

FROM [product] [p] 

JOIN [brand] [B]
ON [B].[id] = [p].[brand_id]

WHERE [quantity] > 0 AND [is_deleted] = 0
AND [company_id] = @companyid
ORDER BY [p].[description]