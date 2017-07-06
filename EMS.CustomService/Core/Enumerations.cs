using System.ComponentModel;

namespace Sampoerna.EMS.CustomService.Core
{
    public class ReferenceKeys
    {
        public enum KeyGroup
        {
            HintComponent,
            ApprovalStatus,
            UploadFileLimit,
            ApproverUser
        };

        public enum HintComponent
        {
            SubmissionDate,
            POAName,
            POAPosition,
            POAAddress,
            NPPBKC,
            KPPBC,
            KPPBCCity,
            RegionalOffice,
            RegionalOfficeAddress,
            Company,
            ExciseCreditAmount,
            FinancialStatement,
            LiquidityRatio,
            SolvencyRatio,
            RentabilityRatio,
            Guarantee,
            SupportingDocument,
            WeightedTariff,
            WeightedTariffPercentage,
            LatestSKEPCredit,
            GovernmentStatus

        };

        public enum ApprovalStatus
        {
            Draft,
            Edited,
            AwaitingAdminApproval,
            AwaitingPoaApproval,
            AwaitingGovernmentApproval,
            AwaitingPoaSkepApproval,
            AwaitingExciseApproval,
            Completed,
            Rejected,
            Canceled
        }

        public enum UploadFileLimit
        {
            General
        }

        public enum Approver
        {
            AdminApprover
        }

        public enum EmailContent
        {         
            FinanceRatioApprovalRequest = 1,
            FinanceRatioApproved = 2,
            FinanceRatioRejected = 3,
            TariffApprovalRequest = 4,
            TariffApproved = 5,
            TariffRejected = 6,
            SupportDocApprovalRequest = 7,
            SupportDocApproved = 8,
            SupportDocRejected = 9,
            ProductTypeApprovalRequest = 10,
            ProductTypeApproved = 11,
            ProductTypeRejected = 12,
            ManufacturingLicenseInterviewApprovalRequest = 13,
            ManufacturingLicenseInterviewApprovalNotification = 14,
            ManufacturingLicenseInterviewRevisionRequest = 15,
            
            ManufacturingLicenseChangeApprovalRequest = 16,
            ManufacturingLicenseChangeApprovalNotification = 17,
            ManufacturingLicenseChangeRevisionRequest = 18,

            ManufacturingLicenseApprovalRequest = 19,
            ManufacturingLicenseApprovalNotification = 20,
            ManufacturingLicenseRevisionRequest = 21,

            ProductDevelopmentApprovalRequest = 32,
            ProductDevelopmentApproved = 33,
            ProductDevelopmentRejected = 34,
            BrandRegistrationApprovalRequest = 28,
            BrandRegistrationApprovedPreGovernmentStatus = 39,
            BrandRegistrationRejected = 45,
            BrandRegistrationApprovalRequestPostGovernmentStatus = 36,
            BrandRegistrationSKEPRejectedRevise = 37,
            BrandRegistrationApprovedPostGovernmentStatus = 38,
            PenetapanSKEPApprovalRequest = 25,
            PenetapanSKEPApproved = 26,
            PenetapanSKEPRejected = 27,
            ExciseCreditApprovalRequest = 22,
            ExciseCreditApprovalNotification = 23,
            ExciseCreditRejection = 24,
            ExciseCreditSKEPApprovalRequest = 29,
            ExciseCreditSKEPApprovalNotification = 30,
            ExciseCreditSKEPApprovalRejection = 31, 
            ExciseCreditWithdrawNotice = 35


        }

        public enum EmailSender
        {
            Admin,
            AdminCreator,
            AdminApprover,
            POA,
            POAManager,
            Viewer,
            CreatorPRD,
            POAExcise
        }

        public enum PrintoutLayout
        {
            [Description("Not Found")]
            None = 0,

            [Description("New Excise Credit Printout")]
            ExciseCreditNewRequest,

            [Description("New Excise Credit Main Printout")]
            ExciseCreditNewRequestMain,

            [Description("Adjustment Excise Credit Printout")]
            ExciseCreditAdjustmentRequest,

            [Description("Adjustment Excise Credit Main Printout")]
            ExciseCreditAdjustmentRequestMain,

            [Description("Excise Financial Ratio Printout")]
            FinanceRatio,

            [Description("List CK1 Printout")]
            DetailExciseCalculation,

            ExciseNewRequestDecree,
            ExciseAdjustmentRequestDecree,

            [Description("Excise Credit Request Guarantee Decree")]
            ExciseRequestGuaranteeDecree
        }


    }
}
