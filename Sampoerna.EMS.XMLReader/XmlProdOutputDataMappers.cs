﻿﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Sampoerna.EMS.BusinessObject;
﻿using Sampoerna.EMS.BusinessObject.Outputs;
﻿using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.DAL;
using Voxteneo.WebComponents.Logger;
namespace Sampoerna.EMS.XMLReader
{
    public class XmlProdOutputDataMapper : IXmlDataReader
    {
        private XmlDataMapper _xmlMapper = null;

        public XmlProdOutputDataMapper(string filename)
        {
            _xmlMapper = new XmlDataMapper(filename);

        }


        public List<PRODUCTION> Items
        {
            get
            {
                var xmlRoot = _xmlMapper.GetElement("PRDOUTPUTDetails");
                var xmlItems = xmlRoot.Elements("row");
                var items = new List<PRODUCTION>();
                var itemTemp = new List<ZAAP_SHIFT_RPT>();
                foreach (var xElement in xmlItems)
                {
                    var zaapShftRptItem = new ZAAP_SHIFT_RPT();
                    zaapShftRptItem.MATDOC = _xmlMapper.GetElementValue(xElement.Element("MatDoc"));

                    zaapShftRptItem.BATCH = _xmlMapper.GetElementValue(xElement.Element("Batch"));

                    var bundle = _xmlMapper.GetElementValue(xElement.Element("Bundle"));
                    if (String.IsNullOrEmpty(bundle))
                        zaapShftRptItem.BUNDLE = null;
                    else
                    {
                        zaapShftRptItem.BUNDLE = Convert.ToDecimal(bundle);
                    }
                    
                    //zaapShftRptItem.MARKET = _xmlMapper.GetElementValue(xElement.Element("Market"));
                    zaapShftRptItem.DOCGMVTER = _xmlMapper.GetElementValue(xElement.Element("DocGMvtEr"));

                    zaapShftRptItem.ORDR = _xmlMapper.GetElementValue(xElement.Element("Order"));
                    zaapShftRptItem.SHIFT = _xmlMapper.GetElementValue(xElement.Element("Shift"));
                    //zaapShftRptItem.COMPANY_CODE = item.COMPANY_CODE;

                    
                    zaapShftRptItem.FA_CODE = _xmlMapper.GetElementValue(xElement.Element("Material"));

                    decimal qty;
                    decimal confQty;
                    try
                    {
                        confQty = Convert.ToDecimal(_xmlMapper.GetElementValue(xElement.Element("ConfQty")).Replace(",", ""));
                        qty = Convert.ToDecimal(_xmlMapper.GetElementValue(xElement.Element("Quantity")).Replace(",", ""));
                    }
                    catch (Exception ex)
                    {
                        string reason = "";
                        var qtyString = _xmlMapper.GetElementValue(xElement.Element("Quantity"));
                        if (ex.GetType() == typeof(FormatException))
                            reason = "format not handled";
                        else
                        {
                            reason = "reason unknown";
                        }
                        _xmlMapper.Errors.Add(String.Format("failed to get qty : {0} value {1}", reason, qtyString));

                        continue;
                    }

                    var bun = _xmlMapper.GetElementValue(xElement.Element("BUn"));
                    switch (bun)
                    {
                        case "TH":
                            zaapShftRptItem.QTY = qty * 1000;
                            zaapShftRptItem.UOM = "Btg";


                            break;
                        case "KG":
                            zaapShftRptItem.QTY = qty * 1000;
                            zaapShftRptItem.UOM = "G";

                            break;
                        default:
                            zaapShftRptItem.QTY = qty;
                            zaapShftRptItem.UOM = bun;


                            break;
                    }
                    //zaapShftRptItem.QTY = item.QTY;
                    //zaapShftRptItem.UOM = item.UOM;

                    zaapShftRptItem.ORIGINAL_QTY = confQty;
                    zaapShftRptItem.ORIGINAL_UOM = bun;

                    zaapShftRptItem.PRODUCTION_DATE = Convert.ToDateTime(_xmlMapper.GetDateDotSeparator(_xmlMapper.GetElementValue(xElement.Element("ProdDate"))));
                    zaapShftRptItem.WERKS = _xmlMapper.GetElementValue(xElement.Element("Plnt"));

                    var existingBrand = GetMaterialBrand(zaapShftRptItem.FA_CODE, zaapShftRptItem.WERKS);
                    if (existingBrand != null)
                    {
                        if (existingBrand.BRAND_CONTENT == null)
                        {
                            throw new Exception(String.Format(
                                "Brand {0} - {2} in plant {1} has null CONTENT value", existingBrand.FA_CODE,
                                existingBrand.WERKS, existingBrand.STICKER_CODE));
                        }

                        //zaapShftRptItem.BRAND_DESC = existingBrand.BRAND_CE;
                    }
                    else
                    {
                        _xmlMapper.Errors.Add(string.Format("no brand fa_code {0} - werks {1}", zaapShftRptItem.FA_CODE,
                            zaapShftRptItem.WERKS));
                    }

                    var mvt = _xmlMapper.GetElementValue(xElement.Element("MvT"));
                    var enteredOn = Convert.ToDateTime(_xmlMapper.GetDateDotSeparator(_xmlMapper.GetElementValue(xElement.Element("EnteredOn"))));
                    var postingDate = Convert.ToDateTime(_xmlMapper.GetDateDotSeparator(_xmlMapper.GetElementValue(xElement.Element("PostgDate")))); 
                    zaapShftRptItem.MVT = mvt;
                    zaapShftRptItem.POSTING_DATE = postingDate;
                    zaapShftRptItem.ENTERED_DATE = enteredOn;

                    var company = GetCompanyByPlant(zaapShftRptItem.WERKS);
                    if (company != null)
                    {
                        zaapShftRptItem.COMPANY_CODE = company.BUKRS;
                        //zaapShftRptItem.COMPANY_NAME = company.T001.BUTXT;
                        //zaapShftRptItem.PLANT_NAME = company.T001W.NAME1;
                    }

                    //var closingDate = CheckClosingDate(zaapShftRptItem.WERKS, zaapShftRptItem.PRODUCTION_DATE);

                    //if (closingDate == null)
                    //{
                        itemTemp.Add(zaapShftRptItem);
                    //}
                    

                }

                

                

                var itemsGrouped = ReconsoleZaap(itemTemp);

                foreach (var zaapShiftRpt in itemsGrouped)
                {
                    var existingZaap = GetExistingZaapShiftRpt(zaapShiftRpt);
                    if (existingZaap != null)
                    {
                        zaapShiftRpt.MODIFIED_BY = "PI";
                        zaapShiftRpt.MODIFIED_DATE = DateTime.Now;
                        zaapShiftRpt.CREATED_DATE = existingZaap.CREATED_DATE;
                        zaapShiftRpt.CREATED_BY = existingZaap.CREATED_BY;
                        zaapShiftRpt.ZAAP_SHIFT_RPT_ID = existingZaap.ZAAP_SHIFT_RPT_ID;
                    }
                    else
                    {
                        zaapShiftRpt.CREATED_BY = "PI";
                        zaapShiftRpt.CREATED_DATE = DateTime.Now;
                    }
                }

                foreach (var zaapShiftRpt in itemsGrouped)
                {
                    _xmlMapper.InsertOrUpdate(zaapShiftRpt);
                }

                //_xmlMapper.uow.SaveChanges();

                var prodDateList = itemsGrouped.GroupBy(x => x.PRODUCTION_DATE).Select(y => y.Key).ToList();

                var newData = RecalculateZaapShftRpt(prodDateList);

                foreach (var production in newData)
                {
                    var existingProd = GetProductionExisting(production.FA_CODE, production.WERKS,
                        production.PRODUCTION_DATE);

                    if (existingProd != null)
                    {
                        
                        production.MODIFIED_BY = "PI";
                        production.MODIFIED_DATE = DateTime.Now;
                        production.CREATED_BY = existingProd.CREATED_BY;
                        production.CREATED_DATE = existingProd.CREATED_DATE;
                        production.QTY = existingProd.QTY;
                        production.ZB = existingProd.ZB;
                        
                        production.BRAND_DESC = existingProd.BRAND_DESC;
                        production.COMPANY_CODE = existingProd.COMPANY_CODE;
                        production.COMPANY_NAME = existingProd.COMPANY_NAME;
                        production.PLANT_NAME = existingProd.PLANT_NAME;

                        switch (production.UOM)
                        {
                            case "Btg":
                                production.QTY_PACKED = production.QTY_PACKED;



                                break;
                            case "G":
                                production.QTY_PACKED = production.QTY_PACKED;


                                break;

                        }

                        if (production.QTY_PACKED == existingProd.QTY_PACKED)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        production.CREATED_BY = "PI";
                        production.CREATED_DATE = DateTime.Now;
                        
                        production.QTY = 0;

                        var existingBrand = GetMaterialBrand(production.FA_CODE, production.WERKS);
                        if (existingBrand != null)
                        {
                            if (existingBrand.BRAND_CONTENT == null)
                            {
                                throw new Exception(String.Format(
                                    "Brand {0} - {2} in plant {1} has null CONTENT value", existingBrand.FA_CODE,
                                    existingBrand.WERKS, existingBrand.STICKER_CODE));
                            }

                            production.BRAND_DESC = existingBrand.BRAND_CE;
                        }
                        else
                        {
                            _xmlMapper.Errors.Add(string.Format("no brand fa_code {0} - werks {1}", production.FA_CODE,
                                production.WERKS));
                        }

                        var company = GetCompanyByPlant(production.WERKS);
                        if (company != null)
                        {
                            production.COMPANY_CODE = company.BUKRS;
                            production.COMPANY_NAME = company.T001.BUTXT;
                            production.PLANT_NAME = company.T001W.NAME1;
                        }

                        switch (production.UOM)
                        {
                            case "Btg":
                                production.QTY_PACKED = production.QTY_PACKED;



                                break;
                            case "G":
                                production.QTY_PACKED = production.QTY_PACKED;


                                break;

                        }
                    }


                    //var closingDate = CheckClosingDate(production.WERKS, production.PRODUCTION_DATE);

                    //if (closingDate == null)
                    //{
                        items.Add(production);
                    //}
                }

                return items;
            }
        }


       


