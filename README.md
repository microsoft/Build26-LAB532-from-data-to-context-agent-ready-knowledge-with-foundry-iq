# 🚀 Get Started

**This repo is where attendees go to continue their learning after your session — and your Copilot agent will help you set it up.**

### Step 1: Open your repo

Open this repo in a **Codespace** (click the green **Code** button → **Create a Codespace**) — or clone it locally. Then open **GitHub Copilot Chat**.

### Step 2: Add your content

Give the agent something to work with. Drag files into the Explorer panel — session abstracts, outlines, screenshots, notes — and drop them in one of two places:

| Where to put it | What goes there | Who sees it |
|---|---|---|
| **`_remove-before-publish/`** | Internal reference materials (abstracts, outlines, screenshots, planning docs) | **Copilot only** — never published |
| **`/docs/`, `/src/`, or repo root** | Lab instructions, demo code, sample data, getting-started guides | **Attendees** — published with the repo |

> 💡 Not sure? Start by dropping your session abstract or outline into `_remove-before-publish/`. The agent will figure out what to do with it.

### Step 3: Ask the Agent

Once your content is in the repo, use these three phrases with Copilot to build out your session repo:

| Phrase to use with Copilot | What it does | When to run it |
|---|---|---|
| **"Help me get started"** | Sets up session title, description, outcomes, and owners | After you've added your session abstract or outline to the repo |
| **"Help me refine content"** | Organizes your session content into the repo | Each time you add or update content |
| **"Help me finalize"** | Final review, cleanup, and publication prep | When you're ready to publish |

> 💡 **These three phrases are just the starting point.** Copilot can do much more — try asking it to brainstorm next steps for attendees, generate code samples, or build out your repo structure. Don't be afraid to put it in plan mode and ask for what you need.

---

<p align="center">
<img src="img/banner-build-26.png" alt="Microsoft Build 2026" width="1200"/>
</p>

# [Microsoft Build 2026](https://build.microsoft.com)

## 🔥 LAB532: Build Agentic Knowledge Bases: Next-Level RAG with Azure AI Search

### Session Description

In this hands-on lab, you'll build Azure AI Search knowledge bases using agentic retrieval and extend them with multiple source types. Through 5 progressive notebook exercises, you'll build a multi-source document-backed knowledge base, extend it with MAI Grounding through MCP, add Fabric IQ and Work IQ, and finish by combining Work IQ and Fabric IQ in one KB. By the end, you'll have flexible agentic knowledge bases that blend multiple source types.

### 🏫 Getting started in a guided session

