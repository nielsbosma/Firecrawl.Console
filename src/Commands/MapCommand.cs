using System.ComponentModel;
using Firecrawl.Console.Infrastructure;
using Spectre.Console.Cli;

namespace Firecrawl.Console.Commands;

public sealed class MapCommand : AsyncCommand<MapCommand.Settings>
{
    public sealed class Settings : GlobalSettings
    {
        [CommandArgument(0, "<URL>")]
        [Description("The base URL to map")]
        public required string Url { get; init; }

        [CommandOption("--search <QUERY>")]
        [Description("Query to order results by relevance")]
        public string? Search { get; init; }

        [CommandOption("--sitemap <MODE>")]
        [Description("Sitemap mode: skip, include, only (default: include)")]
        public string? Sitemap { get; init; }

        [CommandOption("--include-subdomains")]
        [Description("Include subdomains (default: true)")]
        public bool? IncludeSubdomains { get; init; }

        [CommandOption("--ignore-query-params")]
        [Description("Exclude URLs with query parameters (default: true)")]
        public bool? IgnoreQueryParameters { get; init; }

        [CommandOption("--ignore-cache")]
        [Description("Bypass the sitemap cache")]
        public bool IgnoreCache { get; init; }

        [CommandOption("--limit <COUNT>")]
        [Description("Maximum links to return (default: 5000, max: 100000)")]
        public int? Limit { get; init; }

        [CommandOption("--timeout <MS>")]
        [Description("Timeout in milliseconds")]
        public int? Timeout { get; init; }
    }

    protected override async Task<int> ExecuteAsync(CommandContext context, Settings settings, CancellationToken cancellation)
    {
        using var client = settings.CreateClient();

        var body = new Dictionary<string, object> { ["url"] = settings.Url };

        if (settings.Search is not null) body["search"] = settings.Search;
        if (settings.Sitemap is not null) body["sitemap"] = settings.Sitemap;
        if (settings.IncludeSubdomains is not null) body["includeSubdomains"] = settings.IncludeSubdomains;
        if (settings.IgnoreQueryParameters is not null) body["ignoreQueryParameters"] = settings.IgnoreQueryParameters;
        if (settings.IgnoreCache) body["ignoreCache"] = true;
        if (settings.Limit is not null) body["limit"] = settings.Limit;
        if (settings.Timeout is not null) body["timeout"] = settings.Timeout;

        var result = await client.PostAsync("map", body);
        YamlOutput.Write(result);
        return 0;
    }
}
