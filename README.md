# Firecrawl.Console

CLI for the [Firecrawl](https://firecrawl.dev) web scraping API — YAML-first output optimized for LLM agent consumption.

## Installation

```bash
dotnet tool install -g Firecrawl.Console
```

## Authentication

Set your API key as an environment variable:

```bash
export FIRECRAWL_API_KEY=fc-your-key-here
```

Or pass it per-command:

```bash
firecrawl scrape https://example.com --api-key fc-your-key-here
```

## Commands

### scrape — Extract content from a single URL

```bash
firecrawl scrape https://example.com
firecrawl scrape https://example.com --formats markdown,html
firecrawl scrape https://example.com --only-clean-content --mobile
```

### crawl start — Start a multi-page crawl

```bash
firecrawl crawl start https://example.com --limit 100 --max-depth 2
firecrawl crawl start https://example.com --include-paths "/blog.*" --formats markdown
```

### crawl status — Check crawl progress

```bash
firecrawl crawl status 550e8400-e29b-41d4-a716-446655440000
```

### crawl cancel — Cancel a running crawl

```bash
firecrawl crawl cancel 550e8400-e29b-41d4-a716-446655440000
```

### map — Generate URL list from a website

```bash
firecrawl map https://example.com
firecrawl map https://example.com --search blog --limit 100
```

### search — Search the web with content extraction

```bash
firecrawl search "machine learning frameworks"
firecrawl search "rust web frameworks" --limit 5 --formats markdown
```
