using System.ComponentModel;
using Firecrawl.Console.Infrastructure;
using Spectre.Console.Cli;

namespace Firecrawl.Console.Commands;

public sealed class ScrapeCommand : AsyncCommand<ScrapeCommand.Settings>
{
    public sealed class Settings : GlobalSettings
    {
        [CommandArgument(0, "<URL>")]
        [Description("The URL to scrape")]
        public required string Url { get; init; }

        [CommandOption("--formats <FORMATS>")]
        [Description("Comma-separated output formats: markdown, html, rawHtml, screenshot, links")]
        public string? Formats { get; init; }

        [CommandOption("--only-main-content")]
        [Description("Exclude headers, navigation, footers (default: true)")]
        public bool? OnlyMainContent { get; init; }

        [CommandOption("--only-clean-content")]
        [Description("LLM-based pass to remove residual boilerplate")]
        public bool OnlyCleanContent { get; init; }

        [CommandOption("--include-tags <TAGS>")]
        [Description("Comma-separated HTML tags to include")]
        public string? IncludeTags { get; init; }

        [CommandOption("--exclude-tags <TAGS>")]
        [Description("Comma-separated HTML tags to exclude")]
        public string? ExcludeTags { get; init; }

        [CommandOption("--wait-for <MS>")]
        [Description("Milliseconds to wait before extracting content")]
        public int? WaitFor { get; init; }

        [CommandOption("--timeout <MS>")]
        [Description("Request timeout in milliseconds (default: 60000)")]
        public int? Timeout { get; init; }

        [CommandOption("--mobile")]
        [Description("Emulate a mobile device")]
        public bool Mobile { get; init; }

        [CommandOption("--block-ads")]
        [Description("Block ads and cookie popups (default: true)")]
        public bool? BlockAds { get; init; }

        [CommandOption("--remove-base64-images")]
        [Description("Strip base64 images from markdown (default: true)")]
        public bool? RemoveBase64Images { get; init; }
    }

    protected override async Task<int> ExecuteAsync(CommandContext context, Settings settings, CancellationToken cancellation)
    {
        using var client = settings.CreateClient();

        var body = new Dictionary<string, object> { ["url"] = settings.Url };

        if (settings.Formats is not null)
            body["formats"] = SplitCsv(settings.Formats);

        if (settings.OnlyMainContent is not null) body["onlyMainContent"] = settings.OnlyMainContent;
        if (settings.OnlyCleanContent) body["onlyCleanContent"] = true;
        if (settings.WaitFor is not null) body["waitFor"] = settings.WaitFor;
        if (settings.Timeout is not null) body["timeout"] = settings.Timeout;
        if (settings.Mobile) body["mobile"] = true;
        if (settings.BlockAds is not null) body["blockAds"] = settings.BlockAds;
        if (settings.RemoveBase64Images is not null) body["removeBase64Images"] = settings.RemoveBase64Images;

        if (settings.IncludeTags is not null)
            body["includeTags"] = SplitCsv(settings.IncludeTags);

        if (settings.ExcludeTags is not null)
            body["excludeTags"] = SplitCsv(settings.ExcludeTags);

        var result = await client.PostAsync("scrape", body);
        YamlOutput.Write(result);
        return 0;
    }

    private static string[] SplitCsv(string value) =>
        value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
}
