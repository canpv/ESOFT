using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Esso.Web.ViewModels
{
    public class ProductionModel
    {
        public DateTime _tarih { get; set; }
        public int _stationId { get; set; }
        public Nullable<double> _dailyProduction { get; set; }
        public Nullable<double> _monthlyProduction { get; set; }
        public Nullable<double> _annualProduction { get; set; }
        public Nullable<double> _totalProduction { get; set; }
        public Nullable<double> _dailyIncome { get; set; }
        public Nullable<float> _monthlyIncome { get; set; }
        public Nullable<double> _annualIncome { get; set; }
        public Nullable<double> _totalIncome { get; set; }
        public Nullable<double> _dailyIncomeTL { get; set; }
        public Nullable<float> _monthlyIncomeTL { get; set; }
        public Nullable<double> _annualIncomeTL { get; set; }
        public Nullable<double> _totalIncomeTL { get; set; }
        public Nullable<double> _isinim { get; set; }
        public Nullable<double> _ruzgar { get; set; }
        public Nullable<double> _hucreSicakligi { get; set; }
        public Nullable<double> _ortamSicakligi { get; set; }
        public Nullable<double> _pac { get; set; }
        public Nullable<double> _pdc { get; set; }
        public Nullable<double> _specificYield { get; set; }
        public Nullable<double> _actualValue { get; set; }
        public Nullable<int> _invCount { get; set; }
        public string _invName { get; set; }
        public Nullable<double> _dailyTax { get; set; }
        public Nullable<double> _monthlTax { get; set; }
        public Nullable<double> _annualTax { get; set; }
        public Nullable<double> _totalTax { get; set; }
        public Nullable<float> H2_P { get; set; }
        public bool? isEKK { get; set; }
        public bool? isMeteorology { get; set; }
        public Nullable<float> insAC { get; set; }
        public Nullable<double> _dailyPr { get; set; }
        public Nullable<double> _monthlyPr { get; set; }
        public Nullable<double> _annualPr { get; set; }
        public Nullable<double> _totalPr { get; set; }
        public Nullable<double> _dailyKF { get; set; }
        public Nullable<double> _monthlyKF { get; set; }
        public Nullable<double> _annualKF { get; set; }
        public string INV_SUM_ACTIVE_COUNT { get; set; }
        public string StationName { get; set; }

    }
}