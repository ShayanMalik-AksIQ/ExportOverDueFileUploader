using System;
using System.Collections.Generic;

namespace ExportOverDueFileUploader.DBmodels;

public partial class RequestStatus
{
    public long Id { get; set; }

    public string? DisplayName { get; set; }

    public string? StatusCode { get; set; }

    public string? ColorCode { get; set; }

    public long? ModuleID { get; set; }

    public int LoadingOrder { get; set; }

    public int TenantId { get; set; }

    public DateTime CreationTime { get; set; }

    public long? CreatorUserId { get; set; }

    public DateTime? LastModificationTime { get; set; }

    public long? LastModifierUserId { get; set; }

    public bool IsDeleted { get; set; }

    public long? DeleterUserId { get; set; }

    public DateTime? DeletionTime { get; set; }

    public virtual ICollection<FileType> FileTypes { get; set; } = new List<FileType>();

    public virtual ICollection<ImportGdFiLink> ImportGdFiLinks { get; set; } = new List<ImportGdFiLink>();

    public virtual Module? Module { get; set; }
}
