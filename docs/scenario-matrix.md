# Scenario Matrix

| # | Name | PNR File | Policy | Payment Method | Expected Outcome |
|---|------|----------|--------|----------------|-----------------|
| 1 | Complete APAC booking, immediate success | pnr-import-complete.json | CCA-APAC | Mastercard 4444 | Import → Invoice → Payment succeeds |
| 2 | Missing cost centre | pnr-import-missing-cost-centre.json | CCA-EMEA | Visa 1111 | Import → NeedsReview → Assign cost centre → Ready → Invoice |
| 3 | Deposit now, balance later | pnr-import-package-deposit.json | CCA-NA | Amex 8431 | Import → Payment schedule with deposit + future balance |
| 4 | Payment fails, retry succeeds | pnr-import-payment-failure.json | CCA-EMEA | Visa 1111 | Import → Invoice → Payment fails → Retry → Payment succeeds |
| 5 | Manual invoice | pnr-import-manual-invoice.json | CCA-EXEC | Manual Invoice | Import → Invoice (ManualCollection) → Mark manual payment received |
| 6 | External settlement | pnr-import-external-settlement.json | CCA-NA | External | Import → Invoice (ExternalFinanceSystem) → Export → Mark settled |
| 7 | Shadow billing | pnr-import-shadow-billing.json | CCA-TRIAL | Shadow | Import → Invoice (ShadowOnly) → Appears in shadow report |
| 8 | Refund/reissue | pnr-import-refund-reissue.json | CCA-EMEA | Visa 1111 | Import → Booking with refund note → ACM can be matched |
| 9 | ADM recharge to client | bsp-adm-acm-sample.csv | — | — | Import ADM → Match to booking → Select RechargedToClient → New invoice line |
| 10 | Unmatched ADM | bsp-adm-acm-sample.csv | — | — | Import ADM with unknown ticket → Unmatched status → Review queue |
| 11 | ACM credit | bsp-adm-acm-sample.csv | — | — | Import ACM → Match to booking → Select CreditedToClient → Ledger credit |
