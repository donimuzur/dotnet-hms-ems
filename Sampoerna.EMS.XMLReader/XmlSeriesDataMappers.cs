using System;
using System.Collections.Generic;
using System.Linq;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.DAL;
using Voxteneo.WebComponents.Logger;
namespace Sampoerna.EMS.XMLReader
{
    public class XmlSeriesDataMapper : IXmlDataReader 
    {
        private XmlDataMapper _xmlMapper = null;

        public XmlSeriesDataMapper(string filename)
        {
            _xmlMapper = new XmlDataMapper(filename);
           
        }


        public List<ZAIDM_EX_SERIES> Items
        {
            get
            {
                var xmlItems = _xmlMapper.GetElements("ITEM");
                var items = new List<ZAIDM_EX_SERIES>();
                foreach (var xElement in xmlItems)
                {
                    var item = new ZAIDM_EX_SERIES();
                    item.SERIES_CODE = Convert.ToInt32(xElement.Element("SERIES_CODE").Value);
                    item.SERIES_VALUE = xElement.Element("SERIES_VALUE").Value;
                    item.CREATED_DATE = DateTime.Now;
                    var dateXml = Convert.ToDateTime(xElement.Element("MODIFIED_DATE").Value); 
                    var existingSeries = GetSeries(item.SERIES_CODE);
                    if (existingSeries != null)
                    {
                        if (dateXml > existingSeries.CREATED_DATE)
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
            _xmlMapper.InsertToDatabase<ZAIDM_EX_SERIES>(Items);
        }

        public ZAIDM_EX_SERIES GetSeries(int? SeriesCode)
        {
            var exisitingPlant = _xmlMapper.uow.GetGenericRepository<ZAIDM_EX_SERIES>()
                          .Get(p => p.SERIES_CODE == SeriesCode)
                          .OrderByDescending(p => p.CREATED_DATE)
                          .FirstOrDefault();
            return exisitingPlant;
        }





    }
}
