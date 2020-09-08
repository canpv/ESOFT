using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Esso.Web.ViewModels
{
    public class WeatherData
    {
        public string _city { get; set; }
        public DateTime _date { get; set; }
        public DateTime _sunrise { get; set; }
        public DateTime _sunset { get; set; }
    }
}