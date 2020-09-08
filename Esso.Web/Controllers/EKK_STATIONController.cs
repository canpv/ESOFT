using Esso.Data;
using Esso.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace Esso.Web.Controllers
{
    [Authorize]
    public class EKK_STATIONController : BaseController
    {
        EssoEntities DB = new EssoEntities();
        // GET: EKK_STATION
        public ActionResult Index(int stationId)
        {
            return View(stationId);
        }

        public class Daily_Chart_Table
        {
            public List<TBL_OZET_DTO> listChartOzet { get; set; }
            public List<TBL_OZET_DTO> listTableOzet { get; set; }
        }

        public List<TBL_OZET_DTO> realData = new List<TBL_OZET_DTO>();
        public JsonResult GetLineChart(string beginDate, int stationId)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("tr-TR");
            DateTime reqDateParam = DateTime.Parse(@beginDate).Date;
            Daily_Chart_Table dct = new Daily_Chart_Table();
            List<TBL_OZET_DTO> ozetler = DB.Summaries.Where(p => p.STATION_ID == stationId
            && p.tarih.Year == reqDateParam.Year && p.tarih.Month == reqDateParam.Month && p.tarih.Day == reqDateParam.Day)
                .OrderBy(a => a.tarih).Select(a =>
                 new TBL_OZET_DTO
                 {
                     _tarih = a.tarih,
                     _gunlukUretim = a.gunlukUretim == null ? 0 : (float)Math.Round((double)a.gunlukUretim,1)
                 }
                ).ToList();
            try
            {
                foreach (TBL_OZET_DTO item in ozetler)
                {
                    realData.Add(item);
                }

            }
            catch (Exception ex)
            {

   
            }
           

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

            dct.listTableOzet = realData;
            dct.listChartOzet = ozetler;
            return Json(dct, JsonRequestBehavior.AllowGet);
        }
    }
}