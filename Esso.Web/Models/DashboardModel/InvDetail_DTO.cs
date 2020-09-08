using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Esso.Web.Models.DashboardModel
{
    public class InvDetail_DTO
    {
        public int invId { get; set; }
        public int stationId { get; set; }
        public DateTime? date { get; set; }
        public float? powerAC { get; set; }
        public float? powerDC { get; set; }
        public float? currentAC { get; set; }
        public float? currentDC { get; set; }
        public float? voltageAC { get; set; }
        public float? voltageDC { get; set; }
        public float? energy { get; set; }
    }

    public class InvDetail_Main_DTO
    {
        public int invId { get; set; }
        public string invName { get; set; }
        public List<InvDetail_DTO> dataList { get; set; }

    }
}