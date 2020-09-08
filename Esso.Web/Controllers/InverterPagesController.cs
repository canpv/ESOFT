using DevExpress.Web;
using DevExpress.Web.Demos;
using DevExpress.Web.Mvc;
using DevExpress.XtraPrinting;
using Esso.Data;
using Esso.Model.Models;
using Esso.Models;
using Esso.Web.Helpers;
using Esso.Web.ViewModels;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using Esso.Web.Models.DATE_NUMBER;

namespace Esso.Web.Controllers
{
    [Authorize]
    public class InverterPagesController : BaseController
    {
        // GET: InverterPages
        EssoEntities DB = new EssoEntities();
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
        //public class NUMBER_FORMAT_DTO
        //{
        //    public long _begin { get; set; }
        //    public long _end { get; set; }
        //}

        public NUMBER_FORMAT_DTO ConvertNumberFormat(string date)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("tr-TR");
            DateTime selectDate = DateTime.Parse(date);
            string _convertDate = "";
            string _year = selectDate.Year.ToString();
            string _month = selectDate.Month.ToString();
            string _day = selectDate.Day.ToString();
            if (_month.Length < 2)
            {
                _month = "0" + selectDate.Month.ToString();
            }
            if (_day.Length < 2)
            {
                _day = "0" + selectDate.Day.ToString();
            }
            _convertDate = _year + _month + _day;
            string _strDateBegin = _convertDate + "000000";
            string _strDateEnd = _convertDate + "235959";
            NUMBER_FORMAT_DTO ndto = new NUMBER_FORMAT_DTO();
            ndto._begin = Convert.ToInt64(_strDateBegin);
            ndto._end = Convert.ToInt64(_strDateEnd);
            return ndto;
        }
        public ActionResult Detail(int stationId)
        {
            return View(stationId);
        }

        public ActionResult InvPerformanceChart(int stationId)
        {
            return View(stationId);
        }
        public ActionResult OnlyInverterDetail(int stationId)
        {
            return View(stationId);
        }

