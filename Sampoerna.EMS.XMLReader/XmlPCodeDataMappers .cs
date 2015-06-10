using System;
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

        public XmlPCodeDataMapper()
        {
            _xmlMapper = new XmlDataMapper("ZAIDM_EX_PCODE");
           
        }


        public List<ZAIDM_EX_PCODE> Items
        {
            get
            {
                var xmlItems = _xmlMapper.GetElements("ITEM");
                var items = new List<ZAIDM_EX_PCODE>();
                foreach (var xElement in xmlItems)
                {
                    var item = new ZAIDM_EX_PCODE();
                    item.PER_CODE = Convert.ToInt32(xElement.Element("PER_CODE").Value);
                    item.PER_DESC = xElement.Element("PER_DESC").Value;
                    item.CREATED_DATE = DateTime.Now;
                    var dateXml = DateTime.MinValue;
                    DateTime.TryParse(xElement.Element("MODIFIED_DATE").Value, out dateXml);
                    var existingPCode = GetPCode(item.PER_CODE);
                    if (existingPCode != null)
                    {
                        if (dateXml > existingPCode.CREATED_DATE)
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
            _xmlMapper.InsertToDatabase<ZAIDM_EX_PCODE>(Items);
        }

        public ZAIDM_EX_PCODE GetPCode(int? PCode)
        {
            var exisitingPlant = _xmlMapper.uow.GetGenericRepository<ZAIDM_EX_PCODE>()
                          .Get(p => p.PER_CODE == PCode)
                          .OrderByDescending(p => p.CREATED_DATE)
                          .FirstOrDefault();
            return exisitingPlant;
        }





    }
}
