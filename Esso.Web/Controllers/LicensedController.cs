using AutoMapper;
using Esso.Data;
using Esso.Model.Models;
using Esso.Models;
using Esso.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Esso.Web.Models.DATE_NUMBER;

namespace Esso.Web.Controllers
{
    [Authorize]
    public class LicensedController : BaseController
    {
        // GET: Licensed
        EssoEntities DB = new EssoEntities();
        public ActionResult Index(int stationId)
        {
            return View(stationId);
        }
        public ActionResult SingleLine(int stationId)
        {
            return View(stationId);
        }
        public ActionResult SingleLine2(int stationId)
        {
            return View(stationId);
        }
        public ActionResult MeteorolojiDetail(int stationId)
        {
            return View(stationId);
        }
        public JsonResult GetLineChartMeteoroloji(string beginDate, int stationId)
        {
            DateTime reqDateParam = DateTime.Parse(beginDate);
            var met = DB.Summaries.Where(a => a.STATION_ID == stationId && a.tarih.Year == reqDateParam.Year
              && a.tarih.Month == reqDateParam.Month && a.tarih.Day == reqDateParam.Day)
              .Select(a => new Meteoroloji_DTO
              {
                  date = a.tarih,
                  irradiation = (float)Math.Round((float)a.isinim, 1),
                  pyranometer = (float)Math.Round((float)a.PYRANOMETER, 1) < 0 ? 0 : (float)Math.Round((float)a.PYRANOMETER, 1),
                  wind = (float)Math.Round((float)a.ruzgarHizi),
                  cell_temp = (float)Math.Round((float)a.hucreSicakligi),
                  external_temp = (float)Math.Round((float)a.sicaklik),
                  irradiation2 = (float)Math.Round((float)a.ISINIM_2, 1),
                  pyranometer2 = (float)Math.Round((float)a.PYRANOMETER_2, 1) < 0 ? 0 : (float)Math.Round((float)a.PYRANOMETER_2, 1),
                  wind2 = (float)Math.Round((float)a.RUZGARHIZI_2),
                  cell_temp2 = (float)Math.Round((float)a.HUCRESICAKLIGI_2),
                  external_temp2 = (float)Math.Round((float)a.SICAKLIK_2)
              }).OrderBy(a => a.date).ToList();
            return Json(met, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLineChartMeteoroloji2(string beginDate, int stationId)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("tr-TR");
            DateTime reqDateParam = DateTime.Parse(beginDate);
            var met = DB.Summaries.Where(a => a.STATION_ID == stationId && a.tarih.Year == reqDateParam.Year
              && a.tarih.Month == reqDateParam.Month && a.tarih.Day == reqDateParam.Day)
              .Select(a => new Meteoroloji_DTO2
              {
                  date = a.tarih,
                  ruzgar_yonu = (float)Math.Round((float)a.MEAN_WIND_DIRECTION_1, 1),
                  hava_sicakligi = (float)Math.Round((float)a.AIR_TEMPERATURE_1, 1),
                  bagil_nem = (float)Math.Round((float)a.RELATIVE_HUMIDITY_1),
                  mutlak_nem = (float)Math.Round((float)a.ABSOLUTE_HUMIDITY_1),
                  mutlak_hava_basinci = (float)Math.Round((float)a.ABSOLUTE_AIR_PRESSURE_1),
                  ruzgar_yonu2 = (float)Math.Round((float)a.MEAN_WIND_DIRECTION_2, 1),
                  hava_sicakligi2 = (float)Math.Round((float)a.AIR_TEMPERATURE_2, 1),
                  bagil_nem2 = (float)Math.Round((float)a.RELATIVE_HUMIDITY_2),
                  mutlak_nem2 = (float)Math.Round((float)a.ABSOLUTE_HUMIDITY_2),
                  mutlak_hava_basinci2 = (float)Math.Round((float)a.ABSOLUTE_AIR_PRESSURE_2)

              }).OrderBy(a => a.date).ToList();
            return Json(met, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetRainfall(string beginDate, int stationId)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("tr-TR");
            DateTime reqDateParam = DateTime.Parse(beginDate);
            var met = DB.Summaries.Where(a => a.STATION_ID == stationId && a.tarih.Year == reqDateParam.Year
              && a.tarih.Month == reqDateParam.Month && a.tarih.Day == reqDateParam.Day)
              .Select(a => new RainGraph_DTO
              {
                  date = a.tarih,
                  yagis_miktari = a.YAGIS_MIKTARI == null ? 0 : (float)Math.Round((float)a.YAGIS_MIKTARI, 1)


              }).OrderBy(a => a.date).ToList();
            return Json(met, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetHourlyAVG(int stationId, string slcDate)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("tr-TR");
            DateTime reqDateParam = DateTime.Parse(slcDate);
            var hourlyProduction = (from slc in DB.Summaries
                                    where
                                    slc.STATION_ID == stationId &&
                                    slc.tarih.Year == reqDateParam.Year && slc.tarih.Month == reqDateParam.Month && slc.tarih.Day == reqDateParam.Day
                                    orderby slc.tarih ascending
                                    select new Hour_DTO
                                    {
                                        _enerji = slc.Enerji == null ? 0 : slc.Enerji.Value,
                                        _uretilen_enerji = slc.H2_WP_minus == null ? 0 : Math.Round(slc.H2_WP_minus.Value, 2),
                                        _isinimToplam = slc.isinim == null ? 0 : slc.isinim.Value,
                                        _tarih = slc.tarih,

                                    })
                     .AsEnumerable()
                     .GroupBy(x => x._tarih.ToString("dd/MM/yyyy HH:00:00"))
                     .Select(g => new Hour_DTO
                     {
                         _enerji = Math.Round(g.Max(a => Math.Round(a._enerji / 1000000, 2)) - g.Min(a => Math.Round(a._enerji / 1000000, 2)), 2),
                         _uretilen_enerji = Math.Round(g.Max(a => Math.Round(a._uretilen_enerji, 2)) - g.Min(a => Math.Round(a._uretilen_enerji, 2)), 2),
                         //_isinimToplam= Math.Round(g.Max(a => Math.Round(a._isinimToplam, 2)) - g.Min(a => Math.Round(a._isinimToplam, 2)), 2),
                         _isinimToplam = Math.Round(g.Average(a => Math.Round(a._isinimToplam, 1)), 2),
                         _tarih = Convert.ToDateTime(g.Key),
                         _saat = Convert.ToDateTime(g.Key).Hour

                     }).ToList();
            return Json(hourlyProduction, JsonRequestBehavior.AllowGet);
        }

        public JsonResult EndProductionData(int stationId)
        {
            DateTime begindt = Convert.ToDateTime(DateTime.Now);
            var endData = DB.Summaries.Where(t => t.STATION_ID == stationId
            && t.tarih.Year == begindt.Year
            && t.tarih.Month == begindt.Month
            && t.tarih.Day == begindt.Day).OrderByDescending(a => a.tarih).FirstOrDefault();
            //List<EndProductionModel> q = Mapper.Map<List<TBL_OZET>, List<EndProductionModel>>(endData).OrderByDescending(a => a._tarih).Take(1).ToList();
            return Json(endData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult String(int stationId)
        {
            return View(stationId);
        }
        public class StringMin
        {
            public int ColumnIndex { get; set; }
            public int RowIndex { get; set; }

            public decimal Min { get; set; }
            public string FieldName { get; set; }

        }

        public class TempDTO
        {
            public int ID { get; set; }
            public string NAME { get; set; }
        }
        public class STR_DTO
        {
            public int ID { get; set; }
            public string NAME { get; set; }
            public DateTime date { get; set; }
            public long? dateNumber { get; set; }
            public float? VALUE { get; set; }

        }
        public ActionResult StringGridPartial(int stationId, string date, string hour)
        {
            List<TempDTO> strTagNames = DB.stationString
               .Join(DB.Tags, r => r.STRING_ID, ro => ro.ID, (r, ro) => new { r, ro })
               .Where(x => x.r.STATION_ID == stationId && x.r.DISPLAY_NAME.Contains("temp") == false && x.r.IS_DELETED == false)
              .GroupBy(x => x.r.DISPLAY_NAME)
              .Select(g => new TempDTO { NAME = g.Key, ID = g.FirstOrDefault().ro.ID })
              .ToList();

            strTagNames = strTagNames.OrderBy(x => x.NAME).ToList();

            DataTable dt = new DataTable();


            if (strTagNames == null || strTagNames.Count == 0)
            {
                return PartialView(dt);
            }


            List<string> sameTag = new List<string>();
            if (strTagNames != null)
            {
                string sck = string.Empty;


                foreach (TempDTO tag in strTagNames)
                {
                    sck = tag.NAME.Split('_')[0];


                    if (!dt.Columns.Contains(sck + " (A)"))
                    {
                        sck = sck.Replace("SCK", "DCB");

                        dt.Columns.Add("Name:" + sck, typeof(string))/*.SetOrdinal(0)*/;
                        dt.Columns.Add(sck + " (A)");

                        sameTag = strTagNames.Where(x => x.NAME.Split('_')[0] == sck).Select(x => x.NAME).ToList();

                        if (sameTag != null)
                        {
                            for (int i = 0; i <= sameTag.Count - 1; i++)
                            {
                                if (dt.Rows.Count == 0 || dt.Rows.Count < (i + 1))
                                {
                                    DataRow rw = dt.NewRow();
                                    rw["Name:" + sck] = sameTag[i];
                                    rw[sck + " (A)"] = sameTag[i];
                                    dt.Rows.Add(rw);

                                }
                                else
                                {
                                    dt.Rows[i]["Name:" + sck] = sameTag[i];
                                    dt.Rows[i][sck + " (A)"] = sameTag[i];
                                }
                            }

                        }
                    }

                }
            }
            Thread.CurrentThread.CurrentCulture = new CultureInfo("tr-TR");
            List<STR_DTO> values = new List<STR_DTO>();
            if (hour == "")
            {
                values = (from so in DB.StringOzetLive
                          join t in DB.Tags on so.STRING_ID equals t.ID
                          join tct in DB.stationString on t.ID equals tct.STRING_ID
                          where so.STATION_ID == stationId
                          && tct.STATION_ID == stationId
                          && tct.IS_DELETED == false
                          && t.IS_DELETED == false
                          && tct.IS_DELETED == false
                          select new STR_DTO
                          {
                              NAME = tct.DISPLAY_NAME,
                              ID = t.ID,
                              date = so.INSERT_DATE.Value,
                              VALUE = so.VALUE
                          }
                              ).ToList();
                ViewBag.Date = values[0].date;

            }
            else
            {
                DateTime slcDate = DateTime.Parse(date);
                DateTime nowDate = DateTime.Now;

                if (nowDate.Year == slcDate.Year && nowDate.Month == slcDate.Month) //Quarter String
                {
                    string _date = date + " " + hour;
                    ViewBag.Date = _date;
                    string[] hourSplit = hour.Split(':');
                    var _convertDate = ConvertNumberFormatDateAndHourQuarter(date, hourSplit[0], hourSplit[1])._begin;
                    values = (from so in DB.StringOzetQuarterHourAVG
                              join t in DB.Tags on so.STRING_ID equals t.ID
                              join tct in DB.stationString on t.ID equals tct.STRING_ID
                              where so.STATION_ID == stationId && tct.STATION_ID == stationId
                              && tct.IS_DELETED == false
                              && t.IS_DELETED == false
                              && so.TARIH_NUMBER == _convertDate
                              select new STR_DTO
                              {
                                  NAME = tct.DISPLAY_NAME,
                                  ID = t.ID,
                                  VALUE = (float)Math.Round(so.VALUE, 2)
                              }
                                ).ToList();
                }
                else //Hourly String
                {
                    string _date = date + " " + hour;
                    ViewBag.Date = _date;
                    string[] hourSplit = hour.Split(':');
                    var _convertDate = ConvertNumberFormatDateAndHour(date, hourSplit[0])._begin;
                    values = (from so in DB.StringOzetAVG
                              join t in DB.Tags on so.STRING_ID equals t.ID
                              join tct in DB.stationString on t.ID equals tct.STRING_ID
                              where so.STATION_ID == stationId && tct.STATION_ID == stationId
                              && tct.IS_DELETED == false
                              && t.IS_DELETED == false
                              && so.TARIH_NUMBER == _convertDate
                              select new STR_DTO
                              {
                                  NAME = tct.DISPLAY_NAME,
                                  ID = t.ID,
                                  VALUE = (float)Math.Round(so.VALUE, 2)
                              }
                                ).ToList();
                }
            }
            if (values.Count != 0)
            {
                float? _totalDCcurrent = values.Sum(sm => sm.VALUE);
                ViewBag.StringDCSum = _totalDCcurrent;
                if (dt.Columns.Count > 0 && dt.Rows.Count > 0 && values != null)
                {
                    string cellTagName = string.Empty;
                    foreach (DataRow rw in dt.Rows)
                    {
                        foreach (DataColumn col in dt.Columns)
                        {
                            cellTagName = rw[col].ToString();

                            if (values.Any(x => x.NAME == cellTagName))
                            {
                                rw[col] = values.Where(x => x.NAME == cellTagName).FirstOrDefault().VALUE;
                            }
                            //else
                            //{
                            //	rw[col] = "-";
                            //}

                        }
                    }
                }
            }
            else
            {
                string cellTagName = string.Empty;
                foreach (DataRow rw in dt.Rows)
                {
                    foreach (DataColumn col in dt.Columns)
                    {

                        cellTagName = rw[col].ToString();
                        rw[col] = "-";

                    }
                }
            }


            if (values.Count != 0)
            {
                decimal _min;

                List<StringMin> _ListStringMin = new List<StringMin>();

                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    _min = Convert.ToDecimal(dt.Rows[0][j]);

                    StringMin _StringMin = new StringMin();

                    _StringMin.ColumnIndex = j;
                    _StringMin.FieldName = dt.Columns[j].ColumnName;

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {

                        if (dt.Rows[i][j] != DBNull.Value)
                        {
                            if (Convert.ToDecimal(dt.Rows[i][j]) < _min)
                            {
                                _min = Convert.ToDecimal(dt.Rows[i][j]);
                                _StringMin.Min = _min;
                                _StringMin.RowIndex = i;
                            }


                        }

                    }

                    _ListStringMin.Add(_StringMin);
                }

                ViewBag.StringMin = _ListStringMin;

                // Max değeri bulma -----------------------------
                decimal _max = 0;

                List<StringDeneme> _ListStringMax = new List<StringDeneme>();

                for (int j = 1; j < dt.Columns.Count; j++)
                {
                    _max = 0;

                    StringDeneme _cStringDeneme = new StringDeneme();

                    _cStringDeneme.ColumnIndex = j;
                    _cStringDeneme.FieldName = dt.Columns[j].ColumnName;

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (dt.Rows[i][j] != DBNull.Value)
                        {

                            if (Convert.ToDecimal(dt.Rows[i][j]) > _max)
                            {
                                _max = Convert.ToDecimal(dt.Rows[i][j]);
                                _cStringDeneme.Value = _max;
                                _cStringDeneme.RowIndex = i;
                            }
                        }
                    }

                    j++;

                    _ListStringMax.Add(_cStringDeneme);
                }

                ViewBag.StringMax = _ListStringMax;


                // max bitti--------

                // Avg değeri bulma ----------------------

                var ListStringcategory = new List<string>();
                //List<InvAvg> ListStringcategory = new List<InvAvg>();

                InvAvg ListInvAvg = new InvAvg();
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    decimal count = 0;
                    decimal sum = 0;

                    if (dt.Columns[i].ColumnName.IndexOf("Name") == -1)
                    {
                        if (!ListStringcategory.Contains(dt.Columns[i].ColumnName.Substring(0, 5)))
                        {

                            ListStringcategory.Add(dt.Columns[i].ColumnName.Substring(0, 5));
                        }
                    }
                }
                List<StringAvg> ListStringAvg = new List<StringAvg>();


                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    decimal count = 0, sum = 0;

                    if (dt.Columns[i].ColumnName.IndexOf("Name") == -1)
                    {
                        for (int j = 0; j < dt.Rows.Count; j++)
                        {

                            decimal _ActiveValue = 0;

                            if (dt.Rows[j][i] != DBNull.Value)
                                _ActiveValue = Convert.ToDecimal(dt.Rows[j][i]);

                            if (_ActiveValue > 0)
                            {
                                sum += _ActiveValue;

                                count++;
                            }
                        }

                        StringAvg _StrAvg = new StringAvg();

                        if (count != 0)
                            _StrAvg.Avg = decimal.Round(sum / count, 2);
                        else
                            _StrAvg.Avg = 0;

                        _StrAvg.FieldName = dt.Columns[i].ColumnName;
                        _StrAvg.ColumnIndex = i;
                        //   
                        ListStringAvg.Add(_StrAvg);

                        foreach (var item in ListStringcategory)
                        {
                            if (_StrAvg.FieldName.Contains(item))
                            {

                                ListInvAvg.Avg = ListInvAvg.Avg + _StrAvg.Avg;
                                ListInvAvg.AvgCount = ListInvAvg.AvgCount + 1;
                            }

                        }

                    }

                }
                ViewBag.StringAvg = ListStringAvg;

                //Inverter Toplam
                var invGroup = (from u in ListStringAvg
                                group u by u.FieldName.Substring(0, 4) into g
                                select new StringInvNameAvg { invName = "Inverter " + g.Key.Substring(g.Key.Length - 1), Avg = Math.Round(g.Average(oi => oi.Avg), 2) })
                                .ToList();

                ViewBag.ListInvAVG = invGroup;
                // avg bitti-----------------------------------------


                foreach (TempDTO tag in strTagNames)
                {
                    string sck = string.Empty;
                    sck = tag.NAME.Split('_')[0];

                    sameTag = strTagNames.Where(x => x.NAME.Split('_')[0] == sck).Select(x => x.NAME).ToList();

                    if (sameTag != null)
                    {
                        for (int i = 0; i <= sameTag.Count - 1; i++)
                        {
                            string[] tttt = sameTag[i].Split('_');

                            dt.Rows[i]["Name:" + sck] = tttt[1] + " " + tttt[2] + " " + tttt[3];

                        }

                    }
                }

                //TempDTO mt = strTagNames.AsEnumerable()
                //		.GroupBy(r => r.NAME.Split('_')[0])
                //		.Select(g => new TempDTO { NAME = g.Key, ID = g.Count() })//(g => new TempDTO{ NAME = g.OrderByDescending(r => r.NAME.Split('_')[0]  } )
                //		.OrderByDescending(x => x.ID).ToList().First();

                //List<string> tagCOl = strTagNames.Where(x => x.NAME.StartsWith(mt.NAME + "_")).Select(X => X.NAME).ToList();

                var aaa = strTagNames.AsEnumerable()
                        .GroupBy(r => r.NAME.Split('_')[0])
                        .Select(g => new TempDTO { NAME = g.Key, ID = g.Count() })//(g => new TempDTO{ NAME = g.OrderByDescending(r => r.NAME.Split('_')[0]  } )
                        .OrderByDescending(x => x.ID).ToList();



                List<string> listt = new List<string>();

                foreach (var a in aaa)
                {
                    List<string> tagCOl = strTagNames.Where(x => x.NAME.StartsWith(a.NAME + "_")).Select(X => X.NAME).ToList();

                    var fff = strTagNames.Where(x => x.NAME.StartsWith(a.NAME + "_")).Select(X => X.NAME).FirstOrDefault();
                    string ddd = string.Empty;
                    ddd = fff.Split('_')[0];

                    for (int i = 0; i <= 0 - 1; i++)
                    {
                        dt.Columns.Add(ddd, typeof(string)).SetOrdinal(i);

                        for (int t = 0; t <= tagCOl.Count - 1; t++)
                        {
                            if (dt.Rows.Count < t)
                            {
                                break;
                            }
                            string[] ccc = tagCOl[t].Split('_');

                            if (tagCOl[t].Contains("temp"))
                            {
                                dt.Rows[t]["Tag"] = "Temp.";
                            }
                            else
                            {
                                dt.Rows[t][ddd] = ccc[1] + " " + ccc[2] + "-" + ccc[3] /*+ " " + ccc[4]*/;
                            }

                        }
                    }
                }

            }
            //ViewData["columns"] = columns;

            //sveri çekilecek.
            return PartialView(dt);
        }

        public class StringDeneme
        {
            public int ColumnIndex { get; set; }
            public int RowIndex { get; set; }
            public string FieldName { get; set; }
            public decimal Value { get; set; }
        }
        public class StringAvg
        {
            public int ColumnIndex { get; set; }
            public int RowIndex { get; set; }

            public decimal Avg { get; set; }
            public string FieldName { get; set; }

        }
        public class StringInvNameAvg
        {
            public string invName { get; set; }
            public decimal Avg { get; set; }
        }
        public class InvAvg
        {
            public int ColumnIndex { get; set; }
            public int RowIndex { get; set; }

            public decimal Avg { get; set; }
            public string FieldName { get; set; }
            public decimal AvgOrt { get; set; }

            public decimal AvgCount { get; set; }
        }
        public JsonResult HourlyColorReport(int stationId, string slctDate)
        {
            StringPerformanceView _strModel = new StringPerformanceView();
            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("tr-TR");
                DateTime date = DateTime.Parse(@slctDate);
                DateTime curDate = DateTime.Now;
                _strModel.strModel = new StringModel();
                List<String_Hour_DTO> values = new List<String_Hour_DTO>();
                if (curDate.Date == date.Date)
                {
                    values = DB.VWStringOzetDaily.Where(p => p.STATION_ID == stationId).Select(a =>
                              new String_Hour_DTO
                              {
                                  NAME = a.DISPLAY_NAME,
                                  ID = a.STRING_ID,
                                  date = a.INSERT_DATE,
                                  VALUE = a.VALUE,
                              }
                              ).ToList();
                    var stringlist1 = values.OrderBy(a => a.NAME)
                           .Select(a => a.NAME)
                           .Distinct()
                           .ToList();

                    foreach (var item in stringlist1)
                    {
                        _strModel.strModel.StringList.Add(item.Substring(8, 6));
                    }

                    var list1_ = new List<Values>();
                    var groupList1 = values.OrderBy(a => a.NAME).GroupBy(a => a.NAME).ToList();

                    string[] groupTime = values.GroupBy(grp => grp.date).Select(a => a.Key.Split(' ')[1]).ToArray();

                    int endHour = int.Parse(groupTime[groupTime.Length - 1]);
                    for (int i = 0; i < groupTime.Length; i++)
                    {
                        _strModel.strModel.Hours.Add(groupTime[i]);
                    }

                    for (int i = endHour + 1; i < 21; i++)
                    {
                        if (!groupTime.Contains(i.ToString()))
                        {
                            _strModel.strModel.Hours.Add(i.ToString());
                        }
                    }

                    foreach (var item in groupList1)
                    {
                        var list1 = new Values();
                        foreach (var i in item.OrderBy(a => a.date).GroupBy(grp => grp.date).ToList())
                        {
                            list1.values.Add((float)Math.Round((float)i.Max(a => a.VALUE), 2));
                        }
                        list1_.Add(list1);
                    }

                    _strModel.strModel.series = list1_.ToList();
                    _strModel.ErrorMessage = "";
                }
                else
                {
                    NUMBER_FORMAT_DTO convertFormat = ConvertNumberFormatHour(slctDate);
                    long _numberDateBegin = convertFormat._begin;
                    long _numberDateEnd = convertFormat._end;
                    values = (from u in DB.StringOzetAVG
                              join v in DB.stationString on u.STRING_ID equals v.STRING_ID
                              where v.IS_DELETED == false
                              && v.STATION_ID == stationId
                              && u.STATION_ID == stationId
                              && _numberDateBegin < u.TARIH_NUMBER && _numberDateEnd > u.TARIH_NUMBER
                              orderby u.TARIH_NUMBER ascending
                              select new String_Hour_DTO
                              {
                                  NAME = v.DISPLAY_NAME,
                                  ID = u.STRING_ID,
                                  TARIH_NUMBER = u.TARIH_NUMBER.Value,
                                  VALUE = u.VALUE
                              }).ToList();
                    var stringNameList = values.OrderBy(a => a.NAME)
                           .Select(a => a.NAME)
                           .Distinct()
                           .ToList();

                    foreach (var item in stringNameList)
                    {
                        _strModel.strModel.StringList.Add(item.Substring(8, 6));
                    }

                    var valueList = new List<Values>();

                    string[] groupTime = values.GroupBy(grp => grp.TARIH_NUMBER).OrderBy(a => a.Key).Select(a => a.Key.ToString().Substring(8, 2)).ToArray();
                    int endHour = int.Parse(groupTime[groupTime.Length - 1]);
                    for (int i = 0; i < groupTime.Length; i++)
                    {
                        _strModel.strModel.Hours.Add(groupTime[i]);
                    }

                    for (int i = endHour + 1; i < 21; i++)
                    {
                        if (!groupTime.Contains(i.ToString()))
                        {
                            _strModel.strModel.Hours.Add(i.ToString());
                        }
                    }

                    var groupList = values.OrderBy(a => a.NAME).GroupBy(a => a.NAME).ToList();
                    foreach (var item in groupList)
                    {
                        var listvalue = new Values();
                        foreach (var i in item.GroupBy(grp => grp.TARIH_NUMBER).OrderBy(a => a.Key).ToList())
                        {
                            listvalue.values.Add((float)Math.Round(i.Max(a => a.VALUE), 2));
                        }
                        valueList.Add(listvalue);
                    }

                    _strModel.strModel.series = valueList.ToList();
                    _strModel.ErrorMessage = "";
                }

            }
            catch (Exception ex)
            {
                _strModel.ErrorMessage = ex.ToString();
            }

            return Json(_strModel);

        }

