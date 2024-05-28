using System;
using System.Collections.Generic;

namespace ExportOverDueFileUploader.DBmodels;

public partial class FileType
{
    public long Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public string? FilePath { get; set; }

    public int TenantId { get; set; }

    public DateTime CreationTime { get; set; }

    public long? CreatorUserId { get; set; }

    public DateTime? LastModificationTime { get; set; }

    public long? LastModifierUserId { get; set; }

    public bool IsDeleted { get; set; }

    public long? DeleterUserId { get; set; }

    public DateTime? DeletionTime { get; set; }

    public string? ColumnNames { get; set; }

    public long? RequestStatusId { get; set; }

    public int HeaderRow { get; set; }

    public string? ColumnRename { get; set; }
    public virtual ICollection<FileImportAuditTrail> FileImportAuditTrails { get; set; } = new List<FileImportAuditTrail>();

    public virtual RequestStatus? RequestStatus { get; set; }
}
