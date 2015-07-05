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
                    var uomCodeXml = Convert.ToInt32(xElement.Element("UOM_ID").Value);
                    var desc = xElement.Element("UOM_DESC").Value;

                    var existingUom = GetExUoM(uomCodeXml);
                    var dateXml = Convert.ToDateTime(xElement.Element("MODIFIED_DATE").Value);
                    item.UOM_ID = uomCodeXml;
                    item.UOM_DESC = desc;
                    item.CREATED_DATE = DateTime.Now;
                    if (existingUom != null)
                    {
                        item.CREATED_DATE = existingUom.CREATED_DATE;
                        item.MODIFIED_DATE = dateXml;
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

        public UOM GetExUoM(int code)
        {
            var existingUom = _xmlMapper.uow.GetGenericRepository<UOM>()
                .GetByID(code);
            return existingUom;
        }
        


    }
}
