using DocumentFormat.OpenXml.Office2010.CustomUI;
using DocumentFormat.OpenXml.Wordprocessing;
using ExportOverDueFileUploader.DBmodels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public static List<GoodsDeclaration> GetGoodsDeclarationForV20Dates(long TenantId)
        {
            try
            {
                var context = new ExportOverDueContext();
                var lstResult = context.GoodsDeclarations
                                                         .FromSqlRaw($"SELECT GDDate,[Id],IsDeleted,[TenantId], [LstfinInsUniqueNumbers], [blDate], [gdNumber], [totalDeclaredValue], [MatruityDate], [V20Felids], [OutstandingAmount] FROM [GoodsDeclaration] WHERE TenantId = {TenantId} AND [MESSAGE] LIKE '%Received' AND IsDeleted=0")
                                                         
                                                         .Select(g => new
                                                         {
                                                             g.GDDate,
                                                             g.Id,
                                                             g.IsDeleted,
                                                             TenantId = g.TenantId, // Map TenantId separately since it's not prefixed with 'g.'
                                                             g.LstfinInsUniqueNumbers,

                                                             g.blDate,
                                                             g.gdNumber,
                                                             g.totalDeclaredValue,
                                                             g.MatruityDate,
                                                             g.V20Felids,
                                                             g.OutstandingAmount
                                                         })
                                                         .ToList();

                List<GoodsDeclaration> x = lstResult.Select(g => new GoodsDeclaration
                {
                    Id = g.Id,
                    IsDeleted = g.IsDeleted,
                    TenantId = g.TenantId,
                    LstfinInsUniqueNumbers = g.LstfinInsUniqueNumbers,

                    blDate = g.blDate,
                    gdNumber = g.gdNumber,
                    totalDeclaredValue = g.totalDeclaredValue,
                    MatruityDate = g.MatruityDate,
                    V20Felids = g.V20Felids,
                    OutstandingAmount = g.OutstandingAmount,
                    GDDate = g.GDDate,

                }).ToList();

                return x;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Fetching GoodsDeclaration  Data", ex.Message);
                return null;
            }
        }

        public static List<GoodsDeclaration> GetNewGoodsDeclarationForV20Dates(long TenantId ,DateTime CreationDate)
        {
            try
            {
                var context = new ExportOverDueContext();
                var result = new List<GoodsDeclaration>();
                var batchResult = context.GoodsDeclarations
                        .Where(g => g.TenantId == TenantId && EF.Functions.Like(g.MESSAGE, "%Received%") && g.IsDeleted == false && EF.Functions.DateDiffSecond(g.CreationTime, CreationDate) == 0)
                        .Select(g => new 
                        {
                            g.GDDate,
                            g.Id,
                            g.IsDeleted,
                            TenantId = g.TenantId,
                            g.LstfinInsUniqueNumbers,
                            g.blDate,
                            g.gdNumber,
                            g.totalDeclaredValue,
                            g.MatruityDate,
                            g.V20Felids,
                            g.OutstandingAmount
                        })
                        .ToList();

                


                List<GoodsDeclaration> x = batchResult.Select(g => new GoodsDeclaration
                    {
                        Id = g.Id,
                        IsDeleted = g.IsDeleted,
                        TenantId = g.TenantId,
                        LstfinInsUniqueNumbers = g.LstfinInsUniqueNumbers,

                        blDate = g.blDate,
                        gdNumber = g.gdNumber,
                        totalDeclaredValue = g.totalDeclaredValue,
                        MatruityDate = g.MatruityDate,
                        V20Felids = g.V20Felids,
                        OutstandingAmount = g.OutstandingAmount,
                        GDDate = g.GDDate,

                    }).ToList();

                    result.AddRange(x);
              

              

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Fetching GoodsDeclaration  Data", ex.Message);
                return null;
            }
        }

        public static List<GoodsDeclaration> GetGoodsDeclarationForV20Dates(long TenantId, List<string> GdNumbers,List<string> lstfis, DateTime CreationDate)
        {
            try
            {
                var context = new ExportOverDueContext();

                var lstResult = context.GoodsDeclarations.Where(x=>x.TenantId==TenantId && x.MESSAGE.Contains("Received") && x.IsDeleted==false && x.CreationTime==CreationDate )
                                                         .Select(g => new
                                                         {
                                                             g.GDDate,
                                                             g.Id,
                                                             g.IsDeleted,
                                                             TenantId = g.TenantId, // Map TenantId separately since it's not prefixed with 'g.'
                                                             g.LstfinInsUniqueNumbers,

                                                             g.blDate,
                                                             g.gdNumber,
                                                             g.totalDeclaredValue,
                                                             g.MatruityDate,
                                                             g.V20Felids,
                                                             g.OutstandingAmount
                                                         })
                                                         .ToList();

                List<GoodsDeclaration> x = lstResult.Select(g => new GoodsDeclaration
                {
                    Id = g.Id,
                    IsDeleted = g.IsDeleted,
                    TenantId = g.TenantId,
                    LstfinInsUniqueNumbers = g.LstfinInsUniqueNumbers,

                    blDate = g.blDate,
                    gdNumber = g.gdNumber,
                    totalDeclaredValue = g.totalDeclaredValue,
                    MatruityDate = g.MatruityDate,
                    V20Felids = g.V20Felids,
                    OutstandingAmount = g.OutstandingAmount,
                    GDDate = g.GDDate,

                }).ToList();

                return x;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Fetching GoodsDeclaration  Data", ex.Message);
                return null;
            }
        }

        public static List<FinancialInstrument> GetFinancialInstrumentForV20Dates(long TenantId)
        {
            try
            {
                var context = new ExportOverDueContext();
                var lstResult = context.FinancialInstruments
                                                            .FromSqlRaw($"SELECT [Id], IsDeleted, [TenantId], [TRANSACTION_TYPE], [contractCollectionData], [finInsUniqueNumber], [lcData], [modeOfPayment], [openAccountGdNumber], [paymentInformation] FROM [dbo].[FinancialInstrument] WHERE TenantId = {TenantId} AND TRANSACTION_TYPE LIKE '1524' AND IsDeleted=0")
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
                                                                f.paymentInformation
                                                            })
                                                            .ToList();

                List<FinancialInstrument> x = lstResult.Select(f => new FinancialInstrument
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
                    paymentInformation = f.paymentInformation
                }).ToList();

                return x;

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Fetching FinancialInstrument Data", ex.Message);
                return null;
            }
        }

        public static List<FinancialInstrument> GetFinancialInstrumentForV20Dates(long TenantId,List<string> fis)
        {
            try
            {
                var context = new ExportOverDueContext();
                var lstResult = context.FinancialInstruments.Where(g => g.TenantId == TenantId && g.TRANSACTION_TYPE== "1524" && g.IsDeleted == false && fis.Contains(g.finInsUniqueNumber) )
                                                            //.FromSqlRaw($"SELECT [Id], IsDeleted, [TenantId], [TRANSACTION_TYPE], [contractCollectionData], [finInsUniqueNumber], [lcData], [modeOfPayment], [openAccountGdNumber], [paymentInformation] FROM [dbo].[FinancialInstrument] WHERE TenantId = {TenantId} AND TRANSACTION_TYPE LIKE '1524' AND IsDeleted=0 And finInsUniqueNumber in ({string.Join(",", fis.Select(s => $"'{s}'"))})")
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
                                                                f.paymentInformation
                                                            })
                                                            .ToList();

                List<FinancialInstrument> x = lstResult.Select(f => new FinancialInstrument
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
                    paymentInformation = f.paymentInformation
                }).ToList();

                return x;

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Fetching FinancialInstrument Data", ex.Message);
                return null;
            }
        }


        
    }
}
