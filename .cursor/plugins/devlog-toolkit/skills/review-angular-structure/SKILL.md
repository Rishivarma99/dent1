---
name: review-angular-structure
description: Angular-only read-only structural/naming review for `frontend/src/app/`.
tools: ['read', 'glob', 'grep']
---

This skill is a narrower Angular-only version of `devlog-toolkit:review-code`.

1. Verify folder structure:
   - `frontend/src/app/core/`, `shared/`, `features/` exist.
   - `features/` contains `auth/` plus exactly one business folder (e.g. `clinic/`).

2. Verify standalone+OnPush:
   - `glob` for `frontend/src/app/**/*.component.ts`
   - `grep` for `@Component(` and ensure `standalone: true` and `changeDetection: ChangeDetectionStrategy.OnPush` appear in the metadata.
   - `grep` for `@NgModule(` and flag if found.

3. Quick naming heuristics:
   - Flag file names with underscores/spaces or wrong suffixes.

Output only:
- Front-end section with Naming/Folder structure/Standalone+OnPush/Forbidden patterns
- Notes

