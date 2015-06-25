namespace Sampoerna.EMS.Core
{
    public class Constans
    {

        public const string InList = "In List";
        public static readonly string MenuActiveDashboard = "Dashboard";
        /// <summary>
        /// list of SessionKey constanta
        /// </summary>
        public class SessionKey
        {
            /// <summary>
            /// Report Path, ex : "Reports/Employee.rdlc"
            /// </summary>
            public const string ReportPath = "sk_reportpath";
            /// <summary>
            /// List of ReportDataSources
            /// </summary>
            public const string ReportDataSources = "sk_reportdatasources";
            /// <summary>
            /// Current User session key
            /// </summary>
            public const string CurrentUser = "sk_current_user";

            public const string MainMenu = "sk_main_menu";

            public const string ExcelUploadProdConvPbck1 = "ExcelUploadProdConvertedPbck1";
        }

     
    }
}
