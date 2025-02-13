using ExportOverDueFileUploader.DataImporter;
using ExportOverDueFileUploader.DBHelper;
using ExportOverDueFileUploader.DBmodels;
using ExportOverDueFileUploader.ValidateIqBizLogic.Comparison_V2;
using Microsoft.EntityFrameworkCore;

namespace ExportOverDueFileUploader.MatuirtyBO
{
    public static class LinkGdToFI
    {
        public static void SyncDcDueDates(string Entity)
        {

            try
            {
                string Query = "";
                var context = new ExportOverDueContext();
                Query = $"EXEC UpdateMatruityDateFromCollection";
                var result = context.Database.ExecuteSqlRaw(Query);
                Seriloger.LoggerInstance.Information($"{Entity} of Sync Success ");

            }
            catch (Exception ex)
            {
                Seriloger.LoggerInstance.Error("Error RemoveDublicateGds Data", ex.Message);

            }


        }

        //public static List<V20DateData> LoadMatureGds()
        //{
        //    ExportOverDueContext context = new ExportOverDueContext();
        //    int y = 0;
        //    List<GoodsDeclaration> lstgds = CustomRepo.GetGoodsDeclarationForV20Dates(AppSettings.TenantId).ToList();
        //    List<GD_FI_Link> V20Dates = new List<GD_FI_Link>();
        //    var FiDatas = CustomRepo.GetFinancialInstrumentForV20Dates(AppSettings.TenantId).ToList();
        //    foreach (var gd in lstgds)
        //    {
        //        List<GD_FI_Link> GdV20Dates = new List<GD_FI_Link>();
        //        if (gd.gdNumber == null)
        //        {
        //            continue;
        //        }
        //        if (!gd.LstfinInsUniqueNumbers.IsNullOrEmpty())
        //        {
        //            foreach (var item in gd.FiNumbersAndModes)
        //            {
        //                DateTime gdCreationDate = gd.GDDate.Value;
        //                var FiData = FiDatas.Where(x => x.finInsUniqueNumber == item.FiNumber).FirstOrDefault();
        //                if (item.FiNumber.IsNullOrEmpty())
        //                {
        //                    continue;
        //                }
        //                if (FiData != null)
        //                {
        //                    #region If Lc Data is Avaliable
        //                    if (FiData.lcData != null && FiData.lcData != "null")
        //                    {
        //                        DateTime gdDate = gd.GDDate.Value;
        //                        if (false)
        //                        {
        //                            int x = DateTime.Compare(gd.BLDateVale.Value, gd.GDDate.Value);
        //                            if (x > 0)
        //                            {
        //                                gdDate = gd.BLDateVale.Value;
        //                            }
        //                        }
        //                        Lcdata FiLcData = JsonConvert.DeserializeObject<Lcdata>(FiData.lcData);
        //                        try
        //                        {
        //                            GdV20Dates.AddRange(V20Logics.GetV20Date(FiData.Id, gd.Id, FiData.finInsUniqueNumber, gd.totalDeclaredValue.Value, FiLcData.sightPercentage, FiLcData.usancePercentage, 0, 0, FiLcData.advPayPercentage, FiData.modeOfPayment, FiData.FiCertifcationdate, FiLcData.days, gdDate, gd.BLDateVale, item.ModeOFPayment, gd.gdNumber, gd.GDDate.Value));
        //                        }
        //                        catch
        //                        {
        //                            continue;
        //                        }
        //                    }
        //                    #endregion If Lc Data is Avaliable
        //                    #region If Contract Collection Data is Avaliable
        //                    if (FiData.contractCollectionData != null && FiData.contractCollectionData != "null")
        //                    {
        //                        DateTime gdDate = gd.GDDate.Value;
        //                        if (false)
        //                        {
        //                            int x = DateTime.Compare(gd.BLDateVale.Value, gd.GDDate.Value);
        //                            if (x > 0)
        //                            {
        //                                gdDate = gd.BLDateVale.Value;
        //                            }
        //                        }
        //                        Contractcollectiondata FiCCData = JsonConvert.DeserializeObject<Contractcollectiondata>(FiData.contractCollectionData);
        //                        try
        //                        {
        //                            GdV20Dates.AddRange(V20Logics.GetV20Date(FiData.Id, gd.Id, FiData.finInsUniqueNumber, gd.totalDeclaredValue.Value, 0, 0, FiCCData.docAgainstPayPercentage, FiCCData.docAgainstAcceptancePercentage, FiCCData.advPayPercentage, FiData.modeOfPayment, FiData.FiCertifcationdate, FiCCData.days, gdDate, gd.BLDateVale, item.ModeOFPayment, gd.gdNumber, gd.GDDate.Value));
        //                        }
        //                        catch
        //                        {
        //                            continue;
        //                        }
        //                    }
        //                    #endregion If Contract Collection Data is Avaliable
        //                    #region If Mode of Payemnt 306
        //                    // ie Fi is avalible but no lc of cc attacheds
        //                    else if (FiData.modeOfPayment == "306")
        //                    {
        //                        DateTime gdDate = gd.GDDate.Value;
        //                        if (false)
        //                        {
        //                            int x = DateTime.Compare(gd.BLDateVale.Value, gd.GDDate.Value);
        //                            if (x > 0)
        //                            {
        //                                gdDate = gd.BLDateVale.Value;
        //                            }
        //                        }
        //                        try
        //                        {
        //                            GdV20Dates.AddRange(V20Logics.GetV20Date(FiData.Id, gd.Id, FiData.finInsUniqueNumber, gd.totalDeclaredValue.Value, 0, 0, 0, 0, 0, FiData.modeOfPayment, FiData.FiCertifcationdate, 0, gdDate, gd.BLDateVale, item.ModeOFPayment, gd.gdNumber, gd.GDDate.Value));
        //                        }
        //                        catch
        //                        {
        //                            continue;
        //                        }
        //                    }
        //                    #endregion If Mode of Payemnt 306
        //                    #region If Mode of Payemnt 305
        //                    // ie Fi is not avalible in gd
        //                    else if (FiData.modeOfPayment == "305")
        //                    {
        //                        DateTime gdDate = gd.GDDate.Value;
        //                        if (false)
        //                        {
        //                            int x = DateTime.Compare(gd.BLDateVale.Value, gd.GDDate.Value);
        //                            if (x > 0)
        //                            {
        //                                gdDate = gd.BLDateVale.Value;
        //                            }
        //                        }
        //                        GdV20Dates.Add(new GD_FI_Link()
        //                        {
        //                            GdId = gd.Id,
        //                            FiId = FiData.Id,
        //                            MatruityDate = gdDate.ToString("dd-MMM-yyy"),
        //                            _MatruityDate = gdDate
        //                        });
        //                    }
        //                    #endregion If Mode of Payemnt 305
        //                }
        //                else
        //                {
        //                    GdV20Dates.Add(new GD_FI_Link()
        //                    {
        //                        GdId = gd.Id,
        //                        //    GDNumber = gd.gdNumber,
        //                        //    FINumber = item.FiNumber + " - Not in FI File"
        //                    });
        //                }
        //            }
        //        }
        //        if (!GdV20Dates.IsNullOrEmpty())
        //        {


