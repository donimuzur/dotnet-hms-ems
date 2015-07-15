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
            MasterPlant = 25
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
            [Description("Waiting for Government Approval")]
            WaitingGovApproval = 6,
            [Description("Government Approved")]
            GovApproved = 7,
            [Description("Government Rejected")]
            GovRejected = 8,
            [Description("Government Canceled")]
            GovCanceled = 9,
            [Description("STO Created")]
            STOCreated = 10,
            [Description("STO Failed")]
            STOFailed = 11,
            [Description("Outbound Delivery Created")]
            ODCreated = 12,
            [Description("Good Received Created")]
            GRCreated = 13,
            [Description("Good Received Partial")]
            GRPartial = 14,
            [Description("Good Received Completed")]
            GRCompleted = 15,
            [Description("Good Received Reversal")]
            GRReversal = 16,
            [Description("Good Issue Created")]
            GICreated = 17,
            [Description("Good Issue Partial")]
            GIPartial = 18,
            [Description("Good Issue Completed")]
            GICompleted = 19,
            [Description("Good Issue Reversal")]
            GIReversal = 20,
            [Description("Cancelled")]
            Cancelled = 21,
            [Description("Completed")]
            Completed = 22
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

        public enum ActionType
        {
            [Description("Save")]
            Save = 1,
            [Description("Submit")]
            Submit = 2,
            [Description("Approve")]
            Approve = 3,
            [Description("Reject")]
            Reject = 4,
            [Description("GovApprove")]
            GovApprove = 5,
            [Description("GovReject")]
            GovReject = 6,
            [Description("GovCancel")]
            GovCancel = 7,
            [Description("Completed")]
            Completed = 8,
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
            Manager =3
        }

        public enum FormViewType
        {
            Index = 1,
            Create = 2,
            Edit = 3,
            Detail = 4
        }
    }
}
