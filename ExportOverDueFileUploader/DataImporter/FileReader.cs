﻿using ClosedXML.Excel;
using ExportOverDueFileUploader.DBmodels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ExportOverDueFileUploader.DataImporter
{
    public static class FileReader
    {

        
        public static string ReadAndValidateExcelFile(string filePath, int HadderStart, string HaddersToValidator)
        {
            using (var workbook = new XLWorkbook(filePath))
            {
                var worksheet = workbook.Worksheet(1);

                List<Dictionary<string, object>> excelData = ExcelToDictionaryList(worksheet, HadderStart, HaddersToValidator);

                var settings = new JsonSerializerSettings
                {
                    Formatting = Newtonsoft.Json.Formatting.Indented,
                    Converters = { new EmptyStringToNullConverter() }
                };

                return JsonConvert.SerializeObject(excelData, settings);

               
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
                        .Select(Path.GetFileName).Where(x=>x.EndsWith(".csv") || x.EndsWith(".xlsx"))
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

        static List<Dictionary<string, object>> ExcelToDictionaryList(IXLWorksheet worksheet,int HadderStart,string HaddersToValidator)
        {
            List<string> headerRow=new List<string>();
            if (String.Join(",", worksheet.Row(HadderStart).Cells().Select(cell => cell.Value.ToString())) == HaddersToValidator) {

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
         
            for (int row = HadderStart+1; row <= lastRow; row++)
            {
                var rowData = new Dictionary<string, object>();

                for (int col = 1; col <= lastColumn; col++)
                {
                    var headerIndex = col - 1;

                    if (headerIndex < headerRow.Count)
                    {
                        var cellValue = worksheet.Cell(row, col).Value;
                        rowData[headerRow[headerIndex]] = cellValue.GetText != null ? cellValue.ToString() : null;
                    }
                }

                excelData.Add(rowData);
            }

            return excelData;
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
