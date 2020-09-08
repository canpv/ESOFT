using AutoMapper;
using Esso.Data;
using Esso.Models;
using Esso.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Esso.Web.Models.HES_MODEL;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;

namespace Esso.Web.Controllers
{
    public class HESController : BaseController
    {
        EssoEntities DB = new EssoEntities();
        // GET: HES
        public ActionResult Index(int stationId)
        {
            return View(stationId);
        }

        public ActionResult chart(int stationId)
        {
            return View(stationId);
        }

        public long convertDateUTC(DateTime dt)
        {
            //1553472070000
            //1553472070
            var timeSpan = (dt - new DateTime(1970, 1, 1, 0, 0, 0, 0, 0));
            return (long)timeSpan.TotalSeconds*1000;
        }

        public JsonResult GetLineChart2(string beginDate, int stationId)
        {
            //var timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
            //var sds=(long)timeSpan.TotalSeconds;

            Thread.CurrentThread.CurrentCulture = new CultureInfo("tr-TR");
            DateTime reqDateParam = DateTime.Parse(@beginDate);
            OZET_HES_DTO_ oDTO = new OZET_HES_DTO_();
            var stDetail = DB.Stations.Where(a => a.ID == stationId).FirstOrDefault();

            float? scale;
            if (stDetail.IRRADIATION_SCALE != null)
            {
                scale = ((stDetail.AC_INSTALLED_POWER) + ((stDetail.AC_INSTALLED_POWER) * (stDetail.IRRADIATION_SCALE) / 100));

                if (stDetail.STATION_TYPE==5)
                {
                    scale = scale / 1000;
                }
            }
            else
            {
                scale = null;
            }

            float? acInstalled = stDetail.AC_INSTALLED_POWER;



            var ozetler = DB.Summaries.Where(p => p.STATION_ID == stationId
            && p.tarih.Year == reqDateParam.Year && p.tarih.Month == reqDateParam.Month && p.tarih.Day == reqDateParam.Day)
                .OrderBy(a => a.tarih).Select(a =>
                 new TBL_OZET_DTO_HES
                 {
                     _id = a.Id,
                     _tarih = a.tarih,
                     _gunlukUretim = stDetail.STATION_TYPE==5 ? (float)Math.Round((double)Math.Abs(a.gunlukUretim.Value), 1) : (float)Math.Round((double)Math.Abs(a.gunlukUretim.Value/1000), 1),
                     _powerAC = a.gunlukUretim == null ? 0 : (float)Math.Round((double)Math.Abs(a.gunlukUretim.Value), 1),
                     _producedEnergy = a.Enerji == null ? 0f : (float)Math.Round((double)a.Enerji / 1000000, 2),
                     _consumedEnergy = a.H2_WP_plus == null ? 0f : (float)Math.Round((double)a.H2_WP_plus, 2),
                     _debi = a.isinim == null ? 0f : (float)Math.Round((double)a.isinim, 2),
                     _isinim = (float)Math.Round((double)a.isinim, 1),
                 }
                ).ToList();

            if (ozetler.Count > 0)
            {
                DateTime abFirstDate = reqDateParam;
                DateTime abLastDate = reqDateParam.AddHours(23).AddMinutes(59).AddSeconds(59);
                if (ozetler != null && ozetler.Count > 0)
                {
                    DateTime lastDate = ozetler[ozetler.Count - 1]._tarih;
                    DateTime FirsDate = ozetler[0]._tarih;

                    while (abFirstDate < FirsDate)
                    {
                        TBL_OZET_DTO_HES ozet = new TBL_OZET_DTO_HES();
                        ozet._tarih = abFirstDate;
                        ozetler.Add(ozet);
                        abFirstDate = abFirstDate.AddMinutes(5);
                    }

                    while (abLastDate > lastDate)
                    {
                        lastDate = lastDate.AddMinutes(5);
                        TBL_OZET_DTO_HES ozet = new TBL_OZET_DTO_HES();
                        ozet._tarih = lastDate;
                        ozetler.Add(ozet);
                    }
                    ozetler = ozetler.OrderBy(x => x._tarih).ToList();
                }


                for (int i = 0; i < ozetler.Count(); i++)
                {
                    ozetler[i]._max = stDetail.AC_INSTALLED_POWER;
                    ozetler[i].DateUTC = convertDateUTC(ozetler[i]._tarih);
                    //if ((ozetler[i]._dcGuc <= 0 && ozetler[i]._isinim <= 1) || (ozetler[i]._isinim == null && ozetler[i]._dcGuc == null))
                    //{
                    //    ozetler[i]._enerji = null;
                    //}

                }



                oDTO._ozet = ozetler;
                oDTO._irradiationScale = scale;
                oDTO._acInstalledPower = acInstalled;
            }
            return Json(oDTO, JsonRequestBehavior.AllowGet);
        }


        //public class JSON_ARRAY
        //{
        //    public string name { get; set; }
        //    public List<object> data { get; set; }
        //}


        public JsonResult GetLineChart3(string beginDate, int stationId)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("tr-TR");
            DateTime reqDateParam = DateTime.Parse(@beginDate);
            OZET_HES_DTO_ oDTO = new OZET_HES_DTO_();
            var stDetail = DB.Stations.Where(a => a.ID == stationId).FirstOrDefault();

            float? scale;
            if (stDetail.IRRADIATION_SCALE != null)
            {
                scale = ((stDetail.AC_INSTALLED_POWER) + ((stDetail.AC_INSTALLED_POWER) * (stDetail.IRRADIATION_SCALE) / 100));

                if (stDetail.STATION_TYPE == 5)
                {
                    scale = scale / 1000;
                }
            }
            else
            {
                scale = null;
            }

            float? acInstalled = stDetail.AC_INSTALLED_POWER;

            

