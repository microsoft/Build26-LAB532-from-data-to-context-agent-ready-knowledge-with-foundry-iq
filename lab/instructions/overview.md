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

Through 5 progressive notebook exercises, you'll build a multi-source document-backed knowledge base, extend it with MAI Grounding through MCP, add Fabric IQ and Work IQ, and finish by combining Work IQ and Fabric IQ in one KB. By the end, you'll have flexible agentic knowledge bases that blend multiple source types.

## Getting started

Follow the steps below to set up your environment and begin the lab.

### Sign into Windows

In the virtual machine, sign into Windows using the following credentials:

- **User name**: +++@lab.VirtualMachine(Win11-Pro-Base).Username+++  
- **Password**: +++@lab.VirtualMachine(Win11-Pro-Base).Password+++

### Access the lab repository

Once signed in to the Skillable environment, you'll find the lab repository already cloned on your desktop under the folder: **Desktop > mvp26-LAB006-build-agentic-knowledge-bases-next-level-rag-with-azure-ai-search-main**.

> This folder contains all the code, notebooks, and resources you'll need for the lab.

### Open the project folder in Visual Studio Code

Open Visual Studio Code and select **File > Open Folder**. Then navigate to Desktop and select the **mvp26-LAB006-build-agentic-knowledge-bases-next-level-rag-with-azure-ai-search-main** folder and then **Select Folder**.

> [!TIP]
> * When prompted whether to trust the authors of the files, select **Yes, I trust the authors**.

### Verify the environment setup

All required Azure services including **Azure AI Search with pre-indexed data** and **Azure OpenAI deployments** have already been provisioned for you.

<details>
<summary><strong>📋 What's pre-configured (click to expand for details)</strong></summary>

- **Azure AI Search** - Standard tier with two pre-created indexes:
  - **hrdocs:** HR policies, employee handbook, role library, company overview
  - **healthdocs:** Health insurance plans, benefits options, coverage details
- **Azure OpenAI** - Deployed models **gpt-4.1** for chat completion and answer synthesis and **text-embedding-3-large** for vector embeddings
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
   - *PROJECT_ENDPOINT*
   - *PROJECT_RESOURCE_ID*
   - *PROJECT_CONNECTION_NAME*

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

<details>
<summary><strong>⚠️ Troubleshooting ⚠️ click to expand if environment setup fails!</strong></summary>

If the automated environment setup fails, follow these steps to configure your environment manually:

**Step 1: Configure environment variables**

1. In Visual Studio Code, locate the **.env.sample** file in the project root folder.
2. Rename **.env.sample** to **.env**.
3. Gather the required credentials from Azure Portal:

   **For Azure AI Search:**
   - Navigate to **Azure Portal** > Search for +++lab532-search+++ > Select your AI Search service
   - Go to **Settings** > **Keys**
   - Copy the **URL** (endpoint) and **Primary admin key**

   **For Azure OpenAI:**
   - Navigate to **Azure Portal** > Search for +++lab532-openai+++ > Select your OpenAI service
   - Go to **Keys and Endpoint**
   - Copy the **Endpoint** and **KEY 1**

   **For Azure Storage:**
   - Navigate to **Azure Portal** > Search for +++lab532st+++ > Select your Storage Account (it will look like *lab532st...*)
   - Go to **Security + networking** > **Access keys**
   - Copy the **Connection string** from key1

4. Update the **.env** file with your values (replace the placeholder values).

**Step 2: Create Python virtual environment**

1. Open the first notebook **notebooks/part1-standard-foundry-iq-kb.ipynb**.
2. Run the first code cell and when prompted to select a kernel, choose **Create New Environment**.
3. Select **Venv** and then select the **requirements.txt** file in the **notebooks/** folder.
4. Wait for the virtual environment to be created.

**Step 3: Run knowledge base setup script**

1. Open a new terminal in Visual Studio Code (**Terminal** > **New Terminal**).
2. Activate the virtual environment by running:

   +++.\.venv\Scripts\Activate.ps1+++

3. Run the knowledge base creation script:

   +++python infra/create-knowledge.py+++

4. Wait for the script to complete. It will create and populate the required indexes. Check **Azure AI Search > Search management > Indexes** to verify that the indexes **hrdocs** and **healthdocs** are created and populated with documents.

**Step 4: Verify GPT-4.1 model deployment**

If you encounter errors related to the GPT model when running notebook cells:

1. Navigate to +++https://portal.azure.com+++ > Select your OpenAI service.
2. Select **Go to Microsoft Foundry**.
3. Select **Deployments**.
4. Verify that **gpt-4.1** is deployed.
5. If missing, click **Create new deployment**:
   - Select **gpt-4.1** model
   - Set **Standard** deployment type
   - Make sure your existing OpenAI resource is selected
   - Click **Deploy**

</details>

### Work through the Jupyter notebooks

This lab includes 5 progressive notebooks covering the refreshed knowledge-base plan:

1. **Multi-source search indexes** — Build a knowledge base over the restored HR and health indexes
2. **MAI Grounding MCP** — Add MAI Grounding through an MCP knowledge source
3. **Fabric IQ source** — Add Fabric IQ through a Fabric Ontology knowledge source
4. **Work IQ source** — Bring Work IQ into the KB as a first-party source
5. **Work IQ + Fabric IQ** — Combine workplace and structured Fabric data in one KB

Start with **part1-standard-foundry-iq-kb.ipynb** in the **notebooks/** folder and progress through each notebook sequentially.

> [!TIP]
> **Bonus: Copilot CLI sidequest** — Each notebook includes a bonus section that prints an MCP configuration for the knowledge base you just created. Follow the instructions in **notebooks/copilot-cli-sidequest.md** to add it to GitHub Copilot CLI and query your KB directly from the terminal.

Once you've completed all 5 notebooks, select **Next** to review key takeaways and next steps.
