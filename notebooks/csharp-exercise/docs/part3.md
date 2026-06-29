# Part 3 — Add Fabric IQ (structured data via ontology)

You'll add a **Fabric IQ** knowledge source on top of a Microsoft Fabric workspace +
ontology, so the same KB can answer from company docs *and* structured business data.

> Open `Parts/part3-fabric-iq-to-kb.cs`. Requires `FABRIC_WORKSPACE_ID`,
> `FABRIC_ONTOLOGY_ID`, and `AZURE_TENANT_ID` in `.env`, plus
> `az login --tenant $env:AZURE_TENANT_ID`.

Run with:

```powershell
dotnet run -- 3
```

## Why "delegated" matters

Fabric IQ queries data the **end-user** is allowed to see — not the KB's identity. So
both Step 4 (direct retrieve) and Step 5 (agent) forward your user token in the
`x-ms-query-source-authorization` header. `SearchKbClient.GetQuerySourceTokenAsync()` and
`McpKbAgent.ConnectAsync(KbName, useQuerySourceToken: true)` handle that for you.

## Step 1 — Ensure HR + health sources exist

Idempotent re-creation.

## Step 2 — Create the Fabric IQ knowledge source

`kind: "fabricOntology"`. You pass the workspace + ontology IDs and Foundry IQ takes
care of schema discovery.

## Step 3 — Create the multi-source KB (extractiveData)

Three sources, retrieval only.

## Step 4 — Direct retrieve **with** the query-source token

Passes `x-ms-query-source-authorization: Bearer …` so Fabric IQ can authorize against
the user identity. Run `az login` first.

## Step 5 — MAF agent over the KB's MCP endpoint **with** the query-source header

`McpKbAgent.ConnectAsync(KbName, useQuerySourceToken: true)` attaches both `api-key` and
`x-ms-query-source-authorization` to the MCP transport. The agent calls the KB's MCP
tool; that header rides along on every tool call, so Fabric IQ enforces row-level
permissions per user.

## What you've learned

- `fabricOntology` knowledge sources
- Delegated auth via `x-ms-query-source-authorization`
- The same MAF + MCP agent pattern works whether the KB needs delegated auth or not
