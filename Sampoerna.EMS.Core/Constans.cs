namespace Sampoerna.EMS.Core
{
    public class Constans
    {
        public const string MasterDataHeaderFooterFolder = "~/files_upload/";
        public const string UploadPath = "~/files_upload/";
        public const string Pbck1DecreeDocFolderPath = "~/Content/upload/pbck1decreedoc/";
        public const string CK5FolderPath = "~/Content/upload/CK5/";

        public const string InList = "In List";
        public static readonly string MenuActiveDashboard = "Dashboard";
        public static string DelimeterSelectItem = " - ";
        public const string PI = "PI";

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

            public const string ReportParameters = "sk_reportparameters";

            /// <summary>
            /// Current User session key
            /// </summary>
            public const string CurrentUser = "sk_current_user";

            public const string MainMenu = "sk_main_menu";

            public const string ExcelUploadProdConvPbck1 = "ExcelUploadProdConvertedPbck1";
        }

        public class  SubmitType
        {
            public const string Save = "Save";
            public const string Cancel = "Cancel";
            public const string Update = "Update";
            public const string PrintPreview = "PrintPreview";
            public const string Delete = "Delete";
            public const string DataExist = "DataExist";
            public const string DataExistPlant = "Main Plant With NPPBKC ID is Already Exists";
        }
        public class SubmitMessage
        {
            public const string Saved = "Save Succefully";
            public const string Updated = "Update Succefully";
            public const string Deleted = "Delete Succefully";
            public const string DataExist = "Data Already Exist";
            public const string DataExistPlant = "Main Plant With NPPBKC ID is Already Exists";
        }


    }
}
