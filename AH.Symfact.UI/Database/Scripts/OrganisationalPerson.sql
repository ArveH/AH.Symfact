DROP TABLE IF EXISTS OrganisationalPerson
GO

CREATE TABLE OrganisationalPerson(
	Id nvarchar(30) PRIMARY KEY,
	Data Xml(Document contractXOrg)
)
GO
