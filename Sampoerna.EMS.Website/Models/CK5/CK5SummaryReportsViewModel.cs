using System;
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

        
    }

    public class CK5SearchSummaryReportsViewModel
    {
        public Enums.CK5Type Ck5Type { get; set; }

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

       
      

    }

    public class CK5SummaryReportsItem
    {
        public long Ck5Id { get; set; }
        
        #region domestic

        public string ExciseStatus { get; set; }

        public string Pbck1Number { get; set; }

        public string PbckDecreeDate { get; set; }

        public string SealingNotifDate { get; set; }

        public string SealingNotifNumber { get; set; }

        public string UnSealingNotifDate { get; set; }

        public string UnSealingNotifNumber { get; set; }

        public string Lack1Number { get; set; }

        public string Lack2Number { get; set; }
        
        #endregion

        #region export

        public string SubmissionDate { get; set; }

        public string SubmissionNumber { get; set; }

        public string RegistrationDate { get; set; }

        public string RegistrationNumber { get; set; }

        public string ExGoodTypeDesc { get; set; }

        public string RequestType { get; set; }

        public string SourceKppbcName { get; set; }

        public string SourceCompanyName { get; set; }

        public string SourceNppbkcId { get; set; }

        public string SourceCompanyAddress { get; set; }

        //todo ask ??
        public string DestinationCountry { get; set; } //?
        public string TypeOfTobaccoProduct { get; set; } //?

        public string GrandTotal { get; set; }
        public string ContainBox { get; set; }
        public string TotalExcisableGoods { get; set; } //formula number of box *conversion to GRAM

        public string Hje { get; set; }
        public string ExciseTariff { get; set; }
        public string ExciseValue { get; set; } //formula = total of exciseable goods * excise tariff

        public string ForeignExchange { get; set; } //?
        public string ExciseSettlement { get; set; }
        //public string ExciseStatus { get; set; } already
        //public string Pbck1Number { get; set; }
        //public string PbckDecreeDate { get; set; }

        public string DestKppbcName { get; set; }
        public string DestNameAdress { get; set; }
        public string DestNppbkcId { get; set; }
        //public string DestKppbcName { get; set; } //?
        public string LoadingPort { get; set; }
        public string LoadingPortOffice { get; set; }

        //public string SealingNotifDate { get; set; }
        //public string SealingNotifNumber { get; set; }
        //public string Lack1Number { get; set; }
        //public string Lack2Number { get; set; }

        #endregion

    }

    public class CK5ExportSummaryReportsViewModel : CK5SearchSummaryReportsViewModel
    {
        #region Domestic

        public bool ExciseStatus { get; set; }

        public bool Pbck1Number { get; set; }

        public bool PbckDecreeDate { get; set; }

        public bool SealingNotifDate { get; set; }

        public bool SealingNotifNumber { get; set; }

        public bool UnSealingNotifDate { get; set; }

        public bool UnSealingNotifNumber { get; set; }

        public bool Lack1Number { get; set; }

        public bool Lack2Number { get; set; }

        #endregion

        #region Export

        public bool SubmissionDate { get; set; }

        public bool SubmissionNumber { get; set; }

        public bool RegistrationDate { get; set; }

        public bool RegistrationNumber { get; set; }

        public bool ExGoodTypeDesc { get; set; }

        public bool RequestType { get; set; }

        public bool SourceKppbcName { get; set; }

        public bool SourceCompanyName { get; set; }

        public bool SourceNppbkcId { get; set; }

        public bool SourceCompanyAddress { get; set; }

        //todo ask ??
        public bool DestinationCountry { get; set; } //?
        public bool TypeOfTobaccoProduct { get; set; } //?

        public bool GrandTotal { get; set; }
        public bool ContainBox { get; set; }
        public bool TotalExcisableGoods { get; set; } //formula number of box *conversion to GRAM

        public bool Hje { get; set; }
        public bool ExciseTariff { get; set; }
        public bool ExciseValue { get; set; } //formula = total of exciseable goods * excise tariff

        public bool ForeignExchange { get; set; } //?
        public bool ExciseSettlement { get; set; }
        //public string ExciseStatus { get; set; } already
        //public string Pbck1Number { get; set; }
        //public string PbckDecreeDate { get; set; }

        public bool DestKppbcName { get; set; }
        public bool DestNameAdress { get; set; }
        public bool DestNppbkcId { get; set; }
        //public string DestKppbcName { get; set; } //?
        public bool LoadingPort { get; set; }
        public bool LoadingPortOffice { get; set; }

        //public string SealingNotifDate { get; set; }
        //public string SealingNotifNumber { get; set; }
        //public string Lack1Number { get; set; }
        //public string Lack2Number { get; set; }

        #endregion
    }

  

}