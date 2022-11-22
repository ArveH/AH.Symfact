DROP TABLE IF EXISTS Contract
GO

CREATE OR ALTER FUNCTION getcontractid(@contract xml(Document dbo.contractXCol))
RETURNS NVARCHAR(50) WITH SCHEMABINDING AS
BEGIN
RETURN @contract.value(
    'declare namespace C="symfact/Contract";/C:Contract/@ID', 'NVARCHAR(50)')
END;
GO

CREATE TABLE Contract(
    Id AS dbo.getcontractid(Data) PERSISTED PRIMARY KEY,
    Data Xml(Document contractXCol)
)
GO
