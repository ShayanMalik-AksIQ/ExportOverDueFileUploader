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
    public static class FiImporter
    {
        public static List<string> FiColoums = new List<string>
        {
            "paymentInformation",
            "lcData",
            "contractCollectionData",
            "openAccountGdNumber",
            "finInsUniqueNumber",
            "modeOfPayment",
            "ExporterCity",
            "ExporterAddress",
            "ExporterName",
            "ExporterRegNo",
            "ExporterNTN",
            "Days"
        };

        public static void LoadFIInfoColoums(DataRow _row)
        {
            try
            {
                FiPayLoadJson payload = JsonConvert.DeserializeObject<FiPayLoadJson>(_row["PAYLOAD"]?.ToString());
                if (payload != null)
                {
                    _row["paymentInformation"] = payload?.paymentInformation != null ? JsonConvert.SerializeObject(payload?.paymentInformation) : null;
                    _row["lcData"] = payload?.lcData != null ? JsonConvert.SerializeObject(payload.lcData) : null;
                    _row["contractCollectionData"] = payload.contractCollectionData != null ? JsonConvert.SerializeObject(payload.contractCollectionData) : null;
                    _row["openAccountGdNumber"] = payload.openAccountData?.gdNumber?.ToString();
                    _row["finInsUniqueNumber"] = payload.finInsUniqueNumber;
                    _row["modeOfPayment"] = payload.modeOfPayment;
                    _row["ExporterNTN"] = payload.exporterNtn;
                    _row["ExporterRegNo"] = payload.exporterNtn;
                    _row["ExporterAddress"] = payload.paymentInformation?.exporterAddress;
                    _row["ExporterCity"] = payload.paymentInformation?.exporterCountry;
                    _row["ExporterName"] = payload.exporterName;
                    _row["days"] = payload?.lcData?.days ?? (payload?.contractCollectionData?.days ?? 0);
                }

            }
            catch
            {
                return;
            }

        }
    }
}
