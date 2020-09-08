using Esso.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Esso.Web.Helpers
{
    public class TargetHelpers
    {
        public static float GetDailyTarget(EssoEntities DB, int stationId, int _ay)
        {
            if (_ay == 1)
            {
                float trgt = DB.targets.Where(a => a.IS_DELETED == false && a.STATION_ID == stationId).Select(a => a.JAN_PRODUCTION.Value).FirstOrDefault();
                return (float)Math.Round(trgt / 31, 2);
            }
            else if (_ay == 2)
            {
                float trgt = DB.targets.Where(a => a.IS_DELETED == false && a.STATION_ID == stationId).Select(a => a.FEB_PRODUCTION.Value).FirstOrDefault();
                return (float)Math.Round(trgt / 28, 2);
            }
            if (_ay == 3)
            {
                float trgt = DB.targets.Where(a => a.IS_DELETED == false && a.STATION_ID == stationId).Select(a => a.MARCH_PRODUCTION.Value).FirstOrDefault();
                return (float)Math.Round(trgt / 31, 2);
            }
            else if (_ay == 4)
            {
                float trgt = DB.targets.Where(a => a.IS_DELETED == false && a.STATION_ID == stationId).Select(a => a.APRIL_PRODUCTION.Value).FirstOrDefault();
                return (float)Math.Round(trgt / 30, 2);
            }
            if (_ay == 5)
            {
                float trgt = DB.targets.Where(a => a.IS_DELETED == false && a.STATION_ID == stationId).Select(a => a.MAY_PRODUCTION.Value).FirstOrDefault();
                return (float)Math.Round(trgt / 31, 2);
            }
            else if (_ay == 6)
            {
                float trgt = DB.targets.Where(a => a.IS_DELETED == false && a.STATION_ID == stationId).Select(a => a.JUNE_PRODUCTION.Value).FirstOrDefault();
                return (float)Math.Round(trgt / 30, 2);
            }
            else if (_ay == 7)
            {
                float trgt = DB.targets.Where(a => a.IS_DELETED == false && a.STATION_ID == stationId).Select(a => a.JULY_PRODUCTION.Value).FirstOrDefault();
                return (float)Math.Round(trgt / 31, 2);
            }
            if (_ay == 8)
            {
                float trgt = DB.targets.Where(a => a.IS_DELETED == false && a.STATION_ID == stationId).Select(a => a.AUGUST_PRODUCTION.Value).FirstOrDefault();
                return (float)Math.Round(trgt / 31, 2);
            }
            else if (_ay == 9)
            {
                float trgt = DB.targets.Where(a => a.IS_DELETED == false && a.STATION_ID == stationId).Select(a => a.SEP_PRODUCTION.Value).FirstOrDefault();
                return (float)Math.Round(trgt / 30, 2);
            }
            else if (_ay == 10)
            {
                float trgt = DB.targets.Where(a => a.IS_DELETED == false && a.STATION_ID == stationId).Select(a => a.OKT_PRODUCTION.Value).FirstOrDefault();
                return (float)Math.Round(trgt / 31, 2);
            }
            else if (_ay == 11)
            {
                float trgt = DB.targets.Where(a => a.IS_DELETED == false && a.STATION_ID == stationId).Select(a => a.NOV_PRODUCTION.Value).FirstOrDefault();
                return (float)Math.Round(trgt / 30, 2);
            }
            else if (_ay == 12)
            {
                float trgt = DB.targets.Where(a => a.IS_DELETED == false && a.STATION_ID == stationId).Select(a => a.DEC_PRODUCTION.Value).FirstOrDefault();
                return (float)Math.Round(trgt / 31, 2);
            }
            else
            {
                return 0;
            }
        }

        public static float GetMonthlyTarget(EssoEntities DB, int stationId, int _ay)
        {
            if (_ay == 1)
            {
                float trgt = DB.targets.Where(a => a.IS_DELETED == false && a.STATION_ID == stationId).Select(a => a.JAN_PRODUCTION.Value).FirstOrDefault();
                return (float)Math.Round(trgt, 2);
            }
            else if (_ay == 2)
            {
                float trgt = DB.targets.Where(a => a.IS_DELETED == false && a.STATION_ID == stationId).Select(a => a.FEB_PRODUCTION.Value).FirstOrDefault();
                return (float)Math.Round(trgt, 2);
            }
            else if (_ay == 3)
            {
                float trgt = DB.targets.Where(a => a.IS_DELETED == false && a.STATION_ID == stationId).Select(a => a.MARCH_PRODUCTION.Value).FirstOrDefault();
                return (float)Math.Round(trgt, 2);
            }
            else if (_ay == 4)
            {
                float trgt = DB.targets.Where(a => a.IS_DELETED == false && a.STATION_ID == stationId).Select(a => a.APRIL_PRODUCTION.Value).FirstOrDefault();
                return (float)Math.Round(trgt, 2);
            }
            else if (_ay == 5)
            {
                float trgt = DB.targets.Where(a => a.IS_DELETED == false && a.STATION_ID == stationId).Select(a => a.MAY_PRODUCTION.Value).FirstOrDefault();
                return (float)Math.Round(trgt, 2);
            }
            else if (_ay == 6)
            {
                float trgt = DB.targets.Where(a => a.IS_DELETED == false && a.STATION_ID == stationId).Select(a => a.JUNE_PRODUCTION.Value).FirstOrDefault();
                return (float)Math.Round(trgt, 2);
            }
            else if (_ay == 7)
            {
                float trgt = DB.targets.Where(a => a.IS_DELETED == false && a.STATION_ID == stationId).Select(a => a.JULY_PRODUCTION.Value).FirstOrDefault();
                return (float)Math.Round(trgt, 2);
            }
            else if (_ay == 8)
            {
                float trgt = DB.targets.Where(a => a.IS_DELETED == false && a.STATION_ID == stationId).Select(a => a.AUGUST_PRODUCTION.Value).FirstOrDefault();
                return (float)Math.Round(trgt, 2);
            }
            else if (_ay == 9)
            {
                float trgt = DB.targets.Where(a => a.IS_DELETED == false && a.STATION_ID == stationId).Select(a => a.SEP_PRODUCTION.Value).FirstOrDefault();
                return (float)Math.Round(trgt, 2);
            }
            else if (_ay == 10)
            {
                float trgt = DB.targets.Where(a => a.IS_DELETED == false && a.STATION_ID == stationId).Select(a => a.OKT_PRODUCTION.Value).FirstOrDefault();
                return (float)Math.Round(trgt, 2);
            }
            else if (_ay == 11)
            {
                float trgt = DB.targets.Where(a => a.IS_DELETED == false && a.STATION_ID == stationId).Select(a => a.NOV_PRODUCTION.Value).FirstOrDefault();
                return (float)Math.Round(trgt, 2);
            }
            else if (_ay == 12)
            {
                float trgt = DB.targets.Where(a => a.IS_DELETED == false && a.STATION_ID == stationId).Select(a => a.DEC_PRODUCTION.Value).FirstOrDefault();
                return (float)Math.Round(trgt, 2);
            }
            return 0;
        }
    }
}