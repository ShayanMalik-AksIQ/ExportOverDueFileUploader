using ExportOverDueFileUploader.ValidateIqBizLogic.Comparison_V2;

namespace ExportOverDueFileUploader.DBmodels;

public partial class GD_FI_Link
{
    public long Id { get; set; }

    public string? type { get; set; }

    public double Amount { get; set; } = 0;

    public int advPayPercentage { get; set; } = 0;

    public int docAgainstPayPercentage { get; set; } = 0;

    public int docAgainstAcceptancePercentage { get; set; } = 0;

    public int sightPercentage { get; set; } = 0;

    public int usancePercentage { get; set; } = 0;

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
    public long? RequestStatusId { get; set; }
    public virtual RequestStatus? RequestStatus { get; set; }
    public virtual FinancialInstrument? Fi { get; set; }

    public virtual GoodsDeclaration? Gd { get; set; }
    public virtual ICollection<ComparisonResultExport>? ComparisonResultExports { get; set; }
}
