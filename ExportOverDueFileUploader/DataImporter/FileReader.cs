using ClosedXML.Excel;
using CsvHelper;
using CsvHelper.Configuration;
using ExportOverDueFileUploader.DBmodels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ExportOverDueFileUploader.DataImporter
{
    public static class FileReader
    {
        public static string ReadAndValidateCsvFile(string filePath, int HeaderStart, string HeadersToValidate)
        {
            try
            {
                string csvFilePath = filePath;

                // Create a configuration for CsvHelper
                var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture);

                // Read CSV headers
                using (var reader = new StreamReader(csvFilePath))
                using (var csv = new CsvReader(reader, csvConfig))
                {
                    // Read the headers without advancing the reader
                    csv.Read();
                    csv.ReadHeader();

                    // Get the headers as an array of strings
                    var headers = csv.HeaderRecord.ToList();
                    var HeaderToValidate = HeadersToValidate.Split(",").ToList();
                    // Check if the headers match the expected headers
                    if (HeaderToValidate.All(item => headers.Contains(item))&& headers.All(item => HeaderToValidate.Contains(item)))
                    {
                        // Headers match, proceed to read CSV data into a list of dictionaries
                        List<Dictionary<string, object>> csvData = csv.GetRecords<dynamic>()
                            .Select(record => ((IDictionary<string, object>)record).ToDictionary(kvp => kvp.Key, kvp => kvp.Value))
                            .ToList();

                        var settings = new JsonSerializerSettings
                        {
                            Formatting = Newtonsoft.Json.Formatting.Indented,
                            Converters = { new EmptyStringToNullConverter() }
                        };

                        // Return the JSON representation of the CSV data
                        return JsonConvert.SerializeObject(csvData, settings);
                    }
                    else
                    {
                        Seriloger.LoggerInstance.Error("Hadders MissMached");
                        // Headers do not match, return an error message
                        return "Error";
                    }
                }


            }
            catch (Exception ex)
            {
                Seriloger.LoggerInstance.Error(ex.Message);
                return "Error";
            }
                    }

        public static string ReadAndValidateExcelFile(string filePath, int HadderStart, string HaddersToValidator)
        {
            try
            {


                using (var workbook = new XLWorkbook(filePath))
                {
                    var worksheet = workbook.Worksheet(1);

                    List<Dictionary<string, object>> excelData = ExcelToDictionaryList(worksheet, HadderStart, HaddersToValidator);

                    if (excelData == null)
                    {
                        return "Error: Excel To Dictionary";
                    }

                    var settings = new JsonSerializerSettings
                    {
                        Formatting = Newtonsoft.Json.Formatting.Indented,
                        Converters = { new EmptyStringToNullConverter() }
                    };

                    return JsonConvert.SerializeObject(excelData, settings);
                }

            }
            catch(Exception ex)
            {
                Seriloger.LoggerInstance.Error(ex.Message);
                return $"Error :{ex.Message}";
            }
        
        }


        private static List<string> GetFileNamesInFolder(string folderPath)
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

        static List<Dictionary<string, object>> ExcelToDictionaryList(IXLWorksheet worksheet, int HadderStart, string HaddersToValidator)
        {
            try
            {
                // Get the headers as an array of strings
                var headers = worksheet.Row(HadderStart).Cells().Select(cell => cell.Value.ToString());

                var x= string.Join(",",worksheet.Row(HadderStart).Cells().Select(cell => cell.Value.ToString()).ToList());
                var HeaderToValidate = HaddersToValidator.Split(",").ToList();
                List<string> headerRow = new List<string>();
                // Check if the headers match the expected headers
                if (HeaderToValidate.All(item => headers.Contains(item)) && headers.All(item => HeaderToValidate.Contains(item)))
                {
                    headerRow = worksheet.Row(HadderStart).Cells().Select(cell => Regex.Replace(cell.Value.ToString(), "[^a-zA-Z0-9_]", "")).ToList();
                }
                else
                {
                    Console.WriteLine("Hadders MissMached");
                    return null;
                }
                var excelData = new List<Dictionary<string, object>>();

                int lastColumn = worksheet.LastColumnUsed() != null ? worksheet.LastColumnUsed().ColumnNumber() : 0;
                int lastRow = worksheet.LastRowUsed() != null ? worksheet.LastRowUsed().RowNumber() : 0;

                for (int row = HadderStart + 1; row <= lastRow; row++)
                {
                    var rowData = new Dictionary<string, object>();

                    for (int col = 1; col <= lastColumn; col++)
                    {
                        var headerIndex = col - 1;

                        if (headerIndex < headerRow.Count)
                        {
                            var cellValue = worksheet.Cell(row, col).Value;
                            if(cellValue.ToString()!=null && cellValue.ToString() != "")
                            {
                                 rowData[headerRow[headerIndex]] =  cellValue.ToString();
                            }
                            else
                            {
                                rowData[headerRow[headerIndex]] = null;
                            }
                            
                        }
                    }

                    excelData.Add(rowData);
                }

                return excelData;
            }
            catch(Exception ex)
            {
                Seriloger.LoggerInstance.Error(ex.Message);
                throw;
            }
        }

        public class EmptyStringToNullConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(string);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                if (reader.TokenType == JsonToken.String)
                {
                    string value = reader.Value as string;
                    return string.IsNullOrEmpty(value) ? null : value;
                }

                return reader.Value;
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                writer.WriteValue(value);
            }
        }
    }
}
