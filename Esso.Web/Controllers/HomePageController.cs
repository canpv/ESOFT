using Esso.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Esso.Models;
using System.Threading;
using System.Globalization;
using System.Data.Entity.SqlServer;
using System.Xml;
using System.Data;
using Esso.Web.Helpers;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Esso.Web.ViewModels;
using System.Net;
using Esso.Web.Models;

namespace Esso.Web.Controllers
{
    public class HomePageController : BaseController
    {
        EssoEntities DB = new EssoEntities();
        // GET: HomePage
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult GetUSDValue()
        {
            var dolarKuru = Helpers.DovizKuru.GetEndDataBuyingUSD();
            return Json(dolarKuru, JsonRequestBehavior.AllowGet);
        }
        public ActionResult test()
        {
            WeatherAPI wapi = new WeatherAPI("manisa");
            var _Temp = wapi.GetMeteoData();
            return View();
        }
       
        public JsonResult GetAllStation(string date)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("tr-TR");
            DateTime nowDate = new DateTime();
            if (date == "today")
            {
                nowDate = DateTime.Now;
            }
            else if (date == "yesterday")
            {
                nowDate = DateTime.Now.AddDays(-1);
            }
            else if (date == "week")
            {
                nowDate = DateTime.Now;
            }
            else if (date == "month")
            {
                nowDate = DateTime.Now;
            }
            else
            {
                string[] year_month = date.Split('-');
                int _year = Convert.ToInt32(year_month[0]);
                int _month = Convert.ToInt32(year_month[1]);
                nowDate = new DateTime(_year, _month, 1);
            }

            var userId = User.Identity.GetUserId();
            List<TBL_STATION> stations = new List<TBL_STATION>();
            List<STATION_GRUP_COMPANY> stations_dto = new List<STATION_GRUP_COMPANY>();
            bool isDemo = false;
            if (User.IsInRole("M_ADMIN"))
            {
                stations = DB.Stations.Where(a => a.IS_DELETED == false && a.IS_ACTIVE == true && a.STATION_TYPE != 4).ToList();
            }
            else if (User.IsInRole("COMP_ADMIN"))
            {
                int[] ib = DB.CompanyUsers.Where(a => a.USER_ID == userId && a.IS_DELETED == false).Select(a => a.COMPANY_ID).ToArray();
                stations = DB.Stations.Where(a => a.IS_DELETED == false && a.IS_ACTIVE == true && ib.Contains(a.COMPANY_ID) && a.STATION_TYPE != 4).ToList();
            }
            else if (User.IsInRole("COMP_USER"))
            {
                int[] ibs = DB.StationUsers.Where(a => a.USER_ID == userId && a.IS_DELETED == false).Select(a => a.STATION_ID).ToArray();
                stations = DB.Stations.Where(a => a.IS_DELETED == false && a.IS_ACTIVE == true && ibs.Contains(a.ID) && a.STATION_TYPE != 4).ToList();
            }
            else if (User.IsInRole("DEMO"))
            {
                isDemo = true;
                int[] ibs = DB.StationUsers.Where(a => a.USER_ID == userId && a.IS_DELETED == false).Select(a => a.STATION_ID).ToArray();
                stations = DB.Stations.Where(a => a.IS_DELETED == false && a.IS_ACTIVE == true && ibs.Contains(a.ID) && a.STATION_TYPE != 4).ToList();
            }

            bool? money = false;
            ApplicationUser usr = DB.Database.SqlQuery<ApplicationUser>("select \"AspNetUsers\".\"Id\",\"AspNetUsers\".\"UserName\",\"AspNetUsers\".\"Email\",\"AspNetUsers\".\"SHOW_MONEY\",\"AspNetUsers\".\"REPORT_SEND_MAIL\",\"AspNetUsers\".\"ALARM_SEND_MAIL\" from \"AspNetUsers\" where \"AspNetUsers\".\"IS_DELETED\" = 0  and \"AspNetUsers\".\"Id\" = '" + userId + "'")
                                   .Select(a =>
                                   new ApplicationUser
                                   {
                                       Id = a.Id,
                                       UserName = a.UserName,
                                       Email = a.Email,
                                       SHOW_MONEY = a.SHOW_MONEY,
                                       REPORT_SEND_MAIL = a.REPORT_SEND_MAIL,
                                       ALARM_SEND_MAIL = a.ALARM_SEND_MAIL
                                   }).OrderBy(a => a.UserName).FirstOrDefault();

