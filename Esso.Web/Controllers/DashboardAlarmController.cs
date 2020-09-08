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
using System.IO;
using OfficeOpenXml;
using System.Web.UI;
using language;
using Esso.Web.Helpers;

namespace Esso.Web.Controllers
{
    public class DashboardAlarmController : BaseController
    {
        // GET: 
        //public ActionResult Index(int stationId)
        //{
        //    return View(stationId);
        //}
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

        public ActionResult GetData(string stationId, string startDate, string endDate)
        {
            CultureHelper.SetCultureInfo();
            int start = Convert.ToInt32(Request["start"]);
            int length = Convert.ToInt32(Request["length"]);
            string searchValue = Request["search[value]"];
            string sortColumnName = Request["columns[" + Request["order[0][column]"] + "][data]"];
            string sortDirection = Request["order[0][dir]"];

            var role = User.IsInRole("DEMO");
            int station_id = Convert.ToInt32(stationId);
            var EndDate = Convert.ToDateTime(endDate).AddDays(1);
            var StartDate = Convert.ToDateTime(startDate);


            // var _list = DB.AlarmStatus.Where(x => x.STATION_ID == int.Parse(stationId) && x.STATUS != 2).OrderBy(x => x.START_DATE).ToList();
            using (EssoEntities db = new EssoEntities())
            {
                try
                {
                    var _Alarm_list = (from AS in db.AlarmStatus
                                       where AS.STATION_ID == station_id && AS.STATUS != 2 && EndDate >= AS.START_DATE && StartDate <= AS.START_DATE
                                       join AD in db.AlarmDesc on AS.ERROR_NUMBER equals AD.ERROR_NUMBER.ToString() into yG1
                                       from y1 in yG1.DefaultIfEmpty()
                                       join Inv in db.Inverters on AS.INVERTER_ID equals Inv.ID into yG2
                                       from y2 in yG2.DefaultIfEmpty()
                                       join sT in db.Stations on AS.STATION_ID equals sT.ID into yG3
                                       from y3 in yG3.DefaultIfEmpty()

                                       select new
                                       {

                                           ID = AS.ID,
                                           INVERTER_NAME = y2.NAME == null ? y3.NAME.ToString() : y2.NAME.ToString(),
                                           STATUS = AS.STATUS,
                                           START_DATE = AS.START_DATE,
                                           END_DATE = AS.END_DATE,
                                           ERROR_NUMBER = y1.ERROR_DESC,
                                           TYPE = y1.TYPE

                                       }).ToList().Select(o => new
                                       {

                                           ID = o.ID,
                                           INVERTER_NAME = o.INVERTER_NAME == null ? "-" : o.INVERTER_NAME.ToString(),
                                           STATUS = o.STATUS,
                                           START_DATE = o.START_DATE,
                                           END_DATE = o.END_DATE,
                                           ERROR_NUMBER = o.ERROR_NUMBER,
                                           TYPE = o.TYPE
                                       }).ToList();


                    int totalrows = _Alarm_list.Count;
                    if (!string.IsNullOrEmpty(searchValue))
                    {//filter
                        _Alarm_list = _Alarm_list.Where(x => x.ID.ToString().ToLower().Contains(searchValue.ToLower()) || x.INVERTER_NAME.ToString().ToLower().Contains(searchValue.ToLower()) || x.STATUS.ToString().ToLower().Contains(searchValue.ToLower())
                            || x.START_DATE.ToString().ToLower().Contains(searchValue.ToLower()) || x.END_DATE.ToString().ToLower().Contains(searchValue.ToLower()) || x.ERROR_NUMBER.ToLower().Contains(searchValue.ToLower())).ToList();
                    }
                    int totalrowsafterfiltering = _Alarm_list.Count();

                    _Alarm_list = _Alarm_list.OrderBy(sortColumnName + " " + sortDirection).ToList();
                    _Alarm_list = _Alarm_list.Skip(start).Take(length).ToList();


                    return Json(new
                    {
                        data = _Alarm_list.Select(o => new
                        {
                            ID = o.ID,
                            INVERTER_NAME = o.INVERTER_NAME,
                            STATUS = o.STATUS,
                            START_DATE = o.START_DATE.Value.ToString("dd/MM/yyyy HH:mm:ss"),
                            END_DATE = o.END_DATE == null ? "-" : o.END_DATE.Value.ToString("dd/MM/yyyy HH:mm:ss"),
                            ERROR_NUMBER = o.ERROR_NUMBER,
                            TYPE = o.TYPE
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


        public ActionResult ExportExcel(int stationId, string startDate, string endDate, string searchValue)
        {

            Thread.CurrentThread.CurrentCulture = new CultureInfo("tr-TR");
            int start = Convert.ToInt32(Request["start"]);
            int length = Convert.ToInt32(Request["length"]);
            //string searchValue = Request["search[value]"];
            string sortColumnName = Request["columns[" + Request["order[0][column]"] + "][data]"];
            string sortDirection = Request["order[0][dir]"];

            var role = User.IsInRole("DEMO");
            MemoryStream result = new MemoryStream();
            string excelreportPath = DateTime.Now + ".Alarmlar.xlsx";
            EssoEntities db = new EssoEntities();
            var StartDate = Convert.ToDateTime(startDate);
            var EndDate = Convert.ToDateTime(endDate).AddDays(1);

            var _Alarm_list = (from AS in db.AlarmStatus
                               where AS.STATION_ID == stationId && AS.STATUS != 2 && EndDate >= AS.START_DATE && StartDate <= AS.START_DATE
                               join AD in db.AlarmDesc on AS.ERROR_NUMBER equals AD.ERROR_NUMBER.ToString() into yG1
                               from y1 in yG1.DefaultIfEmpty()
                               join Inv in db.Inverters on AS.INVERTER_ID equals Inv.ID into yG2
                               from y2 in yG2.DefaultIfEmpty()
                               join sT in db.Stations on AS.STATION_ID equals sT.ID into yG3
                               from y3 in yG3.DefaultIfEmpty()
                               select new
                               {

                                   ID = AS.ID,
                                   INVERTER_NAME = y2.NAME == null ? y3.NAME.ToString() : y2.NAME.ToString(),
                                   STATUS = AS.STATUS,
                                   START_DATE = AS.START_DATE,
                                   END_DATE = AS.END_DATE,
                                   ERROR_NUMBER = y1.ERROR_DESC,
                                   TYPE = y1.TYPE

                               }).ToList().Select(o => new
                               {

                                   ID = o.ID,
                                   INVERTER_NAME = o.INVERTER_NAME == null ? "-" : o.INVERTER_NAME.ToString(),
                                   STATUS = o.STATUS,
                                   START_DATE = o.START_DATE,
                                   END_DATE = o.END_DATE,
                                   ERROR_NUMBER = o.ERROR_NUMBER,
                                   TYPE = o.TYPE
                               }).OrderByDescending(x => x.START_DATE).ToList();

            int totalrows = _Alarm_list.Count;
            if (!string.IsNullOrEmpty(searchValue))
            {
                _Alarm_list = _Alarm_list.Where(x => x.ID.ToString().ToLower().Contains(searchValue.ToLower()) || x.INVERTER_NAME.ToString().ToLower().Contains(searchValue.ToLower()) || x.STATUS.ToString().ToLower().Contains(searchValue.ToLower())
                    || x.START_DATE.ToString().ToLower().Contains(searchValue.ToLower()) || x.END_DATE.ToString().ToLower().Contains(searchValue.ToLower()) || x.ERROR_NUMBER.ToLower().Contains(searchValue.ToLower())).ToList();
            }

            using (ExcelPackage pck = new ExcelPackage())
            {

                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Report");

                ws.Column(1).Width = 30;
                ws.Column(2).Width = 20;
                ws.Column(3).Width = 20;
                ws.Column(4).Width = 25;

                ws.Cells["A1"].Value = @Resources.Error_Name;
                ws.Cells["B1"].Value = Resources.Error_Type;
                ws.Cells["C1"].Value = @Resources.Start_Date;
                ws.Cells["D1"].Value = @Resources.End_Date;

                ws.Cells["A1"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                ws.Cells["B1"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                ws.Cells["C1"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                ws.Cells["D1"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;

                ws.Cells["A1"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(string.Format("#6aa5e4")));
                ws.Cells["B1"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(string.Format("#6aa5e4")));
                ws.Cells["C1"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(string.Format("#6aa5e4")));
                ws.Cells["D1"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(string.Format("#6aa5e4")));


                for (int i = 0; i < _Alarm_list.Count; i++)
                {
                    var item = _Alarm_list[i];

                    ws.Cells["A" + (i + 2)].Value = _Alarm_list[i].ERROR_NUMBER;
                    ws.Cells["B" + (i + 2)].Value = _Alarm_list[i].INVERTER_NAME;
                    ws.Cells["C" + (i + 2)].Value = _Alarm_list[i].START_DATE.Value.ToString("dd/MM/yyyy HH:mm:ss");
                    ws.Cells["D" + (i + 2)].Value = _Alarm_list[i].END_DATE == null ? "-" : _Alarm_list[i].END_DATE.Value.ToString("dd/MM/yyyy HH:mm:ss");

                    if (_Alarm_list[i].TYPE == 1)
                    {

                        ws.Cells["A" + (i + 2)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        ws.Cells["B" + (i + 2)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        ws.Cells["C" + (i + 2)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        ws.Cells["D" + (i + 2)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;

                        ws.Cells["A" + (i + 2)].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(string.Format("#ff0000")));
                        ws.Cells["B" + (i + 2)].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(string.Format("#ff0000")));
                        ws.Cells["C" + (i + 2)].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(string.Format("#ff0000")));
                        ws.Cells["D" + (i + 2)].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(string.Format("#ff0000")));
                    }
                    else if (_Alarm_list[i].TYPE == 2)
                    {

                        ws.Cells["A" + (i + 2)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        ws.Cells["B" + (i + 2)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        ws.Cells["C" + (i + 2)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        ws.Cells["D" + (i + 2)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;

                        ws.Cells["A" + (i + 2)].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(string.Format("#ffc400")));
                        ws.Cells["B" + (i + 2)].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(string.Format("#ffc400")));
                        ws.Cells["C" + (i + 2)].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(string.Format("#ffc400")));
                        ws.Cells["D" + (i + 2)].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(string.Format("#ffc400")));


                    }


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
            return View();
        }


    }


}
