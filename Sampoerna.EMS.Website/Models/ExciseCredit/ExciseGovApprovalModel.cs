using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sampoerna.EMS.Website.Models.ExciseCredit
{
    public class ExciseGovApprovalModel
    {

        public ExciseGovApprovalModel()
        {
            var govStatus = new Dictionary<int, string>();
            govStatus.Add(0, "Rejected");
            govStatus.Add(1, "Approved");
            var query = from x in govStatus
                        select new SelectItemModel()
                        {
                            ValueField = x.Key,
                            TextField = x.Value
                        };
            this.GovernmentStatus = new SelectList(query.DistinctBy(c => c.ValueField), "ValueField", "TextField");
        }
        public SelectList GovernmentStatus { set; get; }
        public SelectList AvailableProductTypes { set; get; }
        public long Id { set; get; }
        [Required(ErrorMessage = "Government approval status is required")]
        public bool SkepStatus { set; get; }
        public int SkepStatusId { set; get; }
        public string DecreeNumber { set; get; }
        public DateTime DecreeDate { set; get; }
        public decimal CreditAmount { set; get; }
        public string CreditAmountDisplay { set; get; }

        public FileUpload.FileUploadModel SkepFileUpload { set; get; }
        public string SkepDocument { set; get; }
        public DateTime StartDate { set; get; }
        public DateTime BpjDate { set; get; }
        public String BpjNumber { set; get; }
        public FileUpload.FileUploadModel BpjFileUpload { set; get; }
        public string BpjDocument { set; get; }
        public string Notes { set; get; }
        public bool IsNewEntry { set; get; }
        public bool IsDetail { set; get; }
        public bool IsWaitingSkepApproval { set; get; }
        public bool IsApprover { set; get; }

        public List<ExciseApprovedProduct> ApprovedProducts { set; get; }
    }
}