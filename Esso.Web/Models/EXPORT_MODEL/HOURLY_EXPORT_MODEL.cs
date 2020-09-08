using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Esso.Web.Models.EXPORT_MODEL
{
    public class HOURLY_EXPORT_MODEL
    {
        public DateTime DATE { get; set; }
        public double? INV_PRODUCED_CUMULATIVE { get; set; }
        public double? INV_PRODUCED { get; set; }
        public double? EKK_PRODUCED { get; set; }
        public double? IRRADIATION_ENERGY { get; set; }
        public double CELL_TEMPERATURE { get; set; }

    }
}