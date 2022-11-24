using System.Collections.Generic;

namespace AH.Symfact.UI;

public static class SymfactConstants
{
    public const string ContractXCol = "contractXCol";
    public const string ContractXOrg = "contractXOrg";

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
}