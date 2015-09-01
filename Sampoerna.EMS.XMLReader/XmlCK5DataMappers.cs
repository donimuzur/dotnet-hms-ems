using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using Sampoerna.EMS.BLL;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.DAL;
using Sampoerna.EMS.MessagingService;
using Voxteneo.WebCompoments.NLogLogger;
using Voxteneo.WebComponents.Logger;
using Enums = Sampoerna.EMS.Core.Enums;

namespace Sampoerna.EMS.XMLReader
{
    public class XmlCk5DataMapper : IXmlDataReader 
    {
        private XmlDataMapper _xmlMapper = null;

        public XmlCk5DataMapper(string fileName)
        {
            _xmlMapper = new XmlDataMapper(fileName);
           
        }
        
        private Enums.CK5Type GetEMSCk5Type(string type)
        {
            switch (type)
            {
                case "01":
                    return Enums.CK5Type.Domestic;
                case "02":
                    return Enums.CK5Type.Intercompany;
                case "03":
                    return Enums.CK5Type.ImporterToPlant;
                case "04":
                    return Enums.CK5Type.PortToImporter;
                case "05":
                    return Enums.CK5Type.Export;
            }
            return Enums.CK5Type.Manual;

        }
        private Enums.CK5XmlStatus GetEMSCk5Status(string type)
        {
            switch (type)
            {
                case "10":
                    return Enums.CK5XmlStatus.StoCreated;
                case "11":
                    return Enums.CK5XmlStatus.StoFailed;
                case "15":
                    return Enums.CK5XmlStatus.GIPartial;
                case "16":
                    return Enums.CK5XmlStatus.GICompleted;
                case "17":
                    return Enums.CK5XmlStatus.GIReversal;
                case "20":
                    return Enums.CK5XmlStatus.GRPartial;
                case "21":
                    return Enums.CK5XmlStatus.GRCompleted;
                case "22":
                    return Enums.CK5XmlStatus.GRReversal;
                case "30":
                    return Enums.CK5XmlStatus.StobGIPartial;
                case "31":
                    return Enums.CK5XmlStatus.StobGICompleted;
                case "40":
                    return Enums.CK5XmlStatus.StoRecCreated;
                case "41":
                    return Enums.CK5XmlStatus.StoRecGIPartial;
                case "42":
                    return Enums.CK5XmlStatus.StoRecGICompleted;
                case "45":
                    return Enums.CK5XmlStatus.StoRecGRPartial;
                case "46":
                    return Enums.CK5XmlStatus.StoRecGRCompleted;
                default:
                    return Enums.CK5XmlStatus.None;
            }
            return Enums.CK5XmlStatus.StoCreated; 

        }
        public List<CK5> Items
        {
            get
            {
                
                var xmlRoot = _xmlMapper.GetElement("IDOC");
                var xmlItems = xmlRoot.Elements("Z1A_CK5_HDR");
                var items = new List<CK5>();
                foreach (var xElement in xmlItems)
                {
                    try
                    {
                        var item = new CK5();
                        item.SUBMISSION_NUMBER = _xmlMapper.GetElementValue(xElement.Element("CK5_NUMBER"));
                        
                        var status = _xmlMapper.GetElementValue(xElement.Element("STATUS"));
                        var type = _xmlMapper.GetElementValue(xElement.Element("CK5_PROCS_TYP"));
                        item.CK5_TYPE = GetEMSCk5Type(type);
                        var existingCk5 = GetExistingCK5(item.SUBMISSION_NUMBER);
                        
                        if (existingCk5 != null)
                        {
                            item = existingCk5;
                            var statusCk5 = GetEMSCk5Status(status);
                            var typeCk5 = GetEMSCk5Type(type);
                            if (typeCk5 != Enums.CK5Type.Manual)
                            {
                                var workflowHistory = new WORKFLOW_HISTORY();
                                workflowHistory.FORM_ID = existingCk5.CK5_ID;
                                workflowHistory.ACTION_BY = Constans.PI;
                                workflowHistory.ROLE = Enums.UserRole.System;
                                workflowHistory.FORM_NUMBER = existingCk5.SUBMISSION_NUMBER;
                                workflowHistory.FORM_TYPE_ID = Enums.FormType.CK5;
                                workflowHistory.ACTION_DATE = DateTime.Now;
                                
                                if (statusCk5 == Enums.CK5XmlStatus.StoCreated)
                                {
                                    var stoNumber = _xmlMapper.GetElementValue(xElement.Element("STO_NUMBER"));
                                    item.STO_SENDER_NUMBER = stoNumber;
                                    item.STATUS_ID = Enums.DocumentStatus.STOCreated;
                                    workflowHistory.ACTION = Enums.ActionType.STOCreated;
                                }

                                else
                                    if (statusCk5 == Enums.CK5XmlStatus.GICompleted || statusCk5 == Enums.CK5XmlStatus.GIPartial)
                                {
                                    if (statusCk5 == Enums.CK5XmlStatus.GICompleted)
                                    {
                                        workflowHistory.ACTION = Enums.ActionType.GICompleted;
                                        item.STATUS_ID = Enums.DocumentStatus.GICompleted;
                                    }
                                    else
                                    {
                                        workflowHistory.ACTION = Enums.ActionType.GIPartial;
                                        item.STATUS_ID = Enums.DocumentStatus.GIPartial;
                                    }

                                    if (typeCk5 == Enums.CK5Type.Domestic)
                                    {
                                        #region "Domestic"

                                        var giDate = _xmlMapper.GetElementValue(xElement.Element("GI_DATE"));
                                        item.GI_DATE = _xmlMapper.GetDate(giDate);
                                       
                                        var ck5Item = GetExistingCK5Material(existingCk5.CK5_ID);
                                        if (ck5Item.Count > 0)
                                        {
                                            
                                            var xmlCk5Items = xElement.Elements("Z1A_CK5_ITM");
                                            if (ck5Item.Count() >= xmlCk5Items.Count())
                                            {
                                                foreach (var ckt5Item in xmlCk5Items)
                                                {
                                                    var dn_number = _xmlMapper.GetElementValue(ckt5Item.Element("DELIVERY_NOTE"));
                                                    if (!string.IsNullOrEmpty(dn_number))
                                                    {
                                                        item.DN_NUMBER = dn_number;
                                                    }


                                                }
                                            }

                                        }

                                        #endregion
                                    }
                                   

                                }
                                    else if (statusCk5 == Enums.CK5XmlStatus.GRCompleted || statusCk5 == Enums.CK5XmlStatus.GRPartial)
                                {
                                    if (statusCk5 == Enums.CK5XmlStatus.GRCompleted)
                                    {
                                        workflowHistory.ACTION = Enums.ActionType.GRCompleted;
                                        item.STATUS_ID = Enums.DocumentStatus.GRCompleted;
                                    }
                                    else
                                    {
                                        workflowHistory.ACTION = Enums.ActionType.GRPartial;
                                        item.STATUS_ID = Enums.DocumentStatus.GRPartial;
                                    }
                                    if (typeCk5 == Enums.CK5Type.Domestic)
                                    {
                                        var grDate = _xmlMapper.GetElementValue(xElement.Element("GR_DATE"));
                                        item.GR_DATE = _xmlMapper.GetDate(grDate);
                                    }
                                    else if(typeCk5 == Enums.CK5Type.Intercompany)
                                    {
                                        var stoNumber = _xmlMapper.GetElementValue(xElement.Element("STO_NUMBER"));
                                        item.STO_SENDER_NUMBER = stoNumber;
                                        
                                    }

                                }
                                else if (statusCk5 == Enums.CK5XmlStatus.GRReversal)
                                {
                                    
                                    item.STATUS_ID = Enums.DocumentStatus.GRReversal;
                                    workflowHistory.ACTION = Enums.ActionType.GRReversal;

                                }
                                else if (statusCk5 == Enums.CK5XmlStatus.GIReversal)
                                {
                                    workflowHistory.ACTION = Enums.ActionType.GIReversal;
                                    AddWorkflowHistory(workflowHistory, null, null, null, null);

                                    CreateCk5XmlCancel(item);
                                   
                                    item.STATUS_ID = Enums.DocumentStatus.Cancelled;
                                    workflowHistory.ACTION = Enums.ActionType.Cancelled;

                                }
                                else if (statusCk5 == Enums.CK5XmlStatus.StoFailed)
                                {
                                    item.STATUS_ID = Enums.DocumentStatus.Cancelled;
                                    workflowHistory.ACTION = Enums.ActionType.STOFailed;
                                }
                                else if (statusCk5 == Enums.CK5XmlStatus.StobGICompleted)
                                {
                                    item.STATUS_ID = Enums.DocumentStatus.StobGICompleted;
                                    workflowHistory.ACTION = Enums.ActionType.StobGICompleted;
                                    var stobNumber = _xmlMapper.GetElementValue(xElement.Element("STOB_NUMBER"));
                                    item.STOB_NUMBER = stobNumber;
                                    var giDate = _xmlMapper.GetElementValue(xElement.Element("GI_DATE"));
                                    item.GI_DATE = _xmlMapper.GetDate(giDate);
                                    var ck5Item = GetExistingCK5Material(existingCk5.CK5_ID);
                                    if (ck5Item.Count > 0)
                                    {

                                        var xmlCk5Items = xElement.Elements("Z1A_CK5_ITM");
                                        if (ck5Item.Count() >= xmlCk5Items.Count())
                                        {
                                            foreach (var ckt5Item in xmlCk5Items)
                                            {
                                                var dn_number = _xmlMapper.GetElementValue(ckt5Item.Element("DELIVERY_NOTE"));
                                                if (!string.IsNullOrEmpty(dn_number))
                                                {
                                                    item.DN_NUMBER = dn_number;
                                                }


                                            }
                                        }

                                    }

                                }
                                else if (statusCk5 == Enums.CK5XmlStatus.StoRecCreated)
                                {
                                    var stobRecNumber = _xmlMapper.GetElementValue(xElement.Element("STO_REC_NUMBER"));
                                    item.STO_RECEIVER_NUMBER = stobRecNumber;
                                    
                                    item.STATUS_ID = Enums.DocumentStatus.StoRecCreated;
                                    workflowHistory.ACTION = Enums.ActionType.StoRecCreated;
                                    
                                    
                                     
                                }
                               
                                else if (statusCk5 == Enums.CK5XmlStatus.StoRecGIPartial)
                                {

                                    item.STATUS_ID = Enums.DocumentStatus.StoRecGIPartial;
                                    workflowHistory.ACTION = Enums.ActionType.StoRecGIPartial;
                                    var stoNumber = _xmlMapper.GetElementValue(xElement.Element("STO_NUMBER"));
                                    item.STO_SENDER_NUMBER = stoNumber;

                                }
                                    else if (statusCk5 == Enums.CK5XmlStatus.StoRecGICompleted)
                                    {
                                        #region "sto rec gicompleted"
                                        item.STATUS_ID = Enums.DocumentStatus.StoRecGIPartial;
                                        workflowHistory.ACTION = Enums.ActionType.StoRecGIPartial;
                                        var stobNumber = _xmlMapper.GetElementValue(xElement.Element("STOB_NUMBER"));
                                        item.STOB_NUMBER = stobNumber;
                                        var giDate = _xmlMapper.GetElementValue(xElement.Element("GI_DATE"));
                                        item.GI_DATE = _xmlMapper.GetDate(giDate);
                                        var ck5Item = GetExistingCK5Material(existingCk5.CK5_ID);
                                        if (ck5Item.Count > 0)
                                        {

                                            var xmlCk5Items = xElement.Elements("Z1A_CK5_ITM");
                                            if (ck5Item.Count() >= xmlCk5Items.Count())
                                            {
                                                foreach (var ckt5Item in xmlCk5Items)
                                                {
                                                    var dn_number = _xmlMapper.GetElementValue(ckt5Item.Element("DELIVERY_NOTE"));
                                                    if (!string.IsNullOrEmpty(dn_number))
                                                    {
                                                        item.DN_NUMBER = dn_number;
                                                    }


                                                }
                                            }

                                        }
                                        #endregion

                                    }
                                else if (statusCk5 == Enums.CK5XmlStatus.StoRecGRPartial)
                                {

                                    item.STATUS_ID = Enums.DocumentStatus.StoRecGRPartial;
                                    workflowHistory.ACTION = Enums.ActionType.StoRecGRPartial;


                                }
                                else if (statusCk5 == Enums.CK5XmlStatus.StoRecGRCompleted)
                                {
                                    var grdate = _xmlMapper.GetElementValue(xElement.Element("GR_DATE"));
                                    item.GR_DATE = _xmlMapper.GetDate(grdate);

                                    item.STATUS_ID = Enums.DocumentStatus.StoRecGRCompleted;
                                    workflowHistory.ACTION = Enums.ActionType.StoRecGRCompleted;



                                }

                                else if (statusCk5 == Enums.CK5XmlStatus.STOBGRReversal)
                                {

                                    item.STATUS_ID = Enums.DocumentStatus.STOBGRReversal;
                                    workflowHistory.ACTION = Enums.ActionType.STOBGRReversal;

                                }
                                else if (statusCk5 == Enums.CK5XmlStatus.STOBGIReversal)
                                {

                                    workflowHistory.ACTION = Enums.ActionType.STOBGIReversal;
                                    AddWorkflowHistory(workflowHistory, null, null, null, null);
                                    
                                    CreateCk5XmlCancel(item);

                                    item.STATUS_ID = Enums.DocumentStatus.Cancelled;
                                    workflowHistory.ACTION = Enums.ActionType.Cancelled;
                                }
                                if (statusCk5 != Enums.CK5XmlStatus.None)
                                {
                                    var emailCreator = GetEmail(item.CREATED_BY);
                                    var emailPoa = GetEmail(item.APPROVED_BY_POA);
                                    var emailManager = GetEmail(item.APPROVED_BY_MANAGER);
                                    var emailBody = string.Format("Status : {0}", item.STATUS_ID.ToString());
                                    AddWorkflowHistory(workflowHistory, emailCreator, emailPoa, emailManager, emailBody);
                                }
                            }
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
            
            return _xmlMapper.InsertToDatabase<CK5>(Items);
        }

        public List<string> GetErrorList()
        {
            return _xmlMapper.Errors;
        }

        private string GetEmail(string userId)
        {
            var repo = _xmlMapper.uow.GetGenericRepository<USER>();
            var user = repo.GetByID(userId);
            return user == null ? null : user.EMAIL;
        }

        public CK5 GetExistingCK5(string ck5Number)
        {
            var existingData = _xmlMapper.uow.GetGenericRepository<CK5>()
                .Get(p => p.SUBMISSION_NUMBER.Contains(ck5Number)).FirstOrDefault();
            return existingData;
        }
        public List<CK5_MATERIAL> GetExistingCK5Material(long ck5Id)
        {
            var existingData = _xmlMapper.uow.GetGenericRepository<CK5_MATERIAL>()
                .Get(p => p.CK5_ID  == ck5Id).ToList();
            return existingData;
        }
        public void AddWorkflowHistory(WORKFLOW_HISTORY workflowHistory, string EmailCreator, string EmailPOA, string EmailManager, string emailBody)
        {
           
            _xmlMapper.InsertOrUpdate(workflowHistory);
            ILogger logger= new NLogLogger();
            IMessageService messageService = new MessageService(logger);
            var subject = "CK5 Status No: " + workflowHistory.FORM_NUMBER;

            var emailList = new List<string>();
            if(!string.IsNullOrEmpty(EmailCreator))
                emailList.Add(EmailCreator);
            if(!string.IsNullOrEmpty(EmailPOA))
                emailList.Add(EmailPOA);
            if(!string.IsNullOrEmpty(EmailManager))
                emailList.Add(EmailManager);
            if (emailList.Count > 0)
            {
                messageService.SendEmailToList(emailList, subject, emailBody, false);
            }

        }

        private void CreateCk5XmlCancel(CK5 ck5)
        {
            //create xml file status 03 
            var ck5Writer = new XmlCK5DataWriter();
            BLLMapper.InitializeCK5();
            var outboundPath = ConfigurationManager.AppSettings["XmlOutboundPath"];
            var date = DateTime.Now.ToString("yyyyMMdd");
            var time = DateTime.Now.ToString("hhmmss");

            var fileName = string.Format("CK5CAN_{0}-{1}-{2}.xml", ck5.SUBMISSION_NUMBER, date, time);
            var Ck5XmlDto = new CK5BLL(_xmlMapper.uow, new NullLogger()).GetCk5ForXmlById(ck5.CK5_ID);
          
            Ck5XmlDto.Ck5PathXml = Path.Combine(outboundPath, fileName);
          
            ck5Writer.CreateCK5Xml(Ck5XmlDto, "03");
        }

    }
}
