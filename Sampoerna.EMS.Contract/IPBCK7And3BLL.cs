﻿using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject.Business;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.BusinessObject.Outputs;

namespace Sampoerna.EMS.Contract
{
    public interface IPBCK7And3BLL
    {
        List<Pbck7AndPbck3Dto> GetAllPbck7();
        List<Pbck3Dto> GetAllPbck3();


        List<Pbck7SummaryReportDto> GetPbck7SummaryReportsByParam(Pbck7SummaryInput input);
        List<Pbck3SummaryReportDto> GetPbck3SummaryReportsByParam(Pbck3SummaryInput input);

        List<Pbck7AndPbck3Dto> GetPbck7ByParam(Pbck7AndPbck3Input input, Login user, bool IsComplete=false);

        List<Pbck3Dto> GetPbck3ByParam(Pbck7AndPbck3Input input, Login user, bool isComplete = false);


        Pbck7AndPbck3Dto GetPbck7ById(int? id);

        //void Insert(Pbck7AndPbck3Dto pbck7AndPbck3Dto);

        //int? InsertPbck7(Pbck7AndPbck3Dto pbck7AndPbck3Dto);
        //void InsertPbck7Item(Pbck7ItemUpload item);

        //void InsertBack1(Back1Dto back1);
        //void InsertBack3(Back3Dto back3);

        //void InsertCk2(Ck2Dto ck2);


        Back1Dto GetBack1ByPbck7(int pbck7Id);

        Pbck3Dto GetPbck3ByPbck7Id(int? id);

        

        Back3Dto GetBack3ByPbck3Id(int? id);

        Ck2Dto GetCk2ByPbck3Id(int? id);


        //void InsertPbck3(Pbck3Dto pbck3Dto);

        Pbck7AndPbck3Dto SavePbck7(Pbck7Pbck3SaveInput input);

        Pbck7DetailsOutput GetDetailsPbck7ById(int id);

        List<Pbck7ItemsOutput> Pbck7ItemProcess(List<Pbck7ItemsInput> inputs);

        void PBCK7Workflow(Pbck7Pbck3WorkflowDocumentInput input);

        Pbck3Output GetPbck3DetailsById(int id);

        Pbck3Dto SavePbck3(Pbck3SaveInput input);

        void PBCK3Workflow(Pbck3WorkflowDocumentInput input);

        void SendMailCompletedPbck3Document(Pbck3WorkflowDocumentInput input);

        Pbck3Dto GetPbck3ById(int id);

        List<GetListFaCodeByPlantOutput> GetListFaCodeByPlant(string plantId);

        GetBrandItemsByPlantAndFaCodeOutput GetBrandItemsByPlantAndFaCode(string plantId, string faCode, string stickerCode = null);

        void UpdateUploadedFileCompletedPbck7(List<BACK1_DOCUMENTDto> input);

        void UpdateUploadedFileCompletedPbck3(List<BACK3_DOCUMENTDto> inputBack3, List<CK2_DOCUMENTDto> inputCk2);

        Pbck73PrintOutDto GetPbck7PrintOutData(int pbck7Id);

        Pbck73PrintOutDto GetPbck3PrintOutData(int pbck3Id);

        List<Pbck3Dto> GetDashboardPbck3ByParam(GetDashboardPbck3ByParamInput input);
        List<Pbck7AndPbck3Dto> GetDashboardPbck7ByParam(GetDashboardPbck7ByParamInput input);

        BlockedStockQuotaOutput GetBlockedStockQuota(string plant, string faCode);

        List<GetListFaCodeByPlantOutput> GetListFaCodeHaveBlockStockByPlant(string plantId);

        decimal GetCurrentReqQtyByPbck7IdAndFaCode(int pbck7Id, string faCode, string stickerCode = null);

        void EditCompletedDocumentPbck7(EditCompletedDocumentPbck7Input input);

        void EditCompletedDocumentPbck3(EditCompletedDocumentPbck3Input input);
    }
}
