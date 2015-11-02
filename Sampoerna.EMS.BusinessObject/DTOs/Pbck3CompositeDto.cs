using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.BusinessObject.DTOs
{
   public class Pbck3CompositeDto
    {
       public Pbck3CompositeDto()
       {
           Pbck7Composite = new Pbck3Pbck7DtoComposite();
           Ck5Composite = new Pbck3Ck5DtoComposite();
       }

        public int PBCK3_ID { get; set; }
        public string PBCK3_NUMBER { get; set; }
        public System.DateTime PBCK3_DATE { get; set; }
        public Nullable<int> PBCK7_ID { get; set; }
        public Nullable<Sampoerna.EMS.Core.Enums.DocumentStatusGovType3> GOV_STATUS { get; set; }
        public Nullable<Sampoerna.EMS.Core.Enums.DocumentStatus> STATUS { get; set; }
        public string APPROVED_BY { get; set; }
        public Nullable<System.DateTime> APPROVED_DATE { get; set; }
        public string CREATED_BY { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public string MODIFIED_BY { get; set; }
        public Nullable<System.DateTime> MODIFIED_DATE { get; set; }
        public string APPROVED_BY_MANAGER { get; set; }
        public Nullable<System.DateTime> APPROVED_BY_MANAGER_DATE { get; set; }
        public string REJECTED_BY { get; set; }
        public Nullable<System.DateTime> REJECTED_DATE { get; set; }
        public Nullable<long> CK5_ID { get; set; }
        public DateTime EXEC_DATE_FROM { get; set; }
        public DateTime EXEC_DATE_TO { get; set; }

       //pbck7
        public Pbck3Pbck7DtoComposite Pbck7Composite { get; set; } 


        //back1
        public int Back1Id { get; set; }
        public string Back1Number { get; set; }
        public DateTime? Back1Date { get; set; }
        public List<BACK1_DOCUMENT> Back1Documents { get; set; }

        //ck2
        public int Ck2Id { get; set; }
        public string Ck2Number { get; set; }
        public DateTime? Ck2Date { get; set; }
        public decimal? Ck2Value { get; set; }
        public List<CK2_DOCUMENT> Ck2Documents { get; set; }

        //back3
        public int Back3Id { get; set; }
        public string Back3Number { get; set; }
        public DateTime? Back3Date { get; set; }
        public List<BACK3_DOCUMENT> Back3Documents { get; set; }
       
       //general
        public bool FromPbck7 { get; set; }

       //from ck5 market return
        public Pbck3Ck5DtoComposite Ck5Composite { get; set; } 

    }

    public class Pbck3Pbck7DtoComposite
    {
        public int Pbck7Id { get; set; }
        public string Pbck7Number { get; set; }
        public DateTime Pbck7Date { get; set; }

        public Enums.DocumentStatus Pbck7Status { get; set; }
        public string Pbck7StatusDescription { get; set; }
        public Enums.DocumentStatusGov Pbck7GovStatus { get; set; }
        public string Pbck7GovStatusDescription { get; set; }

        public Enums.DocumentTypePbck7AndPbck3 DocumentType { get; set; }
        public string DocumentTypeDescription { get; set; }

        public DateTime? ExecDateFrom { get; set; }
        public DateTime? ExecDateTo { get; set; }
        public string NppbkcId { get; set; }
        public string Lampiran { get; set; }
        public string PlantId { get; set; }
        public string PoaList { get; set; }

        public List<PBCK7_ITEM> Pbck7Documents { get; set; }
    }

    public class Pbck3Ck5DtoComposite
    {
        public Pbck3Ck5DtoComposite()
        {
            ListWorkflowHistorys = new List<WorkflowHistoryDto>();
        }

        public CK5Dto Ck5Dto { get; set; }
        
        public List<WorkflowHistoryDto> ListWorkflowHistorys { get; set; }
        public List<CK5MaterialDto> Ck5MaterialDto { get; set; }
        
    }
}
