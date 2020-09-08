using DevExpress.Web;
using DevExpress.Web.Mvc;
using Esso.Web.Models;
using Esso.Web.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Data.Entity.Validation;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace Esso.Web.Controllers
{
    [Authorize]
    public class RoleController : BaseController
    {
        // GET: Role
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GridRolePartial()
        {
            return PartialView("GridRolePartial");
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Create(RoleGridModel role)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));

                    if (!roleManager.RoleExists(role.Name))
                    {
                        IdentityRole newRole = new IdentityRole(role.Name);
                        IdentityResult result = roleManager.Create(newRole);
                        if (result != null && !result.Succeeded)
                        {
                            ViewData["EditError"] = "Password could not set. \n";
                        }
                    }
                    else
                    {
                        ViewData["EditError"] = "Role is already exist.";
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            return PartialView("GridRolePartial");
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(RoleGridModel role)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));
                    IdentityRole idRole = roleManager.FindById(role.Id);
                    idRole.Name = role.Name;
                    IdentityResult result = roleManager.Update(idRole);
                    if (result != null && !result.Succeeded)
                    {
                        ViewData["EditError"] = "Password could not set. \n";
                    }
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
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            return PartialView("GridRolePartial");
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Delete(string Id)
        {
            if (Id.Length > 0)
            {
                try
                {
                    var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));
                    IdentityRole idRole = roleManager.FindById(Id);
                    IdentityResult result = roleManager.Delete(idRole);
                    if (result != null && !result.Succeeded)
                    {
                        ViewData["EditError"] = "Password could not set. \n";
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            return PartialView("GridRolePartial");
        }
        
        #region GridSettings

        static GridViewSettings gridRoleSettings;
        public static GridViewSettings GridRoleSettings
        {
            get
            {
                if (gridRoleSettings == null)
                    gridRoleSettings = CreateRoleGridSettings();
                return gridRoleSettings;
            }
        }
        static GridViewSettings CreateRoleGridSettings()
        {
            GridViewSettings settings = new GridViewSettings();

            settings.Name = "gvRoles";
            settings.KeyFieldName = "Id";
            settings.CallbackRouteValues = new { Controller = "Role", Action = "GridRolePartial" };
            settings.SettingsEditing.AddNewRowRouteValues = new { Controller = "Role", Action = "Create" };
            settings.SettingsEditing.UpdateRowRouteValues = new { Controller = "Role", Action = "Edit" };
            settings.SettingsEditing.DeleteRowRouteValues = new { Controller = "Role", Action = "Delete" };
            settings.SettingsEditing.Mode = GridViewEditingMode.Batch;
            settings.SettingsBehavior.ConfirmDelete = true;
            settings.Width = Unit.Percentage(100);
            
            settings.SettingsBehavior.ProcessColumnMoveOnClient = true;
            settings.SettingsBehavior.ColumnMoveMode = GridColumnMoveMode.AmongSiblings;
            settings.SettingsPager.PageSize = 10;
            settings.SettingsPager.Position = PagerPosition.Bottom;
            settings.SettingsPager.FirstPageButton.Visible = true;
            settings.SettingsPager.LastPageButton.Visible = true;
            settings.SettingsPager.PageSizeItemSettings.Visible = true;
            settings.SettingsPager.PageSizeItemSettings.Items = new string[] { "10", "20", "50" };

            settings.CommandColumn.ShowNewButtonInHeader = true;
            settings.Settings.ShowFilterRow = true;
            settings.Settings.ShowFilterRowMenu = true;

            settings.EnableRowsCache = true;

            settings.CommandColumn.Visible = true;
            settings.CommandColumn.ShowDeleteButton = true;
            //settings.CommandColumn.ShowEditButton = true;
            settings.CommandColumn.ButtonRenderMode = GridCommandButtonRenderMode.Image;
            settings.SettingsCommandButton.NewButton.Image.IconID = DevExpress.Web.ASPxThemes.IconID.ActionsAdd16x16;
            settings.SettingsCommandButton.DeleteButton.Image.IconID = DevExpress.Web.ASPxThemes.IconID.ActionsTrash16x16;
            settings.SettingsCommandButton.EditButton.Image.IconID = DevExpress.Web.ASPxThemes.IconID.ActionsEditname16x16;
            settings.CommandColumn.ShowNewButtonInHeader = true;
            settings.CommandColumn.Width = Unit.Point(75);

            settings.CommandColumn.Visible = true;
            settings.CommandColumn.ShowDeleteButton = true;
            settings.CommandColumn.ShowEditButton = true;
            settings.CommandColumn.Width = Unit.Point(75);

            settings.Settings.VerticalScrollBarMode = ScrollBarMode.Visible;
            settings.Settings.VerticalScrollableHeight = 300;

            //Column info
            settings.Columns.Add(c => { c.FieldName = "Name"; c.Caption = "Role";});

            return settings;
        }

        #endregion
    }
}