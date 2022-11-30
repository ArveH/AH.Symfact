DROP TABLE IF EXISTS ContractSelectiveIndex
GO

CREATE TABLE ContractSelectiveIndex(
    Id int IDENTITY CONSTRAINT PK_ContractSelectiveIndex_Id PRIMARY KEY CLUSTERED (Id),
    DocName nvarchar(30),
    Data Xml
)
GO

INSERT INTO ContractSelectiveIndex(DocName, Data)
SELECT 
    DocName,
    Data 
FROM Contract
GO

CREATE UNIQUE INDEX IX_ContractSelectiveIndex_DocName ON ContractSelectiveIndex(DocName)
GO

CREATE SELECTIVE XML INDEX SXI_ContractSelectiveIndex
ON ContractSelectiveIndex(Data)
FOR
(
    pathCn = 'declare namespace C="symfact/Contract";/C:Contract/C:Summary/C:GeneralInfo/C:ContractOwnerCN' AS XQUERY 'xs:string',
    pathStatus = 'declare namespace C="symfact/Contract";/C:Contract/C:Status/@status' AS XQUERY 'xs:string',
    pathContractType = 'declare namespace C="symfact/Contract";/C:Contract/C:Summary/C:GeneralInfo/C:ContractType' AS XQUERY 'xs:string'
)
GO
