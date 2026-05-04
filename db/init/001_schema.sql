-- Cinturon360 Mock Database Schema

CREATE TABLE IF NOT EXISTS organisations (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name TEXT NOT NULL,
    code TEXT NOT NULL UNIQUE,
    type INTEGER NOT NULL,
    parent_organisation_id UUID REFERENCES organisations(id),
    created_at_utc TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE TABLE IF NOT EXISTS users (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    email TEXT NOT NULL UNIQUE,
    password_hash TEXT NOT NULL,
    role INTEGER NOT NULL,
    organisation_id UUID NOT NULL REFERENCES organisations(id),
    created_at_utc TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE TABLE IF NOT EXISTS billing_relationships (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    seller_organisation_id UUID NOT NULL REFERENCES organisations(id),
    buyer_organisation_id UUID NOT NULL REFERENCES organisations(id),
    billing_mode INTEGER NOT NULL DEFAULT 1,
    payment_provider_type INTEGER NOT NULL DEFAULT 1,
    currency_code TEXT NOT NULL DEFAULT 'AUD',
    payment_terms_days INTEGER NOT NULL DEFAULT 30,
    is_commercial_risk_held_by_seller BOOLEAN NOT NULL DEFAULT TRUE,
    created_at_utc TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE TABLE IF NOT EXISTS payment_provider_accounts (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    seller_organisation_id UUID NOT NULL REFERENCES organisations(id),
    provider_type INTEGER NOT NULL,
    display_name TEXT NOT NULL,
    fake_external_account_id TEXT NOT NULL,
    is_primary BOOLEAN NOT NULL DEFAULT FALSE,
    is_enabled BOOLEAN NOT NULL DEFAULT TRUE,
    created_at_utc TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE TABLE IF NOT EXISTS buyer_payment_profiles (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    seller_organisation_id UUID NOT NULL REFERENCES organisations(id),
    buyer_organisation_id UUID NOT NULL REFERENCES organisations(id),
    payment_provider_account_id UUID NOT NULL REFERENCES payment_provider_accounts(id),
    fake_provider_customer_id TEXT NOT NULL,
    created_at_utc TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE TABLE IF NOT EXISTS saved_payment_methods (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    buyer_payment_profile_id UUID NOT NULL REFERENCES buyer_payment_profiles(id),
    fake_provider_payment_method_id TEXT NOT NULL,
    brand TEXT NOT NULL,
    last4 TEXT NOT NULL,
    expiry_month INTEGER NOT NULL,
    expiry_year INTEGER NOT NULL,
    is_default BOOLEAN NOT NULL DEFAULT FALSE,
    created_at_utc TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE TABLE IF NOT EXISTS travel_policies (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    client_organisation_id UUID NOT NULL REFERENCES organisations(id),
    code TEXT NOT NULL UNIQUE,
    name TEXT NOT NULL,
    billing_behaviour INTEGER NOT NULL DEFAULT 1,
    saved_payment_method_id UUID REFERENCES saved_payment_methods(id),
    created_at_utc TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE TABLE IF NOT EXISTS travellers (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    client_organisation_id UUID NOT NULL REFERENCES organisations(id),
    external_traveller_id TEXT,
    first_name TEXT NOT NULL,
    last_name TEXT NOT NULL,
    email TEXT NOT NULL,
    default_cost_centre_code TEXT,
    created_at_utc TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE TABLE IF NOT EXISTS bookings (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    booking_reference TEXT NOT NULL UNIQUE,
    pnr_code TEXT,
    ticket_number TEXT,
    traveller_id UUID NOT NULL REFERENCES travellers(id),
    client_organisation_id UUID NOT NULL REFERENCES organisations(id),
    tmc_organisation_id UUID NOT NULL REFERENCES organisations(id),
    vendor_organisation_id UUID NOT NULL REFERENCES organisations(id),
    travel_policy_id UUID REFERENCES travel_policies(id),
    cost_centre_code TEXT,
    status INTEGER NOT NULL DEFAULT 1,
    departure_date DATE NOT NULL,
    return_date DATE NOT NULL,
    currency_code TEXT NOT NULL DEFAULT 'AUD',
    supplier_amount NUMERIC(18,4) NOT NULL DEFAULT 0,
    tax_amount NUMERIC(18,4) NOT NULL DEFAULT 0,
    service_fee_amount NUMERIC(18,4) NOT NULL DEFAULT 0,
    surcharge_amount NUMERIC(18,4) NOT NULL DEFAULT 0,
    total_amount NUMERIC(18,4) NOT NULL DEFAULT 0,
    source_system TEXT NOT NULL DEFAULT 'MockGDS',
    imported_at_utc TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    created_at_utc TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE TABLE IF NOT EXISTS booking_segments (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    booking_id UUID NOT NULL REFERENCES bookings(id),
    segment_type TEXT NOT NULL,
    carrier TEXT,
    flight_number TEXT,
    from_location TEXT,
    to_location TEXT,
    departure_utc TIMESTAMPTZ,
    arrival_utc TIMESTAMPTZ,
    supplier_name TEXT,
    created_at_utc TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE TABLE IF NOT EXISTS booking_documents (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    booking_id UUID NOT NULL REFERENCES bookings(id),
    document_type TEXT NOT NULL,
    document_number TEXT NOT NULL,
    created_at_utc TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE TABLE IF NOT EXISTS booking_payment_schedules (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    booking_id UUID NOT NULL REFERENCES bookings(id),
    total_scheduled_amount NUMERIC(18,4) NOT NULL,
    currency_code TEXT NOT NULL DEFAULT 'AUD',
    status INTEGER NOT NULL DEFAULT 1,
    created_at_utc TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE TABLE IF NOT EXISTS booking_payment_schedule_items (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    booking_payment_schedule_id UUID NOT NULL REFERENCES booking_payment_schedules(id),
    item_type INTEGER NOT NULL,
    payment_timing INTEGER NOT NULL,
    description TEXT NOT NULL,
    amount NUMERIC(18,4) NOT NULL,
    due_date DATE,
    status INTEGER NOT NULL DEFAULT 1,
    created_at_utc TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE TABLE IF NOT EXISTS internal_invoices (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    invoice_number TEXT NOT NULL UNIQUE,
    seller_organisation_id UUID NOT NULL REFERENCES organisations(id),
    buyer_organisation_id UUID NOT NULL REFERENCES organisations(id),
    booking_id UUID REFERENCES bookings(id),
    status INTEGER NOT NULL DEFAULT 1,
    collection_mode INTEGER NOT NULL DEFAULT 1,
    payment_provider_type INTEGER NOT NULL DEFAULT 1,
    issue_date DATE NOT NULL,
    due_date DATE,
    currency_code TEXT NOT NULL DEFAULT 'AUD',
    subtotal_amount NUMERIC(18,4) NOT NULL,
    tax_amount NUMERIC(18,4) NOT NULL DEFAULT 0,
    total_amount NUMERIC(18,4) NOT NULL,
    fake_external_invoice_id TEXT,
    created_at_utc TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE TABLE IF NOT EXISTS internal_invoice_lines (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    internal_invoice_id UUID NOT NULL REFERENCES internal_invoices(id),
    line_type INTEGER NOT NULL,
    description TEXT NOT NULL,
    quantity INTEGER NOT NULL DEFAULT 1,
    unit_amount NUMERIC(18,4) NOT NULL,
    tax_amount NUMERIC(18,4) NOT NULL DEFAULT 0,
    line_total_amount NUMERIC(18,4) NOT NULL,
    created_at_utc TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE TABLE IF NOT EXISTS ledger_entries (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    created_at_utc TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    seller_organisation_id UUID NOT NULL REFERENCES organisations(id),
    buyer_organisation_id UUID NOT NULL REFERENCES organisations(id),
    booking_id UUID REFERENCES bookings(id),
    internal_invoice_id UUID REFERENCES internal_invoices(id),
    entry_type INTEGER NOT NULL,
    description TEXT NOT NULL,
    debit_amount NUMERIC(18,4) NOT NULL DEFAULT 0,
    credit_amount NUMERIC(18,4) NOT NULL DEFAULT 0,
    currency_code TEXT NOT NULL DEFAULT 'AUD',
    external_reference TEXT
);

CREATE TABLE IF NOT EXISTS payment_attempts (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    internal_invoice_id UUID NOT NULL REFERENCES internal_invoices(id),
    attempted_at_utc TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    provider_type INTEGER NOT NULL,
    fake_provider_payment_id TEXT,
    saved_payment_method_id UUID REFERENCES saved_payment_methods(id),
    succeeded BOOLEAN NOT NULL DEFAULT FALSE,
    failure_reason TEXT,
    amount NUMERIC(18,4) NOT NULL,
    currency_code TEXT NOT NULL DEFAULT 'AUD'
);

CREATE TABLE IF NOT EXISTS agency_memos (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    memo_type INTEGER NOT NULL,
    memo_number TEXT NOT NULL UNIQUE,
    airline_code TEXT NOT NULL,
    ticket_number TEXT,
    emd_number TEXT,
    pnr_code TEXT,
    booking_id UUID REFERENCES bookings(id),
    tmc_organisation_id UUID NOT NULL REFERENCES organisations(id),
    client_organisation_id UUID REFERENCES organisations(id),
    reason_code TEXT NOT NULL,
    reason_description TEXT NOT NULL,
    amount NUMERIC(18,4) NOT NULL,
    currency_code TEXT NOT NULL DEFAULT 'AUD',
    status INTEGER NOT NULL DEFAULT 1,
    outcome INTEGER,
    created_at_utc TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE TABLE IF NOT EXISTS imported_content_batches (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    source_system TEXT NOT NULL,
    file_name TEXT NOT NULL,
    import_type TEXT NOT NULL,
    status TEXT NOT NULL DEFAULT 'Completed',
    record_count INTEGER NOT NULL DEFAULT 0,
    imported_by_email TEXT NOT NULL,
    imported_at_utc TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE TABLE IF NOT EXISTS finance_export_batches (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    export_target TEXT NOT NULL,
    created_at_utc TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    created_by_email TEXT NOT NULL,
    invoice_count INTEGER NOT NULL DEFAULT 0,
    status TEXT NOT NULL DEFAULT 'Completed'
);

CREATE TABLE IF NOT EXISTS finance_export_items (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    finance_export_batch_id UUID NOT NULL REFERENCES finance_export_batches(id),
    internal_invoice_id UUID NOT NULL REFERENCES internal_invoices(id),
    exported_at_utc TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE TABLE IF NOT EXISTS audit_events (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    created_at_utc TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    actor_email TEXT NOT NULL,
    organisation_id UUID,
    entity_type TEXT NOT NULL,
    entity_id UUID,
    action TEXT NOT NULL,
    summary TEXT NOT NULL,
    json_details TEXT
);
