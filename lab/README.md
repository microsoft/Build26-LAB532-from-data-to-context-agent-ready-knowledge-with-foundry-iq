
# Lab 006: Build Agentic Knowledge Bases with Azure AI Search

These instructions are for participants of the **instructor-led** Workshop "Build Agentic Knowledge Bases: Next-Level RAG with Azure AI Search" at MVP Summit 2026.

## Lab Overview

In this hands-on lab, you'll build an Azure AI Search Knowledge Base and extend it across a refreshed five-part flow: document sources, Fabric IQ, Work IQ, web grounding, and arbitrary MCP servers. By the end, you'll have a flexible KB that blends indexed, structured, web, and tool-based knowledge sources.

## Pre-Requisites

## Prerequisites

To get the most out of this lab, you should have a basic understanding of the following:

- **Python and Jupyter Notebooks** – You will write and run code cells directly inside a Jupyter environment.  
- **Azure Fundamentals** – Familiarity with Azure services and concepts such as resource groups, storage accounts, and authentication.  
- **Retrieval-Augmented Generation (RAG)** – A general understanding of how LLMs use external data for grounding will help you better follow the agentic retrieval flow.  
- **Azure AI Search and OpenAI** – Basic knowledge of what these services do (indexing, querying, embeddings, completions) is helpful but not required.

> [!NOTE]  
> You do **not** need to provision any Azure services or deploy infrastructure manually for this lab. All required resources including Azure AI Search, OpenAI deployments, and data sources — are pre-created and ready to use.

## Get Started

To begin, open the **notebooks/** folder and start with **part1-standard-foundry-iq-kb.ipynb**. Work through all 5 notebooks sequentially:

1. **part1-standard-foundry-iq-kb.ipynb** — Standard Foundry IQ KB with document sources
2. **part2-fabric-iq-to-kb.ipynb** — Add Fabric IQ to the KB
3. **part3-work-iq-to-kb.ipynb** — Add Work IQ to the KB
4. **part4-web-source-to-kb.ipynb** — Add a web source to the KB
5. **part5-arbitrary-mcp-servers.ipynb** — Add arbitrary MCP servers such as Learn and GitHub

> [!NOTE]
> The original MVP Summit notebook set has been preserved under `notebooks/mvp-summit-notebooks/`.

Once you've completed all 5 notebooks, return to this page and select **Next >** to view the wrap-up and summary section.

## Discussions

If you’d like to contribute, raise an issue, or provide feedback, please open an issue in this repo.

If you enjoyed this workshop, consider giving the repository a ⭐ on GitHub and sharing it with your peers or community.

## Source Code

The source code for this session is available in the [notebooks folder](../notebooks) of this repository.  
You can use it as a reference for future projects, extend it with additional capabilities, or integrate it into your own solutions built on Azure AI Search and agentic retrieval.