DROP TABLE IF EXISTS OrganisationalPersonSelectiveIndex
GO

CREATE TABLE OrganisationalPersonSelectiveIndex(
    Id int IDENTITY CONSTRAINT PK_OrganisationalPersonSelectiveIndex_Id PRIMARY KEY CLUSTERED (Id),
    DocName nvarchar(30),
    Data Xml
)
GO

INSERT INTO OrganisationalPersonSelectiveIndex (DocName, Data)
SELECT 
    DocName,
    Data 
FROM OrganisationalPerson
GO

CREATE UNIQUE INDEX IX_OrganisationalPersonSelectiveIndex_DocName ON OrganisationalPersonSelectiveIndex(DocName)
GO

CREATE SELECTIVE XML INDEX SXI_OrganisationalPersonSelectiveIndex
ON OrganisationalPersonSelectiveIndex(Data)
FOR
(
    pathCn = 'declare namespace ctxO="contractX/contractXOrganisation";/ctxO:OrganisationalPerson/ctxO:cn' AS XQUERY 'xs:string',
    pathInitials = 'declare namespace ctxO="contractX/contractXOrganisation";/ctxO:OrganisationalPerson/ctxO:initials' AS XQUERY 'xs:string'
)
GO
