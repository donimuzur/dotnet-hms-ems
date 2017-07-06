using Sampoerna.EMS.Website.Models.BrandRegistrationTransaction.GeneralModel;
using Sampoerna.EMS.Website.Models.Shared;
using Sampoerna.EMS.Website.Models.WorkflowHistory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sampoerna.EMS.Website.Models.BrandRegistrationTransaction.MapSKEP
{
    public class ReceivedDecreeViewModel : BaseModel
    {
        public ReceivedDecreeViewModel()
        {
            this.ViewModel = new ReceivedDecreeModel();
            this.ListReceivedDecree = new List<ReceivedDecreeModel>();
            this.SearchInput = new SKEPFilterViewModel();
            this.Item = new List<ReceivedDecreeDetailModel>();
            this.OtherFileList = new List<FileUpload.FileUploadModel>();
            this.RemovedFilesId = new List<long>();
            this.SKEPSupportingDocumnet = new List<SKEPSupportingDocumentModel>();
            this.File_Other_Name = new List<string>();
            this.File_Other_Path = new List<string>();
            this.SKEPFileOther = new List<SKEPFileOtherModel>();
            this.ApproveConfirm = new List<ConfirmDialogModel>();
        }

        public ReceivedDecreeModel ViewModel { get; set; }
        public List<ReceivedDecreeModel> ListReceivedDecree { get; set; }
        public List<ReceivedDecreeDetailModel> Item { get; set; }
        public List<FileUpload.FileUploadModel> OtherFileList { get; set; }
        public List<string> File_Other_Name { get; set; }
        public List<HttpPostedFileBase> File_Other { get; set; }
        public List<string> File_Other_Path { get; set; }
        public List<SKEPSupportingDocumentModel> SKEPSupportingDocumnet { get; set; }
        public List<long> RemovedFilesId { set; get; }
        public SelectList NppbkcList { set; get; }
        public SelectList ProductTypeList { set; get; }
        public SelectList BrandList { set; get; }
        public SelectList CompanyTierList { set; get; }
        public NppbkcGeneralModel NPPBKC { set; get; }
        public PlantGeneralModel Plant { get; set; }
        public long File_Size { get; set; }
        public List<WorkflowHistoryViewModel> WorkflowHistory { get; set; }
        public List<SKEPFileOtherModel> SKEPFileOther { get; set; }
        public bool ShowActionOptions { get; set; }
        public bool EnableFormInput { get; set; }
        public bool IsActive { get; set; }
        public bool EditMode { get; set; }
        public bool IsAdminApprover { get; set; }
        public bool IsCompleted { get; set; }
        public string Action { get; set; }
        public string Comment { get; set; }
        public string CurrentUser { get; set; }
        public List<ConfirmDialogModel> ApproveConfirm { get; set; }
        public SKEPFilterViewModel SearchInput { get; set; }
        public PenetapanSKEPExportSummaryReportsViewModel ExportModel { get; set; }
    }

    public class SKEPFileOtherModel
    {
        public long FileId { get; set; }
        public string Path { get; set; }
        public string FileName { get; set; }
        public string Name { get; set; }
    }

    public class PenetapanSKEPExportSummaryReportsViewModel
    {
        public bool Creator { get; set; }
        public bool Status { get; set; }
        public bool FormNo { get; set; }
        public bool Nppbkc { get; set; }
        public bool Kppbc { get; set; }
        public bool CompanyName { get; set; }
        public bool AddressPlant { get; set; }
        public bool DecreeNumber { get; set; }
        public bool DecreeDate { get; set; }
        public bool StartDate { get; set; }        
        public PenetapanSKEPDetailExportSummaryReportsViewModel DetailExportModel { get; set; }
        public SKEPFilterViewModel Filter { set; get; }
    }

    public class PenetapanSKEPDetailExportSummaryReportsViewModel
    {
        public bool RequestNumber { get; set; }
        public bool FAOld { get; set; }
        public bool FAOldDesc { get; set; }
        public bool FANew { get; set; }
        public bool FANewDesc { get; set; }
        public bool Company { get; set; }
        public bool HLCode { get; set; }
        public bool Market { get; set; }
    }
}