        //            y = y + GdV20Dates.Count;
        //            CustomRepo.InsertFI_GD_Link(GdV20Dates);
        //            // _customRepository.UpdateGD(gd.V20Felids, DateTime.Now, gd.Id);




        //        }
        //        V20Dates.AddRange(GdV20Dates);
        //    }
        //    CustomRepo.InsertFI_GD_Link(V20Dates);
        //    return null;
        //}

        //public static string SyncNewGd(long fileId, NewFiGdFilterModel fis_OpenGds)
        //{
        //    try
        //    {
        //        int count = 0;
        //        Seriloger.LoggerInstance.Information($" Sync New Gds In Process.... :");
        //        ExportOverDueContext context = new ExportOverDueContext();
        //        List<GD_FI_Link> V20Dates = new List<GD_FI_Link>();
        //        List<GoodsDeclaration> lstgds = CustomRepo.GetGoodsDeclarationForV20Dates(AppSettings.TenantId, fileId).ToList();//gd that newly came in 

        //        if (lstgds.Count == 0)
        //        {
        //            Seriloger.LoggerInstance.Information($"No Gds To Sync");

        //            return "No Gds";
        //        }
        //        var lstfis = CustomRepo.GetFinancialInstrumentForV20Dates(AppSettings.TenantId, fis_OpenGds).ToList();
        //        if (lstfis.Count == 0)
        //        {
        //            Seriloger.LoggerInstance.Information($"No Fis To Sync");

        //            //return "No Fis";
        //        }

