using Esso.Data;
using Esso.Model.Models;
using Esso.Models;
using Esso.Web.Helpers;
using Esso.Web.Models.DashboardModel;
using Esso.Web.Models.DATE_NUMBER;
using Esso.Web.ViewModels;
using OfficeOpenXml.Table.PivotTable;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using static Esso.Web.Controllers.LicensedController;

namespace Esso.Web.Controllers
{
    public class DashboardInverterController : BaseController
    {
        // GET: DashboardInverter
        EssoEntities DB = new EssoEntities();

        public JsonResult GetInvProduction(string beginDate, int stationId)
        {
            CultureHelper.SetCultureInfo();
            var startDate = DateTimeHelper.BeginDate(beginDate);
            var endDate = DateTimeHelper.EndDate(beginDate);


            var sql = "select * from \"TBL_INV_OZET\" where \"STATION_ID\" = " + stationId + " AND \"Tarih\" between TO_DATE('" + startDate + "','DD.MM.YY hh24:mi:ss') and to_date('" + endDate + "','DD.MM.YY hh24:mi:ss')";

            List<TBL_INVERTER> invertors = DB.Inverters.Where(a => a.STATION_ID == stationId && a.IS_DELETED == false).ToList();
            int[] idsList = new int[invertors.Count];

            //var InvData = DB.Database.SqlQuery<TBL_INV_OZET>(sql)
            //                 .ToList();

            //var sql2 = "select * from \"TBL_INVERTER\" where \"STATION_ID\" = " + stationId;
            //var usr = DB.Database.SqlQuery<TBL_INVERTER>(sql2)
            //            .Select(a =>  
            //            new TBL_INVERTER
            //            {
            //                ID = a.ID,
            //                NAME = a.NAME

            //            }).ToList();


            //var aaa = GetDataTable(null, " SELECT * FROM LOG724DB.TBL_INV_OZET T WHERE \"STATION_ID\" = 990 ')");

            //DataTable _dtStation = _Sql.GetStationTable();

            //var InvData = (from o in DB.InvSums
            //               where o.STATION_ID == stationId
            //                      //&& o.TARIH_NUMBER >= _numberDateBegin && o.TARIH_NUMBER <= _numberDateEnd
            //                      //&& o.Tarih.Value.Hour > 7
            //                      && o.Tarih == startDate //&& o.Tarih < endDate //&& o.Tarih.Value.Hour > 7
            //               select o).ToList();

            var InvData = (from o in DB.InvSums
                           where o.STATION_ID == stationId
                                  && o.Tarih >= startDate && o.Tarih <= endDate
                           select o).ToList();

            var InvGroupList = InvData.Where(w=>w.Tarih.Value.Hour > 5).GroupBy(grp => grp.Inv_Id).ToList();


            int z = 0;
            foreach (var item in InvGroupList)
            {
                int maxValue = (from t1 in item
                                where t1.Gunluk_Enerji == (item.Max(c2 => c2.Gunluk_Enerji))
                                select t1.Id).FirstOrDefault();

                idsList[z] = maxValue;
                z++;
            }


                          List <Inv_Production_DTO> InvTbloztA = (from i in InvData
                                                   join inv in invertors on i.Inv_Id equals inv.ID
                                                   where idsList.Contains(i.Id)
                                                   select new Inv_Production_DTO
                                                   {
                                                       invId = i.Inv_Id,
                                                       date = i.Tarih.Value,
                                                       inverter_Name = inv.NAME,
                                                       stationId = i.STATION_ID,
                                                       dailyProduction = i.Gunluk_Enerji == null ? 0 : (float)Math.Round((double)i.Gunluk_Enerji.Value / 1000, 2),
                                                       specificYield = inv.INV_DC_GUC == null || inv.INV_DC_GUC.Value == 0 ? 0 : (float)Math.Round((double)i.Gunluk_Enerji.Value / 1000 / inv.INV_DC_GUC.Value, 3),
                                                       acPower = i.Guc_AC == null ? 0 : (float)Math.Round(i.Guc_AC.Value, 1),
                                                       dcPower = i.Guc_DC == null ? 0 : (float)Math.Round(i.Guc_DC.Value, 1),
                                                       acCurrent = i.Akim_AC == null ? 0 : (float)Math.Round(i.Akim_AC.Value, 1),
                                                       dcCurrent = i.Akim_DC == null ? 0 : (float)Math.Round(i.Akim_DC.Value, 1),
                                                       acVoltage = i.Gerilim_AC == null ? 0 : (float)Math.Round(i.Gerilim_AC.Value, 1),
                                                       dcVoltage = i.Gerilim_DC == null ? 0 : (float)Math.Round(i.Gerilim_DC.Value, 1),
                                                       totaPanelPower = inv.INV_DC_GUC == null ? 0 : inv.INV_DC_GUC.Value
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

            return Json(InvTbloztA, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetDailyHeatMap(int stationId, string beginDate)
        {
            InverterPerformanceView mdl = new InverterPerformanceView();
            try
            {
                CultureHelper.SetCultureInfo();
                var startDate = DateTimeHelper.BeginDate(beginDate);
                var endDate = DateTimeHelper.EndDate(beginDate);

                mdl.invModel = new InverterModel();

                var sql = "select * from \"TBL_INV_OZET\" where \"STATION_ID\" = " + stationId + " AND \"Tarih\" between TO_DATE('" + startDate + "','DD.MM.YY hh24:mi:ss') and to_date('" + endDate + "','DD.MM.YY hh24:mi:ss')";



                var InvData = DB.Database.SqlQuery<TBL_INV_OZET>(sql)
                                 .ToList();

                var hourlyProduction = (from InverterDetail in InvData
                                        join Inverter in DB.Inverters on
                                        InverterDetail.Inv_Id equals Inverter.ID
                                        where
                                        InverterDetail.STATION_ID == stationId
                                         && InverterDetail.Tarih >= startDate && InverterDetail.Tarih <= endDate
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

            return Json(mdl, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetInvDetail(string beginDate, int stationId)
        {
            try
            {
                CultureHelper.SetCultureInfo();
                var startDate = DateTimeHelper.BeginDate(beginDate);
                var endDate = DateTimeHelper.EndDate(beginDate);

                var invList = DB.Inverters.Where(a => a.STATION_ID == stationId).ToList();
                var invs = invList.OrderBy(o => int.Parse(o.NAME.Split(' ')[1])).ToList();

                List<InvDetail_Main_DTO> mainInvDetailList = new List<InvDetail_Main_DTO>();

                var sql = "select * from \"TBL_INV_OZET\" where \"STATION_ID\" = " + stationId + " AND \"Tarih\" between TO_DATE('" + startDate + "','DD.MM.YY hh24:mi:ss') and to_date('" + endDate + "','DD.MM.YY hh24:mi:ss')";



                var InvData = DB.Database.SqlQuery<TBL_INV_OZET>(sql)
                                 .ToList();

                var dataList = InvData.OrderBy(o=>o.Tarih.Value).Select(p => new InvDetail_DTO
                {
                    stationId = p.STATION_ID,
                    invId = p.Inv_Id,
                    date = p.Tarih.Value,
                    powerAC = p.Guc_AC.Value,
                    powerDC = p.Guc_DC.Value,
                    currentAC = p.Akim_AC.Value,
                    currentDC = p.Akim_DC.Value,
                    voltageAC = p.Gerilim_AC.Value,
                    voltageDC = p.Gerilim_DC.Value,
                    energy = p.Gunluk_Enerji.Value
                }).ToList();

                var dataGrpList = dataList.GroupBy(grp => grp.invId)
                    .Select(s => new InvDetail_Main_DTO { invId = s.Key, dataList = s.Where(w => w.invId == s.Key).OrderBy(o => o.date).ToList() }).ToList();


                foreach (var item in invs)
                {
                    mainInvDetailList.Add(new InvDetail_Main_DTO { invId = item.ID, invName = item.NAME, dataList = dataList.Where(w => w.invId == item.ID).ToList() });
                }


                return Json(mainInvDetailList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("{ err:" + ex.Message + "}", JsonRequestBehavior.AllowGet);
            }
        }

    
        public JsonResult GetInvString(int stationId, string beginDate, string invNo)
        {
            StringPerformanceView _strModel = new StringPerformanceView(); 
            try
            {
                CultureHelper.SetCultureInfo();
                DateTime date = DateTime.Parse(beginDate);
                DateTime curDate = DateTime.Now;
                _strModel.strModel = new StringModel();
                List<String_Hour_DTO> values = new List<String_Hour_DTO>();

                int _invNo = int.Parse(invNo);
                string invStrName = "DCB" + _invNo;
                if (curDate.Date.Year == date.Date.Year && curDate.Date.Month == date.Date.Month)
                {
                    NUMBER_FORMAT_DTO convertFormat = DateTimeHelper.ConvertNumberFormatMinute(beginDate);
                    long _numberDateBegin = convertFormat._begin;
                    long _numberDateEnd = convertFormat._end;
                    values = (from u in DB.StringOzetQuarterHourAVG
                              join v in DB.stationString on u.STRING_ID equals v.STRING_ID
                              where v.IS_DELETED == false
                              && v.STATION_ID == stationId
                              && u.STATION_ID == stationId
                              && _numberDateBegin <= u.TARIH_NUMBER && _numberDateEnd >= u.TARIH_NUMBER
                              && v.DISPLAY_NAME.StartsWith(invStrName)
                              orderby u.TARIH_NUMBER ascending
                              select new String_Hour_DTO
                              {
                                  NAME = v.DISPLAY_NAME,
                                  ID = u.STRING_ID,
                                  TARIH_NUMBER = u.TARIH_NUMBER.Value,
                                  VALUE = u.VALUE
                              }).ToList();

                    var stringNameList = values.OrderBy(a => a.NAME)
                           .Select(a => a.NAME)
                           .Distinct()
                           .ToList();

                    foreach (var item in stringNameList)
                    {
                        _strModel.strModel.StringList.Add(item.Substring(8, 7).Replace("_", ""));
                    }

                    var valueList = new List<Values>();

                    string[] groupTime = values.GroupBy(grp => grp.TARIH_NUMBER).OrderBy(a => a.Key).Select(a => a.Key.ToString().Substring(8, 4)).ToArray();
                    int endHour = int.Parse(groupTime[groupTime.Length - 1]);
                    DateTime EndDate = DateTime.Now.Date;
                    TimeSpan tEndDate = new TimeSpan((int.Parse(groupTime[groupTime.Length - 1].Substring(0, 2))), (int.Parse(groupTime[groupTime.Length - 1].Substring(2, 2))), 0);
                    EndDate = EndDate.Date + tEndDate;

                    DateTime LastDate = EndDate;
                    TimeSpan tLastDate = new TimeSpan(21, 0, 0);
                    LastDate = LastDate.Date + tLastDate;

                    for (int i = 0; i < groupTime.Length; i++)
                    {
                        _strModel.strModel.Hours.Add(groupTime[i]);
                    }

                    while (EndDate < LastDate)
                    {
                        EndDate = EndDate.AddMinutes(15);
                        _strModel.strModel.Hours.Add(DateTimeHelper.HourMinuteFormat(EndDate));
                    }

                    var groupList = values.OrderBy(a => a.NAME).GroupBy(a => a.NAME).ToList();
                    foreach (var item in groupList)
                    {
                        var listvalue = new Values();
                        foreach (var i in item.GroupBy(grp => grp.TARIH_NUMBER).OrderBy(a => a.Key).ToList())
                        {
                            listvalue.values.Add((float)Math.Round(i.Max(a => a.VALUE), 2));
                        }
                        valueList.Add(listvalue);
                    }

                    _strModel.strModel.series = valueList.ToList();
                    _strModel.ErrorMessage = "";
                }
                else
                {
                    NUMBER_FORMAT_DTO convertFormat = DateTimeHelper.ConvertNumberFormatHour(beginDate);
                    long _numberDateBegin = convertFormat._begin;
                    long _numberDateEnd = convertFormat._end;
                    values = (from u in DB.StringOzetAVG
                              join v in DB.stationString on u.STRING_ID equals v.STRING_ID
                              where v.IS_DELETED == false
                              && v.STATION_ID == stationId
                              && u.STATION_ID == stationId
                              && _numberDateBegin < u.TARIH_NUMBER && _numberDateEnd > u.TARIH_NUMBER
                              && v.DISPLAY_NAME.StartsWith(invStrName)
                              orderby u.TARIH_NUMBER ascending
                              select new String_Hour_DTO
                              {
                                  NAME = v.DISPLAY_NAME,
                                  ID = u.STRING_ID,
                                  TARIH_NUMBER = u.TARIH_NUMBER.Value,
                                  VALUE = u.VALUE
                              }).ToList();
                    var stringNameList = values.OrderBy(a => a.NAME)
                           .Select(a => a.NAME)
                           .Distinct()
                           .ToList();

                    foreach (var item in stringNameList)
                    {
                        _strModel.strModel.StringList.Add(item.Substring(8, 7).Replace("_", ""));
                    }

                    var valueList = new List<Values>();

                    string[] groupTime = values.GroupBy(grp => grp.TARIH_NUMBER).OrderBy(a => a.Key).Select(a => a.Key.ToString().Substring(8, 2)).ToArray();
                    int endHour = int.Parse(groupTime[groupTime.Length - 1]);
                    for (int i = 0; i < groupTime.Length; i++)
                    {
                        _strModel.strModel.Hours.Add(groupTime[i] + "00");
                    }

                    for (int i = endHour + 1; i < 21; i++)
                    {
                        if (!groupTime.Contains(i.ToString()))
                        {
                            _strModel.strModel.Hours.Add(i.ToString() + "00");
                        }
                    }

                    var groupList = values.OrderBy(a => a.NAME).GroupBy(a => a.NAME).ToList();
                    foreach (var item in groupList)
                    {
                        var listvalue = new Values();
                        foreach (var i in item.GroupBy(grp => grp.TARIH_NUMBER).OrderBy(a => a.Key).ToList())
                        {
                            listvalue.values.Add((float)Math.Round(i.Max(a => a.VALUE), 2));
                        }
                        valueList.Add(listvalue);
                    }

                    _strModel.strModel.series = valueList.ToList();
                    _strModel.ErrorMessage = "";
                }

            }
            catch (Exception ex)
            {
                _strModel.ErrorMessage = ex.ToString();
            }

            return Json(_strModel, JsonRequestBehavior.AllowGet);

        }
    }
}