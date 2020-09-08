using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Esso.Models;
using Esso.Data;
using System.Threading;
using System.Globalization;

namespace Esso.Web.Controllers
{
    [Authorize]
    public class ReportController : BaseController
    {
        EssoEntities _db = new EssoEntities();

        public ActionResult Index(int? id)
        {
            string curUserId = User.Identity.GetUserId();

            string _role =
                            _db.Database.SqlQuery<string>("select \"AspNetRoles\".\"Name\" from \"AspNetUserRoles\" left join \"AspNetRoles\" on \"AspNetUserRoles\".\"RoleId\" = \"AspNetRoles\".\"Id\" where \"AspNetUserRoles\".\"UserId\" = '" + curUserId + "'")
                            .FirstOrDefault();

            List<TBL_COMPANY> ListCompany;
            List<TBL_STATION> ListStation;
            List<TBL_INVERTER> ListInverter;

            switch (_role)
            {
                case "M_ADMIN":
                    ListCompany = _db.Companies.Where(x => x.IS_DELETED == false).OrderBy(x => x.NAME).ToList();
                    ListStation = _db.Stations.Where(x => x.IS_DELETED == false && x.STATION_TYPE != 4).OrderBy(x => x.NAME).ToList();
                    ListInverter = _db.Inverters.Where(x => x.IS_DELETED == false).ToList();

                    break;
                case "COMP_USER":

                    int[] UserStatIds = _db.StationUsers.Where(a => a.USER_ID == curUserId).Select(a => a.STATION_ID).ToArray();
                    int[] compIds = _db.Stations.Where(a => a.IS_DELETED == false && UserStatIds.Contains(a.ID)).Select(a => a.COMPANY_ID).ToArray();
                    ListCompany = _db.Companies.Where(a => a.IS_DELETED == false && compIds.Contains(a.ID)).OrderBy(x => x.NAME).ToList();

                    ListStation = (from station in _db.Stations
                                   join user_station in _db.StationUsers on station.ID equals user_station.STATION_ID
                                   where user_station.USER_ID == curUserId && user_station.IS_DELETED == false && station.STATION_TYPE != 4
                                   select station).OrderBy(x => x.NAME).ToList();

                    ListInverter = (from inv in _db.Inverters
                                    join station in _db.Stations on inv.STATION_ID equals station.ID
                                    join user_station in _db.StationUsers on station.ID equals user_station.STATION_ID
                                    where user_station.USER_ID == curUserId && user_station.IS_DELETED == false && station.IS_DELETED == false && inv.IS_DELETED == false
                                    select inv).ToList();
                    break;

                case "COMP_ADMIN":
                    ListCompany = (from user_company in _db.CompanyUsers
                                   join company in _db.Companies on user_company.COMPANY_ID equals company.ID
                                   where user_company.USER_ID == curUserId && user_company.IS_DELETED == false
                                   select company).OrderBy(x => x.NAME).ToList();

                    ListStation = (from station in _db.Stations
                                   join company in _db.Companies on station.COMPANY_ID equals company.ID
                                   join user_company in _db.CompanyUsers on company.ID equals user_company.COMPANY_ID
                                   where user_company.USER_ID == curUserId && user_company.IS_DELETED == false && station.STATION_TYPE != 4
                                   select station).OrderBy(x => x.NAME).ToList();

                    ListInverter = (from inv in _db.Inverters
                                    join station in _db.Stations on inv.STATION_ID equals station.ID
                                    join company in _db.Companies on station.COMPANY_ID equals company.ID
                                    join user_company in _db.CompanyUsers on company.ID equals user_company.COMPANY_ID
                                    where user_company.USER_ID == curUserId && user_company.IS_DELETED == false && company.IS_DELETED == false && station.IS_DELETED == false && inv.IS_DELETED == false
                                    select inv).ToList();

                    break;
                case "DEMO":
                    int[] UserDemoStatIds = _db.StationUsers.Where(a => a.USER_ID == curUserId).Select(a => a.STATION_ID).ToArray();
                    int[] compDemoIds = _db.Stations.Where(a => a.IS_DELETED == false && UserDemoStatIds.Contains(a.ID)).Select(a => a.COMPANY_ID).ToArray();
                    ListCompany = _db.Companies.Where(a => a.IS_DELETED == false && compDemoIds.Contains(a.ID)).OrderBy(x => x.NAME).ToList();

                    ListStation = (from station in _db.Stations
                                   join user_station in _db.StationUsers on station.ID equals user_station.STATION_ID
                                   where user_station.USER_ID == curUserId && user_station.IS_DELETED == false && station.STATION_TYPE != 4
                                   select station).OrderBy(x => x.NAME).ToList();

                    ListInverter = (from inv in _db.Inverters
                                    join station in _db.Stations on inv.STATION_ID equals station.ID
                                    join user_station in _db.StationUsers on station.ID equals user_station.STATION_ID
                                    where user_station.USER_ID == curUserId && user_station.IS_DELETED == false && station.IS_DELETED == false && inv.IS_DELETED == false
                                    select inv).ToList();

                    break;
                default:
                    ListCompany = new List<TBL_COMPANY>();
                    ListStation = new List<TBL_STATION>();
                    ListInverter = new List<TBL_INVERTER>();

                    break;
            }

            ViewBag.ListCompany = ListCompany;
            ViewBag.ListStation = ListStation;
            ViewBag.ListInverter = ListInverter;
            ViewBag.UserId = curUserId;

            int _Id = 0;
            string ChartName = "";

            if (id != null)
                _Id = Convert.ToInt32(id);

            TBL_USER_CHART _UserChart = _db.UserCharts.Find(_Id);

            List<TBL_USER_CHART_DETAIL> ListChartDetail;

            if (_UserChart != null)
            {
                ChartName = _UserChart.CHART_NAME;

                ListChartDetail = _db.ChartDetails.Where(x => x.CHART_ID == _UserChart.ID).ToList();
            }
            else
            {
                ListChartDetail = new List<TBL_USER_CHART_DETAIL>();
            }

            ViewBag.ChartId = _Id.ToString();
            ViewBag.ChartName = ChartName;
            ViewBag.ListChartDetail = ListChartDetail;

            return View();
        }

        public ActionResult ChartList()
        {
            string curUserId = User.Identity.GetUserId();

            List<TBL_USER_CHART> ListUserChart = _db.UserCharts.Where(x => x.USER_ID == curUserId && x.TYPE == 0).ToList();

            ViewBag.List = ListUserChart;

            return View();
        }

