﻿using System;
using System.Globalization;
using AutoMapper;
using AutoMapper.Internal;
using Sampoerna.EMS.BusinessObject;

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

    public class SourcePlantTextResolver : ValueResolver<T1001W, string>
    {
        protected override string ResolveCore(T1001W value)
        {
            if (string.IsNullOrEmpty(value.CITY))
                return value.NAME1;

            return value.NAME1 + " - " + value.CITY;
        }
    }

    public class PlantCityCodeResolver : ValueResolver<T1001W, string>
    {
        protected override string ResolveCore(T1001W value)
        {
            return "KPPBC " + value.ZAIDM_EX_NPPBKC.CITY + " - " + value.ZAIDM_EX_NPPBKC.ZAIDM_EX_KPPBC.KPPBC_NUMBER; 
            
        }
    }

    public class CK5ListIndexQtyResolver : ValueResolver<CK5, string>
    {
        protected override string ResolveCore(CK5 value)
        {
            string resultValue = "";
            string resultUOM = "Boxes";

            if (value.GRAND_TOTAL_EX.HasValue)
                resultValue = value.GRAND_TOTAL_EX.Value.ToString("f2");

            if (value.PACKAGE_UOM_ID.HasValue)
            {
                if (value.UOM != null)
                    resultUOM = value.UOM.UOM_NAME;
            }

            return resultValue + " " + resultUOM;
        }
    }
}
