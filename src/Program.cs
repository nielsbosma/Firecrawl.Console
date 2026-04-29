using Firecrawl.Console.Commands;
using Firecrawl.Console.Commands.Crawl;
using Spectre.Console.Cli;

var app = new CommandApp();

app.Configure(config =>
{
    config.SetApplicationName("firecrawl");

    config.AddCommand<ScrapeCommand>("scrape")
        .WithDescription("Scrape a single URL and extract content");

    config.AddBranch("crawl", crawl =>
    {
        crawl.SetDescription("Crawl websites across multiple pages");
        crawl.AddCommand<StartCrawlCommand>("start")
            .WithDescription("Start a new crawl job");
        crawl.AddCommand<StatusCrawlCommand>("status")
            .WithDescription("Get crawl job status and results");
        crawl.AddCommand<CancelCrawlCommand>("cancel")
            .WithDescription("Cancel a running crawl job");
    });

    config.AddCommand<MapCommand>("map")
        .WithDescription("Generate a list of URLs from a website");

    config.AddCommand<SearchCommand>("search")
        .WithDescription("Search the web and retrieve page content");
});

return app.Run(args);
