using System.IO;
using Microsoft.Extensions.Configuration;

namespace AH.Symfact.UI.Config;

public static class ConfigurationHelper
{
    public static IConfigurationRoot GetConfiguration(
        string? userSecretsKey = null,
        string? basePath = null)
    {
        var builder = string.IsNullOrWhiteSpace(basePath) ?
            new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory()) :
            new ConfigurationBuilder()
                .SetBasePath(basePath);

        builder.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

        if (!string.IsNullOrWhiteSpace(userSecretsKey))
            builder.AddUserSecrets(userSecretsKey);

        builder.AddEnvironmentVariables();

        return builder.Build();
    }

}