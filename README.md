# Cinturon360 Mid-Office MVP Mockup — Self-Contained AI Agent Build Instructions

## Read This First

You are a brand-new AI coding agent. Assume you have **no prior knowledge** of Cinturon360, Avanōa Technology, Avanoa Technology, RePass Cloud, Tramada, Dolphin, BSP, GDS, NDC, TMCs, travel mid-office systems, travel trust accounting, Stripe billing, or the business model behind this project.

This repository is for a **throwaway/local demonstration system only**. It is not the real Cinturon360 product and must not connect to live services. Build a deterministic mock application that demonstrates the workflows described here so stakeholders can review the intended operating model before the production system is designed.

## Plain-English Product Context

**Cinturon360** is the working name for a proposed modern travel technology platform. In this mockup, it should behave like a simplified travel mid-office and finance-control layer for Travel Management Companies.

A **Travel Management Company (TMC)** manages travel bookings for client organisations. A client organisation may be a corporate customer such as Coca-Cola. A TMC may use travel content from GDS/NDC/supplier systems, apply service fees, issue invoices, collect payment, track deposits/balances, reconcile supplier/ticketing data, and handle later debit/credit adjustments such as ADMs and ACMs.

The demonstration hierarchy is:

```text
Avanōa Technology
  └── HelloWorld Travel
        └── Coca-Cola
```

For this mockup:

- **Avanōa Technology** is the platform/vendor organisation.
- **HelloWorld Travel** is a TMC using the platform.
- **Coca-Cola** is a client organisation of HelloWorld Travel.
- HelloWorld Travel owns the commercial billing relationship with Coca-Cola.
- Cinturon360 executes configured billing rules and records the audit trail.
- Payment providers are simulated execution channels only.
- Finance systems are simulated export targets only.

The central architecture principle to demonstrate is:

> Cinturon360 owns the internal billing logic, ledger, invoice structure, booking-finance model and audit trail. Stripe, Airwallex, Braintree, PayPal, Square, manual invoices, external finance systems and BSP-style imports are only execution/import/export channels.

## Required Interpretation Rules For The AI Agent

When in doubt, prioritise a working end-to-end demonstration over production correctness.

Use simple code and deterministic data. Do not over-engineer. Do not add real integrations. Do not request live credentials. Do not make external network calls except normal package restore/build operations.

The UI should make the workflow visible and understandable:

1. hierarchy;
2. booking import;
3. missing booking data detection;
4. travel policy billing decision;
5. internal ledger creation;
6. invoice generation;
7. simulated payment routing;
8. deposit/balance schedule;
9. failed/manual/external billing scenarios;
10. ADM/ACM import and decisioning;
11. reporting/export.

The goal is stakeholder feedback, not production deployment.

---

## Purpose

Build a completely simulated, non-production demonstration system for Cinturon360 that can be run locally from a development machine. The system must demonstrate the operational and financial processing flows described in the CEO email: booking finance, invoicing, payment orchestration, travel policy billing rules, service fees, deposits/future balances, ADM/ACM handling, GDS/content import, BSP-style reconciliation, finance exports, audit logging, and hierarchy-aware billing ownership.

This is a separate mockup project. It is not the real Cinturon360 platform. It must be simple, deterministic, visually demonstrable, and suitable for stakeholder feedback.

The mockup should run using Docker with:

- PostgreSQL
- ASP.NET Core WebAPI
- Blazor Server front end
- Seeded database data
- Fake login UI
- Mock payment providers
- Mock GDS/import files
- Mock BSP/ADM/ACM files
- Mock finance export files

Target stack:

- .NET 10
- ASP.NET Core WebAPI
- Blazor Server
- EF Core
- PostgreSQL
- Docker Compose

Do not build real payment integrations. Do not connect to Stripe, Airwallex, Braintree, PayPal, Square, GDS providers, NDC providers, IATA, BSPLink, NetSuite, Xero, MYOB, SAP, Oracle, or any live external system.

Everything must be simulated.

---

## Demonstration Goal

The demo must let a stakeholder log in and walk through the following story:

1. Avanōa Technology is the platform/vendor organisation.
2. Avanōa Technology onboards HelloWorld Travel as a TMC.
3. HelloWorld Travel onboards Coca-Cola as a Client organisation.
4. Coca-Cola has multiple travel policies and multiple payment methods.
5. A booking is imported from a fake GDS/NDC/content file.
6. The booking is matched to traveller, client, TMC, policy, cost centre and supplier data.
7. The system identifies whether data is complete or incomplete.
8. The system calculates booking finance internally first.
9. The system creates an internal ledger and invoice before any payment provider action.
10. The configured billing rule determines whether payment is immediate, delayed, manual, external, shadow, or scheduled.
11. The configured travel policy determines which payment method or billing path is used.
12. The system simulates payment provider routing.
13. The system tracks invoice/payment status.
14. The system handles a deposit now and balance later scenario.
15. The system simulates a failed payment scenario.
16. The system simulates a manual invoice scenario.
17. The system simulates an external finance export scenario.
18. The system imports fake ADM/ACM items and links them back to bookings/tickets.
19. The system lets a user decide whether an ADM should be absorbed, disputed, or recharged.
20. The system generates reports for bookings, invoices, deposits due, balances due, failed payments, service fees, ADM/ACM exposure, refunds, credits and supplier reconciliation.

The demo must clearly prove the architectural point:

> Cinturon360 owns the billing logic, ledger, invoice structure and audit trail. Payment providers and finance systems are execution/export channels only.

---

## Non-Goals

Do not build production security.
Do not build real payment collection.
Do not build real card storage.
Do not build real SSO.
Do not build real GDS integrations.
Do not build real BSP settlement.
Do not build real trust accounting.
Do not build real accounting compliance.
Do not build a complete booking engine.
Do not build an airline/hotel search engine.
Do not use real customer data.
Do not use real payment method data.
Do not use real PNRs unless they are obviously fake.

---

## Local Demo Login

Create a basic simulated login page.

Seed this account:

```text
Email: demo@avanoa.local
Password: DemoPassword123!
Role: PlatformAdmin
Organisation: Avanōa Technology
```

Authentication can be fake/session-based for the demo. It only needs to allow the UI to show role-aware screens.

Recommended approach:

