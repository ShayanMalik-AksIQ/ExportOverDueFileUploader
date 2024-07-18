using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ExportOverDueFileUploader.DBmodels;

public partial class ExportOverDueContext : DbContext
{
    public ExportOverDueContext()
    {
    }

    public ExportOverDueContext(DbContextOptions<ExportOverDueContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ComparatorSetting> ComparatorSettings { get; set; }

    public virtual DbSet<ComparisonResult> ComparisonResults { get; set; }

    public virtual DbSet<DefaultSetting> DefaultSettings { get; set; }

    public virtual DbSet<FileImportAuditTrail> FileImportAuditTrails { get; set; }

    public virtual DbSet<FileType> FileTypes { get; set; }

    public virtual DbSet<FinancialInstrumentImport> FinancialInstrumentImports { get; set; }

    public virtual DbSet<GdFiLink> GdFiLinks { get; set; }

    public virtual DbSet<GoodsDeclarationImport> GoodsDeclarationImports { get; set; }

    public virtual DbSet<Module> Modules { get; set; }

    public virtual DbSet<ReqStatusAuditTrail> ReqStatusAuditTrails { get; set; }

    public virtual DbSet<RequestStatus> RequestStatuses { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer(AppSettings.ConnectionString);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ComparisonResult>(entity =>
        {
            entity.HasIndex(e => e.GdFiLinkId, "IX_ComparisonResults_GdFiLinkId");

            entity.HasIndex(e => e.RequestStatusId, "IX_ComparisonResults_RequestStatusId");

            entity.HasOne(d => d.GdFiLink).WithMany(p => p.ComparisonResults).HasForeignKey(d => d.GdFiLinkId);

            entity.HasOne(d => d.RequestStatus).WithMany(p => p.ComparisonResults).HasForeignKey(d => d.RequestStatusId);
        });

        modelBuilder.Entity<DefaultSetting>(entity =>
        {
            entity.ToTable("DefaultSetting");
        });

        modelBuilder.Entity<FileImportAuditTrail>(entity =>
        {
            entity.ToTable("FileImportAuditTrail");

            entity.HasIndex(e => e.FileTypeId, "IX_FileImportAuditTrail_FileTypeId");

            entity.HasOne(d => d.FileType).WithMany(p => p.FileImportAuditTrails).HasForeignKey(d => d.FileTypeId);
        });

        modelBuilder.Entity<FileType>(entity =>
        {
            entity.ToTable("FileType");

            entity.HasIndex(e => e.RequestStatusId, "IX_FileType_RequestStatusId");

            entity.HasOne(d => d.RequestStatus).WithMany(p => p.FileTypes).HasForeignKey(d => d.RequestStatusId);
        });

        modelBuilder.Entity<FinancialInstrumentImport>(entity =>
        {
            entity.ToTable("FinancialInstrumentImport");
        });

        modelBuilder.Entity<GdFiLink>(entity =>
        {
            entity.ToTable("GdFiLink");

            entity.HasIndex(e => e.FiId, "IX_GdFiLink_FiId");

            entity.HasIndex(e => e.GdId, "IX_GdFiLink_GdId");

            entity.HasOne(d => d.Fi).WithMany(p => p.GdFiLinks).HasForeignKey(d => d.FiId);

            entity.HasOne(d => d.Gd).WithMany(p => p.GdFiLinks).HasForeignKey(d => d.GdId);
        });

        modelBuilder.Entity<GoodsDeclarationImport>(entity =>
        {
            entity.ToTable("GoodsDeclarationImport");
        });

        modelBuilder.Entity<ReqStatusAuditTrail>(entity =>
        {
            entity.ToTable("ReqStatusAuditTrail");
        });

        modelBuilder.Entity<RequestStatus>(entity =>
        {
            entity.ToTable("RequestStatus");

            entity.HasIndex(e => e.ModuleID, "IX_RequestStatus_ModuleID");

            entity.HasOne(d => d.Module).WithMany(p => p.RequestStatuses).HasForeignKey(d => d.ModuleID);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
