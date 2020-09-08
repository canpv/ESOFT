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

namespace Esso.Web.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        EssoEntities DB = new EssoEntities();
        private readonly IGraphicService graphicService;
        private readonly IStationService stationService;
        private readonly IInvOzetService invOzetService;
        private readonly IInverterService inverterService;
        private readonly IPrOzetService prOzetService;
        private readonly IMonthlyTargetService mTargetService;
        private readonly ICompanyService companyService;

        public HomeController(IGraphicService graphicService,
            IStationService stationService,
            IInvOzetService invOzetService,
            IInverterService inverterService,
            IPrOzetService prOzetService,
            ICompanyService companyService,
            IMonthlyTargetService mTargetService)
        {
            this.graphicService = graphicService;
            this.stationService = stationService;
            this.invOzetService = invOzetService;
            this.inverterService = inverterService;
            this.prOzetService = prOzetService;
            this.mTargetService = mTargetService;
            this.companyService = companyService;
        }
        public ActionResult SetCulture(string culture)
        {
            // Validate input
            culture = CultureLanguageHelper.GetImplementedCulture(culture);

            // Save culture in a cookie
            HttpCookie cookie = Request.Cookies["_culture"];
            if (cookie != null)
                cookie.Value = culture;   // update cookie value 
            else
            {

                cookie = new HttpCookie("_culture");
                cookie.Value = culture;
                cookie.Expires = DateTime.Now.AddYears(1);
            }
            Response.Cookies.Add(cookie);

            return RedirectToAction("Index");
        }

        // GET: Home
        public ActionResult Index()
        {
            SetCultureInfo();
            return View();
        }

        public ActionResult Detail(int stationId)
        {
            return View(stationId);
        }

        public ActionResult StationDetail(int stationId)
        {
            return View(stationId);
        }

        public ActionResult MeteorolojiDetail(int stationId)
        {
            return View(stationId);
        }


        public ActionResult ProductionComparison(int companyId)
        {
            return View(companyId);
        }
        public ActionResult ProductionComparisonGroup(int groupId)
        {
            return View(groupId);
        }
        public ActionResult InverterValue(int stationId)
        {
            return View(stationId);
        }
        public ActionResult DataMonitoring(int stationId)
        {
            return View(stationId);
        }
        public ActionResult InstantPR(int companyId)
        {
            return View(companyId);
        }
        public ActionResult Maps(int companyId)
        {
            return View(companyId);
        }
        public ActionResult AllStationMaps()
        {
            return View();
        }
        public ActionResult AllStationMaps2()
        {
            return View();
        }
        private static void SetCultureInfo()
        {
            System.Globalization.CultureInfo _CultureInfo = new System.Globalization.CultureInfo("tr-TR");
            _CultureInfo.NumberFormat.CurrencyDecimalSeparator = ",";
            _CultureInfo.NumberFormat.CurrencyGroupSeparator = ".";
            _CultureInfo.NumberFormat.NumberDecimalSeparator = ",";
            _CultureInfo.NumberFormat.NumberGroupSeparator = ".";
            _CultureInfo.NumberFormat.PercentDecimalSeparator = ",";
            _CultureInfo.NumberFormat.PercentGroupSeparator = ".";
            System.Threading.Thread.CurrentThread.CurrentCulture = _CultureInfo;
        }

        public JsonResult HourlyReport(int stationId, string slctDate, string enddate)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("tr-TR");
            DateTime date = DateTime.Parse(@slctDate);
            DateTime date2 = DateTime.Parse(enddate);
            date2 = date2.AddDays(1);
            var hourlyProduction = (from slc in DB.Summaries
                                    where slc.STATION_ID == stationId && slc.tarih >= date && slc.tarih <= date2
                                    && slc.tarih.Hour >= 5 && slc.tarih.Hour <= 21
                                    orderby slc.tarih ascending
                                    select new Hour_DTO
                                    {
                                        _enerji = slc.Enerji == null ? 0 : slc.Enerji.Value,
                                        _isinimToplam = slc.isinim == null ? 0 : slc.isinim.Value,
                                        _uretilen_enerji = slc.H2_WP_minus == null ? 0 : slc.H2_WP_minus.Value,
                                        _hucre_sicakligi = slc.hucreSicakligi == null ? 0 : slc.hucreSicakligi.Value,
                                        _tarih = slc.tarih
                                    })
                  .AsEnumerable()
                  .GroupBy(x => x._tarih.ToString("dd/MM/yyyy HH:00:00"))
                  .Select(g => new Hour_DTO
                  {
                      _enerji = (g.Max(a => a._enerji)) / 1000000,
                      _uretilen_enerji = (g.Max(a => a._uretilen_enerji)),
                      _isinimToplam = g.Average(a => a._isinimToplam),
                      _hucre_sicakligi = g.Average(a => a._hucre_sicakligi),
                      _tarih = Convert.ToDateTime(g.Key)
                  }).ToList();
            List<Hour_DTO> hTO = new List<Hour_DTO>();
            int say = hourlyProduction.Count();
            foreach (var hh in hourlyProduction.OrderByDescending(o => o._tarih).ToList())
            {
                say--;
                if (say > 0)
                {
                    double fark = 0;
                    double EkkFark = 0;
                    if ((hourlyProduction[say]._tarih.Date) == (hourlyProduction[say - 1]._tarih.Date))
                    {
                        fark = (hourlyProduction[say]._enerji) - (hourlyProduction[say - 1]._enerji);
                        EkkFark = (hourlyProduction[say]._uretilen_enerji) - (hourlyProduction[say - 1]._uretilen_enerji);
                    }
                    hTO.Add(new Hour_DTO { _hucre_sicakligi = Math.Round(hh._hucre_sicakligi), _saat = hh._tarih.Hour, _enerji = Math.Round(fark, 4), _enerjiArtan = Math.Round(hh._enerji, 4), _uretilen_enerji = Math.Round(EkkFark, 4), _isinimToplam = Math.Round(hh._isinimToplam, 1), _tarih = hh._tarih });
                }

            }
            var data = hTO.OrderBy(o => o._tarih).ToList();
            Session["HourlyEnergyData"] = data;
            return Json(data);
        }



        [HttpGet]
        public FileContentResult ExportToExcelHourly(int stationId, string slctDate, string slctDate2)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("tr-TR");
            DateTime date = DateTime.Parse(@slctDate);
            DateTime date2 = DateTime.Parse(@slctDate2);
            date2 = date2.AddDays(1);
            var stat = DB.Stations.Where(w => w.ID == stationId).FirstOrDefault();
            var hourlyProduction = (from slc in DB.Summaries
                                    where slc.STATION_ID == stationId && slc.tarih >= date && slc.tarih <= date2
                                    && slc.tarih.Hour >= 5 && slc.tarih.Hour <= 21
                                    orderby slc.tarih ascending
                                    select new Hour_DTO
                                    {
                                        _enerji = slc.Enerji == null ? 0 : slc.Enerji.Value,
                                        _isinimToplam = slc.isinim == null ? 0 : slc.isinim.Value,
                                        _uretilen_enerji = slc.H2_WP_minus == null ? 0 : slc.H2_WP_minus.Value,
                                        _tarih = slc.tarih,
                                        _hucre_sicakligi=slc.hucreSicakligi == null ? 0 : slc.hucreSicakligi.Value 
                                    })
                  .AsEnumerable()
                  .GroupBy(x => x._tarih.ToString("dd/MM/yyyy HH:00:00"))
                  .Select(g => new Hour_DTO
                  {
                      _enerji = (g.Max(a => a._enerji)) / 1000000,
                      _uretilen_enerji = (g.Max(a => a._uretilen_enerji)),
                      _isinimToplam = g.Average(a => a._isinimToplam),
                      _tarih = Convert.ToDateTime(g.Key),
                      _hucre_sicakligi = g.Average(a => a._hucre_sicakligi)
                  }).ToList();
            List<Hour_DTO> hTO = new List<Hour_DTO>();
            int say = hourlyProduction.Count();
            foreach (var hh in hourlyProduction.OrderByDescending(o => o._tarih).ToList())
            {
                say--;
                if (say > 0)
                {
                    double fark = 0;
                    double EkkFark = 0;
                    if ((hourlyProduction[say]._tarih.Date) == (hourlyProduction[say - 1]._tarih.Date))
                    {
                        fark = (hourlyProduction[say]._enerji) - (hourlyProduction[say - 1]._enerji);
                        EkkFark = (hourlyProduction[say]._uretilen_enerji) - (hourlyProduction[say - 1]._uretilen_enerji);
                    }
                    hTO.Add(new Hour_DTO { _saat = hh._tarih.Hour, _enerji = Math.Round(fark, 4), _enerjiArtan = Math.Round(hh._enerji, 4), _uretilen_enerji = Math.Round(EkkFark, 4), _isinimToplam = Math.Round(hh._isinimToplam, 1), _tarih = hh._tarih, _hucre_sicakligi = Math.Round(hh._hucre_sicakligi) });
                }

            }
            var data = hTO.OrderBy(o => o._tarih).Select(s => new HOURLY_EXPORT_MODEL {
                DATE = s._tarih,
                INV_PRODUCED_CUMULATIVE = s._enerjiArtan,
                INV_PRODUCED = s._enerji,
                EKK_PRODUCED = s._uretilen_enerji,
                IRRADIATION_ENERGY = s._isinimToplam,
                CELL_TEMPERATURE=s._hucre_sicakligi
            }).ToList();

            string fileName = stat.NAME.ToUpper() + " HOURLY_PRODUCTION.xlsx";
            string[] columns = { "DATE", "INV_PRODUCED_CUMULATIVE", "INV_PRODUCED", "EKK_PRODUCED", "IRRADIATION_ENERGY", "CELL_TEMPERATURE" };
            byte[] filecontent = ExcelExportHelper.ExportExcel(data, stat.NAME, true, columns);
            return File(filecontent, ExcelExportHelper.ExcelContentType, fileName);
        }

        public ActionResult bb(int stationId)
        {
            EssoEntities DB = new EssoEntities();
            try
            {
                foreach (var item in DB.Stations.Where(a => a.IS_ACTIVE == true && a.IS_DELETED == false && a.IS_METEOROLOGY == true && a.COMPANY_ID == 122))
                {
                    for (int i = 2; i < 81; i++)
                    {
                        DateTime date = DateTime.Now.AddDays(i * -1);

                        var installedDC = DB.Stations.Where(a => a.ID == item.ID).Select(a => a.DC_INSTALLED_POWER).FirstOrDefault();
                        var endData = DB.Summaries
                            .Where(x => x.STATION_ID == item.ID && x.tarih.Year == date.Year && x.tarih.Month == date.Month && x.tarih.Day == date.Day)
                            .OrderByDescending(x => x.tarih).FirstOrDefault();
                        float enerji = endData == null ? 0 : endData.Enerji.Value;
                        List<PRDTO> pr2 = (from toz in DB.Summaries
                                           where
                                           toz.gunlukUretim > 0 && toz.isinim > 0 && toz.isinim < 2000 && toz.isinim != null &&
                                              toz.STATION_ID == item.ID &&
                                             toz.tarih.Year == date.Year && toz.tarih.Month == date.Month && toz.tarih.Day == date.Day
                                           select new PRDTO
                                           {
                                               isinim = toz.isinim,
                                               tarih = toz.tarih
                                           })
                                 .AsEnumerable()
                                 .GroupBy(x => x.tarih.ToString("dd/MM/yyyy H"))
                                 .Select(g => new PRDTO
                                 {
                                     saatlikIsinimOrt = (g.Sum(ri => ri.isinim) / g.Count()),
                                     isinim = g.Sum(ri => ri.isinim),
                                     recCount = g.Count(),
                                     kTarih = g.Key
                                 }).ToList();
                        float? isinimGunlukOrtalama = pr2.Sum(x => x.saatlikIsinimOrt);

                        if (isinimGunlukOrtalama > 0)
                        {
                            var pr = enerji / isinimGunlukOrtalama / installedDC * 100;
                            if (pr > 100 && pr < 110)
                            {
                                pr = pr * (float)0.90;
                            }
                            if (pr > 110)
                            {
                                pr = 100;
                            }


                            var pr_guncelle = (from p in DB.PRSum
                                               where
         p.STATION_ID == item.ID
         && p.date.Value.Year == date.Year
         && p.date.Value.Month == date.Month
         && p.date.Value.Day == date.Day
                                               select p).SingleOrDefault();
                            if (pr_guncelle != null)
                            {
                                pr_guncelle.pr = pr == null ? 0 : pr;
                                pr_guncelle.isinim_ortalama = isinimGunlukOrtalama == null ? 0 : isinimGunlukOrtalama;
                                DB.SaveChanges();
                            }

                        }





                    }
                }
            }
            catch (Exception ex)
            {

            }
            return View(stationId);
        }

        public ActionResult aa(int stationId)
        {
            EssoEntities DB = new EssoEntities();
            DateTime date = DateTime.Now.AddDays(-2);
            List<PRDTO> pr2 = (from toz in DB.Summaries
                               where
                               toz.gunlukUretim > 0 && toz.isinim > 0 &&
                                  toz.STATION_ID == stationId &&
                                 toz.tarih.Year == date.Year && toz.tarih.Month == date.Month && toz.tarih.Day == date.Day
                                 && toz.tarih.Hour != date.Hour
                               select new PRDTO
                               {
                                   isinim = toz.isinim,
                                   tarih = toz.tarih
                               })
                      .AsEnumerable()
                      .GroupBy(x => x.tarih.ToString("dd/MM/yyyy H"))
                      .Select(g => new PRDTO
                      {
                          saatlikIsinimOrt = (g.Sum(ri => ri.isinim) / g.Count()),
                          isinim = g.Sum(ri => ri.isinim),
                          recCount = g.Count(),
                          kTarih = g.Key
                      }).ToList();

            float? energy = DB.Summaries
                .Where(x => x.STATION_ID == stationId && x.tarih.Year == date.Year && x.tarih.Month == date.Month && x.tarih.Day == date.Day && x.tarih.Hour != date.Hour)
                .OrderByDescending(x => x.tarih).Select(x => x.Enerji).FirstOrDefault();

            float? isinimGunlukOrtalama = pr2.Sum(x => x.saatlikIsinimOrt);

            var installedDC = DB.Stations.Where(a => a.ID == stationId).Select(a => a.DC_INSTALLED_POWER).FirstOrDefault();

            var ee = energy / isinimGunlukOrtalama / installedDC * 100;

            return View(stationId);
        }

        public JsonResult GetLineChart(string beginDate, int stationId)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("tr-TR");
            DateTime reqDateParam = DateTime.Parse(@beginDate);
            OZET_DTO oDTO = new OZET_DTO();
            var stDetail = DB.Stations.Where(a => a.ID == stationId).FirstOrDefault();

            float? scale;
            if (stDetail.IRRADIATION_SCALE != null)
            {
                scale = (stDetail.AC_INSTALLED_POWER) + ((stDetail.AC_INSTALLED_POWER) * (stDetail.IRRADIATION_SCALE) / 100);
            }
            else
            {
                scale = null;
            }

            float? acInstalled = stDetail.AC_INSTALLED_POWER;



            var ozetler = DB.Summaries.Where(p => p.STATION_ID == stationId
            && p.tarih.Year == reqDateParam.Year && p.tarih.Month == reqDateParam.Month && p.tarih.Day == reqDateParam.Day)
                .OrderBy(a => a.tarih).ThenBy(t=>t.Id).Select(a =>
                 new TBL_OZET_DTO
                 {
                     _id = a.Id,
                     _tarih = a.tarih,
                     _gunlukUretim = a.gunlukUretim <= 0 ? 0 : (float)Math.Round((double)a.gunlukUretim / 1000, 1),
                     _dcGuc = a.Dc_Guc,
                     _enerji = a.Enerji <= 0f ? 0f : (float)Math.Round((double)a.Enerji / 1000000, 2),
                     _isinim = (float)Math.Round((double)a.isinim, 1)
                 }
                ).ToList();

            if (ozetler.Count > 0)
            {
                //var endRegister = ozetler[ozetler.Count - 1];

                DateTime abFirstDate = reqDateParam;
                DateTime abLastDate = reqDateParam.AddHours(23).AddMinutes(59).AddSeconds(59);
                if (ozetler != null && ozetler.Count > 0)
                {
                    DateTime lastDate = ozetler[ozetler.Count - 1]._tarih;
                    DateTime FirsDate = ozetler[0]._tarih;

                    while (abFirstDate < FirsDate)
                    {
                        TBL_OZET_DTO ozet = new TBL_OZET_DTO();
                        ozet._tarih = abFirstDate;
                        ozetler.Add(ozet);
                        abFirstDate = abFirstDate.AddMinutes(5);
                    }

                    while (abLastDate > lastDate)
                    {
                        lastDate = lastDate.AddMinutes(5);
                        TBL_OZET_DTO ozet = new TBL_OZET_DTO();
                        ozet._tarih = lastDate;
                        ozetler.Add(ozet);
                    }
                    ozetler = ozetler.OrderBy(x => x._tarih).ToList();
                }

                //float? tempMax = 0;
                //int _idDelete = 0;
                //bool isDelete = false;
                for (int i = 0; i < ozetler.Count(); i++)
                {
                    ozetler[i]._max = stDetail.AC_INSTALLED_POWER;

                    if ((ozetler[i]._dcGuc <= 0 && ozetler[i]._isinim <= 1) || (ozetler[i]._isinim == null && ozetler[i]._dcGuc == null))
                    {
                        ozetler[i]._enerji = null;
                    }

                    //if (!isDelete)
                    //{
                    //    if (ozetler[i]._enerji > 0)
                    //    {
                    //        if (ozetler[i]._enerji > tempMax)
                    //        {
                    //            tempMax = ozetler[i]._enerji;
                    //        }
                    //        else if (ozetler[i]._enerji < tempMax)
                    //        {
                    //            _idDelete = ozetler[i]._id;
                    //            isDelete = true;
                    //            var ff = ozetler[i]._tarih;
                    //        }
                    //        else
                    //        {

                    //        }
                    //    }
                    //}
                }

                //var deleteList = ozetler.Where(d => d._id <= _idDelete).OrderBy(o => o._id).ToList();
                //foreach (var idD in deleteList)
                //{
                //    idD._enerji = null;
                //}


                oDTO._ozet = ozetler;
                oDTO._irradiationScale = scale;
                oDTO._acInstalledPower = acInstalled;
            }
            return Json(oDTO, JsonRequestBehavior.AllowGet);
        }
        public float GetTargetFunc(int stationId, int _ay)
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

        public float GetMonthlyTargetFunc(int stationId, int _ay)
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
        class PRDTO
        {
            public float? isinim { get; set; }
            public float? enerji { get; set; }
            public float? saatlikIsinimOrt { get; set; }
            public int recCount { get; set; }
            public DateTime tarih { get; set; }
            public string kTarih { get; set; }
            public float? PR { get; set; }
        }

        public JsonResult GetDailyProduction(string beginMonth, string beginYear, int stationId)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("tr-TR");
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
            int ay = Int32.Parse(beginMonth);
            int yil = Int32.Parse(beginYear);
            float target = GetTargetFunc(stationId, ay);

            var _stationDetail = DB.Stations.Where(a => a.ID == stationId).FirstOrDefault();
            var installedDC = _stationDetail.DC_INSTALLED_POWER;
            var _stExchange = _stationDetail.EXCHANGE_RATE;

            TBL_PR_DTO mm = new TBL_PR_DTO();
            mm._target = target;
            var prList = (from p in DB.PRSum
                          where p.STATION_ID == stationId && p.date.Value.Month == ay
                          && p.date.Value.Year == yil && p.STATION_ID == stationId
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
        [HttpGet]
        public FileContentResult ExportToExcelDaily(int stationId, string beginMonth, string beginYear)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("tr-TR");
            var userId = User.Identity.GetUserId();
            var stat = DB.Stations.Where(w => w.ID == stationId).FirstOrDefault();
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
            int ay = Int32.Parse(beginMonth);
            int yil = Int32.Parse(beginYear);
            float target = GetTargetFunc(stationId, ay);

            var _stationDetail = DB.Stations.Where(a => a.ID == stationId).FirstOrDefault();
            var installedDC = _stationDetail.DC_INSTALLED_POWER;
            var _stExchange = _stationDetail.EXCHANGE_RATE;

            TBL_PR_DTO mm = new TBL_PR_DTO();
            mm._target = target;
            var prList = (from p in DB.PRSum
                          where p.STATION_ID == stationId && p.date.Value.Month == ay
                          && p.date.Value.Year == yil && p.STATION_ID == stationId
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

            var exportData = mm.listPR.OrderBy(o => o._tarih).Select(s=>new DAILY_PRODUCTION_EXPORT_MODEL {
                DATE=s._tarih,
                ENERGY=Math.Round((float)s._enerji,2),
                IRRADIATION= Math.Round((float)s._IrradiationSum,2),
                PR= Math.Round((float)s._pr,1),
                INCOME_US=Math.Round((float)s._incomeUS,2),
                INCOME_TL= Math.Round((float)s._incomeTL,2),
                FOREX_BUYING= Math.Round((float)s._exchangeTL,4)
            }).ToList();


        
    
            string fileName = stat.NAME.ToUpper() + " DAILY_PRODUCTION.xlsx";
            string[] columns = { "DATE", "ENERGY", "IRRADIATION", "PR", "INCOME_US", "INCOME_TL", "FOREX_BUYING" };
            byte[] filecontent = ExcelExportHelper.ExportExcel(exportData, stat.NAME, true, columns);
            return File(filecontent, ExcelExportHelper.ExcelContentType, fileName);
        }

        public JsonResult GetWeeklyProduction(int stationId)
        {
            DateTime nowDate = DateTime.Now;
            int ay = Int32.Parse(nowDate.Month.ToString());
            float target = GetTargetFunc(stationId, ay);
            var installedDC = DB.Stations.Where(a => a.ID == stationId).Select(a => a.DC_INSTALLED_POWER).FirstOrDefault();
            TBL_PR_DTO2 mm = new TBL_PR_DTO2();
            mm._target = target;
            mm.listPR = (from p in DB.PRSum
                         where p.STATION_ID == stationId
                         orderby p.date descending
                         select new TBL_Week
                         {
                             _tarih = p.date.Value,
                             _enerji = p.enerji == null ? 0 : (float)Math.Round((float)p.enerji, 2),
                             _pr = p.pr == null ? 0 : (float)Math.Round((float)p.pr, 1)
                         }).Take(7).OrderBy(a => a._tarih).ToList();
            for (int i = 0; i < mm.listPR.Count; i++)
            {
                string cultureName = null;
                HttpCookie cultureCookie = Request.Cookies["_culture"];
                if (cultureCookie != null)
                    cultureName = cultureCookie.Value;
                else
                    cultureName = Request.UserLanguages != null && Request.UserLanguages.Length > 0 ? Request.UserLanguages[0] : null;
                CultureInfo convertCulture = new CultureInfo(cultureName);
                mm.listPR[i]._dayName = mm.listPR[i]._tarih.ToString("dddd", convertCulture);
            }

            if (mm.listPR.Count == 0)
            {
                mm.ortalamaPR = 0;
            }
            else
            {
                for (int i = 0; i < mm.listPR.Count; i++)
                {
                    mm.toplamEnerji += mm.listPR[i]._enerji.Value;
                }
                mm.ortalamaPR = mm.listPR.Where(a => a._pr > 0).FirstOrDefault() == null ? 0 : mm.listPR.Where(a => a._pr > 0).Average(a => a._pr).Value;
            }

            return Json(mm, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AnnualyExcelAsDownload()
        {
            return View(Session["AnnualyData"] as List<TBL_PR_MONTH>);
        }
        public JsonResult EndProductionData(int stationId)
        {
            DateTime begindt = Convert.ToDateTime(DateTime.Now);
            var endData = graphicService.GetProductionGraphic(t => t.STATION_ID == stationId && t.tarih.Year == begindt.Year && t.tarih.Month == begindt.Month && t.tarih.Day == begindt.Day).ToList();
            List<EndProductionModel> q = Mapper.Map<List<TBL_OZET>, List<EndProductionModel>>(endData).OrderByDescending(a => a._tarih).Take(1).ToList();
            return Json(q, JsonRequestBehavior.AllowGet);
        }
        public bool IsFirstMonth(int stationId)
        {
            var stt = DB.Stations.Where(b => b.ID == stationId).FirstOrDefault().START_DATE.Value;
            DateTime date = DateTime.Now;
            var gunSayisi = (date - stt).TotalDays;
            if (gunSayisi < 31)
            {
                return false;
            }
            else
            {
                return false;
            }
        }

        public JsonResult ProductionInf(int stationId)
        {
            float monthlySum = 0;
            float annualSum = 0;
            float totalSum = 0;
            float price = 0;
            float taxPrice = 0;
            float indDC = 0;
            float target = 0;
            ProductionModel q = new ProductionModel();
            var stat = DB.Stations.Where(a => a.ID == stationId).FirstOrDefault();
            DateTime startDate = stat.START_DATE.Value;
            DateTime begindt = Convert.ToDateTime(DateTime.Now);
            bool? money = false;
            var userId = User.Identity.GetUserId();
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
            try
            {
               
                var endData = DB.Summaries.Where(a => a.STATION_ID == stationId && a.tarih.Year == begindt.Year
                && a.tarih.Month == begindt.Month && a.tarih.Day == begindt.Day).OrderByDescending(a => a.tarih).Take(1).FirstOrDefault();

                var todayProduction = DB.PRSum.Where(w => w.STATION_ID == stationId && w.date.Value.Year== begindt.Year && w.date.Value.Month == begindt.Month && w.date.Value.Day == begindt.Day).FirstOrDefault();
                q._dailyProduction = todayProduction == null ? 0 : todayProduction.enerji;
                q._isinim = endData == null || endData.isinim==null ? 0 : (float)Math.Round((double)endData.isinim,1);
                q._ruzgar = endData == null || endData.ruzgarHizi == null ? 0 : (float)Math.Round((double)endData.ruzgarHizi,1);
                q._hucreSicakligi = endData == null || endData.ruzgarHizi == null ? 0 : (float)Math.Round((double)endData.hucreSicakligi, 1);
                q._ortamSicakligi = endData == null || endData.ruzgarHizi == null ? 0 : (float)Math.Round((double)endData.sicaklik, 1);
                q.H2_P = endData==null || endData.H2_P == null ? 0 : (float)Math.Round((double)endData.H2_P, 1);
                var GucAc = endData == null || endData.gunlukUretim == null || endData.gunlukUretim <= 0  ? 0 : (float)Math.Round((double)endData.gunlukUretim / 1000, 2);
                q._pdc = endData == null || endData.Dc_Guc == null ? 0 : (float)Math.Round((double)endData.Dc_Guc / 1000, 2);
                q._pac = GucAc;
                price = stat.EXCHANGE_RATE;
                taxPrice = stat.TAX.Value;
                indDC = stat.DC_INSTALLED_POWER.Value;
                var mnth = Convert.ToInt32(begindt.Month);
                target = GetTargetFunc(stationId, mnth);

                var inverterActiveSum = DB.stationSummary.Where(w => w.STATION_ID == stationId).FirstOrDefault();

                q.INV_SUM_ACTIVE_COUNT = (inverterActiveSum.INV_COUNT.Value - inverterActiveSum.PASIVE_INV_COUNT.Value).ToString() + " / " + (inverterActiveSum.INV_COUNT.Value);


                var PrTempList= DB.PRSum.Where(p => p.STATION_ID == stationId && p.date.Value >= startDate).ToList();
                var monthlyList= PrTempList.Where(p => p.STATION_ID == stationId && p.date.Value.Year == begindt.Year && p.date.Value.Month == begindt.Month).ToList();
                monthlySum = monthlyList.Sum(s => s.enerji).Value;
                
                var annualList = PrTempList.Where(p => p.STATION_ID == stationId && p.date.Value.Year == begindt.Year && p.date.Value >= startDate.Date).ToList();
                annualSum = annualList.Sum(a => a.enerji).Value;
                var totalList = PrTempList.Where(p => p.STATION_ID == stationId && p.date.Value >= startDate.Date).ToList();
                totalSum = totalList.Sum(a => a.enerji).Value;

                q._dailyPr = todayProduction == null || todayProduction.pr == null ? 0 : Math.Round(todayProduction.pr.Value,2);

                var montlyPR= monthlyList.Where(w => w.pr > 0).Average(s => s.pr);
                q._monthlyPr = montlyPR==null ? 0 : Math.Round(montlyPR.Value,2);
                var annualPR = annualList.Where(w => w.pr > 0).Average(s => s.pr);
                q._annualPr = annualPR == null ? 0 : Math.Round(annualPR.Value,2);
                var TotalPR = totalList.Where(w => w.pr > 0).Average(s => s.pr);
                q._totalPr = TotalPR == null ? 0 : Math.Round(TotalPR.Value, 2);


                var installed_AC = DB.Stations.Where(a => a.ID == stationId).Select(a => a.AC_INSTALLED_POWER).FirstOrDefault();

                var dailyProduction = todayProduction == null || todayProduction.enerji == null ? 0 : Math.Round(todayProduction.enerji.Value, 2);
                q._dailyKF = ((dailyProduction*1000) / (DateTime.Now.Hour * installed_AC))*100;
                var montlyProduction = monthlyList.Where(w => w.enerji > 0).Sum(s => s.enerji);
                var montlyProduction2 = montlyProduction == null ? 0 : Math.Round(montlyProduction.Value, 2);
                q._monthlyKF = ((montlyProduction2*1000) / ((((DateTime.Now.Day-1) *24)+(DateTime.Now.Hour)) * installed_AC))*100;
                var annualProduction = annualList.Where(w => w.enerji > 0).Sum(s => s.enerji);
                var annualProduction2 = annualProduction == null ? 0 : Math.Round(annualProduction.Value, 2);
                q._annualKF = ((annualProduction2*1000) / ((((DateTime.Now.Month-1) *30*24)+(DateTime.Now.Day * 24))* installed_AC))*100;

                q.isEKK = stat.IS_EKK == null ? false : stat.IS_EKK;
                q.isMeteorology = stat.IS_METEOROLOGY == null ? false : stat.IS_METEOROLOGY;
                q.insAC = stat.AC_INSTALLED_POWER == null ? 0 : stat.AC_INSTALLED_POWER;
                q._dailyIncome = money==false ? 0 : (q._dailyProduction * price * 1000);
                q._monthlyProduction = monthlySum;
                q._monthlyIncome = money == false ? 0 : (monthlySum * price * 1000);
                q._annualProduction = annualSum;
                q._annualIncome = money == false ? 0 : (annualSum * price * 1000);
                q._totalProduction = totalSum;
                q._totalIncome = money == false ? 0 : (totalSum * price * 1000);
                q._specificYield = todayProduction==null || todayProduction.enerji == null || indDC == null ? 0 : (float)Math.Round((double)endData.Enerji / 1000 / indDC, 2);
                q._actualValue =target== null || target==0 ? 0 : (q._dailyProduction * 100) / target;
                q._dailyTax = GucAc * taxPrice;
                q._monthlTax =  money == false ? 0 :(monthlySum * taxPrice * 1000);
                q._annualTax = money == false ? 0 : (annualSum * taxPrice * 1000);
                q._totalTax = money == false ? 0 : (totalSum * taxPrice * 1000);
            }
            catch (Exception ex)
            {

                annualSum = DB.PRSum.Where(p => p.STATION_ID == stationId && p.date.Value.Year == begindt.Year && p.date.Value > startDate.Date).Sum(a => a.enerji).Value;
                totalSum = DB.PRSum.Where(p => p.STATION_ID == stationId && p.date.Value > startDate.Date).Sum(a => a.enerji).Value;
                monthlySum = monthlySum;
                annualSum = annualSum;
                totalSum = totalSum;
                q._dailyIncome = money == false ? 0 : (q._dailyProduction * price * 1000);
                q._monthlyProduction = monthlySum;
                q._monthlyIncome = money == false ? 0 : (monthlySum * price * 1000);
                q._annualProduction = annualSum;
                q._annualIncome = money == false ? 0 : (annualSum * price * 1000);
                q._totalProduction = totalSum;
                q._totalIncome = money == false ? 0 : (totalSum * price * 1000);
                q._specificYield = (q._dailyProduction * 1000) / indDC;
                q._actualValue = (q._dailyProduction * 100) / target;
                q._dailyTax = money == false ? 0 : (q._dailyProduction * taxPrice * 1000);
                q._monthlTax = money == false ? 0 : (monthlySum * taxPrice * 1000);
                q._annualTax = money == false ? 0 : (annualSum * taxPrice * 1000);
                q._totalTax = money == false ? 0 : (totalSum * taxPrice * 1000);

            }

            return Json(q, JsonRequestBehavior.AllowGet);

        }

        public class ProductionEndData
        {
            public int _stationId { get; set; }
            public DateTime _date { get; set; }
            public float _dailyEnergy { get; set; }
            public float _monthlyEnergy { get; set; }
            public float _annualEnergy { get; set; }
            public float _totalEnergy { get; set; }
            public float _dailyIncome { get; set; }
            public float _monthlyIncome { get; set; }
            public float _annualIncome { get; set; }
            public float _totalIncome { get; set; }
            public float _dailyTax { get; set; }
            public float _monthlyTax { get; set; }
            public float _annualTax { get; set; }
            public float _totalTax { get; set; }
            public float _irradiation { get; set; }
            public float _dcPower { get; set; }
            public float _acPower { get; set; }
            public float _PR { get; set; }
            public float _irradationAVG { get; set; }
            public float _specificYield { get; set; }
            public float _actualValue { get; set; }
            public float _externalTemp { get; set; }
            public float _cellTemp { get; set; }
            public float _wind { get; set; }
        }
        public ActionResult ExportExcel(int stationId)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("tr-TR");
            int start = Convert.ToInt32(Request["start"]);
            int length = Convert.ToInt32(Request["length"]);
            string sortColumnName = Request["columns[" + Request["order[0][column]"] + "][data]"];
            string sortDirection = Request["order[0][dir]"];
            var role = User.IsInRole("DEMO");
            MemoryStream result = new MemoryStream();
            string excelreportPath = DateTime.Now + ".AylıkRapor.xlsx";
            Thread.CurrentThread.CurrentCulture = new CultureInfo("tr-TR");

            using (EssoEntities db = new EssoEntities())
            {
                var Station = (from sT in db.Stations
                               where sT.ID == stationId
                               select new
                               {

                                   ID = sT.ID,

                                   STATION_ID = sT.GROUP_ID,
                                   INSTALLED_POWER = sT.INSTALLED_POWER,
                                   INSTALL_DATE = sT.INSTALL_DATE,
                                   INVERTER_MODEL = sT.INVERTER_MODEL,
                                   INVERTER_TYPE = sT.INVERTER_TYPE,
                                   IP_ADDRESS = sT.IP_ADDRESS,
                                   IRRADIATION_SCALE = sT.IRRADIATION_SCALE,
                                   IS_CENTRAL_INV = sT.IS_CENTRAL_INV,
                                   IS_EKK = sT.IS_EKK,
                                   CREATED_DATE = sT.CREATED_DATE,
                                   IS_METEOROLOGY = sT.IS_METEOROLOGY,
                                   IS_PYRANOMETER = sT.IS_PYRANOMETER,
                                   NAME = sT.NAME,
                                   INSTALLED_POWER_AC = sT.AC_INSTALLED_POWER,
                                   INSTALLED_POWER_DC = sT.DC_INSTALLED_POWER,
                                   PANEL_BRAND = sT.PANEL_BRAND,
                                   PITCH_DETAIL = sT.PITCH_DETAIL,
                                   ORIENTATION = sT.ORIENTATION,
                                   PANEL_TYPE = sT.PANEL_TYPE,
                                   START_DATE = sT.START_DATE,
                                   GROUP_ID = sT.GROUP_ID,
                                   COMPANY_ID = sT.COMPANY_ID


                               }).ToList();


                using (ExcelPackage pck = new ExcelPackage())
                {
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add(Resources.Station_Detail);
                    ExcelWorksheet ws2 = pck.Workbook.Worksheets.Add(Resources.Daily_Production);
                    ExcelWorksheet ws3 = pck.Workbook.Worksheets.Add(Resources.Monthly_Production);
                    ExcelWorksheet ws4 = pck.Workbook.Worksheets.Add(Resources.Ekk_Energy_Index_Values);
                    ExcelWorksheet ws5 = pck.Workbook.Worksheets.Add(Resources.Montly_Fence_Company_Compare);
                    ExcelWorksheet ws6 = pck.Workbook.Worksheets.Add(Resources.Monthly+" Alarm");


                    ws.Column(1).Width = 30;
                    ws.Column(2).Width = 30;
                    var path = HttpContext.Server.MapPath("//images//StationImages//" + Station[0].ID + ".png");
                    try
                    {
                        //Image image = Image.FromFile(HttpContext.Server.MapPath("//images//StationImages//" + Station[0].ID + ".png"));
                        using (System.Drawing.Image image = System.Drawing.Image.FromFile(HttpContext.Server.MapPath("//images//StationImages//" + Station[0].ID + ".png")))
                        {
                            var excelImage = ws.Drawings.AddPicture("Station İmage", image);

                            //add the image to row 20, column E
                            excelImage.SetPosition(0, 0, 3, 0);
                            excelImage.SetSize(75);
                        }
                    }
                    catch (Exception ex)
                    {

                    }


                    ws.Cells["A1"].Value = Resources.Installed_Power + " AC (kWe)";
                    ws.Cells["A2"].Value = Resources.Installed_Power + " DC (kWp)";
                    ws.Cells["A3"].Value = "DC/AC";
                    ws.Cells["A4"].Value = Resources.Tilt;
                    ws.Cells["A5"].Value = Resources.Azimuth;
                    ws.Cells["A6"].Value = "Inverter " + Resources.Manufacturer;
                    ws.Cells["A7"].Value = Resources.Module_Manufacturer;
                    ws.Cells["A8"].Value = Resources.Module_Type;
                    ws.Cells["A9"].Value = Resources.Start_Date;
                    ws.Cells["A10"].Value = Resources.Station + " " + Resources.Name;

                    ws.Cells["A1"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    ws.Cells["A2"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    ws.Cells["A3"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    ws.Cells["A4"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    ws.Cells["A5"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    ws.Cells["A6"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    ws.Cells["A7"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    ws.Cells["A8"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    ws.Cells["A9"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    ws.Cells["A10"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;





                    ws.Cells["A1"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(string.Format("#6aa5e4")));
                    ws.Cells["A2"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(string.Format("#6aa5e4")));
                    ws.Cells["A3"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(string.Format("#6aa5e4")));
                    ws.Cells["A4"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(string.Format("#6aa5e4")));
                    ws.Cells["A5"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(string.Format("#6aa5e4")));
                    ws.Cells["A6"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(string.Format("#6aa5e4")));
                    ws.Cells["A7"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(string.Format("#6aa5e4")));
                    ws.Cells["A8"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(string.Format("#6aa5e4")));
                    ws.Cells["A9"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(string.Format("#6aa5e4")));
                    ws.Cells["A10"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(string.Format("#6aa5e4")));

                    ws.Cells["B1"].Style.Numberformat.Format = "#,##0.00";
                    ws.Cells["B2"].Style.Numberformat.Format = "#,##0.00";
                    ws.Cells["B3"].Style.Numberformat.Format = "#,##0.00";
                    ws.Cells["B4"].Style.Numberformat.Format = "#,##0.00";
                    ws.Cells["B5"].Style.Numberformat.Format = "#,##0.00";
                    ws.Cells["B6"].Style.Numberformat.Format = "#,##0.00";
                    ws.Cells["B7"].Style.Numberformat.Format = "#,##0.00";
                    ws.Cells["B8"].Style.Numberformat.Format = "#,##0.00";
                    ws.Cells["B9"].Style.Numberformat.Format = "#,##0.00";
                    ws.Cells["B10"].Style.Numberformat.Format = "#,##0.00";




                    ws.Cells["B1"].Value = Station[0].INSTALLED_POWER_AC.ToString();
                    ws.Cells["B2"].Value = Station[0].INSTALLED_POWER_DC.ToString();
                    ws.Cells["B3"].Value = (Station[0].INSTALLED_POWER_AC / Station[0].INSTALLED_POWER_AC).ToString();
                    ws.Cells["B4"].Value = Station[0].PITCH_DETAIL.ToString();
                    ws.Cells["B5"].Value = Station[0].ORIENTATION.ToString();
                    ws.Cells["B6"].Value = Station[0].INVERTER_MODEL.ToString();
                    ws.Cells["B7"].Value = Station[0].INVERTER_MODEL.ToString();
                    ws.Cells["B8"].Value = Station[0].PANEL_TYPE.ToString();
                    ws.Cells["B9"].Value = Station[0].START_DATE.Value.ToString("dd/MM/yyyy HH:mm:ss");
                    ws.Cells["B10"].Value = Station[0].NAME.ToString();

                    using (ExcelRange range = ws.Cells["A1:B10"])
                    {
                        range.Style.Border.Top.Style = ExcelBorderStyle.Medium;
                        range.Style.Border.Left.Style = ExcelBorderStyle.Medium;
                        range.Style.Border.Right.Style = ExcelBorderStyle.Medium;
                        range.Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                        range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    }


                    //Worksheet 2


                    int ay = Int32.Parse(DateTime.Now.Month.ToString());
                    int yil = Int32.Parse(DateTime.Now.Year.ToString());
                    float target = GetTargetFunc(stationId, ay);

                    var _stationDetail = DB.Stations.Where(a => a.ID == stationId).FirstOrDefault();
                    var installedDC = _stationDetail.DC_INSTALLED_POWER;
                    var _stExchange = _stationDetail.EXCHANGE_RATE;

                    TBL_PR_DTO mm = new TBL_PR_DTO();
                    mm._target = target;
                    var prList = (from p in DB.PRSum
                                  where p.STATION_ID == stationId && p.date.Value.Month == ay
                                  && p.date.Value.Year == yil && p.STATION_ID == stationId
                                  orderby p.date
                                  select p).ToList();

                    var howDays = DateTime.DaysInMonth(yil, ay);




                    mm.listPR = (from p in prList.ToList()

                                 select new TBL_PR
                                 {
                                     _tarih = p.date.Value,
                                     _enerji = p.enerji == null ? 0 : (float)Math.Round((double)p.enerji, 2),
                                     _IrradiationSum = p.isinim_ortalama == null ? 0 : (float)Math.Round((double)p.isinim_ortalama, 2),
                                     _pr = p.pr == null ? 0 : (float)Math.Round((double)p.pr, 1),

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

                            mm.toplamEnerji += mm.listPR[i]._enerji.Value;
                            mm.toplamIsinim += mm.listPR[i]._IrradiationSum.Value;

                        }
                        mm.ortalamaPR = mm.listPR.Where(a => a._pr > 0).FirstOrDefault() == null ? 0 : mm.listPR.Where(a => a._pr > 0).Average(a => a._pr).Value;
                    }


                    ws2.Column(2).Width = 30;
                    ws2.Column(3).Width = 35;
                    ws2.Column(4).Width = 35;
                    ws2.Column(5).Width = 35;

                    ws2.Select("B2:B3");
                    ws2.SelectedRange.Merge = true;

                    ws2.Select("C2:C3");
                    ws2.SelectedRange.Merge = true;

                    ws2.Select("D2:D3");
                    ws2.SelectedRange.Merge = true;

                    ws2.Select("E2:E3");
                    ws2.SelectedRange.Merge = true;

                    ws2.Cells["B2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws2.Cells["C2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws2.Cells["D2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws2.Cells["E2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws2.Cells["B2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws2.Cells["C2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws2.Cells["D2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws2.Cells["E2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws2.Cells["C" + (mm.listPR.Count + 4)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    ws2.Cells["B2"].Value = Resources.Date;
                    ws2.Cells["C2"].Value = Resources.Energy + " (MWh)";
                    ws2.Cells["D2"].Value = "PR %";
                    ws2.Cells["E2"].Value = Resources.Irradiation + " (Wh/m2)";

                    using (ExcelRange range = ws2.Cells["B2:E" + (mm.listPR.Count + 3)])
                    {
                        range.Style.Border.Top.Style = ExcelBorderStyle.Medium;
                        range.Style.Border.Left.Style = ExcelBorderStyle.Medium;
                        range.Style.Border.Right.Style = ExcelBorderStyle.Medium;
                        range.Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

                    }

                    for (int i = 0; i < mm.listPR.Count; i++)
                    {

                        ws2.Cells["B" + (i + 4)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        ws2.Cells["C" + (i + 4)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        ws2.Cells["D" + (i + 4)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        ws2.Cells["E" + (i + 4)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        ws2.Cells["B" + (i + 4)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        ws2.Cells["C" + (i + 4)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        ws2.Cells["D" + (i + 4)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        ws2.Cells["E" + (i + 4)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                        ws2.Cells["C" + (i + 4)].Style.Numberformat.Format = "#,##0.00";
                        ws2.Cells["D" + (i + 4)].Style.Numberformat.Format = "#,##0.00";
                        ws2.Cells["E" + (i + 4)].Style.Numberformat.Format = "#,##0.00";

                        ws2.Cells["B" + (i + 4)].Value = mm.listPR[i]._tarih.ToString("dd/MM/yyyy HH:mm");
                        ws2.Cells["C" + (i + 4)].Value = mm.listPR[i]._enerji;
                        ws2.Cells["D" + (i + 4)].Value = mm.listPR[i]._pr;
                        ws2.Cells["E" + (i + 4)].Value = mm.listPR[i]._IrradiationSum;

                    }


                    DateTime nowDate = DateTime.Now;

                    TBL_PR_MONTH_DTO pmD = new TBL_PR_MONTH_DTO();
                    pmD.listPR = (from i in DB.PRSum
                                  where i.STATION_ID == stationId && i.date.Value.Year == yil
                                  group i by i.date.Value.Month into grp
                                  select new TBL_PR_MONTH
                                  {
                                      energy = (float)Math.Round(grp.Sum(x => x.enerji.Value), 1),
                                      month = grp.Max(a => a.date.Value.Month),
                                      irradiationSum = (float)Math.Round(grp.Sum(x => x.isinim_ortalama.Value), 1),
                                      pr = grp.Where(x => x.pr > 0).Average(x => x.pr) == null ? 0 : (float)Math.Round(grp.Where(x => x.pr > 0).Average(x => x.pr.Value), 1)
                                  }).OrderBy(a => a.month).ToList();

                    for (int i = 0; i < pmD.listPR.Count; i++)
                    {

                        pmD.toplamEnerji += pmD.listPR[i].energy.Value == 0 ? 0 : pmD.listPR[i].energy.Value;
                        pmD.toplamIsinim += pmD.listPR[i].irradiationSum.Value == 0 ? 0 : pmD.listPR[i].irradiationSum.Value;
                        pmD.ortalamaPR += pmD.listPR[i].pr.Value == 0 ? 0 : pmD.listPR[i].pr.Value;
                        pmD.listPR[i].target = GetMonthlyTargetFunc(stationId, pmD.listPR[i].month);

                    }

                    var listTRG = new List<float>();
                    for (int i = 0; i < 12; i++)
                    {
                        listTRG.Add(GetMonthlyTargetFunc(stationId, i + 1));
                    }
                    pmD.listTarget = listTRG;
                    if (pmD.listPR.Count == 0)
                    {
                        pmD.ortalamaEnerji = 0;
                        pmD.ortalamaIsinim = 0;
                        pmD.ortalamaPR = 0;
                    }
                    else
                    {
                        pmD.ortalamaEnerji = pmD.toplamEnerji / pmD.listPR.Count;
                        pmD.ortalamaIsinim = pmD.toplamIsinim / pmD.listPR.Count;
                        pmD.ortalamaPR = pmD.ortalamaPR / pmD.listPR.Count;
                    }

                    ws3.Column(2).Width = 30;
                    ws3.Column(3).Width = 35;
                    ws3.Column(4).Width = 35;
                    ws3.Column(5).Width = 35;
                    ws3.Column(6).Width = 35;

                    ws3.Select("B2:B3");
                    ws3.SelectedRange.Merge = true;

                    ws3.Select("C2:C3");
                    ws3.SelectedRange.Merge = true;

                    ws3.Select("D2:D3");
                    ws3.SelectedRange.Merge = true;

                    ws3.Select("E2:E3");
                    ws3.SelectedRange.Merge = true;

                    ws3.Select("F2:F3");
                    ws3.SelectedRange.Merge = true;

                    ws3.Cells["B2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws3.Cells["C2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws3.Cells["D2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws3.Cells["E2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws3.Cells["F2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws3.Cells["B2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws3.Cells["C2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws3.Cells["D2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws3.Cells["E2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws3.Cells["F2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws3.Cells["C" + (pmD.listPR.Count + 4)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    ws3.Cells["B2"].Value = Resources.Month;
                    ws3.Cells["C2"].Value = Resources.Energy_Produced + " (MWh)";
                    ws3.Cells["D2"].Value = Resources.Irradiation + " (Wh/m2)";
                    ws3.Cells["E2"].Value = "PR %";
                    ws3.Cells["F2"].Value = Resources.Target + " (MWh)";
                    using (ExcelRange range = ws3.Cells["B2:F" + (pmD.listPR.Count + 3)])
                    {
                        range.Style.Border.Top.Style = ExcelBorderStyle.Medium;
                        range.Style.Border.Left.Style = ExcelBorderStyle.Medium;
                        range.Style.Border.Right.Style = ExcelBorderStyle.Medium;
                        range.Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

                    }

                    for (int i = 0; i < pmD.listPR.Count; i++)
                    {

                        ws3.Cells["B" + (i + 4)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        ws3.Cells["C" + (i + 4)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        ws3.Cells["D" + (i + 4)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        ws3.Cells["E" + (i + 4)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        ws3.Cells["F" + (i + 4)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        ws3.Cells["B" + (i + 4)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        ws3.Cells["C" + (i + 4)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        ws3.Cells["D" + (i + 4)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        ws3.Cells["E" + (i + 4)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        ws3.Cells["F" + (i + 4)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;



                        ws3.Cells["C" + (i + 4)].Style.Numberformat.Format = "#,##0.00";
                        ws3.Cells["D" + (i + 4)].Style.Numberformat.Format = "#,##0.00";
                        ws3.Cells["E" + (i + 4)].Style.Numberformat.Format = "#,##0.00";
                        ws3.Cells["F" + (i + 4)].Style.Numberformat.Format = "#,##0.00";

                        ws3.Cells["B" + (i + 4)].Value = pmD.listPR[i].month;
                        ws3.Cells["C" + (i + 4)].Value = pmD.listPR[i].energy;
                        ws3.Cells["D" + (i + 4)].Value = pmD.listPR[i].irradiationSum;
                        ws3.Cells["E" + (i + 4)].Value = pmD.listPR[i].pr;
                        ws3.Cells["F" + (i + 4)].Value = pmD.listPR[i].target;


                    }


                    var ekk = (from i in DB.Summaries
                               where i.STATION_ID == stationId && i.tarih.Year == yil
                               group i by i.tarih.Month into grp
                               select new TBL_OZET_MONTH
                               {
                                   h2_wp_minus = (float)Math.Round(grp.Max(x => x.H2_WP_minus.Value), 1),
                                   month = grp.Max(a => a.tarih.Month),
                                   h2_wp_plus = (float)Math.Round(grp.Max(x => x.H2_WP_plus.Value), 1),
                               }).OrderBy(a => a.month).ToList();

                    ws4.Column(2).Width = 30;
                    ws4.Column(3).Width = 35;
                    ws4.Column(4).Width = 35;


                    ws4.Select("B2:B3");
                    ws4.SelectedRange.Merge = true;

                    ws4.Select("C2:C3");
                    ws4.SelectedRange.Merge = true;

                    ws4.Select("D2:D3");
                    ws4.SelectedRange.Merge = true;



                    ws4.Cells["B2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws4.Cells["C2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws4.Cells["D2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    ws4.Cells["B2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws4.Cells["C2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws4.Cells["D2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    ws4.Cells["B2"].Value = Resources.Month;
                    ws4.Cells["C2"].Value = Resources.Produced_Energy + "(MWh)";
                    ws4.Cells["D2"].Value = Resources.Consumed_Energy + "(MWh)";


                    using (ExcelRange range = ws4.Cells["B2:D" + (ekk.Count + 3)])
                    {
                        range.Style.Border.Top.Style = ExcelBorderStyle.Medium;
                        range.Style.Border.Left.Style = ExcelBorderStyle.Medium;
                        range.Style.Border.Right.Style = ExcelBorderStyle.Medium;
                        range.Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

                    }

                    for (int i = 0; i < ekk.Count; i++)
                    {

                        ws4.Cells["B" + (i + 4)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        ws4.Cells["C" + (i + 4)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        ws4.Cells["D" + (i + 4)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                        ws4.Cells["B" + (i + 4)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        ws4.Cells["C" + (i + 4)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        ws4.Cells["D" + (i + 4)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        ws4.Cells["C" + (i + 4)].Style.Numberformat.Format = "#,##0.00";
                        ws4.Cells["D" + (i + 4)].Style.Numberformat.Format = "#,##0.00";


                        ws4.Cells["B" + (i + 4)].Value = ekk[i].month.ToString();
                        ws4.Cells["C" + (i + 4)].Value = ekk[i].h2_wp_minus;
                        ws4.Cells["D" + (i + 4)].Value = ekk[i].h2_wp_plus;

                    }
                    var Station_group_id = Station[0].GROUP_ID;



                    var userId = User.Identity.GetUserId();
                    List<TBL_STATION> stations = new List<TBL_STATION>();
                    List<STATION_GRUP_COMPANY> sgcList = new List<STATION_GRUP_COMPANY>();
                    List<STATION_GRUP_COMPANY> sgcGroupList = new List<STATION_GRUP_COMPANY>();
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

                    int[] stationIds = stations.Where(a => a.GROUP_ID == Station[0].GROUP_ID).Select(s => s.ID).ToArray();
                    int[] stationCompanyIds = stations.Where(a => a.COMPANY_ID == Station[0].COMPANY_ID).Select(s => s.ID).ToArray();



                    stations_dto = stations.Where(a => a.IS_DELETED == false && a.IS_ACTIVE == true && a.COMPANY_ID == Station[0].COMPANY_ID).Select(a =>
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

                        sgc.IS_ALARM = false;
                        sgc.IS_METEOROLOGY = item.IS_METEOROLOGY;
                        sgc.IS_MONEY = money;
                        sgc.FINANCIAL_USD = (float)Math.Round((((float)item.EXCHANGE_RATE) * (float)sgc.ENERGY * 1000), 2);
                        sgc.FINANCIAL_TL = 0;
                        //sgc.LIST_CHART = null;
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
                        if (sgc.GROUP_ID == Station[0].GROUP_ID)
                        {
                            sgcGroupList.Add(sgc);
                        }
                    }

                    ws5.Column(2).Width = 30;
                    ws5.Column(3).Width = 35;
                    ws5.Column(4).Width = 35;
                    ws5.Column(5).Width = 35;
                    ws5.Column(6).Width = 35;
                    ws5.Column(7).Width = 35;
                    ws5.Column(8).Width = 35;
                    ws5.Column(9).Width = 35;
                    ws5.Column(10).Width = 35;

                    ws5.Select("B2:B3");
                    ws5.SelectedRange.Merge = true;

                    ws5.Select("C2:C3");
                    ws5.SelectedRange.Merge = true;

                    ws5.Select("D2:D3");
                    ws5.SelectedRange.Merge = true;

                    ws5.Select("E2:E3");
                    ws5.SelectedRange.Merge = true;

                    ws5.Select("F2:F3");
                    ws5.SelectedRange.Merge = true;

                    ws5.Select("G2:G3");
                    ws5.SelectedRange.Merge = true;

                    ws5.Select("H2:H3");
                    ws5.SelectedRange.Merge = true;

                    ws5.Select("I2:I3");
                    ws5.SelectedRange.Merge = true;

                    ws5.Select("J2:J3");
                    ws5.SelectedRange.Merge = true;


                    ws5.Cells["B2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws5.Cells["C2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws5.Cells["D2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws5.Cells["E2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws5.Cells["F2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws5.Cells["G2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws5.Cells["H2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws5.Cells["I2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws5.Cells["J2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    ws5.Cells["B2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws5.Cells["C2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws5.Cells["D2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws5.Cells["E2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws5.Cells["F2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws5.Cells["G2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws5.Cells["H2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws5.Cells["I2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws5.Cells["J2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    ws5.Cells["B" + (sgcList.Count + 4)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                    ws5.Cells["B2"].Value = Resources.Station;
                    ws5.Cells["C2"].Value = Resources.Installed_Power + " (kWp)";
                    ws5.Cells["D2"].Value = "AC " + Resources.Power + " (kW)";
                    ws5.Cells["E2"].Value = Resources.Energy + " (MWh)";
                    ws5.Cells["F2"].Value = Resources.Irradiation + " (Wh/m2)";
                    ws5.Cells["G2"].Value = Resources.Specific_Yield + " (kWh/kWp)";
                    ws5.Cells["H2"].Value = Resources.Active_Power_Sum;
                    ws5.Cells["I2"].Value = "PR %";
                    if (money == true)
                    {
                        ws5.Cells["J2"].Value = Resources.Financial_Income;
                        using (ExcelRange range = ws5.Cells["B2:J" + (sgcList.Count + 3)])
                        {
                            range.Style.Border.Top.Style = ExcelBorderStyle.Medium;
                            range.Style.Border.Left.Style = ExcelBorderStyle.Medium;
                            range.Style.Border.Right.Style = ExcelBorderStyle.Medium;
                            range.Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

                        }

                    }
                    else
                    {

                        using (ExcelRange range = ws5.Cells["B2:I" + (sgcList.Count + 3)])
                        {
                            range.Style.Border.Top.Style = ExcelBorderStyle.Medium;
                            range.Style.Border.Left.Style = ExcelBorderStyle.Medium;
                            range.Style.Border.Right.Style = ExcelBorderStyle.Medium;
                            range.Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

                        }
                    }



                    for (int i = 0; i < sgcList.Count; i++)
                    {

                        ws5.Cells["B" + (i + 4)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        ws5.Cells["C" + (i + 4)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        ws5.Cells["D" + (i + 4)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        ws5.Cells["E" + (i + 4)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        ws5.Cells["F" + (i + 4)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        ws5.Cells["G" + (i + 4)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        ws5.Cells["H" + (i + 4)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        ws5.Cells["I" + (i + 4)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;


                        ws5.Cells["B" + (i + 4)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        ws5.Cells["C" + (i + 4)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        ws5.Cells["D" + (i + 4)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        ws5.Cells["E" + (i + 4)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        ws5.Cells["F" + (i + 4)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        ws5.Cells["G" + (i + 4)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        ws5.Cells["H" + (i + 4)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        ws5.Cells["I" + (i + 4)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;




                        ws5.Cells["B" + (i + 4)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        ws5.Cells["C" + (i + 4)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        ws5.Cells["D" + (i + 4)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        ws5.Cells["E" + (i + 4)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        ws5.Cells["F" + (i + 4)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        ws5.Cells["G" + (i + 4)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        ws5.Cells["H" + (i + 4)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        ws5.Cells["I" + (i + 4)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;

                        if (money == true)
                        {
                            ws5.Cells["J" + (i + 4)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            ws5.Cells["J" + (i + 4)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws5.Cells["J" + (i + 4)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        }

                        if (sgcList[i].STATION_ID == Station[0].ID)
                        {
                            ws5.Cells["B" + (i + 4)].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(string.Format("#0bed96")));
                            ws5.Cells["C" + (i + 4)].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(string.Format("#0bed96")));
                            ws5.Cells["D" + (i + 4)].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(string.Format("#0bed96")));
                            ws5.Cells["E" + (i + 4)].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(string.Format("#0bed96")));
                            ws5.Cells["F" + (i + 4)].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(string.Format("#0bed96")));
                            ws5.Cells["G" + (i + 4)].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(string.Format("#0bed96")));
                            ws5.Cells["H" + (i + 4)].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(string.Format("#0bed96")));
                            ws5.Cells["I" + (i + 4)].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(string.Format("#0bed96")));

                            if (money == true)
                            {
                                ws5.Cells["J" + (i + 4)].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(string.Format("#0bed96")));
                            }
                        }
                        else
                        {
                            ws5.Cells["B" + (i + 4)].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(string.Format("#FFFFFF")));
                            ws5.Cells["C" + (i + 4)].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(string.Format("#FFFFFF")));
                            ws5.Cells["D" + (i + 4)].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(string.Format("#FFFFFF")));
                            ws5.Cells["E" + (i + 4)].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(string.Format("#FFFFFF")));
                            ws5.Cells["F" + (i + 4)].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(string.Format("#FFFFFF")));
                            ws5.Cells["G" + (i + 4)].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(string.Format("#FFFFFF")));
                            ws5.Cells["H" + (i + 4)].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(string.Format("#FFFFFF")));
                            ws5.Cells["I" + (i + 4)].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(string.Format("#FFFFFF")));

                            if (money == true)
                            {
                                ws5.Cells["J" + (i + 4)].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(string.Format("#FFFFFF")));
                            }
                        }


                        ws5.Cells["B" + (i + 4)].Style.Numberformat.Format = "#,##0.00";
                        ws5.Cells["C" + (i + 4)].Style.Numberformat.Format = "#,##0.00";
                        ws5.Cells["D" + (i + 4)].Style.Numberformat.Format = "#,##0.00";
                        ws5.Cells["E" + (i + 4)].Style.Numberformat.Format = "#,##0.00";
                        ws5.Cells["F" + (i + 4)].Style.Numberformat.Format = "#,##0.00";
                        ws5.Cells["G" + (i + 4)].Style.Numberformat.Format = "#,##0.00";
                        ws5.Cells["H" + (i + 4)].Style.Numberformat.Format = "#,##0.00";
                        ws5.Cells["I" + (i + 4)].Style.Numberformat.Format = "#,##0.00";
                        ws5.Cells["J" + (i + 4)].Style.Numberformat.Format = "#,##0.00";

                        ws5.Cells["B" + (i + 4)].Value = sgcList[i].STATION_NAME.ToString();
                        ws5.Cells["C" + (i + 4)].Value = sgcList[i].DC_INSTALLED_POWER;
                        ws5.Cells["D" + (i + 4)].Value = "-";
                        ws5.Cells["E" + (i + 4)].Value = sgcList[i].ENERGY;
                        ws5.Cells["F" + (i + 4)].Value = sgcList[i].IRRADIATION;
                        ws5.Cells["G" + (i + 4)].Value = sgcList[i].SPECIFIC_YIELD;

                        if (sgcList[i].INVERTER_ACTIVE_COUNT == null || sgcList[i].INV_SUM_ACTIVE_COUNT == null)
                        {
                            ws5.Cells["H" + (i + 4)].Value = "-";
                        }
                        else
                        {
                            ws5.Cells["H" + (i + 4)].Value = sgcList[i].INVERTER_ACTIVE_COUNT.ToString() + " / " + sgcList[i].INV_SUM_ACTIVE_COUNT.ToString();
                        }

                        ws5.Cells["I" + (i + 4)].Value = sgcList[i].PR;
                        if (money == true)
                        {
                            ws5.Cells["J" + (i + 4)].Value = sgcList[i].FINANCIAL_USD;
                        }
                    }


                    for (int i = 0; i < sgcGroupList.Count; i++)
                    {

                        ws5.Cells["B" + (sgcList.Count + i + 7)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        ws5.Cells["C" + (sgcList.Count + i + 7)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        ws5.Cells["D" + (sgcList.Count + i + 7)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        ws5.Cells["E" + (sgcList.Count + i + 7)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        ws5.Cells["F" + (sgcList.Count + i + 7)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        ws5.Cells["G" + (sgcList.Count + i + 7)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        ws5.Cells["H" + (sgcList.Count + i + 7)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        ws5.Cells["I" + (sgcList.Count + i + 7)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;


                        ws5.Cells["B" + (sgcList.Count + i + 7)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        ws5.Cells["C" + (sgcList.Count + i + 7)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        ws5.Cells["D" + (sgcList.Count + i + 7)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        ws5.Cells["E" + (sgcList.Count + i + 7)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        ws5.Cells["F" + (sgcList.Count + i + 7)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        ws5.Cells["G" + (sgcList.Count + i + 7)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        ws5.Cells["H" + (sgcList.Count + i + 7)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        ws5.Cells["I" + (sgcList.Count + i + 7)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                        ws5.Cells["B" + (sgcList.Count + i + 7)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        ws5.Cells["C" + (sgcList.Count + i + 7)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        ws5.Cells["D" + (sgcList.Count + i + 7)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        ws5.Cells["E" + (sgcList.Count + i + 7)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        ws5.Cells["F" + (sgcList.Count + i + 7)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        ws5.Cells["G" + (sgcList.Count + i + 7)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        ws5.Cells["H" + (sgcList.Count + i + 7)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        ws5.Cells["I" + (sgcList.Count + i + 7)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;

                        if (money == true)
                        {
                            ws5.Cells["J" + (sgcList.Count + i + 7)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            ws5.Cells["J" + (sgcList.Count + i + 7)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws5.Cells["J" + (sgcList.Count + i + 7)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        }

                        if (sgcGroupList[i].STATION_ID == Station[0].ID)
                        {
                            ws5.Cells["B" + (sgcList.Count + i + 7)].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(string.Format("#0bed96")));
                            ws5.Cells["C" + (sgcList.Count + i + 7)].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(string.Format("#0bed96")));
                            ws5.Cells["D" + (sgcList.Count + i + 7)].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(string.Format("#0bed96")));
                            ws5.Cells["E" + (sgcList.Count + i + 7)].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(string.Format("#0bed96")));
                            ws5.Cells["F" + (sgcList.Count + i + 7)].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(string.Format("#0bed96")));
                            ws5.Cells["G" + (sgcList.Count + i + 7)].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(string.Format("#0bed96")));
                            ws5.Cells["H" + (sgcList.Count + i + 7)].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(string.Format("#0bed96")));
                            ws5.Cells["I" + (sgcList.Count + i + 7)].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(string.Format("#0bed96")));

                            if (money == true)
                            {
                                ws5.Cells["J" + (sgcList.Count + i + 7)].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(string.Format("#0bed96")));
                            }
                        }
                        else
                        {
                            ws5.Cells["B" + (sgcList.Count + i + 7)].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(string.Format("#D8E7F8")));
                            ws5.Cells["C" + (sgcList.Count + i + 7)].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(string.Format("#D8E7F8")));
                            ws5.Cells["D" + (sgcList.Count + i + 7)].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(string.Format("#D8E7F8")));
                            ws5.Cells["E" + (sgcList.Count + i + 7)].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(string.Format("#D8E7F8")));
                            ws5.Cells["F" + (sgcList.Count + i + 7)].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(string.Format("#D8E7F8")));
                            ws5.Cells["G" + (sgcList.Count + i + 7)].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(string.Format("#D8E7F8")));
                            ws5.Cells["H" + (sgcList.Count + i + 7)].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(string.Format("#D8E7F8")));
                            ws5.Cells["I" + (sgcList.Count + i + 7)].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(string.Format("#D8E7F8")));
                            if (money == true)
                            {
                                ws5.Cells["J" + (sgcList.Count + i + 7)].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(string.Format("#D8E7F8")));
                            }
                        }



                        ws5.Cells["B" + (sgcList.Count + i + 7)].Style.Numberformat.Format = "#,##0.00";
                        ws5.Cells["C" + (sgcList.Count + i + 7)].Style.Numberformat.Format = "#,##0.00";
                        ws5.Cells["D" + (sgcList.Count + i + 7)].Style.Numberformat.Format = "#,##0.00";
                        ws5.Cells["E" + (sgcList.Count + i + 7)].Style.Numberformat.Format = "#,##0.00";
                        ws5.Cells["F" + (sgcList.Count + i + 7)].Style.Numberformat.Format = "#,##0.00";
                        ws5.Cells["G" + (sgcList.Count + i + 7)].Style.Numberformat.Format = "#,##0.00";
                        ws5.Cells["H" + (sgcList.Count + i + 7)].Style.Numberformat.Format = "#,##0.00";
                        ws5.Cells["I" + (sgcList.Count + i + 7)].Style.Numberformat.Format = "#,##0.00";
                        ws5.Cells["J" + (sgcList.Count + i + 7)].Style.Numberformat.Format = "#,##0.00";


                        ws5.Cells["B" + (sgcList.Count + i + 7)].Value = sgcGroupList[i].STATION_NAME.ToString();
                        ws5.Cells["C" + (sgcList.Count + i + 7)].Value = sgcGroupList[i].DC_INSTALLED_POWER;
                        ws5.Cells["D" + (sgcList.Count + i + 7)].Value = "-";
                        ws5.Cells["E" + (sgcList.Count + i + 7)].Value = sgcGroupList[i].ENERGY;
                        ws5.Cells["F" + (sgcList.Count + i + 7)].Value = sgcGroupList[i].IRRADIATION;
                        ws5.Cells["G" + (sgcList.Count + i + 7)].Value = sgcGroupList[i].SPECIFIC_YIELD;
                        if (sgcGroupList[i].INVERTER_ACTIVE_COUNT == null || sgcGroupList[i].INV_SUM_ACTIVE_COUNT == null)
                        {
                            ws5.Cells["H" + (sgcList.Count + i + 7)].Value = "-";
                        }
                        else
                        {
                            ws5.Cells["H" + (sgcList.Count + i + 7)].Value = sgcGroupList[i].INVERTER_ACTIVE_COUNT.ToString() + " / " + sgcGroupList[i].INV_SUM_ACTIVE_COUNT.ToString();
                        }

                        ws5.Cells["I" + (sgcList.Count + i + 7)].Value = sgcGroupList[i].PR;
                        if (money == true)
                        {
                            ws5.Cells["J" + (sgcList.Count + i + 7)].Value = sgcGroupList[i].FINANCIAL_USD;
                        }


                    }

                    int Station_id = Station[0].ID;
                    var _Alarm_list = (from AS in db.AlarmStatus
                                       where AS.STATION_ID == Station_id
                                       && AS.STATUS != 2 && AS.PROCESS_STEP != 1
                                       && AS.START_DATE.Value.Year == DateTime.Now.Year
                                       && AS.START_DATE.Value.Month == DateTime.Now.Month
                                       join AD in db.AlarmDesc on AS.ERROR_NUMBER equals AD.ERROR_NUMBER.ToString() into yG1
                                       from y1 in yG1.DefaultIfEmpty()
                                       join Inv in db.Inverters on AS.INVERTER_ID equals Inv.ID into yG2
                                       from y2 in yG2.DefaultIfEmpty()
                                       orderby AS.START_DATE ascending
                                       select new
                                       {

                                           ID = AS.ID,
                                           INVERTER_NAME = y2.NAME,
                                           STATUS = AS.STATUS,
                                           START_DATE = AS.START_DATE,
                                           END_DATE = AS.END_DATE,
                                           ERROR_NUMBER = y1.ERROR_DESC,
                                           TYPE = y1.TYPE
                                       }
                                        ).ToList();

                    ws6.Column(2).Width = 30;
                    ws6.Column(3).Width = 35;
                    ws6.Column(4).Width = 35;
                    ws6.Column(5).Width = 35;

                    ws6.Select("B2:B3");
                    ws6.SelectedRange.Merge = true;

                    ws6.Select("C2:C3");
                    ws6.SelectedRange.Merge = true;

                    ws6.Select("D2:D3");
                    ws6.SelectedRange.Merge = true;

                    ws6.Select("E2:E3");
                    ws6.SelectedRange.Merge = true;

                    ws6.Cells["B2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws6.Cells["C2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws6.Cells["D2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws6.Cells["E2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws6.Cells["B2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws6.Cells["C2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws6.Cells["D2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws6.Cells["E2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws6.Cells["C" + (_Alarm_list.Count + 4)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    ws6.Cells["B2"].Value = Resources.Error_Name;
                    ws6.Cells["C2"].Value = Resources.Inverter_Name;
                    ws6.Cells["D2"].Value = Resources.Start_Date;
                    ws6.Cells["E2"].Value = Resources.End_Date;

                    using (ExcelRange range = ws6.Cells["B2:E" + (_Alarm_list.Count + 3)])
                    {
                        range.Style.Border.Top.Style = ExcelBorderStyle.Medium;
                        range.Style.Border.Left.Style = ExcelBorderStyle.Medium;
                        range.Style.Border.Right.Style = ExcelBorderStyle.Medium;
                        range.Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

                    }

                    for (int i = 0; i < _Alarm_list.Count; i++)
                    {

                        ws6.Cells["B" + (i + 4)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        ws6.Cells["C" + (i + 4)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        ws6.Cells["D" + (i + 4)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        ws6.Cells["E" + (i + 4)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                        ws6.Cells["B" + (i + 4)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        ws6.Cells["C" + (i + 4)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        ws6.Cells["D" + (i + 4)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        ws6.Cells["E" + (i + 4)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                        ws6.Cells["B" + (i + 4)].Value = _Alarm_list[i].ERROR_NUMBER.ToString();
                        if (_Alarm_list[i].INVERTER_NAME == null)
                        {
                            ws6.Cells["C" + (i + 4)].Value = "";
                        }
                        else
                        {
                            ws6.Cells["C" + (i + 4)].Value = _Alarm_list[i].INVERTER_NAME.ToString();
                        }

                        ws6.Cells["D" + (i + 4)].Value = _Alarm_list[i].START_DATE.ToString();
                        ws6.Cells["E" + (i + 4)].Value = _Alarm_list[i].END_DATE.ToString();


                    }


                    var reportBuffer = pck.GetAsByteArray();
                    Response.Clear();
                    Response.AddHeader("content-disposition", "attachment;  filename=" + excelreportPath);
                    Response.ContentType = "application/vnd.openxmlformats - officedocument.spreadsheetml.sheet";
                    Response.BinaryWrite(reportBuffer);
                    result.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.Close();
                    Response.End();

                }



            };


            return View();


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
                  pyranometer = (float)Math.Round((float)a.PYRANOMETER, 1),
                  wind = (float)Math.Round((float)a.ruzgarHizi),
                  cell_temp = (float)Math.Round((float)a.hucreSicakligi),
                  external_temp = (float)Math.Round((float)a.sicaklik)
              }).OrderBy(a => a.date).ToList();
            return Json(met, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetMonthlyProduction(int stationId, string slcYear)
        {
            DateTime nowDate = DateTime.Now;
            int yil = Int32.Parse(slcYear);
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
                mm.listPR[i].target = GetMonthlyTargetFunc(stationId, mm.listPR[i].month);
            }

            var listTRG = new List<float>();
            for (int i = 0; i < 12; i++)
            {
                listTRG.Add(GetMonthlyTargetFunc(stationId, i + 1));
            }
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
            Session["AnnualyData"] = mm.listPR;
            return Json(mm, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAnnualProduction(int stationId)
        {
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

        public JsonResult GetTotalProduction(int stationId)
        {
            DateTime startDate = DB.Stations.Where(a => a.ID == stationId).Select(a => a.START_DATE.Value).FirstOrDefault();
            float totalSum = DB.PRSum.Where(p => p.STATION_ID == stationId && p.date.Value > startDate).Sum(a => a.enerji).Value;
            float totalAvg = DB.PRSum.Where(p => p.STATION_ID == stationId && p.date.Value > startDate).Average(a => a.pr).Value;
            TBL_PR_YEARLY pp = new TBL_PR_YEARLY();
            pp.energy = (float)Math.Round(totalSum, 2);
            pp.pr = (float)Math.Round(totalAvg, 2);
            return Json(pp, JsonRequestBehavior.AllowGet);
        }


        public ActionResult ExceleAktarInvDataTable()
        {
            return View(Session["InvDataTable"] as List<Inv_Production_DTO>);
        }

        public JsonResult GetInstalledPower(int stationId)
        {
            var power = stationService.GetStationDetailById(stationId).SIZE;
            return Json(power, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMonthlyTarget(int stationId)
        {
            var dcIns = DB.Stations.Where(a => a.ID == stationId).Select(a => a.DC_INSTALLED_POWER).FirstOrDefault();
            var Mtarget = (from u in DB.targets
                           where u.STATION_ID == stationId
                           select new TARGET_DETAIL
                           {
                               _stationId = u.STATION_ID,
                               _januaryProduction = u.JAN_PRODUCTION.Value,
                               _januaryIrradition = u.JAN_IRRADIATION.Value,
                               _februaryProduction = u.FEB_PRODUCTION.Value,
                               _februaryIrradition = u.FEB_IRRADIATION.Value,
                               _marchProduction = u.MARCH_PRODUCTION.Value,
                               _marchIrradition = u.MARCH_IRRADIATION.Value,
                               _aprilProduction = u.APRIL_PRODUCTION.Value,
                               _aprilIrradition = u.APRIL_IRRADIATION.Value,
                               _mayProduction = u.MAY_PRODUCTION.Value,
                               _mayIrradition = u.MAY_IRRADIATION.Value,
                               _juneProduction = u.JUNE_PRODUCTION.Value,
                               _juneIrradition = u.JUNE_IRRADIATION.Value,
                               _julyProduction = u.JULY_PRODUCTION.Value,
                               _julyIrradition = u.JULY_IRRADIATION.Value,
                               _augustProduction = u.AUGUST_PRODUCTION.Value,
                               _augustIrradition = u.AUGUST_IRRADIATION.Value,
                               _septemberProduction = u.SEP_PRODUCTION.Value,
                               _septemberIrradition = u.SEP_IRRADIATION.Value,
                               _octoberProduction = u.OKT_PRODUCTION.Value,
                               _octoberIrradition = u.OKT_IRRADIATION.Value,
                               _novemberProduction = u.NOV_PRODUCTION.Value,
                               _novemberIrradition = u.NOV_IRRADIATION.Value,
                               _decemberProduction = u.DEC_PRODUCTION.Value,
                               _decemberIrradition = u.DEC_IRRADIATION.Value,
                               _yearProduction = u.YEAR_PRODUCTION.Value,
                               _yearIrradition = u.YEAR_IRRADIATION.Value,
                           }).FirstOrDefault();

            return Json(Mtarget, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CompanyPro(int companyId, DateTime beginDate)
        {
            DateTime nowDate = beginDate;
            int[] ib = DB.Stations.Where(x => x.COMPANY_ID == companyId && x.IS_DELETED == false && x.IS_ACTIVE == true).OrderBy(a => a.GROUP_ID).ThenBy(a => a.NAME).Select(a => a.ID).ToArray();
            var userId = User.Identity.GetUserId();
            List<StationViewModel> lstcompPro = new List<StationViewModel>();
            List<TBL_STATION> stations = new List<TBL_STATION>();
            bool? idDemo = false;
            if (User.IsInRole("DEMO"))
            {
                idDemo = true;
            }
            if (User.IsInRole("M_ADMIN"))
            {
                stations = DB.Stations.Where(a => a.IS_DELETED == false && a.IS_ACTIVE == true && a.STATION_TYPE != 4 && a.COMPANY_ID == companyId).ToList();
            }
            else if (User.IsInRole("COMP_ADMIN"))
            {
                stations = DB.Stations.Where(a => a.IS_DELETED == false && a.IS_ACTIVE == true && a.STATION_TYPE != 4 && a.COMPANY_ID == companyId).ToList();
            }
            else if (User.IsInRole("COMP_USER"))
            {
                int[] ibs = DB.StationUsers.Where(a => a.USER_ID == userId && a.IS_DELETED == false).Select(a => a.STATION_ID).ToArray();
                stations = DB.Stations.Where(a => a.IS_DELETED == false && a.IS_ACTIVE == true && ibs.Contains(a.ID) && a.STATION_TYPE != 4 && a.COMPANY_ID == companyId).ToList();
            }
            else if (User.IsInRole("DEMO"))
            {
                int[] ibs = DB.StationUsers.Where(a => a.USER_ID == userId && a.IS_DELETED == false).Select(a => a.STATION_ID).ToArray();
                stations = DB.Stations.Where(a => a.IS_DELETED == false && a.IS_ACTIVE == true && ibs.Contains(a.ID) && a.STATION_TYPE != 4 && a.COMPANY_ID == companyId).ToList();
            }
            foreach (TBL_STATION item in stations)
            {
                var performance = DB.PRSum.Where(a => a.STATION_ID == item.ID && a.date.Value.Year == nowDate.Year
                     && a.date.Value.Month == nowDate.Month
                     && a.date.Value.Day == nowDate.Day).FirstOrDefault();
                var endData = DB.Summaries.Where(a => a.STATION_ID == item.ID && a.tarih.Year == nowDate.Year
                     && a.tarih.Month == nowDate.Month
                     && a.tarih.Day == nowDate.Day
                ).OrderByDescending(a => a.tarih).Take(1).Select(a => new StationViewModel
                {
                    STATION_ID = item.ID,
                    STATION_NAME = idDemo == true ? item.DEMO_NAME : item.NAME,
                    P_AC = a.gunlukUretim == null ? 0 : (float)Math.Round((a.gunlukUretim.Value), 2),
                    P_DC = a.gunlukUretim == null ? 0 : (float)Math.Round((a.Dc_Guc.Value), 2),
                    Income = a.Enerji == null ? 0 : (item.EXCHANGE_RATE) * (float)Math.Round((a.Enerji.Value / 1000000), 2) * 1000,
                    PR = performance.pr == null ? 0 : (float)Math.Round(((float)performance.pr), 1),
                    Enerji = a.Enerji == null ? 0 : (float)Math.Round((a.Enerji.Value / 1000000), 2)
                }).FirstOrDefault();
                if (endData == null)
                {
                    StationViewModel emp = new StationViewModel();
                    emp.STATION_ID = item.ID;
                    emp.STATION_NAME = idDemo == true ? item.DEMO_NAME : item.NAME;
                    emp.Enerji = 0;
                    emp.Income = 0;
                    emp.PR = 0;
                    lstcompPro.Add(emp);
                }

                if (endData != null)
                {
                    lstcompPro.Add(endData);
                }
            }

            return Json(lstcompPro, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GroupPro(int groupId, string beginDate)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("tr-TR");
            DateTime reqDateParam = DateTime.Parse(beginDate);
            int[] ib = DB.Stations.Where(x => x.GROUP_ID == groupId && x.IS_DELETED == false && x.IS_ACTIVE == true).OrderBy(a => a.GROUP_ID).ThenBy(a => a.NAME).Select(a => a.ID).ToArray();
            var userId = User.Identity.GetUserId();
            List<StationViewModel> lstcompPro = new List<StationViewModel>();
            List<TBL_STATION> stations = new List<TBL_STATION>();
            bool? idDemo = false;
            if (User.IsInRole("DEMO"))
            {
                idDemo = true;
            }
            if (User.IsInRole("M_ADMIN"))
            {
                stations = DB.Stations.Where(a => a.IS_DELETED == false && a.IS_ACTIVE == true && a.STATION_TYPE != 4 && a.GROUP_ID == groupId).ToList();
            }
            else if (User.IsInRole("COMP_ADMIN"))
            {
                stations = DB.Stations.Where(a => a.IS_DELETED == false && a.IS_ACTIVE == true && a.STATION_TYPE != 4 && a.GROUP_ID == groupId).ToList();
            }
            else if (User.IsInRole("COMP_USER"))
            {
                int[] ibs = DB.StationUsers.Where(a => a.USER_ID == userId && a.IS_DELETED == false).Select(a => a.STATION_ID).ToArray();
                stations = DB.Stations.Where(a => a.IS_DELETED == false && a.IS_ACTIVE == true && ibs.Contains(a.ID) && a.STATION_TYPE != 4 && a.GROUP_ID == groupId).ToList();
            }
            else if (User.IsInRole("DEMO"))
            {
                int[] ibs = DB.StationUsers.Where(a => a.USER_ID == userId && a.IS_DELETED == false).Select(a => a.STATION_ID).ToArray();
                stations = DB.Stations.Where(a => a.IS_DELETED == false && a.IS_ACTIVE == true && ibs.Contains(a.ID) && a.STATION_TYPE != 4 && a.GROUP_ID == groupId).ToList();
            }
            foreach (TBL_STATION item in stations)
            {
                var endData = DB.PRSum.Where(a => 
                     a.STATION_ID == item.ID 
                     && a.date.Value.Year == reqDateParam.Year
                     && a.date.Value.Month == reqDateParam.Month
                     && a.date.Value.Day == reqDateParam.Day
                ).Select(a => new StationViewModel
                {
                    STATION_ID = item.ID,
                    STATION_NAME = idDemo == true ? item.DEMO_NAME : item.NAME,
                    P_AC = a.gunlukUretim == null ? 0 : (float)Math.Round((a.gunlukUretim.Value), 2),
                    Income = a.enerji == null ? 0 : (item.EXCHANGE_RATE) * (float)Math.Round((a.enerji.Value / 1000000), 2) * 1000,
                    PR = a.pr == null ? 0 : (float)Math.Round(((float)a.pr), 1),
                    Enerji = a.enerji == null ? 0 : (float)Math.Round((a.enerji.Value), 2)
                }).FirstOrDefault();
                if (endData == null)
                {
                    StationViewModel emp = new StationViewModel();
                    emp.STATION_ID = item.ID;
                    emp.STATION_NAME = idDemo == true ? item.DEMO_NAME : item.NAME;
                    emp.Enerji = 0;
                    emp.Income = 0;
                    emp.PR = 0;
                    lstcompPro.Add(emp);
                }

                if (endData != null)
                {
                    lstcompPro.Add(endData);
                }
            }

            return Json(lstcompPro, JsonRequestBehavior.AllowGet);
        }
       

        public JsonResult GetMapStation(int companyId)
        {
            DateTime nowDate = DateTime.Now;
            bool ConStatus = false;
            int[] iib = DB.Stations.Where(x => x.COMPANY_ID == companyId && x.IS_DELETED == false && x.IS_ACTIVE == true).OrderBy(a => a.NAME).Select(a => a.ID).ToArray();
            List<TBL_STATION> stations = new List<TBL_STATION>();
            List<StationViewModel> lstcompPro = new List<StationViewModel>();
            var userId = User.Identity.GetUserId();
            bool? idDemo = false;
            if (User.IsInRole("DEMO"))
            {
                idDemo = true;
            }
            if (User.IsInRole("M_ADMIN"))
            {
                stations = DB.Stations.Where(a => a.IS_DELETED == false && a.IS_ACTIVE == true && a.STATION_TYPE != 4 && a.COMPANY_ID == companyId).ToList();
            }
            else if (User.IsInRole("COMP_ADMIN"))
            {
                stations = DB.Stations.Where(a => a.IS_DELETED == false && a.IS_ACTIVE == true && a.STATION_TYPE != 4 && a.COMPANY_ID == companyId).ToList();
            }
            else if (User.IsInRole("COMP_USER"))
            {
                int[] ibs = DB.StationUsers.Where(a => a.USER_ID == userId && a.IS_DELETED == false).Select(a => a.STATION_ID).ToArray();
                stations = DB.Stations.Where(a => a.IS_DELETED == false && a.IS_ACTIVE == true && ibs.Contains(a.ID) && a.STATION_TYPE != 4 && a.COMPANY_ID == companyId).ToList();
            }
            else if (User.IsInRole("DEMO"))
            {
                int[] ibs = DB.StationUsers.Where(a => a.USER_ID == userId && a.IS_DELETED == false).Select(a => a.STATION_ID).ToArray();
                stations = DB.Stations.Where(a => a.IS_DELETED == false && a.IS_ACTIVE == true && ibs.Contains(a.ID) && a.STATION_TYPE != 4 && a.COMPANY_ID == companyId).ToList();
            }

            foreach (TBL_STATION item in stations)
            {
                var isConnection = (DB.AlarmStatus.Where(a => a.STATION_ID == item.ID
                                && a.ERROR_NUMBER == "0001" && a.STATUS != 2 && a.END_DATE == null
                                ).OrderByDescending(a => a.START_DATE).Take(1).FirstOrDefault());
                if (isConnection != null)
                {
                    ConStatus = false;
                }
                else
                {
                    ConStatus = true;
                }
                var endData = DB.PRSum.Where(a => a.STATION_ID == item.ID
                && a.date.Value.Year == nowDate.Year
                && a.date.Value.Month == nowDate.Month
                && a.date.Value.Day == nowDate.Day)
                .OrderByDescending(a => a.date).Take(1).Select(a => new StationViewModel
                {
                    CON_STATUS = ConStatus,
                    STATION_ID = item.ID,
                    STATION_NAME = idDemo == true ? item.DEMO_NAME : item.NAME,
                    COORDINANT = item.COORDINATE_INFORMATION,
                    Enerji = a.enerji == null ? 0 : (float)Math.Round((a.enerji.Value), 2),
                    PR = a.pr == null ? 0 : (float)Math.Round((a.pr.Value), 1)
                }).FirstOrDefault();
                if (endData == null)
                {
                    StationViewModel emp = new StationViewModel();
                    emp.CON_STATUS = ConStatus;
                    emp.STATION_ID = item.ID;
                    emp.STATION_NAME = idDemo == true ? item.DEMO_NAME : item.NAME;
                    emp.COORDINANT = item.COORDINATE_INFORMATION;
                    emp.Enerji = 0;
                    emp.PR = 0;
                    lstcompPro.Add(emp);
                }

                if (endData != null)
                {
                    lstcompPro.Add(endData);
                }
            }

            return Json(lstcompPro, JsonRequestBehavior.AllowGet);
        }

        
        public JsonResult GetMapAll()
        {
            bool ConStatus = false;
            DateTime nowDate = DateTime.Now;
            if (User.IsInRole("M_ADMIN"))
            {
                int[] ib = DB.Stations.Where(x => x.IS_DELETED == false && x.IS_ACTIVE == true).OrderBy(a => a.NAME).Take(30).Select(a => a.ID).ToArray();

                List<StationViewModel> lstcompPro = new List<StationViewModel>();
                foreach (int item in ib)
                {
                    var isConnection = (DB.AlarmStatus.Where(a => a.STATION_ID == item
                                    && a.ERROR_NUMBER == "0001" && a.STATUS != 2 && a.END_DATE == null
                                    ).OrderByDescending(a => a.START_DATE).Take(1).FirstOrDefault());
                    if (isConnection != null)
                    {
                        ConStatus = false;
                    }
                    else
                    {
                        ConStatus = true;
                    }

                    var stt = DB.Stations.Where(a => a.ID == item).FirstOrDefault();
                    var energyEndData = DB.PRSum.Where(a => a.STATION_ID == item
                    && a.date.Value.Year == nowDate.Year
                    && a.date.Value.Month == nowDate.Month
                    && a.date.Value.Day == nowDate.Day)
                    .OrderByDescending(a => a.date).Take(1).Select(a => new StationViewModel
                    {
                        CON_STATUS = ConStatus,
                        STATION_ID = stt.ID,
                        STATION_NAME = stt.NAME,
                        PR = a.pr == null ? 0 : (float)Math.Round((a.pr.Value), 1),
                        COORDINANT = stt.COORDINATE_INFORMATION,
                        Enerji = a.enerji == null ? 0 : (float)Math.Round((a.enerji.Value), 2)
                    }).FirstOrDefault();
                    if (energyEndData == null)
                    {
                        StationViewModel emp = new StationViewModel();
                        emp.CON_STATUS = ConStatus;
                        emp.STATION_ID = stt.ID;
                        emp.STATION_NAME = stt.NAME;
                        emp.COORDINANT = stt.COORDINATE_INFORMATION;
                        emp.PR = 0;
                        emp.Enerji = 0;
                        lstcompPro.Add(emp);
                    }

                    if (energyEndData != null)
                    {
                        lstcompPro.Add(energyEndData);
                    }
                }

                return Json(lstcompPro, JsonRequestBehavior.AllowGet);
            }
            else if (User.IsInRole("COMP_ADMIN"))
            {
                var uus = User.Identity.GetUserId();
                
                    var compUs = DB.CompanyUsers.Where(x => x.USER_ID == uus && x.IS_DELETED == false).ToList();
                    List<TBL_STATION> listStat = new List<TBL_STATION>();
                    List<int> listStatID = new List<int>();
                    foreach (var itemCom in compUs)
                    {
                        var stats = DB.Stations.Where(x => x.COMPANY_ID == itemCom.COMPANY_ID && x.IS_DELETED == false && x.IS_ACTIVE == true && x.IS_LOCKED == false).ToList();
                        foreach (var item in stats)
                        {
                            listStatID.Add(item.ID);
                        }
                    }
                    List<StationViewModel> lstcompPro = new List<StationViewModel>();
                    foreach (int item in listStatID)
                    {
                        var isConnection = (DB.AlarmStatus.Where(a => a.STATION_ID == item
                                    && a.ERROR_NUMBER == "0001" && a.STATUS != 2 && a.END_DATE == null
                                    ).OrderByDescending(a => a.START_DATE).Take(1).FirstOrDefault());
                        if (isConnection != null)
                        {
                            ConStatus = false;
                        }
                        else
                        {
                            ConStatus = true;
                        }
                        var stt = DB.Stations.Where(a => a.ID == item).FirstOrDefault();
                        var energyEndData = DB.PRSum.Where(a => a.STATION_ID == item
                        && a.date.Value.Year == nowDate.Year
                        && a.date.Value.Month == nowDate.Month
                        && a.date.Value.Day == nowDate.Day)
                        .OrderByDescending(a => a.date).Take(1).Select(a => new StationViewModel
                        {
                            CON_STATUS = ConStatus,
                            STATION_ID = stt.ID,
                            STATION_NAME = stt.NAME,
                            COORDINANT = stt.COORDINATE_INFORMATION,
                            PR = a.pr == null ? 0 : (float)Math.Round((a.pr.Value), 1),
                            Enerji = a.enerji == null ? 0 : (float)Math.Round((a.enerji.Value), 2)
                        }).FirstOrDefault();
                        if (energyEndData == null)
                        {
                            StationViewModel emp = new StationViewModel();
                            emp.CON_STATUS = ConStatus;
                            emp.STATION_ID = stt.ID;
                            emp.STATION_NAME = stt.NAME;
                            emp.COORDINANT = stt.COORDINATE_INFORMATION;
                            emp.Enerji = 0;
                            emp.PR = 0;
                            lstcompPro.Add(emp);
                        }

                        if (energyEndData != null)
                        {
                            lstcompPro.Add(energyEndData);
                        }
                    }

                    return Json(lstcompPro, JsonRequestBehavior.AllowGet);
                
            }
            else if (User.IsInRole("COMP_USER"))
            {
                List<StationViewModel> lstcompPro = new List<StationViewModel>();
                var userId = User.Identity.GetUserId();
                int[] ib = DB.StationUsers.Where(a => a.USER_ID == userId && a.IS_DELETED == false).Select(a => a.STATION_ID).ToArray();
                var userStations = DB.StationUsers.Where(x => x.USER_ID == userId && ib.Contains(x.STATION_ID) && x.IS_DELETED == false).ToList();
                List<int> listStatID = new List<int>();
                foreach (var usrs in userStations)
                {
                    listStatID.Add(usrs.STATION_ID);
                }

                foreach (int item in listStatID)
                {
                    var isConnection = (DB.AlarmStatus.Where(a => a.STATION_ID == item
                                    && a.ERROR_NUMBER == "0001" && a.STATUS != 2 && a.END_DATE == null
                                    ).OrderByDescending(a => a.START_DATE).Take(1).FirstOrDefault());
                    if (isConnection != null)
                    {
                        ConStatus = false;
                    }
                    else
                    {
                        ConStatus = true;
                    }
                    var stt = DB.Stations.Where(a => a.ID == item).FirstOrDefault();
                    var energyEndData = DB.PRSum.Where(a => a.STATION_ID == item
                    && a.date.Value.Year == nowDate.Year
                    && a.date.Value.Month == nowDate.Month
                    && a.date.Value.Day == nowDate.Day)
                    .OrderByDescending(a => a.date).Take(1).Select(a => new StationViewModel
                    {
                        CON_STATUS = ConStatus,
                        STATION_ID = stt.ID,
                        STATION_NAME = stt.NAME,
                        COORDINANT = stt.COORDINATE_INFORMATION,
                        PR = a.pr == null ? 0 : (float)Math.Round((a.pr.Value), 1),
                        Enerji = a.enerji == null ? 0 : (float)Math.Round((a.enerji.Value), 2)
                    }).FirstOrDefault();
                    if (energyEndData == null)
                    {
                        StationViewModel emp = new StationViewModel();
                        emp.CON_STATUS = ConStatus;
                        emp.STATION_ID = stt.ID;
                        emp.STATION_NAME = stt.NAME;
                        emp.COORDINANT = stt.COORDINATE_INFORMATION;
                        emp.Enerji = 0;
                        emp.PR = 0;
                        lstcompPro.Add(emp);
                    }

                    if (energyEndData != null)
                    {
                        lstcompPro.Add(energyEndData);
                    }
                }

                return Json(lstcompPro, JsonRequestBehavior.AllowGet);
            }
            else if (User.IsInRole("DEMO"))
            {
                List<StationViewModel> lstcompPro = new List<StationViewModel>();
                var userId = User.Identity.GetUserId();
                int[] ib = DB.StationUsers.Where(a => a.USER_ID == userId && a.IS_DELETED == false).Select(a => a.STATION_ID).ToArray();
                var userStations = DB.StationUsers.Where(x => x.USER_ID == userId && ib.Contains(x.STATION_ID) && x.IS_DELETED == false).ToList();
                List<int> listStatID = new List<int>();
                foreach (var usrs in userStations)
                {
                    listStatID.Add(usrs.STATION_ID);
                }

                foreach (int item in listStatID)
                {
                    var isConnection = (DB.AlarmStatus.Where(a => a.STATION_ID == item && a.ERROR_NUMBER == "0001" && a.STATUS != 2 && a.END_DATE == null).OrderByDescending(a => a.START_DATE).Take(1).FirstOrDefault());
                    if (isConnection != null)
                    {
                        ConStatus = false;
                    }
                    else
                    {
                        ConStatus = true;
                    }
                    var stt = DB.Stations.Where(a => a.ID == item).FirstOrDefault();
                    var energyEndData = DB.PRSum.Where(a => a.STATION_ID == item
                    && a.date.Value.Year == nowDate.Year
                    && a.date.Value.Month == nowDate.Month
                    && a.date.Value.Day == nowDate.Day)
                    .OrderByDescending(a => a.date).Take(1).Select(a => new StationViewModel
                    {
                        CON_STATUS = ConStatus,
                        STATION_ID = stt.ID,
                        STATION_NAME = stt.DEMO_NAME,
                        COORDINANT = stt.COORDINATE_INFORMATION,
                        PR = a.pr == null ? 0 : (float)Math.Round((a.pr.Value), 1),
                        Enerji = a.enerji == null ? 0 : (float)Math.Round((a.enerji.Value), 2)
                    }).FirstOrDefault();
                    if (energyEndData == null)
                    {
                        StationViewModel emp = new StationViewModel();
                        emp.CON_STATUS = ConStatus;
                        emp.STATION_ID = stt.ID;
                        emp.STATION_NAME = stt.DEMO_NAME;
                        emp.COORDINANT = stt.COORDINATE_INFORMATION;
                        emp.Enerji = 0;
                        emp.PR = 0;
                        lstcompPro.Add(emp);
                    }

                    if (energyEndData != null)
                    {
                        lstcompPro.Add(energyEndData);
                    }
                }

                return Json(lstcompPro, JsonRequestBehavior.AllowGet);
            }
            return Json(0, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Help()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Help(MailModel model)
        {
            var body = new StringBuilder();
            body.AppendLine("Kullanıcı Adı: " + User.Identity.GetUserName());
            body.AppendLine("İsim: " + model.Name);
            body.AppendLine("Tel: " + model.Phone);
            body.AppendLine("Eposta: " + model.Email);
            body.AppendLine("Mesaj: " + model.Message);

            Mail.SendMail(body.ToString());
            ViewBag.Success = true;
            Response.Write("<script>alert('Send Your Message');</script>");
            return View();
        }

        public class Mail
        {
            public static void SendMail(string body)
            {
                var fromAddress = new MailAddress("support@esso.com.tr", "ESoft Web");
                var toAddress = new MailAddress("support@esso.com.tr");
                const string subject = "Web Site Contact Form";
                using (var smtp = new SmtpClient
                {
                    Host = "mail.esso.com.tr",
                    Port = 587,
                    EnableSsl = false,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, "support0635")

                })
                {
                    try
                    {
                        using (var message = new MailMessage(fromAddress, toAddress) { Subject = subject, Body = body })
                        {
                            smtp.Send(message);
                        }
                    }
                    catch (Exception ex)
                    {

                        throw;
                    }
                }
            }
        }


        public JsonResult GetHourlyAVG(int stationId, string slcDate)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("tr-TR");
            DateTime reqDateParam = DateTime.Parse(slcDate);

            var st = (from u in DB.Summaries
                      where u.STATION_ID == stationId
                      && u.tarih.Year == reqDateParam.Year
                      && u.tarih.Month == reqDateParam.Month
                      && u.tarih.Day == reqDateParam.Day
                      && u.gunlukUretim != null
                      group u by u.tarih.Hour into grp
                      orderby grp.Key
                      select new
                      {
                          hour = grp.Key,
                          isinim_sum = grp.Average(x => x.isinim) == null ? 0 : (float)Math.Round(grp.Average(x => x.isinim.Value), 1),
                          enerji = (float)Math.Round(grp.Average(x => x.gunlukUretim.Value / 1000), 1)
                      }).ToList();
            return Json(st, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Search()
        {
            return View();
        }

        public JsonResult GetStation()
        {
            var st = DB.Stations.Where(a => a.IS_ACTIVE == true && a.IS_DELETED == false).ToList();
            return Json(st, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExceleAktarPrRapor()
        {
            return View(Session["PRData"] as List<PRListModel>);
        }
        public JsonResult AnlikPR(int companyId, DateTime beginDate)
        {
            DateTime nowDate = beginDate;
            var userId = User.Identity.GetUserId();
            List<TBL_STATION> stations = new List<TBL_STATION>();
            List<StationViewModel> lstcompPro = new List<StationViewModel>();
            bool? idDemo = false;
            if (User.IsInRole("DEMO"))
            {
                idDemo = true;
            }
            if (User.IsInRole("M_ADMIN"))
            {
                stations = DB.Stations.Where(a => a.IS_DELETED == false && a.IS_ACTIVE == true && a.STATION_TYPE != 4 && a.COMPANY_ID == companyId).ToList();
            }
            else if (User.IsInRole("COMP_ADMIN"))
            {
                stations = DB.Stations.Where(a => a.IS_DELETED == false && a.IS_ACTIVE == true && a.STATION_TYPE != 4 && a.COMPANY_ID == companyId).ToList();
            }
            else if (User.IsInRole("COMP_USER"))
            {
                int[] ibs = DB.StationUsers.Where(a => a.USER_ID == userId && a.IS_DELETED == false).Select(a => a.STATION_ID).ToArray();
                stations = DB.Stations.Where(a => a.IS_DELETED == false && a.IS_ACTIVE == true && ibs.Contains(a.ID) && a.STATION_TYPE != 4 && a.COMPANY_ID == companyId).ToList();
            }
            else if (User.IsInRole("DEMO"))
            {
                int[] ibs = DB.StationUsers.Where(a => a.USER_ID == userId && a.IS_DELETED == false).Select(a => a.STATION_ID).ToArray();
                stations = DB.Stations.Where(a => a.IS_DELETED == false && a.IS_ACTIVE == true && ibs.Contains(a.ID) && a.STATION_TYPE != 4 && a.COMPANY_ID == companyId).ToList();
            }
            foreach (TBL_STATION item in stations)
            {
                int _sttInvCount = DB.Inverters.Where(x => x.STATION_ID == item.ID).Count();
                int _AlarmInvCount = DB.AlarmStatus.Where(x => x.ERROR_NUMBER == "0007" && x.STATUS != 2 && x.END_DATE == null && x.STATION_ID == item.ID).Count();
                int _InvCount = _sttInvCount - _AlarmInvCount;

                var endData = DB.Summaries.Where(a => a.STATION_ID == item.ID && a.tarih.Year == nowDate.Year
                     && a.tarih.Month == nowDate.Month
                     && a.tarih.Day == nowDate.Day
                ).OrderByDescending(a => a.tarih).Take(1).Select(a => new StationViewModel
                {
                    STATION_ID = item.ID,
                    STATION_NAME = idDemo == true ? item.DEMO_NAME : item.NAME,
                    P_AC = a.gunlukUretim == null ? 0 : (float)Math.Round((a.gunlukUretim.Value), 2),
                    P_DC = a.gunlukUretim == null ? 0 : (float)Math.Round((a.Dc_Guc.Value), 2),
                    IRRADIATION = a.isinim == null ? 0 : (float)Math.Round((a.isinim.Value), 2),
                    Income = a.Enerji == null ? 0 : (item.EXCHANGE_RATE) * (float)Math.Round((a.Enerji.Value / 1000000), 2) * 1000,
                    PR = a.PR == null ? 0 : (float)Math.Round(((float)a.PR), 1),
                    Enerji = a.Enerji == null ? 0 : (float)Math.Round((a.Enerji.Value / 1000000), 2),
                    ActiveInvCount = _InvCount

                }).FirstOrDefault();
                if (endData == null)
                {
                    StationViewModel emp = new StationViewModel();
                    emp.STATION_ID = item.ID;
                    emp.STATION_NAME = idDemo == true ? item.DEMO_NAME : item.NAME;
                    emp.Enerji = 0;
                    emp.Income = 0;
                    emp.PR = 0;
                    emp.ActiveInvCount = _InvCount;
                    lstcompPro.Add(emp);

                }

                if (endData != null)
                {
                    lstcompPro.Add(endData);
                }
            }

            return Json(lstcompPro, JsonRequestBehavior.AllowGet);
        }
        public class ModbusWriteControl
        {
            public bool IsSuccess { get; set; }
            public string ErrorMessage { get; set; }
            public bool? TestState { get; set; }

        }
        public class ModbusReadControl
        {

            public bool IsSuccess { get; set; }
            public string ErrorMessage { get; set; }
            public bool? TestState { get; set; }
            public Array arraylist { get; set; }
            public Array arraylist2 { get; set; }
            public float OrtalamaGerilim { get; set; }

        }
        public JsonResult ModbusOpen(int station_id)
        {
            ModbusReadControl mwc = new ModbusReadControl();

            try
            {
                var modbus_live = (from p in DB.ModbusDataLive
                                   where p.STATION_ID == station_id
                                   orderby p.ADDRESS ascending
                                   select p.VALUE).ToList();

                var Summaries_last = DB.Summaries
                             .Where(x
                                 => x.STATION_ID == station_id
                               )
                             .OrderByDescending(x => x.tarih)
                             .Take(1)
                             .ToList()
                             .FirstOrDefault();





                var digital_alarm = (from i in DB.Tags
                                     join j in DB.TagTemplateDets on i.ID equals j.TAG_ID
                                     join k in DB.DigitalLogsLive on i.ID equals k.TAG_ID

                                     where k.STATION_ID == station_id && (i.TAG_TYPE == 1 || i.TAG_TYPE == 2)

                                     select new
                                     {
                                         ADDRESS = j.ADDRESS,
                                         DESC = k.DESC,
                                         TAG_TYPE = i.TAG_TYPE

                                     }).ToList();



                var V_Ort = (Summaries_last.H2_Vab + Summaries_last.H2_Vac + Summaries_last.H2_Vbc) / 3;

                mwc.IsSuccess = true;
                mwc.ErrorMessage = "";
                mwc.arraylist = modbus_live.ToArray();
                mwc.arraylist2 = digital_alarm.ToArray();
                mwc.OrtalamaGerilim = (float)V_Ort;

                return Json(mwc, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                mwc.IsSuccess = false;
                mwc.ErrorMessage = "Bağlantı Hatası";
                return Json(mwc, JsonRequestBehavior.AllowGet);
            }



        }
        public JsonResult ModbusWrite(string modBusWriteAdress, int command, int station_id, string user_id, string modBusReadAdress)
        {
            ModbusWriteControl mwc2 = new ModbusWriteControl();



            try
            {


                var values = Read2(1, modBusReadAdress, station_id);
                var userId = User.Identity.GetUserId();


                Write(1, modBusWriteAdress, command, station_id);
                Thread.Sleep(5000);
                Write(1, modBusWriteAdress, 0, station_id);

                Thread.Sleep(1000);
                var values2 = Read2(1, modBusReadAdress, station_id);


                TBL_MODBUS_CMD_LOG _ModbusLog = new TBL_MODBUS_CMD_LOG();

                _ModbusLog.STATION_ID = station_id;
                _ModbusLog.ADDRESS = int.Parse(modBusReadAdress);
                _ModbusLog.INSERT_DATE = DateTime.Now;
                _ModbusLog.USER_ID = userId;
                _ModbusLog.VALUE = int.Parse(values2);
                _ModbusLog.OLD_VALUE = int.Parse(values);
                DB.ModbusLog.Add(_ModbusLog);
                DB.SaveChanges();


                TBL_MODBUS_DATA _ModbusData = new TBL_MODBUS_DATA();

                _ModbusData.CREATED_DATE = DateTime.Now;
                _ModbusData.UPDATED_DATE = null;
                _ModbusData.STATION_ID = station_id;
                _ModbusData.ADDRESS = int.Parse(modBusReadAdress);
                _ModbusData.VALUE = int.Parse(values2);
                DB.ModbusData.Add(_ModbusData);
                DB.SaveChanges();





                return Json(mwc2, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(mwc2, JsonRequestBehavior.AllowGet);
            }


        }
        public bool Write(byte slaveAddress, string startAddress, int Command, int station_id)
        {



            var stt = DB.Stations.Where(a => a.ID == station_id).FirstOrDefault();


            var master = new ModbusTCPMaster(stt.IP_ADDRESS, 502, 5000);
            master.Disconnection();
            master.Connection();
            try
            {
                byte[] bytes = BitConverter.GetBytes(Command);
                byte[] cevir = new byte[2];

                cevir[1] = bytes[0];
                cevir[0] = bytes[1];

                byte[] byteArray = master.WriteMultipleRegisters(slaveAddress, startAddress, cevir);

                return true;
            }
            catch (Exception ex)
            {
                master.Disconnection();
                return false;
            }

        }

        public Array Read(byte slaveAddress, string startAddress, int station_id)
        {
            var stt = DB.Stations.Where(a => a.ID == station_id).FirstOrDefault();
            startAddress = "12488";
            var master = new ModbusTCPMaster(stt.IP_ADDRESS, 502, 5000);
            master.Disconnection();
            master.Connection();
            try
            {
                byte[] byteArray = master.ReadHoldingRegisters(slaveAddress, startAddress, 35);
                short[] result = IndustrialNetwork.Modbus.DataType.Int.ToArray(byteArray);
                return result;
                //if (result[0] == 1)
                //{
                //    return true;
                //}
                //else
                //{
                //    return false;
                //}
            }
            catch (Exception ex)
            {
                master.Disconnection();
                string[] hata = new string[1];

                hata[0] = "error";
                return hata;
            }

        }
        public string Read2(byte slaveAddress, string startAddress, int station_id)
        {
            var stt = DB.Stations.Where(a => a.ID == station_id).FirstOrDefault();
            //startAddress = "12488";
            var master = new ModbusTCPMaster(stt.IP_ADDRESS, 502, 5000);
            master.Disconnection();
            master.Connection();
            try
            {
                byte[] byteArray = master.ReadHoldingRegisters(slaveAddress, startAddress, 1);
                short[] result = IndustrialNetwork.Modbus.DataType.Int.ToArray(byteArray);
                return result[0].ToString();

            }
            catch (Exception ex)
            {
                master.Disconnection();
                string[] hata = new string[1];

                hata[0] = "error";
                return hata[0].ToString();
            }

        }
    }
}