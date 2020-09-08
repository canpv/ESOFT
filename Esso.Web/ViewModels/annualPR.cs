using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Esso.Web.ViewModels
{
    public class annualPR
    {
        public float _totalEnerji { get; set; }
        public int _month { get; set; }
        public float _avgPR { get; set; }
        public DateTime _date { get; set; }
    }
}