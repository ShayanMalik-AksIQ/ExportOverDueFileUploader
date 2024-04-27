using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2010.Excel;
using ExportOverDueFileUploader;
using ExportOverDueFileUploader.DataImporter;
using ExportOverDueFileUploader.DBHelper;
using ExportOverDueFileUploader.DBmodels;
using ExportOverDueFileUploader.MatuirtyBO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Data;
using System.Globalization;
using System.Runtime.CompilerServices;

internal class Program
{
    private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(Program));


    private static void Main(string[] args)
    {
        Seriloger.LoggerInstance.Information($"============================================================={DateTime.Now}=============================================================");

        try
        {


            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            //AppSettings.ConnectionString = "Server=DESKTOP-O10K6M5; Database=ExportOverDue_JS_QA_DuplicateTest; Trusted_Connection=True; TrustServerCertificate=True;Command Timeout=16000;";
            AppSettings.ConnectionString = configuration.GetConnectionString("DefaultConnection");
            AppSettings.TenantId = int.Parse(configuration.GetConnectionString("TenantId"));
            AppSettings.BatchSize = int.Parse(configuration.GetConnectionString("BatchSize"));

            Uploader x = new Uploader();
            x.Executeion();
           // Console.ReadKey();
        }
        catch (Exception ex)
        {
            Seriloger.LoggerInstance.Error("Error In Main", ex.Message);
        }
    }

    
}