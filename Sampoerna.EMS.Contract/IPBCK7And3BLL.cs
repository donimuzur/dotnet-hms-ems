using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;

namespace Sampoerna.EMS.Contract
{
    public interface IPBCK7And3BLL
    {
        List<Pbck7AndPbck3Dto> GetAllPbck7();
        List<Pbck3Dto> GetAllPbck3();

        List<Pbck7AndPbck3Dto> GetPbck7SummaryReportsByParam(Pbck7SummaryInput input);
        List<Pbck3Dto> GetPbck3SummaryReportsByParam(Pbck3SummaryInput input);

        List<Pbck7AndPbck3Dto> GetPbck7ByParam(Pbck7AndPbck3Input input);

        List<Pbck3Dto> GetPbck3ByParam(Pbck7AndPbck3Input input);


        Pbck7AndPbck3Dto GetPbck7ById(int? id);

        void Insert(Pbck7AndPbck3Dto pbck7AndPbck3Dto);

        void InsertPbck7(Pbck7AndPbck3Dto pbck7AndPbck3Dto);


        void InsertBack1(Back1Dto back1);
        void InsertBack3(Back3Dto back3);

        void InsertCk2(Ck2Dto ck2);


        Back1Dto GetBack1ByPbck7(int pbck7Id);

        Pbck3Dto GetPbck3ByPbck7Id(int? id);

        

        Back3Dto GetBack3ByPbck3Id(int? id);

        Ck2Dto GetCk2ByPbck3Id(int? id);


        void InsertPbck3(Pbck3Dto pbck3Dto);

    }
}
