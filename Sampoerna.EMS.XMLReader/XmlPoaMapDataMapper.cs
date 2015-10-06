using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;

namespace Sampoerna.EMS.XMLReader
{
    public class XmlPoaMapDataMapper : IXmlDataReader
    {
        private XmlDataMapper _xmlMapper = null;

        public XmlPoaMapDataMapper(string fileName)
        {
            _xmlMapper = new XmlDataMapper(fileName);
           
        }


        public List<POA_MAP> Items
        {
            get
            {
                
                var xmlRoot = _xmlMapper.GetElement("IDOC");
                var xmlItems = xmlRoot.Elements("Z1A_POA_MAP");

                var items = new List<POA_MAP>();
                foreach (var xElement in xmlItems)
                {
                    try
                    {
                        var item = new POA_MAP();
                        item.POA_ID = xElement.Element("POA_ID").Value;
                        item.NPPBKC_ID = xElement.Element("NPPBKC_ID").Value;
                        item.WERKS = xElement.Element("PLANT").Value;

                        var existingData = GetPoaMap(item.NPPBKC_ID, item.WERKS, item.POA_ID);
                        if (existingData == null)
                        {
                            item.CREATED_BY = "PI";
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
            return _xmlMapper.InsertToDatabase<POA_MAP>(Items);
        }

        public List<string> GetErrorList()
        {
            throw new NotImplementedException();
        }


        public POA_MAP GetPoaMap(string nppbkcId, string plant, string poaId)
        {
            var exisitingPoa = _xmlMapper.uow.GetGenericRepository<POA_MAP>()
                .Get(p => p.POA_ID == poaId && p.WERKS == plant && p.NPPBKC_ID == nppbkcId).FirstOrDefault();

            return exisitingPoa;
        }
    }
}
