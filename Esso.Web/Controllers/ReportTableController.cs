using AutoMapper;
using DevExpress.Data.Filtering;
using DevExpress.Web;
using DevExpress.Web.Demos;
using DevExpress.Web.Mvc;
using Esso.Data;
using Esso.Models;
using Esso.Service;
using Esso.ViewModels;
using Esso.Web.App_Start;
using Esso.Web.Models;
using Esso.Web.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using System.Data;
using DevExpress.XtraPrinting;
using System.Drawing;
using Esso.Model.Models;
using Esso.Web.Helpers;
using System.Globalization;
using System.Threading;
using IndustrialNetwork.Modbus.TCP;
using Esso.Web.Models.DATE_NUMBER;
using Esso.Web.Models.EXPORT_MODEL;
using System.IO;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using language;
using Esso.Data;
using System;
using System.Linq.Dynamic;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Z.EntityFramework.Plus;
using System.Drawing;
using OfficeOpenXml;
using System.IO;
using System.Threading;
using System.Globalization;

namespace Esso.Web.Controllers
{
    [Authorize]
    public class ReportTableController : BaseController
    {
        EssoEntities _db = new EssoEntities();
        Array sirketler;
        Array istasyongrupları;
        public ActionResult Index(int? id)
        {
            string curUserId = User.Identity.GetUserId();

            string _role =
                            _db.Database.SqlQuery<string>("select \"AspNetRoles\".\"Name\" from \"AspNetUserRoles\" left join \"AspNetRoles\" on \"AspNetUserRoles\".\"RoleId\" = \"AspNetRoles\".\"Id\" where \"AspNetUserRoles\".\"UserId\" = '" + curUserId + "'")
                            .FirstOrDefault();

            List<TBL_COMPANY> ListCompany;
            List<TBL_STATION> ListStation;
            List<TBL_INVERTER> ListInverter;
            List<TBL_STATION_STRING> ListString;
            switch (_role)
            {
                case "M_ADMIN":
                    ListCompany = _db.Companies.Where(x => x.IS_DELETED == false).OrderBy(x => x.NAME).ToList();
                    ListStation = _db.Stations.Where(x => x.IS_DELETED == false && x.STATION_TYPE != 4).OrderBy(x => x.NAME).ToList();
                    ListInverter = _db.Inverters.Where(x => x.IS_DELETED == false).ToList();
                    ListString = (from inv in _db.stationString
                                  join station in _db.Stations on inv.STATION_ID equals station.ID
                                  where station.IS_DELETED == false && inv.IS_DELETED == false
                                  select inv).ToList();
                    break;
                case "COMP_USER":

                    int[] UserStatIds = _db.StationUsers.Where(a => a.USER_ID == curUserId).Select(a => a.STATION_ID).ToArray();
                    int[] compIds = _db.Stations.Where(a => a.IS_DELETED == false && UserStatIds.Contains(a.ID)).Select(a => a.COMPANY_ID).ToArray();
                    ListCompany = _db.Companies.Where(a => a.IS_DELETED == false && compIds.Contains(a.ID)).OrderBy(x => x.NAME).ToList();

                    ListStation = (from station in _db.Stations
                                   join user_station in _db.StationUsers on station.ID equals user_station.STATION_ID
                                   where user_station.USER_ID == curUserId && user_station.IS_DELETED == false && station.STATION_TYPE != 4
                                   select station).OrderBy(x => x.NAME).ToList();

                    ListInverter = (from inv in _db.Inverters
                                    join station in _db.Stations on inv.STATION_ID equals station.ID
                                    join user_station in _db.StationUsers on station.ID equals user_station.STATION_ID
                                    where user_station.USER_ID == curUserId && user_station.IS_DELETED == false && station.IS_DELETED == false && inv.IS_DELETED == false
                                    select inv).ToList();
                    ListString = (from inv in _db.stationString
                                  join station in _db.Stations on inv.STATION_ID equals station.ID
                                  where station.IS_DELETED == false && inv.IS_DELETED == false
                                  select inv).ToList();
                    break;

                case "COMP_ADMIN":
                    ListCompany = (from user_company in _db.CompanyUsers
                                   join company in _db.Companies on user_company.COMPANY_ID equals company.ID
                                   where user_company.USER_ID == curUserId && user_company.IS_DELETED == false
                                   select company).OrderBy(x => x.NAME).ToList();

                    ListStation = (from station in _db.Stations
                                   join company in _db.Companies on station.COMPANY_ID equals company.ID
                                   join user_company in _db.CompanyUsers on company.ID equals user_company.COMPANY_ID
                                   where user_company.USER_ID == curUserId && user_company.IS_DELETED == false && station.STATION_TYPE != 4
                                   select station).OrderBy(x => x.NAME).ToList();

                    ListInverter = (from inv in _db.Inverters
                                    join station in _db.Stations on inv.STATION_ID equals station.ID
                                    join company in _db.Companies on station.COMPANY_ID equals company.ID
                                    join user_company in _db.CompanyUsers on company.ID equals user_company.COMPANY_ID
                                    where user_company.USER_ID == curUserId && user_company.IS_DELETED == false && company.IS_DELETED == false && station.IS_DELETED == false && inv.IS_DELETED == false
                                    select inv).ToList();
                    ListString = (from inv in _db.stationString
                                  join station in _db.Stations on inv.STATION_ID equals station.ID
                                  where station.IS_DELETED == false && inv.IS_DELETED == false
                                  select inv).ToList();
                    break;
                case "DEMO":
                    int[] UserDemoStatIds = _db.StationUsers.Where(a => a.USER_ID == curUserId).Select(a => a.STATION_ID).ToArray();
                    int[] compDemoIds = _db.Stations.Where(a => a.IS_DELETED == false && UserDemoStatIds.Contains(a.ID)).Select(a => a.COMPANY_ID).ToArray();
                    ListCompany = _db.Companies.Where(a => a.IS_DELETED == false && compDemoIds.Contains(a.ID)).OrderBy(x => x.NAME).ToList();

                    ListStation = (from station in _db.Stations
                                   join user_station in _db.StationUsers on station.ID equals user_station.STATION_ID
                                   where user_station.USER_ID == curUserId && user_station.IS_DELETED == false && station.STATION_TYPE != 4
                                   select station).OrderBy(x => x.NAME).ToList();

                    ListInverter = (from inv in _db.Inverters
                                    join station in _db.Stations on inv.STATION_ID equals station.ID
                                    join user_station in _db.StationUsers on station.ID equals user_station.STATION_ID
                                    where user_station.USER_ID == curUserId && user_station.IS_DELETED == false && station.IS_DELETED == false && inv.IS_DELETED == false
                                    select inv).ToList();
                    ListString = (from inv in _db.stationString
                                  join station in _db.Stations on inv.STATION_ID equals station.ID
                                  where station.IS_DELETED == false && inv.IS_DELETED == false
                                  select inv).ToList();
                    break;
                default:
                    ListCompany = new List<TBL_COMPANY>();
                    ListStation = new List<TBL_STATION>();
                    ListInverter = new List<TBL_INVERTER>();
                    ListString = new List<TBL_STATION_STRING>();
                    break;
            }

            ViewBag.ListCompany = ListCompany;
            ViewBag.ListStation = ListStation;
            ViewBag.ListInverter = ListInverter;
            ViewBag.UserId = curUserId;
            ViewBag.ListString = ListString;
            int _Id = 0;
            string ChartName = "";

            if (id != null)
                _Id = Convert.ToInt32(id);

            TBL_USER_CHART _UserChart = _db.UserCharts.Find(_Id);

            List<TBL_USER_CHART_DETAIL> ListChartDetail;

            if (_UserChart != null)
            {
                ChartName = _UserChart.CHART_NAME;

                ListChartDetail = _db.ChartDetails.Where(x => x.CHART_ID == _UserChart.ID).ToList();
            }
            else
            {
                ListChartDetail = new List<TBL_USER_CHART_DETAIL>();
            }

            ViewBag.ChartId = _Id.ToString();
            ViewBag.ChartName = ChartName;
            ViewBag.ListChartDetail = ListChartDetail;

            return View();
        }

        public ActionResult TableList()
        {
            string curUserId = User.Identity.GetUserId();

            List<TBL_USER_CHART> ListUserChart = _db.UserCharts.Where(x => x.USER_ID == curUserId && x.TYPE == 1).ToList();

            ViewBag.List = ListUserChart;

            return View();
        }

