DROP TABLE IF EXISTS PartyExtractedColumns
GO

CREATE TABLE PartyExtractedColumns(
    Id nvarchar(30) PRIMARY KEY,
    PartyShortName nvarchar(200),
    PartyCity nvarchar(200),
    Data Xml(Document contractXCol)
)
GO

WITH XMLNAMESPACES('symfact/Party' AS P)
INSERT INTO PartyExtractedColumns (Id, PartyShortName, PartyCity, Data)
SELECT 
    Id,
    Data.value('/P:Party/P:Partner/P:PartnerDetails/P:ShortName', 'nvarchar(50)'),
    Data.value('/P:Party/P:Partner/P:Address/P:City', 'nvarchar(200)'),
    Data 
FROM Party
GO

