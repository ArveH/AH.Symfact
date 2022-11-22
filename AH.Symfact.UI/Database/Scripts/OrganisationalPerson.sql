DROP TABLE IF EXISTS OrganisationalPerson
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

CREATE TABLE OrganisationalPerson(
Id AS dbo.getorgpersonid(Data) PERSISTED PRIMARY KEY,
Cn AS dbo.getorgpersoncn(Data),
Initials AS dbo.getorgpersoninitials(Data),
Data Xml(Document contractXOrg)
)
GO
