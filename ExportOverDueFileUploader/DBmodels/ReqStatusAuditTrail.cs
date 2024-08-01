using System;
using System.Collections.Generic;

namespace ExportOverDueFileUploader.DBmodels;

public partial class ReqStatusAuditTrail
{
    public long Id { get; set; }

    public string? OldStatus { get; set; }

    public string? NewStatus { get; set; }

    public string? EntityName { get; set; }

    public long EntityId { get; set; }

    public int TenantId { get; set; }

    public DateTime CreationTime { get; set; }

    public long? CreatorUserId { get; set; }

    public DateTime? LastModificationTime { get; set; }

    public long? LastModifierUserId { get; set; }

    public long NewStatusId { get; set; }

    public long OldStatusId { get; set; }

    public string? Remarks { get; set; }
}
