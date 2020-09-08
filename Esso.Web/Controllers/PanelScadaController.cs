using Esso.Data;
using Esso.Model.Models;
using Esso.Web.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Esso.Web.Controllers
{
    public class PanelScadaController : BaseController
    {
        // GET: PanelScada
        EssoEntities DB = new EssoEntities();
        public ActionResult Index(int stationId)
        {
            var request = WebRequest.Create("https://onesignal.com/api/v1/notifications") as HttpWebRequest;

            request.KeepAlive = true;
            request.Method = "POST";
            request.ContentType = "application/json; charset=utf-8";

            request.Headers.Add("authorization", "Basic YOUR_REST_API_KEY");

            var serializer = new JavaScriptSerializer();
            var obj = new
            {
                app_id = "5eb5a37e-b458-11e3-ac11-000c2940e62c",
                contents = new { en = "English Message" },
                included_segments = new string[] { "Active Users" }
            };
            var param = serializer.Serialize(obj);
            byte[] byteArray = Encoding.UTF8.GetBytes(param);

            string responseContent = null;

            try
            {
                using (var writer = request.GetRequestStream())
                {
                    writer.Write(byteArray, 0, byteArray.Length);
                }

                using (var response = request.GetResponse() as HttpWebResponse)
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        responseContent = reader.ReadToEnd();
                    }
                }
            }
            catch (WebException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                System.Diagnostics.Debug.WriteLine(new StreamReader(ex.Response.GetResponseStream()).ReadToEnd());
            }

            System.Diagnostics.Debug.WriteLine(responseContent);
            //var stat = DB.Stations.Where(w => w.ID == stationId).FirstOrDefault();
            //var location = stat.COORDINATE_INFORMATION.ToString().Split(',').ToArray();
            //var _lat = location[0];
            //var _lng = location[1];
            //Thread.CurrentThread.CurrentCulture = new CultureInfo("tr-TR");
            //string urlParemeters = "?lat=" + _lat + "&lng=" + _lng + "&date=2019-11-30";
            ////string urlParemeters = "?lat=38.426987&lng=27.187813&date=2019-11-30";
            //HttpClient client = new HttpClient();
            //client.BaseAddress = new Uri("https://api.sunrise-sunset.org/json");

            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //HttpResponseMessage response = client.GetAsync(urlParemeters).Result;
            //if (response.IsSuccessStatusCode)
            //{
            //    var dataObjects = response.Content.ReadAsAsync<DataLog>().Result;
            //    DateTime _sunrise = DateTime.Parse(dataObjects.results.sunrise.ToString());
            //    DateTime _sunset = DateTime.Parse(dataObjects.results.sunset.ToString());

            //    TimeZone zone = TimeZone.CurrentTimeZone;
            //    TimeSpan offset = zone.GetUtcOffset(_sunrise);

            //    _sunrise = _sunrise.AddHours(offset.Hours);
            //    _sunset = _sunset.AddHours(offset.Hours);

            //}

            return View(stationId);
        }


        public JsonResult Sunset_Sunrise(int stationId)
        {
            var isSucces = false;
            Sunset_Sunrise_DTO sdto = new Sunset_Sunrise_DTO();
            try
            {

                var stat = DB.Stations.Where(w => w.ID == stationId).FirstOrDefault();
                var location = stat.COORDINATE_INFORMATION.ToString().Split(',').ToArray();
                var _lat = location[0];
                var _lng = location[1];
                DateTime nowDate = DateTime.Now;
                Thread.CurrentThread.CurrentCulture = new CultureInfo("tr-TR");
                string urlParemeters = "?lat=" + _lat + "&lng=" + _lng + "&date=2019-11-30";
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("https://api.sunrise-sunset.org/json");

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = client.GetAsync(urlParemeters).Result;


                if (response.IsSuccessStatusCode)
                {
                    var dataObjects = response.Content.ReadAsAsync<DataLog>().Result;
                    sdto.sunrise = DateTime.Parse(dataObjects.results.sunrise.ToString());
                    sdto.sunset = DateTime.Parse(dataObjects.results.sunset.ToString());

                    TimeZone zone = TimeZone.CurrentTimeZone;
                    TimeSpan offset = zone.GetUtcOffset(sdto.sunrise);

                    sdto.sunrise = sdto.sunrise.AddHours(offset.Hours);
                    sdto.sunset = sdto.sunset.AddHours(offset.Hours);
                    isSucces = true;

                    if (sdto.sunset > sdto.sunrise)
                    {
                        TimeSpan timeDifference = sdto.sunset - sdto.sunrise;
                        sdto.totalDayTime = timeDifference.TotalMinutes;
                    }

                    if (nowDate > sdto.sunrise)
                    {
                        TimeSpan timeDifference = nowDate - sdto.sunrise;
                        sdto.elapsedTime = timeDifference.TotalMinutes;
                    }

                    if (sdto.totalDayTime > 0 && sdto.elapsedTime > 0)
                    {
                        var _percent = sdto.elapsedTime / sdto.totalDayTime * 100;
                        sdto.percent = _percent;
                    }
                    if (sdto.sunset > nowDate)
                    {
                        TimeSpan timeDifference = sdto.sunset - nowDate;
                        sdto.remainingTime = (timeDifference.ToString("%h").Length < 2 ? "0" + timeDifference.ToString("%h") : timeDifference.ToString("%h")) + ":" + (timeDifference.ToString("%m").Length < 2 ? "0" + timeDifference.ToString("%m") : timeDifference.ToString("%m"));
                    }
                }
            }
            catch (Exception ex)
            {
                isSucces = false;
            }


            return Json(sdto, JsonRequestBehavior.AllowGet);
        }

        public class Sunset_Sunrise_DTO
        {
            public DateTime sunrise { get; set; }
            public DateTime sunset { get; set; }
            public double percent { get; set; }
            public double totalDayTime { get; set; } = 0;
            public double elapsedTime { get; set; } = 0;
            public string remainingTime { get; set; } = "";
        }

        public class DataLog
        {
            public results results { get; set; }

        }
        public class results
        {
            public string sunrise { get; set; }
            public string sunset { get; set; }

        }

        public JsonResult SavePanelLocation(int stationId, int stringId, string locX, string locY)
        {
            var isTherePanel = DB.PanelLocations.Where(w => w.STATION_ID == stationId && w.STRING_ID == stringId).FirstOrDefault();

            if (isTherePanel == null)
            {
                DB.PanelLocations.Add(new TBL_PANEL_LOCATIONS { STATION_ID = stationId, STRING_ID = stringId, LOCATIONSX = locX, LOCATIONSY = locY, UPDATE_DATE = DateTime.Now });
                DB.SaveChanges();
            }
            else
            {
                TBL_PANEL_LOCATIONS updateItem = isTherePanel;
                updateItem.LOCATIONSX = locX;
                updateItem.LOCATIONSY = locY;
                updateItem.UPDATE_DATE = DateTime.Now;

                DB.Entry(updateItem).State = EntityState.Modified;
                DB.SaveChanges();
            }

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SavePanelResize(int stationId, int stringId, int height, int width)
        {

            var isTherePanel = DB.PanelLocations.Where(w => w.STATION_ID == stationId && w.STRING_ID == stringId).FirstOrDefault();

            if (isTherePanel != null)
            {
                TBL_PANEL_LOCATIONS updateItem = isTherePanel;
                updateItem.RESIZE_HEIGHT = height;
                updateItem.RESIZE_WIDTH = width;
                updateItem.UPDATE_DATE = DateTime.Now;

                DB.Entry(updateItem).State = EntityState.Modified;
                DB.SaveChanges();
            }


            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetStringTags(int stationId)
        {
            var panelLocations = DB.PanelLocations.Where(w => w.STATION_ID == stationId).ToList();

            List<TempDTO> strTagNames = DB.stationString
               .Join(DB.Tags, r => r.STRING_ID, ro => ro.ID, (r, ro) => new { r, ro })
               .Where(x => x.r.STATION_ID == stationId && x.r.IS_DELETED == false)
              .GroupBy(x => x.ro.NAME)
              .Select(g => new TempDTO { NAME = g.Key, ID = g.FirstOrDefault().ro.ID })
              .ToList();

            foreach (var item in panelLocations)
            {
                var panel = strTagNames.Where(w => w.ID == item.STRING_ID).FirstOrDefault();
                if (panel != null)
                {
                    panel.LOCATIONSX = item.LOCATIONSX;
                    panel.LOCATIONSY = item.LOCATIONSY;
                    panel.RESIZE_HEIGHT = item.RESIZE_HEIGHT;
                    panel.RESIZE_WIDTH = item.RESIZE_WIDTH;
                    panel.UPDATE_DATE = item.UPDATE_DATE;
                }

            }

            return Json(strTagNames, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetStringData(int stationId)
        {
            var stat = DB.Stations.Where(w => w.ID == stationId).FirstOrDefault();
            List<STRING_INV_DTO> data = new List<STRING_INV_DTO>();
            var values = (from so in DB.StringOzetLive
                          join t in DB.Tags on so.STRING_ID equals t.ID
                          join tct in DB.stationString on t.ID equals tct.STRING_ID
                          where so.STATION_ID == stationId
                          && tct.STATION_ID == stationId
                          && tct.IS_DELETED == false
                          && t.IS_DELETED == false
                          && tct.IS_DELETED == false
                          select new CELL_DTO
                          {
                              NAME = t.NAME,
                              VALUE = so.VALUE,
                              ID = t.ID

                          }).ToList();

            if (stat.STATION_TYPE == 4)
            {
                var groupVal = values.GroupBy(grp => grp.NAME.Substring(0, 4)).ToList();
                foreach (var item in values)
                {
                    item.INV_NO = item.NAME.Substring(0, 7).ToString();
                }
            }
            else
            {
                foreach (var item in values)
                {
                    item.INV_NO = item.NAME.Split('_')[0].Replace("INV", "");
                }
            }


            data = values.GroupBy(grp => grp.INV_NO).OrderBy(o => o.Key).
                Select(s => new STRING_INV_DTO
                {
                    INV_NO = s.Key.ToString(),
                    listCell = values.Where(w => w.INV_NO == s.Key).ToList(),
                    MIN_VALUE = s.ToList().Min(m => m.VALUE),
                    MAX_VALUE = s.ToList().Max(m => m.VALUE),
                    AVG_VALUE = (float)Math.Round(s.ToList().Average(m => m.VALUE.Value), 2)

                })
                .ToList();


            return Json(data, JsonRequestBehavior.AllowGet);
        }



        public class TempDTO
        {
            public int ID { get; set; }
            public string NAME { get; set; }
            public int INV_NO { get; set; }
            public int INPUT_NO { get; set; }
            public float? VALUE { get; set; }
            public long TARIH_NUMBER { get; set; }
            public string DISPLAY_NAME { get; set; }
            public string LOCATIONSX { get; set; }
            public string LOCATIONSY { get; set; }
            public int? RESIZE_WIDTH { get; set; }
            public int? RESIZE_HEIGHT { get; set; }
            public DateTime? UPDATE_DATE { get; set; }
        }

        public JsonResult AuthorizeControl()
        {

            bool isUserAuthority = false;

            if (User.IsInRole("M_ADMIN") || User.IsInRole("COMP_ADMIN"))
            {
                isUserAuthority = true;
            }
            else
            {
                isUserAuthority = false;
            }
            return Json(isUserAuthority, JsonRequestBehavior.AllowGet);
        }
    }
}