        //        foreach (var gd in lstgds)
        //        {
        //            List<GD_FI_Link> GdV20Dates = new List<GD_FI_Link>();
        //            if (gd.gdNumber == null)
        //            {
        //                continue;
        //            }
        //            if (!gd.LstfinInsUniqueNumbers.IsNullOrEmpty())
        //            {
        //                foreach (var item in gd.FiNumbersAndModes)
        //                {
        //                    DateTime gdCreationDate = gd.GDDate.Value;
        //                    var FiData = lstfis.Where(x => x.finInsUniqueNumber == item.FiNumber).FirstOrDefault();
        //                    if (item.FiNumber.IsNullOrEmpty())
        //                    {
        //                        // continue;
        //                    }
        //                    if (FiData != null && !item.FiNumber.IsNullOrEmpty())
        //                    {
        //                        #region If Lc Data is Avaliable
        //                        if (FiData.lcData != null && FiData.lcData != "null")
        //                        {
        //                            DateTime gdDate = gd.GDDate.Value;
        //                            if (false)
        //                            {
        //                                int x = DateTime.Compare(gd.BLDateVale.Value, gd.GDDate.Value);
        //                                if (x > 0)
        //                                {
        //                                    gdDate = gd.BLDateVale.Value;
        //                                }
        //                            }
        //                            Lcdata FiLcData = JsonConvert.DeserializeObject<Lcdata>(FiData.lcData);
        //                            try
        //                            {
        //                                GdV20Dates.AddRange(V20Logics.GetV20Date(FiData.Id, gd.Id, FiData.finInsUniqueNumber, gd.totalDeclaredValue.Value, FiLcData.sightPercentage, FiLcData.usancePercentage, 0, 0, FiLcData.advPayPercentage, FiData.modeOfPayment, FiData.FiCertifcationdate, FiLcData.days, gdDate, gd.BLDateVale, item.ModeOFPayment, gd.gdNumber, gd.GDDate.Value));
        //                            }
        //                            catch
        //                            {
        //                                continue;
        //                            }
        //                        }
        //                        #endregion If Lc Data is Avaliable
        //                        #region If Contract Collection Data is Avaliable
        //                        if (FiData.contractCollectionData != null && FiData.contractCollectionData != "null")
        //                        {
        //                            DateTime gdDate = gd.GDDate.Value;
        //                            if (false)
        //                            {
        //                                int x = DateTime.Compare(gd.BLDateVale.Value, gd.GDDate.Value);
        //                                if (x > 0)
        //                                {
        //                                    gdDate = gd.BLDateVale.Value;
        //                                }
        //                            }
        //                            Contractcollectiondata FiCCData = JsonConvert.DeserializeObject<Contractcollectiondata>(FiData.contractCollectionData);
        //                            try
        //                            {
        //                                GdV20Dates.AddRange(V20Logics.GetV20Date(FiData.Id, gd.Id, FiData.finInsUniqueNumber, gd.totalDeclaredValue.Value, 0, 0, FiCCData.docAgainstPayPercentage, FiCCData.docAgainstAcceptancePercentage, FiCCData.advPayPercentage, FiData.modeOfPayment, FiData.FiCertifcationdate, FiCCData.days, gdDate, gd.BLDateVale, item.ModeOFPayment, gd.gdNumber, gd.GDDate.Value));
        //                            }
        //                            catch
        //                            {
        //                                continue;
        //                            }
        //                        }
        //                        #endregion If Contract Collection Data is Avaliable
        //                        #region If Mode of Payemnt 306
        //                        // ie Fi is avalible but no lc of cc attacheds
        //                        else if (FiData.modeOfPayment == "306")
        //                        {
        //                            DateTime gdDate = gd.GDDate.Value;
        //                            if (false)
        //                            {
        //                                int x = DateTime.Compare(gd.BLDateVale.Value, gd.GDDate.Value);
        //                                if (x > 0)
        //                                {
        //                                    gdDate = gd.BLDateVale.Value;
        //                                }
        //                            }
        //                            try
        //                            {
        //                                GdV20Dates.AddRange(V20Logics.GetV20Date(FiData.Id, gd.Id, FiData.finInsUniqueNumber, gd.totalDeclaredValue.Value, 0, 0, 0, 0, 0, FiData.modeOfPayment, FiData.FiCertifcationdate, 0, gdDate, gd.BLDateVale, item.ModeOFPayment, gd.gdNumber, gd.GDDate.Value));
        //                            }
        //                            catch
        //                            {
        //                                continue;
        //                            }
        //                        }
        //                        #endregion If Mode of Payemnt 306
        //                        #region useless code
        //                        //#region If Mode of Payemnt 305
        //                        //// ie Fi is not avalible in gd
        //                        //else if (FiData.modeOfPayment == "305")
        //                        //{
        //                        //    DateTime gdDate = gd.GDDate.Value;
        //                        //    if (false)
        //                        //    {
        //                        //        int x = DateTime.Compare(gd.BLDateVale.Value, gd.GDDate.Value);
        //                        //        if (x > 0)
        //                        //        {
        //                        //            gdDate = gd.BLDateVale.Value;
        //                        //        }
        //                        //    }
        //                        //    gdDate = gdDate.AddDays(45);
        //                        //    GdV20Dates.Add(new GD_FI_Link()
        //                        //    {
        //                        //        GdId = gd.Id,
        //                        //        FiId = FiData.Id,
        //                        //        MatruityDate = gdDate.ToString("dd-MMM-yyy"),
        //                        //        _MatruityDate = gdDate
        //                        //    });
        //                        //}
        //                        //#endregion If Mode of Payemnt 305
        //                        #endregion useless code
        //                    }
        //                    #region If Mode of Payemnt 305
        //                    else if (item.ModeOFPayment == "305")
        //                    {
        //                        FiData = lstfis.Where(x => x.openAccountGdNumber == gd.gdNumber).FirstOrDefault();

        //                        DateTime gdDate = gd.GDDate.Value;
        //                        if (false)
        //                        {
        //                            int x = DateTime.Compare(gd.BLDateVale.Value, gd.GDDate.Value);
        //                            if (x > 0)
        //                            {
        //                                gdDate = gd.BLDateVale.Value;
        //                            }
        //                        }
        //                        gdDate = gdDate.AddDays(45);// OpenAccount
        //                        if (FiData != null)
        //                        {
        //                            GdV20Dates.Add(new GD_FI_Link()
        //                            {
        //                                GdId = gd.Id,
        //                                FiId = FiData == null ? null : FiData.Id,
        //                                type = "Open Account",
        //                                MatruityDate = gdDate.ToString("dd-MMM-yyy"),
        //                                _MatruityDate = gdDate
        //                            });
        //                        }
        //                        else
        //                        {
        //                            GdV20Dates.Add(new GD_FI_Link()
        //                            {
        //                                GdId = gd.Id,
        //                                //   FiId = FiData == null ? null : FiData.Id,
        //                                type = "Open Account",
        //                                MatruityDate = gdDate.ToString("dd-MMM-yyy"),
        //                                _MatruityDate = gdDate
        //                            });
        //                        }


        //                    }
        //                    #endregion  If Mode of Payemnt 305
        //                    else
        //                    {
        //                        count++;
        //                        //GdV20Dates.Add(new GD_FI_Link()
        //                        //{
        //                        //    GdId = gd.Id,
        //                        //    //    GDNumber = gd.gdNumber,
        //                        //    //    FINumber = item.FiNumber + " - Not in FI File"
        //                        //});
        //                    }
        //                }
        //            }
        //            if (!GdV20Dates.IsNullOrEmpty())
        //            {



        //            }
        //            V20Dates.AddRange(GdV20Dates);
        //        }
        //        CustomRepo.InsertFI_GD_Link(V20Dates);
        //        return "Success";
        //    }
        //    catch (Exception ex)
        //    {
        //        Seriloger.LoggerInstance.Error($"Error In Sync New Gds :{ex.Message}");
        //        return $"Error :{ex.Message}";
        //    }
        //}
        //public static string SyncNewFi(long fileId, NewFiGdFilterModel fis_OpenGds)
        //{
        //    try
        //    {
        //        List<long> OpengdIds = new List<long>();
        //        Seriloger.LoggerInstance.Information($" Sync New Fis In Process.... :");
        //        ExportOverDueContext context = new ExportOverDueContext();
        //        List<GD_FI_Link> V20Dates = new List<GD_FI_Link>();
        //        List<GoodsDeclaration> lstgds = CustomRepo.GetGoodsDeclarationForV20Dates(fis_OpenGds, AppSettings.TenantId).DistinctBy(gd => gd.Id).ToList();//gd that newly came in 

