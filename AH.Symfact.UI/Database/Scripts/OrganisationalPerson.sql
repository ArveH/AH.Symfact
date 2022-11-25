DROP TABLE IF EXISTS OrganisationalPerson
GO

CREATE TABLE OrganisationalPerson(
    Id int IDENTITY CONSTRAINT PK_OrganisationalPerson_Id PRIMARY KEY CLUSTERED (Id),
	DocName nvarchar(30),
	Data Xml(Document contractXOrg)
)
GO

CREATE UNIQUE INDEX IX_OrganisationalPerson_DocName ON OrganisationalPerson(DocName)
GO