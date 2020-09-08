using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using Esso.Web.Models.DATE_NUMBER;

namespace Esso.Web.Helpers
{
    public class DateTimeHelper
    {
        public static DateTime BeginDate(string date)
        {
            var startDate = DateTime.Parse(@date);
            TimeSpan ts = new TimeSpan(0, 0, 0);
            startDate = startDate + ts;
            return startDate;
        }

        public static DateTime EndDate(string date)
        {
            var endDate = DateTime.Parse(@date);
            TimeSpan te = new TimeSpan(23, 59, 59);
            endDate = endDate + te;
            return endDate;
        }

        public static long convertDateUTC(DateTime dt)
        {
            var timeSpan = (dt - new DateTime(1970, 1, 1, 0, 0, 0, 0, 0));

            return (long)timeSpan.TotalSeconds * 1000;
        }

        public static NUMBER_FORMAT_DTO ConvertNumberFormatMinute(string date)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("tr-TR");
            DateTime selectDate = DateTime.Parse(date);
            string _convertDate = "";
            string _year = selectDate.Year.ToString();
            string _month = selectDate.Month.ToString();
            string _day = selectDate.Day.ToString();
            if (_month.Length < 2)
            {
                _month = "0" + selectDate.Month.ToString();
            }
            if (_day.Length < 2)
            {
                _day = "0" + selectDate.Day.ToString();
            }
            _convertDate = _year + _month + _day;
            string _strDateBegin = _convertDate + "0400";
            string _strDateEnd = _convertDate + "2100";
            NUMBER_FORMAT_DTO ndto = new NUMBER_FORMAT_DTO();
            ndto._begin = Convert.ToInt64(_strDateBegin);
            ndto._end = Convert.ToInt64(_strDateEnd);
            return ndto;
        }
        public static string HourMinuteFormat(DateTime dt)
        {
            string _date = "";
            string _hour = dt.Hour.ToString();
            string _minute = dt.Minute.ToString();
            if (_hour.Length < 2)
            {
                _hour = "0" + dt.Month.ToString();
            }
            if (_minute.Length < 2)
            {
                _minute = "0" + dt.Minute.ToString();
            }
            _date = _hour + _minute;
            return _date;
        }

        public static NUMBER_FORMAT_DTO ConvertNumberFormatHour(string date)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("tr-TR");
            DateTime selectDate = DateTime.Parse(date);
            string _convertDate = "";
            string _year = selectDate.Year.ToString();
            string _month = selectDate.Month.ToString();
            string _day = selectDate.Day.ToString();
            if (_month.Length < 2)
            {
                _month = "0" + selectDate.Month.ToString();
            }
            if (_day.Length < 2)
            {
                _day = "0" + selectDate.Day.ToString();
            }
            _convertDate = _year + _month + _day;
            string _strDateBegin = _convertDate + "04";
            string _strDateEnd = _convertDate + "21";
            NUMBER_FORMAT_DTO ndto = new NUMBER_FORMAT_DTO();
            ndto._begin = Convert.ToInt64(_strDateBegin);
            ndto._end = Convert.ToInt64(_strDateEnd);
            return ndto;
        }
    }
}