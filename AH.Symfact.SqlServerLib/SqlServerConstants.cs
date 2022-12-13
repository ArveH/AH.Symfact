namespace AH.Symfact.SqlServerLib;

public static class SqlServerConstants
{
    public const string ContractXCol = "contractXCol";
    public const string ContractXOrg = "contractXOrg";

    public const string TableSuffixPlaceHolder = "<<<TABLESUFFIX>>>";

    public static IReadOnlyList<string> ContractXColFiles = new List<string>{
        "Contract.xsd",
        "Clause.xsd",
        "dc.xsd",
        "meta.xsd",
        "office.xsd",
        "Party.xsd",
        "template.xsd"
    };

    public static IReadOnlyList<string> ContractXOrgFiles = new List<string>{
        "contractXOrganisationalPerson.xsd",
        "OrganisationalUnit.xsd",
        "UserProfile.xsd"
    };

    public static IReadOnlyList<string> TableTypes = new List<string>{
        "Source",
        "ComputedColumns",
        "ExtractedColumns",
        "SelectiveIndex",
        "NoSchema"
    };

    public static class Name
    {
        public const string ContractComputedColumns = SharedConstants.Name.Contract + "ComputedColumns";
        public const string ContractExtractedColumns = SharedConstants.Name.Contract + "ExtractedColumns";
        public const string ContractSelectiveIndex = SharedConstants.Name.Contract + "SelectiveIndex";
        public const string ContractNoSchema = SharedConstants.Name.Contract + "NoSchema";
        public const string OrganisationalPersonComputedColumns = SharedConstants.Name.OrganisationalPerson + "ComputedColumns";
        public const string OrganisationalPersonExtractedColumns = SharedConstants.Name.OrganisationalPerson + "PersonExtractedColumns";
        public const string OrganisationalPersonSelectiveIndex = SharedConstants.Name.OrganisationalPerson + "SelectiveIndex";
        public const string OrganisationalPersonNoSchema = SharedConstants.Name.OrganisationalPerson + "NoSchema";
        public const string PartyComputedColumns = SharedConstants.Name.Party + "ComputedColumns";
        public const string PartyExtractedColumns = SharedConstants.Name.Party + "ExtractedColumns";
        public const string PartySelectiveIndex = SharedConstants.Name.Party + "SelectiveIndex";
        public const string PartyNoSchema = SharedConstants.Name.Party + "NoSchema";

        public const string ContractCatalog = "ContractCatalog";
        public const string OrgPersonCatalog = "OrgPersonCatalog";
        public const string PartyCatalog = "PartyCatalog";
    }

    public static IReadOnlyList<string> AllTables = new List<string>{
        SharedConstants.Name.Contract,
        Name.ContractComputedColumns,
        Name.ContractExtractedColumns,
        Name.ContractSelectiveIndex,
        Name.ContractNoSchema,
        SharedConstants.Name.OrganisationalPerson,
        Name.OrganisationalPersonComputedColumns,
        Name.OrganisationalPersonExtractedColumns,
        Name.OrganisationalPersonSelectiveIndex,
        Name.OrganisationalPersonNoSchema,
        SharedConstants.Name.Party,
        Name.PartyComputedColumns,
        Name.PartyExtractedColumns,
        Name.PartySelectiveIndex,
        Name.PartyNoSchema
    };
}