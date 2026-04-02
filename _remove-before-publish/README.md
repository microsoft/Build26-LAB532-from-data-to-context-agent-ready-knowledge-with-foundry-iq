# 📁 Source Materials (Not Published)

Drop your **reference-only** source materials here — session abstracts, screenshots, internal notes, or any files you want Copilot to read but that **should not be seen by customers**.

**Files in this folder are automatically excluded from git.** They will not appear in your commits, pull requests, or the published repo. Only this README is tracked.

> 💡 **Files you DO want customers to see** — like lab instructions, demo code, sample data, or getting-started guides — should go directly into `/docs/`, `/src/`, or the repo root. Don't put publishable content here.

## What to put here

- Session abstract or catalog description (internal reference)
- Screenshots of your session overview page
- Notes extracted from your slide deck
- Internal outlines, agendas, or planning docs
- Any reference material you want Copilot to use as context but not publish

## What NOT to put here

- Source code, lab instructions, or demo content that **should** be published — put those in `/src/` or `/docs/`
- Large binary files (PowerPoint, Word, video) — extract text content into markdown instead

## How it works

Your Copilot agent scans this folder for context when you run any phase (`Get Started`, `Refine Content`, `Finalize`). It uses what it finds here to propose session titles, descriptions, learning outcomes, and more — but never commits these files to the repo.
