﻿using Azure.Core;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2010.Excel;
using ExportOverDueFileUploader.DBHelper;
using ExportOverDueFileUploader.DBmodels;
using ExportOverDueFileUploader.MatuirtyBO;
using ExportOverDueFileUploader.Modles.JsonHelper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
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
            TableNames.Add("ITRS_Data");
            TableNames.Add("NtnConversion");
            TableNames.Add("RealizationReport");
            TableNames.Add("CobGds");
            TableNames.Add("v20SavedData");

        }
        public void Executeion()
        {
            try
            {
                Seriloger.LoggerInstance.Information("Export Over Due Uploader Execution Begins ......");
                ExportOverDueContext context = new ExportOverDueContext();
                var settings = context.DefaultSettings.ToList();
                var maxLoadingOrder = context.RequestStatuses.Where(x => x.Module.ModuleName.ToLower() == "Setups".ToLower()).Select(x => x.LoadingOrder).Max();
                var lstFileTypes = context.FileTypes.Where(x => x.TenantId == AppSettings.TenantId && x.IsDeleted == false && x.RequestStatus.LoadingOrder == maxLoadingOrder).ToList();//aproved only
                if (lstFileTypes.IsNullOrEmpty())
                {
                    Seriloger.LoggerInstance.Error("Error:No File Type In DB..");
                    return;
                }

             //FileReader.DownloadFromFtp(lstFileTypes);

                foreach (var fileType in lstFileTypes)
                {
                    Seriloger.LoggerInstance.Information($"File:{fileType.Name} Folder:{fileType.FilePath} In Progress....");
                    var BasePath = fileType.FilePath;
                    var Files = GetFileNamesInFolder(BasePath);
                    if (Files == null)
                    {
                        Seriloger.LoggerInstance.Error($"Folder:{fileType.FilePath} has No Files");
                        continue;

                    }
                    foreach (var file in Files)
                    {
                        FileImportAuditTrail auditTrail = new FileImportAuditTrail();
                        try
                        {
                            string FileJsonData = string.Empty;
                            Seriloger.LoggerInstance.Information($"FileTyoe:{fileType.Name} File:{file} Reading In Progress....");
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
                                    var filters = ImportData(FileJsonData, fileType.Description, auditTrail.Id, file, fileType.ColumnRename);
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
                                    }if (fileType.Description == "DocumentaryCollection")
                                    {
                                        LinkGdToFI.SyncDcDueDates(fileType.Description);
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
                                    Seriloger.LoggerInstance.Information($"-------------------------{file} Sucess---------------------------");
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
                    CustomRepo.RemoveDublicate(fileType.Description);
                    FileReader.MoveFiles(fileType.FilePath, "*.xlsx", "*.csv");


                }

            }
            catch (Exception ex)
            {
                Seriloger.LoggerInstance.Error(ex.Message);
            }

        }
        public void Executeion(DataTable dataTable, string EntityName, string fileName)
        {
            FileImportAuditTrail auditTrail = new FileImportAuditTrail();
            ExportOverDueContext context = new ExportOverDueContext();
            try
            {
                long id = context.FileTypes.FirstOrDefault(x => x.Description == EntityName).Id;
                auditTrail.FileName = fileName;
                auditTrail.FileTypeId = id;//
                context.FileImportAuditTrails.Add(auditTrail);
                context.SaveChanges();
                if (dataTable != null)
                {
                    var filters = ImportData(null, EntityName, auditTrail.Id, fileName, "", dataTable);
                    if (EntityName == "GoodsDeclaration")
                    {
                        LinkGdToFI.SyncNewGd(auditTrail.Id, filters);
                    }
                    if (EntityName == "FinancialInstrument")
                    {
                        LinkGdToFI.SyncNewFi(auditTrail.Id, filters);
                    }
                    Seriloger.LoggerInstance.Information($"{EntityName} file:{fileName} Db Export Sucess");
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
                Seriloger.LoggerInstance.Error($"Error :{ex.Message} on  {EntityName} file:{fileName} ");
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
                    Seriloger.LoggerInstance.Information($"{EntityName} file:{fileName} Uploaded Sucess With Audit");
                }
                catch (Exception ex)
                {
                    Seriloger.LoggerInstance.Error($"Error :{ex.Message} Bca file:{fileName}  Uploaded Failed ");

                }
            }

        }
        public NewFiGdFilterModel ImportData(string jsondata, string EntityName, long FileID, string fileName, string coloumRename, DataTable dataTable = null)
        {
            cobReturn result = new cobReturn();
            List<FinancialInstrumentInfo> lstCobFis = new List<FinancialInstrumentInfo>();
            List<string?> msgIds = new List<string?>();
            if (TableNames.Contains(EntityName))
            {
                try
                {
                    List<string> newGdFis = new List<string>();
                    DataTable data = dataTable;
                    if (dataTable == null)
                    {
                        JsonSerializerSettings settings = new JsonSerializerSettings
                        {
                            Converters = new List<JsonConverter> { new ForceStringConverter() },
                            DateParseHandling = DateParseHandling.None
                        };

                        data = JsonConvert.DeserializeObject<DataTable>(jsondata, settings);
                        //data = JsonConvert.DeserializeObject<DataTable>(jsondata.ToString());
                    }
                    else
                    {
                        data = dataTable;
                    }
                    DataTable BcaData = new DataTable();
                    if (EntityName == "FinancialInstrument" && dataTable == null)
                    {
                        var bca = data.Select("TRANSACTION_TYPE = '1526'");
                        var fis = data.Select("TRANSACTION_TYPE = '1524'");
                        if (bca.Count() != 0)
                        {
                            BcaData = bca.CopyToDataTable();
                        }
                        if (fis.Count() != 0)
                        {
                            data = fis.CopyToDataTable();
                        }
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
                        // msgIds = ExtractOkMessageId(data);
                        data = data.Select("MESSAGE_TYPE = '102'").CopyToDataTable();
                        //data = data.Select("MESSAGE_TYPE = '307' OR MESSAGE_TYPE = '102'").CopyToDataTable();
                        data = data.Select("DIRECTION = 'REQUEST'").CopyToDataTable();
                        if (data.Rows.Count == 0)
                        {
                            return null;
                        }
                    }

                    //if (EntityName == "LetterOfCredit")
                    //{
                    //    data = data.Select("LCReference <> '' OR LCNo <> ''").CopyToDataTable();
                    //}
                    //if (EntityName == "DocumentaryCollection")
                    //{
                    //    data = data.Select("CollectionNumber <> '' OR TransactionRef <> ''").CopyToDataTable();
                    //}

                    if (EntityName != "ITRS_Data" && EntityName != "CobGds" && EntityName != "NtnConversion" && EntityName != "RealizationReport")
                    {

                        data.Columns.Add("CreationTime");
                        data.Columns.Add("IsDeleted");
                        data.Columns.Add("CreatorUserId");

                    }
                    if (!coloumRename.IsNullOrEmpty())
                    {
                        ModifyDataTable(data, coloumRename?.Split("||").ToList());

                    }
                    if (EntityName == "LetterOfCredit" || EntityName == "DocumentaryCollection")
                    {
                        data = data.Select("GdNumber <> ''").CopyToDataTable();
                        if (data.Rows.Count == 0)
                        {
                            return null;
                        }
                    }
                    

                    data.Columns.Add("TenantId");
                    data.Columns.Add("FileAuditId");

                    if (EntityName == "RealizationReport")
                    {
                        //    data = data.Select("RelAmount <> '-' AND RelAmount <> '' AND FiNumber <> '-' AND FiNumber <> '' AND RealizationDate <> '-' AND RealizationDate <> ''").CopyToDataTable();
                        data.Columns.Add("_RealizationDate");
                    }
                    if (EntityName == "GoodsDeclaration")
                    {
                        AddColumns(data, GdImporter.GdColumns);
                    }
                    else if (EntityName == "FinancialInstrument")
                    {

                        AddColumns(data, FiImporter.FiColoums);
                        if (dataTable != null)
                        {
                            data.Columns.Add("CREATED_DATETIME");
                            data.Columns.Add("TRANSACTION_TYPE");
                        }
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
                        if (EntityName != "ITRS_Data" && EntityName != "CobGds" && EntityName != "NtnConversion" && EntityName != "RealizationReport")
                        {

                            _row["CreationTime"] = DateTimeNow;
                            _row["IsDeleted"] = false;
                            _row["CreatorUserId"] = null;

                        }
                        _row["TenantId"] = tenantId;
                        _row["FileAuditId"] = FileID;
                        //data.Columns.Add("_RealizationDate");

                        if (EntityName == "GoodsDeclaration")
                        {
                            if ((!_row["MESSAGE_ID"].ToString().IsNullOrEmpty()) && _row["DIRECTION"].ToString() == "REQUEST" && msgIds.Contains(_row["MESSAGE_ID"].ToString()) && _row["MESSAGE_TYPE"].ToString() != "101")
                            {
                                _row["STATUS_CODE"] = "200";// custom
                            }
                            else
                            {
                                _row["STATUS_CODE"] = "500";
                            }
                            List<string> fis = new List<string>();
                            if (_row["MESSAGE_TYPE"].ToString() == "307")
                            {
                                result = GdImporter.LoadCobGdInfoColoums(_row);
                                fis = result?.fiNumbers;
                                if (result?.cobFi != null)
                                {
                                    lstCobFis.Add(result.cobFi);
                                }
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
                        else if (EntityName == "FinancialInstrument")
                        {
                            FiImporter.LoadFIInfoColoums(_row);
                        }
                        else if (EntityName == "BcaData")
                        {
                            FiImporter.LoadBcaInfoColoums(_row);
                        }
                        else if (EntityName == "ITRS_Data")
                        {
                            ITRS_Importer.LoadITRSInfoColoums(_row);
                        }
                        else if (EntityName == "RealizationReport")
                        {
                            ITRS_Importer.LoadRelRptInfoColoums(_row);
                        }
                        else if (EntityName == "LetterOfCredit" || EntityName == "DocumentaryCollection")
                        {
                            LodgmentImporter.LoadLodgmentColoums(_row,EntityName);
                        }
                        else if (EntityName == "ITRS_Data" )
                        {
                            LodgmentImporter.LoadLodgmentColoums(_row, EntityName);
                        }
                        else if (EntityName == "v20SavedData")
                        {
                            ITRS_Importer.LoadV20InfoColoums(_row);
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
                            object[] rowArray = row.ItemArray;
                            cobData.Rows.Add(rowArray);
                        }
                        data.Merge(cobData);
                    }
                    DataTable cobFiTable = new DataTable();
                    if (!lstCobFis.IsNullOrEmpty())
                    {
                        cobFiTable.Columns.Add("PAYLOAD", typeof(string));
                        lstCobFis = lstCobFis
                            .Where(x => !string.IsNullOrEmpty(x.finInsUniqueNumber)) // Filter out null or empty finInsUniqueNumber
                            .GroupBy(x => x.finInsUniqueNumber)
                            .Select(group => group.First())
                            .ToList();

                        foreach (var fi in lstCobFis)
                        {
                            var jsonString = JsonConvert.SerializeObject(fi);
                            DataRow row = cobFiTable.NewRow();
                            row["PAYLOAD"] = jsonString;
                            cobFiTable.Rows.Add(row);
                        }
                        //   Executeion(cobFiTable, "FinancialInstrument", fileName);
                        // ImportData(null, "FinancialInstrument", FileID, fileName, cobFiTable);
                    }
                    if (EntityName == "GoodsDeclaration")
                    {
                        //data = data.Select("STATUS_CODE = '200'").CopyToDataTable();
                    }
                    BulkInsert(data, EntityName);
                    CustomRepo.RemoveDublicate(EntityName, FileID);
                    if (!lstCobFis.IsNullOrEmpty())
                    {
                        //Executeion(cobFiTable, "FinancialInstrument", fileName);

                    }


                    NewFiGdFilterModel filter = new NewFiGdFilterModel();

                    if (EntityName == "GoodsDeclaration")
                    {
                        filter = ExtractFiNumberFromNewGDs(data);
                    }
                    if (EntityName == "FinancialInstrument")
                    {
                        filter = ExtractFilisterList(data);
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
                string host = "sftprpa.jsbl.com";
                string username = "your_username";
                string password = "your_password";
                string directory = "/ExportOverdue";


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
            try
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
                            if (x != null || !x.FiNumber.IsNullOrEmpty())
                            {

                                fis.Add(x.FiNumber);
                            }
                            if (fi == "(305)")
                            {
                                if (row["gdNumber"] != null)
                                {
                                    var gdnum = row["gdNumber"].ToString();
                                    if (gdnum.IsNullOrEmpty())
                                    {

                                    }
                                    gds.Add(row["gdNumber"].ToString());
                                }
                            }
                            if (x == null || x.FiNumber.IsNullOrEmpty() && fi != "(305)")
                            {


                            }
                        }
                    }


                }
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
            catch
            {
                return new NewFiGdFilterModel();

            }
        }
        private NewFiGdFilterModel ExtractFilisterList(DataTable dataTable)
        {
            try
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
            catch
            {
                return new NewFiGdFilterModel();

            }
        }
        private List<string?> ExtractOkMessageId(DataTable dataTable)
        {
            List<string?> msgIds = new List<string?>();

            foreach (DataRow row in dataTable.Rows)
            {
                if (row["DIRECTION"].ToString() == "RESPONSE" && row["STATUS_CODE"].ToString() == "200" && row["MESSAGE_TYPE"].ToString() != "101")
                {
                    msgIds.Add(row["MESSAGE_ID"].ToString());
                }
            }
            return msgIds;
        }


        static void ModifyDataTable(DataTable table, List<string> renameInput)
        {

            foreach (var renamePair in renameInput)
            {
                if (!renamePair.Contains(','))
                {

                    table.Columns.Remove(renamePair);

                }
                else
                {
                    var pair = renamePair.Split(',').Select(name => name.Trim()).ToArray();
                    if (pair.Length == 2 && table.Columns.Contains(pair[0]))
                    {
                        table.Columns[pair[0]].ColumnName = pair[1];
                    }
                }


            }

        }

    }
    public class ForceStringConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return JToken.Load(reader).ToString();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value);
        }
    }

}
