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
                var xmlItems = _xmlMapper.GetElements("ITEM");
                var items = new List<ZAIDM_EX_GOODTYP>();
                foreach (var xElement in xmlItems)
                {
                    var item = new ZAIDM_EX_GOODTYP();
                    item.EXC_GOOD_TYP = Convert.ToInt32(xElement.Element("EXC_GOOD_TYP").Value);
                    item.EXT_TYP_DESC = xElement.Element("EXT_TYP_DESC").Value;
                    item.CREATED_DATE = DateTime.Now;
                    var dateXml = Convert.ToDateTime(xElement.Element("MODIFIED_DATE").Value); 
                     var existingGoodsType = GetGoodsType(item.EXC_GOOD_TYP);
                    if (existingGoodsType != null)
                    {
                        if (dateXml > existingGoodsType.CREATED_DATE)
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
            _xmlMapper.InsertToDatabase<ZAIDM_EX_GOODTYP>(Items);
        }

        public ZAIDM_EX_GOODTYP GetGoodsType(int? Code)
        {
            var exisitingPlant = _xmlMapper.uow.GetGenericRepository<ZAIDM_EX_GOODTYP>()
                          .Get(p => p.EXC_GOOD_TYP == Code)
                          .OrderByDescending(p => p.CREATED_DATE)
                          .FirstOrDefault();
            return exisitingPlant;
        }





    }
}
