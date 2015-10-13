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
using Sampoerna.EMS.Core;
using Sampoerna.EMS.DAL;
using Voxteneo.WebComponents.Logger;
namespace Sampoerna.EMS.XMLReader
{
    public class XmlMovementDataMapper : IXmlDataReader 
    {
        private XmlDataMapper _xmlMapper = null;

        public XmlMovementDataMapper(string filename)
        {
            _xmlMapper = new XmlDataMapper(filename);
           
        }


        public List<INVENTORY_MOVEMENT> Items
        {
           get
            {
                var xmlRoot = _xmlMapper.GetElement("InvMovementDetails");
                var xmlItems = xmlRoot.Elements("Movement");
                var items = new List<INVENTORY_MOVEMENT>();
                foreach (var xElement in xmlItems)
                {
                    try
                    {
                        var item = new INVENTORY_MOVEMENT();
                        item.MAT_DOC = _xmlMapper.GetElementValue(xElement.Element("MatDoc"));
                        var existingData = GetMovement(item.MAT_DOC);

                        if (existingData != null)
                        {
                            item = existingData;
                            
                        }
                        

                        
                        item.MVT = _xmlMapper.GetElementValue(xElement.Element("MvT"));
                        item.MATERIAL_ID = _xmlMapper.GetElementValue(xElement.Element("Material"));
                        item.PLANT_ID = _xmlMapper.GetElementValue(xElement.Element("Plnt"));
                        item.QTY = Convert.ToDecimal(_xmlMapper.GetElementValue(xElement.Element("Quantity")));
                        item.VENDOR = _xmlMapper.GetElementValue(xElement.Element("Vendor"));
                        item.BATCH = _xmlMapper.GetElementValue(xElement.Element("Batch"));
                        item.BUN = _xmlMapper.GetElementValue(xElement.Element("BUn"));
                        item.PURCH_DOC = _xmlMapper.GetElementValue(xElement.Element("PurchDoc"));
                        item.POSTING_DATE = _xmlMapper.GetDateDotSeparator(_xmlMapper.GetElementValue(xElement.Element("PstngDate")));
                        item.ENTRY_DATE = _xmlMapper.GetDateDotSeparator(_xmlMapper.GetElementValue(xElement.Element("EntryDate")));
                        item.CREATED_USER = _xmlMapper.GetElementValue(xElement.Element("Username"));
                        
                        items.Add(item);
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
            
            
            return _xmlMapper.InsertToDatabase<INVENTORY_MOVEMENT>(Items);
       
        }

        public List<string> GetErrorList()
        {
            return _xmlMapper.Errors;
        }
        //public INVENTORY_MOVEMENT GetPCode(string PCode)
        //{
        //    var exisitingPlant = _xmlMapper.uow.GetGenericRepository<ZAIDM_EX_PCODE>()
        //        .GetByID(PCode);
        //    return exisitingPlant;
        //}

        public INVENTORY_MOVEMENT GetMovement(string matdoc)
        {
            var existingMvmt = _xmlMapper.uow.GetGenericRepository<INVENTORY_MOVEMENT>()
                .Get(x => x.MAT_DOC == matdoc, null, "").FirstOrDefault();

            return existingMvmt;
        }


    }
}
