# devlog-toolkit (v0.1.0)

Local Cursor plugin that provides a small, read-only code review agent for this repo.

## Included

- `review-code` agent: checks frontend naming/folder structure + Angular standalone conventions, and basic backend layer placement (when present).
- `review-code` + `review-angular-structure` skills: step-by-step review checklist prompts.
- `review-code` + `review-angular-structure` commands: quick entry points in Cursor command palette.
- `sessionStart` hook: shows a short reminder of the commands.

## Usage

- Run the agent via: `devlog-toolkit:review-code`
- Or run the skills via: `devlog-toolkit:review-code` and `devlog-toolkit:review-angular-structure`

## Safety / Permissions

This plugin is intended to be read-only: it should only read files and search for patterns (glob/grep).

