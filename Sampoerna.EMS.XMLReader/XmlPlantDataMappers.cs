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

        
        public List<T001W> Items
        {
            get
            {
                var xmlItems = _xmlMapper.GetElements("ITEM");
                var items = new List<T001W>();
                foreach (var xElement in xmlItems)
                {
                    var item = new T001W();
                    item.WERKS = xElement.Element("WERKS").Value;
                    item.NAME1 = xElement.Element("NAME1").Value;
                    item.ORT01 = xElement.Element("ORT01").Value;
                    item.NPPBKC_ID = xElement.Element("NPPBKC_ID").Value;
                    
                    var plantDateXml = Convert.ToDateTime(xElement.Element("MODIFIED_DATE").Value); 
                    var exisitingPlant = GetPlant(item.WERKS);
                    if (exisitingPlant != null)
                    {
                        item.CREATED_DATE = exisitingPlant.CREATED_DATE;
                        item.MODIFIED_DATE = plantDateXml;
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
          _xmlMapper.InsertToDatabase<T001W>(Items);
        }

        public T001W GetPlant(string PlantId)
        {
            var exisitingPlant = _xmlMapper.uow.GetGenericRepository<T001W>()
                .GetByID(PlantId);
            return exisitingPlant;
        }





    }
}
