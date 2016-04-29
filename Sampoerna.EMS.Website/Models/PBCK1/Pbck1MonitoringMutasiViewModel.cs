using Sampoerna.EMS.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Sampoerna.EMS.Website.Models.PBCK1
{
    public class Pbck1MonitoringMutasiViewModel : BaseModel
    {
        public Pbck1MonitoringMutasiViewModel()
        {
            DetailsList = new List<Pbck1MonitoringMutasiItem>();
            ExportModel = new Pbck1ExportMonitoringMutasiViewModel();
        }

        public string pbck1Number { get; set; }
        public string yearFrom { get; set; }
        public string yearTo { get; set; }
        public string supPlant { get; set; }
        public string supComp { get; set; }
        public string oriNppbkc { get; set; }
        public string oriKppbc { get; set; }
        public string poa { get; set; }
        public string creator { get; set; }
        public SelectList pbck1NumberList { get; set; }
        public SelectList yearFromList { get; set; }
        public SelectList yearToList { get; set; }
        public SelectList supPlantList { get; set; }
        public SelectList supCompList { get; set; }
        public SelectList oriNppbkcList { get; set; }
        public SelectList oriKppbcList { get; set; }
        public SelectList poaList { get; set; }
        public SelectList creatorList { get; set; }
        public List<Pbck1MonitoringMutasiItem> DetailsList { get; set; }
        public Pbck1ExportMonitoringMutasiViewModel ExportModel { get; set; }
    }

    public class Pbck1MonitoringMutasiItem
    {

        public string Pbck1Number { get; set; }
        public SelectList pbck1NumberList { get; set; }
        //include docnumber, GrandtotalExciseable from summaryReport
        public List<CK5.CK5Item> Ck5List { get; set; }

        //From Monitoring Usage
        [UIHint("FormatQty")]
        public decimal QuotaRemaining { get; set; }

        [UIHint("FormatQty")]
        public decimal TotalPbck1Quota { get; set; }

        [UIHint("FormatQty")]
        public decimal Received { get; set; }
        public string Poa { get; set; }
        public string Creator { get; set; }
        public string SupPlant { get; set; }
        public string SupPlantDesc { get; set; }
        public string SupComp { get; set; }
        public string OriNppbkc { get; set; }
        public string OriKppbc { get; set; }
        public string IsNppbkcImport { get; set; }
        public string ExcGoodsType { get; set; }
        public string RecComp { get; set; }
        public string RecNppbkc { get; set; }
        public string RecKppbc { get; set; }
    }

    public class Pbck1ExportMonitoringMutasiViewModel
    {
        public string FilterPbck1Number { get; set; }
        public int? FilterYearFrom { get; set; }
        public int? FilterYearTo { get; set; }
        public string FilterSupPlant { get; set; }
        public string FilterSupComp { get; set; }
        public string FilterOriNppbkc { get; set; }
        public string FilterOriKppbc { get; set; }
        public string FilterPoa { get; set; }
        public string FilterCreator { get; set; }
        public bool Pbck1Number { get; set; }
        public bool QuotaRemaining { get; set; }
        public bool TotalPbck1Quota { get; set; }
        public bool DocNumberCk5 { get; set; }
        public bool GrDateCk5 { get; set; }
        public bool RegDateCk5 { get; set; }
        public bool GrandTotalExciseable { get; set; }
        public bool UoM { get; set; }
        public bool Ck5Type { get; set; }
        public bool Received { get; set; }
        public bool PoaCheck { get; set; }
        public bool CreatorCheck { get; set; }
        public bool SupPlantCheck { get; set; }
        public bool SupPlantDescCheck { get; set; }
        public bool SupCompCheck { get; set; }
        public bool OriNppbkcCheck { get; set; }
        public bool OriKppbcCheck { get; set; }
        public bool IsNppbkcImport { get; set; }
        public bool ExcGoodsType { get; set; }
        public bool RecComp { get; set; }
        public bool RecNppbkc { get; set; }
        public bool RecKppbc { get; set; }
        public bool DetailCk5Plant { get; set; }
        public bool DetailCk5PlantDesc { get; set; }
    }

    public class ExportMonitoringMutasiDataModel
    {
        public string Pbck1Number { get; set; }
        public string QuotaRemaining { get; set; }
        public string TotalPbck1Quota { get; set; }
        public string DocNumberCk5 { get; set; }
        public string GrDateCk5 { get; set; }
        public string RegDateCk5 { get; set; }
        public string GrandTotalExciseable { get; set; }
        public string UoM { get; set; }
        public string Received { get; set; }
        public string Poa { get; set; }
        public string Creator { get; set; }
        public string SupPlant { get; set; }
        public string SupPlantDesc { get; set; }
        public string SupComp { get; set; }
        public string OriNppbkc { get; set; }
        public string OriKppbc { get; set; }
        public string IsNppbkcImport { get; set; }
        public string ExcGoodsType { get; set; }
        public string RecComp { get; set; }
        public string RecNppbkc { get; set; }
        public string RecKppbc { get; set; }
        public string DetailCk5Plant { get; set; }
        public string DetailCk5PlantDesc { get; set; }
    }
}