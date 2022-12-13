namespace AH.Symfact.MongoLib.Services;

public interface ISymfactFileReader
{
    XmlNodeList ReadFromFile(
        string xmlDataFile,
        string nodesPath);
}