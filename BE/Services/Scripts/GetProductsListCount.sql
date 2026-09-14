WITH FilteredData AS (
    SELECT 
        p.[id]
    FROM [product] p
    INNER JOIN [brand] b ON p.[brand_id] = b.[id]
    INNER JOIN [supplier] s ON p.[supplier_id] = s.[id]
	INNER JOIN [entity] e ON s.[id] = e.[id]
	INNER JOIN	[category] c ON p.[category_id] = c.[id]
    WHERE p.[is_deleted] = 0
      AND p.[id] > 0
      AND p.[company_id] = @companyid
      AND (@product IS NULL OR LOWER(p.[description]) LIKE '%' + LOWER(@product) + '%')
      AND (@brand IS NULL OR @brand = 0 OR p.[brand_id] = @brand)
      AND (@category IS NULL OR @category = 0 OR p.[category_id] = @category)
      AND (@code IS NULL OR LOWER(p.[code]) LIKE '%' + LOWER(@code) + '%')
      AND (@barcode IS NULL OR LOWER(p.[bar_code]) LIKE '%' + LOWER(@barcode) + '%')
      AND (
            @suppliercount = 0 
            OR p.[supplier_id] IN (SELECT value FROM STRING_SPLIT(@supplierids, ','))
          )
)

SELECT COUNT(*) 
FROM FilteredData;