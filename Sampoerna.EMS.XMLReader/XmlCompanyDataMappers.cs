﻿using System;
using System.Collections.Generic;
using System.Linq;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.DAL;
using Voxteneo.WebComponents.Logger;
namespace Sampoerna.EMS.XMLReader
{
    public class XmlCompanyDataMapper : IXmlDataReader 
    {
        private XmlDataMapper _xmlMapper = null;
      
        public XmlCompanyDataMapper()
        {
            _xmlMapper = new XmlDataMapper("T1001");
           
        }

        
        public List<T1001> Items
        {
            get
            {
                var xmlItems = _xmlMapper.GetElements("ITEM");
                var items = new List<T1001>();
                foreach (var xElement in xmlItems)
                {
                    var item = new T1001();
                    item.BUKRS = xElement.Element("BUKRS").Value;
                    item.BUKRSTXT = xElement.Element("BUTXT").Value;
                    item.CREATED_DATE = DateTime.Now;
                    var companyDateXml = DateTime.MinValue;
                    DateTime.TryParse(xElement.Element("CHANGES_DATE").Value, out companyDateXml);
                    var exisitingCompany = _xmlMapper.uow.GetGenericRepository<T1001>()
                           .Get(p => p.BUKRS == item.BUKRS)
                           .OrderByDescending(p => p.CREATED_DATE)
                           .FirstOrDefault();

                    if (exisitingCompany != null)
                    {
                        if (companyDateXml > exisitingCompany.CREATED_DATE)
                        {
                            items.Add(item);
                        }
                        else
                        {
                            continue;

                        }
                    }
                    else
                    {
                        items.Add(item);
                    }
                   
                }
                return items;
            }
             
        }

      
        public void InsertToDatabase()
        {
          _xmlMapper.InsertToDatabase<T1001>(Items);
        }





    }
}
