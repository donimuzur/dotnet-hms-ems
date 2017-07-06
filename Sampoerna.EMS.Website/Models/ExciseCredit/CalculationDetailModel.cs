using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sampoerna.EMS.Website.Models.ExciseCredit
{
    public class CalculationDetailModel
    {
        public CalculationDetailModel()
        {
            CreditRanges = new Dictionary<int, Dictionary<string, double>>();
            MaxCreditRange = new Dictionary<string, CreditAdjustment>();
            ProductTypes = new List<string>();
        }

        public string NppbkcId { set; get; }
        public Dictionary<int, Dictionary<string, double>> CreditRanges { get; private set; }
        public Dictionary<string, CreditAdjustment> MaxCreditRange { get; private set; }
        public List<string> ProductTypes { set; get; }
        public double Adjustment { set; get; }
        public string AdjustmentDisplay { set; get; }
        public double LiquidityRatio { set; get; }
        public int Year { set; get; }
        public void SetCreditRange(int monthRange, Dictionary<string, double> values)
        {
            if (!CreditRanges.ContainsKey(monthRange))
            {
                CreditRanges.Add(monthRange, values);
            }
            else
            {
                CreditRanges[monthRange] = values;
            }
        }

        public void CalculateMaxCreditRange()
        {
            List<int> periods = new List<int>();
            foreach (var item in this.CreditRanges)
            {
                periods.Add(item.Key);
            }

            Dictionary<string, double> values = this.CreditRanges[periods.Max()];
            foreach (var item in values)
            {
                var amounts = new List<double>();
                foreach (var span in periods)
                {
                    amounts.Add(this.CreditRanges[span][item.Key]);
                }
                var max = Math.Ceiling(amounts.Max());
                var adjustment = new CreditAdjustment()
                {
                    Adjustment = this.Adjustment / 100.0,
                    Value = max * 2,
                    CssClass = (amounts[0] > amounts[1]) ? "color: black;" : "color: green;"
                };
                adjustment.Calculate();

                if (!MaxCreditRange.ContainsKey(item.Key))
                {
                    MaxCreditRange.Add(item.Key, adjustment);
                }
                else
                {
                    MaxCreditRange[item.Key] = adjustment;
                }
            }
        }

        public class CreditAdjustment
        {
            public double Adjustment { set; get; }
            public double Value { set; get; }
            public double AdditionalValue { set; get; }
            public double Total { set; get; }
            public String CssClass { set; get; }
            public void Calculate()
            {
                this.AdditionalValue = Math.Ceiling(this.Adjustment * this.Value);
                this.Total = this.AdditionalValue + this.Value;
            }
        }

    }
}