        public ActionResult Chart()
        {
            string curUserId = User.Identity.GetUserId();

            List<TBL_USER_CHART> ListUserChart = _db.UserCharts.Where(x => x.USER_ID == curUserId && x.TYPE == 0).ToList();

            if (ListUserChart == null)
            {
                ListUserChart = new List<TBL_USER_CHART>();
            }

            ViewBag._List = ListUserChart;

            return View();
        }
        public JsonResult ChartTemp(string _datepicker, string _chartId)
        {
            ChartValuePgeView _result = new ChartValuePgeView();

            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("tr-TR");
                DateTime _date = DateTime.Parse(@_datepicker);
                int _ChartId = Convert.ToInt32(_chartId);

                DateTime _StartDate = Convert.ToDateTime(_date.ToShortDateString());
                DateTime _EndDate = Convert.ToDateTime(_date.ToShortDateString() + " 23:59:59");

                List<TBL_USER_CHART_DETAIL> _List = _db.ChartDetails.Where(x => x.CHART_ID == _ChartId).ToList();

                if (_List == null) _List = new List<TBL_USER_CHART_DETAIL>();

                for (int i = 0; i < _List.Count; i++)
                {
                    int _StationId = Convert.ToInt32(_List[i].STATION_ID);
                    int _InverterId = Convert.ToInt32(_List[i].INVERTER_ID);
                    string _DataType = _List[i].VALUE_TYPE;

                    TBL_STATION _Station = _db.Stations.Find(_StationId);
                    TBL_INVERTER _Inverter = _db.Inverters.Find(_InverterId);
                    if (User.IsInRole("DEMO"))
                    {
                        _Station.NAME = _Station.DEMO_NAME;
                    }
                    else
                    {
                        _Station.NAME = _Station.NAME;
                    }
                    ChartValue _ChartValue;

                    switch (_DataType)
                    {
                        case "Frequency":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " Frequency";

                            _ChartValue.ListValue = _db.Summaries
                                                    .Where(x => x.tarih >= _StartDate && x.tarih <= _EndDate && x.STATION_ID == _StationId)
                                                    .OrderBy(x => x.tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.H2_F, Tarih = x.tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "H2_WP_minus":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " Produced_Energy";

                            _ChartValue.ListValue = _db.Summaries
                                                    .Where(x => x.tarih >= _StartDate && x.tarih <= _EndDate && x.STATION_ID == _StationId)
                                                    .OrderBy(x => x.tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.H2_WP_minus, Tarih = x.tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "H2_WP_plus":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " Consumed_Energy";

                            _ChartValue.ListValue = _db.Summaries
                                                    .Where(x => x.tarih >= _StartDate && x.tarih <= _EndDate && x.STATION_ID == _StationId)
                                                    .OrderBy(x => x.tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.H2_WP_plus, Tarih = x.tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "Current_L1":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " Current_L1";

                            _ChartValue.ListValue = _db.Summaries
                                                    .Where(x => x.tarih >= _StartDate && x.tarih <= _EndDate && x.STATION_ID == _StationId)
                                                    .OrderBy(x => x.tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.H2_Ia, Tarih = x.tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "Current_L2":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " Current_L2";

                            _ChartValue.ListValue = _db.Summaries
                                                    .Where(x => x.tarih >= _StartDate && x.tarih <= _EndDate && x.STATION_ID == _StationId)
                                                    .OrderBy(x => x.tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.H2_Ib, Tarih = x.tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "Current_L3":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " Current_L3";

                            _ChartValue.ListValue = _db.Summaries
                                                    .Where(x => x.tarih >= _StartDate && x.tarih <= _EndDate && x.STATION_ID == _StationId)
                                                    .OrderBy(x => x.tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.H2_Ic, Tarih = x.tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "Active_Power_Sum":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " Active_Power_Sum";

                            _ChartValue.ListValue = _db.Summaries
                                                    .Where(x => x.tarih >= _StartDate && x.tarih <= _EndDate && x.STATION_ID == _StationId)
                                                    .OrderBy(x => x.tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.H2_P, Tarih = x.tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "Power_Factor":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " Power_Factor";

                            _ChartValue.ListValue = _db.Summaries
                                                    .Where(x => x.tarih >= _StartDate && x.tarih <= _EndDate && x.STATION_ID == _StationId)
                                                    .OrderBy(x => x.tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.H2_PF, Tarih = x.tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "Reactive_Power_Sum":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " Reactive_Power_Sum";

                            _ChartValue.ListValue = _db.Summaries
                                                    .Where(x => x.tarih >= _StartDate && x.tarih <= _EndDate && x.STATION_ID == _StationId)
                                                    .OrderBy(x => x.tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.H2_Q, Tarih = x.tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "Visible_Power_Sum":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " Visible_Power_Sum";

                            _ChartValue.ListValue = _db.Summaries
                                                    .Where(x => x.tarih >= _StartDate && x.tarih <= _EndDate && x.STATION_ID == _StationId)
                                                    .OrderBy(x => x.tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.H2_S, Tarih = x.tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "Voltage_L1-L2":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " Voltage_L1-L2";

                            _ChartValue.ListValue = _db.Summaries
                                                    .Where(x => x.tarih >= _StartDate && x.tarih <= _EndDate && x.STATION_ID == _StationId)
                                                    .OrderBy(x => x.tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.H2_Vab, Tarih = x.tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "Voltage_L1-L3":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " Voltage_L1-L3";

                            _ChartValue.ListValue = _db.Summaries
                                                    .Where(x => x.tarih >= _StartDate && x.tarih <= _EndDate && x.STATION_ID == _StationId)
                                                    .OrderBy(x => x.tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.H2_Vac, Tarih = x.tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "Voltage_L2-L3":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " Voltage_L2-L3";

                            _ChartValue.ListValue = _db.Summaries
                                                    .Where(x => x.tarih >= _StartDate && x.tarih <= _EndDate && x.STATION_ID == _StationId)
                                                    .OrderBy(x => x.tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.H2_Vbc, Tarih = x.tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "Daily_Production":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " Daily_Production";

                            _ChartValue.ListValue = _db.Summaries
                                                    .Where(x => x.tarih >= _StartDate && x.tarih <= _EndDate && x.STATION_ID == _StationId)
                                                    .OrderBy(x => x.tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.Enerji, Tarih = x.tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "Irradiation":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " Irradiation";

                            _ChartValue.ListValue = _db.Summaries
                                                    .Where(x => x.tarih >= _StartDate && x.tarih <= _EndDate && x.STATION_ID == _StationId)
                                                    .OrderBy(x => x.tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.isinim, Tarih = x.tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "Total_Energy":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " Total_Energy";

                            _ChartValue.ListValue = _db.Summaries
                                                    .Where(x => x.tarih >= _StartDate && x.tarih <= _EndDate && x.STATION_ID == _StationId)
                                                    .OrderBy(x => x.tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.toplamEnerji, Tarih = x.tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "Total_Power_AC":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " Total_Power_AC";

                            _ChartValue.ListValue = _db.Summaries
                                                    .Where(x => x.tarih >= _StartDate && x.tarih <= _EndDate && x.STATION_ID == _StationId)
                                                    .OrderBy(x => x.tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.gunlukUretim, Tarih = x.tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "Total_Power_DC":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " Total_Power_DC";

                            _ChartValue.ListValue = _db.Summaries
                                                    .Where(x => x.tarih >= _StartDate && x.tarih <= _EndDate && x.STATION_ID == _StationId)
                                                    .OrderBy(x => x.tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.Dc_Guc, Tarih = x.tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "Current_AC":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " " + _Inverter.NAME + " Current_AC";

                            _ChartValue.ListValue = _db.InvSums
                                                    .Where(x => x.Tarih >= _StartDate && x.Tarih <= _EndDate && x.Inv_Id == _InverterId)
                                                    .OrderBy(x => x.Tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.Akim_AC, Tarih = x.Tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "Current_DC":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " " + _Inverter.NAME + " Current_DC";

                            _ChartValue.ListValue = _db.InvSums
                                                    .Where(x => x.Tarih >= _StartDate && x.Tarih <= _EndDate && x.Inv_Id == _InverterId)
                                                    .OrderBy(x => x.Tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.Akim_DC, Tarih = x.Tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "Voltage_AC":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " " + _Inverter.NAME + " Voltage_AC";

                            _ChartValue.ListValue = _db.InvSums
                                                    .Where(x => x.Tarih >= _StartDate && x.Tarih <= _EndDate && x.Inv_Id == _InverterId)
                                                    .OrderBy(x => x.Tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.Gerilim_AC, Tarih = x.Tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "Voltage_DC":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " " + _Inverter.NAME + " Voltage_DC";

                            _ChartValue.ListValue = _db.InvSums
                                                    .Where(x => x.Tarih >= _StartDate && x.Tarih <= _EndDate && x.Inv_Id == _InverterId)
                                                    .OrderBy(x => x.Tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.Gerilim_DC, Tarih = x.Tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "Power_AC":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " " + _Inverter.NAME + " Power_AC";

                            _ChartValue.ListValue = _db.InvSums
                                                    .Where(x => x.Tarih >= _StartDate && x.Tarih <= _EndDate && x.Inv_Id == _InverterId)
                                                    .OrderBy(x => x.Tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.Guc_AC, Tarih = x.Tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "Power_DC":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " " + _Inverter.NAME + " Power_DC";

                            _ChartValue.ListValue = _db.InvSums
                                                    .Where(x => x.Tarih >= _StartDate && x.Tarih <= _EndDate && x.Inv_Id == _InverterId)
                                                    .OrderBy(x => x.Tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.Guc_DC, Tarih = x.Tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "Production":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " " + _Inverter.NAME + " Production";

                            _ChartValue.ListValue = _db.InvSums
                                                    .Where(x => x.Tarih >= _StartDate && x.Tarih <= _EndDate && x.Inv_Id == _InverterId)
                                                    .OrderBy(x => x.Tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.Gunluk_Enerji, Tarih = x.Tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        //Inverter Values
                        case "HEARTBEAT":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " " + _Inverter.NAME + " HEARTBEAT";

                            _ChartValue.ListValue = _db.InvSums
                                                    .Where(x => x.Tarih >= _StartDate && x.Tarih <= _EndDate && x.Inv_Id == _InverterId)
                                                    .OrderBy(x => x.Tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.HEARTBEAT, Tarih = x.Tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "INVERTER_MAIN_STATUS":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " " + _Inverter.NAME + " INVERTER_MAIN_STATUS";

                            _ChartValue.ListValue = _db.InvSums
                                                    .Where(x => x.Tarih >= _StartDate && x.Tarih <= _EndDate && x.Inv_Id == _InverterId)
                                                    .OrderBy(x => x.Tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.INVERTER_MAIN_STATUS, Tarih = x.Tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "REACTIVE_POWER":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " " + _Inverter.NAME + " REACTIVE_POWER";

                            _ChartValue.ListValue = _db.InvSums
                                                    .Where(x => x.Tarih >= _StartDate && x.Tarih <= _EndDate && x.Inv_Id == _InverterId)
                                                    .OrderBy(x => x.Tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.REACTIVE_POWER, Tarih = x.Tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "GRID_VOLTAGE_VRMS":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " " + _Inverter.NAME + " GRID_VOLTAGE_VRMS";

                            _ChartValue.ListValue = _db.InvSums
                                                    .Where(x => x.Tarih >= _StartDate && x.Tarih <= _EndDate && x.Inv_Id == _InverterId)
                                                    .OrderBy(x => x.Tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.GRID_VOLTAGE_VRMS, Tarih = x.Tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "GRID_FREQUENCY":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " " + _Inverter.NAME + " GRID_FREQUENCY";

                            _ChartValue.ListValue = _db.InvSums
                                                    .Where(x => x.Tarih >= _StartDate && x.Tarih <= _EndDate && x.Inv_Id == _InverterId)
                                                    .OrderBy(x => x.Tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.GRID_FREQUENCY, Tarih = x.Tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "POWER_FACTOR":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " " + _Inverter.NAME + " POWER_FACTOR";

                            _ChartValue.ListValue = _db.InvSums
                                                    .Where(x => x.Tarih >= _StartDate && x.Tarih <= _EndDate && x.Inv_Id == _InverterId)
                                                    .OrderBy(x => x.Tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.POWER_FACTOR, Tarih = x.Tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "CODE_OF_THE_ACTIVE_FAULT":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " " + _Inverter.NAME + " CODE_OF_THE_ACTIVE_FAULT";

                            _ChartValue.ListValue = _db.InvSums
                                                    .Where(x => x.Tarih >= _StartDate && x.Tarih <= _EndDate && x.Inv_Id == _InverterId)
                                                    .OrderBy(x => x.Tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.CODE_OF_THE_ACTIVE_FAULT, Tarih = x.Tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "GRID_CURRENT":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " " + _Inverter.NAME + " GRID_CURRENT";

                            _ChartValue.ListValue = _db.InvSums
                                                    .Where(x => x.Tarih >= _StartDate && x.Tarih <= _EndDate && x.Inv_Id == _InverterId)
                                                    .OrderBy(x => x.Tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.GRID_CURRENT, Tarih = x.Tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "DC_BUS_VOLTAGE":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " " + _Inverter.NAME + " DC_BUS_VOLTAGE";

                            _ChartValue.ListValue = _db.InvSums
                                                    .Where(x => x.Tarih >= _StartDate && x.Tarih <= _EndDate && x.Inv_Id == _InverterId)
                                                    .OrderBy(x => x.Tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.DC_BUS_VOLTAGE, Tarih = x.Tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "GROUNDING_CURRENT":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " " + _Inverter.NAME + " GROUNDING_CURRENT";

                            _ChartValue.ListValue = _db.InvSums
                                                    .Where(x => x.Tarih >= _StartDate && x.Tarih <= _EndDate && x.Inv_Id == _InverterId)
                                                    .OrderBy(x => x.Tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.GROUNDING_CURRENT, Tarih = x.Tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "SOLATION_RESISTANCE":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " " + _Inverter.NAME + " ISOLATION_RESISTANCE";

                            _ChartValue.ListValue = _db.InvSums
                                                    .Where(x => x.Tarih >= _StartDate && x.Tarih <= _EndDate && x.Inv_Id == _InverterId)
                                                    .OrderBy(x => x.Tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.SOLATION_RESISTANCE, Tarih = x.Tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "AMBIENT_TEMPERATURE":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " " + _Inverter.NAME + " AMBIENT_TEMPERATURE";

                            _ChartValue.ListValue = _db.InvSums
                                                    .Where(x => x.Tarih >= _StartDate && x.Tarih <= _EndDate && x.Inv_Id == _InverterId)
                                                    .OrderBy(x => x.Tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.AMBIENT_TEMPERATURE, Tarih = x.Tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "HIGHEST_IGBT_TEMPERATURE_PU1":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " " + _Inverter.NAME + " HIGHEST_IGBT_TEMPERATURE_PU1";

                            _ChartValue.ListValue = _db.InvSums
                                                    .Where(x => x.Tarih >= _StartDate && x.Tarih <= _EndDate && x.Inv_Id == _InverterId)
                                                    .OrderBy(x => x.Tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.HIGHEST_IGBT_TEMPERATURE_PU1, Tarih = x.Tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "HIGHEST_IGBT_TEMPERATURE_PU2":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " " + _Inverter.NAME + " HIGHEST_IGBT_TEMPERATURE_PU2";

                            _ChartValue.ListValue = _db.InvSums
                                                    .Where(x => x.Tarih >= _StartDate && x.Tarih <= _EndDate && x.Inv_Id == _InverterId)
                                                    .OrderBy(x => x.Tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.HIGHEST_IGBT_TEMPERATURE_PU2, Tarih = x.Tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "HIGHEST_IGBT_TEMPERATURE_PU3":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " " + _Inverter.NAME + " HIGHEST_IGBT_TEMPERATURE_PU3";

                            _ChartValue.ListValue = _db.InvSums
                                                    .Where(x => x.Tarih >= _StartDate && x.Tarih <= _EndDate && x.Inv_Id == _InverterId)
                                                    .OrderBy(x => x.Tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.HIGHEST_IGBT_TEMPERATURE_PU3, Tarih = x.Tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "HIGHEST_IGBT_TEMPERATURE_PU4":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " " + _Inverter.NAME + " HIGHEST_IGBT_TEMPERATURE_PU4";

                            _ChartValue.ListValue = _db.InvSums
                                                    .Where(x => x.Tarih >= _StartDate && x.Tarih <= _EndDate && x.Inv_Id == _InverterId)
                                                    .OrderBy(x => x.Tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.HIGHEST_IGBT_TEMPERATURE_PU4, Tarih = x.Tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "CONTROL_SECTION_TEMPERATURE":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " " + _Inverter.NAME + " CONTROL_SECTION_TEMPERATURE";

                            _ChartValue.ListValue = _db.InvSums
                                                    .Where(x => x.Tarih >= _StartDate && x.Tarih <= _EndDate && x.Inv_Id == _InverterId)
                                                    .OrderBy(x => x.Tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.CONTROL_SECTION_TEMPERATURE, Tarih = x.Tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;

                        case "DAILY_KVAH_SUPPLIED":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " " + _Inverter.NAME + " DAILY_KVAH_SUPPLIED";

                            _ChartValue.ListValue = _db.InvSums
                                                    .Where(x => x.Tarih >= _StartDate && x.Tarih <= _EndDate && x.Inv_Id == _InverterId)
                                                    .OrderBy(x => x.Tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.DAILY_KVAH_SUPPLIED, Tarih = x.Tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "TOTAL_KVAH_SUPPLIED":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " " + _Inverter.NAME + " TOTAL_KVAH_SUPPLIED";

                            _ChartValue.ListValue = _db.InvSums
                                                    .Where(x => x.Tarih >= _StartDate && x.Tarih <= _EndDate && x.Inv_Id == _InverterId)
                                                    .OrderBy(x => x.Tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.TOTAL_KVAH_SUPPLIED, Tarih = x.Tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "IGBT_1_T1":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " " + _Inverter.NAME + " IGBT_1_T1";

                            _ChartValue.ListValue = _db.InvSums
                                                    .Where(x => x.Tarih >= _StartDate && x.Tarih <= _EndDate && x.Inv_Id == _InverterId)
                                                    .OrderBy(x => x.Tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.IGBT_1_T1, Tarih = x.Tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "IGBT_1_T2":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " " + _Inverter.NAME + " IGBT_1_T2";

                            _ChartValue.ListValue = _db.InvSums
                                                    .Where(x => x.Tarih >= _StartDate && x.Tarih <= _EndDate && x.Inv_Id == _InverterId)
                                                    .OrderBy(x => x.Tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.IGBT_1_T2, Tarih = x.Tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "IGBT_1_T3":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " " + _Inverter.NAME + " IGBT_1_T3";

                            _ChartValue.ListValue = _db.InvSums
                                                    .Where(x => x.Tarih >= _StartDate && x.Tarih <= _EndDate && x.Inv_Id == _InverterId)
                                                    .OrderBy(x => x.Tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.IGBT_1_T3, Tarih = x.Tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "IGBT_2_T1":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " " + _Inverter.NAME + " IGBT_2_T1";

                            _ChartValue.ListValue = _db.InvSums
                                                    .Where(x => x.Tarih >= _StartDate && x.Tarih <= _EndDate && x.Inv_Id == _InverterId)
                                                    .OrderBy(x => x.Tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.IGBT_2_T1, Tarih = x.Tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "IGBT_2_T2":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " " + _Inverter.NAME + " IGBT_2_T2";

                            _ChartValue.ListValue = _db.InvSums
                                                    .Where(x => x.Tarih >= _StartDate && x.Tarih <= _EndDate && x.Inv_Id == _InverterId)
                                                    .OrderBy(x => x.Tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.IGBT_2_T2, Tarih = x.Tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "IGBT_2_T3":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " " + _Inverter.NAME + " IGBT_2_T3";

                            _ChartValue.ListValue = _db.InvSums
                                                    .Where(x => x.Tarih >= _StartDate && x.Tarih <= _EndDate && x.Inv_Id == _InverterId)
                                                    .OrderBy(x => x.Tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.IGBT_2_T3, Tarih = x.Tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "IGBT_3_T1":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " " + _Inverter.NAME + " IGBT_3_T1";

                            _ChartValue.ListValue = _db.InvSums
                                                    .Where(x => x.Tarih >= _StartDate && x.Tarih <= _EndDate && x.Inv_Id == _InverterId)
                                                    .OrderBy(x => x.Tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.IGBT_3_T1, Tarih = x.Tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "IGBT_3_T2":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " " + _Inverter.NAME + " IGBT_3_T2";

                            _ChartValue.ListValue = _db.InvSums
                                                    .Where(x => x.Tarih >= _StartDate && x.Tarih <= _EndDate && x.Inv_Id == _InverterId)
                                                    .OrderBy(x => x.Tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.IGBT_3_T2, Tarih = x.Tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "IGBT_3_T3":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " " + _Inverter.NAME + " IGBT_3_T3";

                            _ChartValue.ListValue = _db.InvSums
                                                    .Where(x => x.Tarih >= _StartDate && x.Tarih <= _EndDate && x.Inv_Id == _InverterId)
                                                    .OrderBy(x => x.Tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.IGBT_3_T3, Tarih = x.Tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "IGBT_4_T1":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " " + _Inverter.NAME + " IGBT_4_T1";

                            _ChartValue.ListValue = _db.InvSums
                                                    .Where(x => x.Tarih >= _StartDate && x.Tarih <= _EndDate && x.Inv_Id == _InverterId)
                                                    .OrderBy(x => x.Tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.IGBT_4_T1, Tarih = x.Tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "IGBT_4_T2":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " " + _Inverter.NAME + " IGBT_4_T2";

                            _ChartValue.ListValue = _db.InvSums
                                                    .Where(x => x.Tarih >= _StartDate && x.Tarih <= _EndDate && x.Inv_Id == _InverterId)
                                                    .OrderBy(x => x.Tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.IGBT_4_T2, Tarih = x.Tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "IGBT_4_T3":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " " + _Inverter.NAME + " IGBT_4_T3";

                            _ChartValue.ListValue = _db.InvSums
                                                    .Where(x => x.Tarih >= _StartDate && x.Tarih <= _EndDate && x.Inv_Id == _InverterId)
                                                    .OrderBy(x => x.Tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.IGBT_4_T3, Tarih = x.Tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        //Meteoroloji 2
                        case "MEAN_WIND_DIRECTION_1":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " MEAN_WIND_DIRECTION_1";

                            _ChartValue.ListValue = _db.Summaries
                                                    .Where(x => x.tarih >= _StartDate && x.tarih <= _EndDate && x.STATION_ID == _StationId)
                                                    .OrderBy(x => x.tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.MEAN_WIND_DIRECTION_1, Tarih = x.tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "AIR_TEMPERATURE_1":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " AIR_TEMPERATURE_1";

                            _ChartValue.ListValue = _db.Summaries
                                                    .Where(x => x.tarih >= _StartDate && x.tarih <= _EndDate && x.STATION_ID == _StationId)
                                                    .OrderBy(x => x.tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.AIR_TEMPERATURE_1, Tarih = x.tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "RELATIVE_HUMIDITY_1":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " RELATIVE_HUMIDITY_1";

                            _ChartValue.ListValue = _db.Summaries
                                                    .Where(x => x.tarih >= _StartDate && x.tarih <= _EndDate && x.STATION_ID == _StationId)
                                                    .OrderBy(x => x.tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.RELATIVE_HUMIDITY_1, Tarih = x.tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "ABSOLUTE_HUMIDITY_1":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " ABSOLUTE_HUMIDITY_1";

                            _ChartValue.ListValue = _db.Summaries
                                                    .Where(x => x.tarih >= _StartDate && x.tarih <= _EndDate && x.STATION_ID == _StationId)
                                                    .OrderBy(x => x.tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.ABSOLUTE_HUMIDITY_1, Tarih = x.tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "ABSOLUTE_AIR_PRESSURE_1":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " ABSOLUTE_AIR_PRESSURE_1";

                            _ChartValue.ListValue = _db.Summaries
                                                    .Where(x => x.tarih >= _StartDate && x.tarih <= _EndDate && x.STATION_ID == _StationId)
                                                    .OrderBy(x => x.tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.ABSOLUTE_AIR_PRESSURE_1, Tarih = x.tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "ISINIM_2":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " SLAVE IRRADIATION";

                            _ChartValue.ListValue = _db.Summaries
                                                    .Where(x => x.tarih >= _StartDate && x.tarih <= _EndDate && x.STATION_ID == _StationId)
                                                    .OrderBy(x => x.tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.ISINIM_2, Tarih = x.tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "RUZGARHIZI_2":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " SLAVE WIND SPEED";

                            _ChartValue.ListValue = _db.Summaries
                                                    .Where(x => x.tarih >= _StartDate && x.tarih <= _EndDate && x.STATION_ID == _StationId)
                                                    .OrderBy(x => x.tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.RUZGARHIZI_2, Tarih = x.tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "HUCRESICAKLIGI_2":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " SLAVE CELL TEMP.";

                            _ChartValue.ListValue = _db.Summaries
                                                    .Where(x => x.tarih >= _StartDate && x.tarih <= _EndDate && x.STATION_ID == _StationId)
                                                    .OrderBy(x => x.tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.HUCRESICAKLIGI_2, Tarih = x.tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "SICAKLIK_2":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " SLAVE EXTERNAL TEMP.";

                            _ChartValue.ListValue = _db.Summaries
                                                    .Where(x => x.tarih >= _StartDate && x.tarih <= _EndDate && x.STATION_ID == _StationId)
                                                    .OrderBy(x => x.tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.SICAKLIK_2, Tarih = x.tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "MEAN_WIND_DIRECTION_2":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " SLAVE MEAN WIND DIRECTION";

                            _ChartValue.ListValue = _db.Summaries
                                                    .Where(x => x.tarih >= _StartDate && x.tarih <= _EndDate && x.STATION_ID == _StationId)
                                                    .OrderBy(x => x.tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.PYRANOMETER_2, Tarih = x.tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "AIR_TEMPERATURE_2":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " SLAVE AIR TEMPERATURE";

                            _ChartValue.ListValue = _db.Summaries
                                                    .Where(x => x.tarih >= _StartDate && x.tarih <= _EndDate && x.STATION_ID == _StationId)
                                                    .OrderBy(x => x.tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.AIR_TEMPERATURE_2, Tarih = x.tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "RELATIVE_HUMIDITY_2":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " SLAVE RELATIVE HUMIDITY";

                            _ChartValue.ListValue = _db.Summaries
                                                    .Where(x => x.tarih >= _StartDate && x.tarih <= _EndDate && x.STATION_ID == _StationId)
                                                    .OrderBy(x => x.tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.RELATIVE_HUMIDITY_2, Tarih = x.tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "ABSOLUTE_HUMIDITY_2":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " SLAVE ABSOLUTE HUMIDITY";

                            _ChartValue.ListValue = _db.Summaries
                                                    .Where(x => x.tarih >= _StartDate && x.tarih <= _EndDate && x.STATION_ID == _StationId)
                                                    .OrderBy(x => x.tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.ABSOLUTE_HUMIDITY_2, Tarih = x.tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;

                        case "ABSOLUTE_AIR_PRESSURE_2":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " SLAVE ABSOLUTE AIR PRESSURE";

                            _ChartValue.ListValue = _db.Summaries
                                                    .Where(x => x.tarih >= _StartDate && x.tarih <= _EndDate && x.STATION_ID == _StationId)
                                                    .OrderBy(x => x.tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.ABSOLUTE_AIR_PRESSURE_2, Tarih = x.tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "Cell_Temperature":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " MASTER CELL TEMPERATURE";

                            _ChartValue.ListValue = _db.Summaries
                                                    .Where(x => x.tarih >= _StartDate && x.tarih <= _EndDate && x.STATION_ID == _StationId)
                                                    .OrderBy(x => x.tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.hucreSicakligi, Tarih = x.tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;

                        case "External_Temperature":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " MASTER EXTERNAL TEMPERATURE";

                            _ChartValue.ListValue = _db.Summaries
                                                    .Where(x => x.tarih >= _StartDate && x.tarih <= _EndDate && x.STATION_ID == _StationId)
                                                    .OrderBy(x => x.tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.sicaklik, Tarih = x.tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "Wind_Speed":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " MASTER WIND SPEED";

                            _ChartValue.ListValue = _db.Summaries
                                                    .Where(x => x.tarih >= _StartDate && x.tarih <= _EndDate && x.STATION_ID == _StationId)
                                                    .OrderBy(x => x.tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.ruzgarHizi, Tarih = x.tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                _result.ErrorMessage = ex.ToString();
            }

            return Json(_result, JsonRequestBehavior.AllowGet);
        }



        #region AJAX
        [HttpPost]
        public JsonResult GetChartValues(List<TreeViewDetail> data)
        {
            ChartValuePgeView _result = new ChartValuePgeView();

            try
            {
                DateTime _StartDate = Convert.ToDateTime(DateTime.Now.ToShortDateString());
                DateTime _EndDate = Convert.ToDateTime(DateTime.Now.ToShortDateString() + " 23:59:59");

                if (data == null) data = new List<TreeViewDetail>();

                for (int i = 0; i < data.Count; i++)
                {
                    int _StationId = Convert.ToInt32(data[i].StationId);
                    int _InverterId = Convert.ToInt32(data[i].InverterId);
                    string _DataType = data[i].DataType;

                    TBL_STATION _Station = _db.Stations.Find(_StationId);
                    if (User.IsInRole("DEMO"))
                    {
                        _Station.NAME = _Station.DEMO_NAME;
                    }
                    else
                    {
                        _Station.NAME = _Station.NAME;
                    }
                    TBL_INVERTER _Inverter = _db.Inverters.Find(_InverterId);

                    ChartValue _ChartValue;

                    switch (_DataType)
                    {
                        case "Frequency":
                            _ChartValue = new ChartValue();
                            _ChartValue.Name = _Station.NAME + " Frequency";

                            _ChartValue.ListValue = _db.Summaries
                                                                                                                           .Where(x => x.tarih >= _StartDate && x.tarih <= _EndDate && x.STATION_ID == _StationId)
                                                                                                                           .OrderBy(x => x.tarih)
                                                                                                                           .Select(x => new ChartValueDetail { Value = x.H2_F, Tarih = x.tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "H2_WP_minus":
                            _ChartValue = new ChartValue();
                            _ChartValue.Name = _Station.NAME + " Produced Energy";

                            _ChartValue.ListValue = _db.Summaries
                                                                                                                           .Where(x => x.tarih >= _StartDate && x.tarih <= _EndDate && x.STATION_ID == _StationId)
                                                                                                                           .OrderBy(x => x.tarih)
                                                                                                                           .Select(x => new ChartValueDetail { Value = x.H2_WP_minus, Tarih = x.tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "H2_WP_plus":
                            _ChartValue = new ChartValue();
                            _ChartValue.Name = _Station.NAME + " Consumed Energy";

                            _ChartValue.ListValue = _db.Summaries
                                                                                                                           .Where(x => x.tarih >= _StartDate && x.tarih <= _EndDate && x.STATION_ID == _StationId)
                                                                                                                           .OrderBy(x => x.tarih)
                                                                                                                           .Select(x => new ChartValueDetail { Value = x.H2_WP_plus, Tarih = x.tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "Current_L1":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " Current_L1";

                            _ChartValue.ListValue = _db.Summaries
                                                                                                                           .Where(x => x.tarih >= _StartDate && x.tarih <= _EndDate && x.STATION_ID == _StationId)
                                                                                                                           .OrderBy(x => x.tarih)
                                                                                                                           .Select(x => new ChartValueDetail { Value = x.H2_Ia, Tarih = x.tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "Current_L2":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " Current_L2";

                            _ChartValue.ListValue = _db.Summaries
                                                                                                                           .Where(x => x.tarih >= _StartDate && x.tarih <= _EndDate && x.STATION_ID == _StationId)
                                                                                                                           .OrderBy(x => x.tarih)
                                                                                                                           .Select(x => new ChartValueDetail { Value = x.H2_Ib, Tarih = x.tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "Current_L3":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " Current_L3";

                            _ChartValue.ListValue = _db.Summaries
                                                                                                                           .Where(x => x.tarih >= _StartDate && x.tarih <= _EndDate && x.STATION_ID == _StationId)
                                                                                                                           .OrderBy(x => x.tarih)
                                                                                                                           .Select(x => new ChartValueDetail { Value = x.H2_Ic, Tarih = x.tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "Active_Power_Sum":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " Active_Power_Sum";

                            _ChartValue.ListValue = _db.Summaries
                                                                                                                           .Where(x => x.tarih >= _StartDate && x.tarih <= _EndDate && x.STATION_ID == _StationId)
                                                                                                                           .OrderBy(x => x.tarih)
                                                                                                                           .Select(x => new ChartValueDetail { Value = x.H2_P, Tarih = x.tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "Power_Factor":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " Power_Factor";

                            _ChartValue.ListValue = _db.Summaries
                                                                                                                           .Where(x => x.tarih >= _StartDate && x.tarih <= _EndDate && x.STATION_ID == _StationId)
                                                                                                                           .OrderBy(x => x.tarih)
                                                                                                                           .Select(x => new ChartValueDetail { Value = x.H2_PF, Tarih = x.tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "Reactive_Power_Sum":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " Reactive_Power_Sum";

                            _ChartValue.ListValue = _db.Summaries
                                                                                                                           .Where(x => x.tarih >= _StartDate && x.tarih <= _EndDate && x.STATION_ID == _StationId)
                                                                                                                           .OrderBy(x => x.tarih)
                                                                                                                           .Select(x => new ChartValueDetail { Value = x.H2_Q, Tarih = x.tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "Visible_Power_Sum":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " Visible_Power_Sum";

                            _ChartValue.ListValue = _db.Summaries
                                                                                                                           .Where(x => x.tarih >= _StartDate && x.tarih <= _EndDate && x.STATION_ID == _StationId)
                                                                                                                           .OrderBy(x => x.tarih)
                                                                                                                           .Select(x => new ChartValueDetail { Value = x.H2_S, Tarih = x.tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "Voltage_L1-L2":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " Voltage_L1-L2";

                            _ChartValue.ListValue = _db.Summaries
                                                                                                                           .Where(x => x.tarih >= _StartDate && x.tarih <= _EndDate && x.STATION_ID == _StationId)
                                                                                                                           .OrderBy(x => x.tarih)
                                                                                                                           .Select(x => new ChartValueDetail { Value = x.H2_Vab, Tarih = x.tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "Voltage_L1-L3":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " Voltage_L1-L3";

                            _ChartValue.ListValue = _db.Summaries
                                                                                                                           .Where(x => x.tarih >= _StartDate && x.tarih <= _EndDate && x.STATION_ID == _StationId)
                                                                                                                           .OrderBy(x => x.tarih)
                                                                                                                           .Select(x => new ChartValueDetail { Value = x.H2_Vac, Tarih = x.tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "Voltage_L2-L3":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " Voltage_L2-L3";

                            _ChartValue.ListValue = _db.Summaries
                                                                                                                           .Where(x => x.tarih >= _StartDate && x.tarih <= _EndDate && x.STATION_ID == _StationId)
                                                                                                                           .OrderBy(x => x.tarih)
                                                                                                                           .Select(x => new ChartValueDetail { Value = x.H2_Vbc, Tarih = x.tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "Daily_Production":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " Daily_Production";

                            _ChartValue.ListValue = _db.Summaries
                                                                                                                           .Where(x => x.tarih >= _StartDate && x.tarih <= _EndDate && x.STATION_ID == _StationId)
                                                                                                                           .OrderBy(x => x.tarih)
                                                                                                                          .Select(x => new ChartValueDetail { Value = x.Enerji, Tarih = x.tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "Irradiation":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " Irradiation";

                            _ChartValue.ListValue = _db.Summaries
                                                                                                                           .Where(x => x.tarih >= _StartDate && x.tarih <= _EndDate && x.STATION_ID == _StationId)
                                                                                                                           .OrderBy(x => x.tarih)
                                                                                                                           .Select(x => new ChartValueDetail { Value = x.isinim, Tarih = x.tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "Total_Energy":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " Total_Energy";

                            _ChartValue.ListValue = _db.Summaries
                                                                                                                           .Where(x => x.tarih >= _StartDate && x.tarih <= _EndDate && x.STATION_ID == _StationId)
                                                                                                                           .OrderBy(x => x.tarih)
                                                                                                                           .Select(x => new ChartValueDetail { Value = x.toplamEnerji, Tarih = x.tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "Total_Power_AC":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " Total_Power_AC";

                            _ChartValue.ListValue = _db.Summaries
                                                                                                                           .Where(x => x.tarih >= _StartDate && x.tarih <= _EndDate && x.STATION_ID == _StationId)
                                                                                                                           .OrderBy(x => x.tarih)
                                                                                                                           .Select(x => new ChartValueDetail { Value = x.gunlukUretim, Tarih = x.tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "Total_Power_DC":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " Total_Power_DC";

                            _ChartValue.ListValue = _db.Summaries
                                                                                                                           .Where(x => x.tarih >= _StartDate && x.tarih <= _EndDate && x.STATION_ID == _StationId)
                                                                                                                           .OrderBy(x => x.tarih)
                                                                                                                           .Select(x => new ChartValueDetail { Value = x.Dc_Guc, Tarih = x.tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;//TBL_OZET SON
                        case "Current_AC":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " " + _Inverter.NAME + " Current_AC";

                            _ChartValue.ListValue = _db.InvSums
                                                                                                                           .Where(x => x.Tarih >= _StartDate && x.Tarih <= _EndDate && x.Inv_Id == _InverterId)
                                                                                                                           .OrderBy(x => x.Tarih)
                                                                                                                           .Select(x => new ChartValueDetail { Value = x.Akim_AC, Tarih = x.Tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "Current_DC":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " " + _Inverter.NAME + " Current_DC";

                            _ChartValue.ListValue = _db.InvSums
                                                                                                                           .Where(x => x.Tarih >= _StartDate && x.Tarih <= _EndDate && x.Inv_Id == _InverterId)
                                                                                                                           .OrderBy(x => x.Tarih)
                                                                                                                           .Select(x => new ChartValueDetail { Value = x.Akim_DC, Tarih = x.Tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "Voltage_AC":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " " + _Inverter.NAME + " Voltage_AC";

                            _ChartValue.ListValue = _db.InvSums
                                                                                                                           .Where(x => x.Tarih >= _StartDate && x.Tarih <= _EndDate && x.Inv_Id == _InverterId)
                                                                                                                           .OrderBy(x => x.Tarih)
                                                                                                                           .Select(x => new ChartValueDetail { Value = x.Gerilim_AC, Tarih = x.Tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "Voltage_DC":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " " + _Inverter.NAME + " Voltage_DC";

                            _ChartValue.ListValue = _db.InvSums
                                                                                                                           .Where(x => x.Tarih >= _StartDate && x.Tarih <= _EndDate && x.Inv_Id == _InverterId)
                                                                                                                           .OrderBy(x => x.Tarih)
                                                                                                                           .Select(x => new ChartValueDetail { Value = x.Gerilim_DC, Tarih = x.Tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "Power_AC":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " " + _Inverter.NAME + " Power_AC";

                            _ChartValue.ListValue = _db.InvSums
                                                                                                                           .Where(x => x.Tarih >= _StartDate && x.Tarih <= _EndDate && x.Inv_Id == _InverterId)
                                                                                                                           .OrderBy(x => x.Tarih)
                                                                                                                           .Select(x => new ChartValueDetail { Value = x.Guc_AC, Tarih = x.Tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "Power_DC":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " " + _Inverter.NAME + " Power_DC";

                            _ChartValue.ListValue = _db.InvSums
                                                                                                                           .Where(x => x.Tarih >= _StartDate && x.Tarih <= _EndDate && x.Inv_Id == _InverterId)
                                                                                                                           .OrderBy(x => x.Tarih)
                                                                                                                           .Select(x => new ChartValueDetail { Value = x.Guc_DC, Tarih = x.Tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "Production":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " " + _Inverter.NAME + " Production";

                            _ChartValue.ListValue = _db.InvSums
                                                                                                                           .Where(x => x.Tarih >= _StartDate && x.Tarih <= _EndDate && x.Inv_Id == _InverterId)
                                                                                                                          .OrderBy(x => x.Tarih)
                                                                                                                           .Select(x => new ChartValueDetail { Value = x.Gunluk_Enerji, Tarih = x.Tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        //Inverter Values
                        case "HEARTBEAT":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " " + _Inverter.NAME + " HEARTBEAT";

                            _ChartValue.ListValue = _db.InvSums
                                                    .Where(x => x.Tarih >= _StartDate && x.Tarih <= _EndDate && x.Inv_Id == _InverterId)
                                                    .OrderBy(x => x.Tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.HEARTBEAT, Tarih = x.Tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "INVERTER_MAIN_STATUS":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " " + _Inverter.NAME + " INVERTER_MAIN_STATUS";

                            _ChartValue.ListValue = _db.InvSums
                                                    .Where(x => x.Tarih >= _StartDate && x.Tarih <= _EndDate && x.Inv_Id == _InverterId)
                                                    .OrderBy(x => x.Tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.INVERTER_MAIN_STATUS, Tarih = x.Tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "REACTIVE_POWER":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " " + _Inverter.NAME + " REACTIVE_POWER";

                            _ChartValue.ListValue = _db.InvSums
                                                    .Where(x => x.Tarih >= _StartDate && x.Tarih <= _EndDate && x.Inv_Id == _InverterId)
                                                    .OrderBy(x => x.Tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.REACTIVE_POWER, Tarih = x.Tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "GRID_VOLTAGE_VRMS":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " " + _Inverter.NAME + " GRID_VOLTAGE_VRMS";

                            _ChartValue.ListValue = _db.InvSums
                                                    .Where(x => x.Tarih >= _StartDate && x.Tarih <= _EndDate && x.Inv_Id == _InverterId)
                                                    .OrderBy(x => x.Tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.GRID_VOLTAGE_VRMS, Tarih = x.Tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "GRID_FREQUENCY":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " " + _Inverter.NAME + " GRID_FREQUENCY";

                            _ChartValue.ListValue = _db.InvSums
                                                    .Where(x => x.Tarih >= _StartDate && x.Tarih <= _EndDate && x.Inv_Id == _InverterId)
                                                    .OrderBy(x => x.Tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.GRID_FREQUENCY, Tarih = x.Tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "POWER_FACTOR":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " " + _Inverter.NAME + " POWER_FACTOR";

                            _ChartValue.ListValue = _db.InvSums
                                                    .Where(x => x.Tarih >= _StartDate && x.Tarih <= _EndDate && x.Inv_Id == _InverterId)
                                                    .OrderBy(x => x.Tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.POWER_FACTOR, Tarih = x.Tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "CODE_OF_THE_ACTIVE_FAULT":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " " + _Inverter.NAME + " CODE_OF_THE_ACTIVE_FAULT";

                            _ChartValue.ListValue = _db.InvSums
                                                    .Where(x => x.Tarih >= _StartDate && x.Tarih <= _EndDate && x.Inv_Id == _InverterId)
                                                    .OrderBy(x => x.Tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.CODE_OF_THE_ACTIVE_FAULT, Tarih = x.Tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "GRID_CURRENT":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " " + _Inverter.NAME + " GRID_CURRENT";

                            _ChartValue.ListValue = _db.InvSums
                                                    .Where(x => x.Tarih >= _StartDate && x.Tarih <= _EndDate && x.Inv_Id == _InverterId)
                                                    .OrderBy(x => x.Tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.GRID_CURRENT, Tarih = x.Tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "DC_BUS_VOLTAGE":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " " + _Inverter.NAME + " DC_BUS_VOLTAGE";

                            _ChartValue.ListValue = _db.InvSums
                                                    .Where(x => x.Tarih >= _StartDate && x.Tarih <= _EndDate && x.Inv_Id == _InverterId)
                                                    .OrderBy(x => x.Tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.DC_BUS_VOLTAGE, Tarih = x.Tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "GROUNDING_CURRENT":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " " + _Inverter.NAME + " GROUNDING_CURRENT";

                            _ChartValue.ListValue = _db.InvSums
                                                    .Where(x => x.Tarih >= _StartDate && x.Tarih <= _EndDate && x.Inv_Id == _InverterId)
                                                    .OrderBy(x => x.Tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.GROUNDING_CURRENT, Tarih = x.Tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "SOLATION_RESISTANCE":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " " + _Inverter.NAME + " ISOLATION_RESISTANCE";

                            _ChartValue.ListValue = _db.InvSums
                                                    .Where(x => x.Tarih >= _StartDate && x.Tarih <= _EndDate && x.Inv_Id == _InverterId)
                                                    .OrderBy(x => x.Tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.SOLATION_RESISTANCE, Tarih = x.Tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "AMBIENT_TEMPERATURE":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " " + _Inverter.NAME + " AMBIENT_TEMPERATURE";

                            _ChartValue.ListValue = _db.InvSums
                                                    .Where(x => x.Tarih >= _StartDate && x.Tarih <= _EndDate && x.Inv_Id == _InverterId)
                                                    .OrderBy(x => x.Tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.AMBIENT_TEMPERATURE, Tarih = x.Tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "HIGHEST_IGBT_TEMPERATURE_PU1":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " " + _Inverter.NAME + " HIGHEST_IGBT_TEMPERATURE_PU1";

                            _ChartValue.ListValue = _db.InvSums
                                                    .Where(x => x.Tarih >= _StartDate && x.Tarih <= _EndDate && x.Inv_Id == _InverterId)
                                                    .OrderBy(x => x.Tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.HIGHEST_IGBT_TEMPERATURE_PU1, Tarih = x.Tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "HIGHEST_IGBT_TEMPERATURE_PU2":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " " + _Inverter.NAME + " HIGHEST_IGBT_TEMPERATURE_PU2";

                            _ChartValue.ListValue = _db.InvSums
                                                    .Where(x => x.Tarih >= _StartDate && x.Tarih <= _EndDate && x.Inv_Id == _InverterId)
                                                    .OrderBy(x => x.Tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.HIGHEST_IGBT_TEMPERATURE_PU2, Tarih = x.Tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "HIGHEST_IGBT_TEMPERATURE_PU3":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " " + _Inverter.NAME + " HIGHEST_IGBT_TEMPERATURE_PU3";

                            _ChartValue.ListValue = _db.InvSums
                                                    .Where(x => x.Tarih >= _StartDate && x.Tarih <= _EndDate && x.Inv_Id == _InverterId)
                                                    .OrderBy(x => x.Tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.HIGHEST_IGBT_TEMPERATURE_PU3, Tarih = x.Tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "HIGHEST_IGBT_TEMPERATURE_PU4":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " " + _Inverter.NAME + " HIGHEST_IGBT_TEMPERATURE_PU4";

                            _ChartValue.ListValue = _db.InvSums
                                                    .Where(x => x.Tarih >= _StartDate && x.Tarih <= _EndDate && x.Inv_Id == _InverterId)
                                                    .OrderBy(x => x.Tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.HIGHEST_IGBT_TEMPERATURE_PU4, Tarih = x.Tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "CONTROL_SECTION_TEMPERATURE":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " " + _Inverter.NAME + " CONTROL_SECTION_TEMPERATURE";

                            _ChartValue.ListValue = _db.InvSums
                                                    .Where(x => x.Tarih >= _StartDate && x.Tarih <= _EndDate && x.Inv_Id == _InverterId)
                                                    .OrderBy(x => x.Tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.CONTROL_SECTION_TEMPERATURE, Tarih = x.Tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;

                        case "DAILY_KVAH_SUPPLIED":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " " + _Inverter.NAME + " DAILY_KVAH_SUPPLIED";

                            _ChartValue.ListValue = _db.InvSums
                                                    .Where(x => x.Tarih >= _StartDate && x.Tarih <= _EndDate && x.Inv_Id == _InverterId)
                                                    .OrderBy(x => x.Tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.DAILY_KVAH_SUPPLIED, Tarih = x.Tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "TOTAL_KVAH_SUPPLIED":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " " + _Inverter.NAME + " TOTAL_KVAH_SUPPLIED";

                            _ChartValue.ListValue = _db.InvSums
                                                    .Where(x => x.Tarih >= _StartDate && x.Tarih <= _EndDate && x.Inv_Id == _InverterId)
                                                    .OrderBy(x => x.Tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.TOTAL_KVAH_SUPPLIED, Tarih = x.Tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "IGBT_1_T1":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " " + _Inverter.NAME + " IGBT_1_T1";

                            _ChartValue.ListValue = _db.InvSums
                                                    .Where(x => x.Tarih >= _StartDate && x.Tarih <= _EndDate && x.Inv_Id == _InverterId)
                                                    .OrderBy(x => x.Tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.IGBT_1_T1, Tarih = x.Tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "IGBT_1_T2":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " " + _Inverter.NAME + " IGBT_1_T2";

                            _ChartValue.ListValue = _db.InvSums
                                                    .Where(x => x.Tarih >= _StartDate && x.Tarih <= _EndDate && x.Inv_Id == _InverterId)
                                                    .OrderBy(x => x.Tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.IGBT_1_T2, Tarih = x.Tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "IGBT_1_T3":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " " + _Inverter.NAME + " IGBT_1_T3";

                            _ChartValue.ListValue = _db.InvSums
                                                    .Where(x => x.Tarih >= _StartDate && x.Tarih <= _EndDate && x.Inv_Id == _InverterId)
                                                    .OrderBy(x => x.Tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.IGBT_1_T3, Tarih = x.Tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "IGBT_2_T1":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " " + _Inverter.NAME + " IGBT_2_T1";

                            _ChartValue.ListValue = _db.InvSums
                                                    .Where(x => x.Tarih >= _StartDate && x.Tarih <= _EndDate && x.Inv_Id == _InverterId)
                                                    .OrderBy(x => x.Tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.IGBT_2_T1, Tarih = x.Tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "IGBT_2_T2":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " " + _Inverter.NAME + " IGBT_2_T2";

                            _ChartValue.ListValue = _db.InvSums
                                                    .Where(x => x.Tarih >= _StartDate && x.Tarih <= _EndDate && x.Inv_Id == _InverterId)
                                                    .OrderBy(x => x.Tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.IGBT_2_T2, Tarih = x.Tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "IGBT_2_T3":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " " + _Inverter.NAME + " IGBT_2_T3";

                            _ChartValue.ListValue = _db.InvSums
                                                    .Where(x => x.Tarih >= _StartDate && x.Tarih <= _EndDate && x.Inv_Id == _InverterId)
                                                    .OrderBy(x => x.Tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.IGBT_2_T3, Tarih = x.Tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "IGBT_3_T1":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " " + _Inverter.NAME + " IGBT_3_T1";

                            _ChartValue.ListValue = _db.InvSums
                                                    .Where(x => x.Tarih >= _StartDate && x.Tarih <= _EndDate && x.Inv_Id == _InverterId)
                                                    .OrderBy(x => x.Tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.IGBT_3_T1, Tarih = x.Tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "IGBT_3_T2":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " " + _Inverter.NAME + " IGBT_3_T2";

                            _ChartValue.ListValue = _db.InvSums
                                                    .Where(x => x.Tarih >= _StartDate && x.Tarih <= _EndDate && x.Inv_Id == _InverterId)
                                                    .OrderBy(x => x.Tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.IGBT_3_T2, Tarih = x.Tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "IGBT_3_T3":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " " + _Inverter.NAME + " IGBT_3_T3";

                            _ChartValue.ListValue = _db.InvSums
                                                    .Where(x => x.Tarih >= _StartDate && x.Tarih <= _EndDate && x.Inv_Id == _InverterId)
                                                    .OrderBy(x => x.Tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.IGBT_3_T3, Tarih = x.Tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "IGBT_4_T1":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " " + _Inverter.NAME + " IGBT_4_T1";

                            _ChartValue.ListValue = _db.InvSums
                                                    .Where(x => x.Tarih >= _StartDate && x.Tarih <= _EndDate && x.Inv_Id == _InverterId)
                                                    .OrderBy(x => x.Tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.IGBT_4_T1, Tarih = x.Tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "IGBT_4_T2":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " " + _Inverter.NAME + " IGBT_4_T2";

                            _ChartValue.ListValue = _db.InvSums
                                                    .Where(x => x.Tarih >= _StartDate && x.Tarih <= _EndDate && x.Inv_Id == _InverterId)
                                                    .OrderBy(x => x.Tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.IGBT_4_T2, Tarih = x.Tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "IGBT_4_T3":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " " + _Inverter.NAME + " IGBT_4_T3";

                            _ChartValue.ListValue = _db.InvSums
                                                    .Where(x => x.Tarih >= _StartDate && x.Tarih <= _EndDate && x.Inv_Id == _InverterId)
                                                    .OrderBy(x => x.Tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.IGBT_4_T3, Tarih = x.Tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        //Meteoroloji 2
                        case "MEAN_WIND_DIRECTION_1":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " MEAN_WIND_DIRECTION_1";

                            _ChartValue.ListValue = _db.Summaries
                                                    .Where(x => x.tarih >= _StartDate && x.tarih <= _EndDate && x.STATION_ID == _StationId)
                                                    .OrderBy(x => x.tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.MEAN_WIND_DIRECTION_1, Tarih = x.tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "AIR_TEMPERATURE_1":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " AIR_TEMPERATURE_1";

                            _ChartValue.ListValue = _db.Summaries
                                                    .Where(x => x.tarih >= _StartDate && x.tarih <= _EndDate && x.STATION_ID == _StationId)
                                                    .OrderBy(x => x.tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.AIR_TEMPERATURE_1, Tarih = x.tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "RELATIVE_HUMIDITY_1":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " RELATIVE_HUMIDITY_1";

                            _ChartValue.ListValue = _db.Summaries
                                                    .Where(x => x.tarih >= _StartDate && x.tarih <= _EndDate && x.STATION_ID == _StationId)
                                                    .OrderBy(x => x.tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.RELATIVE_HUMIDITY_1, Tarih = x.tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "ABSOLUTE_HUMIDITY_1":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " ABSOLUTE_HUMIDITY_1";

                            _ChartValue.ListValue = _db.Summaries
                                                    .Where(x => x.tarih >= _StartDate && x.tarih <= _EndDate && x.STATION_ID == _StationId)
                                                    .OrderBy(x => x.tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.ABSOLUTE_HUMIDITY_1, Tarih = x.tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "ABSOLUTE_AIR_PRESSURE_1":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " ABSOLUTE_AIR_PRESSURE_1";

                            _ChartValue.ListValue = _db.Summaries
                                                    .Where(x => x.tarih >= _StartDate && x.tarih <= _EndDate && x.STATION_ID == _StationId)
                                                    .OrderBy(x => x.tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.ABSOLUTE_AIR_PRESSURE_1, Tarih = x.tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "ISINIM_2":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " SLAVE IRRADIATION";

                            _ChartValue.ListValue = _db.Summaries
                                                    .Where(x => x.tarih >= _StartDate && x.tarih <= _EndDate && x.STATION_ID == _StationId)
                                                    .OrderBy(x => x.tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.ISINIM_2, Tarih = x.tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "RUZGARHIZI_2":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " SLAVE WIND SPEED";

                            _ChartValue.ListValue = _db.Summaries
                                                    .Where(x => x.tarih >= _StartDate && x.tarih <= _EndDate && x.STATION_ID == _StationId)
                                                    .OrderBy(x => x.tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.RUZGARHIZI_2, Tarih = x.tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "HUCRESICAKLIGI_2":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " SLAVE CELL TEMP.";

                            _ChartValue.ListValue = _db.Summaries
                                                    .Where(x => x.tarih >= _StartDate && x.tarih <= _EndDate && x.STATION_ID == _StationId)
                                                    .OrderBy(x => x.tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.HUCRESICAKLIGI_2, Tarih = x.tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "SICAKLIK_2":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " SLAVE EXTERNAL TEMP.";

                            _ChartValue.ListValue = _db.Summaries
                                                    .Where(x => x.tarih >= _StartDate && x.tarih <= _EndDate && x.STATION_ID == _StationId)
                                                    .OrderBy(x => x.tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.SICAKLIK_2, Tarih = x.tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "MEAN_WIND_DIRECTION_2":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " SLAVE MEAN WIND DIRECTION";

                            _ChartValue.ListValue = _db.Summaries
                                                    .Where(x => x.tarih >= _StartDate && x.tarih <= _EndDate && x.STATION_ID == _StationId)
                                                    .OrderBy(x => x.tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.PYRANOMETER_2, Tarih = x.tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "AIR_TEMPERATURE_2":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " SLAVE AIR TEMPERATURE";

                            _ChartValue.ListValue = _db.Summaries
                                                    .Where(x => x.tarih >= _StartDate && x.tarih <= _EndDate && x.STATION_ID == _StationId)
                                                    .OrderBy(x => x.tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.AIR_TEMPERATURE_2, Tarih = x.tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "RELATIVE_HUMIDITY_2":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " SLAVE RELATIVE HUMIDITY";

                            _ChartValue.ListValue = _db.Summaries
                                                    .Where(x => x.tarih >= _StartDate && x.tarih <= _EndDate && x.STATION_ID == _StationId)
                                                    .OrderBy(x => x.tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.RELATIVE_HUMIDITY_2, Tarih = x.tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "ABSOLUTE_HUMIDITY_2":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " SLAVE ABSOLUTE HUMIDITY";

                            _ChartValue.ListValue = _db.Summaries
                                                    .Where(x => x.tarih >= _StartDate && x.tarih <= _EndDate && x.STATION_ID == _StationId)
                                                    .OrderBy(x => x.tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.ABSOLUTE_HUMIDITY_2, Tarih = x.tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;

                        case "ABSOLUTE_AIR_PRESSURE_2":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " SLAVE ABSOLUTE AIR PRESSURE";

                            _ChartValue.ListValue = _db.Summaries
                                                    .Where(x => x.tarih >= _StartDate && x.tarih <= _EndDate && x.STATION_ID == _StationId)
                                                    .OrderBy(x => x.tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.ABSOLUTE_AIR_PRESSURE_2, Tarih = x.tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "Cell_Temperature":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " MASTER CELL TEMPERATURE";

                            _ChartValue.ListValue = _db.Summaries
                                                    .Where(x => x.tarih >= _StartDate && x.tarih <= _EndDate && x.STATION_ID == _StationId)
                                                    .OrderBy(x => x.tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.hucreSicakligi, Tarih = x.tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;

                        case "External_Temperature":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " MASTER EXTERNAL TEMPERATURE";

                            _ChartValue.ListValue = _db.Summaries
                                                    .Where(x => x.tarih >= _StartDate && x.tarih <= _EndDate && x.STATION_ID == _StationId)
                                                    .OrderBy(x => x.tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.sicaklik, Tarih = x.tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                        case "Wind_Speed":
                            _ChartValue = new ChartValue();

                            _ChartValue.Name = _Station.NAME + " MASTER WIND SPEED";

                            _ChartValue.ListValue = _db.Summaries
                                                    .Where(x => x.tarih >= _StartDate && x.tarih <= _EndDate && x.STATION_ID == _StationId)
                                                    .OrderBy(x => x.tarih)
                                                    .Select(x => new ChartValueDetail { Value = x.ruzgarHizi, Tarih = x.tarih }).ToList();

                            _result.ListChartValues.Add(_ChartValue);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                _result.ErrorMessage = ex.ToString();
            }

            return Json(_result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveUserChart(List<TreeViewDetail> data, string UserId, string ChartName, string ChartId)
        {
            ChartValuePgeView _result = new ChartValuePgeView();

            try
            {


                int _Id = Convert.ToInt32(ChartId);

                TBL_USER_CHART _UserChart = _db.UserCharts.Find(_Id);

                if (_UserChart == null)
                {
                    _UserChart = new TBL_USER_CHART();
                    _db.UserCharts.Add(_UserChart);
                }

                _UserChart.CHART_NAME = ChartName;
                _UserChart.USER_ID = UserId;
                _UserChart.TYPE = 0;

                _db.SaveChanges();

                List<TBL_USER_CHART_DETAIL> _ChartDetailList = _db.ChartDetails.Where(x => x.CHART_ID == _UserChart.ID).ToList();

                for (int i = 0; i < _ChartDetailList.Count; i++)
                    _db.ChartDetails.Remove(_ChartDetailList[i]);

                _db.SaveChanges();

                if (data != null)
                {
                    for (int i = 0; i < data.Count; i++)
                    {
                        TBL_USER_CHART_DETAIL _ChartDetail = new TBL_USER_CHART_DETAIL();

                        _ChartDetail.INVERTER_ID = Convert.ToInt32(data[i].InverterId);
                        _ChartDetail.STATION_ID = Convert.ToInt32(data[i].StationId);
                        _ChartDetail.VALUE_TYPE = data[i].DataType;
                        _ChartDetail.CHART_ID = Convert.ToInt32(_UserChart.ID);

                        _db.ChartDetails.Add(_ChartDetail);
                    }

                    _db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                _result.ErrorMessage = ex.ToString();
            }

            return Json(_result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteUserChart(string Id)
        {
            string _result = "";

            try
            {
                int _Id = Convert.ToInt32(Id);

                TBL_USER_CHART _UserChart = _db.UserCharts.Find(_Id);

                if (_UserChart != null)
                {
                    _db.UserCharts.Remove(_UserChart);

                    List<TBL_USER_CHART_DETAIL> _ChartDetailList = _db.ChartDetails.Where(x => x.CHART_ID == _UserChart.ID).ToList();

                    for (int i = 0; i < _ChartDetailList.Count; i++)
                        _db.ChartDetails.Remove(_ChartDetailList[i]);
                }

                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                _result = ex.ToString();
            }

            return Json(_result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Class
        public class TreeViewDetail
        {
            public string StationId { get; set; }
            public string InverterId { get; set; }
            public string DataType { get; set; }
        }

        public class ChartValuePgeView
        {
            public List<ChartValue> ListChartValues { get; set; } = new List<ChartValue>();
            public string ErrorMessage { get; set; } = "";
        }

        public class ChartValue
        {
            public string Name { get; set; }
            public List<ChartValueDetail> ListValue { get; set; } = new List<ChartValueDetail>();
        }

        public class ChartValueDetail
        {
            public DateTime? Tarih { get; set; }
            public double? Value { get; set; }
        }
        #endregion
    }
}
