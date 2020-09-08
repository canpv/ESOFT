using AutoMapper;
using DevExpress.Utils;
using DevExpress.Web;
using DevExpress.Web.Mvc;
using Esso.Data;
using Esso.Models;
using Esso.Service;
using Esso.ViewModels;
using Esso.Web.App_Start;
using Esso.Web.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Z.EntityFramework.Plus;

namespace Esso.Web.Controllers
{
    [Authorize]
    public class StationController : BaseController
    {
        EssoEntities DB = new EssoEntities();
        private readonly IStationService stationService;
        private readonly IStationGroupService stationGroupService;
        private readonly ICompanyUserService companyUserService;
        private readonly IStationUserService stationUserService;

        public StationController(IStationService stationService, IStationGroupService stationGroupService, ICompanyUserService companyUserService, IStationUserService stationUserService)
        {
            this.stationService = stationService;
            this.stationGroupService = stationGroupService;
            this.companyUserService = companyUserService;
            this.stationUserService = stationUserService;
        }
        
        public ActionResult Index()
        {
            return View();
        }

        [ValidateInput(false)]
        public ActionResult GridStationPartial(int companyId = 0) // ekrana veritabınındaki değerleri getirir.
        {
            ViewData["companyId"] = companyId;

			List<StationGridModel> stDTO = GetStationByCompanyId(companyId).ToList();

			if (stDTO != null)
			{
				foreach (StationGridModel station in stDTO)
				{
					GetStationTargets(station);
				}
				
			}
			
			return PartialView(companyId == 0 ? new List<StationGridModel>() :  stDTO );

        }

        #region Station Strings
        public ActionResult GridStationStringPartial(int stationId,bool selectAll,int bunchSelect = 0)
        {        

            return PartialView(GetStationStrings(stationId, selectAll, bunchSelect));
        }
		

		List<StationStringDTO> GetStationStrings(int stationId, bool selectAll,int bunchSelect)
        {
            List<StationStringDTO> retVal = new List<StationStringDTO>();
            List<TBL_TAG> strTags = DB.Tags.Where(x => x.IS_STRING == true && x.IS_DELETED == false).ToList();
            List<TBL_STATION_STRING> stStrings = DB.stationString.Where(x => x.STATION_ID == stationId && x.IS_DELETED == false).ToList();

            ViewData["stationId"] = stationId;

            retVal = (from t in strTags
                      select new StationStringDTO
                      {
                          STRING_ID = t.ID,
                          STR_TAG_NAME = t.NAME,
						  DISPLAY_NAME = DB.stationString.Where(X=>X.STRING_ID == t.ID && X.STATION_ID== stationId && X.IS_DELETED==false).Select(X => X.DISPLAY_NAME).FirstOrDefault(),
						  IS_SELECTED = bunchSelect == 1 ? selectAll : stStrings.Any(x => x.STRING_ID == t.ID)
                      }).OrderBy(x => x.STR_TAG_NAME).ToList();

            return retVal;
        }
		

