# Deploy to Your Own Azure Subscription

This folder contains resources for deploying the LAB532 Knowledge Base infrastructure to your own Azure subscription.

## Prerequisites

- **Azure subscription** with sufficient permissions to create resources
- **Azure Developer CLI (azd)** installed ([Install guide](https://learn.microsoft.com/azure/developer/azure-developer-cli/install-azd))
- **Azure CLI** installed and configured ([Install guide](https://learn.microsoft.com/cli/azure/install-azure-cli))
- **Python 3.10+** installed
- **Git** (to clone this repository)
- **VS Code** or **GitHub Codespaces** with Jupyter extension (recommended)

### Required Azure Permissions

You'll need permissions to:

- Create resource groups
- Deploy Bicep templates
- Create and manage:
  - Azure AI Search services
  - Microsoft Foundry projects
  - Azure OpenAI model deployments
- Assign Azure RBAC roles

## Quick Start

### 1. Clone the Repository

```bash
git clone https://github.com/microsoft/Build26-LAB532-from-data-to-context-agent-ready-knowledge-with-foundry-iq.git
cd Build26-LAB532-from-data-to-context-agent-ready-knowledge-with-foundry-iq
```

### 2. Create a Python virtual environment

```bash
python3 -m venv .venv
source .venv/bin/activate
```

### 3. Deploy with azd

```bash
azd auth login
azd up
```

This will:

- Provision all Azure resources (AI Search, Foundry project, OpenAI models, Fabric capacity)
- Fetch API keys and write a `.env` file with all required variables
- Create search indexes and upload sample data
- Set up the Fabric Lakehouse with Zava DIY dataset and ontology

> **Note:** Email seeding (used in the hosted Skillable lab for Part 4 - Work IQ) requires a service principal with `Mail.Send` application permission and is **not run** during self-deploy. Part 4 will use your own mailbox data instead.

### 4. Start the Lab

Open the [notebooks](../../notebooks) folder in VS Code and **start with `part1-standard-foundry-iq-kb.ipynb`**.

## Cleanup

To delete all resources and avoid ongoing charges:

```bash
azd down
```

## Additional Resources

- [Azure AI Search Documentation](https://learn.microsoft.com/azure/search/)
- [Azure OpenAI Service Documentation](https://learn.microsoft.com/azure/ai-services/openai/)
- [Azure Bicep Documentation](https://learn.microsoft.com/azure/azure-resource-manager/bicep/)
- [Microsoft Foundry Community Discord](https://aka.ms/AIFoundryDiscord-Ignite25)
