using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ExportOverDueFileUploader.Modles.JsonHelper
{
    public class GDJson
    {
    }

    

    public class GdPayload
    {
        public string messageId { get; set; }
        public string timestamp { get; set; }
        public string senderId { get; set; }
        public string receiverId { get; set; }
        public string processingCode { get; set; }
        public Data data { get; set; }
        public string signature { get; set; }
    }

   

    //public class Data
    //{

    //    public List<Data>? gdInfo { get; set; }
        
    //    public string gdNumber { get; set; }
    //    public string gdStatus { get; set; }
    //    public string consignmentCategory { get; set; }

    //    public string blAwbDate { get; set; }
    //    // Add other properties as needed
    //    public ConsignorConsigneeInfo? consignorConsigneeInfo { get; set; }
    //    public FinancialInformation? financialInformation { get; set; }
    //    public GeneralInformation? generalInformation { get; set; }
    //    public FinancialInfo? financialInfo { get; set; }
    //    public object? itemInformation { get; set; }
    //}

    public class ConsignorConsigneeInfo
    {
        public string ntnFtn { get; set; }
        public string strn { get; set; }
        public string consigneeName { get; set; }
        public string consigneeAddress { get; set; }
        public string consignorName { get; set; }
        public string consignorAddress { get; set; }
    }
    public class FinancialInformation
    {
        public string modeOfPayment { get; set; }//
        public string finInsUniqueNumber { get; set; }
        public List<FinancialInstrument> financialInstrument { get; set; }
        public string currency { get; set; }
        public long totalDeclaredValue { get; set; }

        public float? exchangeRate { get; set; }
        // Add other properties as needed
    }

    public class GeneralInformation
    {
        public List<PackagesInformation> packagesInformation { get; set; }
        public List<ContainerVehicleInformation> containerVehicleInformation { get; set; }

        public string destinationCountry { get; set; }
        public string clearanceDate { get; set; }

        public DateTime? clearanceDateValue
        {
            get
            {
                if (!clearanceDate.IsNullOrEmpty())
                {

                    int a = Convert.ToInt16(clearanceDate.Substring(6, 2));
                    int b = Convert.ToInt16(clearanceDate.Substring(4, 2));
                    int d = Convert.ToInt16(clearanceDate.Substring(0, 4));
                    return new DateTime(d, b, a);
                }
                else { return null; }
            }
        }
        // Add other properties as needed
    }

    public class FinancialInfo
    {
        public string importerIban { get; init; }
        public string modeOfPayment { get; init; }
        public string finInsUniqueNumber { get; init; }
        public string currency { get; init; }
        public double totalDeclaredValue { get; init; }
        public double exchangeRate { get; init; }
    }
    public class FinancialInstrument
    {
        public string exporterIban { get; set; }
        public string modeOfPayment { get; set; }
        public string finInsUniqueNumber { get; set; }
        public double finInsConsumedValue { get; set; }
    }
    public class PackagesInformation
    {
        public double numberOfPackages { get; set; }
        public string packageType { get; set; }
    }

    public class ContainerVehicleInformation
    {
        public string containerOrTruckNumber { get; set; }
        public string sealNumber { get; set; }
        public string containerType { get; set; }
    }


}
