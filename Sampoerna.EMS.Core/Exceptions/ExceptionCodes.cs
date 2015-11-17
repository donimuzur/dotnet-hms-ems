using System.ComponentModel;

namespace Sampoerna.EMS.Core.Exceptions
{
    public abstract partial class ExceptionCodes
    {
        public enum BaseExceptions
        {
            [Description("An unknown error occured")]
            unhandled_exception,

            InvalidDateFormat
        }

        public enum BLLExceptions
        {
            [Description("An unknown error occured")]
            unhandled_exception,

            [Description("Invalid access rights for this action")]
            InvalidAccessRight,

            [Description("Operation not allowed")]
            OperationNotAllowed,

            [Description("The data received is not valid")]
            InvalidData,

            [Description("The data could not found")]
            DataNotFound,

            [Description("There is a duplicate in the entities")]
            DuplicateEntity,

            [Description("The login is not valid")]
            LoginNotMatch,

            [Description("The end date should be greater than start date")]
            StartDateGreaterThanEndDate,

            [Description("BPPKBC data could not found")]
            NppbkcNotFound,

            [Description("PBCK-11 Ref is null")]
            Pbck1RefNull,

            [Description("CK5 Quota Exceeded")]
            CK5QuotaExceeded,

            [Description("PBCK-1 Data in this period not found")]
            Pbck1PeriodNotFound,

            [Description("Do Reversal Manual SAP")]
            ReversalManualSAP,

            [Description("A record with same parameter is already exist")]
            Lack1DuplicateSelectionCriteria,

            [Description("Excisable Group Type not found")]
            ExcisabeGroupTypeNotFound,

            [Description("Missing Income List Item")]
            MissingIncomeListItem,

            [Description("Missing Hasil Produksi BKC")]
            MissingProductionList,

            [Description("Total Usage less than or equals to zero ")]
            TotalUsageLessThanEqualTpZero,

            [Description("Generate Number error : The company data has not be set, please make sure all nppbkc has a main plant")]
            GenerateNumberCompanyDataHasNotSet,
            [Description("Generate Number error : The company alias has not be set")]
            GenerateNumberCompanyAliasHasNotSet,
            [Description("Generate Number error : The plant data has not be set")]
            GenerateNumberPlantDataHasNotSet,
            [Description("Generate Number error : The city alias has not be set")]
            GenerateNumberCityAliasHasNotSet,
            [Description("A record with same parameter is already exist")]
            Lack2DuplicateSelectionCriteria,

            [Description("Missing Ck5 data selected")]
            MissingCk5DataSelected,

            [Description("Convertion to SAP in Material Master is null")]
            ConvertedSAPNull,

            [Description("Convertion to SAP Base UOM in Material Master not exist")]
            ConvertedSAPNotExist,

            [Description("Missing PBCK-1 Data regarding to selection criteria.")]
            Lack1MissingPbck1Selected,

            [Description("PBCK-7 Error BACK-1 QTY Value.")]
            Pbck7ItemErrorBack1QtyValue,


            [Description("PBCK-7 Error BACK-1 Value More or Same than Requested QTY Value.")]
            Pbck7ItemBack1MoreThanQtyValue,

            [Description("Error Item/Material empty.")]
            MaterialOrItemErrorEmpty,

            [Description("PBCK-4 Error Requested QTY Value.")]
            Pbck4ItemErrorRequestedQtyValue,

            [Description("PBCK-4 Error CK-3 Office Value.")]
            Pbck4ErrorCk3OfficeValue,

            [Description("PBCK-4 Error BACK-1 QTY Value.")]
            Pbck4ItemErrorBack1QtyValue,

            [Description("PBCK-4 Error BACK-1 Value More or Same than Requested QTY Value.")]
            Pbck4ItemBack1MoreThanQtyValue,

            [Description("Parameter is not valid or not available")]
            Pbck1ProdPlanUploadInvalidParameter,
            
            [Description("Invalid workflow action type.")]
            InvalidWorkflowActionType,

            [Description("User already have Plant.")]
            UserHavePlantExist,

            [Description("Server is busy, please try again later.")]
            ServerIsBusy
        }

        /// <summary>
        /// Security Exceptions for wcf responses
        /// </summary>
        public enum SecurityExceptions
        {
            access_denied,
            authorization_denied,
            authentication_failure
        }

    }
}
