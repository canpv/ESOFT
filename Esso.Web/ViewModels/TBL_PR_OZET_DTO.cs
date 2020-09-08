using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Esso.Web.ViewModels
{
    public class TBL_PR_OZET_DTO
    {
        public int _id { get; set; }
        public DateTime _tarih { get; set; }
        public float? _enerji { get; set; }
        public float? _isinim_ortalama { get; set; }
        public float? _pr { get; set; }
        public float? _max { get; set; }
    }
}