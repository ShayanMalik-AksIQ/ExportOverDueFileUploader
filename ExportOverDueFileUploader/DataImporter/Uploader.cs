﻿using ExportOverDueFileUploader.DBmodels;
using ExportOverDueFileUploader.MatuirtyBO;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;

namespace ExportOverDueFileUploader.DataImporter
{
    public class Uploader
    {
        List<string> TableNames = new List<string>();
        public DateTime DateTimeNow= new DateTime();
        public Uploader()
        {

            TableNames.Add("FileType");
            TableNames.Add("GoodsDeclaration");
            TableNames.Add("FinancialInstrument");
            TableNames.Add("LetterOfCredit");
            TableNames.Add("DocumentaryCollection");

        }
        public void Executeion()
        {
            ExportOverDueContext context = new ExportOverDueContext();
            var lstFileTypes = context.FileTypes.Where(x => x.TenantId == AppSettings.TenantId && x.IsDeleted==false);//aproved only
            foreach (var fileType in lstFileTypes)
            {
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
                            if (FileJsonData != null && !FileJsonData.StartsWith("Hadders"))
                            {
                                DateTimeNow= DateTime.Now;
                                var gdList = ImportData(FileJsonData, fileType.Description);
                                auditTrail.Remarks = "Success";
                                auditTrail.Success = true;
                                if (fileType.Description == "GoodsDeclaration")
                                {

                                    LinkGdToFI.SyncNewGd(DateTimeNow,gdList.fis);
                                }
                                if (fileType.Description == "FinancialInstrument")
                                {

                                 //   LinkGdToFI.SyncNewFi(DateTimeNow);
                                }

                            }
                            else
                            {
                                auditTrail.Success = false;
                                auditTrail.Remarks = $"Hadders MissMached";

                            }

                        }
                        catch (Exception ex)
                        {
                            auditTrail.Success = false;
                            auditTrail.Remarks = $"{ex.Message}";

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
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
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
        public NewFiGdFilterModel ImportData(string jsondata, string EntityName)
        {
            if (TableNames.Contains(EntityName))
            {
                try
                {
                    List<string> newGdFis = new List<string>();
                    DataTable data = JsonConvert.DeserializeObject<DataTable>(jsondata.ToString());
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
                    }
                    if (data.Columns.Contains("ID"))
                    {
                        data.Columns.Remove("ID");
                    }
                    BulkInsert(data, EntityName);
                    NewFiGdFilterModel filter=new NewFiGdFilterModel();
                    if (EntityName == "GoodsDeclaration")
                    {

                       // var gdNumbers = ExtractGdNumberList(data);
                        filter .fis= newGdFis;
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

                    throw ex;
                }
            }
            else
            {
                throw new Exception("Incorrect table name");
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

        private NewFiGdFilterModel ExtractGdNumberList(DataTable dataTable)
        {
            List<string> gdNumberList = new List<string>();

            foreach (DataRow row in dataTable.Rows)
            {
                // Assuming "id" is the name of the column
                string id = row["gdNumber"].ToString();

                gdNumberList.Add(id);
            }

            return new NewFiGdFilterModel
            {
                gds = gdNumberList
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
