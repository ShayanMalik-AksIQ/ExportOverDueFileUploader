using System.ComponentModel.DataAnnotations.Schema;

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

    public decimal? Variance { get; set; }

    public virtual GdFiLink? GdFiLink { get; set; }


    public long? FiId { get; set; }
    [ForeignKey("FiId")]
    public virtual FinancialInstrumentImport? Fi { get; set; }

    public virtual RequestStatus? RequestStatus { get; set; }
    public override string ToString()
    {
        return $"ComparisonType: {ComparisonType}, ComparisonName: {ComparisonName}, \n " +
               $"Entity1Key: {Entity1Key}, Entity2Key: {Entity2Key}, \n " +
               $"Entity1Value: {Entity1Value}, Entity2Value: {Entity2Value}, \n Result: {Result}, " +
               $"GdFiLinkId: {GdFiLinkId}, Variance: {Variance}  \n \n";
    }
}
