# Copilot CLI sidequest: use your KB as an MCP server

Each notebook creates a separate Azure AI Search knowledge base. After the retrieve cell succeeds, run the sidequest checkpoint cell. It prints:

1. The KB MCP URL.
2. The `api-key` auth header to use for the MCP server.
3. A ready-to-copy MCP config snippet.

> Do not paste service keys or bearer tokens into GitHub issues, pull requests, chat, or any committed file. Keep them in your local Copilot MCP configuration only.

## 1. Sign in to GitHub Copilot CLI

Install and sign in to GitHub Copilot CLI if you have not already:

```powershell
copilot auth login
```

You can confirm the CLI is available with:

```powershell
copilot --help
```

## 2. Add the KB MCP server

Use the MCP config snippet printed by the notebook checkpoint cell. You can add it through the CLI or paste it into your local MCP config file.

Using the CLI:

```powershell
copilot mcp add --name lab532-kb --url "<KB MCP URL>" --header "api-key=<SERVICE KEY>"
```

Or edit your local config file:

- Windows: `C:\Users\<USERNAME>\.copilot\mcp-config.json`
- macOS/Linux: `~/.copilot/mcp-config.json`

Add the printed entry under `mcpServers`. If the file already has other servers, merge the new server entry rather than replacing the whole file.

## 3. Ask a question grounded by the KB

Start Copilot CLI and ask a question that matches the notebook you just ran:

```powershell
copilot -i "Use the LAB532 knowledge base to answer: what health benefits are available?"
```

For Fabric IQ, Work IQ, MAI Grounding, or the combined notebook, ask a question that explicitly mentions that source so Copilot has a reason to call the matching KB MCP server.

## 4. Clean up local MCP config when finished

Remove the temporary server entry from your local MCP config, or run the corresponding Copilot CLI remove command if your installed CLI version supports it.