        //        if (lstgds.Count == 0)
        //        {
        //            Seriloger.LoggerInstance.Information($"No Gds To Sync");

        //            return "No Gds";
        //        }
        //        var lstfis = CustomRepo.GetFinancialInstrumentForV20Dates(AppSettings.TenantId, fileId).ToList();
        //        if (lstfis.Count == 0)
        //        {
        //            Seriloger.LoggerInstance.Information($"No Fis To Sync");

        //            return "No Fis";
        //        }

        //        foreach (var gd in lstgds)
        //        {
        //            List<GD_FI_Link> GdV20Dates = new List<GD_FI_Link>();
        //            if (gd.gdNumber == null)
        //            {
        //                continue;
        //            }
        //            if (!gd.LstfinInsUniqueNumbers.IsNullOrEmpty())
        //            {
        //                foreach (var item in gd.FiNumbersAndModes)
        //                {
        //                    DateTime gdCreationDate = gd.GDDate.Value;
        //                    DBmodels.FinancialInstrument FiData = new DBmodels.FinancialInstrument();
        //                    if (item.FiNumber != null && item.FiNumber != "")
        //                    {
        //                        FiData = lstfis.Where(x => x.finInsUniqueNumber == item.FiNumber).FirstOrDefault();
        //                    }
        //                    if (FiData != null && !item.FiNumber.IsNullOrEmpty())
        //                    {
        //                        #region If Lc Data is Avaliable
        //                        if (FiData.lcData != null && FiData.lcData != "null")
        //                        {
        //                            DateTime gdDate = gd.GDDate.Value;
        //                            if (false)
        //                            {
        //                                int x = DateTime.Compare(gd.BLDateVale.Value, gd.GDDate.Value);
        //                                if (x > 0)
        //                                {
        //                                    gdDate = gd.BLDateVale.Value;
        //                                }
        //                            }
        //                            Lcdata FiLcData = JsonConvert.DeserializeObject<Lcdata>(FiData.lcData);
        //                            try
        //                            {
        //                                GdV20Dates.AddRange(V20Logics.GetV20Date(FiData.Id, gd.Id, FiData.finInsUniqueNumber, gd.totalDeclaredValue.Value, FiLcData.sightPercentage, FiLcData.usancePercentage, 0, 0, FiLcData.advPayPercentage, FiData.modeOfPayment, FiData.FiCertifcationdate, FiLcData.days, gdDate, gd.BLDateVale, item.ModeOFPayment, gd.gdNumber, gd.GDDate.Value));
        //                            }
        //                            catch
        //                            {
        //                                continue;
        //                            }
        //                        }
        //                        #endregion If Lc Data is Avaliable
        //                        #region If Contract Collection Data is Avaliable
        //                        if (FiData.contractCollectionData != null && FiData.contractCollectionData != "null")
        //                        {
        //                            DateTime gdDate = gd.GDDate.Value;
        //                            if (false)
        //                            {
        //                                int x = DateTime.Compare(gd.BLDateVale.Value, gd.GDDate.Value);
        //                                if (x > 0)
        //                                {
        //                                    gdDate = gd.BLDateVale.Value;
        //                                }
        //                            }
        //                            Contractcollectiondata FiCCData = JsonConvert.DeserializeObject<Contractcollectiondata>(FiData.contractCollectionData);
        //                            try
        //                            {
        //                                GdV20Dates.AddRange(V20Logics.GetV20Date(FiData.Id, gd.Id, FiData.finInsUniqueNumber, gd.totalDeclaredValue.Value, 0, 0, FiCCData.docAgainstPayPercentage, FiCCData.docAgainstAcceptancePercentage, FiCCData.advPayPercentage, FiData.modeOfPayment, FiData.FiCertifcationdate, FiCCData.days, gdDate, gd.BLDateVale, item.ModeOFPayment, gd.gdNumber, gd.GDDate.Value));
        //                            }
        //                            catch
        //                            {
        //                                continue;
        //                            }
        //                        }
        //                        #endregion If Contract Collection Data is Avaliable
        //                        #region If Mode of Payemnt 306
        //                        // ie Fi is avalible but no lc of cc attacheds
        //                        else if (FiData.modeOfPayment == "306")
        //                        {
        //                            DateTime gdDate = gd.GDDate.Value;
        //                            if (false)
        //                            {
        //                                int x = DateTime.Compare(gd.BLDateVale.Value, gd.GDDate.Value);
        //                                if (x > 0)
        //                                {
        //                                    gdDate = gd.BLDateVale.Value;
        //                                }
        //                            }
        //                            try
        //                            {
        //                                GdV20Dates.AddRange(V20Logics.GetV20Date(FiData.Id, gd.Id, FiData.finInsUniqueNumber, gd.totalDeclaredValue.Value, 0, 0, 0, 0, 0, FiData.modeOfPayment, FiData.FiCertifcationdate, 0, gdDate, gd.BLDateVale, item.ModeOFPayment, gd.gdNumber, gd.GDDate.Value));
        //                            }
        //                            catch
        //                            {
        //                                continue;
        //                            }
        //                        }
        //                        #endregion If Mode of Payemnt 306
        //                        #region useless code
        //                        //#region If Mode of Payemnt 305
        //                        //// ie Fi is not avalible in gd
        //                        //else if (FiData.modeOfPayment == "305")
        //                        //{
        //                        //    DateTime gdDate = gd.GDDate.Value;
        //                        //    if (false)
        //                        //    {
        //                        //        int x = DateTime.Compare(gd.BLDateVale.Value, gd.GDDate.Value);
        //                        //        if (x > 0)
        //                        //        {
        //                        //            gdDate = gd.BLDateVale.Value;
        //                        //        }
        //                        //    }
        //                        //    gdDate = gdDate.AddDays(45);
        //                        //    GdV20Dates.Add(new GD_FI_Link()
        //                        //    {
        //                        //        GdId = gd.Id,
        //                        //        FiId = FiData.Id,
        //                        //        MatruityDate = gdDate.ToString("dd-MMM-yyy"),
        //                        //        _MatruityDate = gdDate
        //                        //    });
        //                        //}
        //                        //#endregion If Mode of Payemnt 305
        //                        #endregion useless code
        //                    }
        //                    #region If Mode of Payemnt 305
        //                    else if (item.ModeOFPayment == "305")
        //                    {

