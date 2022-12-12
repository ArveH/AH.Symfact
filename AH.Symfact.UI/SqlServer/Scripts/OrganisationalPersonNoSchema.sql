DROP TABLE IF EXISTS OrganisationalPersonNoSchema
GO

CREATE TABLE OrganisationalPersonNoSchema(
    Id int IDENTITY CONSTRAINT PK_OrganisationalPersonNoSchema_Id PRIMARY KEY CLUSTERED (Id),
    DocName nvarchar(30),
    Cn nvarchar(50),
    Initials nvarchar(50),
    Data Xml
)
GO

INSERT INTO OrganisationalPersonNoSchema (DocName, Cn, Initials, Data)
SELECT 
    DocName,
    Data.value('declare namespace ctxO="contractX/contractXOrganisation";/ctxO:OrganisationalPerson/ctxO:cn', 'nvarchar(50)'),
    Data.value('declare namespace ctxO="contractX/contractXOrganisation";/ctxO:OrganisationalPerson/ctxO:initials', 'nvarchar(200)'),
    Data 
FROM OrganisationalPerson
GO

CREATE UNIQUE INDEX IX_OrganisationalPersonNoSchema_DocName ON OrganisationalPersonNoSchema(DocName)
GO