		[ValidateInput(false)]
        public ActionResult UpdateStationStrings(MVCxGridViewBatchUpdateValues<StationStringDTO, int> updateValues, int stationId , bool selectAll, int bunchSelect)
        {
            try
            {
				if (bunchSelect == 1)
                {
                    DB.stationString.Where(x => x.STATION_ID == stationId).Delete();
                    if (selectAll)
                    {
                        List<TBL_TAG> strings = DB.Tags.Where(x => x.IS_STRING == true && x.IS_DELETED == false).ToList();
						List<TBL_STATION_STRING> stStrings = DB.stationString.Where(x => x.STATION_ID == stationId && x.IS_DELETED == false).ToList();
						if (strings != null)
                        {
                            foreach (TBL_TAG st in strings)
                            {
                                TBL_STATION_STRING entity = new TBL_STATION_STRING();
                                entity.IS_DELETED = false;
                                entity.STATION_ID = stationId;
                                entity.STRING_ID = st.ID;
                                DB.stationString.Add(entity);
                                DB.SaveChanges();
                            }
							
                        }


						foreach (StationStringDTO uVal in updateValues.Update)
						{
							if (stStrings.Any(x => x.STRING_ID == uVal.STRING_ID && x.IS_DELETED == false))
							{
								TBL_STATION_STRING entity = new TBL_STATION_STRING();
								if (entity.DISPLAY_NAME != uVal.DISPLAY_NAME)
								{
									DB.stationString.Where(x => x.STATION_ID == stationId && x.STRING_ID == uVal.STRING_ID).Update(x => new TBL_STATION_STRING() { DISPLAY_NAME = uVal.DISPLAY_NAME });
									DB.SaveChanges();
								}


							}

						}


					}
                }
                else 
                {
                    List<TBL_STATION_STRING> stStrings = DB.stationString.Where(x => x.STATION_ID == stationId && x.IS_DELETED == false).ToList();

                    foreach (StationStringDTO uVal in updateValues.Update)
                    {
                        if (stStrings.Any(x => x.STRING_ID == uVal.STRING_ID && x.IS_DELETED == false))
                        {
							bool displayname;
							//string ttt = (from ds in DB.stationString where ds.IS_DELETED == false && ds.STRING_ID == uVal.STRING_ID select  ds.DISPLAY_NAME ).FirstOrDefault();
							var display_name = stStrings.Where(x => x.STRING_ID == uVal.STRING_ID).Select(x => x.DISPLAY_NAME).FirstOrDefault();

							if (display_name == null)
							{displayname = false;}
							else
							{displayname = true;}


							if (displayname == true )
							{
								if (display_name != uVal.DISPLAY_NAME && uVal.IS_SELECTED == true)
								{
									DB.stationString.Where(x=> x.STATION_ID ==stationId && x.STRING_ID==uVal.STRING_ID).Update(x => new TBL_STATION_STRING(){ DISPLAY_NAME = uVal.DISPLAY_NAME });
									DB.SaveChanges();
								}
								else if (display_name == uVal.DISPLAY_NAME && uVal.IS_SELECTED == false)
								{DB.stationString.Where(x => x.STRING_ID == uVal.STRING_ID && x.IS_DELETED == false).Delete();}
							}
							else if (displayname == false)
							{
								if (uVal.IS_SELECTED == true)
								{
									if (display_name == uVal.DISPLAY_NAME)
									{
										TBL_STATION_STRING entity = new TBL_STATION_STRING();
										entity.IS_DELETED = false;
										entity.STATION_ID = stationId;
										entity.STRING_ID = uVal.STRING_ID;
										DB.stationString.Add(entity);
										DB.SaveChanges();
									}
									else
									{
										DB.stationString.Where(x => x.STATION_ID == stationId && x.STRING_ID == uVal.STRING_ID).Update(x => new TBL_STATION_STRING() { DISPLAY_NAME = uVal.DISPLAY_NAME });
										DB.SaveChanges();
									}
									
								}
								else
								{DB.stationString.Where(x => x.STRING_ID == uVal.STRING_ID && x.IS_DELETED == false).Delete();}
								
							}
							else if (uVal.IS_SELECTED == false)
							{DB.stationString.Where(x => x.STRING_ID == uVal.STRING_ID && x.IS_DELETED == false).Delete();}
						

						}
						else
                        {
                            TBL_STATION_STRING entity = new TBL_STATION_STRING();
                            entity.IS_DELETED = false;
                            entity.STATION_ID = stationId;
                            entity.STRING_ID = uVal.STRING_ID;
							entity.DISPLAY_NAME = uVal.DISPLAY_NAME;
							DB.stationString.Add(entity);
                            DB.SaveChanges();
                        }
                    }
                }

             

                

                return PartialView("GridStationStringPartial", GetStationStrings(stationId,false,0));
            }
            catch (Exception ex)
            {
                updateValues.SetErrorText(updateValues.Update[0], ex.Message);
                return PartialView("GridStationStringPartial", GetStationStrings(stationId,false,0));
            }
        }


        #endregion

        [ValidateInput(false)]
        public ActionResult  GridLookupPartial(int companyId, int stationId = 0 )
        {

            string curUserId = User.Identity.GetUserId();


			ViewData["companyId"] = companyId;
            List<string> compUserIds = companyUserService.GetCompanyUsers(x => x.COMPANY_ID == companyId && x.IS_DELETED == false).Select(x => x.USER_ID).ToList();

            List<string> statUserIds = stationUserService.GetStationUsers(x => x.STATION_ID == stationId && x.IS_DELETED == false).Select(x => x.USER_ID).ToList();

			var users = Request.GetOwinContext().GetUserManager<ApplicationUserManager>().Users.Where(x => x.Roles.Any(r => r.RoleId.Equals("d033ae02-905b-424a-b1f3-59657a76b4a1") || r.RoleId.Equals("abc37a3f-b315-4a32-b487-d2e8b67a19b2")) && compUserIds.Contains(x.Id) && x.IS_DELETED == false).Select(x => new { x.Id, x.UserName }).OrderBy(X=>X.UserName).ToList();
            
            ViewBag.List = users;
            return PartialView("GridLookupPartial",statUserIds);
        }




