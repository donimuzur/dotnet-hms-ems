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
using Enums = Sampoerna.EMS.Core.Enums;
using Sampoerna.EMS.BusinessObject.Outputs;

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

        
        public List<USER> Items
        {
         get
            {
                var xmlItems = _xmlMapper.GetElements("row");
                var items = new List<USER>();
                var firstBrole = string.Empty;
                
                foreach (var xElement in xmlItems)
                {
                    try
                    {

                       

                        
                        var role = new USER_BROLE();
                        role.BROLE = _xmlMapper.GetElementValue(xElement.Element("BROLE")).Trim();
                        role.BROLE_DESC = _xmlMapper.GetElementValue(xElement.Element("BROLE_DESC")).TrimEnd();

                        
                       
                        _xmlMapper.InsertOrUpdate(role);
                        
                        
                        var roleMap = new BROLE_MAP();
                        roleMap.BROLE = role.BROLE;
                        roleMap.MSACCT = _xmlMapper.GetElementValue(xElement.Element("MSACCT")).Trim().ToUpper();
                        roleMap.START_DATE = _xmlMapper.GetDate(xElement.Element("STRTDAT").Value);
                        roleMap.END_DATE = _xmlMapper.GetDate(xElement.Element("ENDDAT").Value);

                        var user = new USER();
                        user.USER_ID = roleMap.MSACCT.ToUpper();
                        user.FIRST_NAME = _xmlMapper.GetElementValue(xElement.Element("NACHN_EN")).Trim();
                        user.LAST_NAME = _xmlMapper.GetElementValue(xElement.Element("VORNA_EN")).Trim();
                        user.EMAIL = _xmlMapper.GetElementValue(xElement.Element("WKEMAIL")).Trim();
                        user.ACCT = _xmlMapper.GetElementValue(xElement.Element("ACCT")).Trim();
                        var status = int.Parse(_xmlMapper.GetElementValue(xElement.Element("ACCTSTA")).Trim());
                        if (status == 10)
                        {
                            user.IS_ACTIVE = 1;
                        }
                        else
                        {
                            user.IS_ACTIVE = 0;
                        }
                       
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



                        //UserItems.Add(user);
                        _xmlMapper.InsertOrUpdate(user);

                        if (!role.BROLE_DESC.ToUpper().Contains("POA_MANAGER") && role.BROLE_DESC.ToUpper().Contains("POA"))
                        {
                            InsertPoa(user);
                            roleMap.ROLEID = Enums.UserRole.POA;
                        }
                        else if (role.BROLE_DESC.ToUpper().Contains("POA_MANAGER"))
                        {
                            roleMap.ROLEID = Enums.UserRole.Manager;
                        }
                        else if (role.BROLE_DESC.ToUpper().Contains("VIEWER"))
                        {
                            roleMap.ROLEID = Enums.UserRole.Viewer;
                        }
                        else if(role.BROLE_DESC.ToUpper().Contains("CREATOR"))
                        {
                            roleMap.ROLEID = Enums.UserRole.User;
                        }
                        else if (role.BROLE_DESC.ToUpper().Contains("SUPER_ADMINISTRATOR"))
                        {
                            roleMap.ROLEID = Enums.UserRole.SuperAdmin;
                        }
                        else if (role.BROLE_DESC.ToUpper().Contains("ADMINISTRATOR"))
                        {
                            roleMap.ROLEID = Enums.UserRole.Administrator;
                        }

                        InsertBroleMap(roleMap);
                        
                        items.Add(user);




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


        public MovedFileOutput InsertToDatabase()
        {

            return _xmlMapper.InsertToDatabase<USER>(Items);


        }

        public List<string> GetErrorList()
        {
            return _xmlMapper.Errors;
        }

        public string GetInnerException(Exception ex)
        {
            throw new NotImplementedException();
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

        public IEnumerable<BROLE_MAP> GetOtherBroleMaps(BROLE_MAP broleMap)
        {
            var existingData = _xmlMapper.uow.GetGenericRepository<BROLE_MAP>()
                .Get(x => x.BROLE != broleMap.BROLE && x.MSACCT == broleMap.MSACCT);
            return existingData;
        }

        public bool IsUserPoa(USER userData)
        {
            var isPoa = _xmlMapper.uow.GetGenericRepository<POA>()
                .GetByID(userData.USER_ID) != null;

            return isPoa;
        }

        private void DeletePoaByMsAccount(string msacct)
        {
            var existingPoa = _xmlMapper.uow.GetGenericRepository<POA>()
                .GetByID(msacct);

            POA poa = existingPoa;
            
            if(poa != null)
            {
                poa.IS_ACTIVE = false;
            }
        }

        public void InsertPoa(USER userdata)
        {
            
            var existingPoa = _xmlMapper.uow.GetGenericRepository<POA>()
                .GetByID(userdata.USER_ID);

            POA poa = existingPoa;
            if (poa == null)
            {
                poa = new POA();
                poa.IS_ACTIVE = true;
                poa.POA_ID = userdata.USER_ID;
                poa.LOGIN_AS = userdata.USER_ID;
                poa.PRINTED_NAME = userdata.LAST_NAME + " " + userdata.FIRST_NAME;
                poa.POA_EMAIL = userdata.EMAIL;
                poa.CREATED_BY = "PI";
                poa.POA_ADDRESS = "";
                poa.POA_PHONE = "";
                poa.ID_CARD = "";
                poa.TITLE = "";

                poa.CREATED_BY = "PI";
                poa.CREATED_DATE = DateTime.Now;
            }
            else
            {
                poa.PRINTED_NAME = userdata.FIRST_NAME + " " + userdata.LAST_NAME;
                poa.POA_EMAIL = userdata.EMAIL;
                poa.MODIFIED_BY = "PI";
                poa.MODIFIED_DATE = DateTime.Now;
            }


            _xmlMapper.InsertOrUpdate(poa);
        }

        public USER GetManagerUser(USER userdata,string acctSpv)
        {
            if (IsUserPoa(userdata))
            {
                var manager = _xmlMapper.uow.GetGenericRepository<USER>()
                    .Get(x => x.ACCT == acctSpv).FirstOrDefault();

                return manager;
            }
            else
            {
                return null;
            }
        }

        public void InsertBroleMap(BROLE_MAP roleMap)
        {

            var broleMapToDelete = GetOtherBroleMaps(roleMap);
            if (broleMapToDelete.Any())
            {
                DeleteBroleMap(broleMapToDelete);
                
            }
            
            var existRoleMap = GetBroleMap(roleMap.BROLE, roleMap.MSACCT);
            if (existRoleMap!= null)
            {
                roleMap.BROLE_MAP_ID = existRoleMap.BROLE_MAP_ID;
            }
            _xmlMapper.InsertOrUpdate(roleMap);

            if (roleMap.ROLEID.HasValue && roleMap.ROLEID.Value != Enums.UserRole.POA)
            {
                DeletePoaByMsAccount(roleMap.MSACCT);
            }
        }


        private void DeleteBroleMap(IEnumerable<BROLE_MAP> broleMaps)
        {
            foreach (var broleMap in broleMaps)
            {
                _xmlMapper.uow.GetGenericRepository<BROLE_MAP>().Delete(broleMap);
            }
            _xmlMapper.uow.SaveChanges();
        }
    }
}
