using DevExpress.Web.Mvc;
using Esso.Data;
using Esso.Models;
using Esso.Service;
using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Web.Mvc;
using Z.EntityFramework.Plus;

namespace Esso.Web.Controllers
{
    [Authorize]
    public class AlarmDefinitionController : BaseController
    {
        EssoEntities DB = new EssoEntities();
        private readonly IAlarmDefinitionService alarmDefinitionService;

        public AlarmDefinitionController(IAlarmDefinitionService alarmDefinitionService)
        {
            this.alarmDefinitionService = alarmDefinitionService;
        }
        
        public ActionResult Index()
        {
            return View();
        }
        
        public ActionResult AlarmTempGridPartial()
        {
            return PartialView();
        }

        public ActionResult AlarmDefinitionGridPartial(int templateId)
        {           
                return PartialView(templateId);                     
        }

        #region AlarmDef Grid Functions

        [ValidateInput(false)]
        public ActionResult GridUpdateTemp(MVCxGridViewBatchUpdateValues<TBL_ALARM_TEMP, int> updateValues)
        {
            //object curObj;
            try
            {
                string curUserId = User.Identity.GetUserId();
                foreach (TBL_ALARM_TEMP tag in updateValues.Insert)
                {
                    if (updateValues.IsValid(tag))
                    {
                        DB.AlarmTemplate.Add(tag);
                    }
                }
                foreach (TBL_ALARM_TEMP tag in updateValues.Update)
                {
                    if (updateValues.IsValid(tag))
                    {
                        TBL_ALARM_TEMP ent = DB.AlarmTemplate.FirstOrDefault(x => x.ID == tag.ID);
                        ent.UPDATED_DATE = DateTime.Now;
                        ent.UPDATE_USER = curUserId;
                        ent.NAME = tag.NAME;
                    }
                }
                DB.SaveChanges();
                foreach (var tagIg in updateValues.DeleteKeys)
                {
                    DB.AlarmTemplate.Where(x => x.ID == tagIg)
                        .Update(x => new TBL_ALARM_TEMP() { IS_DELETED = true });
                }

                return PartialView("AlarmTempGridPartial");
            }
            catch (Exception ex)
            {
                //updateValues.SetErrorText( curObj, ex.Message);
                return PartialView("AlarmTempGridPartial");
            }
        }


        #endregion

        #region AlarmDef Grid Functions
        [ValidateInput(false)]
        public ActionResult GridUpdateTemDet(MVCxGridViewBatchUpdateValues<TBL_ALARM_DEF, int> updateValues,int templateId)
        {
            //object curObj;
            try
            {
                string curUserId = User.Identity.GetUserId();
                foreach (TBL_ALARM_DEF tag in updateValues.Insert)
                {
                    if (updateValues.IsValid(tag))
                    {
                        tag.TEMPLATE_ID = templateId;
                        DB.AlarmDefinitions.Add(tag);
                    }
                }
                foreach (TBL_ALARM_DEF tag in updateValues.Update)
                {
                    if (updateValues.IsValid(tag))
                    {
                        TBL_ALARM_DEF ent = DB.AlarmDefinitions.FirstOrDefault(x => x.ID == tag.ID);
                        ent.UPDATED_DATE = DateTime.Now;
                        ent.UPDATE_USER = curUserId;
                        ent.ALARM_TYPE = tag.ALARM_TYPE;
                        ent.IS_ALARM = tag.IS_ALARM;
                        ent.DESC = tag.DESC;
                        ent.OP = tag.OP;
                        ent.STATUS_NO = tag.STATUS_NO;
                        ent.TAG_ID = tag.TAG_ID;
                        ent.VALUE = tag.VALUE;
                    }
                }
              
                foreach (var tagIg in updateValues.DeleteKeys)
                {
                    TBL_ALARM_DEF ent = DB.AlarmDefinitions.FirstOrDefault(x => x.ID == tagIg);
                    ent.IS_DELETED = true;
                    ent.UPDATED_DATE = DateTime.Now;
                    ent.UPDATE_USER = curUserId;

                }
                DB.SaveChanges();
                return PartialView("AlarmDefinitionGridPartial", templateId);
            }
            catch (Exception ex)
            {
                //updateValues.SetErrorText( curObj, ex.Message);
                return PartialView("AlarmDefinitionGridPartial", templateId);
            }
        }

