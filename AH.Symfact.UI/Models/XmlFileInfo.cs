namespace AH.Symfact.UI.Models;

public class XmlFileInfo
{
    public string? ContractElementName { get; set; } = "Contract";
    public int ContractCount { get; set; }
    public string? PartyElementName { get; set; } = "Party";
    public int PartyCount { get; set; }
    public string? OrgPersonElementName { get; set; } = "OrganisationalPerson";
    public int OrgPersonCount { get; set; }
}