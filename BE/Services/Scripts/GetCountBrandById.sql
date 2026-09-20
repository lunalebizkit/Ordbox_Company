  SELECT (1) FROM [brand] B
  INNER JOIN [product] P
  ON P.brand_id = B.id
  WHERE B.id = @brandid
  AND P.[is_deleted] = 0
  AND B.[company_id] = @companyid