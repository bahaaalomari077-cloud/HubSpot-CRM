# crm-web — Angular CRM frontend

Scalable, **feature-based** Angular 21 app (standalone components, signals, zoneless)
wired to the CRM backend (`../CrmBackend`).

## Status

- ✅ `npm run build` succeeds — each page is its own lazy chunk
- ✅ Verified live end-to-end against the API + PostgreSQL: Contacts list, Dashboard
  KPIs ($25k pipeline computed from a real deal), CORS, routing all working in-browser

## Architecture

- **Standalone components** everywhere (no NgModules).
- **Feature-based structure** — each feature owns its pages + services.
- **Signals** for component state; **zoneless** change detection.
- **Lazy loading** — every route uses `loadComponent`, so features ship as separate chunks.
- A generic `ApiService<T>` base centralizes paging/filtering; each feature service extends it.

## Folder structure

```
src/app/
├─ core/                          # cross-cutting, app-wide singletons
│  ├─ models/
│  │  ├─ common.model.ts          #   PagedResult, QueryParams, enums
│  │  └─ entities.model.ts        #   Contact, Company, Application, Activity, Message
│  └─ services/
│     └─ api.service.ts           #   generic REST base (getPaged/getById/create/update/delete)
│
├─ layout/
│  ├─ shell.ts / .html / .scss    # sidebar nav + <router-outlet>
│
├─ features/
│  ├─ dashboard/
│  │  └─ dashboard.ts/.html/.scss # KPI tiles (forkJoin, graceful per-stream fallback)
│  ├─ contacts/
│  │  ├─ services/contact.service.ts
│  │  └─ pages/
│  │     ├─ contacts-list/        # paged + searchable table
│  │     └─ contact-details/      # profile + unified timeline
│  ├─ companies/
│  │  └─ services/company.service.ts
│  ├─ applications/
│  │  ├─ services/application.service.ts   # + moveStage() for kanban
│  │  └─ pages/applications-list/          # stage/amount filters, semantic pills
│  ├─ activities/
│  │  └─ services/activity.service.ts      # forContact() → timeline source
│  └─ messages/
│     └─ services/message.service.ts       # forContact() → timeline source
│
├─ app.ts                         # root: just <router-outlet>
├─ app.config.ts                  # providers: router, HttpClient(fetch), zoneless
└─ app.routes.ts                  # lazy routes under the Shell layout
```

## Component hierarchy

```
App (router-outlet)
└─ Shell (sidebar + content outlet)
   ├─ Dashboard                 /dashboard    — KPI tiles
   ├─ ContactsList              /contacts     — table, search, paging
   ├─ ContactDetails            /contacts/:id — profile + Timeline
   │     └─ (timeline merges ActivityService + MessageService streams)
   └─ ApplicationsList          /applications — filters + semantic pills
```

## Pages (all four required)

| Page | Route | Notes |
|---|---|---|
| **Dashboard** | `/dashboard` | contacts / companies / open apps / pipeline value / won |
| **Contacts List** | `/contacts` | server paging, search, delete |
| **Contact Details** | `/contacts/:id` | profile card + reverse-chron timeline (activities + messages) |
| **Applications List** | `/applications` | filter by stage & min-amount, Won/Lost/Process pills |

## Services (one per module, all extend `ApiService<T>`)

`ContactService` · `CompanyService` · `ApplicationService` (+`moveStage`) ·
`ActivityService` (+`forContact`) · `MessageService` (+`forContact`)

## Run

```bash
# 1. start the backend first (separate terminal)
cd ../CrmBackend && dotnet run --project src/Crm.Api        # → http://localhost:5211

# 2. start this app
npm start                                                    # → http://localhost:4200
```

The API base URL lives in `src/environments/environment.ts`. The backend already
allows `http://localhost:4200` via CORS.

## Notes

- Only `Contacts` and `Applications` controllers exist on the backend so far, so the
  Dashboard's Companies tile shows 0 and degrades gracefully (per-stream `catchError`).
  Add the backend `CompaniesController` and it lights up with no frontend change.
- The `ContactDetails` timeline already consumes Activities + Messages; those endpoints
  return empty until their controllers are added — the page handles that gracefully too.
```
