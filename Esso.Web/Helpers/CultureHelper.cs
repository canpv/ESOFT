using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Esso.Web.Helpers
{
    public class CultureHelper
    {
        public static void SetCultureInfo()
        {
            System.Globalization.CultureInfo _CultureInfo = new System.Globalization.CultureInfo("tr-TR");
            _CultureInfo.NumberFormat.CurrencyDecimalSeparator = ",";
            _CultureInfo.NumberFormat.CurrencyGroupSeparator = ".";
            _CultureInfo.NumberFormat.NumberDecimalSeparator = ",";
            _CultureInfo.NumberFormat.NumberGroupSeparator = ".";
            _CultureInfo.NumberFormat.PercentDecimalSeparator = ",";
            _CultureInfo.NumberFormat.PercentGroupSeparator = ".";
            System.Threading.Thread.CurrentThread.CurrentCulture = _CultureInfo;
        }
    }
}