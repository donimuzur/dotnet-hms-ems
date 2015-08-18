using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.BusinessObject.Inputs
{
    public class Lack1GetByParamInput
    {
        public string NppbKcId { get; set; }
        public string Poa { get; set; }
        public string PlantId { get; set; }
        //public int? PeriodMonth { get; set; }
        //public int? PeriodYear { get; set; }
        public string Creator { get; set; }
        public string SubmissionDate { get; set; }

        /// <summary>
        /// optional if want to sorting from query
        /// </summary>
        public string SortOrderColumn { get; set; }
        public Enums.LACK1Type Lack1Type { get; set; }

      
    }

    public class Lack1GetLatestSaldoPerPeriodInput
    {
        public int MonthTo { get; set; }
        public int YearTo { get; set; }
        public string NppbkcId { get; set; }
    }

}
