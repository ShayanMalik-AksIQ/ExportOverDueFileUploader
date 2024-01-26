using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExportOverDueFileUploader.DBmodels
{
    public static class AppSettings
    {
        public static string ConnectionString { get; set; } = string.Empty;
        public static int BatchSize { get; set; }
        public static int TenantId { get; set; }

    }
}
