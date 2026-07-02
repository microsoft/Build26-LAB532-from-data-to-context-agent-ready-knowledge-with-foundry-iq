# LAB532 — C# track

A standalone C# track for LAB532, for people who want to **read and write real C# code**
while building a Foundry IQ knowledge base.

Each Part is one C# file + one markdown guide:

| Part | C# file | Guide |
|------|---------|-------|
| 1 | `Parts/part1-standard-foundry-iq-kb.cs` | [`labs/part1.md`](labs/part1.md) |
| 2 | `Parts/part2-web-iq-to-kb.cs`  | [`labs/part2.md`](labs/part2.md) |
| 3 | `Parts/part3-fabric-iq-to-kb.cs`       | [`labs/part3.md`](labs/part3.md) |
| 4 | `Parts/part4-work-iq-to-kb.cs`         | [`labs/part4.md`](labs/part4.md) |
| 5 | `Parts/part5-work-iq-fabric-iq-to-kb.cs` | [`labs/part5.md`](labs/part5.md) |

## How a Part works

1. Open the matching `labs/partN.md` for the narrative + step explanations
2. Open the `Parts/partN-*.cs` file side-by-side
3. Each `#region Step N` is collapsible — open one at a time, **uncomment** the code, read it
4. When all regions are uncommented, run:

   ```powershell
   dotnet run -- 1     # or 2, 3, 4, 5
   ```

The step code is pre-written and commented out. You're not writing C# from scratch —
you're walking through real SDK calls, learning what each one does.

## Setup

Pick one of the two paths below — the lab works the same way either way.

### Prereqs (both paths)

- .NET 8 SDK or newer
- Azure CLI (`az`)
- VS Code with the **C# Dev Kit** extension (you'll be prompted on open)
- (Optional) Use the included `.devcontainer/` for a one-click environment with
  .NET 8, Azure CLI, azd, and Python

### Path A — Skillable / hosted lab

The hosted lab environment comes with a pre-provisioned Azure AI Search service, an
OpenAI resource, and the `hrdocs` + `healthdocs` indexes already populated. You don't
need to run `azd up`.

```powershell
Copy-Item .env.sample .env
# Paste the values shown in your lab portal into .env (Search + OpenAI + Tenant)
az login --tenant $env:AZURE_TENANT_ID   # required for Parts 3, 4, 5
dotnet run -- 1
```

### Path B — Deploy yourself

Use your own Azure subscription. The C# track reuses the **same** infrastructure as the
Python lab, so follow the repo-root [`deploy_yourself.md`](../../deploy_yourself.md) guide.
A single `azd up` provisions Search + Foundry + OpenAI, writes a repo-root `.env`,
creates the `hrdocs` + `healthdocs` indexes, uploads the sample data, and sets up the
Fabric Lakehouse + ontology. TL;DR:

```powershell
# 1) From the repo root: provision everything + write .env
azd auth login
azd up

# 2) Run a Part (Config.cs auto-discovers the repo-root .env azd wrote)
cd notebooks\csharp-exercise
az login --tenant $env:AZURE_TENANT_ID
dotnet run -- 1
```

For Parts 2, 3, 4, 5 you'll also need:

- **Part 2**: `WEB_IQ_KEY` in `.env` — get an `x-apikey` from <https://api.microsoft.ai>
- **Parts 3, 5**: `FABRIC_WORKSPACE_ID` + `FABRIC_ONTOLOGY_ID` in `.env`. `azd up`
  provisions the Fabric Lakehouse + ontology and writes these values. See
  [`deploy_yourself.md`](../../deploy_yourself.md) for details.
- **Parts 3, 4, 5**: `az login --tenant $env:AZURE_TENANT_ID` so your user token can be
  forwarded as `x-ms-query-source-authorization` for delegated retrieval

## Layout

```
notebooks/csharp-exercise/
├── labs/                       # 📖 narrative guides
│   └── part1.md … part5.md
├── Parts/                      # ⭐ what you edit (uncomment regions)
│   └── part1-…cs … part5-…cs
├── Shared/
│   ├── Config.cs               # .env loader (walks up to the repo-root .env)
│   ├── SearchKbClient.cs       # HttpClient over Azure AI Search 2026-05-01-preview KB REST
│   └── McpKbAgent.cs           # MCP client + per-request headers for the KB endpoint
├── Program.cs                  # arg dispatcher: runs Part1..Part5
├── KbBuilderAgent.csproj
└── .env.sample

# Reused from the repo root (shared with the Python lab):
#   ../../data/              sample HR + health docs (index.json + .jsonl)
#   ../../infra/             azd Bicep, index seeding, Fabric lakehouse scripts
#   ../../deploy_yourself.md self-deploy guide (azd up)
```

## Why this shape

- **Markdown narrative + commented C# code** — same flow as the Python notebooks, but
  every code block is real, compiled, debugger-friendly C#.
- **`#region Step N` labels** survive accidental toggle-uncomment and collapse nicely in
  the editor — you only see the step you're working on.
- **KBs use `outputMode: extractiveData`**, so Step 4 prints the raw chunks the KB
  returns and you can see exactly what the agent will see. Parts 1, 2, and 4 attach no
  model. Parts 3 and 5 attach an Azure OpenAI model because Fabric IQ needs one to plan
  its ontology queries.
- **One small Microsoft Agent Framework agent** wraps the KB via its MCP endpoint
  (`/knowledgebases/{name}/mcp`). `Shared/McpKbAgent.cs` handles the `api-key` and the
  delegated `x-ms-query-source-authorization` header (Parts 3, 4, 5).

## Notes

- The KB shape (knowledgesources/knowledgebases/retrieve, api-version `2026-05-01-preview`)
  isn't covered end-to-end by `Azure.Search.Documents` yet, so this project calls the REST
  API directly via `HttpClient` — exactly mirroring the Python notebooks' `requests` usage.
- The agent is a local `ChatClientAgent` over Azure OpenAI (`chatClient.AsAIAgent(...)`),
  not a Foundry-hosted agent. That lets us forward each user's delegated query-source
  token client-side for Fabric IQ and Work IQ.