            var ozetler = DB.Summaries.Where(p => p.STATION_ID == stationId
            && p.tarih.Year == reqDateParam.Year && p.tarih.Month == reqDateParam.Month && p.tarih.Day == reqDateParam.Day)
                .OrderBy(a => a.tarih).Select(a =>
                 new TBL_OZET_DTO_HES
                 {
                     _id = a.Id,
                     _tarih = a.tarih,
                     _gunlukUretim = stDetail.STATION_TYPE == 5 ? (float)Math.Round((double)Math.Abs(a.gunlukUretim.Value), 1) : (float)Math.Round((double)Math.Abs(a.gunlukUretim.Value / 1000), 1),
                     _powerAC = a.gunlukUretim == null ? 0 : (float)Math.Round((double)Math.Abs(a.gunlukUretim.Value), 1),
                     _producedEnergy = a.Enerji == null ? 0f : (float)Math.Round((double)a.Enerji / 1000000, 2),
                     _consumedEnergy = a.H2_WP_plus == null ? 0f : (float)Math.Round((double)a.H2_WP_plus, 2),
                     _debi = a.isinim == null ? 0f : (float)Math.Round((double)a.isinim, 2),
                     _isinim = (float)Math.Round((double)a.isinim, 1),
                 }
                ).ToList();


         
          

            for (int i = 0; i < ozetler.Count(); i++)
            {
                ozetler[i].DateUTC = convertDateUTC(ozetler[i]._tarih);

                ////J1[0].data.Add()
                //long[] AA = new long[1];
                ////long longDate = convertDateUTC(ozetler[i]._tarih);
                //AA[0] = 185544;
                //AA[1] = 123;
                //var ssss= dataArr;
            }


           






            if (ozetler.Count > 0)
            {
                DateTime abFirstDate = reqDateParam;
                DateTime abLastDate = reqDateParam.AddHours(23).AddMinutes(59).AddSeconds(59);
                if (ozetler != null && ozetler.Count > 0)
                {
                    DateTime lastDate = ozetler[ozetler.Count - 1]._tarih;
                    DateTime FirsDate = ozetler[0]._tarih;

                    while (abFirstDate < FirsDate)
                    {
                        TBL_OZET_DTO_HES ozet = new TBL_OZET_DTO_HES();
                        ozet._tarih = abFirstDate;
                        ozetler.Add(ozet);
                        abFirstDate = abFirstDate.AddMinutes(5);
                    }

                    while (abLastDate > lastDate)
                    {
                        lastDate = lastDate.AddMinutes(5);
                        TBL_OZET_DTO_HES ozet = new TBL_OZET_DTO_HES();
                        ozet._tarih = lastDate;
                        ozetler.Add(ozet);
                    }
                    ozetler = ozetler.OrderBy(x => x._tarih).ToList();
                }


                for (int i = 0; i < ozetler.Count(); i++)
                {
                    ozetler[i]._max = stDetail.AC_INSTALLED_POWER;
                    ozetler[i].DateUTC = convertDateUTC(ozetler[i]._tarih);
                }

                oDTO._ozet = ozetler;
                oDTO._irradiationScale = scale;
                oDTO._acInstalledPower = acInstalled;
            }


            List<JSON_ARRAY> dataArr = new List<JSON_ARRAY>();
            try
            {
                List<object> listDataArr = new List<object>();
                List<object> listDataArr2 = new List<object>();
                foreach (var item in ozetler)
                {
                    Dictionary<long, float> dictionary = new Dictionary<long, float>();
                    dictionary[item.DateUTC] = item._gunlukUretim == null ? 0 : item._gunlukUretim.Value;
                    listDataArr.Add(dictionary);

                    Dictionary<long, float> dictionary2 = new Dictionary<long, float>();
                    dictionary2[item.DateUTC] = item._isinim == null ? 0 : item._isinim.Value;
                    listDataArr2.Add(dictionary2);
                }
                dataArr.Add(new JSON_ARRAY { name = "AC", data = listDataArr });
                dataArr.Add(new JSON_ARRAY { name = "Irradiation", data = listDataArr2 });
            }
            catch (Exception ex)
            {

            }
            oDTO._AA = dataArr;

