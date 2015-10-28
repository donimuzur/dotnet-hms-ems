using System;
using System.Globalization;
using System.Linq;
using AutoMapper;
using AutoMapper.Internal;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;

namespace Sampoerna.EMS.AutoMapperExtensions
{
    /// <summary>
    /// Resolve String as CultureInfo.InvariantCulture to a nullable DateTime
    /// </summary>
    public class StringToDateResolver : ValueResolver<object, DateTime?>
    {
        protected override DateTime? ResolveCore(object value)
        {
            string InputAsString = value.ToNullSafeString();

            if (string.IsNullOrWhiteSpace(InputAsString))
                return null;

            return DateTime.Parse(InputAsString, CultureInfo.InvariantCulture);
        }
    }

    /// <summary>
    /// Resolve nullable DateTime to a String as CultureInfo.InvariantCulture
    /// </summary>
    public class DateToStringResolver : ValueResolver<object, string>
    {
        protected override string ResolveCore(object value)
        {
            if (value == null)
                return null;

            return ((DateTime)value).ToString(CultureInfo.InvariantCulture);
        }
    }

    /// <summary>
    /// Resolve decimal to a int
    /// </summary>
    public class DecimalToIntResolver : ValueResolver<object, int>
    {
        protected override int ResolveCore(object value)
        {
            try
            {
                return Convert.ToInt32(value);
            }
            catch (Exception ex)
            {

                return -1;
            }

        }
    }

    /// <summary>
    /// Resolve int to a decimal
    /// </summary>
    public class IntToDecimalResolver : ValueResolver<object, decimal>
    {
        protected override decimal ResolveCore(object value)
        {
            try
            {
                return Convert.ToDecimal(value);
            }
            catch (Exception ex)
            {

                return -1;
            }

        }
    }

    /// <summary>
    /// Resolve String as CultureInfo.InvariantCulture to a nullable DateTime
    /// </summary>
    public class StringToNullableIntegerResolver : ValueResolver<object, int?>
    {
        protected override int? ResolveCore(object value)
        {
            string InputAsString = value.ToNullSafeString();

            if (string.IsNullOrWhiteSpace(InputAsString))
                return null;

            return int.Parse(InputAsString);
        }
    }

    public class NullableBooleanToStringDeletedResolver : ValueResolver<bool?, string>
    {
        protected override string ResolveCore(bool? value)
        {
            if (!value.HasValue)
                return "No";
            return value.Value ? "Yes" : "No";
        }
    }

    public class StringToBooleanResolver : ValueResolver<string, bool?>
    {
        protected override bool? ResolveCore(string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                return true;
            }

