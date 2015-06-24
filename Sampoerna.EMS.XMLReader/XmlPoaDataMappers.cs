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
    public class XmlPoaDataMapper : IXmlDataReader 
    {
        private XmlDataMapper _xmlMapper = null;
       
       
        public XmlPoaDataMapper(string fileName)
        {
           
             _xmlMapper = new XmlDataMapper(fileName);
            
        }
       

        public List<ZAIDM_EX_POA> Items
        {
         get
            {
                var xmlItems = _xmlMapper.GetElements("ITEM");
                var items = new List<ZAIDM_EX_POA>();
                foreach (var xElement in xmlItems)
                {
                    var item = new ZAIDM_EX_POA();
                    var poaCodeXml = xElement.Element("POA_ID").Value;
                    var exisitingPoa = GetExPoa(poaCodeXml);
                    var podDateXml = Convert.ToDateTime(xElement.Element("MODIFIED_DATE").Value); 
                    item.POA_CODE = poaCodeXml;
                    item.POA_ID_CARD = xElement.Element("POA_ID_CARD").Value;
                    item.POA_PRINTED_NAME = xElement.Element("POA_PRINTED_NAME").Value;
                    item.POA_PHONE = xElement.Element("POA_PHONE").Value;
                    item.POA_ADDRESS = xElement.Element("POA_ADDRESS").Value;
                    item.CREATED_DATE = DateTime.Now;
                    if (exisitingPoa != null)
                    {
                        if (podDateXml > exisitingPoa.CREATED_DATE)
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
           _xmlMapper.InsertToDatabase<ZAIDM_EX_POA>(Items);
       
        }

        public ZAIDM_EX_POA GetExPoa(string PoaCode)
        {
            var exisitingPoa = _xmlMapper.uow.GetGenericRepository<ZAIDM_EX_POA>()
                            .Get(p => p.POA_CODE == PoaCode)
                            .OrderByDescending(p => p.CREATED_DATE)
                            .FirstOrDefault();
            return exisitingPoa;
        }



    }
}
