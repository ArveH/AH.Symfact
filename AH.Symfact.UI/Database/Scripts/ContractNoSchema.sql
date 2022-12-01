DROP TABLE IF EXISTS ContractNoSchema
GO

CREATE TABLE ContractNoSchema(
    Id int IDENTITY CONSTRAINT PK_ContractNoSchema_Id PRIMARY KEY CLUSTERED (Id),
    DocName nvarchar(30),
    ContractOwnerCN nvarchar(100),
    Status nvarchar(10),
    ContractType nvarchar(100),
    Data Xml
)
GO

INSERT INTO ContractNoSchema(DocName, ContractOwnerCN, Status, ContractType, Data)
SELECT 
    DocName,
    Data.value('declare namespace C="symfact/Contract";/C:Contract/C:Summary/C:GeneralInfo/C:ContractOwnerCN', 'nvarchar(50)'),
    Data.value('declare namespace C="symfact/Contract";/C:Contract/C:Status/@status', 'nvarchar(50)'),
    Data.value('declare namespace C="symfact/Contract";/C:Contract/C:Summary/C:GeneralInfo/C:ContractType', 'nvarchar(100)'),
    Data
FROM Contract
GO

CREATE UNIQUE INDEX IX_ContractNoSchema_DocName ON ContractNoSchema(DocName)
GO