- Store seeded demo users in PostgreSQL.
- On login, validate email/password against seeded records.
- Issue a simple server-side auth cookie.
- Do not implement JWT unless already easier.
- Do not implement SSO.
- Do not implement MFA.

---

## Docker Layout

Create a new repo/project with this shape:

```text
cinturon360-midoffice-mockup/
  docker-compose.yml
  README.md
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
      pnr-import-complete.json
      pnr-import-missing-cost-centre.json
      pnr-import-missing-policy.json
      pnr-import-refund-reissue.json
      pnr-import-hotel-offline.json
    bsp/
      bsp-adm-acm-sample.csv
      bsp-ticket-reconciliation-sample.csv
    finance/
      xero-export-sample.csv
      netsuite-export-sample.csv
      generic-ledger-export-sample.csv
    payments/
      stripe-webhook-payment-succeeded.json
      stripe-webhook-payment-failed.json
      manual-payment-confirmation.json
  docs/
    demo-script.md
    scenario-matrix.md
    domain-model.md
    api-endpoints.md
```

### docker-compose.yml

Required services:

```yaml
services:
  postgres:
    image: postgres:17
    container_name: cinturon360-mock-postgres
    environment:
      POSTGRES_DB: cinturon360_mock
      POSTGRES_USER: cinturon360
      POSTGRES_PASSWORD: cinturon360_dev_password
    ports:
      - "54329:5432"
    volumes:
      - pgdata:/var/lib/postgresql/data
      - ./db/init:/docker-entrypoint-initdb.d:ro
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U cinturon360 -d cinturon360_mock"]
      interval: 5s
      timeout: 5s
      retries: 20

  api:
    build:
      context: .
      dockerfile: src/Cinturon360.Mock.Api/Dockerfile
    container_name: cinturon360-mock-api
    environment:
      ASPNETCORE_ENVIRONMENT: Development
      ConnectionStrings__DefaultConnection: Host=postgres;Port=5432;Database=cinturon360_mock;Username=cinturon360;Password=cinturon360_dev_password
    ports:
      - "5088:8080"
    depends_on:
      postgres:
        condition: service_healthy

  web:
    build:
      context: .
      dockerfile: src/Cinturon360.Mock.Web/Dockerfile
    container_name: cinturon360-mock-web
    environment:
      ASPNETCORE_ENVIRONMENT: Development
      ApiBaseUrl: http://api:8080
    ports:
      - "5089:8080"
    depends_on:
      - api

volumes:
  pgdata:
```

Local URLs:

```text
Web UI: http://localhost:5089
API:    http://localhost:5088
DB:     localhost:54329
```

---

## Deployment Guide

This section is for the infrastructure team. It explains how to run the system in three environments: local development, internal intranet, and public internet (e.g. `https://demo.cinturon360.com`).

### Why there are two API URL settings

The system has two services in Docker:

| Service | Internal Docker hostname | Host port |
|---|---|---|
| API (`Cinturon360.Mock.Api`) | `api:8080` | `5088` |
| Web (`Cinturon360.Mock.Web`) | `web:8080` | `5089` |

The **web server** calls the API on behalf of the Blazor application using `ApiBaseUrl`. This is a server-to-server call inside the Docker network, so it uses the Docker service name `http://api:8080`.

Some actions — such as downloading a generated CSV export — produce a URL that the **browser** must navigate to directly. That URL must use a hostname the browser can resolve. This is controlled by a separate setting: `ApiPublicUrl`.

**Rule:**
- `ApiBaseUrl` = what the web container uses to call the API internally. Never change this unless you change the Docker network topology.
- `ApiPublicUrl` = what the browser uses to reach the API. This must match however you expose the API to the outside world.

---

### Environment 1 — Local development (default)

No changes needed. Run:

```bash
docker compose up -d --build
```

| Setting | Value |
|---|---|
| `ApiBaseUrl` | `http://api:8080` (Docker internal — do not change) |
| `ApiPublicUrl` | `http://localhost:5088` (default) |

Open the UI at `http://localhost:5089`.

---

### Environment 2 — Internal intranet server

The server is on your network at a known IP or hostname, e.g. `192.168.1.50` or `c360-demo.company.local`.

No reverse proxy needed if plain HTTP is acceptable. The API port (`5088`) and web port (`5089`) must both be accessible from the demo machine's browser.

Set `ApiPublicUrl` in `docker-compose.yml` before running:

```yaml
services:
  web:
    environment:
      ApiBaseUrl: http://api:8080       # DO NOT CHANGE — Docker internal
      ApiPublicUrl: http://192.168.1.50:5088   # replace with your server IP or hostname
```

Or pass it as an environment variable without editing the file:

```bash
ApiPublicUrl=http://192.168.1.50:5088 docker compose up -d --build
```

Then open the UI at `http://192.168.1.50:5089`.

---

### Environment 3 — Public internet with HTTPS via Caddy (mvp-dev.cinturon360.com)

This is the setup for deploying to a public server using `docker-compose.yml` (the default compose file). Caddy runs inside the Docker stack and handles TLS termination using Cloudflare Origin certificates.

There is no separate reverse proxy to install on the host — Caddy is a service in the compose file.

#### Compose files

| File | Purpose |
|---|---|
| `docker-compose.yml` | Public deployment — includes Caddy, no host ports exposed for API/web |
| `docker-compose.local.yml` | Local development — no Caddy, API on `8090`, web on `8080` |

#### Public URLs

| URL | Service |
|---|---|
| `https://mvp-dev.cinturon360.com` | Blazor web UI |
| `https://mvp-dev-api.cinturon360.com` | REST API |
| `https://mvp-dev-api.cinturon360.com/swagger` | Swagger UI (engineering) |

Both subdomains must have DNS A records pointing to the server. Both use the same Cloudflare Origin certificate.

#### Certificate setup

Place your Cloudflare Origin certificates in a `certs/` directory at the project root on the server:

```text
certs/
  cloudflare-origin.pem        ← certificate (public)
  cloudflare-origin-key.pem    ← private key (keep secret)
```

The `certs/` directory is in `.gitignore`. Never commit certificates to the repository.

Set correct file permissions before starting the stack:

