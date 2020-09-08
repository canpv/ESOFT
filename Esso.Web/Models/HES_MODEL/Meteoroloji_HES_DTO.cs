using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Esso.Web.Models.HES_MODEL
{
    public class Meteoroloji_HES_DTO
    {
        public DateTime DATE { get; set; }
        public float? FLOW { get; set; }
        public float? KOT { get; set; }

    }
}