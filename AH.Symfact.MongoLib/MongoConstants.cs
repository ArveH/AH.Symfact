namespace AH.Symfact.MongoLib;

public static class MongoConstants
{
    public static Dictionary<string, (string, string, string)> DataInfo = new()
    {
        { SharedConstants.Name.Contract, ("Contract.xml", "//C:Contract", "C:") },
        { SharedConstants.Name.Party, ("Party.xml", "//P:Party", "P:") },
        { SharedConstants.Name.User, ("OrganisationalPerson.xml", "//ctxO:OrganisationalPerson", "ctxO:") },
        { SharedConstants.Name.NonXML, ("NonXMLFull.xml", "//nonXML", "") },
    };
}