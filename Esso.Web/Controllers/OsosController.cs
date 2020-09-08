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
    public class OsosController : BaseController
    {
        public ActionResult Index(int stationId)
        {
            return View(stationId);
        }
        public ActionResult ExportExcel(int stationId, string startDate, string endDate)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("tr-TR");
            int start = Convert.ToInt32(Request["start"]);
            int length = Convert.ToInt32(Request["length"]);
            string sortColumnName = Request["columns[" + Request["order[0][column]"] + "][data]"];
            string sortDirection = Request["order[0][dir]"];
            var role = User.IsInRole("DEMO");
            MemoryStream result = new MemoryStream();
            string excelreportPath = DateTime.Now + ".OsosKayitlar.xlsx";
            var EndDate = Convert.ToDateTime(endDate).AddDays(1);
            var StartDate = Convert.ToDateTime(startDate);

            using (EssoEntities db = new EssoEntities())
            {

                var OsosList = (from Os in db.Osos
                                where Os.STATION_ID == stationId && EndDate >= Os.CREATED_DATE && StartDate <= Os.CREATED_DATE
                                orderby Os.CREATED_DATE

                                select new
                                {

                                    ID = Os.ID,
                                    STATION_ID = Os.STATION_ID,
                                    DEMAND = Os.DEMAND,
                                    P_180 = Os.P_180,
                                    RI_580 = Os.RI_580,
                                    RC_880 = Os.RC_880,
                                    P_280 = Os.P_280,
                                    RI_680 = Os.RI_680,
                                    RC_780 = Os.RC_780,
                                    INSERT_DATE = Os.INSERT_DATE,
                                    CREATED_DATE = Os.CREATED_DATE,
                                    INDUCTIVE_REAKTIF_RATE = Os.INDUCTIVE_REAKTIF_RATE,
                                    CAPACITIVE_REAKTIF_RATE = Os.CAPACITIVE_REAKTIF_RATE,


                                }).ToList();



                using (ExcelPackage pck = new ExcelPackage())
                {
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Report");

                    ws.Column(1).Width = 25;
                    ws.Column(1).Width = 20;
                    ws.Column(3).Width = 15;
                    ws.Column(4).Width = 15;
                    ws.Column(5).Width = 20;
                    ws.Column(6).Width = 20;
                    ws.Column(7).Width = 15;
                    ws.Column(8).Width = 15;
                    ws.Column(9).Width = 25;
                    ws.Column(10).Width = 25;

                    ws.Cells["A1"].Value = "Tarih";
                    ws.Cells["B1"].Value = "Demand";
                    ws.Cells["C1"].Value = "-P(2.8.0)";
                    ws.Cells["D1"].Value = "+Rc(7.8.0)";
                    ws.Cells["E1"].Value = "-Ri(6.8.0)";
                    ws.Cells["F1"].Value = "+P(1.8.0)";
                    ws.Cells["G1"].Value = "+Ri(5.8.0)";
                    ws.Cells["H1"].Value = "-Rc(8.8.0)";
                    ws.Cells["I1"].Value = "Endüktif Reaktif Oran";
                    ws.Cells["J1"].Value = "Kapasitif Reaktif Oran";

                    ws.Cells["A1"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    ws.Cells["B1"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    ws.Cells["C1"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    ws.Cells["D1"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    ws.Cells["E1"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    ws.Cells["F1"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    ws.Cells["G1"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    ws.Cells["H1"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    ws.Cells["I1"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    ws.Cells["J1"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;





                    ws.Cells["A1"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(string.Format("#6aa5e4")));
                    ws.Cells["B1"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(string.Format("#6aa5e4")));
                    ws.Cells["C1"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(string.Format("#6aa5e4")));
                    ws.Cells["D1"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(string.Format("#6aa5e4")));
                    ws.Cells["E1"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(string.Format("#6aa5e4")));
                    ws.Cells["F1"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(string.Format("#6aa5e4")));
                    ws.Cells["G1"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(string.Format("#6aa5e4")));
                    ws.Cells["H1"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(string.Format("#6aa5e4")));
                    ws.Cells["I1"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(string.Format("#6aa5e4")));
                    ws.Cells["J1"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(string.Format("#6aa5e4")));

                    for (int i = 0; i < OsosList.Count; i++)
                    {

                        ws.Cells["B" + (i + 2)].Style.Numberformat.Format = "#,##0.00";
                        ws.Cells["C" + (i + 2)].Style.Numberformat.Format = "#,##0.00";
                        ws.Cells["D" + (i + 2)].Style.Numberformat.Format = "#,##0.00";
                        ws.Cells["E" + (i + 2)].Style.Numberformat.Format = "#,##0.00";
                        ws.Cells["F" + (i + 2)].Style.Numberformat.Format = "#,##0.00";
                        ws.Cells["G" + (i + 2)].Style.Numberformat.Format = "#,##0.00";
                        ws.Cells["H" + (i + 2)].Style.Numberformat.Format = "#,##0.00";
                        ws.Cells["I" + (i + 2)].Style.Numberformat.Format = "#,##0.00";
                        ws.Cells["J" + (i + 2)].Style.Numberformat.Format = "#,##0.00";

                        var item = OsosList[i];

                        ws.Cells["A" + (i + 2)].Value = OsosList[i].CREATED_DATE.ToString("dd/MM/yyyy HH:mm:ss");
                        ws.Cells["B" + (i + 2)].Value = OsosList[i].DEMAND;
                        ws.Cells["C" + (i + 2)].Value = OsosList[i].P_280;
                        ws.Cells["D" + (i + 2)].Value = OsosList[i].RI_680;
                        ws.Cells["E" + (i + 2)].Value = OsosList[i].RC_780;
                        ws.Cells["F" + (i + 2)].Value = OsosList[i].P_180;
                        ws.Cells["G" + (i + 2)].Value = OsosList[i].RI_580;
                        ws.Cells["H" + (i + 2)].Value = OsosList[i].RC_880;
                        ws.Cells["I" + (i + 2)].Value = OsosList[i].INDUCTIVE_REAKTIF_RATE;
                        ws.Cells["J" + (i + 2)].Value = OsosList[i].CAPACITIVE_REAKTIF_RATE;
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
            }
            return View();
        }
        public ActionResult GetData(string stationId, string startDate, string endDate)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("tr-TR");
            int start = Convert.ToInt32(Request["start"]);
            int length = Convert.ToInt32(Request["length"]);
            string sortColumnName = Request["columns[" + Request["order[0][column]"] + "][data]"];
            string sortDirection = Request["order[0][dir]"];
            int station_id = Convert.ToInt32(stationId);
            var EndDate = Convert.ToDateTime(endDate).AddDays(1);
            var StartDate = Convert.ToDateTime(startDate);

            using (EssoEntities db = new EssoEntities())
            {
                try
                {
                    var OsosList = (from Os in db.Osos
                                    where Os.STATION_ID == station_id && EndDate >= Os.CREATED_DATE && StartDate <= Os.CREATED_DATE
                                    select new
                                    {
                                        ID = Os.ID,
                                        STATION_ID = Os.STATION_ID,
                                        DEMAND = Os.DEMAND,
                                        P_180 = Os.P_180,
                                        RI_580 = Os.RI_580,
                                        RC_880 = Os.RC_880,
                                        P_280 = Os.P_280,
                                        RI_680 = Os.RI_680,
                                        RC_780 = Os.RC_780,
                                        INSERT_DATE = Os.INSERT_DATE,
                                        CREATED_DATE = Os.CREATED_DATE,
                                        INDUCTIVE_REAKTIF_RATE = Os.INDUCTIVE_REAKTIF_RATE,
                                        CAPACITIVE_REAKTIF_RATE = Os.CAPACITIVE_REAKTIF_RATE
                                    }).ToList();

                    int totalrows = OsosList.Count;
                    int totalrowsafterfiltering = OsosList.Count();
                    OsosList = OsosList.OrderBy(sortColumnName + " " + sortDirection).ToList();
                    OsosList = OsosList.Skip(start).Take(length).ToList();
                    return Json(new
                    {
                        data = OsosList.Select(o => new
                        {
                            ID = o.ID,
                            STATION_ID = o.STATION_ID,
                            DEMAND = o.DEMAND,
                            P_180 = o.P_180,
                            RI_580 = o.RI_580,
                            RC_880 = o.RC_880,
                            P_280 = o.P_280,
                            RI_680 = o.RI_680,
                            RC_780 = o.RC_780,
                            INDUCTIVE_REAKTIF_RATE = o.INDUCTIVE_REAKTIF_RATE,
                            CAPACITIVE_REAKTIF_RATE = o.CAPACITIVE_REAKTIF_RATE,

                            INSERT_DATE = o.INSERT_DATE.ToString("dd/MM/yyyy HH:mm:ss"),
                            CREATED_DATE = o.CREATED_DATE.ToString("dd/MM/yyyy HH:mm:ss"),

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
        public class Progress
        {
            public string Progress1 { get; set; }
            public string Progress2 { get; set; }
            public bool isSuccess { get; set; }
            public string ErrorMessage { get; set; }

        }
        public JsonResult GetProgress(string stationId)
        {
            Progress mwc = new Progress();
            Thread.CurrentThread.CurrentCulture = new CultureInfo("tr-TR");
            int station_id = Convert.ToInt32(stationId);
            // && EndDate >= Os.CREATED_DATE && StartDate <= Os.CREATED_DATE
            var DatetimeMonth = DateTime.Now.Month;
            var DatetimeYear = DateTime.Now.Year;

            using (EssoEntities db = new EssoEntities())
            {
                try
                {
                    var OsosList = (from Os in db.Osos
                                    where Os.STATION_ID == station_id && DatetimeMonth == Os.CREATED_DATE.Month && DatetimeYear == Os.CREATED_DATE.Year
                                    orderby Os.CREATED_DATE
                                    select new
                                    {
                                        ID = Os.ID,
                                        STATION_ID = Os.STATION_ID,
                                        DEMAND = Os.DEMAND,
                                        P_180 = Os.P_180,
                                        RI_580 = Os.RI_580,
                                        RC_880 = Os.RC_880,
                                        P_280 = Os.P_280,
                                        RI_680 = Os.RI_680,
                                        RC_780 = Os.RC_780,
                                        INSERT_DATE = Os.INSERT_DATE,
                                        CREATED_DATE = Os.CREATED_DATE,
                                        INDUCTIVE_REAKTIF_RATE = Os.INDUCTIVE_REAKTIF_RATE,
                                        CAPACITIVE_REAKTIF_RATE = Os.CAPACITIVE_REAKTIF_RATE
                                    }).ToList();

                    var OsosFirstDayofMonth = (from Os in db.Osos
                                               where Os.STATION_ID == station_id && DatetimeMonth == Os.CREATED_DATE.Month && DatetimeYear == Os.CREATED_DATE.Year && Os.CREATED_DATE.Day == 1
                                               orderby Os.CREATED_DATE
                                               select new
                                               {
                                                   ID = Os.ID,
                                                   STATION_ID = Os.STATION_ID,
                                                   DEMAND = Os.DEMAND,
                                                   P_180 = Os.P_180,
                                                   RI_580 = Os.RI_580,
                                                   RC_880 = Os.RC_880,
                                                   P_280 = Os.P_280,
                                                   RI_680 = Os.RI_680,
                                                   RC_780 = Os.RC_780,
                                                   INSERT_DATE = Os.INSERT_DATE,
                                                   CREATED_DATE = Os.CREATED_DATE,
                                                   INDUCTIVE_REAKTIF_RATE = Os.INDUCTIVE_REAKTIF_RATE,
                                                   CAPACITIVE_REAKTIF_RATE = Os.CAPACITIVE_REAKTIF_RATE
                                               }).ToList();

                    float? mountly_INDUCTIVE_REAKTIF_RATE = 0;
                    float? mountly_CAPACITIVE_REAKTIF_RATE = 0;
                    if (OsosFirstDayofMonth.Count != 0)
                    {
                        mountly_INDUCTIVE_REAKTIF_RATE = ((OsosList[OsosList.Count - 1].RI_580 - (OsosList[0].RI_580)) / (OsosList[OsosList.Count - 1].P_180 - (OsosList[0].P_180))) * 100;
                        mountly_CAPACITIVE_REAKTIF_RATE = ((OsosList[OsosList.Count - 1].RC_880 - (OsosList[0].RC_880)) / (OsosList[OsosList.Count - 1].P_180 - (OsosList[0].P_180))) * 100;


                    }

                    mwc.Progress1 = mountly_INDUCTIVE_REAKTIF_RATE.ToString().Replace(",", ".");
                    mwc.Progress2 = mountly_CAPACITIVE_REAKTIF_RATE.ToString().Replace(",", ".");
                    mwc.isSuccess = true;
                    mwc.ErrorMessage = "";

                    return Json(mwc, JsonRequestBehavior.AllowGet);



                }
                catch (Exception ex)
                {
                    mwc.Progress1 = "";
                    mwc.Progress2 = "";
                    mwc.isSuccess = false;
                    mwc.ErrorMessage = ex.ToString();
                    var a = ex.ToString();
                    return Json(mwc, JsonRequestBehavior.AllowGet);
                }
            }

        }
    }





}






