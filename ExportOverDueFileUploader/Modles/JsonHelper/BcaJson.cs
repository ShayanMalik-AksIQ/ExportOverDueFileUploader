using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExportOverDueFileUploader.Modles.JsonHelper
{
    internal class BcaJson
    {
    }
    public class BcaInformation
    {
        public string BcaEventName { get; set; }
        public string EventDate { get; set; }
        public decimal RunningSerialNumber { get; set; }
        public string SwiftReference { get; set; }
        public string BillNumber { get; set; }
        public string BillDated { get; set; }
        public decimal BillAmount { get; set; }
        public string InvoiceNumber { get; set; }
        public string InvoiceDate { get; set; }
        public decimal InvoiceAmount { get; set; }
    }

    public class Deductions
    {
        public decimal ForeignBankChargesFcy { get; set; }
        public decimal AgentCommissionFcy { get; set; }
        public decimal WithholdingTaxPkr { get; set; }
        public decimal EdsPkr { get; set; }
    }

    public class NetAmountRealized
    {
        public string IsFinInsCurrencyDiff { get; set; }
        public decimal BcaFc { get; set; }
        public decimal FcyExchangeRate { get; set; }
        public decimal BcaPkr { get; set; }
        public string DateOfRealized { get; set; }
        public decimal AdjustFromSpecialFcyAcc { get; set; }
        public string IsRemAmtSettledWithDiscount { get; set; }
        public decimal AllowedDiscountPercentage { get; set; }
        public decimal TotalBcaAmount { get; set; }
        public decimal AmountRealized { get; set; }
        public decimal Balance { get; set; }
    }

    public class BCAPayLoad
    {
        public string BcaUniqueIdNumber { get; set; }
        public string Iban { get; set; }
        public string GdNumber { get; set; }
        public string ExporterNtn { get; set; }
        public string ExporterName { get; set; }
        public string ModeOfPayment { get; set; }
        public string FinInsUniqueNumber { get; set; }
        public BcaInformation BcaInformation { get; set; }
        public Deductions Deductions { get; set; }
        public NetAmountRealized NetAmountRealized { get; set; }
        public string Remarks { get; set; }
    }
}
