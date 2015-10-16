using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Validations;
using Sampoerna.EMS.Website.Models.CK5;

namespace Sampoerna.EMS.Website.Models.PBCK1
{
    public class PBCK1ListCK5Model : BaseModel
    {
        public Pbck1Item Detail { get; set; }

        public List<CK5Item> CK5List { get; set; }
    }
}