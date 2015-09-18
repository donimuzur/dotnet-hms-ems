using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.DAL;
using Voxteneo.WebComponents.Logger;
namespace Sampoerna.EMS.XMLReader
{
    public class XmlCK1DataMapper : IXmlDataReader 
    {
        private XmlDataMapper _xmlMapper = null;

        public XmlCK1DataMapper(string fileName)
        {
            _xmlMapper = new XmlDataMapper(fileName);
           
        }

        
        public List<CK1> Items
        {
            get
            {
                var xmlRoot = _xmlMapper.GetElement("IDOC");
                var xmlRoot2 = xmlRoot.Element("Z1A_EKKO_SSPCP");
                var xmlItems = xmlRoot2.Elements("Z1A_EKPO_SSPCP");
                var items = new List<CK1>();
                foreach (var xElement in xmlItems)
                {
                    try
                    {
                        var item = new CK1();
                        item.CK1_NUMBER = _xmlMapper.GetElementValue(xElement.Element("EBELN"));

                        item.CK1_DATE = Convert.ToDateTime(_xmlMapper.GetDate(_xmlMapper.GetElementValue(xElement.Element("AEDAT"))));
                        //var detail = new CK1_ITEM();
                        //detail.FA_CODE = _xmlMapper.GetElementValue(xElement.Element("FA_CODE"));
                        //detail.MATERIAL_ID = _xmlMapper.GetElementValue(xElement.Element("MATNR"));
                        //item.CK1_ITEM = new List<CK1_ITEM>();
                        //item.CK1_ITEM.Add(detail);
                        item.PLANT_ID = _xmlMapper.GetElementValue(xElement.Element("WERKS"));
                        item.CREATED_BY = Constans.PI;
                        var existingData = GetCk1(item.CK1_NUMBER);

                        if (existingData != null)
                        {
                            item.CREATED_BY = existingData.CREATED_BY;
                            item.CREATED_DATE = existingData.CREATED_DATE;
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

      
        public string InsertToDatabase()
        {
           return _xmlMapper.InsertToDatabase<CK1>(Items);
        }

        public List<string> GetErrorList()
        {
            return _xmlMapper.Errors;
        }

        public CK1 GetCk1(string ck1Number)
        {
            var existingData = _xmlMapper.uow.GetGenericRepository<CK1>()
                .Get(x=>x.CK1_NUMBER == ck1Number ).FirstOrDefault();
            return existingData;
        }



    }
}
