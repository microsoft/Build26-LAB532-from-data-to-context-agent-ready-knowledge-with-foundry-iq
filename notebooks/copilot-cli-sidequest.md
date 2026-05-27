# Copilot CLI sidequest: use your KB as an MCP server

Each notebook creates a separate Foundry IQ knowledge base. After the retrieve cell succeeds, run the sidequest checkpoint cell. It prints a ready-to-copy MCP configuration command.

> Do not paste service keys or bearer tokens into GitHub issues, pull requests, chat, or any committed file. Keep them in your local Copilot MCP configuration only.

## 1. Sign in to GitHub

Login at https://github.com/enterprises/skillable-events/sso

You will be signed into a special account created for the lab environment, not your actual GitHub account.

## 1. Sign in to GitHub Copilot CLI

Open the Terminal in VS Code (Terminal > New Terminal).

Sign in to GitHub Copilot CLI:

```powershell
copilot login
```

As part of the authorization step, you will be prompted to enter a 8-digit device code that is printed in the terminal.


## 2. Add the KB MCP server

Run the command printed by the notebook checkpoint cell. It will look like this:

```powershell
copilot mcp add zava-kb "<KB MCP URL>" --header "api-key=<SERVICE KEY>"
```

When it succeeds, you will see output like this:

```
Added server "zava-kb"

zava-kb
  Type: http
  URL: https://lab532-search-2ijs67lu3y3ty.search.windows.net/knowledgebases/multisource-search-knowledge-base/mcp?api-version=2026-05-01-preview
  Headers:
    api-key: ***
  Tools: * (all)
  Source: User
```

## 3. Ask a question grounded by the KB

Ask Copilot a question that matches the notebook you just ran. For example:

```powershell
copilot -i "Use the Zava knowledge base to answer: what health benefits are available?"
```

You can try asking the question without prefacing it with "Use Zava knowledge base to answer", but the Copilot CLI agent may not choose to invoke the KB MCP server, as it may answer from its weights or using a different tool.
