SET NOCOUNT ON;

IF DB_ID(N'NfeMvp') IS NULL 
BEGIN 
    PRINT 'Banco NfeMvp não existe. Execute db/ddl.sql primeiro.'; 
    RETURN; 
END 

USE [NfeMvp]; 

IF EXISTS (SELECT 1 FROM dbo.NotaFiscal) 
BEGIN 
    PRINT 'Tabela NotaFiscal já possui dados. Seed ignorado.'; 
    RETURN; 
END 

DECLARE @i INT = 1; 

WHILE @i <= 10000 
BEGIN 
    INSERT INTO dbo.NotaFiscal (Emissor, DataEmissao) 
    VALUES ( 
        N'Emissor ' + RIGHT('000' + CAST(@i AS NVARCHAR(3)), 3), 
        DATEADD(DAY, -@i, CAST(GETDATE() AS DATE)) 
    ); 

    DECLARE @notaId INT = SCOPE_IDENTITY(); 

    DECLARE @j INT = 1; 

    WHILE @j <= 10 
    BEGIN 
        INSERT INTO dbo.Item (NotaFiscalId, Descricao, Valor) 
        VALUES ( 
            @notaId, 
            N'Item ' + CAST(@j AS NVARCHAR(3)) + N' da Nota ' + CAST(@i AS NVARCHAR(3)), 
            CAST((ABS(CHECKSUM(NEWID())) % 100000) / 1000.0 AS DECIMAL(15,3)) 
        ); 
        SET @j = @j + 1; 
    END 

    SET @i = @i + 1; 
END 

PRINT 'Seed concluído: 10000 notas e seus 10 itens criados.';