        public MovedFileOutput InsertToDatabase()
        {


            return _xmlMapper.InsertToDatabase<PRODUCTION>(Items);

        }

        public List<string> GetErrorList()
        {
            return _xmlMapper.Errors;
        }

        

        public ZAIDM_EX_BRAND GetMaterialBrand(string materialNumber, string plant)
        {
            var existingData = _xmlMapper.uow.GetGenericRepository<ZAIDM_EX_BRAND>()
                .Get(x => x.FA_CODE == materialNumber && x.WERKS == plant).FirstOrDefault();
            return existingData;
        }

        public T001K GetCompanyByPlant(string plant)
        {
            var existingData = _xmlMapper.uow.GetGenericRepository<T001K>()
                .Get(x => x.BWKEY == plant, null, "T001, T001W").FirstOrDefault();
            return existingData;
        }

        public PRODUCTION GetProductionExisting(string faCode, string plantid, DateTime prodDate)
        {
            var existingData = _xmlMapper.uow.GetGenericRepository<PRODUCTION>()
                .Get(
                    x =>
                      x.WERKS == plantid && x.FA_CODE == faCode &&
                        x.PRODUCTION_DATE == prodDate).FirstOrDefault();
            return existingData;
        }

        private int GetShift(string shift)
        {
            var validValue = new string[] {"1st", "2nd", "3rd", "4th", "5th", "6th"};
            if (string.IsNullOrEmpty(shift))
                return 0;
            if (!validValue.Contains(shift.ToLower()))
                return 0;

            char[] arr = shift.ToCharArray();

            var arrDigit = Array.FindAll<char>(arr, (c => (char.IsDigit(c))));
            var str =  new string(arrDigit);
            return Convert.ToInt32(str);
        }

