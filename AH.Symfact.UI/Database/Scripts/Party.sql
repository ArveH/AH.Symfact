DROP TABLE IF EXISTS Party
GO

CREATE OR ALTER FUNCTION getpartyid(@party xml(Document dbo.contractXCol))
RETURNS NVARCHAR(50) WITH SCHEMABINDING AS
BEGIN
RETURN @party.value(
    'declare namespace C="symfact/Party";/C:Party/@ID', 'NVARCHAR(50)')
END;
GO

CREATE TABLE Party(
Id AS dbo.getpartyid(Data) PERSISTED PRIMARY KEY,
Data Xml(Document contractXCol)
)
GO

CREATE PRIMARY XML INDEX PartyXmlIdx ON Party(Data)
GO
