using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ExportOverDueFileUploader.DBmodels;

namespace ExportOverDueFileUploader.ValidateIqBizLogic.Comparison_V2;

public class ComparisonResult
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long? Id { get; set; }

    public string? ComparisonType { get; set; }

    public string? ComparisonName { get; set; }

    public string? Entity1Key { get; set; }

    public string? Entity2Key { get; set; }

    public string? Entity1Value { get; set; }

    public string? Entity2Value { get; set; }

    public int Result { get; set; }

    public int TenantId { get; set; }

    public DateTime CreationTime { get; set; } = DateTime.UtcNow;

    public long? CreatorUserId { get; set; }

    public DateTime? LastModificationTime { get; set; }

    public long? LastModifierUserId { get; set; }

    public decimal? Variance { get; set; }

    public long? RequestStatusId { get; set; }

    [ForeignKey(nameof(RequestStatusId))]
    public virtual RequestStatus? RequestStatus { get; set; }

}

public class ComparisonResultImport : ComparisonResult
{
    public long? GdFiLinkId { get; set; }

    [ForeignKey(nameof(GdFiLinkId))]
    public virtual GdFiLink? GdFiLink { get; set; }

}

public class ComparisonResultExport : ComparisonResult
{
    public long? Gd_Fi_LinkId { get; set; }

    [ForeignKey(nameof(Gd_Fi_LinkId))]
    public virtual GD_FI_Link? Gd_Fi_Link { get; set; }
}

public class AggregiatedResultImport : ComparisonResult
{
    public long? FiId { get; set; }
    public long? GdId { get; set; }

    [ForeignKey(nameof(FiId))]
    public virtual FinancialInstrumentImport? Fi { get; set; }

    [ForeignKey(nameof(GdId))]
    public virtual GoodsDeclarationImport? Gd { get; set; }
}

public class AggregiatedResultExport : ComparisonResult
{
    public long? FiId { get; set; }
    public long? GdId { get; set; }

    [ForeignKey(nameof(FiId))]
    public virtual FinancialInstrument? Fi { get; set; }

    [ForeignKey(nameof(GdId))]
    public virtual GoodsDeclaration? Gd { get; set; }
}
