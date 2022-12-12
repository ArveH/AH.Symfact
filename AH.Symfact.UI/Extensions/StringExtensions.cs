namespace AH.Symfact.UI.Extensions;

public static class StringExtensions
{
    public static string CatalogName(this string tableName)
    {
        if (tableName.StartsWith(SymfactConstants.Name.Contract)) 
            return SymfactConstants.Name.ContractCatalog;
        if (tableName.StartsWith(SymfactConstants.Name.OrganisationalPerson))
            return SymfactConstants.Name.OrgPersonCatalog;
        if (tableName.StartsWith(SymfactConstants.Name.Party))
            return SymfactConstants.Name.PartyCatalog;

        throw new ArgumentOutOfRangeException(nameof(tableName), $"Table '{tableName}' is unrecognized");
    }
}