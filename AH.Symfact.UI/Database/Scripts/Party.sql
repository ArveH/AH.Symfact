DROP TABLE IF EXISTS Party
GO

CREATE TABLE Party(
	Id nvarchar(30) PRIMARY KEY,
	Data Xml(Document contractXCol)
)
GO

