using System;
using System.Collections.Generic;

namespace ExportOverDueFileUploader.DBmodels;

public partial class FinancialInstrument
{
    public long Id { get; set; }

    public string? I_D { get; set; }

    public string? TRANSACTION_STATUS { get; set; }

    public string? RETRY_COUNT { get; set; }

    public string? MAX_RETRY_COUNT { get; set; }

    public string? IBAN { get; set; }

    public string? CREATED_DATETIME { get; set; }

    public string? CREATED_BY { get; set; }

    public string? MODIFIED_DATETIME { get; set; }

    public string? MODIFIED_BY { get; set; }

    public string? RECEIVE_DATE { get; set; }

    public string? SEQUENCE_NUMBER { get; set; }

    public string? PAYLOAD { get; set; }

    public string? SERVICE_URL { get; set; }

    public string? PROCESSING_QUEUE { get; set; }

    public string? TRANSACTION_TYPE { get; set; }

    public string? RESPONSE_CODE { get; set; }

    public string? RESPONSE_DESCRIPTION { get; set; }

    public string? HOST { get; set; }

    public string? T24_REC_ID { get; set; }

    public int TenantId { get; set; }

    public DateTime CreationTime { get; set; }

    public long? CreatorUserId { get; set; }

    public DateTime? LastModificationTime { get; set; }

    public long? LastModifierUserId { get; set; }

    public bool IsDeleted { get; set; }

    public long? DeleterUserId { get; set; }

    public DateTime? DeletionTime { get; set; }

    public string? contractCollectionData { get; set; }

    public string? finInsUniqueNumber { get; set; }

    public string? lcData { get; set; }

    public string? modeOfPayment { get; set; }

    public string? openAccountGdNumber { get; set; }

    public string? paymentInformation { get; set; }

    public string? ExporterRegNo { get; set; }

    public string? ExporterAddress { get; set; }

    public string? ExporterCity { get; set; }

    public string? ExporterNTN { get; set; }

    public string? ExporterName { get; set; }

    public int? Days { get; set; }

    public DateTime? FiCertifcationdate { get; set; }

    public virtual ICollection<GD_FI_Link> GD_FI_Links { get; set; } = new List<GD_FI_Link>();


}
