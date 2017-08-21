using Sampoerna.EMS.Website.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sampoerna.EMS.Website.Models.ChangeRequest
{
    public class ChangeRequestNppbkc
    {
        public ChangeRequestNppbkc()
        {
            this.Company = new CompanyModel();
            this.Plants = new List<ManufacturePlant>();
        }
        public string Id { set; get; }
        public string KppbcId { set; get; }
        public string City { set; get; }
        public string Region { set; get; }
        public string Address { set; get; }
        public CompanyModel Company { set; get; }

        public string CompanyAlias { set; get; }
        public string CityAlias { set; get; }

        public List<ManufacturePlant> Plants { get; set; }
    }

    public class ManufacturePlant
    {
        public ManufacturePlant()
        {

        }

        public string WERKS { get; set; }
        public string PlantName { get; set; }
        public string PlantCity { get; set; }
        public string PlantPhone { get; set; }
        public string PlantAddress { get; set; }
        public string NPPBKCId { get; set; }
        public bool IsMainPlant { get; set; }
        public string NPPBKCIdImport { get; set; }


    }
}