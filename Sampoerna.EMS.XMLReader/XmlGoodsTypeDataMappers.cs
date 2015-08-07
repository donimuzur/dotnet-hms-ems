using System;
using System.Collections.Generic;
using System.Linq;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.DAL;
using Voxteneo.WebComponents.Logger;
namespace Sampoerna.EMS.XMLReader
{
    public class XmlGoodsTypeDataMapper : IXmlDataReader 
    {
        private XmlDataMapper _xmlMapper = null;

        public XmlGoodsTypeDataMapper(string filename)
        {
            _xmlMapper = new XmlDataMapper(filename);
           
        }


        public List<ZAIDM_EX_GOODTYP> Items

        {
            get
            {
                var xmlRoot = _xmlMapper.GetElement("IDOC");
                
                var xmlItems = xmlRoot.Elements("Z1A_GOODTYP");
                var items = new List<ZAIDM_EX_GOODTYP>();
                foreach (var xElement in xmlItems)
                {
                    var item = new ZAIDM_EX_GOODTYP();
                    item.EXC_GOOD_TYP = xElement.Element("EXC_GOOD_TYP").Value;
                    item.EXT_TYP_DESC = xElement.Element("EXC_TYP_DESC") == null ? null : xElement.Element("EXC_TYP_DESC").Value;
                    //var dateXml = Convert.ToDateTime(xElement.Element("MODIFIED_DATE").Value); 
                    var existingGoodsType = GetGoodsType(item.EXC_GOOD_TYP);
                    if (existingGoodsType != null)
                    {
                        
                        item.CREATED_DATE = existingGoodsType.CREATED_DATE;
                        item.MODIFIED_DATE = DateTime.Now;
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
            _xmlMapper.InsertToDatabase<ZAIDM_EX_GOODTYP>(Items);
        }

        public ZAIDM_EX_GOODTYP GetGoodsType(string Code)
        {
            var exisitingPlant = _xmlMapper.uow.GetGenericRepository<ZAIDM_EX_GOODTYP>()
                .GetByID(Code);
            return exisitingPlant;
        }





    }
}
