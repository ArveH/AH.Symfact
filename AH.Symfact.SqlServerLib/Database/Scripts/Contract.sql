DROP TABLE IF EXISTS Contract
GO

CREATE TABLE Contract(
    Id int IDENTITY CONSTRAINT PK_Contract_Id PRIMARY KEY CLUSTERED (Id),
    DocName nvarchar(30),
    Data Xml(Document contractXCol)
)
GO

CREATE UNIQUE INDEX IX_Contract_DocName ON Contract(DocName)
GO