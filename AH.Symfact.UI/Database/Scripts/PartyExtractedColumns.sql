DROP TABLE IF EXISTS PartyExtractedColumns
GO

CREATE TABLE PartyExtractedColumns(
    Id int IDENTITY PRIMARY KEY,
    DocName nvarchar(30),
    PartnerShortName nvarchar(200),
    PartnerCity nvarchar(200),
    Data Xml(Document contractXCol)
)
GO

WITH XMLNAMESPACES('symfact/Party' AS P)
INSERT INTO PartyExtractedColumns (DocName, PartnerShortName, PartnerCity, Data)
SELECT 
    DocName,
    Data.value('/P:Party/P:Partner/P:PartnerDetails/P:ShortName', 'nvarchar(50)'),
    Data.value('/P:Party/P:Partner/P:Address/P:City', 'nvarchar(200)'),
    Data 
FROM Party
GO

CREATE UNIQUE INDEX IX_PartyExtractedColumns_DocName ON PartyExtractedColumns(DocName)
GO
