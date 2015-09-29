namespace Sampoerna.EMS.BusinessObject.DTOs
{
    public class Lack1PrintOutDto : Lack1DetailsDto
    {
        public HEADER_FOOTER_MAPDto HeaderFooter { get; set; }
        public string SubmissionDateDisplayString { get; set; }

        public string ExcisableExecutiveCreator { get; set; }

        public string NppbkcCity { get; set; }
    }
}
