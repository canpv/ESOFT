using DevExpress.Web;
using DevExpress.Web.Mvc;
using Esso.Models;
using Esso.Service;
using Microsoft.AspNet.Identity;
using System;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace Esso.Web.Controllers
{
    [Authorize]
    public class CompanyController : BaseController
    {
        private readonly ICompanyService companyService;

        public CompanyController(ICompanyService companyService)
        {
            this.companyService = companyService;
        }
        
        public ActionResult Index(string category = null)
        {
            return View();
        }
        
        public ActionResult GridCompanyPartial()
        {
            return PartialView("GridCompanyPartial");
        }

        public ActionResult TagTemplatePartial()
        {
            return PartialView("TagTemplatePartial");
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult SaveCompany(TBL_COMPANY company)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (!companyService.IsCompanyExist(company))
                    {
                        company.CREATED_DATE = DateTime.Now;
                        company.UPDATE_USER = User.Identity.GetUserId();
                        companyService.CreateCompany(company);
                        companyService.SaveCompany();
                    }
                    else
                    {
                        ViewData["EditableProduct"] = company;
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
                ViewData["EditableProduct"] = company;
            }
            return PartialView("GridCompanyPartial");
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult UpdateCompany(TBL_COMPANY company)
        {
            if (ModelState.IsValid)
            {
                if (!companyService.IsCompanyExist(company))
                {
                    companyService.UpdateCompany(company);
                    company.UPDATE_USER = User.Identity.GetUserId();
                    companyService.SaveCompany();
                }
                else
                {
                    ViewData["EditableProduct"] = company;
                    ViewData["EditError"] = "Name is already taken";
                }
            }
            else
            {
                ViewData["EditError"] = "Please, correct all errors.";
                ViewData["EditableProduct"] = company;
            }

            return PartialView("GridCompanyPartial");
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult DeleteCompany(int ID)
        {
            if (ID > 0)
            {
                try
                {
                    companyService.DeleteCompany(ID, User.Identity.GetUserId());
                    companyService.SaveCompany();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            return PartialView("GridCompanyPartial");
        }

        #region GridSettings

        static GridViewSettings gridCompanySettings;
        public static GridViewSettings GridCompanySettings
        {
            get
            {
                if (gridCompanySettings == null)
                    gridCompanySettings = CreateCompanyGridSettings();
                return gridCompanySettings;
            }
        }
        static GridViewSettings CreateCompanyGridSettings()
        {
            GridViewSettings settings = new GridViewSettings();

            settings.Name = "gvCompany";
            settings.KeyFieldName = "ID";
            settings.CallbackRouteValues = new { Controller = "Company", Action = "GridCompanyPartial" };
            settings.SettingsEditing.AddNewRowRouteValues = new { Controller = "Company", Action = "SaveCompany" };
            settings.SettingsEditing.UpdateRowRouteValues = new { Controller = "Company", Action = "UpdateCompany" };
            settings.SettingsEditing.DeleteRowRouteValues = new { Controller = "Company", Action = "DeleteCompany" };



            settings.SettingsEditing.Mode = GridViewEditingMode.EditFormAndDisplayRow;
            settings.SettingsBehavior.ConfirmDelete = true;
            settings.Width = Unit.Percentage(100);

            settings.SettingsBehavior.AllowFocusedRow = true;
            //settings.SettingsBehavior.ProcessColumnMoveOnClient = true;
            //settings.SettingsBehavior.ColumnMoveMode = GridColumnMoveMode.AmongSiblings;
            settings.SettingsPager.PageSize = 10;
            settings.SettingsPager.Position = PagerPosition.Bottom;
            settings.SettingsPager.FirstPageButton.Visible = true;
            settings.SettingsPager.LastPageButton.Visible = true;
            //settings.SettingsPager.PageSizeItemSettings.Visible = true;
            //settings.SettingsPager.PageSizeItemSettings.Items = new string[] { "10", "20", "50" };


            settings.Settings.ShowFilterRow = true;
            settings.Settings.ShowFilterRowMenu = true;

            //settings.Settings.ShowGroupPanel = true;

            settings.ClientSideEvents.FocusedRowChanged = "OnCompanySelected";
            settings.ClientSideEvents.EndCallback = "OnCompanyEndCallBack";
            settings.ClientSideEvents.BeginCallback = "OnCompanyBeginCallBack";

            settings.Settings.VerticalScrollBarMode = ScrollBarMode.Visible;
            settings.Settings.VerticalScrollableHeight = 300;

            //Column info
            settings.Columns.Add(c =>
            {
                c.FieldName = "NAME";
                c.Caption = "Company Name";
                c.EditorProperties().TextBox(t =>
                {
                    t.Width = Unit.Pixel(250);
                });
            });
            settings.Columns.Add(c =>
            {
                c.FieldName = "CREATED_DATE"; c.Caption = "Created Date"; c.Width = Unit.Pixel(200);
                c.EditFormSettings.Visible = DevExpress.Utils.DefaultBoolean.False;
                c.SortOrder = DevExpress.Data.ColumnSortOrder.Ascending;
            });

            settings.EditFormLayoutProperties.ColCount = 3;

            settings.EditFormLayoutProperties.Items.Add("NAME");

         
            return settings;
        }

        #endregion
    }
}