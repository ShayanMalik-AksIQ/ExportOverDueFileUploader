using DocumentFormat.OpenXml.Office2010.Excel;
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

namespace ExportOverDueFileUploader.DataImporter
{
    public class Uploader
    {
        List<string> TableNames = new List<string>();
        public DateTime DateTimeNow= new DateTime();
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
            Console.WriteLine("Export Over Due Uploader Execution Begins......");
            ExportOverDueContext context = new ExportOverDueContext();
            var lstFileTypes = context.FileTypes.Where(x => x.TenantId == AppSettings.TenantId && x.IsDeleted==false);//aproved only
            if (lstFileTypes.IsNullOrEmpty())
            {
                Console.WriteLine("Error:No File Type In DB..");
                return;
            }
            foreach (var fileType in lstFileTypes)
            {
                Console.WriteLine($"File:{fileType.Name} Folder:{fileType.FilePath} Uploader Begins.....");
                var BasePath = fileType.FilePath;
                var Files = GetFileNamesInFolder(BasePath);

                foreach (var file in Files)
                {
                    FileImportAuditTrail auditTrail = new FileImportAuditTrail();
                    try
                    {
                        string FileJsonData = string.Empty;
                        if (file.EndsWith(".xlsx"))
                        {
                            FileJsonData = FileReader.ReadAndValidateExcelFile(Path.Combine(BasePath, file), fileType.HeaderRow == 0 ? 1 : fileType.HeaderRow, fileType.ColumnNames);
                        }
                        else if (file.EndsWith(".csv"))
                        {
                            FileJsonData = FileReader.ReadAndValidateCsvFile(Path.Combine(BasePath, file), fileType.HeaderRow == 0 ? 1 : fileType.HeaderRow, fileType.ColumnNames);
                        }
                        else
                        {
                            Console.WriteLine("File Type Incorect");
                            continue;
                        }
                        try
                        {

                            auditTrail.FileName = file;
                            auditTrail.FileTypeId = fileType.Id;//
                            if (FileJsonData != null && !FileJsonData.StartsWith("Error"))
                            {
                                Console.WriteLine($"{fileType.Name} file:{file} Reading Sucess {Files.IndexOf(file) + 1}/{Files.Count}");
                                DateTimeNow = DateTime.Now;
                                var filters = ImportData(FileJsonData, fileType.Description);
                                Console.WriteLine($"{fileType.Name} file:{file} Db Export Sucess {Files.IndexOf(file) + 1}/{Files.Count}");
                                auditTrail.Remarks = "Success";
                                auditTrail.Success = true;
                                if (fileType.Description == "GoodsDeclaration")
                                {

                                    LinkGdToFI.SyncNewGd(DateTimeNow, filters.fis);
                                }
                                if (fileType.Description == "FinancialInstrument")
                                {

                                 //   LinkGdToFI.SyncNewFi(DateTimeNow);
                                }
                                Console.WriteLine($"{fileType.Name} file:{file} Sync Sucess {Files.IndexOf(file) + 1}/{Files.Count}");

                            }
                            else
                            {
                                auditTrail.Success = false;
                                auditTrail.Remarks = $"{FileJsonData}";
                                Console.WriteLine($"{fileType.Name} file:{file} Hadders MissMached {Files.IndexOf(file) + 1}/{Files.Count}");

                            }

                        }
                        catch (Exception ex)
                        {
                            auditTrail.Success = false;
                            auditTrail.Remarks = $"{ex.Message}";
                            Console.WriteLine($"Error :{ex.Message} on  {fileType.Name} file:{file} - {Files.IndexOf(file) + 1}/{Files.Count}");
                        }
                        finally
                        {

                            try
                            {
                                auditTrail.TenantId = AppSettings.TenantId;
                                auditTrail.CreationTime = DateTimeNow;
                                ExportOverDueContext context1 = new ExportOverDueContext();
                                context1.FileImportAuditTrails.Add(auditTrail);
                                context1.SaveChanges();
                                Console.WriteLine($"{fileType.Name} file:{file} {Files.IndexOf(file) + 1}/{Files.Count}  Uploaded Sucess With Audit");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error :{ex.Message} {fileType.Name} file:{file} {Files.IndexOf(file) + 1}/{Files.Count}  Uploaded Failed ");
                               
                            }
                        }

                    }

                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }

            }
        }
        public NewFiGdFilterModel ImportData(string jsondata, string EntityName, DataTable dataTable=null)
        {
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

                    if (EntityName == "FinancialInstrument")
                    {
                        DataTable NonBcaData = data.Select("TRANSACTION_TYPE <> 1526").CopyToDataTable();

                        // Assuming data is a DataTable
                        var rowsToRemove = data.Select("TRANSACTION_TYPE <> 1526");
                        foreach (var row in rowsToRemove)
                        {
                            data.Rows.Remove(row);
                        }

                        DataTable BcaData = data.Copy();
                        data = NonBcaData;
                        if (BcaData!=null && BcaData.Rows.Count>0)
                        {
                            ImportData(null, "BcaData", BcaData);
                        }
                        if (data.Rows.Count == 0)
                        {
                            return null;
                        }

                    }
                    data.Columns.Add("CreationTime");
                    data.Columns.Add("IsDeleted");
                    data.Columns.Add("TenantId");
                    data.Columns.Add("CreatorUserId");
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
                    foreach (DataRow _row in data.Rows)
                    {
                        if (data.Columns.Contains("ID"))
                        {
                            _row["I_D"] = _row["ID"];
                        }
                        _row["CreationTime"] = DateTimeNow;
                        _row["IsDeleted"] = false;
                        _row["TenantId"] = tenantId;
                        _row["CreatorUserId"] = null;
                      
                        if (EntityName == "GoodsDeclaration")
                        {
                            var fis = GdImporter.LoadGdInfoColoums(_row);
                            if (fis!= null)
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
                    }
                    if (data.Columns.Contains("ID"))
                    {
                        data.Columns.Remove("ID");
                    }
                    BulkInsert(data, EntityName);
                    NewFiGdFilterModel filter=new NewFiGdFilterModel();
                    if (EntityName == "GoodsDeclaration")
                    {
                        filter.fis = ExtractFiNumberFromNewGDs(data).fis;  
                    }
                    if (EntityName == "FinancialInstrument")
                    {
                        var gdNumbers = ExtractFilisterList(data);
                        filter = gdNumbers;
                    } 
                    return filter;
                }
                catch (Exception ex)
                {

                    Console.WriteLine($"Error In Import Data:{ex.Message}");
                    return null; 
                }
            }
            else
            {
                Console.WriteLine($"Incorrect Table Name ");
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
                    Console.WriteLine($"Folder not found: {folderPath}");
                    return null; // Return an empty array if the folder doesn't exist
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return null; // Return an empty array in case of an exception
            }
        }

        private NewFiGdFilterModel ExtractFiNumberFromNewGDs(DataTable dataTable)
        {
            List<string> fiNumberList = new List<string>();
           
            foreach (DataRow row in dataTable.Rows)
            {
                
                string LstfinInsUniqueNumbers = row["LstfinInsUniqueNumbers"].ToString();
                List<FiNumberAndMode> fiNumberAndModes = new List<FiNumberAndMode>();
                if (LstfinInsUniqueNumbers != null && LstfinInsUniqueNumbers !="" && !LstfinInsUniqueNumbers.StartsWith("("))
                {
                    foreach (var fi in LstfinInsUniqueNumbers.Split(","))
                    {
                        fiNumberList.Add(Regex.Match(fi, @"^(?<FiNumber>[\w-]+)(\((?<Value>\d+)\))?$").Groups["FiNumber"].Value);
                    }
                }
               
            }

            return new NewFiGdFilterModel
            {
                fis = fiNumberList
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

                gdNumberList.Add(gd);
                fis.Add(fi);
            }

            return new NewFiGdFilterModel
            {
                gds = gdNumberList,
                fis = fis

            };
        }

        
                                           
    }

   
}
