using Esso.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace Esso.Web.Controllers
{
    public class UnitController : BaseController
    {
        // GET: Unit
        EssoEntities DB = new EssoEntities();
        public ActionResult Index(int stationId)
        {
            return View(stationId);
        }

        public JsonResult UnitValues(int stationId)
        {
            // süleyman tfs kontrol
            Thread.CurrentThread.CurrentCulture = new CultureInfo("tr-TR");
            DateTime nowDate = DateTime.Now;
            int unitCount = DB.Inverters.Where(a => a.STATION_ID == stationId).Select(n => n.ID).Count();

            var unitsValue = DB.UnitSums.Where(w => w.STATION_ID == stationId).OrderByDescending(o => o.DATE_NUMBER).Take(unitCount).AsEnumerable().Select(s =>
                new TBL_UNIT_OZET_DTO
                {
                    INSERT_DATE = s.INSERT_DATE.ToString("dd.MM.yyyy HH:mm:ss"),
                    WP_MINUS_DAILY =s.WP_MINUS_DAILY,
                    WP_MINUS_TOTAL = s.WP_MINUS_TOTAL,
                    WP_PLUS_DAILY =s.WP_PLUS_DAILY,
                    WP_PLUS_TOTAL = s.WP_PLUS_TOTAL,
                    REACTIVE_POWER = s.REACTIVE_POWER,
                    VISIBLE_POWER = s.VISIBLE_POWER,
                    FREQUENCY = s.FREQUENCY,
                    IA = s.IA,
                    IB = s.IB,
                    IC = s.IC,
                    TEMPERATURE1 = s.TEMPERATURE1,
                    TEMPERATURE2 = s.TEMPERATURE2,
                    TEMPERATURE3 = s.TEMPERATURE3,
                    TEMPERATURE4 = s.TEMPERATURE4,
                    TEMPERATURE5 = s.TEMPERATURE5,
                    TEMPERATURE6 = s.TEMPERATURE6,
                    GEN_TEMPERATURE1 = s.GEN_TEMPERATURE1,
                    GEN_TEMPERATURE2 = s.GEN_TEMPERATURE2,
                    GEN_TEMPERATURE3 = s.GEN_TEMPERATURE3,
                    GEN_TEMPERATURE4 = s.GEN_TEMPERATURE4,
                    GEN_TEMPERATURE5 = s.GEN_TEMPERATURE5,
                    GEN_TEMPERATURE6 = s.GEN_TEMPERATURE6,
                    SPEED = s.SPEED,
                    VAB = s.VAB,
                    VBC = s.VBC,
                    VCA = s.VCA,
                    ACTIVE_POWER = s.ACTIVE_POWER,
                    UNIT_ID = s.UNIT_ID
                }).ToList();

            return Json(unitsValue.OrderBy(o => o.UNIT_ID), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetTag()
        {
            string[] columnTag = { "WP_MINUS_DAILY", "WP_MINUS_TOTAL", "WP_PLUS_DAILY", "WP_PLUS_TOTAL", "REACTIVE_POWER","ACTIVE_POWER","VISIBLE_POWER", "FREQUENCY",
                "IA", "IB", "IC", "TEMPERATURE1", "TEMPERATURE2", "TEMPERATURE3", "TEMPERATURE4","TEMPERATURE5","TEMPERATURE6",
                 "GEN_TEMPERATURE1", "GEN_TEMPERATURE2", "GEN_TEMPERATURE3", "GEN_TEMPERATURE4", "GEN_TEMPERATURE5", "GEN_TEMPERATURE6", "SPEED","VAB","VBC","VCA"
            };
            return Json(columnTag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetChartUnitDetail(string beginDate, int stationId)
        {
            try
            {
                NUMBER_FORMAT_DTO convertFormat = ConvertNumberFormat(beginDate);
                long _numberDateBegin = convertFormat._begin;
                long _numberDateEnd = convertFormat._end;
                Thread.CurrentThread.CurrentCulture = new CultureInfo("tr-TR");
                DateTime selectDate = DateTime.Parse(@beginDate);

                int[] ib = DB.Inverters.Where(a => a.STATION_ID == stationId).Select(n => n.ID).ToArray();
                var _count = ib.Count();
                UnitlistDTO invD = new UnitlistDTO();
                invD.units = (from s in DB.UnitSums
                                  where s.STATION_ID == stationId && ib.Contains(s.UNIT_ID)
                                  && s.DATE_NUMBER >= _numberDateBegin && s.DATE_NUMBER <= _numberDateEnd
                                  orderby s.DATE_NUMBER ascending
                                  select new TBL_UNIT_OZET_CHART_DTO
                                  {
                                      INSERT_DATE = s.INSERT_DATE,
                                      WP_MINUS_DAILY =s.WP_MINUS_DAILY,
                                      WP_MINUS_TOTAL = s.WP_MINUS_TOTAL,
                                      WP_PLUS_DAILY =s.WP_PLUS_DAILY,
                                      WP_PLUS_TOTAL = s.WP_PLUS_TOTAL,
                                      REACTIVE_POWER = s.REACTIVE_POWER,
                                      VISIBLE_POWER = s.VISIBLE_POWER,
                                      FREQUENCY = s.FREQUENCY,
                                      IA = s.IA,
                                      IB = s.IB,
                                      IC = s.IC,
                                      TEMPERATURE1 = s.TEMPERATURE1,
                                      TEMPERATURE2 = s.TEMPERATURE2,
                                      TEMPERATURE3 = s.TEMPERATURE3,
                                      TEMPERATURE4 = s.TEMPERATURE4,
                                      TEMPERATURE5 = s.TEMPERATURE5,
                                      TEMPERATURE6 = s.TEMPERATURE6,
                                      GEN_TEMPERATURE1 = s.GEN_TEMPERATURE1,
                                      GEN_TEMPERATURE2 = s.GEN_TEMPERATURE2,
                                      GEN_TEMPERATURE3 = s.GEN_TEMPERATURE3,
                                      GEN_TEMPERATURE4 = s.GEN_TEMPERATURE4,
                                      GEN_TEMPERATURE5 = s.GEN_TEMPERATURE5,
                                      GEN_TEMPERATURE6 = s.GEN_TEMPERATURE6,
                                      SPEED = s.SPEED,
                                      VAB = s.VAB,
                                      VBC = s.VBC,
                                      VCA = s.VCA,
                                      ACTIVE_POWER = s.ACTIVE_POWER,
                                      UNIT_ID = s.UNIT_ID
                                  }
                          ).ToList();

                return Json(invD);
            }
            catch (Exception ex)
            {
                return Json("{ err:" + ex.Message + "}", JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult GetUnitCount(int stationId)
        {
            var invs = DB.Inverters.Where(a => a.IS_DELETED == false && a.STATION_ID == stationId).OrderBy(a => a.ID).ToList();
            return Json(invs, JsonRequestBehavior.AllowGet);
        }
        public class UnitlistDTO
        {
            public List<TBL_UNIT_OZET_CHART_DTO> units { get; set; }
        }

        public class NUMBER_FORMAT_DTO
        {
            public long _begin { get; set; }
            public long _end { get; set; }
        }

        public NUMBER_FORMAT_DTO ConvertNumberFormat(string date)
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
            string _strDateBegin = _convertDate + "000000";
            string _strDateEnd = _convertDate + "235959";
            NUMBER_FORMAT_DTO ndto = new NUMBER_FORMAT_DTO();
            ndto._begin = Convert.ToInt64(_strDateBegin);
            ndto._end = Convert.ToInt64(_strDateEnd);
            return ndto;
        }
        public partial class TBL_UNIT_OZET_DTO
        {

            public string INSERT_DATE { get; set; }
            public Nullable<float> WP_MINUS_DAILY { get; set; }
            public Nullable<float> WP_MINUS_TOTAL { get; set; }
            public Nullable<float> WP_PLUS_DAILY { get; set; }
            public Nullable<float> WP_PLUS_TOTAL { get; set; }
            public Nullable<float> REACTIVE_POWER { get; set; }
            public Nullable<float> VISIBLE_POWER { get; set; }
            public Nullable<float> FREQUENCY { get; set; }
            public Nullable<float> IA { get; set; }
            public Nullable<float> IB { get; set; }
            public Nullable<float> IC { get; set; }
            public Nullable<float> TEMPERATURE1 { get; set; }
            public Nullable<float> TEMPERATURE2 { get; set; }
            public Nullable<float> TEMPERATURE3 { get; set; }
            public Nullable<float> TEMPERATURE4 { get; set; }
            public Nullable<float> TEMPERATURE5 { get; set; }
            public Nullable<float> TEMPERATURE6 { get; set; }
            public Nullable<float> GEN_TEMPERATURE1 { get; set; }
            public Nullable<float> GEN_TEMPERATURE2 { get; set; }
            public Nullable<float> GEN_TEMPERATURE3 { get; set; }
            public Nullable<float> GEN_TEMPERATURE4 { get; set; }
            public Nullable<float> GEN_TEMPERATURE5 { get; set; }
            public Nullable<float> GEN_TEMPERATURE6 { get; set; }
            public Nullable<float> SPEED { get; set; }
            public Nullable<float> VAB { get; set; }
            public Nullable<float> VBC { get; set; }
            public Nullable<float> VCA { get; set; }
            public Nullable<float> ACTIVE_POWER { get; set; }
            public int UNIT_ID { get; set; }

        }

        public partial class TBL_UNIT_OZET_CHART_DTO
        {

            public DateTime INSERT_DATE { get; set; }
            public Nullable<float> WP_MINUS_DAILY { get; set; }
            public Nullable<float> WP_MINUS_TOTAL { get; set; }
            public Nullable<float> WP_PLUS_DAILY { get; set; }
            public Nullable<float> WP_PLUS_TOTAL { get; set; }
            public Nullable<float> REACTIVE_POWER { get; set; }
            public Nullable<float> VISIBLE_POWER { get; set; }
            public Nullable<float> FREQUENCY { get; set; }
            public Nullable<float> IA { get; set; }
            public Nullable<float> IB { get; set; }
            public Nullable<float> IC { get; set; }
            public Nullable<float> TEMPERATURE1 { get; set; }
            public Nullable<float> TEMPERATURE2 { get; set; }
            public Nullable<float> TEMPERATURE3 { get; set; }
            public Nullable<float> TEMPERATURE4 { get; set; }
            public Nullable<float> TEMPERATURE5 { get; set; }
            public Nullable<float> TEMPERATURE6 { get; set; }
            public Nullable<float> GEN_TEMPERATURE1 { get; set; }
            public Nullable<float> GEN_TEMPERATURE2 { get; set; }
            public Nullable<float> GEN_TEMPERATURE3 { get; set; }
            public Nullable<float> GEN_TEMPERATURE4 { get; set; }
            public Nullable<float> GEN_TEMPERATURE5 { get; set; }
            public Nullable<float> GEN_TEMPERATURE6 { get; set; }
            public Nullable<float> SPEED { get; set; }
            public Nullable<float> VAB { get; set; }
            public Nullable<float> VBC { get; set; }
            public Nullable<float> VCA { get; set; }
            public Nullable<float> ACTIVE_POWER { get; set; }
            public int UNIT_ID { get; set; }

        }
    }
}