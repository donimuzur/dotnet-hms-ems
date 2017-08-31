using Sampoerna.EMS.Website.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sampoerna.EMS.Website.Models.ManufacturingLicense
{
    public class InterviewRequestSummaryReportsViewModel : MasterViewModel
    {
        public InterviewRequestSummaryReportsViewModel() : base()
        {
                        
        }
        public List<vwMLInterviewRequestModel> InterviewRequestDocuments { set; get; }
        public SelectList YearList { set; get; }
        public SelectList KppbcList { set; get; }
        public SelectList PoaList { set; get; }
        public SelectList CreatorList { set; get; }
        public SelectList CompanyType { set; get; }        
        public InterviewRequestFilterModel Filter { set; get; }
        public InterviewRequestExportSummaryReportsViewModel ExportModel { get; set; }
    }

    public class InterviewRequestExportSummaryReportsViewModel
    {        
        public bool Status { get; set; }
        public bool FormNo { get; set; }
        public bool RequestDate { get; set; }
        public bool Perihal { get; set; }
        public bool CompanyType { get; set; }
        public bool POAName { get; set; }
        public bool POAPosition { get; set; }
        public bool POAAddress { get; set; }
        public bool KPPBCId { get; set; }
        public bool KPPBCAddress { get; set; }
        public bool CompanyName { get; set; }
        public bool NPWP { get; set; }
        public bool CompanyAddress { get; set; }
        public bool GovStatus { get; set; }
        public bool BANumber { get; set; }
        public bool BADate { get; set; }
        public InterviewRequestDetailExportSummaryReportsViewModel DetailExportModel { get; set; }
        public InterviewRequestFilterModel Filter { set; get; }
    }

    public class InterviewRequestDetailExportSummaryReportsViewModel
    {
        public bool Address { get; set; }
        public bool City { get; set; }
        public bool Province { get; set; }
        public bool SubDistrict { get; set; }
        public bool Village { get; set; }
        public bool Phone { get; set; }
        public bool Fax { get; set; }
    }
}