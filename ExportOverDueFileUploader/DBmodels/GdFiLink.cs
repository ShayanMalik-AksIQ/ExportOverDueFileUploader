﻿using System;
using System.Collections.Generic;

namespace ExportOverDueFileUploader.DBmodels;

public partial class GdFiLink
{
    public long Id { get; set; }

    public string? Type { get; set; }

    public long? FiId { get; set; }

    public long? GdId { get; set; }

    public int TenantId { get; set; }

    public DateTime CreationTime { get; set; }

    public long? CreatorUserId { get; set; }

    public DateTime? LastModificationTime { get; set; }

    public long? LastModifierUserId { get; set; }

    public bool IsDeleted { get; set; }

    public long? DeleterUserId { get; set; }

    public DateTime? DeletionTime { get; set; }

    public long? RequestStatusId { get; set; }

    public virtual ICollection<ComparisonResult> ComparisonResults { get; set; } = new List<ComparisonResult>();

    public virtual FinancialInstrumentImport? Fi { get; set; }

    public virtual GoodsDeclarationImport? Gd { get; set; }

    public virtual RequestStatus? RequestStatus { get; set; }
}