        private ZAAP_SHIFT_RPT GetExistingZaapShiftRpt(ZAAP_SHIFT_RPT item)
        {
            var existingData = _xmlMapper.uow.GetGenericRepository<ZAAP_SHIFT_RPT>()
                .Get(x=> x.BATCH == item.BATCH
                    && x.FA_CODE == item.FA_CODE
                    && x.WERKS == item.WERKS
                    && x.MATDOC == item.MATDOC
                    && x.MVT == item.MVT
                    && x.ORDR == item.ORDR
                    && x.SHIFT == item.SHIFT
                    && x.DOCGMVTER == item.DOCGMVTER
                    && x.PRODUCTION_DATE  == item.PRODUCTION_DATE
                    && x.POSTING_DATE == item.POSTING_DATE
                    && x.ENTERED_DATE == item.ENTERED_DATE
                    ,null).FirstOrDefault();

            return existingData;
        }

        private ZAAP_SHIFT_RPT GetExistingZaapShiftRpt(string matdoc)
        {
            var existingData = _xmlMapper.uow.GetGenericRepository<ZAAP_SHIFT_RPT>()
                .GetByID(matdoc);

            return existingData;
        }

        private void RecalculateWithDatabase(List<PRODUCTION> itemList)
        {
            foreach (var item in itemList)
            {


                var existingProduction = GetProductionExisting(item.FA_CODE, item.WERKS,item.PRODUCTION_DATE);
                
                if (existingProduction == null)
                {
                    item.PROD_QTY_STICK = item.QTY;            
                    item.CREATED_DATE = DateTime.Now;
                    item.CREATED_BY = "PI";

                    switch (item.UOM)
                    {
                        case "TH":

                            
                                item.QTY_PACKED = item.QTY;

                            


                            break;
                        case "KG":

                            
                                item.QTY_PACKED = item.QTY;
                                //item.QTY_UNPACKED = existingProduction.QTY_UNPACKED;

                            
                            break;
                        default:


                           
                                //item.QTY_PACKED = existingProduction.QTY_PACKED;
                                if (String.IsNullOrEmpty(item.BATCH))
                                {
                                    //item.QTY_UNPACKED = existingProduction.QTY_UNPACKED;
                                }
                            
                            break;
                    }
                    item.QTY = 0;
                }
                else
                {

                    
                    //if(item)
                    item.PROD_QTY_STICK = item.QTY;
                    item.QTY = existingProduction.QTY;
                    item.MODIFIED_DATE = DateTime.Now;
                    item.MODIFIED_BY = "PI";
                    item.CREATED_BY = existingProduction.CREATED_BY;
                    item.CREATED_DATE = existingProduction.CREATED_DATE;
                                
                }

                

                var existingBrand = GetMaterialBrand(item.FA_CODE, item.WERKS);
                if (existingBrand != null)
                {
                    int tempContent = 1;
                    if (int.TryParse(existingBrand.BRAND_CONTENT, out tempContent))
                    {
                        
                        if (item.PROD_QTY_STICK != null)
                        {
                            decimal tempPack = decimal.Floor(item.PROD_QTY_STICK.Value / tempContent);
                            decimal tempQtyPacked = tempPack*tempContent;

                            item.QTY_PACKED = tempQtyPacked;
                        }
                        //item.QTY_UNPACKED = item.QTY - item.QTY_PACKED;
                        //item.PROD_QTY_STICK = item.QTY;
                            
                        
                    }
                    else
                    {
                        throw new Exception(String.Format("Brand {0} - {1} dont have recognized content value",existingBrand.FA_CODE,existingBrand.WERKS));
                    }


                    
                }
            }
            
        }