        //                        // Coment as 305 will be handled by Lodgment and dosent Requrier Fi to be atttached


        //                        //FiData = lstfis.Where(x => x.openAccountGdNumber == gd.gdNumber).FirstOrDefault();

        //                        //DateTime gdDate = gd.GDDate.Value;
        //                        //if (false)
        //                        //{
        //                        //    int x = DateTime.Compare(gd.BLDateVale.Value, gd.GDDate.Value);
        //                        //    if (x > 0)
        //                        //    {
        //                        //        gdDate = gd.BLDateVale.Value;
        //                        //    }
        //                        //}
        //                        //gdDate = gdDate.AddDays(45);
        //                        //if (FiData != null)
        //                        //{

        //                        //    GdV20Dates.Add(new GD_FI_Link()
        //                        //    {
        //                        //        GdId = gd.Id,
        //                        //        FiId = FiData == null ? null : FiData.Id,
        //                        //        type = "Open Account",
        //                        //        MatruityDate = gdDate.ToString("dd-MMM-yyy"),
        //                        //        _MatruityDate = gdDate
        //                        //    });
        //                        //    OpengdIds.Add(gd.Id);

        //                        //}


        //                    }
        //                    #endregion  If Mode of Payemnt 305
        //                    else
        //                    {
        //                        //GdV20Dates.Add(new GD_FI_Link()
        //                        //{
        //                        //    GdId = gd.Id,
        //                        //    //    GDNumber = gd.gdNumber,
        //                        //    //    FINumber = item.FiNumber + " - Not in FI File"
        //                        //});
        //                    }
        //                }
        //            }
        //            if (!GdV20Dates.IsNullOrEmpty())
        //            {



        //            }
        //            V20Dates.AddRange(GdV20Dates);
        //        }
        //        CustomRepo.InsertFI_GD_Link(V20Dates);
        //        // CustomRepo.RemoveLinkFI_GD_Link(OpengdIds);
        //        return "Success";
        //    }
        //    catch (Exception ex)
        //    {
        //        Seriloger.LoggerInstance.Error($"Error In Sync New Gds :{ex.Message}");
        //        return $"Error :{ex.Message}";
        //    }
        //}


