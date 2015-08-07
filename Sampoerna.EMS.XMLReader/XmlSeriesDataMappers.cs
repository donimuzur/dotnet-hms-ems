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
                var xmlRoot = _xmlMapper.GetElement("IDOC");
                var xmlItems = xmlRoot.Elements("Z1A_SERIES");
                var items = new List<ZAIDM_EX_SERIES>();
                foreach (var xElement in xmlItems)
                {
                    try
                    {
                        var item = new ZAIDM_EX_SERIES();
                        item.SERIES_CODE = xElement.Element("SERIES_CODE").Value;
                        item.SERIES_VALUE = xElement.Element("SERIES_VALUE").Value;
                        // var dateXml = Convert.ToDateTime(xElement.Element("MODIFIED_DATE").Value); 
                        var existingSeries = GetSeries(item.SERIES_CODE);
                        if (existingSeries != null)
                        {
                            item.CREATED_DATE = existingSeries.CREATED_DATE;
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
            return _xmlMapper.InsertToDatabase<ZAIDM_EX_SERIES>(Items);
        }

        public ZAIDM_EX_SERIES GetSeries(string SeriesCode)
        {
            var exisitingPlant = _xmlMapper.uow.GetGenericRepository<ZAIDM_EX_SERIES>()
                .GetByID(SeriesCode);
            return exisitingPlant;
        }





    }
}
