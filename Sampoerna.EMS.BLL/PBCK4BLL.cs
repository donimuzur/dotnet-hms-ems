using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using AutoMapper;
using Sampoerna.EMS.BLL.Services;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Business;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.BusinessObject.Outputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Contract.Services;
using Sampoerna.EMS.Core.Exceptions;
using Sampoerna.EMS.MessagingService;
using Sampoerna.EMS.Utils;
using Voxteneo.WebComponents.Logger;
using Enums = Sampoerna.EMS.Core.Enums;

namespace Sampoerna.EMS.BLL
{
   public class PBCK4BLL : IPBCK4BLL
   {
       private ILogger _logger;
       private IUnitOfWork _uow;

       private IGenericRepository<PBCK4> _repository;
       private IGenericRepository<PBCK4_ITEM> _repositoryPbck4Items;

       private IMonthBLL _monthBll;
       private IDocumentSequenceNumberBLL _docSeqNumBll;
       private IWorkflowHistoryBLL _workflowHistoryBll;
       private IChangesHistoryBLL _changesHistoryBll;
       private IPrintHistoryBLL _printHistoryBll;
       private IBrandRegistrationService _brandRegistrationServices;
       private ICK1BLL _ck1Bll;
       private IMessageService _messageService;
       private IPOABLL _poaBll;
       private IUserBLL _userBll;

       private string includeTables = "PBCK4_ITEM,PBCK4_DOCUMENT";

       public PBCK4BLL(IUnitOfWork uow, ILogger logger)
       {
           _logger = logger;
           _uow = uow;

           _repository = _uow.GetGenericRepository<PBCK4>();
           _repositoryPbck4Items = _uow.GetGenericRepository<PBCK4_ITEM>();

           _monthBll = new MonthBLL(_uow, _logger);
           _docSeqNumBll = new DocumentSequenceNumberBLL(_uow, _logger);
           _workflowHistoryBll = new WorkflowHistoryBLL(_uow,_logger);
           _changesHistoryBll = new ChangesHistoryBLL(_uow,_logger);
           _printHistoryBll = new PrintHistoryBLL(_uow,_logger);
           _brandRegistrationServices = new BrandRegistrationService(_uow, _logger);
           _ck1Bll = new CK1BLL(_uow,_logger);
           _messageService = new MessageService(_logger);
           _poaBll = new POABLL(_uow,_logger);
           _userBll = new UserBLL(_uow,_logger);
       }

       public List<Pbck4Dto> GetPbck4ByParam(Pbck4GetByParamInput input)
       {

           Expression<Func<PBCK4, bool>> queryFilter = PredicateHelper.True<PBCK4>();

           if (!string.IsNullOrEmpty(input.NppbkcId))
           {
               queryFilter = queryFilter.And(c => c.NPPBKC_ID.Contains(input.NppbkcId));
           }

           if (!string.IsNullOrEmpty(input.PlantId))
           {
               queryFilter = queryFilter.And(c => c.PLANT_ID.Contains(input.PlantId));

           }

           if (input.ReportedOn.HasValue)
           {
               queryFilter =
                   queryFilter.And(
                       c => c.REPORTED_ON == input.ReportedOn.Value);
           }


           if (!string.IsNullOrEmpty(input.Poa))
           {
               queryFilter = queryFilter.And(c => c.APPROVED_BY_POA.Contains(input.Poa));
           }

           if (!string.IsNullOrEmpty(input.Creator))
           {
               queryFilter = queryFilter.And(c => c.CREATED_BY.Contains(input.Creator));
           }

           if (input.IsCompletedDocument)
           {
               queryFilter = queryFilter.And(c => c.STATUS == Enums.DocumentStatus.Completed);
           }
           else
           {
               queryFilter = queryFilter.And(c => c.STATUS != Enums.DocumentStatus.Completed);
           }

           Func<IQueryable<PBCK4>, IOrderedQueryable<PBCK4>> orderByFilter = n => n.OrderByDescending(z => z.CREATED_DATE);
        

           var rc = _repository.Get(queryFilter, orderByFilter, includeTables);
           if (rc == null)
           {
               throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);
           }

           var mapResult = Mapper.Map<List<Pbck4Dto>>(rc.ToList());

           return mapResult;


       }

