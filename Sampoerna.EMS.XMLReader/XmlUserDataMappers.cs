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
                        item.USER_ID = Convert.ToInt32(xElement.Element("USER_ID").Value);
                        item.USERNAME = xElement.Element("USERNAME").Value;
                        if (!string.IsNullOrEmpty((xElement.Element("MANAGER_ID").Value)))
                        {
                            item.MANAGER_ID = Convert.ToInt32(xElement.Element("MANAGER_ID").Value);
                            var findManager = GetUser(item.MANAGER_ID);
                            if (findManager == null)
                                continue;

                        }
                        var userGroup = xElement.Element("USER_GROUP").Value;

                        if (string.IsNullOrEmpty(userGroup))
                            continue;
                        var existingGroup = GetUserGroup(userGroup);
                        if (existingGroup == null)
                        {
                            //insert to table group if new group
                            List<USER_GROUP> listGroup = new List<USER_GROUP>();
                            var roleName = xElement.Element("ROLE_NAME").Value;

                            listGroup.Add(new USER_GROUP { GROUP_NAME = userGroup, ROLE_NAME = roleName });
                            _xmlMapper.InsertToDatabase<USER_GROUP>(listGroup);
                            item.USER_GROUP_ID = GetUserGroup(userGroup).GROUP_ID


                                ;
                        }
                        else
                        {
                            item.USER_GROUP_ID = existingGroup.GROUP_ID;
                        }
                        item.FIRST_NAME = xElement.Element("FIRST_NAME").Value;
                        item.LAST_NAME = xElement.Element("LAST_NAME").Value;
                        item.EMAIL = xElement.Element("EMAIL").Value;
                        item.CREATED_DATE = DateTime.Now;
                        var dateXml = DateTime.MinValue;
                        DateTime.TryParse(xElement.Element("CHANGES_DATE").Value, out dateXml);
                        var exsitingUser = GetUser(item.USER_ID);
                        if (exsitingUser != null)
                        {
                            if (dateXml > exsitingUser.CREATED_DATE)
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
                    catch (Exception)
                    {
                       continue;
                        
                    }
                    
                }
                return items;
            }
             
        }


        public void InsertToDatabase()
        {
           _xmlMapper.InsertToDatabase<USER>(Items);
       
        }

        public USER GetUser(int? UserId)
        {
            var exisitingUser = _xmlMapper.uow.GetGenericRepository<USER>()
                            .Get(p => p.USER_ID == UserId )
                         .FirstOrDefault();
            return exisitingUser;
        }
        public USER_GROUP GetUserGroup(string userGroupName)
        {
            var exisitingUser = _xmlMapper.uow.GetGenericRepository<USER_GROUP>()
                            .Get(p => p.GROUP_NAME == userGroupName)
                         .FirstOrDefault();
            return exisitingUser;
        }



    }
}
