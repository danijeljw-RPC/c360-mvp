# CLAUDE.md

## Project Role

You are building a local-only, fully simulated Cinturon360 mid-office MVP mockup.

Assume you have no prior project context. Everything needed to understand the domain and build the demo is in `README.md` and this file.

## Non-Negotiable Goal

Build a deterministic demo system that proves this architecture principle:

> Cinturon360 owns the internal booking-finance logic, ledger, invoice structure and audit trail. Payment providers, finance systems, GDS/NDC imports and BSP-style files are simulated channels only.

This is not a production system. Do not integrate with live services.

## Product Context

Cinturon360 is a proposed travel technology platform for travel agencies, brokers and Travel Management Companies.

The mockup must simulate a modern mid-office layer that connects booking data to financial outcomes:

- booking finance;
- invoicing;
- payment orchestration;
- travel policy billing rules;
- deposits and future balances;
- service fees and markups;
- manual/external/shadow billing;
- GDS/NDC/content import;
- BSP-style ticket reconciliation;
- ADM/ACM handling;
- accounting/finance exports;
- reporting and audit logging.

## Demonstration Hierarchy

Use this hierarchy everywhere:

```text
Avanōa Technology
  └── HelloWorld Travel
        └── Coca-Cola
```

Definitions:

- Avanōa Technology: platform/vendor organisation.
- HelloWorld Travel: TMC organisation using the platform.
- Coca-Cola: client organisation onboarded by HelloWorld Travel.
- Seller/billing owner: the parent organisation that commercially bills the child organisation.
- Buyer: the organisation receiving travel services or platform services.

Commercial responsibility rule:

- If HelloWorld Travel configures billing for Coca-Cola, HelloWorld Travel owns the credit/payment risk.
- Avanōa Technology is responsible only for platform operation, rule execution, payment routing simulation and audit records unless Avanōa Technology is explicitly the seller for that relationship.

## Stack

Use:

- .NET 10;
- ASP.NET Core WebAPI;
- Blazor Server;
- EF Core;
- PostgreSQL 17;
- Docker Compose.

Do not use `Startup.cs`. Use modern `Program.cs` hosting.

## Repository Shape

Create or maintain this structure:

```text
cinturon360-midoffice-mockup/
  CLAUDE.md
  README.md
  docker-compose.yml
  .env.example
  src/
    Cinturon360.Mock.Api/
    Cinturon360.Mock.Web/
    Cinturon360.Mock.Shared/
  db/
    init/
      001_schema.sql
      002_seed.sql
  mock-data/
    gds/
    bsp/
    finance/
    payments/
  docs/
    demo-script.md
    scenario-matrix.md
    domain-model.md
    api-endpoints.md
```

## Build Priority

Build in this order:

1. Docker Compose with PostgreSQL, API and Web services.
2. Database schema and seed data.
3. Fake login.
4. Organisation hierarchy screen.
5. Booking import screen.
6. Booking detail + completeness checks.
7. Booking finance calculation.
8. Internal ledger and invoice generation.
9. Payment schedule screen.
10. Payment routing simulation.
11. Manual/external/shadow billing simulation.
12. ADM/ACM import and decisioning.
13. Reporting/export screens.
14. Demo script documentation.

## Authentication

Create a fake login only.

Seed account:

```text
Email: demo@avanoa.local
Password: DemoPassword123!
Role: PlatformAdmin
Organisation: Avanōa Technology
```

Requirements:

- validate against seeded database data;
- issue a simple server-side cookie;
- no real SSO;
- no MFA;
- no JWT required unless simpler;
- no production security claims.

## Mock Providers

All provider behaviour must be fake and deterministic.

Supported simulated payment providers:

- Stripe;
- Airwallex;
- Braintree;
- PayPal;
- Square;
- Manual Invoice;
- External Finance System;
- Partner Managed Billing;
- No Charge / Trial / Shadow Billing.

Never collect, store or request real card details.

Use safe display-only fake payment metadata:

```text
American Express ending 8431, expires 03/2044
Mastercard ending 4444, expires 12/2033
Visa ending 1111, expires 08/2030
```

## Core Scenarios To Implement

Implement these as seeded scenarios and UI actions.

### Scenario 1 — Immediate Card Payment

- Import complete PNR.
- Match traveller, policy, cost centre and supplier.
- Calculate fare, taxes, service fee and surcharge.
- Create internal invoice.
- Route to simulated Stripe.
- Mark payment as succeeded.

### Scenario 2 — Deposit Now, Balance Later

- Package total: AUD 5,000.
- Deposit: AUD 1,000.
- Service fee: AUD 150.
- Balance: AUD 4,000.
- Balance due 30 days before departure.
- Create schedule items for immediate deposit/service fee and future balance.

### Scenario 3 — Missing Cost Centre

- Import PNR missing cost centre.
- Mark booking as incomplete.
- Prevent final invoice/payment action until fixed.
- Allow user to assign a cost centre.
- Re-run validation.

### Scenario 4 — Policy Payment Override

- Coca-Cola has default payment method.
- APAC travel policy uses APAC card.
- EMEA travel policy uses EMEA card.
- North America policy uses default organisation billing.
- Booking payment route must follow policy configuration.

