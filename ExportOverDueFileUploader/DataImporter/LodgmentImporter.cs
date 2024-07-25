using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ExportOverDueFileUploader.DataImporter
{
    public static class LodgmentImporter
    {
        static bool IsOnlyDigits(string input)
        {
            if (string.IsNullOrEmpty(input)) return false;
            string pattern = @"^[0-9]+$";
            return Regex.IsMatch(input, pattern);
        }
        static bool IsAmount(string input)
        {
            string pattern = @"^[0-9]*\.?[0-9]*$";
            return Regex.IsMatch(input, pattern);
        }

        public static void LoadLodgmentColoums(DataRow dataRow, string EntityName)
        {
            if (EntityName == "DocumentaryCollection")
            {
                if (dataRow["LiabilityAmount"] == "" || dataRow["LiabilityAmount"] == null)
                {
                    dataRow["LiabilityAmount"] = "0";
                }
                if (dataRow["CollectionAmount"] == null || dataRow["CollectionAmount"] == "")
                {
                    dataRow["CollectionAmount"] = "0";
                }
                if (dataRow["DueDate"] != null && dataRow["DueDate"].ToString().Length == 8 && IsOnlyDigits(dataRow["DueDate"].ToString()))
                {
                    try
                    {

                        var date = dataRow["DueDate"].ToString();
                        int a = Convert.ToInt16(date.Substring(6, 2));
                        int b = Convert.ToInt16(date.Substring(4, 2));
                        int d = Convert.ToInt16(date.Substring(0, 4));


                        dataRow["DueDate"] = new DateTime(d, b, a);
                    }
                    catch
                    {

                        dataRow["DueDate"] = null;

                    }

                }
                else
                {
                    dataRow["DueDate"] = null;
                }
                if (dataRow["DateofLodgement"] != null && dataRow["DateofLodgement"].ToString().Length == 8 && IsOnlyDigits(dataRow["DateofLodgement"].ToString()))
                {
                    try
                    {
                        var date = dataRow["DateofLodgement"].ToString();
                        int a = Convert.ToInt16(date.Substring(6, 2));
                        int b = Convert.ToInt16(date.Substring(4, 2));
                        int d = Convert.ToInt16(date.Substring(0, 4));


                        dataRow["DateofLodgement"] = new DateTime(d, b, a);
                    }
                    catch
                    {
                        dataRow["DateofLodgement"] = null;


                    }


                }
                else
                {
                    dataRow["DateofLodgement"] = null;
                }


            }
            else
                if (EntityName == "LetterOfCredit")
            {


                if (!IsAmount(dataRow["LcAmount"].ToString()) || (
                    dataRow["LcAmount"] == "" || dataRow["LcAmount"] == null)
                    )
                {
                    dataRow["LcAmount"] = "0";
                }
                if (dataRow["DateofLodgement"] != null && IsOnlyDigits(dataRow["DateofLodgement"].ToString()) && dataRow["DateofLodgement"].ToString().Length == 8)
                {
                    try
                    {
                        var date = dataRow["DateofLodgement"].ToString();
                        int a = Convert.ToInt16(date.Substring(6, 2));
                        int b = Convert.ToInt16(date.Substring(4, 2));
                        int d = Convert.ToInt16(date.Substring(0, 4));


                        dataRow["DateofLodgement"] = new DateTime(d, b, a);
                    }
                    catch
                    {
                        dataRow["DateofLodgement"] = null;


                    }


                }
                else
                {
                    dataRow["DateofLodgement"] = null;
                }

                if (dataRow["DueDate"] != null && dataRow["DueDate"].ToString().Length == 8 && IsOnlyDigits(dataRow["DueDate"].ToString()))
                {
                    try
                    {
                        var date = dataRow["DueDate"].ToString();
                        int a = Convert.ToInt16(date.Substring(6, 2));
                        int b = Convert.ToInt16(date.Substring(4, 2));
                        int d = Convert.ToInt16(date.Substring(0, 4));


                        dataRow["DueDate"] = new DateTime(d, b, a);
                    }
                    catch
                    {

                        dataRow["DueDate"] = null;

                    }

                }
                else
                {
                    dataRow["DueDate"] = null;
                }
            }

        }


       
    }
}