            if (usr.SHOW_MONEY != null)
            {
                if (usr.SHOW_MONEY.Value == 1)
                {
                    money = true;
                }
                else
                {
                    money = false;
                }
            }

            List<STATION_GRUP_COMPANY> sgcList = new List<STATION_GRUP_COMPANY>();
            if (date == "today")
            {
                int[] stationIds = stations.Select(s => s.ID).ToArray();
                try
                {


                    List<STATION_GRUP_COMPANY> stationSummary = (from v in DB.Stations
                                                                 join grp in DB.StationGroups on v.GROUP_ID equals grp.ID
                                                                 join comp in DB.Companies on v.COMPANY_ID equals comp.ID
                                                                 join u in DB.stationSummary on v.ID equals u.STATION_ID
                                                                 into abc
                                                                 where stationIds.Contains(v.ID)
                                                                 from b in abc.DefaultIfEmpty()
                                                                 orderby v.NAME ascending
                                                                 select new STATION_GRUP_COMPANY
                                                                 {
                                                                     STATION_ID = (b == null ? v.ID : b.STATION_ID),
                                                                     DATE = b.DATE.Value == null ? DateTime.Now : b.DATE.Value,
                                                                     STATION_NAME = (isDemo == true ? v.DEMO_NAME : v.NAME),
                                                                     DEMO_STATION_NAME = v.DEMO_NAME,
                                                                     COMPANY_ID = comp.ID,
                                                                     COMPANY_GROUP_NAME = (isDemo == true ? comp.DEMO_NAME + " / " + grp.DEMO_NAME : comp.NAME + " / " + grp.NAME),
                                                                     GROUP_ID = (b == null ? grp.ID : b.GROUP_ID),
                                                                     GROUP_NAME = (grp.NAME == null ? "" : grp.NAME),
                                                                     ACTIVE_INV_COUNT = (b.ALARM_COUNT == null ? 0 : b.ALARM_COUNT.Value),
                                                                     ENERGY = ((float)Math.Round(b.ENERGY.Value, 2)),
                                                                     IRRADIATION = ((float)Math.Round(b.IRRADIATION.Value, 1)),
                                                                     //CON_STATUS = (b.COMMINCATION == false ? true : false),
                                                                     DAILY_PRODUCTION = ((float)Math.Round(b.DAILY_PRODUCTION.Value / 1000, 1)),
                                                                     DC_INSTALLED_POWER = v.DC_INSTALLED_POWER,
                                                                     PR = (b.PR == null ? 0 : (float)Math.Round(b.PR.Value, 1)),
                                                                     ALARM_COUNT = 0,
                                                                     FINANCIAL_USD = ((float)Math.Round(b.FINANCIAL_INCOME.Value, 2)),
                                                                     INVERTER_ACTIVE_COUNT = (b.PASIVE_INV_COUNT == null ? 0 : b.INV_COUNT - b.PASIVE_INV_COUNT.Value),
                                                                     INVERTER_COUNT = (b.INV_COUNT == null ? 0 : b.INV_COUNT),
                                                                     IS_MONEY = money,
                                                                     SPECIFIC_YIELD = ((float)Math.Round(b.SPESIFIC_YIELD.Value, 2)),
                                                                     IS_METEOROLOGY = (v.IS_METEOROLOGY == null ? false : v.IS_METEOROLOGY.Value),
                                                                     IS_ALARM = (b.ALARM_COUNT == 0 || b.ALARM_COUNT == null ? false : true),
                                                                     INV_ERROR = (b.PASIVE_INV_COUNT > 0 ? true : false),
                                                                     STATION_TYPE=v.STATION_TYPE
                                                                 }).ToList();

                    var ConnectionList = (DB.AlarmStatus.Where(a => a.STATUS != 2 && a.END_DATE == null
                                   ).OrderByDescending(a => a.START_DATE).ToList());

                    var alarmDescList = DB.AlarmDesc.ToList();

                    List<STATION_GRUP_COMPANY> stationSummaryList = new List<STATION_GRUP_COMPANY>();
                    foreach (var st in stationSummary)
                    {
                        var isConnection = ConnectionList.Where(a => a.STATION_ID == st.STATION_ID
                                  && a.ERROR_NUMBER == "0001" && a.STATUS != 2 && a.END_DATE == null
                                  ).OrderByDescending(a => a.START_DATE).Take(1).FirstOrDefault();

                        var alarmCount = (from a1 in ConnectionList
                                          join a2 in alarmDescList on a1.ERROR_NUMBER equals a2.ERROR_NUMBER
                                          where a1.STATION_ID==st.STATION_ID && a1.STATUS!=2 && a1.END_DATE==null && a2.TYPE==1
                                          select a1).ToList().Count();

                        if (isConnection != null)
                        {
                            st.CON_STATUS = false;
                            st.IS_ALARM = true;
                        }
                        else
                        {
                            st.CON_STATUS = true;
                        }

                        if (alarmCount > 0)
                        {
                            st.ALARM_COUNT = alarmCount;
                            st.IS_ALARM = true;
                        }

                        if (st.DATE.Date != nowDate.Date)
                        {
                            st.ENERGY = null;
                            st.FINANCIAL_TL = null;
                            st.FINANCIAL_USD = null;
                            st.SPECIFIC_YIELD = null;
                            st.DAILY_PRODUCTION = null;
                            st.IRRADIATION = null;
                            st.PR = 0;
                        }

                        stationSummaryList.Add(st);
                    }

                    sgcList = stationSummaryList;
                }
                catch (Exception ex)
                {

                }
            }
            else
            {
                stations_dto = stations.Where(a => a.IS_DELETED == false && a.IS_ACTIVE == true).Select(a =>
                   new STATION_GRUP_COMPANY
                   {
                       STATION_NAME = a.NAME.ToString(),
                       STATION_ID = a.ID,
                       GROUP_ID = a.GROUP_ID,
                       COMPANY_ID = a.COMPANY_ID,
                       DEMO_STATION_NAME = a.DEMO_NAME,
                       DC_INSTALLED_POWER = a.DC_INSTALLED_POWER,
                       EXCHANGE_RATE = a.EXCHANGE_RATE,
                       IS_METEOROLOGY = a.IS_METEOROLOGY == null ? false : a.IS_METEOROLOGY.Value,
                       STATION_TYPE = a.STATION_TYPE
                   }).OrderBy(a => a.STATION_NAME).ToList();
                var compList = DB.Companies.Where(a => a.IS_DELETED == false).ToList();
                var groupList = DB.StationGroups.Where(a => a.IS_DELETED == false).ToList();
                foreach (var item in stations_dto)
                {
                    STATION_GRUP_COMPANY sgc = new STATION_GRUP_COMPANY();
                    TBL_PR_OZET prOzet = new TBL_PR_OZET();
                    List<TBL_PR_OZET> prOzetList = new List<TBL_PR_OZET>();
                    float? _specificYield = 0;

                    if (date == "yesterday")
                    {
                        prOzet = DB.PRSum.Where(a => a.STATION_ID == item.STATION_ID
                          && a.date.Value.Year == nowDate.Year
                          && a.date.Value.Month == nowDate.Month
                          && a.date.Value.Day == nowDate.Day).FirstOrDefault();

                        _specificYield = prOzet == null ? 0 : (float)prOzet.enerji.Value * 1000 / item.DC_INSTALLED_POWER;
                        if (prOzet == null)
                        {
                            sgc.PR = 0;
                            sgc.IRRADIATION = null;
                            sgc.DAILY_PRODUCTION = null;
                            sgc.ENERGY = 0;
                        }
                        else
                        {
                            sgc.PR = prOzet.pr == null ? 0 : (float)Math.Round(prOzet.pr.Value, 1);
                            sgc.IRRADIATION = prOzet.isinim_ortalama == null ? 0 : (float)Math.Round(prOzet.isinim_ortalama.Value, 1);
                            if (date == "today")
                            {
                                sgc.DAILY_PRODUCTION = prOzet.gunlukUretim == null ? 0 : (float)Math.Round(prOzet.gunlukUretim.Value / 1000, 1);
                            }
                            else
                            {
                                sgc.DAILY_PRODUCTION = null;
                            }

                            sgc.ENERGY = prOzet.enerji == null ? 0 : (float)Math.Round(prOzet.enerji.Value, 2);
                        }
                    }
                    else if (date == "week")
                    {
                        DateTime lastWeek = nowDate.AddDays(-7);
                        prOzetList = DB.PRSum.Where(a => a.STATION_ID == item.STATION_ID
                        ).OrderByDescending(a => a.date).Take(7).ToList();


                        _specificYield = prOzetList == null ? 0 : (float)prOzetList.Sum(a => a.enerji) * 1000 / item.DC_INSTALLED_POWER;
                        if (prOzet == null)
                        {
                            sgc.PR = 0;
                            sgc.IRRADIATION = null;
                            sgc.DAILY_PRODUCTION = null;
                            sgc.ENERGY = 0;
                        }
                        else
                        {
                            sgc.PR = prOzetList.Average(a => a.pr) == null ? 0 : (float)Math.Round(prOzetList.Average(a => a.pr.Value), 1);
                            sgc.IRRADIATION = prOzetList == null ? 0 : (float)Math.Round(prOzetList.Sum(a => a.isinim_ortalama).Value, 1);
                            sgc.DAILY_PRODUCTION = null;
                            sgc.ENERGY = prOzetList == null ? 0 : (float)Math.Round(prOzetList.Sum(a => a.enerji).Value, 2);
                        }
                    }

                    else if (date == "month")
                    {
                        DateTime thisMonth = nowDate;

                        prOzetList = DB.PRSum.Where(a => a.STATION_ID == item.STATION_ID
                                     && a.date.Value.Year == nowDate.Year
                                     && a.date.Value.Month == nowDate.Month).ToList();

                        _specificYield = prOzetList == null ? 0 : (float)prOzetList.Sum(a => a.enerji) * 1000 / item.DC_INSTALLED_POWER;
                        if (prOzetList.Count == 0)
                        {
                            sgc.PR = 0;
                            sgc.IRRADIATION = null;
                            sgc.DAILY_PRODUCTION = null;
                            sgc.ENERGY = 0;
                        }
                        else
                        {
                            var sumEnergy = prOzetList.Where(a => a.enerji != null).Sum(a => a.enerji);
                            var sumPR = prOzetList.Where(a => a.pr != null).Average(a => a.pr.Value);
                            var sumIrradiation = prOzetList.Count == 0 ? 0 : prOzetList.Where(a => a.isinim_ortalama != null).Sum(a => a.isinim_ortalama).Value;
                            _specificYield = sumEnergy * 1000 / item.DC_INSTALLED_POWER;
                            sgc.PR = (float)Math.Round(sumPR, 1);
                            sgc.IRRADIATION = (float)Math.Round(sumIrradiation, 1);
                            sgc.DAILY_PRODUCTION = null;
                            sgc.ENERGY = (float)Math.Round((float)sumEnergy, 2);
                        }

                    }
                    else
                    {
                        DateTime thisMonth = nowDate;

                        prOzetList = DB.PRSum.Where(a => a.STATION_ID == item.STATION_ID
                                     && a.date.Value.Year == nowDate.Year
                                     && a.date.Value.Month == nowDate.Month
                        ).ToList();

                        if (prOzetList.Count == 0)
                        {
                            sgc.PR = 0;
                            sgc.IRRADIATION = null;
                            sgc.DAILY_PRODUCTION = null;
                            sgc.ENERGY = 0;
                        }
                        else
                        {
                            var sumEnergy = prOzetList.Where(a => a.enerji != null).Sum(a => a.enerji);
                            var sumPR = prOzetList.Where(a => a.pr != null).Average(a => a.pr.Value);
                            var sumIrradiation = prOzetList.Count == 0 ? 0 : prOzetList.Where(a => a.isinim_ortalama != null).Sum(a => a.isinim_ortalama).Value;
                            _specificYield = sumEnergy * 1000 / item.DC_INSTALLED_POWER;
                            sgc.PR = (float)Math.Round(sumPR, 1);
                            sgc.IRRADIATION = (float)Math.Round(sumIrradiation, 1);
                            sgc.DAILY_PRODUCTION = null;
                            sgc.ENERGY = (float)Math.Round((float)sumEnergy, 2);
                        }
                    }

                    sgc.IS_ALARM = false;
                    sgc.IS_METEOROLOGY = item.IS_METEOROLOGY;
                    sgc.IS_MONEY = money;
                    sgc.FINANCIAL_USD = (float)Math.Round((((float)item.EXCHANGE_RATE) * (float)sgc.ENERGY * 1000), 2);
                    sgc.FINANCIAL_TL = 0;
                    sgc.DC_INSTALLED_POWER = item.DC_INSTALLED_POWER;
                    sgc.SPECIFIC_YIELD = (float)Math.Round((float)_specificYield, 2);
                    sgc.COMPANY_ID = item.COMPANY_ID;
                    sgc.STATION_ID = item.STATION_ID;
                    sgc.GROUP_ID = item.GROUP_ID;
                    sgc.CON_STATUS = null;
                    sgc.STATION_TYPE = item.STATION_TYPE;
                    if (User.IsInRole("DEMO"))
                    {
                        sgc.STATION_NAME = item.DEMO_STATION_NAME;
                        sgc.GROUP_NAME = groupList.Where(c => c.ID == item.GROUP_ID).FirstOrDefault() == null ? "AA" : groupList.Where(c => c.ID == item.GROUP_ID).FirstOrDefault().DEMO_NAME;
                        sgc.COMPANY_NAME = compList.Where(b => b.ID == item.COMPANY_ID).FirstOrDefault() == null ? "BB" : compList.Where(b => b.ID == item.COMPANY_ID).FirstOrDefault().DEMO_NAME;
                        sgc.COMPANY_GROUP_NAME = sgc.COMPANY_NAME + " / " + sgc.GROUP_NAME;
                    }
                    else
                    {
                        sgc.STATION_NAME = item.STATION_NAME;
                        sgc.GROUP_NAME = groupList.Where(c => c.ID == item.GROUP_ID).FirstOrDefault() == null ? "AA" : groupList.Where(c => c.ID == item.GROUP_ID).FirstOrDefault().NAME;
                        sgc.COMPANY_NAME = compList.Where(b => b.ID == item.COMPANY_ID).FirstOrDefault() == null ? "BB" : compList.Where(b => b.ID == item.COMPANY_ID).FirstOrDefault().NAME;
                        sgc.COMPANY_GROUP_NAME = sgc.COMPANY_NAME + " / " + sgc.GROUP_NAME;
                    }

                    sgcList.Add(sgc);

                }
            }

