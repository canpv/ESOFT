using Esso.Models;
using Esso.Service;
using Microsoft.AspNet.Identity;
using System;
using System.Data.Entity.Validation;
using System.Web.Mvc;

namespace Esso.Web.Controllers
{
    [Authorize]
    public class InverterController : BaseController
    {

        private readonly IInverterService inverterService;
        private readonly IInvAddressService invAddressService;
        private readonly ITagService tagService;

        public InverterController(IInverterService inverterService, IInvAddressService invAddressService, ITagService tagService)
        {
            this.inverterService = inverterService;
            this.invAddressService = invAddressService;
            this.tagService = tagService;
        }

        // GET: Inverter
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GridInverterPartial(int stationId)
        {
            return PartialView("GridInverterPartial", stationId);
        }

        //public ActionResult InvAdressDetailPartial(int inverterId)
        //{
        //    List<InvAdressDetailModel> models = new List<InvAdressDetailModel>();
        //    List<TBL_INV_ADDRESS> invAddresses= invAddressService.GetInvAddressByInvId(inverterId);
        //    List<TBL_TAG> tags = tagService.GetTags(x => x.IS_INV_TAG && x.IS_DELETED == false);

        //    if ((tags != null && tags.Count > 0) && (invAddresses != null && invAddresses.Count > 0))
        //    {
        //        foreach (TBL_TAG tag in tags)
        //        {
        //            InvAdressDetailModel detModel = new InvAdressDetailModel();
        //            detModel.ID = tag.ID;
        //            detModel.ADDRESS = invAddresses.Where(x => x.TAG_ID == tag.ID).Select(x => x.ADDRESS).FirstOrDefault();
        //            detModel.TAG_NAME = tag.NAME;

        //            models.Add(detModel);
        //        }
        //    }

        //    ViewData["invId"] = inverterId;
        //    return PartialView("InvAdressDetailPartial", models);
        //}
        
        [HttpPost, ValidateInput(false)]
        public ActionResult Create(TBL_INVERTER inverter)
        {
            int stationId = !string.IsNullOrEmpty(Request.Params["stationId"]) ? int.Parse(Request.Params["stationId"]) : 0;
            if (ModelState.IsValid && stationId > 0)
            {
                try
                {
                    inverter.STATION_ID = stationId;
                    if (!inverterService.IsInverterExist(inverter))
                    {
                        inverter.CREATED_DATE = DateTime.Now;
                        inverter.UPDATE_USER = User.Identity.GetUserId();
                        inverterService.CreateInverter(inverter);
                        inverterService.SaveInverter();                        
                    }
                    else
                    {
                        ViewData["EditableProduct"] = inverter;
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
                ViewData["EditableProduct"] = inverter;
            }
            return PartialView("GridInverterPartial", stationId);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(TBL_INVERTER inverter)
        {

            int stationId = !string.IsNullOrEmpty(Request.Params["stationId"]) ? int.Parse(Request.Params["stationId"]) : 0;

            if (ModelState.IsValid)
            { 
                try
                {
                    
                    inverter.STATION_ID = stationId;
                    if (!inverterService.IsInverterExist(inverter))
                    {
                        inverterService.UpdateInverter(inverter);
                        inverter.UPDATE_USER = User.Identity.GetUserId();
                        inverterService.SaveInverter();
                    }
                    else
                    {
                        ViewData["EditableProduct"] = inverter;
                        ViewData["EditError"] = "Inverter is already taken";
                    }

                    return PartialView("GridInverterPartial", stationId);
                }
                catch (DbEntityValidationException e)
                {
                    string errorStr = string.Empty;
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        errorStr = string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                            eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        foreach (var ve in eve.ValidationErrors)
                        {
                            errorStr += string.Format("\n - Property: \"{0}\", Error: \"{1}\"",
                                ve.PropertyName, ve.ErrorMessage);
                        }
                    }
                    ViewData["EditError"] = errorStr;
                    return PartialView("GridInverterPartial", stationId);
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                    return PartialView("GridInverterPartial", stationId);
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            return PartialView("GridInverterPartial", stationId);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Delete(int Id,int stationId)
        {
            if (Id > 0)
            {
                try
                {
                    inverterService.DeleteInverter(Id, User.Identity.GetUserId());
                    inverterService.SaveInverter();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            return PartialView("GridInverterPartial",stationId);
        }

      
    }
}