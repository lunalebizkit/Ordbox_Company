USE [Ordbox];

-- 1. Agregar columna solo si no existe
IF NOT EXISTS (
    SELECT 1
    FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'debit_memo'
      AND COLUMN_NAME = 'company_id'
)
BEGIN
    ALTER TABLE [debit_memo] ADD company_id BIGINT NULL;
END

-- 2. Actualizar datos solo si hay registros con NULL
UPDATE [debit_memo]
SET company_id = 6
WHERE company_id IS NULL;

-- 3. Alterar columna a NOT NULL solo si actualmente permite NULL
IF EXISTS (
    SELECT 1
    FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'debit_memo'
      AND COLUMN_NAME = 'company_id'
      AND IS_NULLABLE = 'YES'
)
BEGIN
    ALTER TABLE [debit_memo] ALTER COLUMN company_id BIGINT NOT NULL;
END

-- 4. Crear FK solo si no existe
IF NOT EXISTS (
    SELECT 1
    FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
    WHERE TABLE_NAME = 'debit_memo'
      AND CONSTRAINT_NAME = 'FK_debit_memo_company_id_company_id'
)
BEGIN
    ALTER TABLE [debit_memo]
    ADD CONSTRAINT FK_debit_memo_company_id_company_id FOREIGN KEY ([company_id]) REFERENCES [company](Id);
END

-- 5. Crear índice solo si no existe
IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_debit_memo_company_id'
      AND object_id = OBJECT_ID('debit_memo')
)
BEGIN
    CREATE INDEX IX_debit_memo_company_id ON [debit_memo](company_id);
END

