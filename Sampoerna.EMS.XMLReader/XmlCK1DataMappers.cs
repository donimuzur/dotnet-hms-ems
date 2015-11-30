using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Linq;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Outputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.DAL;
using Voxteneo.WebComponents.Logger;
namespace Sampoerna.EMS.XMLReader
{
    public class XmlCK1DataMapper : IXmlDataReader 
    {
        private XmlDataMapper _xmlMapper = null;

        public XmlCK1DataMapper(string fileName)
        {
            _xmlMapper = new XmlDataMapper(fileName);
           
        }

        
        public List<CK1> Items
        {
            get
            {
                var xmlRoot = _xmlMapper.GetElement("IDOC");
                var xmlRoot2 = xmlRoot.Elements("Z1A_EKKO_SSPCP");

                var items = new List<CK1>();
                foreach (var xElement in xmlRoot2)
                {
                    try
                    {
                        var item = new CK1();
                        var ck1SapNumber = _xmlMapper.GetElementValue(xElement.Element("EBELN"));
                        var ck1Number = _xmlMapper.GetElementValue(xElement.Element("ZZCK1_CE_OFFICE"));

                        if (ck1Number == null)
                        {
                            throw new Exception(String.Format("ck1 number cannot be null, field (ZZCK1_CE_OFFICE)"));
                        }
                        else
                        {
                            item.CK1_NUMBER = ck1Number;    
                        }

                        if (ck1SapNumber == null)
                        {
                            throw new Exception(String.Format("ck1 SAP number cannot be null, field (EBELN)"));
                        }
                        else
                        {
                            item.CK1_SAP_NUMBER = ck1SapNumber;
                        }
                        
                        item.CK1_DATE = Convert.ToDateTime(_xmlMapper.GetDate(_xmlMapper.GetElementValue(xElement.Element("AEDAT"))));
                        item.PLANT_ID = _xmlMapper.GetElementValue(xElement.Element("WERKS"));
                        item.CREATED_BY = Constans.PI;
                        item.COMPANY_ID = _xmlMapper.GetElementValue(xElement.Element("BUKRS"));
                        item.NPPBKC_ID = _xmlMapper.GetElementValue(xElement.Element("NPPBKC_ID"));

                        var existingNppbkc = getNppbkc(item.NPPBKC_ID);
                        if (existingNppbkc == null)
                        {
                            throw new Exception(String.Format("{0} not exist in EMS database",item.NPPBKC_ID));
                        }

                        item.VENDOR_ID = _xmlMapper.GetElementValue(xElement.Element("LIFNR"));
                        item.ORDER_DATE = _xmlMapper.GetDate(_xmlMapper.GetElementValue(xElement.Element("ORDER_DATE")));
                        var company = _xmlMapper.uow.GetGenericRepository<T001>().GetByID(item.COMPANY_ID);
                        if (company != null)
                        {

                            item.COMPANY_NAME = company.BUTXT;
                        }

                        

                        var xmlItems = xElement.Elements("Z1A_EKPO_SSPCP");

                        var existingData = GetCk1(item.CK1_NUMBER);
                        if (existingData != null)
                        {
                            item.CK1_ITEM = existingData.CK1_ITEM;
                        }
                        else
                        {
                            item.CK1_ITEM = new List<CK1_ITEM>();
                        }
                        
                        foreach (var xElementItem in xmlItems)
                        {
                            var detail = new CK1_ITEM();
                            
                            

                            detail.FA_CODE = _xmlMapper.GetElementValue(xElementItem.Element("FA_CODE"));
                            detail.MATERIAL_ID = _xmlMapper.GetElementValue(xElementItem.Element("MATNR"));
                            detail.WERKS = _xmlMapper.GetElementValue(xElementItem.Element("WERKS"));
                            detail.UOM = _xmlMapper.GetElementValue(xElementItem.Element("MEINS"));
                            var existingBrand = getBrand(detail.MATERIAL_ID, detail.FA_CODE, detail.WERKS);
                            if (existingBrand == null)
                            {
                                throw new Exception(String.Format("{0} - {1} in {2} not exist in EMS Brand registration master", detail.MATERIAL_ID, detail.FA_CODE,detail.WERKS));
                            }

                            var lembaran = Convert.ToDecimal(_xmlMapper.GetElementValue(xElementItem.Element("MENGE")));
                            
                            //////
                            if (detail.UOM.ToUpper() == "PC")
                            {
                                detail.MENGE = lembaran;
                            }
                            else
                            {
                                var seriesValue = existingBrand.ZAIDM_EX_SERIES.SERIES_VALUE.HasValue
                                ? existingBrand.ZAIDM_EX_SERIES.SERIES_VALUE.Value
                                : 0;
                                detail.MENGE = lembaran * seriesValue;
                            }
                            
                            //detail.MENGE = lembaran;
                            
                            
                            item.CK1_ITEM.Add(detail);
                        }
                        

                        if (existingData != null)
                        {
                            item.CREATED_BY = existingData.CREATED_BY;
                            item.CREATED_DATE = existingData.CREATED_DATE;
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


        public MovedFileOutput InsertToDatabase()
        {
           return _xmlMapper.InsertToDatabase<CK1>(Items);
        }

        public List<string> GetErrorList()
        {
            return _xmlMapper.Errors;
        }

        public CK1 GetCk1(string ck1Number)
        {
            var existingData = _xmlMapper.uow.GetGenericRepository<CK1>()
                .Get(x=>x.CK1_NUMBER == ck1Number ).FirstOrDefault();
            return existingData;
        }

        public ZAIDM_EX_NPPBKC getNppbkc(string nppbkcId)
        {
            var existingData = _xmlMapper.uow.GetGenericRepository<ZAIDM_EX_NPPBKC>()
                .Get(x => x.NPPBKC_ID == nppbkcId).FirstOrDefault();

            return existingData;
        }

        public ZAIDM_EX_BRAND getBrand(string stickerCode,string brand, string werks)
        {
            var existingBrand = _xmlMapper.uow.GetGenericRepository<ZAIDM_EX_BRAND>()
                .Get(x => x.FA_CODE == brand && x.WERKS == werks && x.STICKER_CODE == stickerCode,null,"ZAIDM_EX_SERIES").FirstOrDefault();

            return existingBrand;
        }

    }
}
