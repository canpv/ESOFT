using Esso.Data;
using Esso.Models;
using Esso.ViewModels;
using Esso.Web.Models;
using Esso.Web.ViewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace Esso.Web.Controllers
{
    public class MapController : BaseController
    {
        EssoEntities DB = new EssoEntities();
        // GET: Map
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Index2()
        {
            return View();
        }
        public ActionResult Index3()
        {
            return View();
        }
        public ActionResult Index4()
        {
            return View();
        }
        public ActionResult Index5()
        {
            return View();
        }
        public ActionResult Index6()
        {
            return View();
        }
        public ActionResult Index7()
        {
            return View();
        }

        //public JsonResult GetStationMapData()
        //{
        //    var userId = User.Identity.GetUserId();
        //    List<STATION_GRUP_COMPANY> sgcList = new List<STATION_GRUP_COMPANY>();
        //    List<TBL_STATION> stations = new List<TBL_STATION>();
        //    List<STATION_GRUP_COMPANY> stations_dto = new List<STATION_GRUP_COMPANY>();
        //    DateTime nowDate = new DateTime();
        //    bool isDemo = false;
        //    if (User.IsInRole("M_ADMIN"))
        //    {
        //        stations = DB.Stations.Where(a => a.IS_DELETED == false && a.IS_ACTIVE == true && a.STATION_TYPE != 4).ToList();
        //    }
        //    else if (User.IsInRole("COMP_ADMIN"))
        //    {
        //        int[] ib = DB.CompanyUsers.Where(a => a.USER_ID == userId && a.IS_DELETED == false).Select(a => a.COMPANY_ID).ToArray();
        //        stations = DB.Stations.Where(a => a.IS_DELETED == false && a.IS_ACTIVE == true && ib.Contains(a.COMPANY_ID) && a.STATION_TYPE != 4).ToList();
        //    }
        //    else if (User.IsInRole("COMP_USER"))
        //    {
        //        int[] ibs = DB.StationUsers.Where(a => a.USER_ID == userId && a.IS_DELETED == false).Select(a => a.STATION_ID).ToArray();
        //        stations = DB.Stations.Where(a => a.IS_DELETED == false && a.IS_ACTIVE == true && ibs.Contains(a.ID) && a.STATION_TYPE != 4).ToList();
        //    }
        //    else if (User.IsInRole("DEMO"))
        //    {
        //        isDemo = true;
        //        int[] ibs = DB.StationUsers.Where(a => a.USER_ID == userId && a.IS_DELETED == false).Select(a => a.STATION_ID).ToArray();
        //        stations = DB.Stations.Where(a => a.IS_DELETED == false && a.IS_ACTIVE == true && ibs.Contains(a.ID) && a.STATION_TYPE != 4).ToList();
        //    }

        //    int[] stationIds = stations.Select(s => s.ID).ToArray();
        //    try
        //    {


        //        List<STATION_GRUP_COMPANY> stationSummary = (from v in DB.Stations
        //                                                     join grp in DB.StationGroups on v.GROUP_ID equals grp.ID
        //                                                     join comp in DB.Companies on v.COMPANY_ID equals comp.ID
        //                                                     join u in DB.stationSummary on v.ID equals u.STATION_ID
        //                                                     into abc
        //                                                     where stationIds.Contains(v.ID)
        //                                                     from b in abc.DefaultIfEmpty()
        //                                                     orderby v.NAME ascending
        //                                                     select new STATION_GRUP_COMPANY
        //                                                     {
        //                                                         STATION_ID = (b == null ? v.ID : b.STATION_ID),
        //                                                         DATE = b.DATE.Value == null ? DateTime.Now : b.DATE.Value,
        //                                                         STATION_NAME = (isDemo == true ? v.DEMO_NAME : v.NAME),
        //                                                         DEMO_STATION_NAME = v.DEMO_NAME,
        //                                                         COMPANY_ID = comp.ID,
        //                                                         COMPANY_GROUP_NAME = (isDemo == true ? comp.DEMO_NAME + " / " + grp.DEMO_NAME : comp.NAME + " / " + grp.NAME),
        //                                                         GROUP_ID = (b == null ? grp.ID : b.GROUP_ID),
        //                                                         GROUP_NAME = (grp.NAME == null ? "" : grp.NAME),
        //                                                         ENERGY = ((float)Math.Round(b.ENERGY.Value, 2)),
        //                                                         DAILY_PRODUCTION = ((float)Math.Round(b.DAILY_PRODUCTION.Value / 1000, 1)),
        //                                                         DC_INSTALLED_POWER = v.DC_INSTALLED_POWER,
        //                                                         STATION_TYPE = v.STATION_TYPE,
        //                                                         COORDINANT = v.COORDINATE_INFORMATION
        //                                                     }).ToList();

        //        var ConnectionList = (DB.AlarmStatus.Where(a => a.STATUS != 2 && a.END_DATE == null
        //                       ).OrderByDescending(a => a.START_DATE).ToList());

        //        var alarmDescList = DB.AlarmDesc.ToList();

        //        List<STATION_GRUP_COMPANY> stationSummaryList = new List<STATION_GRUP_COMPANY>();
        //        foreach (var st in stationSummary)
        //        {
        //            var isConnection = ConnectionList.Where(a => a.STATION_ID == st.STATION_ID
        //                      && a.ERROR_NUMBER == "0001" && a.STATUS != 2 && a.END_DATE == null
        //                      ).OrderByDescending(a => a.START_DATE).Take(1).FirstOrDefault();

        //            var alarmCount = (from a1 in ConnectionList
        //                              join a2 in alarmDescList on a1.ERROR_NUMBER equals a2.ERROR_NUMBER
        //                              where a1.STATION_ID == st.STATION_ID && a1.STATUS != 2 && a1.END_DATE == null && a2.TYPE == 1
        //                              select a1).ToList().Count();

        //            if (isConnection != null)
        //            {
        //                st.CON_STATUS = false;
        //                st.IS_ALARM = true;
        //            }
        //            else
        //            {
        //                st.CON_STATUS = true;
        //            }

        //            if (alarmCount > 0)
        //            {
        //                st.ALARM_COUNT = alarmCount;
        //                st.IS_ALARM = true;
        //            }

        //            if (st.DATE.Date != nowDate.Date)
        //            {
        //                st.ENERGY = null;
        //                st.FINANCIAL_TL = null;
        //                st.FINANCIAL_USD = null;
        //                st.SPECIFIC_YIELD = null;
        //                st.DAILY_PRODUCTION = null;
        //                st.IRRADIATION = null;
        //                st.PR = 0;
        //            }

        //            stationSummaryList.Add(st);
        //        }

        //        sgcList = stationSummaryList;
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //    return Json(sgcList, JsonRequestBehavior.AllowGet);
        //}

        public JsonResult GetStationMapData(string date)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("tr-TR");
            DateTime nowDate = new DateTime();

            nowDate = DateTime.Now;

            MAP_DTO mp = new MAP_DTO();
            var userId = User.Identity.GetUserId();
            List<STATION_GRUP_COMPANY> sgcList = new List<STATION_GRUP_COMPANY>();
            List<TBL_STATION> stations = new List<TBL_STATION>();
            List<Layout_Station_DTO> stations_dto = new List<Layout_Station_DTO>();

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

            int[] stationIds = stations.Select(s => s.ID).ToArray();
            try
            {              

                List<STATION_GRUP_COMPANY> stationSummary = (from v in DB.Stations
                                                             //join grp in DB.StationGroups on v.GROUP_ID equals grp.ID
                                                             //join comp in DB.Companies on v.COMPANY_ID equals comp.ID
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
                                                                 COMPANY_ID = b.COMPANY_ID,
                                                                 //COMPANY_GROUP_NAME = (isDemo == true ? comp.DEMO_NAME + " / " + grp.DEMO_NAME : comp.NAME + " / " + grp.NAME),
                                                                 GROUP_ID = (b == null ? b.GROUP_ID : b.GROUP_ID),
                                                                 //GROUP_NAME = (grp.NAME == null ? "" : grp.NAME),
                                                                 ACTIVE_INV_COUNT = (b.ALARM_COUNT == null ? 0 : b.ALARM_COUNT.Value),
                                                                 ENERGY = ((float)Math.Round(b.ENERGY.Value, 2)),
                                                                 IRRADIATION = ((float)Math.Round(b.IRRADIATION.Value, 1)),
                                                                 //CON_STATUS = (b.COMMINCATION == false ? true : false),
                                                                 DAILY_PRODUCTION = ((float)Math.Round(b.DAILY_PRODUCTION.Value / 1000, 1)),
                                                                 DC_INSTALLED_POWER = v.DC_INSTALLED_POWER,
                                                                 PR = (b.PR == null ? 0 : (float)Math.Round(b.PR.Value, 1)),
                                                                 ALARM_COUNT = 0,
                                                                 //FINANCIAL_USD = ((float)Math.Round(b.FINANCIAL_INCOME.Value, 2)),
                                                                 //INVERTER_ACTIVE_COUNT = (b.PASIVE_INV_COUNT == null ? 0 : b.INV_COUNT - b.PASIVE_INV_COUNT.Value),
                                                                 INVERTER_COUNT = (b.INV_COUNT == null ? 0 : b.INV_COUNT),
                                                                 IS_MONEY = null,
                                                                 SPECIFIC_YIELD = ((float)Math.Round(b.SPESIFIC_YIELD.Value, 2)),
                                                                 IS_METEOROLOGY = (v.IS_METEOROLOGY == null ? false : v.IS_METEOROLOGY.Value),
                                                                 //IS_ALARM = (b.ALARM_COUNT == 0 || b.ALARM_COUNT == null ? false : true),
                                                                 //INV_ERROR = (b.PASIVE_INV_COUNT > 0 ? true : false),
                                                                 STATION_TYPE = v.STATION_TYPE,
                                                                 COORDINANT = v.COORDINATE_INFORMATION
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


                    if (isConnection != null)
                    {
                        st.CON_STATUS = false;
                    }
                    else
                    {
                        st.CON_STATUS = true;
                    }


                    stationSummaryList.Add(st);
                }

                sgcList = stationSummaryList;

                mp.listStation = sgcList;
            }
            catch (Exception ex)
            {

            }
            return Json(sgcList, JsonRequestBehavior.AllowGet);
        }
        public class MAP_DTO
        {
            public MAP_DTO()
            {
                listComp = new List<Layout_Company_DTO>();
                listStation = new List<STATION_GRUP_COMPANY>();
            }
            public List<Layout_Company_DTO> listComp { get; set; }
            public List<STATION_GRUP_COMPANY> listStation { get; set; }

        }
    }
}