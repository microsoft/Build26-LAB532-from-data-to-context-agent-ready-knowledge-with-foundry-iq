# Part 2 — Add Web IQ to the knowledge base

You'll add **Web IQ** (Microsoft's web grounding MCP server) alongside the HR + health
indexes, so the same KB can answer from company docs *and* the public web.

> Open `Parts/part2-web-iq-to-kb.cs`. Requires `WEB_IQ_KEY` in `.env`.

Run with:

```powershell
dotnet run -- 2
```

## Step 1 — Ensure HR + health sources exist

Idempotent re-creation of the two `searchIndex` sources from Part 1 (safe to re-run).

## Step 2 — Create the Web IQ knowledge source

Web IQ is an **`mcpServer`** knowledge source. You point Foundry IQ at
`https://api.microsoft.ai/v3/mcp` and give it your `x-apikey` via `storedHeaders` auth —
Foundry IQ stores the key and calls Web IQ on the KB's behalf at retrieve time.

## Step 3 — Create the multi-source KB (extractiveData)

Same shape as Part 1 but with three sources. Still no model attached — retrieval only.

## Step 4 — Direct retrieve across all three sources

The question deliberately spans two topics ("employee health benefits" → indexes,
"Azure AI Search knowledge base preview" → web) so you can see chunks come back from
both kinds of sources.

## Step 5 — MAF agent over the KB's MCP endpoint

Same pattern as Part 1: `McpKbAgent.ConnectAsync(KbName)` → list tools → wrap with
`chatClient.AsAIAgent(...)`. The agent now has one tool that hides three sources behind
it. Ask a question that needs both internal docs and live web context.

## What you've learned

- `mcpServer` knowledge sources with stored-header auth
- Combining `searchIndex` and `mcpServer` sources in one KB
- The agent doesn't know or care how many sources sit behind its single MCP tool