        private List<ZAAP_SHIFT_RPT> ReconsoleZaap(List<ZAAP_SHIFT_RPT> itemTemp)
        {
            
            var itemreconsole = itemTemp.GroupBy(x => new
            {
                x.BATCH,
                x.FA_CODE,
                x.WERKS,
                x.ORDR,
                x.POSTING_DATE,
                x.PRODUCTION_DATE,
                x.ENTERED_DATE,
                x.MATDOC,
                x.SHIFT,
                x.MVT,
                x.DOCGMVTER,
                x.BUNDLE,
                x.COMPANY_CODE
                
            }).Select(x =>
            {
                var firstOrDefault = x.FirstOrDefault();
                return firstOrDefault != null ? new ZAAP_SHIFT_RPT()
                {
                    MATDOC = x.Key.MATDOC,
                    PRODUCTION_DATE = x.Key.PRODUCTION_DATE,
                    FA_CODE = x.Key.FA_CODE,
                    WERKS = x.Key.WERKS,
                    BATCH = x.Key.BATCH,
                    ORDR = x.Key.ORDR,
                    SHIFT = x.Key.SHIFT,
                    MVT = x.Key.MVT,
                    POSTING_DATE = x.Key.POSTING_DATE,
                    ENTERED_DATE = x.Key.ENTERED_DATE,
                    DOCGMVTER = x.Key.DOCGMVTER,
                    BUNDLE = x.Key.BUNDLE,
                    QTY = x.Sum(y => y.QTY),
                    ORIGINAL_QTY = firstOrDefault.ORIGINAL_QTY,
                    UOM = firstOrDefault.UOM,
                    ORIGINAL_UOM = firstOrDefault.ORIGINAL_UOM,
                    COMPANY_CODE = firstOrDefault.COMPANY_CODE
                } : null;
            }).ToList();

            //var itemsTobeAdded = new List<ZAAP_SHIFT_RPT>();

            //foreach (var item in itemreconsole)
            //{
            //    var tempExisting = itemTemp.FirstOrDefault(x => x.BATCH == item.BATCH
            //                                                    && x.FA_CODE == item.FA_CODE
            //                                                    && x.WERKS == item.WERKS
            //                                                    && x.MATDOC == item.MATDOC
            //                                                    && x.MVT == item.MVT
            //                                                    && x.ORDR == item.ORDR
            //                                                    && x.SHIFT == item.SHIFT

            //                                                    && x.PRODUCTION_DATE == item.PRODUCTION_DATE
            //                                                    && x.POSTING_DATE == item.POSTING_DATE
            //                                                    && x.ENTERED_DATE == item.ENTERED_DATE);


            //    if (tempExisting != null)
            //    {
            //        var tempItem = tempExisting;
            //        tempItem.QTY = item.QTY;
            //        tempItem.ORIGINAL_QTY = item.ORIGINAL_QTY;
            //        itemsTobeAdded.Add(tempItem);
            //    }
                
            //}

            return itemreconsole;
        }

