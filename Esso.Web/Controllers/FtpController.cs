using AutoMapper;
using DevExpress.Utils;
using Esso.Data;
using Esso.Models;
using Esso.Web.ViewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Z.EntityFramework.Plus;

namespace Esso.Web.Controllers
{
	[Authorize]
	public class FtpController : BaseController
	{


		EssoEntities DB = new EssoEntities();
	
		public ActionResult Index()
		{
			return View();
		}

		public ActionResult FtpGridPartial()
		{
			return PartialView(GetFtpList());
		}

		List<FtpDTO> GetFtpList() 
		{
			List<FtpDTO> _ftpList = new List<FtpDTO>();

			List<TBL_FTP> ftp = DB.FtpList.ToList();
			

			for (int i = 0; i < ftp.Count; i++)
			{
				FtpDTO _ftp = new FtpDTO();

				_ftp.ID = ftp[i].ID;
				_ftp.STATION_ID = ftp[i].STATION_ID;
				_ftp.STATION_NAME = (from t in DB.Stations join tt in DB.FtpList on t.ID equals tt.STATION_ID where t.ID == tt.STATION_ID select t.NAME).FirstOrDefault();
				_ftp.IP_ADDRESS = ftp[i].IP_ADDRESS;
				_ftp.PORT_NO = ftp[i].PORT_NO;
				_ftp.USER_NAME = ftp[i].USER_NAME;
				_ftp.PASSWORD = ftp[i].PASSWORD;
				_ftpList.Add(_ftp);
			}

			return _ftpList;
			
		}
		
		public ActionResult InsertFtp(FtpDTO ftp)
		{

			if (ModelState.IsValid )
			{
				TBL_FTP _ftp = new TBL_FTP();
				_ftp.ID = ftp.ID;
				_ftp.STATION_ID = ftp.STATION_ID;
				_ftp.IP_ADDRESS = ftp.IP_ADDRESS;
				_ftp.PORT_NO = ftp.PORT_NO;
				_ftp.USER_NAME = ftp.USER_NAME;
				_ftp.PASSWORD = ftp.PASSWORD;

				DB.FtpList.Add(_ftp);
				DB.SaveChanges();


			}
			else
			{
				ViewData["EditError"] = "Please, correct all errors.";
				ViewData["EditableProduct"] = ftp;
			}

			return PartialView("FtpGridPartial", GetFtpList());

		}

		public ActionResult UpdateFtp(FtpDTO ftp)
		{
			if (ModelState.IsValid )
			{
				TBL_FTP _ftp = DB.FtpList.AsNoTracking().FirstOrDefault(x => x.ID == ftp.ID);

				_ftp.ID = ftp.ID;
				_ftp.STATION_ID = ftp.STATION_ID;
				_ftp.IP_ADDRESS = ftp.IP_ADDRESS;
				_ftp.PORT_NO = ftp.PORT_NO;
				_ftp.USER_NAME = ftp.USER_NAME;
				_ftp.PASSWORD = ftp.PASSWORD;
				
				DB.Entry(_ftp).State = EntityState.Modified;

				DB.SaveChanges();
			}
			else
			{
				ViewData["EditError"] = "Please, correct all errors.";
				ViewData["EditableProduct"] = ftp;
			}
			return PartialView("FtpGridPartial", GetFtpList());
		}

		public ActionResult DeleteFtp(FtpDTO ftp)
		{
			try
			{

				DB.FtpList.Where(x => x.ID == ftp.ID).Delete();
				

				DB.SaveChanges();
				return PartialView("FtpGridPartial", GetFtpList());
			}
			catch (Exception ex)
			{
				ViewData["EditError"] = ex.Message;
				return PartialView("FtpGridPartial", GetFtpList());
			}
		}

	}
}