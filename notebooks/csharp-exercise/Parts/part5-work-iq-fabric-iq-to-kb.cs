#pragma warning disable CS1998
using Azure;
using Azure.AI.OpenAI;
using Lab532.Shared;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;

namespace Lab532.Parts;

// Part 5 — Combine restored indexes + Work IQ + Fabric IQ behind one MCP-backed agent.
// Follow labs/part5.md. Requires:  az login --tenant $env:AZURE_TENANT_ID
// 206 Partial Content is acceptable — one source can fail while others succeed.
public static class Part5
{
    private const string HrSource     = "hrdocs-knowledge-source";
    private const string HealthSource = "healthdocs-knowledge-source";
    private const string WorkSource   = "workiq-knowledge-source";
    private const string FabricSource = "fabric-ontology-knowledge-source";
    private const string KbName       = "multisource-work-fabric-knowledge-base";

    public static async Task RunAsync()
    {
        if (string.IsNullOrEmpty(Config.FabricWorkspaceId) || string.IsNullOrEmpty(Config.FabricOntologyId))
            throw new InvalidOperationException("Set FABRIC_WORKSPACE_ID and FABRIC_ONTOLOGY_ID in .env for Part 5.");

        #region Step 1: Ensure HR + health search-index sources exist
        // await SearchKbClient.PutAsync($"/knowledgesources('{HrSource}')",
        //     SearchKbClient.SearchIndexSourceBody(HrSource, "hrdocs", "LAB532 HR documents"));
        // await SearchKbClient.PutAsync($"/knowledgesources('{HealthSource}')",
        //     SearchKbClient.SearchIndexSourceBody(HealthSource, "healthdocs", "LAB532 health benefits documents"));
        #endregion

        #region Step 2: Create Work IQ + Fabric Ontology sources
        // await SearchKbClient.PutAsync(
        //     $"/knowledgesources('{WorkSource}')",
        //     new { name = WorkSource, kind = "workIQ", description = "LAB532 Work IQ knowledge source" });
        //
        // await SearchKbClient.PutAsync(
        //     $"/knowledgesources('{FabricSource}')",
        //     new
        //     {
        //         name = FabricSource,
        //         kind = "fabricOntology",
        //         description = "LAB532 Fabric Ontology knowledge source",
        //         fabricOntologyParameters = new
        //         {
        //             workspaceId = Config.FabricWorkspaceId,
        //             ontologyId = Config.FabricOntologyId,
        //         },
        //     });
        #endregion

        #region Step 3: Create the combined KB in extractiveData mode
        // var kbBody = SearchKbClient.KnowledgeBaseBody(
        //     name: KbName,
        //     description: "LAB532 KB combining restored indexes, Work IQ, and Fabric Ontology",
        //     sourceNames: new[] { HrSource, HealthSource, WorkSource, FabricSource },
        //     outputMode: "extractiveData",
        //     reasoning: "low",
        // includeModel: true);
        // await SearchKbClient.PutAsync($"/knowledgebases('{KbName}')", kbBody);
        #endregion

        #region Step 4: Direct retrieve across all four sources
        // var token = await SearchKbClient.GetQuerySourceTokenAsync();
        // var retrieveBody = new
        // {
        //     messages = new[] { new { role = "user", content = new[] { new { type = "text", text = "Gresham/Aviva context + Zava DIY Product stockLevel" } } } },
        //     knowledgeSourceParams = new object[]
        //     {
        //         new { kind = "searchIndex",    knowledgeSourceName = HrSource,     includeReferences = true, includeReferenceSourceData = true, alwaysQuerySource = true },
        //         new { kind = "searchIndex",    knowledgeSourceName = HealthSource, includeReferences = true, includeReferenceSourceData = true, alwaysQuerySource = true },
        //         new { kind = "workIQ",         knowledgeSourceName = WorkSource,   includeReferences = true, includeReferenceSourceData = true, alwaysQuerySource = true },
        //         new { kind = "fabricOntology", knowledgeSourceName = FabricSource, includeReferences = true, includeReferenceSourceData = true, alwaysQuerySource = true },
        //     },
        //     maxRuntimeInSeconds = 300,
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
        //         instructions: $"Answer using the '{KbName}' knowledge base. Use Work IQ for workplace context, Fabric Ontology for structured operational data, and the restored indexes for HR/health docs. Cite sources.",
        //         tools: [.. mcpTools.Cast<AITool>()]);
        //
        // var question =
        //     "Use Work IQ for any relevant context about Gresham Insurance Company Limited and Aviva International Insurance. " +
        //     "Use the Zava DIY Fabric ontology Product entity to list a few product names with category and stockLevel, " +
        //     "then explain how stockLevel can help with inventory planning.";
        // Console.WriteLine($"\nYou: {question}\n");
        // await foreach (var update in agent.RunStreamingAsync(question))
        //     Console.Write(update);
        // Console.WriteLine();
        #endregion
    }
}
