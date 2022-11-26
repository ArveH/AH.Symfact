using System.Collections.Generic;

namespace AH.Symfact.UI;

public static class SymfactConstants
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
        "SelectiveIndex"
    };

    public static class Name
    {
        public const string Contract = "Contract";
        public const string ContractComputedColumns = "ContractComputedColumns";
        public const string ContractExtractedColumns = "ContractExtractedColumns";
        public const string ContractSelectiveIndex = "ContractSelectiveIndex";
        public const string OrganisationalPerson = "OrganisationalPerson";
        public const string OrganisationalPersonComputedColumns = "OrganisationalPersonComputedColumns";
        public const string OrganisationalPersonExtractedColumns = "OrganisationalPersonExtractedColumns";
        public const string OrganisationalPersonSelectiveIndex = "OrganisationalPersonSelectiveIndex";
        public const string Party = "Party";
        public const string PartyComputedColumns = "PartyComputedColumns";
        public const string PartyExtractedColumns = "PartyExtractedColumns";
        public const string PartySelectiveIndex = "PartySelectiveIndex";

        public const string ContractCatalog = "ContractCatalog";
        public const string OrgPersonCatalog = "OrgPersonCatalog";
        public const string PartyCatalog = "PartyCatalog";
    }

    public static IReadOnlyList<string> AllTables = new List<string>{
        Name.Contract,
        Name.ContractComputedColumns,
        Name.ContractExtractedColumns,
        Name.ContractSelectiveIndex,
        Name.OrganisationalPerson,
        Name.OrganisationalPersonComputedColumns,
        Name.OrganisationalPersonExtractedColumns,
        Name.OrganisationalPersonSelectiveIndex,
        Name.Party,
        Name.PartyComputedColumns,
        Name.PartyExtractedColumns,
        Name.PartySelectiveIndex
    };
}