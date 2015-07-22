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

        public XmlMarketDataMapper(string filename)
        {
            _xmlMapper = new XmlDataMapper(filename);
           
        }


        public List<ZAIDM_EX_MARKET> Items
        {
         get
            {
                var xmlRoot = _xmlMapper.GetElement("IDOC");
                var xmlItems = xmlRoot.Elements("Z1A_MARKET");
                var items = new List<ZAIDM_EX_MARKET>();
                foreach (var xElement in xmlItems)
                {
                    var item = new ZAIDM_EX_MARKET();
                    item.MARKET_ID = xElement.Element("MARKET").Value;
                    item.MARKET_DESC = xElement.Element("MARKET_DESC").Value;
                    var exisitingMarket = GetMarket(item.MARKET_ID);
                    //var marketDateXml = Convert.ToDateTime(xElement.Element("MODIFIED_DATE").Value); 
                    if (exisitingMarket != null)
                    {
                       item.CREATED_DATE = exisitingMarket.CREATED_DATE;
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
            
            
            _xmlMapper.InsertToDatabase<ZAIDM_EX_MARKET>(Items);
       
        }

        public ZAIDM_EX_MARKET GetMarket(string MarketId)
        {
            var exisitingPlant = _xmlMapper.uow.GetGenericRepository<ZAIDM_EX_MARKET>()
                .GetByID(MarketId);
            return exisitingPlant;
        }



    }
}