        public ActionResult Table()
        {
            string curUserId = User.Identity.GetUserId();

            List<TBL_USER_CHART> ListUserChart = _db.UserCharts.Where(x => x.USER_ID == curUserId && x.TYPE == 1).ToList();

            if (ListUserChart == null)
            {
                ListUserChart = new List<Esso.Models.TBL_USER_CHART>();
            }

            ViewBag._List = ListUserChart;

            return View();
        }
        public ActionResult ProductionTable()
        {
            return View();
        }
        public ActionResult GetProductionTable(DateTime startdate)
        {
            //string curUserId = User.Identity.GetUserId();

            //List<TBL_USER_CHART> ListUserChart = _db.UserCharts.Where(x => x.USER_ID == curUserId && x.TYPE == 1).ToList();

            //if (ListUserChart == null)
            //{
            //    ListUserChart = new List<Esso.Models.TBL_USER_CHART>();
            //}

            //ViewBag._List = ListUserChart;

            //return View();
            EssoEntities DB = new EssoEntities();
            Thread.CurrentThread.CurrentCulture = new CultureInfo("tr-TR");
            int start = Convert.ToInt32(Request["start"]);
            int length = Convert.ToInt32(Request["length"]);
            string sortColumnName = Request["columns[" + Request["order[0][column]"] + "][data]"];
            string sortDirection = Request["order[0][dir]"];
            var role = User.IsInRole("DEMO");
            MemoryStream result = new MemoryStream();
            string excelreportPath = DateTime.Now + ".AylıkRapor.xlsx";
            Thread.CurrentThread.CurrentCulture = new CultureInfo("tr-TR");
            DateTime nowDate = DateTime.Now;
            using (EssoEntities db = new EssoEntities())
            {
                //var Station = (from sT in db.Stations
                //               where sT.ID == stationId
                //               select new
                //               {

                //                   ID = sT.ID,

                //                   STATION_ID = sT.GROUP_ID,
                //                   INSTALLED_POWER = sT.INSTALLED_POWER,
                //                   INSTALL_DATE = sT.INSTALL_DATE,
                //                   INVERTER_MODEL = sT.INVERTER_MODEL,
                //                   INVERTER_TYPE = sT.INVERTER_TYPE,
                //                   IP_ADDRESS = sT.IP_ADDRESS,
                //                   IRRADIATION_SCALE = sT.IRRADIATION_SCALE,
                //                   IS_CENTRAL_INV = sT.IS_CENTRAL_INV,
                //                   IS_EKK = sT.IS_EKK,
                //                   CREATED_DATE = sT.CREATED_DATE,
                //                   IS_METEOROLOGY = sT.IS_METEOROLOGY,
                //                   IS_PYRANOMETER = sT.IS_PYRANOMETER,
                //                   NAME = sT.NAME,
                //                   INSTALLED_POWER_AC = sT.AC_INSTALLED_POWER,
                //                   INSTALLED_POWER_DC = sT.DC_INSTALLED_POWER,
                //                   PANEL_BRAND = sT.PANEL_BRAND,
                //                   PITCH_DETAIL = sT.PITCH_DETAIL,
                //                   ORIENTATION = sT.ORIENTATION,
                //                   PANEL_TYPE = sT.PANEL_TYPE,
                //                   START_DATE = sT.START_DATE,
                //                   GROUP_ID = sT.GROUP_ID,
                //                   COMPANY_ID = sT.COMPANY_ID


                //               }).ToList();
                //var Station_group_id = Station[0].GROUP_ID;

                try
                {

                    var userId = User.Identity.GetUserId();
                    List<TBL_STATION> stations = new List<TBL_STATION>();
                    List<STATION_GRUP_COMPANY> sgcList = new List<STATION_GRUP_COMPANY>();
                    List<STATION_GRUP_COMPANY> sgcGroupList = new List<STATION_GRUP_COMPANY>();
                    List<STATION_GRUP_COMPANY> stations_dto = new List<STATION_GRUP_COMPANY>();
                    List<STATION_GRUP_COMPANY> stationList = new List<STATION_GRUP_COMPANY>();
                    float totalEnergy = 0;
                    float totalIrradiation = 0;


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

                    //bool? money = false;
                    //ApplicationUser usr = DB.Database.SqlQuery<ApplicationUser>("select \"AspNetUsers\".\"Id\",\"AspNetUsers\".\"UserName\",\"AspNetUsers\".\"Email\",\"AspNetUsers\".\"SHOW_MONEY\",\"AspNetUsers\".\"REPORT_SEND_MAIL\",\"AspNetUsers\".\"ALARM_SEND_MAIL\" from \"AspNetUsers\" where \"AspNetUsers\".\"IS_DELETED\" = 0  and \"AspNetUsers\".\"Id\" = '" + userId + "'")
                    //                       .Select(a =>
                    //                       new ApplicationUser
                    //                       {
                    //                           Id = a.Id,
                    //                           UserName = a.UserName,
                    //                           Email = a.Email,
                    //                           SHOW_MONEY = a.SHOW_MONEY,
                    //                           REPORT_SEND_MAIL = a.REPORT_SEND_MAIL,
                    //                           ALARM_SEND_MAIL = a.ALARM_SEND_MAIL
                    //                       }).OrderBy(a => a.UserName).FirstOrDefault();

                    //if (usr.SHOW_MONEY != null)
                    //{
                    //    if (usr.SHOW_MONEY.Value == 1)
                    //    {
                    //        money = true;
                    //    }
                    //    else
                    //    {
                    //        money = false;
                    //    }
                    //}

                    //int[] stationIds = stations.Where(a => a.GROUP_ID == Station[0].GROUP_ID).Select(s => s.ID).ToArray();
                    //int[] stationCompanyIds = stations.Where(a => a.COMPANY_ID == Station[0].COMPANY_ID).Select(s => s.ID).ToArray();



                    stations_dto = stations.Where(a => a.IS_DELETED == false && a.IS_ACTIVE == true /*&& a.COMPANY_ID == Station[0].COMPANY_ID*/).Select(a =>
                 new STATION_GRUP_COMPANY
                 {
                     STATION_NAME = a.NAME.ToString(),
                     STATION_ID = a.ID,
                     //GROUP_ID = a.GROUP_ID,
                     //COMPANY_ID = a.COMPANY_ID,
                     DEMO_STATION_NAME = a.DEMO_NAME,
                     //DC_INSTALLED_POWER = a.DC_INSTALLED_POWER,
                     //EXCHANGE_RATE = a.EXCHANGE_RATE,
                     //IS_METEOROLOGY = a.IS_METEOROLOGY == null ? false : a.IS_METEOROLOGY.Value,
                     //STATION_TYPE = a.STATION_TYPE
                 }).OrderBy(a => a.STATION_NAME).ToList();
                    //var compList = DB.Companies.Where(a => a.IS_DELETED == false).ToList();
                    //var groupList = DB.StationGroups.Where(a => a.IS_DELETED == false).ToList();
                    //var stationList = DB.Stations.Where(a => a.IS_DELETED == false).ToList();
                    foreach (var item in stations_dto)
                    {
                        STATION_GRUP_COMPANY sgc = new STATION_GRUP_COMPANY();
                        TBL_PR_OZET prOzet = new TBL_PR_OZET();
                        List<TBL_PR_OZET> prOzetList = new List<TBL_PR_OZET>();
                        float? _specificYield = 0;
                        DateTime thisMonth = nowDate;

                        prOzetList = DB.PRSum.Where(a => a.STATION_ID == item.STATION_ID
                                     && a.date.Value.Year == startdate.Year
                                     && a.date.Value.Month == startdate.Month
                                     && a.date.Value.Day == startdate.Day).ToList();

                        _specificYield = prOzetList == null ? 0 : (float)prOzetList.Sum(a => a.enerji) * 1000 / item.DC_INSTALLED_POWER;
                        //totalIrradiation += (float)item.IRRADIATION;
                        //totalEnergy += (float)item.ENERGY;
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
                            //var sumPR = prOzetList.Where(a => a.pr != null).Average(a => a.pr.Value);
                            var sumIrradiation = prOzetList.Count == 0 ? 0 : prOzetList.Where(a => a.isinim_ortalama != null).Sum(a => a.isinim_ortalama).Value;

                            //var sumIrradiation = prOzetList.Count == 0 ? 0 : prOzetList.Where(a => a.isinim_ortalama != null).Sum(a => a.isinim_ortalama).Value;
                            //_specificYield = sumEnergy * 1000 / item.DC_INSTALLED_POWER;
                            //sgc.PR = (float)Math.Round(sumPR, 1);
                            sgc.IRRADIATION = (float)Math.Round(sumIrradiation, 1);
                            sgc.DAILY_PRODUCTION = null;
                            sgc.ENERGY = (float)Math.Round((float)sumEnergy, 2);
                        }

                        //sgc.IS_ALARM = false;
                        //sgc.IS_METEOROLOGY = item.IS_METEOROLOGY;
                        //sgc.IS_MONEY = money;
                        //sgc.FINANCIAL_USD = (float)Math.Round((((float)item.EXCHANGE_RATE) * (float)sgc.ENERGY * 1000), 2);
                        //sgc.FINANCIAL_TL = 0;
                        ////sgc.LIST_CHART = null;
                        //sgc.DC_INSTALLED_POWER = item.DC_INSTALLED_POWER;
                        //sgc.SPECIFIC_YIELD = (float)Math.Round((float)_specificYield, 2);
                        //sgc.COMPANY_ID = item.COMPANY_ID;
                        sgc.STATION_ID = item.STATION_ID;
                        //sgc.GROUP_ID = item.GROUP_ID;
                        //sgc.CON_STATUS = null;
                        //sgc.STATION_TYPE = item.STATION_TYPE;
                        if (User.IsInRole("DEMO"))
                        {
                            sgc.STATION_NAME = item.DEMO_STATION_NAME;
                            //sgc.GROUP_NAME = groupList.Where(c => c.ID == item.GROUP_ID).FirstOrDefault() == null ? "AA" : groupList.Where(c => c.ID == item.GROUP_ID).FirstOrDefault().DEMO_NAME;
                            //sgc.COMPANY_NAME = compList.Where(b => b.ID == item.COMPANY_ID).FirstOrDefault() == null ? "BB" : compList.Where(b => b.ID == item.COMPANY_ID).FirstOrDefault().DEMO_NAME;
                            //sgc.COMPANY_GROUP_NAME = sgc.COMPANY_NAME + " / " + sgc.GROUP_NAME;
                        }
                        else
                        {
                            sgc.STATION_NAME = item.STATION_NAME;
                            //sgc.GROUP_NAME = groupList.Where(c => c.ID == item.GROUP_ID).FirstOrDefault() == null ? "AA" : groupList.Where(c => c.ID == item.GROUP_ID).FirstOrDefault().NAME;
                            //sgc.COMPANY_NAME = compList.Where(b => b.ID == item.COMPANY_ID).FirstOrDefault() == null ? "BB" : compList.Where(b => b.ID == item.COMPANY_ID).FirstOrDefault().NAME;
                            //sgc.COMPANY_GROUP_NAME = sgc.COMPANY_NAME + " / " + sgc.GROUP_NAME;
                        }

                        sgcList.Add(sgc);
                        //if (sgc.GROUP_ID == Station[0].GROUP_ID)
                        //{
                        //    sgcGroupList.Add(sgc);
                        //}


                    }

                    int totalrows = sgcList.Count;
                    int totalrowsafterfiltering = sgcList.Count();
                    //stations_dto = stations_dto.OrderBy(sortColumnName + " " + sortDirection).ToList();
                    sgcList = sgcList.Skip(start).Take(length).ToList();
                    return Json(new
                    {
                        data = sgcList.Select(o => new
                        {

                            STATION_ID = o.STATION_ID,
                            STATION_NAME = o.STATION_NAME,
                            IRRADIATION = o.IRRADIATION,
                            ENERGY = o.ENERGY,
                            //DATE = o.DATE.ToString("dd/MM/yyyy HH:mm:ss"),

                        }).ToList(),
                        draw = Request["draw"],
                        recordsTotal = totalrows,
                        recordsFiltered = totalrowsafterfiltering
                    }, JsonRequestBehavior.AllowGet);

                }
                catch (Exception ex)
                {
                    var a = ex.ToString();
                }












                //using (ExcelPackage pck = new ExcelPackage())
                //{
                //    ExcelWorksheet ws = pck.Workbook.Worksheets.Add(Resources.Station_Detail);




                //    int ay = Int32.Parse(DateTime.Now.Month.ToString());
                //    int yil = Int32.Parse(DateTime.Now.Year.ToString());
                //    float target = GetTargetFunc(stationId, ay);

                //    var _stationDetail = DB.Stations.Where(a => a.ID == stationId).FirstOrDefault();


                //    var reportBuffer = pck.GetAsByteArray();
                //    Response.Clear();
                //    Response.AddHeader("content-disposition", "attachment;  filename=" + excelreportPath);
                //    Response.ContentType = "application/vnd.openxmlformats - officedocument.spreadsheetml.sheet";
                //    Response.BinaryWrite(reportBuffer);
                //    result.WriteTo(Response.OutputStream);
                //    Response.Flush();
                //    Response.Close();
                //    Response.End();

                //};



            };
            return View();
        }

