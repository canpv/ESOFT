using AutoMapper;
using DevExpress.Web;
using DevExpress.Web.Mvc;
using Esso.Models;
using Esso.Service;
using Esso.ViewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace Esso.Web.Controllers
{
    [Authorize]
    public class StationGroupController : BaseController
    {
        private readonly IStationGroupService stationGroupService;

        public StationGroupController(IStationGroupService stationGroupService)
        {
            this.stationGroupService = stationGroupService;
        }
        
        public ActionResult Index()
        {
            return View();
        }

        [ValidateInput(false)]
        public ActionResult CreateStationGroupPartial(int companyId)
        {
            StationGroupViewModel st = new StationGroupViewModel();
            st.COMPANY_ID = companyId;
            return PartialView(st);
        }

        [ValidateInput(false)]
        public JsonResult SaveStationGroup(StationGroupViewModel stg)
        {
            try
            {
                TBL_STATION_GROUP entity = Mapper.Map<StationGroupViewModel, TBL_STATION_GROUP>(stg);
                entity.UPDATE_USER = User.Identity.GetUserId();
                entity.CREATED_DATE = DateTime.Now;
                stationGroupService.CreateStationGroup(entity);
                stationGroupService.SaveStationGroup();
                return Json("");
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }
        

        //List<StationGroupl> GetStationGroupByCompanyId(int companyId)
        //{
        //    List<StationGroup> statEntities = stationGroupService.GetStationGroupByCompanyId(companyId).ToList();
        //    List<StationGroupGridModel> stats = Mapper.Map<List<StationGroup>, List<StationGroupGridModel>>(statEntities);
        //    return stats;
        //}

        //[HttpPost, ValidateInput(false)]
        //public ActionResult SaveStationGroup(StationGroup stationGroup)
        //{
        //    int companyId = !string.IsNullOrEmpty(Request.Params["companyId"]) ? int.Parse(Request.Params["companyId"]) : 0;
        //    if (ModelState.IsValid && companyId > 0)
        //    {
        //        try
        //        {
        //            stationGroup.COMPANY_ID = companyId;
        //            if (!stationGroupService.IsStationGroupExist(stationGroup))
        //            {                       
        //                stationGroup.CREATED_DATE = DateTime.Now;
        //                stationGroup.UPDATE_USER = User.Identity.GetUserId();
        //                stationGroupService.CreateStationGroup(stationGroup);
        //                stationGroupService.SaveStationGroup();
        //            }
        //            else
        //            {
        //                ViewData["EditableProduct"] = stationGroup;
        //                ViewData["EditError"] = "Name is already taken";
        //            }                   
        //        }
        //        catch (Exception e)
        //        {
        //            ViewData["EditError"] = e.Message;
        //        }
        //    }
        //    else
        //    {
        //        ViewData["EditError"] = "Please, correct all errors.";
        //        ViewData["EditableProduct"] = stationGroup;
        //    }
        //    return PartialView("GridStationGroupPartial", GetStationGroupByCompanyId(companyId));
        //}

        //[HttpPost, ValidateInput(false)]
        //public ActionResult UpdateStationGroup(StationGroup stationGroup)
        //{
        //    int companyId = !string.IsNullOrEmpty(Request.Params["companyId"]) ? int.Parse(Request.Params["companyId"]) : 0;
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            stationGroup.COMPANY_ID = companyId;
        //            if (!stationGroupService.IsStationGroupExist(stationGroup))
        //            {
        //                stationGroup.UPDATE_USER = User.Identity.GetUserId();
        //                stationGroupService.UpdateStationGroup(stationGroup);
        //                stationGroupService.SaveStationGroup();
        //            }
        //            else
        //            {
        //                ViewData["EditError"] = "Name is already taken";
        //                ViewData["EditableProduct"] = stationGroup;
        //            }                    
        //        }
        //        catch (Exception e)
        //        {
        //            ViewData["EditError"] = e.Message;
        //        }
        //    }
        //    else
        //    {
        //        ViewData["EditError"] = companyId == 0 ? "Please select a company." : "Please, correct all errors.";
        //        ViewData["EditableProduct"] = stationGroup;
        //    }

        //    return PartialView("GridStationGroupPartial", GetStationGroupByCompanyId(companyId));
        //}
        //[HttpPost, ValidateInput(false)]
        //public ActionResult DeleteStationGroup(int ID)
        //{
        //    int companyId = !string.IsNullOrEmpty(Request.Params["companyId"]) ? int.Parse(Request.Params["companyId"]) : 0;
        //    if (ID > 0)
        //    {
        //        try
        //        {
        //            stationGroupService.DeleteStationGroup(ID,User.Identity.GetUserId());
        //            stationGroupService.SaveStationGroup();
        //        }
        //        catch (Exception e)
        //        {
        //            ViewData["EditError"] = e.Message;
        //        }
        //    }
        //    return PartialView("GridStationGroupPartial", GetStationGroupByCompanyId(companyId));
        //}










        //[HttpPost, ValidateInput(false)]
        //public ActionResult BinaryImageColumnUpdatePartial(StationGroup employee)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            //NorthwindDataProvider.UpdateEditableEmployee(employee);
        //        }
        //        catch (Exception e)
        //        {
        //            ViewData["EditError"] = e.Message;
        //        }
        //    }
        //    else
        //    {
        //        ViewData["EditError"] = "Please, correct all errors.";
        //    }

        //    return PartialView("BinaryImageColumnpartial");
        //}
        //public ActionResult BinaryImageColumnPhotoUpdate()
        //{
        //    return BinaryImageEditExtension.GetCallbackResult();
        //}



    }
}