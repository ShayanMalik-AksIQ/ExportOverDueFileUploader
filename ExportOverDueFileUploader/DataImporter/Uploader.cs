using ExportOverDueFileUploader.DBmodels;
using ExportOverDueFileUploader.MatuirtyBO;
using ExportOverDueFileUploader.ValidateIqBizLogic.Comparison_V2;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace ExportOverDueFileUploader.DataImporter
{
    public class Uploader
    {

        List<string> TableNames = new List<string>();
        public DateTime DateTimeNow = new DateTime();
        private readonly ILogger _logger;
        public Uploader()
        {
            TableNames.Add("FinancialInstrumentImport");
            TableNames.Add("GoodsDeclarationImport");
            TableNames.Add("BranchWiseSegments");
            TableNames.Add("FinancialInstrument");
            TableNames.Add("GoodsDeclaration");

        }

        public void Test()
        {
            ExportOverDueContext context = new();
            var settings = context.ComparatorSettings
                .ToList();

            var figdLink = context.FinancialInstrumentImports
                .Include(figd => figd.GdFiLinks)
                    .ThenInclude(fi => fi!.Gd)
                .Where(fi => fi.GdFiLinks.Count(link => link.Gd != null) > 1)
                .Skip(1)
                .Take(1)
                .ToList();


            //var figdLink = context.GoodsDeclaration
            //    .Include(figd => figd.GD_FI_Links)
            //        .ThenInclude(fi => fi!.Fi)
            //    .Where(fi => fi.GD_FI_Links.Count(link => link.Gd != null) > 1)
            //    //.Skip(1)
            //    //.Take(1)
            //    .ToList();


            //var figdLink = context.FinancialInstrumentImports
            //    .Include(figd => figd.GdFiLinks)
            //        .ThenInclude(fi => fi!.Gd)
            //    .Where(fi => fi.GdFiLinks.All(gd => gd.Gd!.Payload != null))
            //    .AsEnumerable() // Materialize data
            //    .Where(fi => fi.GdFiLinks
            //        .Count(link => link.Gd != null && System.Text.Json.JsonDocument.Parse(link.Gd.Payload!)
            //            .RootElement.GetProperty("data")
            //            .GetProperty("itemInformation")
            //            .EnumerateArray()
            //            .Count() > 4) > 1)
            //    .Skip(2)
            //    .Take(1)
            //    .ToList();

            //List<ComparisonInput> fiGds = figdLink.Select(fi => new ComparisonInput
            //{
            //    Payload = fi.PAYLOAD ?? string.Empty,
            //    Id = fi.Id,
            //    Type = DocumentType.GD,
            //    RelatedRecords = fi.GD_FI_Links.Select(figd => new RelatedRecord{ Payload =  figd.Fi!.PAYLOAD ?? string.Empty, RelationId = figd.Id}).ToList() ?? []
            //}).ToList();

            List<ComparisonInput> fiGds = figdLink.Select(fi => new ComparisonInput
            {
                Payload = fi.Payload ?? string.Empty,
                Id = fi.Id,
                Type = DocumentType.FI,
                RelatedRecords = fi.GdFiLinks.Select(figd => new RelatedRecord { Payload = figd.Gd!.Payload ?? string.Empty, RelationId = figd.Id }).ToList() ?? []
            }).ToList();

            Stopwatch sw = new();
            sw.Start();
            var result = Comparison_V2.CompareGdAndFi(fiGds, settings.Where(cs => cs.ModuleId == (int)ComparisonType.Import).ToList(), 3, ComparisonType.Import);
            sw.Stop();

            Console.WriteLine($"----------------{sw.ElapsedMilliseconds} ms -------------------\n\n");

            //var aggImport = result.OfType<AggregiatedResultImport>().ToList();
            //var compImport = result.OfType<ComparisonResultImport>().ToList();

            //var aggImport = result.Where(x => x is AggregiatedResultImport)
            //          .Select(x => (AggregiatedResultImport)x)
            //          .ToList();

            //var compImport = result.Where(x => x is ComparisonResultImport)
            //                       .Select(x => (ComparisonResultImport)x)
            //                       .ToList();

            var aggImport = result.Where(x => x is AggregiatedResultExport)
                      .Select(x => (AggregiatedResultExport)x)
                      .ToList();

            var compImport = result.Where(x => x is ComparisonResultExport)
                                   .Select(x => (ComparisonResultExport)x)
                                   .ToList();

            context.AggregiatedResultExports.AddRange(aggImport);
            context.ComparisonResultExports.AddRange(compImport);

            //context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT AggregiatedResultImports ON");
            //context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT ComparisonResultImports ON");
            // Perform insertion
            context.SaveChanges();
            //context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT AggregiatedResultImports OFF");
            //context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT ComparisonResultImports OFF");


            //foreach (KeyValuePair<string, List<ComparisonResult>> kvPair in result)
            //{
            //    Console.WriteLine($"---------------{kvPair.Key}-----------------\n\n");
            //    foreach (var item in kvPair.Value)
            //    {
            //        Console.WriteLine(item.ToString());
            //    }
            //}

        }

        public void Execution()
        {
            try
            {
                Seriloger.LoggerInstance.Information("Export Over Due Uploader Execution Begins ......");
                ExportOverDueContext context = new ExportOverDueContext();
                AppSettings.NotMatchReqStats = 11;
                AppSettings.MatchReqStats = 15;
                AppSettings.GdFiLinkReqStats = 12;

                var settings = context.DefaultSettings.ToList();

                var maxLoadingOrder = context.RequestStatuses
                    .Where(x => x.Module!.ModuleName!.ToLower() == "Setups".ToLower())
                    .Select(x => x.LoadingOrder)
                    .Max();

                var lstFileTypes = context.FileTypes
                    .Where(x => x.TenantId == AppSettings.TenantId && x.IsDeleted == false && x.RequestStatus!.LoadingOrder == maxLoadingOrder)
                    .ToList();//aproved only

                if (lstFileTypes.IsNullOrEmpty())
                {
                    Seriloger.LoggerInstance.Error("Error:No File Type In DB..");
                    return;
                }

                //  FileReader.DownloadFromFtp(lstFileTypes);

                foreach (var fileType in lstFileTypes)
                {
                    Seriloger.LoggerInstance.Information($"File:{fileType.Name} Folder:{fileType.FilePath} In Progress....");
                    var BasePath = fileType.FilePath ?? "";
                    var Files = GetFileNamesInFolder(BasePath);
                    if (Files is null)
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
                            Seriloger.LoggerInstance.Information($"FileType:{fileType.Name} File:{file} Reading In Progress....");
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
                                Seriloger.LoggerInstance.Error("File Type Incorrect");
                                continue;
                            }
                            try
                            {

                                auditTrail.FileName = file;
                                auditTrail.FileTypeId = fileType.Id;//
                                if (FileJsonData != null && !FileJsonData.StartsWith("Error"))
                                {
                                    Seriloger.LoggerInstance.Information($"{fileType.Name} file:{file} Reading Success {Files.IndexOf(file) + 1}/{Files.Count}");
                                    DateTimeNow = DateTime.Now;
                                    auditTrail.TenantId = AppSettings.TenantId;
                                    auditTrail.CreationTime = DateTimeNow;
                                    context.FileImportAuditTrails.Add(auditTrail);
                                    context.SaveChanges();
                                    var filters = ImportData(FileJsonData, fileType.Description ?? "", auditTrail.Id, file, fileType.ColumnRename ?? "");
                                    Seriloger.LoggerInstance.Information($"{fileType.Name} file:{file} Db Export Success {Files.IndexOf(file) + 1}/{Files.Count}");
                                    auditTrail.Remarks = "Success";
                                    auditTrail.Success = true;
                                    if (fileType.Description == "GoodsDeclarationImport")
                                    {
                                        LinkGdToFI.SyncNewImportGd(auditTrail.Id, filters ?? new());
                                    }
                                    else if (fileType.Description == "FinancialInstrumentImport")
                                    {
                                        LinkGdToFI.SyncImportNewFi(auditTrail.Id, filters ?? new());
                                    }
                                    else if (fileType.Description == "FinancialInstrument")
                                    {
                                        LinkGdToFI.SyncNewFi(auditTrail.Id, filters ?? new());
                                    }
                                    else if (fileType.Description == "GoodsDeclaration")
                                    {
                                        LinkGdToFI.SyncNewGd(auditTrail.Id, filters ?? new());
                                    }
                                    Seriloger.LoggerInstance.Information($"{fileType.Name} file:{file} Sync Success {Files.IndexOf(file) + 1}/{Files.Count}");
                                }
                                else
                                {
                                    auditTrail.Success = false;
                                    auditTrail.Remarks = $"{FileJsonData}";
                                    Seriloger.LoggerInstance.Information($"{fileType.Name} file:{file} Handers Miss Matched {Files.IndexOf(file) + 1}/{Files.Count}");
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
                                    Seriloger.LoggerInstance.Information($"{fileType.Name} file:{file} {Files.IndexOf(file) + 1}/{Files.Count}  Uploaded Success With Audit");
                                    Seriloger.LoggerInstance.Information($"-------------------------{file} Success---------------------------");
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
                    //FileReader.MoveFiles(fileType.FilePath, "*.xlsx", "*.csv");


                }

            }
            catch (Exception ex)
            {
                Seriloger.LoggerInstance.Error(ex.Message);
            }

        }
        public void Execution(DataTable dataTable, string EntityName, string fileName)
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
                    Seriloger.LoggerInstance.Information($"{EntityName} file:{fileName} Db Export Success");
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
                    Seriloger.LoggerInstance.Information($"{EntityName} file:{fileName} Uploaded Success With Audit");
                }
                catch (Exception ex)
                {
                    Seriloger.LoggerInstance.Error($"Error :{ex.Message} Bca file:{fileName}  Uploaded Failed ");

                }
            }

        }
        public NewFiGdFilterModel? ImportData(string jsonData, string EntityName, long FileID, string fileName, string columnRename, DataTable dataTable = null)
        {
            NewFiGdFilterModel filter = new NewFiGdFilterModel();
            if (TableNames.Contains(EntityName))
            {
                try
                {
                    List<string> newGdFis = [];
                    DataTable data = dataTable;
                    if (dataTable == null)
                    {
                        JsonSerializerSettings settings = new()
                        {
                            Converters = [new ForceStringConverter()],
                            DateParseHandling = DateParseHandling.None
                        };

                        data = JsonConvert.DeserializeObject<DataTable>(jsonData, settings) ?? new DataTable();
                    }
                    else
                    {
                        data = dataTable;
                    }

                    DataTable BcaData = new();

                    data.Columns.Add("CreationTime");
                    data.Columns.Add("IsDeleted");
                    data.Columns.Add("CreatorUserId");

                    if (!columnRename.IsNullOrEmpty())
                    {
                        ModifyDataTable(data, columnRename?.Split("||").ToList() ?? []);
                    }

                    data.Columns.Add("TenantId");
                    data.Columns.Add("FileAuditId");

                    if (EntityName == "GoodsDeclarationImport")
                    {
                        data = data.Select("ProcessCode = '101'").CopyToDataTable();
                        AddColumns(data, GdImporter.GdImportColumns);
                    }
                    else if (EntityName == "FinancialInstrumentImport")
                    {
                        data = data.Select("MethodId = '1520'").CopyToDataTable();
                        AddColumns(data, FiImporter.FiImportColoums);
                    }
                    else if (EntityName == "FinancialInstrument")
                    {
                        // data = data.Select("TRANSACTION_TYPE = '1524'").CopyToDataTable();
                        AddColumns(data, FiImporter.FiColoums);
                    }
                    else if (EntityName == "GoodsDeclaration")
                    {
                        //data.Columns.Add("TRANSMISSION_DATETIME");

                        // data = data.Select("MESSAGE_TYPE = '102'").CopyToDataTable();
                        //var responseRows = data.Select("DIRECTION = 'RESPONSE' AND STATUS_CODE = '200'");
                        //var responseMessageIds = responseRows.Select(row => row["MESSAGE_ID"].ToString()).ToHashSet();

                        //data = data.AsEnumerable()
                        //    .Where(row => row["DIRECTION"].ToString() == "REQUEST" &&
                        //                responseMessageIds.Contains(row["MESSAGE_ID"].ToString()))
                        //    .ToList()
                        //    .CopyToDataTable();

                        AddColumns(data, GdImporter.GdColumns);
                    }
                    int tenantId = AppSettings.TenantId;

                    foreach (DataRow _row in data.Rows)
                    {
                        _row["CreationTime"] = DateTimeNow;
                        _row["IsDeleted"] = false;
                        _row["CreatorUserId"] = null;
                        _row["TenantId"] = tenantId;
                        _row["FileAuditId"] = FileID;

                        if (EntityName == "GoodsDeclarationImport")
                        {
                            GdImporter.LoadImportGdInfoColoums(_row);

                        }
                        else if (EntityName == "FinancialInstrumentImport")
                        {
                            FiImporter.LoadImportFIInfoColoums(_row);
                        }
                        else if (EntityName == "FinancialInstrument")
                        //else if (EntityName == "FinancialInstrumentExport")
                        {
                            FiImporter.LoadFIInfoColoums(_row);
                        }
                        else if (EntityName == "GoodsDeclaration")
                        {
                            GdImporter.LoadGdInfoColoums(_row);
                        }
                    }
                    if (EntityName == "GoodsDeclarationImport")
                    {
                        data = data.Select("gdNumber <> ''").CopyToDataTable();

                    }
                    else if (EntityName == "FinancialInstrumentImport")
                    {
                        data = data.Select("FinInsUniqueNumber <> ''").CopyToDataTable();
                    }
                    if (EntityName == "GoodsDeclaration")
                    {
                        data = data.Select("gdNumber <> ''").CopyToDataTable();

                    }
                    else if (EntityName == "FinancialInstrument")
                    {
                        data = data.Select("finInsUniqueNumber <> ''").CopyToDataTable();
                    }

                    BulkInsert(data, EntityName);




                    if (EntityName == "GoodsDeclarationImport")
                    {
                        filter = ExtractFiNumberFromNewGDs(data);
                    }
                    else if (EntityName == "FinancialInstrumentImport")
                    {
                        // CustomRepo.UpdateBranchSegment(EntityName);
                        filter = ExtractFilisterList(data);
                    }
                    else if (EntityName == "FinancialInstrument")
                    {
                        //CustomRepo.UpdateBranchSegment(EntityName);
                        filter = ExtractFilisterList(data);
                    }
                    else if (EntityName == "GoodsDeclaration")
                    {
                        filter = ExtractFiNumberFromNewGDsExport(data);
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
        public void BulkInsert(DataTable dt, string entityName)
        {
            try
            {// get fro df
                var connectionString = AppSettings.ConnectionString;

                int batchSize = AppSettings.BatchSize; // Set your desired batch size df
                int totalRows = dt.Rows.Count;

                using var sqlbulk = new SqlBulkCopy(connectionString);
                sqlbulk.DestinationTableName = entityName;
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
            catch (Exception ex)
            {
                throw ex;
            }

        }
        private void AddColumns(DataTable xlsxFi, List<string> columnNames)//gd or fi list 
        {
            foreach (string columnName in columnNames)
            {
                //xlsxFi.Columns.Add(columnName);
                if (!xlsxFi.Columns.Contains(columnName)) // Check if column exists
                {
                    xlsxFi.Columns.Add(columnName);
                }
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
            try
            {
                List<string> fis = new List<string>();
                List<string> gds = new List<string>();

                foreach (DataRow row in dataTable.Rows)
                {
                    if (row["ModeOfPayment"].ToString() == "302")
                    {
                        gds.Add(row["gdNumber"].ToString());
                    }
                    IEnumerable<string> lstFinInsUniqueNumbers = [];
                    string? finInsUniqueNumbers = row["FinInsUniqueNumber"].ToString();
                    string? strlstFinInsUniqueNumbers = row.Table.Columns.Contains("LstfinInsUniqueNumbers")
                        ? row["LstfinInsUniqueNumbers"]?.ToString()
                        : null;
                    if (!string.IsNullOrEmpty(strlstFinInsUniqueNumbers))
                    {
                        lstFinInsUniqueNumbers = strlstFinInsUniqueNumbers.Split(", ");
                    }

                    if (!string.IsNullOrEmpty(finInsUniqueNumbers))
                    {
                        fis.Add(finInsUniqueNumbers);
                    }
                    else if (lstFinInsUniqueNumbers.Any())
                    {
                        fis.AddRange(lstFinInsUniqueNumbers);
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

        private NewFiGdFilterModel ExtractFiNumberFromNewGDsExport(DataTable dataTable)
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
                List<string> gdNumberList = [];
                List<string> fis = [];

                foreach (DataRow row in dataTable.Rows)
                {

                    string? fi = row["FinInsUniqueNumber"].ToString();
                    string? gd = row["OpenAccountGdNumber"].ToString();


                    if (!string.IsNullOrEmpty(fi))
                    {
                        fis.Add(fi);
                    }
                    if (!string.IsNullOrEmpty(gd))
                    {
                        gdNumberList.Add(gd);
                    }
                }

                return new NewFiGdFilterModel
                {
                    fis = fis.Where(x => x != null && x != "")
                             .Distinct()
                             .ToList(),
                    gds = gdNumberList
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
