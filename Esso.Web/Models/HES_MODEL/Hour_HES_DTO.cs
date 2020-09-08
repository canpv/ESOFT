using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Esso.Web.Models.HES_MODEL
{
    public class Hour_HES_DTO
    {
        public DateTime _tarih { get; set; }
        public double _enerjiArtan { get; set; }
        public double _enerji { get; set; }
        public double _isinimToplam { get; set; }
        public double _uretilen_enerji { get; set; }
        public string _saat { get; set; }
    }
}