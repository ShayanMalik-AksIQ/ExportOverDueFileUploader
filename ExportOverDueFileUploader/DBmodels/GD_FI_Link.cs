using System;
using System.Collections.Generic;

namespace ExportOverDueFileUploader.DBmodels;

public partial class GD_FI_Link
{
    public long Id { get; set; }

    public string? type { get; set; }

    public double Amount { get; set; }

    public int advPayPercentage { get; set; }

    public int docAgainstPayPercentage { get; set; }

    public int docAgainstAcceptancePercentage { get; set; }

    public int sightPercentage { get; set; }

    public int usancePercentage { get; set; }

    public string? MatruityDate { get; set; }

    public DateTime? _MatruityDate { get; set; }

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

    public virtual FinancialInstrument? Fi { get; set; }

    public virtual GoodsDeclaration? Gd { get; set; }
}
