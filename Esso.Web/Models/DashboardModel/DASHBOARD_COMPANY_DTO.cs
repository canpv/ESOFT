using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Esso.Web.Models.DashboardModel
{
    public class DASHBOARD_COMPANY_DTO
    {
        public string CompanyName { get; set; }
        public string GroupName { get; set; }
        public float? TotalProduction { get; set; }
        public List<StationProduction> listStations { get; set; }
    }

    public class StationProduction
    {
        public int StationId { get; set; }
        public string StationName { get; set; }
        public float? PowerAC { get; set; }
        public float? Production { get; set; }
        public float? PR { get; set; }
        public float? Latitude { get; set; }
        public float? Longitude { get; set; }

    }
}