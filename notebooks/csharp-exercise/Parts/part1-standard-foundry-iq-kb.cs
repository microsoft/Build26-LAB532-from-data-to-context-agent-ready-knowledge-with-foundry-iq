#pragma warning disable CS1998 // Async will activate once you uncomment the awaits below
using Azure;
using Azure.AI.OpenAI;
using Lab532.Shared;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;

namespace Lab532.Parts;

// Part 1 — Build a multi-source KB over hrdocs + healthdocs, then talk to it
// via an MAF agent wired to the KB's MCP endpoint. Follow labs/part1.md.
public static class Part1
{
    private const string HrIndex      = "hrdocs";
    private const string HealthIndex  = "healthdocs";
    private const string HrSource     = "hrdocs-knowledge-source";
    private const string HealthSource = "healthdocs-knowledge-source";
    private const string KbName       = "multisource-search-knowledge-base";

    public static async Task RunAsync()
    {
        #region Step 1: Verify which indexes you're working with
        Console.WriteLine($"Using pre-built indexes '{HrIndex}' and '{HealthIndex}'.");
        #endregion

        #region Step 2: Create two search-index knowledge sources
        // await SearchKbClient.PutAsync(
        //     $"/knowledgesources('{HrSource}')",
        //     SearchKbClient.SearchIndexSourceBody(HrSource, HrIndex,
        //         "HR documents from the restored hrdocs index"));
        //
        // await SearchKbClient.PutAsync(
        //     $"/knowledgesources('{HealthSource}')",
        //     SearchKbClient.SearchIndexSourceBody(HealthSource, HealthIndex,
        //         "Health benefits documents from the restored healthdocs index"));
        #endregion

        #region Step 3: Create the KB in extractiveData mode (no model attached)
        // var kbBody = SearchKbClient.KnowledgeBaseBody(
        //     name: KbName,
        //     description: "Multi-source knowledge base over HR and health document indexes",
        //     sourceNames: new[] { HrSource, HealthSource },
        //     outputMode: "extractiveData",
        //     reasoning: "minimal",
        //     includeModel: false);
        // await SearchKbClient.PutAsync($"/knowledgebases('{KbName}')", kbBody);
        #endregion

        #region Step 4: Direct retrieve — print the raw chunks the KB returns
        // var retrieveBody = new
        // {
        //     intents = new[] { new { type = "semantic", search = "Zava CEO responsibilities" } },
        //     knowledgeSourceParams = new object[]
        //     {
        //         new { kind = "searchIndex", knowledgeSourceName = HrSource,     includeReferences = true, includeReferenceSourceData = true, alwaysQuerySource = true },
        //         new { kind = "searchIndex", knowledgeSourceName = HealthSource, includeReferences = true, includeReferenceSourceData = true, alwaysQuerySource = true },
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
        //     .AsAIAgent(
        //         instructions: $"Answer using the '{KbName}' knowledge base via the available retrieve tool. Cite knowledge source names.",
        //         tools: [.. mcpTools.Cast<AITool>()]);
        //
        // var question =
        //     "What is the responsibility of the Zava CEO? " +
        //     "What health plan would you recommend if they wanted the best coverage for mental health services?";
        // Console.WriteLine($"\nYou: {question}\n");
        // await foreach (var update in agent.RunStreamingAsync(question))
        //     Console.Write(update);
        // Console.WriteLine();
        #endregion
    }
}