       public Pbck4Dto GetPbck4ById(int id)
       {
           var dtData = _repository.Get(c => c.PBCK4_ID == id, null, includeTables).FirstOrDefault();
           if (dtData == null)
               throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

           return Mapper.Map<Pbck4Dto>(dtData);

       }

       public Pbck4DetailsOutput GetDetailsPbck4(int id)
       {
           var output = new Pbck4DetailsOutput();

           var dtData = _repository.Get(c => c.PBCK4_ID == id, null, includeTables).FirstOrDefault();
           if (dtData == null)
               throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

           output.Pbck4Dto = Mapper.Map<Pbck4Dto>(dtData);

           //details
           output.Pbck4ItemsDto = Mapper.Map<List<Pbck4ItemDto>>(dtData.PBCK4_ITEM);

           //change history data
           output.ListChangesHistorys = _changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.PBCK4, output.Pbck4Dto.PBCK4_ID.ToString());

           //workflow history
           var input = new GetByFormNumberInput();
           input.FormNumber = dtData.PBCK4_NUMBER;
           input.DocumentStatus = dtData.STATUS;
           input.NPPBKC_Id = dtData.NPPBKC_ID;
           
           output.ListWorkflowHistorys = _workflowHistoryBll.GetByFormNumber(input);


