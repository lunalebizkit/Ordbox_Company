WITH FilteredReceipts AS (
    SELECT 
        r.Id
    FROM [receipt] [r]
    INNER JOIN [user] u ON [r].[user_id] = u.Id
    INNER JOIN [company] c ON u.[company_id] = c.Id
    WHERE c.Id = @companyid
      AND r.is_inactive = 0
      AND (@cuit IS NULL OR r.[supplier_cuit] LIKE '%' + @cuit + '%')
      AND (@number IS NULL OR r.[receipt_number] = @number)
      AND (@date IS NULL OR CONVERT(VARCHAR(10), r.DateTime, 120) LIKE '%' + @date + '%')
      AND (@customername IS NULL OR LOWER(r.[supplier_name]) LIKE '%' + LOWER(@customername) + '%')
)

SELECT COUNT(*) 
FROM FilteredReceipts;