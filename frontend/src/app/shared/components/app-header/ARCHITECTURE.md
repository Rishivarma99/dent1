# App Header — Architecture & Decisions

Reference for how the shared clinic header (`app-header`) was designed and evolved.  
**Location:** `frontend/src/app/shared/components/app-header/`  
**Used by:** `ClinicLayoutComponent` (`clinic-layout.component.html`)

---

## 1. Role in the application

| Decision | Rationale |
|----------|-----------|
| **Single shared header** for the authenticated clinic shell | One place for search, notifications, theme, profile, and wayfinding — avoids duplicating chrome on every feature page. |
| **Replaced `clinic-top-header`** | Older layout-specific header was removed; layout only composes `app-header` + sidebar + footer + `router-outlet`. |
| **Header does not own the sidebar** | Sidebar state lives in `ClinicLayoutComponent` (`isSidebarOpen` signal). Header emits `(openSidebar)` on mobile; layout opens the drawer. Keeps header presentational and layout orchestration in one parent. |
| **Standalone Angular component** | `imports: [RouterLink, ButtonModule, MenuModule, …]` — no shared NgModule wrapper; matches the rest of the frontend. |
| **`ChangeDetectionStrategy.OnPush`** | Header updates from signals/computed (`currentUrl`, `pageContext`, `breadcrumbs`, `searchExpanded`). Fits router-driven UI without unnecessary checks. |

---

## 2. Visual & styling stack

| Layer | What we use | Why |
|-------|-------------|-----|
| **Layout & spacing** | Tailwind utility classes in template | Fast iteration; aligns with design tokens (`bg-surface`, `border-border-muted`, `text-text-strong`, etc.). |
| **Component-scoped CSS** | `app-header.css` | Only rules that are awkward in utilities (search panel width caps, `:host { height: 100% }`). |
| **Global PrimeNG overrides** | `frontend/src/styles/vendors/_primeng-overrides.scss` | PrimeNG v20 DOM/classes fight Tailwind; search input transparency and profile menu hover/logout colors live here under `.app-header-*` prefixes. |
| **PrimeNG for controls** | `p-button`, `p-menu`, `pInputText`, `pTooltip` | Consistent with the rest of the app; icons via PrimeIcons (`pi pi-*`). |

**Tailwind `!` prefix:** Used sparingly on `styleClass` (e.g. `!size-10`) when PrimeNG’s default button styles win specificity. Prefer SCSS overrides for repeated cases (search input).

---

## 3. Responsive navigation (core UX split)

Breakpoint: **Tailwind `sm` = 640px** — same breakpoint as sidebar “mobile drawer” behavior in `clinic-layout`.

### Below 640px — mobile chrome

No breadcrumbs. Left region shows **one control + one title**:

| Screen type | Control | Title | Behavior |
|-------------|---------|-------|----------|
| **Top-level** (Dashboard, Patients, Settings, …) | ☰ Hamburger | Module name | `(openSidebar)` → layout sets `isSidebarOpen(true)` |
| **Nested / detail** (e.g. `/patients/123`) | ← Back | Page / entity name | Navigate to **parent URL** (`backUrl`), else `Location.back()` |

**Top-level rule (code):** URL has **≤ 1 path segment** after the root → `navMode: 'hamburger'`.  
**Nested rule:** **> 1 segment** → `navMode: 'back'`, `backUrl` = parent path (drop last segment).

Examples:

```text
/patients          →  ☰  Patients
/patients/42       →  ←  Patient Profile   (back → /patients)
/patients/42/visits/7  →  ←  Visit Workspace   (back → /patients/42/visits)
```

### 640px and above — desktop chrome

| Left | Right |
|------|-------|
| **Breadcrumb trail** (home icon + Dashboard + … + current) | Search, notifications, theme toggle, profile |

- Hamburger/back **hidden** (`sm:hidden` on mobile block, `hidden sm:flex` on breadcrumb `<nav>`).
- Sidebar is always visible (icon-only or expanded); header does not duplicate nav toggles on tablet/desktop.

This matches the product rule: **mobile = minimal wayfinding; desktop = full trail.**

---

## 4. Title & breadcrumb data model

Two parallel computed values from the same URL signal:

```text
currentUrl  (from Router NavigationEnd + startWith)
    ├── pageContext()   → mobile title + navMode + backUrl
    └── breadcrumbs()   → desktop trail (label + path per crumb)
```

### Label maps (maintain when adding routes)

- **`MODULE_LABELS`** — known URL segments → display names (`patients` → `Patients`, `visits` → `Visit Workspace`).
- **`DETAIL_TITLE_BY_MODULE`** — numeric ID segments under a parent (`/patients/42` → `Patient Profile`).

Unknown segments: `formatSegmentLabel()` (kebab/snake → Title Case).

### Route override for dynamic titles

```typescript
// In route config, e.g. patient detail:
data: { headerTitle: 'Ravi Kumar' }
```

`readRouteHeaderTitle()` walks the `ActivatedRoute` tree and takes the deepest non-empty `headerTitle`. Used for:

- Mobile `<h1>` on nested routes
- **Last breadcrumb** label on desktop when set

Prefer this over hard-coding entity names in the header when detail pages load async data.

---

## 5. Breadcrumb construction (`buildBreadcrumbs`)

