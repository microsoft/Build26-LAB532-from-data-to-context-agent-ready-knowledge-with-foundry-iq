#pragma warning disable CS1998
using Azure;
using Azure.AI.OpenAI;
using Lab532.Shared;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;

namespace Lab532.Parts;

// Part 4 — Add Work IQ. The MCP agent must forward YOUR delegated token. Follow docs/part4.md.
// Requires:  az login --tenant $env:AZURE_TENANT_ID
public static class Part4
{
    private const string HrSource     = "hrdocs-knowledge-source";
    private const string HealthSource = "healthdocs-knowledge-source";
    private const string WorkSource   = "workiq-knowledge-source";
    private const string KbName       = "multisource-workiq-knowledge-base";

    public static async Task RunAsync()
    {
        #region Step 1: Ensure HR + health search-index sources exist
        // await SearchKbClient.PutAsync($"/knowledgesources('{HrSource}')",
        //     SearchKbClient.SearchIndexSourceBody(HrSource, "hrdocs", "LAB532 HR documents"));
        // await SearchKbClient.PutAsync($"/knowledgesources('{HealthSource}')",
        //     SearchKbClient.SearchIndexSourceBody(HealthSource, "healthdocs", "LAB532 health benefits documents"));
        #endregion

        #region Step 2: Create the Work IQ knowledge source
        // var workBody = new { name = WorkSource, kind = "workIQ", description = "LAB532 Work IQ knowledge source" };
        // await SearchKbClient.PutAsync($"/knowledgesources('{WorkSource}')", workBody);
        #endregion

        #region Step 3: Create the KB in extractiveData mode (no model attached)
        // var kbBody = SearchKbClient.KnowledgeBaseBody(
        //     name: KbName,
        //     description: "LAB532 KB combining restored indexes and Work IQ",
        //     sourceNames: new[] { HrSource, HealthSource, WorkSource },
        //     outputMode: "extractiveData",
        //     reasoning: "minimal",
        //     includeModel: false);
        // await SearchKbClient.PutAsync($"/knowledgebases('{KbName}')", kbBody);
        #endregion

        #region Step 4: Direct retrieve (Work IQ needs the query-source token)
        // var token = await SearchKbClient.GetQuerySourceTokenAsync();
        // var retrieveBody = new
        // {
        //     intents = new[] { new { type = "semantic", search = "Gresham Insurance and Aviva International — workplace + HR context" } },
        //     knowledgeSourceParams = new object[]
        //     {
        //         new { kind = "searchIndex", knowledgeSourceName = HrSource,     includeReferences = true, includeReferenceSourceData = true, alwaysQuerySource = true },
        //         new { kind = "searchIndex", knowledgeSourceName = HealthSource, includeReferences = true, includeReferenceSourceData = true, alwaysQuerySource = true },
        //         new { kind = "workIQ",      knowledgeSourceName = WorkSource,   includeReferences = true, includeReferenceSourceData = true, alwaysQuerySource = true },
        //     },
        //     maxRuntimeInSeconds = 120,
        // };
        // using var retrieved = await SearchKbClient.PostAsync(
        //     $"/knowledgebases('{KbName}')/retrieve",
        //     retrieveBody,
        //     new Dictionary<string, string> { ["x-ms-query-source-authorization"] = token });
        // var refCount = retrieved.RootElement.TryGetProperty("references", out var refs) ? refs.GetArrayLength() : 0;
        // Console.WriteLine($"Direct retrieve returned {refCount} reference(s).");
        #endregion

        #region Step 5: Build an MAF agent over the KB's MCP endpoint (forward delegated token)
        // await using McpClient mcpClient = await McpKbAgent.ConnectAsync(KbName, useQuerySourceToken: true);
        // IList<McpClientTool> mcpTools = await mcpClient.ListToolsAsync();
        // Console.WriteLine($"MCP tools: {string.Join(", ", mcpTools.Select(t => t.Name))}");
        //
        // AIAgent agent = new AzureOpenAIClient(new Uri(Config.OpenAIEndpoint), new AzureKeyCredential(Config.OpenAIKey))
        //     .GetChatClient(Config.ChatDeployment)
        //     .AsIChatClient()
        //     .AsAIAgent(
        //         instructions: $"Answer using the '{KbName}' knowledge base. Use Work IQ for workplace context and the restored indexes for HR/health docs. Cite sources.",
        //         tools: [.. mcpTools.Cast<AITool>()]);
        //
        // var question =
        //     "Gresham Insurance Company Limited and Aviva International Insurance — " +
        //     "what relevant workplace context is available, and what HR/health coverage details apply?";
        // Console.WriteLine($"\nYou: {question}\n");
        // await foreach (var update in agent.RunStreamingAsync(question))
        //     Console.Write(update);
        // Console.WriteLine();
        #endregion
    }
}
