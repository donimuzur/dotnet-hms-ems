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
                        item.BUNDLE = Convert.ToInt32(_xmlMapper.GetElementValue(xElement.Element("Bundle")));
                        item.MARKET = _xmlMapper.GetElementValue(xElement.Element("Market"));
                        item.DOCGMVTER = _xmlMapper.GetElementValue(xElement.Element("DocGMvtEr"));
                        item.MATDOC = _xmlMapper.GetElementValue(xElement.Element("MatDoc"));
                        item.ORDR = _xmlMapper.GetElementValue(xElement.Element("Order"));
                        
                        var bun = _xmlMapper.GetElementValue(xElement.Element("BUn"));
                        var qty = Convert.ToDecimal(_xmlMapper.GetElementValue(xElement.Element("Quantity")));
                        var existingProduction = GetProductionExisting(item.FA_CODE, item.WERKS, item.COMPANY_CODE,
                            item.PRODUCTION_DATE);
                        
                            //var prodQty = qty;
                        var existingBrand = GetMaterialBrand(item.FA_CODE, item.WERKS);
                        if (existingBrand != null)
                        {
                            switch (bun)
                            {
                                case "TH":
                                    item.QTY = qty * 1000;
                                    item.UOM = "PC";
                                    if (existingProduction == null)
                                    {
                                        item.PROD_QTY_STICK = item.QTY;
                                        item.QTY_PACKED = item.QTY/Convert.ToDecimal(existingBrand.BRAND_CONTENT);
                                    }
                                    else
                                    {
                                        item.PROD_QTY_STICK = existingProduction.PROD_QTY_STICK;
                                        item.QTY_PACKED = existingProduction.QTY_PACKED;
                                        item.QTY_UNPACKED = existingProduction.QTY_UNPACKED;
                                    }

                                    break;
                                case "KG":
                                    item.QTY = qty * 1000;
                                    item.UOM = "G";

                                    item.QTY_PACKED = existingProduction.QTY_PACKED;
                                    item.QTY_UNPACKED = existingProduction.QTY_UNPACKED;
                                    break;
                                default:
                                    item.QTY = qty;
                                    item.UOM = bun;

                                    item.QTY_PACKED = existingProduction.QTY_PACKED;
                                    item.QTY_UNPACKED = existingProduction.QTY_UNPACKED;
                                    break;
                            }


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
                        continue;

                    }


                }
                return items;
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
                .GetByID(plant,materialNumber);
            return existingData;
        }

        public T001K GetCompanyByPlant(string plant)
        {
            var existingData = _xmlMapper.uow.GetGenericRepository<T001K>()
                .Get(x=>x.BWKEY == plant, null, "T001, T001W").FirstOrDefault();
            return existingData;
        }

        public PRODUCTION GetProductionExisting(string faCode, string plantid, string companyCode, DateTime prodDate)
        {
            var existingData = _xmlMapper.uow.GetGenericRepository<PRODUCTION>()
                .Get(
                    x =>
                        x.COMPANY_CODE == companyCode && x.WERKS == plantid && x.FA_CODE == faCode &&
                        x.PRODUCTION_DATE == prodDate).FirstOrDefault();
            return existingData;
        }
    }
}