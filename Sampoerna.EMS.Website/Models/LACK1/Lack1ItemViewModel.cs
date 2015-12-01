﻿using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Models.WorkflowHistory;

namespace Sampoerna.EMS.Website.Models.LACK1
{
    public class Lack1ItemViewModel : Lack1BaseItemModel
    {
        public Lack1ItemViewModel()
        {
            WorkflowHistory = new List<WorkflowHistoryViewModel>();
        }

        #region ------------ Lack1 Field List ------
        
        public string Bukrs { get; set; }
        public string Butxt { get; set; }
        public int? PeriodMonth { get; set; }
        public string PeriodNameInd { get; set; }
        public string PerionNameEng { get; set; }
        public int? PeriodYears { get; set; }
        /// <summary>
        /// Concate PeriodMonth and PeriodYear
        /// </summary>
        public DateTime Periode { get; set; }
        public string LevelPlantId { get; set; }
        public string LevelPlantName { get; set; }
        public DateTime? SubmissionDate { get; set; }
        public string SupplierPlant { get; set; }
        public string SupplierPlantId { get; set; }
        public string SupplierPlantAddress { get; set; }
        public string SupplierCompanyCode { get; set; }
        public string SupplierCompanyName { get; set; }

        public string ExGoodsTypeId { get; set; }
        public string ExGoodsTypeDesc { get; set; }
        public decimal? WasteQty { get; set; }
        public string WasteUom { get; set; }
        public string WasteUomDesc { get; set; }
        public decimal ReturnQty { get; set; }
        public string ReturnUom { get; set; }
        public string ReturnUomDesc { get; set; }
        public Enums.DocumentStatus Status { get; set; }
        public string StatusDescription { get; set; }
        public Enums.DocumentStatusGovType2? GovStatus { get; set; }
        public string GovStatusDescription { get; set; }
        public DateTime? DecreeDate { get; set; }
        public string NppbkcId { get; set; }
        public decimal BeginingBalance { get; set; }
        public decimal TotalIncome { get; set; }
        public decimal TotalUsage { get; set; }
        public decimal EndingBalance { get; set; }
        public string Lack1UomId { get; set; }
        public string Lack1UomName { get; set; }
        public Enums.Lack1Level Lack1Level { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateBy { get; set; }
        public string ApprovedByPoa { get; set; }
        public DateTime? ApprovedPoaDate { get; set; }
        public string ApprovedByManager { get; set; }
        public DateTime? ApprovedManagerDate { get; set; }
        public List<Lack1DocumentItemModel> Lack1Document { get; set; }
        public List<Lack1IncomeDetailItemModel> IncomeList { get; set; }
        public List<Lack1Pbck1MappingItemModel> Lack1Pbck1Mapping { get; set; }
        public List<Lack1PlantItemModel> Lack1Plant { get; set; }
        
        public string Noted { get; set; }
        public string DocumentNoted { get; set; }
        public bool IsTisToTisReport { get; set; }
        public Lack1InventoryAndProductionModel InventoryProductionTisToFa { get; set; }
        public Lack1InventoryAndProductionModel InventoryProductionTisToTis { get; set; }
        public List<Lack1SummaryProductionItemModel> FusionSummaryProductionList { get; set; }//set by controller
        public List<Lack1ProductionDetailItemSummaryByProdTypeModel> FusionSummaryProductionByProdTypeList { get; set; }

        #endregion

        #region --------------- View Purpose --------------
        public string DisplayLevelPlantName { get; set; }
        public string DisplaySupplierPlant { get; set; }
        public string ControllerAction { get; set; }
        public bool AllowGovApproveAndReject { get; set; }
        public bool AllowApproveAndReject { get; set; }
        public bool AllowManagerReject { get; set; }
        public string Comment { get; set; }
        public Enums.LACK1Type Lack1Type { get; set; }
        public string MenuPlantAddClassCss { get; set; }
        public string MenuNppbkcAddClassCss { get; set; }
        public string MenuCompletedAddClassCss { get; set; }
        public int IncomeListCount { get; set; }

        public SelectList BukrList { get; set; }
        public SelectList MontList { get; set; }
        public SelectList YearsList { get; set; }
        public SelectList NppbkcList { get; set; }
        public SelectList ReceivePlantList { get; set; }
        public SelectList SupplierList { get; set; }
        public SelectList ExGoodTypeList { get; set; }
        public SelectList WasteUomList { get; set; }
        public SelectList ReturnUomList { get; set; }
        public Enums.DocumentStatusGov DocGovStatusList { get; set; }
        
        public List<HttpPostedFileBase> DecreeFiles { get; set; }
        public Enums.ActionType GovApprovalActionType { get; set; }
        public string IsSaveSubmit { get; set; }
        public bool IsDisplayOnly { get; set; }

        #endregion

        public List<WorkflowHistoryViewModel> WorkflowHistory { get; set; }
    }


    #region ------------ Class Definition related with Lack1ItemViewModel -----------------
    public class Lack1SummaryProductionItemModel
    {
        public decimal Amount { get; set; }
        public string UomId { get; set; }
        public string UomDesc { get; set; }
    }

    public class Lack1ProductionDetailItemModel
    {
        public long Lack1ProductionDetailId { get; set; }
        public int Lack1Id { get; set; }
        public string ProdCode { get; set; }
        public string ProductType { get; set; }
        public string ProductAlias { get; set; }
        public decimal Amount { get; set; }
        public string UomId { get; set; }
        public string UomDesc { get; set; }
        public string FaCode { get; set; }
        public string Ordr { get; set; }
    }

    public class Lack1ProductionDetailItemSummaryByProdTypeModel
    {
        public string ProdCode { get; set; }
        public string ProductType { get; set; }
        public string ProductAlias { get; set; }
        public decimal TotalAmount { get; set; }
        public string UomId { get; set; }
        public string UomDesc { get; set; }
    }

    public class Lack1PlantItemModel
    {
        public long Lack1PlantId { get; set; }
        public int Lack1Id { get; set; }
        public string Werks { get; set; }
        public string Name1 { get; set; }
        public string Address { get; set; }
    }

    public class Lack1Pbck1MappingItemModel
    {
        public long LACK1_PBCK1_MAPPING_ID { get; set; }
        public int LACK1_ID { get; set; }
        public int PBCK1_ID { get; set; }
        public string PBCK1_NUMBER { get; set; }
        public DateTime? DECREE_DATE { get; set; }
        public string DisplayDecreeDate { get; set; }
    }

    public class Lack1IncomeDetailItemModel
    {
        public long Lack1IncomeDetailId { get; set; }
        public int Lack1Id { get; set; }
        public long Ck5Id { get; set; }
        public decimal Amount { get; set; }
        public string RegistrationNumber { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public string StringRegistrationDate { get; set; }
    }

    public class Lack1DocumentItemModel
    {
        public int LACK1_DOCUMENT_ID { get; set; }
        public int LACK1_ID { get; set; }
        public string FILE_NAME { get; set; }
        public string FILE_PATH { get; set; }
    }

    public class Lack1InventoryAndProductionModel
    {
        public Lack1ProductionModel ProductionData { get; set; }
    }

    public class Lack1ProductionModel
    {
        public List<Lack1ProductionDetailItemModel> ProductionList { get; set; }
        public List<Lack1ProductionDetailItemSummaryByProdTypeModel> ProductionSummaryByProdTypeList { get; set; }
        public List<Lack1SummaryProductionItemModel> SummaryProductionList { get; set; }//todo: set manually from controller to create summary of ProductionList
    }

    #endregion

}