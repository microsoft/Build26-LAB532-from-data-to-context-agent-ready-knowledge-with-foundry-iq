# LAB532 — C# track

A standalone C# track for LAB532, for people who want to **read and write real C# code**
while building a Foundry IQ knowledge base.

Each Part is one C# file + one markdown guide:

| Part | C# file | Guide |
|------|---------|-------|
| 1 | `Parts/part1-standard-foundry-iq-kb.cs` | [`docs/part1.md`](docs/part1.md) |
| 2 | `Parts/part2-web-iq-to-kb.cs`  | [`docs/part2.md`](docs/part2.md) |
| 3 | `Parts/part3-fabric-iq-to-kb.cs`       | [`docs/part3.md`](docs/part3.md) |
| 4 | `Parts/part4-work-iq-to-kb.cs`         | [`docs/part4.md`](docs/part4.md) |
| 5 | `Parts/part5-work-iq-fabric-iq-to-kb.cs` | [`docs/part5.md`](docs/part5.md) |

## How a Part works

1. Open the matching `docs/partN.md` for the narrative + step explanations
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
need to run `azd up` or `restore-rest.py`.

```powershell
Copy-Item .env.sample .env
# Paste the values shown in your lab portal into .env (Search + OpenAI + Tenant)
az login --tenant $env:AZURE_TENANT_ID   # required for Parts 3, 4, 5
dotnet run -- 1
```

### Path B — Deploy yourself

Use your own Azure subscription. See [`infra/deploy-yourself/README.md`](infra/deploy-yourself/README.md)
for the full guide. TL;DR:

```powershell
# 1) Provision Search + Foundry + OpenAI deployments (gpt-5.4-mini, text-embedding-3-large)
cd ..\..\infra
azd up

# 2) Copy outputs into .env
cd ..\notebooks\csharp-exercise
Copy-Item .env.sample .env
# Fill in AZURE_SEARCH_*, AZURE_OPENAI_*, AZURE_TENANT_ID from azd outputs

# 3) Populate the hrdocs + healthdocs indexes (REST-based, no broken SDK)
pip install python-dotenv requests
python infra\deploy-yourself\restore-rest.py

# 4) Run a Part
az login --tenant $env:AZURE_TENANT_ID
dotnet run -- 1
```

For Parts 2, 3, 5 you'll also need:

- **Part 2**: `WEB_IQ_KEY` — get an `x-apikey` from <https://api.microsoft.ai>
- **Parts 3, 5**: a Fabric Lakehouse + ontology. The repo includes a script that
  provisions both end-to-end from a Fabric capacity:

  ```powershell
  cd ..\..\infra
  .\setup-lakehouse.ps1 -CapacityId "<fabric-capacity-guid>" -TenantId "$env:AZURE_TENANT_ID"
  # Copy the FABRIC_WORKSPACE_ID + FABRIC_ONTOLOGY_ID it prints into .env
  ```

  See [`infra/deploy-yourself/README.md`](infra/deploy-yourself/README.md#parts-3--5--fabric-iq) for details.
- **Parts 3, 4, 5**: `az login --tenant $env:AZURE_TENANT_ID` so your user token can be
  forwarded as `x-ms-query-source-authorization` for delegated retrieval

## Layout

```
notebooks/csharp-exercise/
├── docs/                       # 📖 narrative guides
│   └── part1.md … part5.md
├── Parts/                      # ⭐ what you edit (uncomment regions)
│   └── part1-…cs … part5-…cs
├── Shared/
│   ├── Config.cs               # .env loader
│   ├── SearchKbClient.cs       # HttpClient over Azure AI Search 2026-05-01-preview KB REST
│   └── McpKbAgent.cs           # MCP client + per-request headers for the KB endpoint
├── infra/
│   └── deploy-yourself/
│       ├── README.md           # full deploy-yourself walkthrough
│       └── restore-rest.py     # populates hrdocs + healthdocs via REST
├── Program.cs                  # arg dispatcher: runs Part1..Part5
├── KbBuilderAgent.csproj
└── .env.sample

# Reused from the repo root (shared with the Python lab):
#   ../../data/       sample HR + health docs (index.json + .jsonl)
#   ../../infra/      azd Bicep, Fabric lakehouse scripts
```

## Why this shape

- **Markdown narrative + commented C# code** — same flow as the Python notebooks, but
  every code block is real, compiled, debugger-friendly C#.
- **`#region Step N` labels** survive accidental toggle-uncomment and collapse nicely in
  the editor — you only see the step you're working on.
- **KBs are pure retrieval** (`outputMode: extractiveData`, no model attached). Step 4
  prints the raw chunks the KB returns so you can see what the agent will see.
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