        public ActionResult ExportDailyProductionExcel(string startdate)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("tr-TR");
            int start = Convert.ToInt32(Request["start"]);
            int length = Convert.ToInt32(Request["length"]);
            string sortColumnName = Request["columns[" + Request["order[0][column]"] + "][data]"];
            string sortDirection = Request["order[0][dir]"];
            var role = User.IsInRole("DEMO");
            MemoryStream result = new MemoryStream();
            string excelreportPath = DateTime.Now + ".ÜretimRaporları.xlsx";
            //var EndDate = Convert.ToDateTime(endDate).AddDays(1);
            var startDate = Convert.ToDateTime(startdate);

            var userId = User.Identity.GetUserId();
            List<TBL_STATION> stations = new List<TBL_STATION>();

            List<TBL_COMPANY> companies = new List<TBL_COMPANY>();
            List<TBL_STATION_GROUP> groups = new List<TBL_STATION_GROUP>();
            List<STATION_GRUP_COMPANY> sgcList = new List<STATION_GRUP_COMPANY>();
            List<STATION_GRUP_COMPANY> sgcGroupList = new List<STATION_GRUP_COMPANY>();
            List<STATION_GRUP_COMPANY> stations_dto = new List<STATION_GRUP_COMPANY>();
            List<STATION_GRUP_COMPANY> stationList = new List<STATION_GRUP_COMPANY>();