            return Json(sgcList, JsonRequestBehavior.AllowGet);
        }
   
        public class company_dto
        {
            public company_dto()
            {
                listGroup = new List<group_dto>();
            }
            public int COMPANY_ID { get; set; }
            public int GROUP_ID { get; set; }
            public string COMPANY_NAME { get; set; }
            public string DEMO_COMPANY_NAME { get; set; }
            public List<group_dto> listGroup { get; set; }
        }
        public class group_dto
        {
            public group_dto()
            {
                listStation = new List<station_dto>();
            }
            public int GROUP_ID { get; set; }
            public string GROUP_NAME { get; set; }
            public string DEMO_GROUP_NAME { get; set; }
            public int COMPANY_ID { get; set; }
            public List<station_dto> listStation { get; set; }
        }
        public class station_dto
        {
            public int STATION_ID { get; set; }
            public string STATION_NAME { get; set; }
            public string DEMO_STATION_NAME { get; set; }
            public int GROUP_ID { get; set; }
            public int COMPANY_ID { get; set; }
            public float IRRADIATION { get; set; }
            public float ENERGY { get; set; }
            public float DAILY_PRODUCTION { get; set; }
            public float PR { get; set; }
            public List<chart_dto> LIST_CHART { get; set; }
            public int? STATION_TYPE { get; set; }
        }
        public class chart_dto
        {
            public int STATION_ID { get; set; }
            public int? HOUR { get; set; }
            public string HOUR_DATE { get; set; }
            public float? HOURLY_PRODUCTION_AVG { get; set; }
            public float? HOURLY_IRRADIATION_AVG { get; set; }
            public float? IRRADIATION { get; set; }
            public float? DAILY_PRODUCTION { get; set; }
            public DateTime DATE { get; set; }
            public float? HOURLY_PRODUCTION_SUM { get; set; }
            public float? HOURLY_IRRADIATION_SUM { get; set; }
        }