           output.ListPrintHistorys = _printHistoryBll.GetByFormTypeAndFormId(Enums.FormType.PBCK4, dtData.PBCK4_ID);
           return output;
       }



       public Pbck4Dto SavePbck4(Pbck4SaveInput input)
       {
           //workflowhistory
           var inputWorkflowHistory = new Pbck4WorkflowHistoryInput();

           PBCK4 dbData = null;
           if (input.Pbck4Dto.PBCK4_ID > 0)
           {
               //update
               dbData = _repository.Get(c => c.PBCK4_ID == input.Pbck4Dto.PBCK4_ID, null, includeTables).FirstOrDefault();
               if (dbData == null)
                   throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

               //set changes history
               var origin = Mapper.Map<Pbck4Dto>(dbData);
               
               SetChangesHistory(origin, input.Pbck4Dto, input.UserId);

               Mapper.Map<Pbck4Dto, PBCK4>(input.Pbck4Dto, dbData);
               
               dbData.MODIFIED_DATE = DateTime.Now;
               dbData.MODIFIED_BY = input.UserId;


               //delete child first
               foreach (var pbck4Item in dbData.PBCK4_ITEM.ToList())
               {
                   _repositoryPbck4Items.Delete(pbck4Item);
               }

               inputWorkflowHistory.ActionType = Enums.ActionType.Modified;

               //insert new data
               foreach (var pbck4Material in input.Pbck4Items)
               {
                   var pbck4Item = Mapper.Map<PBCK4_ITEM>(pbck4Material);
                   pbck4Item.PLANT_ID = dbData.PLANT_ID;
                   dbData.PBCK4_ITEM.Add(pbck4Item);
               }

           }
           else
           {
              
               var generateNumberInput = new GenerateDocNumberInput()
               {
                   Year = DateTime.Now.Year,
                   Month = DateTime.Now.Month,
                   NppbkcId = input.Pbck4Dto.NppbkcId,
                   FormType = Enums.FormType.PBCK4
               };

               input.Pbck4Dto.PBCK4_NUMBER = _docSeqNumBll.GenerateNumber(generateNumberInput);
              
               input.Pbck4Dto.Status = Enums.DocumentStatus.Draft;
               input.Pbck4Dto.CREATED_DATE = DateTime.Now;
               input.Pbck4Dto.CREATED_BY = input.UserId;

               dbData = new PBCK4();

               Mapper.Map<Pbck4Dto, PBCK4>(input.Pbck4Dto, dbData);

               inputWorkflowHistory.ActionType = Enums.ActionType.Created;

               //insert new data
               foreach (var pbck4Material in input.Pbck4Items)
               {
                   
                   var pbck4Item = Mapper.Map<PBCK4_ITEM>(pbck4Material);
                   pbck4Item.PLANT_ID = dbData.PLANT_ID;
                   dbData.PBCK4_ITEM.Add(pbck4Item);
               }

               _repository.Insert(dbData);

           
           }

           inputWorkflowHistory.DocumentId = dbData.PBCK4_ID;
           inputWorkflowHistory.DocumentNumber = dbData.PBCK4_NUMBER;
           inputWorkflowHistory.UserId = input.UserId;
           inputWorkflowHistory.UserRole = input.UserRole;


           AddWorkflowHistory(inputWorkflowHistory);

           try
           {
               _uow.SaveChanges();
           }
           catch (DbEntityValidationException e)
           {
               foreach (var eve in e.EntityValidationErrors)
               {
                   Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                       eve.Entry.Entity.GetType().Name, eve.Entry.State);
                   foreach (var ve in eve.ValidationErrors)
                   {
                       Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                           ve.PropertyName, ve.ErrorMessage);
                   }
               }
               throw;
           }


           return Mapper.Map<Pbck4Dto>(dbData);
       }

       private void AddWorkflowHistory(Pbck4WorkflowHistoryInput input)
       {
           var inputWorkflowHistory = new GetByActionAndFormNumberInput();
           inputWorkflowHistory.ActionType = input.ActionType;
           inputWorkflowHistory.FormNumber = input.DocumentNumber;

           var dbData = new WorkflowHistoryDto();
           dbData.ACTION = input.ActionType;
           dbData.FORM_NUMBER = input.DocumentNumber;
           dbData.FORM_TYPE_ID = Enums.FormType.PBCK4;

           dbData.FORM_ID = input.DocumentId;
           if (!string.IsNullOrEmpty(input.Comment))
               dbData.COMMENT = input.Comment;


           dbData.ACTION_BY = input.UserId;
           dbData.ROLE = input.UserRole;
           dbData.ACTION_DATE = DateTime.Now;

           _workflowHistoryBll.Save(dbData);
       }

       private void AddWorkflowHistory(Pbck4WorkflowDocumentInput input)
       {
           var inputWorkflowHistory = new Pbck4WorkflowHistoryInput();

           inputWorkflowHistory.DocumentId = input.DocumentId;
           inputWorkflowHistory.DocumentNumber = input.DocumentNumber;
           inputWorkflowHistory.UserId = input.UserId;
           inputWorkflowHistory.UserRole = input.UserRole;
           inputWorkflowHistory.ActionType = input.ActionType;
           inputWorkflowHistory.Comment = input.Comment;

           AddWorkflowHistory(inputWorkflowHistory);
       }
       
       private void SetChangesHistory(Pbck4Dto origin, Pbck4Dto data, string userId)
       {
            var changesData = new Dictionary<string, bool>();

           changesData.Add("PLANT_ID", origin.PlantId == data.PlantId);
           changesData.Add("REPORTED_ON", origin.ReportedOn == data.ReportedOn);

           foreach (var listChange in changesData)
           {
               if (listChange.Value) continue;
               var changes = new CHANGES_HISTORY();
               changes.FORM_TYPE_ID = Enums.MenuList.PBCK4;
               changes.FORM_ID = origin.PBCK4_ID.ToString();
               changes.FIELD_NAME = listChange.Key;
               changes.MODIFIED_BY = userId;
               changes.MODIFIED_DATE = DateTime.Now;
               switch (listChange.Key)
               {
                   case "PLANT_ID":
                       changes.OLD_VALUE = origin.PlantId;
                       changes.NEW_VALUE = data.PlantId;
                       break;

                   case "REPORTED_ON":
                       changes.OLD_VALUE = origin.ReportedOn.HasValue ? origin.ReportedOn.Value.ToString("dd MMM yyyy") : string.Empty;
                       changes.NEW_VALUE = data.ReportedOn.HasValue ? data.ReportedOn.Value.ToString("dd MMM yyyy") : string.Empty;
                       break;
                 
               }
               _changesHistoryBll.AddHistory(changes);
           }
       }

       private void SetChangeHistory(string oldValue, string newValue, string fieldName, string userId, string ck5Id)
       {
           var changes = new CHANGES_HISTORY();
           changes.FORM_TYPE_ID = Enums.MenuList.CK5;
           changes.FORM_ID = ck5Id;
           changes.FIELD_NAME = fieldName;
           changes.MODIFIED_BY = userId;
           changes.MODIFIED_DATE = DateTime.Now;

           changes.OLD_VALUE = oldValue;
           changes.NEW_VALUE = newValue;

           _changesHistoryBll.AddHistory(changes);

       }

       private List<Pbck4ItemsOutput> ValidatePbck4Items(List<Pbck4ItemsInput> inputs)
       {
           var messageList = new List<string>();
           var outputList = new List<Pbck4ItemsOutput>();

           foreach (var pbck4ItemInput in inputs)
           {
               messageList.Clear();
               
               var output = Mapper.Map<Pbck4ItemsOutput>(pbck4ItemInput);

               var dbBrand = _brandRegistrationServices.GetByPlantIdAndFaCode(pbck4ItemInput.Plant, pbck4ItemInput.FaCode);
               if (dbBrand == null)
                   messageList.Add("FA Code Not Exist");

               var dbCk1 = _ck1Bll.GetCk1ByCk1Number(pbck4ItemInput.Ck1No);
               if (dbCk1 == null)
                   messageList.Add("CK-1 Number Not Exist");

               if (!ConvertHelper.IsNumeric(pbck4ItemInput.ReqQty))
                   messageList.Add("Req Qty not valid");

               if (!ConvertHelper.IsNumeric(pbck4ItemInput.ApprovedQty))
                   messageList.Add("Approved Qty not valid");

               if (!string.IsNullOrEmpty(pbck4ItemInput.NoPengawas))
               {
                   if (pbck4ItemInput.NoPengawas.Length > 10)
                       messageList.Add("No Pengawas Max Length 10");
               }

               if (messageList.Count > 0)
               {
                   output.IsValid = false;
                   output.Message = "";
                   foreach (var message in messageList)
                   {
                       output.Message += message + ";";
                   }
               }
               else
               {
                   output.IsValid = true;
               }

               outputList.Add(output);
           }

         
           return outputList;
       }

       private Pbck4ItemsOutput GetAdditionalValuePbck4Items(Pbck4ItemsOutput input)
       {
           var dbBrand = _brandRegistrationServices.GetByPlantIdAndFaCode(input.Plant, input.FaCode);
           if (dbBrand == null)
           {
               input.StickerCode = "";
               input.SeriesCode = "";
               input.Content = "0";
               input.Hje = "0";
               input.Tariff = "0";
               input.TotalHje = "0";

           }
           else
           {
               input.StickerCode = dbBrand.STICKER_CODE;
               input.SeriesCode = dbBrand.SERIES_CODE;
               input.BrandName = dbBrand.BRAND_CE;
               input.ProductAlias = dbBrand.ZAIDM_EX_PRODTYP.PRODUCT_ALIAS;

               input.Content = ConvertHelper.ConvertToDecimalOrZero(dbBrand.BRAND_CONTENT).ToString();

               input.Hje = dbBrand.HJE_IDR.HasValue ? dbBrand.HJE_IDR.Value.ToString() : "0";
               input.Tariff = dbBrand.TARIFF.HasValue ? dbBrand.TARIFF.Value.ToString() : "0";
               input.Colour = dbBrand.COLOUR;

               input.TotalHje = (ConvertHelper.GetDecimal(input.Hje)*ConvertHelper.GetDecimal(input.ReqQty)).ToString("f2");

               input.TotalStamps =
                   (ConvertHelper.GetDecimal(input.Hje)*ConvertHelper.GetDecimal(input.ReqQty)*
                    ConvertHelper.GetDecimal(input.Content)).ToString("f2");


           }

           var dbCk1 = _ck1Bll.GetCk1ByCk1Number(input.Ck1No);
           if (dbCk1 == null)
           {
               input.Ck1Date = "";
           }
           else
           {
               input.CK1_ID = dbCk1.CK1_ID;
               input.Ck1Date = dbCk1.CK1_DATE.ToString("dd MMM yyyy");
           }

         

           return input;
       }

       public List<Pbck4ItemsOutput> Pbck4ItemProcess(List<Pbck4ItemsInput> inputs)
       {
           var outputList = ValidatePbck4Items(inputs);

           if (!outputList.All(c => c.IsValid))
               return outputList;

           foreach (var output in outputList)
           {
               var resultValue = GetAdditionalValuePbck4Items(output);


               output.StickerCode = resultValue.StickerCode;
               output.SeriesCode = resultValue.SeriesCode;
               output.BrandName = resultValue.BrandName;
               output.ProductAlias = resultValue.ProductAlias;

               output.Content = resultValue.Content;

               output.Hje = resultValue.Hje;
               output.Tariff = resultValue.Tariff;
               output.Colour = resultValue.Colour;

               output.TotalHje = resultValue.TotalHje;

               output.TotalStamps = resultValue.TotalStamps;
               output.CK1_ID = resultValue.CK1_ID;
           }

           return outputList;
       }

       public void PBCK4Workflow(Pbck4WorkflowDocumentInput input)
       {
           var isNeedSendNotif = false;

           switch (input.ActionType)
           {
               case Enums.ActionType.Submit:
                   SubmitDocument(input);
                   isNeedSendNotif = true;
                   break;
               case Enums.ActionType.Approve:
                   ApproveDocument(input);
                   isNeedSendNotif = true;
                   break;
               case Enums.ActionType.Reject:
                   RejectDocument(input);
                   isNeedSendNotif = true;
                   break;
               case Enums.ActionType.Cancel:
                   CancelledDocument(input);
                   break;
               case Enums.ActionType.GovApprove:
                   GovApproveDocument(input);
                   break;
               case Enums.ActionType.GovReject:
                   GovRejectedDocument(input);
                   break;
               case Enums.ActionType.GovPartialApprove:
                   GovPartialApproveDocument(input);
                   break;
              
           }

           //todo sent mail
           if (isNeedSendNotif)
               SendEmailWorkflow(input);


           _uow.SaveChanges();
       }

       private void SendEmailWorkflow(Pbck4WorkflowDocumentInput input)
       {
           //todo: body message from email template
           //todo: to = ?
           //todo: subject = from email template
           //var to = "irmansulaeman41@gmail.com";
           //var subject = "this is subject for " + input.DocumentNumber;
           //var body = "this is body message for " + input.DocumentNumber;
           //var from = "a@gmail.com";

           var pbck4Dto = Mapper.Map<Pbck4Dto>(_repository.Get(c => c.PBCK4_ID == input.DocumentId).FirstOrDefault());

           var mailProcess = ProsesMailNotificationBody(pbck4Dto, input.ActionType);

           _messageService.SendEmailToList(mailProcess.To, mailProcess.Subject, mailProcess.Body, true);

       }

       private MailNotification ProsesMailNotificationBody(Pbck4Dto pbck4Dto, Enums.ActionType actionType)
       {
           var bodyMail = new StringBuilder();
           var rc = new MailNotification();

           var webRootUrl = ConfigurationManager.AppSettings["WebRootUrl"];

           rc.Subject = "PBCK-4 " + pbck4Dto.PBCK4_NUMBER + " is " + EnumHelper.GetDescription(pbck4Dto.Status);
           bodyMail.Append("Dear Team,<br />");
           bodyMail.AppendLine();
           bodyMail.Append("Kindly be informed, " + rc.Subject + ". <br />");
           bodyMail.AppendLine();
           bodyMail.Append("<table><tr><td>Company Code </td><td>: " + pbck4Dto.COMPANY_ID + "</td></tr>");
           bodyMail.AppendLine();
           bodyMail.Append("<tr><td>NPPBKC </td><td>: " + pbck4Dto.NppbkcId + "</td></tr>");
           bodyMail.AppendLine();
           bodyMail.Append("<tr><td>Document Number</td><td> : " + pbck4Dto.PBCK4_NUMBER + "</td></tr>");
           bodyMail.AppendLine();
           bodyMail.Append("<tr><td>Document Type</td><td> : PBCK-4</td></tr>");
           bodyMail.AppendLine();
           bodyMail.Append("<tr colspan='2'><td><i>Please click this <a href='" + webRootUrl + "/PBCK4/Details/" + pbck4Dto.PBCK4_ID + "'>link</a> to show detailed information</i></td></tr>");
           bodyMail.AppendLine();
           bodyMail.Append("</table>");
           bodyMail.AppendLine();
           bodyMail.Append("<br />Regards,<br />");
           switch (actionType)
           {
               case Enums.ActionType.Submit:
                   if (pbck4Dto.Status == Enums.DocumentStatus.WaitingForApproval)
                   {
                       var poaList = _poaBll.GetPoaByNppbkcId(pbck4Dto.NppbkcId);

                       foreach (var poaDto in poaList)
                       {
                           rc.To.Add(poaDto.POA_EMAIL);
                       }
                   }
                   else if (pbck4Dto.Status == Enums.DocumentStatus.WaitingForApprovalManager)
                   {
                       var managerId = _poaBll.GetManagerIdByPoaId(pbck4Dto.CREATED_BY);
                       var managerDetail = _userBll.GetUserById(managerId);
                       rc.To.Add(managerDetail.EMAIL);
                   }
                   break;
               case Enums.ActionType.Approve:
                   if (pbck4Dto.Status == Enums.DocumentStatus.WaitingForApprovalManager)
                   {
                       rc.To.Add(GetManagerEmail(pbck4Dto.APPROVED_BY_POA));
                   }
                   else if (pbck4Dto.Status == Enums.DocumentStatus.WaitingGovApproval)
                   {
                       var poaData = _poaBll.GetById(pbck4Dto.CREATED_BY);
                       if (poaData != null)
                       {
                           //creator is poa user
                           rc.To.Add(poaData.POA_EMAIL);
                       }
                       else
                       {
                           //creator is excise executive
                           var userData = _userBll.GetUserById(pbck4Dto.CREATED_BY);
                           rc.To.Add(userData.EMAIL);
                       }
                   }
                   break;
               case Enums.ActionType.Reject:
                   //send notification to creator
                   var userDetail = _userBll.GetUserById(pbck4Dto.CREATED_BY);
                   rc.To.Add(userDetail.EMAIL);
                   break;
           }
           rc.Body = bodyMail.ToString();
           return rc;
       }

       private string GetManagerEmail(string poaId)
       {
           var managerId = _poaBll.GetManagerIdByPoaId(poaId);
           var managerDetail = _userBll.GetUserById(managerId);
           return managerDetail.EMAIL;
       }


       private void SubmitDocument(Pbck4WorkflowDocumentInput input)
       {
           var dbData = _repository.GetByID(input.DocumentId);

           if (dbData == null)
               throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

           if (dbData.STATUS != Enums.DocumentStatus.Draft)
               throw new BLLException(ExceptionCodes.BLLExceptions.OperationNotAllowed);

           string newValue = "";
           string oldValue = EnumHelper.GetDescription(dbData.STATUS);

           dbData.STATUS = Enums.DocumentStatus.WaitingForApproval;

           input.DocumentNumber = dbData.PBCK4_NUMBER;

           AddWorkflowHistory(input);

           switch (input.UserRole)
           {
               case Enums.UserRole.User:
                   dbData.STATUS = Enums.DocumentStatus.WaitingForApproval;
                   newValue = EnumHelper.GetDescription(Enums.DocumentStatus.WaitingForApproval);
                   break;
               case Enums.UserRole.POA:
                   dbData.STATUS = Enums.DocumentStatus.WaitingForApprovalManager;
                   newValue = EnumHelper.GetDescription(Enums.DocumentStatus.WaitingForApprovalManager);
                   break;
               default:
                   throw new BLLException(ExceptionCodes.BLLExceptions.OperationNotAllowed);
           }

           //set change history
           SetChangeHistory(oldValue, newValue, "STATUS", input.UserId, dbData.PBCK4_ID.ToString());


       }

       private void ApproveDocument(Pbck4WorkflowDocumentInput input)
       {
           var dbData = _repository.GetByID(input.DocumentId);

           if (dbData == null)
               throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

           if (dbData.STATUS != Enums.DocumentStatus.WaitingForApproval &&
               dbData.STATUS != Enums.DocumentStatus.WaitingForApprovalManager)
               throw new BLLException(ExceptionCodes.BLLExceptions.OperationNotAllowed);

           string oldValue = EnumHelper.GetDescription(dbData.STATUS);
           string newValue = "";

           if (input.UserRole == Enums.UserRole.POA)
           {
               dbData.STATUS = Enums.DocumentStatus.WaitingForApprovalManager;
               dbData.APPROVED_BY_POA = input.UserId;
               dbData.APPROVED_BY_POA_DATE = DateTime.Now;
               newValue = EnumHelper.GetDescription(Enums.DocumentStatus.WaitingForApprovalManager);
           }
           else if (input.UserRole == Enums.UserRole.Manager)
           {
               dbData.STATUS = Enums.DocumentStatus.WaitingGovApproval;
               dbData.APPROVED_BY_MANAGER = input.UserId;
               dbData.APPROVED_BY_MANAGER_DATE = DateTime.Now;
               newValue = EnumHelper.GetDescription(Enums.DocumentStatus.WaitingGovApproval);
           }


           input.DocumentNumber = dbData.PBCK4_NUMBER;

           AddWorkflowHistory(input);

           //set change history
           SetChangeHistory(oldValue, newValue, "STATUS", input.UserId, dbData.PBCK4_ID.ToString());

       }

       private void RejectDocument(Pbck4WorkflowDocumentInput input)
       {
           var dbData = _repository.GetByID(input.DocumentId);

           if (dbData == null)
               throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

           if (dbData.STATUS != Enums.DocumentStatus.WaitingForApproval &&
               dbData.STATUS != Enums.DocumentStatus.WaitingForApprovalManager &&
               dbData.STATUS != Enums.DocumentStatus.WaitingGovApproval)
               throw new BLLException(ExceptionCodes.BLLExceptions.OperationNotAllowed);

           string oldValue = EnumHelper.GetDescription(dbData.STATUS);
           string newValue = "";

           //change back to draft
           dbData.STATUS = Enums.DocumentStatus.Draft;
           newValue = EnumHelper.GetDescription(Enums.DocumentStatus.Draft);

           dbData.REJECTED_BY = input.UserId;
           dbData.REJECTED_DATE = DateTime.Now;

           input.DocumentNumber = dbData.PBCK4_NUMBER;

           AddWorkflowHistory(input);

           //set change history
           SetChangeHistory(oldValue, newValue, "STATUS", input.UserId, dbData.PBCK4_ID.ToString());
       }

       private void CancelledDocument(Pbck4WorkflowDocumentInput input)
       {
           var dbData = _repository.GetByID(input.DocumentId);

           if (dbData == null)
               throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

           if (dbData.STATUS != Enums.DocumentStatus.Draft)
               throw new BLLException(ExceptionCodes.BLLExceptions.OperationNotAllowed);

           string oldValue = EnumHelper.GetDescription(dbData.STATUS);
           string newValue = EnumHelper.GetDescription(Enums.DocumentStatus.Cancelled); ;
           //set change history
           if (oldValue != newValue)
               SetChangeHistory(oldValue, newValue, "STATUS", input.UserId, dbData.PBCK4_ID.ToString());


           dbData.STATUS = Enums.DocumentStatus.Cancelled;


           input.DocumentNumber = dbData.PBCK4_NUMBER;

           AddWorkflowHistory(input);
       }

       private void WorkflowStatusAddChanges(Pbck4WorkflowDocumentInput input, Enums.DocumentStatus oldStatus, Enums.DocumentStatus newStatus)
       {
           //set changes log
           var changes = new CHANGES_HISTORY
           {
               FORM_TYPE_ID = Enums.MenuList.PBCK4,
               FORM_ID = input.DocumentId.ToString(),
               FIELD_NAME = "STATUS",
               NEW_VALUE = EnumHelper.GetDescription(newStatus),
               OLD_VALUE = EnumHelper.GetDescription(oldStatus),
               MODIFIED_BY = input.UserId,
               MODIFIED_DATE = DateTime.Now
           };
           _changesHistoryBll.AddHistory(changes);
       }

       private void WorkflowStatusGovAddChanges(Pbck4WorkflowDocumentInput input, Enums.DocumentStatusGov? oldStatus, Enums.DocumentStatusGov newStatus)
       {
           //set changes log
           var changes = new CHANGES_HISTORY
           {
               FORM_TYPE_ID = Enums.MenuList.PBCK4,
               FORM_ID = input.DocumentId.ToString(),
               FIELD_NAME = "GOV_STATUS",
               NEW_VALUE = EnumHelper.GetDescription(newStatus),
               OLD_VALUE = oldStatus.HasValue ? EnumHelper.GetDescription(oldStatus) : "NULL",
               MODIFIED_BY = input.UserId,
               MODIFIED_DATE = DateTime.Now
           };

           _changesHistoryBll.AddHistory(changes);
       }

       private void GovApproveDocument(Pbck4WorkflowDocumentInput input)
       {
           var dbData = _repository.GetByID(input.DocumentId);

           if (dbData == null)
               throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

           if (dbData.STATUS != Enums.DocumentStatus.WaitingGovApproval)
               throw new BLLException(ExceptionCodes.BLLExceptions.OperationNotAllowed);

           //Add Changes
           WorkflowStatusAddChanges(input, dbData.STATUS, Enums.DocumentStatus.Completed);
           WorkflowStatusGovAddChanges(input, dbData.GOV_STATUS, Enums.DocumentStatusGov.FullApproved);

           dbData.STATUS = Enums.DocumentStatus.Completed;
         
           dbData.BACK1_NO = input.AdditionalDocumentData.Back1No;
           dbData.BACK1_DATE = input.AdditionalDocumentData.Back1Date;
           
           dbData.CK3_NO = input.AdditionalDocumentData.Ck3No;
           dbData.CK3_DATE = input.AdditionalDocumentData.Ck3Date;
           dbData.CK3_OFFICE_VALUE = input.AdditionalDocumentData.Ck3OfficeValue;

           dbData.GOV_STATUS = Enums.DocumentStatusGov.FullApproved;

           dbData.MODIFIED_DATE = DateTime.Now;
           dbData.MODIFIED_BY = input.UserId;

           var pbckDocument = input.AdditionalDocumentData.Back1FileUploadList.ToList();
           pbckDocument.AddRange(input.AdditionalDocumentData.Ck3FileUploadList);

           dbData.PBCK4_DOCUMENT = Mapper.Map<List<PBCK4_DOCUMENT>>(pbckDocument);

           input.DocumentNumber = dbData.PBCK4_NUMBER;

           AddWorkflowHistory(input);

       }

       private void GovPartialApproveDocument(Pbck4WorkflowDocumentInput input)
       {
           var dbData = _repository.GetByID(input.DocumentId);

           if (dbData == null)
               throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

           if (dbData.STATUS != Enums.DocumentStatus.WaitingGovApproval)
               throw new BLLException(ExceptionCodes.BLLExceptions.OperationNotAllowed);

           //Add Changes
           WorkflowStatusAddChanges(input, dbData.STATUS, Enums.DocumentStatus.Completed);
           WorkflowStatusGovAddChanges(input, dbData.GOV_STATUS, Enums.DocumentStatusGov.PartialApproved);

           dbData.STATUS = Enums.DocumentStatus.Completed;

           dbData.BACK1_NO = input.AdditionalDocumentData.Back1No;
           dbData.BACK1_DATE = input.AdditionalDocumentData.Back1Date;

           dbData.CK3_NO = input.AdditionalDocumentData.Ck3No;
           dbData.CK3_DATE = input.AdditionalDocumentData.Ck3Date;
           dbData.CK3_OFFICE_VALUE = input.AdditionalDocumentData.Ck3OfficeValue;

           dbData.GOV_STATUS = Enums.DocumentStatusGov.PartialApproved;

           dbData.MODIFIED_DATE = DateTime.Now;
           dbData.MODIFIED_BY = input.UserId;

           var pbckDocument = input.AdditionalDocumentData.Back1FileUploadList.ToList();
           pbckDocument.AddRange(input.AdditionalDocumentData.Ck3FileUploadList);

           dbData.PBCK4_DOCUMENT = Mapper.Map<List<PBCK4_DOCUMENT>>(pbckDocument);

           input.DocumentNumber = dbData.PBCK4_NUMBER;

           AddWorkflowHistory(input);
       }

       private void GovRejectedDocument(Pbck4WorkflowDocumentInput input)
       {
           var dbData = _repository.GetByID(input.DocumentId);

           if (dbData == null)
               throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

           if (dbData.STATUS != Enums.DocumentStatus.WaitingGovApproval)
               throw new BLLException(ExceptionCodes.BLLExceptions.OperationNotAllowed);

           //Add Changes
           WorkflowStatusAddChanges(input, dbData.STATUS, Enums.DocumentStatus.GovRejected);
           WorkflowStatusGovAddChanges(input, dbData.GOV_STATUS, Enums.DocumentStatusGov.Rejected);

           dbData.STATUS = Enums.DocumentStatus.GovRejected;
           dbData.GOV_STATUS = Enums.DocumentStatusGov.Rejected;

           dbData.MODIFIED_DATE = DateTime.Now;
           dbData.MODIFIED_BY = input.UserId;

           input.DocumentNumber = dbData.PBCK4_NUMBER;

           AddWorkflowHistory(input);

       }
    }
}
