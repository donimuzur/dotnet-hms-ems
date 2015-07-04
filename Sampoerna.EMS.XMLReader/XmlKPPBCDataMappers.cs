﻿using System;
using System.Collections.Generic;
using System.Linq;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.DAL;
using Voxteneo.WebComponents.Logger;
namespace Sampoerna.EMS.XMLReader
{
    public class XmlKPPBCDataMapper : IXmlDataReader 
    {
        private XmlDataMapper _xmlMapper = null;

        public XmlKPPBCDataMapper(string filename)
        {
            _xmlMapper = new XmlDataMapper(filename);
           
        }


        public List<ZAIDM_EX_KPPBC> Items
        {
            get
            {
                var xmlItems = _xmlMapper.GetElements("ITEM");
                var items = new List<ZAIDM_EX_KPPBC>();
                foreach (var xElement in xmlItems)
                {
                    var item = new ZAIDM_EX_KPPBC();
                    item.KPPBC_ID = xElement.Element("KPPBC_ID").Value;
                    item.KPPBC_TYPE = xElement.Element("KPPBC_TYPE").Value;
                    item.CK1_KEP_FOOTER = xElement.Element("CK1_KEP_FOOTER").Value;
                    item.CK1_KEP_HEADER = xElement.Element("CK1_KEP_HEADER").Value;
                    item.MODIFIED_DATE = DateTime.Now;
                    var dateXml = Convert.ToDateTime(xElement.Element("MODIFIED_DATE").Value); ;
                    var existingKppbc = GetKPPBC(item.KPPBC_ID);
                    if (existingKppbc != null)
                    {
                        if (dateXml > existingKppbc.MODIFIED_DATE)
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
            _xmlMapper.InsertToDatabase<ZAIDM_EX_KPPBC>(Items);
        }

        public ZAIDM_EX_KPPBC GetKPPBC(string KppbcId)
        {
            var exisitingPlant = _xmlMapper.uow.GetGenericRepository<ZAIDM_EX_KPPBC>()
                .GetByID(KppbcId);
            return exisitingPlant;
        }





    }
}
