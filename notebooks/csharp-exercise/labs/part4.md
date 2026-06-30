# Part 4 — Add Work IQ (Microsoft 365 grounding)

You'll add a **Work IQ** knowledge source on top of the user's Microsoft 365 content
(mail, files, chats, meetings) and put a MAF agent in front of it.

> Open `Parts/part4-work-iq-to-kb.cs`. Requires `AZURE_TENANT_ID` and
> `az login --tenant $env:AZURE_TENANT_ID`.

Run with:

```powershell
dotnet run -- 4
```

## Why "delegated" matters (again)

Work IQ only ever returns content the **signed-in user** is allowed to see. Same
mechanism as Part 3: `x-ms-query-source-authorization: Bearer …` forwarded at retrieve
time and on every MCP tool call.

## Step 1 — Create the Work IQ knowledge source

`kind: "workplaceContent"`. No keys, no IDs to configure — Foundry IQ asks the user's
graph at retrieve time on behalf of the query-source token.

## Step 2 — Create the KB in extractiveData mode

Single Work IQ source, retrieval only, `reasoning: "minimal"`.

## Step 3 — *(merged into Steps 1 and 2 above)*

This Part has fewer setup steps than Parts 1–3.

## Step 4 — Direct retrieve with the query-source token

Ask something like "summarize my last week of meetings" and watch the references that
come back — these came from your own M365 graph, filtered to what you can access.

## Step 5 — MAF agent over the KB's MCP endpoint with delegated header

`McpKbAgent.ConnectAsync(KbName, useQuerySourceToken: true)` again. Ask the agent a
natural M365 question; it calls the KB's MCP tool with your delegated token attached, so
Work IQ scopes results to you.

## What you've learned

- `workplaceContent` knowledge sources
- A KB can have a single source and still benefit from being agent-callable over MCP
- Delegated auth is identical to Part 3 — same helper, same header
