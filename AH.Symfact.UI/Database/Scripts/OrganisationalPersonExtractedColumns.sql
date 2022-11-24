DROP TABLE IF EXISTS OrganisationalPersonExtractedColumns
GO

CREATE TABLE OrganisationalPersonExtractedColumns(
    Id int IDENTITY PRIMARY KEY,
    DocName nvarchar(30),
    Cn nvarchar(50),
    Initials nvarchar(50),
    Data Xml(Document contractXOrg)
)
GO

WITH XMLNAMESPACES('contractX/contractXOrganisation' AS ctxO)
INSERT INTO OrganisationalPersonExtractedColumns (DocName, Cn, Initials, Data)
SELECT 
    DocName,
    Data.value('/ctxO:OrganisationalPerson/ctxO:cn', 'nvarchar(50)'),
    Data.value('/ctxO:OrganisationalPerson/ctxO:initials', 'nvarchar(200)'),
    Data 
FROM OrganisationalPerson
GO

CREATE UNIQUE INDEX IX_OrganisationalPersonExtractedColumns_DocName ON OrganisationalPersonExtractedColumns(DocName)
GO
