using System;
using System.Collections.Generic;

namespace ExportOverDueFileUploader.DBmodels;

public partial class GoodsDeclaration_Import
{
    public long Id { get; set; }

    public DateTime? TransmissionDate { get; set; }

    public string? ProcessCode { get; set; }

    public string? MethodId { get; set; }

    public string? Payload { get; set; }

    public string? ImporterIban { get; set; }

    public string? ImporterName { get; set; }

    public string? ImporterNtn { get; set; }

    public long? FileAuditId { get; set; }

    public string? gdNumber { get; set; }

    public string? gdStatus { get; set; }

    public DateTime? GDDate { get; set; }

    public string? ModeOfPayment { get; set; }

    public string? FinInsUniqueNumber { get; set; }

    public int TenantId { get; set; }

    public DateTime CreationTime { get; set; }

    public long? CreatorUserId { get; set; }

    public DateTime? LastModificationTime { get; set; }

    public long? LastModifierUserId { get; set; }

    public bool IsDeleted { get; set; }

    public long? DeleterUserId { get; set; }

    public DateTime? DeletionTime { get; set; }
}
