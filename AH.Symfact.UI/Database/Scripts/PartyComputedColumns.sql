DROP TABLE IF EXISTS PartyComputedColumns
GO

CREATE OR ALTER FUNCTION getpartyid(@party xml(Document dbo.contractXCol))
RETURNS NVARCHAR(50) WITH SCHEMABINDING AS
BEGIN
RETURN @party.value(
    'declare namespace P="symfact/Party";/P:Party/@ID', 'NVARCHAR(50)')
END;
GO

CREATE OR ALTER FUNCTION getpartyshortname(@party xml(Document dbo.contractXCol))
RETURNS NVARCHAR(50) WITH SCHEMABINDING AS
BEGIN
RETURN @party.value(
    'declare namespace P="symfact/Party";/P:Party/P:Partner/P:PartnerDetails/P:ShortName', 'NVARCHAR(50)')
END;
GO

CREATE OR ALTER FUNCTION getpartycity(@party xml(Document dbo.contractXCol))
RETURNS NVARCHAR(50) WITH SCHEMABINDING AS
BEGIN
RETURN @party.value(
    'declare namespace P="symfact/Party";/P:Party/P:Partner/P:Address/P:City', 'NVARCHAR(50)')
END;
GO

CREATE TABLE PartyComputedColumns(
    Id int IDENTITY PRIMARY KEY,
    DocName AS dbo.getpartyid(Data) PERSISTED,
    PartnerShortName nvarchar(50),
    PartnerCity nvarchar(200),
    Data Xml(Document contractXCol)
)
GO

INSERT INTO PartyComputedColumns (Data)
SELECT Data 
FROM Party
GO

CREATE UNIQUE INDEX IX_PartyComputedColumns_DocName ON PartyComputedColumns(DocName)
GO
