using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2010.CustomUI;
using DocumentFormat.OpenXml.Wordprocessing;
using ExportOverDueFileUploader.DataImporter;
using ExportOverDueFileUploader.DBmodels;
using Microsoft.EntityFrameworkCore;
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
            //  string update = "UPDATE GoodsDeclaration SET [V20Felids] = {0}, MatruityDate = {1} WHERE Id = {2}";
            
            foreach(var field in v20fields)
            {
                field.TenantId=AppSettings.TenantId;
                field.CreationTime=DateTime.Now;
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
                               && g.FileAuditId== fileId)
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
                var rawResult = context.FinancialInstruments.Where(g => g.TenantId == TenantId && g.TRANSACTION_TYPE == "1524" && g.IsDeleted == false && fis_gds.fis.Contains(g.finInsUniqueNumber))
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

                rawResult.AddRange(context.FinancialInstruments.Where(g => g.TenantId == TenantId && g.TRANSACTION_TYPE == "1524" && g.IsDeleted == false && fis_gds.gds.Contains(g.openAccountGdNumber))
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
                // var ids= context.FinancialInstruments.Where(g => g.TenantId == TenantId && g.TRANSACTION_TYPE == "1524" && g.IsDeleted == false)
                var rawResult = context.FinancialInstruments.Where(g => g.TenantId == TenantId && g.TRANSACTION_TYPE == "1524" && g.IsDeleted == false
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



        public static int RemoveDublicateGds(long FileAuditId)
        {
            var context = new ExportOverDueContext();
      //      var Query = $"WITH rankedgoodsdeclarations AS (\r\n  SELECT \r\n    id, \r\n    gdnumber, \r\n    row_number() OVER (\r\n      partition BY gdnumber \r\n      ORDER BY \r\n        transmission_datetime DESC\r\n    ) AS rownum \r\n  FROM \r\n    goodsdeclaration \r\n  WHERE \r\n    message_id IN (\r\n      SELECT \r\n        message_id \r\n      FROM \r\n        goodsdeclaration \r\n      WHERE \r\n
      //      LIKE 'RESPONSE' \r\n        AND status_code LIKE '200' \r\n    ) \r\n    AND direction LIKE 'REQUEST' \r\n    AND lstfininsuniquenumbers IS NOT NULL \r\n    \r\n) \r\nDELETE FROM \r\n  goodsdeclaration \r\nWHERE \r\n  id NOT IN (\r\n    select \r\n      id \r\n    from \r\n      rankedgoodsdeclarations \r\n    where \r\n      rownum = 1\r\n  ) \r\n";
            //var Query = $"WITH RankedGoodsDeclarations AS (\r\n    SELECT \r\n        Id,\r\n        gdNumber,\r\n        ROW_NUMBER() OVER (PARTITION BY gdnumber ORDER BY TRANSMISSION_DATETIME DESC) AS RowNum\r\n    FROM \r\n        goodsdeclaration \r\n    WHERE  \r\n        message_id IN (\r\n            SELECT MESSAGE_ID \r\n            FROM GoodsDeclaration \r\n            WHERE DIRECTION LIKE 'RESPONSE' \r\n            AND STATUS_CODE LIKE '200'\r\n\t\t\tand FileAuditId = {FileAuditId}\r\n        ) \r\n        AND DIRECTION LIKE 'REQUEST' \r\n        AND LstfinInsUniqueNumbers IS NOT NULL and FileAuditId = {FileAuditId}\r\n)delete from GoodsDeclaration where id  not in (\r\nSELECT \r\n    Id\r\nFROM \r\n    RankedGoodsDeclarations\r\nWHERE \r\n    RowNum = 1) and FileAuditId = {FileAuditId};\r\n\t";
            var Query = $"EXEC DeleteDuplicateGoodsDeclarations @FileAuditId = {FileAuditId}";
            var result = context.Database.ExecuteSqlRaw(Query);
            return result;

        }


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
