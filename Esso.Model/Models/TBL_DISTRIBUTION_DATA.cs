using System;
using System.ComponentModel.DataAnnotations;

namespace Esso.Models
{
    public class TBL_DISTRIBUTION_DATA
    {
        public int ID { get; set; }
        public int? STATION_ID { get; set; }
        public float? P_180 { get; set; }
        public float? RI_580 { get; set; }
        public float? RC_880 { get; set; }
        public float? P_280 { get; set; }
        public float? RI_680 { get; set; }
        public float? RC_780 { get; set; }
        public float? DEMAND { get; set; }
        public float? INDUCTIVE_REAKTIF_RATE { get; set; }
        public float? CAPACITIVE_REAKTIF_RATE { get; set; }
        public DateTime INSERT_DATE { get; set; }
        public DateTime CREATED_DATE { get; set; }
    }
}
