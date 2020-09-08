using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Esso.Web.ViewModels
{
    public class InverterDetailDTO
    {
        public int Id { get; set; }
        public int Inv_Id { get; set; }
        public int STATION_ID { get; set; }
        public DateTime Tarih { get; set; }
        public float? Guc_AC { get; set; }
        public float? Guc_DC { get; set; }
        public float? Akim_AC { get; set; }
        public float? Akim_DC { get; set; }
        public float? Gerilim_AC { get; set; }
        public float? Gerilim_DC { get; set; }
        public string InvName { get; set; }

        public float? InverterProduction { get; set; }
    }
}