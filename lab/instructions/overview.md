## Before you begin

<details>
<summary><strong>🔑 Lab credentials (click to expand when you need to sign in)</strong></summary>

At any point during the lab, if you need to sign in to the virtual machine (Windows) or any Azure or Microsoft 365 apps (M365 Copilot, SharePoint, Teams, and so on), use the credentials provided below.

### Sign into virtual machine (Windows)

If you need to sign in the virtual machine, use the following credentials:

- **User name**: +++@lab.VirtualMachine(Win11-Pro-Base).Username+++  
- **Password**: +++@lab.VirtualMachine(Win11-Pro-Base).Password+++

### Sign into Azure & Microsoft 365

If you need to sign in to any Azure or Microsoft 365 apps, use the following credentials:

- **Username**: +++@lab.CloudPortalCredential(User1).Username+++  
- **Temporary Access Pass**: +++@lab.CloudPortalCredential(User1).AccessToken+++

</details>

## Overview

In this hands-on lab, you'll build an Azure AI Search knowledge base using agentic retrieval and extend it with Model Context Protocol (MCP) knowledge sources. You'll connect the knowledge base to both indexed enterprise content and live MCP servers, enabling grounded, citation-backed answers across multiple systems.

Through 5 progressive notebook exercises, you'll build a multi-source document-backed knowledge base, extend it with web search results through MCP, add Fabric IQ and Work IQ, and finish by combining Work IQ and Fabric IQ in one KB. By the end, you'll have flexible agentic knowledge bases that blend multiple source types.

## Getting started

Follow the steps below to set up your environment and begin the lab.

### Sign into Windows

In the virtual machine, sign into Windows using the following credentials:

- **User name**: +++@lab.VirtualMachine(Win11-Pro-Base).Username+++  
- **Password**: +++@lab.VirtualMachine(Win11-Pro-Base).Password+++

### Access the lab repository

Once signed in to the Skillable environment, you'll find the lab repository already cloned on your desktop under the folder: **Desktop > Build26-LAB532-from-data-to-context-agent-ready-knowledge-with-foundry-iq-main**.

> This folder contains all the code, notebooks, and resources you'll need for the lab.

### Open the project folder in Visual Studio Code

Open Visual Studio Code and select **File > Open Folder**. Then navigate to Desktop and select the **Build26-LAB532-from-data-to-context-agent-ready-knowledge-with-foundry-iq-main** folder and then **Select Folder**.

> [!TIP]
> * When prompted whether to trust the authors of the files, select **Yes, I trust the authors**.

### Verify the environment setup

All required Azure services including **Azure AI Search with pre-indexed data** and **Azure OpenAI deployments** have already been provisioned for you.

<details>
<summary><strong>📋 What's pre-configured (click to expand for details)</strong></summary>

- **Azure AI Search** - Standard tier with two pre-created indexes:
  - **hrdocs:** HR policies, employee handbook, role library, company overview
  - **healthdocs:** Health insurance plans, benefits options, coverage details
- **Azure OpenAI** - Deployed models **gpt-5.4** for chat completion and answer synthesis and **text-embedding-3-large** for vector embeddings
- **Pre-computed vectors** - All 384 document chunks are already vectorized and indexed

</details>

#### Verify environment variables

1. Open the **.env** file under the main project folder.  
2. Verify that it includes these environment variables:
   - *AZURE_SEARCH_SERVICE_ENDPOINT*
   - *AZURE_SEARCH_ADMIN_KEY*
   - *AZURE_OPENAI_ENDPOINT*
   - *AZURE_OPENAI_KEY*
   - *AZURE_OPENAI_CHATGPT_DEPLOYMENT*
   - *AZURE_OPENAI_CHATGPT_MODEL_NAME*
   - *AZURE_TENANT_ID*
   - *FABRIC_WORKSPACE_ID*
   - *FABRIC_ONTOLOGY_ID*
   - *MS_SPEEDBIRD_SEARCH_KEY*

If these variables are present, proceed to verify the indexes in Azure Portal.

#### Verify indexes in Azure Portal

Confirm that the search indexes have been created successfully:

1. Open a web browser and navigate to the +++https://portal.azure.com+++.
2. Sign in using your lab credentials:
    - **Username**: +++@lab.CloudPortalCredential(User1).Username+++  
    - **Temporary Access Pass**: +++@lab.CloudPortalCredential(User1).AccessToken+++
3. In the Azure Portal search bar at the top, search for +++lab532-search+++ and select your AI Search service (it will look like *lab532-search-.....*).
4. In the left navigation menu, select **Search management** > **Indexes**.
5. You should see two indexes:
   - **hrdocs** - Should show document count of 50
   - **healthdocs** - Should show document count of 334

If your indexes are present and populated, your environment is ready to use. You can now proceed to start with the Jupyter notebooks.

### Work through the Jupyter notebooks

This lab includes 5 progressive notebooks covering different knowledge base and source type patterns:

1. **Multi-source search indexes** - Build a knowledge base over the restored HR and health indexes
2. **Microsoft Speedbird Search MCP** - Add Microsoft Speedbird Search through an MCP knowledge source for web results
3. **Fabric IQ source** - Add Fabric IQ through a Fabric Ontology knowledge source
4. **Work IQ source** - Bring Work IQ into the KB as a first-party source
5. **Work IQ + Fabric IQ** - Combine workplace and structured Fabric data in one KB

Start with **part1-standard-foundry-iq-kb.ipynb** in the **notebooks/** folder and progress through each notebook sequentially.

> [!TIP]
> **Bonus: Copilot CLI sidequest** - Each notebook includes a bonus section that prints an MCP configuration for the knowledge base you just created. Follow the instructions in **notebooks/copilot-cli-sidequest.md** to add it to GitHub Copilot CLI and query your KB directly from the terminal.

Once you've completed all 5 notebooks, select **Next** to review key takeaways and next steps.