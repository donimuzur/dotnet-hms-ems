using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sampoerna.EMS.Website.Models.NPPBKC
{
    public class NewNPPBKCViewModel : NPPBKCIViewModels
    {
        public NewNPPBKCViewModel() : base()
        {
            Details = new List<NewNppbckDetails>();
        }

        public class NewNppbckDetails : VirtualNppbckDetails
        { 
            [Required(ErrorMessage = "DGCE Address can't be empty"), StringLength(400, ErrorMessage = "DGCE Address can't longer than 400 characters")]
            public string DgceAddress { set; get; }
            [Required(ErrorMessage = "Location can't be empty"), StringLength(400, ErrorMessage = "Location can't longer than 400 characters")]
            public string Location { set; get; }
            [Required(ErrorMessage = "KPPBC Address can't be empty"), StringLength(400, ErrorMessage = "KPPBC Address can't longer than 400 characters")]
            public string KppbcAddress { set; get; }
            public string CreatedBy { set; get; }
            public string ModifiedBy { set; get; }
            public DateTime? ModifiedDate { set; get; }

            public new KppbcModel KPPBC { get; set; }

            public new VendorModel VENDOR { get; set; }
        }

        public class NppbkcModel : NppbkcFormModel
        {
            public NppbkcModel() : base()
            {
                Detail = new NewNppbckDetails();
                Plant = new List<BusinessObject.T001W>();
            }
            public new NewNppbckDetails Detail { set; get; }
        }

        public class KppbcModel
        {
            public string KppbcId { set; get; }
            public string Type { set; get; }
            public string Detail { set; get; }
            public string Header { set; get; }
            public string Footer { set; get; }
            public DateTime CreatedDate { set; get; }
            public DateTime? ModifiedDate { set; get; }
            public string CreatedBy { set; get; }
            public string ModifiedBy { set; get; }
            public bool? IsDeleted { set; get; }
            public string Notes { set; get; }
        }

        public class VendorModel
        {
            public string Id { set; get; }
            public string CompanyName { set; get; }
            public string KppbcName { set; get; }
            public DateTime CreatedDate { set; get; }
            public DateTime ? ModifiedDate { set; get; }
            public string CreatedBy { set; get; }
            public string ModifiedBy { set; get; }
            public string CityName { set; get; }
            public string Address { set; get; }

        }

        public new List<NewNppbckDetails> Details { get; set; }
    }
}