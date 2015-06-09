using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.DAL;
using Voxteneo.WebComponents.Logger;
namespace Sampoerna.EMS.XMLReader
{
    public class XmlPoaMapDataMapper : IXmlDataReader 
    {
        private XmlDataMapper _xmlMapper = null;
       
        public XmlPoaMapDataMapper()
        {
            _xmlMapper = new XmlDataMapper("ZAIDM_POA_MAP");
           
        }


        public List<ZAIDM_POA_MAP> Items
        {
         get
            {
                var xmlItems = _xmlMapper.GetElements("ITEM");
                var items = new List<ZAIDM_POA_MAP>();
                foreach (var xElement in xmlItems)
                {
                    var item = new ZAIDM_POA_MAP();
                    var poaCode  = xElement.Element("POA_ID").Value;
                    var existingPoa = new XmlPoaDataMapper().GetExPoa(poaCode);
                    if(existingPoa == null)
                        continue;
                    var plantCode = xElement.Element("PLANT_ID").Value;
                    var existingPlant = new XmlPlantDataMapper().GetPlant(plantCode);
                    if(existingPlant == null)
                        continue;
                    item.PLANT_ID = existingPlant.PLANT_ID;
                    item.POA_ID = existingPoa.POA_ID;
                    item.NPPBKC_ID = xElement.Element("NPPBKC_ID").Value;
                    item.CREATED_DATE = DateTime.Now;
                    
                    var podDateXml = DateTime.MinValue;
                    DateTime.TryParse(xElement.Element("CHANGES_DATE").Value, out podDateXml);
                    var existingPoaMap = GetPoaMap(item.PLANT_ID, item.POA_ID);
                    if (existingPoaMap != null)
                    {
                        if (podDateXml > existingPoaMap.CREATED_DATE)
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
            _xmlMapper.InsertToDatabase<ZAIDM_POA_MAP>(Items);
       
        }
        public ZAIDM_POA_MAP GetPoaMap(long? PlantId, int? PoaId)
        {
            var existingData = _xmlMapper.uow.GetGenericRepository<ZAIDM_POA_MAP>()
                          .Get(p => p.PLANT_ID == PlantId && p.POA_ID == PoaId)
                          .OrderByDescending(p => p.CREATED_DATE)
                          .FirstOrDefault();
            return existingData;
        }




    }
}
