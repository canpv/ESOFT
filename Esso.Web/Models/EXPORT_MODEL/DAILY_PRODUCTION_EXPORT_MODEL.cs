using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Esso.Web.Models.EXPORT_MODEL
{
    public class DAILY_PRODUCTION_EXPORT_MODEL
    {
        public DateTime DATE { get; set; }
        public double? ENERGY { get; set; }
        public double? IRRADIATION { get; set; }
        public double? PR { get; set; }
        public double? INCOME_US { get; set; }
        public double? INCOME_TL { get; set; }
        public double? FOREX_BUYING { get; set; }
    }
}