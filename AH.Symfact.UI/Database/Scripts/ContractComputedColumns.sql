DROP TABLE IF EXISTS ContractComputedColumns
GO

CREATE OR ALTER FUNCTION getcontractid(@contract xml(Document dbo.contractXCol))
RETURNS NVARCHAR(50) WITH SCHEMABINDING AS
BEGIN
RETURN @contract.value(
    'declare namespace C="symfact/Contract";/C:Contract/@ID', 'NVARCHAR(50)')
END;
GO

CREATE OR ALTER FUNCTION getcontractcn(@contract xml(Document dbo.contractXCol))
RETURNS NVARCHAR(50) WITH SCHEMABINDING AS
BEGIN
RETURN @contract.value(
    'declare namespace C="symfact/Contract";/C:Contract/C:Summary/C:GeneralInfo/C:ContractOwnerCN', 'NVARCHAR(50)')
END;
GO

CREATE OR ALTER FUNCTION getcontractstatus(@contract xml(Document dbo.contractXCol))
RETURNS NVARCHAR(50) WITH SCHEMABINDING AS
BEGIN
RETURN @contract.value(
    'declare namespace C="symfact/Contract";/C:Status/@status', 'NVARCHAR(10)')
END;
GO

CREATE TABLE ContractComputedColumns(
    Id AS dbo.getcontractid(Data) PERSISTED PRIMARY KEY,
    ContractOwnerCN AS dbo.getcontractcn(Data) PERSISTED,
    Status AS dbo.getcontractstatus(Data) PERSISTED,
    Data Xml(Document contractXCol)
)
GO

INSERT INTO ContractComputedColumns(Data)
SELECT Data FROM Contract
GO