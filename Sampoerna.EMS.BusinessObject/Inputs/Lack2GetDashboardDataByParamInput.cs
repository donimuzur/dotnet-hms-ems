namespace Sampoerna.EMS.BusinessObject.Inputs
{
    public class Lack2GetDashboardDataByParamInput
    {
        public int? PeriodFromMonth { get; set; }
        public int? PeriodFromYear { get; set; }
        public int? PeriodToMonth { get; set; }
        public int? PeriodToYear { get; set; }
        public string Creator { get; set; }
        public string Poa { get; set; }
    }
}
