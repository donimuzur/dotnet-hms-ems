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
                var xmlRoot = _xmlMapper.GetElement("IDOC");
                var xmlItems = xmlRoot.Elements("Z1A_BRAND");
                var items = new List<ZAIDM_EX_BRAND>();
                try
                {
                    foreach (var xElement in xmlItems)
                    {
                        var item = new ZAIDM_EX_BRAND();
                        item.STICKER_CODE = xElement.Element("STICKER_CODE").Value;

                        var plantCode = xElement.Element("PLANT").Value;
                        var plant = new XmlPlantDataMapper(null).GetPlant(plantCode);
                        if (plant == null)
                        {
                            //insert PLANT
                            var plantToAdd = new T001W();
                            plantToAdd.WERKS = plantCode;
                            plantToAdd.CREATED_DATE = DateTime.Now;
                            _xmlMapper.InsertToDatabase(plantToAdd);
                        }
                        item.WERKS = plantCode;
                        item.FA_CODE = xElement.Element("FA_CODE").Value;
                        item.COUNTRY = xElement.Element("COUNTRY").Value;

                        var pcodeCode = xElement.Element("PER_CODE").Value;
                        var pCode = new XmlPCodeDataMapper(null).GetPCode(pcodeCode);
                        if (pCode == null)
                        {
                            var pCodeToAdd = new ZAIDM_EX_PCODE();
                            pCodeToAdd.PER_CODE = pcodeCode;
                            pCodeToAdd.PER_DESC = xElement.Element("PER_DESC") == null ? null : xElement.Element("PER_DESC").Value;
                            pCodeToAdd.CREATED_DATE = DateTime.Now;
                            _xmlMapper.InsertToDatabase(pCodeToAdd);
                        }
                        item.PER_CODE = pcodeCode;
                        item.BRAND_CE = xElement.Element("BRAND_CE").Value;
                        item.SKEP_NO = xElement.Element("SKEP_NO").Value;
                        item.SKEP_DATE = _xmlMapper.GetDate(xElement.Element("SKEP_DATE").Value);
                        var prodCode = xElement.Element("PROD_CODE").Value;
                        var prodType = new XmlProdTypeDataMapper(null).GetProdType(prodCode);
                        if (prodType == null)
                        {
                            var prodTypeToAdd = new ZAIDM_EX_PRODTYP();
                            prodTypeToAdd.PROD_CODE = prodCode;
                            prodTypeToAdd.PRODUCT_TYPE = xElement.Element("PRODUCT_TYPE").Value;
                            prodTypeToAdd.PRODUCT_ALIAS = xElement.Element("PRODUCT_ALIAS").Value;
                            prodTypeToAdd.CREATED_DATE = DateTime.Now;
                            _xmlMapper.InsertToDatabase(prodTypeToAdd);
                        }
                        item.PROD_CODE = prodCode;
                        var series_id = xElement.Element("SERIES_CODE").Value;
                        var series =
                            new XmlSeriesDataMapper(null).GetSeries(series_id);
                        if (series == null)
                        {
                            var seriesToAdd = new ZAIDM_EX_SERIES();
                            seriesToAdd.SERIES_CODE = series_id;
                            seriesToAdd.SERIES_VALUE = xElement.Element("SERIES_VALUE").Value;
                            seriesToAdd.CREATED_DATE = DateTime.Now;
                            _xmlMapper.InsertToDatabase(seriesToAdd);
                        }
                        item.SERIES_CODE = series.SERIES_CODE;
                        var marketId = xElement.Element("MARKET").Value;
                        var market =
                            new XmlMarketDataMapper(null).GetMarket(marketId);
                        if (market == null)
                        {
                            var marketToAdd = new ZAIDM_EX_MARKET();
                            marketToAdd.MARKET_ID = marketId;
                            marketToAdd.MARKET_DESC = xElement.Element("MARKET_DESC").Value;
                            marketToAdd.CREATED_DATE = DateTime.Now;
                            _xmlMapper.InsertToDatabase(marketToAdd);
                        }
                        item.MARKET_ID = marketId;


                        var exGoodType = xElement.Element("EXC_GOOD_TYP").Value;
                        var goodsType =
                            new XmlGoodsTypeDataMapper(null).GetGoodsType(exGoodType);
                        if (goodsType == null)
                        {
                            var goodTypeToAdd = new ZAIDM_EX_GOODTYP();
                            goodTypeToAdd.EXC_GOOD_TYP = exGoodType;
                            goodTypeToAdd.EXT_TYP_DESC = xElement.Element("EXC_TYP_DESC").Value;
                            goodTypeToAdd.CREATED_DATE = DateTime.Now;
                            _xmlMapper.InsertToDatabase(goodTypeToAdd);
                        }
                        item.EXC_GOOD_TYP = exGoodType;
                        item.HJE_IDR = Convert.ToDecimal(xElement.Element("HJE_IDR").Value);
                        item.HJE_CURR = xElement.Element("HJE_CURR").Value;
                        item.TARIFF = Convert.ToDecimal(xElement.Element("TARIFF").Value);
                        item.TARIF_CURR = xElement.Element("TARIFF_CURR").Value;
                        item.COLOUR = xElement.Element("COLOUR").Value;
                        //item.CUT_FILLER_CODE = xElement.Element("CUT_FILLER_CODE").Value;
                        //item.PRINTING_PRICE = Convert.ToDecimal(xElement.Element("PRINTING_PRICE").Value);
                        item.START_DATE = _xmlMapper.GetDate(xElement.Element("START_DATE").Value);
                        item.END_DATE = _xmlMapper.GetDate(xElement.Element("END_DATE").Value);
                        //var dateXml = Convert.ToDateTime(xElement.Element("MODIFIED_DATE").Value);
                        var existingMaterial = GetBrand(item.WERKS, item.FA_CODE);
                        if (existingMaterial != null)
                        {

                            item.CREATED_DATE = existingMaterial.CREATED_DATE;
                            item.MODIFIED_DATE = DateTime.Now;
                            items.Add(item);

                        }
                        else
                        {
                            item.CREATED_DATE = DateTime.Now;
                            items.Add(item);
                        }
                    }
                }
                catch (Exception ex)
                {

                    throw;
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
