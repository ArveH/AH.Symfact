DROP TABLE IF EXISTS ContractExtractedColumns
GO

CREATE TABLE ContractExtractedColumns(
    Id int IDENTITY PRIMARY KEY,
    DocName nvarchar(30),
    ContractOwnerCN nvarchar(100),
    Status nvarchar(10),
    Data Xml(Document contractXCol)
)
GO

WITH XMLNAMESPACES('symfact/Contract' AS C)
INSERT INTO ContractExtractedColumns(DocName, ContractOwnerCN, Status, Data)
SELECT 
    DocName,
    Data.value('/C:Contract/C:Summary/C:GeneralInfo/C:ContractOwnerCN', 'nvarchar(50)'),
    Data.value('/C:Status/@status', 'nvarchar(50)'),
    Data 
FROM Contract
GO

CREATE UNIQUE INDEX IX_ContractExtractedColumns_DocName ON ContractExtractedColumns(DocName)
GO