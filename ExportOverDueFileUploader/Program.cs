using ExportOverDueFileUploader.DataImporter;
using ExportOverDueFileUploader.DBmodels;
using Microsoft.Extensions.Configuration;


internal class Program
{
    private static void Main(string[] args)
    {

        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();
        AppSettings.ConnectionString= configuration.GetConnectionString("DefaultConnection");
        AppSettings.TenantId= int.Parse(configuration.GetConnectionString("TenantId"));
        AppSettings.BatchSize= int.Parse(configuration.GetConnectionString("BatchSize"));

        Uploader x=new Uploader();
        x.Executeion();
    }
}