		void GetStationTargets(StationGridModel stationDTO) // veri tabanındaki target ları çekiyoruz
		{
			TBL_TARGET targett = DB.targets.Where(x => x.STATION_ID == stationDTO.ID && x.IS_DELETED == false).FirstOrDefault();

			stationDTO.JAN_PRODUCTION = targett.JAN_PRODUCTION;
			stationDTO.FEB_PRODUCTION = targett.FEB_PRODUCTION;
			stationDTO.MARCH_PRODUCTION = targett.MARCH_PRODUCTION;
			stationDTO.APRIL_PRODUCTION = targett.APRIL_PRODUCTION;
			stationDTO.MAY_PRODUCTION = targett.MAY_PRODUCTION;
			stationDTO.JUNE_PRODUCTION = targett.JUNE_PRODUCTION;
			stationDTO.JULY_PRODUCTION = targett.JULY_PRODUCTION;
			stationDTO.AUGUST_PRODUCTION = targett.AUGUST_PRODUCTION;
			stationDTO.SEP_PRODUCTION = targett.SEP_PRODUCTION;
			stationDTO.OKT_PRODUCTION = targett.OKT_PRODUCTION;
			stationDTO.NOV_PRODUCTION = targett.NOV_PRODUCTION;
			stationDTO.DEC_PRODUCTION = targett.DEC_PRODUCTION;

			stationDTO.JAN_IRRADIATION = targett.JAN_IRRADIATION;
			stationDTO.FEB_IRRADIATION = targett.FEB_IRRADIATION;
			stationDTO.MARCH_IRRADIATION = targett.MARCH_IRRADIATION;
			stationDTO.APRIL_IRRADIATION = targett.APRIL_IRRADIATION;
			stationDTO.MAY_IRRADIATION = targett.MAY_IRRADIATION;
			stationDTO.JUNE_IRRADIATION = targett.JUNE_IRRADIATION;
			stationDTO.JULY_IRRADIATION = targett.JULY_IRRADIATION;
			stationDTO.AUGUST_IRRADIATION = targett.AUGUST_IRRADIATION;
			stationDTO.SEP_IRRADIATION = targett.SEP_IRRADIATION;
			stationDTO.OKT_IRRADIATION = targett.OKT_IRRADIATION;
			stationDTO.NOV_IRRADIATION = targett.NOV_IRRADIATION;
			stationDTO.DEC_IRRADIATION = targett.DEC_IRRADIATION;

			stationDTO.YEAR_PRODUCTION = targett.YEAR_PRODUCTION;
			stationDTO.YEAR_IRRADIATION = targett.YEAR_IRRADIATION;
		}

		

		List<StationGridModel> GetStationByCompanyId(int companyId)
        {
            List<TBL_STATION> statEntities = stationService.GetStationByCompanyId(companyId).ToList();
            List<StationGridModel> stats = Mapper.Map<List<TBL_STATION>, List<StationGridModel>>(statEntities);

            System.Drawing.Image img;
            string imgPath;
            foreach (StationGridModel stat in stats)
            {
                imgPath = Server.MapPath("~/images/StationImages/" + stat.ID.ToString() + ".png");

                if (System.IO.File.Exists(imgPath))
                {
                    img = System.Drawing.Image.FromFile(imgPath);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        img.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                        stat.PHOTO = ms.ToArray();
                    }
                }
                else
                {
                    img = System.Drawing.Image.FromFile(Server.MapPath("~/images/StationImages/0.png"));
                    using (MemoryStream ms = new MemoryStream())
                    {
                        img.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                        stat.PHOTO = ms.ToArray();
                    }
                }
            }

            return stats;
        }

