DROP TABLE IF EXISTS Contract
GO

CREATE TABLE Contract(
    Id nvarchar(30) PRIMARY KEY,
    Data Xml(Document contractXCol)
)
GO
