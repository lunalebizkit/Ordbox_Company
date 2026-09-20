  SELECT (1) FROM [category] C
  INNER JOIN [product] P
  ON P.[category_id] = C.id
  WHERE C.id = @categoryid
  AND P.[is_deleted] = 0
  AND C.[company_id] = @companyid