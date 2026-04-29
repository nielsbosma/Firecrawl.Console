using System.ComponentModel;
using Firecrawl.Console.Infrastructure;
using Spectre.Console.Cli;

namespace Firecrawl.Console.Commands;

public sealed class SearchCommand : AsyncCommand<SearchCommand.Settings>
{
    public sealed class Settings : GlobalSettings
    {
        [CommandArgument(0, "<QUERY>")]
        [Description("The search query (max 500 characters)")]
        public required string Query { get; init; }

        [CommandOption("--limit <COUNT>")]
        [Description("Number of results (1-100, default: 10)")]
        public int? Limit { get; init; }

        [CommandOption("--country <CODE>")]
        [Description("ISO country code (default: US)")]
        public string? Country { get; init; }

        [CommandOption("--location <LOCATION>")]
        [Description("Geo-target location string")]
        public string? Location { get; init; }

        [CommandOption("--timeout <MS>")]
        [Description("Request timeout in milliseconds (default: 60000)")]
        public int? Timeout { get; init; }

        [CommandOption("--formats <FORMATS>")]
        [Description("Comma-separated scrape formats: markdown, html, rawHtml, links")]
        public string? Formats { get; init; }

        [CommandOption("--only-main-content")]
        [Description("Exclude headers, navigation, footers")]
        public bool OnlyMainContent { get; init; }

        [CommandOption("--mobile")]
        [Description("Emulate a mobile device for scraping")]
        public bool Mobile { get; init; }
    }

    protected override async Task<int> ExecuteAsync(CommandContext context, Settings settings, CancellationToken cancellation)
    {
        using var client = settings.CreateClient();

        var body = new Dictionary<string, object> { ["query"] = settings.Query };

        if (settings.Limit is not null) body["limit"] = settings.Limit;
        if (settings.Country is not null) body["country"] = settings.Country;
        if (settings.Location is not null) body["location"] = settings.Location;
        if (settings.Timeout is not null) body["timeout"] = settings.Timeout;

        var scrapeOptions = new Dictionary<string, object>();
        if (settings.Formats is not null)
            scrapeOptions["formats"] = SplitCsv(settings.Formats);
        if (settings.OnlyMainContent) scrapeOptions["onlyMainContent"] = true;
        if (settings.Mobile) scrapeOptions["mobile"] = true;
        if (scrapeOptions.Count > 0)
            body["scrapeOptions"] = scrapeOptions;

        var result = await client.PostAsync("search", body);
        YamlOutput.Write(result);
        return 0;
    }

    private static string[] SplitCsv(string value) =>
        value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
}
