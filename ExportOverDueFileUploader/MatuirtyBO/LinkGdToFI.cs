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

namespace ExportOverDueFileUploader.MatuirtyBO
{
    public static class LinkGdToFI
    {

        public static List<V20DateData> LoadMatureGds()
        {
            ExportOverDueContext context = new ExportOverDueContext();
            int y = 0;
            List<GoodsDeclaration> lstgds = CustomRepo.GetGoodsDeclarationForV20Dates(AppSettings.TenantId).ToList();
            List<GD_FI_Link> V20Dates = new List<GD_FI_Link>();
            var FiDatas = CustomRepo.GetFinancialInstrumentForV20Dates(AppSettings.TenantId).ToList();
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
                        var FiData = FiDatas.Where(x => x.finInsUniqueNumber == item.FiNumber).FirstOrDefault();
                        if (item.FiNumber.IsNullOrEmpty())
                        {
                            continue;
                        }
                        if (FiData != null)
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
                            #region If Mode of Payemnt 305
                            // ie Fi is not avalible in gd
                            else if (FiData.modeOfPayment == "305")
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
                                GdV20Dates.Add(new GD_FI_Link()
                                {
                                    GdId = gd.Id,
                                    FiId = FiData.Id,
                                    MatruityDate = gdDate.ToString("dd-MMM-yyy"),
                                    _MatruityDate = gdDate
                                });
                            }
                            #endregion If Mode of Payemnt 305
                        }
                        else
                        {
                            GdV20Dates.Add(new GD_FI_Link()
                            {
                                GdId = gd.Id,
                                //    GDNumber = gd.gdNumber,
                                //    FINumber = item.FiNumber + " - Not in FI File"
                            });
                        }
                    }
                }
                if (!GdV20Dates.IsNullOrEmpty())
                {

                    gd.V20Felids = JsonConvert.SerializeObject(GdV20Dates);
                    gd.MatruityDate = GdV20Dates[0].MatruityDate != null ? DateTime.ParseExact(GdV20Dates[0].MatruityDate, "dd-MMM-yyyy", null) : null;

                    y = y + GdV20Dates.Count;
                    CustomRepo.InsertFI_GD_Link(GdV20Dates);
                    // _customRepository.UpdateGD(gd.V20Felids, DateTime.Now, gd.Id);




                }
                V20Dates.AddRange(GdV20Dates);
            }
            CustomRepo.InsertFI_GD_Link(V20Dates);
            return null;
        }




        public static string SyncNewGd(DateTime dateTime, List<string> fis )
        {
            ExportOverDueContext context = new ExportOverDueContext();
            int y = 0;
            List<GoodsDeclaration> lstgds = CustomRepo.GetGoodsDeclarationForV20Dates(AppSettings.TenantId,dateTime).ToList();

            if (lstgds.Count == 0)
            {
                return "No Gds";
            }
            List<GD_FI_Link> V20Dates = new List<GD_FI_Link>();
            var FiDatas = CustomRepo.GetFinancialInstrumentForV20Dates(AppSettings.TenantId,fis).ToList();
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
                        var FiData = FiDatas.Where(x => x.finInsUniqueNumber == item.FiNumber).FirstOrDefault();
                        if (item.FiNumber.IsNullOrEmpty())
                        {
                            continue;
                        }
                        if (FiData != null)
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
                            #region If Mode of Payemnt 305
                            // ie Fi is not avalible in gd
                            else if (FiData.modeOfPayment == "305")
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
                                GdV20Dates.Add(new GD_FI_Link()
                                {
                                    GdId = gd.Id,
                                    FiId = FiData.Id,
                                    MatruityDate = gdDate.ToString("dd-MMM-yyy"),
                                    _MatruityDate = gdDate
                                });
                            }
                            #endregion If Mode of Payemnt 305
                        }
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

                    gd.V20Felids = JsonConvert.SerializeObject(GdV20Dates);
                    gd.MatruityDate = GdV20Dates[0].MatruityDate != null ? DateTime.ParseExact(GdV20Dates[0].MatruityDate, "dd-MMM-yyyy", null) : null;

                    y = y + GdV20Dates.Count;
                    // CustomRepo.InsertFI_GD_Link(GdV20Dates);
                    // _customRepository.UpdateGD(gd.V20Felids, DateTime.Now, gd.Id);




                }
                V20Dates.AddRange(GdV20Dates);
            }
            CustomRepo.InsertFI_GD_Link(V20Dates);
            return null;



        }


        public static string SyncNewFi(DateTime dateTime)
        {
            ExportOverDueContext context = new ExportOverDueContext();
            int y = 0;
            List<GoodsDeclaration> lstgds = CustomRepo.GetGoodsDeclarationForV20Dates(AppSettings.TenantId, null, null,dateTime).ToList();
           
            List<GD_FI_Link> V20Dates = new List<GD_FI_Link>();
            var FiDatas = CustomRepo.GetFinancialInstrumentForV20Dates(AppSettings.TenantId).ToList();
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
                        var FiData = FiDatas.Where(x => x.finInsUniqueNumber == item.FiNumber).FirstOrDefault();
                        if (item.FiNumber.IsNullOrEmpty())
                        {
                            continue;
                        }
                        if (FiData != null)
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
                            #region If Mode of Payemnt 305
                            // ie Fi is not avalible in gd
                            else if (FiData.modeOfPayment == "305")
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
                                GdV20Dates.Add(new GD_FI_Link()
                                {
                                    GdId = gd.Id,
                                    FiId = FiData.Id,
                                    MatruityDate = gdDate.ToString("dd-MMM-yyy"),
                                    _MatruityDate = gdDate
                                });
                            }
                            #endregion If Mode of Payemnt 305
                        }
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

                    gd.V20Felids = JsonConvert.SerializeObject(GdV20Dates);
                    gd.MatruityDate = GdV20Dates[0].MatruityDate != null ? DateTime.ParseExact(GdV20Dates[0].MatruityDate, "dd-MMM-yyyy", null) : null;

                    y = y + GdV20Dates.Count;
                    // CustomRepo.InsertFI_GD_Link(GdV20Dates);
                    // _customRepository.UpdateGD(gd.V20Felids, DateTime.Now, gd.Id);




                }
                V20Dates.AddRange(GdV20Dates);
            }
            CustomRepo.InsertFI_GD_Link(V20Dates);
            return null;



        }
    }
}
