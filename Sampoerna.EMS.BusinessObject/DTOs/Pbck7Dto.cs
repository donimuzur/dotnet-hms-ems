using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sampoerna.EMS.BusinessObject.DTOs
{
    public class Pbck7Dto
    {
        public int Pbck3Pbck7Id { get; set; }
        public string Pbck7Number { get; set; }
        public string Pbck3Number { get; set; }
        public int Pbck7Status { get; set; }
        public int Pbck3Status { get; set; }
        public DateTime Pbck7Date { get; set; }
        public DateTime? Pbck3Date { get; set; }
        public int DocumnetType { get; set; }
        public string NppbkcId { get; set; }
        public string PlantId { get; set; }
        public string PlantName { get; set; }
        public string PlantCity { get; set; }
        public DateTime? ExecDateFrom { get; set; }
        public DateTime? ExecDateTo { get; set; }
        public int GovStatus { get; set; }
        public int Status { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime ApprovedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreateDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }

        //from table BACK1
        public int PBCK3PBCK7Id { get; set; }
        public string Back1Number { get; set; }
        public DateTime Back1Date { get; set; }


        
    }
}
