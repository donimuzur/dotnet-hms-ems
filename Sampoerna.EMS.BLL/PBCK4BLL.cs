using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
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

       private string includeTables = "";// "CK5_MATERIAL, PBCK1, UOM, USER, USER1, CK5_FILE_UPLOAD";

       public PBCK4BLL(IUnitOfWork uow, ILogger logger)
       {
           _logger = logger;
           _uow = uow;

           _repository = _uow.GetGenericRepository<PBCK4>();

           _monthBll = new MonthBLL(_uow, _logger);
           _docSeqNumBll = new DocumentSequenceNumberBLL(_uow, _logger);
           _workflowHistoryBll = new WorkflowHistoryBLL(_uow,_logger);

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
                       c => c.CREATED_DATE == input.ReportedOn.Value);
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

               inputWorkflowHistory.DocumentId = dbData.PBCK4_ID;
               inputWorkflowHistory.DocumentNumber = dbData.PBCK4_NUMBER;
               inputWorkflowHistory.UserId = input.UserId;
               inputWorkflowHistory.UserRole = input.UserRole;


               AddWorkflowHistory(inputWorkflowHistory);


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

               //input.Pbck4Dto.PBCK1_NUMBER = _docSeqNumBll.GenerateNumberByFormType(Enums.FormType.CK5);
               //if (!input.Ck5Dto.SUBMISSION_DATE.HasValue)
               //{
               //    input.Ck5Dto.SUBMISSION_DATE = DateTime.Now;
               //}

               //input.Ck5Dto.STATUS_ID = Enums.DocumentStatus.Draft;
               //input.Ck5Dto.CREATED_DATE = DateTime.Now;
               //input.Ck5Dto.CREATED_BY = input.UserId;
           }

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
           //todo check the new value
           var changesData = new Dictionary<string, bool>();

           changesData.Add("KPPBC_CITY", origin.KPPBC_CITY == data.KPPBC_CITY);
           changesData.Add("REGISTRATION_NUMBER", origin.REGISTRATION_NUMBER == data.REGISTRATION_NUMBER);

           changesData.Add("EX_GOODS_TYPE", origin.EX_GOODS_TYPE == data.EX_GOODS_TYPE);

           changesData.Add("EX_SETTLEMENT_ID", origin.EX_SETTLEMENT_ID == data.EX_SETTLEMENT_ID);
           changesData.Add("EX_STATUS_ID", origin.EX_STATUS_ID == data.EX_STATUS_ID);
           changesData.Add("REQUEST_TYPE_ID", origin.REQUEST_TYPE_ID == data.REQUEST_TYPE_ID);
           changesData.Add("SOURCE_PLANT_ID", origin.SOURCE_PLANT_ID == (data.SOURCE_PLANT_ID));
           changesData.Add("DEST_PLANT_ID", origin.DEST_PLANT_ID == (data.DEST_PLANT_ID));

           changesData.Add("INVOICE_NUMBER", origin.INVOICE_NUMBER == data.INVOICE_NUMBER);
           changesData.Add("INVOICE_DATE", origin.INVOICE_DATE == (data.INVOICE_DATE));

           changesData.Add("PBCK1_DECREE_ID", origin.PBCK1_DECREE_ID == (data.PBCK1_DECREE_ID));
           changesData.Add("CARRIAGE_METHOD_ID", origin.CARRIAGE_METHOD_ID == (data.CARRIAGE_METHOD_ID));

           changesData.Add("GRAND_TOTAL_EX", origin.GRAND_TOTAL_EX == (data.GRAND_TOTAL_EX));

           changesData.Add("PACKAGE_UOM_ID", origin.PACKAGE_UOM_ID == data.PACKAGE_UOM_ID);

           changesData.Add("DESTINATION_COUNTRY", origin.DEST_COUNTRY_NAME == data.DEST_COUNTRY_NAME);

           changesData.Add("SUBMISSION_DATE", origin.SUBMISSION_DATE == data.SUBMISSION_DATE);

           foreach (var listChange in changesData)
           {
               if (listChange.Value) continue;
               var changes = new CHANGES_HISTORY();
               changes.FORM_TYPE_ID = Enums.MenuList.CK5;
               changes.FORM_ID = origin.CK5_ID.ToString();
               changes.FIELD_NAME = listChange.Key;
               changes.MODIFIED_BY = userId;
               changes.MODIFIED_DATE = DateTime.Now;
               switch (listChange.Key)
               {
                   case "KPPBC_CITY":
                       changes.OLD_VALUE = origin.KPPBC_CITY;
                       changes.NEW_VALUE = data.KPPBC_CITY;
                       break;
                   case "REGISTRATION_NUMBER":
                       changes.OLD_VALUE = origin.REGISTRATION_NUMBER;
                       changes.NEW_VALUE = data.REGISTRATION_NUMBER;
                       break;
                   case "EX_GOODS_TYPE":
                       changes.OLD_VALUE = EnumHelper.GetDescription(origin.EX_GOODS_TYPE);
                       changes.NEW_VALUE = EnumHelper.GetDescription(data.EX_GOODS_TYPE);
                       break;
                   case "EX_SETTLEMENT_ID":
                       changes.OLD_VALUE = EnumHelper.GetDescription(origin.EX_SETTLEMENT_ID);
                       changes.NEW_VALUE = EnumHelper.GetDescription(data.EX_SETTLEMENT_ID);
                       break;
                   case "EX_STATUS_ID":
                       changes.OLD_VALUE = EnumHelper.GetDescription(origin.EX_STATUS_ID);
                       changes.NEW_VALUE = EnumHelper.GetDescription(data.EX_STATUS_ID);
                       break;
                   case "REQUEST_TYPE_ID":
                       changes.OLD_VALUE = EnumHelper.GetDescription(origin.REQUEST_TYPE_ID);
                       changes.NEW_VALUE = EnumHelper.GetDescription(data.REQUEST_TYPE_ID);
                       break;
                   case "SOURCE_PLANT_ID":
                       changes.OLD_VALUE = origin.SOURCE_PLANT_ID;
                       changes.NEW_VALUE = data.SOURCE_PLANT_ID;
                       break;
                   case "DEST_PLANT_ID":
                       changes.OLD_VALUE = origin.DEST_PLANT_ID;
                       changes.NEW_VALUE = data.DEST_PLANT_ID;
                       break;
                   case "INVOICE_NUMBER":
                       changes.OLD_VALUE = origin.INVOICE_NUMBER;
                       changes.NEW_VALUE = data.INVOICE_NUMBER;
                       break;
                   case "INVOICE_DATE":
                       changes.OLD_VALUE = origin.INVOICE_DATE != null ? origin.INVOICE_DATE.Value.ToString("dd MMM yyyy") : string.Empty;
                       changes.NEW_VALUE = data.INVOICE_DATE != null ? data.INVOICE_DATE.Value.ToString("dd MMM yyyy") : string.Empty;
                       break;
                   case "PBCK1_DECREE_ID":

                       changes.OLD_VALUE = origin.PbckNumber;
                       changes.NEW_VALUE = data.PbckNumber;
                       break;

                   case "CARRIAGE_METHOD_ID":
                       changes.OLD_VALUE = origin.CARRIAGE_METHOD_ID.HasValue ? EnumHelper.GetDescription(origin.CARRIAGE_METHOD_ID) : "NULL";
                       changes.NEW_VALUE = data.CARRIAGE_METHOD_ID.HasValue ? EnumHelper.GetDescription(data.CARRIAGE_METHOD_ID) : "NULL";
                       break;

                   case "GRAND_TOTAL_EX":
                       changes.OLD_VALUE = origin.GRAND_TOTAL_EX.ToString();
                       changes.NEW_VALUE = data.GRAND_TOTAL_EX.ToString();
                       break;

                   case "PACKAGE_UOM_ID":
                       changes.OLD_VALUE = origin.PackageUomName;
                       changes.NEW_VALUE = data.PackageUomName;
                       break;
                   case "DESTINATION_COUNTRY":
                       changes.OLD_VALUE = origin.DEST_COUNTRY_NAME;
                       changes.NEW_VALUE = data.DEST_COUNTRY_NAME;
                       break;
                   case "SUBMISSION_DATE":
                       changes.OLD_VALUE = origin.SUBMISSION_DATE != null ? origin.SUBMISSION_DATE.Value.ToString("dd MMM yyyy") : string.Empty;
                       changes.NEW_VALUE = data.SUBMISSION_DATE != null ? data.SUBMISSION_DATE.Value.ToString("dd MMM yyyy") : string.Empty;
                       break;
               }
               _changesHistoryBll.AddHistory(changes);
           }
       }
    }
}
