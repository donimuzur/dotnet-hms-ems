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
    public class XmlMarketDataMapper : IXmlDataReader 
    {
        private XmlDataMapper _xmlMapper = null;

        public XmlMarketDataMapper()
        {
            _xmlMapper = new XmlDataMapper("ZAIDM_EX_MARKET");
           
        }


        public List<ZAIDM_EX_MARKET> Items
        {
         get
            {
                var xmlItems = _xmlMapper.GetElements("ITEM");
                var items = new List<ZAIDM_EX_MARKET>();
                foreach (var xElement in xmlItems)
                {
                    var item = new ZAIDM_EX_MARKET();
                    var marketCodeXml = Convert.ToInt32(xElement.Element("MARKET_CODE").Value);

                    var exisitingMarket = _xmlMapper.uow.GetGenericRepository<ZAIDM_EX_MARKET>()
                            .Get(p => p.MARKET_CODE == marketCodeXml)
                            .OrderByDescending(p => p.CREATED_DATE)
                            .FirstOrDefault();
                    var marketDateXml = DateTime.MinValue;
                    DateTime.TryParse(xElement.Element("MODIFIED_DATE").Value, out marketDateXml);
                    item.MARKET_CODE = marketCodeXml;
                    item.MARKET_DESC = xElement.Element("MARKET_DESC").Value;
                    item.CREATED_DATE = DateTime.Now;
                    if (exisitingMarket != null)
                    {
                        if (marketDateXml > exisitingMarket.CREATED_DATE)
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
            _xmlMapper.InsertToDatabase<ZAIDM_EX_MARKET>(Items);
       
        }





    }
}
