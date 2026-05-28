# AI Agent Guidelines

This file contains instructions and guidelines for AI agents working on this repository.

## 🔒 Security Best Practices

**Never commit sensitive information to this repository:**
- API keys, tokens, or credentials
- Personal access tokens (PATs)
- Database connection strings with passwords
- Environment-specific configuration values

**For MCP configuration files (`mcp.json`):**
- Use placeholder values like `"YOUR_API_KEY_HERE"` or `"${API_KEY}"`
- Reference environment variables for sensitive data
- Include documentation about required environment variables

## 📋 Repository Guidelines

### Purpose
This repository is a Microsoft Build 2026 session content repository and should:
- Provide clear, actionable content for session attendees
- Support self-guided learning for remote/at-home learners
- Follow the structure established by GUIDANCE.md

### What NOT to modify without permission:
- License files (`LICENSE`, `LICENSE-DOCS`, `CODE_OF_CONDUCT.md`)
- Security files (`SECURITY.md`)
- GitHub workflow files in `.github/` directory

### Content Rules
- No large binary files (PowerPoint decks, videos, recordings) in the repo
- Links to slides and recordings are fine — just don't host the actual files
- All README files should be kept up to date
- Unused folders (containing only a placeholder README) should be removed before release
- Always clear notebook outputs and reset execution counts before committing `.ipynb` files

### Issue Management
When a user reports a problem, asks a question that should be tracked, or wants to file an issue:

1. **Discover available templates** — Check `.github/ISSUE_TEMPLATE/` for any `.yml` or `.md` template files. Read them to understand what fields and labels each template expects.
2. **Match the request to a template** — Based on what the user is describing, pick the best-fit template. If no templates exist, create a plain issue.
3. **Help the user fill in the fields** — Walk through the template's required fields interactively, proposing answers where possible.
4. **Create the issue** — Use `gh issue create --template <template-file>` if a template matches, or `gh issue create` for a plain issue.
5. **Apply labels** — Check `gh label list` to see what labels exist in the repo. Apply relevant labels based on the issue type. Don't try to apply labels that don't exist.

When reviewing open issues at the start of each phase, summarize them and propose actions — this behavior already exists in the Issue Tracking and Commits section of GUIDANCE.md.

### Getting Started
If this repo still has a `GUIDANCE.md` file, that means setup isn't complete yet. Read it and follow the instructions to prepare the repo for publication.

## 📓 Notebook Style Guidelines

These rules apply to all `.ipynb` files in `notebooks/`.

### Writing style
- Do not use semi-colons in prose descriptions. Reword the sentence instead.
- Do not use em dashes (`—`) in prose descriptions. Use a period to start a new sentence, or restructure.
- Use plain dashes (`-`) only in lists, not as clause separators in running text.

### Structure (per notebook)

- **First cell**: Combined title + scenario lore + `**🎯 Mission**` bullets.
- **Lore/scenario**: Written as a Part X narrative ("Part 1 at Zava…") that motivates why the attendee needs the technical feature being taught.
- **Mission bullets**: 1-3 items matching what the attendee will build. The `## ✅ Mission complete` cell at the end should cover the same outcomes, but may expand on them with fuller explanations.
- **Building blocks**: Optional. If included, explain only concepts that are NEW in this notebook and avoid re-explaining concepts already introduced in earlier parts.
- **Last cell** (before the Continue link): `## ✅ Mission complete` with `**What you built:**` and `✓` bullets. The capstone notebook (Part 5) uses `**What you built across the lab:**` instead.
- **Source Hunt**: Include a `#### 🔍 Source Hunt` markdown cell after the references print cell and activity log cells in each notebook.
