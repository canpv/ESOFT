using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Esso.Web.ViewModels
{
    public class String_Hour_DTO
    {
        public string date { get; set; }
        public int ID { get; set; }
        public string NAME { get; set; }
        public float VALUE { get; set; }
        public int _saat { get; set; }
        public double result { get; set; }
        public long TARIH_NUMBER { get; set; }
        public string DISPLAY_NAME { get; set; }
    }

    public class Hour_DTO
    { 
        public DateTime _tarih { get; set; }
        public double _enerjiArtan { get; set; }
        public double _enerji { get; set; }
        public double _isinimToplam { get; set; }
        public double _uretilen_enerji { get; set; }
        public int _saat { get; set; }
        public double _hucre_sicakligi { get; set; }
    }

    public class Hour_DTO_
    {
        public string _hour { get; set; }
        public String InverterName { get; set; }
        public int InverterId { get; set; }
        public float? _enerji { get; set; }
        public float? _enerji_guc { get; set; }
        public double result { get; set; }
        public DateTime _tarih { get; set; }
    }
}