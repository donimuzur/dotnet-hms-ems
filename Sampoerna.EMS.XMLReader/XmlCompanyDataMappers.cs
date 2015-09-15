using System;
using System.Collections.Generic;
using System.Linq;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
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
                    try
                    {
                        var item = new T001();
                        var bukrs = xElement.Element("BUKRS").Value;
                        item.BUTXT = _xmlMapper.GetElementValue(xElement.Element("BUTXT"));
                        item.ORT01 = _xmlMapper.GetElementValue(xElement.Element("ORT01"));
                        //item.SPRAS = _xmlMapper.GetElementValue(xElement.Element("SPRAS"));
                       // item.NPWP = _xmlMapper.GetElementValue(xElement.Element("STCEG"));
                        item.CREATED_BY = Constans.PI;
                        var exisitingCompany = _xmlMapper.uow.GetGenericRepository<T001>()
                            .GetByID(bukrs);
                        item.BUKRS = bukrs;
                        if (exisitingCompany != null)
                        {
                            item.CREATED_BY = exisitingCompany.CREATED_BY;
                            item.CREATED_DATE = exisitingCompany.CREATED_DATE;
                            item.MODIFIED_DATE = DateTime.Now;
                            item.MODIFIED_BY = Constans.PI;
                            item.BUTXT_ALIAS = exisitingCompany.BUTXT_ALIAS;
                            items.Add(item);

                        }
                        else
                        {
                            item.CREATED_DATE = DateTime.Now;
                            items.Add(item);
                        }
                    }
                    catch (Exception ex)
                    {
                        _xmlMapper.Errors.Add(ex.Message);
                        continue;
                        

                    }
                    
                   
                }
                return items;
            }
             
        }

      
        public string InsertToDatabase()
        {
           return _xmlMapper.InsertToDatabase<T001>(Items);
        }

        public List<string> GetErrorList()
        {
            return _xmlMapper.Errors;
        }

        public T001 GetCompany(string CompanyCode)
        {
            var exisitingCompany = _xmlMapper.uow.GetGenericRepository<T001>()
                .GetByID(CompanyCode);
            return exisitingCompany;
        }



    }
}
