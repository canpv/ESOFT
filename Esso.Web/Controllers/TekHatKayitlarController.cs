using DevExpress.Web.Demos;
using DevExpress.Web.Mvc;
using Esso.Data;
using Esso.Web.ViewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq.Dynamic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Z.EntityFramework.Plus;
using static DevExpress.XtraPrinting.Native.ExportOptionsPropertiesNames;
using static Esso.Web.Controllers.HomeController;
using DevExpress.XtraGrid;
using System.Drawing;
using DevExpress.XtraPrinting;
using Esso.Models;
using Esso.Model.Models;


namespace Esso.Web.Controllers
{
    public class TekHatKayitlarController : BaseController
    {
      
        public ActionResult Index(int stationId)
        {
            return View(stationId);
        }
      
        public ActionResult GetData(string stationId)
        {

            int start = Convert.ToInt32(Request["start"]);
            int length = Convert.ToInt32(Request["length"]);
            string searchValue = Request["search[value]"];
            string sortColumnName = Request["columns[" + Request["order[0][column]"] + "][data]"];
            string sortDirection = Request["order[0][dir]"];
            List<TBL_MODBUS_CMD_LOG> LogList = new List<TBL_MODBUS_CMD_LOG>();
            int station_id = Convert.ToInt32(stationId);

            using (EssoEntities db = new EssoEntities())
            {
                try
                {
                    var LogList1 = (from ML in db.ModbusLog
                                    where ML.STATION_ID == station_id
                                    join Us in db.Users on ML.USER_ID equals Us.Id.ToString() into yG1
                                    from y1 in yG1.DefaultIfEmpty()
                                    join Md in db.ModbusTag on ML.ADDRESS equals Md.ADDRESS into yG2
                                    from y2 in yG2.DefaultIfEmpty()
                                    select new
                                    {

                                        ML.ID,
                                        INSERT_DATE = ML.INSERT_DATE,
                                        ML.OLD_VALUE,
                                        ADDRESS = y2.NAME,
                                        ML.STATION_ID,
                                        UserName = y1.UserName,
                                        ML.VALUE

                                    }).ToList().Select(o => new
                                    {
                                        o.ID,
                                        INSERT_DATE = o.INSERT_DATE,
                                        OLD_VALUE = o.OLD_VALUE == 2 ? "KAPALI" : o.OLD_VALUE == 1 ? "AÇIK" : o.OLD_VALUE.ToString(),
                                        ADDRESS = o.ADDRESS == null ? "-" : o.ADDRESS.ToString(),
                                        o.STATION_ID,
                                        UserName = o.UserName == null ? "-" : o.UserName,
                                        VALUE = o.VALUE == 2 ? "KAPALI" : o.VALUE == 1 ? "AÇIK" : o.VALUE.ToString(),
                                    }).ToList();

                    int totalrows = LogList1.Count;
                    if (!string.IsNullOrEmpty(searchValue))
                    {//filter
                        LogList1 = LogList1.Where(x => x.ADDRESS.ToString().ToLower().Contains(searchValue.ToLower()) || x.INSERT_DATE.ToString().ToLower().Contains(searchValue.ToLower()) || x.VALUE.ToString().ToLower().Contains(searchValue.ToLower())
                            || x.STATION_ID.ToString().ToLower().Contains(searchValue.ToLower()) || x.OLD_VALUE.ToString().ToLower().Contains(searchValue.ToLower()) || x.UserName.ToLower().Contains(searchValue.ToLower())).ToList();
                    }
                    int totalrowsafterfiltering = LogList1.Count();

                    LogList1 = LogList1.OrderBy(sortColumnName + " " + sortDirection).ToList();
                    LogList1 = LogList1.Skip(start).Take(length).ToList();


                    return Json(new
                    {
                        data = LogList1.Select(o => new
                        {
                            o.ID,
                            INSERT_DATE = o.INSERT_DATE.ToString("dd/MM/yyyy HH:mm:ss"),
                            OLD_VALUE = o.OLD_VALUE,
                            ADDRESS = o.ADDRESS,
                            o.STATION_ID,
                            UserName = o.UserName,
                            VALUE = o.VALUE,
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
            }
            return View();
        }
    }
  




}






