using ExportOverDueFileUploader.Modles.JsonHelper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExportOverDueFileUploader.DataImporter
{

    public static class GdImporter
    {
        public static List<string> GdColumns = new List<string>
        {
            "gdNumber",
            "blDate",
            "totalDeclaredValue",
            "LstfinInsUniqueNumbers",
            "CurrencyCode",
            "ShipmentDate",
            "itemInformationJson",
            "exchangeRate",
            "ShipmentCity",
            "GDDate",
            "consigneeName",
            "finInsUniqueNumber",
            "modeOfPayment"
        };

        public static void LoadGdInfoColoums(DataRow _row)
        {
            try
            {
                GdPayload payload = JsonConvert.DeserializeObject<GdPayload>(_row["PAYLOAD"]?.ToString());
                List<string> lstFiNumber = new List<string>();
                List<string> lstModeOfPayment = new List<string>();

                if (payload?.data?.financialInformation?.financialInstrument != null)
                {
                    foreach (var financialInstrument in payload?.data?.financialInformation?.financialInstrument)
                    {
                        lstFiNumber.Add($"{financialInstrument.finInsUniqueNumber?.ToString()}({financialInstrument.modeOfPayment?.ToString()})");
                    }
                    _row["totalDeclaredValue"] = payload?.data?.financialInformation?.totalDeclaredValue;
                    _row["CurrencyCode"] = payload?.data?.financialInformation?.currency?.ToString();
                    _row["exchangeRate"] = payload?.data?.financialInformation?.exchangeRate ?? 0;


                }
                else if (payload?.data?.financialInfo != null)
                {


                    _row["finInsUniqueNumber"] = payload?.data?.financialInfo?.finInsUniqueNumber;
                    _row["modeOfPayment"] = payload?.data?.financialInfo?.modeOfPayment;
                    _row["totalDeclaredValue"] = payload?.data?.financialInfo?.totalDeclaredValue;
                    _row["CurrencyCode"] = payload?.data?.financialInfo?.currency?.ToString();
                    _row["exchangeRate"] = payload?.data?.financialInfo?.exchangeRate ?? 0;
                }

                _row["gdNumber"] = payload?.data?.gdNumber?.ToString();
                _row["consigneeName"] = payload?.data?.consignorConsigneeInfo?.consigneeName.ToString();
                _row["LstfinInsUniqueNumbers"] = lstFiNumber.Count > 0 ? string.Join(",", lstFiNumber) : null;
                _row["blDate"] = payload?.data?.blAwbDate?.ToString();
                _row["ShipmentDate"] = payload?.data?.blAwbDate?.ToString();
                _row["itemInformationJson"] = payload?.data?.itemInformation != null ? JsonConvert.SerializeObject(payload?.data?.itemInformation) : null;
                _row["ShipmentCity"] = payload?.data?.generalInformation?.destinationCountry;//ask
                _row["blDate"] = payload?.data?.blAwbDate?.ToString();

                if (payload?.data?.gdNumber != null)
                {

                    var lstgddate = payload?.data?.gdNumber?.ToString().Split('-').ToList().Skip(3).Take(3).ToList();
                    _row["GDDate"] = new DateTime(Convert.ToInt16(lstgddate[2]), Convert.ToInt16(lstgddate[1]), Convert.ToInt16(lstgddate[0]));
                }
            }
            catch
            {
                return;
            }

        }





    }
}
