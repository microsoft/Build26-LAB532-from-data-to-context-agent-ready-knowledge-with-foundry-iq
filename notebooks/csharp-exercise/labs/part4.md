# Part 4 — Add Work IQ (Microsoft 365 grounding)

You'll add a **Work IQ** knowledge source on top of the user's Microsoft 365 content
(mail, files, chats, meetings) and combine it with the restored HR and health indexes in
one KB, then put a MAF agent in front of it.

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

## Step 1 — Ensure HR + health search-index sources exist

Idempotent re-creation of the two `searchIndex` sources from Part 1.

## Step 2 — Create the Work IQ knowledge source

`kind: "workIQ"`. No keys, no IDs to configure. Foundry IQ queries the user's graph at
retrieve time on behalf of the query-source token.

## Step 3 — Create the KB in extractiveData mode

Three sources (HR, health, Work IQ), retrieval only, `reasoning: "minimal"`,
`includeModel: false`. No Azure OpenAI model is attached, so the KB just returns
extracted references.

## Step 4 — Direct retrieve with the query-source token

The retrieve forwards `x-ms-query-source-authorization` so Work IQ can authorize against
your identity. Watch the references that come back. The Work IQ ones came from your own
M365 graph, filtered to what you can access.

## Step 5 — MAF agent over the KB's MCP endpoint with delegated header

`McpKbAgent.ConnectAsync(KbName, useQuerySourceToken: true)` again. Ask the agent a
question that spans workplace and HR/health context. It calls the KB's MCP tool with your
delegated token attached, so Work IQ scopes results to you.

## What you've learned

- `workIQ` knowledge sources
- Combining per-user M365 content with indexed docs in a single KB
- Delegated auth is identical to Part 3. Same helper, same header
