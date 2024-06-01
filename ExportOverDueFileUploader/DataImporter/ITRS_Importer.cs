using ExportOverDueFileUploader.Modles.JsonHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExportOverDueFileUploader.DataImporter
{
    public static class ITRS_Importer
    {
        public static void LoadITRSInfoColoums(DataRow dataRow)
        {

        }
        public static void LoadRelRptInfoColoums(DataRow dataRow)
        {
            if (dataRow["RealizationDate"] != null && dataRow["RealizationDate"].ToString().Length==8)
            {
                try
                {
                    var date = dataRow["RealizationDate"].ToString();
                    int a = Convert.ToInt16(date.Substring(6, 2));
                    int b = Convert.ToInt16(date.Substring(4, 2));
                    int d = Convert.ToInt16(date.Substring(0, 4));


                    dataRow["_RealizationDate"] = new DateTime(d, b, a);
                }
                catch {
                   


                }

            }
        }
    }
}
