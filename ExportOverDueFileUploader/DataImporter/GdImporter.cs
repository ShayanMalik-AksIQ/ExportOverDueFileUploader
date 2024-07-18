using DocumentFormat.OpenXml.Drawing.Charts;
using ExportOverDueFileUploader.DBmodels;
using ExportOverDueFileUploader.Modles.JsonHelper;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
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
        public static List<string> GdImportColumns = new List<string>
        {
            "gdNumber",
            "gdStatus",
            "ImporterIban",
            "ImporterName",
            "ImporterNtn",
            "ModeOfPayment",
            "FinInsUniqueNumber",
            "GDDate"
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



                }
                else if (payload?.data?.financialInfo != null)
                {
                    _row["FinInsUniqueNumber"] = payload?.data?.financialInfo?.finInsUniqueNumber;
                    _row["ModeOfPayment"] = payload?.data?.financialInfo?.modeOfPayment;
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




        public static List<string> LoadImportGdInfoColoums(DataRow _row)
        {
            try
            {
                List<string> fiNumber = new List<string>();
                GdImportJson payload = JsonConvert.DeserializeObject<GdImportJson>(_row["PAYLOAD"]?.ToString());
                List<string> lstFiNumber = new List<string>();
                List<string> lstModeOfPayment = new List<string>();

                if (payload?.data?.financialInfo != null)
                {
                    fiNumber.Add(payload?.data?.financialInfo?.finInsUniqueNumber);
                    _row["FinInsUniqueNumber"] = payload?.data?.financialInfo?.finInsUniqueNumber;
                    _row["ModeOfPayment"] = payload?.data?.financialInfo?.modeOfPayment;
                    _row["gdNumber"] = payload?.data?.gdNumber?.ToString();
                    _row["gdStatus"] = payload?.data?.gdStatus?.ToString();
                    _row["ImporterNtn"] = payload?.data?.consignorConsigneeInfo?.ntnFtn.ToString();
                    _row["ImporterName"] = payload?.data?.consignorConsigneeInfo?.consigneeName.ToString();
                    _row["ImporterIban"] = payload?.data?.financialInfo?.importerIban;
                }



                if (payload?.data?.gdNumber != null)
                {
                    try
                    {
                        var lstgddate = payload?.data?.gdNumber?.ToString().Split('-').ToList().Skip(3).Take(3).ToList();
                        _row["GDDate"] = new DateTime(Convert.ToInt16(lstgddate[2]), Convert.ToInt16(lstgddate[1]), Convert.ToInt16(lstgddate[0]));

                    }
                    catch
                    {
                        _row["GDDate"] = null;
                    }
                }
                if (_row["TransmissionDate"].ToString() != null && _row["TransmissionDate"].ToString().Length==9)
                {
                    try
                    {
                        string format = "dd-MMM-yy";
                        CultureInfo provider = CultureInfo.InvariantCulture;

                        DateTime result = DateTime.ParseExact(_row["TransmissionDate"].ToString(), format, provider);
                        _row["TransmissionDate"] = result;

                    }
                    catch
                    {
                        _row["TransmissionDate"] = null;
                    }
                }

                return fiNumber;
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


