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
using Sampoerna.EMS.Core;
using Sampoerna.EMS.DAL;
using Voxteneo.WebComponents.Logger;
namespace Sampoerna.EMS.XMLReader
{
    public class XmlBlockStockDataMapper : IXmlDataReader 
    {
        private XmlDataMapper _xmlMapper = null;

        public XmlBlockStockDataMapper(string filename)
        {
            _xmlMapper = new XmlDataMapper(filename);
           
        }


        public List<BLOCK_STOCK> Items
        {
           get
            {
                var xmlRoot = _xmlMapper.GetElement("BLOCKSTOCKDetails");
                var xmlItems = xmlRoot.Elements("Stock");
                var items = new List<BLOCK_STOCK>();
                foreach (var xElement in xmlItems)
                {
                    try
                    {
                        var item = new BLOCK_STOCK();
                        item.PLANT = _xmlMapper.GetElementValue(xElement.Element("Plnt")); ;
                        item.MATERIAL_ID = _xmlMapper.GetElementValue(xElement.Element("Material"));
                        item.SLOC = _xmlMapper.GetElementValue(xElement.Element("SLoc"));

                        var existingdata = GetBlockStock(item.PLANT, item.MATERIAL_ID, item.SLOC);

                        if (existingdata != null)
                        {
                            item.BLOCK_STOCK_ID = existingdata.BLOCK_STOCK_ID;
                        }

                        item.BLOCKED = Convert.ToDecimal(_xmlMapper.GetElementValue(xElement.Element("Blocked")));
                        item.BUN = _xmlMapper.GetElementValue(xElement.Element("BUn"));
                        items.Add(item);
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
            
            
            return _xmlMapper.InsertToDatabase<BLOCK_STOCK>(Items);
       
        }

        public List<string> GetErrorList()
        {
            return _xmlMapper.Errors;
        }


        public BLOCK_STOCK GetBlockStock(string plantid, string materialid, string sloc)
        {
            var existingData =
                _xmlMapper.uow.GetGenericRepository<BLOCK_STOCK>()
                    .Get(x => x.MATERIAL_ID == materialid && x.PLANT == plantid && x.SLOC == sloc)
                    .FirstOrDefault();
            return existingData;
        }

    




    }
}
