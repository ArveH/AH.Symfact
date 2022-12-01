DROP TABLE IF EXISTS PartyNoSchema
GO

CREATE TABLE PartyNoSchema(
    Id int IDENTITY CONSTRAINT PK_PartyNoSchema_Id PRIMARY KEY CLUSTERED (Id),
    DocName nvarchar(30),
    PartnerShortName nvarchar(200),
    PartnerCity nvarchar(200),
    Data Xml
)
GO

INSERT INTO PartyNoSchema (DocName, PartnerShortName, PartnerCity, Data)
SELECT 
    DocName,
    Data.value('declare namespace P="symfact/Party";/P:Party/P:Partner/P:PartnerDetails/P:ShortName', 'nvarchar(50)'),
    Data.value('declare namespace P="symfact/Party";/P:Party/P:Partner/P:Address/P:City', 'nvarchar(200)'),
    Data 
FROM Party
GO

CREATE UNIQUE INDEX IX_PartyNoSchema_DocName ON PartyNoSchema(DocName)
GO
