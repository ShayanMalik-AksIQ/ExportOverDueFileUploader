using System;
using System.Collections.Generic;

namespace ExportOverDueFileUploader.DBmodels;

public partial class ComparatorSetting
{
    public long Id { get; set; }

    public string? ValidationType { get; set; }

    public string? Entity2Key { get; set; }

    public string? Entity1Key { get; set; }

    public int TenantId { get; set; }

    public DateTime CreationTime { get; set; }

    public long? CreatorUserId { get; set; }

    public DateTime? LastModificationTime { get; set; }

    public long? LastModifierUserId { get; set; }

    public int IsSameEntity { get; set; }

    public bool CalculateVariance { get; set; }
}
