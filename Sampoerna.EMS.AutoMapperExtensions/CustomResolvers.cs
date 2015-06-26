using System;
using System.Globalization;
using AutoMapper;
using AutoMapper.Internal;

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


    //public class VirtualPlantMapCompanyNameResolver : ValueResolver<object, string>
    //{
    //    protected override string ResolveCore(object value)
    //    {
            
    //        string InputAsString = value.ToNullSafeString();

    //        if (string.IsNullOrWhiteSpace(InputAsString))
    //            return null;

    //        return int.Parse(InputAsString);
    //    }
    //}
}
