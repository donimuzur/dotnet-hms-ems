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
    public class XmlUserDataMapper : IXmlDataReader 
    {
        private XmlDataMapper _xmlMapper = null;
       
        public XmlUserDataMapper()
        {
            _xmlMapper = new XmlDataMapper("USER");
           
        }


        public List<USER> Items
        {
         get
            {
                var xmlItems = _xmlMapper.GetElements("ITEM");
                var items = new List<USER>();
                foreach (var xElement in xmlItems)
                {
                    try
                    {
                        var item = new USER();
                        item.USER_ID = xElement.Element("USER_ID").Value;
                        var userGroup = xElement.Element("USER_GROUP").Value;

                        
                        item.FIRST_NAME = xElement.Element("FIRST_NAME").Value;
                        item.LAST_NAME = xElement.Element("LAST_NAME").Value;
                        item.EMAIL = xElement.Element("EMAIL").Value;
                    
                        var dateXml = Convert.ToDateTime(xElement.Element("MODIFIED_DATE").Value); 
                        var exsitingUser = GetUser(item.USER_ID);
                        if (exsitingUser != null)
                        {
                            
                            item.MODIFIED_DATE = dateXml;
                            items.Add(item);
                          
                        }
                        else
                        {
                            item.CREATED_DATE = DateTime.Now;
                            items.Add(item);
                        }

                    }
                    catch (Exception)
                    {
                       continue;
                        
                    }
                    
                }
                return items;
            }
             
        }


        public string InsertToDatabase()
        {
            return _xmlMapper.InsertToDatabase<USER>(Items);
       
        }

        public List<string> GetErrorList()
        {
            throw new NotImplementedException();
        }

        public USER GetUser(string UserId)
        {
            var exisitingUser = _xmlMapper.uow.GetGenericRepository<USER>()
                .GetByID(UserId);
            return exisitingUser;
        }
     



    }
}
