
# Lab 532: Build Agentic Knowledge Bases with Azure AI Search

These instructions are for participants of the **instructor-led** Workshop "Build Agentic Knowledge Bases: Next-Level RAG with Azure AI Search" at Microsoft Build 2026.

## Lab Overview

In this hands-on lab, you'll build Azure AI Search Knowledge Bases across a refreshed five-part flow: restored search indexes, MAI Grounding through MCP, Fabric IQ, Work IQ, and a combined Work IQ + Fabric IQ experience. By the end, you'll have flexible KBs that blend indexed, structured, workplace, and web-grounded knowledge sources.

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

1. **part1-standard-foundry-iq-kb.ipynb** — Build a multi-source KB with the restored HR and health search indexes
2. **part2-mai-grounding-mcp-kb.ipynb** — Add MAI Grounding through an MCP knowledge source
3. **part3-fabric-iq-to-kb.ipynb** — Add Fabric IQ through a Fabric Ontology knowledge source
4. **part4-work-iq-to-kb.ipynb** — Add Work IQ as a first-party knowledge source
5. **part5-work-iq-fabric-iq-to-kb.ipynb** — Combine Work IQ and Fabric IQ in one KB

Once you've completed all 5 notebooks, return to this page and select **Next >** to view the wrap-up and summary section.

## Discussions

If you’d like to contribute, raise an issue, or provide feedback, please open an issue in this repo.

If you enjoyed this workshop, consider giving the repository a ⭐ on GitHub and sharing it with your peers or community.

## Source Code

The source code for this session is available in the [notebooks folder](../notebooks) of this repository.  
You can use it as a reference for future projects, extend it with additional capabilities, or integrate it into your own solutions built on Azure AI Search and agentic retrieval.