		public ActionResult SaveTargetStation(StationGridModel trgt,int stationId)
		{
			
			if (ModelState.IsValid && stationId > 0)
			{
				TBL_TARGET target = new TBL_TARGET();
				target.STATION_ID = stationId;
				target.CREATED_DATE = DateTime.Now;
				target.UPDATED_DATE = DateTime.Now;
				target.INSTALL_DATE = DateTime.Now;
				target.UPDATE_USER = User.Identity.GetUserId();
				if (trgt.JAN_IRRADIATION != null
				&&
				trgt.FEB_IRRADIATION != null
				&&
				trgt.MARCH_IRRADIATION != null
				&&
				trgt.APRIL_IRRADIATION != null
				&&
				trgt.MAY_IRRADIATION != null
				&&
				trgt.JUNE_IRRADIATION != null
				&&
				trgt.JULY_IRRADIATION != null
				&&
				trgt.AUGUST_IRRADIATION != null
				&&
				trgt.SEP_IRRADIATION != null
				&&
				trgt.OKT_IRRADIATION != null
				&&
				trgt.NOV_IRRADIATION != null
				&&
				trgt.DEC_IRRADIATION != null
				&&
				trgt.JAN_PRODUCTION != null
				&&
				trgt.FEB_PRODUCTION != null
				&&
				trgt.MARCH_PRODUCTION != null
				&&
				trgt.APRIL_PRODUCTION != null
				&&
				trgt.MAY_PRODUCTION != null
				&&
				trgt.JUNE_PRODUCTION != null
				&&
				trgt.JULY_PRODUCTION != null
				&&
				trgt.AUGUST_PRODUCTION != null
				&&
				trgt.SEP_PRODUCTION != null
				&&
				trgt.OKT_PRODUCTION != null
				&&
				trgt.NOV_PRODUCTION != null
				&&
				trgt.DEC_PRODUCTION != null
				&&
				trgt.YEAR_IRRADIATION != null
				&&
				trgt.YEAR_PRODUCTION != null)
				{


					target.JAN_PRODUCTION = trgt.JAN_PRODUCTION;
					target.FEB_PRODUCTION = trgt.FEB_PRODUCTION;
					target.MARCH_PRODUCTION = trgt.MARCH_PRODUCTION;
					target.APRIL_PRODUCTION = trgt.APRIL_PRODUCTION;
					target.MAY_PRODUCTION = trgt.MAY_PRODUCTION;
					target.JUNE_PRODUCTION = trgt.JUNE_PRODUCTION;
					target.JULY_PRODUCTION = trgt.JULY_PRODUCTION;
					target.AUGUST_PRODUCTION = trgt.AUGUST_PRODUCTION;
					target.SEP_PRODUCTION = trgt.SEP_PRODUCTION;
					target.OKT_PRODUCTION = trgt.OKT_PRODUCTION;
					target.NOV_PRODUCTION = trgt.NOV_PRODUCTION;
					target.DEC_PRODUCTION = trgt.DEC_PRODUCTION;

					target.JAN_IRRADIATION = trgt.JAN_IRRADIATION;
					target.FEB_IRRADIATION = trgt.FEB_IRRADIATION;
					target.MARCH_IRRADIATION = trgt.MARCH_IRRADIATION;
					target.APRIL_IRRADIATION = trgt.APRIL_IRRADIATION;
					target.MAY_IRRADIATION = trgt.MAY_IRRADIATION;
					target.JUNE_IRRADIATION = trgt.JUNE_IRRADIATION;
					target.JULY_IRRADIATION = trgt.JULY_IRRADIATION;
					target.AUGUST_IRRADIATION = trgt.AUGUST_IRRADIATION;
					target.SEP_IRRADIATION = trgt.SEP_IRRADIATION;
					target.OKT_IRRADIATION = trgt.OKT_IRRADIATION;
					target.NOV_IRRADIATION = trgt.NOV_IRRADIATION;
					target.DEC_IRRADIATION = trgt.DEC_IRRADIATION;

					target.YEAR_PRODUCTION = trgt.YEAR_PRODUCTION;
					target.YEAR_IRRADIATION = trgt.YEAR_IRRADIATION;
				}
				else {
					target.JAN_IRRADIATION = 0;
					target.JAN_PRODUCTION = 0;
					target.FEB_IRRADIATION = 0;
					target.FEB_PRODUCTION = 0;
					target.MARCH_IRRADIATION = 0;
					target.MARCH_PRODUCTION = 0;
					target.APRIL_IRRADIATION = 0;
					target.APRIL_PRODUCTION = 0;
					target.MAY_IRRADIATION = 0;
					target.MAY_PRODUCTION = 0;
					target.JUNE_IRRADIATION = 0;
					target.JUNE_PRODUCTION = 0;
					target.JULY_IRRADIATION = 0;
					target.JULY_PRODUCTION = 0;
					target.AUGUST_IRRADIATION = 0;
					target.AUGUST_PRODUCTION = 0;
					target.SEP_IRRADIATION = 0;
					target.SEP_PRODUCTION = 0;
					target.OKT_IRRADIATION = 0;
					target.OKT_PRODUCTION = 0;
					target.NOV_IRRADIATION = 0;
					target.NOV_PRODUCTION = 0;
					target.DEC_IRRADIATION = 0;
					target.DEC_PRODUCTION = 0;
					target.YEAR_IRRADIATION = 0;
					target.YEAR_PRODUCTION = 0;

				}
				DB.targets.Add(target);
				DB.SaveChanges();
			

			}
			else
			{
				ViewData["EditError"] = "Please, correct all errors.";
				ViewData["EditableProduct"] = trgt;
			}

			return PartialView("GridStationPartial", GetStationByCompanyId(stationId));

		}

