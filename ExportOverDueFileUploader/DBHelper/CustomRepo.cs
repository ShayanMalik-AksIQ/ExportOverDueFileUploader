using DocumentFormat.OpenXml.Drawing.Diagrams;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2010.CustomUI;
using DocumentFormat.OpenXml.Wordprocessing;
using ExportOverDueFileUploader.DataImporter;
using ExportOverDueFileUploader.DBmodels;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ExportOverDueFileUploader.DBHelper
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> source, int batchSize)
        {
            using var enumerator = source.GetEnumerator();
            while (enumerator.MoveNext())
            {
                yield return TakeBatch(enumerator, batchSize);
            }
        }

        private static IEnumerable<T> TakeBatch<T>(IEnumerator<T> enumerator, int batchSize)
        {
            do
            {
                yield return enumerator.Current;
            } while (--batchSize > 0 && enumerator.MoveNext());
        }
    }
    public static class CustomRepo
    {
        public static void InsertFI_GD_Link(List<GD_FI_Link> v20fields)
        {
            foreach (var field in v20fields)
            {
                field.TenantId = AppSettings.TenantId;
                field.CreationTime = DateTime.Now;
            }
            var context = new ExportOverDueContext();

            context.GD_FI_Links.AddRange(v20fields);
            context.SaveChanges();
        }


        #region GD_FI_Link


        #region Sync newGD
        public static List<GoodsDeclaration> GetGoodsDeclarationForV20Dates(long TenantId, long fileId)
        {
            try
            {
                var context = new ExportOverDueContext();
                var result = new List<GoodsDeclaration>();
                var rawResult = context.GoodsDeclarations
                        .Where(g => g.TenantId == TenantId && EF.Functions.Like(g.MESSAGE, "%Received%") && g.IsDeleted == false
                               && g.FileAuditId == fileId)
                        .Select(g => new
                        {
                            g.GDDate,
                            g.Id,
                            g.IsDeleted,
                            TenantId = g.TenantId,
                            g.LstfinInsUniqueNumbers,
                            g.blDate,
                            g.gdNumber,
                            g.totalDeclaredValue
                        })
                        .ToList();
                result = rawResult.Select(g => new GoodsDeclaration
                {
                    Id = g.Id,
                    IsDeleted = g.IsDeleted,
                    TenantId = g.TenantId,
                    LstfinInsUniqueNumbers = g.LstfinInsUniqueNumbers,

                    blDate = g.blDate,
                    gdNumber = g.gdNumber,
                    totalDeclaredValue = g.totalDeclaredValue,
                    GDDate = g.GDDate,

                }).ToList();
                return result;
            }
            catch (Exception ex)
            {
                Seriloger.LoggerInstance.Error("Error Fetching GoodsDeclaration  Data", ex.Message);
                return null;
            }
        }
        public static List<FinancialInstrument> GetFinancialInstrumentForV20Dates(long TenantId, NewFiGdFilterModel fis_gds)
        {
            try
            {
                var context = new ExportOverDueContext();
                // var ids= context.FinancialInstruments.Where(g => g.TenantId == TenantId && g.TRANSACTION_TYPE == "1524" && g.IsDeleted == false)
                var rawResult = context.FinancialInstruments.Where(g => g.TenantId == TenantId && g.IsDeleted == false && fis_gds.fis.Contains(g.finInsUniqueNumber))
                                                            .Select(f => new
                                                            {
                                                                f.Id,
                                                                f.IsDeleted,
                                                                TenantId = f.TenantId,
                                                                f.TRANSACTION_TYPE,
                                                                f.contractCollectionData,
                                                                f.finInsUniqueNumber,
                                                                f.lcData,
                                                                f.modeOfPayment,
                                                                f.openAccountGdNumber,
                                                                f.paymentInformation,
                                                                f.FiCertifcationdate
                                                            })
                                                            .ToList();

                rawResult.AddRange(context.FinancialInstruments.Where(g => g.TenantId == TenantId && g.IsDeleted == false && fis_gds.gds.Contains(g.openAccountGdNumber))
                                                           .Select(f => new
                                                           {
                                                               f.Id,
                                                               f.IsDeleted,
                                                               TenantId = f.TenantId,
                                                               f.TRANSACTION_TYPE,
                                                               f.contractCollectionData,
                                                               f.finInsUniqueNumber,
                                                               f.lcData,
                                                               f.modeOfPayment,
                                                               f.openAccountGdNumber,
                                                               f.paymentInformation,
                                                               f.FiCertifcationdate
                                                           })
                                                           .ToList());

                List<FinancialInstrument> result = rawResult.Select(f => new FinancialInstrument
                {
                    Id = f.Id,
                    IsDeleted = f.IsDeleted,
                    TenantId = f.TenantId,
                    TRANSACTION_TYPE = f.TRANSACTION_TYPE,
                    contractCollectionData = f.contractCollectionData,
                    finInsUniqueNumber = f.finInsUniqueNumber,
                    lcData = f.lcData,
                    modeOfPayment = f.modeOfPayment,
                    openAccountGdNumber = f.openAccountGdNumber,
                    paymentInformation = f.paymentInformation,
                    FiCertifcationdate = f.FiCertifcationdate,
                }).ToList();
                return result;

            }
            catch (Exception ex)
            {
                Seriloger.LoggerInstance.Error("Error Fetching FinancialInstrument Data", ex.Message);
                return null;
            }
        }
        #endregion  Sync newGD

        #region Sync newFi
        public static List<FinancialInstrument> GetFinancialInstrumentForV20Dates(long TenantId, long fileId)
        {
            try
            {
                var context = new ExportOverDueContext();
                var rawResult = context.FinancialInstruments.Where(g => g.TenantId == TenantId && g.IsDeleted == false
                                                                    && g.FileAuditId == fileId)
                                                            .Select(f => new
                                                            {
                                                                f.Id,
                                                                f.IsDeleted,
                                                                TenantId = f.TenantId,
                                                                f.TRANSACTION_TYPE,
                                                                f.contractCollectionData,
                                                                f.finInsUniqueNumber,
                                                                f.lcData,
                                                                f.modeOfPayment,
                                                                f.openAccountGdNumber,
                                                                f.paymentInformation,
                                                                f.FiCertifcationdate
                                                            })
                                                            .ToList();

                List<FinancialInstrument> result = rawResult.Select(f => new FinancialInstrument
                {
                    Id = f.Id,
                    IsDeleted = f.IsDeleted,
                    TenantId = f.TenantId,
                    TRANSACTION_TYPE = f.TRANSACTION_TYPE,
                    contractCollectionData = f.contractCollectionData,
                    finInsUniqueNumber = f.finInsUniqueNumber,
                    lcData = f.lcData,
                    modeOfPayment = f.modeOfPayment,
                    openAccountGdNumber = f.openAccountGdNumber,
                    paymentInformation = f.paymentInformation,
                    FiCertifcationdate = f.FiCertifcationdate,
                }).ToList();
                return result;

            }
            catch (Exception ex)
            {
                Seriloger.LoggerInstance.Error("Error Fetching FinancialInstrument Data", ex.Message);
                return null;
            }
        }
        public static List<GoodsDeclaration> GetGoodsDeclarationForV20Dates(NewFiGdFilterModel fis_gds, long TenantId)
        {
            try
            {
                var context = new ExportOverDueContext();
                var result = new List<GoodsDeclaration>();
                var rawResult = context.GoodsDeclarations
                        .Where(g => g.TenantId == TenantId && EF.Functions.Like(g.MESSAGE, "%Received%") && g.IsDeleted == false
                               && fis_gds.gds.Contains(g.gdNumber))
                        .Select(g => new
                        {
                            g.GDDate,
                            g.Id,
                            g.IsDeleted,
                            TenantId = g.TenantId,
                            g.LstfinInsUniqueNumbers,
                            g.blDate,
                            g.gdNumber,
                            g.totalDeclaredValue
                        })
                        .ToList();
                if (!rawResult.IsNullOrEmpty())
                {
                    var fisInGd = context.Database
                                         .SqlQuery<FisInGdView>(FormattableStringFactory.Create($"select id,LstfinInsUniqueNumbers from [FIsInGoodsDeclarations] where TenantId={TenantId}"))
                                         .ToList().Where(x => fis_gds.fis.Contains(x.LstfinInsUniqueNumbers)).Select(x => x.id).ToList();

                    rawResult.AddRange(context.GoodsDeclarations
                            .Where(g => fisInGd.Contains(g.Id))
                            .Select(g => new
                            {
                                g.GDDate,
                                g.Id,
                                g.IsDeleted,
                                TenantId = g.TenantId,
                                g.LstfinInsUniqueNumbers,
                                g.blDate,
                                g.gdNumber,
                                g.totalDeclaredValue
                            })
                            .ToList());

                    result = rawResult.Select(g => new GoodsDeclaration
                    {
                        Id = g.Id,
                        IsDeleted = g.IsDeleted,
                        TenantId = g.TenantId,
                        LstfinInsUniqueNumbers = g.LstfinInsUniqueNumbers,
                        blDate = g.blDate,
                        gdNumber = g.gdNumber,
                        totalDeclaredValue = g.totalDeclaredValue,
                        GDDate = g.GDDate,

                    }).ToList();
                }
                return result;
            }
            catch (Exception ex)
            {
                Seriloger.LoggerInstance.Error("Error Fetching GoodsDeclaration  Data", ex.Message);
                return null;
            }
        }
        #endregion  Sync newFI


        #endregion  GD_FI_Link



        //public static int RemoveDublicateGdsFileWise(long FileAuditId)
        //{
        //    try
        //    {

        //        var context = new ExportOverDueContext();
        //        var Query = $"EXEC DeleteDuplicateGoodsDeclarationsFileWise @FileAuditId = {FileAuditId}";
        //        var result = context.Database.ExecuteSqlRaw(Query);
        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        Seriloger.LoggerInstance.Error("Error RemoveDublicateGds File Wise Data", ex.Message);
        //        return 0;
        //    }

        //}
        public static int RemoveDublicate(string Entity, long FileAuditId = 0)
        {
            try
            {
                string Query="";
                var context = new ExportOverDueContext();

                if (Entity == "GoodsDeclaration")
                {
                    Query = $"EXEC DeleteDuplicateGoodsDeclarations";
                    if (FileAuditId > 0)
                    {
                        Query = $"{Query}FileWise  @FileAuditId = {FileAuditId}";
                    }
                }
                else if (Entity == "FinancialInstrument")
                {
                    Query = $"EXEC DeleteDuplicateFinancialInstrument";
                    if (FileAuditId > 0)
                    {
                        Query = $"{Query}FileWise  @FileAuditId = {FileAuditId}";
                    }
                }
                else
                {
                    return 0;
                }

                
                var result = context.Database.ExecuteSqlRaw(Query);
                Seriloger.LoggerInstance.Information($"{Entity} of file id:{FileAuditId} dublication Removed ");
                return result;
            }
            catch (Exception ex)
            {
                Seriloger.LoggerInstance.Error("Error RemoveDublicateGds Data", ex.Message);
                return 0;
            }

        }
        //public static int RemoveDublicateGds()
        //{
        //    try
        //    {

        //        var context = new ExportOverDueContext();
        //        var Query = $"EXEC DeleteDuplicateGoodsDeclarations";
        //        var result = context.Database.ExecuteSqlRaw(Query);
        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        Seriloger.LoggerInstance.Error("Error RemoveDublicateGds Data", ex.Message);
        //        return 0;
        //    }

        //}

        //public static List<GoodsDeclaration> GetGoodsDeclarationForV20Dates(long TenantId)
        //{
        //    try
        //    {
        //        var context = new ExportOverDueContext();
        //        var lstResult = context.GoodsDeclarations
        //                                                 .FromSqlRaw($"SELECT GDDate,[Id],IsDeleted,[TenantId], [LstfinInsUniqueNumbers], [blDate], [gdNumber], [totalDeclaredValue] FROM [GoodsDeclaration] WHERE TenantId = {TenantId} AND [MESSAGE] LIKE '%Received' AND IsDeleted=0")

        //                                                 .Select(g => new
        //                                                 {
        //                                                     g.GDDate,
        //                                                     g.Id,
        //                                                     g.IsDeleted,
        //                                                     TenantId = g.TenantId, // Map TenantId separately since it's not prefixed with 'g.'
        //                                                     g.LstfinInsUniqueNumbers,

        //                                                     g.blDate,
        //                                                     g.gdNumber,
        //                                                     g.totalDeclaredValue,
        //                                                 })
        //                                                 .ToList();

        //        List<GoodsDeclaration> x = lstResult.Select(g => new GoodsDeclaration
        //        {
        //            Id = g.Id,
        //            IsDeleted = g.IsDeleted,
        //            TenantId = g.TenantId,
        //            LstfinInsUniqueNumbers = g.LstfinInsUniqueNumbers,

        //            blDate = g.blDate,
        //            gdNumber = g.gdNumber,
        //            totalDeclaredValue = g.totalDeclaredValue,
        //            GDDate = g.GDDate,

        //        }).ToList();

        //        return x;
        //    }
        //    catch (Exception ex)
        //    {
        //        Seriloger.LoggerInstance.Error("Error Fetching GoodsDeclaration  Data", ex.Message);
        //        return null;
        //    }
        //}

        //public static List<GoodsDeclaration> GetGoodsDeclarationForV20Dates(long TenantId, List<string> GdNumbers,List<string> lstfis, DateTime CreationDate)
        //{
        //    try
        //    {
        //        var context = new ExportOverDueContext();

        //        var lstResult = context.GoodsDeclarations.Where(x=>x.TenantId==TenantId && x.MESSAGE.Contains("Received") && x.IsDeleted==false && x.CreationTime==CreationDate )
        //                                                 .Select(g => new
        //                                                 {
        //                                                     g.GDDate,
        //                                                     g.Id,
        //                                                     g.IsDeleted,
        //                                                     TenantId = g.TenantId, // Map TenantId separately since it's not prefixed with 'g.'
        //                                                     g.LstfinInsUniqueNumbers,

        //                                                     g.blDate,
        //                                                     g.gdNumber,
        //                                                     g.totalDeclaredValue
        //                                                 })
        //                                                 .ToList();

        //        List<GoodsDeclaration> x = lstResult.Select(g => new GoodsDeclaration
        //        {
        //            Id = g.Id,
        //            IsDeleted = g.IsDeleted,
        //            TenantId = g.TenantId,
        //            LstfinInsUniqueNumbers = g.LstfinInsUniqueNumbers,

        //            blDate = g.blDate,
        //            gdNumber = g.gdNumber,
        //            totalDeclaredValue = g.totalDeclaredValue,
        //            GDDate = g.GDDate,

        //        }).ToList();

        //        return x;
        //    }
        //    catch (Exception ex)
        //    {
        //        Seriloger.LoggerInstance.Error("Error Fetching GoodsDeclaration  Data", ex.Message);
        //        return null;
        //    }
        //}

        //public static List<FinancialInstrument> GetFinancialInstrumentForV20Dates(long TenantId)
        //{
        //    try
        //    {
        //        var context = new ExportOverDueContext();
        //        var lstResult = context.FinancialInstruments
        //                                                    .FromSqlRaw($"SELECT [Id], IsDeleted, [TenantId], [TRANSACTION_TYPE], [contractCollectionData], [finInsUniqueNumber], [lcData], [modeOfPayment], [openAccountGdNumber], [paymentInformation] FROM [dbo].[FinancialInstrument] WHERE TenantId = {TenantId} AND TRANSACTION_TYPE LIKE '1524' AND IsDeleted=0")
        //                                                    .Select(f => new
        //                                                    {
        //                                                        f.Id,
        //                                                        f.IsDeleted,
        //                                                        TenantId = f.TenantId,
        //                                                        f.TRANSACTION_TYPE,
        //                                                        f.contractCollectionData,
        //                                                        f.finInsUniqueNumber,
        //                                                        f.lcData,
        //                                                        f.modeOfPayment,
        //                                                        f.openAccountGdNumber,
        //                                                        f.paymentInformation
        //                                                    })
        //                                                    .ToList();

        //        List<FinancialInstrument> x = lstResult.Select(f => new FinancialInstrument
        //        {
        //            Id = f.Id,
        //            IsDeleted = f.IsDeleted,
        //            TenantId = f.TenantId,
        //            TRANSACTION_TYPE = f.TRANSACTION_TYPE,
        //            contractCollectionData = f.contractCollectionData,
        //            finInsUniqueNumber = f.finInsUniqueNumber,
        //            lcData = f.lcData,
        //            modeOfPayment = f.modeOfPayment,
        //            openAccountGdNumber = f.openAccountGdNumber,
        //            paymentInformation = f.paymentInformation
        //        }).ToList();

        //        return x;

        //    }
        //    catch (Exception ex)
        //    {
        //        Seriloger.LoggerInstance.Error("Error Fetching FinancialInstrument Data", ex.Message);
        //        return null;
        //    }
        //}





    }
}
