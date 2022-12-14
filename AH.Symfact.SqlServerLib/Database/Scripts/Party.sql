DROP TABLE IF EXISTS Party
GO

CREATE TABLE Party(
    Id int IDENTITY CONSTRAINT PK_Party_Id PRIMARY KEY CLUSTERED (Id),
	DocName nvarchar(30),
	Data Xml(Document contractXCol)
)
GO

CREATE UNIQUE INDEX IX_Party_DocName ON Party(DocName)
GO

