using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
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
                var xmlItems = _xmlMapper.GetElements("ITEM");
                var items = new List<UOM>();
                foreach (var xElement in xmlItems)
                {
                    var item = new UOM();
                    var uomCodeXml = xElement.Element("CODE").Value;

                    var existingUom = GetExUoM(uomCodeXml);
                    var dateXml = DateTime.MinValue;
                    DateTime.TryParse(xElement.Element("MODIFIED_DATE").Value, out dateXml);
                    item.UOM_NAME = uomCodeXml;
                    item.CREATED_DATE = DateTime.Now;
                    if (existingUom != null)
                    {
                        if (dateXml > existingUom.CREATED_DATE)
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
           _xmlMapper.InsertToDatabase<UOM>(Items);
       
        }

        public UOM GetExUoM(string code)
        {
            var existingUom = _xmlMapper.uow.GetGenericRepository<UOM>()
                            .Get(p => p.UOM_NAME == code)
                            .OrderByDescending(p => p.CREATED_DATE)
                            .FirstOrDefault();
            return existingUom;
        }



    }
}
