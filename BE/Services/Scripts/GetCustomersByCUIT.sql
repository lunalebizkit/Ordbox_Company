WITH EntityCustomer AS (
    SELECT 
        [e].[id],
        [e].[dni],
        [e].[cuit],
        REPLACE([e].[cuit], '-', '') AS CuitNormalized,
        [e].[name],
        [e].[address],
        [ee].[email]
    FROM entity e
    INNER JOIN customer c ON c.[id] = [e].[id]
    OUTER APPLY (
        SELECT TOP 1 email
        FROM email_entity ee
        WHERE ee.entity_id = c.id
        ORDER BY ee.id
    ) ee
    WHERE [e].[isInactive] = 0 AND [c].[company_id] = @companyid
)
SELECT TOP (10) [ec].[id]
      ,[ec].[dni]
      ,[ec].[cuit]
      ,[ec].[name]
      ,[ec].[address]
      ,[ec].[email]
  FROM EntityCustomer [ec]
  WHERE [ec].[CuitNormalized] like @cuit + '%';