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
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Esso.Data;
using Z.EntityFramework.Plus;

namespace Esso.Web.Controllers
{
    [Authorize]
    public class TagTemplateController : BaseController
    {
        EssoEntities DB = new EssoEntities();
        private readonly ITagTemplateService tagTemplateService;
        private readonly ICompanyService  companyService;
        private readonly ITagService tagService;
        private readonly ITagTemplateDetService tagTemplateDetService;
        private readonly IInvAddressService invAddressService;
        private readonly IInverterService inverterService;

        public TagTemplateController(
            ITagTemplateService tagTemplateService, 
            ICompanyService companyService, 
            ITagTemplateDetService tagTemplateDetService,
            ITagService tagService,
            IInvAddressService invAddressService,
            IInverterService inverterService
            )
        {
            this.tagTemplateService = tagTemplateService;
            this.companyService = companyService;
            this.tagTemplateDetService = tagTemplateDetService;
            this.tagService = tagService;
            this.invAddressService = invAddressService;
            this.inverterService = inverterService;
        }

        // GET: TagTemplate
        public ActionResult Index()
        {
            return View(tagService.GetTags(x => x.IS_DELETED == false));
        }

        public ActionResult TagTemplateComboPartial()
        {
            return PartialView("TagTemplateComboPartial", tagTemplateService.GetTagTemplates(x => x.IS_DELETED == false).ToList());
        }

        public ActionResult GridTagTemplatePartial()
        {
            return PartialView("GridTagTemplatePartial");
        }

        public ActionResult GridTagTemplateDetPartial(int templateId = 0, bool newData = false)
        {
            return PartialView("GridTagTemplateDetPartial", GetTemplateDetails( templateId,  newData, false));
        }

        public ActionResult GridTagTemplateDigPartial(int templateId = 0)
        {
            return PartialView("GridTagTemplateDigPartial", GetTemplateDetails(templateId, false, false,true));
        }

        public ActionResult GridTagTemplateGenPartial(int templateId = 0, bool newData = false)
        {
            return PartialView("GridTagTemplateGenPartial", GetTemplateDetails(templateId, newData,true));
        }

