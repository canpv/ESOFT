using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Esso.Web.ViewModels
{
    public class TBL_PR_DTO
    {
        public TBL_PR_DTO()
        {
            listPR = new List<TBL_PR>();
        }
        public List<TBL_PR> listPR { get; set; }
        public float toplamEnerji { get; set; }
        public float toplamIsinim { get; set; }
        public float toplamPR { get; set; }
        public float ortalamaPR { get; set; }
        public float ortalamaEnerji { get; set; }
        public float anlikPR { get; set; }
        public float anlikEnerji { get; set; }
        public float _target { get; set; }
        public float toplamKazancUS { get; set; }
        public float toplamKazancTL { get; set; }

    }
    public class TBL_PR
    {
        public DateTime _tarih { get; set; }
        public float? _enerji { get; set; }
        public float? _IrradiationSum { get; set; }
        public float? _pr { get; set; }
        public float? _exchangeTL { get; set; }
        public float? _incomeUS { get; set; }
        public float? _incomeTL { get; set; }
    }

    public class TBL_PR_DTO2
    {
        public List<TBL_Week> listPR { get; set; }
        public float toplamEnerji { get; set; }
        public float toplamIsinim { get; set; }

        public float toplamPR { get; set; }
        public float ortalamaPR { get; set; }
        public float ortalamaEnerji { get; set; }
        public float anlikPR { get; set; }
        public float anlikEnerji { get; set; }

        public float _target { get; set; }

    }
    public class TBL_Week
    {
        public DateTime _tarih { get; set; }
        public string _dayName { get; set; }
        public float? _enerji { get; set; }
        public float? _pr { get; set; }
    }

}