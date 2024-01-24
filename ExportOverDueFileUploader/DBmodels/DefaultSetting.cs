using System;
using System.Collections.Generic;

namespace ExportOverDueFileUploader.DBmodels;

public partial class DefaultSetting
{
    public long Id { get; set; }

    public string? DisplayName { get; set; }

    public string? Key { get; set; }

    public string? Value { get; set; }

    public string? Category { get; set; }

    public int TenantId { get; set; }
}
