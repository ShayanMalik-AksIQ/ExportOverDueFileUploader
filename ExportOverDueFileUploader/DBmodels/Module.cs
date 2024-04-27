using System;
using System.Collections.Generic;

namespace ExportOverDueFileUploader.DBmodels;

public partial class Module
{
    public long Id { get; set; }

    public string? ModuleName { get; set; }

    public int TenantId { get; set; }

    public DateTime CreationTime { get; set; }

    public long? CreatorUserId { get; set; }

    public DateTime? LastModificationTime { get; set; }

    public long? LastModifierUserId { get; set; }

    public bool IsDeleted { get; set; }

    public long? DeleterUserId { get; set; }

    public DateTime? DeletionTime { get; set; }

    public virtual ICollection<RequestStatus> RequestStatuses { get; set; } = new List<RequestStatus>();
}
