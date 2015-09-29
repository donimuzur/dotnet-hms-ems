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
    public class XmlT001KDataMapper : IXmlDataReader 
    {
        private XmlDataMapper _xmlMapper = null;

        public XmlT001KDataMapper(string fileName)
        {
            _xmlMapper = new XmlDataMapper(fileName);
           
        }

        
        public List<T001K> Items
        {
            get
            {
                var xmlRoot = _xmlMapper.GetElement("IDOC");
                var xmlItems = xmlRoot.Elements("Z1AXX_T001K");
                var items = new List<T001K>();
                foreach (var xElement in xmlItems)
                {
                    try
                    {
                        var item = new T001K();
                        var bwkey = xElement.Element("BWKEY").Value;
                        item.BUKRS = _xmlMapper.GetElementValue(xElement.Element("BUKRS"));
                        item.CREATED_BY = Constans.PI;
                        var existingData = _xmlMapper.uow.GetGenericRepository<T001K>()
                            .GetByID(bwkey);
                        item.BWKEY = bwkey;
                        item.CREATED_BY = Constans.PI;
                        if (existingData != null)
                        {
                            item.CREATED_BY = existingData.CREATED_BY;
                            item.CREATED_DATE = existingData.CREATED_DATE;
                            item.MODIFIED_DATE = DateTime.Now;
                            item.MODIFIED_BY = Constans.PI;
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
           return _xmlMapper.InsertToDatabase<T001K>(Items);
        }

        public List<string> GetErrorList()
        {
            return _xmlMapper.Errors;
        }

        public T001K GetT001K(string id)
        {
            var exisitingCompany = _xmlMapper.uow.GetGenericRepository<T001K>()
                .GetByID(id);
            return exisitingCompany;
        }



    }
}
