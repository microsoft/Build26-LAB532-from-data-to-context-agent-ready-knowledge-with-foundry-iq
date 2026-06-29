# Part 5 — Combine everything (HR + health + Web IQ + Fabric IQ + Work IQ)

The full multi-source KB: two search indexes, Web IQ, Fabric IQ, and Work IQ — all
behind one MCP endpoint, with one MAF agent in front of it.

> Open `Parts/part5-work-iq-fabric-iq-to-kb.cs`. Requires all `.env` values from Parts
> 2, 3, 4 plus `az login --tenant $env:AZURE_TENANT_ID`.

Run with:

```powershell
dotnet run -- 5
```

## Step 1 — Ensure HR + health + Web IQ sources exist

Idempotent re-creation of `searchIndex` and `mcpServer` sources.

## Step 2 — Ensure Fabric IQ + Work IQ sources exist

Idempotent re-creation of `fabricOntology` and `workplaceContent` sources.

## Step 3 — Create the all-source KB (extractiveData)

Five sources, one KB, no model attached.

## Step 4 — Direct retrieve across all five sources

Pass `useQuerySourceToken: true` — Fabric IQ and Work IQ need it; the others ignore it.
Confirms every source contributes references.

## Step 5 — MAF agent over the KB's MCP endpoint

Same agent shape as every other Part:

```csharp
await using McpClient mcpClient = await McpKbAgent.ConnectAsync(KbName, useQuerySourceToken: true);
IList<McpClientTool> mcpTools = await mcpClient.ListToolsAsync();
AIAgent agent = new AzureOpenAIClient(...).GetChatClient(...).AsAIAgent(
    instructions: "Cite which source(s) you used: indexes, Web IQ, Fabric IQ, Work IQ.",
    tools: [.. mcpTools.Cast<AITool>()]);
```

Ask a question that genuinely spans sources, e.g. *"For each of my upcoming meetings
this week, pull related HR/health policies and any current sales numbers from the
Fabric ontology, then summarize."*

## What you've learned

- A single KB can fan out to indexes, web, structured data, and per-user M365 content
- The agent surface stays tiny — one MCP connection, one tool list, one `AsAIAgent` call
- Delegated headers are the only thing that changes between "public" and
  "per-user" sources
