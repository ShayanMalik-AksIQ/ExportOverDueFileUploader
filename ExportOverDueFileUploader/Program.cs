using ExportOverDueFileUploader;
using ExportOverDueFileUploader.DataImporter;
using ExportOverDueFileUploader.DBmodels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

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
            //AppSettings.ConnectionString = configuration.GetConnectionString("DefaultConnection");
            AppSettings.ConnectionString = "Server=DESKTOP-O10K6M5\\SQL15; Database=ValidateIQ_Dev; Trusted_Connection=True; TrustServerCertificate=True;";
            AppSettings.TenantId = int.Parse(configuration.GetConnectionString("TenantId"));
            AppSettings.BatchSize = int.Parse(configuration.GetConnectionString("BatchSize"));

            ExportOverDueContext context = new ExportOverDueContext();
            var settings = context.ComparatorSettings.ToList();
            Uploader x = new Uploader();
            x.Execution();

        }
        catch (Exception ex)
        {
            Seriloger.LoggerInstance.Error("Error In Main", ex.Message);
        }
    }

}