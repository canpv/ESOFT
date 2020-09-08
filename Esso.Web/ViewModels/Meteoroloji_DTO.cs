using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Esso.Web.ViewModels
{
    public class Meteoroloji_DTO
    {
        public DateTime date { get; set; }
        public float? irradiation { get; set; }
        public float? pyranometer { get; set; }
        public float? cell_temp { get; set; }
        public float? external_temp { get; set; }
        public float? wind { get; set; }
        public float? irradiation2 { get; set; }
        public float? pyranometer2 { get; set; }
        public float? cell_temp2 { get; set; }
        public float? external_temp2 { get; set; }
        public float? wind2 { get; set; }
    }
}