            return Json(oDTO, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Report(int? id)
        {

            string curUserId = User.Identity.GetUserId();

            string _role =
                            DB.Database.SqlQuery<string>("select \"AspNetRoles\".\"Name\" from \"AspNetUserRoles\" left join \"AspNetRoles\" on \"AspNetUserRoles\".\"RoleId\" = \"AspNetRoles\".\"Id\" where \"AspNetUserRoles\".\"UserId\" = '" + curUserId + "'")
                            .FirstOrDefault();

            List<TBL_COMPANY> ListCompany;
            List<TBL_STATION> ListStation;
            List<TBL_INVERTER> ListInverter;
            List<TBL_STATION_STRING> ListString;
            switch (_role)
            {
                case "M_ADMIN":
                    //ListCompany = DB.Companies.Where(x => x.IS_DELETED == false).OrderBy(x => x.NAME).ToList();
                    //ListStation = DB.Stations.Where(x => x.IS_DELETED == false && x.STATION_TYPE == 5).OrderBy(x => x.NAME).ToList();
                    //ListInverter = DB.Inverters.Where(x => x.IS_DELETED == false).ToList();
                    //ListString = (from inv in DB.stationString
                    //              join station in DB.Stations on inv.STATION_ID equals station.ID
                    //              where station.IS_DELETED == false && inv.IS_DELETED == false
                    //              select inv).ToList();
                    int[] StatIdsA = DB.Stations.Where(x => x.IS_DELETED == false && x.STATION_TYPE == 5).Select(a => a.ID).ToArray();
                    int[] compIdsA = DB.Stations.Where(a => a.IS_DELETED == false && StatIdsA.Contains(a.ID)).Select(a => a.COMPANY_ID).ToArray();
                    ListCompany = DB.Companies.Where(a => a.IS_DELETED == false && compIdsA.Contains(a.ID)).OrderBy(x => x.NAME).ToList();

                    ListStation = (from station in DB.Stations
                                   where station.STATION_TYPE == 5
                                   select station).OrderBy(x => x.NAME).ToList();
                    ListInverter = null;
                    ListString = null;
                    //ListInverter = (from inv in DB.Inverters
                    //                join station in DB.Stations on inv.STATION_ID equals station.ID
                    //                join user_station in DB.StationUsers on station.ID equals user_station.STATION_ID
                    //                where user_station.USER_ID == curUserId && user_station.IS_DELETED == false && station.IS_DELETED == false && inv.IS_DELETED == false
                    //                select inv).ToList();
                    //ListString = (from inv in DB.stationString
                    //              join station in DB.Stations on inv.STATION_ID equals station.ID
                    //              where station.IS_DELETED == false && inv.IS_DELETED == false
                    //              select inv).ToList();
                    break;
                case "COMP_USER":

                    int[] UserStatIds = DB.StationUsers.Where(a => a.USER_ID == curUserId).Select(a => a.STATION_ID).ToArray();
                    int[] compIds = DB.Stations.Where(a => a.IS_DELETED == false && UserStatIds.Contains(a.ID)).Select(a => a.COMPANY_ID).ToArray();
                    ListCompany = DB.Companies.Where(a => a.IS_DELETED == false && compIds.Contains(a.ID)).OrderBy(x => x.NAME).ToList();

                    ListStation = (from station in DB.Stations
                                   join user_station in DB.StationUsers on station.ID equals user_station.STATION_ID
                                   where user_station.USER_ID == curUserId && user_station.IS_DELETED == false && station.STATION_TYPE != 4
                                   select station).OrderBy(x => x.NAME).ToList();

                    ListInverter = (from inv in DB.Inverters
                                    join station in DB.Stations on inv.STATION_ID equals station.ID
                                    join user_station in DB.StationUsers on station.ID equals user_station.STATION_ID
                                    where user_station.USER_ID == curUserId && user_station.IS_DELETED == false && station.IS_DELETED == false && inv.IS_DELETED == false
                                    select inv).ToList();
                    ListString = (from inv in DB.stationString
                                  join station in DB.Stations on inv.STATION_ID equals station.ID
                                  where station.IS_DELETED == false && inv.IS_DELETED == false
                                  select inv).ToList();
                    break;

                case "COMP_ADMIN":
                    ListCompany = (from user_company in DB.CompanyUsers
                                   join company in DB.Companies on user_company.COMPANY_ID equals company.ID
                                   where user_company.USER_ID == curUserId && user_company.IS_DELETED == false
                                   select company).OrderBy(x => x.NAME).ToList();

                    ListStation = (from station in DB.Stations
                                   join company in DB.Companies on station.COMPANY_ID equals company.ID
                                   join user_company in DB.CompanyUsers on company.ID equals user_company.COMPANY_ID
                                   where user_company.USER_ID == curUserId && user_company.IS_DELETED == false && station.STATION_TYPE != 4
                                   select station).OrderBy(x => x.NAME).ToList();

                    ListInverter = (from inv in DB.Inverters
                                    join station in DB.Stations on inv.STATION_ID equals station.ID
                                    join company in DB.Companies on station.COMPANY_ID equals company.ID
                                    join user_company in DB.CompanyUsers on company.ID equals user_company.COMPANY_ID
                                    where user_company.USER_ID == curUserId && user_company.IS_DELETED == false && company.IS_DELETED == false && station.IS_DELETED == false && inv.IS_DELETED == false
                                    select inv).ToList();
                    ListString = (from inv in DB.stationString
                                  join station in DB.Stations on inv.STATION_ID equals station.ID
                                  where station.IS_DELETED == false && inv.IS_DELETED == false
                                  select inv).ToList();
                    break;
                case "DEMO":
                    int[] UserDemoStatIds = DB.StationUsers.Where(a => a.USER_ID == curUserId).Select(a => a.STATION_ID).ToArray();
                    int[] compDemoIds = DB.Stations.Where(a => a.IS_DELETED == false && UserDemoStatIds.Contains(a.ID)).Select(a => a.COMPANY_ID).ToArray();
                    ListCompany = DB.Companies.Where(a => a.IS_DELETED == false && compDemoIds.Contains(a.ID)).OrderBy(x => x.NAME).ToList();

                    ListStation = (from station in DB.Stations
                                   join user_station in DB.StationUsers on station.ID equals user_station.STATION_ID
                                   where user_station.USER_ID == curUserId && user_station.IS_DELETED == false && station.STATION_TYPE != 4
                                   select station).OrderBy(x => x.NAME).ToList();

                    ListInverter = (from inv in DB.Inverters
                                    join station in DB.Stations on inv.STATION_ID equals station.ID
                                    join user_station in DB.StationUsers on station.ID equals user_station.STATION_ID
                                    where user_station.USER_ID == curUserId && user_station.IS_DELETED == false && station.IS_DELETED == false && inv.IS_DELETED == false
                                    select inv).ToList();
                    ListString = (from inv in DB.stationString
                                  join station in DB.Stations on inv.STATION_ID equals station.ID
                                  where station.IS_DELETED == false && inv.IS_DELETED == false
                                  select inv).ToList();
                    break;
                default:
                    ListCompany = new List<TBL_COMPANY>();
                    ListStation = new List<TBL_STATION>();
                    ListInverter = new List<TBL_INVERTER>();
                    ListString = new List<TBL_STATION_STRING>();
                    break;
            }

            ViewBag.ListCompany = ListCompany;
            ViewBag.ListStation = ListStation;
            ViewBag.ListInverter = ListInverter;
            ViewBag.UserId = curUserId;
            ViewBag.ListString = ListString;
            int _Id = 0;
            string ChartName = "";

            if (id != null)
                _Id = Convert.ToInt32(id);

            TBL_USER_CHART _UserChart = DB.UserCharts.Find(_Id);

            List<TBL_USER_CHART_DETAIL> ListChartDetail;

            if (_UserChart != null)
            {
                ChartName = _UserChart.CHART_NAME;

                ListChartDetail = DB.ChartDetails.Where(x => x.CHART_ID == _UserChart.ID).ToList();
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
      
        public class TblOzet
        {
            public System.DateTime tarih { get; set; }
            public Nullable<float> Enerji { get; set; }
            public Nullable<float> aylikEnerji { get; set; }
            public Nullable<float> yillikEnerji { get; set; }
            public Nullable<float> toplamEnerji { get; set; }
            public Nullable<float> isinim { get; set; }
            public Nullable<float> gunlukUretim { get; set; }
            public Nullable<float> H2_Vbn { get; set; }
            public Nullable<float> H2_Van { get; set; }
            public Nullable<float> H2_Vcn { get; set; }
            public Nullable<float> H2_Vab { get; set; }
            public Nullable<float> H2_Vbc { get; set; }
            public Nullable<float> H2_Vac { get; set; }
            public Nullable<float> H2_F { get; set; }
            public Nullable<float> H2_Q { get; set; }
            public Nullable<float> H2_P { get; set; }
            public Nullable<float> H2_Ic { get; set; }
            public Nullable<float> H2_Ib { get; set; }
            public Nullable<float> H2_Ia { get; set; }
            public Nullable<float> H2_S { get; set; }
            public Nullable<float> H2_PF { get; set; }
            public Nullable<float> H2_WP_minus { get; set; }
            public Nullable<float> H2_WP_plus { get; set; }
            public Nullable<float> Dc_Guc { get; set; }
            public Nullable<double> sicaklik { get; set; }
            public Nullable<float> hucreSicakligi { get; set; }
            public Nullable<float> ruzgarHizi { get; set; }
            public Nullable<float> PYRANOMETER { get; set; }

            public Nullable<float> MEAN_WIND_DIRECTION_1 { get; set; }
            public Nullable<float> AIR_TEMPERATURE_1 { get; set; }
            public Nullable<float> RELATIVE_HUMIDITY_1 { get; set; }
            public Nullable<float> ABSOLUTE_HUMIDITY_1 { get; set; }
            public Nullable<float> ABSOLUTE_AIR_PRESSURE_1 { get; set; }
            public Nullable<float> ISINIM_2 { get; set; }
            public Nullable<float> RUZGARHIZI_2 { get; set; }
            public Nullable<float> HUCRESICAKLIGI_2 { get; set; }
            public Nullable<float> SICAKLIK_2 { get; set; }
            public Nullable<float> PYRANOMETER_2 { get; set; }
            public Nullable<float> MEAN_WIND_DIRECTION_2 { get; set; }
            public Nullable<float> AIR_TEMPERATURE_2 { get; set; }
            public Nullable<float> RELATIVE_HUMIDITY_2 { get; set; }
            public Nullable<float> ABSOLUTE_HUMIDITY_2 { get; set; }
            public Nullable<float> ABSOLUTE_AIR_PRESSURE_2 { get; set; }

        }
        public class TblInvOzet
        {

            public Nullable<System.DateTime> Tarih { get; set; }
            public int Inv_Id { get; set; }
            public int STATION_ID { get; set; }
            public string NAME { get; set; }
            public Nullable<float> Akim_AC { get; set; }
            public Nullable<float> Akim_DC { get; set; }
            public Nullable<float> Gerilim_AC { get; set; }
            public Nullable<float> Gerilim_DC { get; set; }
            public Nullable<float> Guc_AC { get; set; }
            public Nullable<float> Guc_DC { get; set; }
            public Nullable<float> Gunluk_Enerji { get; set; }

            public Nullable<float> HEARTBEAT { get; set; }
            public Nullable<float> INVERTER_MAIN_STATUS { get; set; }
            public Nullable<float> REACTIVE_POWER { get; set; }
            public Nullable<float> GRID_VOLTAGE_VRMS { get; set; }
            public Nullable<float> GRID_FREQUENCY { get; set; }
            public Nullable<float> POWER_FACTOR { get; set; }
            public Nullable<float> CODE_OF_THE_ACTIVE_FAULT { get; set; }
            public Nullable<float> GRID_CURRENT { get; set; }
            public Nullable<float> DC_BUS_VOLTAGE { get; set; }
            public Nullable<float> GROUNDING_CURRENT { get; set; }
            public Nullable<float> SOLATION_RESISTANCE { get; set; }
            public Nullable<float> AMBIENT_TEMPERATURE { get; set; }
            public Nullable<float> HIGHEST_IGBT_TEMPERATURE_PU1 { get; set; }
            public Nullable<float> HIGHEST_IGBT_TEMPERATURE_PU2 { get; set; }
            public Nullable<float> HIGHEST_IGBT_TEMPERATURE_PU3 { get; set; }
            public Nullable<float> HIGHEST_IGBT_TEMPERATURE_PU4 { get; set; }
            public Nullable<float> CONTROL_SECTION_TEMPERATURE { get; set; }
            public Nullable<float> DAILY_KVAH_SUPPLIED { get; set; }
            public Nullable<float> TOTAL_KVAH_SUPPLIED { get; set; }
            public Nullable<float> IGBT_1_T1 { get; set; }
            public Nullable<float> IGBT_1_T2 { get; set; }
            public Nullable<float> IGBT_1_T3 { get; set; }
            public Nullable<float> IGBT_2_T1 { get; set; }
            public Nullable<float> IGBT_2_T2 { get; set; }
            public Nullable<float> IGBT_2_T3 { get; set; }
            public Nullable<float> IGBT_3_T1 { get; set; }
            public Nullable<float> IGBT_3_T2 { get; set; }
            public Nullable<float> IGBT_3_T3 { get; set; }
            public Nullable<float> IGBT_4_T1 { get; set; }
            public Nullable<float> IGBT_4_T2 { get; set; }
            public Nullable<float> IGBT_4_T3 { get; set; }

        }
        #region AJAX
        [HttpPost]
        public JsonResult GetTable(List<TreeViewDetail> data)
        {
            ReportTablePageView _result = new ReportTablePageView();

            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("tr-TR");

                DateTime _StartDate = Convert.ToDateTime(DateTime.Now.ToShortDateString());
                DateTime _EndDate = Convert.ToDateTime(DateTime.Now.ToShortDateString() + " 23:59:59");

                List<GridViewFieldItem> _MasterFieldList = new List<GridViewFieldItem>();
                List<GridViewFieldItem> _DetailFieldList = new List<GridViewFieldItem>();

                if (data == null) data = new List<TreeViewDetail>();

                string _SqlInvOzet = "";
                string _SqlTblOzet = "";
                string _InvNo = "";
                string _SqlTblStrOzet = "";

                for (int i = 0; i < data.Count; i++)
                {
                    int _StationId = Convert.ToInt32(data[i].StationId);
                    int _InverterId = Convert.ToInt32(data[i].InverterId);
                    string _DataType = data[i].DataType;


                    GridViewFieldItem _cFieldItem = new GridViewFieldItem();

                    _cFieldItem.FieldName = GetColumnName(_DataType);
                    _cFieldItem.DisplayName = _DataType.ToUpper().Replace('_', ' ');

                    if (_InverterId == 0)
                    {
                        _SqlTblOzet += "\"" + _cFieldItem.FieldName + "\",";

                        _MasterFieldList.Add(_cFieldItem);
                    }
                    else if (data[i].DataType == "STR_Value")
                    {
                        _cFieldItem.FieldName = "STR";
                        _cFieldItem.DisplayName = "str";
                        _DetailFieldList.Add(_cFieldItem);
                    }
                    else
                    {
                        if (_SqlInvOzet.IndexOf(_cFieldItem.FieldName) == -1)
                        {
                            _SqlInvOzet += "TBL_INV_OZET.\"" + _cFieldItem.FieldName + "\",";

                            _DetailFieldList.Add(_cFieldItem);
                        }

                        if (_InvNo.IndexOf(_InverterId.ToString()) == -1)
                        {
                            _InvNo += _InverterId + ",";
                        }

                    }
                }
                if (_InvNo == "")
                {
                    _InvNo = "0";
                }
                else
                {
                    _InvNo = _InvNo.Remove(_InvNo.Length - 1);
                }


                if (data.Count > 0)
                {
                    _SqlInvOzet += "TBL_INV_OZET.\"Tarih\",TBL_INVERTER.NAME";
                    _SqlTblOzet += "\"tarih\"";
                    _SqlTblStrOzet += "";
                    _SqlInvOzet = "SELECT " + _SqlInvOzet + " FROM TBL_INV_OZET LEFT JOIN TBL_INVERTER ON TBL_INV_OZET.\"Inv_Id\" = TBL_INVERTER.ID WHERE \"Tarih\" >= TO_DATE('" + _StartDate + "', 'DD/MM/YY HH24:MI:SS') AND \"Tarih\" <= TO_DATE('" + _EndDate + "', 'DD/MM/YY HH24:MI:SS') AND \"Inv_Id\" IN (" + _InvNo + ") ORDER BY TBL_INV_OZET.\"Tarih\" DESC ";
                    _SqlTblOzet = "SELECT " + _SqlTblOzet + " FROM TBL_OZET WHERE \"tarih\" >= TO_DATE('" + _StartDate + "', 'DD/MM/YY HH24:MI:SS') AND \"tarih\" <= TO_DATE('" + _EndDate + "', 'DD/MM/YY HH24:MI:SS') AND STATION_ID =" + data[0].StationId + " ORDER BY TBL_OZET.\"tarih\" DESC ";

                    _SqlTblStrOzet = "select * from TBL_STR_OZET_AVG where \"STATION_ID\"=103 AND \"TARIH_NUMBER\">2019010120 AND \"TARIH_NUMBER\"<2019010122";


                    _result._SqlDetail = _SqlInvOzet;
                    _result._SqlMaster = _SqlTblOzet;

                    _result._MasterFieldList = _MasterFieldList;
                    _result._DetailFieldList = _DetailFieldList;

                    List<TblOzet> _ListTblOzet = DB.Database.SqlQuery<TblOzet>(_SqlTblOzet).ToList();
                    List<TblInvOzet> _ListInvOzet = DB.Database.SqlQuery<TblInvOzet>(_SqlInvOzet).ToList();
                    //List<TBL_STR_OZET_AVG> _ListStrOzet = DB.Database.SqlQuery<TBL_STR_OZET_AVG>(_SqlTblStrOzet).ToList();

                    for (int i = 0; i < _ListTblOzet.Count; i++)
                    {
                        try
                        {
                            List<string> _ListObject = new List<string>();

                            _ListObject.Add(_ListTblOzet[i].tarih.ToString());

                            for (int j = 0; j < _MasterFieldList.Count; j++)
                            {
                                _ListObject.Add(GetPropValue(_ListTblOzet[i], _MasterFieldList[j].FieldName).ToString());
                            }

                            _result._ListTblOzet.Add(_ListObject);
                        }
                        catch (Exception EX)
                        {

                            throw;
                        }

                    }
                    for (int i = 0; i < _ListInvOzet.Count; i++)
                    {
                        List<string> _ListObject = new List<string>();

                        _ListObject.Add(_ListInvOzet[i].Tarih.ToString());
                        _ListObject.Add(_ListInvOzet[i].NAME);

                        for (int j = 0; j < _DetailFieldList.Count; j++)
                        {
                            _ListObject.Add(GetPropValue(_ListInvOzet[i], _DetailFieldList[j].FieldName).ToString());
                        }

                        _result._ListInvOzet.Add(_ListObject);
                    }


                }
            }
            catch (Exception ex)
            {
                _result.ErrorMessage = ex.ToString();
            }

            return Json(_result, JsonRequestBehavior.AllowGet);
        }

        public object GetPropValue(object src, string propName)
        {
            object _result = src.GetType().GetProperty(propName).GetValue(src, null);

            if (_result == null)
                return 0;
            else
                return _result;
        }

        private string GetColumnName(string DataType)
        {
            string _result = string.Empty;

            switch (DataType)
            {
                case "STR_Value":
                    _result = "Value";
                    break;
                case "Frequency":
                    _result = "H2_F";
                    break;
                case "H2_WP_minus":
                    _result = "H2_WP_minus";
                    break;
                case "H2_WP_plus":
                    _result = "H2_WP_plus";
                    break;
                case "H2_WQ_minus":
                    _result = "H2_WQ_minus";
                    break;
                case "H2_WQ_plus":
                    _result = "H2_WQ_plus";
                    break;
                case "Current_L1":
                    _result = "H2_Ia";
                    break;
                case "Current_L2":
                    _result = "H2_Ib";
                    break;
                case "Current_L3":
                    _result = "H2_Ic";
                    break;
                case "Active_Power_Sum":
                    _result = "H2_P";
                    break;
                case "Power_Factor":
                    _result = "H2_PF";
                    break;
                case "Reactive_Power_Sum":
                    _result = "H2_Q";
                    break;
                case "Visible_Power_Sum":
                    _result = "H2_S";
                    break;
                case "Voltage_L1-L2":
                    _result = "H2_Vab";
                    break;
                case "Voltage_L1-L3":
                    _result = "H2_Vac";
                    break;
                case "Voltage_L2-L3":
                    _result = "H2_Vbc";
                    break;
                case "Daily_Production":
                    _result = "gunlukUretim";
                    break;
                case "Irradiation":
                    _result = "isinim";
                    break;
                case "Total_Energy":
                    _result = "Enerji";
                    break;
                //case "Total_Power_AC":
                //	_result = "gunlukUretim";
                //	break;
                case "Total_Power_DC":
                    _result = "Dc_Guc";
                    break;
                case "Pyranometer":
                    _result = "PYRANOMETER";
                    break;
                case "External_Temperature":
                    _result = "sicaklik";
                    break;
                case "Cell_Temperature":
                    _result = "hucreSicakligi";
                    break;
                case "Wind_Speed":
                    _result = "ruzgarHizi";
                    break;//TBL_OZET SON
                case "Current_AC":
                    _result = "Akim_AC";
                    break;
                case "Current_DC":
                    _result = "Akim_DC";
                    break;
                case "Voltage_AC":
                    _result = "Gerilim_AC";
                    break;
                case "Voltage_DC":
                    _result = "Gerilim_DC";
                    break;
                case "Power_AC":
                    _result = "Guc_AC";
                    break;
                case "Power_DC":
                    _result = "Guc_DC";
                    break;
                case "Production":
                    _result = "Gunluk_Enerji";
                    break;
                //Inverter Values
                case "HEARTBEAT":
                    _result = "HEARTBEAT";
                    break;
                case "INVERTER_MAIN_STATUS":
                    _result = "INVERTER_MAIN_STATUS";
                    break;
                case "REACTIVE_POWER":
                    _result = "REACTIVE_POWER";
                    break;
                case "GRID_VOLTAGE_VRMS":
                    _result = "GRID_VOLTAGE_VRMS";
                    break;
                case "GRID_FREQUENCY":
                    _result = "GRID_FREQUENCY";
                    break;
                case "POWER_FACTOR":
                    _result = "POWER_FACTOR";
                    break;
                case "CODE_OF_THE_ACTIVE_FAULT":
                    _result = "CODE_OF_THE_ACTIVE_FAULT";
                    break;
                case "GRID_CURRENT":
                    _result = "GRID_CURRENT";
                    break;
                case "DC_BUS_VOLTAGE":
                    _result = "DC_BUS_VOLTAGE";
                    break;
                case "GROUNDING_CURRENT":
                    _result = "GROUNDING_CURRENT";
                    break;
                case "SOLATION_RESISTANCE":
                    _result = "SOLATION_RESISTANCE";
                    break;
                case "AMBIENT_TEMPERATURE":
                    _result = "AMBIENT_TEMPERATURE";
                    break;
                case "HIGHEST_IGBT_TEMPERATURE_PU1":
                    _result = "HIGHEST_IGBT_TEMPERATURE_PU1";
                    break;
                case "HIGHEST_IGBT_TEMPERATURE_PU2":
                    _result = "HIGHEST_IGBT_TEMPERATURE_PU2";
                    break;
                case "HIGHEST_IGBT_TEMPERATURE_PU3":
                    _result = "HIGHEST_IGBT_TEMPERATURE_PU3";
                    break;
                case "HIGHEST_IGBT_TEMPERATURE_PU4":
                    _result = "HIGHEST_IGBT_TEMPERATURE_PU4";
                    break;
                case "CONTROL_SECTION_TEMPERATURE":
                    _result = "CONTROL_SECTION_TEMPERATURE";
                    break;
                case "DAILY_KVAH_SUPPLIED":
                    _result = "DAILY_KVAH_SUPPLIED";
                    break;
                case "TOTAL_KVAH_SUPPLIED":
                    _result = "TOTAL_KVAH_SUPPLIED";
                    break;
                case "IGBT_1_T1":
                    _result = "IGBT_1_T1";
                    break;
                case "IGBT_1_T2":
                    _result = "IGBT_1_T2";
                    break;
                case "IGBT_1_T3":
                    _result = "IGBT_1_T3";
                    break;
                case "IGBT_2_T1":
                    _result = "IGBT_2_T1";
                    break;
                case "IGBT_2_T2":
                    _result = "IGBT_2_T2";
                    break;
                case "IGBT_2_T3":
                    _result = "IGBT_2_T3";
                    break;
                case "IGBT_3_T1":
                    _result = "IGBT_3_T1";
                    break;
                case "IGBT_3_T2":
                    _result = "IGBT_3_T2";
                    break;
                case "IGBT_3_T3":
                    _result = "IGBT_3_T3";
                    break;
                case "IGBT_4_T1":
                    _result = "IGBT_4_T1";
                    break;
                case "IGBT_4_T2":
                    _result = "IGBT_4_T2";
                    break;
                case "IGBT_4_T3":
                    _result = "IGBT_4_T3";
                    break;
                //Meteoroloji 2
                case "MEAN_WIND_DIRECTION_1":
                    _result = "MEAN_WIND_DIRECTION_1";
                    break;
                case "AIR_TEMPERATURE_1":
                    _result = "AIR_TEMPERATURE_1";
                    break;
                case "RELATIVE_HUMIDITY_1":
                    _result = "RELATIVE_HUMIDITY_1";
                    break;
                case "ABSOLUTE_HUMIDITY_1":
                    _result = "ABSOLUTE_HUMIDITY_1";
                    break;
                case "ABSOLUTE_AIR_PRESSURE_1":
                    _result = "ABSOLUTE_AIR_PRESSURE_1";
                    break;
                case "MEAN_WIND_DIRECTION_2":
                    _result = "MEAN_WIND_DIRECTION_2";
                    break;
                case "AIR_TEMPERATURE_2":
                    _result = "AIR_TEMPERATURE_2";
                    break;
                case "RELATIVE_HUMIDITY_2":
                    _result = "RELATIVE_HUMIDITY_2";
                    break;
                case "ABSOLUTE_HUMIDITY_2":
                    _result = "ABSOLUTE_HUMIDITY_2";
                    break;
                case "ABSOLUTE_AIR_PRESSURE_2":
                    _result = "ABSOLUTE_AIR_PRESSURE_2";
                    break;
                case "HUCRESICAKLIGI_2":
                    _result = "HUCRESICAKLIGI_2";
                    break;
                case "SICAKLIK_2":
                    _result = "SICAKLIK_2";
                    break;
                case "ISINIM_2":
                    _result = "ISINIM_2";
                    break;
                case "RUZGARHIZI_2":
                    _result = "RUZGARHIZI_2";
                    break;
                case "PYRANOMETER_2":
                    _result = "PYRANOMETER_2";
                    break;
            }

            return _result;
        }

        [HttpPost]
        public JsonResult SaveUserChart(List<TreeViewDetail> data, string UserId, string ChartName, string ChartId)
        {
            ReportTablePageView _result = new ReportTablePageView();

            try
            {
                int _Id = Convert.ToInt32(ChartId);

                TBL_USER_CHART _UserChart = DB.UserCharts.Find(_Id);

                if (_UserChart == null)
                {
                    _UserChart = new TBL_USER_CHART();
                    DB.UserCharts.Add(_UserChart);
                }

                _UserChart.CHART_NAME = ChartName;
                _UserChart.USER_ID = UserId;
                _UserChart.TYPE = 1;

                DB.SaveChanges();

                List<TBL_USER_CHART_DETAIL> _ChartDetailList = DB.ChartDetails.Where(x => x.CHART_ID == _UserChart.ID).ToList();

                for (int i = 0; i < _ChartDetailList.Count; i++)
                    DB.ChartDetails.Remove(_ChartDetailList[i]);

                DB.SaveChanges();

                if (data != null)
                {
                    for (int i = 0; i < data.Count; i++)
                    {
                        TBL_USER_CHART_DETAIL _ChartDetail = new TBL_USER_CHART_DETAIL();

                        _ChartDetail.INVERTER_ID = Convert.ToInt32(data[i].InverterId);
                        _ChartDetail.STATION_ID = Convert.ToInt32(data[i].StationId);
                        _ChartDetail.VALUE_TYPE = data[i].DataType;
                        _ChartDetail.CHART_ID = Convert.ToInt32(_UserChart.ID);

                        DB.ChartDetails.Add(_ChartDetail);
                    }

                    DB.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                //_result.ErrorMessage = ex.ToString();
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

                TBL_USER_CHART _UserChart = DB.UserCharts.Find(_Id);

                if (_UserChart != null)
                {
                    DB.UserCharts.Remove(_UserChart);

                    List<TBL_USER_CHART_DETAIL> _ChartDetailList = DB.ChartDetails.Where(x => x.CHART_ID == _UserChart.ID).ToList();

                    for (int i = 0; i < _ChartDetailList.Count; i++)
                        DB.ChartDetails.Remove(_ChartDetailList[i]);
                }

                DB.SaveChanges();
            }
            catch (Exception ex)
            {
                _result = ex.ToString();
            }

            return Json(_result, JsonRequestBehavior.AllowGet);
        }
        #endregion
       
        public JsonResult GetLineChart(string beginDate, int stationId)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("tr-TR");
            DateTime reqDateParam = DateTime.Parse(@beginDate);
            OZET_HES_DTO_ oDTO = new OZET_HES_DTO_();
            var stDetail = DB.Stations.Where(a => a.ID == stationId).FirstOrDefault();

            float? scale;
            if (stDetail.IRRADIATION_SCALE != null)
            {
                scale = ((stDetail.AC_INSTALLED_POWER) + ((stDetail.AC_INSTALLED_POWER) * (stDetail.IRRADIATION_SCALE) / 100))/1000;
            }
            else
            {
                scale = null;
            }

            float? acInstalled = stDetail.AC_INSTALLED_POWER;



            var ozetler = DB.Summaries.Where(p => p.STATION_ID == stationId
            && p.tarih.Year == reqDateParam.Year && p.tarih.Month == reqDateParam.Month && p.tarih.Day == reqDateParam.Day)
                .OrderBy(a => a.tarih).Select(a =>
                 new TBL_OZET_DTO_HES
                 {
                     _id = a.Id,
                     _tarih = a.tarih,
                     _gunlukUretim = a.gunlukUretim == null ? 0 : (float)Math.Round((double)Math.Abs(a.gunlukUretim.Value), 1),
                     _powerAC = a.gunlukUretim == null ? 0 : (float)Math.Round((double)Math.Abs(a.gunlukUretim.Value), 1),
                     _producedEnergy = a.Enerji == null ? 0f : (float)Math.Round((double)a.Enerji/1000000, 2),
                     _consumedEnergy = a.H2_WP_plus == null ? 0f : (float)Math.Round((double)a.H2_WP_plus, 2),
                     _debi= a.isinim == null ? 0f : (float)Math.Round((double)a.isinim, 2),
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
                        TBL_OZET_DTO_HES ozet = new TBL_OZET_DTO_HES();
                        ozet._tarih = abFirstDate;
                        ozetler.Add(ozet);
                        abFirstDate = abFirstDate.AddMinutes(5);
                    }

                    while (abLastDate > lastDate)
                    {
                        lastDate = lastDate.AddMinutes(5);
                        TBL_OZET_DTO_HES ozet = new TBL_OZET_DTO_HES();
                        ozet._tarih = lastDate;
                        ozetler.Add(ozet);
                    }
                    ozetler = ozetler.OrderBy(x => x._tarih).ToList();
                }


                for (int i = 0; i < ozetler.Count(); i++)
                {
                    ozetler[i]._max = stDetail.AC_INSTALLED_POWER;

                    //if ((ozetler[i]._dcGuc <= 0 && ozetler[i]._isinim <= 1) || (ozetler[i]._isinim == null && ozetler[i]._dcGuc == null))
                    //{
                    //    ozetler[i]._enerji = null;
                    //}

                }



                oDTO._ozet = ozetler;
                oDTO._irradiationScale = scale;
                oDTO._acInstalledPower = acInstalled;
            }
            return Json(oDTO, JsonRequestBehavior.AllowGet);
        }


        public JsonResult ProductionInf(int stationId)
        {
            float monthlySum = 0;
            float annualSum = 0;
            float totalSum = 0;
            float monthlySum2 = 0;
            float annualSum2 = 0;
            float totalSum2 = 0;
            float price = 0;
            float taxPrice = 0;
            float indDC = 0;
            float target = 0;
            float dailySum = 0;
            ProductionModel q = new ProductionModel();
            var stat = DB.Stations.Where(a => a.ID == stationId).FirstOrDefault();
            DateTime startDate = stat.START_DATE.Value;
            DateTime begindt = Convert.ToDateTime(DateTime.Now);
            try
            {
                var endData = DB.Summaries.Where(a => a.STATION_ID == stationId && a.tarih.Year == begindt.Year && a.tarih.Month == begindt.Month && a.tarih.Day == begindt.Day).OrderByDescending(a => a.tarih).Take(1).FirstOrDefault();

                price = DB.Stations.Where(a => a.ID == stationId).Select(a => a.EXCHANGE_RATE).FirstOrDefault();
                taxPrice = DB.Stations.Where(a => a.ID == stationId).Select(a => a.TAX.Value).FirstOrDefault();
                indDC = DB.Stations.Where(a => a.ID == stationId).Select(a => a.DC_INSTALLED_POWER.Value).FirstOrDefault();
                var mnth = Convert.ToInt32(begindt.Month);
                target = GetTargetFunc(stationId, mnth);
                var _dailyProductionDTO = DB.PRSum.Where(p => p.STATION_ID == stationId && p.date.Value.Year == begindt.Year && p.date.Value.Month == begindt.Month && p.date.Value.Day == begindt.Day).FirstOrDefault();
                q._dailyProduction = _dailyProductionDTO.enerji.Value;
                dailySum= _dailyProductionDTO.enerji.Value;
                monthlySum = DB.PRSum.Where(p => p.STATION_ID == stationId && p.date.Value.Year == begindt.Year && p.date.Value.Month == begindt.Month).Sum(a => a.enerji).Value;
                annualSum = DB.PRSum.Where(p => p.STATION_ID == stationId && p.date.Value.Year == begindt.Year && p.date.Value >= startDate.Date).Sum(a => a.enerji).Value;
                totalSum = DB.PRSum.Where(p => p.STATION_ID == stationId && p.date.Value >= startDate.Date).Sum(a => a.enerji).Value;
                monthlySum2 = monthlySum;
                annualSum2 = annualSum;
                totalSum2 = totalSum;
                q._pac = endData.gunlukUretim == null? 0 :Math.Abs(endData.gunlukUretim.Value);
                q._isinim= endData.isinim == null ? 0 : endData.isinim;
                q._ruzgar = endData.ruzgarHizi == null ? 0 : endData.ruzgarHizi;
                q.isEKK = stat.IS_EKK == null ? false : stat.IS_EKK;
                q.isMeteorology = stat.IS_METEOROLOGY == null ? false : stat.IS_METEOROLOGY;
                q.insAC = stat.AC_INSTALLED_POWER == null ? 0 : stat.AC_INSTALLED_POWER;
                q._dailyIncome = dailySum * price * 1000;
                q._monthlyProduction = monthlySum2;
                q._monthlyIncome = monthlySum2 * price * 1000;
                q._annualProduction = annualSum2;
                q._annualIncome = annualSum2 * price * 1000;
                q._totalProduction = totalSum2;
                q._totalIncome = totalSum2 * price * 1000;
                q._specificYield = dailySum / indDC;
                q._actualValue = dailySum / target;
                q._dailyTax = dailySum * taxPrice * 1000;
                q._monthlTax = monthlySum2 * taxPrice * 1000;
                q._annualTax = annualSum2 * taxPrice * 1000;
                q._totalTax = totalSum2 * taxPrice * 1000;
            }
            catch (Exception ex)
            {
              
            }

            return Json(q, JsonRequestBehavior.AllowGet);

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

        public JsonResult HourlyReport(int stationId, string slctDate, string enddate)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("tr-TR");
            DateTime date = DateTime.Parse(@slctDate);
            DateTime date2 = DateTime.Parse(enddate);
            date2 = date2.AddDays(1);
            var hourlyProduction = (from slc in DB.Summaries
                                    where slc.STATION_ID == stationId && slc.tarih >= date && slc.tarih <= date2
                                    //&& slc.tarih.Hour >= 5 && slc.tarih.Hour <= 21
                                    orderby slc.tarih ascending
                                    select new Hour_HES_DTO
                                    {
                                        _enerji = slc.Enerji == null ? 0 : slc.Enerji.Value,
                                        _isinimToplam = slc.isinim == null ? 0 : slc.isinim.Value,
                                        _uretilen_enerji = slc.H2_WP_minus == null ? 0 : slc.H2_WP_minus.Value,
                                        _tarih = slc.tarih
                                    })
                  .AsEnumerable()
                  .GroupBy(x => x._tarih.ToString("dd/MM/yyyy HH:00:00"))
                  .Select(g => new Hour_HES_DTO
                  {
                      _enerji = (g.Max(a => a._enerji)) / 1000000,
                      _uretilen_enerji = (g.Max(a => a._uretilen_enerji)),
                      _isinimToplam = g.Average(a => a._isinimToplam),
                      _tarih = Convert.ToDateTime(g.Key)
                  }).ToList();
            List<Hour_HES_DTO> hTO = new List<Hour_HES_DTO>();
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
                    string time;
                    if (hh._tarih.Hour.ToString().Length<2)
                    {
                        time = "0" + hh._tarih.Hour +":00";
                    }
                    else
                    {
                        time= hh._tarih.Hour + ":00";
                    }
                    hTO.Add(new Hour_HES_DTO { _saat = time, _enerji = Math.Round(fark, 4), _enerjiArtan = Math.Round(hh._enerji, 4), _uretilen_enerji = Math.Round(EkkFark, 4), _isinimToplam = Math.Round(hh._isinimToplam, 1), _tarih = hh._tarih });
                }

            }
            var data = hTO.OrderBy(o => o._tarih).ToList();
            Session["HourlyEnergyData"] = data;
            return Json(data);
        }

        #region StationDetail
        public ActionResult StationDetail(int stationId)
        {
            return View(stationId);
        }
        #endregion StationDetail

        #region MeteorologyDetail
        public ActionResult MeteorolojiDetail(int stationId)
        {
            return View(stationId);
        }

        public JsonResult GetLineChartMeteoroloji(string beginDate, int stationId)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("tr-TR");
            DateTime reqDateParam = DateTime.Parse(beginDate);
            var met = DB.Summaries.Where(a => a.STATION_ID == stationId && a.tarih.Year == reqDateParam.Year
              && a.tarih.Month == reqDateParam.Month && a.tarih.Day == reqDateParam.Day)
              .Select(a => new Meteoroloji_HES_DTO
              {
                  DATE = a.tarih,
                  FLOW = (float)Math.Round(a.isinim.Value, 1),
                  KOT = (float)Math.Round(a.ruzgarHizi.Value, 1)
              }).OrderBy(a => a.DATE).ToList();
            return Json(met, JsonRequestBehavior.AllowGet);
        }
        #endregion MeteorologyDetail
    }
}