namespace AH.Symfact.MongoLib;

public static class MongoConstants
{
    public static Dictionary<string, (string, string)> DataInfo = new Dictionary<string, (string, string)>
    {
        { SharedConstants.Name.Contract, ("Contract.xml", "//C:Contract") },
        { SharedConstants.Name.Party, ("Party.xml", "//P:Party") },
        { SharedConstants.Name.OrganisationalPerson, ("OrganisationalPerson.xml", "//ctxO:OrganisationalPerson") },
        { SharedConstants.Name.NonXML, ("NonXMLFull.xml", "//nonXML") },
    };
}