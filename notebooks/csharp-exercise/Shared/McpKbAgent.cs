using ModelContextProtocol.Client;

namespace Lab532.Shared;

/// <summary>
/// Helpers for connecting to a Foundry IQ knowledge base over its built-in MCP endpoint.
/// Each KB exposes:
///   {searchEndpoint}/knowledgebases/{kbName}/mcp?api-version={apiVersion}
/// We connect with the search api-key, plus a delegated x-ms-query-source-authorization
/// token when the KB has Work IQ or Fabric Ontology sources (Parts 3, 4, 5).
/// </summary>
public static class McpKbAgent
{
    /// <summary>The MCP URL for a knowledge base.</summary>
    public static string McpUrl(string kbName) =>
        $"{Config.SearchEndpoint}/knowledgebases/{kbName}/mcp?api-version={SearchKbClient.ApiVersion}";

    /// <summary>
    /// Open an MCP client against the KB's MCP endpoint. Caller disposes via <c>await using</c>.
    /// </summary>
    public static async Task<McpClient> ConnectAsync(string kbName, bool useQuerySourceToken = false)
    {
        var headers = new Dictionary<string, string> { ["api-key"] = Config.SearchApiKey };
        if (useQuerySourceToken)
            headers["x-ms-query-source-authorization"] = await SearchKbClient.GetQuerySourceTokenAsync();

        return await McpClient.CreateAsync(new HttpClientTransport(new HttpClientTransportOptions
        {
            Endpoint = new Uri(McpUrl(kbName)),
            Name = $"Lab532-{kbName}",
            AdditionalHeaders = headers,
        }));
    }
}
