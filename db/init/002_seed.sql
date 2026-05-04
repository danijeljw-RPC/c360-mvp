-- Seed organisations
INSERT INTO organisations (id, name, code, type, parent_organisation_id) VALUES
('a0000001-0000-0000-0000-000000000001', 'Avanōa Technology', 'AVANOA', 1, NULL),
('a0000002-0000-0000-0000-000000000002', 'HelloWorld Travel', 'HELLOWORLD', 2, 'a0000001-0000-0000-0000-000000000001'),
('a0000003-0000-0000-0000-000000000003', 'Coca-Cola', 'COCACOLA', 3, 'a0000002-0000-0000-0000-000000000002')
ON CONFLICT DO NOTHING;

-- Seed users (passwords are bcrypt hash of 'DemoPassword123!')
INSERT INTO users (id, email, password_hash, role, organisation_id) VALUES
('b0000001-0000-0000-0000-000000000001', 'demo@avanoa.local', '$2a$11$J7fzl1TEw.jyrCXCvvOXXeac9eWNmQFmT4zjlXIyT92rKHRVUbmrC', 1, 'a0000001-0000-0000-0000-000000000001'),
('b0000002-0000-0000-0000-000000000002', 'ops@helloworld.local', '$2a$11$J7fzl1TEw.jyrCXCvvOXXeac9eWNmQFmT4zjlXIyT92rKHRVUbmrC', 2, 'a0000002-0000-0000-0000-000000000002'),
('b0000003-0000-0000-0000-000000000003', 'finance@cocacola.local', '$2a$11$J7fzl1TEw.jyrCXCvvOXXeac9eWNmQFmT4zjlXIyT92rKHRVUbmrC', 3, 'a0000003-0000-0000-0000-000000000003')
ON CONFLICT DO NOTHING;

-- Seed billing relationships
-- Avanōa Technology -> HelloWorld Travel
INSERT INTO billing_relationships (id, seller_organisation_id, buyer_organisation_id, billing_mode, payment_provider_type, currency_code, payment_terms_days, is_commercial_risk_held_by_seller) VALUES
('c0000001-0000-0000-0000-000000000001', 'a0000001-0000-0000-0000-000000000001', 'a0000002-0000-0000-0000-000000000002', 1, 1, 'AUD', 30, TRUE),
('c0000002-0000-0000-0000-000000000002', 'a0000002-0000-0000-0000-000000000002', 'a0000003-0000-0000-0000-000000000003', 1, 1, 'AUD', 14, TRUE)
ON CONFLICT DO NOTHING;

-- Seed payment provider accounts
INSERT INTO payment_provider_accounts (id, seller_organisation_id, provider_type, display_name, fake_external_account_id, is_primary, is_enabled) VALUES
('d0000001-0000-0000-0000-000000000001', 'a0000002-0000-0000-0000-000000000002', 1, 'HelloWorld Travel Stripe Account', 'acct_hw_stripe_mock', TRUE, TRUE),
('d0000002-0000-0000-0000-000000000002', 'a0000002-0000-0000-0000-000000000002', 6, 'HelloWorld Travel Manual Invoice', 'manual_hw_invoice', FALSE, TRUE),
('d0000003-0000-0000-0000-000000000003', 'a0000001-0000-0000-0000-000000000001', 1, 'Avanōa Technology Stripe Account', 'acct_avanoa_stripe_mock', TRUE, TRUE),
('d0000004-0000-0000-0000-000000000004', 'a0000001-0000-0000-0000-000000000001', 10, 'Avanōa Technology Shadow Billing', 'shadow_avanoa_mock', FALSE, TRUE)
ON CONFLICT DO NOTHING;

-- Seed buyer payment profile (HelloWorld sells to Coca-Cola via Stripe)
INSERT INTO buyer_payment_profiles (id, seller_organisation_id, buyer_organisation_id, payment_provider_account_id, fake_provider_customer_id) VALUES
('e0000001-0000-0000-0000-000000000001', 'a0000002-0000-0000-0000-000000000002', 'a0000003-0000-0000-0000-000000000003', 'd0000001-0000-0000-0000-000000000001', 'cus_cocacola_hw_mock')
ON CONFLICT DO NOTHING;

-- Seed saved payment methods for Coca-Cola
INSERT INTO saved_payment_methods (id, buyer_payment_profile_id, fake_provider_payment_method_id, brand, last4, expiry_month, expiry_year, is_default) VALUES
('f0000001-0000-0000-0000-000000000001', 'e0000001-0000-0000-0000-000000000001', 'pm_mock_amex_8431', 'American Express', '8431', 3, 2044, TRUE),
('f0000002-0000-0000-0000-000000000002', 'e0000001-0000-0000-0000-000000000001', 'pm_mock_mc_4444', 'Mastercard', '4444', 12, 2033, FALSE),
('f0000003-0000-0000-0000-000000000003', 'e0000001-0000-0000-0000-000000000001', 'pm_mock_visa_1111', 'Visa', '1111', 8, 2030, FALSE)
ON CONFLICT DO NOTHING;

-- Seed travel policies for Coca-Cola
INSERT INTO travel_policies (id, client_organisation_id, code, name, billing_behaviour, saved_payment_method_id) VALUES
('g0000001-0000-0000-0000-000000000001', 'a0000003-0000-0000-0000-000000000003', 'CCA-APAC', 'Coca-Cola APAC Travel Policy', 2, 'f0000002-0000-0000-0000-000000000002'),
('g0000002-0000-0000-0000-000000000002', 'a0000003-0000-0000-0000-000000000003', 'CCA-EMEA', 'Coca-Cola EMEA Travel Policy', 2, 'f0000003-0000-0000-0000-000000000003'),
('g0000003-0000-0000-0000-000000000003', 'a0000003-0000-0000-0000-000000000003', 'CCA-NA', 'Coca-Cola North America Policy', 1, 'f0000001-0000-0000-0000-000000000001'),
('g0000004-0000-0000-0000-000000000004', 'a0000003-0000-0000-0000-000000000003', 'CCA-EXEC', 'Coca-Cola Executive Travel Policy', 3, NULL),
('g0000005-0000-0000-0000-000000000005', 'a0000003-0000-0000-0000-000000000003', 'CCA-TRIAL', 'Coca-Cola Trial/Shadow Policy', 5, NULL)
ON CONFLICT DO NOTHING;

-- Seed travellers for Coca-Cola
INSERT INTO travellers (id, client_organisation_id, external_traveller_id, first_name, last_name, email, default_cost_centre_code) VALUES
('h0000001-0000-0000-0000-000000000001', 'a0000003-0000-0000-0000-000000000003', 'TRV-CCA-001', 'Jordan', 'Nguyen', 'jordan.nguyen@example.test', 'APAC-SALES'),
('h0000002-0000-0000-0000-000000000002', 'a0000003-0000-0000-0000-000000000003', 'TRV-CCA-002', 'Avery', 'Williams', 'avery.williams@example.test', 'EMEA-MARKETING'),
('h0000003-0000-0000-0000-000000000003', 'a0000003-0000-0000-0000-000000000003', 'TRV-CCA-003', 'Morgan', 'Patel', 'morgan.patel@example.test', 'NA-FINANCE'),
('h0000004-0000-0000-0000-000000000004', 'a0000003-0000-0000-0000-000000000003', 'TRV-CCA-004', 'Taylor', 'Chen', 'taylor.chen@example.test', 'EXEC')
ON CONFLICT DO NOTHING;
