using System;
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
                var xmlRoot = _xmlMapper.GetElement("IDOC");
                var xmlItems = xmlRoot.Elements("Z1A_KPPBC");
                var items = new List<ZAIDM_EX_KPPBC>();

                foreach (var xElement in xmlItems)
                {
                    try
                    {
                        var item = new ZAIDM_EX_KPPBC();
                        item.KPPBC_ID = xElement.Element("KPPBC_ID").Value;
                        item.KPPBC_TYPE = xElement.Element("KPPBC_TYPE") == null ? null : xElement.Element("KPPBC_TYPE").Value;
                        item.MENGETAHUI = xElement.Element("MENGETAHUI") == null ? null : xElement.Element("MENGETAHUI").Value;
                        item.CK1_KEP_FOOTER = xElement.Element("CK1_KEP_FOOTER") == null ? null : xElement.Element("CK1_KEP_FOOTER").Value;
                        item.CK1_KEP_HEADER = xElement.Element("CK1_KEP_HEADER") == null ? null : xElement.Element("CK1_KEP_HEADER").Value;
                        //item.MODIFIED_DATE = DateTime.Now;
                        // var dateXml = Convert.ToDateTime(xElement.Element("MODIFIED_DATE").Value); ;
                        var existingKppbc = GetKPPBC(item.KPPBC_ID);
                        if (existingKppbc != null)
                        {
                            item.CREATED_DATE = existingKppbc.CREATED_DATE;
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
            return _xmlMapper.InsertToDatabase<ZAIDM_EX_KPPBC>(Items);
        }

        public ZAIDM_EX_KPPBC GetKPPBC(string KppbcId)
        {
            var exisitingPlant = _xmlMapper.uow.GetGenericRepository<ZAIDM_EX_KPPBC>()
                .GetByID(KppbcId);
            return exisitingPlant;
        }





    }
}
