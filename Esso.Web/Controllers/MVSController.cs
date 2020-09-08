using Esso.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Esso.Web.Controllers
{
    public class MVSController : BaseController
    {
        // GET: MVS

        EssoEntities DB = new EssoEntities();

        [Authorize]
        public ActionResult Index(int stationId)
        {
            return View(stationId);
        }

        public JsonResult GetEkk(int stationId)
        {
            var ekk = DB.Summaries.Where(p => p.STATION_ID == stationId).OrderByDescending(a => a.tarih).FirstOrDefault();

            return Json(ekk, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetHucre(int stationId)
        {
            string[] hucreTagName = { "h01_toprak_ayirici", "h01_ayirici", "h02_toprak_ayirici", "h02_ayirici", "h03_toprak_ayirici", "h03_ayirici","h3_52cb" };
            int[] hucreName = DB.Tags.Where(a => a.IS_DELETED == false && hucreTagName.Contains(a.NAME)).Select(a => a.ID).ToArray();

            var hucreBilgileri = (from u in DB.DigitalLogsLive
                                  join v in DB.Tags on u.TAG_ID equals v.ID
                                  where u.TAG_ID == v.ID && v.IS_DELETED == false && u.STATION_ID == stationId && hucreName.Contains(v.ID)
                                  orderby u.CREATED_DATE descending
                                  select new {
									  _tagId = u.TAG_ID,
									  _tagName = v.NAME,
									  _value = u.DESC,
									  _date = u.CREATED_DATE
								  }).ToList().GroupBy(g => g._tagId).Select(p => new { hucre = p.First() }).ToList();

            return Json(hucreBilgileri, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDigitalData(int stationId)
        {
            string[] digitalTagName = { "kapi_switch", "redresor_ac_ariza", "redresor_dc_dusuk", "redresor_dc_kacak", "redresor_dc_yuksek", "duman_dedektoru" };
            int[] digitalTagId = DB.Tags.Where(x => x.IS_DELETED == false && digitalTagName.Contains(x.NAME)).Select(x => x.ID).ToArray();

            var digitalData = (from dll in DB.DigitalLogsLive
                               join t in DB.Tags on dll.TAG_ID equals t.ID
                               where dll.TAG_ID == t.ID
                                     && t.IS_DELETED == false
                                     && dll.STATION_ID == stationId
                                     && digitalTagId.Contains(t.ID)
                               select new
                               {
                                   tagId = dll.TAG_ID,
                                   tagName = t.NAME,
                                   value = dll.DESC,
                                   date = dll.CREATED_DATE
                               }).ToList().GroupBy(grp => grp.tagId).Select(x => new { hucre = x.First() }).ToList();

            return Json(digitalData,JsonRequestBehavior.AllowGet);
        }
    }
}