# API Endpoints

Base URL: `http://localhost:5088`

Swagger UI: `http://localhost:5088/swagger`

## Auth
| Method | Path | Description |
|--------|------|-------------|
| POST | /api/auth/login | Login with email/password |

## Dashboard
| Method | Path | Description |
|--------|------|-------------|
| GET | /api/dashboard | Summary statistics |

## Organisations
| Method | Path | Description |
|--------|------|-------------|
| GET | /api/organisations/tree | Full hierarchy tree |
| GET | /api/organisations/{id} | Organisation detail |

## Billing
| Method | Path | Description |
|--------|------|-------------|
| GET | /api/billing-relationships | All billing relationships |
| GET | /api/payment-methods | Saved payment methods |
| GET | /api/travel-policies | Travel policies |

## Imports
| Method | Path | Description |
|--------|------|-------------|
| GET | /api/import-files/gds | List available GDS files |
| POST | /api/imports/gds/{fileName} | Import a GDS PNR file |
| POST | /api/agency-memos/import-csv | Import BSP ADM/ACM CSV |

## Bookings
| Method | Path | Description |
|--------|------|-------------|
| GET | /api/bookings | List bookings (filter: status, pnr) |
| GET | /api/bookings/{id} | Booking detail with segments |
| POST | /api/bookings/{id}/mark-ready-for-billing | Mark ready |
| POST | /api/bookings/{id}/generate-invoice | Generate invoice |
| POST | /api/bookings/{id}/set-cost-centre | Assign cost centre |
| POST | /api/bookings/{id}/set-travel-policy | Assign travel policy |

## Invoices
| Method | Path | Description |
|--------|------|-------------|
| GET | /api/invoices | List invoices |
| GET | /api/invoices/{id} | Invoice detail with lines |
| POST | /api/invoices/{id}/simulate-collection | Simulate payment collection |
| POST | /api/invoices/{id}/retry-payment | Retry failed payment |
| POST | /api/invoices/{id}/mark-manual-payment-received | Record manual payment |
| POST | /api/invoices/{id}/mark-externally-settled | Mark externally settled |

## ADM/ACM
| Method | Path | Description |
|--------|------|-------------|
| GET | /api/agency-memos | List all memos |
| GET | /api/agency-memos/{id} | Memo detail |
| POST | /api/agency-memos/{id}/set-outcome | Set ADM/ACM outcome |

## Ledger & Audit
| Method | Path | Description |
|--------|------|-------------|
| GET | /api/ledger | Ledger entries (filter: bookingId, invoiceId) |
| GET | /api/audit | Audit events (filter: entityType, entityId) |

## Reports
| Method | Path | Description |
|--------|------|-------------|
| GET | /api/reports/outstanding-balances | Unpaid/pending invoices |
| GET | /api/reports/future-balances | Future balance schedule items |
| GET | /api/reports/failed-payments | Failed payment invoices |
| GET | /api/reports/service-fees | Service fee invoice lines |
| GET | /api/reports/adm-acm-exposure | All ADM/ACM records |
| GET | /api/reports/shadow-billing | Shadow-only invoices |
| GET | /api/reports/supplier-reconciliation | Booking/ticket reconciliation |

## Finance Exports
| Method | Path | Description |
|--------|------|-------------|
| GET | /api/finance-exports | List export batches |
| POST | /api/finance-exports | Create export batch |
| GET | /api/finance-exports/{id}/download | Download CSV |
