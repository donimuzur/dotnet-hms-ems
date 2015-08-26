using System.ComponentModel;

namespace Sampoerna.EMS.Core
{
    public class Enums
    {
        public enum MenuList
        {
            MasterData = 2,
            ExcisableGoodsMovement = 3,
            ExcisableGoodsClaimable = 4,
            Settings = 5,
            PBCK1 = 6,
            CK5 = 7,
            LACK1 = 8,
            LACK2 = 9,
            CK4C = 10,
            PBCK4 = 11,
            PBCK7 = 12,
            PBCK3 = 13,
            CK5MRETURN = 14,
            USER = 15,
            LOGIN = 16,
            COMPANY = 17,
            POA = 18,
            NPPBKC = 19,
            HeaderFooter = 20,
            User = 21,
            BrandRegistration = 22,
            VirtualMappingPlant = 23,
            MaterialMaster = 24,
            MasterPlant = 25,
            Uom = 27,
            GoodsTypeGroup = 26,
            UserAuthorization = 28,
            POAMap = 29,
            Vendor = 30,
            KPPBC = 31,
            UserPlantMap = 32
        }
        public enum PBCK1Type
        {
            [Description("New")]
            New = 1,
            [Description("Additional")]
            Additional = 2
        }

        public enum CK5Type
        {
            [Description("Domestic")]
            Domestic = 0,
            [Description("Intercompany")]
            Intercompany = 1,
            [Description("Import")]
            PortToImporter = 2,
            [Description("Import")]
            ImporterToPlant = 3,
           [Description("Export")]
            Export = 4,
            [Description("Manual")]
            Manual = 5,
            [Description("Domestic Alcohol")]
            DomesticAlcohol = 6,
            [Description("Completed")]
            Completed = 7
        }

       

        public enum CK5XmlStatus
        {

            StoCreated = 10,

            GICompleted = 16,

            GRCompleted = 21
         

        }

        public enum Pbck1DocumentType
        {
            OpenDocument = 1,
            CompletedDocument = 2
        }

        public enum DocumentStatus
        {
            [Description("Draft")]
            Draft = 1,
            [Description("Revised")]
            Revised = 5,
            [Description("Waiting for Approval")]
            WaitingForApproval = 10,
            [Description("Waiting for Approval")]
            WaitingForApprovalManager = 11,
            [Description("Approved")]
            Approved = 15,
            [Description("Rejected")]
            Rejected = 20,
            [Description("Waiting for Government Approval")]
            WaitingGovApproval = 25,
            [Description("Government Approved")]
            GovApproved = 30,
            [Description("Government Rejected")]
            GovRejected = 35,
            [Description("Government Canceled")]
            GovCanceled = 40,
            [Description("STO Created")]
            STOCreated = 45,
            [Description("STO Failed")]
            STOFailed = 50,
            [Description("Outbound Delivery Created")]
            ODCreated = 55,
            [Description("Good Received Created")]
            GRCreated = 60,
            [Description("Good Received Partial")]
            GRPartial = 65,
            [Description("Good Received Completed")]
            GRCompleted = 70,
            [Description("Good Received Reversal")]
            GRReversal = 75,
            [Description("Good Issue Created")]
            GICreated = 80,
            [Description("Good Issue Partial")]
            GIPartial = 85,
            [Description("Good Issue Completed")]
            GICompleted = 90,
            [Description("Good Issue Reversal")]
            GIReversal = 95,
            [Description("Cancelled")]
            Cancelled = 100,
            [Description("Completed")]
            Completed = 105
        }

        public enum DocumentStatusGov
        {
            [Description("Partial Approved")]
            PartialApproved = 1,
            [Description("Full Approved")]
            FullApproved = 2,
            [Description("Rejected")]
            Rejected = 3
        }

        public enum FormType
        {
            [Description("PBCK-1")]
            PBCK1 = 1,
            [Description("CK-5")]
            CK5 = 2,
            [Description("PBCK-4")]
            PBCK4 = 3,
            [Description("PBCK-3")]
            PBCK3 = 4,
            [Description("LACK-1")]
            LACK1 = 5
        }

        public enum ActionType
        {
            [Description("Created")]
            Created = 1,
            [Description("Cancel")]
            Cancel = 2,
            [Description("Modified")]
            Modified = 5,
            [Description("Submit")]
            Submit = 10,
            [Description("Waiting for Approval")]
            WaitingForApproval = 11,
            [Description("Approve")]
            Approve = 15,
            [Description("Reject")]
            Reject = 20,
            [Description("Gov Approve")]
            GovApprove = 25,
            [Description("Gov Partial Approve")]
            GovPartialApprove = 26,
            [Description("Gov Reject")]
            GovReject = 30,
            [Description("GovCancel")]
            GovCancel = 35,
            [Description("Completed")]
            Completed = 40,
        }

