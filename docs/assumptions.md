# Assumptions

## Architecture

- BCrypt cost factor 11 is used for demo password hashing. In production, use a higher cost factor and a proper identity provider.
- The Blazor Web App uses `InteractiveServer` render mode globally for simplicity. In production, render mode would be scoped per component.
- Session state (`SessionState`) is a Scoped service, not a proper auth principal. The demo does not use ASP.NET Core `ClaimsPrincipal` for authorisation — it uses a simple flag on `SessionState`. A production system would use claims-based identity.
- The `/logout` GET endpoint is a simplification; production would use POST with CSRF protection.

## Billing Logic

- Surcharge is calculated as 1.5% of (fare + tax) for immediate card payment scenarios. This is arbitrary for demo purposes.
- "Deposit now, balance later" uses a hardcoded 30-day-before-departure balance due date unless specified in the PNR file.
- Service fees are taken directly from the GDS import file rather than from a rules engine.

## Payment Simulation

- Visa ending 1111 fails on the first payment attempt and succeeds on retry. This is hardcoded in `PaymentSimulationService`.
- All other cards succeed immediately.
- Fake provider IDs use a global incrementing counter seeded from 1 per API instance restart.

## Data

- All traveller emails use `@example.test` domain to clearly indicate fake data.
- Fixed UUIDs are used for seeded organisations, users, policies and payment methods to make the demo fully reproducible.
- Existing PNR files use PNR codes `C360A1`, `C360B2`, `C360C3` rather than the `AVN1xx` codes in the README. Both naming conventions work; the system matches by file content.

## Docker

- The API copies the `mock-data/` directory into the container image at build time. Changes to mock-data files after image build require a rebuild.
- PostgreSQL runs on host port 54329 to avoid conflicts with any existing local PostgreSQL on 5432.
- No TLS is configured for the demo. Production deployments would require HTTPS termination.

## Scope Exclusions

- Real GDS/NDC integration: not built. Import is from local JSON files only.
- Real BSP/ACM file format: not implemented. Demo uses simplified CSV.
- Real finance system export: not implemented. Demo generates CSV for download.
- Trust accounting: not implemented. The ledger is a simplified double-entry-style record, not a full trust accounting system.
- Multi-currency FX: not implemented. All amounts assumed to be AUD.
- Refund workflows: partial — booking can be marked refunded via status, but the full refund processing flow is not automated.
