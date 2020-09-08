using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Esso.Web.Models.HES_MODEL
{
    public class OZET_HES_DTO_
    {
        public List<TBL_OZET_DTO_HES> _ozet { get; set; }
        public float? _irradiationScale { get; set; }
        public float? _acInstalledPower { get; set; }
        public List<JSON_ARRAY> _AA { get; set; }
    }
    public class Hour_DTO
    {
        public DateTime _tarih { get; set; }
        public double _enerjiArtan { get; set; }
        public double _enerji { get; set; }
        public double _isinimToplam { get; set; }
        public double _uretilen_enerji { get; set; }
        public int _saat { get; set; }
    }
    public class JSON_ARRAY
    {
        public string name { get; set; }
        public List<object> data { get; set; }
    }

}