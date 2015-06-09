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
    public class XmlAreaDataMapper : IXmlDataReader 
    {
        private XmlDataMapper _xmlMapper = null;
       
        public XmlAreaDataMapper()
        {
            _xmlMapper = new XmlDataMapper("T1001K");
           
        }

        
        public List<T1001K> Items
        {
            get
            {
                var xmlItems = _xmlMapper.GetElements("ITEM");
                var items = new List<T1001K>();
                foreach (var xElement in xmlItems)
                {
                    var item = new T1001K();
                    item.BWKEY = xElement.Element("BWKEY").Value;
                    var companyCode  = xElement.Element("BUKRS").Value;
                    var companyById =
                        _xmlMapper.uow.GetGenericRepository<T1001>()
                            .Get(p => p.BUKRS == companyCode)
                            .OrderByDescending(p => p.CREATED_DATE)
                            .FirstOrDefault();

                    item.CREATED_DATE = DateTime.Now;
                    if (companyById != null)
                            item.COMPANY_ID = companyById.COMPANY_ID;
                       
                    var areaDateXml = DateTime.MinValue;
                    DateTime.TryParse(xElement.Element("MODIFIED_DATE").Value, out areaDateXml);
                    var exisitingArea = _xmlMapper.uow.GetGenericRepository<T1001K>()
                           .Get(p => p.BWKEY == item.BWKEY)
                           .OrderByDescending(p => p.CREATED_DATE)
                           .FirstOrDefault();

                    if (exisitingArea != null)
                    {
                        if (areaDateXml > exisitingArea.CREATED_DATE)
                        {
                             items.Add(item);
                        }
                        else
                        {
                            continue;

                        }
                    }
                   
                }
                return items;
            }
             
        }


        public void InsertToDatabase()
        {
           _xmlMapper.InsertToDatabase<T1001K>(Items);
        }





    }
}
