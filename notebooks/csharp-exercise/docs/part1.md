# Part 1 — Standard Foundry IQ knowledge base

You'll combine the pre-loaded **hrdocs** and **healthdocs** search indexes into a single
Foundry IQ knowledge base (KB), retrieve raw chunks from it, then plug that KB into a
small Microsoft Agent Framework agent over its MCP endpoint.

> Open `Parts/part1-standard-foundry-iq-kb.cs` side-by-side. Each `#region Step N` block
> is commented out — uncomment one at a time as you go.

Run with:

```powershell
dotnet run -- 1
```

## Step 1 — Verify the indexes

`hrdocs` and `healthdocs` are populated either by your hosted lab environment or by
running `infra/deploy-yourself/restore-rest.py` once. Step 1 just prints what
you'll be working with — nothing to uncomment yet.

## Step 2 — Create two `searchIndex` knowledge sources

A **knowledge source** is the Foundry IQ wrapper around something a KB can query. Here
you create one `searchIndex` source per index using
`SearchKbClient.SearchIndexSourceBody(name, indexName, description)`.

## Step 3 — Create the KB in `extractiveData` mode

You build the KB over both sources with:

- `outputMode: "extractiveData"` → the KB returns raw retrieved chunks, no LLM synthesis
- `includeModel: false` → no model is attached to the KB
- `reasoning: "low"` → cheap retrieval planner

The agent (Step 5) is what actually answers — the KB just retrieves.

## Step 4 — Direct retrieve

`POST /knowledgebases('{name}')/retrieve` with a question and the list of source params.
This prints how many references came back — confirming the KB is wired up before you put
an agent in front of it.

## Step 5 — MAF agent over the KB's MCP endpoint

Every KB exposes an MCP endpoint at
`{searchEndpoint}/knowledgebases/{name}/mcp?api-version=2026-05-01-preview`.

You connect to it with `McpKbAgent.ConnectAsync(KbName)` (sends the search `api-key`
header), list the MCP tools it exposes, and hand them to a local `ChatClientAgent`:

```csharp
AIAgent agent = new AzureOpenAIClient(...)
    .GetChatClient(Config.ChatDeployment)
    .AsAIAgent(instructions: "...", tools: [.. mcpTools.Cast<AITool>()]);
```

Ask it the multi-source question and it'll call the KB tool as needed.

## What you've learned

- Knowledge sources vs. knowledge bases
- `extractiveData` mode (retrieval only, no model)
- Direct `/retrieve` to inspect what the KB sees
- Wrapping the KB's MCP endpoint as a tool for a MAF agent
