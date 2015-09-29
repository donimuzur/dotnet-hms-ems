using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.Website.Models.LACK2
{
    public class Lack2DetailReportsViewModel : BaseModel
    {
        public Lack2DetailReportsViewModel()
        {
            SearchView = new Lack2SearchDetailReportsViewModel();
            DetailsList = new List<Lack2DetailReportsItem>();
        }

        public Lack2SearchDetailReportsViewModel SearchView { get; set; }
        public List<Lack2DetailReportsItem> DetailsList { get; set; }
        public Lack2ExportDetailReportsViewModel ExportModel { get; set; }
    }

    public class Lack2SearchDetailReportsViewModel
        {

            public string CompanyCode { get; set; }
            public string NppbkcId { get; set; }
            public string SendingPlantId { get; set; }
            public string GoodType { get; set; }
            public int? PeriodMonth { get; set; }
            public int? PeriodYear { get; set; }
            public DateTime? DateFrom { get; set; }
            public SelectList DateFromList { get; set; }

            public DateTime? DateTo { get; set; }
            public SelectList DateToList { get; set; }    

            public SelectList CompanyCodeList { get; set; }
            public SelectList NppbkcIdList { get; set; }
            public SelectList SendingPlantIdList { get; set; }
            public SelectList GoodTypeList { get; set; }
            
            public SelectList PeriodMonthList { get; set; }
            public SelectList PeriodYearList { get; set; }


        }

    public class Lack2DetailReportsItem
    {
        public string Lack2Number { get; set; }
        public string Ck5GiDate { get; set; }
        public string Ck5RegistrationNumber { get; set; }
        public string Ck5RegistrationDate { get; set; }
        public string Ck5Total { get; set; }
        public string ReceivingCompanyCode { get; set; }
        public string ReceivingCompanyName { get; set; }
        public string ReceivingNppbkc { get; set; }
        public string ReceivingAddress { get; set; }

        public string Ck5SendingPlant { get; set; }
        public string SendingPlantAddress { get; set; }
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public string NppbkcId { get; set; }
        public string TypeExcisableGoods { get; set; }
        public string TypeExcisableGoodsDesc { get; set; }
    }

    public class Lack2ExportDetailReportsViewModel : Lack2SearchDetailReportsViewModel
        {
            public bool BLack2Number { get; set; }
            public bool BCk5GiDate { get; set; }
            public bool BCk5RegistrationNumber { get; set; }
            public bool BCk5RegistrationDate { get; set; }
            public bool BCk5Total { get; set; }
            public bool BReceivingCompanyCode { get; set; }
            public bool BReceivingCompanyName { get; set; }
            public bool BReceivingNppbkc { get; set; }
            public bool BReceivingAddress { get; set; }

            public bool BCk5SendingPlant { get; set; }
            public bool BSendingPlantAddress { get; set; }
            public bool BCompanyCode { get; set; }
            public bool BCompanyName { get; set; }
            public bool BNppbkcId { get; set; }
            public bool BTypeExcisableGoods { get; set; }
        }
}