        public List<chart_dto> hourlyCalculation(int stationId, DateTime reqDateParam)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("tr-TR");
            int nowHour = reqDateParam.Hour;
            List<chart_dto> hData = new List<chart_dto>();
            try
            {
                hData = (from t in DB.Summaries
                         where
                            t.STATION_ID == stationId
                            && t.tarih.Year == reqDateParam.Year
                            && t.tarih.Month == reqDateParam.Month
                            && t.tarih.Day == reqDateParam.Day
                         select new chart_dto
                         {
                             DATE = t.tarih,
                             HOUR = t.tarih.Hour,
                             IRRADIATION = t.isinim == null ? 0 : t.isinim,
                             DAILY_PRODUCTION = t.gunlukUretim == null ? 0 : t.gunlukUretim
                         })
                     .AsEnumerable()
                     .GroupBy(grp => grp.DATE.ToString("dd/MM/yyyy H"))
                     .Select(g => new chart_dto
                     {
                         STATION_ID = stationId,
                         HOUR_DATE = g.Key,
                         DATE = Convert.ToDateTime(g.Key.ToString() + ":00"),
                         HOURLY_IRRADIATION_SUM = g.Sum(x => x.IRRADIATION) == null ? 0 : (float)Math.Round(g.Sum(x => x.IRRADIATION.Value), 1),
                         HOURLY_PRODUCTION_SUM = g.Sum(x => x.DAILY_PRODUCTION) == null ? 0 : (float)Math.Round(g.Sum(x => x.DAILY_PRODUCTION.Value / 1000), 1)
                     }).Where(a => a.DATE.Hour != nowHour).ToList();
                if (hData.Count == 0 || hData == null)
                {
                    hData.Add(new chart_dto { STATION_ID = stationId, HOUR_DATE = DateTime.Now.ToString(), DATE = DateTime.Now, HOURLY_IRRADIATION_SUM = 0, HOURLY_PRODUCTION_SUM = 0 });
                }
                else
                {
                    for (int i = 0; i < 24; i++)
                    {
                        if (i < nowHour)
                        {
                            if (hData.Where(a => a.DATE.Hour == i).FirstOrDefault() == null)
                            {
                                hData.Add(new chart_dto
                                {
                                    STATION_ID = stationId,
                                    DATE = (new DateTime(reqDateParam.Year, reqDateParam.Month, reqDateParam.Day)),
                                    HOUR = i
                                });
                            }
                        }
                    }
                    hData.OrderBy(a => a.DATE);
                }
            }
            catch (Exception ex)
            {

            }
            return hData;
        }
        public void ALL()
        {
            var userId = User.Identity.GetUserId();
            var compList = DB.Companies.Where(a => a.IS_DELETED == false).ToList();
            var groupList = DB.StationGroups.Where(a => a.IS_DELETED == false).ToList();
            List<TBL_STATION> stations = new List<TBL_STATION>();
            List<Layout_Station_DTO> stations_dto = new List<Layout_Station_DTO>();
            if (User.IsInRole("M_ADMIN"))
            {
                stations = DB.Stations.Where(a => a.IS_DELETED == false && a.IS_ACTIVE == true).ToList();
            }
            else if (User.IsInRole("COMP_ADMIN"))
            {
                int[] ib = DB.CompanyUsers.Where(a => a.USER_ID == userId && a.IS_DELETED == false).Select(a => a.COMPANY_ID).ToArray();
                stations = DB.Stations.Where(a => a.IS_DELETED == false && a.IS_ACTIVE == true && ib.Contains(a.COMPANY_ID)).ToList();
            }
            else if (User.IsInRole("COMP_USER"))
            {
                int[] ibs = DB.StationUsers.Where(a => a.USER_ID == userId && a.IS_DELETED == false).Select(a => a.STATION_ID).ToArray();
                stations = DB.Stations.Where(a => a.IS_DELETED == false && a.IS_ACTIVE == true && ibs.Contains(a.ID)).ToList();
            }
            else if (User.IsInRole("DEMO"))
            {
                int[] ibs = DB.StationUsers.Where(a => a.USER_ID == userId && a.IS_DELETED == false).Select(a => a.STATION_ID).ToArray();
                stations = DB.Stations.Where(a => a.IS_DELETED == false && a.IS_ACTIVE == true && ibs.Contains(a.ID)).ToList();
            }

            stations_dto = stations.Where(a => a.IS_DELETED == false && a.IS_ACTIVE == true).Select(a =>
                  new Layout_Station_DTO
                  {
                      STATION_NAME = (User.IsInRole("DEMO")) ? a.DEMO_NAME : a.NAME.ToString(),
                      STATION_ID = a.ID,
                      GROUP_ID = a.GROUP_ID,
                      COMPANY_ID = a.COMPANY_ID
                  }).ToList();

            List<Layout_Group_DTO> gdtlist = new List<Layout_Group_DTO>();
            foreach (Layout_Station_DTO st in stations_dto)
            {
                Layout_Group_DTO gdt = new Layout_Group_DTO();
                gdt.listStation.Add(new Layout_Station_DTO { STATION_ID = st.STATION_ID, STATION_NAME = st.STATION_NAME, COMPANY_ID = st.COMPANY_ID, GROUP_ID = st.GROUP_ID });
                gdt.GROUP_ID = st.GROUP_ID;
                gdt.COMPANY_ID = st.COMPANY_ID;
                if (User.IsInRole("DEMO"))
                {
                    gdt.GROUP_NAME = groupList.Where(c => c.ID == st.GROUP_ID).FirstOrDefault() == null ? "BB" : groupList.Where(c => c.ID == st.GROUP_ID).FirstOrDefault().DEMO_NAME;
                }
                else
                {
                    gdt.GROUP_NAME = groupList.Where(c => c.ID == st.GROUP_ID).FirstOrDefault() == null ? "BB" : groupList.Where(c => c.ID == st.GROUP_ID).FirstOrDefault().NAME;
                }

                if (gdtlist.Where(a => a.GROUP_ID == st.GROUP_ID).FirstOrDefault() == null)
                {
                    gdtlist.Add(gdt);
                }
                else
                {
                    gdtlist.Where(a => a.GROUP_ID == st.GROUP_ID).FirstOrDefault().listStation.Add(st);
                }

            }
            List<Layout_Company_DTO> complist = new List<Layout_Company_DTO>();
            foreach (Layout_Group_DTO grp in gdtlist)
            {
                Layout_Company_DTO compdt = new Layout_Company_DTO();
                compdt.GROUP_ID = grp.GROUP_ID;
                compdt.COMPANY_ID = grp.COMPANY_ID;
                if (User.IsInRole("DEMO"))
                {
                    compdt.COMPANY_NAME = compList.Where(b => b.ID == grp.COMPANY_ID).FirstOrDefault() == null ? "AA" : compList.Where(b => b.ID == grp.COMPANY_ID).FirstOrDefault().DEMO_NAME;
                }
                else
                {
                    compdt.COMPANY_NAME = compList.Where(b => b.ID == grp.COMPANY_ID).FirstOrDefault() == null ? "AA" : compList.Where(b => b.ID == grp.COMPANY_ID).FirstOrDefault().NAME;
                }

                if (complist.Where(a => a.COMPANY_ID == grp.COMPANY_ID).FirstOrDefault() == null)
                {
                    complist.Add(compdt);
                    complist.Where(a => a.COMPANY_ID == grp.COMPANY_ID).FirstOrDefault().listGroup.Add(grp);
                }
                else
                {
                    complist.Where(a => a.COMPANY_ID == grp.COMPANY_ID).FirstOrDefault().listGroup.Add(grp);
                }
            }
        }

        public ActionResult Detail(int stationId)
        {
            return View(stationId);
        }
    }
}