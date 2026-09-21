# CRM Backend — ASP.NET Core + EF Core + PostgreSQL

Production-ready CRM backend built with **Clean Architecture**. The schema and
pipeline behavior are modeled on a live Bitrix24 tenant that was reverse-engineered
via its own API (see `../BITRIX24-FULL-DOCS.md`).

## Status

- ✅ Builds clean (`dotnet build`, 0 warnings / 0 errors)
- ✅ Migration applied to the **`HubS-pot-CRM`** database (PostgreSQL 17, port 5433)
- ✅ All 6 tables + 11 foreign keys created and verified in the DB
- ✅ CRUD, paging, filtering, sorting and kanban stage-move tested end-to-end

## Solution structure

```
CrmBackend/
├─ Crm.sln
└─ src/
   ├─ Crm.Domain/                 # Entities, enums, no dependencies
   │  ├─ Common/BaseEntity.cs     #   audit + soft-delete base
   │  ├─ Entities/                #   Contact, Company, Application,
   │  │                           #   Activity, Message, User
   │  └─ Enums/Enums.cs
   │
   ├─ Crm.Application/            # DTOs, services, interfaces (depends on Domain)
   │  ├─ Common/Interfaces/       #   ICrmDbContext
   │  ├─ Common/Models/           #   PagedResult, QueryParameters
   │  ├─ Contacts/                #   IContactService, ContactService, DTOs
   │  ├─ Applications/            #   IApplicationService, ApplicationService, DTOs
   │  └─ DependencyInjection.cs
   │
   ├─ Crm.Infrastructure/        # EF Core (depends on Application)
   │  ├─ Persistence/
   │  │  ├─ CrmDbContext.cs
   │  │  ├─ CrmDbContextFactory.cs         # design-time factory for dotnet ef
   │  │  ├─ Configurations/                # Fluent API, one file per entity
   │  │  └─ Migrations/                    # InitialCreate
   │  └─ DependencyInjection.cs
   │
   └─ Crm.Api/                    # Controllers (depends on Infrastructure)
      ├─ Controllers/             #   ContactsController, ApplicationsController
      ├─ Program.cs              #   DI wiring, Swagger, CORS, auto-migrate
      └─ appsettings.json
```

Dependency direction: **Api → Infrastructure → Application → Domain**. The Domain
references nothing; the Application talks to the database only through
`ICrmDbContext`, never the concrete `CrmDbContext`.

## Entities & relationships

| Entity | Key relationships |
|---|---|
| **User** | owns Contacts, Companies, Applications, Activities |
| **Company** | has many Contacts, Applications · assigned to User |
| **Contact** | belongs to Company · has Applications, Activities, Messages |
| **Application** | belongs to Contact + Company · has Activities · pipeline stage + semantic |
| **Activity** | belongs to Contact + Application · scheduled, has deadline |
| **Message** | belongs to Contact · immutable communication log |

Design decisions carried over from the Bitrix24 study:
- **Money is `numeric(18,2)`**, never `double` (Bitrix24's one clear schema flaw).
- **Soft delete** everywhere via a global query filter — nothing is physically removed.
- **Stage semantic** (Process/Success/Failure) stored separately from the stage name.
- **Kanban move** recomputes semantic, flips `IsClosed`, and resets `CloseDate` — the
  exact behavior verified live against the real system.

## Connection

The connection string lives in **User Secrets** (not in `appsettings.json`):

```
Host=localhost;Port=5433;Database=HubS-pot-CRM;Username=postgres;Password=******
```

To reset it:

```bash
cd src/Crm.Api
dotnet user-secrets set "ConnectionStrings:Default" "Host=localhost;Port=5433;Database=HubS-pot-CRM;Username=postgres;Password=YOURPASS"
```

## Run

```bash
# from CrmBackend/
dotnet build
dotnet run --project src/Crm.Api
```

Then open **`/swagger`** for the interactive API. In Development the app applies any
pending migrations automatically on startup.

## Migrations

```bash
# create a new migration
dotnet ef migrations add <Name> \
  --project src/Crm.Infrastructure --startup-project src/Crm.Api \
  --output-dir Persistence/Migrations

# apply
dotnet ef database update \
  --project src/Crm.Infrastructure --startup-project src/Crm.Api
```

## API surface (Contacts + Applications implemented as samples)

| Method | Route | Purpose |
|---|---|---|
| GET | `/api/contacts` | list — `?page&pageSize&search&companyId&assignedToId&sortBy&sortDescending` |
| GET | `/api/contacts/{id}` | single, with company + owner |
| POST | `/api/contacts` | create |
| PUT | `/api/contacts/{id}` | update |
| DELETE | `/api/contacts/{id}` | soft delete |
| GET | `/api/applications` | list — `?stage&isClosed&minAmount&maxAmount&contactId&companyId&…` |
| GET | `/api/applications/{id}` | single, with contact + company + owner |
| POST | `/api/applications` | create |
| PUT | `/api/applications/{id}` | update |
| PATCH | `/api/applications/{id}/stage` | move pipeline stage (kanban) |
| DELETE | `/api/applications/{id}` | soft delete |

The remaining modules (Companies, Activities, Messages, Users) reuse the exact same
service + controller pattern — add a service, register it in
`Crm.Application/DependencyInjection.cs`, and drop in a controller.
```
