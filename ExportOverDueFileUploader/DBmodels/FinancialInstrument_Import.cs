using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExportOverDueFileUploader.DBmodels
{
    public class FinancialInstrument_Import
    {

        public long Id { get; set; }
        public int TenantId { get; set; }

        public DateTime CreationTime { get; set; }

        public long? CreatorUserId { get; set; }

        public DateTime? LastModificationTime { get; set; }

        public long? LastModifierUserId { get; set; }

        public bool IsDeleted { get; set; }

        public long? DeleterUserId { get; set; }
        public DateTime? TransmissionDate { get; set; }
        public string? ResponseCode { get; set; }
        public string? MethodId { get; set; }
        public string? FinInsUniqueNumber { get; set; }
        public string? modeOfPayment { get; set; }
        public string? Payload { get; set; }
        public string? ImporterIban { get; set; }
        public string? ImporterName { get; set; }
        public string? ImporterNtn { get; set; }
        public DateTime? FiCertifcationDate { get; set; }
        public int? Days { get; set; }
        public long? FileAuditId { get; set; }

        public virtual ICollection<ImportGdFiLink> ImportGdFiLinks { get; set; } = new List<ImportGdFiLink>();
    }
}
