# Clinical Workspace Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Split operational **Dashboard** from clinical **Workspace**, add role-based landing and sidebar, remove Doctors from nav, and share one workspace for doctors and assistants with permission-gated actions.

**Architecture:** Reuse `ClinicLayoutComponent` and existing dashboard UI patterns. Introduce `features/clinic/workspace/` for the clinical home. Centralize role → nav + landing in a small `ClinicNavConfigService`. Gate routes with functional guards reading `TokenStorageService.getRole()`. MVP uses mock data; API filtering by `DoctorId` is a follow-up plan.

**Tech Stack:** Angular 20, signals, lazy routes, PrimeNG, Tailwind + design tokens, existing JWT role string (`Doctor`, `Assistant`, `Receptionist`, `Admin`).

**Design spec:** `docs/superpowers/specs/2026-05-22-clinical-workspace-design.md`

**Branch:** `feature/clinical-workspace`  
**Worktree (isolated implementation):** `.worktrees/clinical-workspace/` (full path below)

---

## Phase 0 — Git worktree (DONE)

### Task 0: Isolated worktree (`using-git-worktrees`)

- [x] **Step 1:** Added `.worktrees/` and `worktrees/` to root `.gitignore` (commit on `dev-rishi` when ready).
- [x] **Step 2:** `git worktree add .worktrees/clinical-workspace -b feature/clinical-workspace`
- [x] **Step 3:** `npm install` + `npm run build` in worktree `frontend/` — exit 0 (existing budget warnings only).
- [x] **Step 4:** Open worktree in Cursor for implementation (or agent edits paths under `.worktrees/clinical-workspace/`).
- [x] **Step 5:** Run serve from worktree on another port (`ng serve --port 4201`) when testing feature branch.

**Paths:**

| Checkout | Branch | Use for |
|----------|--------|---------|
| `dent1/` (main) | `dev-rishi` | Your current `ng serve`, day-to-day |
| `dent1/.worktrees/clinical-workspace/` | `feature/clinical-workspace` | Clinical workspace implementation |

---

## File map

| File | Responsibility |
|------|----------------|
| `core/constants/clinic-roles.ts` | Role string constants + helpers (`isClinicalRole`, `isOpsRole`) |
| `core/services/clinic-nav-config.service.ts` | Sidebar items + home path per role |
| `core/guards/clinical-role.guard.ts` | Allow Doctor, Assistant |
| `core/guards/ops-role.guard.ts` | Allow Admin, Receptionist |
| `features/clinic/workspace/` | Workspace page + presentational widgets |
| `features/clinic/dashboard/` | Trim/guard ops dashboard |
| `features/clinic/layout/components/clinic-sidebar-nav/*` | Consume nav config |
| `features/clinic/clinic.routes.ts` | New routes + role default redirect |
| `features/auth/pages/login-page/login-page.ts` | Role-based post-login navigate |
| `shared/components/app-header/app-header.ts` | Module labels for workspace |

---

## Phase 1 — Role & navigation foundation

### Task 1: Role helpers and nav config

**Files:**
- Create: `frontend/src/app/core/constants/clinic-roles.ts`
- Create: `frontend/src/app/core/services/clinic-nav-config.service.ts`

- [ ] **Step 1:** Add constants matching API `UserRole` strings: `Doctor`, `Assistant`, `Receptionist`, `Admin`, `Patient`.
- [ ] **Step 2:** Add `isClinicalRole(role)`, `isOpsRole(role)`, `getDefaultLandingPath(role)`:
  - Clinical → `/workspace`
  - Ops → `/dashboard`
  - Unknown → `/dashboard` (safe fallback)
- [ ] **Step 3:** Add `getSidebarItems(role)` returning `{ label, icon, routerLink, exact? }[]`:

**Doctor / Assistant:**
```text
My Workspace  → /workspace
Patients      → /patients
Appointments  → /appointments  (stub route OK)
Settings      → /settings
```

**Receptionist / Admin:**
```text
Dashboard     → /dashboard
Patients      → /patients
Appointments  → /appointments
Settings      → /settings
```

- [ ] **Step 4:** Add workspace icon to `clinic-sidebar-icons.ts` + `clinic-sidebar-nav-icon.ts` (briefcase or calendar-clock SVG).

---

### Task 2: Wire sidebar to nav config

