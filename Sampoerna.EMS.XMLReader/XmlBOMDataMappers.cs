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
    public class XmlBOMDataMapper : IXmlDataReader 
    {
        private XmlDataMapper _xmlMapper = null;

        public XmlBOMDataMapper(string filename)
        {
            _xmlMapper = new XmlDataMapper(filename);
           
        }


        public List<BOM> Items
        {
           get
            {
                var xmlRoot = _xmlMapper.GetElement("BOMDetails");
                var xmlItems = xmlRoot.Elements("BOM");
                var items = new List<BOM>();
                foreach (var xElement in xmlItems)
                {
                    try
                    {
                        var item = new BOM();
                        item.PLANT = _xmlMapper.GetElementValue(xElement.Element("Plnt")); ;
                        item.MATERIAL_ID = _xmlMapper.GetElementValue(xElement.Element("BaseMaterial"));
                        item.MATERIAL_DESC = _xmlMapper.GetElementValue(xElement.Element("BaseMaterialDescription"));
                        item.LEVEL1 = _xmlMapper.GetElementValue(xElement.Element("Level1"));
                        item.LEVEL2 = _xmlMapper.GetElementValue(xElement.Element("Level2"));
                        item.LEVEL3 = _xmlMapper.GetElementValue(xElement.Element("Level3"));
                        item.LEVEL4 = _xmlMapper.GetElementValue(xElement.Element("Level4"));
                        item.LEVEL5 = _xmlMapper.GetElementValue(xElement.Element("Level5"));
                        item.LEVEL6 = _xmlMapper.GetElementValue(xElement.Element("Level6"));
                        item.LEVEL7 = _xmlMapper.GetElementValue(xElement.Element("Level7"));
                        item.LEVEL8 = _xmlMapper.GetElementValue(xElement.Element("Level8"));
                        item.LEVEL9 = _xmlMapper.GetElementValue(xElement.Element("Level9"));
                        item.LEVEL10 = _xmlMapper.GetElementValue(xElement.Element("Level10"));
                       
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
            
            
            return _xmlMapper.InsertToDatabase<BOM>(Items);
       
        }

        public List<string> GetErrorList()
        {
            return _xmlMapper.Errors;
        }
        //public INVENTORY_MOVEMENT GetPCode(string PCode)
        //{
        //    var exisitingPlant = _xmlMapper.uow.GetGenericRepository<ZAIDM_EX_PCODE>()
        //        .GetByID(PCode);
        //    return exisitingPlant;
        //}




    }
}
