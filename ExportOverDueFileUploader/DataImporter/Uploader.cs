using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExportOverDueFileUploader.DataImporter
{
    public class Uploader
    {

        public string ImportData(string jsondata, string EntityName)
        {
            if (true)//TableNames.Contains(EntityName))
            {
                try
                {
                    DataTable data = JsonConvert.DeserializeObject<DataTable>(jsondata.ToString());
                    data.Columns.Add("CreationTime");
                    data.Columns.Add("IsDeleted");
                    data.Columns.Add("TenantId");
                    data.Columns.Add("CreatorUserId");
                    if (EntityName == "GoodsDeclaration")
                    {
                        AddColumns(data,GdImporter.GdColumns);
                    }
                    else if (EntityName == "FinancialInstrument")
                    {
                     //   AddFIInfoColoums(data);
                    }

                    if (data.Columns.Contains("ID"))
                    {
                        data.Columns.Add("I_D");
                    }
                    int tenantId = 1;//config
                    foreach (DataRow _row in data.Rows)
                    {
                        if (data.Columns.Contains("ID"))
                        {
                            _row["I_D"] = _row["ID"];
                        }
                        _row["CreationTime"] = DateTime.Now;
                        _row["IsDeleted"] = false;
                        _row["TenantId"] = tenantId;// appsettings
                        _row["CreatorUserId"] = null;

                        if (EntityName == "GoodsDeclaration")
                        {
                            GdImporter.LoadGdInfoColoums(_row);
                        }
                        else if (EntityName == "FinancialInstrument")
                        {
                            //LoadFIInfoColoums(_row);
                        }
                    }
                    if (data.Columns.Contains("ID"))
                    {
                        data.Columns.Remove("ID");
                    }
                    BulkInsert(data, EntityName);
                    return "data inserted successfully";
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
                var connectionString = "Server=DESKTOP-O10K6M5; Database=ExportOverDue_dbRefactor; Trusted_Connection=True; TrustServerCertificate=True;Command Timeout=160;";

                int batchSize = 1000; // Set your desired batch size df
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

    }
}
