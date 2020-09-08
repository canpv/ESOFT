using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Esso.Web.Models;
using Esso.Web.App_Start;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Web.Mvc;
using System.Collections.Generic;
using AutoMapper;
using Esso.Web.ViewModels;
using Esso.Service;
using Esso.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using Esso.ViewModels;
using DevExpress.Utils;
using Esso.Data;
using System.Data.Entity;
using System.Net.Mail;
using System.Text;
using System.Net;
using Esso.Web.Helpers;
using System.Data.SqlClient;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Text.RegularExpressions;

namespace Esso.Web.Controllers
{
    //[Authorize]
    public class AccountController : BaseController
    {
        EssoEntities DB = new EssoEntities();

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        private readonly ICompanyService companyService;
        private readonly IStationService stationService;
        private readonly ICompanyUserService companyUserService;
        private readonly IStationUserService stationUserService;

        public AccountController(
            ICompanyService companyService,
            ICompanyUserService companyUserService,
            IStationService stationService,
            IStationUserService stationUserService
            )
        {
            this.companyService = companyService;
            this.companyUserService = companyUserService;
            this.stationService = stationService;
            this.stationUserService = stationUserService;
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        [Authorize]
        public ActionResult UserBasedInsertion()
        {
            return View();
        }

        public class STATION_USER_DTO
        {
            public int stationId { get; set; }
            public string stationName { get; set; }
            public bool isAuthorization { get; set; }
        }

        [Authorize]
        [HttpPost]
        public JsonResult GetUserStationList(string stUserId)
        {
            List<STATION_USER_DTO> allStations = new List<STATION_USER_DTO>();
            var userId = User.Identity.GetUserId();
            //List<IdentityUserRole> userRoles = UserManager.Users.Where(x => x.Id == userId).FirstOrDefault().Roles.ToList();
            //List<ApplicationUser> users = UserManager.Users.Where(x => x.IS_DELETED == false).ToList();
            string _role = DB.Database.SqlQuery<string>("select \"AspNetRoles\".\"Name\" from \"AspNetUserRoles\" left join \"AspNetRoles\" on \"AspNetUserRoles\".\"RoleId\" = \"AspNetRoles\".\"Id\" where \"AspNetUserRoles\".\"UserId\" = '" + userId + "'")
                            .FirstOrDefault();
            List<TBL_STATION_USER> suList = new List<TBL_STATION_USER>();
            List<TBL_STATION> stationList = new List<TBL_STATION>();
            if (_role == "M_ADMIN")
            {
                stationList = DB.Stations.Where(a => a.IS_DELETED == false).ToList();
                suList = DB.StationUsers.Where(a => a.IS_DELETED == false && a.USER_ID == stUserId).ToList();
            }
            else
            {
                int[] UserCompIds = DB.CompanyUsers.Where(a => a.USER_ID == userId).Select(a => a.COMPANY_ID).ToArray();
                stationList = DB.Stations.Where(a => a.IS_DELETED == false && UserCompIds.Contains(a.COMPANY_ID)).ToList();
                suList = DB.StationUsers.Where(a => a.IS_DELETED == false && a.USER_ID == stUserId).ToList();
            }


            foreach (var item in stationList)
            {
                if (suList.Where(a => a.STATION_ID == item.ID).FirstOrDefault() != null)
                {
                    allStations.Add(new STATION_USER_DTO { stationId = item.ID, stationName = item.NAME, isAuthorization = true });
                }
                else
                {
                    allStations.Add(new STATION_USER_DTO { stationId = item.ID, stationName = item.NAME, isAuthorization = false });
                }
            }

            return Json(allStations.OrderBy(a => a.stationName), JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [HttpPost]
        public JsonResult GetUserList()
        {
            var userId = User.Identity.GetUserId();
            //Int16 _authority = DB.Database.SqlQuery<Int16>("select \"AspNetUsers\".\"USER_AUTHORITY\" from \"AspNetUsers\" where \"AspNetUsers\".\"Id\" = '" + userId + "'")
            //              .FirstOrDefault();
            //string _role = DB.Database.SqlQuery<string>("select \"AspNetRoles\".\"Name\" from \"AspNetUserRoles\" left join \"AspNetRoles\" on \"AspNetUserRoles\".\"RoleId\" = \"AspNetRoles\".\"Id\" where \"AspNetUserRoles\".\"UserId\" = '" + userId + "'")
            //                .FirstOrDefault();
            List<ApplicationUser> usrList = new List<ApplicationUser>();
            if (User.IsInRole("M_ADMIN") || User.IsInRole("COMP_ADMIN"))
            {
                List<ApplicationUser> users = UserManager.Users.Where(x => x.IS_DELETED == false).ToList();
                string[] userIds;
                if (User.IsInRole("M_ADMIN"))
                {
                    //userIds = DB.StationUsers.Where(a => a.IS_DELETED == false).GroupBy(grp => grp.USER_ID).Select(a => a.Key).ToArray();
                    userIds = DB.Database.SqlQuery<string>("select \"AspNetUsers\".\"Id\" from \"AspNetUsers\" left join \"AspNetUserRoles\" on \"AspNetUsers\".\"Id\" = \"AspNetUserRoles\".\"UserId\" where \"AspNetUsers\".\"IS_DELETED\" = 0  and \"AspNetUserRoles\".\"RoleId\" = 'd033ae02-905b-424a-b1f3-59657a76b4a1'").ToArray();
                    try
                    {
                        usrList = DB.Database.SqlQuery<ApplicationUser>("select \"AspNetUsers\".\"Id\",\"AspNetUsers\".\"UserName\",\"AspNetUsers\".\"Email\",\"AspNetUsers\".\"SHOW_MONEY\",\"AspNetUsers\".\"REPORT_SEND_MAIL\",\"AspNetUsers\".\"ALARM_SEND_MAIL\" from \"AspNetUsers\" left join \"AspNetUserRoles\" on \"AspNetUsers\".\"Id\" = \"AspNetUserRoles\".\"UserId\" where \"AspNetUsers\".\"IS_DELETED\" = 0  and \"AspNetUserRoles\".\"RoleId\" = 'd033ae02-905b-424a-b1f3-59657a76b4a1' or \"AspNetUserRoles\".\"RoleId\" = 'abc37a3f-b315-4a32-b487-d2e8b67a19b2' order by \"AspNetUsers\".\"UserName\" asc")
                                       .Select(a =>
                                       new ApplicationUser
                                       {
                                           Id = a.Id,
                                           UserName = a.UserName,
                                           Email = a.Email,
                                           SHOW_MONEY = a.SHOW_MONEY,
                                           REPORT_SEND_MAIL = a.REPORT_SEND_MAIL,
                                           ALARM_SEND_MAIL = a.ALARM_SEND_MAIL
                                       }).OrderBy(a => a.UserName).ToList();
                    }
                    catch (Exception ex)
                    {

                    }
                }
                else if (User.IsInRole("COMP_ADMIN"))
                {
                    int[] UserCompIds = DB.CompanyUsers.Where(a => a.USER_ID == userId).Select(a => a.COMPANY_ID).ToArray();
                    int[] UserStatIds = DB.Stations.Where(a => a.IS_DELETED == false && UserCompIds.Contains(a.COMPANY_ID)).Select(a => a.ID).ToArray();
                    userIds = DB.StationUsers.Where(a => a.IS_DELETED == false && UserStatIds.Contains(a.STATION_ID)).GroupBy(grp => grp.USER_ID).Select(a => a.Key).ToArray();

                    string userIdsSql = string.Empty;

                    foreach (var item in userIds)
                    {
                        var sayi = userIds.Length;
                        if (item == userIds[userIds.Length - 1])
                        {
                            userIdsSql += "'" + item + "'";
                        }
                        else
                        {
                            userIdsSql += "'" + item + "',";
                        }
                    }
                    string userIdsSql2 = string.Empty;
                    userIdsSql2 = "(" + userIdsSql.Replace("\"", "'") + ")";
                    try
                    {
                        //usrList = DB.Database.SqlQuery<ApplicationUser>("select \"AspNetUsers\".\"Id\",\"AspNetUsers\".\"UserName\",\"AspNetUsers\".\"Email\",\"AspNetUsers\".\"SHOW_MONEY\",\"AspNetUsers\".\"REPORT_SEND_MAIL\",\"AspNetUsers\".\"ALARM_SEND_MAIL\" from \"AspNetUsers\" left join \"AspNetUserRoles\" on \"AspNetUsers\".\"Id\" = \"AspNetUserRoles\".\"UserId\" where \"AspNetUsers\".\"IS_DELETED\" = 0  and \"AspNetUserRoles\".\"RoleId\" = 'd033ae02-905b-424a-b1f3-59657a76b4a1' and \"AspNetUsers\".\"Id\" in " + userIdsSql2 + " or \"AspNetUsers\".\"CREATE_USER\"='" + userId + "' order by \"AspNetUsers\".\"UserName\" asc")
                        usrList = DB.Database.SqlQuery<ApplicationUser>("select \"AspNetUsers\".\"Id\",\"AspNetUsers\".\"UserName\",\"AspNetUsers\".\"Email\",\"AspNetUsers\".\"SHOW_MONEY\",\"AspNetUsers\".\"REPORT_SEND_MAIL\",\"AspNetUsers\".\"ALARM_SEND_MAIL\" from \"AspNetUsers\" left join \"AspNetUserRoles\" on \"AspNetUsers\".\"Id\" = \"AspNetUserRoles\".\"UserId\" where \"AspNetUsers\".\"IS_DELETED\" = 0  and \"AspNetUserRoles\".\"RoleId\" = 'd033ae02-905b-424a-b1f3-59657a76b4a1' and \"AspNetUsers\".\"CREATE_USER\"='" + userId + "' order by \"AspNetUsers\".\"UserName\" asc")
                        .Select(a =>
                                       new ApplicationUser
                                       {
                                           Id = a.Id,
                                           UserName = a.UserName,
                                           Email = a.Email,
                                           SHOW_MONEY = a.SHOW_MONEY,
                                           REPORT_SEND_MAIL = a.REPORT_SEND_MAIL,
                                           ALARM_SEND_MAIL = a.ALARM_SEND_MAIL
                                       }).OrderBy(a => a.UserName).ToList();
                    }
                    catch (Exception ex)
                    {

                    }
                }


            }

            return Json(usrList, JsonRequestBehavior.AllowGet);
        }
        public class UserPageView
        {
            public ApplicationUser __user { get; set; }
            public string __ErrorMessage { get; set; }
        }
        private bool EmailKontrol(string email)
        {
            string patternStrict = @"^(([^<>()[\]\\.,;:\s@\""]+"
                 + @"(\.[^<>()[\]\\.,;:\s@\""]+)*)|(\"".+\""))@"
                 + @"((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}"
                 + @"\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+"
                 + @"[a-zA-Z]{2,}))$";
            Regex reStrict = new Regex(patternStrict);
            bool isStrictMatch = reStrict.IsMatch(email);
            return isStrictMatch;
        }

        public class StationPageView
        {
            public TBL_STATION_USER __stUser { get; set; }
            public string __ErrorMessage { get; set; }
        }
        public JsonResult SaveStation(string userId, int[] stListId)
        {
            StationPageView spv = new StationPageView();
            try
            {
                var creatUserId = User.Identity.GetUserId();
                var _stationUserDelete = DB.StationUsers.Where(a => a.IS_DELETED == false && a.USER_ID == userId && !stListId.Contains(a.STATION_ID)).ToList();
                foreach (var item in stListId)
                {
                    var _stationUser = DB.StationUsers.Where(a => a.IS_DELETED == false && a.USER_ID == userId && a.STATION_ID == item).ToList();
                    if (_stationUser.Count == 0)
                    {
                        //insert
                        DB.StationUsers.Add(new TBL_STATION_USER { STATION_ID = item, USER_ID = userId, UPDATE_USER = creatUserId, CREATED_DATE = DateTime.Now, IS_DELETED = false });
                        DB.SaveChanges();
                    }
                }
                foreach (TBL_STATION_USER item in _stationUserDelete)
                {
                    //Delete
                    EssoEntities DB = new EssoEntities();
                    //StationPageView sp = new StationPageView();
                    TBL_STATION_USER aa = item;
                    aa.IS_DELETED = true;
                    aa.UPDATED_DATE = DateTime.Now;
                    DB.Entry(aa).State = EntityState.Modified;
                    DB.SaveChanges();
                }
                spv.__ErrorMessage = "";
            }
            catch (Exception ex)
            {
                spv.__ErrorMessage = "Başarısız Kayıt!";
            }


            return Json(spv, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public async Task<ActionResult> SaveUser(string Id, string Name, string Email, string ShowMoney, string ReportSend, string AlarmSend, string CheckPassword, string Password)
        {
            var creatUserId = User.Identity.GetUserId();
            UserPageView _result = new UserPageView();
            int? userCount = 0;

            if (User.IsInRole("COMP_ADMIN"))
            {
                userCount = DB.Database.SqlQuery<int>("select Count(\"AspNetUsers\".\"CREATE_USER\") from \"AspNetUsers\" where \"AspNetUsers\".\"IS_DELETED\" = 0 and \"AspNetUsers\".\"CREATE_USER\"='" + creatUserId + "' ").Count();
            }
            else
            {
                userCount = 0;
            }

            bool kontrolEmail = EmailKontrol(Email);
            _result.__ErrorMessage = "";
            bool _ShowMoney = bool.Parse(ShowMoney);
            bool _ReportSend = bool.Parse(ReportSend);
            bool _AlarmSend = bool.Parse(AlarmSend);
            string _userName = Name.Trim();

            if (Id != "" && Name.Trim() != "" && Email.Trim() != "")
            {
                if (Id.Trim() == "0")
                {
                    if (userCount <= 5)
                    {
                        if (kontrolEmail)
                        {
                            if (Password.Trim() != "" && Password.Trim().Length > 5)
                            {
                                ApplicationUser record = UserManager.Users.FirstOrDefault(x => x.UserName == _userName && x.IS_DELETED == false);
                                if (record == null)
                                {
                                    var user = new ApplicationUser
                                    {
                                        UserName = _userName,
                                        Email = Email,
                                        CREATE_USER = creatUserId,
                                        UPDATE_USER = creatUserId,
                                        IS_DELETED = false,
                                        CREATED_DATE = DateTime.Now,
                                        SHOW_MONEY = Convert.ToInt16(_ShowMoney),
                                        REPORT_SEND_MAIL = Convert.ToInt16(_ReportSend),
                                        ALARM_SEND_MAIL = Convert.ToInt16(_AlarmSend),
                                        IS_DEMO = false
                                    };
                                    string _password = Password.Trim();
                                    var result = await UserManager.CreateAsync(user, _password);

                                    if (result.Succeeded)
                                    {
                                        string newId = user.Id;
                                        string role = "d033ae02-905b-424a-b1f3-59657a76b4a1";
                                        string isRole = DB.Database.SqlQuery<string>("select \"AspNetUserRoles\".\"RoleId\" from \"AspNetUserRoles\" where \"AspNetUserRoles\".\"UserId\" = '" + newId + "'").FirstOrDefault();

                                        if (isRole == null)
                                        {
                                            DB.Database.ExecuteSqlCommand(
                                            "insert into \"AspNetUserRoles\"(\"UserId\",\"RoleId\") values ('" + newId + "','" + role + "')");

                                            string content = "<i style='color:Black'><h3>Merhaba,</h3></i>" +
                                                            "<i style='color:Black'><h3>Güneş enerji santraliniz E-SOFT’da izlemeye açılmıştır. www.e-soft.com.tr ‘dan ya da Android-IOS için E-SOFT uygulamasını indirerek giriş yapabilirsiniz.</h3></i><br>" +
                                                            "<i style='color:#00629e'><h3><b style='color:Black'>Kullanıcı Adı :</b>" + user.UserName + "</h3></i> " +
                                                            "<i style='color:#00629e'><h3><b style='color:Black'> Şifre :</b>" + Password + " </h3></i><br>" +
                                                            "<i style='color:Black'><h3>Bol Güneşli Günler Dileriz….</h3></i><br><br>" +
                                                            "<img src = 'http://www.e-soft.com.tr/images/EsoftLogo/esso.png' height ='80px'><br>" +
                                                            "<i style='color:#00629e'><h3>MERKEZ:<br>Uzayçağı Cad. 1269.Sok.No:23 - 25<br>Ostim Yeni Mahalle / ANKARA<br>ŞUBE:<br>Mersinli Mah. 1201 / 1 Sokak No:2 Kat: 1<br>KONAK / İZMİR<br>TEL: +90 312 387 52 80<br>FAKS: +90 312 387 52 81 </h3></i><br><br> ";

                                            string content2 = "<i style='color:#00629e'><h3><b style='color:Black'>Kullanıcı Adı :</b>" + user.UserName + "</h3></i> " +
                                                            "<i style='color:#00629e'><h3><b style='color:Black'> Şifre :</b>" + Password + " </h3></i><br>" +
                                                            "<img src = 'http://www.e-soft.com.tr/images/EsoftLogo/esso.png' height ='80px'><br>" +
                                                            "<i style='color:#00629e'><h3>MERKEZ:<br>Uzayçağı Cad. 1269.Sok.No:23 - 25<br>Ostim Yeni Mahalle / ANKARA<br>ŞUBE:<br>Mersinli Mah. 1201 / 1 Sokak No:2 Kat: 1<br>KONAK / İZMİR<br>TEL: +90 312 387 52 80<br>FAKS: +90 312 387 52 81 </h3></i><br><br> ";

                                            SendMail(user.Email, content);
                                            SendMail("gizem.gungordu@esso.com.tr", content2);
                                            _result.__user = user;
                                        }
                                        else
                                        {
                                            _result.__ErrorMessage = "Rol hatası";
                                        }
                                    }
                                    else
                                    {
                                        _result.__ErrorMessage = "Kayıtlı Email adresi mevcut!";
                                    }

                                }
                                else
                                {
                                    _result.__ErrorMessage = "Kullanıcı adı mevcut.";
                                }
                            }
                            else
                            {
                                _result.__ErrorMessage = "Geçersiz Şifre";
                            }
                        }
                        else
                        {
                            _result.__ErrorMessage = "Geçersiz Email";
                        }

                    }
                    else
                    {
                        _result.__ErrorMessage = "5 Kullanıcıdan Fazla Ekleyemezsiniz.";
                    }
                }
                else
                {
                    //ApplicationUser recordUpdate = UserManager.Users.FirstOrDefault(x => x.Id == Id && x.IS_DELETED == false);
                    ApplicationUser isName = UserManager.Users.Where(x => x.Id != Id && x.IS_DELETED == false && x.UserName.ToUpper()==_userName.ToUpper()).FirstOrDefault();
                    if (isName == null)
                    {
                        if (CheckPassword == "true")
                        {
                            if (Password.Trim() != "" && Password.Trim().Length > 5)
                            {
                                if (kontrolEmail)
                                {
                                    // record = UserManager.Users.FirstOrDefault(x => x.Id != Id && x.IS_DELETED == false);
                                    ApplicationUser recordUpdate = UserManager.Users.FirstOrDefault(x => x.Id == Id && x.IS_DELETED == false);
                                    IdentityResult result = new IdentityResult();
                                    string resetToken = UserManager.GeneratePasswordResetToken(recordUpdate.Id);
                                    string _password = Password.Trim();
                                    result = UserManager.ResetPassword(recordUpdate.Id, resetToken, _password);

                                    _result.__user = UserManager.Users.Where(x => x.IS_DELETED == false && x.Id == Id).FirstOrDefault();
                                    _result.__user.Email = Email.Trim() == "" ? null : Email.Trim();
                                    _result.__user.UserName = _userName;
                                    _result.__user.SHOW_MONEY = Convert.ToInt16(_ShowMoney);
                                    _result.__user.REPORT_SEND_MAIL = Convert.ToInt16(_ReportSend);
                                    _result.__user.ALARM_SEND_MAIL = Convert.ToInt16(_AlarmSend);
                                    UserManager.Update(_result.__user);
                                    DB.SaveChanges();
                                }
                                else
                                {
                                    _result.__ErrorMessage = "Geçersiz Email";
                                }
                            }
                            else
                            {
                                _result.__ErrorMessage = "Geçersiz Şifre";
                            }
                        }
                        else
                        {
                            if (kontrolEmail)
                            {
                                _result.__user = UserManager.Users.Where(x => x.IS_DELETED == false && x.Id == Id).FirstOrDefault();
                                _result.__user.Email = Email.Trim() == "" ? null : Email.Trim();
                                _result.__user.UserName = _userName;
                                _result.__user.SHOW_MONEY = Convert.ToInt16(_ShowMoney);
                                _result.__user.REPORT_SEND_MAIL = Convert.ToInt16(_ReportSend);
                                _result.__user.ALARM_SEND_MAIL = Convert.ToInt16(_AlarmSend);
                                UserManager.Update(_result.__user);
                                DB.SaveChanges();
                            }
                            else
                            {
                                _result.__ErrorMessage = "Geçersiz Email";
                            }
                        }
                    }
                    else
                    {

                        _result.__ErrorMessage = "Kullanıcı adı mevcut.";

                    }

                }
            }
            else
            {
                _result.__ErrorMessage = "Boş Alan Var";
            }




            return Json(_result, JsonRequestBehavior.AllowGet);
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }



        public int GunFarkikBul(DateTime dt1, DateTime dt2)

        {

            TimeSpan zaman = new TimeSpan(); // zaman farkını bulmak adına kullanılacak olan nesne

            zaman = dt1 - dt2;// gelen 2 tarih arasındaki fark

            return Math.Abs(zaman.Days); // 2 tarih arasındaki farkın kaç gün olduğu döndürülüyor.

        }


        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {

            //RegisterViewModel rwm = new RegisterViewModel();

            //rwm.Email = "a@a.com";
            //rwm.UserName = "a@a.com";
            //rwm.Password = "111111";
            //rwm.UPDATE_USER = "asdas";
            //await Register(rwm);
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, shouldLockout: false);

            if (!UserManager.Users.Any(x => x.UserName.ToLower() == model.UserName.ToLower()))
            {
                AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                ModelState.AddModelError("", "Kullanıcı adı veya şifre hatalı.");
                return View(model);
            }

            switch (result)
            {
                case SignInStatus.Success:

                    string curUserId = User.Identity.GetUserId();
                    EssoEntities DB = new EssoEntities();

                    if (UserManager.Users.Any(x => x.UserName.ToLower() == model.UserName.ToLower() && x.IS_DEMO == false && x.IS_DELETED == false))

                    {
                        var user = await UserManager.FindAsync(model.UserName, model.Password);
                        var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));

                        List<LayoutViewModel> lvms = new List<LayoutViewModel>();
                        List<TBL_COMPANY> companies;

                        if (user.Roles.Any(x => x.RoleId == roleManager.Roles.FirstOrDefault(v => v.Name == "M_ADMIN").Id))
                        {
                            companies = companyService.GetCompanies(x => x.IS_DELETED == false);


                            foreach (TBL_COMPANY comp in companies)
                            {
                                LayoutViewModel lvm = new LayoutViewModel();
                                lvm.COMPANY_ID = comp.ID;
                                lvm.COMPANY_NAME = comp.NAME;

                                List<TBL_STATION> stations = stationService.GetStations(x => x.COMPANY_ID == comp.ID && x.IS_DELETED == false && x.IS_ACTIVE == true && x.IS_LOCKED == false);

                                if (stations != null && stations.Count > 0)
                                {
                                    foreach (TBL_STATION station in stations)
                                    {
                                        lvm.STATIONS.Add(new StationViewModel { STATION_ID = station.ID, STATION_NAME = station.NAME, PHOTO_PATH = station.PHOTO_PATH });
                                    }
                                }

                                lvms.Add(lvm);
                            }
                        }
                        else
                        {
                            //companies = companyService.GetCompanies(x => x.IS_DELETED == false);
                            List<int> userStationIds = stationUserService.GetStationUsers(x => x.USER_ID == user.Id && x.IS_DELETED == false).Select(x => x.STATION_ID).ToList();

                            if (userStationIds != null && userStationIds.Count > 0)
                            {

                                List<TBL_STATION> userStations = stationService.GetStations(x => userStationIds.Contains(x.ID));

                                List<int> userCompanyIds = userStations.Select(x => x.COMPANY_ID).Distinct().ToList();

                                List<TBL_COMPANY> userCompanies = companyService.GetCompanies(x => userCompanyIds.Contains(x.ID) && x.IS_DELETED == false);

                                foreach (TBL_COMPANY comp in userCompanies)
                                {
                                    LayoutViewModel lvm = new LayoutViewModel();
                                    lvm.COMPANY_ID = comp.ID;
                                    lvm.COMPANY_NAME = comp.NAME;

                                    List<TBL_STATION> stations = userStations.Where(x => userStationIds.Contains(x.ID) && x.COMPANY_ID == comp.ID && x.IS_ACTIVE && !x.IS_LOCKED).ToList();

                                    if (stations != null && stations.Count > 0)
                                    {
                                        foreach (TBL_STATION station in stations)
                                        {
                                            lvm.STATIONS.Add(new StationViewModel { STATION_ID = station.ID, STATION_NAME = station.NAME, PHOTO_PATH = station.PHOTO_PATH });
                                        }
                                    }

                                    lvms.Add(lvm);
                                }
                            }

                            //foreach (Company comp in companies)
                            //{
                            //    LayoutViewModel lvm = new LayoutViewModel();
                            //    lvm.COMPANY_ID = comp.ID;
                            //    lvm.COMPANY_NAME = comp.NAME;

                            //    List<TBL_STATION> stations = stationService.GetStationByCompanyId(comp.ID);

                            //    if (stations != null && stations.Count > 0)
                            //    {
                            //        foreach (TBL_STATION station in stations)
                            //        {
                            //            lvm.STATIONS.Add(new StationViewModel { STATION_ID = station.ID, STATION_NAME = station.NAME });
                            //        }
                            //    }

                            //    lvms.Add(lvm);
                            //}

                            //stationIds = stationUserService.GetStationUsers(x => x.USER_ID == user.Id && x.IS_DELETED == false).Select(x => x.STATION_ID).ToList();
                        }
                        Session["UserCompanies"] = lvms;
                        return RedirectToLocal(returnUrl);

                    }

                    else

                    {
                        var created_date = UserManager.Users.Where(x => x.UserName == model.UserName && x.IS_DELETED == false).Select(x => x.CREATED_DATE).FirstOrDefault();
                        TimeSpan days = Convert.ToDateTime(created_date).Subtract(DateTime.Now);
                        double Day = days.TotalDays;


                        if ((Day * -1) > 7)
                        {
                            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                            ModelState.AddModelError("", "Demo hesabınızın süresi bitmiş olup bizimle iletişime geçebilirsiniz.  ");
                            return View(model);

                        }

                        return RedirectToAction("Index", "Home");

                    }



                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Kullanıcı adı veya şifre hatalı." /*"Invalid login attempt."*/);
                    return View(model);
            }
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult DeleteUser(string id)
        {
            var user = UserManager.FindById(id);

            if (user != null)
            {
                user.IS_DELETED = true;
                user.UPDATE_USER = User.Identity.GetUserId();
                var result = UserManager.UpdateAsync(user);
                if (!result.Result.Succeeded)
                {

                    if (result.Result.Errors != null)
                    {
                        string errTxt = string.Empty;
                        foreach (string errStr in result.Result.Errors)
                        {
                            errTxt += errStr + "\n";
                        }
                        ViewData["EditError"] = errTxt;
                    }
                }
            }
            return PartialView("GridUserPartial", GetUsers());
        }

        [ValidateInput(false)]
        public ActionResult UpdateUserCompanies(MVCxGridViewBatchUpdateValues<CompanyUserViewModel, int> updateValues)
        {
            string userId = Request.Params["userId"];
            try
            {
                if (!string.IsNullOrEmpty(userId) && updateValues.Update.Count > 0)
                {
                    foreach (CompanyUserViewModel uVal in updateValues.Update)
                    {
                        if (uVal.AUTH)
                        {
                            TBL_COMPANY_USER entity = new TBL_COMPANY_USER();
                            entity.COMPANY_ID = uVal.COMPANY_ID;
                            entity.UPDATE_USER = User.Identity.GetUserId();
                            entity.USER_ID = userId;
                            companyUserService.CreateCompanyUser(entity);
                        }
                        else
                        {
                            TBL_COMPANY_USER entity = companyUserService.GetCompanyUsers(x => x.USER_ID == userId && x.COMPANY_ID == uVal.COMPANY_ID && x.IS_DELETED == false).FirstOrDefault();

                            if (entity != null)
                            {
                                entity.IS_DELETED = true;
                                entity.UPDATE_USER = User.Identity.GetUserId();
                                companyUserService.UpdateCompanyUser(entity);
                            }
                        }
                    }
                    companyUserService.SaveCompanyUser();
                }

                return PartialView("GridUserCompanyPartial", GetUserCompInfo(userId));
            }
            catch (Exception ex)
            {
                ViewData["EditableProduct"] = updateValues;
                ViewData["EditError"] = ex.Message;
                return PartialView("GridUserCompanyPartial", userId);
            }
            //catch (Exception ex)
            //{
            //    updateValues.SetErrorText(updateValues.Update[0], ex.Message);
            //    return PartialView("GridUserCompanyPartial", GetUserCompInfo(userId));
            //}
        }


        [ValidateInput(false)]
        public ActionResult UpdateUserRoles(MVCxGridViewBatchUpdateValues<RoleUserViewModel, int> updateValues)
        {
            string userId = Request.Params["userId"];
            try
            {
                if (!string.IsNullOrEmpty(userId) && updateValues.Update.Count > 0)
                {
                    foreach (RoleUserViewModel uVal in updateValues.Update)
                    {
                        if (uVal.AUTH)
                        {
                            IdentityResult res = UserManager.AddToRole(userId, uVal.ROLE_NAME);

                            if (!res.Succeeded)
                            {
                                string errTxt = string.Empty;
                                foreach (string err in res.Errors)
                                {
                                    errTxt += err;
                                }
                                updateValues.SetErrorText(uVal, errTxt);
                            }
                        }
                        else
                        {
                            IdentityResult res = UserManager.RemoveFromRole(userId, uVal.ROLE_NAME);

                            if (!res.Succeeded)
                            {
                                string errTxt = string.Empty;
                                foreach (string err in res.Errors)
                                {
                                    errTxt += err;
                                }
                                updateValues.SetErrorText(uVal, errTxt);
                            }
                        }
                    }
                }

                return PartialView("GridUserRolePartial", GetUserRoleInfo(userId));
            }
            catch (Exception ex)
            {
                updateValues.SetErrorText(updateValues.Update[0], ex.Message);
                return PartialView("GridUserRolePartial", GetUserRoleInfo(userId));
            }
        }

        //
        // GET: /Account/Register

        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GridUserPartial()
        {
            return PartialView(GetUsers());
        }

        [ValidateInput(false)]
        public ActionResult GridUserCompanyPartial(string userId)
        {
            return PartialView(GetUserCompInfo(userId));
        }

        [ValidateInput(false)]
        public ActionResult GridUserRolePartial(string userId)
        {
            return PartialView(GetUserRoleInfo(userId));
        }

        List<RoleUserViewModel> GetUserRoleInfo(string userId)
        {

            List<RoleUserViewModel> vwRolesList = new List<RoleUserViewModel>();

            if (userId == null || userId.Length == 0 || userId == "0")
            {
                return vwRolesList;
            }

            List<IdentityUserRole> userRoles = UserManager.Users.Where(x => x.Id == userId).FirstOrDefault().Roles.ToList();

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));

            List<IdentityRole> allRoles = new List<IdentityRole>();

            if (User.IsInRole("M_ADMIN"))
            {
                allRoles = roleManager.Roles.ToList();
            }
            else
            {
                if (User.IsInRole("RESELLER"))
                {
                    allRoles = roleManager.Roles.Where(x => x.Name != "M_ADMIN" && x.Name != "RESELLER").ToList();
                }
            }

            if (allRoles != null)
            {
                foreach (IdentityRole role in allRoles)
                {
                    RoleUserViewModel rwm = new RoleUserViewModel();
                    rwm.ROLE_ID = role.Id;
                    rwm.ROLE_NAME = role.Name;
                    if (userRoles.Any(x => x.RoleId == role.Id))
                    {
                        rwm.AUTH = true;
                    }
                    vwRolesList.Add(rwm);
                }
            }

            return vwRolesList;

        }

        List<CompanyUserViewModel> GetUserCompInfo(string userId)
        {
            List<CompanyUserViewModel> cuwms = new List<CompanyUserViewModel>();

            if (userId == null || userId.Length == 0 || userId == "0")
            {
                return cuwms;
            }
            string curUserId = User.Identity.GetUserId();
            List<TBL_COMPANY_USER> cusrs = companyUserService.GetCompanyUsers(x => x.USER_ID == userId && x.IS_DELETED == false);

            List<int> usrCompIds = cusrs.Select(z => z.COMPANY_ID).ToList();


            List<TBL_COMPANY> comps = new List<TBL_COMPANY>();

            if (User.IsInRole("M_ADMIN"))
            {
                comps = companyService.GetCompanies(x => x.IS_DELETED == false).ToList();
            }
            else
            {
                if (User.IsInRole("RESELLER"))
                {
                    comps = companyService.GetCompanies(x => x.UPDATE_USER == curUserId && x.IS_DELETED == false).ToList();
                }
            }

            if (comps != null && comps.Count() > 0)
            {
                TBL_COMPANY cmpTemp = new TBL_COMPANY();
                foreach (TBL_COMPANY comp in comps)
                {

                    cmpTemp = comps.FirstOrDefault(x => x.ID == comp.ID);
                    if (cmpTemp != null)
                    {
                        CompanyUserViewModel cuvm = new CompanyUserViewModel();
                        cuvm.COMPANY_ID = cmpTemp.ID;
                        cuvm.COMPANY_NAME = cmpTemp.NAME;

                        cuvm.AUTH = usrCompIds.Contains(comp.ID);

                        cuwms.Add(cuvm);
                    }
                }

                cuwms = cuwms.OrderByDescending(x => x.COMPANY_NAME).ToList();
            }

            return cuwms;
        }

        List<RegisterViewModel> GetUsers()
        {
            List<RegisterViewModel> rwms = new List<RegisterViewModel>();
            List<ApplicationUser> aUsrs = new List<ApplicationUser>();

            string curUserId = User.Identity.GetUserId();

            if (User.IsInRole("M_ADMIN"))
            {
                aUsrs = UserManager.Users.Where(x => x.IS_DELETED == false).ToList();
            }
            else
            {
                if (User.IsInRole("RESELLER"))
                {
                    aUsrs = UserManager.Users.Where(x => x.UPDATE_USER == curUserId && x.IS_DELETED == false && x.Id != curUserId).ToList();
                }

            }

            rwms = Mapper.Map<List<ApplicationUser>, List<RegisterViewModel>>(aUsrs);

            for (int i = 0; i < rwms.Count; i++)
            {
                var ıd = rwms[i].ID;
                TBL_USER_ENTITY _UserEntity = DB.UserEntity.Where(x => x.USER_ID == ıd).FirstOrDefault();

                if (_UserEntity != null)
                {
                    rwms[i].SHOW_MONEY = _UserEntity.SHOW_MONEY;
                    rwms[i].SEND_MAIL = _UserEntity.SEND_MAIL;
                }

            }

            return rwms;
        }


        #region [ Grid Settings ]

        static GridViewSettings gridUsersSettings;

        public static GridViewSettings GridUsersSettings
        {
            get
            {
                if (gridUsersSettings == null)
                    gridUsersSettings = CreateUsersGridSettings();
                return gridUsersSettings;
            }
        }
        static GridViewSettings CreateUsersGridSettings()
        {
            GridViewSettings settings = new GridViewSettings();

            settings.Name = "gvUser";

            settings.KeyFieldName = "ID";
            settings.CallbackRouteValues = new { Controller = "Account", Action = "GridUserPartial" };
            settings.SettingsEditing.AddNewRowRouteValues = new { Controller = "Account", Action = "Register" };
            settings.SettingsEditing.UpdateRowRouteValues = new { Controller = "Account", Action = "Register" };
            settings.SettingsEditing.DeleteRowRouteValues = new { Controller = "Account", Action = "DeleteUser" };
            settings.Settings.ShowFilterRow = true;
            settings.SettingsBehavior.ConfirmDelete = true;

            settings.CommandColumn.Visible = true;
            settings.CommandColumn.ShowDeleteButton = true;
            settings.CommandColumn.ShowEditButton = true;

            //settings.CommandColumn.Visible = true;
            //settings.CommandColumn.ShowNewButtonInHeader = true;
            //settings.CommandColumn.ShowDeleteButton = true;
            //settings.CommandColumn.ShowEditButton = true;
            //settings.CommandColumn.Width = Unit.Point(75);

            //settings.Settings.VerticalScrollBarMode = ScrollBarMode.Visible;
            //settings.Settings.VerticalScrollableHeight = 300;

            settings.Width = Unit.Percentage(100);

            settings.SettingsEditing.Mode = GridViewEditingMode.EditFormAndDisplayRow;

            settings.SettingsBehavior.ProcessColumnMoveOnClient = true;
            settings.SettingsBehavior.ColumnMoveMode = GridColumnMoveMode.AmongSiblings;
            settings.SettingsPager.PageSize = 10;
            settings.SettingsPager.Position = PagerPosition.Bottom;
            settings.SettingsPager.FirstPageButton.Visible = true;
            settings.SettingsPager.LastPageButton.Visible = true;
            settings.SettingsPager.PageSizeItemSettings.Visible = true;
            settings.SettingsPager.PageSizeItemSettings.Items = new string[] { "10", "20", "50" };


            settings.SettingsBehavior.AllowFocusedRow = true;
            settings.ClientSideEvents.FocusedRowChanged = "OnUserelected";
            settings.ClientSideEvents.BeginCallback = "OnUserBeginCallBack";
            settings.ClientSideEvents.EndCallback = "OnUserEndCallBack";

            //Column info
            //settings.Columns.Add(c => { c.FieldName = "UserName";
            //    c.Caption = "Username";

            //});
            //settings.Columns.Add(c => { c.FieldName = "Email";
            //    c.Caption = "Email";
            //});

            //settings.Columns.Add("UserName");
            settings.Columns.Add(column =>
            {
                column.FieldName = "UserName";
                column.Caption = "User Name";
                column.SortOrder = DevExpress.Data.ColumnSortOrder.Ascending;
                column.EditorProperties().TextBox(p =>
                {
                    // p.ValidationSettings.ErrorDisplayMode = ErrorDisplayMode.ImageWithTooltip;
                    p.Width = Unit.Pixel(200);

                });
            });

            //settings.Columns.Add("Email");
            settings.Columns.Add(column =>
            {
                column.FieldName = "Email";
                column.Caption = "Email";

                column.EditorProperties().TextBox(p =>
                {

                    //p.ValidationSettings.ErrorDisplayMode = ErrorDisplayMode.ImageWithTooltip;
                    p.Width = Unit.Pixel(200);

                });
            });

            // settings.Columns.Add("Password");
            settings.Columns.Add(column =>
            {
                column.FieldName = "Password";
                column.Caption = "Password";
                column.Visible = false;
                column.EditFormSettings.Visible = DefaultBoolean.True;

                column.EditorProperties().TextBox(p =>
                {

                    p.ValidationSettings.ErrorDisplayMode = ErrorDisplayMode.ImageWithTooltip;
                    p.Width = Unit.Pixel(200);

                });
            });

            //settings.Columns.Add("PUSH_NOT");
            settings.Columns.Add(column =>
            {
                column.FieldName = "PUSH_NOT";
                column.Caption = "Push Notification";
                column.Visible = false;
                column.EditFormSettings.Visible = DefaultBoolean.True;
                column.EditorProperties().CheckBox(p =>
                {
                    p.ValueType = typeof(bool);
                });
            });

            settings.Columns.Add(column =>
            {
                column.FieldName = "IS_DEMO";
                column.Caption = "Demo User";
                column.Visible = false;
                column.EditFormSettings.Visible = DefaultBoolean.True;
                column.EditorProperties().CheckBox(p =>
                {
                    p.ValueType = typeof(bool);
                });
            });

            settings.Columns.Add(column =>
            {
                column.FieldName = "CREATED_DATE";
                column.Caption = "";
                column.Visible = false;
                column.EditFormSettings.Visible = DefaultBoolean.False;
                column.PropertiesEdit.DisplayFormatString = "d/M/yyyy HH:mm:ss";
                column.ColumnType = MVCxGridViewColumnType.DateEdit;

            });

            //settings.Columns.Add(column =>
            //{
            //	column.FieldName = "SHOW_MONEY";
            //	column.Caption = "Display Money";
            //	column.Visible = false;
            //	column.EditFormSettings.Visible = DefaultBoolean.True;
            //	column.EditorProperties().CheckBox(p =>
            //	{
            //		p.ValueType = typeof(bool);
            //	});
            //});
            //settings.Columns.Add(c =>
            //{
            //	c.FieldName = "SHOW_MONEY_NAME";
            //	c.Caption = "";
            //	c.EditFormSettings.Visible = DefaultBoolean.False;

            //});
            settings.EditFormLayoutProperties.ColCount = 2;
            //settings.EditFormLayoutProperties.Items.Add("User Name");
            settings.EditFormLayoutProperties.Items.Add("UserName");

            settings.EditFormLayoutProperties.Items.Add("Email");

            settings.EditFormLayoutProperties.Items.Add("Password");
            //settings.EditFormLayoutProperties.Items.Add("Push Notification");
            settings.EditFormLayoutProperties.Items.Add("PUSH_NOT");
            settings.EditFormLayoutProperties.Items.Add("IS_DEMO");
            settings.EditFormLayoutProperties.Items.Add("CREATED_DATE");
            //settings.EditFormLayoutProperties.Items.Add("SHOW_MONEY");
            settings.EditFormLayoutProperties.Items.AddCommandItem(itemSettings =>
            {
                itemSettings.ColSpan = 2;
                itemSettings.HorizontalAlign = FormLayoutHorizontalAlign.Right;
            });
            //settings.PreRender = (sender, e) =>
            //{
            //    ((MVCxGridView)sender).StartEdit(1);
            //};


            return settings;
        }

        #endregion

        //
        // POST: /Account/Register
        [HttpPost]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            try
            {
                model.PUSH_NOT = model.PUSH_NOT == null ? false : model.PUSH_NOT;
                model.IS_DEMO = model.IS_DEMO == null ? false : model.IS_DEMO;
                //model.SHOW_MONEY = model.SHOW_MONEY == null ? null : model.SHOW_MONEY;
                //model.SHOW_MONEY_NAME = model.SHOW_MONEY == 1 ? "Not Show" : "Show";

                string curUserId = User.Identity.GetUserId();
                if (ModelState.IsValid)
                {
                    ApplicationUser record = UserManager.Users.FirstOrDefault(x => x.Id == model.ID && x.IS_DELETED == false);
                    TBL_USER_ENTITY record_ = DB.UserEntity.Where(x => x.USER_ID == model.ID).FirstOrDefault();

                    if (record != null)
                    {
                        model.ID = record.Id;
                        if (model.Password != null && model.Password.Trim().Length > 0)
                        {
                            IdentityResult result = new IdentityResult();
                            string resetToken = UserManager.GeneratePasswordResetToken(record.Id);
                            result = UserManager.ResetPassword(record.Id, resetToken, model.Password);

                            if (result != null && !result.Succeeded)
                            {
                                ViewData["EditError"] = "Password could not set. \n";
                            }
                        }

                        if (UserManager.Users.Any(x => x.Email == model.Email && x.Id != model.ID && x.IS_DELETED == false))
                        {
                            ViewData["EditError"] = ViewData["EditError"] + "Email is allready taken.";
                        }
                        else if (UserManager.Users.Any(x => x.Email == model.UserName && x.Id != model.ID && x.IS_DELETED == false))
                        {
                            ViewData["EditError"] = ViewData["EditError"] + "Username is allready taken.";
                        }
                        else
                        {

                            record.UserName = model.UserName; //Mapper.Map<RegisterViewModel, ApplicationUser>(model);
                            record.Email = model.Email;
                            record.Id = model.ID;
                            record.UPDATE_USER = curUserId;
                            record.PUSH_NOT = model.PUSH_NOT;
                            record.IS_DEMO = model.IS_DEMO.Value;
                            record.CREATED_DATE = model.CREATED_DATE;
                            if (record_ != null)
                            {
                                record_.SHOW_MONEY = model.SHOW_MONEY;
                                record_.SEND_MAIL = model.SEND_MAIL;
                                DB.Entry(record_).State = EntityState.Modified;
                                DB.SaveChanges();
                            }
                            else
                            {
                                TBL_USER_ENTITY entity = new TBL_USER_ENTITY();
                                entity.USER_ID = model.ID;
                                entity.SHOW_MONEY = model.SHOW_MONEY;
                                entity.SEND_MAIL = model.SEND_MAIL;
                                DB.UserEntity.Add(entity);
                                DB.SaveChanges();

                            }
                            UserManager.Update(record);
                            DB.SaveChanges();

                        }



                    }
                    else
                    {
                        var user = new ApplicationUser { UserName = model.UserName, Email = model.Email, CREATE_USER = curUserId, UPDATE_USER = curUserId, IS_DELETED = false, CREATED_DATE = DateTime.Now, IS_DEMO = model.IS_DEMO.Value };
                        var result = await UserManager.CreateAsync(user, model.Password);

                        if (result.Succeeded)
                        {
                            var user_money = DB.UserEntity.Where(x => x.USER_ID == user.Id).FirstOrDefault();

                            if (user_money == null)
                            {
                                TBL_USER_ENTITY entity = new TBL_USER_ENTITY();

                                entity.USER_ID = user.Id;
                                entity.SHOW_MONEY = model.SHOW_MONEY;
                                entity.SEND_MAIL = model.SEND_MAIL;
                                DB.UserEntity.Add(entity);
                                DB.SaveChanges();
                            }
                            else
                            {
                                user_money.SHOW_MONEY = model.SHOW_MONEY;

                                DB.UserEntity.Add(user_money);
                                DB.SaveChanges();
                            }

                            string content = "<i style='color:Black'><h3>Merhaba,</h3></i>" +
                                            "<i style='color:Black'><h3>Güneş enerji santraliniz E-SOFT’da izlemeye açılmıştır. www.e-soft.com.tr ‘dan ya da Android-IOS için E-SOFT uygulamasını indirerek giriş yapabilirsiniz.</h3></i><br>" +
                                            "<i style='color:#00629e'><h3><b style='color:Black'>Kullanıcı Adı :</b>" + user.UserName + "</h3></i> " +
                                            "<i style='color:#00629e'><h3><b style='color:Black'> Şifre :</b>" + model.Password + " </h3></i><br>" +
                                            "<i style='color:Black'><h3>Bol Güneşli Günler Dileriz….</h3></i><br><br>" +
                                            "<img src = 'http://www.e-soft.com.tr/images/EsoftLogo/esso.png' height ='80px'><br>" +
                                            "<i style='color:#00629e'><h3>MERKEZ:<br>Uzayçağı Cad. 1269.Sok.No:23 - 25<br>Ostim Yeni Mahalle / ANKARA<br>ŞUBE:<br>Mersinli Mah. 1201 / 1 Sokak No:2 Kat: 1<br>KONAK / İZMİR<br>TEL: +90 312 387 52 80<br>FAKS: +90 312 387 52 81 </h3></i><br><br> ";

                            string content2 = "<i style='color:#00629e'><h3><b style='color:Black'>Kullanıcı Adı :</b>" + user.UserName + "</h3></i> " +
                                            "<i style='color:#00629e'><h3><b style='color:Black'> Şifre :</b>" + model.Password + " </h3></i><br>" +
                                            "<img src = 'http://www.e-soft.com.tr/images/EsoftLogo/esso.png' height ='80px'><br>" +
                                            "<i style='color:#00629e'><h3>MERKEZ:<br>Uzayçağı Cad. 1269.Sok.No:23 - 25<br>Ostim Yeni Mahalle / ANKARA<br>ŞUBE:<br>Mersinli Mah. 1201 / 1 Sokak No:2 Kat: 1<br>KONAK / İZMİR<br>TEL: +90 312 387 52 80<br>FAKS: +90 312 387 52 81 </h3></i><br><br> ";

                            SendMail(user.Email, content);
                            SendMail("gizem.gungordu@esso.com.tr", content2);

                        }
                        else
                        {
                            if (result.Errors != null)
                            {
                                string errTxt = string.Empty;
                                foreach (string errStr in result.Errors)
                                {
                                    errTxt += errStr + "\n";
                                }
                                ViewData["EditError"] = errTxt;
                                ViewData["EditableProduct"] = model;
                            }
                        }
                    }

                }
                else
                {

                    ViewData["EditError"] = "Please, correct all errors.";
                    ViewData["EditableProduct"] = model;
                }
                return PartialView("GridUserPartial", GetUsers());
            }
            catch (Exception ex)
            {
                ViewData["EditError"] = ex.Message;
                ViewData["EditableProduct"] = model;
                return PartialView("GridUserPartial", GetUsers());
            }
            // If we got this far, something failed, redisplay form
        }


        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        //
        public static void SendMail(string mail, string content)
        {
            string htmlBody = string.Empty;
            string htmlBody2 = string.Empty;

            SmtpClient smtp = new SmtpClient();
            smtp.Host = "mail.esso.com.tr";
            smtp.Port = 587;
            smtp.EnableSsl = false;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential("esoft@esso.com.tr", "Esoft1234567?");
            using (var message = new MailMessage("esoft@esso.com.tr", mail))
            {
                message.Subject = "E-Soft Kullanıcı Hesap Bilgileri";
                var body = new StringBuilder();
                body.AppendLine(content);
                body.AppendLine("</table><br/>");

                message.Body = body.ToString();
                message.IsBodyHtml = true;
                smtp.Send(message);
            }


        }

        #region Helpers
        // Used for XSRF protection when adding external logins


        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

        }
        #endregion

        public ActionResult SetCulture(string culture)
        {
            // Validate input
            culture = CultureLanguageHelper.GetImplementedCulture(culture);

            // Save culture in a cookie
            HttpCookie cookie = Request.Cookies["_culture"];
            if (cookie != null)
            {
                cookie = new HttpCookie("_culture");
                cookie.Value = culture;   // update cookie value
                cookie.Expires = DateTime.Now.AddYears(1);
            }
            else
            {

                cookie = new HttpCookie("_culture");
                cookie.Value = culture;
                cookie.Expires = DateTime.Now.AddYears(1);
            }
            Response.Cookies.Add(cookie);

            return RedirectToAction("Login");
        }
    }
}