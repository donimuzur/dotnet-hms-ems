using System;
using System.Collections.Generic;
using System.Linq;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Outputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
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
                var xmlRoot = _xmlMapper.GetElement("IDOC");
                var xmlItems = xmlRoot.Elements("Z1A_PRODTYP");
                var items = new List<ZAIDM_EX_PRODTYP>();
                foreach (var xElement in xmlItems)
                {
                    try
                    {
                        var item = new ZAIDM_EX_PRODTYP();
                        item.PROD_CODE = xElement.Element("PROD_CODE").Value;
                        item.PRODUCT_TYPE = _xmlMapper.GetElementValue(xElement.Element("PRODUCT_TYPE"));
                        item.PRODUCT_ALIAS = _xmlMapper.GetElementValue(xElement.Element("PRODUCT_ALIAS"));
                        item.CREATED_BY = Constans.PI;
                        var existingProdType = GetProdType(item.PROD_CODE);
                        if (existingProdType != null)
                        {
                            item.CREATED_DATE = existingProdType.CREATED_DATE;
                            item.MODIFIED_DATE = DateTime.Now;
                            item.CREATED_BY = existingProdType.CREATED_BY;
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


        public MovedFileOutput InsertToDatabase()
        {
            return _xmlMapper.InsertToDatabase<ZAIDM_EX_PRODTYP>(Items);
        }

        public List<string> GetErrorList()
        {
            return  _xmlMapper.Errors;
        }

        public ZAIDM_EX_PRODTYP GetProdType(string ProdTypeCode)
        {
            var exisitingPlant = _xmlMapper.uow.GetGenericRepository<ZAIDM_EX_PRODTYP>()
                .GetByID(ProdTypeCode);
            return exisitingPlant;
        }





    }
}
