DROP TABLE IF EXISTS OrganisationalPersonExtractedColumns
GO

CREATE TABLE OrganisationalPersonExtractedColumns(
    Id nvarchar(30) PRIMARY KEY,
    Cn nvarchar(50),
    Initials nvarchar(50),
    Data Xml(Document contractXOrg)
)
GO

WITH XMLNAMESPACES('contractX/contractXOrganisation' AS ctxO)
INSERT INTO OrganisationalPersonExtractedColumns (Id, Cn, Initials, Data)
SELECT 
    Id,
    Data.value('/ctxO:OrganisationalPerson/ctxO:cn', 'nvarchar(50)'),
    Data.value('/ctxO:OrganisationalPerson/ctxO:initials', 'nvarchar(200)'),
    Data 
FROM OrganisationalPerson
GO