        //Syncs good declaration import with fis in import, sends the result to the comparison algo for the computation of gi and fi comparison.
        public static string SyncNewImportGd(long fileId, NewFiGdFilterModel fis_OpenGds)
        {
            try
            {
                Seriloger.LoggerInstance.Information($" Sync New Gds In Process.... :");

                ExportOverDueContext context = new();
                List<ComparatorSetting> comparatorSettings = context.ComparatorSettings.ToList();
                List<GdFiLink> Link = [];
                List<GoodsDeclarationImport> lstgds = CustomRepo.GetGoodsDeclarationImportForLink(AppSettings.TenantId, fileId).ToList();//gd that newly came in 

                //Select the list of all the goods declaration that came in to get records for open account fi's.
                List<string?> gdNums = lstgds.Select(g => g.gdNumber)
                    .Distinct()
                    .ToList();

                if (lstgds.Count == 0)
                {
                    Seriloger.LoggerInstance.Information($"No Gds To Sync");

                    return "No Gds";
                }

                //Gets the open account fis and fis that newly camein.
                List<FinancialInstrumentImport> lstfis = CustomRepo.GetFinancialInstrumentForImportForLink(gdNums ,AppSettings.TenantId, fis_OpenGds).ToList();
                if (lstfis.Count == 0)
                {
                    Seriloger.LoggerInstance.Information($"No Fis To Sync");

                    return "No Fis";
                }

                foreach (var gd in lstgds)
                {
                    List<FinancialInstrumentImport> fiData = [];

                    //Gets the gds for fi either by matching fi number or by matching of the open acc gd number with gd's number.
                    fiData = lstfis
                        .Where(x => x.FinInsUniqueNumber != null 
                            && gd.FinInsUniqueNumber != null 
                            && x.FinInsUniqueNumber == gd.FinInsUniqueNumber
                        )
                        .OrderByDescending(x => x.TransmissionDate)
                        .ToList();

                    if ((fiData is null || fiData.Count == 0) && gd.ModeOfPayment == "302")
                    {
                        fiData = lstfis
                            .Where(x => x.OpenAccountGdNumber != null
                                && gd.gdNumber != null
                                && x.OpenAccountGdNumber == gd.gdNumber
                            )
                            .OrderByDescending(x => x.TransmissionDate)
                            .ToList();
                    }

                    //Inserts the fi and gd link for gd and fi
                    foreach (var fi in fiData ?? [])
                    {
                        Link.Add(new GdFiLink()
                        {
                            Type = "Import",
                            GdId = gd.Id,
                            FiId = fi.Id,
                            CreationTime = DateTime.Now,
                            IsDeleted = false,
                            RequestStatusId = 12,
                            TenantId = AppSettings.TenantId
                        });
                    }
                }
                CustomRepo.InsertFI_GD_Link(Link);


                var gdfilinkIds = Link.Select(link => link.Id);

                //Get the base fi and its multiple gds for comaparison.
                List<ComparisonInput> groupedFiGds = context.FinancialInstrumentImports
                    .Include(figd => figd.GdFiLinks)
                        .ThenInclude(fi => fi!.Gd)
                    .Where(fi => fi.GdFiLinks.Any(link => gdfilinkIds.Contains(link.Id)) && fi.GdFiLinks.Count() >= 1)
                    .Select(figdLink => new ComparisonInput
                    {
                        Payload = figdLink.Payload ?? string.Empty,
                        Id = figdLink.Id,
                        Type = DocumentType.FI,
                        RelatedRecords = figdLink.GdFiLinks.Select(figd => new RelatedRecord{ Payload = figd.Gd!.Payload ?? string.Empty, RelationId = figd.Id }).ToList() 
                    }).ToList();

                //Gets the base gd and its multiple fi for comparison.
                List<ComparisonInput> groupedGdFis = context.GoodsDeclarationImports
                    .Include(gd => gd.GdFiLinks)
                        .ThenInclude(gdfi => gdfi!.Fi)
                    .Where(gd => gd.GdFiLinks.Any(link => gdfilinkIds.Contains(link.Id)) && gd.GdFiLinks.Count() > 1)
                    .Select(gd => new ComparisonInput
                    {
                        Payload = gd.Payload ?? string.Empty,
                        Id = gd.Id,
                        Type = DocumentType.GD,
                        RelatedRecords = gd.GdFiLinks.Select(figd => new RelatedRecord { Payload = figd.Fi!.Payload ?? string.Empty, RelationId = figd.Id }).ToList() 
                    }).ToList();

                if (groupedFiGds.Count == 0 && groupedGdFis.Count == 0)
                {
                    Seriloger.LoggerInstance.Information($"No Matching GDs Found for FIs");
                    return "No Matches";
                }

                // Perform Comparison for each FI with its related GDs
                List<ValidateIqBizLogic.Comparison_V2.ComparisonResult> comparisonResults = [];
                comparisonResults.AddRange(
                    Comparison_V2.CompareGdAndFi(groupedFiGds, comparatorSettings.Where(cs => cs.ModuleId == (int)ComparisonType.Import).ToList(), 3, ComparisonType.Import)
                );

                //Performs the comparison for each gd with its multiple fis.
                comparisonResults.AddRange(
                    Comparison_V2.CompareGdAndFi(groupedGdFis, comparatorSettings.Where(cs => cs.ModuleId == (int)ComparisonType.Import).ToList(), 3, ComparisonType.Import)
                );

                CustomRepo.InsertFI_GD_ComparisonResult(comparisonResults);
                return "Success";
            }
            catch (Exception ex)
            {
                Seriloger.LoggerInstance.Error($"Error In Sync New Gds :{ex.Message}");
                return $"Error :{ex.Message}";
            }
        }
        public static string SyncNewGd(long fileId, NewFiGdFilterModel fis_OpenGds)
        {
            try
            {
                Seriloger.LoggerInstance.Information($" Sync New Gds In Process.... :");

                ExportOverDueContext context = new();
                List<ComparatorSetting> comparatorSettings = context.ComparatorSettings.ToList();
                List<GD_FI_Link> Link = [];
                List<GoodsDeclaration> lstgds = CustomRepo.GetGoodsDeclarationForLink(AppSettings.TenantId, fileId).ToList();//gd that newly came in 

                List<string?> gdNums = lstgds.Select(g => g.gdNumber)
                    .Distinct()
                    .ToList();

                if (lstgds.Count == 0)
                {
                    Seriloger.LoggerInstance.Information($"No Gds To Sync");

                    return "No Gds";
                }
                List<DBmodels.FinancialInstrument> lstfis = CustomRepo.GetFinancialInstrumentForExportForLink(gdNums ,AppSettings.TenantId, fis_OpenGds).ToList();
                if (lstfis.Count == 0)
                {
                    Seriloger.LoggerInstance.Information($"No Fis To Sync");

                    return "No Fis";
                }

                foreach (var gd in lstgds)
                {
                    List<DBmodels.FinancialInstrument> fiData = [];

                    fiData = lstfis
                        .Where(x => x.finInsUniqueNumber != null 
                            && gd.finInsUniqueNumber != null 
                            && x.finInsUniqueNumber == gd.finInsUniqueNumber
                        )
                        .ToList();

                    if ((fiData is null || fiData.Count == 0)
                        )
                    {
                        fiData = lstfis
                            .Where(x => x.openAccountGdNumber != null
                                && gd.gdNumber != null
                                && x.openAccountGdNumber == gd.gdNumber
                            )
                            .ToList();
                    }

                    foreach (var fi in fiData ?? [])
                    {
                        Link.Add(new GD_FI_Link()
                        {
                            type = "Export",
                            GdId = gd.Id,
                            FiId = fi.Id,
                            CreationTime = DateTime.UtcNow,
                            IsDeleted = false,
                            TenantId = AppSettings.TenantId
                        });
                    }
                }
                CustomRepo.InsertFI_GD_Link(Link);

                var gdfilinkIds = Link.Select(link => link.Id);

                List<ComparisonInput> groupedFiGds = context.FinancialInstrument
                    .Include(figd => figd.GD_FI_Links)
                        .ThenInclude(fi => fi!.Gd)
                    .Where(fi => fi.GD_FI_Links.Any(link => gdfilinkIds.Contains(link.Id)) && fi.GD_FI_Links.Count() >= 1)
                    .Select(figdLink => new ComparisonInput
                    {
                        Payload = figdLink.PAYLOAD ?? string.Empty,
                        Id = figdLink.Id,
                        Type = DocumentType.FI,
                        RelatedRecords = figdLink.GD_FI_Links.Select(figd => new RelatedRecord { Payload = figd.Gd!.PAYLOAD ?? string.Empty, RelationId = figd.Id }).ToList() 
                    }).ToList();


                List<ComparisonInput> groupedGdFis = context.GoodsDeclaration
                    .Include(gd => gd.GD_FI_Links)
                        .ThenInclude(gdfi => gdfi!.Fi)
                    .Where(gd => gd.GD_FI_Links.Any(link => gdfilinkIds.Contains(link.Id)) && gd.GD_FI_Links.Count() > 1)
                    .Select(gd => new ComparisonInput
                    {
                        Payload = gd.PAYLOAD ?? string.Empty,
                        Id = gd.Id,
                        Type = DocumentType.GD,
                        RelatedRecords = gd.GD_FI_Links.Select(figd => new RelatedRecord { Payload = figd.Fi!.PAYLOAD ?? string.Empty, RelationId = figd.Id }).ToList() 
                    }).ToList();

                if (groupedFiGds.Count == 0 && groupedGdFis.Count == 0)
                {
                    Seriloger.LoggerInstance.Information($"No Matching GDs Found for FIs");
                    return "No Matches";
                }

                // Perform Comparison for each FI with its related GDs
                List<ValidateIqBizLogic.Comparison_V2.ComparisonResult> comparisonResults = [];
                comparisonResults.AddRange(
                    Comparison_V2.CompareGdAndFi(groupedFiGds, comparatorSettings.Where(cs => cs.ModuleId == (int)ComparisonType.Export).ToList(), 3, ComparisonType.Export)
                );
                comparisonResults.AddRange(
                    Comparison_V2.CompareGdAndFi(groupedGdFis, comparatorSettings.Where(cs => cs.ModuleId == (int)ComparisonType.Export).ToList(), 3, ComparisonType.Export)
                );

                CustomRepo.InsertFI_GD_ComparisonResult(comparisonResults);
                return "Success";
            }
            catch (Exception ex)
            {
                Seriloger.LoggerInstance.Error($"Error In Sync New Gds :{ex.Message}");
                return $"Error :{ex.Message}";
            }
        }