            using (EssoEntities db = new EssoEntities())
            {







                float totalEnergy = 0;
                float totalIrradiation = 0;


                bool isDemo = false;
                if (User.IsInRole("M_ADMIN"))
                {
                    stations = db.Stations.Where(a => a.IS_DELETED == false && a.IS_ACTIVE == true && a.STATION_TYPE != 4).ToList();
                    companies = db.Companies.Where(a => a.IS_DELETED == false).ToList();
                    groups = db.StationGroups.Where(a => a.IS_DELETED == false).ToList();

                    istasyongrupları =
(from sT in db.Stations.Where(a => a.IS_DELETED == false && a.IS_ACTIVE == true && a.STATION_TYPE != 4)
join sG in db.StationGroups on sT.GROUP_ID equals sG.ID

 //where post.ID == id
 select new { ID = sG.ID, NAME = sG.NAME }).Select(p => p.ID)
            .Distinct().ToArray();

                    sirketler =
(from sT in db.Stations.Where(a => a.IS_DELETED == false && a.IS_ACTIVE == true && a.STATION_TYPE != 4)
join cMp in db.Companies on sT.COMPANY_ID equals cMp.ID
 //where post.ID == id
 select new { ID = cMp.ID, NAME = cMp.NAME }).Select(p => p.ID)
                                 .Distinct().ToArray();
                }
                else if (User.IsInRole("COMP_ADMIN"))
                {
                    int[] ib = db.CompanyUsers.Where(a => a.USER_ID == userId && a.IS_DELETED == false).Select(a => a.COMPANY_ID).ToArray();
                    stations = db.Stations.Where(a => a.IS_DELETED == false && a.IS_ACTIVE == true && ib.Contains(a.COMPANY_ID) && a.STATION_TYPE != 4).ToList();
                    companies = db.Companies.Where(a => a.IS_DELETED == false && ib.Contains(a.ID)).ToList();
                    groups = db.StationGroups.Where(a => a.IS_DELETED == false).ToList();

                    istasyongrupları =
(from sT in db.Stations.Where(a => a.IS_DELETED == false && a.IS_ACTIVE == true && ib.Contains(a.COMPANY_ID) && a.STATION_TYPE != 4)
 join sG in db.StationGroups on sT.GROUP_ID equals sG.ID

  //where post.ID == id
  select new { ID = sG.ID, NAME = sG.NAME }).Select(p => p.ID)
             .Distinct().ToArray();

                    sirketler =
(from sT in db.Stations.Where(a => a.IS_DELETED == false && a.IS_ACTIVE == true && ib.Contains(a.COMPANY_ID) && a.STATION_TYPE != 4)
join cMp in db.Companies on sT.COMPANY_ID equals cMp.ID
 //where post.ID == id
 select new { ID = cMp.ID, NAME = cMp.NAME }).Select(p => p.ID)
                                 .Distinct().ToArray().ToArray();

                }
                else if (User.IsInRole("COMP_USER"))
                {
                    int[] ibs = db.StationUsers.Where(a => a.USER_ID == userId && a.IS_DELETED == false).Select(a => a.STATION_ID).ToArray();
                    stations = db.Stations.Where(a => a.IS_DELETED == false && a.IS_ACTIVE == true && ibs.Contains(a.ID) && a.STATION_TYPE != 4).ToList();


                    istasyongrupları =
(from sT in db.Stations.Where(a => a.IS_DELETED == false && a.IS_ACTIVE == true && ibs.Contains(a.ID) && a.STATION_TYPE != 4)
 join sG in db.StationGroups on sT.GROUP_ID equals sG.ID

  //where post.ID == id
  select new { ID = sG.ID, NAME = sG.NAME }).Select(p => p.ID)
             .Distinct().ToArray();

                    sirketler =
(from sT in db.Stations.Where(a => a.IS_DELETED == false && a.IS_ACTIVE == true && ibs.Contains(a.ID) && a.STATION_TYPE != 4)
join cMp in db.Companies on sT.COMPANY_ID equals cMp.ID
 //where post.ID == id
 select new { ID = cMp.ID, NAME = cMp.NAME }).Select(p => p.ID)
                                 .Distinct().ToArray();

                }
                else if (User.IsInRole("DEMO"))
                {
                    isDemo = true;
                    int[] ibs = db.StationUsers.Where(a => a.USER_ID == userId && a.IS_DELETED == false).Select(a => a.STATION_ID).ToArray();
                    stations = db.Stations.Where(a => a.IS_DELETED == false && a.IS_ACTIVE == true && ibs.Contains(a.ID) && a.STATION_TYPE != 4).ToList();

                    //                    var istasyongrupları =
                    // (from sT in db.Stations.Where(a => a.IS_DELETED == false && a.IS_ACTIVE == true && ibs.Contains(a.ID) && a.STATION_TYPE != 4)
                    //  join sG in db.StationGroups on sT.GROUP_ID equals sG.ID

                    //  //where post.ID == id
                    //  select new { ID = sG.ID, NAME = sG.NAME }).Select(p => p.ID)
                    //              .Distinct();

                    //                    var sirketler =
                    //(from sT in db.Stations.Where(a => a.IS_DELETED == false && a.IS_ACTIVE == true && ibs.Contains(a.ID) && a.STATION_TYPE != 4)
                    // join cMp in db.Companies on sT.COMPANY_ID equals cMp.ID
                    // //where post.ID == id
                    // select new { ID = cMp.ID, NAME = cMp.NAME }).Select(p => p.ID)
                    //                                  .Distinct();
                    //groups = db.StationGroups.Where(a => a.IS_DELETED == false && ibs.Contains(a.ID)).ToList();
                }

                //bool? money = false;
                //ApplicationUser usr = DB.Database.SqlQuery<ApplicationUser>("select \"AspNetUsers\".\"Id\",\"AspNetUsers\".\"UserName\",\"AspNetUsers\".\"Email\",\"AspNetUsers\".\"SHOW_MONEY\",\"AspNetUsers\".\"REPORT_SEND_MAIL\",\"AspNetUsers\".\"ALARM_SEND_MAIL\" from \"AspNetUsers\" where \"AspNetUsers\".\"IS_DELETED\" = 0  and \"AspNetUsers\".\"Id\" = '" + userId + "'")
                //                       .Select(a =>
                //                       new ApplicationUser
                //                       {
                //                           Id = a.Id,
                //                           UserName = a.UserName,
                //                           Email = a.Email,
                //                           SHOW_MONEY = a.SHOW_MONEY,
                //                           REPORT_SEND_MAIL = a.REPORT_SEND_MAIL,
                //                           ALARM_SEND_MAIL = a.ALARM_SEND_MAIL
                //                       }).OrderBy(a => a.UserName).FirstOrDefault();

                //if (usr.SHOW_MONEY != null)
                //{
                //    if (usr.SHOW_MONEY.Value == 1)
                //    {
                //        money = true;
                //    }
                //    else
                //    {
                //        money = false;
                //    }
                //}

                //int[] stationIds = stations.Where(a => a.GROUP_ID == Station[0].GROUP_ID).Select(s => s.ID).ToArray();
                //int[] stationCompanyIds = stations.Where(a => a.COMPANY_ID == Station[0].COMPANY_ID).Select(s => s.ID).ToArray();



                stations_dto = stations.Where(a => a.IS_DELETED == false && a.IS_ACTIVE == true /*&& a.COMPANY_ID == Station[0].COMPANY_ID*/).Select(a =>
             new STATION_GRUP_COMPANY
             {
                 STATION_NAME = a.NAME.ToString(),
                 STATION_ID = a.ID,
                 DEMO_STATION_NAME = a.DEMO_NAME,
                 GROUP_ID = a.GROUP_ID,
                 COMPANY_ID = a.COMPANY_ID


             }).OrderBy(a => a.STATION_NAME).ToList();
                var compList = db.Companies.Where(a => a.IS_DELETED == false).ToList();
                var groupList = db.StationGroups.Where(a => a.IS_DELETED == false).ToList();
                foreach (var item in stations_dto)
                {
                    STATION_GRUP_COMPANY sgc = new STATION_GRUP_COMPANY();
                    TBL_PR_OZET prOzet = new TBL_PR_OZET();
                    List<TBL_PR_OZET> prOzetList = new List<TBL_PR_OZET>();
                    float? _specificYield = 0;


                    //prOzetList = DB.PRSum.Where(a => a.STATION_ID == item.STATION_ID
                    //             && a.date.Value.Year == startdate.Year
                    //             && a.date.Value.Month == startdate.Month
                    //             && a.date.Value.Day == startdate.Day).ToList();

                    //_specificYield = prOzetList == null ? 0 : (float)prOzetList.Sum(a => a.enerji) * 1000 / item.DC_INSTALLED_POWER;
                    ////totalIrradiation += (float)item.IRRADIATION;
                    ////totalEnergy += (float)item.ENERGY;
                    //if (prOzetList.Count == 0)
                    //{
                    //    sgc.PR = 0;
                    //    sgc.IRRADIATION = null;
                    //    sgc.DAILY_PRODUCTION = null;
                    //    sgc.ENERGY = 0;
                    //}
                    //else
                    //{

                    //    var sumEnergy = prOzetList.Where(a => a.enerji != null).Sum(a => a.enerji);
                    //    //var sumPR = prOzetList.Where(a => a.pr != null).Average(a => a.pr.Value);
                    //    var sumIrradiation = prOzetList.Count == 0 ? 0 : prOzetList.Where(a => a.isinim_ortalama != null).Sum(a => a.isinim_ortalama).Value;

                    //    //var sumIrradiation = prOzetList.Count == 0 ? 0 : prOzetList.Where(a => a.isinim_ortalama != null).Sum(a => a.isinim_ortalama).Value;
                    //    //_specificYield = sumEnergy * 1000 / item.DC_INSTALLED_POWER;
                    //    //sgc.PR = (float)Math.Round(sumPR, 1);
                    //    sgc.IRRADIATION = (float)Math.Round(sumIrradiation, 1);
                    //    sgc.DAILY_PRODUCTION = null;
                    //    sgc.ENERGY = (float)Math.Round((float)sumEnergy, 2);
                    //}


                    prOzetList = db.PRSum.Where(a => a.STATION_ID == item.STATION_ID
                                     && a.date.Value.Year == startDate.Year
                                     && a.date.Value.Month == startDate.Month
                                     && a.date.Value.Day == startDate.Day).ToList();

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
                        var sumIrradiation = prOzetList.Count == 0 ? 0 : prOzetList.Where(a => a.isinim_ortalama != null).Sum(a => a.isinim_ortalama).Value;
                        sgc.IRRADIATION = (float)Math.Round(sumIrradiation, 1);
                        sgc.DAILY_PRODUCTION = null;
                        sgc.ENERGY = (float)Math.Round((float)sumEnergy, 2);
                    }

                    sgc.COMPANY_ID = item.COMPANY_ID;
                    sgc.STATION_ID = item.STATION_ID;
                    sgc.GROUP_ID = item.GROUP_ID;


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
                    //if (sgcGroupList.Contains(sgc.GROUP_ID))
                    //{
                    //    sgcGroupList.Add(sgc);
                    //}


                }