        /// <summary>
        /// message popup type
        /// </summary>
        public enum MessageInfoType
        {
            Success,
            Error,
            Warning,
            Info
        }

        public enum UserRole
        {
            User = 1,
            POA = 2,
            Manager = 3
        }

        public enum FormViewType
        {
            Index = 1,
            Create = 2,
            Edit = 3,
            Detail = 4
        }

        public enum ExciseSettlement
        {
            [Description("Pembayaran")]
            Pembayaran = 1,
            [Description("Pelekatan Pita Cukai")]
            PitaCukai = 2,
            [Description("Pembubuhan Tanda LunasLainnya")]
            PembubuhanTandaLunasLainnya = 3
        }

        public enum ExciseStatus
        {
            [Description("Belum Dilunasi")]
            BelumDilunasi = 1,
            [Description("Sudah Dilunasi")]
            SudahDilunasi = 2
        }

        public enum RequestType
        {
            [Description("Dibayar")]
            Dibayar = 10,
            [Description("Tunai")]
            Tunai = 11,
            [Description("Tunda")]
            Tunda = 12,
            [Description("Berkala")]
            Berkala = 13,
            [Description("Tidak Dipungut")]
            TidakDipungut = 20,
            [Description("Diekspor")]
            Diekspor = 21,
            [Description("Ke/Dari Pabrik/Tempat Penyimpanan")]
            TempatPenyimpanan = 22,
            [Description("Bahan Baku/Penolong BHA/BKC")]
            BahanBakuBHABKC = 23,
            [Description("Dibebaskan")]
            Dibebaskan = 30,
            [Description("Bahan Baku/Penolong BHA Non BKC")]
            BahanBakuBHANonBKC = 31,
            [Description("Iptek/Sosial Tenaga Ahli/Perwakilan Asing")]
            IptekSosial = 32,
            [Description("Ke TPB")]
            KeTPB = 33,
            [Description("Telah/Untuk dirusak sehingga tidak baik untuk diminum")]
            UntukDirusak = 34,
            [Description("Untuk konsumsi Penumpang/Awak Sarana Pengangkut ke Luar Daerah Pabean")]
            UntukKonsumsi = 35,
            [Description("Lainnya")]
            Lainnya = 40,
            [Description("Dimusnahkan")]
            Dimusnahkan = 41,
            [Description("DiolahKembali")]
            DiolahKembali = 42

        }

        public enum CarriageMethod
        {
            [Description("Darat")]
            Darat = 1,
            [Description("Laut")]
            Laut = 2,
            [Description("Udara")]
            Udara = 3
        }

        public enum LACK1Type
        {
            ListByNppbkc = 1,
            ListByPlant = 2,
            ComplatedDocument = 3
        }
        public enum CK5TransType
        {
            [Description("Created")]
            Created = 1,
            [Description("Modified")]
            Modified = 2,
            [Description("Cancelled")]
            Cancelled = 3
        }

        public enum ExGoodsType
        {
            [Description("Etil Alcohol")]
            EtilAlcohol = 1,
            [Description("MMEA")]
            MMEA = 2,
            [Description("Hasil Tembakau")]
            HasilTembakau = 3,
            [Description("Lainnya")]
            Lainnya = 4
        }

        public enum Lack1Level
        {
            [Description("NPPBKC")]
            Nppbkc = 1,
            [Description("Plant")]
            Plant = 2
        }

        public enum CK5GovStatus
        {
            [Description("Government Approved")]
            GovApproved = 1,
            [Description("Government Reject")]
            GovReject = 2,
            [Description("Government Cancel")]
            GovCancel = 3
        }
        public enum Pbck7Type
        {
            [Description("PBCK7")]
            Pbck7List =1,
            [Description("PBCK3")]
            Pbck3List  =2
        }

        public enum DocumentTypePbck7AndPbck3
        {
            [Description("Pemusnahan")]
            Pemusnahan=1,
            [Description("Pengolahan")]
            Pengolahaan=2,
            
        }

        public enum CK4CType
        {
            [Description("DailyProduction")]
            DailyProduction = 1,
            [Description("WasteProduction")]
            WasteProduction = 2,
            [Description("CK4CDocument")]
            Ck4CDocument = 3

        }
    }
}
