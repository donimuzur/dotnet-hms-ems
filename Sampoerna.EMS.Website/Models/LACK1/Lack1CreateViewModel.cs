﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Validations;

namespace Sampoerna.EMS.Website.Models.LACK1
{
    public class Lack1CreateViewModel : Lack1BaseItemModel
    {
        public Lack1CreateViewModel()
        {
            SubmissionDate = null;
        }
        [Required]
        public string Bukrs { get; set; }
        public SelectList BukrList { get; set; }
        public string Butxt { get; set; }

        [Required]
        public int PeriodMonth { get; set; }
        public SelectList MontList { get; set; }

        [Required]
        public int PeriodYears { get; set; }
        public SelectList YearsList { get; set; }
        [Required]
        public string NppbkcId { get; set; }
        public SelectList NppbkcList { get; set; }
        [RequiredIf("Lack1Level", Enums.Lack1Level.Plant)]
        public string LevelPlantId { get; set; }//Receiving Plant Id
        public SelectList ReceivePlantList { get; set; }

        [Required]
        public DateTime? SubmissionDate { get; set; }

        [Required]
        public string SupplierPlantId { get; set; }//Supplier Plant Werks
        public SelectList SupplierList { get; set; }

        [Required]
        public string ExGoodsTypeId { get; set; }
        public string ExGoodsTypeDesc { get; set; }
        public SelectList ExGoodTypeList { get; set; }
        public decimal? WasteQty { get; set; }
        public string WasteUom { get; set; }
        public SelectList WasteUomList { get; set; }
        public decimal? ReturnQty { get; set; }
        public string ReturnUom { get; set; }
        public SelectList ReturnUomList { get; set; }
        public Enums.DocumentStatus Status { get; set; }
        public Enums.DocumentStatusGovType2? GovStatus { get; set; }
        public DateTime? DecreeDate { get; set; }
        public long DecreeDoc { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateBy { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime ApprovedDate { get; set; }

        public Enums.Lack1Level Lack1Level { get; set; }

        public string Lack1LevelDesc { get; set; }

        public string MenuPlantAddClassCss { get; set; }
        public string MenuNppbkcAddClassCss { get; set; }

        public string Noted { get; set; }

        public int IncomeListCount { get; set; }

        public Enums.DocumentStatusGovType2 DocGovStatusList { get; set; }

        public bool? IsCreateNew { get; set; }

        public bool IsTisToTisReport { get; set; }

        public bool IsSupplierNppbkcImport { get; set; }
    }
}