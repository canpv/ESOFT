using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Esso.Web.ViewModels
{
    public class Inv_Production_DTO
    {
        public DateTime date { get; set; }
        public int invId { get; set; }
        public float totaPanelPower { get; set; }
        public string inverter_Name { get; set; }
        public int stationId { get; set; }
        public float? dailyProduction { get; set; }
        public float? specificYield { get; set; }
        public float? acCurrent { get; set; }
        public float? dcCurrent { get; set; }
        public string invName { get; set; }
        public float irradiation { get; set; }
        public float? acPower { get; set; }
        public float? dcPower { get; set; }
        public float? acVoltage { get; set; }
        public float? dcVoltage { get; set; }
    }
}