### Scenario 5 — Failed Payment

- Generate invoice.
- Route to simulated provider.
- Return failed payment response.
- Show outstanding balance and retry/manual settlement options.

### Scenario 6 — Manual Invoice

- Billing mode is manual.
- Generate internal invoice.
- Do not route to card provider.
- Mark as awaiting manual settlement.
- Allow mock manual payment confirmation.

### Scenario 7 — External Finance Export

- Generate internal invoice/ledger.
- Export to fake Xero/NetSuite/generic CSV.
- Mark invoice as externally managed.

### Scenario 8 — ADM/ACM Handling

- Import fake BSP ADM/ACM CSV.
- Match memo to booking/ticket/document number.
- Show reason code, amount, supplier, traveller, policy and original invoice.
- Allow decision: dispute, absorb internally, recharge client.
- If recharge, create adjustment invoice line.

## Domain Modelling Rules

Use clear domain names. Required concepts:

- Organisation;
- OrganisationRelationship;
- Traveller;
- TravelPolicy;
- PaymentProviderConnection;
- PaymentMethodReference;
- Booking;
- BookingSegment;
- BookingDocument;
- BookingPaymentSchedule;
- BookingPaymentScheduleItem;
- Invoice;
- InvoiceLine;
- LedgerEntry;
- PaymentAttempt;
- ServiceFeeRule;
- ImportedContentBatch;
- ImportedContentIssue;
- AdmAcmMemo;
- FinanceExportBatch;
- AuditEvent.

Keep the model simple enough for a demo. Prefer readable code over perfect abstraction.

## API Requirements

Expose REST endpoints for the Blazor UI.

Minimum groups:

- `/api/auth`
- `/api/organisations`
- `/api/bookings`
- `/api/imports`
- `/api/billing`
- `/api/invoices`
- `/api/payments`
- `/api/adm-acm`
- `/api/reports`
- `/api/exports`

Use DTOs in `Cinturon360.Mock.Shared`.

## UI Requirements

Blazor Server pages required:

- Login;
- Dashboard;
- Organisation hierarchy;
- Billing configuration;
- Payment methods;
- Travel policies;
- Booking imports;
- Booking list;
- Booking detail;
- Booking finance/ledger;
- Invoice detail;
- Payment schedule;
- Payment attempts;
- ADM/ACM workbench;
- Reports;
- Finance exports;
- Audit log.

The UI should be clear and demo-friendly. It does not need pixel-perfect styling.

## Coding Style

Use concise C#.

Prefer:

- explicit DTOs;
- small services;
- simple controllers/minimal APIs;
- EF Core migrations or SQL init scripts;
- deterministic seed data;
- readable enum names;
- comments only where useful.

Avoid:

- real external service SDKs;
- over-engineered domain frameworks;
- CQRS/event sourcing unless already trivial;
- unnecessary auth complexity;
- storing secrets;
- production claims.

## Commands You May Need

```bash
dotnet new sln -n Cinturon360.Mock

dotnet new webapi -n Cinturon360.Mock.Api -o src/Cinturon360.Mock.Api

dotnet new blazorserver -n Cinturon360.Mock.Web -o src/Cinturon360.Mock.Web

dotnet new classlib -n Cinturon360.Mock.Shared -o src/Cinturon360.Mock.Shared

dotnet sln add src/Cinturon360.Mock.Api/Cinturon360.Mock.Api.csproj

dotnet sln add src/Cinturon360.Mock.Web/Cinturon360.Mock.Web.csproj

dotnet sln add src/Cinturon360.Mock.Shared/Cinturon360.Mock.Shared.csproj

dotnet add src/Cinturon360.Mock.Api/Cinturon360.Mock.Api.csproj reference src/Cinturon360.Mock.Shared/Cinturon360.Mock.Shared.csproj

dotnet add src/Cinturon360.Mock.Web/Cinturon360.Mock.Web.csproj reference src/Cinturon360.Mock.Shared/Cinturon360.Mock.Shared.csproj

dotnet add src/Cinturon360.Mock.Api package Npgsql.EntityFrameworkCore.PostgreSQL

dotnet add src/Cinturon360.Mock.Api package Microsoft.EntityFrameworkCore.Design

dotnet add src/Cinturon360.Mock.Api package Swashbuckle.AspNetCore

dotnet restore

dotnet build

docker compose up -d
```

If the installed .NET SDK template names differ, adapt the commands but keep the stack and intent.

## Completion Criteria

The demo is complete when a user can:

1. run `docker compose up -d`;
2. open the Blazor UI;
3. log in using the seeded demo account;
4. view Avanōa → HelloWorld → Coca-Cola hierarchy;
5. import/select fake PNR scenarios;
6. see incomplete data warnings;
7. generate internal booking finance, ledger and invoice records;
8. simulate payment routing outcomes;
9. view deposits and future balances;
10. import ADM/ACM records;
11. dispute/absorb/recharge ADM items;
12. generate fake finance exports;
13. show dashboard/reporting screens;
14. explain from the UI that Cinturon360 owns billing logic and providers are channels only.

## Final Instruction

Do not ask for more business context before building. Use `README.md` and this file as the source material. Make reasonable assumptions and document them in `docs/assumptions.md`.
