using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Sampoerna.EMS.BusinessObject;

namespace Sampoerna.EMS.Website.Models
{
    public class CompanyViewModel 
    {
        
        public long CompanyId { get; set; }

        [ReadOnly(true)]
        public string DocumentBukrs { get; set; }

        public string DocumentBukrstxt { get; set; }
        
        [Display(Name = "Create Date")]
        [DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]    
        public Nullable<System.DateTime> CreatedDate { get; set; }

        public virtual ICollection<T1001K> T1001K { get; set; }
       
    }
}