using DocumentFormat.OpenXml.Drawing.Charts;
using ExportOverDueFileUploader.DBmodels;
using ExportOverDueFileUploader.Modles.JsonHelper;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ExportOverDueFileUploader.DataImporter
{

    public static class GdImporter
    {
        public static List<string> GdColumns = new List<string>
        {
            "gdNumber",
            "gdStatus",
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

        public static List<string> LoadGdInfoColoums(DataRow _row)
        {
            try
            {
                List<string> fiNumber = new List<string>();
                GdPayload payload = JsonConvert.DeserializeObject<GdPayload>(_row["PAYLOAD"]?.ToString());
                List<string> lstFiNumber = new List<string>();
                List<string> lstModeOfPayment = new List<string>();

                if (payload?.data?.financialInformation?.financialInstrument != null)
                {
                    foreach (var financialInstrument in payload?.data?.financialInformation?.financialInstrument)
                    {
                        if (financialInstrument.finInsUniqueNumber != null)
                        {

                            fiNumber.Add(financialInstrument.finInsUniqueNumber?.ToString());
                        }
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
                _row["gdStatus"] = payload?.data?.gdStatus?.ToString();
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

                return fiNumber;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public class  cobReturn{

            public List<string> fiNumbers { get; set; }
            public List<DataRow> aditionalRows { get; set; }
            }

        public static cobReturn LoadCobGdInfoColoums(DataRow _row)
        {
            try
            {
                if (!_row["PAYLOAD"].ToString().IsNullOrEmpty())
                {
                    List<string> fiNumber = new List<string>();
                    List<DataRow> dataRows = new List<DataRow>();
                     var x = _row["PAYLOAD"]?.ToString();
                    cobPayLoad payload = JsonConvert.DeserializeObject<cobPayLoad>(_row["PAYLOAD"]?.ToString());
                    if (payload?.data?.gdInfo.Count()>1)
                    {

                    }
                    var CountGds = payload?.data?.gdInfo.Count();
                        int i = 0;
                    foreach (var gd in payload?.data?.gdInfo)
                    {
                        if (gd != null)
                        {
                            List<string> lstFiNumber = new List<string>();
                            List<string> lstModeOfPayment = new List<string>();
                            if (gd?.financialInfo != null)
                            {
                                lstFiNumber.Add($"{gd.financialInfo?.finInsUniqueNumber?.ToString()}({gd.financialInfo.modeOfPayment?.ToString()})");

                                _row["totalDeclaredValue"] = gd.financialInfo.totalDeclaredValue;
                                _row["CurrencyCode"] = gd.financialInfo.currency?.ToString();
                                _row["exchangeRate"] = gd?.financialInfo.exchangeRate ?? 0;

                                if (gd.financialInfo?.finInsUniqueNumber != null)
                                {
                                    fiNumber.Add(gd?.financialInfo?.finInsUniqueNumber?.ToString());
                                }

                            }
                            _row["gdNumber"] = gd.gdNumber?.ToString();
                            _row["gdStatus"] = gd.gdStatus?.ToString();
                            _row["consigneeName"] = gd.consignorConsigneeInfo?.consigneeName.ToString();
                            _row["LstfinInsUniqueNumbers"] = lstFiNumber.Count > 0 ? string.Join(",", lstFiNumber) : null;
                            _row["blDate"] = gd.blawbDate?.ToString();
                            _row["ShipmentDate"] = gd.blawbDate?.ToString();
                            _row["itemInformationJson"] = gd.itemInformation != null ? JsonConvert.SerializeObject(gd.itemInformation) : null;
                            _row["ShipmentCity"] = gd.generalInformation?.destinationCountry;//ask
                            _row["blDate"] = gd.blawbDate?.ToString();

                            if (gd.gdNumber != null)
                            {
                                var lstgddate = gd.gdNumber?.ToString().Split('-').ToList().Skip(3).Take(3).ToList();
                                _row["GDDate"] = new DateTime(Convert.ToInt16(lstgddate[2]), Convert.ToInt16(lstgddate[1]), Convert.ToInt16(lstgddate[0]));
                            }
                            i++;
                            if (i != CountGds - 1)
                            {
                                dataRows.Add(DeepCopyDataRow(_row));
                            }
                            }
                    }
                    return new cobReturn
                    {
                        fiNumbers=fiNumber,
                        aditionalRows= dataRows

                    };
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                
                return null;
            }

        }

        public static System.Data.DataTable FilterGds(System.Data.DataTable gds)
        {
            // Create a new DataTable to store the filtered and processed data
            System.Data.DataTable latestRows = new System.Data.DataTable();
            latestRows = gds.Clone();

            // Apply filters and conversions similar to your SQL query
            foreach (DataRow row in gds.Rows)
            {
                if (row["DIRECTION"].ToString().Contains("REQUEST") &&
                    row["LstfinInsUniqueNumbers"] != null)
                {
                    DateTime convertedDateTime;
                    if (DateTime.TryParseExact(row["TRANSMISSION_DATETIME"].ToString(), "d MMM yyyy",
                        System.Globalization.CultureInfo.InvariantCulture,
                        System.Globalization.DateTimeStyles.None, out convertedDateTime))
                    {
                        latestRows.Rows.Add(row.ItemArray);
                    }
                }
            }

            // Filter the latest rows for each gdnumber
            System.Data.DataTable result = latestRows.Clone();
            foreach (DataRow groupRow in latestRows.AsEnumerable()
                .GroupBy(r => r.Field<string>("gdnumber"))
                .Select(g => g.OrderByDescending(r => r.Field<DateTime>("ConvertedDateTime")).First()))
            {
                result.ImportRow(groupRow);
            }

            return result;
        }

        public static DateTime? ConvertTransmissionDate(string dateString)
        {
            try
            {
                int lastDotIndex = dateString.LastIndexOf('.');
                string dateStringWithoutMilliseconds = dateString.Substring(0, lastDotIndex);
                string format = "dd-MMM-yy hh.mm.ss"; // Format without milliseconds
                DateTime result = DateTime.ParseExact(dateStringWithoutMilliseconds, format, System.Globalization.CultureInfo.InvariantCulture);
                string amPm = dateString.Substring(dateString.Length - 2);
                if (amPm == "PM")
                {
                    result.AddHours(12);
                }
                return result;
            }
            catch (Exception ex)
            {
                
                return null;
            }
        }


        public static DataRow DeepCopyDataRow(DataRow originalRow)
        {
            // Create a new DataRow with the same schema as the original
            System.Data.DataTable table = originalRow.Table;
            DataRow newRow = table.NewRow();

            // Copy each column value from the original DataRow to the new DataRow
            foreach (DataColumn column in table.Columns)
            {
                newRow[column.ColumnName] = originalRow[column.ColumnName];
            }

            return newRow;
        }


    }
}