```bash
# The cert directory must be readable by the Caddy container (runs as root)
chmod 755 certs/

# Certificate file — readable by owner and group, not world-writable
chmod 644 certs/cloudflare-origin.pem

# Private key — readable by owner only
chmod 600 certs/cloudflare-origin-key.pem
```

Verify:

```bash
ls -la certs/
# Expected output:
# drwxr-xr-x  cloudflare-origin.pem
# -rw-r--r--  cloudflare-origin.pem
# -rw-------  cloudflare-origin-key.pem
```

#### Running on the target server

```bash
# Clone or copy the project to the server
git clone <repo-url> c360-mvp
cd c360-mvp

# Place certs (copy from secure storage, do not commit)
mkdir -p certs
cp /path/to/cloudflare-origin.pem certs/
cp /path/to/cloudflare-origin-key.pem certs/
chmod 644 certs/cloudflare-origin.pem
chmod 600 certs/cloudflare-origin-key.pem

# Build and start the full stack (Caddy + web + API + postgres)
docker compose up -d --build

# Check all services are running
docker compose ps

# Follow logs
docker compose logs -f caddy web api
```

The site will be available at `https://mvp-dev.cinturon360.com` once DNS resolves to the server and the stack is running.

#### Stopping and resetting

```bash
# Stop without removing data
docker compose down

# Full reset — removes all data volumes
docker compose down -v
docker compose up -d --build
```

> **Blazor Server requires WebSocket / SignalR.** Caddy proxies WebSocket connections automatically. No additional configuration is required.

---

### Quick reference — what to change per environment

| Environment | `ApiBaseUrl` | `ApiPublicUrl` |
|---|---|---|
| Local (`localhost`) | `http://api:8080` | `http://localhost:5088` |
| Intranet by IP | `http://api:8080` | `http://192.168.x.x:5088` |
| Intranet by hostname | `http://api:8080` | `http://c360-demo.company.local:5088` |
| Public HTTPS (two subdomains) | `http://api:8080` | `https://api-demo.cinturon360.com` |
| Public HTTPS (single domain) | `http://api:8080` | `https://demo.cinturon360.com` |

`ApiBaseUrl` is always `http://api:8080`. Only `ApiPublicUrl` changes between environments.

---

## Core Domain Concepts

### Organisation hierarchy

Represent platform hierarchy separately from billing ownership.

Seed hierarchy:

```text
Avanōa Technology
└── HelloWorld Travel
    └── Coca-Cola
```

Organisation types:

```csharp
public enum OrganisationType
{
    Vendor = 1,
    Tmc = 2,
    Client = 3
}
```

Important rule:

A parent organisation can configure billing for a child organisation. In that case, the parent is the seller/billing owner for that relationship.

Example:

- Avanōa Technology bills HelloWorld Travel.
- HelloWorld Travel bills Coca-Cola.
- Coca-Cola travellers generate bookings.
- HelloWorld Travel owns Coca-Cola commercial risk unless Avanōa Technology is explicitly configured as seller for that billing relationship.

### Billing relationship

Create an explicit billing relationship record:

```csharp
public sealed class BillingRelationship
{
    public Guid Id { get; set; }
    public Guid SellerOrganisationId { get; set; }
    public Guid BuyerOrganisationId { get; set; }
    public BillingMode BillingMode { get; set; }
    public PaymentProviderType PaymentProviderType { get; set; }
    public string CurrencyCode { get; set; } = "AUD";
    public int PaymentTermsDays { get; set; }
    public bool IsCommercialRiskHeldBySeller { get; set; } = true;
}
```

```csharp
public enum BillingMode
{
    ImmediatePayment = 1,
    AuthoriseOnlyCaptureLater = 2,
    DelayedBilling = 3,
    ManualInvoice = 4,
    ExternalSettlement = 5,
    ShadowBilling = 6,
    NoCharge = 7,
    Trial = 8
}
```

```csharp
public enum PaymentProviderType
{
    Stripe = 1,
    Airwallex = 2,
    Braintree = 3,
    PayPal = 4,
    Square = 5,
    ManualInvoice = 6,
    ExternalProcessor = 7,
    PartnerManagedBilling = 8,
    NoCharge = 9,
    ShadowBilling = 10
}
```

### Payment provider account

Simulate seller-owned provider accounts.

```csharp
public sealed class PaymentProviderAccount
{
    public Guid Id { get; set; }
    public Guid SellerOrganisationId { get; set; }
    public PaymentProviderType ProviderType { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string FakeExternalAccountId { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }
    public bool IsEnabled { get; set; }
}
```

Example seeded rows:

```text
HelloWorld Travel Stripe Account        acct_hw_stripe_mock
HelloWorld Travel Manual Invoice        manual_hw_invoice
Avanōa Technology Stripe Account        acct_avanoa_stripe_mock
Avanōa Technology Shadow Billing        shadow_avanoa_mock
```

### Buyer payment profile

Simulate buyer/customer references in the seller's payment provider.

```csharp
public sealed class BuyerPaymentProfile
{
    public Guid Id { get; set; }
    public Guid SellerOrganisationId { get; set; }
    public Guid BuyerOrganisationId { get; set; }
    public Guid PaymentProviderAccountId { get; set; }
    public string FakeProviderCustomerId { get; set; } = string.Empty;
}
```

Example:

```text
Seller: HelloWorld Travel
Buyer: Coca-Cola
Provider: HelloWorld Travel Stripe Account
Fake Customer ID: cus_cocacola_hw_mock
```

### Saved payment method

Only safe display metadata is allowed.

```csharp
public sealed class SavedPaymentMethod
{
    public Guid Id { get; set; }
    public Guid BuyerPaymentProfileId { get; set; }
    public string FakeProviderPaymentMethodId { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string Last4 { get; set; } = string.Empty;
    public int ExpiryMonth { get; set; }
    public int ExpiryYear { get; set; }
    public bool IsDefault { get; set; }
}
```

Seed examples:

```text
American Express ending 8431, expires 03/2044, default
Mastercard ending 4444, expires 12/2033, APAC policy
Visa ending 1111, expires 08/2032, EMEA policy
```

---

## Travel Policies and Billing Rules

Each client organisation can have multiple travel policies.

```csharp
public sealed class TravelPolicy
{
    public Guid Id { get; set; }
    public Guid ClientOrganisationId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public TravelPolicyBillingBehaviour BillingBehaviour { get; set; }
    public Guid? SavedPaymentMethodId { get; set; }
}
```

