namespace ExportOverDueFileUploader.DBmodels
{
    public static class AppSettings
    {
        public static string ConnectionString { get; set; } = string.Empty;
        public static int BatchSize { get; set; }
        public static int TenantId { get; set; }

        public static long MatchReqStats { get; set; }
        public static long NotMatchReqStats { get; set; }
        public static long GdFiLinkReqStats { get; set; }

    }
}
