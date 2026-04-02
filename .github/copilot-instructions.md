This is a Microsoft Build 2026 session content repository.

If GUIDANCE.md exists in this repo, the repo has not yet been fully set up. When a content creator asks for help preparing the repo, read GUIDANCE.md and follow its instructions. The setup uses a **three-phase model**:

- **Get Started** — Session identity, learning outcomes, technologies, content owners
- **Refine Content** — Organize session content into /docs/ and /src/, fill in Getting Started sections (can be run multiple times as content evolves)
- **Finalize** — Final review, repo settings, slides/recordings links, delete GUIDANCE.md

When the creator asks for help, determine which phase they want to work on. You can detect this based on what's already filled in:
- If the README still has placeholder text (BRKXXX, "Add Session Description"), start with Get Started
- If the README has session info but content isn't organized yet, suggest Refine Content
- If content is organized and they want to finalize, suggest Finalize

Key constraints:
- Never commit secrets, API keys, or credentials. Use environment variables.
- Do not modify LICENSE, LICENSE-DOCS, CODE_OF_CONDUCT.md, or SECURITY.md.
- Do not add large binary files (PowerPoint, video, recordings) to the repo. Links are fine.
- The `_remove-before-publish/` folder is for source materials (abstracts, screenshots, notes). Its contents are gitignored — scan it for context but never try to commit files from it. Direct creators to put reference materials there, not in the repo root.
- Use the Microsoft Learn MCP Server (configured in .vscode/mcp.json) to find relevant learn.microsoft.com links when populating resource sections.

### Issue Support
If a user asks for help filing an issue, or reports a problem:
- Check `.github/ISSUE_TEMPLATE/` to discover available issue templates
- If templates exist, match the user's request to the best-fit template and walk them through the fields
- If no templates exist, create a plain issue with a clear title and description
- Check `gh label list` for available labels and apply relevant ones
- Do not hardcode template names or labels — always discover what's available at runtime
