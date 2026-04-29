using System.ComponentModel;
using Firecrawl.Console.Infrastructure;
using Spectre.Console.Cli;

namespace Firecrawl.Console.Commands.Crawl;

public sealed class StartCrawlCommand : AsyncCommand<StartCrawlCommand.Settings>
{
    public sealed class Settings : GlobalSettings
    {
        [CommandArgument(0, "<URL>")]
        [Description("The base URL to start crawling from")]
        public required string Url { get; init; }

        [CommandOption("--limit <COUNT>")]
        [Description("Maximum pages to crawl (default: 10000)")]
        public int? Limit { get; init; }

        [CommandOption("--max-depth <DEPTH>")]
        [Description("Maximum discovery depth")]
        public int? MaxDiscoveryDepth { get; init; }

        [CommandOption("--include-paths <PATTERNS>")]
        [Description("Comma-separated URL path regex patterns to include")]
        public string? IncludePaths { get; init; }

        [CommandOption("--exclude-paths <PATTERNS>")]
        [Description("Comma-separated URL path regex patterns to exclude")]
        public string? ExcludePaths { get; init; }

        [CommandOption("--sitemap <MODE>")]
        [Description("Sitemap mode: skip, include, only (default: include)")]
        public string? Sitemap { get; init; }

        [CommandOption("--ignore-query-params")]
        [Description("Prevent re-scraping same path with different query params")]
        public bool IgnoreQueryParameters { get; init; }

        [CommandOption("--entire-domain")]
        [Description("Follow sibling/parent URLs, not just child paths")]
        public bool CrawlEntireDomain { get; init; }

        [CommandOption("--allow-external")]
        [Description("Follow external website links")]
        public bool AllowExternalLinks { get; init; }

        [CommandOption("--allow-subdomains")]
        [Description("Follow subdomain links")]
        public bool AllowSubdomains { get; init; }

        [CommandOption("--delay <SECONDS>")]
        [Description("Delay in seconds between scrapes")]
        public double? Delay { get; init; }

        [CommandOption("--formats <FORMATS>")]
        [Description("Comma-separated output formats: markdown, html, rawHtml, links")]
        public string? Formats { get; init; }

        [CommandOption("--only-main-content")]
        [Description("Exclude headers, navigation, footers (default: true)")]
        public bool? OnlyMainContent { get; init; }
    }

    protected override async Task<int> ExecuteAsync(CommandContext context, Settings settings, CancellationToken cancellation)
    {
        using var client = settings.CreateClient();

        var body = new Dictionary<string, object> { ["url"] = settings.Url };

        if (settings.Limit is not null) body["limit"] = settings.Limit;
        if (settings.MaxDiscoveryDepth is not null) body["maxDiscoveryDepth"] = settings.MaxDiscoveryDepth;
        if (settings.Sitemap is not null) body["sitemap"] = settings.Sitemap;
        if (settings.IgnoreQueryParameters) body["ignoreQueryParameters"] = true;
        if (settings.CrawlEntireDomain) body["crawlEntireDomain"] = true;
        if (settings.AllowExternalLinks) body["allowExternalLinks"] = true;
        if (settings.AllowSubdomains) body["allowSubdomains"] = true;
        if (settings.Delay is not null) body["delay"] = settings.Delay;

        if (settings.IncludePaths is not null)
            body["includePaths"] = SplitCsv(settings.IncludePaths);

        if (settings.ExcludePaths is not null)
            body["excludePaths"] = SplitCsv(settings.ExcludePaths);

        var scrapeOptions = new Dictionary<string, object>();
        if (settings.Formats is not null)
            scrapeOptions["formats"] = SplitCsv(settings.Formats);
        if (settings.OnlyMainContent is not null)
            scrapeOptions["onlyMainContent"] = settings.OnlyMainContent;
        if (scrapeOptions.Count > 0)
            body["scrapeOptions"] = scrapeOptions;

        var result = await client.PostAsync("crawl", body);
        YamlOutput.Write(result);
        return 0;
    }

    private static string[] SplitCsv(string value) =>
        value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
}
