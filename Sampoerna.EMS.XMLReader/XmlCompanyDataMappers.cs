using System;
using System.Collections.Generic;
using System.Linq;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.DAL;
using Voxteneo.WebComponents.Logger;
namespace Sampoerna.EMS.XMLReader
{
    public class XmlCompanyDataMapper : IXmlDataReader 
    {
        private XmlDataMapper _xmlMapper = null;
      
        public XmlCompanyDataMapper(string fileName)
        {
            _xmlMapper = new XmlDataMapper(fileName);
           
        }

        
        public List<T001> Items
        {
            get
            {
                var xmlRoot = _xmlMapper.GetElement("IDOC");
                var xmlItems = xmlRoot.Elements("Z1AXX_T001");
                var items = new List<T001>();
                foreach (var xElement in xmlItems)
                {
                    var item = new T001();
                    var bukrs = xElement.Element("BUKRS").Value;
                    item.BUTXT = xElement.Element("BUTXT").Value;
                    item.ORT01 = xElement.Element("ORT01") == null ? null : xElement.Element("ORT01").Value;
                    item.SPRAS = xElement.Element("SPRAS") == null ? null : xElement.Element("SPRAS").Value;
                    //var companyDateXml = Convert.ToDateTime(xElement.Element("MODIFIED_DATE").Value);
                    var exisitingCompany = _xmlMapper.uow.GetGenericRepository<T001>()
                        .GetByID(bukrs);
                    item.BUKRS = bukrs;
                    if (exisitingCompany != null)
                    {
                        item.CREATED_DATE = exisitingCompany.CREATED_DATE;
                        item.MODIFIED_DATE = DateTime.Now;
                        items.Add(item);
                        
                    }
                    else
                    {
                        item.CREATED_DATE = DateTime.Now;
                        items.Add(item);
                    }
                   
                }
                return items;
            }
             
        }

      
        public string InsertToDatabase()
        {
           return _xmlMapper.InsertToDatabase<T001>(Items);
        }

        public T001 GetCompany(string CompanyCode)
        {
            var exisitingCompany = _xmlMapper.uow.GetGenericRepository<T001>()
                .GetByID(CompanyCode);
            return exisitingCompany;
        }



    }
}
