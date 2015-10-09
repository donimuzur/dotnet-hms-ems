﻿using System;
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
                foreach (var xElement in xmlItems)
                {
                    try
                    {
                        var item = new PRODUCTION();

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
                            qty = Convert.ToDecimal(_xmlMapper.GetElementValue(xElement.Element("Quantity")).Replace(",",""));
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

                            

                            

                            
                            
                                
                            items.Add(item);
                            
                        }
                        else
                        {
                            _xmlMapper.Errors.Add(string.Format("no brand fa_code {0} - werks {1}", item.FA_CODE,
                            item.WERKS));
                        }



                    }
                    catch (Exception ex)
                    {
                        _xmlMapper.Errors.Add(ex.Message);
                        

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
                                WERKS = cl.First().WERKS,
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


        public string InsertToDatabase()
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

        private void RecalculateWithDatabase(List<PRODUCTION> itemList)
        {
            foreach (var item in itemList)
            {
                var existingProduction = GetProductionExisting(item.FA_CODE, item.WERKS,item.PRODUCTION_DATE);
                
                if (existingProduction == null)
                {
                                
                    item.CREATED_DATE = DateTime.Now;
                    item.CREATED_BY = "PI";
                }
                else
                {
                    item.QTY = existingProduction.QTY + item.QTY;
                    item.MODIFIED_DATE = DateTime.Now;
                    item.MODIFIED_BY = "PI";
                    item.CREATED_BY = existingProduction.CREATED_BY;
                    item.CREATED_DATE = existingProduction.CREATED_DATE;
                                
                }

                switch (item.UOM)
                {
                    case "TH":
                        
                        if (existingProduction == null)
                        {
                            item.PROD_QTY_STICK = item.QTY;
                                        
                        }
                        else
                        {
                                        
                            //item.QTY_PACKED = existingProduction.QTY_PACKED;
                            item.QTY_UNPACKED = existingProduction.QTY_UNPACKED;
                        }

                        break;
                    case "KG":
                        
                        if (existingProduction != null)
                        {

                            item.QTY_PACKED = item.QTY;
                            //item.QTY_UNPACKED = existingProduction.QTY_UNPACKED;
                                
                        }
                        break;
                    default:
                        

                        if (existingProduction != null)
                        {
                            //item.QTY_PACKED = existingProduction.QTY_PACKED;
                            item.QTY_UNPACKED = existingProduction.QTY_UNPACKED;
                        }
                        break;
                }
                var existingBrand = GetMaterialBrand(item.FA_CODE, item.WERKS);
                if (existingBrand != null)
                {
                    var tempPack = decimal.Floor(item.QTY.Value/decimal.Parse(existingBrand.BRAND_CONTENT));
                    var tempQtyPacked = tempPack*int.Parse(existingBrand.BRAND_CONTENT);

                    item.QTY_PACKED = tempQtyPacked;
                    item.PROD_QTY_STICK = item.QTY;
                }
            }
            
        }
    }
}