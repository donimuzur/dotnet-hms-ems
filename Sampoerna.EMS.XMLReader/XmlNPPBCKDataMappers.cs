using System;
using System.Collections.Generic;
using System.Linq;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.DAL;
using Voxteneo.WebComponents.Logger;
namespace Sampoerna.EMS.XMLReader
{
    public class XmlNPPBKCDataMapper : IXmlDataReader 
    {
        private XmlDataMapper _xmlMapper = null;

        public XmlNPPBKCDataMapper(string filename)
        {
            _xmlMapper = new XmlDataMapper(filename);
           
        }


        public List<ZAIDM_EX_NPPBKC> Items
        {
            get
            {
                var xmlRoot = _xmlMapper.GetElement("IDOC");
                var xmlItems = xmlRoot.Elements("Z1A_NPPBKC");
                var items = new List<ZAIDM_EX_NPPBKC>();
              
                try
                {
                   
                    foreach (var xElement in xmlItems)
                    {
                        try
                        {
                            var item = new ZAIDM_EX_NPPBKC();
                            item.NPPBKC_ID = xElement.Element("NPPBKC_ID").Value;
                            item.ADDR1 = xElement.Element("ADDR1") == null ? null : xElement.Element("ADDR1").Value;
                            item.ADDR2 = xElement.Element("ADDR2") == null ? null : xElement.Element("ADDR2").Value;
                            item.CITY = xElement.Element("CITY") == null ? null : xElement.Element("CITY").Value;
                            item.REGION = xElement.Element("REGION") == null ? null : xElement.Element("REGION").Value;
                            var kppbcNo = xElement.Element("KPPBC_ID") == null ? null : xElement.Element("KPPBC_ID").Value;
                            var kppbc = new XmlKPPBCDataMapper(null
                                ).GetKPPBC(kppbcNo);
                            if (kppbc == null)
                            {
                                //insert kppbc
                                var kppbcItem = new ZAIDM_EX_KPPBC();
                                kppbcItem.KPPBC_ID = kppbcNo;
                                kppbcItem.CREATED_DATE = DateTime.Now;
                                _xmlMapper.InsertToDatabase(kppbcItem);
                            }
                            item.KPPBC_ID = new XmlKPPBCDataMapper(null
                                ).GetKPPBC(kppbcNo).KPPBC_ID;

                            item.START_DATE = _xmlMapper.GetDate(xElement.Element("START_DATE").Value);
                            item.END_DATE = _xmlMapper.GetDate(xElement.Element("END_DATE").Value);
                           
                            var exisitingNppbkc = GetNPPBKC(item.NPPBKC_ID);
                            if (exisitingNppbkc != null)
                            {
                                item.CITY_ALIAS = exisitingNppbkc.CITY_ALIAS;
                                item.TEXT_TO = exisitingNppbkc.TEXT_TO;
                                item.REGION_DGCE = exisitingNppbkc.REGION_DGCE;
                                item.CREATED_DATE = exisitingNppbkc.CREATED_DATE;
                                item.MODIFIED_DATE = DateTime.Now;
                                item.MODIFIED_BY = Constans.PICreator;
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
                            continue;
                            
                        }
                        
                    }
                }
                catch (Exception ex)
                {
                    
                }
               
                return items;
            }
             
        }

      
        public string InsertToDatabase()
        {
            return _xmlMapper.InsertToDatabase<ZAIDM_EX_NPPBKC>(Items);
        }

        public ZAIDM_EX_NPPBKC GetNPPBKC(string Number)
        {
            var existing = _xmlMapper.uow.GetGenericRepository<ZAIDM_EX_NPPBKC>()
                .GetByID(Number);
            return existing;
        }





    }
}
