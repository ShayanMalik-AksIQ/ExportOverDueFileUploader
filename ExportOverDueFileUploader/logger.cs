
using Serilog;

namespace ExportOverDueFileUploader
{
    public static class Seriloger
    {
        private static readonly ILogger _loggerInstance = ConfigureLogger();

        public static ILogger LoggerInstance => _loggerInstance;

        private static ILogger ConfigureLogger()
        {
            return new LoggerConfiguration().WriteTo.Console()
                .WriteTo.File("MounthlyLogs/Logs.txt", rollingInterval: RollingInterval.Month)
                .CreateLogger();
        }
    }
}

