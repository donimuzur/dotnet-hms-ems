using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Validations;

namespace Sampoerna.EMS.Website.Models.PBCK1
{
    public class Pbck1SupInfo
    {
        public string SupplierPlantWerks { get; set; }

        public string SupplierAddress { get; set; }

        public string SupplierNppkbc { get; set; }

        public string SupplierKppkbc { get; set; }

        public string SupplierPlantName { get; set; }
        
        public string SupplierPhone { get; set; }
    }
}