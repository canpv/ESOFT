using Esso.Data;
using Esso.Models;
using Esso.Web.Helpers;
using Esso.Web.Models;
using Esso.Web.Models.DashboardModel;
using Esso.Web.ViewModels;
using Microsoft.AspNet.Identity;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace Esso.Web.Controllers
{
    public class DashboardController : BaseController
    {
        EssoEntities DB = new EssoEntities();
        // GET: Dashboard
        public ActionResult Index()
        {
            ViewBag.routerStationId = TempData["stationId"] == null ? 0 : TempData["stationId"];
            return View();
        }

        public ActionResult Router(int stationId)
        {
            TempData["stationId"] = stationId;
            return RedirectToAction("Index");
        }

        public JsonResult GetDailyProductionChart(int stationId, string beginDate)
        {
            CultureHelper.SetCultureInfo();
            var startDate = DateTimeHelper.BeginDate(beginDate);
            var endDate = DateTimeHelper.EndDate(beginDate);
            DateTime reqDateParam = DateTime.Parse(@beginDate);
            Production_Main_DTO oDTO = new Production_Main_DTO();
            DateTime nowDate = DateTime.Now;

            if (nowDate.Date == startDate.Date)
            {
                oDTO.isToday = true;
            }
            try
            {
                var stDetail = DB.Stations.Where(a => a.ID == stationId).FirstOrDefault();

                oDTO.station = stDetail;

                float? scale;
                if (stDetail.IRRADIATION_SCALE != null)
                {
                    scale = (stDetail.AC_INSTALLED_POWER) + ((stDetail.AC_INSTALLED_POWER) * (stDetail.IRRADIATION_SCALE) / 100);
                }
                else
                {
                    scale = null;
                }

                oDTO._irradiationScale = scale;
                oDTO._acInstalledPower = stDetail.AC_INSTALLED_POWER;
                oDTO.isEKK = stDetail.IS_EKK;
                oDTO.isMeteorology = stDetail.IS_METEOROLOGY;
                oDTO.stationName = stDetail.NAME;//demo için tatmamla
                oDTO.isPyranometer = stDetail.IS_PYRANOMETER;
                oDTO.stationType = stDetail.STATION_TYPE;

                var dList = DB.Summaries.Where(p => p.STATION_ID == stationId
                            && p.tarih >= startDate && p.tarih <= endDate)
                            .OrderBy(a => a.tarih).ThenBy(t => t.Id).Select(a =>
                   new Production_DTO
                   {
                       id = a.Id,
                       date = a.tarih,
                       powerAc = a.gunlukUretim <= 0 ? 0 : (float)Math.Round((double)a.gunlukUretim / 1000, 1),
                       powerDc = a.Dc_Guc == null ? 0 : (float)Math.Round((double)a.Dc_Guc / 1000, 1),
                       energy = a.Enerji <= 0f ? 0f : (float)Math.Round((double)a.Enerji / 1000000, 2),
                       irradiation = (float)Math.Round(a.isinim.Value, 1),
                       wind = (float)Math.Round(a.ruzgarHizi.Value, 1),
                       cellTemp = (float)Math.Round(a.hucreSicakligi.Value, 1),
                       extTemp = (float)Math.Round(a.sicaklik.Value, 1),
                       ekkPowerAc = (float)Math.Round(Math.Abs(a.H2_P.Value), 1)
                   }
                ).ToList();


                if (dList.Count > 0)
                {
                    oDTO.endData = dList[dList.Count() - 1];
                    oDTO.endData.powerDc = oDTO.endData.powerDc;
                    oDTO.efficiency = oDTO.endData.powerDc == 0 ? 0 : oDTO.endData.powerAc / oDTO.endData.powerDc * 100;

                    DateTime abFirstDate = startDate;
                    DateTime abLastDate = startDate.AddHours(23).AddMinutes(59).AddSeconds(59);
                    if (dList != null && dList.Count > 0)
                    {
                        DateTime lastDate = dList[dList.Count - 1].date;
                        DateTime FirsDate = dList[0].date;

                        while (abFirstDate < FirsDate)
                        {
                            Production_DTO ozet = new Production_DTO();
                            ozet.date = abFirstDate;
                            dList.Add(ozet);
                            abFirstDate = abFirstDate.AddMinutes(5);
                        }

                        while (abLastDate > lastDate)
                        {
                            lastDate = lastDate.AddMinutes(5);
                            Production_DTO ozet = new Production_DTO();
                            ozet.date = lastDate;
                            dList.Add(ozet);
                        }
                        dList = dList.OrderBy(x => x.date).ToList();
                    }

                    for (int i = 0; i < dList.Count(); i++)
                    {
                        dList[i].maxPowerAc = stDetail.AC_INSTALLED_POWER;

                        if ((dList[i].powerDc <= 0 && dList[i].irradiation <= 1) || (dList[i].irradiation == null && dList[i].powerDc == null))
                        {
                            dList[i].energy = null;
                        }
                    }

                    foreach (var item in dList)
                    {
                        item.dateUTC = DateTimeHelper.convertDateUTC(item.date);
                    }

                }
                else
                {
                    oDTO.ErrorMessage = "No Data";
                }


                var summaryProductionList = DB.PRSum.Where(p => p.STATION_ID == stationId && p.date.Value >= stDetail.START_DATE).ToList();

                var target = TargetHelpers.GetDailyTarget(DB, stationId, nowDate.Month);

                if (summaryProductionList.Count > 0)
                {

                    var daily = summaryProductionList.Where(p => p.STATION_ID == stationId && p.date.Value.Year == nowDate.Year && p.date.Value.Month == nowDate.Month && p.date.Value.Day == nowDate.Day).FirstOrDefault();
                    oDTO._dailyProduction = daily == null ? 0 : daily.enerji;
                    oDTO._dailyPr = daily == null ? 0 : daily.pr;
                    oDTO._dailyKF = ((oDTO._dailyProduction * 1000) / (nowDate.Hour * oDTO._acInstalledPower)) * 100;
                    oDTO._dailyIncome = stDetail.EXCHANGE_RATE == null || oDTO._dailyProduction == null ? 0 : (oDTO._dailyProduction * stDetail.EXCHANGE_RATE * 1000);

                    var monthlyList = summaryProductionList.Where(p => p.STATION_ID == stationId && p.date.Value.Year == nowDate.Year && p.date.Value.Month == nowDate.Month).ToList();
                    oDTO._monthlyProduction = monthlyList.Count > 0 ? monthlyList.Sum(s => s.enerji).Value : 0;
                    oDTO._monthlyPr = monthlyList.Count > 0 ? monthlyList.Average(s => s.pr).Value : 0;
                    oDTO._monthlyKF = ((oDTO._monthlyProduction * 1000) / ((((nowDate.Day - 1) * 24) + (DateTime.Now.Hour)) * oDTO._acInstalledPower)) * 100;
                    oDTO._monthlyIncome = stDetail.EXCHANGE_RATE == null || oDTO._monthlyProduction == null ? 0 : (oDTO._monthlyProduction * stDetail.EXCHANGE_RATE * 1000);

                    var annualList = summaryProductionList.Where(p => p.STATION_ID == stationId && p.date.Value.Year == nowDate.Year).ToList();
                    oDTO._annualProduction = annualList.Count > 0 ? annualList.Sum(a => a.enerji).Value : 0;
                    oDTO._annualPr = annualList.Count > 0 ? annualList.Average(s => s.pr).Value : 0;
                    oDTO._annualKF = ((oDTO._annualProduction * 1000) / ((((nowDate.Month - 1) * 30 * 24) + (nowDate.Day * 24)) * oDTO._acInstalledPower)) * 100;
                    oDTO._annualIncome = stDetail.EXCHANGE_RATE == null || oDTO._annualProduction == null ? 0 : (oDTO._annualProduction * stDetail.EXCHANGE_RATE * 1000);

                    oDTO._totalProduction = summaryProductionList.Count > 0 ? summaryProductionList.Sum(a => a.enerji).Value : 0;
                    oDTO._totalPr = summaryProductionList.Count > 0 ? summaryProductionList.Average(s => s.pr).Value : 0;
                    oDTO._totalIncome = stDetail.EXCHANGE_RATE == null || oDTO._totalProduction == null ? 0 : (oDTO._totalProduction * stDetail.EXCHANGE_RATE * 1000);


                    oDTO.specificYield = (float)(oDTO._dailyProduction.Value / stDetail.DC_INSTALLED_POWER * 1000);
                    oDTO.actualValue = target == 0 ? 0 : (oDTO._dailyProduction * 100) / target;

                }


                oDTO.listData = dList;
            }
            catch (Exception ex)
            {
                oDTO.ErrorMessage = "Error";
            }

            return Json(oDTO, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetStationDetail(int stationId)
        {
            var station = DB.Stations.Where(w => w.ID == stationId).FirstOrDefault();

            return Json(station, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMeteorologyChart(string beginDate, int stationId)
        {
            Meteorology_Main_DTO meteoList = new Meteorology_Main_DTO();
            try
            {
                CultureHelper.SetCultureInfo();
                var startDate = DateTimeHelper.BeginDate(beginDate);
                var endDate = DateTimeHelper.EndDate(beginDate);

                var dList = DB.Summaries.Where(a => a.STATION_ID == stationId
                 && a.tarih >= startDate && a.tarih <= endDate)
                                          .Select(a => new Meteorology_DTO
                                          {
                                              date = a.tarih,
                                              irradiation = (float)Math.Round((float)a.isinim, 1),
                                              pyranometer = (float)Math.Round((float)a.PYRANOMETER, 1),
                                              wind = (float)Math.Round((float)a.ruzgarHizi, 1),
                                              cellTemp = (float)Math.Round((float)a.hucreSicakligi, 1),
                                              extTemp = (float)Math.Round((float)a.sicaklik, 1)
                                          }).OrderBy(a => a.date).ToList();

                if (dList.Count > 0)
                {
                    foreach (var item in dList)
                    {
                        item.dateUTC = DateTimeHelper.convertDateUTC(item.date);
                    }
                }
                else
                {
                    meteoList.ErrorMessage = "No Data";
                }
                meteoList.listData = dList;
                meteoList.endData = dList[dList.Count() - 1];
            }
            catch (Exception ex)
            {
                meteoList.ErrorMessage = "Error";
            }

            return Json(meteoList, JsonRequestBehavior.AllowGet);

        }


        public JsonResult GetMonthlyProduction(int stationId, string beginDate)
        {
            CultureHelper.SetCultureInfo();
            var userId = User.Identity.GetUserId();
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
            string[] dateArr = beginDate.Split('-');
            int ay = Int32.Parse(dateArr[1]);
            int yil = Int32.Parse(dateArr[0]);
            float target = TargetHelpers.GetDailyTarget(DB, stationId, ay);

            var _stationDetail = DB.Stations.Where(a => a.ID == stationId).FirstOrDefault();
            var installedDC = _stationDetail.DC_INSTALLED_POWER;
            var _stExchange = _stationDetail.EXCHANGE_RATE;

            TBL_PR_DTO mm = new TBL_PR_DTO();
            mm._target = target;
            var prList = (from p in DB.PRSum
                          where p.STATION_ID == stationId && p.date.Value.Month == ay
                          && p.date.Value.Year == yil
                          orderby p.date
                          select p).ToList();

            var howDays = DateTime.DaysInMonth(yil, ay);


            var exchangeList = DB.exchange.Where(a => a.EXCHANGE_DATE.Month == ay && a.EXCHANGE_DATE.Year == yil).ToList();


            mm.listPR = (from p in prList.ToList()
                         join ex in exchangeList.ToList() on p.date.Value.Date.ToString("yyyy-MM-dd") equals ex.EXCHANGE_DATE.Date.ToString("yyyy-MM-dd")
                         into abc
                         from b in abc.DefaultIfEmpty()
                         select new TBL_PR
                         {
                             _tarih = p.date.Value,
                             _enerji = p.enerji == null ? 0 : (float)Math.Round((double)p.enerji, 2),
                             _IrradiationSum = p.isinim_ortalama == null ? 0 : (float)Math.Round((double)p.isinim_ortalama, 2),
                             _pr = p.pr == null ? 0 : (float)Math.Round((double)p.pr, 1),
                             _exchangeTL = b == null ? 0 : b.BUYING_VALUE,
                             _incomeUS = (p.enerji == null || _stExchange == null) ? 0 : (float)Math.Round((_stExchange * (float)p.enerji.Value * 1000), 2),
                             _incomeTL = (b == null || _stExchange == null || p.enerji == null) ? 0 : (float)Math.Round(((_stExchange) * (b.BUYING_VALUE.Value) * (p.enerji.Value) * 1000), 2)
                         }).OrderBy(o => o._tarih).ToList();


            for (int k = 1; k <= howDays; k++)
            {
                var isThere = mm.listPR.Where(w => w._tarih.Day == k).FirstOrDefault();
                if (isThere == null)
                {
                    mm.listPR.Add(new TBL_PR { _tarih = new DateTime(yil, ay, k), _enerji = 0, _IrradiationSum = 0, _pr = 0, _incomeTL = 0, _incomeUS = 0, _exchangeTL = 0 });
                }
            }

            mm.listPR = mm.listPR.OrderBy(o => o._tarih).ToList();

            if (mm.listPR.Count == 0)
            {
                mm.ortalamaPR = 0;
                mm.toplamIsinim = 0;
            }
            else
            {
                for (int i = 0; i < mm.listPR.Count; i++)
                {
                    if (money.Value == false)
                    {
                        mm.listPR[i]._incomeUS = 0;
                        mm.listPR[i]._incomeTL = 0;
                        mm.toplamKazancUS = 0;
                        mm.toplamKazancTL = 0;
                    }
                    else
                    {
                        mm.toplamKazancUS += mm.listPR[i]._incomeUS.Value;
                        mm.toplamKazancTL += mm.listPR[i]._incomeTL.Value;

                    }
                    mm.toplamEnerji += mm.listPR[i]._enerji.Value;
                    mm.toplamIsinim += mm.listPR[i]._IrradiationSum.Value;

                }
                mm.ortalamaPR = mm.listPR.Where(a => a._pr > 0).FirstOrDefault() == null ? 0 : mm.listPR.Where(a => a._pr > 0).Average(a => a._pr).Value;
            }

            return Json(mm, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAnnualProduction(int stationId, string beginDate)
        {
            CultureHelper.SetCultureInfo();
            int yil = Int32.Parse(beginDate);
            TBL_PR_MONTH_DTO mm = new TBL_PR_MONTH_DTO();
            mm.listPR = (from i in DB.PRSum
                         where i.STATION_ID == stationId && i.date.Value.Year == yil
                         group i by i.date.Value.Month into grp
                         select new TBL_PR_MONTH
                         {
                             energy = (float)Math.Round(grp.Sum(x => x.enerji.Value), 1),
                             month = grp.Max(a => a.date.Value.Month),
                             irradiationSum = grp.Sum(x => x.isinim_ortalama) == null ? 0 : grp.Sum(x => x.isinim_ortalama.Value),
                             pr = grp.Where(x => x.pr > 0).Average(x => x.pr) == null ? 0 : (float)Math.Round(grp.Where(x => x.pr > 0).Average(x => x.pr.Value), 1)
                         }).OrderBy(a => a.month).ToList();

            for (int i = 0; i < mm.listPR.Count; i++)
            {
                mm.toplamEnerji += mm.listPR[i].energy == null ? 0 : mm.listPR[i].energy.Value;
                mm.toplamIsinim += mm.listPR[i].irradiationSum.Value;
                mm.ortalamaPR += mm.listPR[i].pr.Value;
                mm.listPR[i].target = TargetHelpers.GetMonthlyTarget(DB, stationId, mm.listPR[i].month);
            }

            var listTRG = new List<float>();

            for (int i = 1; i <= 12; i++)
            {
                if (mm.listPR.Where(w => w.month == i).Count() == 0)
                {
                    mm.listPR.Add(new TBL_PR_MONTH { target = TargetHelpers.GetMonthlyTarget(DB, stationId, i), month = i });
                }
            }

            mm.listPR = mm.listPR.OrderBy(o => o.month).ToList();

            mm.listTarget = listTRG;
            if (mm.listPR.Count == 0)
            {
                mm.ortalamaEnerji = 0;
                mm.ortalamaIsinim = 0;
                mm.ortalamaPR = 0;
            }
            else
            {
                mm.ortalamaEnerji = mm.toplamEnerji / mm.listPR.Count;
                mm.ortalamaIsinim = mm.toplamIsinim / mm.listPR.Count;
                mm.ortalamaPR = mm.ortalamaPR / mm.listPR.Count;
            }

            return Json(mm, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTotalProduction(int stationId)
        {
            CultureHelper.SetCultureInfo();
            DateTime startDate = DB.Stations.Where(a => a.ID == stationId).Select(a => a.START_DATE.Value).FirstOrDefault();
            var mm = (from i in DB.PRSum
                      where i.STATION_ID == stationId && i.date.Value > startDate
                      group i by i.date.Value.Year into grp
                      select new TBL_PR_YEARLY
                      {
                          energy = (float)Math.Round(grp.Sum(x => x.enerji.Value), 1),
                          year = grp.Max(a => a.date.Value.Year),
                          pr = (float)Math.Round(grp.Where(x => x.pr > 0).Average(x => x.pr.Value), 1)
                      }).OrderBy(a => a.year).ToList();

            return Json(mm, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDropdownStationList(int companyId,int stationId)
        {
            List<TBL_STATION> stations = new List<TBL_STATION>();
            var userId = User.Identity.GetUserId();
            bool isDemo = false;
            if (User.IsInRole("M_ADMIN"))
            {
                stations = DB.Stations.Where(a => a.IS_DELETED == false && a.IS_ACTIVE == true && a.COMPANY_ID == companyId && a.ID != stationId && a.STATION_TYPE != 4).ToList();
            }
            else if (User.IsInRole("COMP_ADMIN"))
            {
                //int[] ib = DB.CompanyUsers.Where(a => a.USER_ID == userId && a.IS_DELETED == false).Select(a => a.COMPANY_ID).ToArray();
                stations = DB.Stations.Where(a => a.IS_DELETED == false && a.IS_ACTIVE == true && a.COMPANY_ID == companyId && a.ID != stationId && a.STATION_TYPE != 4).ToList();
            }
            else if (User.IsInRole("COMP_USER"))
            {
                int[] cStationIs = DB.Stations.Where(a => a.IS_ACTIVE == true && a.IS_DELETED == false && a.COMPANY_ID == companyId).Select(a => a.ID).ToArray();
                int[] ibs = DB.StationUsers.Where(a => a.USER_ID == userId && a.IS_DELETED == false && cStationIs.Contains(a.STATION_ID)).Select(a => a.STATION_ID).ToArray();
                stations = DB.Stations.Where(a => a.IS_DELETED == false && a.IS_ACTIVE == true && ibs.Contains(a.ID) && a.ID != stationId && a.STATION_TYPE != 4).ToList();
            }
            else if (User.IsInRole("DEMO"))
            {
                isDemo = true;
                int[] ibs = DB.StationUsers.Where(a => a.USER_ID == userId && a.IS_DELETED == false).Select(a => a.STATION_ID).ToArray();
                stations = DB.Stations.Where(a => a.IS_DELETED == false && a.IS_ACTIVE == true && ibs.Contains(a.ID) && a.ID != stationId && a.STATION_TYPE != 4).ToList();
            }

            return Json(stations.OrderBy(o=>o.NAME), JsonRequestBehavior.AllowGet);
        }


    }
}