		public ActionResult UpdateTargetStation(StationGridModel statdDTO, int companyId)
		{
			if (ModelState.IsValid && companyId > 0)
			{

				//TBL_TARGET target = DB.targets.Where(x => x.STATION_ID == companyId && x.IS_DELETED == false).FirstOrDefault();
				TBL_TARGET target = DB.targets.AsNoTracking().FirstOrDefault(x => x.STATION_ID == statdDTO.ID);
		
				target.UPDATE_USER = User.Identity.GetUserId();

				target.JAN_PRODUCTION = statdDTO.JAN_PRODUCTION;
				target.FEB_PRODUCTION = statdDTO.FEB_PRODUCTION;
				target.MARCH_PRODUCTION = statdDTO.MARCH_PRODUCTION;
				target.APRIL_PRODUCTION = statdDTO.APRIL_PRODUCTION;
				target.MAY_PRODUCTION = statdDTO.MAY_PRODUCTION;
				target.JUNE_PRODUCTION = statdDTO.JUNE_PRODUCTION;
				target.JULY_PRODUCTION = statdDTO.JULY_PRODUCTION;
				target.AUGUST_PRODUCTION = statdDTO.AUGUST_PRODUCTION;
				target.SEP_PRODUCTION = statdDTO.SEP_PRODUCTION;
				target.OKT_PRODUCTION = statdDTO.OKT_PRODUCTION;
				target.NOV_PRODUCTION = statdDTO.NOV_PRODUCTION;
				target.DEC_PRODUCTION = statdDTO.DEC_PRODUCTION;

				target.JAN_IRRADIATION = statdDTO.JAN_IRRADIATION;
				target.FEB_IRRADIATION = statdDTO.FEB_IRRADIATION;
				target.MARCH_IRRADIATION = statdDTO.MARCH_IRRADIATION;
				target.APRIL_IRRADIATION = statdDTO.APRIL_IRRADIATION;
				target.MAY_IRRADIATION = statdDTO.MAY_IRRADIATION;
				target.JUNE_IRRADIATION = statdDTO.JUNE_IRRADIATION;
				target.JULY_IRRADIATION = statdDTO.JULY_IRRADIATION;
				target.AUGUST_IRRADIATION = statdDTO.AUGUST_IRRADIATION;
				target.SEP_IRRADIATION = statdDTO.SEP_IRRADIATION;
				target.OKT_IRRADIATION = statdDTO.OKT_IRRADIATION;
				target.NOV_IRRADIATION = statdDTO.NOV_IRRADIATION;
				target.DEC_IRRADIATION = statdDTO.DEC_IRRADIATION;

				target.YEAR_PRODUCTION = statdDTO.YEAR_PRODUCTION;
				target.YEAR_IRRADIATION = statdDTO.YEAR_IRRADIATION;

				DB.Entry(target).State = EntityState.Modified;
				
				DB.SaveChanges();
			}
			else
			{
				ViewData["EditError"] = "Please, correct all errors.";
				ViewData["EditableProduct"] = statdDTO;
			}
			return PartialView("GridStationPartial", GetStationByCompanyId(companyId));
		}



