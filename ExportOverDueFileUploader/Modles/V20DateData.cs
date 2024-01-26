using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExportOverDueFileUploader.Modles
{
    public class V20DateData
    {
        public string FINumber { get; set; }
        public string GDNumber { get; set; }
        public string FICertificateDate { get; set; }
        public string GDDate { get; set; }
        public string BlDate { get; set; }
        public int days { get; set; }
        public string modeOfPayment { get; set; }
        public double Total_Declared_Value { get; set; }
        public string type { get; set; }
        public double Amount { get; set; }
        public int advPayPercentage { get; set; }
        public int docAgainstPayPercentage { get; set; }
        public int docAgainstAcceptancePercentage { get; set; }
        public int sightPercentage { get; set; }
        public int usancePercentage { get; set; }
        public string MatruityDate { get; set; }
        public DateTime _MatruityDate { get; set; }
    }
}