        private List<PRODUCTION> RecalculateZaapShftRpt(List<DateTime> prodDateList)
        {
            var tempProdList = _xmlMapper.uow.GetGenericRepository<ZAAP_SHIFT_RPT>().Get(x => x.MVT == "101" && prodDateList.Contains(x.PRODUCTION_DATE)).GroupBy(x => new
            {
                FA_CODE = x.FA_CODE,
                WERKS = x.WERKS,
                PRODUCTION_DATE = x.PRODUCTION_DATE,
                //MATDOC = x.MATDOC,
                //BATCH = x.BATCH
            }).Select(x =>
            {
                var zaapShiftRpt = x.FirstOrDefault();
                return zaapShiftRpt != null ? new PRODUCTION()
                {

                    FA_CODE = zaapShiftRpt.FA_CODE,
                    WERKS = zaapShiftRpt.WERKS,
                    PRODUCTION_DATE = zaapShiftRpt.PRODUCTION_DATE,
                    QTY_PACKED = x.Sum(y => y.QTY),
                    PROD_QTY_STICK = x.Sum(y=> y.QTY),
                    //COMPANY_CODE = zaapShiftRpt.COMPANY_CODE,
                    //BUNDLE = zaapShiftRpt.BUNDLE,
                    //ORDR = zaapShiftRpt.ORDR,
                    //DOCGMVTER = zaapShiftRpt.DOCGMVTER,
                    UOM = zaapShiftRpt.UOM
                } : null;
            });


            //var prodList = tempProdList.GroupBy(x => new
            //{
            //    FA_CODE = x.FA_CODE,
            //    WERKS = x.WERKS,
            //    PRODUCTION_DATE = x.PRODUCTION_DATE
            //    //MATDOC = x.MATDOC
            //}).Select(x =>
            //{
            //    var production = x.FirstOrDefault();
            //    return production != null ? new PRODUCTION()
            //                  {

            //                      FA_CODE = production.FA_CODE,
            //                      WERKS = production.WERKS,
            //                      PRODUCTION_DATE = production.PRODUCTION_DATE,
            //                      QTY_PACKED = x.Sum(y => y.QTY_PACKED),// production.QTY_PACKED,
            //                      //COMPANY_CODE = zaapShiftRpt.COMPANY_CODE,
            //                      PROD_QTY_STICK = x.Sum(y => y.PROD_QTY_STICK),
            //                      //BUNDLE = production.BUNDLE,
            //                      //ORDR = production.ORDR,
            //                      //DOCGMVTER = production.DOCGMVTER,
            //                      UOM = production.UOM
            //                  } : null;
            //});


            return tempProdList.ToList();
        }

        public MONTH_CLOSING CheckClosingDate(string plant, DateTime date)
        {
            var existingData = _xmlMapper.uow.GetGenericRepository<MONTH_CLOSING>()
                .Get(c => c.PLANT_ID == plant && c.CLOSING_DATE <= date
                                    && c.CLOSING_DATE.Value.Month == date.Month
                                    && c.CLOSING_DATE.Value.Year == date.Year).FirstOrDefault();
            return existingData;
        }
    }
}