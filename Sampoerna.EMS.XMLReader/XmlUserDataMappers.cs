using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
        private string _xmlFile = null;
        public XmlUserDataMapper(string xmlFile)
        {
            _xmlMapper = new XmlDataMapper(xmlFile);
            _xmlFile = xmlFile;
        }


        public List<BROLE_MAP> Items
        {
         get
            {
                var xmlItems = _xmlMapper.GetElements("row");
                var items = new List<BROLE_MAP>();
                var firstBrole = string.Empty;
               
                foreach (var xElement in xmlItems)
                {
                    try
                    {

                       

                        
                        var role = new USER_BROLE();
                        role.BROLE = _xmlMapper.GetElementValue(xElement.Element("BROLE")).Trim();
                        role.BROLE_DESC = _xmlMapper.GetElementValue(xElement.Element("BROLE_DESC")).TrimEnd();
                        var ExistBrole = GetBrole(role.BROLE);
                       
                        _xmlMapper.InsertOrUpdate(role);
                        
                        
                        var roleMap = new BROLE_MAP();
                        roleMap.BROLE = role.BROLE;
                        roleMap.MSACCT = _xmlMapper.GetElementValue(xElement.Element("MSACCT")).Trim();
                        roleMap.START_DATE = _xmlMapper.GetDate(xElement.Element("STRTDAT").Value);
                        roleMap.END_DATE = _xmlMapper.GetDate(xElement.Element("ENDDAT").Value);

                        var user = new USER();
                        user.USER_ID = roleMap.MSACCT.ToUpper();
                        user.FIRST_NAME = _xmlMapper.GetElementValue(xElement.Element("NACHN_EN")).Trim();
                        user.LAST_NAME = _xmlMapper.GetElementValue(xElement.Element("VORNA_EN")).Trim();
                        user.EMAIL = _xmlMapper.GetElementValue(xElement.Element("WKEMAIL")).Trim();
                        
                       
                        var ExistUser = GetUser(user.USER_ID);
                        if (ExistUser == null)
                        {
                            user.CREATED_DATE = DateTime.Now;
                           
                            
                        }
                        else
                        {
                            user.MODIFIED_DATE = DateTime.Now;
                            user.CREATED_DATE = ExistUser.CREATED_DATE;
                        }
                        _xmlMapper.InsertOrUpdate(user);

                        var ExistRoleMap = GetBroleMap(roleMap.BROLE, roleMap.MSACCT);
                        if (ExistRoleMap != null)
                        {
                            roleMap.BROLE_MAP_ID = ExistRoleMap.BROLE_MAP_ID;
                        }
                       _xmlMapper.InsertOrUpdate(roleMap);
                        items.Add(roleMap);




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
            return _xmlMapper.NoInsert(Items);

        }

        public List<string> GetErrorList()
        {
            return _xmlMapper.Errors;
        }

        public USER GetUser(string UserId)
        {
            var exisitingUser = _xmlMapper.uow.GetGenericRepository<USER>()
                .GetByID(UserId);
            return exisitingUser;
        }
        public USER_BROLE GetBrole(string BroleId)
        {
            var existingData = _xmlMapper.uow.GetGenericRepository<USER_BROLE>()
                .GetByID(BroleId);
            return existingData;
        }
        public BROLE_MAP GetBroleMap(string BroleId, string UserId)
        {
            var existingData = _xmlMapper.uow.GetGenericRepository<BROLE_MAP>()
                .Get(x=>x.BROLE == BroleId && x.MSACCT == UserId).FirstOrDefault();
            return existingData;
        }



    }
}
