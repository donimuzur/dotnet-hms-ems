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
    public class XmlMovementDataMapper : IXmlDataReader 
    {
        private XmlDataMapper _xmlMapper = null;

        public XmlMovementDataMapper(string filename)
        {
            _xmlMapper = new XmlDataMapper(filename);
           
        }


        public List<INVENTORY_MOVEMENT> Items
        {
           get
            {
                var xmlRoot = _xmlMapper.GetElement("InvMovementDetails");
                var xmlItems = xmlRoot.Elements("Movement");
                var items = new List<INVENTORY_MOVEMENT>();
                foreach (var xElement in xmlItems)
                {
                    try
                    {
                        var item = new INVENTORY_MOVEMENT();
                        item.MVT = xElement.Element("MvT").Value;
                        item.MATERIAL_ID = _xmlMapper.GetElementValue(xElement.Element("Material"));
                        //item.CREATED_BY = Constans.PI;
                        //var exisitingMarket = GetMarket(item.MARKET_ID);
                        //if (exisitingMarket != null)
                        //{
                        //    item.CREATED_BY = exisitingMarket.CREATED_BY;
                        //    item.CREATED_DATE = exisitingMarket.CREATED_DATE;
                        //    item.MODIFIED_DATE = DateTime.Now;
                        //    item.MODIFIED_BY = Constans.PI;
                        //    items.Add(item);

                        //}
                        //else
                        //{
                        //    item.CREATED_DATE = DateTime.Now;
                        //    items.Add(item);
                        //}
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
            
            
            return _xmlMapper.InsertToDatabase<INVENTORY_MOVEMENT>(Items);
       
        }

        public List<string> GetErrorList()
        {
            return _xmlMapper.Errors;
        }




    }
}
