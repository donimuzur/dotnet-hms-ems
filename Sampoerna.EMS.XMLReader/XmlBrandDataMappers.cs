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
    public class XmlBrandDataMapper : IXmlDataReader 
    {
        private XmlDataMapper _xmlMapper = null;

        public XmlBrandDataMapper(string filename)
        {
            _xmlMapper = new XmlDataMapper(filename);
           
        }


        public List<ZAIDM_EX_BRAND> Items
        {
         get
            {
                var xmlItems = _xmlMapper.GetElements("ITEM");
                var items = new List<ZAIDM_EX_BRAND>();
                foreach (var xElement in xmlItems)
                {
                    var item = new ZAIDM_EX_BRAND();
                    item.STICKER_CODE = xElement.Element("STICKER_CODE").Value;
                    var pcodeCode = Convert.ToInt32(xElement.Element("PER_CODE").Value);
                    var pCode = new XmlPCodeDataMapper(null).GetPCode(pcodeCode);
                    if (pCode == null)
                       throw  new Exception("no existing PCODE PER_CODE " + pcodeCode);
                    item.PER_CODE = pCode.PER_CODE;
                    var plantCode = xElement.Element("PLANT_ID").Value;
                    var plant = new XmlPlantDataMapper(null).GetPlant(plantCode);
                    if(plant == null)
                        throw  new Exception("no existing plant plant code " + plantCode);
                    item.WERKS = plant.WERKS;
                    var exGoodType = Convert.ToInt32(xElement.Element("EXC_GOOD_TYP").Value);
                    var goodsType =
                        new XmlGoodsTypeDataMapper(null).GetGoodsType(exGoodType);
                    if(goodsType== null)
                        throw new Exception("no existing goods type " + exGoodType);
                    item.EXC_GOOD_TYP = goodsType.EXC_GOOD_TYP;
                    var marketId = Convert.ToInt32(xElement.Element("MARKET_ID").Value);
                    var market =
                        new XmlMarketDataMapper(null).GetMarket(marketId);
                    if(market == null)
                        throw new Exception("no existing market  market id" + marketId);
                    item.MARKET_ID = market.MARKET_ID;
                    var series_id = Convert.ToInt32(xElement.Element("SERIES_CODE").Value);
                    var series =
                        new XmlSeriesDataMapper(null).GetSeries(series_id);
                    if(series == null)
                        throw new Exception("no existing series  series id" + series_id);
                    item.SERIES_CODE = series.SERIES_CODE;
                    item.HJE_IDR = Convert.ToDecimal(xElement.Element("HJE_IDR").Value);
                    var prodCode = Convert.ToInt32(xElement.Element("PROD_CODE").Value);
                    var prodType = new XmlProdTypeDataMapper(null).GetProdType(prodCode);
                    if(prodType == null)
                        throw new Exception("no existing product code " + prodCode);
                    item.PROD_CODE = prodType.PROD_CODE;
                    item.BRAND_CE = xElement.Element("BRAND_CE").Value;
                    item.SKEP_NP = xElement.Element("SKEP_NP").Value;
                    item.SKEP_DATE = Convert.ToDateTime(xElement.Element("SKEP_DATE").Value);
                    item.COLOUR = xElement.Element("COLOUR").Value;
                    item.CUT_FILLER_CODE = xElement.Element("CUT_FILLER_CODE").Value;
                    item.PRINTING_PRICE = Convert.ToDecimal(xElement.Element("PRINTING_PRICE").Value);
                    item.START_DATE = Convert.ToDateTime(xElement.Element("START_DATE").Value);
                    item.END_DATE = Convert.ToDateTime(xElement.Element("END_DATE").Value);
                    item.FA_CODE = xElement.Element("FA_CODE").Value;
                    
                    var dateXml = Convert.ToDateTime(xElement.Element("MODIFIED_DATE").Value);
                    var existingMaterial = GetBrand(item.WERKS, item.FA_CODE);
                    if (existingMaterial != null)
                    {
                        if (dateXml > existingMaterial.CREATED_DATE)
                        {
                            item.MODIFIED_DATE = dateXml;
                            items.Add(item);
                        }
                        else
                        {
                            continue;
                            
                        }
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
            _xmlMapper.InsertToDatabase<ZAIDM_EX_BRAND>(Items);
       
        }
        public ZAIDM_EX_BRAND GetBrand(string plant_id, string fa_code)
        {
            var existingData = _xmlMapper.uow.GetGenericRepository<ZAIDM_EX_BRAND>()
                .GetByID(plant_id, fa_code);
            return existingData;
        }
        



    }
}
