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
                
                    foreach (var xElement in xmlItems)
                    {
                        try
                        {
                        var item = new ZAIDM_EX_BRAND();
                        item.STICKER_CODE = xElement.Element("STICKER_CODE").Value;

                        var plantCode = _xmlMapper.GetElementValue(xElement.Element("PLANT"));
                        //var plant = new XmlPlantDataMapper(null).GetPlant(plantCode);
                        //if (plant == null)
                        //{
                        //    //insert PLANT
                        //    var plantToAdd = new T001W();
                        //    plantToAdd.WERKS = plantCode;
                        //    plantToAdd.CREATED_DATE = DateTime.Now;
                        //    _xmlMapper.InsertToDatabase(plantToAdd);
                        //}
                        item.WERKS = plantCode;
                        item.FA_CODE = _xmlMapper.GetElementValue(xElement.Element("FA_CODE"));
                        item.COUNTRY = _xmlMapper.GetElementValue(xElement.Element("COUNTRY"));
                        item.BRAND_CONTENT = _xmlMapper.GetElementValue(xElement.Element("CONTENT"));
                        item.CREATED_BY = _xmlMapper.GetElementValue(xElement.Element("MODIFIED_BY"));
                        var pcodeCode = _xmlMapper.GetElementValue(xElement.Element("PER_CODE"));
                        //var pCode = new XmlPCodeDataMapper(null).GetPCode(pcodeCode);
                        //if (pCode == null)
                        //{
                        //    var pCodeToAdd = new ZAIDM_EX_PCODE();
                        //    pCodeToAdd.PER_CODE = pcodeCode;
                        //    pCodeToAdd.PER_DESC = xElement.Element("PER_DESC") == null ? null : xElement.Element("PER_DESC").Value;
                        //    pCodeToAdd.CREATED_DATE = DateTime.Now;
                        //    _xmlMapper.InsertToDatabase(pCodeToAdd);
                        //}
                        item.PER_CODE = pcodeCode;
                        item.BRAND_CE = _xmlMapper.GetElementValue(xElement.Element("BRAND_CE"));
                        item.SKEP_NO = _xmlMapper.GetElementValue(xElement.Element("SKEP_NO"));
                        item.SKEP_DATE = _xmlMapper.GetDate(_xmlMapper.GetElementValue(xElement.Element("SKEP_DATE")));
                        var prodCode = _xmlMapper.GetElementValue(xElement.Element("PROD_CODE"));
                        //var prodType = new XmlProdTypeDataMapper(null).GetProdType(prodCode);
                        //if (prodType == null)
                        //{
                        //    var prodTypeToAdd = new ZAIDM_EX_PRODTYP();
                        //    prodTypeToAdd.PROD_CODE = prodCode;
                        //    prodTypeToAdd.PRODUCT_TYPE = xElement.Element("PRODUCT_TYPE").Value;
                        //    prodTypeToAdd.PRODUCT_ALIAS = xElement.Element("PRODUCT_ALIAS").Value;
                        //    prodTypeToAdd.CREATED_DATE = DateTime.Now;
                        //    _xmlMapper.InsertToDatabase(prodTypeToAdd);
                        //}
                        item.PROD_CODE = prodCode;
                        var series_id = _xmlMapper.GetElementValue(xElement.Element("SERIES_CODE"));
                        //var series =
                        //    new XmlSeriesDataMapper(null).GetSeries(series_id);
                        //if (series == null)
                        //{
                        //    var seriesToAdd = new ZAIDM_EX_SERIES();
                        //    seriesToAdd.SERIES_CODE = series_id;
                        //    seriesToAdd.SERIES_VALUE = xElement.Element("SERIES_VALUE").Value;
                        //    seriesToAdd.CREATED_DATE = DateTime.Now;
                        //    _xmlMapper.InsertToDatabase(seriesToAdd);
                        //}
                        item.SERIES_CODE = series_id;
                        var marketId = _xmlMapper.GetElementValue(xElement.Element("MARKET"));
                        //var market =
                        //    new XmlMarketDataMapper(null).GetMarket(marketId);
                        //if (market == null)
                        //{
                        //    var marketToAdd = new ZAIDM_EX_MARKET();
                        //    marketToAdd.MARKET_ID = marketId;
                        //    marketToAdd.MARKET_DESC = xElement.Element("MARKET_DESC").Value;
                        //    marketToAdd.CREATED_DATE = DateTime.Now;
                        //    _xmlMapper.InsertToDatabase(marketToAdd);
                        //}
                        item.MARKET_ID = marketId;


                        var exGoodType = _xmlMapper.GetElementValue(xElement.Element("EXC_GOOD_TYP"));
                        //var goodsType =
                        //    new XmlGoodsTypeDataMapper(null).GetGoodsType(exGoodType);
                        //if (goodsType == null)
                        //{
                        //    var goodTypeToAdd = new ZAIDM_EX_GOODTYP();
                        //    goodTypeToAdd.EXC_GOOD_TYP = exGoodType;
                        //    goodTypeToAdd.EXT_TYP_DESC = xElement.Element("EXC_TYP_DESC").Value;
                        //    goodTypeToAdd.CREATED_DATE = DateTime.Now;
                        //    _xmlMapper.InsertToDatabase(goodTypeToAdd);
                        //}
                        item.EXC_GOOD_TYP = exGoodType;
                        item.HJE_IDR = Convert.ToDecimal(_xmlMapper.GetElementValue(xElement.Element("HJE_IDR")));
                        item.HJE_CURR = _xmlMapper.GetElementValue(xElement.Element("HJE_CURR"));
                        item.TARIFF = Convert.ToDecimal(_xmlMapper.GetElementValue(xElement.Element("TARIFF")));
                        item.TARIF_CURR = _xmlMapper.GetElementValue(xElement.Element("TARIFF_CURR"));
                        item.COLOUR = _xmlMapper.GetElementValue(xElement.Element("COLOUR"));
                        item.START_DATE = _xmlMapper.GetDate(_xmlMapper.GetElementValue(xElement.Element("START_DATE")));
                        item.END_DATE = _xmlMapper.GetDate(_xmlMapper.GetElementValue(xElement.Element("END_DATE")));
                        var existingMaterial = GetBrand(item.WERKS, item.FA_CODE);
                        if (existingMaterial != null)
                        {
                            item.CONVERSION = existingMaterial.CONVERSION;
                            item.PRINTING_PRICE = existingMaterial.PRINTING_PRICE;
                            item.CUT_FILLER_CODE = existingMaterial.CUT_FILLER_CODE;
                            item.CREATED_DATE = existingMaterial.CREATED_DATE;
                            item.MODIFIED_BY = item.CREATED_BY;
                            item.MODIFIED_DATE = DateTime.Now;
                            item.CREATED_BY = existingMaterial.CREATED_BY;
                            items.Add(item);

                        }
                        else
                        {
                            item.CREATED_DATE = DateTime.Now;
                            items.Add(item);
                        }
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
            return _xmlMapper.InsertToDatabase<ZAIDM_EX_BRAND>(Items);
       
        }

        public List<string> GetErrorList()
        {
            return _xmlMapper.Errors;
        }

        public ZAIDM_EX_BRAND GetBrand(string plant_id, string fa_code)
        {
            var existingData = _xmlMapper.uow.GetGenericRepository<ZAIDM_EX_BRAND>()
                .GetByID(plant_id, fa_code);
            return existingData;
        }
        



    }
}
