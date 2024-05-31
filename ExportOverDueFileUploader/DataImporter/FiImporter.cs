using ExportOverDueFileUploader.Modles.JsonHelper;
using Microsoft.IdentityModel.Tokens;
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
            "Days",
            "FiCertifcationdate"
        };

        //public static List<string> CobFiColoums = new List<string>
        //{
          
        //    "openAccountGdNumber",
        //    "modeOfPayment",
        //    "ExporterCity",
        //    "ExporterAddress",
        //    "ExporterName",
        //    "ExporterRegNo",
        //    "ExporterNTN",
        //    "Days",
        //    "FiCertifcationdate"
        //};

        public static List<string> BcaColoums = new List<string>
        {
            "RealizationInfoJson",
            "finInsUniqueNumber",
            "gdNumber",
            "bcaUniqueIdNumber",
            "BcaDate"

        };
        public static void LoadFIInfoColoums(DataRow _row)
        {
            try
            {
                var x = _row["PAYLOAD"]?.ToString();
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

                    if (!payload.finInsUniqueNumber.IsNullOrEmpty())
                    {
                        try
                        {

                            string ctdate = payload.finInsUniqueNumber.Split('-').ToList().LastOrDefault();
                            int a = Convert.ToInt16(ctdate.Substring(0, 2));
                            int b = Convert.ToInt16(ctdate.Substring(2, 2));
                            int d = Convert.ToInt16(ctdate.Substring(4, 4));

                            _row["FiCertifcationdate"] = new DateTime(d, b, a);
                        }
                        catch
                        {
                            _row["FiCertifcationdate"] = null;
                        }

                    }
                    else
                    {
                        _row["FiCertifcationdate"] = null;
                    }
                    if(_row["TRANSACTION_TYPE"].ToString().IsNullOrEmpty() && _row["CREATED_DATETIME"].ToString().IsNullOrEmpty())
                    {
                        _row["TRANSACTION_TYPE"] = "1524";
                    }
                }

            }
            catch
            {
                //Console.WriteLine(_row["ResponceCode"]?.ToString());
                return;
            }

        }

        public static void LoadBcaInfoColoums(DataRow _row)
        {
            try
            {
                BCAPayLoad payload = JsonConvert.DeserializeObject<BCAPayLoad>(_row["PAYLOAD"]?.ToString());
                _row["bcaUniqueIdNumber"] = payload?.BcaUniqueIdNumber;
                _row["gdNumber"] = payload?.GdNumber;
                _row["finInsUniqueNumber"] = payload?.FinInsUniqueNumber;
                _row["RealizationInfoJson"] = payload?.NetAmountRealized != null ? JsonConvert.SerializeObject(payload.NetAmountRealized) : null;
                if (payload?.BcaUniqueIdNumber != null)
                {
                    string ctdate = payload?.BcaUniqueIdNumber?.Split('-').ToList().LastOrDefault();
                    int a = Convert.ToInt16(ctdate.Substring(0, 2));
                    int b = Convert.ToInt16(ctdate.Substring(2, 2));
                    int d = Convert.ToInt16(ctdate.Substring(4, 4));

                    _row["BcaDate"] = new DateTime(d, b, a);

             
                }

            }
            catch(Exception ex)
            {
                 return;
            }

        }


        
    }
}
