# Cinturon360 Mid-Office MVP — Build Progress

## Phase 1: Shared models, DB schema, mock data ✅

- [x] Delete default Class1.cs from Cinturon360.Mock.Shared
- [x] Update Shared csproj to target net10.0
- [x] Create all enum files (17 enums in Enums/)
- [x] Create all DTO files (24 DTOs in Dtos/)
- [x] Create db/init/001_schema.sql (full PostgreSQL schema)
- [x] Create db/init/002_seed.sql (organisations, users, billing, policies, travellers)
- [x] Create mock-data/gds/ — 8 PNR scenario files
- [x] Create mock-data/finance/ — xero, netsuite, generic CSV samples
- [x] Create mock-data/bsp/ — ADM/ACM and ticket reconciliation CSV samples

## Phase 2: API — DbContext, entities, services, controllers ✅

- [x] Add EF Core entities mirroring schema tables (21 entity classes)
- [x] Create AppDbContext with all DbSets and snake_case column mapping
- [x] Configure connection string and register DbContext
- [x] AuditService, InvoiceNumberService, BillingService, PaymentSimulationService, GdsImportService
- [x] AuthController — POST /api/auth/login (bcrypt verify)
- [x] DashboardController — GET /api/dashboard
- [x] OrganisationsController — tree + detail
- [x] BillingRelationshipsController
- [x] PaymentMethodsController
- [x] TravelPoliciesController
- [x] ImportsController — GDS file list + import, BSP stub
- [x] BookingsController — list, detail, ready, invoice generation, cost centre, policy
- [x] InvoicesController — list, detail, simulate, retry, manual, external
- [x] AgencyMemosController — list, detail, outcome, CSV import
- [x] ReportsController — 7 report endpoints
- [x] FinanceExportsController — create batch + download CSV
- [x] AuditController, LedgerController
- [x] Swagger/OpenAPI via Swashbuckle

## Phase 3: Blazor Web — pages, components ✅

- [x] Login page
- [x] Dashboard page (12 stat cards + architecture principle)
- [x] Organisation hierarchy page (recursive tree + detail)
- [x] Billing relationships page
- [x] Payment methods page + policy→payment mapping table
- [x] Travel policies page
- [x] GDS Import page
- [x] Booking list page
- [x] Booking detail page (remediation: cost centre, policy assignment, generate invoice)
- [x] Invoice list page
- [x] Invoice detail page (simulate, retry, manual, external settle actions)
- [x] ADM/ACM workbench list page + CSV import
- [x] ADM/ACM detail page (outcome actions)
- [x] Reports page (outstanding balances, ADM/ACM exposure, future balances)
- [x] Ledger page
- [x] Finance exports page (create + download)
- [x] Audit log page
- [x] Sidebar layout with simulation banner

## Phase 4: Docker infrastructure ✅

- [x] docker-compose.yml (PostgreSQL 17 + API + Web)
- [x] .env.example
- [x] DB init scripts mounted at /docker-entrypoint-initdb.d
- [x] Service dependencies and health checks
- [x] Dockerfiles for API and Web (multi-stage, .NET 10 SDK → runtime)

## Phase 5: Integration verification

- [ ] Run: docker compose up -d --build
- [ ] Verify login works (demo@avanoa.local / DemoPassword123!)
- [ ] Walk through all 8 GDS import scenarios
- [ ] Verify ADM/ACM import and decisioning
- [ ] Verify finance export CSV download
- [ ] Verify audit log records
