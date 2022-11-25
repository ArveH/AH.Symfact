DROP TABLE IF EXISTS OrganisationalPersonComputedColumns
GO

CREATE OR ALTER FUNCTION getorgpersonid(@contract xml(Document dbo.contractXOrg))
RETURNS NVARCHAR(50) WITH SCHEMABINDING AS
BEGIN
RETURN @contract.value(
    'declare namespace ctxO="contractX/contractXOrganisation";/ctxO:OrganisationalPerson/@ID', 'NVARCHAR(50)')
END;
GO

CREATE OR ALTER FUNCTION getorgpersoncn(@contract xml(Document dbo.contractXOrg))
RETURNS NVARCHAR(50) WITH SCHEMABINDING AS
BEGIN
RETURN @contract.value(
    'declare namespace ctxO="contractX/contractXOrganisation";/ctxO:OrganisationalPerson/ctxO:cn', 'NVARCHAR(50)')
END;
GO

CREATE OR ALTER FUNCTION getorgpersoninitials(@contract xml(Document dbo.contractXOrg))
RETURNS NVARCHAR(50) WITH SCHEMABINDING AS
BEGIN
RETURN @contract.value(
    'declare namespace ctxO="contractX/contractXOrganisation";/ctxO:OrganisationalPerson/ctxO:initials', 'NVARCHAR(50)')
END;
GO

CREATE TABLE OrganisationalPersonComputedColumns(
    Id int IDENTITY CONSTRAINT PK_OrganisationalPersonComputedColumns_Id PRIMARY KEY CLUSTERED (Id),
    DocName AS dbo.getorgpersonid(Data) PERSISTED,
    Cn AS dbo.getorgpersoncn(Data) PERSISTED,
    Initials AS dbo.getorgpersoninitials(Data) PERSISTED,
    Data Xml(Document contractXOrg)
)
GO

WITH XMLNAMESPACES('contractX/contractXOrganisation' AS ctxO)
INSERT INTO OrganisationalPersonComputedColumns (Data)
SELECT 
    Data 
FROM OrganisationalPerson
GO

CREATE UNIQUE INDEX IX_OrganisationalPersonComputedColumns_DocName ON OrganisationalPersonComputedColumns(DocName)
GO


