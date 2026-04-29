using System.ComponentModel;
using Spectre.Console.Cli;

namespace Firecrawl.Console.Infrastructure;

public class GlobalSettings : CommandSettings
{
    [CommandOption("--api-key <KEY>")]
    [Description("Firecrawl API key (or set FIRECRAWL_API_KEY env var)")]
    public string? ApiKey { get; init; }

    public FirecrawlClient CreateClient()
    {
        var key = ApiKey ?? Environment.GetEnvironmentVariable("FIRECRAWL_API_KEY")
            ?? throw new InvalidOperationException(
                "API key required. Use --api-key or set FIRECRAWL_API_KEY.");
        return new FirecrawlClient(key);
    }
}
