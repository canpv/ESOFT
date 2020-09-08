using Esso.Data;
using Esso.Web.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Esso.Web.Controllers
{
    public class ProfileController : BaseController
    {
        EssoEntities DB = new EssoEntities();
        // GET: Profile
        public ActionResult Index()
        {
            return View();
        }
        public class UserPageView
        {
            public ApplicationUser __user { get; set; }
            public string __ErrorMessage { get; set; }
        }
        //public async Task<ActionResult> ChangeReportMail()
        //{
        //    var userId = User.Identity.GetUserId();
        //    UserPageView _result = new UserPageView();
        //    IdentityResult result = new IdentityResult();
        //    _result.__user = UserManager.Users.Where(x => x.IS_DELETED == false && x.Id == Id).FirstOrDefault();
        //    _result.__user.REPORT_SEND_MAIL = bool;
        //    UserManager.Update(_result.__user);
        //    DB.SaveChanges();
        //    return Json(null, JsonRequestBehavior.AllowGet);
        //}
    }
}