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
       private IGenericRepository<PBCK4_DOCUMENT> _repositoryPbck4Documents;

       private IMonthBLL _monthBll;
       private IDocumentSequenceNumberBLL _docSeqNumBll;
       private IWorkflowHistoryBLL _workflowHistoryBll;
       private IChangesHistoryBLL _changesHistoryBll;
       private IPrintHistoryBLL _printHistoryBll;
       private IBrandRegistrationService _brandRegistrationServices;
       private ICK1Services _ck1Services;
       private IMessageService _messageService;
       private IPOABLL _poaBll;
       private IUserBLL _userBll;
       private IZaidmExNPPBKCBLL _nppbkcBll;
       private IPlantBLL _plantBll;
       private IBlockStockBLL _blockStockBll;
       private IHeaderFooterBLL _headerFooterBll;
       private IWorkflowBLL _workflowBll;

       private string includeTables = "PBCK4_ITEM,PBCK4_DOCUMENT, POA, USER, PBCK4_ITEM.CK1";

       public PBCK4BLL(IUnitOfWork uow, ILogger logger)
       {
           _logger = logger;
           _uow = uow;

           _repository = _uow.GetGenericRepository<PBCK4>();
           _repositoryPbck4Items = _uow.GetGenericRepository<PBCK4_ITEM>();
           _repositoryPbck4Documents = _uow.GetGenericRepository<PBCK4_DOCUMENT>();

           _monthBll = new MonthBLL(_uow, _logger);
           _docSeqNumBll = new DocumentSequenceNumberBLL(_uow, _logger);
           _workflowHistoryBll = new WorkflowHistoryBLL(_uow,_logger);
           _changesHistoryBll = new ChangesHistoryBLL(_uow,_logger);
           _printHistoryBll = new PrintHistoryBLL(_uow,_logger);
           _brandRegistrationServices = new BrandRegistrationService(_uow, _logger);
           _ck1Services = new CK1Services(_uow, _logger);
           _messageService = new MessageService(_logger);
           _poaBll = new POABLL(_uow,_logger);
           _userBll = new UserBLL(_uow,_logger);
           _nppbkcBll = new ZaidmExNPPBKCBLL(_uow, _logger);
           _plantBll = new PlantBLL(_uow, _logger);
           _blockStockBll = new BlockStockBLL(_uow,_logger);
           _headerFooterBll = new HeaderFooterBLL(_uow, _logger);
           _workflowBll = new WorkflowBLL(_uow, _logger);
       }

       public List<Pbck4Dto> GetPbck4ByParam(Pbck4GetByParamInput input)
       {

           Expression<Func<PBCK4, bool>> queryFilter = PredicateHelper.True<PBCK4>();

           if (input.UserRole == Enums.UserRole.POA)
           {
               var nppbkc = _nppbkcBll.GetNppbkcsByPOA(input.UserId).Select(d => d.NPPBKC_ID).ToList();

               queryFilter = queryFilter.And(c => (c.CREATED_BY == input.UserId || (c.STATUS != Enums.DocumentStatus.Draft && nppbkc.Contains(c.NPPBKC_ID))));


           }
           else if (input.UserRole == Enums.UserRole.Manager)
           {
               var poaList = _poaBll.GetPOAIdByManagerId(input.UserId);
               var document = _workflowHistoryBll.GetDocumentByListPOAId(poaList);

               queryFilter = queryFilter.And(c => c.STATUS != Enums.DocumentStatus.Draft && c.STATUS != Enums.DocumentStatus.WaitingForApproval && document.Contains(c.PBCK4_NUMBER));
           }
           else
           {
               queryFilter = queryFilter.And(c => c.CREATED_BY == input.UserId);
           }

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
               queryFilter = queryFilter.And(c => c.STATUS == Enums.DocumentStatus.Completed || c.STATUS == Enums.DocumentStatus.Cancelled);
           }
           else
           {
               queryFilter = queryFilter.And(c => !(c.STATUS == Enums.DocumentStatus.Completed || c.STATUS == Enums.DocumentStatus.Cancelled));
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
           bool isModified = false;

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

               isModified = SetChangesHistory(origin, input.Pbck4Dto, input.UserId);

               Mapper.Map<Pbck4Dto, PBCK4>(input.Pbck4Dto, dbData);

               if (dbData.STATUS == Enums.DocumentStatus.Rejected)
               {
                   dbData.STATUS = Enums.DocumentStatus.Draft;
               }

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

           var resultDto = Mapper.Map<Pbck4Dto>(dbData);
           resultDto.IsModifiedHistory = isModified;

           return resultDto;

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

           if (!input.IsModified && input.ActionType == Enums.ActionType.Submit)
               _workflowHistoryBll.UpdateHistoryModifiedForSubmit(dbData);
           else
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
           inputWorkflowHistory.IsModified = input.IsModified;

           AddWorkflowHistory(inputWorkflowHistory);
       }
       
       private bool SetChangesHistory(Pbck4Dto origin, Pbck4Dto data, string userId)
       {
           bool isModified = false;

            var changesData = new Dictionary<string, bool>();

           changesData.Add("PLANT_ID", origin.PlantId == data.PlantId);
           changesData.Add("REPORTED_ON", origin.ReportedOn == data.ReportedOn);

           if (origin.Status == Enums.DocumentStatus.WaitingGovApproval ||
               origin.Status == Enums.DocumentStatus.Completed)
           {
               changesData.Add("BACK1_NO", origin.BACK1_NO == data.BACK1_NO);
               changesData.Add("BACK1_DATE", origin.BACK1_DATE == data.BACK1_DATE);
               changesData.Add("CK3_NO", origin.CK3_NO == data.CK3_NO);
               changesData.Add("CK3_DATE", origin.CK3_DATE == data.CK3_DATE);
               changesData.Add("CK3_OFFICE_VALUE", origin.CK3_OFFICE_VALUE == data.CK3_OFFICE_VALUE);
           }
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

                   case "BACK1_NO":
                       changes.OLD_VALUE = origin.BACK1_NO;
                       changes.NEW_VALUE = data.BACK1_NO;
                       break;

                   case "BACK1_DATE":
                       changes.OLD_VALUE = ConvertHelper.ConvertDateToStringddMMMyyyy(origin.BACK1_DATE);
                       changes.NEW_VALUE = ConvertHelper.ConvertDateToStringddMMMyyyy(data.BACK1_DATE);
                       break;

                   case "CK3_NO":
                       changes.OLD_VALUE = origin.CK3_NO;
                       changes.NEW_VALUE = data.CK3_NO;
                       break;

                   case "CK3_DATE":
                       changes.OLD_VALUE = ConvertHelper.ConvertDateToStringddMMMyyyy(origin.CK3_DATE);
                       changes.NEW_VALUE = ConvertHelper.ConvertDateToStringddMMMyyyy(data.CK3_DATE);
                       break;

                   case "CK3_OFFICE_VALUE":
                       changes.OLD_VALUE = ConvertHelper.ConvertDecimalToString(origin.CK3_OFFICE_VALUE);
                       changes.NEW_VALUE = ConvertHelper.ConvertDecimalToString(data.CK3_OFFICE_VALUE);
                       break;

               }
               
               _changesHistoryBll.AddHistory(changes);
               isModified = true;
           }
           return isModified;
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

       private void SetChangesHistory(Pbck4Dto origin, Pbck4WorkflowDocumentInput input, string userId)
       {
           var changesData = new Dictionary<string, bool>();


           changesData.Add("BACK1_NO", origin.BACK1_NO == input.AdditionalDocumentData.Back1No);
           changesData.Add("BACK1_DATE", origin.BACK1_DATE == input.AdditionalDocumentData.Back1Date);
           changesData.Add("CK3_NO", origin.CK3_NO == input.AdditionalDocumentData.Ck3No);
           changesData.Add("CK3_DATE", origin.CK3_DATE == input.AdditionalDocumentData.Ck3Date);
           

           decimal? ck3OfficeValue = ConvertHelper.ConvertToDecimalOrZero(input.AdditionalDocumentData.Ck3OfficeValue);

           changesData.Add("CK3_OFFICE_VALUE", origin.CK3_OFFICE_VALUE == ck3OfficeValue);
           
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
                  

                   case "BACK1_NO":
                       changes.OLD_VALUE = origin.BACK1_NO;
                       changes.NEW_VALUE = input.AdditionalDocumentData.Back1No;
                       break;

                   case "BACK1_DATE":
                       changes.OLD_VALUE = ConvertHelper.ConvertDateToStringddMMMyyyy(origin.BACK1_DATE);
                       changes.NEW_VALUE = ConvertHelper.ConvertDateToStringddMMMyyyy(input.AdditionalDocumentData.Back1Date);
                       break;

                   case "CK3_NO":
                       changes.OLD_VALUE = origin.CK3_NO;
                       changes.NEW_VALUE = input.AdditionalDocumentData.Ck3No;
                       break;

                   case "CK3_DATE":
                       changes.OLD_VALUE = ConvertHelper.ConvertDateToStringddMMMyyyy(origin.CK3_DATE);
                       changes.NEW_VALUE = ConvertHelper.ConvertDateToStringddMMMyyyy(input.AdditionalDocumentData.Ck3Date);
                       break;

                   case "CK3_OFFICE_VALUE":
                       changes.OLD_VALUE = ConvertHelper.ConvertDecimalToString(origin.CK3_OFFICE_VALUE);
                       changes.NEW_VALUE = ConvertHelper.ConvertDecimalToString(ck3OfficeValue);
                       break;

               }

               _changesHistoryBll.AddHistory(changes);
           }
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

               var dbCk1 = _ck1Services.GetCk1ByCk1Number(pbck4ItemInput.Ck1No);
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

               //validate ReqQty to block stock

               var blockStockData = _blockStockBll.GetBlockStockByPlantAndMaterialId(pbck4ItemInput.Plant,
                   pbck4ItemInput.FaCode);
               if (blockStockData.Count == 0)
               {
                   messageList.Add("Block Stock not available");
               }
               else
               {
                   var blockDecimal = blockStockData.Sum(blockStockDto => blockStockDto.BLOCKED.HasValue ? blockStockDto.BLOCKED.Value : 0);
                   if (ConvertHelper.ConvertToDecimalOrZero(pbck4ItemInput.ReqQty) > blockDecimal)
                       messageList.Add("Req Qty more than Block Stock");
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

           var dbCk1 = _ck1Services.GetCk1ByCk1Number(input.Ck1No);
           if (dbCk1 == null)
           {
               input.Ck1Date = "";
           }
           else
           {
               input.CK1_ID = dbCk1.CK1_ID;
               input.Ck1Date = dbCk1.CK1_DATE.ToString("dd MMM yyyy");
           }


           var dbBlockStock = _blockStockBll.GetBlockStockByPlantAndMaterialId(input.Plant, input.FaCode);
           if (dbBlockStock.Count == 0)
               input.BlockedStock = "0";
           else
           {
               var sumBlockStock = dbBlockStock.Sum(blockStockDto => blockStockDto.BLOCKED.HasValue ? blockStockDto.BLOCKED.Value : 0);
               input.BlockedStock = sumBlockStock.ToString();

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
               output.BlockedStock = resultValue.BlockedStock;
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
               case Enums.ActionType.GovPartialApprove:
                   GovApproveDocument(input);
                   //isNeedSendNotif = true;
                   break;
               case Enums.ActionType.GovReject:
                   GovRejectedDocument(input);
                   break;
              
           }

           //todo sent mail
           if (isNeedSendNotif)
               SendEmailWorkflow(input);


           _uow.SaveChanges();
       }

       public void SendMailCompletedPbck4Document(Pbck4WorkflowDocumentInput input)
       {
           input.ActionType = Enums.ActionType.GovApprove;
           SendEmailWorkflow(input);
       }

       private void SendEmailWorkflow(Pbck4WorkflowDocumentInput input)
       {
         
           var pbck4Dto = Mapper.Map<Pbck4Dto>(_repository.Get(c => c.PBCK4_ID == input.DocumentId).FirstOrDefault());

           if ((input.ActionType == Enums.ActionType.GovApprove || input.ActionType == Enums.ActionType.GovPartialApprove)
               && pbck4Dto.Status != Enums.DocumentStatus.Completed)
               return;

           var mailProcess = ProsesMailNotificationBody(pbck4Dto, input);
           
           //distinct double To email
           List<string> ListTo = mailProcess.To.Distinct().ToList();
           
           if (mailProcess.IsCCExist)
               //Send email with CC
               _messageService.SendEmailToListWithCC(ListTo, mailProcess.CC, mailProcess.Subject, mailProcess.Body, true);
           else
               _messageService.SendEmailToList(ListTo, mailProcess.Subject, mailProcess.Body, true);

       }

       private MailNotification ProsesMailNotificationBody(Pbck4Dto pbck4Dto, Pbck4WorkflowDocumentInput input)
       {
           var bodyMail = new StringBuilder();
           var rc = new MailNotification();

           var rejected = _workflowHistoryBll.GetApprovedOrRejectedPOAStatusByDocumentNumber(new GetByFormTypeAndFormIdInput() { FormId = pbck4Dto.PBCK4_ID, FormType = Enums.FormType.PBCK4 });
           var poaList = _poaBll.GetPoaByNppbkcId(pbck4Dto.NppbkcId);

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
           if (input.ActionType == Enums.ActionType.Reject)
           {
               bodyMail.Append("<tr><td>Comment</td><td> : " + input.Comment + "</td></tr>");
               bodyMail.AppendLine();
           }
           else if (input.ActionType == Enums.ActionType.GovApprove || input.ActionType == Enums.ActionType.GovPartialApprove)
           {
               string back1Date = ConvertHelper.ConvertDateToString(pbck4Dto.BACK1_DATE,"dd MMMM yyyy");

               string ck3Date = ConvertHelper.ConvertDateToString(pbck4Dto.CK3_DATE,"dd MMMM yyyy");


               string ck3Value = ConvertHelper.ConvertDecimalToStringMoneyFormat(pbck4Dto.CK3_OFFICE_VALUE);


               bodyMail.Append("<tr><td>BACK-1 Number</td><td> : " + pbck4Dto.BACK1_NO + "</td></tr>");
               bodyMail.AppendLine();
               bodyMail.Append("<tr><td>BACK-1 Date</td><td> : " + back1Date + "</td></tr>");
               bodyMail.AppendLine();
               bodyMail.Append("<tr><td>CK-3 Number</td><td> : " + pbck4Dto.CK3_NO + "</td></tr>");
               bodyMail.AppendLine();
               bodyMail.Append("<tr><td>CK-3 Date</td><td> : " + ck3Date + "</td></tr>");
               bodyMail.AppendLine();
               bodyMail.Append("<tr><td>CK-3 Value</td><td> : " + ck3Value + "</td></tr>");
               bodyMail.AppendLine();
           }
           bodyMail.Append("<tr colspan='2'><td><i>Please click this <a href='" + webRootUrl + "/PBCK4/Edit/" + pbck4Dto.PBCK4_ID + "'>link</a> to show detailed information</i></td></tr>");
           bodyMail.AppendLine();
           bodyMail.Append("</table>");
           bodyMail.AppendLine();
           bodyMail.Append("<br />Regards,<br />");
           switch (input.ActionType)
           {
               case Enums.ActionType.Submit:
                 if (pbck4Dto.Status == Enums.DocumentStatus.WaitingForApproval)
                    {
                        if (rejected != null)
                        {
                            rc.To.Add(_poaBll.GetById(rejected.ACTION_BY).POA_EMAIL);
                        }
                        else
                        {
                            foreach (var poaDto in poaList)
                            {
                                rc.To.Add(poaDto.POA_EMAIL);
                            }
                        }

                        rc.CC.Add(_userBll.GetUserById(pbck4Dto.CREATED_BY).EMAIL);
                    }
                 else if (pbck4Dto.Status == Enums.DocumentStatus.WaitingForApprovalManager)
                    {
                        var poaData = _poaBll.GetById(pbck4Dto.CREATED_BY);
                        rc.To.Add(GetManagerEmail(pbck4Dto.CREATED_BY));
                        rc.CC.Add(poaData.POA_EMAIL);
                        
                        foreach (var poaDto in poaList)
                        {
                            if (poaData.POA_ID != poaDto.POA_ID)
                                rc.To.Add(poaDto.POA_EMAIL);
                        }
                    }
                    rc.IsCCExist = true;
                    break;
               case Enums.ActionType.Approve:
                    if (pbck4Dto.Status == Enums.DocumentStatus.WaitingForApprovalManager)
                    {
                        rc.To.Add(GetManagerEmail(pbck4Dto.APPROVED_BY_POA));

                        if (rejected != null)
                        {
                            rc.CC.Add(_poaBll.GetById(rejected.ACTION_BY).POA_EMAIL);
                        }
                        else
                        {
                            foreach (var poaDto in poaList)
                            {
                                rc.CC.Add(poaDto.POA_EMAIL);
                            }
                        }

                        rc.CC.Add(_userBll.GetUserById(pbck4Dto.CREATED_BY).EMAIL);

                    }
                    else if (pbck4Dto.Status == Enums.DocumentStatus.WaitingGovApproval)
                    {
                        var poaData = _poaBll.GetById(pbck4Dto.CREATED_BY);
                        if (poaData != null)
                        {
                            //creator is poa user
                            rc.To.Add(poaData.POA_EMAIL);
                            rc.CC.Add(GetManagerEmail(pbck4Dto.CREATED_BY));
                        }
                        else
                        {
                            //creator is excise executive
                            var userData = _userBll.GetUserById(pbck4Dto.CREATED_BY);
                            rc.To.Add(userData.EMAIL);
                            rc.CC.Add(_poaBll.GetById(pbck4Dto.APPROVED_BY_POA).POA_EMAIL);
                            rc.CC.Add(GetManagerEmail(pbck4Dto.APPROVED_BY_POA));
                        }
                    }
                    rc.IsCCExist = true;
                    break;
               case Enums.ActionType.Reject:
                    //send notification to creator
                    var userDetail = _userBll.GetUserById(pbck4Dto.CREATED_BY);
                    var poaData2 = _poaBll.GetById(pbck4Dto.CREATED_BY);

                    if (pbck4Dto.APPROVED_BY_POA != null || poaData2 != null)
                    {
                        if (poaData2 == null)
                        {
                            var poa = _poaBll.GetById(pbck4Dto.APPROVED_BY_POA);
                            rc.To.Add(userDetail.EMAIL);
                            rc.CC.Add(poa.POA_EMAIL);
                            rc.CC.Add(GetManagerEmail(pbck4Dto.APPROVED_BY_POA));
                        }
                        else
                        {
                            rc.To.Add(poaData2.POA_EMAIL);
                            rc.CC.Add(GetManagerEmail(pbck4Dto.CREATED_BY));
                        }
                    }
                    else
                    {
                        rc.To.Add(userDetail.EMAIL);

                        foreach (var poaDto in poaList)
                        {
                            rc.CC.Add(poaDto.POA_EMAIL);
                        }
                    }

                    rc.IsCCExist = true;
                    break;
               case Enums.ActionType.GovApprove:
               case Enums.ActionType.GovPartialApprove:
                   if (pbck4Dto.Status == Enums.DocumentStatus.Completed)
                   {

                       var userData = _userBll.GetUserById(pbck4Dto.CREATED_BY);
                       rc.To.Add(userData.EMAIL);
                       var poaData3 = _poaBll.GetById(pbck4Dto.CREATED_BY);

                       if (poaData3 != null)
                       {
                           //creator is poa user
                           rc.CC.Add(GetManagerEmail(pbck4Dto.CREATED_BY));

                       }
                       else
                       {
                           //creator is excise executive
                           rc.CC.Add(_poaBll.GetById(pbck4Dto.APPROVED_BY_POA).POA_EMAIL);
                           rc.CC.Add(GetManagerEmail(pbck4Dto.APPROVED_BY_POA));
                          

                       }
                       rc.IsCCExist = true;
                   }
                   break;


                   //case Enums.ActionType.GovApprove:
                   //     var poaData3 = _poaBll.GetById(pbck4Dto.CREATED_BY);
                   //     if (poaData3 != null)
                   //     {
                   //         //creator is poa user
                   //         rc.To.Add(GetManagerEmail(pbck4Dto.CREATED_BY));
                   //         rc.CC.Add(poaData3.POA_EMAIL);
                   //     }
                   //     else
                   //     {
                   //         //creator is excise executive
                   //         var userData = _userBll.GetUserById(pbck4Dto.CREATED_BY);
                   //         rc.To.Add(_poaBll.GetById(pbck4Dto.APPROVED_BY_POA).POA_EMAIL);
                   //         rc.To.Add(GetManagerEmail(pbck4Dto.APPROVED_BY_POA));
                   //         rc.CC.Add(userData.EMAIL);
                   //     }
                   //     rc.IsCCExist = true;
                   //     break;


                   //case Enums.ActionType.GovPartialApprove:
                   //     var poaData4 = _poaBll.GetById(pbck4Dto.CREATED_BY);
                   //     if (poaData4 != null)
                   //     {
                   //         //creator is poa user
                   //         rc.To.Add(GetManagerEmail(pbck4Dto.CREATED_BY));
                   //         rc.CC.Add(poaData4.POA_EMAIL);
                   //     }
                   //     else
                   //     {
                   //         //creator is excise executive
                   //         var userData = _userBll.GetUserById(pbck4Dto.CREATED_BY);
                   //         rc.To.Add(_poaBll.GetById(pbck4Dto.APPROVED_BY_POA).POA_EMAIL);
                   //         rc.To.Add(GetManagerEmail(pbck4Dto.APPROVED_BY_POA));
                   //         rc.CC.Add(userData.EMAIL);
                   //     }
                   //     rc.IsCCExist = true;
                   //     break;
                   //case Enums.ActionType.GovReject:
                   //     var poaData5 = _poaBll.GetById(pbck4Dto.CREATED_BY);
                   //     if (poaData5 != null)
                   //     {
                   //         //creator is poa user
                   //         rc.To.Add(GetManagerEmail(pbck4Dto.CREATED_BY));
                   //         rc.CC.Add(poaData5.POA_EMAIL);
                   //     }
                   //     else
                   //     {
                   //         //creator is excise executive
                   //         var userData = _userBll.GetUserById(pbck4Dto.CREATED_BY);
                   //         rc.To.Add(_poaBll.GetById(pbck4Dto.APPROVED_BY_POA).POA_EMAIL);
                   //         rc.To.Add(GetManagerEmail(pbck4Dto.APPROVED_BY_POA));
                   //         rc.CC.Add(userData.EMAIL);
                   //     }
                   //     rc.IsCCExist = true;
                   //     break;

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

           if (dbData.STATUS != Enums.DocumentStatus.Draft && dbData.STATUS != Enums.DocumentStatus.Rejected)
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

           //if (dbData.STATUS != Enums.DocumentStatus.WaitingForApproval &&
           //    dbData.STATUS != Enums.DocumentStatus.WaitingForApprovalManager)
           //    throw new BLLException(ExceptionCodes.BLLExceptions.OperationNotAllowed);
           var isOperationAllow = _workflowBll.AllowApproveAndReject(new WorkflowAllowApproveAndRejectInput()
           {
               CreatedUser = dbData.CREATED_BY,
               CurrentUser = input.UserId,
               DocumentStatus = dbData.STATUS,
               UserRole = input.UserRole,
               NppbkcId = dbData.NPPBKC_ID,
               DocumentNumber = dbData.PBCK4_NUMBER
           });

           if (!isOperationAllow)
               throw new BLLException(ExceptionCodes.BLLExceptions.OperationNotAllowed);


           string oldValue = EnumHelper.GetDescription(dbData.STATUS);
           string newValue = "";

           if (input.UserRole == Enums.UserRole.POA)
           {
               dbData.STATUS = Enums.DocumentStatus.WaitingForApprovalManager;
               dbData.APPROVED_BY_POA = input.UserId;
               dbData.APPROVED_BY_POA_DATE = DateTime.Now;

               //get poa printed name
               string poaPrintedName = "";
               var poaData = _poaBll.GetDetailsById(input.UserId);
               if (poaData != null)
                   poaPrintedName = poaData.PRINTED_NAME;

               dbData.POA_PRINTED_NAME = poaPrintedName;

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
           dbData.STATUS = Enums.DocumentStatus.Rejected;
           newValue = EnumHelper.GetDescription(Enums.DocumentStatus.Rejected);

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

       public void GovApproveDocumentRollback(Pbck4WorkflowDocumentInput input)
       {
           var dbData = _repository.GetByID(input.DocumentId);

           var newValue = dbData.GOV_STATUS.HasValue ? EnumHelper.GetDescription(dbData.GOV_STATUS.Value) : string.Empty;


           dbData.STATUS = Enums.DocumentStatus.WaitingGovApproval;
           dbData.GOV_STATUS = null;
           dbData.BACK1_NO = string.Empty;
           dbData.BACK1_DATE = null;

           dbData.CK3_NO = string.Empty;
           dbData.CK3_DATE = null;
        
           dbData.CK3_OFFICE_VALUE = null;
         
           foreach (var pbck4FileUpload in dbData.PBCK4_DOCUMENT.ToList())
           {
               _repositoryPbck4Documents.Delete(pbck4FileUpload);
           }

           var inputHistory = new GetByActionAndFormNumberInput();
           inputHistory.FormNumber = dbData.PBCK4_NUMBER;
           inputHistory.ActionType = input.ActionType;// Enums.ActionType.GovApprove;

           _workflowHistoryBll.DeleteByActionAndFormNumber(inputHistory);

           
           _changesHistoryBll.DeleteByFormIdAndNewValue(dbData.PBCK4_ID.ToString(), newValue);

           _uow.SaveChanges();

       }

       private bool IsCompletedWorkflow(PBCK4 pbck4)
       {
           if (string.IsNullOrEmpty(pbck4.BACK1_NO))
               return false;

           if (!pbck4.BACK1_DATE.HasValue)
               return false;

           if (string.IsNullOrEmpty(pbck4.CK3_NO))
               return false;

           if (!pbck4.CK3_DATE.HasValue)
               return false;

           if (!pbck4.CK3_OFFICE_VALUE.HasValue)
               return false;
           //back doc = 1
           if (pbck4.PBCK4_DOCUMENT.Count(a => a.DOC_TYPE == 1) == 0)
           {
               //check in database
               if (!IsExistDocumentByPbck4IdAndDocType(pbck4.PBCK4_ID, 1))
                    return false;
           }
           //ck3 doc = 2
           if (pbck4.PBCK4_DOCUMENT.Count(a => a.DOC_TYPE == 2) == 0)
           {
               //check in database
               if (!IsExistDocumentByPbck4IdAndDocType(pbck4.PBCK4_ID, 2))
                    return false;
           }

           return true;
       }

       private bool IsExistDocumentByPbck4IdAndDocType(long pbck4Id, int docType)
       {
           var pbck4Doc = _repositoryPbck4Documents.Get(c => c.PBCK4_ID == pbck4Id && c.DOC_TYPE == docType).ToList();
           return pbck4Doc.Count != 0;
       }

       private void UpdatePbck4ItemApprovedQtyById(long id, decimal? approvedQty)
       {
           var dbData = _repositoryPbck4Items.GetByID(id);
           if (dbData == null) return;
           dbData.APPROVED_QTY = approvedQty;
           _repositoryPbck4Items.Update(dbData);
       }
       private void GovApproveDocument(Pbck4WorkflowDocumentInput input)
       {
           var dbData = _repository.GetByID(input.DocumentId);

           if (dbData == null)
               throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

           if (dbData.STATUS != Enums.DocumentStatus.WaitingGovApproval)
               throw new BLLException(ExceptionCodes.BLLExceptions.OperationNotAllowed);

           //prepare for set changes history
           var origin = Mapper.Map<Pbck4Dto>(dbData);
           
           
           //add to change log
           SetChangesHistory(origin, input, input.UserId);
           
           //Add Changes
           if (dbData.GOV_STATUS != input.GovStatusInput)
               WorkflowStatusGovAddChanges(input, dbData.GOV_STATUS, input.GovStatusInput);
           
           dbData.BACK1_NO = input.AdditionalDocumentData.Back1No;
           dbData.BACK1_DATE = input.AdditionalDocumentData.Back1Date;
           
           dbData.CK3_NO = input.AdditionalDocumentData.Ck3No;
           dbData.CK3_DATE = input.AdditionalDocumentData.Ck3Date;
           
           dbData.CK3_OFFICE_VALUE = ConvertHelper.ConvertToDecimalOrZero(input.AdditionalDocumentData.Ck3OfficeValue);

           dbData.GOV_STATUS = input.GovStatusInput;

           dbData.MODIFIED_DATE = DateTime.Now;
           dbData.MODIFIED_BY = input.UserId;

           var pbckDocument = input.AdditionalDocumentData.Back1FileUploadList.ToList();
           pbckDocument.AddRange(input.AdditionalDocumentData.Ck3FileUploadList);

           //dbData.PBCK4_DOCUMENT = Mapper.Map<List<PBCK4_DOCUMENT>>(pbckDocument);
           InsertOrDeletePbck4Item(pbckDocument);

           //update item updated
           foreach (var pbck4ItemDto in input.UploadItemDto)
           {
               UpdatePbck4ItemApprovedQtyById(pbck4ItemDto.PBCK4_ITEM_ID, pbck4ItemDto.APPROVED_QTY);
           }

           input.DocumentNumber = dbData.PBCK4_NUMBER;

           AddWorkflowHistory(input);

           if (IsCompletedWorkflow(dbData))
           {
               WorkflowStatusAddChanges(input, dbData.STATUS, Enums.DocumentStatus.Completed);

               dbData.STATUS = Enums.DocumentStatus.Completed;
               input.ActionType = Enums.ActionType.Completed;

               AddWorkflowHistory(input);
           }
          
           //set back for email workflow
           if (input.GovStatusInput == Enums.DocumentStatusGov.FullApproved)
               input.ActionType = Enums.ActionType.GovApprove;
           else if (input.GovStatusInput == Enums.DocumentStatusGov.PartialApproved)
               input.ActionType = Enums.ActionType.GovPartialApprove;

          
           
       }

       public void UpdateUploadedFileCompleted(List<PBCK4_DOCUMENTDto> input)
       {
           InsertOrDeletePbck4Item(input);
           _uow.SaveChanges();
       }

       private void InsertOrDeletePbck4Item(List<PBCK4_DOCUMENTDto> input)
       {
           foreach (var pbck4DocumentDto in input)
           {
               if (pbck4DocumentDto.IsDeleted)
               {
                   var pbck4Doc = _repositoryPbck4Documents.GetByID(pbck4DocumentDto.PBCK4_DOCUMENT_ID);
                   if (pbck4Doc != null)
                       _repositoryPbck4Documents.Delete(pbck4Doc);
               }
               else
               {
                   if (pbck4DocumentDto.PBCK4_DOCUMENT_ID == 0)
                        _repositoryPbck4Documents.Insert(Mapper.Map<PBCK4_DOCUMENT>(pbck4DocumentDto));
               }
           }
       }

       //private void GovPartialApproveDocument(Pbck4WorkflowDocumentInput input)
       //{
       //    var dbData = _repository.GetByID(input.DocumentId);

       //    if (dbData == null)
       //        throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

       //    if (dbData.STATUS != Enums.DocumentStatus.WaitingGovApproval)
       //        throw new BLLException(ExceptionCodes.BLLExceptions.OperationNotAllowed);

       //    //prepare for set changes history
       //    var origin = Mapper.Map<Pbck4Dto>(dbData);


       //    //add to change log
       //    SetChangesHistory(origin, input, input.UserId);

       //    //Add Changes
       //    if (dbData.GOV_STATUS != Enums.DocumentStatusGov.PartialApproved)
       //        WorkflowStatusGovAddChanges(input, dbData.GOV_STATUS, Enums.DocumentStatusGov.PartialApproved);

       //    dbData.BACK1_NO = input.AdditionalDocumentData.Back1No;
       //    dbData.BACK1_DATE = input.AdditionalDocumentData.Back1Date;

       //    dbData.CK3_NO = input.AdditionalDocumentData.Ck3No;
       //    dbData.CK3_DATE = input.AdditionalDocumentData.Ck3Date;

       //    dbData.CK3_OFFICE_VALUE = ConvertHelper.ConvertToDecimalOrZero(input.AdditionalDocumentData.Ck3OfficeValue);

       //    dbData.GOV_STATUS = Enums.DocumentStatusGov.PartialApproved;

       //    dbData.MODIFIED_DATE = DateTime.Now;
       //    dbData.MODIFIED_BY = input.UserId;

       //    var pbckDocument = input.AdditionalDocumentData.Back1FileUploadList.ToList();
       //    pbckDocument.AddRange(input.AdditionalDocumentData.Ck3FileUploadList);

       //    dbData.PBCK4_DOCUMENT = Mapper.Map<List<PBCK4_DOCUMENT>>(pbckDocument);

       //    //update item updated
       //    foreach (var pbck4ItemDto in input.UploadItemDto)
       //    {
       //        UpdatePbck4ItemApprovedQtyById(pbck4ItemDto.PBCK4_ITEM_ID, pbck4ItemDto.APPROVED_QTY);
       //    }

       //    if (IsCompletedWorkflow(dbData))
       //    {
       //        WorkflowStatusAddChanges(input, dbData.STATUS, Enums.DocumentStatus.Completed);

       //        dbData.STATUS = Enums.DocumentStatus.Completed;

       //        AddWorkflowHistory(input);
       //    }


       //    input.DocumentNumber = dbData.PBCK4_NUMBER;

       //    //var dbData = _repository.GetByID(input.DocumentId);

       //    //if (dbData == null)
       //    //    throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

       //    //if (dbData.STATUS != Enums.DocumentStatus.WaitingGovApproval)
       //    //    throw new BLLException(ExceptionCodes.BLLExceptions.OperationNotAllowed);

       //    ////Add Changes
       //    //WorkflowStatusAddChanges(input, dbData.STATUS, Enums.DocumentStatus.Completed);
       //    //WorkflowStatusGovAddChanges(input, dbData.GOV_STATUS, Enums.DocumentStatusGov.PartialApproved);

       //    //dbData.STATUS = Enums.DocumentStatus.Completed;

       //    //dbData.BACK1_NO = input.AdditionalDocumentData.Back1No;
       //    //dbData.BACK1_DATE = input.AdditionalDocumentData.Back1Date;

       //    //dbData.CK3_NO = input.AdditionalDocumentData.Ck3No;
       //    //dbData.CK3_DATE = input.AdditionalDocumentData.Ck3Date;
           
       //    //dbData.CK3_OFFICE_VALUE = ConvertHelper.ConvertToDecimalOrZero(input.AdditionalDocumentData.Ck3OfficeValue);

       //    //dbData.GOV_STATUS = Enums.DocumentStatusGov.PartialApproved;

       //    //dbData.MODIFIED_DATE = DateTime.Now;
       //    //dbData.MODIFIED_BY = input.UserId;

       //    //var pbckDocument = input.AdditionalDocumentData.Back1FileUploadList.ToList();
       //    //pbckDocument.AddRange(input.AdditionalDocumentData.Ck3FileUploadList);

       //    //dbData.PBCK4_DOCUMENT = Mapper.Map<List<PBCK4_DOCUMENT>>(pbckDocument);

       //    //input.DocumentNumber = dbData.PBCK4_NUMBER;

       //    //AddWorkflowHistory(input);
       //}

       private void GovRejectedDocument(Pbck4WorkflowDocumentInput input)
       {
           var dbData = _repository.GetByID(input.DocumentId);

           if (dbData == null)
               throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

           if (dbData.STATUS != Enums.DocumentStatus.WaitingGovApproval)
               throw new BLLException(ExceptionCodes.BLLExceptions.OperationNotAllowed);

           //Add Changes
           WorkflowStatusAddChanges(input, dbData.STATUS, Enums.DocumentStatus.Rejected);
           WorkflowStatusGovAddChanges(input, dbData.GOV_STATUS, Enums.DocumentStatusGov.Rejected);

           dbData.STATUS = Enums.DocumentStatus.Rejected;
           dbData.GOV_STATUS = Enums.DocumentStatusGov.Rejected;

           dbData.MODIFIED_DATE = DateTime.Now;
           dbData.MODIFIED_BY = input.UserId;

           input.DocumentNumber = dbData.PBCK4_NUMBER;

           AddWorkflowHistory(input);

       }

       private string DateReportDisplayString(DateTime dt, bool isMonthYear)
       {
           var monthPeriodFrom = _monthBll.GetMonth(dt.Month);
           if (isMonthYear) return monthPeriodFrom.MONTH_NAME_IND + " " + dt.ToString("yyyy");
           return dt.ToString("dd") + " " + monthPeriodFrom.MONTH_NAME_IND +
                                  " " + dt.ToString("yyyy");
       }

       private Pbck4ReportDto SetPbck4Items(Pbck4ReportDto pbck4ReportDto, PBCK4 dtData)
       {
           var listGroupBy = dtData.PBCK4_ITEM.GroupBy(a => new {a.SERIES_CODE, a.BRAND_CONTENT, a.HJE, a.TARIFF, a.NO_PENGAWAS})
               .Select(x => new Pbck4ItemReportDto
               {
                   Seri = x.Key.SERIES_CODE,
                   Hje = x.Key.HJE.HasValue ? x.Key.HJE.Value:0,
                   Content = ConvertHelper.ConvertToDecimalOrZero(x.Key.BRAND_CONTENT),
                   Tariff = x.Key.TARIFF.HasValue ? x.Key.TARIFF.Value : 0,
                   NoPengawas = x.Key.NO_PENGAWAS,
                   ReqQty = x.Sum(c => c.REQUESTED_QTY.HasValue ? c.REQUESTED_QTY.Value : 0),
                   TotalHje = x.Sum(c => c.TOTAL_HJE.HasValue ? c.TOTAL_HJE.Value : 0),
                   TotalCukai = x.Sum(c => c.TOTAL_STAMPS.HasValue ? c.TOTAL_STAMPS.Value : 0)
               }).ToList();

           foreach (var pbck4ItemReportDto in listGroupBy)
           {
               pbck4ReportDto.ListPbck4Items.Add(pbck4ItemReportDto);
           }

           return pbck4ReportDto;
       }
      
       public Pbck4ReportDto GetPbck4ReportDataById(int id)
        {
            var dtData = _repository.Get(c => c.PBCK4_ID == id, null, includeTables).FirstOrDefault();
            if (dtData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

           var nppbkcData = _nppbkcBll.GetById(dtData.NPPBKC_ID);

           var plantData = _plantBll.GetT001WById(dtData.PLANT_ID);

            var result = new Pbck4ReportDto();
             result.ReportDetails.Pbck4Number = dtData.PBCK4_NUMBER;
             result.ReportDetails.Pbck4Lampiran = "";
             result.ReportDetails.TextTo = nppbkcData != null ? nppbkcData.TEXT_TO : string.Empty;
             result.ReportDetails.CityTo = nppbkcData != null ? nppbkcData.CITY : string.Empty;
           result.ReportDetails.PoaName = dtData.POA_PRINTED_NAME;// dtData.POA != null ? dtData.POA.PRINTED_NAME : string.Empty;
             result.ReportDetails.PoaTitle = dtData.POA != null ? dtData.POA.TITLE : string.Empty;
             result.ReportDetails.CompanyName = dtData.COMPANY_NAME;
             result.ReportDetails.CompanyAddress = plantData!=null ? plantData.CompanyAddress : string.Empty;
             result.ReportDetails.NppbkcId = dtData.NPPBKC_ID;

           string nppbkcDate = "";
           if (nppbkcData != null)
           {
               if (nppbkcData.START_DATE.HasValue)
                   nppbkcDate = DateReportDisplayString(nppbkcData.START_DATE.Value, false);
           }
           result.ReportDetails.NppbkcDate = nppbkcDate;

           result.ReportDetails.PlantCity = result.ReportDetails.CityTo;
           result.ReportDetails.PrintDate = DateReportDisplayString(DateTime.Now, false);
           result.ReportDetails.RegionOffice = nppbkcData != null ? nppbkcData.REGION_DGCE : string.Empty;
           int i = 0;

           result = SetPbck4Items(result, dtData);

           foreach (var pbck4Item in dtData.PBCK4_ITEM)
           {
              
                var pbck4Matrikck1 = new Pbck4IMatrikCk1ReportDto();
               pbck4Matrikck1.Number = i + 1;
               pbck4Matrikck1.SeriesCode = pbck4Item.SERIES_CODE;
               pbck4Matrikck1.Hje = pbck4Item.HJE.HasValue ? pbck4Item.HJE.Value : 0;
               pbck4Matrikck1.JenisHt = pbck4Item.PRODUCT_ALIAS;
               pbck4Matrikck1.Content = ConvertHelper.ConvertToDecimalOrZero(pbck4Item.BRAND_CONTENT);
               pbck4Matrikck1.Ck1RequestedQty = pbck4Item.REQUESTED_QTY.HasValue ? pbck4Item.REQUESTED_QTY.Value : 0;

               pbck4Matrikck1.BrandName = pbck4Item.BRAND_NAME;
               if (pbck4Item.CK1 == null)
               {
                   pbck4Matrikck1.Ck1No = "";
                   pbck4Matrikck1.Ck1Date = "";
                   pbck4Matrikck1.Ck1OrderQty = 0;
                   //pbck4Matrikck1.Ck1RequestedQty = 0;
               }
               else
               {
                   pbck4Matrikck1.Ck1No = pbck4Item.CK1.CK1_NUMBER;
                   pbck4Matrikck1.Ck1Date = DateReportDisplayString(pbck4Item.CK1.CK1_DATE, false);
                   pbck4Matrikck1.Ck1OrderQty = 0;//todo ask
                   //pbck4Matrikck1.Ck1RequestedQty = 0;//todo ask
               }

               pbck4Matrikck1.Tariff = pbck4Item.TARIFF.HasValue ? pbck4Item.TARIFF.Value : 0;
               pbck4Matrikck1.TotalHje = pbck4Item.TOTAL_HJE.HasValue ? pbck4Item.TOTAL_HJE.Value : 0;
               pbck4Matrikck1.TotalCukai = pbck4Item.TOTAL_STAMPS.HasValue ? pbck4Item.TOTAL_STAMPS.Value : 0;
               pbck4Matrikck1.NoPengawas = pbck4Item.REMARKS;// "Tidak Dipakai";
              
               result.ListPbck4MatrikCk1.Add(pbck4Matrikck1);
               i = i + 1;


           }

           //set header footer data by CompanyCode and FormTypeId
           var headerFooterData = _headerFooterBll.GetByComanyAndFormType(new HeaderFooterGetByComanyAndFormTypeInput()
           {
               FormTypeId = Enums.FormType.PBCK4,
               CompanyCode = dtData.COMPANY_ID
           });

           result.ReportDetails.HeaderImage = string.Empty;

           if (headerFooterData.IS_HEADER_SET.HasValue && headerFooterData.IS_HEADER_SET.Value)
           {
               result.ReportDetails.HeaderImage = headerFooterData.HEADER_IMAGE_PATH;
           }

           return result;
        }

       public List<Pbck4SummaryReportDto> GetSummaryReportsByParam(Pbck4GetSummaryReportByParamInput input)
       {

           Expression<Func<PBCK4, bool>> queryFilter = PredicateHelper.True<PBCK4>();

           if (!string.IsNullOrEmpty(input.Pbck4No))
           {
               queryFilter = queryFilter.And(c => c.PBCK4_NUMBER.Contains(input.Pbck4No));
           }

           if (input.YearFrom.HasValue)
               queryFilter =
                   queryFilter.And(c => c.REPORTED_ON.HasValue && c.REPORTED_ON.Value.Year >= input.YearFrom.Value);
           if (input.YearTo.HasValue)
               queryFilter =
                   queryFilter.And(c => c.REPORTED_ON.HasValue && c.REPORTED_ON.Value.Year <= input.YearTo.Value);

           if (!string.IsNullOrEmpty(input.PlantId))
           {
               queryFilter = queryFilter.And(c => c.PLANT_ID.Contains(input.PlantId));
           }

           
           queryFilter = queryFilter.And(c => c.STATUS == Enums.DocumentStatus.Completed);


           var rc = _repository.Get(queryFilter, null, includeTables).ToList();
           if (rc == null)
           {
               throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);
           }

           //var mapResult = Mapper.Map<List<Pbck4Dto>>(rc.ToList());

           //return mapResult;

           return  SetDataSummaryReport(rc);

           //return mapResult;

       }

       private List<Pbck4SummaryReportDto> SetDataSummaryReport(List<PBCK4> listPbck4)
       {
           var result = new List<Pbck4SummaryReportDto>();

           foreach (var dtData in listPbck4)
           {



               foreach (var pbck4Item in dtData.PBCK4_ITEM)
               {
                   var summaryDto = new Pbck4SummaryReportDto();

                   summaryDto.Pbck4No = dtData.PBCK4_NUMBER;
                   summaryDto.Pbck4Date = dtData.REPORTED_ON.HasValue
                       ? dtData.REPORTED_ON.Value.ToString("dd MMM yyyy")
                       : string.Empty;
                   //summaryDto.CeOffice = dtData.NPPBKC_ID;
                   var nppbkcData = _nppbkcBll.GetById(dtData.NPPBKC_ID);
                   summaryDto.CeOffice = nppbkcData != null ? nppbkcData.KPPBC_ID : string.Empty;
                   summaryDto.Brand = pbck4Item.BRAND_NAME;
                   summaryDto.Content = pbck4Item.BRAND_CONTENT;
                   summaryDto.Hje = pbck4Item.HJE.HasValue ? pbck4Item.HJE.Value.ToString("f2") : string.Empty;
                   summaryDto.Tariff = pbck4Item.TARIFF.HasValue ? pbck4Item.TARIFF.Value.ToString("f2") : string.Empty;
                   summaryDto.ProductType = pbck4Item.PRODUCT_ALIAS;


                   summaryDto.SeriesCode = pbck4Item.SERIES_CODE;
                   summaryDto.RequestedQty = ConvertHelper.ConvertDecimalToString(pbck4Item.REQUESTED_QTY);

                   summaryDto.ExciseValue =
                       (ConvertHelper.ConvertToDecimalOrZero(summaryDto.Content)*
                        ConvertHelper.ConvertToDecimalOrZero(summaryDto.Tariff)*
                        ConvertHelper.ConvertToDecimalOrZero(summaryDto.RequestedQty)).ToString();

                   summaryDto.Remarks = pbck4Item.REMARKS;

                   summaryDto.Back1Date = ConvertHelper.ConvertDateToStringddMMMyyyy(dtData.BACK1_DATE);
                   summaryDto.Back1Number = dtData.BACK1_NO;
                   summaryDto.Ck3Date = ConvertHelper.ConvertDateToStringddMMMyyyy(dtData.CK3_DATE);
                   summaryDto.Ck3Number = dtData.CK3_NO;
                   summaryDto.Ck3Value = ConvertHelper.ConvertDecimalToString(dtData.CK3_OFFICE_VALUE);

                   var dbBrand = _brandRegistrationServices.GetByPlantIdAndFaCode(dtData.PLANT_ID, pbck4Item.FA_CODE);
                   if (dbBrand == null)
                   {
                       summaryDto.FiscalYear = "";
                       summaryDto.PrintingCost = "0";
                   }
                   else
                   {
                       summaryDto.FiscalYear = dbBrand.START_DATE.HasValue
                           ? dbBrand.START_DATE.Value.ToString("yyyy")
                           : string.Empty;

                       summaryDto.PrintingCost =
                           ConvertHelper.ConvertDecimalToString(
                               ConvertHelper.ConvertToDecimalOrZero(
                                   ConvertHelper.ConvertDecimalToString(dbBrand.PRINTING_PRICE))*
                               ConvertHelper.ConvertToDecimalOrZero(summaryDto.RequestedQty));
                   }

                   //todo ask from where the value is
                   summaryDto.CompensatedCk1Date = "";
                   summaryDto.CompensatedCk1Number = "";
                   summaryDto.PaymentDate = "";

                   summaryDto.Status = EnumHelper.GetDescription(dtData.STATUS);
                   if (dtData.REPORTED_ON.HasValue)
                       summaryDto.ReportedOn = Convert.ToInt32(dtData.REPORTED_ON.Value.ToString("yyyy"));


                   //summaryDto.ReportedOn = dtData.REPORTED_ON.HasValue
                   //    ? dtData.REPORTED_ON.Value.ToString("yyyy")
                   //    : string.Empty;

                   summaryDto.PlantId = dtData.PLANT_ID;
                   summaryDto.PlantDescription = dtData.PLANT_NAME;

                   result.Add(summaryDto);
               }

           }

           return result;
       }

        public Pbck4XmlDto GetPbck4ForXmlById(int id)
       {
          
           var dtData = _repository.Get(c => c.PBCK4_ID == id, null, includeTables).FirstOrDefault();
           if (dtData == null)
               throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

          
           var dataXmlDto = new Pbck4XmlDto();
           dataXmlDto.PbckNo = dtData.PBCK4_NUMBER;
           dataXmlDto.NppbckId = dtData.NPPBKC_ID;
           dataXmlDto.CompNo = dtData.CK3_NO;
           dataXmlDto.CompType = "CK-3";
           dataXmlDto.CompnDate = dtData.CK3_DATE;
         
           dataXmlDto.CompnValue = dtData.CK3_OFFICE_VALUE.HasValue
               ? dtData.CK3_OFFICE_VALUE.Value.ToString()
               : string.Empty;
           dataXmlDto.DeleteFlag = "";

            dataXmlDto.Status = dtData.STATUS;

           return dataXmlDto;

       }

        public List<GetListBrandByPlantOutput> GetListBrandByPlant(string plantId)
       {
           var dbBrand = _brandRegistrationServices.GetBrandByPlant(plantId);

            return Mapper.Map<List<GetListBrandByPlantOutput>>(dbBrand);
       }

        public List<GetListCk1ByNppbkcOutput> GetListCk1ByNppbkc(string nppbkcId)
        {
            var dbCk1 = _ck1Services.GetCk1ByNppbkc(nppbkcId);

            return Mapper.Map<List<GetListCk1ByNppbkcOutput>>(dbCk1);
        }

        public GetBrandItemsOutput GetBrandItemsStickerCodeByPlantAndFaCode(string plant, string faCode)
       {
           var dbBrand = _brandRegistrationServices.GetByPlantIdAndFaCode(plant, faCode);
            return Mapper.Map<GetBrandItemsOutput>(dbBrand);
       }

        public string GetCk1DateByCk1Id(long ck1Id)
        {
            var dbCK1 = _ck1Services.GetCk1ById(ck1Id);
            return dbCK1 == null ? string.Empty : dbCK1.CK1_DATE.ToString("dd MMM yyyy");
        }

        public decimal GetBlockedStockByPlantAndFaCode(string plant, string faCode)
        {
            var dbBlock = _blockStockBll.GetBlockStockByPlantAndMaterialId(plant, faCode);
            return dbBlock.Count == 0
                ? 0
                : dbBlock.Sum(blockStockDto => blockStockDto.BLOCKED.HasValue ? blockStockDto.BLOCKED.Value : 0);
        }

        public BlockedStockQuotaOutput GetBlockedStockQuota(string plant, string faCode)
        {
            var dbBlock = _blockStockBll.GetBlockStockByPlantAndMaterialId(plant, faCode);
            decimal blockStock = dbBlock.Count == 0
                ? 0
                : dbBlock.Sum(blockStockDto => blockStockDto.BLOCKED.HasValue ? blockStockDto.BLOCKED.Value : 0);
            
            //get from pbck4 

            var dbPbck4 = _repository.Get(c => c.PLANT_ID == plant &&
                        (c.STATUS != Enums.DocumentStatus.Cancelled), null, "PBCK4_ITEM");

            decimal blockStockUsed =
                dbPbck4.Sum(
                    pbck4 =>
                        pbck4.PBCK4_ITEM.Where(pbck4Item => pbck4Item.FA_CODE == faCode)
                            .Sum(pbck4Item => pbck4Item.REQUESTED_QTY.HasValue ? pbck4Item.REQUESTED_QTY.Value : 0));

            var result = new BlockedStockQuotaOutput();
            result.BlockedStock = blockStock.ToString();
            result.BlockedStockUsed = blockStockUsed.ToString();
            result.BlockedStockRemaining = (blockStock - blockStockUsed).ToString();

            return result;

        }

       public decimal GetCurrentReqQtyByPbck4IdAndFaCode(int pbck4Id, string faCode)
       {
           decimal result = 0;
           var dbPbck = _repository.Get(c => c.PBCK4_ID == pbck4Id, null, includeTables).FirstOrDefault();

           if (dbPbck != null)
           {
               foreach (var pbck4Item in dbPbck.PBCK4_ITEM)
               {
                   if (pbck4Item.FA_CODE == faCode)
                   {
                       result = pbck4Item.REQUESTED_QTY.HasValue ? pbck4Item.REQUESTED_QTY.Value : 0;
                       return result;
                   }
               }
           }

           return result;

       }

       public List<GetListCk1ByNppbkcOutput> GetListCk1ByPlantAndFaCode(GetListCk1ByPlantAndFaCodeInput input)
       {
           var dbCk1 = _ck1Services.GetCk1ByNppbkc(input.NppbkcId);

           var result = new List<GetListCk1ByNppbkcOutput>();

           foreach (var ck1 in dbCk1)
           {
               foreach (var ck1Item in ck1.CK1_ITEM)
               {
                   if (ck1Item.WERKS == input.PlantId && ck1Item.FA_CODE == input.FaCode)
                   {
                       var found = new GetListCk1ByNppbkcOutput();
                       found.Ck1Id = ck1.CK1_ID.ToString();
                       found.Ck1No = ck1.CK1_NUMBER;
                       found.Ck1Date = ConvertHelper.ConvertDateToStringddMMMyyyy(ck1.CK1_DATE);

                       result.Add(found);
                   }
               }

           }

           return result;
       }

       public string GetListPoaByNppbkcId(string nppbkcId)
       {
           var dbPoa = _poaBll.GetPoaByNppbkcId(nppbkcId);
           var result = "";

           foreach (var poaDto in dbPoa)
           {
               result += poaDto.PRINTED_NAME + ",";
           }

           if (result.Length > 0)
           {
               result = result.Substring(0, result.Length - 1);
           }

         

           return result;
       }
   }
}
