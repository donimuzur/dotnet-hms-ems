﻿using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Models.WorkflowHistory;

namespace Sampoerna.EMS.Website.Models.LACK1
{
    public class Lack1EditViewModel : Lack1BaseItemModel
    {
        public Lack1EditViewModel()
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
        public decimal? ReturnQty { get; set; }
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
        public decimal TotalLaboratorium { get; set; }
        public decimal? TotalUsageTisToTis { get; set; }
        public decimal EndingBalance { get; set; }
        public decimal CloseBalance { get; set; }
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
        //public List<Lack1PlantItemModel> Lack1Plant { get; set; }
        
        public string Noted { get; set; }
        public string DocumentNoted { get; set; }
        public bool IsTisToTisReport { get; set; }
        public bool IsSupplierNppbkcImport { get; set; }
        public Lack1InventoryAndProductionModel InventoryProductionTisToFa { get; set; }
        public Lack1InventoryAndProductionModel InventoryProductionTisToTis { get; set; }
        public List<Lack1SummaryProductionItemModel> FusionSummaryProductionList { get; set; }//set by controller
        public List<Lack1ProductionDetailItemSummaryByProdTypeModel> FusionSummaryProductionByProdTypeList { get; set; }

        public List<Lack1CalculationModel> CalculationDetails { get; set; }

        public List<Lack1PeriodSummaryModel> PeriodSummaries { get; set; }

        public Lack1PeriodSummaryModel StartPeriodData { get; set; }
        public Lack1PeriodSummaryModel CurrentPeriodData { get; set; }
        public Lack1PeriodSummaryModel EndPeriodData { get; set; }

        public Lack1RemarkModel Ck5RemarkData { get; set; }
        public bool IsEtilAlcohol { get; set; }
        public bool HasWasteData { get; set; }

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
        public bool? IsCreateNew { get; set; }

        public SelectList BukrList { get; set; }
        public SelectList MontList { get; set; }
        public SelectList YearsList { get; set; }
        public SelectList NppbkcList { get; set; }
        public SelectList ReceivePlantList { get; set; }
        public SelectList SupplierList { get; set; }
        public SelectList ExGoodTypeList { get; set; }
        public SelectList WasteUomList { get; set; }
        public SelectList ReturnUomList { get; set; }
        public Enums.DocumentStatusGovType2 DocGovStatusList { get; set; }
        
        public List<HttpPostedFileBase> DecreeFiles { get; set; }
        public Enums.ActionType GovApprovalActionType { get; set; }
        public string IsSaveSubmit { get; set; }

        public string JsonData { get; set; }

        #endregion

        public List<WorkflowHistoryViewModel> WorkflowHistory { get; set; }
    }
}