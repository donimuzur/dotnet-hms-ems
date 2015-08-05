using System.Collections.Generic;

namespace Sampoerna.EMS.BusinessObject.DTOs
{
    public class Pbck1ReportDto
    {
        public Pbck1ReportDto()
        {
            ProdConverterList = new List<Pbck1ReportProdConverterDto>();
            ProdPlanList = new List<Pbck1ReportProdPlanDto>();
            Detail = new Pbck1ReportInformationDto();
        }
        public Pbck1ReportInformationDto Detail { get; set; }
        public List<Pbck1ReportProdConverterDto> ProdConverterList { get; set; }
        public List<Pbck1ReportProdPlanDto> ProdPlanList { get; set; }
        public List<Pbck1ReportBrandRegistrationDto> BrandRegistrationList { get; set; }
    }

    public class Pbck1ReportInformationDto
    {
        public int Pbck1Id { get; set; }
        public string Pbck1Number { get; set; }
        /// <summary>
        /// “Tambahan” is written when PBKC-1 Type in PBCK-1 form is “Additional”
        /// </summary>
        public string Pbck1AdditionalText { get; set; }
        public string Year { get; set; }
        /// <summary>
        /// Kepala KPPBC ..... => Name 2 in Vendor Master(alias of vendor name)
        /// </summary>
        public string VendorAliasName { get; set; }
        /// <summary>
        /// Di ...... => City in Vendor Master
        /// </summary>
        public string VendorCityName { get; set; }
        /// <summary>
        /// “Juanda Sukrisdianto” is taken from POA in PBCK-1 form
        /// Pengusaha Pengguna
        /// </summary>
        public string PoaName { get; set; }
        /// <summary>
        /// “Excise Executive” is taken from Title in POA Master, refers to POA in PBCK-1 form
        /// </summary>
        public string PoaTitle { get; set; }
        /// <summary>
        /// “PT HM Sampoerna, Tbk” is taken from Company Name in PBCK-1 form
        /// </summary>
        public string CompanyName { get; set; }
        /// <summary>
        /// “0703.1.3.0234” is taken from NPPBKC ID in PBCK-1 form
        /// </summary>
        public string NppbkcId { get; set; }
        /// <summary>
        /// from all plant(s) address in NPPBKC
        /// Each address will be displayed in new line, begin with dash “- “
        /// </summary>
        public string NppbkcAddress { get; set; }
        /// <summary>
        /// taken from Telephone in Plant Master, refers to main plant of NPPBKC ID in PBCK-1 (Maint Plant “Yes” in NPPBKC - Plant Master) 
        /// </summary>
        public string PlantPhoneNumber { get; set; }
        /// <summary>
        /// format : {product type} ({product type alias})
        /// </summary>
        public string ProdConverterProductType { get; set; }

        public string ExcisableGoodsDescription { get; set; }
        /// <summary>
        /// “1 Januari 2013 – 31 Desember 2013”
        /// dd month yyyy
        /// </summary>
        public string PeriodFrom { get; set; }
        /// <summary>
        /// “1 Januari 2013 – 31 Desember 2013”
        /// dd month yyyy
        /// </summary>
        public string PeriodTo { get; set; }
        /// <summary>
        /// If there are more than 1 value of Product Type, then list of all Converted Output, 
        /// Converted UOM, Product Type and Product Type Alias in new line
        /// example : 1.558.580.336 Batang Sigaret Kretek Tangan (SKT)
        /// format : {total amount} {Uom} {ProductType} {ProductTypeAlias}
        /// </summary>
        public string ProductConvertedOutputs { get; set; }
        public string RequestQty { get; set; }
        public string RequestQtyUom { get; set; }
        /// <summary>
        /// Latest Saldo from LACK-1 in PBCK-1 form
        /// </summary>
        public string LatestSaldo { get; set; }
        /// <summary>
        /// from Requested UOM in PBCK-1 form. If the value is “Litre” then dislayed as “liter”
        /// </summary>
        public string LatestSaldoUom { get; set; }
        public string SupplierCompanyName { get; set; }
        public string SupplierNppbkcId { get; set; }
        public string SupplierPlantAddress { get; set; }
        public string SupplierPlantPhone { get; set; }
        public string SupplierKppbcId { get; set; }
        public string SupplierKppbcMengetahui { get; set; }
        public string SupplierPortName { get; set; }
        public string NppbkcCity { get; set; }
        /// <summary>
        /// “05 Desember 2012” is taken from Printed Date in PBCK-1 form
        /// dd month yyyy
        /// </summary>
        public string PrintedDate { get; set; }
        /// <summary>
        /// Pemasok
        /// </summary>
        public string ExciseManager { get; set; }
    }

    public class Pbck1ReportProdConverterDto
    {
        
    }

    public class Pbck1ReportProdPlanDto
    {
        
    }

    public class Pbck1ReportBrandRegistrationDto
    {
        public string Type { get; set; }
        public string Brand { get; set; }
        public string Kadar { get; set; }
        public string Convertion { get; set; }
        public string ConvertionUom { get; set; }
    }
}
