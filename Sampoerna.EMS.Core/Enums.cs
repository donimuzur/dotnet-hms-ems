﻿using System.ComponentModel;

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
            GoodsTypeGroup =26
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
            Revised = 5,
            [Description("Waiting for Approval")]
            WaitingForApproval = 10,
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
            PBKC4 = 3,
            [Description("PBCK-3")]
            PBKC3 = 4
        }

        public enum ActionType
        {
            [Description("Created")]
            Created = 1,
            [Description("Modified")]
            Modified = 5,
            [Description("Submit")]
            Submit = 10,
            [Description("Approve")]
            Approve = 15,
            [Description("Reject")]
            Reject = 20,
            [Description("GovApprove")]
            GovApprove = 25,
            [Description("GovReject")]
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
            [Description("Pita Cukai")]
            PitaCukai = 2
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
            [Description("Tunai")]
            Tunai = 1,
            [Description("Tunda")]
            Tunda = 2
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
