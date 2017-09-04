using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Sampoerna.EMS.Website.Models.Shared
{
    public class ChangesLogModel : BaseModel
    {
        [Required, Key, DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public long Id { set; get; }
        [Required]
        public int FormTypeID { set; get; }
        public Enum FormType { set; get; }
        public string FormID { set; get; }
        public string FieldName { set; get; }
        public string OldValue { set; get; }
        public string NewValue { set; get; }
        public string ModifiedBy { set; get; }
        public DateTime? ModifiedDate { set; get; }
        public UserModel Changer { set; get; }

    }
}