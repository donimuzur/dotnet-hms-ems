using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Sampoerna.EMS.Website.Models.Shared
{
    public class WorkflowHistory : BaseModel
    {
        [Required, Key, DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public long Id { set; get; }
        [Required]
        public int FormTypeID { set; get; }
        [Required]
        public long FormID { set; get; }
        public string FormNumber { set; get; }
        [Required]
        public int Action { set; get; }
        [Required]
        public string ActionBy { set; get; }
        public UserModel ActionUser { set; get; }
        public DateTime? ActionDate { set; get; }
        public string Comment { set; get; }
        [Required]
        public int Role { set; get; }




    }
}