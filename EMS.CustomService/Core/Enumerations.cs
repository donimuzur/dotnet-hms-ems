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

            ProductDevelopmentApprovalRequest = 25,
            ProductDevelopmentApproved = 33,
            ProductDevelopmentRejected = 27,
            BrandRegistrationApprovalRequest = 31,
            BrandRegistrationApprovedPreGovernmentStatus = 29,
            BrandRegistrationRejected = 35,
            BrandRegistrationApprovalRequestPostGovernmentStatus = 36,
            BrandRegistrationSKEPRejectedRevise = 37,
            BrandRegistrationApprovedPostGovernmentStatus = 38,
            PenetapanSKEPApprovalRequest = 28,
            PenetapanSKEPApproved = 29,
            PenetapanSKEPRejected = 30,
            ExciseCreditApprovalRequest = 22,
            ExciseCreditApprovalNotification = 23,
            ExciseCreditRejection = 24,
            ExciseCreditSKEPApprovalRequest = 25,
            ExciseCreditSKEPApprovalNotification = 26,
            ExciseCreditSKEPApprovalRejection = 27


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
            [Description("New Excise Credit Printout")]
            ExciseCreditNewRequest,

            [Description("Adjustment Excise Credit Printout")]
            ExciseCreditAdjustmentRequest,

            [Description("Excise Financial Ratio Printout")]
            FinanceRatio,

            DetailExciseCalculation,
            ExciseNewRequestDecree,
            ExciseAdjustmentRequestDecree,
            ExciseRequestGuaranteeDecree
        }


    }
}
