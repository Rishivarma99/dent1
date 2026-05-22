# Clinical Workspace & Role-Based Landing — Design Spec

**Status:** Approved (user decision, 2026-05-22)  
**Scope:** dent1 clinic shell — frontend-first MVP with mock data; API scoping follows in a later phase.

---

## Core principle

```text
Dashboard     = operational overview (metrics + quick navigation)
Workspace     = clinical work area (queue, visits, follow-ups)
```

These are **not** the same screen. Mixing them confuses doctor UX.

---

## Personas & landing routes

| Role | Post-login landing | Sidebar emphasis |
|------|-------------------|------------------|
| Doctor | `/workspace` | **My Workspace**, Patients, Appointments, Settings |
| Assistant | `/workspace` | **My Workspace** (same label), Patients, Appointments, Settings |
| Receptionist | `/dashboard` | Dashboard, Patients, Appointments, Settings |
| Admin | `/dashboard` | Dashboard, Patients, Appointments, Settings |

**Doctors** is removed from sidebar for all roles. Doctor master-data CRUD is **not** a top-level clinic route in this phase — it moves under **Settings** when that module is built (e.g. Settings → Staff / Doctors). Existing `features/clinic/doctors/` code can be reused there; do not expose `/doctors` in the clinic shell for MVP.

---

## 1. Dashboard (`/dashboard`) — overview only

**Audience:** Admin, Receptionist (operations-oriented).

**Purpose:** Clinic flow monitoring — not where clinical work happens.

**Keep (MVP):**

- Stat cards: Total Patients, Today's Appointments, In Queue, Pending Follow-ups (clinic-wide)
- Today's Queue (all doctors / clinic view)
- Upcoming Appointments
- Pending Follow-ups
- Recent Activity
- Create Appointment (preview drawer)

**Remove from clinical roles:**

- Doctors must not land here by default
- Assistants must not land here by default

**Doctor “dashboard” (light):** Optional later route `/dashboard` for doctors showing only 4 personal stats + link to Workspace — **not in MVP** unless time permits. MVP: doctors skip dashboard entirely.

---

## 2. Workspace (`/workspace`) — primary clinical module

**Audience:** Doctor, Assistant (same shell, permission-gated actions).

**Module label in nav:** `My Workspace` (doctor/assistant). Not “Doctor Workspace” or “Assistant Workspace”.

**Purpose:** Answer: *Who is my next patient?* — task-oriented home.

### Layout (mobile-first)

```text
[ Page header: My Workspace + subtitle ]

[ 4 stat cards — 1 col → 2 col (sm) → 4 col (lg) ]
  - Appointments Today (mine)
  - In Queue (mine)
  - Completed Today (mine)
  - Pending Follow-ups (mine)

[ Main — full width on mobile, 2/3 on lg ]
  Today's Queue
    - status badges
    - Start Visit (permission: visit.start or equivalent)

[ Secondary — sidebar on lg ]
  Pending Follow-ups
  (In Progress Visits — MVP section if queue data supports it)
  (Recent Patients — defer post-MVP unless trivial)
```

### Sections (MVP vs later)

| Section | MVP | Notes |
|---------|-----|-------|
| Today's Queue | Yes | Filtered to current user / assigned doctor |
| Start Visit | Yes | Navigates to visit flow (stub → `/patients` until visit route exists) |
| Pending Follow-ups | Yes | Doctor-scoped list |
| In Progress Visits | Stretch | Subset of queue with `in-progress` status |
| Recent Patients | Defer | Nice-to-have |
| Upcoming Appointments block | Defer | Dashboard/receptionist concern |

### Permission model (UX, not duplicate pages)

One workspace UI. Actions shown/hidden by permissions (seeded today; extend later):

| Action | Doctor (typical) | Assistant (typical) |
|--------|------------------|---------------------|
| Start Visit | Yes | View queue; start only if permitted |
| Complete Visit | Yes | No (MVP) |
| Add notes / upload | Yes | Yes (when permissions added) |
| Draft prescription | Yes | Configurable |
| Finalize prescription | Yes | No (MVP) |

**Do not build** separate assistant routes or duplicate workspace pages.

---

## 3. Shared shell

- Same `ClinicLayoutComponent`, sidebar, header, footer, design tokens
- Only **nav items**, **default redirect**, and **page content** differ by role
- Figma reference: dentova-figma-make dashboard components for visual parity; workspace reuses card/queue patterns already in `dashboard-page`

---

## 4. Routing

```text
/clinic (layout)
  /dashboard     → ops overview (guard: Admin, Receptionist)
  /workspace     → clinical home (guard: Doctor, Assistant)
  /patients      → existing
  /appointments  → stub or existing (add route when ready)
  /settings      → existing shell; doctor CRUD nested here later
```

**Doctors CRUD:** `/doctors` redirects to `/settings/staff` (admin-only). Not in sidebar.

Default child redirect (`''`) must be **role-aware** (not always `dashboard`).

---

## 5. Out of scope (MVP)

- Separate doctor app or layout fork
- Assistant-specific workspace module
- **Settings → Doctors / Staff CRUD** (reuse existing doctors feature; build with settings module)
- Standalone `/doctors` route in clinic layout
- Real-time WebSocket queue
- Multi-branch switcher
- Configurable permission UI (use seeded role → permission map in code first)
- Backend workspace aggregate API (mock data in frontend phase 1)

---

## 6. Success criteria

- Doctor logs in → lands on `/workspace`, never sees clinic-wide “Total Patients 1,248” as home
- Receptionist logs in → lands on `/dashboard`
- Sidebar never shows **Doctors**
- Doctor and assistant share workspace; assistant does not see “Complete Visit” without permission
- Mobile: queue usable without horizontal scroll; primary CTA visible
