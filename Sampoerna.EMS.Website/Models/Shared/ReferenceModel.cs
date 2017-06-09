using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sampoerna.EMS.Website.Models.Shared
{
    public class ReferenceModel : BaseModel
    {
        public long Id { set; get; }
        public string Name { set; get; }
        public string Key { set; get; }
        public string TypeID { set; get; }
        public string Value { set; get; }
        public string CreatedBy { set; get; }
        public DateTime CreatedDate { set; get; }
        public string LastModifiedBy { set; get; }
        public DateTime? LastModifiedDate { set; get; }
        public bool IsActive { set; get; }

        #region Navigation Properties
        public ReferenceTypeModel Type { set; get; }
        #endregion
    }
}