using Esso.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Esso.Web.Models.DashboardModel
{
    public class Production_DTO
    { 
        public int id { get; set; }
        public DateTime date { get; set; }
        public float? powerAc { get; set; }
        public float? energy { get; set; }
        public float? maxPowerAc { get; set; }
        public float? powerDc { get; set; }
        public long dateUTC { get; set; }
        public float? ekkPowerAc { get; set; }
        public float? irradiation { get; set; }
        public double? extTemp { get; set; }
        public float? cellTemp { get; set; }
        public float? wind { get; set; }
    }

    public class Production_Main_DTO
    {
        public List<Production_DTO> listData { get; set; }
        public TBL_STATION station { get; set; }
        public bool isToday { get; set; }
        public float? _irradiationScale { get; set; }
        public float? _acInstalledPower { get; set; }
        public string stationName { get; set; }
        public Nullable<float> specificYield { get; set; }
        public Nullable<float> actualValue { get; set; }
        public Nullable<float> efficiency { get; set; }
        public bool? isMeteorology { get; set; }
        public bool? isPyranometer { get; set; }
        public bool? isEKK { get; set; }
        public int? stationType { get; set; }
        public string ErrorMessage { get; set; }
        public Production_DTO endData { get; set; }
        public Nullable<float> _dailyProduction { get; set; }
        public Nullable<float> _monthlyProduction { get; set; }
        public Nullable<float> _annualProduction { get; set; }
        public Nullable<float> _totalProduction { get; set; }
        public Nullable<double> _dailyIncome { get; set; }
        public Nullable<float> _monthlyIncome { get; set; }
        public Nullable<double> _annualIncome { get; set; }
        public Nullable<double> _totalIncome { get; set; }
        public Nullable<double> _dailyIncomeTL { get; set; }
        public Nullable<float> _monthlyIncomeTL { get; set; }
        public Nullable<double> _annualIncomeTL { get; set; }
        public Nullable<double> _totalIncomeTL { get; set; }
        public Nullable<double> _dailyTax { get; set; }
        public Nullable<double> _monthlTax { get; set; }
        public Nullable<double> _annualTax { get; set; }
        public Nullable<double> _totalTax { get; set; }
        public Nullable<double> _dailyPr { get; set; }
        public Nullable<double> _monthlyPr { get; set; }
        public Nullable<double> _annualPr { get; set; }
        public Nullable<double> _totalPr { get; set; }
        public Nullable<double> _dailyKF { get; set; }
        public Nullable<double> _monthlyKF { get; set; }
        public Nullable<double> _annualKF { get; set; }
        public Nullable<float> H2_P { get; set; }
    }
}