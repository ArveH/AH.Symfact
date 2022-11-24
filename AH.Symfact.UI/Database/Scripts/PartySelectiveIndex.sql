DROP TABLE IF EXISTS PartySelectiveIndex
GO

CREATE TABLE PartySelectiveIndex(
    Id int IDENTITY PRIMARY KEY,
    DocName nvarchar(30),
    Data Xml
)
GO

INSERT INTO PartySelectiveIndex (DocName, Data)
SELECT 
    DocName,
    Data 
FROM Party
GO

CREATE UNIQUE INDEX IX_PartySelectiveIndex_DocName ON PartySelectiveIndex(DocName)
GO

CREATE SELECTIVE XML INDEX SXI_PartySelectiveIndex
ON PartySelectiveIndex(Data)
FOR
(
    pathShortName = 'declare namespace P="symfact/Party";/P:Party/P:Partner/P:PartnerDetails/P:ShortName' AS XQUERY 'xs:string',
    pathCity = 'declare namespace P="symfact/Party";/P:Party/P:Partner/P:Address/P:City' AS XQUERY 'xs:string'
)
GO

