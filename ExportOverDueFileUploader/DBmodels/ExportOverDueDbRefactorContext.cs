using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ExportOverDueFileUploader.DBmodels;

public partial class ExportOverDueDbRefactorContext : DbContext
{
    public ExportOverDueDbRefactorContext()
    {
    }

    public ExportOverDueDbRefactorContext(DbContextOptions<ExportOverDueDbRefactorContext> options)
        : base(options)
    {
    }

    public virtual DbSet<DefaultSetting> DefaultSettings { get; set; }

    public virtual DbSet<FileImportAuditTrail> FileImportAuditTrails { get; set; }

    public virtual DbSet<FileType> FileTypes { get; set; }

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

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
