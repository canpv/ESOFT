using Esso.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;

namespace Esso.Web.Helpers
{
    public class Helpers
    {
        
        public static DateTime GetQueryDate(DateTime startDate)
        {
            DateTime date = DateTime.Now;
            DateTime curDate = DateTime.Now.AddDays(-15);

            if (startDate.Day == curDate.Day)
            {
                return startDate;
            }
            else
            {
                while (date.Day != startDate.Day)
                {
                    date = date.AddDays(-1);
                }
                return date;
            }
        }

				
	}
}