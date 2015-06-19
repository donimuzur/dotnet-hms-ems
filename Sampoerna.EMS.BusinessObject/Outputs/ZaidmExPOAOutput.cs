using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sampoerna.EMS.BusinessObject.Outputs
{
    public class ZaidmExPOAOutput : BLLBaseOutput
    {
        //ZaidmExPOA
        public int  PoaId { get; set; }
        public string PoaCode { get; set; }
        public string PoaIdCard { get; set; }
        public string PoaAddress { get; set; }
        public string PoaPhone { get; set; }
        public string PoaPrintedName { get; set; }
        public string Title { get; set; }
        public int? UserIdZaidmExPoa { get; set; }
        public DateTime? CreatedDate { get; set; }
      

        //User
        public int UserIdUser { get; set; }
        public string UserName { get; set; }
        public int? ManagerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool? IsActive { get; set; }
        public int? UserGruopId { get; set; }
        public string Email { get; set; }
        public DateTime? CreateDateTime { get; set; }
                
    }
}
