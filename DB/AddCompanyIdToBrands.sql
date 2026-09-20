USE [Ordbox];

-- 1. Agregar columna solo si no existe
IF NOT EXISTS (
    SELECT 1
    FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'brand'
      AND COLUMN_NAME = 'company_id'
)
BEGIN
    ALTER TABLE [brand] ADD company_id BIGINT NULL;
END

-- 2. Actualizar datos solo si hay registros con NULL
UPDATE [brand]
SET company_id = 1002
WHERE company_id IS NULL;

-- 3. Alterar columna a NOT NULL solo si actualmente permite NULL
IF EXISTS (
    SELECT 1
    FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'brand'
      AND COLUMN_NAME = 'company_id'
      AND IS_NULLABLE = 'YES'
)
BEGIN
    ALTER TABLE [brand] ALTER COLUMN company_id BIGINT NOT NULL;
END

-- 4. Crear FK solo si no existe
IF NOT EXISTS (
    SELECT 1
    FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
    WHERE TABLE_NAME = 'brand'
      AND CONSTRAINT_NAME = 'FK_brand_company'
)
BEGIN
    ALTER TABLE [brand]
    ADD CONSTRAINT FK_brand_company FOREIGN KEY (company_id) REFERENCES [company](Id);
END

-- 5. Crear índice solo si no existe
IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_brand_company_id'
      AND object_id = OBJECT_ID('brand')
)
BEGIN
    CREATE INDEX IX_brand_company_id ON [brand](company_id);
END

