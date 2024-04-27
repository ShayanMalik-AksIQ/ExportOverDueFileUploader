using ExportOverDueFileUploader.DBmodels;
using ExportOverDueFileUploader.Modles.JsonHelper;
using ExportOverDueFileUploader.Modles;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExportOverDueFileUploader.DBHelper;
using ExportOverDueFileUploader.DataImporter;
using Serilog;

namespace ExportOverDueFileUploader.MatuirtyBO
{
    public static class LinkGdToFI
    {
        

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
        //                        if (!gd.blDate.IsNullOrEmpty())
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
        //                        if (!gd.blDate.IsNullOrEmpty())
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
        //                        if (!gd.blDate.IsNullOrEmpty())
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
        //                        if (!gd.blDate.IsNullOrEmpty())
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
        public static string SyncNewGd(long fileId, NewFiGdFilterModel fis_OpenGds)
        {
            try
            {
                Seriloger.LoggerInstance.Error($" Sync New Gds In Process.... :");
                ExportOverDueContext context = new ExportOverDueContext();
                List<GD_FI_Link> V20Dates = new List<GD_FI_Link>();
                List<GoodsDeclaration> lstgds = CustomRepo.GetGoodsDeclarationForV20Dates(AppSettings.TenantId, fileId).ToList();//gd that newly came in 

                if (lstgds.Count == 0)
                {
                    Seriloger.LoggerInstance.Information($"No Gds To Sync");

                    return "No Gds";
                }
                var lstfis = CustomRepo.GetFinancialInstrumentForV20Dates(AppSettings.TenantId, fis_OpenGds).ToList();
                if (lstfis.Count == 0)
                {
                    Seriloger.LoggerInstance.Information($"No Fis To Sync");

                    return "No Fis";
                }

                foreach (var gd in lstgds)
                {
                    List<GD_FI_Link> GdV20Dates = new List<GD_FI_Link>();
                    if (gd.gdNumber == null)
                    {
                        continue;
                    }
                    if (!gd.LstfinInsUniqueNumbers.IsNullOrEmpty())
                    {
                        foreach (var item in gd.FiNumbersAndModes)
                        {
                            DateTime gdCreationDate = gd.GDDate.Value;
                            var FiData = lstfis.Where(x => x.finInsUniqueNumber == item.FiNumber).FirstOrDefault();
                            if (item.FiNumber.IsNullOrEmpty())
                            {
                               // continue;
                            }
                            if (FiData != null && !item.FiNumber.IsNullOrEmpty())
                            {
                                #region If Lc Data is Avaliable
                                if (FiData.lcData != null && FiData.lcData != "null")
                                {
                                    DateTime gdDate = gd.GDDate.Value;
                                    if (!gd.blDate.IsNullOrEmpty())
                                    {
                                        int x = DateTime.Compare(gd.BLDateVale.Value, gd.GDDate.Value);
                                        if (x > 0)
                                        {
                                            gdDate = gd.BLDateVale.Value;
                                        }
                                    }
                                    Lcdata FiLcData = JsonConvert.DeserializeObject<Lcdata>(FiData.lcData);
                                    try
                                    {
                                        GdV20Dates.AddRange(V20Logics.GetV20Date(FiData.Id, gd.Id, FiData.finInsUniqueNumber, gd.totalDeclaredValue.Value, FiLcData.sightPercentage, FiLcData.usancePercentage, 0, 0, FiLcData.advPayPercentage, FiData.modeOfPayment, FiData.FiCertifcationdate, FiLcData.days, gdDate, gd.BLDateVale, item.ModeOFPayment, gd.gdNumber, gd.GDDate.Value));
                                    }
                                    catch
                                    {
                                        continue;
                                    }
                                }
                                #endregion If Lc Data is Avaliable
                                #region If Contract Collection Data is Avaliable
                                if (FiData.contractCollectionData != null && FiData.contractCollectionData != "null")
                                {
                                    DateTime gdDate = gd.GDDate.Value;
                                    if (!gd.blDate.IsNullOrEmpty())
                                    {
                                        int x = DateTime.Compare(gd.BLDateVale.Value, gd.GDDate.Value);
                                        if (x > 0)
                                        {
                                            gdDate = gd.BLDateVale.Value;
                                        }
                                    }
                                    Contractcollectiondata FiCCData = JsonConvert.DeserializeObject<Contractcollectiondata>(FiData.contractCollectionData);
                                    try
                                    {
                                        GdV20Dates.AddRange(V20Logics.GetV20Date(FiData.Id, gd.Id, FiData.finInsUniqueNumber, gd.totalDeclaredValue.Value, 0, 0, FiCCData.docAgainstPayPercentage, FiCCData.docAgainstAcceptancePercentage, FiCCData.advPayPercentage, FiData.modeOfPayment, FiData.FiCertifcationdate, FiCCData.days, gdDate, gd.BLDateVale, item.ModeOFPayment, gd.gdNumber, gd.GDDate.Value));
                                    }
                                    catch
                                    {
                                        continue;
                                    }
                                }
                                #endregion If Contract Collection Data is Avaliable
                                #region If Mode of Payemnt 306
                                // ie Fi is avalible but no lc of cc attacheds
                                else if (FiData.modeOfPayment == "306")
                                {
                                    DateTime gdDate = gd.GDDate.Value;
                                    if (!gd.blDate.IsNullOrEmpty())
                                    {
                                        int x = DateTime.Compare(gd.BLDateVale.Value, gd.GDDate.Value);
                                        if (x > 0)
                                        {
                                            gdDate = gd.BLDateVale.Value;
                                        }
                                    }
                                    try
                                    {
                                        GdV20Dates.AddRange(V20Logics.GetV20Date(FiData.Id, gd.Id, FiData.finInsUniqueNumber, gd.totalDeclaredValue.Value, 0, 0, 0, 0, 0, FiData.modeOfPayment, FiData.FiCertifcationdate, 0, gdDate, gd.BLDateVale, item.ModeOFPayment, gd.gdNumber, gd.GDDate.Value));
                                    }
                                    catch
                                    {
                                        continue;
                                    }
                                }
                                #endregion If Mode of Payemnt 306
                                #region useless code
                                //#region If Mode of Payemnt 305
                                //// ie Fi is not avalible in gd
                                //else if (FiData.modeOfPayment == "305")
                                //{
                                //    DateTime gdDate = gd.GDDate.Value;
                                //    if (!gd.blDate.IsNullOrEmpty())
                                //    {
                                //        int x = DateTime.Compare(gd.BLDateVale.Value, gd.GDDate.Value);
                                //        if (x > 0)
                                //        {
                                //            gdDate = gd.BLDateVale.Value;
                                //        }
                                //    }
                                //    gdDate = gdDate.AddDays(45);
                                //    GdV20Dates.Add(new GD_FI_Link()
                                //    {
                                //        GdId = gd.Id,
                                //        FiId = FiData.Id,
                                //        MatruityDate = gdDate.ToString("dd-MMM-yyy"),
                                //        _MatruityDate = gdDate
                                //    });
                                //}
                                //#endregion If Mode of Payemnt 305
                                #endregion useless code
                            }
                            #region If Mode of Payemnt 305
                            else if (item.ModeOFPayment == "305")
                            {
                                FiData = lstfis.Where(x => x.openAccountGdNumber == gd.gdNumber).FirstOrDefault();

                                DateTime gdDate = gd.GDDate.Value;
                                if (!gd.blDate.IsNullOrEmpty())
                                {
                                    int x = DateTime.Compare(gd.BLDateVale.Value, gd.GDDate.Value);
                                    if (x > 0)
                                    {
                                        gdDate = gd.BLDateVale.Value;
                                    }
                                }
                                gdDate = gdDate.AddDays(45);
                                if (FiData != null)
                                {
                                    GdV20Dates.Add(new GD_FI_Link()
                                    {
                                        GdId = gd.Id,
                                        FiId = FiData == null ? null : FiData.Id,
                                        type = "Open Account",
                                        MatruityDate = gdDate.ToString("dd-MMM-yyy"),
                                        _MatruityDate = gdDate
                                    }) ;
                                }


                            }
                            #endregion  If Mode of Payemnt 305
                            else
                            {
                                //GdV20Dates.Add(new GD_FI_Link()
                                //{
                                //    GdId = gd.Id,
                                //    //    GDNumber = gd.gdNumber,
                                //    //    FINumber = item.FiNumber + " - Not in FI File"
                                //});
                            }
                        }
                    }
                    if (!GdV20Dates.IsNullOrEmpty())
                    {



                    }
                    V20Dates.AddRange(GdV20Dates);
                }
                CustomRepo.InsertFI_GD_Link(V20Dates);
                return "Sucess";
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
                Seriloger.LoggerInstance.Information($" Sync New Fis In Process.... :");
                ExportOverDueContext context = new ExportOverDueContext();
                List<GD_FI_Link> V20Dates = new List<GD_FI_Link>();
                List<GoodsDeclaration> lstgds = CustomRepo.GetGoodsDeclarationForV20Dates(fis_OpenGds,AppSettings.TenantId).DistinctBy(gd=>gd.Id).ToList();//gd that newly came in 

                if (lstgds.Count == 0)
                {
                    Seriloger.LoggerInstance.Information($"No Gds To Sync");

                    return "No Gds";
                }
                var lstfis = CustomRepo.GetFinancialInstrumentForV20Dates(AppSettings.TenantId, fileId).ToList();
                if (lstfis.Count == 0)
                {
                    Seriloger.LoggerInstance.Information($"No Fis To Sync");

                    return "No Fis";
                }

                foreach (var gd in lstgds)
                {
                    List<GD_FI_Link> GdV20Dates = new List<GD_FI_Link>();
                    if (gd.gdNumber == null)
                    {
                        continue;
                    }
                    if (!gd.LstfinInsUniqueNumbers.IsNullOrEmpty())
                    {
                        foreach (var item in gd.FiNumbersAndModes)
                        {
                            DateTime gdCreationDate = gd.GDDate.Value;
                            DBmodels.FinancialInstrument FiData =new DBmodels.FinancialInstrument();
                            if (item.FiNumber != null && item.FiNumber != "")
                            {
                               FiData = lstfis.Where(x => x.finInsUniqueNumber == item.FiNumber).FirstOrDefault();
                            }
                            if (FiData != null && !item.FiNumber.IsNullOrEmpty())
                            {
                                #region If Lc Data is Avaliable
                                if (FiData.lcData != null && FiData.lcData != "null")
                                {
                                    DateTime gdDate = gd.GDDate.Value;
                                    if (!gd.blDate.IsNullOrEmpty())
                                    {
                                        int x = DateTime.Compare(gd.BLDateVale.Value, gd.GDDate.Value);
                                        if (x > 0)
                                        {
                                            gdDate = gd.BLDateVale.Value;
                                        }
                                    }
                                    Lcdata FiLcData = JsonConvert.DeserializeObject<Lcdata>(FiData.lcData);
                                    try
                                    {
                                        GdV20Dates.AddRange(V20Logics.GetV20Date(FiData.Id, gd.Id, FiData.finInsUniqueNumber, gd.totalDeclaredValue.Value, FiLcData.sightPercentage, FiLcData.usancePercentage, 0, 0, FiLcData.advPayPercentage, FiData.modeOfPayment, FiData.FiCertifcationdate, FiLcData.days, gdDate, gd.BLDateVale, item.ModeOFPayment, gd.gdNumber, gd.GDDate.Value));
                                    }
                                    catch
                                    {
                                        continue;
                                    }
                                }
                                #endregion If Lc Data is Avaliable
                                #region If Contract Collection Data is Avaliable
                                if (FiData.contractCollectionData != null && FiData.contractCollectionData != "null")
                                {
                                    DateTime gdDate = gd.GDDate.Value;
                                    if (!gd.blDate.IsNullOrEmpty())
                                    {
                                        int x = DateTime.Compare(gd.BLDateVale.Value, gd.GDDate.Value);
                                        if (x > 0)
                                        {
                                            gdDate = gd.BLDateVale.Value;
                                        }
                                    }
                                    Contractcollectiondata FiCCData = JsonConvert.DeserializeObject<Contractcollectiondata>(FiData.contractCollectionData);
                                    try
                                    {
                                        GdV20Dates.AddRange(V20Logics.GetV20Date(FiData.Id, gd.Id, FiData.finInsUniqueNumber, gd.totalDeclaredValue.Value, 0, 0, FiCCData.docAgainstPayPercentage, FiCCData.docAgainstAcceptancePercentage, FiCCData.advPayPercentage, FiData.modeOfPayment, FiData.FiCertifcationdate, FiCCData.days, gdDate, gd.BLDateVale, item.ModeOFPayment, gd.gdNumber, gd.GDDate.Value));
                                    }
                                    catch
                                    {
                                        continue;
                                    }
                                }
                                #endregion If Contract Collection Data is Avaliable
                                #region If Mode of Payemnt 306
                                // ie Fi is avalible but no lc of cc attacheds
                                else if (FiData.modeOfPayment == "306")
                                {
                                    DateTime gdDate = gd.GDDate.Value;
                                    if (!gd.blDate.IsNullOrEmpty())
                                    {
                                        int x = DateTime.Compare(gd.BLDateVale.Value, gd.GDDate.Value);
                                        if (x > 0)
                                        {
                                            gdDate = gd.BLDateVale.Value;
                                        }
                                    }
                                    try
                                    {
                                        GdV20Dates.AddRange(V20Logics.GetV20Date(FiData.Id, gd.Id, FiData.finInsUniqueNumber, gd.totalDeclaredValue.Value, 0, 0, 0, 0, 0, FiData.modeOfPayment, FiData.FiCertifcationdate, 0, gdDate, gd.BLDateVale, item.ModeOFPayment, gd.gdNumber, gd.GDDate.Value));
                                    }
                                    catch
                                    {
                                        continue;
                                    }
                                }
                                #endregion If Mode of Payemnt 306
                                #region useless code
                                //#region If Mode of Payemnt 305
                                //// ie Fi is not avalible in gd
                                //else if (FiData.modeOfPayment == "305")
                                //{
                                //    DateTime gdDate = gd.GDDate.Value;
                                //    if (!gd.blDate.IsNullOrEmpty())
                                //    {
                                //        int x = DateTime.Compare(gd.BLDateVale.Value, gd.GDDate.Value);
                                //        if (x > 0)
                                //        {
                                //            gdDate = gd.BLDateVale.Value;
                                //        }
                                //    }
                                //    gdDate = gdDate.AddDays(45);
                                //    GdV20Dates.Add(new GD_FI_Link()
                                //    {
                                //        GdId = gd.Id,
                                //        FiId = FiData.Id,
                                //        MatruityDate = gdDate.ToString("dd-MMM-yyy"),
                                //        _MatruityDate = gdDate
                                //    });
                                //}
                                //#endregion If Mode of Payemnt 305
                                #endregion useless code
                            }
                            #region If Mode of Payemnt 305
                            else if (item.ModeOFPayment == "305")
                            {
                                FiData = lstfis.Where(x => x.openAccountGdNumber == gd.gdNumber).FirstOrDefault();

                                DateTime gdDate = gd.GDDate.Value;
                                if (!gd.blDate.IsNullOrEmpty())
                                {
                                    int x = DateTime.Compare(gd.BLDateVale.Value, gd.GDDate.Value);
                                    if (x > 0)
                                    {
                                        gdDate = gd.BLDateVale.Value;
                                    }
                                }
                                gdDate = gdDate.AddDays(45);
                                if (FiData != null)
                                {
                                    GdV20Dates.Add(new GD_FI_Link()
                                    {
                                        GdId = gd.Id,
                                        FiId = FiData == null ? null : FiData.Id,
                                        type = "Open Account",
                                        MatruityDate = gdDate.ToString("dd-MMM-yyy"),
                                        _MatruityDate = gdDate
                                    });
                                }


                            }
                            #endregion  If Mode of Payemnt 305
                            else
                            {
                                //GdV20Dates.Add(new GD_FI_Link()
                                //{
                                //    GdId = gd.Id,
                                //    //    GDNumber = gd.gdNumber,
                                //    //    FINumber = item.FiNumber + " - Not in FI File"
                                //});
                            }
                        }
                    }
                    if (!GdV20Dates.IsNullOrEmpty())
                    {



                    }
                    V20Dates.AddRange(GdV20Dates);
                }
                CustomRepo.InsertFI_GD_Link(V20Dates);
                return "Sucess";
            }
            catch (Exception ex)
            {
                Seriloger.LoggerInstance.Information($"Error In Sync New Gds :{ex.Message}");
                return $"Error :{ex.Message}";
            }
        }

    }
}
