﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Mvc;
using DocumentFormat.OpenXml.Wordprocessing;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.Website.Models.CK5
{
    public class CK5SummaryReportsViewModel : BaseModel
    {
        public CK5SummaryReportsViewModel()
        {
            SearchView = new CK5SearchSummaryReportsViewModel();
            DetailsList = new List<CK5SummaryReportsItem>();
        }

        public CK5SearchSummaryReportsViewModel SearchView { get; set; }
        public List<CK5SummaryReportsItem> DetailsList { get; set; }

        public CK5ExportSummaryReportsViewModel ExportModel { get; set; }

        public Enums.CK5Type Ck5Type { get; set; }

        public int TotalData { get; set; }
        public int TotalDataPerPage { get; set; }

        public int CurrentPage { get; set; }

        
    }

    public class CK5SearchSummaryReportsViewModel
    {
        public Enums.CK5Type Ck5Type { get; set; }
        public Enums.CK5Type Ck5TypeList { get; set; }

        public string CompanyCodeSource { get; set; }
        public SelectList CompanyCodeSourceList { get; set; }

        public string CompanyCodeDest { get; set; }
        public SelectList CompanyCodeDestList { get; set; }

        public string NppbkcIdSource { get; set; }
        public SelectList NppbkcIdSourceList { get; set; }

        public string NppbkcIdDest { get; set; }
        public SelectList NppbkcIdDestList { get; set; }

        public string PlantSource { get; set; }
        public SelectList PlantSourceList { get; set; }

        public string PlantDest { get; set; }
        public SelectList PlantDestList { get; set; }

        public DateTime? DateFrom { get; set; }
        public SelectList DateFromList { get; set; }

        public DateTime? DateTo { get; set; }
        public SelectList DateToList { get; set; }

        public string Month { get; set; }
        public SelectList MonthList { get; set; }

        public string Year { get; set; }
        public SelectList YearList { get; set; }

        public string MaterialNumber { get; set; }
        public string MaterialDescription { get; set; }
        public string Poa { get; set; }
        public string Creator { get; set; }

        public SelectList MaterialNumberList { get; set; }
        public SelectList MaterialDescriptionList { get; set; }
        public SelectList PoaList { get; set; }
        public SelectList CreatorList { get; set; }

       

    }

    public class CK5SummaryReportsItem
    {

        public long Ck5Id { get; set; }

        public string Ck5TypeDescription { get; set; }
        public string KppbcCityName { get; set; }
        public string SubmissionNumber { get; set; }
        public string SubmissionDate { get; set; }
        public string ExGoodTypeDesc { get; set; }
        public string ExciseSettlement { get; set; }
        public string ExciseStatus { get; set; }
        public string RequestType { get; set; }
        public string SourcePlantId { get; set; }
        public string SourcePlant { get; set; }
        public string DestinationPlantId { get; set; }
        public string DestinationPlant { get; set; }
        
        public string UnpaidExciseFacilityNumber { get; set; }
        public string UnpaidExciseFacilityDate { get; set; }
        public string SealingNotificationDate { get; set; }
        public string SealingNotificationNumber { get; set; }
        public string UnSealingNotificationDate { get; set; }
        public string UnSealingNotificationNumber { get; set; }
        public string Lack1 { get; set; }
        public string Lack2 { get; set; }

        public string TanggalAju { get; set; }
        public string NomerAju { get; set; }
        public string TanggalPendaftaran { get; set; }
        public string NomerPendaftaran { get; set; }
        public string OriginCeOffice { get; set; }
        public string OriginCompany { get; set; }
        public string OriginCompanyNppbkc { get; set; }
        public string OriginCompanyAddress { get; set; }
        public string DestinationCountry { get; set; }
        public string NumberBox { get; set; }
        public string ContainPerBox { get; set; }
        public string ConvertedUom { get; set; }
        public string ConvertedQty { get; set; }
        public string TotalOfExcisableGoods { get; set; }
        public string BanderolPrice { get; set; }
        public string ExciseTariff { get; set; }
        public string ExciseValue { get; set; }
        public string DestinationCeOffice { get; set; }
        public string DestCompanyAddress { get; set; }
        public string DestCompanyNppbkc { get; set; }
        public string DestCompanyName { get; set; }
        public string LoadingPort { get; set; }
        public string LoadingPortName { get; set; }
        public string StoNumberSender { get; set; }
        public string StoNumberReciever { get; set; }
        public string StoBNumber { get; set; }
        public string DnNumber { get; set; }
        public string GrDate { get; set; }
        public string GiDate { get; set; }
        public string Status { get; set; }

        public string CompanySource { get; set; }
        public string CompanyDestination { get; set; }
        public string Poa { get; set; }
        public string Creator { get; set; }

        public string MaterialNumber { get; set; }
        public string MaterialDescription { get; set; }
        public string CompletedDate { get; set; }

        public string SOURCE_PLANT_COMPANY_CODE { get; set; }
        public string DEST_PLANT_COMPANY_CODE { get; set; }

        public string SOURCE_PLANT_ID { get; set; }
        public string DEST_PLANT_ID { get; set; }
        public DateTime? SUBMISSION_DATE { get; set; }
    }

    public class CK5ExportSummaryReportsViewModel : CK5SearchSummaryReportsViewModel
    {
        public bool NoRow { get; set; }
        public bool Ck5TypeDescription { get; set; }
        public bool KppbcCityName { get; set; }
        public bool SubmissionNumber { get; set; }
        public bool SubmissionDate { get; set; }
        public bool ExGoodTypeDesc { get; set; }
        public bool ExciseSettlement { get; set; }
        public bool ExciseStatus { get; set; }
        public bool RequestType { get; set; }
        public bool SourcePlantId { get; set; }
        public bool SourcePlant { get; set; }
        public bool DestinationPlantId { get; set; }
        public bool DestinationPlant { get; set; }

        public bool UnpaidExciseFacilityNumber { get; set; }
        public bool UnpaidExciseFacilityDate { get; set; }

        public bool SealingNotificationDate { get; set; }
        public bool SealingNotificationNumber { get; set; }
        public bool UnSealingNotificationDate { get; set; }
        public bool UnSealingNotificationNumber { get; set; }
        public bool Lack1 { get; set; }
        public bool Lack2 { get; set; }
        public bool TanggalAju { get; set; }
        public bool NomerAju { get; set; }
        public bool TanggalPendaftaran { get; set; }
        public bool NomerPendaftaran { get; set; }
        public bool OriginCeOffice { get; set; }
        public bool OriginCompany { get; set; }
        public bool OriginCompanyNppbkc { get; set; }
        public bool OriginCompanyAddress { get; set; }
        public bool DestinationCountry { get; set; }
        public bool NumberBox { get; set; }
        public bool ContainPerBox { get; set; }
        public bool ConvertedUom { get; set; }
        public bool ConvertedQty { get; set; }
        public bool TotalOfExcisableGoods { get; set; }
        public bool BanderolPrice { get; set; }
        public bool ExciseTariff { get; set; }
        public bool ExciseValue { get; set; }
        public bool DestinationCeOffice { get; set; }
        public bool DestCompanyAddress { get; set; }
        public bool DestCompanyNppbkc { get; set; }
        public bool DestCompanyName { get; set; }
        public bool LoadingPort { get; set; }
        public bool LoadingPortName { get; set; }
        public bool StoNumberSender { get; set; }
        public bool StoNumberReciever { get; set; }
        public bool StoBNumber { get; set; }
        public bool DnNumber { get; set; }
        public bool GrDate { get; set; }
        public bool GiDate { get; set; }
        public bool Status { get; set; }

        public bool CompanySource { get; set; }
        public bool CompanyDestination { get; set; }
        public bool IsSelectPoa { get; set; }
        public bool IsSelectCreator { get; set; }

        public bool IsSelectMaterialNumber { get; set; }
        public bool IsSelectMaterialDescription { get; set; }
        public bool CompletedDate { get; set; }
    }

  

}