using System;
using System.Collections.Generic;
using System.Configuration;
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
    public class XmlMaterialDataMapper : IXmlDataReader 
    {
        private XmlDataMapper _xmlMapper = null;

        public XmlMaterialDataMapper(string fileName)
        {
            _xmlMapper = new XmlDataMapper(fileName);

        }

       
        public List<ZAIDM_EX_MATERIAL> Items
        {
            get
            {
                var xmlItems = _xmlMapper.GetElements("ITEM");
                var items = new List<ZAIDM_EX_MATERIAL>();
                foreach (var xElement in xmlItems)
                {
                    var item = new ZAIDM_EX_MATERIAL();
                    item.STICKER_CODE = xElement.Element("STICKER_CODE").Value;
                    item.MATERIAL_DESC = xElement.Element("MATERIAL_DESC").Value;
                    item.MATERIAL_GROUP = xElement.Element("MATERIAL_GROUP").Value;
                    item.PURCHASING_GROUP = xElement.Element("PURCHASING_GROUP").Value;
                    item.ISSUE_STORANGE_LOC = xElement.Element("ISSUE_STORANGE_LOC").Value;
                    item.WERKS = xElement.Element("PLANT_ID").Value;
                    var exGoodTypCode = Convert.ToInt32(xElement.Element("EX_GOOD_TYP").Value);
                    var exGoodType = new XmlGoodsTypeDataMapper(null).GetGoodsType(exGoodTypCode);
                    if(exGoodType == null)
                    {
                        throw new Exception(string.Format("There no data GoodType macthing with  {0} ", exGoodTypCode));

                    }
                    item.EXC_GOOD_TYP = exGoodType.EXC_GOOD_TYP;
                    var baseUomId =Convert.ToInt32(xElement.Element("BASE_UOM").Value);
                    var baseUoM = new XmlUoMDataMapper(null).GetExUoM(baseUomId);
                    if (baseUoM == null)
                    {
                        throw new Exception(string.Format("There no data UoM macthing with  {0} ", baseUomId));

                    }
                    item.BASE_UOM_ID = baseUoM.UOM_ID;
                    item.CONVERSION = Convert.ToDecimal(xElement.Element("CONVERSION").Value);
                    
                    var dateXml = Convert.ToDateTime(xElement.Element("MODIFIED_DATE").Value);
                    var existingMaterial = GetMaterial(item.STICKER_CODE);
                    if (existingMaterial != null)
                    {
                        item.CREATED_DATE = existingMaterial.CREATED_DATE;
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
            
            _xmlMapper.InsertToDatabase<ZAIDM_EX_MATERIAL>(Items);
            

        }
        public ZAIDM_EX_MATERIAL GetMaterial(string materialNumber)
        {
            var existingData = _xmlMapper.uow.GetGenericRepository<ZAIDM_EX_MATERIAL>()
                .GetByID(materialNumber);
            return existingData;
        }
        



    }
}
