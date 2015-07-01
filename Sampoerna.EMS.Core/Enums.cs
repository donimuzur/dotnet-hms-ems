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
            MasterPlant = 22
        }
        public enum PBCK1Type
        {
            [Description("New")]
            New,
            [Description("Additional")]
            Additional
        }

        public enum CK5Type
        {
            Domestic = 0,
            Intercompany = 1,
            PortToImporter = 2,
            ImporterToPlant = 3,
            Export = 4,
            Manual = 5,
            DomesticAlcohol = 6,
            Completed = 7
        }

        public enum DocumentStatus
        {
            [Description("Draft")]
            Draft = 1,
            [Description("Revised")]
            Revised = 2,
            [Description("Waiting for Approval")]
            WaitingForApproval = 3,
            [Description("Approved")]
            Approved = 4,
            [Description("Rejected")]
            Rejected = 5,
            [Description("Completed")]
            Completed = 6
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
            PBKC1 = 1,
            [Description("CK-5")]
            CK5 = 2,
            [Description("PBCK-4")]
            PBKC4 = 3,
            [Description("PBCK-3")]
            PBKC3 = 4
        }

    }
}
