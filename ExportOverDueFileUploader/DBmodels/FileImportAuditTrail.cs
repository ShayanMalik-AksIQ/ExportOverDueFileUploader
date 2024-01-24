using System;
using System.Collections.Generic;

namespace ExportOverDueFileUploader.DBmodels;

public partial class FileImportAuditTrail
{
    public long Id { get; set; }

    public string? FileName { get; set; }

    public int TenantId { get; set; }

    public DateTime CreationTime { get; set; }

    public long? CreatorUserId { get; set; }

    public DateTime? LastModificationTime { get; set; }

    public long? LastModifierUserId { get; set; }

    public bool IsDeleted { get; set; }

    public long? DeleterUserId { get; set; }

    public DateTime? DeletionTime { get; set; }

    public string? Remarks { get; set; }

    public bool Success { get; set; }

    public long? FileTypeId { get; set; }

    public virtual FileType? FileType { get; set; }
}
