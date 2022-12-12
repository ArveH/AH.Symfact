namespace AH.Symfact.SqlServerLib.Extensions;

public static class StringExtensions
{
    public static string CatalogName(this string tableName)
    {
        if (tableName.StartsWith(SqlServerConstants.Name.Contract)) 
            return SqlServerConstants.Name.ContractCatalog;
        if (tableName.StartsWith(SqlServerConstants.Name.OrganisationalPerson))
            return SqlServerConstants.Name.OrgPersonCatalog;
        if (tableName.StartsWith(SqlServerConstants.Name.Party))
            return SqlServerConstants.Name.PartyCatalog;

        throw new ArgumentOutOfRangeException(nameof(tableName), $"Table '{tableName}' is unrecognized");
    }
}