**Files:**
- Modify: `clinic-sidebar-nav.ts`, `clinic-sidebar-nav.html`
- Modify: `clinic-sidebar-nav.ts` — inject `ClinicNavConfigService` + `TokenStorageService`

- [ ] **Step 1:** Replace hardcoded `menuItems` with computed signal from role.
- [ ] **Step 2:** Brand link `routerLink` → role default landing (not always `/dashboard`).
- [ ] **Step 3:** Verify tablet tooltips still work with dynamic item count.

---

### Task 3: Route guards and clinic routes

**Files:**
- Create: `frontend/src/app/core/guards/clinical-role.guard.ts`
- Create: `frontend/src/app/core/guards/ops-role.guard.ts`
- Modify: `frontend/src/app/features/clinic/clinic.routes.ts` — **remove** lazy `doctors` child route (keep `features/clinic/doctors/` code for Settings reuse later)

- [ ] **Step 1:** Clinical guard allows `Doctor`, `Assistant`; else redirect to `/dashboard`.
- [ ] **Step 2:** Ops guard allows `Admin`, `Receptionist`; else redirect to `/workspace`.
- [ ] **Step 3:** Add lazy `workspace` route with clinical guard.
- [ ] **Step 4:** Add `appointments` child route stub (empty page “Coming soon”) so nav link is valid.
- [ ] **Step 5:** Remove `/doctors` from `clinic.routes.ts` (not sidebar, not standalone — Settings module will host CRUD later).
- [ ] **Step 6:** Replace `redirectTo: 'dashboard'` with a small redirect component or guard that reads role and navigates to default landing.

---

### Task 4: Login and default redirects

**Files:**
- Modify: `login-page.ts` (3 navigate calls currently go to `/patients`)
- Modify: `app.routes.ts` if global post-auth redirect exists

- [ ] **Step 1:** After successful login, `navigate` to `getDefaultLandingPath(role)`.
- [ ] **Step 2:** Smoke-test seed users: `arjun.rao` (Doctor), `nikhil.reception` (Receptionist).

---

## Phase 2 — Workspace module (clinical home)

### Task 5: Scaffold workspace feature

**Files:**
- Create: `features/clinic/workspace/routes.ts`
- Create: `features/clinic/workspace/pages/workspace-page/workspace-page.ts|html|css`
- Create: `features/clinic/workspace/models/workspace.models.ts`

- [ ] **Step 1:** Define interfaces: `WorkspaceStats`, `QueueItem`, `FollowUpItem` (reuse shapes from dashboard-page).
- [ ] **Step 2:** Page header: “My Workspace”, subtitle “Your patients and visits for today”.
- [ ] **Step 3:** Mobile-first layout matching design spec (stats grid + 1/2 column split at `lg:`).

---

### Task 6: Extract reusable widgets from dashboard

**Files:**
- Create: `features/clinic/workspace/components/workspace-stats/workspace-stats.ts`
- Create: `features/clinic/workspace/components/today-queue/today-queue.ts`
- Create: `features/clinic/workspace/components/pending-follow-ups/pending-follow-ups.ts`
- Optional: `features/clinic/workspace/components/in-progress-visits/in-progress-visits.ts`

- [ ] **Step 1:** Copy/adapt stat card markup from `dashboard-page.html` — 4 doctor-scoped labels only.
- [ ] **Step 2:** Extract Today's Queue list + status helpers from `dashboard-page.ts` into `today-queue` component with `@Input() items` and `@Output() startVisit`.
- [ ] **Step 3:** Extract follow-ups card into `pending-follow-ups` component.
- [ ] **Step 4:** Pass mock data from `workspace-page` (filter out other doctors in mock arrays).

---

### Task 7: Permission-gated actions (MVP)

**Files:**
- Create: `frontend/src/app/core/services/clinic-permissions.service.ts`
- Modify: `today-queue` template

- [ ] **Step 1:** Map role → capability flags: `canStartVisit`, `canCompleteVisit`, `canDraftPrescription`, etc. (hardcoded from seed permissions for MVP).
- [ ] **Step 2:** Show **Start Visit** only when `canStartVisit`; assistant default `false` for complete/finalize.
- [ ] **Step 3:** Document in code comment that this moves to API permission codes later (`appointment.update`, etc.).

---

### Task 8: Start Visit navigation stub

