using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.BusinessObject.DTOs
{
    public class Pbck7AndPbck3Dto
    {
        public Pbck7AndPbck3Dto()
        {
            Pbck3Status = Enums.DocumentStatus.Draft;
        }

        public int Pbck7Id { get; set; }
        public string Pbck7Number { get; set; }
        public string Pbck3Number { get; set; }
        public Enums.DocumentStatus Pbck7Status { get; set; }
        public Enums.DocumentStatus Pbck3Status { get; set; }
        public DateTime Pbck7Date { get; set; }
        public DateTime? Pbck3Date { get; set; }
        public Enums.DocumentTypePbck7AndPbck3 DocumentType { get; set; }
        public string NppbkcId { get; set; }
        public string PlantId { get; set; }
        public string PlantName { get; set; }
        public string PlantCity { get; set; }
        public DateTime? ExecDateFrom { get; set; }
        public DateTime? ExecDateTo { get; set; }
        public Enums.DocumentStatusGov? GovStatus { get; set; }
        public Enums.DocumentStatus Status { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string ApprovedByManager { get; set; }
        public DateTime? ApprovedDateManager { get; set; }

        public string RejectedBy { get; set; } 
        public DateTime? RejectedDate { get; set; }
       
       

        public string CreatedBy { get; set; }
        public DateTime CreateDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }


        public string ApprovedPbck3By { get; set; }
        public DateTime? ApprovedPbck3Date { get; set; }
        public string ApprovedPbck3ByManager { get; set; }
        public DateTime? ApprovedPbck3DateManager { get; set; }

        public string RejectedPbck3By { get; set; }
        public DateTime? RejectedPbck3Date { get; set; }



        public string CreatedPbck3By { get; set; }
        public DateTime CreatePbck3Date { get; set; }
        public string ModifiedPbck3By { get; set; }
        public DateTime? ModifiedPbck3Date { get; set; }

        //from table BACK1
        public string Back1Number { get; set; }
        public DateTime? Back1Date { get; set; }

        public List<Pbck7ItemUpload> UploadItems { get; set; }

        public string Comment { get; set; }

        public bool IsRejected { get; set; }

        public bool IsCreatedPbck7 { get; set; }

        public bool IsCreatedPbkc3 { get; set; }

        public Enums.DocumentStatusGov? Pbck7GovStatus { get; set; }


        public string Back1Lampiran { get; set; }
        public Enums.DocumentStatusGov? Back1GovStatus { get; set; }
        public Enums.DocumentStatusGov Back1GovStatusList { get; set; }

        public List<BACK1_DOCUMENT> DocumentsBack1 { get; set; } 


    }

    public class Pbck7ItemUpload
    {
        public long Id { get; set; }
        public string FaCode { get; set; }

        public string ProdTypeAlias { get; set; }

        public string Brand { get; set; }

        public Decimal? Content { get; set; }

        public Decimal? Pbck7Qty { get; set; }

        public Decimal? Back1Qty { get; set; }

        public int? FiscalYear { get; set; }

        public Decimal? Hje { get; set; }

        public Decimal? Tariff { get; set; }

        public Decimal? ExciseValue { get; set; }

        public Decimal? SeriesValue { get; set; }

    }

   
}
