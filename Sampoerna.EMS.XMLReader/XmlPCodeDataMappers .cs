﻿using System;
using System.Collections.Generic;
using System.Linq;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.DAL;
using Voxteneo.WebComponents.Logger;
namespace Sampoerna.EMS.XMLReader
{
    public class XmlPCodeDataMapper : IXmlDataReader 
    {
        private XmlDataMapper _xmlMapper = null;

        public XmlPCodeDataMapper(string filename)
        {
            _xmlMapper = new XmlDataMapper(filename);
           
        }


        public List<ZAIDM_EX_PCODE> Items
        {
            get
            {
                var xmlRoot = _xmlMapper.GetElement("IDOC");
                var xmlItems = xmlRoot.Elements("Z1A_PCODE");
                var items = new List<ZAIDM_EX_PCODE>();
                foreach (var xElement in xmlItems)
                {
                    try
                    {
                        var item = new ZAIDM_EX_PCODE();
                        item.PER_CODE = xElement.Element("PER_CODE").Value;
                        item.PER_DESC = xElement.Element("PER_DESC").Value;
                        item.CREATED_DATE = DateTime.Now;
                        //var dateXml =  Convert.ToDateTime(xElement.Element("MODIFIED_DATE").Value); 
                        var existingPCode = GetPCode(item.PER_CODE);
                        if (existingPCode != null)
                        {
                            item.CREATED_DATE = existingPCode.CREATED_DATE;
                            item.MODIFIED_DATE = DateTime.Now;
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
                return items;
            }
             
        }

      
        public string InsertToDatabase()
        {
           return _xmlMapper.InsertToDatabase<ZAIDM_EX_PCODE>(Items);
        }

        public ZAIDM_EX_PCODE GetPCode(string PCode)
        {
            var exisitingPlant = _xmlMapper.uow.GetGenericRepository<ZAIDM_EX_PCODE>()
                .GetByID(PCode);
            return exisitingPlant;
        }





    }
}
