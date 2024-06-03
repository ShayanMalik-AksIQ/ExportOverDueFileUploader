using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExportOverDueFileUploader.Modles.JsonHelper
{
    public class FiJson
    {
    }
    public class FIPayload
    {
        public string MessageId { get; set; }
        public string Timestamp { get; set; }
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public string MethodId { get; set; }
        public object Data { get; set; }
        public string Signature { get; set; }
    }

    public class FiPayLoadJson
    {
        public string importerNtn { get; set; }
        public string importerName { get; set; }
        public string importerIban { get; set; }
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
    public class Openaccountdata//
    {
        public string gdNumber { get; set; }
    }

    public class Cashmargin//
    {
        public int cashmarginPercentage { get; set; }
        public int cashmarginValue { get; set; }
    }

    public class Contractcollectiondata//
    {
        public int advPayPercentage { get; set; }
        public int docAgainstPayPercentage { get; set; }
        public int docAgainstAcceptancePercentage { get; set; }
        public int days { get; set; }
        public int totalPercentage { get; set; }
    }

    public class Lcdata//
    {
        public int advPayPercentage { get; set; }
        public int sightPercentage { get; set; }
        public int usancePercentage { get; set; }
        public int days { get; set; }
        public int totalPercentage { get; set; }
    }

    public class Paymentinformation
    {
        public string beneficiaryName { get; set; }
        public string beneficiaryAddress { get; set; }
        public string beneficiaryCountry { get; set; }
        public string beneficiaryIban { get; set; }
        public string exporterName { get; set; }
        public string exporterAddress { get; set; }
        public string exporterCountry { get; set; }
        public string portOfShipment { get; set; }
        public string deliveryTerms { get; set; }
        public double financialInstrumentValue { get; set; }
        public string financialInstrumentCurrency { get; set; }
        public double exchangeRate { get; set; }
        public string lcContractNo { get; set; }
    }

    public class Financialtraninformation
    {
        public string intendedPayDate { get; set; }
        public string transportDocDate { get; set; }
        public string finalDateOfShipment { get; set; }
        public string expiryDate { get; set; }
    }

}
