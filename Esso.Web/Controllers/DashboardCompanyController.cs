using DevExpress.Web.Rendering;
using DevExpress.XtraPrinting.Native;
using Esso.Data;
using Esso.Models;
using Esso.ViewModels;
using Esso.Web.Helpers;
using Esso.Web.Models.DashboardModel;
using Microsoft.AspNet.Identity;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace Esso.Web.Controllers
{
    public class DashboardCompanyController : BaseController
    {
        EssoEntities DB = new EssoEntities();

        // GET: DashboardCompany
        public ActionResult Index()
        {
            ViewBag.routerGroupId = TempData["groupId"] == null ? 0 : TempData["groupId"];
            ViewBag.routerCompanyId = TempData["companyId"] == null ? 0 : TempData["companyId"];
            return View();
        }

        public ActionResult RouterGroup(int groupId)
        {
            TempData["groupId"] = groupId;
            return RedirectToAction("Index");
        }

        public ActionResult RouterCompany(int companyId)
        {
            TempData["companyId"] = companyId;
            return RedirectToAction("Index");
        }

        public JsonResult GetCompProduction(int groupId, string date, int companyId)
        {
            CultureHelper.SetCultureInfo();
            DateTime reqDateParam = new DateTime();
            if (date == "" || date == null)
            {
                reqDateParam = DateTime.Now;
            }
            else
            {
                reqDateParam = DateTime.Parse(date);
            }

            DASHBOARD_COMPANY_DTO comp = new DASHBOARD_COMPANY_DTO();
            bool isDemo = false;
            if (User.IsInRole("DEMO"))
            {
                isDemo = true;
            }

            List<TBL_STATION> stationList = new List<TBL_STATION>();

            if (companyId == 0)
            {
                stationList = DB.Stations.Where(x => x.GROUP_ID == groupId && x.IS_DELETED == false && x.IS_ACTIVE == true && x.STATION_TYPE != 4).OrderBy(a => a.GROUP_ID).ThenBy(a => a.NAME).ToList();
            }
            else
            {
                stationList = DB.Stations.Where(x => x.COMPANY_ID == companyId && x.IS_DELETED == false && x.IS_ACTIVE == true && x.STATION_TYPE != 4).OrderBy(a => a.GROUP_ID).ThenBy(a => a.NAME).ToList();
            }



            var stationSummary = (from st in stationList

                                  join sumr in DB.PRSum.Where(w =>
                                  w.date.Value.Year == reqDateParam.Year &&
                                  w.date.Value.Month == reqDateParam.Month &&
                                  w.date.Value.Day == reqDateParam.Day) on st.ID equals sumr.STATION_ID
                                  into abc
                                  //where stationIds.Contains(v.ID)
                                  from b in abc.DefaultIfEmpty()
                                  orderby st.NAME ascending
                                  select new StationProduction
                                  {
                                      Production = b == null ? 0 : ((float)Math.Round(b.enerji.Value, 2)),
                                      PowerAC = b == null ? 0 : ((float)Math.Round(b.gunlukUretim.Value / 1000, 1)),
                                      PR = b == null ? 0 : ((float)Math.Round(b.pr.Value, 1)),
                                      StationName = (isDemo == true ? st.DEMO_NAME : st.NAME),
                                      StationId = st.ID,
                                      Latitude = st.COORDINATE_INFORMATION == null || st.COORDINATE_INFORMATION.Split(',').Length<2 ? 0 : float.Parse(st.COORDINATE_INFORMATION.Split(',')[0].Replace('.',',')),
                                      Longitude = st.COORDINATE_INFORMATION == null || st.COORDINATE_INFORMATION.Split(',').Length < 2 ? 0 : float.Parse(st.COORDINATE_INFORMATION.Split(',')[1].Replace('.', ','))

                                  }).ToList();


            return Json(stationSummary, JsonRequestBehavior.AllowGet);
        }




    }
}