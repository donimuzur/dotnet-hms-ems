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
    public class XmlVendorDataMapper : IXmlDataReader 
    {
        private XmlDataMapper _xmlMapper = null;


        public XmlVendorDataMapper(string fileName)
        {
           
             _xmlMapper = new XmlDataMapper(fileName);
            
        }
       

        public List<LFA1> Items
        {
         get
            {
                var xmlRoot = _xmlMapper.GetElement("IDOC");
                var xmlItems = xmlRoot.Elements("Z1A_MARKET");
                var items = new List<LFA1>();
                foreach (var xElement in xmlItems)
                {
                    var item = new LFA1();
                    var vendorCodeXml = xElement.Element("LIFNR").Value;

                    var exsitingVendor = GetExVendor(vendorCodeXml);
                    var dateXml = Convert.ToDateTime(xElement.Element("MODIFIED_DATE").Value); 
                    item.LIFNR = vendorCodeXml;
                    item.NAME1 = xElement.Element("NAME1").Value;
                    item.NAME2 = xElement.Element("NAME2").Value;
                    
                    if (exsitingVendor != null)
                    {
                        item.CREATED_DATE = exsitingVendor.CREATED_DATE;
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
            _xmlMapper.InsertToDatabase<LFA1>(Items);
       
        }

        public LFA1 GetExVendor(string vendorCode)
        {
            var exisitingPoa = _xmlMapper.uow.GetGenericRepository<LFA1>()
                .GetByID(vendorCode);
            return exisitingPoa;
        }



    }
}
