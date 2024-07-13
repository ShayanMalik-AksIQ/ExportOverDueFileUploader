using System;
using System.Collections.Generic;

namespace ExportOverDueFileUploader.DBmodels;

public partial class ComparisonResult
{
    public long Id { get; set; }

    public string? ComparisonType { get; set; }

    public string? ComparisonName { get; set; }

    public string? Entity1Key { get; set; }

    public string? Entity2Key { get; set; }

    public string? Entity1Value { get; set; }

    public string? Entity2Value { get; set; }

    public int Result { get; set; }

    public long? RequestStatusId { get; set; }

    public int TenantId { get; set; }

    public DateTime CreationTime { get; set; }

    public long? CreatorUserId { get; set; }

    public DateTime? LastModificationTime { get; set; }

    public long? LastModifierUserId { get; set; }

    public long? GdFiLinkId { get; set; }

    public virtual GdFiLink? GdFiLink { get; set; }

    public virtual RequestStatus? RequestStatus { get; set; }
}