        public static string SyncImportNewFi(long fileId, NewFiGdFilterModel fis_OpenGds)
        {
            try
            {
                ExportOverDueContext context = new();
                List<ComparatorSetting> comparatorSettings = context.ComparatorSettings.ToList();
                List<GdFiLink> links = [];
                List<GoodsDeclarationImport> lstgds = CustomRepo.GetGoodsDeclarationImportForLink(fis_OpenGds, AppSettings.TenantId).DistinctBy(gd => gd.Id).ToList();//gd that newly came in 

                if (lstgds.Count == 0)
                {
                    Seriloger.LoggerInstance.Information($"No Gds To Sync");

                    return "No Gds";
                }
                var lstfis = CustomRepo.GetFinancialInstrumentImportForLink(AppSettings.TenantId, fileId).ToList();
                if (lstfis.Count == 0)
                {
                    Seriloger.LoggerInstance.Information($"No Fis To Sync");

                    return "No Fis";
                }

                foreach (var gd in lstgds)
                {
                    List<FinancialInstrumentImport> fiData = [];

                    fiData = lstfis
                        .Where(x => x.FinInsUniqueNumber != null
                            && gd.FinInsUniqueNumber != null
                            && x.FinInsUniqueNumber == gd.FinInsUniqueNumber
                        )
                        .OrderByDescending(x => x.TransmissionDate)
                        .ToList();

                    if ((fiData is null || fiData.Count == 0) && gd.ModeOfPayment == "302")
                    {
                        fiData = lstfis
                            .Where(x => x.OpenAccountGdNumber != null
                                && gd.gdNumber != null
                                && x.OpenAccountGdNumber == gd.gdNumber
                            )
                            .OrderByDescending(x => x.TransmissionDate)
                            .ToList();
                    }

                    foreach (var fi in fiData ?? [])
                    {
                        links.Add(new GdFiLink()
                        {
                            Type = "Import",
                            GdId = gd.Id,
                            FiId = fi.Id,
                            CreationTime = DateTime.Now,
                            IsDeleted = false,
                            RequestStatusId = 12,
                            TenantId = AppSettings.TenantId
                        });
                    }
                }
                CustomRepo.InsertFI_GD_Link(links);

                var gdfilinkIds = links.Select(link => link.Id);

                // Group GDs by FI Unique Number

                List<ComparisonInput> groupedFiGds = context.FinancialInstrumentImports
                    .Include(figd => figd.GdFiLinks)
                        .ThenInclude(fi => fi!.Gd)
                    .Where(fi => fi.GdFiLinks.Any(link => gdfilinkIds.Contains(link.Id)) && fi.GdFiLinks.Count() >= 1)
                    .Select(figdLink => new ComparisonInput
                    {
                        Payload = figdLink.Payload ?? string.Empty,
                        Id = figdLink.Id,
                        Type = DocumentType.FI,
                        RelatedRecords = figdLink.GdFiLinks.Select(figd => new RelatedRecord { Payload = figd.Gd!.Payload ?? string.Empty, RelationId = figd.Id }).ToList() 
                    }).ToList();


                List<ComparisonInput> groupedGdFis = context.GoodsDeclarationImports
                    .Include(gd => gd.GdFiLinks)
                        .ThenInclude(gdfi => gdfi!.Fi)
                    .Where(gd => gd.GdFiLinks.Any(link => gdfilinkIds.Contains(link.Id)) && gd.GdFiLinks.Count() > 1)
                    .Select(gd => new ComparisonInput
                    {
                        Payload = gd.Payload ?? string.Empty,
                        Id = gd.Id,
                        Type = DocumentType.GD,
                        RelatedRecords = gd.GdFiLinks.Select(figd => new RelatedRecord { Payload = figd.Fi!.Payload ?? string.Empty, RelationId = figd.Id }).ToList() 
                    }).ToList();

                if (groupedFiGds.Count == 0 && groupedGdFis.Count == 0)
                {
                    Seriloger.LoggerInstance.Information($"No Matching GDs Found for FIs");
                    return "No Matches";
                }

                // Perform Comparison for each FI with its related GDs
                List<ValidateIqBizLogic.Comparison_V2.ComparisonResult> comparisonResults = [];
                comparisonResults.AddRange(
                    Comparison_V2.CompareGdAndFi(groupedFiGds, comparatorSettings.Where(cs => cs.ModuleId == (int)ComparisonType.Import).ToList(), 3, ComparisonType.Import)
                );
                comparisonResults.AddRange(
                    Comparison_V2.CompareGdAndFi(groupedGdFis, comparatorSettings.Where(cs => cs.ModuleId == (int)ComparisonType.Import).ToList(), 3, ComparisonType.Import)
                );

                CustomRepo.InsertFI_GD_ComparisonResult(comparisonResults);

                return "Success";
            }
            catch (Exception ex)
            {
                Seriloger.LoggerInstance.Error($"Error In Sync New Gds :{ex.Message}");
                return $"Error :{ex.Message}";
            }
        }

