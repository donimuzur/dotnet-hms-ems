using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sampoerna.EMS.BusinessObject.Business
{
    public class InvalidBrandByCk1ForEmail
    {

        public InvalidBrandByCk1ForEmail()
        {
            BrandList = new List<InvalidCk1Brand>();
            userTo = new List<USER>();
            userCc = new List<USER>();
        }

        public string BrandName { get; set; }

        public List<InvalidCk1Brand> BrandList { get; set; }
        

        public List<USER> userTo { get; set; }

        public List<USER> userCc { get; set; }
    }


    public class InvalidCk1Brand
    {
        public string FaCode { get; set; }
        public string Werks { get; set; }
        public string BrandCe { get; set; }
        public string Hje { get; set; }
        public string Tariff { get; set; }

        public CK1 LastCk1 { get; set; }
    }

    public class InvalidBrandByCk5ForEmail
    {

        public InvalidBrandByCk5ForEmail()
        {
            BrandList = new List<InvalidCk5Brand>();
            userTo = new List<USER>();
            userCc = new List<USER>();
        }

        public string BrandName { get; set; }

        public List<InvalidCk5Brand> BrandList { get; set; }


        public List<USER> userTo { get; set; }

        public List<USER> userCc { get; set; }
    }


    public class InvalidCk5Brand
    {
        public string FaCode { get; set; }
        public string Werks { get; set; }
        public string BrandCe { get; set; }
        public string Hje { get; set; }
        public string Tariff { get; set; }

        public CK5 LastCk5 { get; set; }
    }
}
