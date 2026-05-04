using Cinturon360.Mock.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Cinturon360.Mock.Api.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Organisation> Organisations => Set<Organisation>();
    public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<BillingRelationship> BillingRelationships => Set<BillingRelationship>();
    public DbSet<PaymentProviderAccount> PaymentProviderAccounts => Set<PaymentProviderAccount>();
    public DbSet<BuyerPaymentProfile> BuyerPaymentProfiles => Set<BuyerPaymentProfile>();
    public DbSet<SavedPaymentMethod> SavedPaymentMethods => Set<SavedPaymentMethod>();
    public DbSet<TravelPolicy> TravelPolicies => Set<TravelPolicy>();
    public DbSet<Traveller> Travellers => Set<Traveller>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<BookingSegment> BookingSegments => Set<BookingSegment>();
    public DbSet<BookingPaymentSchedule> BookingPaymentSchedules => Set<BookingPaymentSchedule>();
    public DbSet<BookingPaymentScheduleItem> BookingPaymentScheduleItems => Set<BookingPaymentScheduleItem>();
    public DbSet<InternalInvoice> InternalInvoices => Set<InternalInvoice>();
    public DbSet<InternalInvoiceLine> InternalInvoiceLines => Set<InternalInvoiceLine>();
    public DbSet<LedgerEntry> LedgerEntries => Set<LedgerEntry>();
    public DbSet<PaymentAttempt> PaymentAttempts => Set<PaymentAttempt>();
    public DbSet<AgencyMemo> AgencyMemos => Set<AgencyMemo>();
    public DbSet<ImportedContentBatch> ImportedContentBatches => Set<ImportedContentBatch>();
    public DbSet<FinanceExportBatch> FinanceExportBatches => Set<FinanceExportBatch>();
    public DbSet<FinanceExportItem> FinanceExportItems => Set<FinanceExportItem>();
    public DbSet<AuditEvent> AuditEvents => Set<AuditEvent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Organisation>(e =>
        {
            e.ToTable("organisations");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.Name).HasColumnName("name");
            e.Property(x => x.Code).HasColumnName("code");
            e.Property(x => x.Type).HasColumnName("type");
            e.Property(x => x.ParentOrganisationId).HasColumnName("parent_organisation_id");
            e.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc");
            e.HasOne(x => x.ParentOrganisation).WithMany(x => x.Children).HasForeignKey(x => x.ParentOrganisationId);
        });

        modelBuilder.Entity<AppUser>(e =>
        {
            e.ToTable("users");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.Email).HasColumnName("email");
            e.Property(x => x.PasswordHash).HasColumnName("password_hash");
            e.Property(x => x.Role).HasColumnName("role");
            e.Property(x => x.OrganisationId).HasColumnName("organisation_id");
            e.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc");
            e.HasOne(x => x.Organisation).WithMany().HasForeignKey(x => x.OrganisationId);
        });

        modelBuilder.Entity<BillingRelationship>(e =>
        {
            e.ToTable("billing_relationships");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.SellerOrganisationId).HasColumnName("seller_organisation_id");
            e.Property(x => x.BuyerOrganisationId).HasColumnName("buyer_organisation_id");
            e.Property(x => x.BillingMode).HasColumnName("billing_mode");
            e.Property(x => x.PaymentProviderType).HasColumnName("payment_provider_type");
            e.Property(x => x.CurrencyCode).HasColumnName("currency_code");
            e.Property(x => x.PaymentTermsDays).HasColumnName("payment_terms_days");
            e.Property(x => x.IsCommercialRiskHeldBySeller).HasColumnName("is_commercial_risk_held_by_seller");
            e.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc");
            e.HasOne(x => x.SellerOrganisation).WithMany().HasForeignKey(x => x.SellerOrganisationId);
            e.HasOne(x => x.BuyerOrganisation).WithMany().HasForeignKey(x => x.BuyerOrganisationId);
        });

        modelBuilder.Entity<PaymentProviderAccount>(e =>
        {
            e.ToTable("payment_provider_accounts");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.SellerOrganisationId).HasColumnName("seller_organisation_id");
            e.Property(x => x.ProviderType).HasColumnName("provider_type");
            e.Property(x => x.DisplayName).HasColumnName("display_name");
            e.Property(x => x.FakeExternalAccountId).HasColumnName("fake_external_account_id");
            e.Property(x => x.IsPrimary).HasColumnName("is_primary");
            e.Property(x => x.IsEnabled).HasColumnName("is_enabled");
            e.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc");
            e.HasOne(x => x.SellerOrganisation).WithMany().HasForeignKey(x => x.SellerOrganisationId);
        });

        modelBuilder.Entity<BuyerPaymentProfile>(e =>
        {
            e.ToTable("buyer_payment_profiles");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.SellerOrganisationId).HasColumnName("seller_organisation_id");
            e.Property(x => x.BuyerOrganisationId).HasColumnName("buyer_organisation_id");
            e.Property(x => x.PaymentProviderAccountId).HasColumnName("payment_provider_account_id");
            e.Property(x => x.FakeProviderCustomerId).HasColumnName("fake_provider_customer_id");
            e.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc");
            e.HasOne(x => x.SellerOrganisation).WithMany().HasForeignKey(x => x.SellerOrganisationId);
            e.HasOne(x => x.BuyerOrganisation).WithMany().HasForeignKey(x => x.BuyerOrganisationId);
            e.HasOne(x => x.PaymentProviderAccount).WithMany().HasForeignKey(x => x.PaymentProviderAccountId);
            e.HasMany(x => x.SavedPaymentMethods).WithOne(x => x.BuyerPaymentProfile).HasForeignKey(x => x.BuyerPaymentProfileId);
        });

        modelBuilder.Entity<SavedPaymentMethod>(e =>
        {
            e.ToTable("saved_payment_methods");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.BuyerPaymentProfileId).HasColumnName("buyer_payment_profile_id");
            e.Property(x => x.FakeProviderPaymentMethodId).HasColumnName("fake_provider_payment_method_id");
            e.Property(x => x.Brand).HasColumnName("brand");
            e.Property(x => x.Last4).HasColumnName("last4");
            e.Property(x => x.ExpiryMonth).HasColumnName("expiry_month");
            e.Property(x => x.ExpiryYear).HasColumnName("expiry_year");
            e.Property(x => x.IsDefault).HasColumnName("is_default");
            e.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc");
        });

        modelBuilder.Entity<TravelPolicy>(e =>
        {
            e.ToTable("travel_policies");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.ClientOrganisationId).HasColumnName("client_organisation_id");
            e.Property(x => x.Code).HasColumnName("code");
            e.Property(x => x.Name).HasColumnName("name");
            e.Property(x => x.BillingBehaviour).HasColumnName("billing_behaviour");
            e.Property(x => x.SavedPaymentMethodId).HasColumnName("saved_payment_method_id");
            e.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc");
            e.HasOne(x => x.ClientOrganisation).WithMany().HasForeignKey(x => x.ClientOrganisationId);
            e.HasOne(x => x.SavedPaymentMethod).WithMany().HasForeignKey(x => x.SavedPaymentMethodId);
        });

        modelBuilder.Entity<Traveller>(e =>
        {
            e.ToTable("travellers");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.ClientOrganisationId).HasColumnName("client_organisation_id");
            e.Property(x => x.ExternalTravellerId).HasColumnName("external_traveller_id");
            e.Property(x => x.FirstName).HasColumnName("first_name");
            e.Property(x => x.LastName).HasColumnName("last_name");
            e.Property(x => x.Email).HasColumnName("email");
            e.Property(x => x.DefaultCostCentreCode).HasColumnName("default_cost_centre_code");
            e.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc");
            e.Ignore(x => x.FullName);
            e.HasOne(x => x.ClientOrganisation).WithMany().HasForeignKey(x => x.ClientOrganisationId);
        });

        modelBuilder.Entity<Booking>(e =>
        {
            e.ToTable("bookings");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.BookingReference).HasColumnName("booking_reference");
            e.Property(x => x.PnrCode).HasColumnName("pnr_code");
            e.Property(x => x.TicketNumber).HasColumnName("ticket_number");
            e.Property(x => x.TravellerId).HasColumnName("traveller_id");
            e.Property(x => x.ClientOrganisationId).HasColumnName("client_organisation_id");
            e.Property(x => x.TmcOrganisationId).HasColumnName("tmc_organisation_id");
            e.Property(x => x.VendorOrganisationId).HasColumnName("vendor_organisation_id");
            e.Property(x => x.TravelPolicyId).HasColumnName("travel_policy_id");
            e.Property(x => x.CostCentreCode).HasColumnName("cost_centre_code");
            e.Property(x => x.Status).HasColumnName("status");
            e.Property(x => x.DepartureDate).HasColumnName("departure_date");
            e.Property(x => x.ReturnDate).HasColumnName("return_date");
            e.Property(x => x.CurrencyCode).HasColumnName("currency_code");
            e.Property(x => x.SupplierAmount).HasColumnName("supplier_amount");
            e.Property(x => x.TaxAmount).HasColumnName("tax_amount");
            e.Property(x => x.ServiceFeeAmount).HasColumnName("service_fee_amount");
            e.Property(x => x.SurchargeAmount).HasColumnName("surcharge_amount");
            e.Property(x => x.TotalAmount).HasColumnName("total_amount");
            e.Property(x => x.SourceSystem).HasColumnName("source_system");
            e.Property(x => x.ImportedAtUtc).HasColumnName("imported_at_utc");
            e.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc");
            e.HasOne(x => x.Traveller).WithMany().HasForeignKey(x => x.TravellerId);
            e.HasOne(x => x.ClientOrganisation).WithMany().HasForeignKey(x => x.ClientOrganisationId);
            e.HasOne(x => x.TmcOrganisation).WithMany().HasForeignKey(x => x.TmcOrganisationId);
            e.HasOne(x => x.VendorOrganisation).WithMany().HasForeignKey(x => x.VendorOrganisationId);
            e.HasOne(x => x.TravelPolicy).WithMany().HasForeignKey(x => x.TravelPolicyId);
            e.HasMany(x => x.Segments).WithOne(x => x.Booking).HasForeignKey(x => x.BookingId);
            e.HasMany(x => x.PaymentSchedules).WithOne(x => x.Booking).HasForeignKey(x => x.BookingId);
            e.HasMany(x => x.Invoices).WithOne(x => x.Booking).HasForeignKey(x => x.BookingId);
            e.HasMany(x => x.LedgerEntries).WithOne(x => x.Booking).HasForeignKey(x => x.BookingId);
            e.HasMany(x => x.AgencyMemos).WithOne(x => x.Booking).HasForeignKey(x => x.BookingId);
        });

        modelBuilder.Entity<BookingSegment>(e =>
        {
            e.ToTable("booking_segments");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.BookingId).HasColumnName("booking_id");
            e.Property(x => x.SegmentType).HasColumnName("segment_type");
            e.Property(x => x.Carrier).HasColumnName("carrier");
            e.Property(x => x.FlightNumber).HasColumnName("flight_number");
            e.Property(x => x.FromLocation).HasColumnName("from_location");
            e.Property(x => x.ToLocation).HasColumnName("to_location");
            e.Property(x => x.DepartureUtc).HasColumnName("departure_utc");
            e.Property(x => x.ArrivalUtc).HasColumnName("arrival_utc");
            e.Property(x => x.SupplierName).HasColumnName("supplier_name");
            e.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc");
        });

        modelBuilder.Entity<BookingPaymentSchedule>(e =>
        {
            e.ToTable("booking_payment_schedules");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.BookingId).HasColumnName("booking_id");
            e.Property(x => x.TotalScheduledAmount).HasColumnName("total_scheduled_amount");
            e.Property(x => x.CurrencyCode).HasColumnName("currency_code");
            e.Property(x => x.Status).HasColumnName("status");
            e.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc");
            e.HasMany(x => x.Items).WithOne(x => x.BookingPaymentSchedule).HasForeignKey(x => x.BookingPaymentScheduleId);
        });

        modelBuilder.Entity<BookingPaymentScheduleItem>(e =>
        {
            e.ToTable("booking_payment_schedule_items");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.BookingPaymentScheduleId).HasColumnName("booking_payment_schedule_id");
            e.Property(x => x.ItemType).HasColumnName("item_type");
            e.Property(x => x.PaymentTiming).HasColumnName("payment_timing");
            e.Property(x => x.Description).HasColumnName("description");
            e.Property(x => x.Amount).HasColumnName("amount");
            e.Property(x => x.DueDate).HasColumnName("due_date");
            e.Property(x => x.Status).HasColumnName("status");
            e.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc");
        });

        modelBuilder.Entity<InternalInvoice>(e =>
        {
            e.ToTable("internal_invoices");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.InvoiceNumber).HasColumnName("invoice_number");
            e.Property(x => x.SellerOrganisationId).HasColumnName("seller_organisation_id");
            e.Property(x => x.BuyerOrganisationId).HasColumnName("buyer_organisation_id");
            e.Property(x => x.BookingId).HasColumnName("booking_id");
            e.Property(x => x.Status).HasColumnName("status");
            e.Property(x => x.CollectionMode).HasColumnName("collection_mode");
            e.Property(x => x.PaymentProviderType).HasColumnName("payment_provider_type");
            e.Property(x => x.IssueDate).HasColumnName("issue_date");
            e.Property(x => x.DueDate).HasColumnName("due_date");
            e.Property(x => x.CurrencyCode).HasColumnName("currency_code");
            e.Property(x => x.SubtotalAmount).HasColumnName("subtotal_amount");
            e.Property(x => x.TaxAmount).HasColumnName("tax_amount");
            e.Property(x => x.TotalAmount).HasColumnName("total_amount");
            e.Property(x => x.FakeExternalInvoiceId).HasColumnName("fake_external_invoice_id");
            e.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc");
            e.HasOne(x => x.SellerOrganisation).WithMany().HasForeignKey(x => x.SellerOrganisationId);
            e.HasOne(x => x.BuyerOrganisation).WithMany().HasForeignKey(x => x.BuyerOrganisationId);
            e.HasMany(x => x.Lines).WithOne(x => x.InternalInvoice).HasForeignKey(x => x.InternalInvoiceId);
            e.HasMany(x => x.PaymentAttempts).WithOne(x => x.InternalInvoice).HasForeignKey(x => x.InternalInvoiceId);
            e.HasMany(x => x.LedgerEntries).WithOne(x => x.InternalInvoice).HasForeignKey(x => x.InternalInvoiceId);
        });

        modelBuilder.Entity<InternalInvoiceLine>(e =>
        {
            e.ToTable("internal_invoice_lines");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.InternalInvoiceId).HasColumnName("internal_invoice_id");
            e.Property(x => x.LineType).HasColumnName("line_type");
            e.Property(x => x.Description).HasColumnName("description");
            e.Property(x => x.Quantity).HasColumnName("quantity");
            e.Property(x => x.UnitAmount).HasColumnName("unit_amount");
            e.Property(x => x.TaxAmount).HasColumnName("tax_amount");
            e.Property(x => x.LineTotalAmount).HasColumnName("line_total_amount");
            e.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc");
        });

        modelBuilder.Entity<LedgerEntry>(e =>
        {
            e.ToTable("ledger_entries");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc");
            e.Property(x => x.SellerOrganisationId).HasColumnName("seller_organisation_id");
            e.Property(x => x.BuyerOrganisationId).HasColumnName("buyer_organisation_id");
            e.Property(x => x.BookingId).HasColumnName("booking_id");
            e.Property(x => x.InternalInvoiceId).HasColumnName("internal_invoice_id");
            e.Property(x => x.EntryType).HasColumnName("entry_type");
            e.Property(x => x.Description).HasColumnName("description");
            e.Property(x => x.DebitAmount).HasColumnName("debit_amount");
            e.Property(x => x.CreditAmount).HasColumnName("credit_amount");
            e.Property(x => x.CurrencyCode).HasColumnName("currency_code");
            e.Property(x => x.ExternalReference).HasColumnName("external_reference");
            e.HasOne(x => x.SellerOrganisation).WithMany().HasForeignKey(x => x.SellerOrganisationId);
            e.HasOne(x => x.BuyerOrganisation).WithMany().HasForeignKey(x => x.BuyerOrganisationId);
        });

        modelBuilder.Entity<PaymentAttempt>(e =>
        {
            e.ToTable("payment_attempts");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.InternalInvoiceId).HasColumnName("internal_invoice_id");
            e.Property(x => x.AttemptedAtUtc).HasColumnName("attempted_at_utc");
            e.Property(x => x.ProviderType).HasColumnName("provider_type");
            e.Property(x => x.FakeProviderPaymentId).HasColumnName("fake_provider_payment_id");
            e.Property(x => x.SavedPaymentMethodId).HasColumnName("saved_payment_method_id");
            e.Property(x => x.Succeeded).HasColumnName("succeeded");
            e.Property(x => x.FailureReason).HasColumnName("failure_reason");
            e.Property(x => x.Amount).HasColumnName("amount");
            e.Property(x => x.CurrencyCode).HasColumnName("currency_code");
            e.HasOne(x => x.SavedPaymentMethod).WithMany().HasForeignKey(x => x.SavedPaymentMethodId);
        });

        modelBuilder.Entity<AgencyMemo>(e =>
        {
            e.ToTable("agency_memos");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.MemoType).HasColumnName("memo_type");
            e.Property(x => x.MemoNumber).HasColumnName("memo_number");
            e.Property(x => x.AirlineCode).HasColumnName("airline_code");
            e.Property(x => x.TicketNumber).HasColumnName("ticket_number");
            e.Property(x => x.EmdNumber).HasColumnName("emd_number");
            e.Property(x => x.PnrCode).HasColumnName("pnr_code");
            e.Property(x => x.BookingId).HasColumnName("booking_id");
            e.Property(x => x.TmcOrganisationId).HasColumnName("tmc_organisation_id");
            e.Property(x => x.ClientOrganisationId).HasColumnName("client_organisation_id");
            e.Property(x => x.ReasonCode).HasColumnName("reason_code");
            e.Property(x => x.ReasonDescription).HasColumnName("reason_description");
            e.Property(x => x.Amount).HasColumnName("amount");
            e.Property(x => x.CurrencyCode).HasColumnName("currency_code");
            e.Property(x => x.Status).HasColumnName("status");
            e.Property(x => x.Outcome).HasColumnName("outcome");
            e.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc");
            e.HasOne(x => x.TmcOrganisation).WithMany().HasForeignKey(x => x.TmcOrganisationId);
            e.HasOne(x => x.ClientOrganisation).WithMany().HasForeignKey(x => x.ClientOrganisationId);
        });

        modelBuilder.Entity<ImportedContentBatch>(e =>
        {
            e.ToTable("imported_content_batches");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.SourceSystem).HasColumnName("source_system");
            e.Property(x => x.FileName).HasColumnName("file_name");
            e.Property(x => x.ImportType).HasColumnName("import_type");
            e.Property(x => x.Status).HasColumnName("status");
            e.Property(x => x.RecordCount).HasColumnName("record_count");
            e.Property(x => x.ImportedByEmail).HasColumnName("imported_by_email");
            e.Property(x => x.ImportedAtUtc).HasColumnName("imported_at_utc");
        });

        modelBuilder.Entity<FinanceExportBatch>(e =>
        {
            e.ToTable("finance_export_batches");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.ExportTarget).HasColumnName("export_target");
            e.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc");
            e.Property(x => x.CreatedByEmail).HasColumnName("created_by_email");
            e.Property(x => x.InvoiceCount).HasColumnName("invoice_count");
            e.Property(x => x.Status).HasColumnName("status");
            e.HasMany(x => x.Items).WithOne(x => x.FinanceExportBatch).HasForeignKey(x => x.FinanceExportBatchId);
        });

        modelBuilder.Entity<FinanceExportItem>(e =>
        {
            e.ToTable("finance_export_items");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.FinanceExportBatchId).HasColumnName("finance_export_batch_id");
            e.Property(x => x.InternalInvoiceId).HasColumnName("internal_invoice_id");
            e.Property(x => x.ExportedAtUtc).HasColumnName("exported_at_utc");
            e.HasOne(x => x.InternalInvoice).WithMany().HasForeignKey(x => x.InternalInvoiceId);
        });

        modelBuilder.Entity<AuditEvent>(e =>
        {
            e.ToTable("audit_events");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc");
            e.Property(x => x.ActorEmail).HasColumnName("actor_email");
            e.Property(x => x.OrganisationId).HasColumnName("organisation_id");
            e.Property(x => x.EntityType).HasColumnName("entity_type");
            e.Property(x => x.EntityId).HasColumnName("entity_id");
            e.Property(x => x.Action).HasColumnName("action");
            e.Property(x => x.Summary).HasColumnName("summary");
            e.Property(x => x.JsonDetails).HasColumnName("json_details");
        });
    }
}
