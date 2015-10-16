using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.BusinessObject.DTOs
{
    public class Pbck3Dto
    {
        public Pbck3Dto()
        {
            Pbck3Status = Enums.DocumentStatus.Draft;
            Back3Dto = new Back3Dto();
            Ck2Dto = new Ck2Dto();
           
        }

        public string Pbck3Number { get; set; }
        public string Pbck7Number { get; set; }
       
        public int Pbck7Id { get; set; }
        public int Pbck3Id { get; set; }
        public Enums.DocumentStatus Pbck3Status { get; set; }
        public Enums.DocumentStatusGov? Pbck3GovStatus { get; set; }

        public string ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string ApprovedByManager { get; set; }
        public DateTime? ApprovedDateManager { get; set; }

        public string RejectedBy { get; set; }
        public DateTime? RejectedDate { get; set; }

        public string Comment { get; set; }

        public bool IsRejected { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreateDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public DateTime Pbck3Date { get; set; }
        public Back3Dto Back3Dto { get; set; }
        public Ck2Dto Ck2Dto { get; set; }

        public string NppbckId { get; set; }

        public string Plant { get; set; }

        
       
    }

    public class Pbck7AndPbck3Dto
    {
        public Pbck7AndPbck3Dto()
        {
            Pbck3Dto = new Pbck3Dto();

            Back1Dto = new Back1Dto();
            
        }
        public Pbck3Dto Pbck3Dto { get; set; }
        public int Pbck7Id { get; set; }
        public string Pbck7Number { get; set; }
        public Enums.DocumentStatus Pbck7Status { get; set; }
        public DateTime Pbck7Date { get; set; }
        public string Lampiran { get; set; }
        public Enums.DocumentTypePbck7AndPbck3 DocumentType { get; set; }
        public string NppbkcId { get; set; }
        public string PlantId { get; set; }
        public string PlantName { get; set; }
        public string PlantCity { get; set; }
        public DateTime? ExecDateFrom { get; set; }
        public DateTime? ExecDateTo { get; set; }
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

       
        public List<Pbck7ItemUpload> UploadItems { get; set; }

        public string Comment { get; set; }

        public bool IsRejected { get; set; }

        public bool IsCreatedPbck7 { get; set; }

        public bool IsCreatedPbkc3 { get; set; }

        public Enums.DocumentStatusGov? Pbck7GovStatus { get; set; }


      
        public Enums.DocumentStatusGov? Back1GovStatus { get; set; }
        public Enums.DocumentStatusGov Back1GovStatusList { get; set; }

        public Back1Dto Back1Dto { get; set; }

        public Back3Dto Back3Dto { get; set; }
        public Ck2Dto Ck2Dto { get; set; }

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

        public string SeriesValue { get; set; }

        public int? Pbck7Id { get; set; }

        public string Message { get; set; }

    }

    public class Back3Dto
    {
        public int Back3ID { get; set; }
        public string Back3Number { get; set; }
        public Nullable<System.DateTime> Back3Date { get; set; }
        public int Pbck3ID { get; set; }

        public List<BACK3_DOCUMENT> Back3Document { get; set; }
      
    }

    public class Ck2Dto
    {
        public int Ck2ID { get; set; }
        public string Ck2Number { get; set; }
        public Nullable<System.DateTime> Ck2Date { get; set; }
        public Nullable<decimal> Ck2Value { get; set; }
        public int Pbck3ID { get; set; }

        public List<CK2_DOCUMENT> Ck2Document { get; set; }
        
    }
}
