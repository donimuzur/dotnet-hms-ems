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
                        item.PER_DESC = _xmlMapper.GetElementValue(xElement.Element("PER_DESC"));
                        item.CREATED_DATE = DateTime.Now;
                        item.CREATED_BY = Constans.PICreator;
                        var existingPCode = GetPCode(item.PER_CODE);
                        if (existingPCode != null)
                        {
                            item.CREATED_DATE = existingPCode.CREATED_DATE;
                            item.MODIFIED_BY = Constans.PICreator;
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
