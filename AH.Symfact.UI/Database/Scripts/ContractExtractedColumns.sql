DROP TABLE IF EXISTS ContractExtractedColumns
GO

CREATE TABLE ContractExtractedColumns(
    Id nvarchar(30) PRIMARY KEY,
    ContractOwnerCN nvarchar(100),
    Status nvarchar(10),
    Data Xml(Document contractXCol)
)
GO

WITH XMLNAMESPACES('symfact/Contract' AS C)
INSERT INTO ContractExtractedColumns(Id, ContractOwnerCN, Status, Data)
SELECT 
    Id,
    Data.value('/C:Contract/C:Summary/C:GeneralInfo/C:ContractOwnerCN', 'nvarchar(50)'),
    Data.value('/C:Status/@status', 'nvarchar(50)'),
    Data 
FROM Contract
GO