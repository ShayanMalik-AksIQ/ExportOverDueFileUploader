using ExportOverDueFileUploader.DataImporter;
using ExportOverDueFileUploader.DBmodels;
using ExportOverDueFileUploader.MatuirtyBO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

[assembly: log4net.Config.XmlConfigurator(ConfigFile = "log4net.config")]
internal class Program
{
    private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


    private static void Main(string[] args)
    {

        logger.Info("dasdsad");
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();
        AppSettings.ConnectionString= configuration.GetConnectionString("DefaultConnection");
        AppSettings.TenantId= int.Parse(configuration.GetConnectionString("TenantId"));
        AppSettings.BatchSize= int.Parse(configuration.GetConnectionString("BatchSize"));
      
    Uploader x =new Uploader();
        x.Executeion();
    }
}