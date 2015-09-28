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
                var xmlRoot = _xmlMapper.GetElement("IDOC");
                var xmlItems = xmlRoot.Elements("Z1A_KPPBC");
                var items = new List<ZAIDM_EX_KPPBC>();

                foreach (var xElement in xmlItems)
                {
                    try
                    {
                        var item = new ZAIDM_EX_KPPBC();
                        item.KPPBC_ID = xElement.Element("KPPBC_ID").Value;
                        item.KPPBC_TYPE = _xmlMapper.GetElementValue(xElement.Element("KPPBC_TYPE"));
                        item.MENGETAHUI = _xmlMapper.GetElementValue(xElement.Element("MENGETAHUI"));
                        item.CK1_KEP_FOOTER = _xmlMapper.GetElementValue(xElement.Element("CK1_KEP_FOOTER"));
                        item.CK1_KEP_HEADER = _xmlMapper.GetElementValue(xElement.Element("CK1_KEP_HEADER"));
                        item.CREATED_BY = Constans.PI;
                        var existingKppbc = GetKPPBC(item.KPPBC_ID);
                        if (existingKppbc != null)
                        {
                            item.CREATED_BY = existingKppbc.CREATED_BY;
                            item.CREATED_DATE = existingKppbc.CREATED_DATE;
                            item.MENGETAHUI_DETAIL = existingKppbc.MENGETAHUI_DETAIL;
                            item.MODIFIED_DATE = DateTime.Now;
                            item.MODIFIED_BY = Constans.PI;
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

      
        public string InsertToDatabase()
        {
            return _xmlMapper.InsertToDatabase<ZAIDM_EX_KPPBC>(Items);
        }

        public List<string> GetErrorList()
        {
            return _xmlMapper.Errors;
        }

        public ZAIDM_EX_KPPBC GetKPPBC(string KppbcId)
        {
            var exisitingPlant = _xmlMapper.uow.GetGenericRepository<ZAIDM_EX_KPPBC>()
                .GetByID(KppbcId);
            return exisitingPlant;
        }





    }
}
