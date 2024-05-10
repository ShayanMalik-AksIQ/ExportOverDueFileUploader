using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExportOverDueFileUploader.Modles.JsonHelper
{
    public class CobJson
    {
    }


public class ContractCollectionData
    {
        public int advPayPercentage { get; set; }
        public int docAgainstPayPercentage { get; set; }
        public int docAgainstAcceptancePercentage { get; set; }
        public int days { get; set; }
        public int totalPercentage { get; set; }
    }

    public class PaymentInformation
    {
        public string consigneeName { get; set; }
        public string consigneeAddress { get; set; }
        public string consigneeCountry { get; set; }
        public string consigneeIban { get; set; }
        public string portOfDischarge { get; set; }
        public string deliveryTerms { get; set; }
        public string financialInstrumentCurrency { get; set; }
        public double financialInstrumentValue { get; set; }
        public double remainingInvoiceValue { get; set; }
        public string expiryDate { get; set; }
    }

    public class ItemInformation
    {
        public string hsCode { get; set; }
        public string goodsDescription { get; set; }
        public double quantity { get; set; }
        public string uom { get; set; }
        public string countryOfOrigin { get; set; }
        public double itemInvoiceValue { get; set; }
    }

    public class FinancialInstrumentInfo
    {
        public string exporterNtn { get; set; }
        public string exporterName { get; set; }
        public string exporterIban { get; set; }
        public string modeOfPayment { get; set; }
        public string finInsUniqueNumber { get; set; }
        public Openaccountdata openAccountData { get; set; }
        public Cashmargin cashMargin { get; set; }
        public Contractcollectiondata contractCollectionData { get; set; }
        public Lcdata lcData { get; set; }
        public Paymentinformation paymentInformation { get; set; }
        public Financialtraninformation financialTranInformation { get; set; }
        public string remarks { get; set; }
    }

    public class ItemInformationGdInfo
    {
        public string hsCode { get; set; }
        public double quantity { get; set; }
        public double unitPrice { get; set; }
        public double totalValue { get; set; }
        public double exportValue { get; set; }
        public string uom { get; set; }
    }

    public class GdInfo
    {
        public string gdNumber { get; set; }
        public string gdStatus { get; set; }
        public string consignmentCategory { get; set; }
        public string gdType { get; set; }
        public string collectorate { get; set; }
        public string blawbNumber { get; set; }
        public string blawbDate { get; set; }
        public object virAirNumber { get; set; }
        public string clearanceDate { get; set; }
        public ConsignorConsigneeInfo consignorConsigneeInfo { get; set; }
        public FinancialInfo financialInfo { get; set; }
        public GeneralInformation generalInformation { get; set; }
        public List<ItemInformationGdInfo> itemInformation { get; set; }
    }

    public class DataCob
    {
        public string cobUniqueIdNumber { get; set; }
        public string tradeTranType { get; set; }
        public string iban { get; set; }
        public FinancialInstrumentInfo financialInstrumentInfo { get; set; }
        public List<object> bankAdviceInfo { get; set; }
        public List<GdInfo> gdInfo { get; set; }
    }

    public class cobPayLoad
    {
        public string messageId { get; set; }
        public string timestamp { get; set; }
        public string senderId { get; set; }
        public string receiverId { get; set; }
        public string processingCode { get; set; }
        public DataCob data { get; set; }
        public string signature { get; set; }
    }



}