```csharp
public enum TravelPolicyBillingBehaviour
{
    UseOrganisationDefault = 1,
    UseSpecificPaymentMethod = 2,
    ManualInvoice = 3,
    ExternalSettlement = 4,
    ShadowBilling = 5,
    NoCharge = 6
}
```

Seed policies:

```text
CCA-APAC      Coca-Cola APAC Travel Policy        UseSpecificPaymentMethod -> Mastercard 4444
CCA-EMEA      Coca-Cola EMEA Travel Policy        UseSpecificPaymentMethod -> Visa 1111
CCA-NA        Coca-Cola North America Policy      UseOrganisationDefault -> Amex 8431
CCA-EXEC      Coca-Cola Executive Travel Policy   ManualInvoice
CCA-TRIAL     Coca-Cola Trial/Shadow Policy       ShadowBilling
```

Rule:

If the travel policy billing behaviour is `UseOrganisationDefault`, use the default payment method/payment profile from the billing relationship.

If the travel policy has a specific payment method, route the booking invoice/payment to that method.

If manual/external/shadow/no-charge is selected, do not simulate a card charge. Create invoice/ledger/audit records and set the correct status.

---

## Booking Finance Model

Create internal booking finance before pushing anything to a provider.

```csharp
public sealed class Booking
{
    public Guid Id { get; set; }
    public string BookingReference { get; set; } = string.Empty;
    public string? PnrCode { get; set; }
    public Guid TravellerId { get; set; }
    public Guid ClientOrganisationId { get; set; }
    public Guid TmcOrganisationId { get; set; }
    public Guid VendorOrganisationId { get; set; }
    public Guid? TravelPolicyId { get; set; }
    public string? CostCentreCode { get; set; }
    public BookingStatus Status { get; set; }
    public DateOnly DepartureDate { get; set; }
    public DateOnly ReturnDate { get; set; }
    public string CurrencyCode { get; set; } = "AUD";
    public decimal SupplierAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal ServiceFeeAmount { get; set; }
    public decimal SurchargeAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public string SourceSystem { get; set; } = string.Empty;
}
```

```csharp
public enum BookingStatus
{
    Imported = 1,
    NeedsReview = 2,
    ReadyForBilling = 3,
    Invoiced = 4,
    PartiallyPaid = 5,
    Paid = 6,
    Cancelled = 7,
    Refunded = 8,
    Exchanged = 9
}
```

### Payment schedule

```csharp
public sealed class BookingPaymentSchedule
{
    public Guid Id { get; set; }
    public Guid BookingId { get; set; }
    public decimal TotalScheduledAmount { get; set; }
    public string CurrencyCode { get; set; } = "AUD";
    public PaymentScheduleStatus Status { get; set; }
}
```

```csharp
public sealed class BookingPaymentScheduleItem
{
    public Guid Id { get; set; }
    public Guid BookingPaymentScheduleId { get; set; }
    public PaymentScheduleItemType ItemType { get; set; }
    public PaymentTiming PaymentTiming { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateOnly? DueDate { get; set; }
    public PaymentScheduleItemStatus Status { get; set; }
}
```

```csharp
public enum PaymentScheduleItemType
{
    Deposit = 1,
    SupplierBalance = 2,
    ServiceFee = 3,
    CardSurcharge = 4,
    Tax = 5,
    Adjustment = 6,
    Refund = 7,
    Credit = 8,
    Adm = 9,
    Acm = 10
}
```

```csharp
public enum PaymentTiming
{
    Immediate = 1,
    DueOnDate = 2,
    EndOfBillingPeriod = 3,
    Manual = 4,
    External = 5,
    ShadowOnly = 6
}
```

Required demo example:

```text
Travel package total: $5,000
Deposit required now: $1,000
Booking/service fee: $150
Remaining supplier balance: $4,000
Balance due date: 30 days before departure

Immediate charge:
- $1,000 supplier deposit
- $150 service fee

Future charge:
- $4,000 supplier balance
```

---

## Internal Invoice and Ledger

Stripe or any other provider must never be the source of truth.

Cinturon360 mock must create internal invoices and ledger records first.

```csharp
public sealed class InternalInvoice
{
    public Guid Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public Guid SellerOrganisationId { get; set; }
    public Guid BuyerOrganisationId { get; set; }
    public Guid? BookingId { get; set; }
    public InvoiceStatus Status { get; set; }
    public InvoiceCollectionMode CollectionMode { get; set; }
    public PaymentProviderType PaymentProviderType { get; set; }
    public DateOnly IssueDate { get; set; }
    public DateOnly? DueDate { get; set; }
    public string CurrencyCode { get; set; } = "AUD";
    public decimal SubtotalAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public string? FakeExternalInvoiceId { get; set; }
}
```

```csharp
public sealed class InternalInvoiceLine
{
    public Guid Id { get; set; }
    public Guid InternalInvoiceId { get; set; }
    public InvoiceLineType LineType { get; set; }
    public string Description { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal LineTotalAmount { get; set; }
}
```

```csharp
public enum InvoiceStatus
{
    Draft = 1,
    Issued = 2,
    PaymentPending = 3,
    PartiallyPaid = 4,
    Paid = 5,
    Failed = 6,
    Disputed = 7,
    ExternallyManaged = 8,
    Cancelled = 9,
    ShadowOnly = 10
}
```

```csharp
public enum InvoiceCollectionMode
{
    AutomaticProviderCollection = 1,
    ManualCollection = 2,
    ExternalFinanceSystem = 3,
    ShadowOnly = 4,
    NoCharge = 5
}
```

```csharp
public enum InvoiceLineType
{
    SupplierFare = 1,
    SupplierDeposit = 2,
    SupplierBalance = 3,
    Tax = 4,
    BookingFee = 5,
    AmendmentFee = 6,
    CancellationFee = 7,
    RefundProcessingFee = 8,
    PolicyExceptionFee = 9,
    AfterHoursFee = 10,
    CardSurcharge = 11,
    PlatformUsageFee = 12,
    ReportingModuleFee = 13,
    GdsNdcTransactionFee = 14,
    CreditAdjustment = 15,
    AdmRecharge = 16,
    AcmCredit = 17
}
```

