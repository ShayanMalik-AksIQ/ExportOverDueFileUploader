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
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-O10K6M5; Database=ExportOverDue_dbRefactor; Trusted_Connection=True; TrustServerCertificate=True;");

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
