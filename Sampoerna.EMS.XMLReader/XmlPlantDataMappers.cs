using System;
using System.Collections.Generic;
using System.Linq;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.DAL;
using Voxteneo.WebComponents.Logger;
namespace Sampoerna.EMS.XMLReader
{
    public class XmlPlantDataMapper : IXmlDataReader 
    {
        private XmlDataMapper _xmlMapper = null;

        public XmlPlantDataMapper(string filename)
        {
            _xmlMapper = new XmlDataMapper(filename);
           
        }

        
        public List<T1001W> Items
        {
            get
            {
                var xmlItems = _xmlMapper.GetElements("ITEM");
                var items = new List<T1001W>();
                foreach (var xElement in xmlItems)
                {
                    var item = new T1001W();
                    item.WERKS = xElement.Element("WERKS").Value;
                    item.NAME1 = xElement.Element("NAME1").Value;
                    item.ORT01 = xElement.Element("ORT01").Value;
                    item.CREATED_DATE = DateTime.Now;
                    var plantDateXml = DateTime.MinValue;
                    DateTime.TryParse(xElement.Element("MODIFIED_DATE").Value, out plantDateXml);
                    var exisitingPlant = GetPlant(item.WERKS);
                    if (exisitingPlant != null)
                    {
                        if (plantDateXml > exisitingPlant.CREATED_DATE)
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
          _xmlMapper.InsertToDatabase<T1001W>(Items);
        }

        public T1001W GetPlant(string PlantId)
        {
            var exisitingPlant = _xmlMapper.uow.GetGenericRepository<T1001W>()
                          .Get(p => p.WERKS == PlantId)
                          .OrderByDescending(p => p.CREATED_DATE)
                          .FirstOrDefault();
            return exisitingPlant;
        }





    }
}