        public JsonResult AssignTagTemplateToStation(int stationId, int templateId)
        {
            try
            {
                DateTime curDate = DateTime.Now;
                string curUserId = User.Identity.GetUserId();
                DB.Stations.Where(x => x.ID == stationId)
                .Update(x => new TBL_STATION() { TAG_TEMP_ID = templateId, UPDATED_DATE = curDate, UPDATE_USER = curUserId});
             
                return Json("");
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        List<TagTempDetGridModel> GetTemplateDetails(int templateId , bool newData,bool isGeneral,bool isDigital = false)
        {
            List<TagTempDetGridModel> vModels = new List<TagTempDetGridModel>();

            List<TBL_TAG> tags = new List<TBL_TAG>();
            if (templateId == 0)
            {
                return vModels;
            }

            if (isGeneral)
            {

                tags = tagService.GetTags(x => x.IS_INV_TAG == false && x.IS_DIGITAL == false && x.IS_DELETED == false);
            }
            else
            {
                if (isDigital)
                {
                    tags = tagService.GetTags(x => x.IS_DIGITAL == true && x.IS_DELETED == false);
                }
                else
                {
                    tags = tagService.GetTags(x => x.IS_INV_TAG == true && x.IS_DELETED == false);
                }
            }
            

            List<int> tagIds = tags.Select(x => x.ID).ToList();

            List<TBL_TAG_TEMP_DET> tagDets = tagTemplateDetService.GetTagTemplateDets(x => x.TEMPLATE_ID == templateId && tagIds.Contains(x.TAG_ID) && x.IS_DELETED == false ).OrderBy(x => x.ID).ToList();
            if (!isGeneral && !isDigital)
            {
                if (tags != null && tags.Count > 0)
                {
                    if (tagDets != null && tagDets.Count > 0)
                    {
                        string tagName = string.Empty;
                        foreach (TBL_TAG_TEMP_DET tagDet in tagDets)
                        {
                            TagTempDetGridModel vModel = new TagTempDetGridModel();
                            vModel.ADDRESS = tagDet.ADDRESS;
                            vModel.ID = tagDet.ID;
                            vModel.TAG_ID = tagDet.TAG_ID;
                            vModel.INV_NO = tagDet.INV_NO;
                            tagName = tags.Where(x => x.ID == tagDet.TAG_ID).Select(x => x.NAME).FirstOrDefault();
                            vModel.TAG_NAME = tagName == null || tagName.Length == 0 ? "-- Undefined --" : tagName;
                            vModels.Add(vModel);
                        }
                    }


                    if (newData)
                    {
                        string tagName = string.Empty;
                        int lastId = -1;// tagDets != null && tagDets.Count > 0 ? tagDets.Max(x => x.ID) : 1;  
                                        //lastId++;
                        int invNo = tagDets != null && tagDets.Count > 0 ? tagDets[tagDets.Count - 1].INV_NO + 1 : 1;
                        foreach (TBL_TAG tag in tags)
                        {
                            TagTempDetGridModel vModel = new TagTempDetGridModel();
                            vModel.ADDRESS = "";
                            vModel.ID = lastId;
                            vModel.TAG_ID = tag.ID;
                            vModel.INV_NO = invNo;
                            vModel.TAG_NAME = tag.NAME;

                            vModels.Add(vModel);
                            lastId--;
                        }
                    }
                }
            }
            else if(isGeneral || isDigital )
            {
                TBL_TAG_TEMP_DET tagDet = new TBL_TAG_TEMP_DET();
                int fakeId = 0;
                foreach (TBL_TAG tag in tags)
                {
                    tagDet = tagDets.FirstOrDefault(x => x.TAG_ID == tag.ID);
                    TagTempDetGridModel vModel = new TagTempDetGridModel();
                    if (tagDet != null)
                    {
                        
                        vModel.ADDRESS = tagDet.ADDRESS;
                        vModel.ID = tagDet.ID;
                        vModel.TAG_ID = tagDet.TAG_ID;
                        vModel.INV_NO = tagDet.INV_NO;
                        vModel.TAG_NAME = tag.NAME;
                        vModels.Add(vModel);
                    }
                    else
                    {
                        vModel.ID = fakeId;
                        vModel.TAG_ID = tag.ID;
                        vModel.INV_NO = -1;
                        vModel.TAG_NAME = tag.NAME;
                        vModels.Add(vModel);
                        fakeId--;
                    }
                }


                //string tagName = string.Empty;
                //foreach (TBL_TAG_TEMP_DET tagDet in tagDets)
                //{
                //    TagTempDetGridModel vModel = new TagTempDetGridModel();
                //    vModel.ADDRESS = tagDet.ADDRESS;
                //    vModel.ID = tagDet.ID;
                //    vModel.TAG_ID = tagDet.TAG_ID;
                //    vModel.INV_NO = tagDet.INV_NO;
                //    tagName = tags.Where(x => x.ID == tagDet.TAG_ID).Select(x => x.NAME).FirstOrDefault();
                //    vModel.TAG_NAME = tagName == null || tagName.Length == 0 ? "-- Undefined --" : tagName;
                //    vModels.Add(vModel);
                //}
            }

            return vModels;
        }

        public ActionResult CompanyPartial()
        {
            IEnumerable<TBL_COMPANY> companies = new List<TBL_COMPANY>();
            string curUserId = User.Identity.GetUserId();
            
            if (User.IsInRole("M_ADMIN"))
            {
                companies = companyService.GetCompanies(x => x.IS_DELETED == false);
            }
            else
            {
                companies = companyService.GetCompanies(x => x.UPDATE_USER == curUserId); ;
            }

            return PartialView("CompanyPartial", companies);
        }

        #region Template Det Grid Functions
        [ValidateInput(false)]
        public ActionResult GridUpdateTagTemplateDet(MVCxGridViewBatchUpdateValues<TagTempDetGridModel, int> updateValues, int templateId, bool newData,bool isGeneral,bool isDigital = false)
        {
            try
            { 
                foreach (TagTempDetGridModel tagTemp in updateValues.Update)
                {
                    tagTemp.TEMPLATE_ID = templateId;

                    if (updateValues.IsValid(tagTemp))
                    {
                        if (tagTemp.ID > 0)
                        {
                            
                            UpdateTagTemplateDet(tagTemp, updateValues);
                        }
                        else
                        {
                            InsertTagTemplateDet(tagTemp, updateValues);
                        }
                    }
                       
                }

                if (isDigital)
                {
                    return PartialView("GridTagTemplateDigPartial", GetTemplateDetails(templateId, false, false,true));
                }
                else
                {
                    if (isGeneral)
                    {
                        return PartialView("GridTagTemplateGenPartial", GetTemplateDetails(templateId, newData, true));
                    }
                    else
                    {
                        return PartialView("GridTagTemplateDetPartial", GetTemplateDetails(templateId, newData, false));
                    }
                }
            }
            catch (Exception ex)
            {
                if (isDigital)
                {
                    return PartialView("GridTagTemplateDigPartial", GetTemplateDetails(templateId, false, false, true));
                }
                else
                {
                    if (isGeneral)
                    {
                        return PartialView("GridTagTemplateGenPartial", GetTemplateDetails(templateId, newData, true));
                    }
                    else
                    {
                        return PartialView("GridTagTemplateDetPartial", GetTemplateDetails(templateId, newData, false));
                    }
                }
            }
        }

        protected void InsertTagTemplateDet(TagTempDetGridModel tagTempDet, MVCxGridViewBatchUpdateValues<TagTempDetGridModel, int> updateValues)
        {
            try
            {
                TBL_TAG_TEMP_DET entity = Mapper.Map<TagTempDetGridModel, TBL_TAG_TEMP_DET>(tagTempDet);
                entity.CREATED_DATE = DateTime.Now;
                entity.UPDATE_USER = User.Identity.GetUserId();
                    tagTemplateDetService.CreateTagTemplateDet(entity);
                    tagTemplateDetService.SaveTagTemplateDet();
              
            }
            catch (Exception e)
            {
                updateValues.SetErrorText(tagTempDet, e.Message);
            }
        }
        protected void UpdateTagTemplateDet(TagTempDetGridModel tagTempDet, MVCxGridViewBatchUpdateValues<TagTempDetGridModel, int> updateValues)
        {
            try
            {
                TBL_TAG_TEMP_DET entity = Mapper.Map<TagTempDetGridModel, TBL_TAG_TEMP_DET>(tagTempDet);
                tagTemplateDetService.UpdateTagTemplateDet(entity);
                    tagTemplateDetService.SaveTagTemplateDet();  
            }
            catch (Exception e)
            {
                updateValues.SetErrorText(tagTempDet, e.Message);
            }
        }
        protected void DeleteTagTemplateDet(int templateId, MVCxGridViewBatchUpdateValues<TagTempDetGridModel, int> updateValues)
        {
            try
            {
                tagTemplateDetService.DeleteTagTemplateDet(templateId, User.Identity.GetUserId());
                tagTemplateDetService.SaveTagTemplateDet();
            }
            catch (Exception e)
            {
                updateValues.SetErrorText(templateId, e.Message);
            }
        }
        #endregion

        #region Template Grid Functions
        [ValidateInput(false)]
        public ActionResult UpdateTagTemplateP(MVCxGridViewBatchUpdateValues<TBL_TAG_TEMP, int> updateValues)
        {
            //object curObj;
            try
            {
                foreach (TBL_TAG_TEMP tagTemp in updateValues.Update)
                {
                    if (updateValues.IsValid(tagTemp))
                        UpdateTagTemplate(tagTemp, updateValues);
                }
                foreach (TBL_TAG_TEMP tagTemp in updateValues.Insert)
                {
                    if (updateValues.IsValid(tagTemp))
                        InsertTagTemplate(tagTemp, updateValues);
                }
               
                foreach (var companyID in updateValues.DeleteKeys)
                {
                    DeleteTagTemplate(companyID, updateValues);
                }

                return PartialView("GridTagTemplatePartial");
            }
            catch (Exception ex)
            {
                //updateValues.SetErrorText( curObj, ex.Message);
                return PartialView("GridTagTemplatePartial");
            }
        }

        protected void InsertTagTemplate(TBL_TAG_TEMP tagTemp, MVCxGridViewBatchUpdateValues<TBL_TAG_TEMP, int> updateValues)
        {
            try
            {

                if (!tagTemplateService.IsTagTemplateExist(tagTemp))
                {
                    tagTemp.CREATED_DATE = DateTime.Now;
                    tagTemp.UPDATE_USER = User.Identity.GetUserId();
                    tagTemplateService.CreateTagTemplate(tagTemp);
                    tagTemplateService.SaveTagTemplate();
                }
                else
                {
                    updateValues.SetErrorText(tagTemp, "Bu isimde şirket bulunmaktadır.");
                }

            }
            catch (Exception e)
            {
                updateValues.SetErrorText(tagTemp, e.Message);
            }
        }
        protected void UpdateTagTemplate(TBL_TAG_TEMP tagTemp, MVCxGridViewBatchUpdateValues<TBL_TAG_TEMP, int> updateValues)
        {
            try
            {
                if (!tagTemplateService.IsTagTemplateExist(tagTemp))
                {
                    tagTemplateService.UpdateTagTemplate(tagTemp);
                    tagTemplateService.SaveTagTemplate();
                }
                else
                {
                    updateValues.SetErrorText(tagTemp, "Bu isimde şirket bulunmaktadır.");
                }
            }
            catch (Exception e)
            {
                updateValues.SetErrorText(tagTemp, e.Message);
            }
        }
        protected void DeleteTagTemplate(int templateId, MVCxGridViewBatchUpdateValues<TBL_TAG_TEMP, int> updateValues)
        {
            try
            {
                tagTemplateService.DeleteTagTemplate(templateId, User.Identity.GetUserId());
                tagTemplateService.SaveTagTemplate();
            }
            catch (Exception e)
            {
                updateValues.SetErrorText(templateId, e.Message);
            }
        } 
        #endregion

        #region GridSettings
        public class TemplateGridHelper
        {
            GridViewSettings gridTagTemplateSettings;
          
            
            public GridViewSettings GridTagTemplateSettings
            {
                get
                {
                    if (gridTagTemplateSettings == null)
                        gridTagTemplateSettings = CreateTagTemplateGridSettings();
                    return gridTagTemplateSettings;
                }
            }
            GridViewSettings CreateTagTemplateGridSettings()
            {
                GridViewSettings settings = new GridViewSettings();
                
                settings.Name = "gvTemplate";
                settings.KeyFieldName = "ID";
                settings.CallbackRouteValues = new { Controller = "TagTemplate", Action = "GridTagTemplatePartial" };
                settings.Width = Unit.Percentage(100);
                settings.SettingsBehavior.AllowFocusedRow = true;

                settings.SettingsEditing.BatchUpdateRouteValues = new { Controller = "TagTemplate", Action = "UpdateTagTemplateP" };

                settings.SettingsBehavior.ProcessColumnMoveOnClient = true;
                settings.SettingsBehavior.ColumnMoveMode = GridColumnMoveMode.AmongSiblings;
                
                settings.SettingsEditing.Mode = GridViewEditingMode.Batch;
                settings.SettingsEditing.BatchEditSettings.EditMode = GridViewBatchEditMode.Cell;
                settings.SettingsEditing.BatchEditSettings.StartEditAction = GridViewBatchStartEditAction.FocusedCellClick;

                settings.SettingsEditing.BatchEditSettings.EditMode = GridViewBatchEditMode.Row;

                settings.SettingsPager.PageSize = 10;
                settings.SettingsPager.Position = PagerPosition.Bottom;
                settings.SettingsPager.FirstPageButton.Visible = true;
                settings.SettingsPager.LastPageButton.Visible = true;
                settings.SettingsPager.PageSizeItemSettings.Visible = true;
                settings.SettingsPager.PageSizeItemSettings.Items = new string[] { "10", "20", "50" };
                
                settings.CommandColumn.ButtonRenderMode = GridCommandButtonRenderMode.Image;
                settings.SettingsCommandButton.NewButton.Image.IconID = DevExpress.Web.ASPxThemes.IconID.ActionsAdd16x16;
                settings.SettingsCommandButton.DeleteButton.Image.IconID = DevExpress.Web.ASPxThemes.IconID.ActionsTrash16x16;
                settings.SettingsCommandButton.RecoverButton.Image.IconID = DevExpress.Web.ASPxThemes.IconID.ActionsConvert16x16;

                settings.CommandColumn.Visible = true;
                settings.CommandColumn.ShowDeleteButton = true;
                settings.CommandColumn.ShowNewButtonInHeader = true;
                settings.CommandColumn.Width = Unit.Point(75);

                //settings.Settings.ShowFilterRow = true;
                //settings.Settings.ShowFilterRowMenu = true;

                //settings.Settings.ShowGroupPanel = true;

                settings.Settings.VerticalScrollBarMode = ScrollBarMode.Visible;
                settings.Settings.VerticalScrollableHeight = 200;

                //settings.ClientSideEvents.BeginCallback = "onTemplateGridCallback";
                settings.ClientSideEvents.EndCallback = "onTemplateGridEndCallback";
                settings.ClientSideEvents.FocusedRowChanged = "onTemplateSelected";

                settings.EnableRowsCache = true;
                
                //Column info
                settings.Columns.Add(c => { c.FieldName = "NAME"; c.Caption = "Template Name"; });
                settings.Columns.Add(c => { c.FieldName = "CREATED_DATE"; c.Caption = ""; c.Caption = "Save Date";  c.Width = Unit.Pixel(150); }); 
              
                settings.CellEditorInitialize += (s, e) => {

                    ASPxEdit editor = (ASPxEdit)e.Editor;
                    editor.ValidationSettings.Display = Display.None;
                    if (e.Column.FieldName == "CREATED_DATE")
                    {
                        e.Editor.ReadOnly = true;
                    }
                };

                return settings;
            }
        }


        public class TemplateDetGridHelper
        {

            static GridViewSettings gridTagTemplateGenSettings;

            public static GridViewSettings GridTagTemplateGenSettings
            {
                get
                {
                    if (gridTagTemplateGenSettings == null)
                        gridTagTemplateGenSettings = CreateTagTemplateGenGridSettings();
                    return gridTagTemplateGenSettings;
                }
            }
            static GridViewSettings CreateTagTemplateGenGridSettings()
            {
                GridViewSettings settings = new GridViewSettings();

                settings.Name = "gvTemplateGen";
                settings.KeyFieldName = "ID";
                settings.CallbackRouteValues = new { Controller = "TagTemplate", Action = "GridTagTemplateGenPartial" };
                settings.Width = Unit.Percentage(100);
                settings.Height = Unit.Percentage(100);
                settings.SettingsBehavior.AllowFocusedRow = true;

                settings.SettingsEditing.BatchUpdateRouteValues = new { Controller = "TagTemplate", Action = "GridUpdateTagTemplateDet" , isGeneral = true };


                settings.SettingsEditing.Mode = GridViewEditingMode.Batch;
                settings.SettingsEditing.BatchEditSettings.EditMode = GridViewBatchEditMode.Cell;
                settings.SettingsEditing.BatchEditSettings.StartEditAction = GridViewBatchStartEditAction.Click;

                settings.SettingsEditing.BatchEditSettings.EditMode = GridViewBatchEditMode.Row;
                settings.SettingsPager.Mode = GridViewPagerMode.ShowAllRecords;

                settings.ClientSideEvents.BeginCallback = "onTemplateGenGridCallback";
                settings.ClientSideEvents.EndCallback = "onTemplateGenGridEndCallback";

                settings.Settings.ShowFilterRow = true;
                settings.Settings.ShowFilterRowMenu = true;

                settings.Settings.ShowGroupPanel = true;
                //settings.SettingsBehavior.AllowSort = false;
                //settings.ClientSideEvents.BeginCallback = "onTemplateGridCallback";

                settings.EnableRowsCache = true;

                //Column info
                settings.Columns.Add(c => { c.FieldName = "TAG_NAME"; c.Caption = "Tag"; });
                settings.Columns.Add(c => { c.FieldName = "ADDRESS"; c.Caption = "Address"; c.Width = Unit.Pixel(150); });
                settings.Columns.Add(c => { c.FieldName = "TAG_ID"; c.Caption = "TID"; c.Width = Unit.Pixel(50); });

                settings.CellEditorInitialize += (s, e) =>
                {

                    ASPxEdit editor = (ASPxEdit)e.Editor;
                    editor.ValidationSettings.Display = Display.None;
                    if (e.Column.FieldName == "TAG_NAME" || e.Column.FieldName == "TAG_ID")
                    {
                        e.Editor.ReadOnly = true;
                    }
                };

                return settings;
            }

            static GridViewSettings gridTagTemplateDetSettings;

            public static GridViewSettings GridTagTemplateDetSettings
            {
                get
                {
                    if (gridTagTemplateDetSettings == null)
                        gridTagTemplateDetSettings = CreateTagTemplateDetGridSettings();
                    return gridTagTemplateDetSettings;
                }
            }
            static GridViewSettings CreateTagTemplateDetGridSettings()
            {
                GridViewSettings settings = new GridViewSettings();

                settings.Name = "gvTemplateDet";
                settings.KeyFieldName = "ID";
                settings.CallbackRouteValues = new { Controller = "TagTemplate", Action = "GridTagTemplateDetPartial" };
                settings.Width = Unit.Percentage(100);
                settings.Height = Unit.Percentage(100);
                settings.SettingsBehavior.AllowFocusedRow = true;

                settings.SettingsEditing.BatchUpdateRouteValues = new { Controller = "TagTemplate", Action = "GridUpdateTagTemplateDet", isGeneral = false };

              
                settings.SettingsEditing.Mode = GridViewEditingMode.Batch;
                settings.SettingsEditing.BatchEditSettings.EditMode = GridViewBatchEditMode.Cell;
                settings.SettingsEditing.BatchEditSettings.StartEditAction = GridViewBatchStartEditAction.Click;

                settings.SettingsEditing.BatchEditSettings.EditMode = GridViewBatchEditMode.Row;
                settings.SettingsPager.Mode = GridViewPagerMode.ShowAllRecords;                

                settings.ClientSideEvents.BeginCallback = "onTemplateDetGridCallback";
                settings.ClientSideEvents.EndCallback = "onTemplateDetGridEndCallback";

                settings.Settings.ShowFilterRow = true;
                settings.Settings.ShowFilterRowMenu = true;

                settings.Settings.ShowGroupPanel = true;
                //settings.SettingsBehavior.AllowSort = false;
                //settings.ClientSideEvents.BeginCallback = "onTemplateGridCallback";

                settings.EnableRowsCache = true;

                //Column info
                settings.Columns.Add(c => { c.FieldName = "TAG_NAME"; c.Caption = "Tag"; });
                settings.Columns.Add(c => { c.FieldName = "INV_NO"; c.Caption = "Inverter Number"; c.Width = Unit.Pixel(50); });
                settings.Columns.Add(c => { c.FieldName = "ADDRESS"; c.Caption = "Address"; c.Width = Unit.Pixel(150); });
                settings.Columns.Add(c => { c.FieldName = "TAG_ID"; c.Caption = "TID"; c.Width = Unit.Pixel(50); });

                settings.CellEditorInitialize += (s, e) =>
                {

                    ASPxEdit editor = (ASPxEdit)e.Editor;
                    editor.ValidationSettings.Display = Display.None;
                    if (e.Column.FieldName == "TAG_NAME" || e.Column.FieldName == "INV_NO" || e.Column.FieldName == "TAG_ID")
                    {
                        e.Editor.ReadOnly = true;
                    }
                };

                return settings;
            }
        }


        #endregion
    }
}