### Ledger

```csharp
public sealed class LedgerEntry
{
    public Guid Id { get; set; }
    public DateTimeOffset CreatedAtUtc { get; set; }
    public Guid SellerOrganisationId { get; set; }
    public Guid BuyerOrganisationId { get; set; }
    public Guid? BookingId { get; set; }
    public Guid? InternalInvoiceId { get; set; }
    public LedgerEntryType EntryType { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal DebitAmount { get; set; }
    public decimal CreditAmount { get; set; }
    public string CurrencyCode { get; set; } = "AUD";
    public string? ExternalReference { get; set; }
}
```

```csharp
public enum LedgerEntryType
{
    InvoiceRaised = 1,
    PaymentAuthorised = 2,
    PaymentCaptured = 3,
    PaymentFailed = 4,
    ManualPaymentRecorded = 5,
    ExternalSettlementMarked = 6,
    RefundIssued = 7,
    CreditApplied = 8,
    ServiceFeeRecognised = 9,
    SupplierPayableRecognised = 10,
    AdmRaised = 11,
    AcmRaised = 12,
    AdmAbsorbed = 13,
    AdmRecharged = 14,
    AdmDisputed = 15
}
```

---

## Mock Payment Orchestration

Implement an internal simulation service.

```csharp
public interface IPaymentOrchestrationService
{
    Task<PaymentSimulationResult> ExecuteInvoiceCollectionAsync(Guid internalInvoiceId, CancellationToken cancellationToken);
}
```

The service must:

1. Read the internal invoice.
2. Read seller/buyer/provider/payment method details.
3. Decide payment behaviour based on billing relationship and travel policy.
4. Create fake external provider IDs.
5. Update invoice status.
6. Write ledger entries.
7. Write audit events.

Fake behaviour:

```text
Payment method ending 8431 -> succeeds
Payment method ending 4444 -> succeeds
Payment method ending 1111 -> fails first attempt, succeeds on retry
Manual invoice -> no provider charge, status PaymentPending or Issued
External settlement -> status ExternallyManaged
Shadow billing -> status ShadowOnly
No charge -> status Paid with zero payable provider collection
```

Fake provider IDs:

```text
Stripe invoice: in_mock_000001
Stripe payment intent: pi_mock_000001
Airwallex payment: awx_mock_000001
Braintree transaction: bt_mock_000001
PayPal order: pp_mock_000001
Square payment: sq_mock_000001
```

Required UI labels:

```text
Provider action simulated only
No external payment was made
Cinturon360 internal invoice remains the source of truth
```

---

## Mock GDS / Content Import

Create an import screen where the user can select one of the JSON files from `mock-data/gds` and import it.

The importer must:

- Display raw source file summary.
- Parse booking reference/PNR.
- Create or update booking.
- Match traveller.
- Match client organisation.
- Match TMC.
- Match policy where possible.
- Match cost centre where possible.
- Calculate supplier amount, tax, fees and total.
- Flag incomplete data.
- Create remediation tasks for missing fields.

### Missing data checks

Flag booking as `NeedsReview` if any of these are missing:

- Client organisation
- Traveller
- Travel policy
- Cost centre
- Supplier amount
- Tax amount
- Departure date
- Ticket/document number where expected

### Fake PNRs

Seed/import these fake PNRs:

```text
AVN123   Complete APAC booking, immediate payment, succeeds
AVN124   Missing cost centre, needs review
AVN125   Missing travel policy, needs review
AVN126   Deposit + future balance booking
AVN127   EMEA booking using Visa 1111, payment fails first attempt
AVN128   Executive policy, manual invoice
AVN129   External settlement booking
AVN130   Shadow billing/trial booking
AVN131   Refund/reissue booking
AVN132   Hotel/offline booking
```

### Example GDS JSON shape

```json
{
  "sourceSystem": "MockAmadeus",
  "pnrCode": "AVN123",
  "bookingReference": "BKG-AVN123",
  "ticketNumber": "0810000000123",
  "documentType": "ETKT",
  "clientOrganisationCode": "COCACOLA",
  "tmcOrganisationCode": "HELLOWORLD",
  "traveller": {
    "externalTravellerId": "TRV-CCA-001",
    "firstName": "Jordan",
    "lastName": "Nguyen",
    "email": "jordan.nguyen@example.test"
  },
  "travelPolicyCode": "CCA-APAC",
  "costCentreCode": "APAC-SALES",
  "itinerary": [
    {
      "segmentType": "Flight",
      "carrier": "QF",
      "flightNumber": "401",
      "from": "SYD",
      "to": "MEL",
      "departureLocal": "2026-06-10T09:00:00",
      "arrivalLocal": "2026-06-10T10:35:00"
    }
  ],
  "amounts": {
    "currencyCode": "AUD",
    "supplierAmount": 420.00,
    "taxAmount": 58.20,
    "serviceFeeAmount": 45.00,
    "surchargeAmount": 7.85,
    "totalAmount": 531.05
  },
  "paymentSchedule": [
    {
      "itemType": "SupplierFare",
      "paymentTiming": "Immediate",
      "description": "Domestic flight SYD/MEL",
      "amount": 420.00,
      "dueDate": null
    },
    {
      "itemType": "Tax",
      "paymentTiming": "Immediate",
      "description": "Taxes and charges",
      "amount": 58.20,
      "dueDate": null
    },
    {
      "itemType": "ServiceFee",
      "paymentTiming": "Immediate",
      "description": "TMC booking fee",
      "amount": 45.00,
      "dueDate": null
    },
    {
      "itemType": "CardSurcharge",
      "paymentTiming": "Immediate",
      "description": "Card surcharge",
      "amount": 7.85,
      "dueDate": null
    }
  ]
}
```

---

## ADM / ACM Handling

Create ADM/ACM import and management screens.

ADM = Agency Debit Memo.
ACM = Agency Credit Memo.

The mock should demonstrate that these are linked to booking finance and reconciliation, not treated as isolated accounting records.

