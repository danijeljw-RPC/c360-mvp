What was built

  97 files across 3 projects. Full solution summary:

  To start the demo

  cd /Users/danijeljw/Developer/c360-mvp
  docker compose up -d --build

  Then open http://localhost:5089 and log in with demo@avanoa.local / DemoPassword123!

  ---
  What's in the system

  ┌──────────┬──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────┐
  │  Layer   │                                                                                                         Contents                                                                                                         │
  ├──────────┼──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────┤
  │ Shared   │ 17 enums, 24 DTOs covering all domain concepts                                                                                                                                                                           │
  ├──────────┼──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────┤
  │ Database │ 22-table PostgreSQL schema + deterministic seed (orgs, users, billing relationships, payment methods, travel policies, travellers)                                                                                       │
  ├──────────┼──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────┤
  │ API      │ 14 controllers, 5 services (billing, payment simulation, GDS import, audit, invoice numbering), Swagger at /swagger                                                                                                      │
  ├──────────┼──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────┤
  │ Web UI   │ 17 Blazor pages: login, dashboard, org hierarchy, billing config, payment methods, policies, GDS import, booking list/detail, invoice list/detail, ADM/ACM workbench + detail, reports, ledger, finance exports, audit   │
  │          │ log                                                                                                                                                                                                                      │
  ├──────────┼──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────┤
  │ Mock     │ 8 GDS PNR JSON files, BSP ADM/ACM CSV, finance export samples                                                                                                                                                            │
  │ data     │                                                                                                                                                                                                                          │
  ├──────────┼──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────┤
  │ Docker   │ Multi-stage .NET 10 Dockerfiles for API and Web, docker-compose with PostgreSQL 17 + health checks                                                                                                                       │
  └──────────┴──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────┘

  Key demo flows implemented

  - GDS import with completeness validation — 8 scenarios including deposit/balance, policy payment routing, shadow billing
  - Payment simulation — Visa 1111 fails first attempt / succeeds on retry; manual invoice; external settlement; shadow-only
  - ADM/ACM workbench — import CSV, match by ticket number, set outcome (absorb/recharge/dispute/credit), generates ledger entries and invoice adjustment lines
  - Finance exports — create batch targeting Xero/NetSuite/Generic, download CSV
  - Audit trail — every meaningful action writes an AuditEvent row viewable in the UI
  - Simulation banner always visible: "Provider actions are simulated only. No external payments are made. Cinturon360 owns billing logic, ledger and audit trail."