		[HttpPost, ValidateInput(false)]
        public ActionResult SaveStation(StationGridModel station)
        {
            int companyId = !string.IsNullOrEmpty(Request.Params["companyId"]) ? int.Parse(Request.Params["companyId"]) : 0;
			int stationId = !string.IsNullOrEmpty(Request.Params["stationId"]) ? int.Parse(Request.Params["stationId"]) : 0;
			if (ModelState.IsValid && companyId > 0)
            {
                TBL_STATION stat = Mapper.Map<StationGridModel, TBL_STATION>(station);
                try
                {
                    stat.COMPANY_ID = companyId;
					
                    if (!stationService.IsStationExist(stat))
                    {
                        stat.CREATED_DATE = DateTime.Now;
                        stat.UPDATE_USER = User.Identity.GetUserId();
                        stationService.CreateStation(stat);
                        stationService.SaveStation();
                        //SaveStationUsers(stat.ID,station.USERS);
                        string photoFileName = string.Empty;
                        if (station.PHOTO != null && station.PHOTO.Length > 0)
                        {
                            string retVal = SaveStationPic(station.PHOTO, stat.ID);
                            
                            if (retVal.Length > 0)
                            {
                                ViewData["EditError"] = "Could not save picture. " + retVal;
                                ViewData["EditableProduct"] = station;
                                photoFileName = "noimage.png";
                            }
                            else
                            {
                                photoFileName = stat.ID.ToString() + ".png";
                            }
                        }
                        else
                        {
                            photoFileName = "noimage.png";
                        }
                        stationService.UpdateStation(stat);
                        stationService.SaveStation();
						stationId = stat.ID;

						SaveTargetStation(station,stationId );

					}
                    else
                    {
                        ViewData["EditableProduct"] = station;
                        ViewData["EditError"] = "Name is already taken";
                    }                   
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
            {
                ViewData["EditError"] = "Please, correct all errors.";
                ViewData["EditableProduct"] = station;
            }
            return PartialView("GridStationPartial", GetStationByCompanyId(companyId));
        }


        void SaveStationUsers(int stationId,string[] userIds)
        {
            List<TBL_STATION_USER> ststUsers = stationUserService.GetStationUsers(x => x.STATION_ID == stationId && x.IS_DELETED == false).ToList();
            
            if (userIds != null)
            {
               
                    foreach (TBL_STATION_USER tsu in ststUsers)
                    {
                        if (!userIds.Any(x => x == tsu.USER_ID))
                        {
                            tsu.UPDATE_USER = User.Identity.GetUserId();
                            stationUserService.DeleteStationUser(tsu.ID);
                        }
                    }
                

                foreach (string uId in userIds)
                {
                    if (!ststUsers.Any(x => x.USER_ID == uId && x.IS_DELETED == false))
                    {
                        TBL_STATION_USER tsu = new TBL_STATION_USER();
                        tsu.USER_ID = uId;
                        tsu.STATION_ID = stationId;
                        tsu.CREATED_DATE = DateTime.Now;
                        tsu.UPDATE_USER = User.Identity.GetUserId();
                        stationUserService.CreateStationUser(tsu);
                    }
                }
            }
            else
            {
                if (ststUsers != null)
                {
                    foreach (TBL_STATION_USER tsu in ststUsers)
                    {
                        tsu.UPDATE_USER = User.Identity.GetUserId();
                        stationUserService.DeleteStationUser(tsu.ID);
                    }
                }
            }


            stationUserService.SaveStationUser();
        }
        
        [HttpPost, ValidateInput(false)]
        public ActionResult UpdateStation(StationGridModel station)
        {
            int companyId = !string.IsNullOrEmpty(Request.Params["companyId"]) ? int.Parse(Request.Params["companyId"]) : 0;
            if (ModelState.IsValid)
            {
                try
                {               
                    TBL_STATION stat = Mapper.Map<StationGridModel, TBL_STATION>(station);
                    stat.COMPANY_ID = companyId;
                    if (!stationService.IsStationExist(stat))
                    {
                        stat.UPDATE_USER = User.Identity.GetUserId();
                
                        //SaveStationUsers(station.ID, station.USERS);

                        //if (station.PHOTO != null && station.PHOTO.Length > 0)
                        //{
                        //    string retVal = SaveStationPic(station.PHOTO, station.ID);
                        //    if (retVal.Length > 0)
                        //    {
                        //        ViewData["EditError"] = "Could not save picture. " + retVal;
                        //        ViewData["EditableProduct"] = station;
                        //    }
                        //    else
                        //    {
                        //        stat.PHOTO_PATH = stat.ID.ToString() + ".png";
                        //    }
                        //}
                        //else
                        //{
                        //    string retVal = DeleteStationPic(stat.ID);
                        //    if (retVal.Length > 0)
                        //    {
                        //        ViewData["EditError"] = "Could not delete picture. " + retVal;
                        //        ViewData["EditableProduct"] = station;
                        //    }
                        //    else
                        //    {
                        //        stat.PHOTO_PATH = "noimage.png";
                        //    }
                        //}
                        stationService.UpdateStation(stat);
                        stationService.SaveStation();
						UpdateTargetStation(station, companyId);


					}
                    else
                    {
                        ViewData["EditError"] = "Name is already taken";
                        ViewData["EditableProduct"] = station;
                    }                    
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
            {
                ViewData["EditError"] = companyId == 0 ? "Please select a company." : "Please, correct all errors.";
                ViewData["EditableProduct"] = station;
            }

            return PartialView("GridStationPartial", GetStationByCompanyId(companyId));
        }


        string DeleteStationPic(int stationId)
        {
            string retMessage = "";

            try
            {        
                string imgPath = Server.MapPath("~/images/StationImages/" + stationId.ToString() + ".png");
                
                if (System.IO.File.Exists(imgPath))
                {
                    System.IO.File.Delete(imgPath);
                }
            }
            catch (Exception ex)
            {
                retMessage = ex.Message;
            }
            return retMessage;
        }

        string SaveStationPic(byte[] imageBytes, int stationId)
        {
            string retMessage = "";

            try
            {
                MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
                ms.Write(imageBytes, 0, imageBytes.Length);
                System.Drawing.Image image = System.Drawing.Image.FromStream(ms, true);

                string savePath = Server.MapPath("~/images/StationImages/");

                if (!Directory.Exists(savePath))
                {
                    Directory.CreateDirectory(savePath);
                }
                DeleteStationPic(stationId);
                image.Save(savePath + "/" + stationId.ToString() + ".png", System.Drawing.Imaging.ImageFormat.Png);
                image.Dispose();
                ms.Dispose();
            }
            catch (Exception ex)
            {
                retMessage = ex.Message;
            }
            return retMessage;
        }

        public ActionResult BinaryImageColumnPhotoUpdate()
        {
            return BinaryImageEditExtension.GetCallbackResult();
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult DeleteStation(int ID)
        {
            int companyId = !string.IsNullOrEmpty(Request.Params["companyId"]) ? int.Parse(Request.Params["companyId"]) : 0;
            if (ID > 0)
            {
                try
                {
                    stationService.DeleteStation(ID,User.Identity.GetUserId());
                    stationService.SaveStation();
                    string retVal = DeleteStationPic(ID);
                    if (retVal.Length > 0)
                    {
                        ViewData["EditError"] = "Could not delete picture. " + retVal;
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            return PartialView("GridStationPartial", GetStationByCompanyId(companyId));
        }
        
        
    protected static void grid_InitNewRow(object sender, DevExpress.Web.Data.ASPxDataInitNewRowEventArgs e)
        {
            e.NewValues["INSTALL_DATE"] = DateTime.Now;
            e.NewValues["SIZE"] = 0;
            e.NewValues["IS_ACTIVE"] = true;
            e.NewValues["IS_LOCKED"] = false;
        }

        
        #region GridSettings
        public class GridHelper
        {
            static int companyId;
            public GridHelper(int _companyId)
            {
                companyId = _companyId;
            }

            static GridViewSettings gridStationSettings;
            public  GridViewSettings GridStationSettings
            {
                get
                {
                    if (gridStationSettings == null)
                        gridStationSettings = CreateStationGridSettings();
                    return gridStationSettings;
                }
            }
            static GridViewSettings CreateStationGridSettings()
            {
                EssoEntities DB2 = new EssoEntities();
                GridViewSettings settings = new GridViewSettings();

                settings.Name = "gvStation";
                settings.KeyFieldName = "ID";
                settings.CallbackRouteValues = new { Controller = "Station", Action = "GridStationPartial" };
                settings.Width = Unit.Percentage(100);


                settings.InitNewRow += grid_InitNewRow;
                settings.SettingsEditing.Mode = GridViewEditingMode.EditForm;
                settings.SettingsEditing.AddNewRowRouteValues = new { Controller = "Station", Action = "SaveStation" };
                settings.SettingsEditing.UpdateRowRouteValues = new { Controller = "Station", Action = "UpdateStation" };
                settings.SettingsEditing.DeleteRowRouteValues = new { Controller = "Station", Action = "DeleteStation" };

                settings.SettingsBehavior.ConfirmDelete = true;

                settings.SettingsBehavior.AllowGroup = true;
                settings.SettingsBehavior.AllowSort = true;

                settings.Settings.VerticalScrollBarMode = ScrollBarMode.Visible;
                settings.Settings.VerticalScrollableHeight = 500;


                settings.SettingsPager.PageSize = 20;
                settings.SettingsPager.Position = PagerPosition.Bottom;
                //settings.SettingsPager.FirstPageButton.Visible = true;
                //settings.SettingsPager.LastPageButton.Visible = true;

                //settings.SettingsPager.PageSizeItemSettings.Visible = true;
                //settings.SettingsPager.PageSizeItemSettings.Items = new string[] { "10", "20", "50" };

                settings.Settings.ShowFilterRow = true;
                settings.Settings.ShowFilterRowMenu = true;
                
                settings.ClientSideEvents.BeginCallback = "OnStationBeginCallback";

                //settings.Columns.Add(c =>
                //{
                //    c.FieldName = "PHOTO";
                //    c.EditorProperties().BinaryImage(p =>
                //    {
                //        p.ImageHeight = 170;
                //        p.ImageWidth = 160;
                //        p.EnableServerResize = true;
                //        p.ImageSizeMode = ImageSizeMode.FitProportional;
                //        p.CallbackRouteValues = new { Action = "BinaryImageColumnPhotoUpdate", Controller = "Station" };
                //        p.EditingSettings.Enabled = true;
                //        p.EditingSettings.UploadSettings.UploadValidationSettings.MaxFileSize = 4194304;
                //    });
                //});

                settings.Columns.Add(c =>
                {
                    c.FieldName = "NAME";
                    c.Caption = "Plant Name";
                });
                settings.Columns.Add(c =>
                { c.FieldName = "SIZE";
                    c.Caption = "Size"; c.Width = Unit.Pixel(150);
                });


                //settings.Columns.Add(c =>
                //{
                //    c.FieldName = "NAME";
                //    c.Caption = "Group Name";
                //    c.Visible = false;
                //    c.GroupIndex = 0;
                //    c.EditFormSettings.Visible = DefaultBoolean.False;

                //    c.EditorProperties().ComboBox(p =>
                //    {
                //    p.BindList(DB2.StationGroups.Where(x => x.IS_DELETED == false).Select(x => x.NAME).ToList());



                //    });
                //});

          
                settings.Columns.Add(column => {
                    column.FieldName = "GROUP_ID";
                    column.Caption = "Grup";
                    column.EditorProperties().ComboBox(p =>
                    {
                        p.TextField = "NAME";
                        p.ValueField = "ID";
                        p.ValueType = typeof(int);
                        p.DataSource = DB2.StationGroups.Where(x => x.IS_DELETED == false).Select(x => new { x.NAME,x.ID }).ToList();
                    });
                });


                //settings.Columns.Add(column =>
                //{
                //    column.FieldName = "GROUP_ID";
                //    column.Caption = "Group";
                //    //column.GroupIndex = 0;
                //    //column.EditFormSettings.Visible = DefaultBoolean.False;

                //    //column.EditorProperties().ComboBox(p =>
                //    //{
                //    //    p.BindList(DB2.StationGroups.Where(x => x.IS_DELETED == false).Select(x => x.NAME).ToList());

                //    //});
                 
                    

                //});

                settings.Columns.Add(column =>
                {
                    column.FieldName = "USERS";
                    column.Caption = "Users";
                    column.EditFormSettings.Visible = DefaultBoolean.False;
                });

                settings.Columns.Add(column =>
                {
                    column.FieldName = "IP_ADDRESS";
                    column.Caption = "Ip Address";
                });

                settings.Columns.Add(column =>
                {
                    column.FieldName = "PORT";
                    column.Caption = "PORT";
                });


                settings.Columns.Add(column =>
                {
                    column.FieldName = "ALARM_TEMP_ID";
                    column.Caption = "Alarm Tipi";
                    column.Visible = false;
                    column.EditFormSettings.Visible = DefaultBoolean.True;
                });

                settings.Columns.Add(column =>
                {
                    column.FieldName = "IS_ACTIVE";
                    column.Caption = "Active";
                    column.Width = Unit.Pixel(50);
                    column.ColumnType = MVCxGridViewColumnType.CheckBox;
                });
                settings.Columns.Add(column =>
                {
                    column.FieldName = "IS_LOCKED";
                    column.Caption = "Locked";
                    column.Width = Unit.Pixel(50);
                    column.ColumnType = MVCxGridViewColumnType.CheckBox;
                });
                settings.Columns.Add(c => { c.FieldName = "INSTALL_DATE"; c.Caption = "Installation Date"; c.Width = Unit.Pixel(200); });
                settings.Columns.Add(c => { c.FieldName = "CREATED_DATE"; c.Caption = "Inserted Date"; c.Width = Unit.Pixel(200); }); //"CREATED_DATE").SortOrder = DevExpress.Data.ColumnSortOrder.Ascending;

                settings.Columns.Add(column =>
                {
                    column.FieldName = "ADDRESS";
                    column.Caption = "Address";
                    column.EditFormSettings.Visible = DefaultBoolean.False;
                    column.Visible = false;
                });

                settings.Columns.Add(column =>
                {
                    column.FieldName = "DESCRIPTION";
                    column.Caption = "Desription";
                    column.EditFormSettings.Visible = DefaultBoolean.False;
                    column.Visible = false;
                });
                settings.Columns.Add(column =>
                {
                    column.FieldName = "PLC_INTERFACE";
                    column.Caption = "PLC INTERFACE";
                    column.EditFormSettings.Visible = DefaultBoolean.False;
                    column.Visible = false;
                });
                settings.Columns.Add(column =>
                {
                    column.FieldName = "EKK_INTERFACE";
                    column.Caption = "EKK INTERFACE";
                    column.EditFormSettings.Visible = DefaultBoolean.False;
                    column.Visible = false;
                });

                settings.Columns.Add(column =>
                {
                    column.FieldName = "IRRADIATION_SCALE";
                    column.Caption = "Irradiation Scale(%)";
                    column.EditFormSettings.Visible = DefaultBoolean.False;
                    column.Visible = false;
                });

                settings.BeforeGetCallbackResult = (sender, e) =>
                {
                    MVCxGridView grid = (MVCxGridView)sender;
                    grid.GroupBy(grid.Columns["GROUP_ID"]);
                };


                settings.PreRender = (sender, e) =>
                {
                    ASPxGridView grid = (ASPxGridView)sender;
                    grid.ExpandRow(0);
                };

                return settings;
            }
        }
        #endregion
    }
}