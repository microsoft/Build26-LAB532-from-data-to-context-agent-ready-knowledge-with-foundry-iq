#pragma warning disable CS1998
using Azure;
using Azure.AI.OpenAI;
using Lab532.Shared;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;

namespace Lab532.Parts;

// Part 2 — Add Web IQ (web grounding via MCP) and talk to it through an MAF agent
// pointed at the KB's MCP endpoint. Follow docs/part2.md.
public static class Part2
{
    private const string HrSource     = "hrdocs-knowledge-source";
    private const string HealthSource = "healthdocs-knowledge-source";
    private const string WebSource    = "web-knowledge-source";
    private const string KbName       = "multisource-web-knowledge-base";

    public static async Task RunAsync()
    {
        if (string.IsNullOrEmpty(Config.WebIqKey))
            throw new InvalidOperationException("Set WEB_IQ_KEY in .env for Part 2.");

        #region Step 1: Ensure HR + health search-index sources exist
        // await SearchKbClient.PutAsync($"/knowledgesources('{HrSource}')",
        //     SearchKbClient.SearchIndexSourceBody(HrSource, "hrdocs", "LAB532 HR documents"));
        // await SearchKbClient.PutAsync($"/knowledgesources('{HealthSource}')",
        //     SearchKbClient.SearchIndexSourceBody(HealthSource, "healthdocs", "LAB532 health benefits documents"));
        #endregion

        #region Step 2: Create the Web IQ knowledge source (MCP server, key auth)
        // var webBody = new
        // {
        //     name = WebSource,
        //     kind = "mcpServer",
        //     description = "LAB532 Web IQ knowledge source",
        //     mcpServerParameters = new
        //     {
        //         serverURL = "https://api.microsoft.ai/v3/mcp",
        //         authentication = new
        //         {
        //             kind = "storedHeaders",
        //             storedHeadersParameters = new
        //             {
        //                 headers = new Dictionary<string, string> { ["x-apikey"] = Config.WebIqKey! },
        //             },
        //         },
        //         tools = new[] { new { name = "web", outputParsing = new { kind = "auto" } } },
        //     },
        // };
        // await SearchKbClient.PutAsync($"/knowledgesources('{WebSource}')", webBody);
        #endregion

        #region Step 3: Create the KB in extractiveData mode (no model attached)
        // var kbBody = SearchKbClient.KnowledgeBaseBody(
        //     name: KbName,
        //     description: "LAB532 KB combining restored indexes and Web IQ",
        //     sourceNames: new[] { HrSource, HealthSource, WebSource },
        //     outputMode: "extractiveData",
        //     reasoning: "minimal",
        //     includeModel: false);
        // await SearchKbClient.PutAsync($"/knowledgebases('{KbName}')", kbBody);
        #endregion

        #region Step 4: Direct retrieve — see what the KB returns from all three sources
        // var retrieveBody = new
        // {
        //     intents = new[] { new { type = "semantic", search = "employee health benefits and Azure AI Search knowledge base preview" } },
        //     knowledgeSourceParams = new object[]
        //     {
        //         new { kind = "searchIndex", knowledgeSourceName = HrSource,     includeReferences = true, includeReferenceSourceData = true, alwaysQuerySource = true },
        //         new { kind = "searchIndex", knowledgeSourceName = HealthSource, includeReferences = true, includeReferenceSourceData = true, alwaysQuerySource = true },
        //         new { kind = "mcpServer",   knowledgeSourceName = WebSource,    includeReferences = true, includeReferenceSourceData = true, alwaysQuerySource = true },
        //     },
        // };
        // using var retrieved = await SearchKbClient.PostAsync($"/knowledgebases('{KbName}')/retrieve", retrieveBody);
        // var refCount = retrieved.RootElement.TryGetProperty("references", out var refs) ? refs.GetArrayLength() : 0;
        // Console.WriteLine($"Direct retrieve returned {refCount} reference(s).");
        #endregion

        #region Step 5: Build an MAF agent over the KB's MCP endpoint
        // await using McpClient mcpClient = await McpKbAgent.ConnectAsync(KbName);
        // IList<McpClientTool> mcpTools = await mcpClient.ListToolsAsync();
        // Console.WriteLine($"MCP tools: {string.Join(", ", mcpTools.Select(t => t.Name))}");
        //
        // AIAgent agent = new AzureOpenAIClient(new Uri(Config.OpenAIEndpoint), new AzureKeyCredential(Config.OpenAIKey))
        //     .GetChatClient(Config.ChatDeployment)
        //     .AsIChatClient()
        //     .AsAIAgent(
        //         instructions: $"Answer using the '{KbName}' knowledge base. Use the indexes for company policy questions and Web IQ for current public web context. Cite sources.",
        //         tools: [.. mcpTools.Cast<AITool>()]);
        //
        // var question =
        //     "Answer with citations: what employee health benefits are described in the company docs, " +
        //     "and what is Azure AI Search knowledge base preview?";
        // Console.WriteLine($"\nYou: {question}\n");
        // await foreach (var update in agent.RunStreamingAsync(question))
        //     Console.Write(update);
        // Console.WriteLine();
        #endregion
    }
}
