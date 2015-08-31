using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Validations;

namespace Sampoerna.EMS.Website.Models.LACK1
{
    public class Lack1CreateViewModel : BaseModel
    {
        public Lack1CreateViewModel()
        {
            SubmissionDate = DateTime.Now;
        }
        public long Lack1Id { get; set; }
        public string Lack1Number { get; set; }

        [Required]
        public string Bukrs { get; set; }
        public SelectList BukrList { get; set; }

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
        public string LevelPlantId { get; set; }
        public string LevelPlantName { get; set; }
        public SelectList ReceivePlantList { get; set; }

        [Required]
        public DateTime? SubmissionDate { get; set; }

        [Required]
        public string SupplierPlant { get; set; }
        public SelectList SupplierList { get; set; }

        [Required]
        public string ExGoodsType { get; set; }
        public SelectList ExGoodTypeList { get; set; }
        public decimal? WasteQty { get; set; }
        public string WasteUom { get; set; }
        public SelectList WasteUomList { get; set; }
        public decimal? ReturnQty { get; set; }
        public string ReturnUom { get; set; }
        public SelectList ReturnUomList { get; set; }
        public Enums.DocumentStatus Status { get; set; }
        public Enums.DocumentStatusGov GovStatus { get; set; }
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

        public List<Lack1GeneratedDto> Lack1Generated { get; set; }

    }
}