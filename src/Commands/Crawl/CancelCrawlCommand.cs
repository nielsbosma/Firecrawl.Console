using System.ComponentModel;
using Firecrawl.Console.Infrastructure;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Firecrawl.Console.Commands.Crawl;

public sealed class CancelCrawlCommand : AsyncCommand<CancelCrawlCommand.Settings>
{
    public sealed class Settings : GlobalSettings
    {
        [CommandArgument(0, "<ID>")]
        [Description("The crawl job ID to cancel")]
        public required string Id { get; init; }
    }

    protected override async Task<int> ExecuteAsync(CommandContext context, Settings settings, CancellationToken cancellation)
    {
        using var client = settings.CreateClient();

        var result = await client.DeleteAsync($"crawl/{settings.Id}");
        YamlOutput.Write(result);
        return 0;
    }
}
