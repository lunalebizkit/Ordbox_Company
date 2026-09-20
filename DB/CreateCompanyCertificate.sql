
IF NOT EXISTS (
    SELECT 1 
    FROM INFORMATION_SCHEMA.TABLES 
    WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = N'company_certificate'
)
BEGIN
    CREATE TABLE [dbo].[company_certificate](
        [id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [company_id] BIGINT NOT NULL,
        [certificate_data] VARBINARY(MAX) NOT NULL,
        [password_encrypted] VARBINARY(MAX) NULL,
        [fecha_expiracion] DATETIME NOT NULL,
        [fecha_creacion] DATETIME NOT NULL DEFAULT GETDATE(),
        [fecha_actualizacion] DATETIME NULL,
        [is_active] BIT NOT NULL DEFAULT 1,
        [algoritmo_cifrado] VARCHAR(50) NOT NULL DEFAULT 'AES256',
		[guid_unico] UNIQUEIDENTIFIER NOT NULL 
            CONSTRAINT DF_company_certificate_guid_unico DEFAULT NEWID(),

        CONSTRAINT [FK_company_certificate_company_id] 
            FOREIGN KEY ([company_id]) REFERENCES [dbo].[company]([id]),

        CONSTRAINT [UQ_company_certificate_guid_unico] UNIQUE ([guid_unico])
    ) ON [PRIMARY]
END
GO

-- Índices recomendados
CREATE NONCLUSTERED INDEX IX_company_certificate_company_id
    ON [dbo].[company_certificate](company_id);

CREATE NONCLUSTERED INDEX IX_company_certificate_fecha_expiracion
    ON [dbo].[company_certificate](fecha_expiracion);

-- Opcional: garantizar un único certificado activo por empresa
CREATE UNIQUE INDEX UQ_company_certificate_company_id_active
    ON [dbo].[company_certificate](company_id, is_active)
    WHERE [is_active] = 1;
