DROP TABLE IF EXISTS ContractExtractedColumns
GO

CREATE TABLE ContractExtractedColumns(
    Id int IDENTITY CONSTRAINT PK_ContractExtractedColumns_Id PRIMARY KEY CLUSTERED (Id),
    DocName nvarchar(30),
    ContractOwnerCN nvarchar(100),
    Status nvarchar(10),
    ContractType nvarchar(100),
    Data Xml(Document contractXCol)
)
GO

WITH XMLNAMESPACES('symfact/Contract' AS C)
INSERT INTO ContractExtractedColumns(DocName, ContractOwnerCN, Status, ContractType, Data)
SELECT 
    DocName,
    Data.value('/C:Contract/C:Summary/C:GeneralInfo/C:ContractOwnerCN', 'nvarchar(50)'),
    Data.value('/C:Contract/C:Status/@status', 'nvarchar(50)'),
    Data.value('/C:Contract/C:Summary/C:GeneralInfo/C:ContractType', 'nvarchar(100)'),
    Data 
FROM Contract
GO

CREATE UNIQUE INDEX IX_ContractExtractedColumns_DocName ON ContractExtractedColumns(DocName)
GO