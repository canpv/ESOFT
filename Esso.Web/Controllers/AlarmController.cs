using DevExpress.Web.Demos;
using DevExpress.Web.Mvc;
using Esso.Data;
using Esso.Web.ViewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
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

namespace Esso.Web.Controllers
{
	[Authorize]
	public class AlarmController : BaseController
    {
		EssoEntities DB = new EssoEntities();
		public ActionResult Index(int stationId)
		{         
            return View(stationId);
		}


		public ActionResult AlarmListPartial(int stationId ,/* string date,*/ string date1, string date2)
		{
			ViewData["date1"] = date1;
			ViewData["date2"] = date2;
			//ViewData["date"] = date;
			return PartialView(stationId);
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




		public ActionResult ExportTo(GridViewExportFormat? exportFormat, int stationId, DateTime d_From, DateTime d_To)
		{
            Thread.CurrentThread.CurrentCulture = new CultureInfo("tr-TR");
            string _stationName = (from tt in DB.Stations
								   where tt.ID == stationId && tt.IS_DELETED == false
								   select new { tt.NAME }).FirstOrDefault().NAME;
			if (exportFormat == null || !GridViewExportHelper.ExportFormatsInfo.ContainsKey(exportFormat.Value))
				return RedirectToAction("AlarmListPartial", stationId);

			//DateTime dts = Convert.ToDateTime(datePck);
			string date1 = Convert.ToString(d_From);
			date1 = date1.Substring(0 , date1.Length -9);
			DateTime date_1 = Convert.ToDateTime(date1);

			DateTime date_2;
			DateTime date2 = Convert.ToDateTime(d_To);
			if (date2.Hour == 00)
			{
				date_2 = date2.AddHours(23);
				date_2 = date_2.AddMinutes(59);
			}
			else
			{
				date_2 = date2;
			}
			
			var stationName = DB.Stations.Where(a => a.ID == stationId).FirstOrDefault().NAME;
            var _listAlarmDesc = DB.AlarmDesc.ToList();
            var _list = DB.AlarmStatus.Where(x => x.STATION_ID == stationId).OrderBy(x => x.START_DATE).ToList();

			var _listInv = DB.Inverters.ToList();

			List<AlarmStatusDTO> _ListDataSource = new List<AlarmStatusDTO>();

			for (int i = 0; i < _list.Count; i++)
			{
				AlarmStatusDTO alarmStatus = new AlarmStatusDTO();

				alarmStatus.INVERTER_ID = _list[i].INVERTER_ID;

				TBL_INVERTER _TblInv = _listInv.Where(x => x.ID == _list[i].INVERTER_ID).FirstOrDefault();

				if (_TblInv != null)
				{
					alarmStatus.INV_NAME = _TblInv.NAME;
				}
				else
				{
					alarmStatus.INV_NAME = stationName;
				}

				alarmStatus.ERROR_NUMBER = _list[i].ERROR_NUMBER;
				//alarmStatus.STATUS = _list[i].STATUS;
				alarmStatus.START_DATE = _list[i].START_DATE;

				alarmStatus.END_DATE = _list[i].END_DATE;
				alarmStatus.STATION_ID = _list[i].STATION_ID;

                alarmStatus.ERROR_NUMBER_NAME = _listAlarmDesc.Where(a => a.ERROR_NUMBER == _list[i].ERROR_NUMBER).FirstOrDefault().ERROR_DESC;

                _ListDataSource.Add(alarmStatus);
			}

			List<AlarmStatusExportDTO> _ListExport = new List<AlarmStatusExportDTO>();

			var _TempList = _ListDataSource.Where(x => x.START_DATE >= date_1 && x.START_DATE <= date_2)/*.OrderBy(x => x.START_DATE)*/.ToList();
			

			for (int i = 0; i < _TempList.Count; i++)
			{
				AlarmStatusExportDTO _ExportDto = new AlarmStatusExportDTO();

				_ExportDto.ID = i + 1;
				_ExportDto.DEVICE = _TempList[i].INV_NAME;
				_ExportDto.ERROR_DEFINITION = _TempList[i].ERROR_NUMBER_NAME;
				_ExportDto.ALERT_START_DATE = _TempList[i].START_DATE.ToString();
				_ExportDto.ALERT_END_DATE = _TempList[i].END_DATE.ToString();
				
				_ListExport.Add(_ExportDto);
			}

			string filename = _stationName + " (" + date1.ToString() + " - " + date2.ToShortDateString() + ")";
			GridHelper gh = new GridHelper(stationId);

			if (exportFormat == GridViewExportFormat.Xls)
			{
				XlsExportOptionsEx exportOptions = new XlsExportOptionsEx();
				exportOptions.CustomizeCell += new DevExpress.Export.CustomizeCellEventHandler(exportOptions_CustomizeCell);
				return GridViewExtension.ExportToXls(gh.GridStationSetting, _ListExport, filename, exportOptions);
			}
			else if (exportFormat == GridViewExportFormat.Pdf)
			{
				return GridViewExtension.ExportToPdf(gh.GridStationSetting, _ListExport, filename);
			}
			else if (exportFormat == GridViewExportFormat.Xlsx)
			{
				return GridViewExtension.ExportToXlsx(gh.GridStationSetting, _ListExport, filename);
			}
			return View(stationId);

		}

		public class GridHelper
		{
			static int _stationId;
			public GridHelper(int stationId)
			{
				_stationId = stationId;
			}
			static GridViewSettings gridStationSettings;
			public GridViewSettings GridStationSetting
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
				
				settings.CallbackRouteValues = new { Controller = "Alarm", Action = "AlarmListPartial", stationId = _stationId };

				settings.Name = "gridViewAlarm";
				settings.KeyFieldName = "ID";
				settings.SettingsPager.PageSize = 30;

				settings.SettingsPager.Position = PagerPosition.Bottom;
				settings.SettingsPager.NumericButtonCount = 50;
				settings.SettingsPager.EnableAdaptivity = true;
				settings.SettingsPager.FirstPageButton.Visible = true;
				settings.SettingsPager.LastPageButton.Visible = true;
				settings.SettingsPager.PageSizeItemSettings.Visible = true;
				//settings.SettingsPager.PageSizeItemSettings.Items = new string[] { "10", "20", "50" };

				settings.SettingsBehavior.AllowFocusedRow = false;
				
				return settings;
			}
		}

	[HttpGet]
        public JsonResult GetAlarmCount(int stationId)
        {
            int? alarcount = DB.AlarmStatus.Where(x => x.STATION_ID == stationId && x.END_DATE == null && x.ERROR_NUMBER != "0005" && x.ERROR_NUMBER != "0004" && x.ERROR_NUMBER != "0003" && x.ERROR_NUMBER != "0002" && x.STATUS != 2).Count();

            var d = "{COUNT:" + (alarcount == null ? "0" : alarcount.ToString()) + "}";

            return Json((alarcount == null ? "0" : alarcount.ToString()),JsonRequestBehavior.AllowGet);
        }

    }



}