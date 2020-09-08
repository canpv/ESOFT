using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Esso.Web.ViewModels
{
    public class EndProductionModel
    {
        public DateTime _tarih { get; set; }
        public Nullable<double> _pac { get; set; }
        public Nullable<double> _dailyProduction { get; set; }
        public Nullable<double> _monthlyProduction { get; set; }
        public Nullable<double> _annualProduction { get; set; }
        public Nullable<double> _totalProduction { get; set; }
        public Nullable<double> _isinim { get; set; }
        public Nullable<double> _ortamSicakligi { get; set; }
        public Nullable<double> _hucreSicakligi { get; set; }
        public Nullable<double> _ruzgar { get; set; }
        public Nullable<double> _pdc { get; set; }
    }
}