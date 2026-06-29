using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Azure.Core;
using Azure.Identity;

namespace Lab532.Shared;

/// <summary>
/// Tiny HTTP client over the Azure AI Search 2026-05-01-preview KB REST API.
/// Mirrors the Python notebooks' <c>session</c> helper.
/// </summary>
public static class SearchKbClient
{
    public const string ApiVersion = "2026-05-01-preview";

    private static readonly HttpClient _http = new() { Timeout = TimeSpan.FromMinutes(5) };

    public static string Endpoint => Config.SearchEndpoint;
    public static string ApiKey   => Config.SearchApiKey;

    public static string Url(string path)
    {
        var sep = path.Contains('?') ? '&' : '?';
        return $"{Endpoint}{path}{sep}api-version={ApiVersion}";
    }

    public static Task<JsonDocument> PutAsync(string path, object body, IDictionary<string, string>? extraHeaders = null) =>
        SendAsync(HttpMethod.Put, path, body, extraHeaders);

    public static Task<JsonDocument> PostAsync(string path, object body, IDictionary<string, string>? extraHeaders = null) =>
        SendAsync(HttpMethod.Post, path, body, extraHeaders);

    public static async Task<string> DeleteAsync(string path)
    {
        using var req = new HttpRequestMessage(HttpMethod.Delete, Url(path));
        AddDefaultHeaders(req, null);
        using var resp = await _http.SendAsync(req);
        return $"{(int)resp.StatusCode} {resp.ReasonPhrase}";
    }

    /// <summary>
    /// Delegated bearer token for https://search.azure.com/.default — required for retrieves
    /// that hit Fabric Ontology or Work IQ knowledge sources.
    /// </summary>
    public static async Task<string> GetQuerySourceTokenAsync()
    {
        var credential = string.IsNullOrEmpty(Config.TenantId)
            ? new DefaultAzureCredential()
            : new DefaultAzureCredential(new DefaultAzureCredentialOptions { TenantId = Config.TenantId });
        var token = await credential.GetTokenAsync(
            new TokenRequestContext(new[] { "https://search.azure.com/.default" }),
            CancellationToken.None);
        return token.Token;
    }

    private static async Task<JsonDocument> SendAsync(HttpMethod method, string path, object body, IDictionary<string, string>? extra)
    {
        using var req = new HttpRequestMessage(method, Url(path));
        AddDefaultHeaders(req, extra);
        req.Content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
        using var resp = await _http.SendAsync(req);
        var text = await resp.Content.ReadAsStringAsync();
        if (!resp.IsSuccessStatusCode && resp.StatusCode != System.Net.HttpStatusCode.PartialContent)
            throw new HttpRequestException($"{(int)resp.StatusCode} {resp.ReasonPhrase}: {Truncate(text, 1000)}");
        return string.IsNullOrWhiteSpace(text) ? JsonDocument.Parse("{}") : JsonDocument.Parse(text);
    }

    private static void AddDefaultHeaders(HttpRequestMessage req, IDictionary<string, string>? extra)
    {
        req.Headers.Add("api-key", ApiKey);
        req.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        req.Headers.Add("Prefer", "return=representation");
        if (extra is null) return;
        foreach (var (k, v) in extra) req.Headers.TryAddWithoutValidation(k, v);
    }

    private static string Truncate(string s, int n) => s.Length <= n ? s : s[..n];

    // ---- Convenience body builders that mirror the Python notebooks ----

    public static object SearchIndexSourceBody(string name, string indexName, string description) => new
    {
        name,
        description,
        kind = "searchIndex",
        searchIndexParameters = new
        {
            searchIndexName = indexName,
            sourceDataFields = new[]
            {
                new { name = "uid" }, new { name = "snippet_parent_id" },
                new { name = "blob_path" }, new { name = "snippet" },
            },
            searchFields = new[] { new { name = "snippet" } },
            semanticConfigurationName = "semantic-configuration",
        },
    };

    public static object AzureOpenAIModel() => new
    {
        kind = "azureOpenAI",
        azureOpenAIParameters = new
        {
            resourceUri = Config.OpenAIEndpoint + "/",
            apiKey = Config.OpenAIKey,
            deploymentId = Config.ChatDeployment,
            modelName = Config.ChatModel,
        },
    };

    public static object KnowledgeBaseBody(
        string name, string description, IEnumerable<string> sourceNames,
        string outputMode = "extractiveData", string reasoning = "low",
        bool includeModel = false, string? retrievalInstructions = null)
    {
        var body = new Dictionary<string, object?>
        {
            ["name"] = name,
            ["description"] = description,
            ["outputMode"] = outputMode,
            ["retrievalReasoningEffort"] = new { kind = reasoning },
            ["knowledgeSources"] = sourceNames.Select(n => new { name = n }).ToArray(),
        };
        if (includeModel) body["models"] = new[] { AzureOpenAIModel() };
        if (!string.IsNullOrWhiteSpace(retrievalInstructions))
            body["retrievalInstructions"] = retrievalInstructions;
        return body;
    }
}
