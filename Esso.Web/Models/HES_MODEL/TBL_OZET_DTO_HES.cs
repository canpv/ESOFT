using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Esso.Web.Models.HES_MODEL
{
    public class TBL_OZET_DTO_HES
    {
        public int _id { get; set; }
        public DateTime _tarih { get; set; }
        public int _stationId { get; set; }
        public float? _powerAC { get; set; }
        public double? _sicaklik { get; set; }
        public float? _isinim { get; set; }
        public float? _producedEnergy { get; set; }
        public float? _consumedEnergy { get; set; }
        public float? _acGuc { get; set; }
        public float? _max { get; set; }
        public float? _hucreSicakligi { get; set; }
        public float? _ruzgarHizi { get; set; }
        public float? _dcGuc { get; set; }
        public float? _debi { get; set; }
        public float? _gunlukUretim { get; set; }
        public long DateUTC { get; set; }
    }
}