---
name: review-code
description: Step-by-step prompt for running the `devlog-toolkit:review-code` agent in read-only mode.
tools: ['read', 'glob', 'grep']
---

Run the repo review with these steps:

1. Identify roots
   - Frontend: find `frontend/src/app/` (if it exists).
   - Backend: check for `api/Dent1.Api/` and `api/Dent1.Data/` (if it exists).

2. Frontend checks (simple)
   - Use `glob` to list `frontend/src/app/**` `*.component.ts` files and `*.ts` files containing `@Component(`.
   - Use `grep` to check for:
     - `@NgModule(`
     - `*ngIf` / `*ngFor`
     - `@ViewChild(` / `@ContentChild(` / `@HostBinding(` / `@HostListener(`
   - For each matched component file, `read` just the relevant `@Component({ ... })` block and verify:
     - `standalone: true`
     - `changeDetection: ChangeDetectionStrategy.OnPush`

3. Folder structure check
   - Use `glob` to check `frontend/src/app/core/`, `shared/`, and `features/`.
   - Under `features/`, ensure `auth/` exists and there is a single business area folder (e.g., `clinic/`).

4. Naming heuristics
   - Use `glob` to list component files and flag non-kebab-case names (underscores, spaces, uppercase in the file name part).
   - Flag obvious suffix mismatches (`page` components not using `-page.component.ts` when the route-level naming pattern is used in the repo).

5. Backend checks (only when folders exist)
   - Contracts: use `glob` to verify `Dent1.Api/Contracts/Requests/` and `Dent1.Api/Contracts/Responses/` are present.
   - Controllers: use `glob` for `Dent1.Api/Controllers/`.
   - Layering heuristic: `grep` in controllers for `DbContext` / `Repository` / `Handler` / `Command` / `Query`.
   - In Data project, `grep` for `ControllerBase` / `IActionResult` / `Http` as a violation signal.

6. Report in the exact format required by the agent:
- Front-end section
- Back-end section
- Notes

Never propose edits or new files. Only output findings.

