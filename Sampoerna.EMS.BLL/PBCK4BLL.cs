using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.BusinessObject.Outputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core.Exceptions;
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

       private IMonthBLL _monthBll;
       private IDocumentSequenceNumberBLL _docSeqNumBll;
       private IWorkflowHistoryBLL _workflowHistoryBll;
       private IChangesHistoryBLL _changesHistoryBll;
       private IPrintHistoryBLL _printHistoryBll;
       private IBrandRegistrationBLL _brandRegistrationBll;
       private ICK1BLL _ck1Bll;

       private string includeTables = "";// "CK5_MATERIAL, PBCK1, UOM, USER, USER1, CK5_FILE_UPLOAD";

       public PBCK4BLL(IUnitOfWork uow, ILogger logger)
       {
           _logger = logger;
           _uow = uow;

           _repository = _uow.GetGenericRepository<PBCK4>();

           _monthBll = new MonthBLL(_uow, _logger);
           _docSeqNumBll = new DocumentSequenceNumberBLL(_uow, _logger);
           _workflowHistoryBll = new WorkflowHistoryBLL(_uow,_logger);
           _changesHistoryBll = new ChangesHistoryBLL(_uow,_logger);
           _printHistoryBll = new PrintHistoryBLL(_uow,_logger);
           _brandRegistrationBll = new BrandRegistrationBLL(_uow,_logger);
           _ck1Bll = new CK1BLL(_uow,_logger);
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

           ////details
          // output.Pbck4ItemsDto = Mapper.Map<List<Pbck4ItemDto>>(dtData.PBCK4_ITEM);

           //change history data
           output.ListChangesHistorys = _changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.PBCK4, output.Pbck4Dto.PBCK4_ID.ToString());

           //workflow history
           var input = new GetByFormNumberInput();
           input.FormNumber = dtData.PBCK4_NUMBER;
           input.DocumentStatus = dtData.STATUS;
           
           
           output.ListWorkflowHistorys = _workflowHistoryBll.GetByFormNumber(input);


           output.ListPrintHistorys = _printHistoryBll.GetByFormTypeAndFormId(Enums.FormType.PBCK4, dtData.PBCK4_ID);
           return output;
       }



       public Pbck4Dto SavePbck4(Pbck4SaveInput input)
       {
           //workflowhistory
           var inputWorkflowHistory = new CK5WorkflowHistoryInput();

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


               ////delete child first
               //foreach (var ck5Material in dbData.CK5_MATERIAL.ToList())
               //{
               //    _repositoryCK5Material.Delete(ck5Material);
               //}

               inputWorkflowHistory.ActionType = Enums.ActionType.Modified;

               ////insert new data
               //foreach (var ck5Item in input.Ck5Material)
               //{
               //    var ck5Material = Mapper.Map<CK5_MATERIAL>(ck5Item);
               //    ck5Material.PLANT_ID = dbData.SOURCE_PLANT_ID;
               //    dbData.CK5_MATERIAL.Add(ck5Material);
               //}

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

               //foreach (var ck5Item in input.Ck5Material)
               //{

               //    var ck5Material = Mapper.Map<CK5_MATERIAL>(ck5Item);
               //    ck5Material.PLANT_ID = dbData.SOURCE_PLANT_ID;
               //    dbData.CK5_MATERIAL.Add(ck5Material);
               //}

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

       private void AddWorkflowHistory(CK5WorkflowHistoryInput input)
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

       private List<Pbck4ItemsOutput> ValidatePbck4Items(List<Pbck4ItemsInput> inputs)
       {
           var messageList = new List<string>();
           var outputList = new List<Pbck4ItemsOutput>();

           foreach (var pbck4ItemInput in inputs)
           {
               messageList.Clear();
               
               var output = Mapper.Map<Pbck4ItemsOutput>(pbck4ItemInput);

               var dbBrand = _brandRegistrationBll.GetByPlantIdAndFaCode(pbck4ItemInput.Plant, pbck4ItemInput.FaCode);
               if (dbBrand == null)
                   messageList.Add("FA Code Not Exist");

               var dbCk1 = _ck1Bll.GetCk1ByCk1Number(pbck4ItemInput.Ck1No);
               if (dbCk1 == null)
                   messageList.Add("CK-1 Number Not Exist");

               if (!ConvertHelper.IsNumeric(pbck4ItemInput.ReqQty))
                   messageList.Add("Req Qty not valid");

               if (!ConvertHelper.IsNumeric(pbck4ItemInput.ApprovedQty))
                   messageList.Add("Approved Qty not valid");
               

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
           var dbBrand = _brandRegistrationBll.GetByPlantIdAndFaCode(input.Plant, input.FaCode);
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

           }

           return outputList;
       }
    }
}
