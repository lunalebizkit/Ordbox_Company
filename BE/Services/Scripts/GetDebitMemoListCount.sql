WITH FilteredData AS (
    SELECT 
        r.Id
    FROM [debit_memo] [r]
    INNER JOIN [user] u ON [r].[user_id] = u.Id
    WHERE r.[company_id] = @companyid
      AND (@cuit IS NULL OR r.[customer_cuit] LIKE '%' + @cuit + '%')
      AND (@number IS NULL OR r.[debitMemo_number] = @number)
      AND (@date IS NULL OR CONVERT(VARCHAR(10), r.DateTime, 120) LIKE '%' + @date + '%')
      AND (@customername IS NULL OR LOWER(r.[customer_name]) LIKE '%' + LOWER(@customername) + '%')
)

SELECT COUNT(*) 
FROM FilteredData;