        public static string SyncNewFi(long fileId, NewFiGdFilterModel fis_OpenGds)
        {
            try
            {
                ExportOverDueContext context = new();
                List<ComparatorSetting> comparatorSettings = context.ComparatorSettings.ToList();
                List<GD_FI_Link> links = [];
                List<GoodsDeclaration> lstgds = CustomRepo.GetGoodsDeclarationForLink(fis_OpenGds, AppSettings.TenantId)
                    .DistinctBy(gd => gd.Id)
                    .ToList();//gd that newly came in 

                if (lstgds.Count == 0)
                {
                    Seriloger.LoggerInstance.Information($"No Gds To Sync");

                    return "No Gds";
                }
                var lstfis = CustomRepo.GetFinancialInstrumentForLink(AppSettings.TenantId, fileId).ToList();
                if (lstfis.Count == 0)
                {
                    Seriloger.LoggerInstance.Information($"No Fis To Sync");

                    return "No Fis";
                }

                foreach (var gd in lstgds)
                {
                    List<DBmodels.FinancialInstrument> fiData = [];

                    fiData = lstfis
                        .Where(x => x.finInsUniqueNumber != null
                            && gd.finInsUniqueNumber != null
                            && x.finInsUniqueNumber == gd.finInsUniqueNumber
                        )
                        .ToList();

                    if ((fiData is null || fiData.Count == 0) && gd.modeOfPayment == "302")
                    {
                        fiData = lstfis
                            .Where(x => x.openAccountGdNumber != null
                                && gd.gdNumber != null
                                && x.openAccountGdNumber == gd.gdNumber
                            )
                            .ToList();
                    }

                    foreach (var fi in fiData ?? [])
                    {
                        links.Add(new GD_FI_Link()
                        {
                            type = "Export",
                            GdId = gd.Id,
                            FiId = fi.Id,
                            CreationTime = DateTime.Now,
                            IsDeleted = false,
                            TenantId = AppSettings.TenantId
                        });
                    }
                }
                CustomRepo.InsertFI_GD_Link(links);

                var gdfilinkIds = links.Select(link => link.Id);

                // Group GDs by FI Unique Number

                List<ComparisonInput> groupedFiGds = context.FinancialInstrument
                    .Include(figd => figd.GD_FI_Links)
                        .ThenInclude(fi => fi!.Gd)
                    .Where(fi => fi.GD_FI_Links.Any(link => gdfilinkIds.Contains(link.Id)) && fi.GD_FI_Links.Count() >= 1)
                    .Select(figdLink => new ComparisonInput
                    {
                        Payload = figdLink.PAYLOAD ?? string.Empty,
                        Id = figdLink.Id,
                        Type = DocumentType.FI,
                        RelatedRecords = figdLink.GD_FI_Links.Select(figd => new RelatedRecord { Payload = figd.Gd!.PAYLOAD ?? string.Empty, RelationId = figd.Id }).ToList() 
                    }).ToList();


                List<ComparisonInput> groupedGdFis = context.GoodsDeclaration
                    .Include(gd => gd.GD_FI_Links)
                        .ThenInclude(gdfi => gdfi!.Fi)
                    .Where(gd => gd.GD_FI_Links.Any(link => gdfilinkIds.Contains(link.Id)) && gd.GD_FI_Links.Count() > 1)
                    .Select(gd => new ComparisonInput
                    {
                        Payload = gd.PAYLOAD ?? string.Empty,
                        Id = gd.Id,
                        Type = DocumentType.GD,
                        RelatedRecords = gd.GD_FI_Links.Select(figd => new RelatedRecord { Payload = figd.Fi!.PAYLOAD ?? string.Empty, RelationId = figd.Id }).ToList() 
                    }).ToList();

                if (groupedFiGds.Count == 0 && groupedGdFis.Count == 0)
                {
                    Seriloger.LoggerInstance.Information($"No Matching GDs Found for FIs");
                    return "No Matches";
                }

                // Perform Comparison for each FI with its related GDs
                List<ValidateIqBizLogic.Comparison_V2.ComparisonResult> comparisonResults = [];
                comparisonResults.AddRange(
                    Comparison_V2.CompareGdAndFi(groupedFiGds, comparatorSettings.Where(cs => cs.ModuleId == (int)ComparisonType.Export).ToList(), 3, ComparisonType.Export)
                );
                comparisonResults.AddRange(
                    Comparison_V2.CompareGdAndFi(groupedGdFis, comparatorSettings.Where(cs => cs.ModuleId == (int)ComparisonType.Export).ToList(), 3, ComparisonType.Export)
                );

                CustomRepo.InsertFI_GD_ComparisonResult(comparisonResults);

                return "Success";
            }
            catch (Exception ex)
            {
                Seriloger.LoggerInstance.Error($"Error In Sync New Gds :{ex.Message}");
                return $"Error :{ex.Message}";
            }
        }
    }
}
