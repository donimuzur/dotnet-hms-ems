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
       

        public List<C1LFA1> Items
        {
         get
            {
                var xmlItems = _xmlMapper.GetElements("ITEM");
                var items = new List<C1LFA1>();
                foreach (var xElement in xmlItems)
                {
                    var item = new C1LFA1();
                    var vendorCodeXml = xElement.Element("LIFNR").Value;

                    var exsitingVendor = GetExVendor(vendorCodeXml);
                    var dateXml = DateTime.MinValue;
                    DateTime.TryParse(xElement.Element("MODIFIED_DATE").Value, out dateXml);
                    item.LIFNR = vendorCodeXml;
                    item.NAME1 = xElement.Element("NAME1").Value;
                    item.NAME2 = xElement.Element("NAME2").Value;
                    item.CREATED_DATE = DateTime.Now;
                    if (exsitingVendor != null)
                    {
                        if (dateXml > exsitingVendor.CREATED_DATE)
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
            _xmlMapper.InsertToDatabase<C1LFA1>(Items);
       
        }

        public C1LFA1 GetExVendor(string vendorCode)
        {
            var exisitingPoa = _xmlMapper.uow.GetGenericRepository<C1LFA1>()
                            .Get(p => p.LIFNR == vendorCode)
                            .OrderByDescending(p => p.CREATED_DATE)
                            .FirstOrDefault();
            return exisitingPoa;
        }



    }
}
