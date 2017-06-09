using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Mvc;
using DocumentFormat.OpenXml.Wordprocessing;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.Website.Models.ManufacturingLicense
{
    public class LicenseRequestSummaryReportsViewModel : BaseModel
    {
        public LicenseRequestSummaryReportsViewModel()
        {
            SearchView = new LicenseRequestSearchSummaryReportsViewModel();
            LicenseRequestDocuments = new List<vwMLLicenseRequestModel>();
        }

        public LicenseRequestSearchSummaryReportsViewModel SearchView { get; set; }
        public List<vwMLLicenseRequestModel> LicenseRequestDocuments { set; get; }

        public LicenseRequestExportSummaryReportsViewModel ExportModel { get; set; }

        //public Enums.LicenseRequestType LicenseRequestType { get; set; }

        public int TotalData { get; set; }
        public int TotalDataPerPage { get; set; }

        public int CurrentPage { get; set; }

        
    }

    public class LicenseRequestSearchSummaryReportsViewModel
    {
        public int FormNumberSource { set; get; }
        public string CompanyTypeSource { set; get; }
        public string KPPBCSource { set; get; }
        public string LastApprovedStatusSource { set; get; }

        public SelectList FormNumberList { set; get; }
        public SelectList CompanyTypeList { set; get; }
        public SelectList KPPBCList { set; get; }
        public SelectList LastApprovedStatusList { set; get; }

    }

    public class LicenseRequestExportSummaryReportsViewModel : LicenseRequestSearchSummaryReportsViewModel
    {
        public bool FormNumber { get; set; }
        public bool RequestDate { get; set; }
        public bool CompanyName { get; set; }
        public bool KPPBC { get; set; }
        public bool CreatedBy { get; set; }
        public bool CreatedDate { get; set; }
        public bool ModifyBy { get; set; }
        public bool ModifyDate { get; set; }
        public bool LastApprovedBy { get; set; }
        public bool LastApprovedDate { get; set; }
        public bool LastApprovedStatus { get; set; }

        public bool MnfRequestId { get; set; }
        public bool DecreeNo { get; set; }
        public bool DecreeDate { get; set; }
        public bool DecreeStatus { get; set; }
        public bool NppbkcID { get; set; }
        public LicenseRequestDetailExportSummaryReportsViewModel DetailExportModel { get; set; }

    }

    public class LicenseRequestDetailExportSummaryReportsViewModel
    {
        public bool ManufactureAddress { get; set; }
        public bool CityName { get; set; }
        public bool StateName { get; set; }
        public bool SubDistrict { get; set; }
        public bool Village { get; set; }
        public bool Phone { get; set; }
        public bool Fax { get; set; }

        public bool North { get; set; }
        public bool East { get; set; }
        public bool South { get; set; }
        public bool West { get; set; }
        public bool LandArea { get; set; }
        public bool BuildingArea { get; set; }
        public bool OwnershipStatus { get; set; }
    }



}