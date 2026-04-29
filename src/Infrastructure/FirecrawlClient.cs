using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace Firecrawl.Console.Infrastructure;

public sealed class FirecrawlClient : IDisposable
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };

    private readonly HttpClient _http;

    public FirecrawlClient(string apiKey)
    {
        _http = new HttpClient { BaseAddress = new Uri("https://api.firecrawl.dev/v2/") };
        _http.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", apiKey);
        _http.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<JsonDocument> GetAsync(string path)
    {
        var response = await _http.GetAsync(path);
        return await HandleResponseAsync(response);
    }

    public async Task<JsonDocument> PostAsync(string path, object body)
    {
        var response = await _http.PostAsJsonAsync(path, body, JsonOptions);
        return await HandleResponseAsync(response);
    }

    public async Task<JsonDocument> DeleteAsync(string path)
    {
        var response = await _http.DeleteAsync(path);
        return await HandleResponseAsync(response);
    }

    private static async Task<JsonDocument> HandleResponseAsync(HttpResponseMessage response)
    {
        var body = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            var message = TryExtractError(body)
                ?? $"HTTP {(int)response.StatusCode}: {body}";
            throw new HttpRequestException(message);
        }

        if (string.IsNullOrWhiteSpace(body))
            return JsonDocument.Parse("{}");

        return JsonDocument.Parse(body);
    }

    private static string? TryExtractError(string body)
    {
        try
        {
            using var doc = JsonDocument.Parse(body);
            if (doc.RootElement.TryGetProperty("error", out var error))
                return error.GetString();
        }
        catch { }
        return null;
    }

    public void Dispose() => _http.Dispose();
}
