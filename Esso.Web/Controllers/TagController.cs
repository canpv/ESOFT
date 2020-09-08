using Esso.Data;
using DevExpress.Web;
using System;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Esso.Service;
using Esso.Models;
using DevExpress.Web.Mvc;
using Z.EntityFramework.Plus;
using Esso.Web.ViewModels;
using AutoMapper;
using System.Data.Entity;

namespace Esso.Web.Controllers
{
    public class TagController : BaseController
    {

        public TagController()
        {
        }

        EssoEntities DB = new EssoEntities();



        public ActionResult GridTagPartial()
        {


            return PartialView();

        }
        private object companyService;

        public object NorthwindDataProvider { get; private set; }

        // GET: Tag
        public ActionResult Index()
        {
            return View();
        }


        private ActionResult PartialView(string v, object p)
        {
            throw new NotImplementedException();
        }

        public class TempDTO
        {
            public int ID { get; set; }
            public string NAME { get; set; }
        }
        

        public ActionResult TagGridUpdateTemp(MVCxGridViewBatchUpdateValues<TagDTO, int> updateValues)
        {
            string curUserId = User.Identity.GetUserId();
            foreach (TagDTO tagDto in updateValues.Insert)
            {
                if (updateValues.IsValid(tagDto))
                {
              
                    TBL_TAG tag = new TBL_TAG();
                    tag = Mapper.Map<TagDTO, TBL_TAG>(tagDto);
                    tag.CREATED_DATE = DateTime.Now;
                    if (tag.IS_DIGITAL == null)
                    {
                        tag.IS_DIGITAL = false;
                    }
                    DB.Tags.Add(tag);
                   
                }
            }
         
            foreach (TagDTO tagDto in updateValues.Update)
            {
                if (updateValues.IsValid(tagDto))
                {
                    TBL_TAG ent = DB.Tags.FirstOrDefault(x => x.ID == tagDto.ID);

                    //ent = Mapper.Map<TagDTO, TBL_TAG>(tagDto);
                    ent.NAME = tagDto.NAME;
                    ent.UPDATED_DATE = DateTime.Now;
                    ent.UPDATE_USER = curUserId;
					ent.IS_DIGITAL = tagDto.IS_DIGITAL;
					ent.IS_INV_TAG = tagDto.IS_INV_TAG.Value;
					ent.IS_STRING = tagDto.IS_STRING;

					DB.Entry(ent).State = EntityState.Modified;
					DB.SaveChanges();

					//DB.Tags.Attach(ent);
                    
                }
            }
           
            foreach (var tagIg in updateValues.DeleteKeys)
            {
                TBL_TAG ent = DB.Tags.FirstOrDefault(x => x.ID == tagIg);
                ent.IS_DELETED = true;
               
            }
            DB.SaveChanges();
            return PartialView("GridTagPartial");



        }
    }
}
    
