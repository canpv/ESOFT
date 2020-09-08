using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Esso.Web.Models.DashboardModel
{
    public class Meteorology_DTO
    {
        public DateTime date { get; set; }
        public long dateUTC { get; set; }
        public float? irradiation { get; set; }
        public double? extTemp { get; set; }
        public float? cellTemp { get; set; }
        public float? wind { get; set; }
        public float? pyranometer { get; set; }
        public float? irradiation2 { get; set; }
        public double? extTemp2 { get; set; }
        public float? cellTemp2 { get; set; }
        public float? wind2 { get; set; }
        public float? pyranometer2 { get; set; }
    }
    public class Meteorology_Main_DTO
    {
        public List<Meteorology_DTO> listData { get; set; }
        public Meteorology_DTO endData { get; set; }
        public string ErrorMessage { get; set; }
    }
}