            if (source == "Yes")
            {
                return true;
            }
            return false;
        }
    }

    public class SourcePlantTextResolver : ValueResolver<T001W, string>
    {
        protected override string ResolveCore(T001W value)
        {
            if (string.IsNullOrEmpty(value.ORT01))
                return value.NAME1;

            return value.NAME1 + " - " + value.ORT01;
        }
    }


    public class PlantCityCodeResolver : ValueResolver<T001W, string>
    {
        protected override string ResolveCore(T001W value)
        {
            return "KPPBC " + value.ZAIDM_EX_NPPBKC.CITY + " - " + value.ZAIDM_EX_NPPBKC.ZAIDM_EX_KPPBC.KPPBC_ID;

        }
    }

    public class CK5ListIndexQtyResolver : ValueResolver<CK5Dto, string>
    {
        protected override string ResolveCore(CK5Dto value)
        {
            string resultValue = "";
            string resultUOM = "Boxes";

            if (value.GRAND_TOTAL_EX.HasValue)
                resultValue = value.Ck5MaterialDtos.Sum(x => x.QTY).ToString();

            if (!string.IsNullOrEmpty(value.PACKAGE_UOM_ID))
            {
                var firstOrDefault = value.Ck5MaterialDtos.FirstOrDefault();
                if (firstOrDefault != null)
                    resultUOM = firstOrDefault.UOM;

            }

            return resultValue + " " + resultUOM;
        }
    }

    public class StringToDecimalResolver : ValueResolver<string, decimal>
    {
        protected override decimal ResolveCore(string value)
        {
            try
            {
                return Convert.ToDecimal(value);
            }
            catch (Exception ex)
            {

                return -1;
            }

        }
    }

    public class DecimalToStringResolver : ValueResolver<decimal?, string>
    {
        protected override string ResolveCore(decimal? value)
        {
            if (!value.HasValue)
                return "0";

            return value.Value.ToString();

        }

    }

    /// <summary>
    /// Resolve String as CultureInfo.InvariantCulture to a nullable DateTime
    /// </summary>
    public class StringToNullableDecimalResolver : ValueResolver<object, decimal?>
    {
        protected override decimal? ResolveCore(object value)
        {
            string InputAsString = value.ToNullSafeString();

            if (string.IsNullOrWhiteSpace(InputAsString))
                return null;

            return decimal.Parse(InputAsString);
        }
    }

    public class CK5ListIndexDestinationPlantResolver : ValueResolver<CK5Dto, string>
    {
        protected override string ResolveCore(CK5Dto value)
        {

            if (value.IsCk5Export)
                return value.DEST_COUNTRY_CODE + " - " + value.DEST_COUNTRY_NAME;

            return value.DEST_PLANT_ID + " - " + value.DEST_PLANT_NAME;
        }
    }

    public class CK5ListIndexPOAResolver : ValueResolver<CK5Dto, string>
    {
        protected override string ResolveCore(CK5Dto value)
        {

            if (!string.IsNullOrEmpty(value.APPROVED_BY_POA))
                return value.APPROVED_BY_POA;
            if (!string.IsNullOrEmpty(value.APPROVED_BY_MANAGER))
                return value.APPROVED_BY_MANAGER;

            return "";
        }
    }

    public class Ck5MaterialHjeSummaryReportsResolver : ValueResolver<CK5, string>
    {
        protected override string ResolveCore(CK5 dbCk5)
        {
            string resultValue = "";

            foreach (var ck5Material in dbCk5.CK5_MATERIAL)
            {
                string ck5Hje = ck5Material.HJE.HasValue ? ck5Material.HJE.Value.ToString("#,##0.#0") : "0";
                resultValue += ck5Material.BRAND + "-" + ck5Hje + ";;";
            }

            if (resultValue.Length > 0)
            {
                resultValue = resultValue.Substring(0, resultValue.Length - 2);
                resultValue = resultValue.Replace(";;", "\r\n");
            }
            return resultValue;
        }
    }

    public class Ck5MaterialTariffSummaryReportsResolver : ValueResolver<CK5, string>
    {
        protected override string ResolveCore(CK5 dbCk5)
        {
            string resultValue = "";

            foreach (var ck5Material in dbCk5.CK5_MATERIAL)
            {
                string ck5Hje = ck5Material.TARIFF.HasValue ? ck5Material.TARIFF.Value.ToString("#,##0.#0") : "0";
                resultValue += ck5Material.BRAND + "-" + ck5Hje + ";;";
            }

            if (resultValue.Length > 0)
            {
                resultValue = resultValue.Substring(0, resultValue.Length - 2);
                resultValue = resultValue.Replace(";;", "\r\n");
            }
            return resultValue;
        }
    }

    public class Ck5MaterialExciseValueSummaryReportsResolver : ValueResolver<CK5, string>
    {
        protected override string ResolveCore(CK5 dbCk5)
        {
            string resultValue = "";

            foreach (var ck5Material in dbCk5.CK5_MATERIAL)
            {
                string ck5Hje = ck5Material.EXCISE_VALUE.HasValue ? ck5Material.EXCISE_VALUE.Value.ToString("#,##0.#0") : "0";
                resultValue += ck5Material.BRAND + "-" + ck5Hje + ";;";
            }

            if (resultValue.Length > 0)
            {
                resultValue = resultValue.Substring(0, resultValue.Length - 2);
                resultValue = resultValue.Replace(";;", "\r\n");
            }
            return resultValue;
        }
    }

    public class Ck5MaterialConvertionSummaryReportsResolver : ValueResolver<CK5, string>
    {
        protected override string ResolveCore(CK5 dbCk5)
        {
            string resultValue = "";

            var listGroup = dbCk5.CK5_MATERIAL.GroupBy(a => a.CONVERTED_UOM)
              .Select(x => new CK5ReportMaterialGroupUomDto
              {
                  Uom = x.Key,
                  SumUom = x.Sum(c => c.QTY.HasValue ? c.QTY.Value : 0)
              }).ToList();


            resultValue = string.Join(Environment.NewLine,
                listGroup.Select(c => c.SumUom.ToString("#,##0.#0") + " " + c.Uom));


            //foreach (var ck5Material in dbCk5.CK5_MATERIAL.GroupBy(a=> a.BRAND))
            //{
            //    string ck5Convertion = ck5Material.CONVERTION.HasValue ? ck5Material.CONVERTION.ToString() : "0";
            //    resultValue += ck5Convertion + " " + ck5Material.CONVERTED_UOM + ";;";
            //}

            //if (resultValue.Length > 0)
            //{
            //    resultValue = resultValue.Substring(0, resultValue.Length - 2);
            //    resultValue = resultValue.Replace(";;", "\r\n");
            //}
            return resultValue;
        }
    }

    public class Ck5MaterialNumberBoxUomSummaryReportsResolver : ValueResolver<CK5, string>
    {
        protected override string ResolveCore(CK5 dbCk5)
        {
            string resultValue = "";

            var listGroup = dbCk5.CK5_MATERIAL.GroupBy(a => a.UOM)
              .Select(x => new CK5ReportMaterialGroupUomDto
              {
                  Uom = x.Key,
                  SumUom = x.Sum(c => c.QTY.HasValue ? c.QTY.Value : 0)
              }).ToList();


            resultValue = string.Join(Environment.NewLine,
                listGroup.Select(c => c.SumUom.ToString("#,##0.#0") + " " + c.Uom));
       
            return resultValue;
        }
    }
}
