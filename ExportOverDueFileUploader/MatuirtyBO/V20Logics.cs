using ExportOverDueFileUploader.DBmodels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExportOverDueFileUploader.MatuirtyBO
{
    public static class V20Logics
    {
        public static List<GD_FI_Link> GetV20Date(long? FiId, long? gdId, string FiNumber, double Total_Declared_Value, int Sight_Percentage, int Usance_Percentage, int DocAgainstPayment, int DocumentAgainstAceptance, int Adv_Pay_Percentage, string shipmenttype, DateTime? certificateDate, int GD_Days, DateTime GDDate, DateTime? blDate, string modeOfPayment, string GdNumber, DateTime GDActualValue)
        {
            #region Tenor logic New

            List<GD_FI_Link> lsGDDateSplitter = new List<GD_FI_Link>();
            int days = GD_Days;
            DateTime certificatedate = Convert.ToDateTime(certificateDate);
            string shipmentType = shipmenttype;

            #region Days
            //if (shipmentType == "305")
            //{
            //    day = days;
            //    if (day == 0)
            //    {
            //        day = 45;
            //    }
            //}

            if (shipmentType == "306") //ADP
            {
                //model.stCode = strStCode;
                ////certificatedate + 365;
                //day = 365;
                GD_FI_Link gDDateSplitter = new GD_FI_Link();
                gDDateSplitter.FiId = FiId;
                gDDateSplitter.GdId = gdId;

                gDDateSplitter.type = "Adv_Pay";
                gDDateSplitter._MatruityDate = certificatedate.AddDays(365);
                gDDateSplitter.MatruityDate = gDDateSplitter._MatruityDate.Value.ToString("dd-MMM-yyy");
                //ParameterFilling(FiNumber, Total_Declared_Value, certificateDate, GDActualValue, blDate, modeOfPayment, GdNumber, days, gDDateSplitter);
                lsGDDateSplitter.Add(gDDateSplitter);
                return lsGDDateSplitter;
            }
            else if (DocAgainstPayment > 0 && DocumentAgainstAceptance > 0)
            {
                if (DocAgainstPayment > 0)
                {
                    GD_FI_Link gDDateSplitter = new GD_FI_Link();
                    gDDateSplitter.FiId = FiId;
                    gDDateSplitter.GdId = gdId;
                    gDDateSplitter.type = "DocAgainstPayment";
                    gDDateSplitter.Amount = (Total_Declared_Value * DocAgainstPayment) / 100;
                    gDDateSplitter._MatruityDate = GDDate.AddDays(45);
                    gDDateSplitter.docAgainstPayPercentage = DocAgainstPayment;
                    gDDateSplitter.MatruityDate = gDDateSplitter._MatruityDate.Value.ToString("dd-MMM-yyy");
                    //   ParameterFilling(FiNumber, Total_Declared_Value, certificateDate, GDActualValue, blDate, modeOfPayment, GdNumber, days, gDDateSplitter);
                    lsGDDateSplitter.Add(gDDateSplitter);
                }
                if (DocumentAgainstAceptance > 0)
                {
                    //model.DAValue = (Total_Declared_Value * DA) / 100;
                    //model.DAValueDate = GDDate.AddDays(days);
                    //day = days;//2
                    GD_FI_Link gDDateSplitter = new GD_FI_Link();
                    gDDateSplitter.FiId = FiId;
                    gDDateSplitter.GdId = gdId;
                    gDDateSplitter.type = "DocumentAgainstAceptance";
                    // gDDateSplitter. = modeOfPayment;
                    gDDateSplitter.Amount = (Total_Declared_Value * DocumentAgainstAceptance) / 100;
                    gDDateSplitter._MatruityDate = GDDate.AddDays(days);
                    //gDDateSplitter.FINumber = FiNumber;
                    gDDateSplitter.docAgainstAcceptancePercentage = DocumentAgainstAceptance;
                    gDDateSplitter.MatruityDate = gDDateSplitter._MatruityDate.Value.ToString("dd-MMM-yyy");
                    //ParameterFilling(FiNumber, Total_Declared_Value, certificateDate, GDActualValue, blDate, modeOfPayment, GdNumber, days, gDDateSplitter);
                    lsGDDateSplitter.Add(gDDateSplitter);
                }
            }
            else if (DocAgainstPayment > 0 && Adv_Pay_Percentage > 0)
            {
                if (DocAgainstPayment > 0)
                {
                    //model.DPValue = (Total_Declared_Value * DP) / 100;
                    //model.DPValueDate = GDDate.AddDays(45);
                    //day = 45;//1
                    GD_FI_Link gDDateSplitter = new GD_FI_Link();
                    gDDateSplitter.FiId = FiId;
                    gDDateSplitter.GdId = gdId;
                    gDDateSplitter.type = "DocAgainstPayment";
                    // gDDateSplitter.modeOfPayment = modeOfPayment;
                    gDDateSplitter.Amount = (Total_Declared_Value * DocAgainstPayment) / 100;
                    gDDateSplitter._MatruityDate = GDDate.AddDays(45);
                    //gDDateSplitter.FINumber = FiNumber;
                    gDDateSplitter.docAgainstPayPercentage = DocAgainstPayment;
                    // ParameterFilling(FiNumber, Total_Declared_Value, certificateDate, GDActualValue, blDate, modeOfPayment, GdNumber, days, gDDateSplitter);
                    lsGDDateSplitter.Add(gDDateSplitter);
                }
                if (Adv_Pay_Percentage > 0)
                {
                    //model.ADPValue = (Total_Declared_Value * ADP) / 100;
                    //model.ADPValueDate = certificatedate.AddDays(365);
                    GD_FI_Link gDDateSplitter = new GD_FI_Link();
                    gDDateSplitter.FiId = FiId;
                    gDDateSplitter.GdId = gdId;
                    gDDateSplitter.type = "Adv_Pay";
                    //  gDDateSplitter.modeOfPayment = modeOfPayment;
                    gDDateSplitter.Amount = (Total_Declared_Value * Adv_Pay_Percentage) / 100;
                    gDDateSplitter._MatruityDate = certificatedate.AddDays(365);
                    // gDDateSplitter.FINumber = FiNumber;
                    gDDateSplitter.advPayPercentage = Adv_Pay_Percentage;
                    gDDateSplitter.MatruityDate = gDDateSplitter._MatruityDate.Value.ToString("dd-MMM-yyy");
                    /// ParameterFilling(FiNumber, Total_Declared_Value, certificateDate, GDActualValue, blDate, modeOfPayment, GdNumber, days, gDDateSplitter);
                    lsGDDateSplitter.Add(gDDateSplitter);
                    //days = certificate  date + 365
                    // certificatedate.AddDays(365); //2
                }
            }
            else if (DocumentAgainstAceptance > 0 && Adv_Pay_Percentage > 0)
            {

                if (DocumentAgainstAceptance > 0)
                {
                    //model.DAValue = (Total_Declared_Value * DA) / 100;
                    //model.DAValueDate = GDDate.AddDays(days);
                    //day = days;//2
                    GD_FI_Link gDDateSplitter = new GD_FI_Link();
                    gDDateSplitter.FiId = FiId;
                    gDDateSplitter.GdId = gdId;
                    gDDateSplitter.type = "DocumentAgainstAceptance";
                    //  gDDateSplitter.modeOfPayment = modeOfPayment;
                    gDDateSplitter.Amount = (Total_Declared_Value * DocumentAgainstAceptance) / 100;
                    gDDateSplitter._MatruityDate = GDDate.AddDays(days);
                    //gDDateSplitter.FINumber = FiNumber;
                    gDDateSplitter.docAgainstAcceptancePercentage = DocumentAgainstAceptance;
                    gDDateSplitter.MatruityDate = gDDateSplitter._MatruityDate.Value.ToString("dd-MMM-yyy");
                    //ParameterFilling(FiNumber, Total_Declared_Value, certificateDate, GDActualValue, blDate, modeOfPayment, GdNumber, days, gDDateSplitter);
                    lsGDDateSplitter.Add(gDDateSplitter);
                }
                if (Adv_Pay_Percentage > 0)
                {
                    //model.ADPValue = (Total_Declared_Value * ADP) / 100;
                    //model.ADPValueDate = certificatedate.AddDays(365);
                    GD_FI_Link gDDateSplitter = new GD_FI_Link();
                    gDDateSplitter.FiId = FiId;
                    gDDateSplitter.GdId = gdId;
                    gDDateSplitter.type = "Adv_Pay";
                    // gDDateSplitter.modeOfPayment = modeOfPayment;
                    gDDateSplitter.Amount = (Total_Declared_Value * Adv_Pay_Percentage) / 100;
                    gDDateSplitter._MatruityDate = certificatedate.AddDays(365);
                    //gDDateSplitter.FINumber = FiNumber;
                    gDDateSplitter.MatruityDate = gDDateSplitter._MatruityDate.Value.ToString("dd-MMM-yyy");
                    gDDateSplitter.advPayPercentage = Adv_Pay_Percentage;
                    gDDateSplitter.MatruityDate = gDDateSplitter._MatruityDate.Value.ToString("dd-MMM-yyy");
                    // ParameterFilling(FiNumber, Total_Declared_Value, certificateDate, GDActualValue, blDate, modeOfPayment, GdNumber, days, gDDateSplitter);
                    lsGDDateSplitter.Add(gDDateSplitter);
                    //days = certificate  date + 365
                    // certificatedate.AddDays(365); //2
                }
            }
            else if (DocAgainstPayment > 0 || Sight_Percentage > 0)
            {
                //day = 45;
                if (DocAgainstPayment > 0)
                {
                    GD_FI_Link gDDateSplitter = new GD_FI_Link();
                    gDDateSplitter.FiId = FiId;
                    gDDateSplitter.GdId = gdId;
                    gDDateSplitter.type = "DocAgainstPayment";
                    // gDDateSplitter.modeOfPayment = modeOfPayment;
                    gDDateSplitter._MatruityDate = GDDate.AddDays(45);
                    //gDDateSplitter.FINumber = FiNumber;
                    gDDateSplitter.docAgainstPayPercentage = DocAgainstPayment;
                    gDDateSplitter.MatruityDate = gDDateSplitter._MatruityDate.Value.ToString("dd-MMM-yyy");
                    //ParameterFilling(FiNumber, Total_Declared_Value, certificateDate, GDActualValue, blDate, modeOfPayment, GdNumber, days, gDDateSplitter);
                    lsGDDateSplitter.Add(gDDateSplitter);
                }
                else
                {
                    GD_FI_Link gDDateSplitter = new GD_FI_Link();
                    gDDateSplitter.FiId = FiId;
                    gDDateSplitter.GdId = gdId;
                    gDDateSplitter.type = "Sight";
                    // gDDateSplitter.modeOfPayment = modeOfPayment;
                    gDDateSplitter._MatruityDate = GDDate.AddDays(45);
                    //gDDateSplitter.FINumber = FiNumber;
                    gDDateSplitter.sightPercentage = Sight_Percentage;
                    gDDateSplitter.MatruityDate = gDDateSplitter._MatruityDate.Value.ToString("dd-MMM-yyy");
                    //ParameterFilling(FiNumber, Total_Declared_Value, certificateDate, GDActualValue, blDate, modeOfPayment, GdNumber, days, gDDateSplitter);
                    lsGDDateSplitter.Add(gDDateSplitter);
                }

            }
            else if (DocumentAgainstAceptance > 0 || Usance_Percentage > 0)
            {

                if (DocumentAgainstAceptance > 0)
                {
                    GD_FI_Link gDDateSplitter = new GD_FI_Link();
                    gDDateSplitter.FiId = FiId;
                    gDDateSplitter.GdId = gdId;
                    gDDateSplitter.type = "DocumentAgainstAceptance";
                    //gDDateSplitter.modeOfPayment = modeOfPayment;
                    gDDateSplitter._MatruityDate = GDDate.AddDays(days);
                    //gDDateSplitter.FINumber = FiNumber;
                    gDDateSplitter.docAgainstAcceptancePercentage = DocumentAgainstAceptance;
                    gDDateSplitter.MatruityDate = gDDateSplitter._MatruityDate.Value.ToString("dd-MMM-yyy");
                    // ParameterFilling(FiNumber, Total_Declared_Value, certificateDate, GDActualValue, blDate, modeOfPayment, GdNumber, days, gDDateSplitter);
                    lsGDDateSplitter.Add(gDDateSplitter);
                }
                else
                {
                    GD_FI_Link gDDateSplitter = new GD_FI_Link();
                    gDDateSplitter.FiId = FiId;
                    gDDateSplitter.GdId = gdId;
                    gDDateSplitter.type = "Usance";
                    //gDDateSplitter.modeOfPayment = modeOfPayment;
                    gDDateSplitter._MatruityDate = GDDate.AddDays(days);
                    // gDDateSplitter.FINumber = FiNumber;
                    gDDateSplitter.usancePercentage = Usance_Percentage;
                    gDDateSplitter.MatruityDate = gDDateSplitter._MatruityDate.Value.ToString("dd-MMM-yyy");
                    //ParameterFilling(FiNumber, Total_Declared_Value, certificateDate, GDActualValue, blDate, modeOfPayment, GdNumber, days, gDDateSplitter);
                    lsGDDateSplitter.Add(gDDateSplitter);
                }

            }
            #endregion

            return lsGDDateSplitter;

            #endregion
        }

        //public static List<GD_FI_Link> GetV20Date(string FiNumber, double Total_Declared_Value, int Sight_Percentage, int Usance_Percentage, int DocAgainstPayment, int DocumentAgainstAceptance, int Adv_Pay_Percentage, string shipmenttype, DateTime? certificateDate, int GD_Days, string modeOfPayment)
        //{
        //    #region Tenor logic New

        //    List<GD_FI_Link> lsGDDateSplitter = new List<GD_FI_Link>();
        //    int days = GD_Days;
        //    DateTime certificatedate = Convert.ToDateTime(certificateDate);
        //    string shipmentType = shipmenttype;

        //    if (shipmentType == "306") //ADP
        //    {
        //        GD_FI_Link gDDateSplitter = new GD_FI_Link();
        //        gDDateSplitter.type = "Adv_Pay";
        //        gDDateSplitter._MatruityDate = certificatedate.AddDays(365);
        //        ParameterFilling(FiNumber, Total_Declared_Value, certificateDate, modeOfPayment, days, gDDateSplitter);
        //        lsGDDateSplitter.Add(gDDateSplitter);
        //        return lsGDDateSplitter;
        //    }
        //    else if (DocAgainstPayment > 0 && DocumentAgainstAceptance > 0)
        //    {
        //        if (DocAgainstPayment > 0)
        //        {
        //            GD_FI_Link gDDateSplitter = new GD_FI_Link();
        //            gDDateSplitter.type = "DocAgainstPayment";
        //            gDDateSplitter._MatruityDate = certificatedate.AddDays(45);
        //            gDDateSplitter.docAgainstPayPercentage = DocAgainstPayment;
        //            ParameterFilling(FiNumber, Total_Declared_Value, certificateDate, modeOfPayment, days, gDDateSplitter);
        //            lsGDDateSplitter.Add(gDDateSplitter);
        //        }
        //        if (DocumentAgainstAceptance > 0)
        //        {
        //            //model.DAValue = (Total_Declared_Value * DA) / 100;
        //            //model.DAValueDate = GDDate.AddDays(days);
        //            //day = days;//2
        //            GD_FI_Link gDDateSplitter = new GD_FI_Link();
        //            gDDateSplitter.type = "DocumentAgainstAceptance";
        //            gDDateSplitter.modeOfPayment = modeOfPayment;
        //            gDDateSplitter._MatruityDate = certificatedate.AddDays(days);
        //            gDDateSplitter.FINumber = FiNumber;
        //            gDDateSplitter.docAgainstAcceptancePercentage = DocumentAgainstAceptance;
        //            ParameterFilling(FiNumber, Total_Declared_Value, certificateDate, modeOfPayment, days, gDDateSplitter);
        //            lsGDDateSplitter.Add(gDDateSplitter);
        //        }
        //    }
        //    else if (DocAgainstPayment > 0 && Adv_Pay_Percentage > 0)
        //    {
        //        if (DocAgainstPayment > 0)
        //        {
        //            //model.DPValue = (Total_Declared_Value * DP) / 100;
        //            //model.DPValueDate = GDDate.AddDays(45);
        //            //day = 45;//1
        //            GD_FI_Link gDDateSplitter = new GD_FI_Link();
        //            gDDateSplitter.type = "DocAgainstPayment";
        //            gDDateSplitter.modeOfPayment = modeOfPayment;
        //            gDDateSplitter._MatruityDate = certificatedate.AddDays(45);
        //            gDDateSplitter.FINumber = FiNumber;
        //            gDDateSplitter.docAgainstPayPercentage = DocAgainstPayment;
        //            ParameterFilling(FiNumber, Total_Declared_Value, certificateDate, modeOfPayment, days, gDDateSplitter);
        //            lsGDDateSplitter.Add(gDDateSplitter);
        //        }
        //        if (Adv_Pay_Percentage > 0)
        //        {
        //            //model.ADPValue = (Total_Declared_Value * ADP) / 100;
        //            //model.ADPValueDate = certificatedate.AddDays(365);
        //            GD_FI_Link gDDateSplitter = new GD_FI_Link();
        //            gDDateSplitter.type = "Adv_Pay";
        //            gDDateSplitter.modeOfPayment = modeOfPayment;
        //            gDDateSplitter._MatruityDate = certificatedate.AddDays(365);
        //            gDDateSplitter.FINumber = FiNumber;
        //            gDDateSplitter.advPayPercentage = Adv_Pay_Percentage;
        //            ParameterFilling(FiNumber, Total_Declared_Value, certificateDate, modeOfPayment, days, gDDateSplitter);
        //            lsGDDateSplitter.Add(gDDateSplitter);
        //        }
        //    }
        //    else if (DocumentAgainstAceptance > 0 && Adv_Pay_Percentage > 0)
        //    {

        //        if (DocumentAgainstAceptance > 0)
        //        {
        //            //model.DAValue = (Total_Declared_Value * DA) / 100;
        //            //model.DAValueDate = GDDate.AddDays(days);
        //            //day = days;//2
        //            GD_FI_Link gDDateSplitter = new GD_FI_Link();
        //            gDDateSplitter.type = "DocumentAgainstAceptance";
        //            gDDateSplitter.modeOfPayment = modeOfPayment;
        //            gDDateSplitter._MatruityDate = certificatedate.AddDays(days);
        //            gDDateSplitter.FINumber = FiNumber;
        //            gDDateSplitter.docAgainstAcceptancePercentage = DocumentAgainstAceptance;
        //            ParameterFilling(FiNumber, Total_Declared_Value, certificateDate, modeOfPayment, days, gDDateSplitter);
        //            lsGDDateSplitter.Add(gDDateSplitter);
        //        }
        //        if (Adv_Pay_Percentage > 0)
        //        {
        //            //model.ADPValue = (Total_Declared_Value * ADP) / 100;
        //            //model.ADPValueDate = certificatedate.AddDays(365);
        //            GD_FI_Link gDDateSplitter = new GD_FI_Link();
        //            gDDateSplitter.type = "Adv_Pay";
        //            gDDateSplitter.modeOfPayment = modeOfPayment;
        //            gDDateSplitter._MatruityDate = certificatedate.AddDays(365);
        //            gDDateSplitter.FINumber = FiNumber;
        //            gDDateSplitter.advPayPercentage = Adv_Pay_Percentage;
        //            ParameterFilling(FiNumber, Total_Declared_Value, certificateDate, modeOfPayment, days, gDDateSplitter);
        //            lsGDDateSplitter.Add(gDDateSplitter);
        //        }
        //    }
        //    else if (DocAgainstPayment > 0 || Sight_Percentage > 0)
        //    {
        //        //day = 45;
        //        if (DocAgainstPayment > 0)
        //        {
        //            GD_FI_Link gDDateSplitter = new GD_FI_Link();
        //            gDDateSplitter.type = "DocAgainstPayment";
        //            gDDateSplitter.modeOfPayment = modeOfPayment;
        //            gDDateSplitter._MatruityDate = certificatedate.AddDays(45);
        //            gDDateSplitter.FINumber = FiNumber;
        //            gDDateSplitter.docAgainstPayPercentage = DocAgainstPayment;
        //            ParameterFilling(FiNumber, Total_Declared_Value, certificateDate, modeOfPayment, days, gDDateSplitter);
        //            lsGDDateSplitter.Add(gDDateSplitter);
        //        }
        //        else
        //        {
        //            GD_FI_Link gDDateSplitter = new GD_FI_Link();
        //            gDDateSplitter.type = "Sight";
        //            gDDateSplitter.modeOfPayment = modeOfPayment;
        //            gDDateSplitter._MatruityDate = certificatedate.AddDays(45);
        //            gDDateSplitter.FINumber = FiNumber;
        //            gDDateSplitter.sightPercentage = Sight_Percentage;
        //            ParameterFilling(FiNumber, Total_Declared_Value, certificateDate, modeOfPayment, days, gDDateSplitter);
        //            lsGDDateSplitter.Add(gDDateSplitter);
        //        }

        //    }
        //    else if (DocumentAgainstAceptance > 0 || Usance_Percentage > 0)
        //    {

        //        if (DocumentAgainstAceptance > 0)
        //        {
        //            GD_FI_Link gDDateSplitter = new GD_FI_Link();
        //            gDDateSplitter.type = "DocumentAgainstAceptance";
        //            gDDateSplitter.modeOfPayment = modeOfPayment;
        //            gDDateSplitter._MatruityDate = certificatedate.AddDays(days);
        //            gDDateSplitter.FINumber = FiNumber;
        //            gDDateSplitter.docAgainstAcceptancePercentage = DocumentAgainstAceptance;
        //            ParameterFilling(FiNumber, Total_Declared_Value, certificateDate, modeOfPayment, days, gDDateSplitter);
        //            lsGDDateSplitter.Add(gDDateSplitter);
        //        }
        //        else
        //        {
        //            GD_FI_Link gDDateSplitter = new GD_FI_Link();
        //            gDDateSplitter.type = "Usance";
        //            gDDateSplitter.modeOfPayment = modeOfPayment;
        //            gDDateSplitter._MatruityDate = certificatedate.AddDays(days);
        //            gDDateSplitter.FINumber = FiNumber;
        //            gDDateSplitter.usancePercentage = Usance_Percentage;
        //            ParameterFilling(FiNumber, Total_Declared_Value, certificateDate, modeOfPayment, days, gDDateSplitter);
        //            lsGDDateSplitter.Add(gDDateSplitter);
        //        }

        //    }
        //    #endregion

        //    return lsGDDateSplitter;
        //}


        //private static void ParameterFilling(string FiNumber, double Total_Declared_Value, DateTime? certificateDate, DateTime GDDate, DateTime? blDate, string modeOfPayment, string GdNumber, int days, GD_FI_Link gDDateSplitter)
        //{
        //    gDDateSplitter.modeOfPayment = modeOfPayment;
        //    gDDateSplitter.GDNumber = GdNumber;
        //    gDDateSplitter.GDDate = GDDate.ToString("dd-MMM-yyyy");
        //    gDDateSplitter.FINumber = FiNumber;
        //    gDDateSplitter.FICertificateDate = certificateDate.HasValue ? certificateDate.Value.ToString("dd-MMM-yyyy") : null;
        //    gDDateSplitter.days = days;
        //    gDDateSplitter.Total_Declared_Value = Total_Declared_Value;
        //    gDDateSplitter.BlDate = blDate.HasValue ? blDate.Value.ToString("dd-MMM-yyy") : null;
        //    gDDateSplitter.MatruityDate = gDDateSplitter._MatruityDate.Value.ToString("dd-MMM-yyy");
        //}
        //private static void ParameterFilling(string FiNumber, double Total_Declared_Value, DateTime? certificateDate, string modeOfPayment, int days, GD_FI_Link gDDateSplitter)
        //{
        //    gDDateSplitter.modeOfPayment = modeOfPayment;
        //    gDDateSplitter.FINumber = FiNumber;
        //    gDDateSplitter.FICertificateDate = certificateDate.HasValue ? certificateDate.Value.ToString("dd-MMM-yyyy") : null;
        //    gDDateSplitter.days = days;
        //    gDDateSplitter.Total_Declared_Value = Total_Declared_Value;
        //    gDDateSplitter.MatruityDate = gDDateSplitter._MatruityDate.Value.ToString("dd-MMM-yyy");
        //}


    }
}