using System.Collections.Generic;

namespace Sampoerna.EMS.BusinessObject.DTOs
{
    public class Pbck1ReportDto
    {
        public Pbck1ReportDto()
        {
            ProdPlanList = new List<Pbck1ReportProdPlanDto>();
            Detail = new Pbck1ReportInformationDto();
            RealisasiP3Bkc = new List<Pbck1RealisasiP3BkcDto>();
            HeaderFooter = new HEADER_FOOTER_MAPDto();
        }
        public Pbck1ReportInformationDto Detail { get; set; }
        public List<Pbck1ReportProdPlanDto> ProdPlanList { get; set; }

        /// <summary>
        /// Pbck-1 Prod Conv uploaded file
        /// </summary>
        public List<Pbck1ReportBrandRegistrationDto> BrandRegistrationList { get; set; }
        public List<Pbck1RealisasiP3BkcDto> RealisasiP3Bkc { get; set; }

        public HEADER_FOOTER_MAPDto HeaderFooter { get; set; }
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
        /// get from POA Address on POA Master
        /// </summary>
        public string PoaAddress { get; set; }
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
        /// <summary>
        /// Request Qty Uom Id
        /// </summary>
        public string RequestQtyUom { get; set; }
        /// <summary>
        /// Request Qty Uom Desc
        /// </summary>
        public string RequestQtyUomName { get; set; }
        /// <summary>
        /// Latest Saldo from LACK-1 in PBCK-1 form
        /// </summary>
        public string LatestSaldo { get; set; }
        /// <summary>
        /// from Requested UOM in PBCK-1 form. If the value is “Litre” then dislayed as “liter”
        /// </summary>
        public string LatestSaldoUom { get; set; }
        public string SupplierCompanyName { get; set; }
        public string SupplierPlantId { get; set; }
        public string SupplierPlantName { get; set; }
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
        public string ProdPlanPeriode { get; set; }
        public string Lack1Periode { get; set; }
    }

    public class Pbck1ReportProdPlanDto
    {
        public string ProdTypeCode { get; set; }
        public string ProdTypeName { get; set; }
        public string ProdAlias { get; set; }
        public decimal? Amount { get; set; }
        public decimal? BkcRequired { get; set; }
        public string BkcRequiredUomId { get; set; }
        public string BkcRequiredUomName { get; set; }
        public int MonthId { get; set; }
        public string MonthName { get; set; }
    }

    public class Pbck1ReportBrandRegistrationDto
    {
        public string Type { get; set; }
        public string Brand { get; set; }
        public string Kadar { get; set; }
        public string Convertion { get; set; }
        public string ConvertionUomId { get; set; }
        public string ConvertionUom { get; set; }
    }

    public class Pbck1RealisasiP3BkcDto
    {
        public string Bulan { get; set; }
        public decimal SaldoAwal { get; set; }
        public decimal Pemasukan { get; set; }
        public decimal Penggunaan { get; set; }
        public string Jenis { get; set; }
        public decimal Jumlah { get; set; }
        public decimal SaldoAkhir { get; set; }
        public string Lack1UomId { get; set; }
        public string Lack1UomName { get; set; }
        
        //lack1 production detail
        public decimal ProdAmount { get; set; }
        public string ProductCode { get; set; }
        public string ProductType { get; set; }
        public string ProductAlias { get; set; }
    }
}
