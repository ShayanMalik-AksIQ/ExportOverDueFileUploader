using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExportOverDueFileUploader.Modles.JsonHelper
{
    public class GdImportJson
    {
        public string messageId { get; set; }
        public string timestamp { get; set; }
        public string senderId { get; set; }
        public string receiverId { get; set; }
        public string processingCode { get; set; }
        public Data data { get; set; }
        public string signature { get; set; }
    }

    public class Data
    {
        public string gdNumber { get; set; }
        public string gdStatus { get; set; }
        public string consignmentCategory { get; set; }

        public string blAwbNumber { get; set; }
        public string blAwbDate { get; set; }

        // Add other properties as needed
        public ConsignorConsigneeInfo consignorConsigneeInfo { get; set; }
        public FinancialInformation financialInformation { get; set; }
        public GeneralInformation generalInformation { get; set; }
        public FinancialInfo financialInfo { get; set; }
        public object itemInformation { get; set; }
    }
}