- [ ] **Step 1:** `startVisit(appointmentId)` → navigate to patient/visit stub (same as dashboard today or `/patients/:id` when ready).
- [ ] **Step 2:** Ensure header back/breadcrumb can show “Workspace” via route data.

---

## Phase 3 — Dashboard cleanup (ops only)

### Task 9: Protect and clarify ops dashboard

**Files:**
- Modify: `dashboard-page.html`, `dashboard-page.ts`
- Modify: `dashboard/routes.ts`

- [ ] **Step 1:** Apply `opsRoleGuard` on dashboard route.
- [ ] **Step 2:** Remove “Manage Doctors” button from dashboard (doctor CRUD deferred to Settings module).
- [ ] **Step 3:** Keep clinic-wide metrics and full queue for receptionist/admin.
- [ ] **Step 4:** Optional: add prominent link/card “Open clinical workspace” hidden for ops roles (skip if YAGNI).

---

## Phase 4 — Header, polish, verification

### Task 10: App header route labels

**Files:**
- Modify: `app-header.ts` — `MODULE_LABELS`

- [ ] **Step 1:** Add `workspace: 'My Workspace'`.
- [ ] **Step 2:** Confirm breadcrumb/title for `/workspace`.

---

### Task 11: Verification (superpowers:verification-before-completion)

- [x] **Step 1:** `npm run build` in `frontend/` — exit 0 (worktree).
- [x] **Step 1b:** `dotnet build` in `api/` — exit 0 (worktree).
- [ ] **Step 2:** Manual matrix:

| User | Landing | Sidebar |
|------|---------|---------|
| Doctor | `/workspace` | My Workspace, Patients, Appointments, Settings — no Dashboard |
| Assistant | `/workspace` | Same |
| Receptionist | `/dashboard` | Dashboard, Patients, Appointments, Settings — no Workspace |
| Admin | `/dashboard` | Same as receptionist |

- [ ] **Step 3:** Confirm `/doctors` is not registered (404 or app redirect) until Settings ships.
- [ ] **Step 4:** Resize mobile / tablet / desktop — workspace queue readable, CTA tappable.

---

## Phase 5 — Settings module (DONE)

- [x] Settings shell with sections (profile, clinic, **staff/doctors**, etc.)
- [x] Embed or lazy-load existing `doctors-page` under Settings (admin-only tab)
- [x] Redirect legacy `/doctors` → `/settings/staff` for bookmarks
- [x] `GET/POST/PUT/DELETE /api/doctors` — staff list backed by `User` rows with `Role=Doctor`

---

## Phase 6 — Workspace API + permissions (DONE on `feature/clinical-workspace`)

- [x] `GET /api/workspace/today` — queue, stats, follow-ups (seed data scoped by `UserId` until appointments exist)
- [x] JWT `doctor_id` claim for Doctor/Assistant; login response includes `permissions[]`
- [x] Frontend `WorkspaceApiService` calls API; `ClinicPermissionsService` uses stored permissions
- [ ] Filter appointments by `DoctorId` + branch (requires appointment schema + migration — separate)

---

## Next step — merge & verify (Task 11)

1. Run API from worktree: `dotnet run --project api/Dent1.Api`
2. Manual matrix (doctor/assistant/receptionist/admin) — see Task 11 table below (**you**)
3. **Re-login** after pull so `permissions[]` is stored (old sessions lack it)
4. [ ] Merge `feature/clinical-workspace` → `dev-rishi` (commit on feature branch, then merge or PR)
5. [ ] Remove worktree when done: `git worktree remove .worktrees/clinical-workspace`

---

## Suggested implementation order

```text
1 → 2 → 3 → 4   (nav + auth — unblocks correct shell)
5 → 6 → 7 → 8   (workspace MVP)
9               (dashboard guard)
10 → 11         (polish + verify)
```

**Estimated effort:** 1–2 sessions for Phases 1–4 with mock data; API phase separate.

---

## Risks & mitigations

| Risk | Mitigation |
|------|------------|
| `/appointments` missing | Stub route in Task 3 |
| Role string mismatch | Align with `UserRole` enum names from API |
| Dashboard duplication | Extract widgets in Task 6; don’t copy-paste twice long-term |
| Assistant over-blocked | Permission service easy to extend per clinic later |
| Doctor CRUD orphaned | Keep `features/clinic/doctors/`; wire only via Settings follow-up |