To get started in a guided lab session:
- Open the lab environment and sign in with the provided credentials
- Open the **notebooks/** folder in Visual Studio Code
- Start with **part1-standard-foundry-iq-kb.ipynb** and work through all 5 notebooks sequentially

### 🏠 Getting started in your own environment

If you're following these steps at your own pace:
- Clone this repository
- Set up your development environment (see [lab/README.md](lab/README.md) for prerequisites)
- Configure Azure AI Search, Azure OpenAI, and related services using the `.env.sample` file

### 🧠 Learning Outcomes

By the end of this session, you will be able to:

- Build a multi-source knowledge base over indexed enterprise content using Azure AI Search agentic retrieval
- Extend a knowledge base with MAI Grounding, Fabric IQ, and Work IQ knowledge sources
- Combine multiple source types (indexed, structured, workplace, and web-grounded) in a single knowledge base
- Query knowledge bases with citation-backed answer synthesis

### 💬 Keep Learning with Copilot

Try these prompts with GitHub Copilot to explore the topics from this session. Open Copilot Chat in VS Code (`Ctrl+Alt+I` on Windows/Linux, `Cmd+Shift+I` on Mac), paste a prompt, and see what you learn. Try connecting the [Microsoft Learn MCP Server](#-microsoft-learn-mcp-server) for the latest official documentation.

Use these as a starting point — or write your own!

- "What is agentic retrieval in Azure AI Search and how does it differ from standard RAG?"
- "How do I create a knowledge base with multiple knowledge sources in Azure AI Search?"
- "What is MAI Grounding and how can I add it as an MCP knowledge source?"
- "How do Fabric IQ and Work IQ integrate with Azure AI Search knowledge bases?"

### 💻 Technologies Used

1. Azure AI Search (agentic retrieval, knowledge bases)
1. Azure OpenAI (gpt-5.4-mini, text-embedding-3-large)
1. Model Context Protocol (MCP)
1. Microsoft Fabric IQ and Work IQ
1. Python and Jupyter Notebooks

### 📚 Resources and Next Steps

| Resource | Description |
|:---------|:------------|
| [Azure AI Search](https://learn.microsoft.com/azure/search/) | Full capabilities of Azure AI Search |
| [Design an index for agentic retrieval](https://learn.microsoft.com/azure/search/search-agentic-retrieval-how-to-index) | Best practices for structuring data for agentic retrieval |
| [Create a knowledge base](https://learn.microsoft.com/azure/search/search-agentic-retrieval-how-to-create) | Step-by-step guide to creating and configuring knowledge bases |
| [Answer synthesis](https://learn.microsoft.com/azure/search/search-agentic-retrieval-how-to-synthesize) | Generate grounded answers with citations |
| [https://aka.ms/build26-next-steps](https://aka.ms/build26-next-steps) | Take the next step in your learning journey after Build 2026 |


### 🌟 Microsoft Learn MCP Server

[![Install in VS Code](https://img.shields.io/badge/VS_Code-Install_Microsoft_Docs_MCP-0098FF?style=flat-square&logo=visualstudiocode&logoColor=white)](https://vscode.dev/redirect/mcp/install?name=microsoft.docs.mcp&config=%7B%22type%22%3A%22http%22%2C%22url%22%3A%22https%3A%2F%2Flearn.microsoft.com%2Fapi%2Fmcp%22%7D)

The Microsoft Learn MCP Server is a remote MCP Server that enables clients like GitHub Copilot and other AI agents to bring trusted and up-to-date information directly from Microsoft's official documentation. Get started by using the one-click button above for VSCode or access the [mcp.json](.vscode/mcp.json) file included in this repo.

For more information, setup instructions for other dev clients, and to post comments and questions, visit our Learn MCP Server GitHub repo at [https://github.com/MicrosoftDocs/MCP](https://github.com/MicrosoftDocs/MCP). Find other MCP Servers to connect your agent to at [https://mcp.azure.com](https://mcp.azure.com).

*Note: When you use the Learn MCP Server, you agree with [Microsoft Learn](https://learn.microsoft.com/en-us/legal/termsofuse) and [Microsoft API Terms](https://learn.microsoft.com/en-us/legal/microsoft-apis/terms-of-use) of Use.*

## Content Owners

<!-- TODO: Add yourself as a content owner
1. Change the src in the image tag to {your github url}.png
2. Change INSERT NAME HERE to your name
3. Change the github url in the final href to your url. -->

<table>
<tr>
    <td align="center"><a href="http://github.com/yourGitHubHandle">
        <img src="https://github.com/yourGitHubHandle.png" width="100px;" alt="INSERT NAME HERE"/><br />
        <sub><b>INSERT NAME HERE</b></sub></a><br />
            <a href="https://github.com/yourGitHubHandle" title="talk">📢</a>
    </td>
</tr></table>

## Contributing

This project welcomes contributions and suggestions.  Most contributions require you to agree to a
Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us
the rights to use your contribution. For details, visit [Contributor License Agreements](https://cla.opensource.microsoft.com).

When you submit a pull request, a CLA bot will automatically determine whether you need to provide
a CLA and decorate the PR appropriately (e.g., status check, comment). Simply follow the instructions
provided by the bot. You will only need to do this once across all repos using our CLA.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or
contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.

## Trademarks

This project may contain trademarks or logos for projects, products, or services. Authorized use of Microsoft
trademarks or logos is subject to and must follow
[Microsoft's Trademark & Brand Guidelines](https://www.microsoft.com/legal/intellectualproperty/trademarks/usage/general).
Use of Microsoft trademarks or logos in modified versions of this project must not cause confusion or imply Microsoft sponsorship.
Any use of third-party trademarks or logos are subject to those third-party's policies.
