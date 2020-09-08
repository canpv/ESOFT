using DevExpress.Web;
using DevExpress.Web.Mvc;
using Esso.Models;
using Esso.Service;
using Microsoft.AspNet.Identity;
using System;
using System.Data.Entity.Validation;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace Esso.Web.Controllers
{
    [Authorize]
    public class TempController : BaseController
    {
        
        public TempController()
        {
        }

        // GET: Inverter
        public ActionResult Index()
        {
            return View();
        }

      
    }
}