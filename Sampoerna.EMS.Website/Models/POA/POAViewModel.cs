using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Sampoerna.EMS.BusinessObject;

namespace Sampoerna.EMS.Website.Models.POA
{
    public class POAViewModel : BaseModel
    {
        public POAViewModel()
        {
            Details = new List<ZAIDM_EX_POA>();
        }
        public List<ZAIDM_EX_POA> Details { get; set; }

        public int PoaId { get; set; }

        [Display(Name = "ID Card")]
        public string PoaIdCard { get; set; }

        [Display(Name = "Name")]
        public string PoaCode { get; set; }

        [Display(Name = "Printed Name")]
        public string PoaPrintedName { get; set; }

        [Display(Name = "Address")]
        public string PoaAddress { get; set; }

        [Display(Name = "Phone")]
        public string PoaPhone { get; set; }

        [Display(Name = "Title")]
        public string Title { get; set; }

        public bool isNewData { get; set; }
    }
    
}