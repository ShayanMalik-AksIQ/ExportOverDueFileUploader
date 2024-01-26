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

    public virtual DbSet<DefaultSetting> DefaultSettings { get; set; }

    public virtual DbSet<FileImportAuditTrail> FileImportAuditTrails { get; set; }

    public virtual DbSet<FileType> FileTypes { get; set; }

    public virtual DbSet<FinancialInstrument> FinancialInstruments { get; set; }

    public virtual DbSet<GD_FI_Link> GD_FI_Links { get; set; }

    public virtual DbSet<GoodsDeclaration> GoodsDeclarations { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer(AppSettings.ConnectionString);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
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
        });

        modelBuilder.Entity<FinancialInstrument>(entity =>
        {
            entity.ToTable("FinancialInstrument");
        });

        modelBuilder.Entity<GD_FI_Link>(entity =>
        {
            entity.ToTable("GD_FI_Link");

            entity.HasIndex(e => e.FiId, "IX_GD_FI_Link_FiId");

            entity.HasIndex(e => e.GdId, "IX_GD_FI_Link_GdId");

            entity.HasIndex(e => new { e.Id, e._MatruityDate, e.FiId, e.GdId }, "NonClusteredIndex-20240124-124710");

            entity.HasOne(d => d.Fi).WithMany(p => p.GD_FI_Links).HasForeignKey(d => d.FiId);

            entity.HasOne(d => d.Gd).WithMany(p => p.GD_FI_Links).HasForeignKey(d => d.GdId);
        });

        modelBuilder.Entity<GoodsDeclaration>(entity =>
        {
            entity.ToTable("GoodsDeclaration");

            entity.HasIndex(e => new { e.Id, e.GDDate }, "NonClusteredIndex-20240124-124535");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
