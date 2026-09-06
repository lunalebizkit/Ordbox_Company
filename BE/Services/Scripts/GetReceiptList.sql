WITH FilteredReceipts AS (
    SELECT 
        r.Id,
        r.[receipt_number] AS ReceiptNumber,
        r.[dateTime],
        r.[supplier_cuit] AS SupplierCuit,
        r.[supplier_name] AS SupplierName,
		u.[first_name] AS CreatedBy,
		r.[total],
		r.[type]
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

SELECT *
FROM FilteredReceipts
ORDER BY DateTime DESC
OFFSET @Page * @PageSize ROWS
FETCH NEXT @PageSize ROWS ONLY;
