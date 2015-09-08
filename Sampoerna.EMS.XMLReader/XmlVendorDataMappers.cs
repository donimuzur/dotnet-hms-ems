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
    public class XmlVendorDataMapper : IXmlDataReader 
    {
        private XmlDataMapper _xmlMapper = null;


        public XmlVendorDataMapper(string fileName)
        {
           
             _xmlMapper = new XmlDataMapper(fileName);
            
        }
       

        public List<LFA1> Items
        {
         get
            {
                var xmlRoot = _xmlMapper.GetElement("IDOC");
                var xmlItems = xmlRoot.Elements("E1LFA1M");
                
                var items = new List<LFA1>();
                foreach (var xElement in xmlItems)
                {
                    try
                    {
                        var item = new LFA1();
                        var vendorCodeXml = xElement.Element("LIFNR").Value;

                        var exsitingVendor = GetExVendor(vendorCodeXml);
                        var existingCompany = GetExCompany(vendorCodeXml);
                        if (existingCompany != null)
                        {
                            existingCompany.NPWP = _xmlMapper.GetElementValue(xElement.Element("STCEG"));
                            existingCompany.SPRAS = _xmlMapper.GetElementValue(xElement.Element("SPRAS"));
                            _xmlMapper.InsertOrUpdate(existingCompany);
                        }

                        item.LIFNR = vendorCodeXml;
                        item.NAME1 = _xmlMapper.GetElementValue(xElement.Element("NAME1"));
                        item.ORT01 = _xmlMapper.GetElementValue(xElement.Element("ORT01"));
                        item.STRAS = _xmlMapper.GetElementValue(xElement.Element("STRAS"));
                        var isDeleted = _xmlMapper.GetElementValue(xElement.Element("LOEVM"));
                        if (isDeleted != null)
                        {
                            item.IS_DELETED = true;
                        }
                        item.CREATED_BY = Constans.PI;

                        if (exsitingVendor != null)
                        {
                            item.CREATED_DATE = exsitingVendor.CREATED_DATE;
                            item.MODIFIED_DATE = DateTime.Now;
                            item.CREATED_BY = exsitingVendor.CREATED_BY;
                            item.MODIFIED_BY = Constans.PI;
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
            return  _xmlMapper.InsertToDatabase<LFA1>(Items);
       
        }

        public List<string> GetErrorList()
        {
            return _xmlMapper.Errors;
        }

        public LFA1 GetExVendor(string vendorCode)
        {
            var exisitingPoa = _xmlMapper.uow.GetGenericRepository<LFA1>()
                .GetByID(vendorCode);
            return exisitingPoa;
        }

        public T001 GetExCompany(string vendorCode)
        {
            var exisitingPoa = _xmlMapper.uow.GetGenericRepository<T001>()
                .GetByID(vendorCode);
            return exisitingPoa;
        }


    }
}
