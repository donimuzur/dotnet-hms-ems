using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sampoerna.EMS.CustomService.Core
{
    public class ReferenceLookup
    {
        private static ReferenceLookup instance = new ReferenceLookup();
        private Dictionary<Enum, String> lookupTable = new Dictionary<Enum, string>();

        public ReferenceLookup()
        {
            #region ReferenceLookup class member list

            #region Hint Component References
            lookupTable.Add(ReferenceKeys.KeyGroup.HintComponent, "HINT_COMPONENT");
            lookupTable.Add(ReferenceKeys.HintComponent.SubmissionDate, "HINT_COMPONENT");
            lookupTable.Add(ReferenceKeys.HintComponent.POAName, "POA_NAME");
            lookupTable.Add(ReferenceKeys.HintComponent.POAPosition, "POA_POSITION");
            lookupTable.Add(ReferenceKeys.HintComponent.POAAddress, "POA_ADDRESS");
            lookupTable.Add(ReferenceKeys.HintComponent.NPPBKC, "NPPBKC");
            lookupTable.Add(ReferenceKeys.HintComponent.KPPBC, "KPPBC");
            lookupTable.Add(ReferenceKeys.HintComponent.KPPBCCity, "KPPBC_CITY");
            lookupTable.Add(ReferenceKeys.HintComponent.RegionalOffice, "KANWIL");
            lookupTable.Add(ReferenceKeys.HintComponent.RegionalOfficeAddress, "KANWIL_ADDRESS");
            lookupTable.Add(ReferenceKeys.HintComponent.Company, "COMPANY");
            lookupTable.Add(ReferenceKeys.HintComponent.ExciseCreditAmount, "EXCISE_CREDIT_AMOUNT");
            lookupTable.Add(ReferenceKeys.HintComponent.FinancialStatement, "FINANCIAL_STATEMENT");
            lookupTable.Add(ReferenceKeys.HintComponent.LiquidityRatio, "LIQUIDITY_RATIO");
            lookupTable.Add(ReferenceKeys.HintComponent.SolvencyRatio, "SOLVENCY_RATIO");
            lookupTable.Add(ReferenceKeys.HintComponent.RentabilityRatio, "RENTABILITY_RATIO");
            lookupTable.Add(ReferenceKeys.HintComponent.Guarantee, "GUARANTEE");
            lookupTable.Add(ReferenceKeys.HintComponent.SupportingDocument, "SUPPORTING_DOCUMENT");
            lookupTable.Add(ReferenceKeys.HintComponent.WeightedTariff, "WEIGHTED_TARIFF");
            lookupTable.Add(ReferenceKeys.HintComponent.WeightedTariffPercentage, "WEIGHTED_TARIFF_PERCENTAGE");
            lookupTable.Add(ReferenceKeys.HintComponent.LatestSKEPCredit, "LATEST_SKEP_CREDIT");
            lookupTable.Add(ReferenceKeys.HintComponent.GovernmentStatus, "GOVERNMENT_SKEP");

            #endregion

            #region Approval Status
            lookupTable.Add(ReferenceKeys.KeyGroup.ApprovalStatus, "APPROVAL_STATUS");
            lookupTable.Add(ReferenceKeys.ApprovalStatus.Draft, "DRAFT_NEW_STATUS");
            lookupTable.Add(ReferenceKeys.ApprovalStatus.Edited, "DRAFT_EDIT_STATUS");
            lookupTable.Add(ReferenceKeys.ApprovalStatus.AwaitingAdminApproval, "WAITING_ADMIN_APPROVAL");
            lookupTable.Add(ReferenceKeys.ApprovalStatus.AwaitingPoaApproval, "WAITING_POA_APPROVAL");
            lookupTable.Add(ReferenceKeys.ApprovalStatus.AwaitingPoaSkepApproval, "WAITING_POA_SKEP_APPROVAL");
            lookupTable.Add(ReferenceKeys.ApprovalStatus.AwaitingExciseApproval, "WAITING_EXCISE_APPROVAL");
            lookupTable.Add(ReferenceKeys.ApprovalStatus.AwaitingGovernmentApproval, "WAITING_GOVERNMENT_APPROVAL");
            lookupTable.Add(ReferenceKeys.ApprovalStatus.Completed, "COMPLETED");
            lookupTable.Add(ReferenceKeys.ApprovalStatus.Rejected, "REJECTED");
            lookupTable.Add(ReferenceKeys.ApprovalStatus.Canceled, "CANCELED");
            #endregion

            #region Upload File Limit
            lookupTable.Add(ReferenceKeys.KeyGroup.UploadFileLimit, "UPLOAD_FILE_LIMIT");
            lookupTable.Add(ReferenceKeys.UploadFileLimit.General, "UPLOAD_FILE_LIMIT");
            #endregion

            #region Approver
            lookupTable.Add(ReferenceKeys.KeyGroup.ApproverUser, "ADMIN_APPROVER");
            lookupTable.Add(ReferenceKeys.Approver.AdminApprover, "APPROVER_ADMIN");
            #endregion

            #region Email Sender
            lookupTable.Add(ReferenceKeys.EmailSender.Admin, "Administrator");
            lookupTable.Add(ReferenceKeys.EmailSender.AdminCreator, "Admin Creator");
            lookupTable.Add(ReferenceKeys.EmailSender.AdminApprover, "Admin Approver");
            lookupTable.Add(ReferenceKeys.EmailSender.CreatorPRD, "Creator PRD");
            lookupTable.Add(ReferenceKeys.EmailSender.POAExcise, "POA Excise Approver");

            #endregion

            #region Printout
            lookupTable.Add(ReferenceKeys.PrintoutLayout.ExciseCreditNewRequest, "EXCISE_CREDIT_NEW_REQUEST");
            lookupTable.Add(ReferenceKeys.PrintoutLayout.ExciseCreditNewRequestMain, "EXCISE_CREDIT_NEW_REQUEST_MAIN");

            lookupTable.Add(ReferenceKeys.PrintoutLayout.ExciseCreditAdjustmentRequest, "EXCISE_CREDIT_ADJUSTMENT_REQUEST");
            lookupTable.Add(ReferenceKeys.PrintoutLayout.ExciseCreditAdjustmentRequestMain, "EXCISE_CREDIT_ADJUSTMENT_REQUEST_MAIN");

            lookupTable.Add(ReferenceKeys.PrintoutLayout.DetailExciseCalculation, "EXCISE_CREDIT_DETAIL_CALCULATION");
            lookupTable.Add(ReferenceKeys.PrintoutLayout.FinanceRatio, "EXCISE_CREDIT_FINANCIAL_RATIO");
            lookupTable.Add(ReferenceKeys.PrintoutLayout.ExciseNewRequestDecree, "EXCISE_CREDIT_NEW_REQUEST_DECREE");
            lookupTable.Add(ReferenceKeys.PrintoutLayout.ExciseAdjustmentRequestDecree, "EXCISE_CREDIT_ADJUSTMENT_REQUEST_DECREE");
            lookupTable.Add(ReferenceKeys.PrintoutLayout.ExciseRequestGuaranteeDecree, "EXCISE_CREDIT_GUARANTEE_DECREE");
            #endregion


            #endregion
        }

        #region ReferenceLookup class utility
        public string GetReferenceKey(Enum key)
        {
            return this.lookupTable[key];
        }

        public static ReferenceLookup Instance
        {
            get
            {
                return instance;
            }
        }

        #endregion
    }
}
