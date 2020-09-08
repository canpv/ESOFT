using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Esso.Web.ViewModels
{
    public class Meteoroloji_DTO2
    {
        public DateTime date { get; set; }
        public float? ruzgar_yonu { get; set; }
        public float? hava_sicakligi { get; set; }
        public float? bagil_nem { get; set; }
        public float? mutlak_nem { get; set; }
        public float? mutlak_hava_basinci { get; set; }

        public float? ruzgar_yonu2 { get; set; }
        public float? hava_sicakligi2 { get; set; }
        public float? bagil_nem2 { get; set; }
        public float? mutlak_nem2 { get; set; }
        public float? mutlak_hava_basinci2 { get; set; }

    }
}