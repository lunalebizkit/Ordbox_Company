SELECT TOP(1)
	[e].[id]
    FROM entity e
    INNER JOIN customer c ON c.[id] = [e].[id]
    WHERE REPLACE([e].[cuit], '-', '') = @cuit
    AND c.[company_id] = @companyid;