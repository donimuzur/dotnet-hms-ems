using System;
using System.Collections.Generic;
using System.Linq;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.DAL;
using Voxteneo.WebComponents.Logger;
namespace Sampoerna.EMS.XMLReader
{
    public class XmlUoMDataMapper : IXmlDataReader 
    {
        private XmlDataMapper _xmlMapper = null;
      
        public XmlUoMDataMapper(string fileName)
        {
            _xmlMapper = new XmlDataMapper(fileName);
           
        }

        
        public List<UOM> Items
        {
            get
            {
                var xmlRoot = _xmlMapper.GetElement("IDOC");
                var xmlItems = xmlRoot.Elements("Z1AXX_T006A");
                var items = new List<UOM>();
                foreach (var xElement in xmlItems)
                {
                    var item = new UOM();
                    item.UOM_ID = xElement.Element("MSEHI").Value;
                    item.UOM_DESC = xElement.Element("MSEHL").Value;
                    var existingData = GetUoM(item.UOM_ID);
                    if (existingData != null)
                    {
                        item.CREATED_DATE = existingData.CREATED_DATE;
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

      
        public void InsertToDatabase()
        {
          _xmlMapper.InsertToDatabase<UOM>(Items);
        }

        public UOM GetUoM(string uomId)
        {
            var existingData = _xmlMapper.uow.GetGenericRepository<UOM>()
                .GetByID(uomId);
            return existingData;
        }



    }
}
