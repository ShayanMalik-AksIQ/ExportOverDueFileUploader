using Azure.Core;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2010.Excel;
using ExportOverDueFileUploader.DBHelper;
using ExportOverDueFileUploader.DBmodels;
using ExportOverDueFileUploader.MatuirtyBO;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static ExportOverDueFileUploader.DataImporter.GdImporter;

namespace ExportOverDueFileUploader.DataImporter
{
    public class Uploader
    {

        List<string> TableNames = new List<string>();
        public DateTime DateTimeNow = new DateTime();
        private readonly ILogger _logger;
        public Uploader()
        {
            //_logger = new Logger<>();
            TableNames.Add("FileType");
            TableNames.Add("GoodsDeclaration");
            TableNames.Add("FinancialInstrument");
            TableNames.Add("LetterOfCredit");
            TableNames.Add("DocumentaryCollection");
            TableNames.Add("BcaData");

        }
        public void Executeion()
        {
            try
            {
                Seriloger.LoggerInstance.Information("Export Over Due Uploader Execution Begins ......");
                ExportOverDueContext context = new ExportOverDueContext();
                var maxLoadingOrder = context.RequestStatuses.Where(x => x.Module.ModuleName.ToLower() == "Setups".ToLower()).Select(x => x.LoadingOrder).Max();

                var lstFileTypes = context.FileTypes.Where(x => x.TenantId == AppSettings.TenantId && x.IsDeleted == false && x.RequestStatus.LoadingOrder == maxLoadingOrder).ToList();//aproved only
                if (lstFileTypes.IsNullOrEmpty())
                {
                    Seriloger.LoggerInstance.Error("Error:No File Type In DB..");
                    return;
                }
                foreach (var fileType in lstFileTypes)
                {
                    Seriloger.LoggerInstance.Information($"File:{fileType.Name} Folder:{fileType.FilePath} In Progress....");
                    var BasePath = fileType.FilePath;
                    var Files = GetFileNamesInFolder(BasePath);
                    if (Files == null)
                    {
                        Seriloger.LoggerInstance.Information($"File:{fileType.Name} Folder:{fileType.FilePath} In Progress");
                    }
                    foreach (var file in Files)
                    {
                        FileImportAuditTrail auditTrail = new FileImportAuditTrail();
                        try
                        {
                            string FileJsonData = string.Empty;
                            Seriloger.LoggerInstance.Error($"FileTyoe:{fileType.Name} File:{file} Reading In Progress....");
                            if (file.EndsWith(".xlsx") || file.EndsWith(".xls"))
                            {
                                FileJsonData = FileReader.ReadAndValidateExcelFile(Path.Combine(BasePath, file), fileType.HeaderRow == 0 ? 1 : fileType.HeaderRow, fileType.ColumnNames);
                            }
                            else if (file.EndsWith(".csv"))
                            {
                                FileJsonData = FileReader.ReadAndValidateCsvFile(Path.Combine(BasePath, file), fileType.HeaderRow == 0 ? 1 : fileType.HeaderRow, fileType.ColumnNames);
                            }
                            else
                            {
                                Seriloger.LoggerInstance.Error("File Type Incorect");
                                continue;
                            }
                            try
                            {

                                auditTrail.FileName = file;
                                auditTrail.FileTypeId = fileType.Id;//
                                if (FileJsonData != null && !FileJsonData.StartsWith("Error"))
                                {
                                    Seriloger.LoggerInstance.Information($"{fileType.Name} file:{file} Reading Sucess {Files.IndexOf(file) + 1}/{Files.Count}");
                                    DateTimeNow = DateTime.Now;
                                    auditTrail.TenantId = AppSettings.TenantId;
                                    auditTrail.CreationTime = DateTimeNow;
                                    context.FileImportAuditTrails.Add(auditTrail);
                                    context.SaveChanges();
                                    var filters = ImportData(FileJsonData, fileType.Description, auditTrail.Id, file);
                                    Seriloger.LoggerInstance.Information($"{fileType.Name} file:{file} Db Export Sucess {Files.IndexOf(file) + 1}/{Files.Count}");
                                    auditTrail.Remarks = "Success";
                                    auditTrail.Success = true;
                                    if (fileType.Description == "GoodsDeclaration")
                                    {
                                        LinkGdToFI.SyncNewGd(auditTrail.Id, filters);
                                    }
                                    if (fileType.Description == "FinancialInstrument")
                                    {
                                        LinkGdToFI.SyncNewFi(auditTrail.Id, filters);
                                    }
                                    Seriloger.LoggerInstance.Information($"{fileType.Name} file:{file} Sync Sucess {Files.IndexOf(file) + 1}/{Files.Count}");
                                }
                                else
                                {
                                    auditTrail.Success = false;
                                    auditTrail.Remarks = $"{FileJsonData}";
                                    Seriloger.LoggerInstance.Information($"{fileType.Name} file:{file} Hadders MissMached {Files.IndexOf(file) + 1}/{Files.Count}");
                                }
                            }
                            catch (Exception ex)
                            {
                                auditTrail.Success = false;
                                auditTrail.Remarks = $"{ex.Message}";
                                Seriloger.LoggerInstance.Information($"Error :{ex.Message} on  {fileType.Name} file:{file} - {Files.IndexOf(file) + 1}/{Files.Count}");
                            }
                            finally
                            {
                                try
                                {
                                    ExportOverDueContext context1 = new ExportOverDueContext();
                                    context1.FileImportAuditTrails.Update(auditTrail);
                                    context1.SaveChanges();
                                    Seriloger.LoggerInstance.Information($"{fileType.Name} file:{file} {Files.IndexOf(file) + 1}/{Files.Count}  Uploaded Sucess With Audit");
                                }
                                catch (Exception ex)
                                {
                                    Seriloger.LoggerInstance.Error($"Error :{ex.Message} {fileType.Name} file:{file} {Files.IndexOf(file) + 1}/{Files.Count}  Uploaded Failed ");

                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Seriloger.LoggerInstance.Error(ex.Message);
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                Seriloger.LoggerInstance.Error(ex.Message);
            }

        }







        public void Executeion(DataTable dataTable, string fileName, string EntityName)
        {
            FileImportAuditTrail auditTrail = new FileImportAuditTrail();
            ExportOverDueContext context = new ExportOverDueContext();
            try
            {
                long id = context.FileTypes.FirstOrDefault(x => x.Description == "BcaData").Id;
                auditTrail.FileName = fileName;
                auditTrail.FileTypeId = id;//
                context.FileImportAuditTrails.Add(auditTrail);
                context.SaveChanges();
                if (dataTable != null)
                {
                    var filters = ImportData(null, fileName, auditTrail.Id, EntityName, dataTable);
                    Seriloger.LoggerInstance.Information($"Bca file:{fileName} Db Export Sucess");
                    auditTrail.Remarks = "Success";
                    auditTrail.Success = true;

                }
                else
                {
                    auditTrail.Success = false;
                }

            }
            catch (Exception ex)
            {
                auditTrail.Success = false;
                auditTrail.Remarks = $"{ex.Message}";
                Seriloger.LoggerInstance.Error($"Error :{ex.Message} on  BCA file:{fileName} ");
            }
            finally
            {

                try
                {
                    auditTrail.TenantId = AppSettings.TenantId;
                    auditTrail.CreationTime = DateTimeNow;
                    ExportOverDueContext context1 = new ExportOverDueContext();
                    context1.FileImportAuditTrails.Update(auditTrail);
                    context1.SaveChanges();
                    Seriloger.LoggerInstance.Information($"Bca file:{fileName} Uploaded Sucess With Audit");
                }
                catch (Exception ex)
                {
                    Seriloger.LoggerInstance.Error($"Error :{ex.Message} Bca file:{fileName}  Uploaded Failed ");

                }
            }

        }







        public NewFiGdFilterModel ImportData(string jsondata, string EntityName, long FileID, string fileName, DataTable dataTable = null)
        {
            cobReturn result = new cobReturn();
            List<string?> msgIds = new List<string?>(); 
            if (TableNames.Contains(EntityName))
            {
                try
                {
                    List<string> newGdFis = new List<string>();
                    DataTable data = dataTable;
                    if (dataTable == null)
                    {
                        data = JsonConvert.DeserializeObject<DataTable>(jsondata.ToString());
                    }
                    else
                    {
                        data = dataTable;
                    }
                    if (EntityName == "FinancialInstrument")
                    {

                        DataTable BcaData = data.Select("TRANSACTION_TYPE = 1526 AND RESPONSE_CODE = 200").CopyToDataTable();
                        data = data.Select("TRANSACTION_TYPE = 1524 AND RESPONSE_CODE = 200").CopyToDataTable();
                        if (BcaData != null && BcaData.Rows.Count > 0)
                        {
                            Executeion(BcaData, "BcaData", fileName);
                        }
                        if (data.Rows.Count == 0)
                        {
                            return null;
                        }

                    }
                    if (EntityName == "GoodsDeclaration")
                    {

                        data = data.Select("MESSAGE_TYPE = 307 OR MESSAGE_TYPE = 102").CopyToDataTable();
                      // data = data.Select("DIRECTION = 'REQUEST'").CopyToDataTable();
                        if (data.Rows.Count == 0)
                        {
                            return null;
                        }

                    }
                    data.Columns.Add("CreationTime");
                    data.Columns.Add("IsDeleted");
                    data.Columns.Add("TenantId");
                    data.Columns.Add("CreatorUserId");
                    data.Columns.Add("FileAuditId");
                    if (EntityName == "GoodsDeclaration")
                    {
                        AddColumns(data, GdImporter.GdColumns);
                    }
                    else if (EntityName == "FinancialInstrument")
                    {
                        AddColumns(data, FiImporter.FiColoums);
                    }
                    else if (EntityName == "BcaData")
                    {
                        AddColumns(data, FiImporter.BcaColoums);
                    }
                    if (data.Columns.Contains("ID"))
                    {
                        data.Columns.Add("I_D");
                    }
                    int tenantId = AppSettings.TenantId;
                     if (EntityName == "GoodsDeclaration")
                    {
                        msgIds = ExtractOkMessageId(data);
                        
                    }
                    foreach (DataRow _row in data.Rows)
                    {
                        if (data.Columns.Contains("ID"))
                        {
                            _row["I_D"] = _row["ID"];
                        }
                        if (data.Columns.Contains("TRANSMISSION_DATETIME"))
                        {
                            _row["TRANSMISSION_DATETIME"] = GdImporter.ConvertTransmissionDate(_row["TRANSMISSION_DATETIME"].ToString());
                        }
                        _row["CreationTime"] = DateTimeNow;
                        _row["IsDeleted"] = false;
                        _row["TenantId"] = tenantId;
                        _row["CreatorUserId"] = null;
                        _row["FileAuditId"] = FileID;


                        if (EntityName == "GoodsDeclaration")
                        {
                            if ((!_row["MESSAGE_ID"].ToString().IsNullOrEmpty()) && _row["DIRECTION"].ToString() == "REQUEST" && msgIds.Contains(_row["MESSAGE_ID"].ToString()) && _row["MESSAGE_TYPE"].ToString() != "101")
                            {
                                _row["STATUS_CODE"] = "OK";

                                List<string> fis = new List<string>();
                                if (_row["MESSAGE_TYPE"].ToString() == "307")
                                {
                                    result = GdImporter.LoadCobGdInfoColoums(_row);
                                    fis = result?.fiNumbers;

                                }
                                else
                                {
                                    fis = GdImporter.LoadGdInfoColoums(_row);
                                }
                                if (!fis.IsNullOrEmpty())
                                {
                                    newGdFis.AddRange(fis);
                                }
                            }

                        }
                        else if (EntityName == "FinancialInstrument")
                        {
                            FiImporter.LoadFIInfoColoums(_row);
                        }
                        else if (EntityName == "BcaData")
                        {
                            FiImporter.LoadBcaInfoColoums(_row);
                        }
                    }
                    if (data.Columns.Contains("ID"))
                    {
                        data.Columns.Remove("ID");
                    }
                    if (result?.aditionalRows?.Count() > 0)
                    {
                        DataTable cobData = data.Clone();
                        foreach (DataRow row in result?.aditionalRows)
                        {
                            // Extract values from the DataRow
                            object[] rowArray = row.ItemArray;

                            // Add the rowArray to cobData
                            cobData.Rows.Add(rowArray);
                        }
                        data.Merge(cobData);
                    }
                    if (EntityName == "GoodsDeclaration")
                    {
                        data = data.Select("STATUS_CODE = 'OK'").CopyToDataTable();
                    }
                    BulkInsert(data, EntityName);

                    // CustomRepo.RemoveDublicateGds(FileID);
                    NewFiGdFilterModel filter = new NewFiGdFilterModel();
                    if (EntityName == "GoodsDeclaration")
                    {
                        filter = ExtractFiNumberFromNewGDs(data);
                    }
                    if (EntityName == "FinancialInstrument")
                    {
                        filter = ExtractFilisterList(data);
                        //filter = gdNumbers;
                    }
                    return filter;
                }
                catch (Exception ex)
                {

                    Seriloger.LoggerInstance.Error($"Error In Import Data:{ex.Message}");
                    return null;
                }
            }
            else
            {
                Seriloger.LoggerInstance.Error($"Incorrect Table Name ");
                return null;
            }
        }
        public void BulkInsert(DataTable dt, string entityname)
        {
            try
            {// get fro df
                var connectionString = AppSettings.ConnectionString;

                int batchSize = AppSettings.BatchSize; // Set your desired batch size df
                int totalRows = dt.Rows.Count;

                using (var sqlbulk = new SqlBulkCopy(connectionString))
                {
                    sqlbulk.DestinationTableName = entityname;
                    for (int i = 0; i < totalRows; i += batchSize)
                    {
                        var currentBatch = dt.AsEnumerable().Skip(i).Take(batchSize).CopyToDataTable();
                        sqlbulk.ColumnMappings.Clear();
                        foreach (DataColumn _col in currentBatch.Columns)
                        {
                            sqlbulk.ColumnMappings.Add(_col.ColumnName, _col.ColumnName);
                        }
                        sqlbulk.WriteToServer(currentBatch);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        private void AddColumns(DataTable xlsxFi, List<string> columnNames)//gd or fi list 
        {
            foreach (string columnName in columnNames)
            {
                xlsxFi.Columns.Add(columnName);
            }
        }

        private List<string> GetFileNamesInFolder(string folderPath)
        {
            try
            {
                // Check if the folder exists
                if (Directory.Exists(folderPath))
                {
                    // Get all file names in the folder
                    return Directory.GetFiles(folderPath)
                        .Select(Path.GetFileName).Where(x => x.EndsWith(".csv") || x.EndsWith(".xlsx"))
                        .ToList();
                }
                else
                {
                    Seriloger.LoggerInstance.Information($"Folder not found: {folderPath}");
                    return null; // Return an empty array if the folder doesn't exist
                }
            }
            catch (Exception ex)
            {
                Seriloger.LoggerInstance.Error($"Error: {ex.Message}");
                return null; // Return an empty array in case of an exception
            }
        }

        private NewFiGdFilterModel ExtractFiNumberFromNewGDs(DataTable dataTable)
        {
            List<string> fis = new List<string>();
            List<string> gds = new List<string>();

            foreach (DataRow row in dataTable.Rows)
            {

                string LstfinInsUniqueNumbers = row["LstfinInsUniqueNumbers"].ToString();
                List<FiNumberAndMode> fiNumberAndModes = new List<FiNumberAndMode>();
                if (LstfinInsUniqueNumbers != null && LstfinInsUniqueNumbers != "")
                {
                    foreach (var fi in LstfinInsUniqueNumbers.Split(","))
                    {
                        var x = new FiNumberAndMode
                        {
                            FiNumber = Regex.Match(fi, @"^(?<FiNumber>[\w-]+)(\((?<Value>\d+)\))?$").Groups["FiNumber"].Value ?? null,
                            ModeOFPayment = Regex.Match(fi, @"^(?<FiNumber>[\w-]+)(\((?<Value>\d+)\))?$").Groups["Value"]?.Value ?? null
                        };
                        fis.Add(x.FiNumber);
                        if (fi == "(305)")
                        {
                            if (row["gdNumber"] != null)
                            {
                                gds.Add(row["gdNumber"].ToString());
                            }
                        }
                    }
                }


            }
            var xx = fis.Select(x => x != null || x != "")
                  .Distinct()
                 .Select(b => b.ToString())
                .ToList();

            var y = gds.Select(x => x != null || x != "")
                 .Distinct()
                .Select(b => b.ToString())
               .ToList();

            return new NewFiGdFilterModel
            {
                fis = fis.Where(x => x != null && x != "")
                         .Distinct()
                         .ToList(),
                gds = gds.Where(x => x != null && x != "")
                                  .Distinct()
                                  .ToList()
            };
        }

        private NewFiGdFilterModel ExtractFilisterList(DataTable dataTable)
        {
            List<string> gdNumberList = new List<string>();
            List<string> fis = new List<string>();

            foreach (DataRow row in dataTable.Rows)
            {
                string gd = row["openAccountGdNumber"].ToString();
                string fi = row["finInsUniqueNumber"].ToString();
                if (gd != "")
                {
                    gdNumberList.Add(gd);
                }
                if (fi != "")
                {
                    fis.Add(fi);
                }
            }

            //var x = fis.Where(x => x != null && x != "")
            //             .Distinct()
            //             .ToList();
            //var y = gdNumberList.Where(x => x != null && x != "")
            //                      .Distinct()
            //                      .ToList();

            return new NewFiGdFilterModel
            {
                fis = fis.Where(x => x != null && x != "")
                         .Distinct()
                         .ToList(),
                gds = gdNumberList.Where(x => x != null && x != "")
                                  .Distinct()
                                  .ToList()
            };
        }


        private List<string?> ExtractOkMessageId(DataTable dataTable)
        {
            List<string?> msgIds = new List<string?>();

            foreach (DataRow row in dataTable.Rows)
            {
                if(row["DIRECTION"].ToString()== "RESPONSE" && row["STATUS_CODE"].ToString() == "200" && row["MESSAGE_TYPE"].ToString()!="101")
                {
                    msgIds.Add(row["MESSAGE_ID"].ToString());
                }
            }
            return msgIds;
        }

    }


}