| Rule | Detail |
|------|--------|
| **Always starts with** | `{ label: 'Dashboard', path: '/dashboard' }` |
| **Skips** | `dashboard` segment in URL (avoid duplicate Dashboard) |
| **Known segments** | Push `MODULE_LABELS[segment]` with cumulative path |
| **Numeric segments** | Label = `headerTitle` (if last) else module/detail label else `#id` |
| **Last crumb** | Non-link `<span>`; earlier crumbs use `[routerLink]` |
| **Home icon** | Decorative `pi-home` before the trail (not a separate link) |

When adding new modules, add entries to **`MODULE_LABELS`** (and **`DETAIL_TITLE_BY_MODULE`** if list → detail uses `:id`).

---

## 6. Search (expand-left pattern)

| Decision | Rationale |
|----------|-----------|
| **Fixed `size-10` slot** on the right | Icons (bell, theme, avatar) do not shift when search opens. |
| **Panel anchored `absolute right-0`** | Expands **left** from the search icon (matches Figma / dent-figma-make reference). |
| **`searchExpanded` signal** | Opens on `mouseenter`; closes on outside `mousedown` (HostListener + `#searchRef`). |
| **Width in CSS, not only Tailwind** | `.app-header-search-panel--expanded` uses `min(20rem, calc(100vw - 11rem))` so narrow phones don’t overflow; slightly wider offset at `sm+`. |
| **Input styling in global SCSS** | `.app-header-search-input` — transparent background, no PrimeNG focus ring box. |

Search is UI-only for now (no service/API wired).

---

## 7. Actions on the right

| Feature | Breakpoint | Notes |
|---------|------------|-------|
| **Notifications** | All sizes | `p-button` + badge dot; placeholder (no API yet). |
| **Theme toggle** | `sm+` only (`hidden sm:block`) | Uses injectable **`ThemeService`** (`data-theme="dark"` on `<html>`, `localStorage`). Mobile users use **Settings** page appearance section instead of duplicating toggle in header. |
| **Profile** | All sizes | Avatar always visible; name/role text `hidden` until `sm`. |

---

## 8. Profile menu (PrimeNG pitfalls we fixed)

| Decision | Rationale |
|----------|-----------|
| **`p-menu` with `[popup]="true"` + `appendTo="body"`** | Avoids clipping inside header overflow; positions like a dropdown. |
| **Trigger = separate `p-button`** | Menu is not the trigger; button calls `profileMenu.toggle(event)`. |
| **User block via `pTemplate="start"`** | **Do not use `header` template** — in PrimeNG v20 that repeated the user block on every menu item. |
| **Menu items in `computed` `MenuItem[]`** | Settings navigates to `/settings` then `hide()`; Logout calls `AuthSessionService.logout()`. |
| **Logout styling** | `styleClass: 'app-header-profile-menu-logout'` on item; global SCSS targets `.p-menu-item-content` / `.p-menu-item-icon` (v20 structure, not legacy `.p-menu-item-link`). |
| **Close on resize** | `@HostListener('window:resize')` hides menu — avoids orphaned overlay after breakpoint change. |

Placeholder data today: `userEmail`, `clinicName`, `avatarUrl` are static; `userName` / `userTitle` come from `TokenStorageService` role formatting.

---

## 9. Integration diagram

```text
┌─────────────────────────────────────────────────────────────┐
│ ClinicLayoutComponent                                        │
│  isSidebarOpen ─────────────────────────► app-sidebar      │
│  openSidebar() ◄── (openSidebar) ─────── app-header        │
│  backdrop click ─────────────────────────► closeSidebar()    │
└─────────────────────────────────────────────────────────────┘
         │
         ▼
┌─────────────────┐     NavigationEnd      ┌──────────────────┐
│  app-header     │ ◄──────────────────────│  Angular Router  │
│  pageContext    │                        │  + route data    │
│  breadcrumbs    │                        │  headerTitle     │
└────────┬────────┘                        └──────────────────┘
         │
         ├── ThemeService (theme toggle)
         ├── AuthSessionService (logout)
         └── TokenStorageService (role display)
```

---

## 10. Files to touch when changing behavior

| Change | Files |
|--------|--------|
| New top-level module route | `MODULE_LABELS` in `app-header.ts`; sidebar nav link |
| New nested route pattern | `DETAIL_TITLE_BY_MODULE` and/or route `headerTitle` |
| Mobile vs desktop nav rules | `resolvePageContext`, template `sm:hidden` / `hidden sm:flex` |
| Search width / overflow | `app-header.css` |
| PrimeNG menu/input look | `_primeng-overrides.scss` |
| Theme persistence / attribute | `theme.service.ts` (+ Settings page for mobile) |
| Wire sidebar open | `clinic-layout` only — do not put drawer logic in header |

---

## 11. Intentional non-goals (for now)

- **No header service** — URL + route `data` are enough until multiple features need to push title without routing.
- **No duplicate Help in header** — Help lives in profile menu only.
- **Search not connected** to patient/record APIs.
- **Breadcrumbs not shown on mobile** — by design; do not re-add without revisiting the 640px spec.

---

## 12. Quick checklist for new detail pages

1. Add route under clinic feature (e.g. `patients/:id`).
2. Add `MODULE_LABELS` / `DETAIL_TITLE_BY_MODULE` if the URL pattern is generic.
3. Set `data: { headerTitle: entityName }` when the title is loaded from API.
4. Verify mobile: back goes to list parent; desktop: breadcrumb trail reads correctly.
5. If the screen is top-level on mobile, ensure URL depth is 1 segment so hamburger shows.

---

*Last updated from header work in clinic shell — shared component, responsive nav split, PrimeNG + token styling, and layout event wiring.*
