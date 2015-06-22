using System;
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
                    item.NPPBKC_NO = xElement.Element("NPPBKC_NO").Value;
                    item.ADDR1 = xElement.Element("ADDR1").Value;
                    item.ADDR2 = xElement.Element("ADDR2").Value;
                    item.CITY = xElement.Element("CITY").Value;
                    var kppbc = new XmlKPPBCDataMapper(null
                        ).GetKPPBC(xElement.Element("KPPBC_NO").Value);
                    if(kppbc == null)
                        continue;
                    item.KPPBC_ID = kppbc.KPPBC_ID;
                    var company = new XmlCompanyDataMapper(null).GetCompany(xElement.Element("BUKRS").Value);
                    if(company == null)
                        continue;
                    item.COMPANY_ID = company.COMPANY_ID;

                    item.CREATED_DATE = DateTime.Now;
                    var dateXml = DateTime.MinValue;
                    DateTime.TryParse(xElement.Element("MODIFIED_DATE").Value, out dateXml);
                    var exisitingNppbkc = GetNPPBKC(item.NPPBKC_NO);
                    if (exisitingNppbkc != null)
                    {
                        if (dateXml > exisitingNppbkc.CREATED_DATE)
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
            _xmlMapper.InsertToDatabase<ZAIDM_EX_NPPBKC>(Items);
        }

        public ZAIDM_EX_NPPBKC GetNPPBKC(string Number)
        {
            var existing = _xmlMapper.uow.GetGenericRepository<ZAIDM_EX_NPPBKC>()
                          .Get(p => p.NPPBKC_NO == Number)
                          .OrderByDescending(p => p.CREATED_DATE)
                          .FirstOrDefault();
            return existing;
        }





    }
}
