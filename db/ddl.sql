IF DB_ID(N'NfeMvp') IS NULL
BEGIN
    CREATE DATABASE [NfeMvp];
END
GO

USE [NfeMvp];
GO

CREATE TABLE dbo.NotaFiscal (
    Id BIGINT IDENTITY(1,1) PRIMARY KEY,
    Emissor NVARCHAR(150) NOT NULL,
    DataEmissao DATE NOT NULL
);

CREATE INDEX IX_NotaFiscal_DataEmissao ON dbo.NotaFiscal(DataEmissao);

CREATE TABLE dbo.Item (
    Id BIGINT IDENTITY(1,1) PRIMARY KEY,
    NotaFiscalId BIGINT  NOT NULL,
    Descricao NVARCHAR(255) NOT NULL,
    Valor DECIMAL(15, 3) NOT NULL,
    CONSTRAINT FK_Item_NotaFiscal FOREIGN KEY (NotaFiscalId)
        REFERENCES dbo.NotaFiscal(Id) ON DELETE CASCADE
);

CREATE INDEX IX_Item_NotaFiscalId ON dbo.Item(NotaFiscalId);
CREATE INDEX IX_Item_Descricao ON dbo.Item(Descricao);
