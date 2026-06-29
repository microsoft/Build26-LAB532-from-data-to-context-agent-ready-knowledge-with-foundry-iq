# Deploy to Your Own Azure Subscription (C# track)

Provision the LAB532 Foundry IQ infrastructure in your own Azure subscription so you
can run the C# Parts end-to-end.

## Prerequisites

- **Azure subscription** with permissions to create resource groups, Cognitive
  Services accounts, Search services, and deploy OpenAI models
- **Azure Developer CLI (azd)** — [install](https://learn.microsoft.com/azure/developer/azure-developer-cli/install-azd)
- **Azure CLI** — [install](https://learn.microsoft.com/cli/azure/install-azure-cli)
- **.NET 8 SDK** — [install](https://dotnet.microsoft.com/download/dotnet/8.0)
- **Python 3.10+** (only used by `restore-rest.py` to seed the search indexes)

## Quick Start

### 1. Provision infrastructure

```bash
azd auth login
cd infra
azd up
```

This deploys:

- Azure AI Search service (preview API `2026-05-01-preview`)
- Microsoft Foundry / Azure OpenAI account with `gpt-5.4-mini` and
  `text-embedding-3-large` deployments
- All required RBAC role assignments

> If `text-embedding-3-large` isn't available in your region, deploy it manually:
> `az cognitiveservices account deployment create -g <rg> -n <oai-account>
>   --deployment-name text-embedding-3-large --model-name text-embedding-3-large
>   --model-version 1 --model-format OpenAI --sku-capacity 50 --sku-name Standard`

### 2. Write `.env`

Copy `.env.sample` to `.env` at the repo root and fill in the values from `azd env get-values`:

```text
AZURE_SEARCH_SERVICE_ENDPOINT=https://<your-search>.search.windows.net
AZURE_SEARCH_ADMIN_KEY=<from az search admin-key show>
AZURE_OPENAI_ENDPOINT=https://<your-foundry>.services.ai.azure.com/
AZURE_OPENAI_KEY=<from az cognitiveservices account keys list>
AZURE_OPENAI_CHATGPT_DEPLOYMENT=gpt-5.4-mini
AZURE_OPENAI_CHATGPT_MODEL_NAME=gpt-5.4-mini
AZURE_TENANT_ID=<your tenant id>
```

### 3. Populate the `hrdocs` + `healthdocs` indexes

```bash
pip install python-dotenv requests
python infra/deploy-yourself/restore-rest.py
```

This script PUTs the index schema (`data/index-data/index.json`) and bulk-uploads the
sample documents (`*.jsonl`) using the REST API. It does **not** depend on the
`azure-search-documents` Python SDK, which has a breaking change that prevents the
older `SearchIndex.deserialize()` notebook flow from working.

You should see:

```text
=== hrdocs ===
  Index created/updated
  Uploaded 50 docs in 1 batches

=== healthdocs ===
  Index created/updated
  Uploaded 334 docs in 4 batches
```

### 4. Run Part 1 to verify

```bash
az login --tenant $AZURE_TENANT_ID
cd ..
dotnet run -- 1
```

You should see the KB get built, ~20+ references returned from `/retrieve`, and the
agent answer a multi-source question with citations.

### 5. Optional: enable Parts 2–5

#### Part 2 — Web IQ

Set `WEB_IQ_KEY` in `.env` (`x-apikey` from <https://api.microsoft.ai>). That's it.

#### Parts 3 & 5 — Fabric IQ

These Parts query a Fabric Lakehouse + ontology with the Zava DIY dataset. You can
either point them at an existing Fabric workspace/ontology you already have, or
provision a fresh one with the included script.

**Provision Fabric with the script:**

```powershell
# Prereqs: Fabric capacity (Trial works) and az login with Fabric API access
# Get your Fabric capacity GUID (e.g. from the Fabric portal → Admin portal → Capacities)
# or pass an existing -WorkspaceId GUID to skip workspace creation.

cd infra
.\setup-lakehouse.ps1 `
    -CapacityId   "<fabric-capacity-guid>" `
    -TenantId     "$env:AZURE_TENANT_ID" `
    -LakehouseName "ZavaDIYLakehouse" `
    -OntologyName "ZavaDIYOntology"
```

What this does (via `create-lakehouse.py`):

1. Creates the Fabric workspace if you passed `-CapacityId`
2. Creates the lakehouse (`ZavaDIYLakehouse`)
3. Downloads the Zava DIY dataset (product + reference data) from GitHub
4. Uploads CSVs to OneLake and loads them as Delta tables
5. Creates a Fabric IQ ontology bound to those tables
6. Prints the `FABRIC_WORKSPACE_ID` and `FABRIC_ONTOLOGY_ID` you need to paste
   into the C#-track `.env`

After it finishes, copy those two values into the repo-root `.env`:

```text
FABRIC_WORKSPACE_ID=<from script output>
FABRIC_ONTOLOGY_ID=<from script output>
```

#### Parts 3, 4, 5 — delegated query-source token

Each user's delegated token is forwarded as `x-ms-query-source-authorization` for
Fabric IQ / Work IQ retrieval. Just make sure `AZURE_TENANT_ID` is set and you've run:

```powershell
az login --tenant $env:AZURE_TENANT_ID
```

## Cleanup

```bash
cd infra
azd down
```

## Additional Resources

- [Azure AI Search Documentation](https://learn.microsoft.com/azure/search/)
- [Azure OpenAI Service Documentation](https://learn.microsoft.com/azure/ai-services/openai/)
- [Microsoft Agent Framework samples](https://github.com/microsoft/agent-framework/tree/main/dotnet/samples)