        protected void InsertTag(TBL_ALARM_TEMP tag, MVCxGridViewBatchUpdateValues<TBL_ALARM_TEMP, int> updateValues)
        {
            try
            {
                //tag.CREATED_DATE = DateTime.Now;
                //tag.UPDATE_USER = User.Identity.GetUserId();
                //alarmDefinitionService.CreateAlarmDefinition(tag);
                //alarmDefinitionService.SaveAlarmDefinition();
            }
            catch (Exception e)
            {
                updateValues.SetErrorText(tag, e.Message);
            }
        }
        protected void UpdateTag(TBL_ALARM_TEMP def, MVCxGridViewBatchUpdateValues<TBL_ALARM_TEMP, int> updateValues)
        {
            //try
            //{
            //    alarmDefinitionService.UpdateAlarmDefinition(def);
            //    alarmDefinitionService.SaveAlarmDefinition();
            //}
            //catch (Exception e)
            //{
            //    updateValues.SetErrorText(def, e.Message);
            //}
        }
        protected void DeleteTag(int defId, MVCxGridViewBatchUpdateValues<TBL_ALARM_TEMP, int> updateValues)
        {
            try
            {
                alarmDefinitionService.DeleteAlarmDefinition(defId, User.Identity.GetUserId());
                alarmDefinitionService.SaveAlarmDefinition();
            }
            catch (Exception e)
            {
                updateValues.SetErrorText(defId, e.Message);
            }
        }
        #endregion

        //#region GridSettings

        //public class TagGridHelper
        //{
        //    static GridViewSettings gridTagSettings;

        //    public static GridViewSettings GridTagSettings
        //    {
        //        get
        //        {
        //            if (gridTagSettings == null)
        //                gridTagSettings = CreateTagGridSettings();
        //            return gridTagSettings;
        //        }
        //    }

        //    static GridViewSettings CreateTagGridSettings()
        //    {
        //        GridViewSettings settings = new GridViewSettings();

        //        settings.Name = "gvTag";
        //        settings.KeyFieldName = "ID";
        //        settings.CallbackRouteValues = new { Controller = "Tag", Action = "GridTagPartial" };
        //        settings.Width = Unit.Percentage(100);
        //        settings.SettingsBehavior.AllowFocusedRow = true;

        //        settings.SettingsEditing.BatchUpdateRouteValues = new { Controller = "Tag", Action = "GridUpdateTag" };

        //        settings.SettingsEditing.Mode = GridViewEditingMode.Batch;
        //        settings.SettingsEditing.BatchEditSettings.EditMode = GridViewBatchEditMode.Cell;
        //        settings.SettingsEditing.BatchEditSettings.StartEditAction = GridViewBatchStartEditAction.Click;

        //        settings.SettingsEditing.BatchEditSettings.EditMode = GridViewBatchEditMode.Row;
        //        settings.SettingsPager.Mode = GridViewPagerMode.ShowAllRecords;

        //        settings.SettingsPager.PageSize = 50;
        //        settings.SettingsPager.Position = PagerPosition.Bottom;
        //        settings.SettingsPager.FirstPageButton.Visible = true;
        //        settings.SettingsPager.LastPageButton.Visible = true;
        //        settings.SettingsPager.PageSizeItemSettings.Visible = true;
        //        settings.SettingsPager.PageSizeItemSettings.Items = new string[] { "10", "20", "50" };

        //        settings.CommandColumn.ButtonRenderMode = GridCommandButtonRenderMode.Image;
        //        settings.SettingsCommandButton.NewButton.Image.IconID = DevExpress.Web.ASPxThemes.IconID.ActionsAdd16x16;
        //        settings.SettingsCommandButton.DeleteButton.Image.IconID = DevExpress.Web.ASPxThemes.IconID.ActionsTrash16x16;
        //        settings.SettingsCommandButton.RecoverButton.Image.IconID = DevExpress.Web.ASPxThemes.IconID.ActionsConvert16x16;

        //        settings.CommandColumn.Visible = true;
        //        settings.CommandColumn.ShowDeleteButton = true;
        //        settings.CommandColumn.ShowNewButtonInHeader = true;
        //        settings.CommandColumn.Width = Unit.Point(75);

        //        settings.Settings.ShowFilterRow = true;
        //        settings.Settings.ShowFilterRowMenu = true;

        //        settings.Settings.ShowGroupPanel = true;

        //        settings.EnableRowsCache = true;

        //        //Column info
        //        settings.Columns.Add(c => { c.FieldName = "NAME"; c.Caption = "Tag"; });
        //        settings.Columns.Add("IS_INV_TAG", "Inverter based", MVCxGridViewColumnType.CheckBox).Width = Unit.Pixel(100);
        //        settings.Columns.Add(c => { c.FieldName = "CREATED_DATE"; c.Caption = "Insert Date"; c.Width = Unit.Pixel(200); });
        //        settings.Columns.Add(c => { c.FieldName = "UPDATED_DATE"; c.Caption = "Update Date"; c.Width = Unit.Pixel(200); });

        //        settings.CellEditorInitialize += (s, e) =>
        //        {
        //            ASPxEdit editor = (ASPxEdit)e.Editor;
        //            editor.ValidationSettings.Display = Display.None;
        //            if (e.Column.FieldName == "CREATED_DATE" || e.Column.FieldName == "UPDATED_DATE")
        //            {
        //                e.Editor.ReadOnly = true;
        //            }
        //        };

        //        return settings;
        //    }
        //}


        //#endregion
    }
}