        public JsonResult HourlyColorReport(int stationId, string slctDate)
        {
            InverterPerformanceView mdl = new InverterPerformanceView();
            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("tr-TR");
                DateTime nowDate = DateTime.Parse(slctDate);
                NUMBER_FORMAT_DTO convertFormat = ConvertNumberFormat(slctDate);
                long _numberDateBegin = convertFormat._begin;
                long _numberDateEnd = convertFormat._end;

                mdl.invModel = new InverterModel();
                var hourlyProduction = (from InverterDetail in DB.InvSums
                                        join Inverter in DB.Inverters on
                                        InverterDetail.Inv_Id equals Inverter.ID
                                        where
                                        InverterDetail.STATION_ID == stationId
                                         && InverterDetail.TARIH_NUMBER >= _numberDateBegin && InverterDetail.TARIH_NUMBER <= _numberDateEnd
                                         && InverterDetail.Tarih.Value.Hour > 5
                                         && InverterDetail.Tarih.Value.Hour < 22
                                        select new Hour_DTO_
                                        {
                                            InverterName = Inverter.NAME,
                                            result = Inverter.INV_DC_GUC == null || InverterDetail.Guc_AC == null ? 0 : Math.Round(((float)InverterDetail.Guc_AC / 1000) / ((float)Inverter.INV_DC_GUC), 1),//Math.Round((double)(InverterDetail.Guc_AC == null ? 0 : InverterDetail.Guc_AC) / 1000, 2) / Math.Round((double)(Inverter.INV_DC_GUC == null ? 1 : Inverter.INV_DC_GUC), 2),
                                            InverterId = InverterDetail.Inv_Id,
                                            _tarih = InverterDetail.Tarih.Value
                                        })
                                       .AsEnumerable()
                                       .Distinct()
                                       .ToList();
                mdl.invModel.InverterList = hourlyProduction.OrderBy(a => a.InverterId)
                        .Select(a => a.InverterName)
                        .Distinct()
                        .ToList();

                var list_ = new List<Values>();
                var groupList = hourlyProduction.OrderBy(a => a.InverterId).GroupBy(a => a.InverterId).ToList();

                foreach (var hour in groupList[0].OrderBy(a => a._tarih))
                {
                    mdl.invModel.Hours.Add(hour._tarih.ToString("HH:mm"));
                }

                foreach (var item in groupList)
                {
                    var list = new Values();
                    foreach (var i in item.OrderBy(a => a._tarih).ToList())
                    {
                        list.values.Add((float)i.result);
                    }
                    list_.Add(list);
                }

                mdl.invModel.series = list_;
                mdl.ErrorMessage = "";
            }
            catch (Exception ex)
            {
                mdl.ErrorMessage = ex.ToString();
            }
            return Json(mdl);
        }

        public NUMBER_FORMAT_DTO ConvertNumberFormatInvHeatMapMonthly(string date)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("tr-TR");
            DateTime selectDate = DateTime.Parse(date);
            string _convertDate = "";
            string _year = selectDate.Year.ToString();
            string _month = selectDate.Month.ToString();
   
            if (_month.Length < 2)
            {
                _month = "0" + selectDate.Month.ToString();
            }
            
            _convertDate = _year + _month;
            string _strDateBegin = _convertDate + "00000000";
            string _strDateEnd = _convertDate + "32000000";
            NUMBER_FORMAT_DTO ndto = new NUMBER_FORMAT_DTO();
            ndto._begin = Convert.ToInt64(_strDateBegin);
            ndto._end = Convert.ToInt64(_strDateEnd);
            return ndto;
        }

        
        public static List<TBL_INV_OZET> INV_TABLE()
        {
            OracleConnection orclConSrv = new OracleConnection("User Id=LOG724DB;Password=Orcl1881;Data Source=136.243.45.231/xe");

          
                orclConSrv.Open();
            

            List<TBL_INV_OZET> tblInv = new List<TBL_INV_OZET>();
            try
            {
                using (OracleCommand cmd = orclConSrv.CreateCommand())
                {
                    cmd.CommandText = "SELECT \"STATION_ID\",\"Inv_Id\" from TBL_INV_OZET WHERE STATION_ID=301 AND TARIH_NUMBER>=20190400000000 AND TARIH_NUMBER<=20190432000000 ";

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tblInv.Add(new TBL_INV_OZET()
                            {

                                // STATION_ID = reader["STATION_ID"] == DBNull.Value ? (int?)null : reader["STATION_ID"].ToInt32(),   
                                //Inv_Id = reader["Inv_Id"] == DBNull.Value ? (int?)null : reader["Inv_Id"].ToInt32()

                                STATION_ID = Convert.ToInt32(reader["STATION_ID"]),
                                Inv_Id = Convert.ToInt32(reader["Inv_Id"])
                            });
                        }
                    }
                }
                orclConSrv.Close();
            }
            catch (Exception ex)
            {
                return null;
            }
            return tblInv;

        }

        //public void GridExportToExcel()
        //{
        //    Thread.CurrentThread.CurrentCulture = new CultureInfo("tr-TR");
        //    DateTime datePicker = DateTime.Now;
        //    //string[] year_month = slctDate.Split('-');
        //    int _year = 2019;
        //    int _month = 04;
        //    datePicker = new DateTime(_year, _month, 1);

        //    NUMBER_FORMAT_DTO convertFormat = ConvertNumberFormatInvHeatMapMonthly(datePicker.ToString());
        //    long _numberDateBegin = convertFormat._begin;
        //    long _numberDateEnd = convertFormat._end;

        //    MontlyInverterProduction mi = new MontlyInverterProduction();

        //    var inverterDetail = DB.Inverters.Where(w => w.STATION_ID == 370 && w.IS_DELETED == false).ToList();

        //    var tblInv = inverterDetail.OrderBy(o => int.Parse(o.NAME.Split(' ')[1])).ToList();

        //    var listInvOzet = DB.InvSums.Where(ww => ww.STATION_ID == 370 && ww.TARIH_NUMBER >= _numberDateBegin && ww.TARIH_NUMBER <= _numberDateEnd && ww.Tarih.Value.Hour > 7) 
        //                            .GroupBy(grp => new { grp.Tarih.Value.Day, grp.Inv_Id })
        //                            .Select(s => new InvMonthlyValue { day = s.Key.Day, inv = s.Key.Inv_Id, energy = (float)Math.Round((s.Max(m => m.Gunluk_Enerji.Value)) / 1000, 2) })
        //                            .ToList();

        //    var MonthDaysCount = DateTime.DaysInMonth(_year, _month);
        //    List<InvMonthlyValue> orderByList = new List<InvMonthlyValue>();
        //    for (int i = 1; i <= MonthDaysCount; i++)
        //    {
        //        foreach (var inv in tblInv)
        //        {
        //            var reg = listInvOzet.Where(w => w.day == i && w.inv == inv.ID).FirstOrDefault();
        //            if (reg == null)
        //            {
        //                orderByList.Add(new InvMonthlyValue { day = i, inv = inv.ID, energy = null });
        //            }
        //            else
        //            {
        //                orderByList.Add(reg);
        //            }
        //        }
        //    }

        //    mi.listInvValue = orderByList.OrderBy(o => o.day).ThenBy(t => t.inv).ToList();

        //    var ggrp = mi.listInvValue.GroupBy(grp => grp.day).ToList();

  
        //    DataTable dt = new DataTable();
        //    dt.Columns.AddRange(new DataColumn[3] {
        //                new DataColumn("SiparişId"),
        //                new DataColumn("Ürün"),
        //                new DataColumn("Adet")});
        //    dt.Columns.Add(new DataColumn("www"));
        //    dt.Rows.Add(101, "Cam bardak", 5);
        //    dt.Rows.Add(102, "Pantolon", 2);
        //    dt.Rows.Add(103, "Tişört", 12);
        //    dt.Rows.Add(104, "Gömlek", 9);

        //    string dosyaAdi = "ornek_dosya_adi";
        //    var grid = new GridView();
        //    grid.DataSource = dt;
        //    grid.DataBind();

        //    Response.ClearContent();
        //    Response.Charset = "utf-8";
        //    Response.AddHeader("content-disposition", "attachment; filename=" + dosyaAdi + ".xls");

        //    Response.ContentType = "application/vnd.ms-excel";
        //    StringWriter sw = new StringWriter();
        //    HtmlTextWriter htw = new HtmlTextWriter(sw);

        //    grid.RenderControl(htw);

        //    Response.Write(sw.ToString());
        //    Response.End();
        //}

        public class INV_PROD_EXCEL_DTO
        {
            public DateTime DATE { get; set; }
            public string INVERTERNAME { get; set; }
            public decimal? PRODUCTION { get; set; }
        }

        [HttpGet]
        public FileContentResult ExportToExcel(int stationId, string slctDate)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("tr-TR");
            DateTime datePicker = DateTime.Now;
            string[] year_month = slctDate.Split('-');
            int _year = Convert.ToInt32(year_month[0]);
            int _month = Convert.ToInt32(year_month[1]);
            datePicker = new DateTime(_year, _month, 1);

            var stat = DB.Stations.Where(w => w.ID == stationId).FirstOrDefault();

            NUMBER_FORMAT_DTO convertFormat = ConvertNumberFormatInvHeatMapMonthly(datePicker.ToString());
            long _numberDateBegin = convertFormat._begin;
            long _numberDateEnd = convertFormat._end;

            MontlyInverterProduction mi = new MontlyInverterProduction();

            var inverterDetail = DB.Inverters.Where(w => w.STATION_ID == stationId && w.IS_DELETED == false).ToList();
            var tblInv = inverterDetail.OrderBy(o => int.Parse(o.NAME.Split(' ')[1])).ToList();

            var listInvOzet = DB.InvSums.Where(ww => ww.STATION_ID == stationId && ww.TARIH_NUMBER >= _numberDateBegin && ww.TARIH_NUMBER <= _numberDateEnd && ww.Tarih.Value.Hour > 7)
                                    .GroupBy(grp => new { grp.Tarih.Value.Day, grp.Inv_Id })
                                    .Select(s => new InvMonthlyValue { day = s.Key.Day, inv = s.Key.Inv_Id, energy = (float)Math.Round((s.Max(m => m.Gunluk_Enerji.Value)) / 1000, 2) })
                                    .ToList();

            var MonthDaysCount = DateTime.DaysInMonth(_year, _month);
            List<InvMonthlyValue> orderByList = new List<InvMonthlyValue>();
            for (int i = 1; i <= MonthDaysCount; i++)
            {
                foreach (var inv in tblInv)
                {
                    var reg = listInvOzet.Where(w => w.day == i && w.inv == inv.ID).FirstOrDefault();
                    if (reg == null)
                    {
                        orderByList.Add(new InvMonthlyValue { day = i, inv = inv.ID, energy = null, name=inv.NAME });
                    }
                    else
                    {
                        reg.name = inv.NAME;
                        orderByList.Add(reg);
                    }
                }
            }

            mi.listInvValue = orderByList.OrderBy(o => o.day).ThenBy(t => t.inv).ToList();


            List<INV_PROD_EXCEL_DTO> invProdList = mi.listInvValue.Select(s => new INV_PROD_EXCEL_DTO
            {
                DATE = new DateTime(_year, _month,s.day),
                INVERTERNAME = s.name,
                PRODUCTION = s.energy==null ? 0 : Math.Round((decimal)s.energy,2)
            }).ToList();

            string fileName = stat.NAME.ToUpper() + " INVERTER_PRODUCTION_MONTHLY.xlsx";
            string[] columns = { "DATE", "INVERTERNAME", "PRODUCTION" };
            byte[] filecontent = ExcelExportHelper.ExportExcel(invProdList, stat.NAME, true, columns);
            return File(filecontent, ExcelExportHelper.ExcelContentType, fileName);
        }


        public JsonResult HeatMapInvProductionData(int stationId,string slctDate)
        {
            InverterProductionView mdl = new InverterProductionView();
            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("tr-TR");
                DateTime datePicker = DateTime.Now;
                string[] year_month = slctDate.Split('-');
                int _year = Convert.ToInt32(year_month[0]);
                int _month = Convert.ToInt32(year_month[1]);
                datePicker = new DateTime(_year, _month, 1);

                NUMBER_FORMAT_DTO convertFormat = ConvertNumberFormatInvHeatMapMonthly(datePicker.ToString());
                long _numberDateBegin = convertFormat._begin;
                long _numberDateEnd = convertFormat._end;

                MontlyInverterProduction mi = new MontlyInverterProduction();

                var inverterDetail = DB.Inverters.Where(w => w.STATION_ID == stationId && w.IS_DELETED == false).ToList();

                var tblInv = inverterDetail.OrderBy(o => int.Parse(o.NAME.Split(' ')[1])).ToList();

                //var adoInv = DB.Database.SqlQuery<TBL_INV_OZET>("select * from TBL_INV_OZET WHERE STATION_ID=370 AND TARIH_NUMBER>='20190400000000' AND TARIH_NUMBER<='20190432000000'").ToList();

                //var tblInvTable = DB.InvSums.Where(w => w.STATION_ID == stationId && w.TARIH_NUMBER >= _numberDateBegin && w.TARIH_NUMBER <= _numberDateEnd && (Math.Round(((decimal)w.TARIH_NUMBER.Value%1000000)/10000))>7).ToList();

                var listInvOzet = DB.InvSums.Where(ww => ww.STATION_ID == stationId && ww.TARIH_NUMBER >= _numberDateBegin && ww.TARIH_NUMBER <= _numberDateEnd && ww.Tarih.Value.Hour>7) // int.Parse(ww.TARIH_NUMBER.ToString().Substring(8, 2)) > 07 //(Math.Round(((decimal)ww.TARIH_NUMBER.Value % 1000000) / 10000)) > 7
                                        .GroupBy(grp => new { grp.Tarih.Value.Day, grp.Inv_Id })
                                        .Select(s => new InvMonthlyValue { day = s.Key.Day, inv = s.Key.Inv_Id, energy = (float)Math.Round((s.Max(m => m.Gunluk_Enerji.Value)) / 1000, 2) })
                                        .ToList();

                //mi.listInvValue = DB.InvSums.Join(DB.Inverters.Where(ww => ww.STATION_ID == stationId),
                //    invs => invs.Inv_Id, inv => inv.ID, (inv, invs) => inv)
                //    .Where(w => w.STATION_ID == stationId && w.TARIH_NUMBER >= 20190400000000 && w.TARIH_NUMBER <= 20190432000000)
                //                        .GroupBy(grp => new { grp.Tarih.Value.Day, grp.Inv_Id })
                //                        .Select(s => new InvMonthlyValue { day = s.Key.Day, inv = s.Key.Inv_Id, energy = (float)Math.Round((s.Max(m => m.Gunluk_Enerji.Value)) / 1000, 1) })
                //                        .OrderBy(o => o.day).ThenBy(t => t.inv).ToList();

                var MonthDaysCount = DateTime.DaysInMonth(_year, _month);
                List<InvMonthlyValue> orderByList = new List<InvMonthlyValue>();
                for (int i = 1; i <= MonthDaysCount; i++)
                {
                    foreach (var inv in tblInv)
                    {
                        var reg = listInvOzet.Where(w => w.day == i && w.inv == inv.ID).FirstOrDefault();
                        if (reg == null)
                        {
                            orderByList.Add(new InvMonthlyValue { day = i, inv = inv.ID, energy = null });
                        }
                        else
                        {
                            orderByList.Add(reg);
                        }
                    }
                }
                
                mi.listInvValue = orderByList.OrderBy(o => o.day).ThenBy(t => t.inv).ToList();
       
                var groupDay = mi.listInvValue.GroupBy(grp => grp.day).ToList();
                foreach (var itemDay in groupDay)
                {
                    mi.listDay.Add(itemDay.Key);
                }
                foreach (var invName in tblInv)
                {
                    mi.listInvName.Add(invName.NAME);
                }
                mdl.invModel = mi;
                mdl.ErrorMessage = "";
            }
            catch (Exception ex)
            {
                mdl.ErrorMessage = ex.ToString();
            }

            return Json(mdl);
        }

        public NUMBER_FORMAT_DTO ConvertNumberFormatMonthly(string date)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("tr-TR");
            DateTime selectDate = DateTime.Parse(date);
            string _convertDate = "";
            string _year = selectDate.Year.ToString();
            string _month = selectDate.Month.ToString();
            string _day = selectDate.Day.ToString();
            if (_month.Length < 2)
            {
                _month = "0" + selectDate.Month.ToString();
            }
            if (_day.Length < 2)
            {
                _day = "0" + selectDate.Day.ToString();
            }
            _convertDate = _year + _month + _day;
            string _strDateBegin = _convertDate + "000000";
            string _strDateEnd = _convertDate + "235959";
            NUMBER_FORMAT_DTO ndto = new NUMBER_FORMAT_DTO();
            ndto._begin = Convert.ToInt64(_strDateBegin);
            ndto._end = Convert.ToInt64(_strDateEnd);
            return ndto;
        }


        public JsonResult GetChartInverterDetail(string beginDate, int stationId)
        {
            try
            {
                NUMBER_FORMAT_DTO convertFormat = ConvertNumberFormat(beginDate);
                long _numberDateBegin = convertFormat._begin;
                long _numberDateEnd = convertFormat._end;
                Thread.CurrentThread.CurrentCulture = new CultureInfo("tr-TR");
                DateTime selectDate = DateTime.Parse(@beginDate);

                int[] ib = DB.Inverters.Where(a => a.STATION_ID == stationId).Select(n => n.ID).ToArray();
                var _count = ib.Count();
                InvlistDTO invD = new InvlistDTO();
                invD.inverters = (from p in DB.InvSums
                                  where p.STATION_ID == stationId && ib.Contains(p.Inv_Id)
                                  && p.TARIH_NUMBER >= _numberDateBegin && p.TARIH_NUMBER <= _numberDateEnd
                                  orderby p.Tarih ascending
                                  select new InverterDetailDTO
                                  {
                                      STATION_ID = p.STATION_ID,
                                      Inv_Id = p.Inv_Id,
                                      Tarih = p.Tarih.Value,
                                      Guc_AC = p.Guc_AC.Value,
                                      Guc_DC = p.Guc_DC.Value,
                                      Akim_AC = p.Akim_AC.Value,
                                      Akim_DC = p.Akim_DC.Value,
                                      Gerilim_AC = p.Gerilim_AC.Value,
                                      Gerilim_DC = p.Gerilim_DC.Value,
                                      InverterProduction = p.Gunluk_Enerji.Value
                                  }
                          ).ToList();
                //var timeLine= invD.inverters.GroupBy(grp=>grp.Tarih.ToString("dd/MM/yyyy H:mm")).ToList();
                return Json(invD);
            }
            catch (Exception ex)
            {
                return Json("{ err:" + ex.Message + "}", JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ExceleAktarInvDataTable()
        {
            return View(Session["InvDataTable"] as List<Inv_Production_DTO>);
        }
        public JsonResult GetInvProduction(string beginDate, int stationId)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("tr-TR");
            DateTime nowDate = DateTime.Parse(@beginDate);
            NUMBER_FORMAT_DTO convertFormat = ConvertNumberFormat(beginDate);
            long _numberDateBegin = convertFormat._begin;
            long _numberDateEnd = convertFormat._end;
            List<TBL_INVERTER> invertors = DB.Inverters.Where(a => a.STATION_ID == stationId && a.IS_DELETED == false).ToList();
            //int[] lastIds = (from o in DB.InvSums
            //                 where o.STATION_ID == stationId
            //                 && o.TARIH_NUMBER >= _numberDateBegin && o.TARIH_NUMBER <= _numberDateEnd
            //                       && o.Gunluk_Enerji > 0
            //                       && o.Tarih.Value.Hour > 7
            //                 group o by o.Inv_Id
            //                      into g 
            //                 select g.Max(p => p.Id)).ToArray<int>();

            var InvData = (from o in DB.InvSums
                             where o.STATION_ID == stationId
                             && o.TARIH_NUMBER >= _numberDateBegin && o.TARIH_NUMBER <= _numberDateEnd
                                   && o.Tarih.Value.Hour > 7
                             select o).ToList();
            var InvGroupList = InvData.GroupBy(grp => grp.Inv_Id).ToList();

            int[] idsList = new int[invertors.Count];
            int z = 0;
            foreach (var item in InvGroupList)
            {
                int maxValue = (from t1 in item
                                where t1.Gunluk_Enerji == (item.Max(c2 => c2.Gunluk_Enerji))
                                select t1.Id).FirstOrDefault();

                idsList[z] = maxValue;
                z++;
            }

            List<Inv_Production_DTO> InvTbloztA = (from i in DB.InvSums
                                                   join inv in DB.Inverters on i.Inv_Id equals inv.ID
                                                   where i.TARIH_NUMBER >= _numberDateBegin && i.TARIH_NUMBER <= _numberDateEnd
                                                   && idsList.Contains(i.Id)
                                                   select new Inv_Production_DTO
                                                   {
                                                       invId = i.Inv_Id,
                                                       date = i.Tarih.Value,
                                                       inverter_Name = inv.NAME,
                                                       stationId = i.STATION_ID,
                                                       dailyProduction = (float)Math.Round((double)i.Gunluk_Enerji.Value / 1000, 2),
                                                       specificYield = inv.INV_DC_GUC == null || inv.INV_DC_GUC.Value == 0 ? 0 : (float)Math.Round((double)i.Gunluk_Enerji.Value / 1000 / inv.INV_DC_GUC.Value, 3),
                                                       acPower = i.Guc_AC.Value,
                                                       dcPower = i.Guc_DC.Value,
                                                       acCurrent = i.Akim_AC.Value,
                                                       irradiation = i.Isinim == null ? 0 : i.Isinim.Value,
                                                       dcVoltage = i.Gerilim_DC.Value,
                                                       totaPanelPower = inv.INV_DC_GUC == null || inv.INV_DC_GUC.Value == 0 ? 0 : inv.INV_DC_GUC.Value
                                                   }).ToList();

            

            foreach (var item in invertors)
            {
                if (InvTbloztA.Where(a => a.invId == item.ID).Count() == 0)
                {
                    InvTbloztA.Add(new Inv_Production_DTO
                    {
                        invId = item.ID,
                        date = DateTime.Now,
                        inverter_Name = item.NAME,
                        stationId = item.STATION_ID,
                        dailyProduction = 0,
                        specificYield = 0,
                        acPower = 0,
                        dcPower = 0,
                        acCurrent = 0,
                        irradiation = 0,
                        dcVoltage = 0,
                        totaPanelPower = 0
                    });
                }
            }


            InvTbloztA = InvTbloztA.OrderBy(x => int.Parse(x.inverter_Name.Split(' ')[1])).ToList();
            Session["InvDataTable"] = InvTbloztA;
            return Json(InvTbloztA, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetInvState(string invId)
        {
            DateTime nowDate = DateTime.Now;
            Inv_States_DTO invModel = new Inv_States_DTO();
            int invIdNo = Convert.ToInt32(invId);
            var st = (from u in DB.InvSums
                      where u.Inv_Id == invIdNo
                      orderby u.Tarih descending
                      select new Inv_States_DTO
                      {
                          _tarih = u.Tarih.Value,
                          _invId = u.Inv_Id,
                          _globalState = u.Global_State.Value,
                          _alarmState = u.Alarm_State.Value,
                          _DcAcConverterState = u.DC_AC_Conv_State.Value,
                          _DcDcConverterState = u.DC_DC_Conv_State.Value,
                          _deratingState = u.Derating_State.Value,
                          _HEARTBEAT = u.HEARTBEAT.Value,
                          _INVERTER_MAIN_STATUS = u.INVERTER_MAIN_STATUS.Value,
                          _REACTIVE_POWER = u.REACTIVE_POWER.Value,
                          _GRID_VOLTAGE_VRMS = u.GRID_VOLTAGE_VRMS.Value,
                          _GRID_FREQUENCY = u.GRID_FREQUENCY.Value,
                          _POWER_FACTOR = u.POWER_FACTOR.Value,
                          _CODE_OF_THE_ACTIVE_FAULT = u.CODE_OF_THE_ACTIVE_FAULT.Value,
                          _GRID_CURRENT = u.GRID_CURRENT.Value,
                          _DC_BUS_VOLTAGE = u.DC_BUS_VOLTAGE.Value,
                          _GROUNDING_CURRENT = u.GROUNDING_CURRENT.Value,
                          _SOLATION_RESISTANCE = u.SOLATION_RESISTANCE.Value,
                          _AMBIENT_TEMPERATURE = u.AMBIENT_TEMPERATURE.Value,
                          _HIGHEST_IGBT_TEMPERATURE_PU1 = u.HIGHEST_IGBT_TEMPERATURE_PU1.Value,
                          _HIGHEST_IGBT_TEMPERATURE_PU2 = u.HIGHEST_IGBT_TEMPERATURE_PU2.Value,
                          _HIGHEST_IGBT_TEMPERATURE_PU3 = u.HIGHEST_IGBT_TEMPERATURE_PU3.Value,
                          _HIGHEST_IGBT_TEMPERATURE_PU4 = u.HIGHEST_IGBT_TEMPERATURE_PU4.Value,
                          _CONTROL_SECTION_TEMPERATURE = u.CONTROL_SECTION_TEMPERATURE.Value,
                          _DAILY_KVAH_SUPPLIED = u.DAILY_KVAH_SUPPLIED.Value,
                          _TOTAL_KVAH_SUPPLIED = u.TOTAL_KVAH_SUPPLIED.Value,
                          _IGBT_1_T1 = u.IGBT_1_T1.Value,
                          _IGBT_1_T2 = u.IGBT_1_T2.Value,
                          _IGBT_1_T3 = u.IGBT_1_T3.Value,
                          _IGBT_2_T1 = u.IGBT_2_T1.Value,
                          _IGBT_2_T2 = u.IGBT_2_T2.Value,
                          _IGBT_2_T3 = u.IGBT_2_T3.Value,
                          _IGBT_3_T1 = u.IGBT_3_T1.Value,
                          _IGBT_3_T2 = u.IGBT_3_T2.Value,
                          _IGBT_3_T3 = u.IGBT_3_T3.Value,
                          _IGBT_4_T1 = u.IGBT_4_T1.Value,
                          _IGBT_4_T2 = u.IGBT_4_T2.Value,
                          _IGBT_4_T3 = u.IGBT_4_T3.Value
                      }).FirstOrDefault();
            return Json(st, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetInverterCount(int stationId)
        {
            var invs = DB.Inverters.Where(a => a.IS_DELETED == false && a.STATION_ID == stationId).OrderBy(a => a.ID).ToList();
            return Json(invs, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetOnlyInv(string invId, int stationId, string beginDate)
        {
            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("tr-TR");
                DateTime nowDate = DateTime.Parse(@beginDate);
                NUMBER_FORMAT_DTO convertFormat = ConvertNumberFormat(beginDate);
                long _numberDateBegin = convertFormat._begin;
                long _numberDateEnd = convertFormat._end;

                int id = Convert.ToInt32(invId);
                var invOnly = (from i in DB.InvSums
                               where i.Inv_Id == id
                               && i.STATION_ID == stationId
                               && i.TARIH_NUMBER >= _numberDateBegin && i.TARIH_NUMBER <= _numberDateEnd
                               orderby i.Tarih ascending
                               select new InverterDetailDTO
                               {
                                   STATION_ID = i.STATION_ID,
                                   Inv_Id = i.Inv_Id,
                                   Tarih = i.Tarih.Value,
                                   Guc_AC = i.Guc_AC.Value,
                                   Guc_DC = i.Guc_DC.Value,
                                   Akim_AC = i.Akim_AC.Value,
                                   Akim_DC = i.Akim_DC.Value,
                                   Gerilim_AC = i.Gerilim_AC.Value,
                                   Gerilim_DC = i.Gerilim_DC.Value,
                                   InverterProduction = i.Gunluk_Enerji.Value
                               }).ToList();
                return Json(invOnly);
            }
            catch (Exception ex)

            {
                return Json("{ err:" + ex.Message + "}", JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult InvDetailReport(int stationId)
        {
            return View(stationId);
        }

        public ActionResult InvDetailReportPartial(int stationId/*, string date*/, string date1, string date2)
        {
            ViewData["date1"] = date1;
            ViewData["date2"] = date2;

            return PartialView(stationId);
        }

        public class ExportEsso
        {
            public int Id { get; set; }
            public string DATE { get; set; }
            public string INV { get; set; }
            public decimal CRNT_AC { get; set; }
            public decimal CRNT_DC { get; set; }
            public decimal V_AC { get; set; }
            public decimal V_DC { get; set; }
            public decimal PWR_AC { get; set; }
            public decimal PWR_DC { get; set; }
            public decimal DAILY_YIELD { get; set; }
            public decimal TOTAL_YIELD { get; set; }
        }


        public ActionResult ExportTo(GridViewExportFormat? exportFormat, int stationId,/*DateTime datePck,*/ DateTime dFrom, DateTime dTo/*, string filterString*/)
        {


            string _stationName = (from tt in DB.Stations
                                   where tt.ID == stationId && tt.IS_DELETED == false
                                   select new { tt.NAME }).FirstOrDefault().NAME;

            if (exportFormat == null || !GridViewExportHelper.ExportFormatsInfo.ContainsKey(exportFormat.Value))
                return RedirectToAction("InvDetailReportPartial", stationId);

            // DateTime dts = Convert.ToDateTime(datePck);
            DateTime date1 = Convert.ToDateTime(dFrom);
            DateTime date2 = Convert.ToDateTime(dTo);
            DateTime date1_ = date1.Date;
            DateTime date2_ = date2.Date;
            date2_ = date2_.AddHours(23);
            date2_ = date2_.AddMinutes(59);
            GridHelper gh = new GridHelper(stationId);



            var _TempInvOzet = (from x in DB.InvSums
                                join inv in DB.Inverters on x.Inv_Id equals inv.ID
                                where
                                x.STATION_ID == stationId
                                &&
                                date1_ <= x.Tarih.Value
                                &&
                                date2_ >= x.Tarih.Value
                                //&&
                                //date2.Month >= x.Tarih.Value.Month
                                //&&
                                //date1.Month <= x.Tarih.Value.Month
                                //&&
                                //date2.Day >= x.Tarih.Value.Day
                                //&&
                                //date1.Day <= x.Tarih.Value.Day
                                select new
                                {
                                    x,
                                    inv.NAME
                                }).OrderBy(X => X.x.Tarih.Value).ToList();

            List<ExportEsso> ListExport = new List<ExportEsso>();

            for (int i = 0; i < _TempInvOzet.Count; i++)
            {
                ExportEsso _cExportEsso = new ExportEsso();

                _cExportEsso.Id = _TempInvOzet[i].x.Id;
                _cExportEsso.DATE = _TempInvOzet[i].x.Tarih.Value.ToString("dd/MM/yyyy HH:mm");
                _cExportEsso.INV = _TempInvOzet[i].NAME;
                _cExportEsso.CRNT_AC = Convert.ToDecimal(_TempInvOzet[i].x.Akim_AC);
                _cExportEsso.CRNT_DC = Convert.ToDecimal(_TempInvOzet[i].x.Akim_DC);
                _cExportEsso.V_AC = Convert.ToDecimal(_TempInvOzet[i].x.Gerilim_AC);
                _cExportEsso.V_DC = Convert.ToDecimal(_TempInvOzet[i].x.Gerilim_DC);
                _cExportEsso.PWR_AC = Convert.ToDecimal(_TempInvOzet[i].x.Guc_AC);
                _cExportEsso.PWR_DC = Convert.ToDecimal(_TempInvOzet[i].x.Guc_DC);
                _cExportEsso.DAILY_YIELD = Convert.ToDecimal(_TempInvOzet[i].x.Gunluk_Enerji);
                _cExportEsso.TOTAL_YIELD = Convert.ToDecimal(_TempInvOzet[i].x.Toplam_Enerji);

                ListExport.Add(_cExportEsso);
            }

            if (exportFormat == GridViewExportFormat.Xls)
            {
                XlsExportOptionsEx exportOptions = new XlsExportOptionsEx();
                exportOptions.CustomizeCell += new DevExpress.Export.CustomizeCellEventHandler(exportOptions_CustomizeCell);

                string filename = _stationName + " (" + date1.ToShortDateString() + " - " + date2.ToShortDateString() + ")";

                return GridViewExtension.ExportToXls(gh.GridStationSettings, ListExport, filename, exportOptions);
            }
            else
            {
                return GridViewExportHelper.ExportFormatsInfo[exportFormat.Value](
                gh.GridStationSettings, ListExport);
            }
        }

        void exportOptions_CustomizeCell(DevExpress.Export.CustomizeCellEventArgs ea)
        {
            if (ea.AreaType == DevExpress.Export.SheetAreaType.Header)
            {

                ea.Formatting.BackColor = System.Drawing.Color.FromArgb(1, 0, 98, 158);
                ea.Formatting.Font.Color = System.Drawing.Color.WhiteSmoke;
                ea.Handled = true;
            }

        }
        #region GridSettings
        public class GridHelper
        {
            static int _stationId;

            public GridHelper(int stationId)
            {
                _stationId = stationId;
            }

            static GridViewSettings gridStationSettings;
            public GridViewSettings GridStationSettings
            {
                get
                {
                    //if (gridStationSettings == null)
                    return CreateExcelDataAwareExportGridViewSettings();
                    //return gridStationSettings;
                }
            }
            GridViewSettings CreateExcelDataAwareExportGridViewSettings()
            {
                GridViewSettings settings = new GridViewSettings();
                settings.Name = "grdInvDetail";
                settings.CallbackRouteValues = new { Controller = "InverterPages", Action = "InvDetailReportPartial", stationId = _stationId };
                settings.KeyFieldName = "Id";

                settings.SettingsExport.BeforeExport = (s, e) =>
                {
                    (s as MVCxGridView).Columns["Id"].Visible = false;

                };
                settings.SettingsExport.RenderBrick = (sender, e) =>
                {
                    if (e.RowType == GridViewRowType.Data && e.VisibleIndex % 2 == 0)
                        e.BrickStyle.BackColor = System.Drawing.Color.FromArgb(237, 237, 237);

                };
                settings.SettingsPager.PageSize = 30;

                settings.SettingsBehavior.AllowFocusedRow = true;
                settings.Width = Unit.Percentage(100);
                return settings;
            }

        }
        #endregion

        public ActionResult HeatMapInverter(int stationId)
        {
            return View(stationId);
        }
        public ActionResult HeatMapInverter2(int stationId)
        {
            return View(stationId);
        }

    }
}