```csharp
public sealed class AgencyMemo
{
    public Guid Id { get; set; }
    public AgencyMemoType MemoType { get; set; }
    public string MemoNumber { get; set; } = string.Empty;
    public string AirlineCode { get; set; } = string.Empty;
    public string? TicketNumber { get; set; }
    public string? EmdNumber { get; set; }
    public Guid? BookingId { get; set; }
    public Guid TmcOrganisationId { get; set; }
    public Guid? ClientOrganisationId { get; set; }
    public string ReasonCode { get; set; } = string.Empty;
    public string ReasonDescription { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string CurrencyCode { get; set; } = "AUD";
    public AgencyMemoStatus Status { get; set; }
    public AgencyMemoOutcome? Outcome { get; set; }
}
```

```csharp
public enum AgencyMemoType
{
    Adm = 1,
    Acm = 2
}
```

```csharp
public enum AgencyMemoStatus
{
    Imported = 1,
    Matched = 2,
    Unmatched = 3,
    UnderReview = 4,
    Disputed = 5,
    Settled = 6,
    Closed = 7
}
```

```csharp
public enum AgencyMemoOutcome
{
    AbsorbedByTmc = 1,
    RechargedToClient = 2,
    DisputedWithAirline = 3,
    CreditedToClient = 4,
    WrittenOff = 5
}
```

Required ADM/ACM demo flows:

1. Import ADM matched to ticket `0810000000123` / PNR `AVN123`.
2. Show original booking, traveller, policy, invoice and payment method.
3. Select outcome `RechargedToClient`.
4. Generate an internal invoice line `ADM recharge`.
5. Write ledger entries.
6. Simulate provider/payment handling based on Coca-Cola APAC policy.
7. Import ADM with unknown ticket number.
8. Show it as unmatched and requiring review.
9. Import ACM matched to refunded/reissued booking.
10. Show credit outcome and ledger credit.

---

## BSP / Reconciliation Simulation

Create simple CSV imports under `mock-data/bsp`.

### bsp-ticket-reconciliation-sample.csv

Columns:

```text
BspMarket,PeriodStart,PeriodEnd,AirlineCode,TicketNumber,PnrCode,DocumentType,PassengerName,GrossFare,TaxAmount,CommissionAmount,NetRemitAmount,CurrencyCode,Status
```

### bsp-adm-acm-sample.csv

Columns:

```text
BspMarket,PeriodStart,PeriodEnd,MemoType,MemoNumber,AirlineCode,TicketNumber,EmdNumber,PnrCode,ReasonCode,ReasonDescription,Amount,CurrencyCode,IssueDate,DisputeByDate,Status
```

The reconciliation screen must show:

- Matched tickets
- Unmatched tickets
- Matched ADMs/ACMs
- Unmatched ADMs/ACMs
- Gross fare differences
- Tax differences
- Commission differences
- Net remit differences
- Items requiring finance review

No real BSPLink formats are required for the first demo. Use simplified CSV that approximates the type of financial data the platform would ingest.

---

## Finance Export Simulation

Create a finance export screen where the user can export invoices/ledger records to one of these fake targets:

- Xero CSV
- NetSuite CSV
- Generic ledger CSV
- External API payload preview

Required export fields:

```text
ExportBatchId
ExportTarget
SellerOrganisation
BuyerOrganisation
InvoiceNumber
InvoiceDate
DueDate
BookingReference
PnrCode
TravellerName
CostCentreCode
PolicyCode
LineType
LineDescription
DebitAmount
CreditAmount
TaxAmount
CurrencyCode
ExternalReference
Status
```

Export action should:

1. Create an export batch.
2. Mark selected invoices/ledger entries as exported.
3. Store fake export destination/reference.
4. Write audit events.
5. Allow download of generated CSV from the web UI.

---

## Required Screens

### 1. Login

Simple email/password login.

### 2. Dashboard

Cards:

- Bookings imported
- Bookings needing review
- Invoices issued
- Payments pending
- Failed payments
- Deposits due
- Future balances due
- ADM exposure
- ACM credits
- Manual invoices
- External settlement items
- Shadow billing total

### 3. Organisation hierarchy

Tree view:

```text
Avanōa Technology
└── HelloWorld Travel
    └── Coca-Cola
```

Selecting an organisation shows:

- Organisation type
- Parent organisation
- Billing owner/seller for parent-child relationship
- Billing mode
- Payment provider routing
- Commercial risk owner

### 4. Billing relationships

Show:

- Seller
- Buyer
- Billing mode
- Payment provider
- Terms
- Risk owner
- Configured customer/profile IDs

### 5. Payment methods

Show safe metadata only:

```text
American Express ending 8431, expires 03/2044
Mastercard ending 4444, expires 12/2033
Visa ending 1111, expires 08/2032
```

Show policy mapping:

```text
CCA-APAC -> Mastercard 4444
CCA-EMEA -> Visa 1111
CCA-NA -> Organisation default Amex 8431
CCA-EXEC -> Manual invoice
CCA-TRIAL -> Shadow billing
```

### 6. Travel policies

Show each policy and billing behaviour.

### 7. GDS/content import

Actions:

- Select mock import file
- Preview raw summary
- Import
- Show validation result
- Show created booking
- Show missing data warnings
- Show created payment schedule

### 8. Booking list

Filters:

- Status
- Client
- TMC
- Policy
- Payment status
- Needs review
- PNR

Columns:

- Booking reference
- PNR
- Traveller
- Client
- Policy
- Cost centre
- Supplier amount
- Service fee
- Tax
- Total
- Status

### 9. Booking detail

Sections:

- Booking summary
- Traveller
- Organisation path
- Policy
- Cost centre
- Itinerary
- Amounts
- Payment schedule
- Invoice links
- Payment attempts
- Ledger entries
- ADM/ACM links
- Audit trail

### 10. Invoice list

Columns:

- Invoice number
- Seller
- Buyer
- Booking
- Collection mode
- Provider
- Status
- Due date
- Total

### 11. Invoice detail

Sections:

- Header
- Lines
- Internal ledger
- Provider simulation
- Payment attempts
- Audit trail

Actions:

- Simulate collect payment
- Retry failed payment
- Mark manual payment received
- Mark externally settled
- Export to finance system

### 12. Payment operations

Show fake provider events:

- Succeeded
- Failed
- Retry succeeded
- Manual recorded
- External marked
- Shadow only

### 13. ADM/ACM import

Actions:

- Import BSP ADM/ACM CSV
- Match by ticket number/PNR
- Show matched/unmatched records

