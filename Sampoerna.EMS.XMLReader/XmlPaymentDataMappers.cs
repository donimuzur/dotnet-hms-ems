using System;
using System.Collections.Generic;
using System.Linq;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.DAL;
using Voxteneo.WebComponents.Logger;
namespace Sampoerna.EMS.XMLReader
{
    public class XmlPaymentDataMapper : IXmlDataReader 
    {
        private XmlDataMapper _xmlMapper = null;

        public XmlPaymentDataMapper(string filename)
        {
            _xmlMapper = new XmlDataMapper(filename);
           
        }

        
        public List<CK1> Items
        {
            get
            {
                var xmlRoot = _xmlMapper.GetElement("IDOC");
                var xmlItems = xmlRoot.Elements("Z1A_SSPCP");
                var items = new List<CK1>();
                foreach (var xElement in xmlItems)
                {
                    try
                    {
                        var item = new CK1();
                        var ck1Id = _xmlMapper.GetElementValue(xElement.Element("CK1_ID"));
                        var existingCk1 = GetCk1(ck1Id);
                        if (existingCk1 != null)
                        {
                            item = existingCk1;
                            
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
                .Get(x => x.CK1_NUMBER == ck1Number).FirstOrDefault();
            return existingData;
        }






    }
}
