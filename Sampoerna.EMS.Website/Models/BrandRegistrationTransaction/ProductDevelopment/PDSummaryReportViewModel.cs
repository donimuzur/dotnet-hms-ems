using Sampoerna.EMS.Website.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sampoerna.EMS.Website.Models.BrandRegistrationTransaction.ProductDevelopment
{
    public class PDSummaryReportViewModel : BaseModel
    {
        public PDSummaryReportViewModel()
        {
            this.ViewModel = new ProductDevelopmentModel();
        }
        public bool IsExciser { get; set; }

        public bool IsCreator { get; set; }
        public List<vwProductDevelopmentModel> ProductDevelopmentDocuments { get; set; }
        public List<ProductDevDetailModel> ProductDocuments { get; set; }
        public List<ProductDevelopmentModel> ProductOpenDoc { get; set; }
        public ProductDevelopmentModel ViewModel { get; set; }
        public SelectList YearList { set; get; }
        public SelectList KppbcList { set; get; }
        public SelectList PoaList { set; get; }
        public SelectList CreatorList { set; get; }
        public SelectList CompanyType { set; get; }
        public ProductDevelopmentFilterModel Filter { get; set; }
        public ProductDevelopmentExportSummaryReportsViewModel ExportModel { get; set; }
    }

    public class ProductDevelopmentExportSummaryReportsViewModel
    {
        public bool PD_NO { get; set; }
        public bool PD_ID { get; set; }            
        public bool Next_Action { get; set; }
        public bool CreatedDate { get; set; }
        public bool CreatedBy { get; set; }
        public ProductDevelopmentDetailExportSummaryReportsViewModel DetailExportModel { get; set; }
        public ProductDevelopmentFilterModel Filter { get; set; }
    }

    public class ProductDevelopmentDetailExportSummaryReportsViewModel
    {
        public bool PD_DETAIL_ID { get; set; }
        public bool Fa_Code_Old { get; set; }
        public bool Fa_Code_New { get; set; }
        public bool Hl_Code { get; set; }
        public bool Market_Id { get; set; }
        public bool Fa_Code_Old_Desc { get; set; }
        public bool Fa_Code_New_Desc { get; set; }
        public bool Werks { get; set; }
        public bool Is_Import { get; set; }
        public bool PD_ID { get; set; }
        public bool Request_No { get; set; }
        public bool Bukrs { get; set; }
        public bool Status { get; set; }
          
    }
    
}