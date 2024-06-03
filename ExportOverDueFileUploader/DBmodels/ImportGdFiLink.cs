using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExportOverDueFileUploader.DBmodels
{
    public class ImportGdFiLink
    {
        public long Id { get; set; }
        public int TenantId { get; set; }

        public DateTime CreationTime { get; set; }

        public long? CreatorUserId { get; set; }

        public DateTime? LastModificationTime { get; set; }

        public long? LastModifierUserId { get; set; }

        public bool IsDeleted { get; set; }

        public long? DeleterUserId { get; set; }

        public DateTime? DeletionTime { get; set; }

        public long? FiId { get; set; }

        public long? GdId { get; set; }
        public long? RequestStatusId { get; set; }
        [ForeignKey("RequestStatusId")]
        public virtual RequestStatus RequestStatus { get; set; }
        [NotMapped]
        public string RequestStatusName
        {
            get
            {
                if (RequestStatus != null)
                {
                    return RequestStatus.DisplayName;
                }
                return string.Empty;
            }
        }

        [ForeignKey("FiId")]
        public virtual FinancialInstrument_Import? Fi { get; set; }
        [ForeignKey("GdId")]
        public virtual GoodsDeclaration_Import? Gd { get; set; }

    }
}
