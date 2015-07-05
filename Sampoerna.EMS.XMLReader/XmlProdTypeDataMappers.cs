using System;
using System.Collections.Generic;
using System.Linq;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.DAL;
using Voxteneo.WebComponents.Logger;
namespace Sampoerna.EMS.XMLReader
{
    public class XmlProdTypeDataMapper : IXmlDataReader 
    {
        private XmlDataMapper _xmlMapper = null;

        public XmlProdTypeDataMapper(string filename)
        {
            _xmlMapper = new XmlDataMapper(filename);
           
        }


        public List<ZAIDM_EX_PRODTYP> Items
        {
            get
            {
                var xmlItems = _xmlMapper.GetElements("ITEM");
                var items = new List<ZAIDM_EX_PRODTYP>();
                foreach (var xElement in xmlItems)
                {
                    var item = new ZAIDM_EX_PRODTYP();
                    item.PROD_CODE = Convert.ToInt32(xElement.Element("PROD_CODE").Value);
                    item.PRODUCT_TYPE = xElement.Element("PRODUCT_TYPE").Value;
                    item.PRODUCT_ALIAS = xElement.Element("PRODUCT_ALIAS").Value;
                    var dateXml = Convert.ToDateTime(xElement.Element("MODIFIED_DATE").Value); 
                    var existingProdType = GetProdType(item.PROD_CODE);
                    if (existingProdType != null)
                    {
                        item.CREATED_DATE = existingProdType.CREATED_DATE;
                        item.MODIFIED_DATE = dateXml;
                        items.Add(item);
                        
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
            _xmlMapper.InsertToDatabase<ZAIDM_EX_PRODTYP>(Items);
        }

        public ZAIDM_EX_PRODTYP GetProdType(int? ProdTypeCode)
        {
            var exisitingPlant = _xmlMapper.uow.GetGenericRepository<ZAIDM_EX_PRODTYP>()
                .GetByID(ProdTypeCode);
            return exisitingPlant;
        }





    }
}
