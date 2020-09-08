using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Esso.Web.ViewModels
{
    public class TBL_OZET_DTO
    {
        public int _id { get; set; }
        public DateTime _tarih { get; set; }
        public int _stationId { get; set; }
        public float? _gunlukUretim { get; set; }
        public double? _sicaklik { get; set; }
        public float? _isinim { get; set; }
        public float? _enerji { get; set; }
        public float? _acGuc { get; set; }
        public float? _max { get; set; }
        public float? _hucreSicakligi { get; set; }
        public float? _ruzgarHizi { get; set; }
        public float? _dcGuc { get; set; }
        public long _DateUTC { get; set; }
        public float? _EKK_AC_Power { get; set; }
    }

    public class OZET_DTO
    {
        public List<TBL_OZET_DTO> _ozet { get; set; }
        public float? _irradiationScale { get; set; }

        public float? _acInstalledPower { get; set; }
    }
}