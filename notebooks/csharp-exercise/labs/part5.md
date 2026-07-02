# Part 5 — Combine everything (HR + health + Fabric IQ + Work IQ)

The full multi-source KB: two search indexes, Fabric IQ, and Work IQ. All four sources
sit behind one MCP endpoint, with one MAF agent in front of it.

> Open `Parts/part5-work-iq-fabric-iq-to-kb.cs`. Requires all `.env` values from Parts
> 3 and 4 plus `az login --tenant $env:AZURE_TENANT_ID`.

Run with:

```powershell
dotnet run -- 5
```

## Step 1 — Ensure HR + health sources exist

Idempotent re-creation of the two `searchIndex` sources.

## Step 2 — Ensure Work IQ + Fabric IQ sources exist

Idempotent re-creation of the `workIQ` and `fabricOntology` sources.

## Step 3 — Create the all-source KB (extractiveData) with a model attached

Four sources, one KB. `outputMode: "extractiveData"` still returns extracted chunks, but
Fabric IQ needs an Azure OpenAI model on the KB to plan its ontology queries, so this KB
passes `includeModel: true`.

## Step 4 — Direct retrieve across all four sources

Pass `useQuerySourceToken: true` — Fabric IQ and Work IQ need it; the others ignore it.
Confirms every source contributes references.

## Step 5 — MAF agent over the KB's MCP endpoint

Same agent shape as every other Part:

```csharp
await using McpClient mcpClient = await McpKbAgent.ConnectAsync(KbName, useQuerySourceToken: true);
IList<McpClientTool> mcpTools = await mcpClient.ListToolsAsync();
AIAgent agent = new AzureOpenAIClient(...).GetChatClient(...).AsAIAgent(
    instructions: "Cite which source(s) you used: indexes, Fabric IQ, Work IQ.",
    tools: [.. mcpTools.Cast<AITool>()]);
```

Ask a question that genuinely spans sources, e.g. *"For each of my upcoming meetings
this week, pull related HR/health policies and any current sales numbers from the
Fabric ontology, then summarize."*

## What you've learned

- A single KB can fan out to indexes, structured Fabric data, and per-user M365 content
- The agent surface stays tiny — one MCP connection, one tool list, one `AsAIAgent` call
- Delegated headers are the only thing that changes between "public" and
  "per-user" sources
