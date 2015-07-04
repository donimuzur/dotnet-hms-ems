﻿using System;
using System.Collections.Generic;
using System.Linq;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
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
                var xmlItems = _xmlMapper.GetElements("ITEM");
                var items = new List<ZAIDM_EX_NPPBKC>();
                foreach (var xElement in xmlItems)
                {
                    var item = new ZAIDM_EX_NPPBKC();
                    item.NPPBKC_ID = xElement.Element("NPPBKC_ID").Value;
                    item.ADDR1 = xElement.Element("ADDR1").Value;
                    item.ADDR2 = xElement.Element("ADDR2").Value;
                    item.CITY = xElement.Element("CITY").Value;
                    var kppbcNo = xElement.Element("KPPBC_ID").Value;
                    var kppbc = new XmlKPPBCDataMapper(null
                        ).GetKPPBC(kppbcNo);
                    if(kppbc == null)
                        throw new Exception("no existing KPPBC NO " + kppbcNo);
                    item.KPPBC_ID = kppbc.KPPBC_ID;
                    var companyCode = xElement.Element("BUKRS").Value;
                    
                    var dateXml = Convert.ToDateTime(xElement.Element("MODIFIED_DATE").Value); 
                    var exisitingNppbkc = GetNPPBKC(item.NPPBKC_ID);
                    if (exisitingNppbkc != null)
                    {
                        if (dateXml > exisitingNppbkc.CREATED_DATE)
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
            _xmlMapper.InsertToDatabase<ZAIDM_EX_NPPBKC>(Items);
        }

        public ZAIDM_EX_NPPBKC GetNPPBKC(string Number)
        {
            var existing = _xmlMapper.uow.GetGenericRepository<ZAIDM_EX_NPPBKC>()
                .GetByID(Number);
            return existing;
        }





    }
}