### 14. ADM/ACM detail

Sections:

- Memo details
- Matched booking/ticket
- Original invoice
- Original payment method
- Reason code
- Recommended action
- Outcome selection

Actions:

- Absorb by TMC
- Recharge to client
- Dispute with airline
- Credit to client
- Write off

### 15. Reports

Create simple report pages:

- Outstanding balances
- Deposits due
- Future balances due
- Failed payments
- Service fees
- ADM/ACM exposure
- Supplier reconciliation
- Client usage
- TMC usage
- Vendor usage
- Shadow billing report

### 16. Finance exports

Actions:

- Select target
- Preview export rows
- Generate CSV
- Download CSV

---

## API Endpoints

Create a clean mock API. Suggested endpoints:

```text
POST   /api/auth/login
POST   /api/auth/logout
GET    /api/dashboard
GET    /api/organisations/tree
GET    /api/organisations/{id}
GET    /api/billing-relationships
GET    /api/payment-methods
GET    /api/travel-policies
GET    /api/import-files/gds
POST   /api/imports/gds/{fileName}
GET    /api/bookings
GET    /api/bookings/{id}
POST   /api/bookings/{id}/mark-ready-for-billing
POST   /api/bookings/{id}/generate-invoice
GET    /api/invoices
GET    /api/invoices/{id}
POST   /api/invoices/{id}/simulate-collection
POST   /api/invoices/{id}/retry-payment
POST   /api/invoices/{id}/mark-manual-payment-received
POST   /api/invoices/{id}/mark-externally-settled
GET    /api/agency-memos
GET    /api/agency-memos/{id}
POST   /api/imports/bsp/agency-memos
POST   /api/agency-memos/{id}/set-outcome
GET    /api/reports/outstanding-balances
GET    /api/reports/deposits-due
GET    /api/reports/future-balances
GET    /api/reports/failed-payments
GET    /api/reports/service-fees
GET    /api/reports/adm-acm-exposure
GET    /api/reports/supplier-reconciliation
POST   /api/finance-exports
GET    /api/finance-exports/{id}/download
```

---

## Seed Data Requirements

Seed organisations:

```text
Avanōa Technology | Vendor | AVANOA
HelloWorld Travel | TMC    | HELLOWORLD
Coca-Cola         | Client | COCACOLA
```

Seed users:

```text
demo@avanoa.local | DemoPassword123! | PlatformAdmin | Avanōa Technology
finance@cocacola.local | DemoPassword123! | ClientFinance | Coca-Cola
ops@helloworld.local | DemoPassword123! | TmcOps | HelloWorld Travel
```

Seed travellers:

```text
Jordan Nguyen     | jordan.nguyen@example.test     | Coca-Cola | APAC-SALES
Avery Williams    | avery.williams@example.test    | Coca-Cola | EMEA-MARKETING
Morgan Patel      | morgan.patel@example.test      | Coca-Cola | NA-FINANCE
Taylor Chen       | taylor.chen@example.test       | Coca-Cola | EXEC
```

Seed cost centres:

```text
APAC-SALES
EMEA-MARKETING
NA-FINANCE
EXEC
UNASSIGNED
```

Seed suppliers:

```text
Qantas
Singapore Airlines
British Airways
Hilton
Mock Offline Supplier
```

Seed service fees:

```text
Domestic booking fee: AUD 45.00
International booking fee: AUD 95.00
Amendment fee: AUD 65.00
Cancellation fee: AUD 75.00
Refund processing fee: AUD 55.00
Policy exception fee: AUD 120.00
After-hours service fee: AUD 150.00
GDS/NDC transaction fee: AUD 3.50
Reporting module fee: AUD 99.00 monthly
```

---

## Required Scenarios

### Scenario 1 — Complete APAC booking, immediate payment succeeds

Input file: `pnr-import-complete.json`

Expected result:

- Booking `AVN123` imported.
- Policy `CCA-APAC` matched.
- Payment method Mastercard 4444 selected.
- Invoice generated by HelloWorld Travel to Coca-Cola.
- Payment provider simulated as Stripe.
- Payment succeeds.
- Ledger shows invoice raised and payment captured.

### Scenario 2 — Missing cost centre, booking needs review

Input file: `pnr-import-missing-cost-centre.json`

Expected result:

- Booking `AVN124` imported.
- Booking status `NeedsReview`.
- Missing data warning: cost centre.
- No invoice generated until remediated.
- User can set cost centre to `APAC-SALES`.
- Booking can then be marked ready for billing.

### Scenario 3 — Missing policy, booking needs review

Input file: `pnr-import-missing-policy.json`

Expected result:

- Booking `AVN125` imported.
- Booking status `NeedsReview`.
- Missing data warning: travel policy.
- User can assign `CCA-NA`.
- Default organisation payment method Amex 8431 is used.

### Scenario 4 — Deposit now and supplier balance later

Input file: `pnr-import-deposit-balance.json`

Expected result:

- Booking `AVN126` imported.
- Payment schedule created:
  - Deposit AUD 1,000 immediate
  - Service fee AUD 150 immediate
  - Supplier balance AUD 4,000 due 30 days before departure
- Immediate invoice total AUD 1,150.
- Future balance is visible in future balances report.

### Scenario 5 — Payment fails and retry succeeds

Input file: `pnr-import-payment-failure.json`

Expected result:

- Booking `AVN127` imported.
- Policy `CCA-EMEA` uses Visa 1111.
- First payment attempt fails.
- Invoice status `Failed`.
- Failed payment appears in report.
- Retry action succeeds.
- Ledger records failed attempt and successful capture.

### Scenario 6 — Executive policy manual invoice

Input file: `pnr-import-manual-invoice.json`

Expected result:

- Booking `AVN128` imported.
- Policy `CCA-EXEC` uses manual invoice.
- Invoice generated with collection mode `ManualCollection`.
- No provider charge simulated.
- User can mark manual payment received.

### Scenario 7 — External settlement

Input file: `pnr-import-external-settlement.json`

Expected result:

- Booking `AVN129` imported.
- Collection mode `ExternalFinanceSystem`.
- Invoice status `ExternallyManaged`.
- User can export to finance system.
- User can mark externally settled.