        public JsonResult HourlyColorReport2(int stationId, string slctDate)
        {
            StringPerformanceView _strModel = new StringPerformanceView();
            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("tr-TR");
                DateTime date = DateTime.Parse(@slctDate);
                DateTime curDate = DateTime.Now;
                _strModel.strModel = new StringModel();
                List<String_Hour_DTO> values = new List<String_Hour_DTO>();
                if (curDate.Date.Year == date.Date.Year && curDate.Date.Month == date.Date.Month)
                {
                    NUMBER_FORMAT_DTO convertFormat = ConvertNumberFormatMinute(slctDate);
                    long _numberDateBegin = convertFormat._begin;
                    long _numberDateEnd = convertFormat._end;
                    values = (from u in DB.StringOzetQuarterHourAVG
                              join v in DB.stationString on u.STRING_ID equals v.STRING_ID
                              where v.IS_DELETED == false
                              && v.STATION_ID == stationId
                              && u.STATION_ID == stationId
                              && _numberDateBegin <= u.TARIH_NUMBER && _numberDateEnd >= u.TARIH_NUMBER
                              orderby u.TARIH_NUMBER ascending
                              select new String_Hour_DTO
                              {
                                  NAME = v.DISPLAY_NAME,
                                  ID = u.STRING_ID,
                                  TARIH_NUMBER = u.TARIH_NUMBER.Value,
                                  VALUE = u.VALUE
                              }).ToList();
                    var stringNameList = values.OrderBy(a => a.NAME)
                           .Select(a => a.NAME)
                           .Distinct()
                           .ToList();

                    foreach (var item in stringNameList)
                    {
                        _strModel.strModel.StringList.Add(item.Substring(8, 7).Replace("_", ""));
                    }

                    var valueList = new List<Values>();

                    string[] groupTime = values.GroupBy(grp => grp.TARIH_NUMBER).OrderBy(a => a.Key).Select(a => a.Key.ToString().Substring(8, 4)).ToArray();
                    int endHour = int.Parse(groupTime[groupTime.Length - 1]);
                    DateTime EndDate = DateTime.Now.Date;
                    TimeSpan tEndDate = new TimeSpan((int.Parse(groupTime[groupTime.Length - 1].Substring(0, 2))), (int.Parse(groupTime[groupTime.Length - 1].Substring(2, 2))), 0);
                    EndDate = EndDate.Date + tEndDate;

                    DateTime LastDate = EndDate;
                    TimeSpan tLastDate = new TimeSpan(21, 0, 0);
                    LastDate = LastDate.Date + tLastDate;

                    for (int i = 0; i < groupTime.Length; i++)
                    {
                        _strModel.strModel.Hours.Add(groupTime[i]);
                    }

                    while (EndDate < LastDate)
                    {
                        EndDate = EndDate.AddMinutes(15);
                        _strModel.strModel.Hours.Add(HourMinuteFormat(EndDate));
                    }

                    var groupList = values.OrderBy(a => a.NAME).GroupBy(a => a.NAME).ToList();
                    foreach (var item in groupList)
                    {
                        var listvalue = new Values();
                        foreach (var i in item.GroupBy(grp => grp.TARIH_NUMBER).OrderBy(a => a.Key).ToList())
                        {
                            listvalue.values.Add((float)Math.Round(i.Max(a => a.VALUE), 2));
                        }
                        valueList.Add(listvalue);
                    }

                    _strModel.strModel.series = valueList.ToList();
                    _strModel.ErrorMessage = "";
                }
                else
                {
                    NUMBER_FORMAT_DTO convertFormat = ConvertNumberFormatHour(slctDate);
                    long _numberDateBegin = convertFormat._begin;
                    long _numberDateEnd = convertFormat._end;
                    values = (from u in DB.StringOzetAVG
                              join v in DB.stationString on u.STRING_ID equals v.STRING_ID
                              where v.IS_DELETED == false
                              && v.STATION_ID == stationId
                              && u.STATION_ID == stationId
                              && _numberDateBegin < u.TARIH_NUMBER && _numberDateEnd > u.TARIH_NUMBER
                              orderby u.TARIH_NUMBER ascending
                              select new String_Hour_DTO
                              {
                                  NAME = v.DISPLAY_NAME,
                                  ID = u.STRING_ID,
                                  TARIH_NUMBER = u.TARIH_NUMBER.Value,
                                  VALUE = u.VALUE
                              }).ToList();
                    var stringNameList = values.OrderBy(a => a.NAME)
                           .Select(a => a.NAME)
                           .Distinct()
                           .ToList();

                    foreach (var item in stringNameList)
                    {
                        _strModel.strModel.StringList.Add(item.Substring(8, 7).Replace("_", ""));
                    }

                    var valueList = new List<Values>();

                    string[] groupTime = values.GroupBy(grp => grp.TARIH_NUMBER).OrderBy(a => a.Key).Select(a => a.Key.ToString().Substring(8, 2)).ToArray();
                    int endHour = int.Parse(groupTime[groupTime.Length - 1]);
                    for (int i = 0; i < groupTime.Length; i++)
                    {
                        _strModel.strModel.Hours.Add(groupTime[i] + "00");
                    }

                    for (int i = endHour + 1; i < 21; i++)
                    {
                        if (!groupTime.Contains(i.ToString()))
                        {
                            _strModel.strModel.Hours.Add(i.ToString() + "00");
                        }
                    }

                    var groupList = values.OrderBy(a => a.NAME).GroupBy(a => a.NAME).ToList();
                    foreach (var item in groupList)
                    {
                        var listvalue = new Values();
                        foreach (var i in item.GroupBy(grp => grp.TARIH_NUMBER).OrderBy(a => a.Key).ToList())
                        {
                            listvalue.values.Add((float)Math.Round(i.Max(a => a.VALUE), 2));
                        }
                        valueList.Add(listvalue);
                    }

                    _strModel.strModel.series = valueList.ToList();
                    _strModel.ErrorMessage = "";
                }

            }
            catch (Exception ex)
            {
                _strModel.ErrorMessage = ex.ToString();
            }

            return Json(_strModel);

        }

        //public class NUMBER_FORMAT_DTO
        //{
        //    public long _begin { get; set; }
        //    public long _end { get; set; }
        //}

        public NUMBER_FORMAT_DTO ConvertNumberFormatHour(string date)
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

        public NUMBER_FORMAT_DTO ConvertNumberFormatMinute(string date)
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
        public string HourMinuteFormat(DateTime dt)
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
        public NUMBER_FORMAT_DTO ConvertNumberFormatDateAndHour(string date, string hour)
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
            string _strDateBegin = _convertDate + hour;
            NUMBER_FORMAT_DTO ndto = new NUMBER_FORMAT_DTO();
            ndto._begin = Convert.ToInt64(_strDateBegin);
            return ndto;
        }
        public NUMBER_FORMAT_DTO ConvertNumberFormatDateAndHourQuarter(string date, string hour, string minute)
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
            string _strDateBegin = _convertDate + hour + minute;
            NUMBER_FORMAT_DTO ndto = new NUMBER_FORMAT_DTO();
            ndto._begin = Convert.ToInt64(_strDateBegin);
            return ndto;
        }
    }
}