                using (ExcelPackage pck = new ExcelPackage())
                {
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add(Resources.Station_Detail);

                    ws.Row(1).Height = 50;
                    ws.Column(2).Width = 10;
                    ws.Column(3).Width = 15;
                    ws.Column(4).Width = 15;
                    ws.Column(5).Width = 15;
                    ws.Column(6).Width = 15;
                    ws.Column(7).Width = 15;
                    ws.Column(8).Width = 15;
                    ws.Column(9).Width = 15;
                    ws.Column(10).Width = 15;
                    ws.Column(11).Width = 15;
                    ws.Column(12).Width = 15;

                    ws.Select("A1:B1");
                    ws.SelectedRange.Merge = true;

                    ws.Select("C1:D1");
                    ws.SelectedRange.Merge = true;

                    ws.Select("E1:F1");
                    ws.SelectedRange.Merge = true;

                    ws.Select("G1:H1");
                    ws.SelectedRange.Merge = true;

                    ws.Select("I1:J1");
                    ws.SelectedRange.Merge = true;

                    ws.Select("K1:L1");
                    ws.SelectedRange.Merge = true;

                    ws.Cells["A1"].Value = Resources.Companies;
                    ws.Cells["C1"].Value = Resources.Groups;
                    ws.Cells["E1"].Value = Resources.Station;
                    ws.Cells["G1"].Value = Resources.Station + Resources.Produced_Energy + "(MWh)";
                    ws.Cells["I1"].Value = Resources.Groups + Resources.Produced_Energy + "(MWh)";
                    ws.Cells["K1"].Value = Resources.Companies + Resources.Total + "(MWh)";

                    ws.Cells["A1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws.Cells["B1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws.Cells["C1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws.Cells["D1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws.Cells["E1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws.Cells["F1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws.Cells["G1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws.Cells["H1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws.Cells["I1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws.Cells["J1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws.Cells["K1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    ws.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["B1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["C1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["D1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["E1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["F1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["G1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["H1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["I1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["J1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["K1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    ws.Cells["A1"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    ws.Cells["C1"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    ws.Cells["E1"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    ws.Cells["G1"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    ws.Cells["I1"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    ws.Cells["K1"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    ws.Cells["A1"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(string.Format("#f9d355")));
                    ws.Cells["C1"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(string.Format("#f9d355")));
                    ws.Cells["E1"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(string.Format("#f9d355")));
                    ws.Cells["G1"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(string.Format("#f9d355")));
                    ws.Cells["I1"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(string.Format("#f9d355")));
                    ws.Cells["K1"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(string.Format("#f9d355")));


                    var station_toplam = 0;
                    var group_station_total = 0;
                    var company_name = "";
                    var station_toplam_company = 0;
                    float grup_toplam_enerji = 0;
                    float sirket_toplam_enerji = 0;
                    for (int i = 0; i < sirketler.Length; i++)
                    {
                        var sirket_id = sirketler.GetValue(i);

                        var station_groups = db.StationGroups.Where(x => x.COMPANY_ID == (int)sirket_id).ToList();


                        for (int j = 0; j < station_groups.Count; j++)
                        {
                            var istasyon_grup_id = station_groups[j].ID;

                            var istasyonlar = sgcList.Where(a => (int)istasyon_grup_id == a.GROUP_ID).ToList();

                            for (int k = 0; k < istasyonlar.Count; k++)
                            {

                                ws.Select("E" + (k + 2 + station_toplam) + ":F" + (k + 2 + station_toplam));
                                ws.SelectedRange.Merge = true;
                                ws.Cells["E" + (k + 2 + station_toplam)].Value = istasyonlar[k].STATION_NAME;


                                ws.Cells["G" + (k + 2 + station_toplam)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells["G" + (k + 2 + station_toplam)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                ws.Select("G" + (k + 2 + station_toplam) + ":H" + (k + 2 + station_toplam));
                                ws.SelectedRange.Merge = true;
                                ws.Cells["G" + (k + 2 + station_toplam)].Value = Math.Round((double)istasyonlar[k].ENERGY, 2);
                                grup_toplam_enerji += (float)istasyonlar[k].ENERGY;
                                sirket_toplam_enerji += (float)istasyonlar[k].ENERGY;
                            }


                            if (istasyonlar.Count != 0)
                            {


                                ws.Cells["C" + +(2 + station_toplam)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells["C" + +(2 + station_toplam)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                ws.Cells["C" + (2 + station_toplam)].Value = istasyonlar[0].GROUP_NAME;
                                ws.Select("C" + (2 + station_toplam) + ":D" + (1 + station_toplam + istasyonlar.Count));
                                ws.SelectedRange.Merge = true;
                                company_name = istasyonlar[0].COMPANY_NAME;

                                ws.Cells["I" + +(2 + station_toplam)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells["I" + +(2 + station_toplam)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                ws.Cells["I" + (2 + station_toplam)].Value = Math.Round((float)grup_toplam_enerji, 2);
                                ws.Select("I" + (2 + station_toplam) + ":J" + (1 + station_toplam + istasyonlar.Count));
                                ws.SelectedRange.Merge = true;
                                grup_toplam_enerji = 0;

                                //station_toplam += istasyonlar.Count;


                                //ws.Cells["C" + (2 +  station_toplam)].Value = istasyonlar[0].GROUP_NAME;
                                //ws.Select("C" + (2 +  station_toplam) + ":D" + ( 1+ station_toplam+ istasyonlar.Count));
                                //ws.SelectedRange.Merge = true;
                                //station_toplam += istasyonlar.Count;


                                //grup_count++;



                            }
                            //if (station_groups.Count-1==j) {

                            //}
                            station_toplam += istasyonlar.Count;
                            station_toplam_company += istasyonlar.Count;
                        }


                        //if (station_groups.Count != 0)
                        //{


                        ws.Cells["A" + +(2 + group_station_total)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        ws.Cells["A" + +(2 + group_station_total)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        ws.Cells["A" + (2 + group_station_total)].Value = company_name;
                        ws.Select("A" + (2 + group_station_total) + ":B" + (1 + station_toplam_company + group_station_total));
                        ws.SelectedRange.Merge = true;

                        ws.Cells["K" + +(2 + group_station_total)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        ws.Cells["K" + +(2 + group_station_total)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        ws.Cells["K" + (2 + group_station_total)].Value = sirket_toplam_enerji;
                        ws.Select("K" + (2 + group_station_total) + ":L" + (1 + station_toplam_company + group_station_total));
                        ws.SelectedRange.Merge = true;

                        sirket_toplam_enerji = 0;

                        //ws.Cells["C" + (2 +  station_toplam)].Value = istasyonlar[0].GROUP_NAME;
                        //ws.Select("C" + (2 +  station_toplam) + ":D" + ( 1+ station_toplam+ istasyonlar.Count));
                        //ws.SelectedRange.Merge = true;
                        //station_toplam += istasyonlar.Count;






                        //}

                        group_station_total += station_toplam_company;
                        station_toplam_company = 0;
                        //station_toplam = 0;
                    }







                    int ay = Int32.Parse(DateTime.Now.Month.ToString());
                    int yil = Int32.Parse(DateTime.Now.Year.ToString());





                    var reportBuffer = pck.GetAsByteArray();
                    Response.Clear();
                    Response.AddHeader("content-disposition", "attachment;  filename=" + excelreportPath);
                    Response.ContentType = "application/vnd.openxmlformats - officedocument.spreadsheetml.sheet";
                    Response.BinaryWrite(reportBuffer);
                    result.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.Close();
                    Response.End();

                };



            }

            return View();


        }

        public JsonResult ReportTemp(string date1, string date2, string _chartId)
        {
            ReportTablePageView _result = new ReportTablePageView();

            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("tr-TR");
                DateTime _date = Convert.ToDateTime(date1);
                DateTime _date2 = Convert.ToDateTime(date2);
                int _ChartId = Convert.ToInt32(_chartId);

                DateTime _StartDate = Convert.ToDateTime(_date.ToShortDateString());
                DateTime _EndDate = Convert.ToDateTime(_date2.ToShortDateString() + " 23:59:59");

                List<GridViewFieldItem> _MasterFieldList = new List<GridViewFieldItem>();
                List<GridViewFieldItem> _DetailFieldList = new List<GridViewFieldItem>();

                List<TBL_USER_CHART_DETAIL> _List = _db.ChartDetails.Where(x => x.CHART_ID == _ChartId).ToList();

                if (_List == null) _List = new List<TBL_USER_CHART_DETAIL>();
                string _SqlInvOzet = "";
                string _SqlTblOzet = "";
                string _InvNo = "";

                for (int i = 0; i < _List.Count; i++)
                {
                    int _StationId = Convert.ToInt32(_List[i].STATION_ID);
                    int _InverterId = Convert.ToInt32(_List[i].INVERTER_ID);
                    string _DataType = _List[i].VALUE_TYPE;
                    GridViewFieldItem _cFieldItem = new GridViewFieldItem();

                    _cFieldItem.FieldName = GetColumnName(_DataType);
                    _cFieldItem.DisplayName = _DataType.ToUpper().Replace('_', ' ');

                    if (_InverterId == 0)
                    {
                        _SqlTblOzet += "\"" + _cFieldItem.FieldName + "\",";

                        _MasterFieldList.Add(_cFieldItem);
                    }
                    else
                    {
                        if (_SqlInvOzet.IndexOf(_cFieldItem.FieldName) == -1)
                        {
                            _SqlInvOzet += "TBL_INV_OZET.\"" + _cFieldItem.FieldName + "\",";

                            _DetailFieldList.Add(_cFieldItem);
                        }

                        if (_InvNo.IndexOf(_InverterId.ToString()) == -1)
                        {
                            _InvNo += _InverterId + ",";
                        }
                    }

                }
                if (_InvNo == "")
                {
                    _InvNo = "0";
                }
                else
                {
                    _InvNo = _InvNo.Remove(_InvNo.Length - 1);
                }

                if (_List.Count > 0)
                {
                    _SqlInvOzet += "TBL_INV_OZET.\"Tarih\",TBL_INVERTER.NAME";
                    _SqlTblOzet += "\"tarih\"";

                    _SqlInvOzet = "SELECT " + _SqlInvOzet + " FROM TBL_INV_OZET LEFT JOIN TBL_INVERTER ON TBL_INV_OZET.\"Inv_Id\" = TBL_INVERTER.ID WHERE \"Tarih\" >= TO_DATE('" + _StartDate + "', 'DD/MM/YY HH24:MI:SS') AND \"Tarih\" <= TO_DATE('" + _EndDate + "', 'DD/MM/YY HH24:MI:SS') AND \"Inv_Id\" IN (" + _InvNo + ") ORDER BY TBL_INV_OZET.\"Tarih\" DESC ";
                    _SqlTblOzet = "SELECT " + _SqlTblOzet + " FROM TBL_OZET WHERE \"tarih\" >= TO_DATE('" + _StartDate + "', 'DD/MM/YY HH24:MI:SS') AND \"tarih\" <= TO_DATE('" + _EndDate + "', 'DD/MM/YY HH24:MI:SS') AND STATION_ID =" + _List[0].STATION_ID + "ORDER BY TBL_OZET.\"tarih\" DESC ";

                    _result._SqlDetail = _SqlInvOzet;
                    _result._SqlMaster = _SqlTblOzet;

                    _result._MasterFieldList = _MasterFieldList;
                    _result._DetailFieldList = _DetailFieldList;

                    List<TblOzet> _ListTblOzet = _db.Database.SqlQuery<TblOzet>(_SqlTblOzet).ToList();
                    List<TblInvOzet> _ListInvOzet = _db.Database.SqlQuery<TblInvOzet>(_SqlInvOzet).ToList();

                    for (int i = 0; i < _ListTblOzet.Count; i++)
                    {
                        List<string> _ListObject = new List<string>();

                        _ListObject.Add(_ListTblOzet[i].tarih.ToString());

                        for (int j = 0; j < _MasterFieldList.Count; j++)
                        {
                            _ListObject.Add(GetPropValue(_ListTblOzet[i], _MasterFieldList[j].FieldName).ToString());
                        }

                        _result._ListTblOzet.Add(_ListObject);
                    }
                    for (int i = 0; i < _ListInvOzet.Count; i++)
                    {
                        List<string> _ListObject = new List<string>();

                        _ListObject.Add(_ListInvOzet[i].Tarih.ToString());
                        _ListObject.Add(_ListInvOzet[i].NAME);

                        for (int j = 0; j < _DetailFieldList.Count; j++)
                        {
                            _ListObject.Add(GetPropValue(_ListInvOzet[i], _DetailFieldList[j].FieldName).ToString());
                        }

                        _result._ListInvOzet.Add(_ListObject);
                    }

                }
            }
            catch (Exception ex)
            {
                _result.ErrorMessage = ex.ToString();
            }

            return Json(_result, JsonRequestBehavior.AllowGet);
        }

        #region AJAX
        [HttpPost]
        public JsonResult GetTable(List<TreeViewDetail> data)
        {
            ReportTablePageView _result = new ReportTablePageView();

            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("tr-TR");

                DateTime _StartDate = Convert.ToDateTime(DateTime.Now.ToShortDateString());
                DateTime _EndDate = Convert.ToDateTime(DateTime.Now.ToShortDateString() + " 23:59:59");

                List<GridViewFieldItem> _MasterFieldList = new List<GridViewFieldItem>();
                List<GridViewFieldItem> _DetailFieldList = new List<GridViewFieldItem>();

                if (data == null) data = new List<TreeViewDetail>();

                string _SqlInvOzet = "";
                string _SqlTblOzet = "";
                string _InvNo = "";
                string _SqlTblStrOzet = "";

                for (int i = 0; i < data.Count; i++)
                {
                    int _StationId = Convert.ToInt32(data[i].StationId);
                    int _InverterId = Convert.ToInt32(data[i].InverterId);
                    string _DataType = data[i].DataType;


                    GridViewFieldItem _cFieldItem = new GridViewFieldItem();

                    _cFieldItem.FieldName = GetColumnName(_DataType);
                    _cFieldItem.DisplayName = _DataType.ToUpper().Replace('_', ' ');

                    if (_InverterId == 0)
                    {
                        _SqlTblOzet += "\"" + _cFieldItem.FieldName + "\",";

                        _MasterFieldList.Add(_cFieldItem);
                    }
                    else if (data[i].DataType == "STR_Value")
                    {
                        _cFieldItem.FieldName = "STR";
                        _cFieldItem.DisplayName = "str";
                        _DetailFieldList.Add(_cFieldItem);
                    }
                    else
                    {
                        if (_SqlInvOzet.IndexOf(_cFieldItem.FieldName) == -1)
                        {
                            _SqlInvOzet += "TBL_INV_OZET.\"" + _cFieldItem.FieldName + "\",";

                            _DetailFieldList.Add(_cFieldItem);
                        }

                        if (_InvNo.IndexOf(_InverterId.ToString()) == -1)
                        {
                            _InvNo += _InverterId + ",";
                        }

                    }
                }
                if (_InvNo == "")
                {
                    _InvNo = "0";
                }
                else
                {
                    _InvNo = _InvNo.Remove(_InvNo.Length - 1);
                }


                if (data.Count > 0)
                {
                    _SqlInvOzet += "TBL_INV_OZET.\"Tarih\",TBL_INVERTER.NAME";
                    _SqlTblOzet += "\"tarih\"";
                    _SqlTblStrOzet += "";
                    _SqlInvOzet = "SELECT " + _SqlInvOzet + " FROM TBL_INV_OZET LEFT JOIN TBL_INVERTER ON TBL_INV_OZET.\"Inv_Id\" = TBL_INVERTER.ID WHERE \"Tarih\" >= TO_DATE('" + _StartDate + "', 'DD/MM/YY HH24:MI:SS') AND \"Tarih\" <= TO_DATE('" + _EndDate + "', 'DD/MM/YY HH24:MI:SS') AND \"Inv_Id\" IN (" + _InvNo + ") ORDER BY TBL_INV_OZET.\"Tarih\" DESC ";
                    _SqlTblOzet = "SELECT " + _SqlTblOzet + " FROM TBL_OZET WHERE \"tarih\" >= TO_DATE('" + _StartDate + "', 'DD/MM/YY HH24:MI:SS') AND \"tarih\" <= TO_DATE('" + _EndDate + "', 'DD/MM/YY HH24:MI:SS') AND STATION_ID =" + data[0].StationId + "ORDER BY TBL_OZET.\"tarih\" DESC ";

                    _SqlTblStrOzet = "select * from TBL_STR_OZET_AVG where \"STATION_ID\"=103 AND \"TARIH_NUMBER\">2019010120 AND \"TARIH_NUMBER\"<2019010122";


                    _result._SqlDetail = _SqlInvOzet;
                    _result._SqlMaster = _SqlTblOzet;

                    _result._MasterFieldList = _MasterFieldList;
                    _result._DetailFieldList = _DetailFieldList;

                    List<TblOzet> _ListTblOzet = _db.Database.SqlQuery<TblOzet>(_SqlTblOzet).ToList();
                    List<TblInvOzet> _ListInvOzet = _db.Database.SqlQuery<TblInvOzet>(_SqlInvOzet).ToList();
                    //List<TBL_STR_OZET_AVG> _ListStrOzet = _db.Database.SqlQuery<TBL_STR_OZET_AVG>(_SqlTblStrOzet).ToList();

                    for (int i = 0; i < _ListTblOzet.Count; i++)
                    {
                        try
                        {
                            List<string> _ListObject = new List<string>();

                            _ListObject.Add(_ListTblOzet[i].tarih.ToString());

                            for (int j = 0; j < _MasterFieldList.Count; j++)
                            {
                                _ListObject.Add(GetPropValue(_ListTblOzet[i], _MasterFieldList[j].FieldName).ToString());
                            }

                            _result._ListTblOzet.Add(_ListObject);
                        }
                        catch (Exception)
                        {

                            throw;
                        }

                    }
                    for (int i = 0; i < _ListInvOzet.Count; i++)
                    {
                        List<string> _ListObject = new List<string>();

                        _ListObject.Add(_ListInvOzet[i].Tarih.ToString());
                        _ListObject.Add(_ListInvOzet[i].NAME);

                        for (int j = 0; j < _DetailFieldList.Count; j++)
                        {
                            _ListObject.Add(GetPropValue(_ListInvOzet[i], _DetailFieldList[j].FieldName).ToString());
                        }

                        _result._ListInvOzet.Add(_ListObject);
                    }

                    //for (int i = 0; i < _ListStrOzet.Count; i++)
                    //{

                    //    List<string> _ListObject = new List<string>();

                    //    _ListObject.Add(_ListStrOzet[i].TARIH_NUMBER.ToString());
                    //    _ListObject.Add(_ListStrOzet[i].STRING_ID.ToString());

                    //    for (int j = 0; j < _DetailFieldList.Count; j++)
                    //    {
                    //        _ListObject.Add(_ListStrOzet[i].VALUE.ToString());
                    //    }

                    //    _result._ListStrOzet.Add(_ListObject);
                    //}

                }
            }
            catch (Exception ex)
            {
                _result.ErrorMessage = ex.ToString();
            }

            return Json(_result, JsonRequestBehavior.AllowGet);
        }

        public object GetPropValue(object src, string propName)
        {
            object _result = src.GetType().GetProperty(propName).GetValue(src, null);

            if (_result == null)
                return 0;
            else
                return _result;
        }

        private string GetColumnName(string DataType)
        {
            string _result = string.Empty;

            switch (DataType)
            {
                case "STR_Value":
                    _result = "Value";
                    break;
                case "Frequency":
                    _result = "H2_F";
                    break;
                case "H2_WP_minus":
                    _result = "H2_WP_minus";
                    break;
                case "H2_WP_plus":
                    _result = "H2_WP_plus";
                    break;
                case "Current_L1":
                    _result = "H2_Ia";
                    break;
                case "Current_L2":
                    _result = "H2_Ib";
                    break;
                case "Current_L3":
                    _result = "H2_Ic";
                    break;
                case "Active_Power_Sum":
                    _result = "H2_P";
                    break;
                case "Power_Factor":
                    _result = "H2_PF";
                    break;
                case "Reactive_Power_Sum":
                    _result = "H2_Q";
                    break;
                case "Visible_Power_Sum":
                    _result = "H2_S";
                    break;
                case "Voltage_L1-L2":
                    _result = "H2_Vab";
                    break;
                case "Voltage_L1-L3":
                    _result = "H2_Vac";
                    break;
                case "Voltage_L2-L3":
                    _result = "H2_Vbc";
                    break;
                case "Daily_Production":
                    _result = "gunlukUretim";
                    break;
                case "Irradiation":
                    _result = "isinim";
                    break;
                case "Total_Energy":
                    _result = "Enerji";
                    break;
                //case "Total_Power_AC":
                //	_result = "gunlukUretim";
                //	break;
                case "Total_Power_DC":
                    _result = "Dc_Guc";
                    break;
                case "Pyranometer":
                    _result = "PYRANOMETER";
                    break;
                case "External_Temperature":
                    _result = "sicaklik";
                    break;
                case "Cell_Temperature":
                    _result = "hucreSicakligi";
                    break;
                case "Wind_Speed":
                    _result = "ruzgarHizi";
                    break;//TBL_OZET SON
                case "Current_AC":
                    _result = "Akim_AC";
                    break;
                case "Current_DC":
                    _result = "Akim_DC";
                    break;
                case "Voltage_AC":
                    _result = "Gerilim_AC";
                    break;
                case "Voltage_DC":
                    _result = "Gerilim_DC";
                    break;
                case "Power_AC":
                    _result = "Guc_AC";
                    break;
                case "Power_DC":
                    _result = "Guc_DC";
                    break;
                case "Production":
                    _result = "Gunluk_Enerji";
                    break;
                //Inverter Values
                case "HEARTBEAT":
                    _result = "HEARTBEAT";
                    break;
                case "INVERTER_MAIN_STATUS":
                    _result = "INVERTER_MAIN_STATUS";
                    break;
                case "REACTIVE_POWER":
                    _result = "REACTIVE_POWER";
                    break;
                case "GRID_VOLTAGE_VRMS":
                    _result = "GRID_VOLTAGE_VRMS";
                    break;
                case "GRID_FREQUENCY":
                    _result = "GRID_FREQUENCY";
                    break;
                case "POWER_FACTOR":
                    _result = "POWER_FACTOR";
                    break;
                case "CODE_OF_THE_ACTIVE_FAULT":
                    _result = "CODE_OF_THE_ACTIVE_FAULT";
                    break;
                case "GRID_CURRENT":
                    _result = "GRID_CURRENT";
                    break;
                case "DC_BUS_VOLTAGE":
                    _result = "DC_BUS_VOLTAGE";
                    break;
                case "GROUNDING_CURRENT":
                    _result = "GROUNDING_CURRENT";
                    break;
                case "SOLATION_RESISTANCE":
                    _result = "SOLATION_RESISTANCE";
                    break;
                case "AMBIENT_TEMPERATURE":
                    _result = "AMBIENT_TEMPERATURE";
                    break;
                case "HIGHEST_IGBT_TEMPERATURE_PU1":
                    _result = "HIGHEST_IGBT_TEMPERATURE_PU1";
                    break;
                case "HIGHEST_IGBT_TEMPERATURE_PU2":
                    _result = "HIGHEST_IGBT_TEMPERATURE_PU2";
                    break;
                case "HIGHEST_IGBT_TEMPERATURE_PU3":
                    _result = "HIGHEST_IGBT_TEMPERATURE_PU3";
                    break;
                case "HIGHEST_IGBT_TEMPERATURE_PU4":
                    _result = "HIGHEST_IGBT_TEMPERATURE_PU4";
                    break;
                case "CONTROL_SECTION_TEMPERATURE":
                    _result = "CONTROL_SECTION_TEMPERATURE";
                    break;
                case "DAILY_KVAH_SUPPLIED":
                    _result = "DAILY_KVAH_SUPPLIED";
                    break;
                case "TOTAL_KVAH_SUPPLIED":
                    _result = "TOTAL_KVAH_SUPPLIED";
                    break;
                case "IGBT_1_T1":
                    _result = "IGBT_1_T1";
                    break;
                case "IGBT_1_T2":
                    _result = "IGBT_1_T2";
                    break;
                case "IGBT_1_T3":
                    _result = "IGBT_1_T3";
                    break;
                case "IGBT_2_T1":
                    _result = "IGBT_2_T1";
                    break;
                case "IGBT_2_T2":
                    _result = "IGBT_2_T2";
                    break;
                case "IGBT_2_T3":
                    _result = "IGBT_2_T3";
                    break;
                case "IGBT_3_T1":
                    _result = "IGBT_3_T1";
                    break;
                case "IGBT_3_T2":
                    _result = "IGBT_3_T2";
                    break;
                case "IGBT_3_T3":
                    _result = "IGBT_3_T3";
                    break;
                case "IGBT_4_T1":
                    _result = "IGBT_4_T1";
                    break;
                case "IGBT_4_T2":
                    _result = "IGBT_4_T2";
                    break;
                case "IGBT_4_T3":
                    _result = "IGBT_4_T3";
                    break;
                //Meteoroloji 2
                case "MEAN_WIND_DIRECTION_1":
                    _result = "MEAN_WIND_DIRECTION_1";
                    break;
                case "AIR_TEMPERATURE_1":
                    _result = "AIR_TEMPERATURE_1";
                    break;
                case "RELATIVE_HUMIDITY_1":
                    _result = "RELATIVE_HUMIDITY_1";
                    break;
                case "ABSOLUTE_HUMIDITY_1":
                    _result = "ABSOLUTE_HUMIDITY_1";
                    break;
                case "ABSOLUTE_AIR_PRESSURE_1":
                    _result = "ABSOLUTE_AIR_PRESSURE_1";
                    break;
                case "MEAN_WIND_DIRECTION_2":
                    _result = "MEAN_WIND_DIRECTION_2";
                    break;
                case "AIR_TEMPERATURE_2":
                    _result = "AIR_TEMPERATURE_2";
                    break;
                case "RELATIVE_HUMIDITY_2":
                    _result = "RELATIVE_HUMIDITY_2";
                    break;
                case "ABSOLUTE_HUMIDITY_2":
                    _result = "ABSOLUTE_HUMIDITY_2";
                    break;
                case "ABSOLUTE_AIR_PRESSURE_2":
                    _result = "ABSOLUTE_AIR_PRESSURE_2";
                    break;
                case "HUCRESICAKLIGI_2":
                    _result = "HUCRESICAKLIGI_2";
                    break;
                case "SICAKLIK_2":
                    _result = "SICAKLIK_2";
                    break;
                case "ISINIM_2":
                    _result = "ISINIM_2";
                    break;
                case "RUZGARHIZI_2":
                    _result = "RUZGARHIZI_2";
                    break;
                case "PYRANOMETER_2":
                    _result = "PYRANOMETER_2";
                    break;
            }

            return _result;
        }

        [HttpPost]
        public JsonResult SaveUserChart(List<TreeViewDetail> data, string UserId, string ChartName, string ChartId)
        {
            ReportTablePageView _result = new ReportTablePageView();

            try
            {
                int _Id = Convert.ToInt32(ChartId);

                TBL_USER_CHART _UserChart = _db.UserCharts.Find(_Id);

                if (_UserChart == null)
                {
                    _UserChart = new TBL_USER_CHART();
                    _db.UserCharts.Add(_UserChart);
                }

                _UserChart.CHART_NAME = ChartName;
                _UserChart.USER_ID = UserId;
                _UserChart.TYPE = 1;

                _db.SaveChanges();

                List<TBL_USER_CHART_DETAIL> _ChartDetailList = _db.ChartDetails.Where(x => x.CHART_ID == _UserChart.ID).ToList();

                for (int i = 0; i < _ChartDetailList.Count; i++)
                    _db.ChartDetails.Remove(_ChartDetailList[i]);

                _db.SaveChanges();

                if (data != null)
                {
                    for (int i = 0; i < data.Count; i++)
                    {
                        TBL_USER_CHART_DETAIL _ChartDetail = new TBL_USER_CHART_DETAIL();

                        _ChartDetail.INVERTER_ID = Convert.ToInt32(data[i].InverterId);
                        _ChartDetail.STATION_ID = Convert.ToInt32(data[i].StationId);
                        _ChartDetail.VALUE_TYPE = data[i].DataType;
                        _ChartDetail.CHART_ID = Convert.ToInt32(_UserChart.ID);

                        _db.ChartDetails.Add(_ChartDetail);
                    }

                    _db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                //_result.ErrorMessage = ex.ToString();
            }

            return Json(_result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteUserChart(string Id)
        {
            string _result = "";

            try
            {
                int _Id = Convert.ToInt32(Id);

                TBL_USER_CHART _UserChart = _db.UserCharts.Find(_Id);

                if (_UserChart != null)
                {
                    _db.UserCharts.Remove(_UserChart);

                    List<TBL_USER_CHART_DETAIL> _ChartDetailList = _db.ChartDetails.Where(x => x.CHART_ID == _UserChart.ID).ToList();

                    for (int i = 0; i < _ChartDetailList.Count; i++)
                        _db.ChartDetails.Remove(_ChartDetailList[i]);
                }

                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                _result = ex.ToString();
            }

            return Json(_result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Class

        public class TblOzet
        {
            public System.DateTime tarih { get; set; }
            public Nullable<float> Enerji { get; set; }
            public Nullable<float> aylikEnerji { get; set; }
            public Nullable<float> yillikEnerji { get; set; }
            public Nullable<float> toplamEnerji { get; set; }
            public Nullable<float> isinim { get; set; }
            public Nullable<float> gunlukUretim { get; set; }
            public Nullable<float> H2_Vbn { get; set; }
            public Nullable<float> H2_Van { get; set; }
            public Nullable<float> H2_Vcn { get; set; }
            public Nullable<float> H2_Vab { get; set; }
            public Nullable<float> H2_Vbc { get; set; }
            public Nullable<float> H2_Vac { get; set; }
            public Nullable<float> H2_F { get; set; }
            public Nullable<float> H2_Q { get; set; }
            public Nullable<float> H2_P { get; set; }
            public Nullable<float> H2_Ic { get; set; }
            public Nullable<float> H2_Ib { get; set; }
            public Nullable<float> H2_Ia { get; set; }
            public Nullable<float> H2_S { get; set; }
            public Nullable<float> H2_PF { get; set; }
            public Nullable<float> H2_WP_minus { get; set; }
            public Nullable<float> H2_WP_plus { get; set; }
            public Nullable<float> Dc_Guc { get; set; }
            public Nullable<double> sicaklik { get; set; }
            public Nullable<float> hucreSicakligi { get; set; }
            public Nullable<float> ruzgarHizi { get; set; }
            public Nullable<float> PYRANOMETER { get; set; }

            public Nullable<float> MEAN_WIND_DIRECTION_1 { get; set; }
            public Nullable<float> AIR_TEMPERATURE_1 { get; set; }
            public Nullable<float> RELATIVE_HUMIDITY_1 { get; set; }
            public Nullable<float> ABSOLUTE_HUMIDITY_1 { get; set; }
            public Nullable<float> ABSOLUTE_AIR_PRESSURE_1 { get; set; }
            public Nullable<float> ISINIM_2 { get; set; }
            public Nullable<float> RUZGARHIZI_2 { get; set; }
            public Nullable<float> HUCRESICAKLIGI_2 { get; set; }
            public Nullable<float> SICAKLIK_2 { get; set; }
            public Nullable<float> PYRANOMETER_2 { get; set; }
            public Nullable<float> MEAN_WIND_DIRECTION_2 { get; set; }
            public Nullable<float> AIR_TEMPERATURE_2 { get; set; }
            public Nullable<float> RELATIVE_HUMIDITY_2 { get; set; }
            public Nullable<float> ABSOLUTE_HUMIDITY_2 { get; set; }
            public Nullable<float> ABSOLUTE_AIR_PRESSURE_2 { get; set; }

        }
        public class TblInvOzet
        {

            public Nullable<System.DateTime> Tarih { get; set; }
            public int Inv_Id { get; set; }
            public int STATION_ID { get; set; }
            public string NAME { get; set; }
            public Nullable<float> Akim_AC { get; set; }
            public Nullable<float> Akim_DC { get; set; }
            public Nullable<float> Gerilim_AC { get; set; }
            public Nullable<float> Gerilim_DC { get; set; }
            public Nullable<float> Guc_AC { get; set; }
            public Nullable<float> Guc_DC { get; set; }
            public Nullable<float> Gunluk_Enerji { get; set; }

            public Nullable<float> HEARTBEAT { get; set; }
            public Nullable<float> INVERTER_MAIN_STATUS { get; set; }
            public Nullable<float> REACTIVE_POWER { get; set; }
            public Nullable<float> GRID_VOLTAGE_VRMS { get; set; }
            public Nullable<float> GRID_FREQUENCY { get; set; }
            public Nullable<float> POWER_FACTOR { get; set; }
            public Nullable<float> CODE_OF_THE_ACTIVE_FAULT { get; set; }
            public Nullable<float> GRID_CURRENT { get; set; }
            public Nullable<float> DC_BUS_VOLTAGE { get; set; }
            public Nullable<float> GROUNDING_CURRENT { get; set; }
            public Nullable<float> SOLATION_RESISTANCE { get; set; }
            public Nullable<float> AMBIENT_TEMPERATURE { get; set; }
            public Nullable<float> HIGHEST_IGBT_TEMPERATURE_PU1 { get; set; }
            public Nullable<float> HIGHEST_IGBT_TEMPERATURE_PU2 { get; set; }
            public Nullable<float> HIGHEST_IGBT_TEMPERATURE_PU3 { get; set; }
            public Nullable<float> HIGHEST_IGBT_TEMPERATURE_PU4 { get; set; }
            public Nullable<float> CONTROL_SECTION_TEMPERATURE { get; set; }
            public Nullable<float> DAILY_KVAH_SUPPLIED { get; set; }
            public Nullable<float> TOTAL_KVAH_SUPPLIED { get; set; }
            public Nullable<float> IGBT_1_T1 { get; set; }
            public Nullable<float> IGBT_1_T2 { get; set; }
            public Nullable<float> IGBT_1_T3 { get; set; }
            public Nullable<float> IGBT_2_T1 { get; set; }
            public Nullable<float> IGBT_2_T2 { get; set; }
            public Nullable<float> IGBT_2_T3 { get; set; }
            public Nullable<float> IGBT_3_T1 { get; set; }
            public Nullable<float> IGBT_3_T2 { get; set; }
            public Nullable<float> IGBT_3_T3 { get; set; }
            public Nullable<float> IGBT_4_T1 { get; set; }
            public Nullable<float> IGBT_4_T2 { get; set; }
            public Nullable<float> IGBT_4_T3 { get; set; }

        }
        public class TreeViewDetail
        {
            public string StationId { get; set; }
            public string InverterId { get; set; }
            public string DataType { get; set; }
        }

        public class ReportTablePageView
        {
            public string _SqlMaster { get; set; } = "";
            public string _SqlDetail { get; set; } = "";
            public List<GridViewFieldItem> _MasterFieldList { get; set; } = new List<GridViewFieldItem>();
            public List<GridViewFieldItem> _DetailFieldList { get; set; } = new List<GridViewFieldItem>();
            public List<List<string>> _ListTblOzet { get; set; } = new List<List<string>>();
            public List<List<string>> _ListInvOzet { get; set; } = new List<List<string>>();
            public List<List<string>> _ListStrOzet { get; set; } = new List<List<string>>();
            public string ErrorMessage { get; set; } = "";
        }

        public class GridViewFieldItem
        {
            public string FieldName { get; set; }
            public string DisplayName { get; set; }
        }


        #endregion
    }
}