### Scenario 8 — Shadow billing/trial

Input file: `pnr-import-shadow-billing.json`

Expected result:

- Booking `AVN130` imported.
- Invoice generated with status `ShadowOnly`.
- No payment collection.
- Appears in shadow billing report.

### Scenario 9 — Refund/reissue booking

Input file: `pnr-import-refund-reissue.json`

Expected result:

- Booking `AVN131` imported as refund/reissue.
- Ledger includes adjustment/refund line.
- ACM can be matched against this booking later.

### Scenario 10 — ADM recharge to client

Input file: `bsp-adm-acm-sample.csv`

Expected result:

- ADM matched to ticket from booking `AVN123`.
- User chooses `RechargedToClient`.
- New invoice line created for ADM recharge.
- Ledger records ADM raised and ADM recharged.

### Scenario 11 — Unmatched ADM

Input file: `bsp-adm-acm-sample.csv`

Expected result:

- ADM with unknown ticket number is imported.
- Status `Unmatched`.
- Appears in ADM/ACM exposure report.

### Scenario 12 — ACM credit

Input file: `bsp-adm-acm-sample.csv`

Expected result:

- ACM matched to refund/reissue booking `AVN131`.
- User chooses credit to client.
- Ledger credit entry created.

---

## UI Demonstration Script

Create `docs/demo-script.md` in the generated repo with this script:

```text
1. Open http://localhost:5089.
2. Log in as demo@avanoa.local / DemoPassword123!.
3. Open Dashboard and show the operating/finance overview.
4. Open Organisation Hierarchy and show Avanōa > HelloWorld > Coca-Cola.
5. Open Billing Relationships and explain that HelloWorld is seller/billing owner for Coca-Cola.
6. Open Payment Methods and show card metadata only.
7. Open Travel Policies and show APAC/EMEA/NA/EXEC/TRIAL billing behaviours.
8. Import complete APAC PNR AVN123.
9. Open booking detail and show imported itinerary, policy, cost centre, amounts and schedule.
10. Generate invoice.
11. Simulate payment collection and show payment success.
12. Show invoice lines, ledger entries and audit trail.
13. Import missing cost centre PNR AVN124 and show Needs Review.
14. Remediate missing cost centre and mark ready for billing.
15. Import deposit/balance PNR AVN126 and show future balance report.
16. Import EMEA failed payment PNR AVN127, simulate failure, then retry.
17. Import manual invoice PNR AVN128 and mark manual payment received.
18. Import external settlement PNR AVN129 and export to finance system.
19. Import shadow billing PNR AVN130 and show shadow report.
20. Import ADM/ACM CSV.
21. Open matched ADM, select Recharge to Client, and show new invoice/ledger entries.
22. Open unmatched ADM and show review state.
23. Open ACM and show credit handling.
24. Open reports and show financial control overview.
25. Close by explaining that provider integrations are execution/export channels only; Cinturon360 owns billing logic, ledger and audit trail.
```

---

## Audit Trail

Every meaningful action must create an audit event.

```csharp
public sealed class AuditEvent
{
    public Guid Id { get; set; }
    public DateTimeOffset CreatedAtUtc { get; set; }
    public string ActorEmail { get; set; } = string.Empty;
    public Guid? OrganisationId { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public Guid? EntityId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string? JsonDetails { get; set; }
}
```

Examples:

```text
GDS import completed
Booking marked needs review
Cost centre remediated
Invoice generated
Payment simulated
Payment failed
Payment retry simulated
Manual payment recorded
Invoice exported
ADM imported
ADM matched
ADM outcome selected
ADM recharge invoice generated
ACM credit applied
```

---

## Acceptance Criteria

The generated mock system is acceptable when:

- It runs locally with Docker Compose.
- The user can log in.
- The seeded hierarchy is visible.
- Billing relationships are visible.
- Policy/payment method routing is visible.
- Mock GDS imports work.
- Missing booking data creates review states.
- Internal invoices are generated before payment simulation.
- Ledger entries are created.
- Payment success/failure/manual/external/shadow scenarios work.
- Deposit/future balance schedules are visible.
- ADM/ACM CSV import works.
- ADM/ACM items can be matched, reviewed and actioned.
- Reports show seeded/generated data.
- Finance export generates downloadable CSV.
- The UI clearly labels all external-provider behaviour as simulated.
- No live external systems are called.

---

## AI Agent Implementation Instructions

When building this project:

1. Prioritise a working demo over architectural perfection.
2. Keep code simple and readable.
3. Use deterministic seeded data.
4. Avoid complex abstractions unless they directly help the demo.
5. Use EF Core migrations or SQL init scripts; either is acceptable for the mock.
6. Prefer one WebAPI project, one Blazor Server project and one Shared contracts project.
7. Keep all provider integrations fake.
8. Store fake external IDs as strings.
9. Use PostgreSQL for persistence.
10. Ensure the whole demo can be reset by deleting the Docker volume.
11. Include clear README commands.
12. Build the UI so the scenario flow is obvious to non-engineering stakeholders.
13. Add labels/tooltips explaining what each scenario demonstrates.
14. Ensure all sample imports are included in the repo.
15. Ensure all generated CSV exports are downloadable from the UI.

Recommended commands:

```bash
docker compose up -d --build
```

```bash
docker compose down -v
```

```bash
docker compose logs -f api web
```

---

## README Commands for Generated Project

The generated project README must include:

```bash
# Start demo
cd cinturon360-midoffice-mockup
docker compose up -d --build

# Open UI
open http://localhost:5089

# Login
# Email: demo@avanoa.local
# Password: DemoPassword123!

# Stop demo
cd cinturon360-midoffice-mockup
docker compose down

# Full reset
cd cinturon360-midoffice-mockup
docker compose down -v
docker compose up -d --build
```

---

## Key Message the Demo Must Communicate

This demo exists to prove that Cinturon360 should act as the operational and financial control layer for TMCs.

The platform should own:

- hierarchy-aware billing ownership;
- booking finance;
- payment schedules;
- service fee calculation;
- internal invoices;
- internal ledger;
- payment routing decisions;
- policy-specific billing rules;
- ADM/ACM matching and outcomes;
- audit trail;
- reporting;
- finance export data.

Payment providers and accounting systems should remain downstream execution/export channels.

