﻿using System;
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


        public List<PRODUCTION> ItemsTemp
        {
            get
            {
                var xmlRoot = _xmlMapper.GetElement("PRDOUTPUTDetails");
                var xmlItems = xmlRoot.Elements("row");
                var items = new List<PRODUCTION>();
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
                        zaapShftRptItem.BUNDLE = Convert.ToInt32(bundle);
                    }
                    
                    //zaapShftRptItem.MARKET = _xmlMapper.GetElementValue(xElement.Element("Market"));
                    zaapShftRptItem.DOCGMVTER = _xmlMapper.GetElementValue(xElement.Element("DocGMvtEr"));

                    zaapShftRptItem.ORDR = _xmlMapper.GetElementValue(xElement.Element("Order"));
                    zaapShftRptItem.SHIFT = _xmlMapper.GetElementValue(xElement.Element("Shift"));
                    //zaapShftRptItem.COMPANY_CODE = item.COMPANY_CODE;

                    
                    zaapShftRptItem.FA_CODE = _xmlMapper.GetElementValue(xElement.Element("Material"));

                    decimal qty;
                    try
                    {
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

                    zaapShftRptItem.ORIGINAL_QTY = qty;
                    zaapShftRptItem.ORIGINAL_UOM = bun;

                    zaapShftRptItem.PRODUCTION_DATE = Convert.ToDateTime(_xmlMapper.GetDateDotSeparator(_xmlMapper.GetElementValue(xElement.Element("ProdDate"))));
                    zaapShftRptItem.WERKS = _xmlMapper.GetElementValue(xElement.Element("Plnt"));

                    var mvt = _xmlMapper.GetElementValue(xElement.Element("MvT"));
                    var enteredOn = Convert.ToDateTime(_xmlMapper.GetDateDotSeparator(_xmlMapper.GetElementValue(xElement.Element("EnteredOn"))));
                    var postingDate = Convert.ToDateTime(_xmlMapper.GetDateDotSeparator(_xmlMapper.GetElementValue(xElement.Element("PostgDate")))); 
                    zaapShftRptItem.MVT = mvt;
                    zaapShftRptItem.POSTING_DATE = postingDate;
                    zaapShftRptItem.ENTERED_DATE = enteredOn;


                    var existingZaap = GetExistingZaapShiftRpt(zaapShftRptItem);
                    if (existingZaap != null)
                    {
                        zaapShftRptItem.MODIFIED_BY = "PI";
                        zaapShftRptItem.MODIFIED_DATE = DateTime.Now;
                        zaapShftRptItem.CREATED_DATE = existingZaap.CREATED_DATE;
                        zaapShftRptItem.CREATED_BY = existingZaap.CREATED_BY;

                    }
                    else
                    {
                        zaapShftRptItem.CREATED_BY = "PI";
                        zaapShftRptItem.CREATED_DATE = DateTime.Now;
                    }

                    _xmlMapper.InsertOrUpdate(zaapShftRptItem);

                }

                var newData = RecalculateZaapShftRpt();

                foreach (var production in newData)
                {
                    var existingProd = GetProductionExisting(production.FA_CODE, production.WERKS,
                        production.PRODUCTION_DATE);

                    if (existingProd != null)
                    {
                        production.MODIFIED_BY = "PI";
                        production.MODIFIED_DATE = DateTime.Now;

//                        production.
                    }
                }

                return items;
            }
        }


        public List<PRODUCTION> Items
        {
            get
            {
                var xmlRoot = _xmlMapper.GetElement("PRDOUTPUTDetails");
                var xmlItems = xmlRoot.Elements("row");
                var items = new List<PRODUCTION>();
                foreach (var xElement in xmlItems)
                {
                    try
                    {
                        var item = new PRODUCTION();
                        var zaapShftRptItem = new ZAAP_SHIFT_RPT();
                        item.WERKS = _xmlMapper.GetElementValue(xElement.Element("Plnt")); ;
                        item.FA_CODE = _xmlMapper.GetElementValue(xElement.Element("Material"));
                        item.BRAND_DESC = _xmlMapper.GetElementValue(xElement.Element("MaterialDescription"));
                        item.PRODUCTION_DATE = Convert.ToDateTime(_xmlMapper.GetDateDotSeparator(_xmlMapper.GetElementValue(xElement.Element("ProdDate"))));
                        var company = GetCompanyByPlant(item.WERKS);
                        if (company != null)
                        {
                            item.COMPANY_CODE = company.BUKRS;
                            item.COMPANY_NAME = company.T001.BUTXT;
                            item.PLANT_NAME = company.T001W.NAME1;
                        }

                        item.BATCH = _xmlMapper.GetElementValue(xElement.Element("Batch"));
                        
                        var bundle = _xmlMapper.GetElementValue(xElement.Element("Bundle"));
                        if(String.IsNullOrEmpty(bundle))
                            item.BUNDLE = null;
                        else
                        {
                            item.BUNDLE = Convert.ToInt32(bundle);
                        }

                        var mvt = _xmlMapper.GetElementValue(xElement.Element("MvT"));
                        var enteredOn = Convert.ToDateTime(_xmlMapper.GetDateDotSeparator(_xmlMapper.GetElementValue(xElement.Element("EnteredOn"))));
                        var postingDate = Convert.ToDateTime(_xmlMapper.GetDateDotSeparator(_xmlMapper.GetElementValue(xElement.Element("PostgDate")))); 
                        
                        //item.MARKET = _xmlMapper.GetElementValue(xElement.Element("Market"));
                        //item.DOCGMVTER = _xmlMapper.GetElementValue(xElement.Element("DocGMvtEr"));
                        //item.MATDOC = _xmlMapper.GetElementValue(xElement.Element("MatDoc"));
                        //item.ORDR = _xmlMapper.GetElementValue(xElement.Element("Order"));
                        //var shift = _xmlMapper.GetElementValue(xElement.Element("Shift"));
                        //if (!string.IsNullOrEmpty(shift))
                        //    item.LAST_SHIFT = GetShift(shift);
                        //else
                        //    item.LAST_SHIFT = 0;

                        var bun = _xmlMapper.GetElementValue(xElement.Element("BUn"));

                        decimal qty;
                        try
                        {
                            qty = Convert.ToDecimal(_xmlMapper.GetElementValue(xElement.Element("ConfQty")).Replace(",", ""));
                        }
                        catch (Exception ex)
                        {
                            string reason = "";
                            var qtyString = _xmlMapper.GetElementValue(xElement.Element("ConfQty"));
                            if (ex.GetType() == typeof(FormatException))
                                reason = "format not handled";
                            else
                            {
                                reason = "reason unknown";
                            }
                            _xmlMapper.Errors.Add(String.Format("failed to get qty : {0} value {1}", reason,qtyString));

                            continue;
                        }
                        
                        var existingBrand = GetMaterialBrand(item.FA_CODE, item.WERKS);
                        if (existingBrand != null)
                        {
                            if (existingBrand.BRAND_CONTENT == null)
                            {
                                throw new Exception(String.Format("Brand {0} - {2} in plant {1} has null CONTENT value", existingBrand.FA_CODE, existingBrand.WERKS, existingBrand.STICKER_CODE));
                            }

                            
                           
                            switch (bun)
                            {
                                case "TH":
                                    item.QTY = qty * 1000;
                                    item.UOM = "Btg";
                                    

                                    break;
                                case "KG":
                                    item.QTY = qty * 1000;
                                    item.UOM = "G";
                                    
                                    break;
                                default:
                                    item.QTY = qty;
                                    item.UOM = bun;

                                    
                                    break;
                            }
                            item.BRAND_DESC = existingBrand.BRAND_CE;

                            if (mvt == "101")
                            {
                                items.Add(item);    
                            }    
                            


                            zaapShftRptItem.MATDOC = _xmlMapper.GetElementValue(xElement.Element("MatDoc"));

                            zaapShftRptItem.BATCH = item.BATCH;
                            zaapShftRptItem.BUNDLE = item.BUNDLE;
                            //zaapShftRptItem.MARKET = _xmlMapper.GetElementValue(xElement.Element("Market"));
                            zaapShftRptItem.DOCGMVTER = _xmlMapper.GetElementValue(xElement.Element("DocGMvtEr"));

                            zaapShftRptItem.ORDR = _xmlMapper.GetElementValue(xElement.Element("Order"));
                            zaapShftRptItem.SHIFT = _xmlMapper.GetElementValue(xElement.Element("Shift"));
                            zaapShftRptItem.COMPANY_CODE = item.COMPANY_CODE;
                            zaapShftRptItem.FA_CODE = item.FA_CODE;
                            zaapShftRptItem.QTY = item.QTY;
                            zaapShftRptItem.UOM = item.UOM;
                            zaapShftRptItem.ORIGINAL_QTY = qty;
                            zaapShftRptItem.ORIGINAL_UOM = bun;
                            zaapShftRptItem.PRODUCTION_DATE = item.PRODUCTION_DATE;
                            zaapShftRptItem.WERKS = item.WERKS;
                            zaapShftRptItem.MVT = mvt;
                            zaapShftRptItem.POSTING_DATE = postingDate;
                            zaapShftRptItem.ENTERED_DATE = enteredOn;


                            var existingZaap = GetExistingZaapShiftRpt(zaapShftRptItem.MATDOC);
                            if (existingZaap != null)
                            {
                                zaapShftRptItem.MODIFIED_BY = "PI";
                                zaapShftRptItem.MODIFIED_DATE = DateTime.Now;
                                zaapShftRptItem.CREATED_DATE = existingZaap.CREATED_DATE;
                                zaapShftRptItem.CREATED_BY = existingZaap.CREATED_BY;

                            }
                            else
                            {
                                zaapShftRptItem.CREATED_BY = "PI";
                                zaapShftRptItem.CREATED_DATE = DateTime.Now;
                            }

                            _xmlMapper.InsertOrUpdate(zaapShftRptItem);
                        }
                        else
                        {
                            _xmlMapper.Errors.Add(string.Format("no brand fa_code {0} - werks {1}", item.FA_CODE,
                            item.WERKS));
                        }



                    }
                    catch (Exception ex)
                    {
                        string errorMessage = _xmlMapper.GetInnerException(ex);
                        _xmlMapper.Errors.Add(errorMessage);
                        

                    }


                }
                List<PRODUCTION> result = items
                    .GroupBy(l => new 
                    {
                        l.COMPANY_CODE,
                        l.WERKS,
                        l.PRODUCTION_DATE,
                        l.FA_CODE
                    })
                    .Select(cl => new PRODUCTION()
                            {
                                COMPANY_CODE = cl.First().COMPANY_CODE,
                                COMPANY_NAME = cl.First().COMPANY_NAME,
                                WERKS = cl.First().WERKS,
                                PLANT_NAME = cl.First().PLANT_NAME,
                                PRODUCTION_DATE = cl.First().PRODUCTION_DATE,
                                FA_CODE = cl.First().FA_CODE,
                                UOM = cl.First().UOM,
                                BATCH = cl.First().BATCH,
                                BRAND_DESC = cl.First().BRAND_DESC,
                                QTY = cl.Sum(x => x.QTY)
                                
                            }).ToList();
                RecalculateWithDatabase(result);
                
                return result;
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

        public MATERIAL_UOM GetMaterialUom(string materialNumber, string plant)
        {
            var existingData = _xmlMapper.uow.GetGenericRepository<MATERIAL_UOM>()
                .Get(p => p.STICKER_CODE == materialNumber && p.WERKS == plant
                          && p.MEINH == "BTG").FirstOrDefault();
            return existingData;
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
                    && x.PRODUCTION_DATE  == item.PRODUCTION_DATE
                    && x.POSTING_DATE == item.POSTING_DATE
                    && x.ENTERED_DATE == item.ENTERED_DATE
                    ,null,null).FirstOrDefault();

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

        private List<PRODUCTION> RecalculateZaapShftRpt()
        {
            var prodList = _xmlMapper.uow.GetGenericRepository<ZAAP_SHIFT_RPT>().Get(x=> x.MVT == "101").GroupBy(x => new
            {
                FA_CODE = x.FA_CODE,
                WERKS = x.WERKS,
                PRODUCTION_DATE = x.PRODUCTION_DATE
            }).Select(x=>
            {
                var zaapShiftRpt = x.FirstOrDefault();
                return zaapShiftRpt != null ? new PRODUCTION()
                              {
                                  FA_CODE = zaapShiftRpt.FA_CODE,
                                  WERKS = zaapShiftRpt.WERKS,
                                  PRODUCTION_DATE = zaapShiftRpt.PRODUCTION_DATE,
                                  QTY_PACKED = x.Sum(y=> y.QTY)
                              } : null;
            });

            return prodList.ToList();
        }
    }
}