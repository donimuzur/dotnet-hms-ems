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
                    var pCode = new XmlPCodeDataMapper().GetPCode(Convert.ToInt32(xElement.Element("PER_CODE").Value));
                    if (pCode == null)
                        continue;
                    item.PER_ID = pCode.PER_ID;
                    var plant = new XmlPlantDataMapper().GetPlant(xElement.Element("PLANT_ID").Value);
                    if(plant == null)
                        continue;
                    item.PLANT_ID = plant.PLANT_ID;
                    var goodsType =
                        new XmlGoodsTypeDataMapper().GetGoodsType(Convert.ToInt32(xElement.Element("EXC_GOOD_TYP").Value));
                    if(goodsType== null)
                        continue;
                    item.GOODTYP_ID = goodsType.GOODTYPE_ID;
                    var market =
                        new XmlMarketDataMapper().GetMarket(Convert.ToInt32(xElement.Element("MARKET_ID").Value));
                    if(market == null)
                        continue;
                    item.MARKET_ID = market.MARKET_ID;
                    
                    var series =
                        new XmlSeriesDataMapper().GetSeries(Convert.ToInt32(xElement.Element("SERIES_ID").Value));
                    if(series == null)
                        continue;
                    item.SERIES_ID = series.SERIES_ID;
                    item.HJE_IDR = Convert.ToDecimal(xElement.Element("HJE_IDR").Value);
                    var prodType = new XmlProdTypeDataMapper().GetProdType(Convert.ToInt32(xElement.Element("PROD_CODE").Value));
                    if(prodType == null)
                        continue;
                    item.PRODUCT_ID = prodType.PRODUCT_ID;
                    item.BRAND_CE = xElement.Element("BRAND_CE").Value;
                    item.SKEP_NP = xElement.Element("SKEP_NP").Value;
                    item.SKEP_DATE = Convert.ToDateTime(xElement.Element("SKEP_DATE").Value);
                    item.COLOUR = xElement.Element("COLOUR").Value;
                    item.CUT_FILLER_CODE = xElement.Element("CUT_FILLER_CODE").Value;
                    item.PRINTING_PRICE = Convert.ToDecimal(xElement.Element("PRINTING_PRICE").Value);
                    item.START_DATE = Convert.ToDateTime(xElement.Element("START_DATE").Value);
                    item.END_DATE = Convert.ToDateTime(xElement.Element("END_DATE").Value);
                    item.FA_CODE = xElement.Element("FA_CODE").Value;
                    item.CREATED_DATE = DateTime.Now;
                   
                    var dateXml = DateTime.MinValue;
                    DateTime.TryParse(xElement.Element("MODIFIED_DATE").Value, out dateXml);
                    var existingMaterial = GetMaterial(item.STICKER_CODE,item.PLANT_ID, item.FA_CODE);
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
            
            File.Move(_xmlMapper._xmlName, @"H:/Archieve/"+_xmlMapper._xmlName);

        }
        public ZAIDM_EX_MATERIAL GetMaterial(string stickerCode, long?plant_id, string fa_code)
        {
            var existingData = _xmlMapper.uow.GetGenericRepository<ZAIDM_EX_MATERIAL>()
                          .Get(p => p.STICKER_CODE == stickerCode && p.PLANT_ID == plant_id && p.FA_CODE== fa_code)
                          .OrderByDescending(p => p.CREATED_DATE)
                          .FirstOrDefault();
            return existingData;
        }
        



    }
}
