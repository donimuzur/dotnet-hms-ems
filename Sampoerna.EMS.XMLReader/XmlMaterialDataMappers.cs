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
                    item.MATERIAL_NUMBER = xElement.Element("MATERIAL_NUMBER").Value;
                    item.MATERIAL_GROUP = xElement.Element("MATERIAL_GROUP").Value;
                    item.PURCHASING_GROUP = xElement.Element("PURCHASING_GROUP").Value;
                    item.ISSUE_STORANGE_LOC = xElement.Element("ISSUE_STORANGE_LOC").Value;
                    var brandfaCode = xElement.Element("BRAND_FA_CODE").Value;
                    var brandPlant = xElement.Element("BRAND_PLANT_ID").Value;
                    var plant = new XmlPlantDataMapper(null).GetPlant(brandPlant);
                    if(plant == null)
                        throw new Exception(string.Format("There no data plant macthing with  {0} ", brandPlant));

                    var brand = new XmlBrandDataMapper(null).GetBrand(plant.PLANT_ID, brandfaCode);
                    if(brand == null)
                        throw  new Exception(string.Format("There no data brand macthing with plant {0} " +
                                                           " fa code {1}", brandPlant, brandfaCode));
                    item.BRAND_ID = brand.BRAND_ID;
                    var exGoodTypCode = Convert.ToInt32(xElement.Element("EX_GOODTYP").Value);
                    var exGoodType = new XmlGoodsTypeDataMapper(null).GetGoodsType(exGoodTypCode);
                    if(exGoodType == null)
                    {
                        throw new Exception(string.Format("There no data GoodType macthing with  {0} ", exGoodTypCode));

                    }
                    item.EX_GOODTYP = exGoodType.GOODTYPE_ID;
                    var baseUomId = xElement.Element("BASE_UOM").Value;
                    var baseUoM = new XmlUoMDataMapper(null).GetExUoM(baseUomId);
                    if (baseUoM == null)
                    {
                        throw new Exception(string.Format("There no data UoM macthing with  {0} ", baseUomId));

                    }
                    item.BASE_UOM = baseUoM.UOM_ID;
                    item.CONVERSION = Convert.ToDecimal(xElement.Element("CONVERSION").Value);
                    item.CREATED_DATE = DateTime.Now;

                    var dateXml = Convert.ToDateTime(xElement.Element("MODIFIED_DATE").Value);
                    var existingMaterial = GetMaterial(item.MATERIAL_NUMBER);
                    if (existingMaterial != null)
                    {
                        if (dateXml > existingMaterial.CREATED_DATE)
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
            
            _xmlMapper.InsertToDatabase<ZAIDM_EX_MATERIAL>(Items);
            

        }
        public ZAIDM_EX_MATERIAL GetMaterial(string materialNumber)
        {
            var existingData = _xmlMapper.uow.GetGenericRepository<ZAIDM_EX_MATERIAL>()
                          .Get(p => p.MATERIAL_NUMBER == materialNumber)
                          .OrderByDescending(p => p.CREATED_DATE)
                          .FirstOrDefault();
            return existingData;
        }
        



    }
}
