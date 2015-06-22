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

        
        public List<T1001> Items
        {
            get
            {
                var xmlItems = _xmlMapper.GetElements("ITEM");
                var items = new List<T1001>();
                foreach (var xElement in xmlItems)
                {
                    var item = new T1001();
                    item.BUKRS = xElement.Element("BUKRS").Value;
                    item.BUKRSTXT = xElement.Element("BUTXT").Value;
                    item.CREATED_DATE = DateTime.Now;
                    var companyDateXml = DateTime.MinValue;
                    var modifiedDateXml =  xElement.Element("MODIFIED_DATE").Value;
                    DateTime.TryParse(xElement.Element("MODIFIED_DATE").Value, out companyDateXml);
                    var exisitingCompany = _xmlMapper.uow.GetGenericRepository<T1001>()
                           .Get(p => p.BUKRS == item.BUKRS)
                           .OrderByDescending(p => p.CREATED_DATE)
                           .FirstOrDefault();

                    if (exisitingCompany != null)
                    {
                        if (companyDateXml > exisitingCompany.CREATED_DATE)
                        {
                            items.Add(item);
                        }
                        else
                        {
                            continue;

                        }
                    }
                    else
                    {
                        items.Add(item);
                    }
                   
                }
                return items;
            }
             
        }

      
        public void InsertToDatabase()
        {
          _xmlMapper.InsertToDatabase<T1001>(Items);
        }

        public T1001 GetCompany(string CompanyCode)
        {
            var exisitingCompany = _xmlMapper.uow.GetGenericRepository<T1001>()
                          .Get(p => p.BUKRS == CompanyCode)
                          .OrderByDescending(p => p.CREATED_DATE)
                          .FirstOrDefault();
            return exisitingCompany;
        }



    }
}
