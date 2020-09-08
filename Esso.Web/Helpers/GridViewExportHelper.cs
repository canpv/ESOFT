using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Data;
using DevExpress.Web.Mvc;
using DevExpress.XtraPrinting;
using Esso.Data;

namespace DevExpress.Web.Demos
{
    public enum GridViewExportFormat { None, Pdf, Xls, Xlsx, Rtf, Csv }
    public delegate ActionResult GridViewExportMethod(GridViewSettings settings, object dataObject);

    public class GridViewExportHelper
    {
        static string ExcelDataAwareGridViewSettingsKey = "51172A18-2073-426B-A5CB-136347B3A79F";
        static string FormatConditionsExportGridViewSettingsKey = "14634B6F-E1DC-484A-9728-F9608615B628";

        static Dictionary<GridViewExportFormat, GridViewExportMethod> exportFormatsInfo;
        public static Dictionary<GridViewExportFormat, GridViewExportMethod> ExportFormatsInfo
        {
            get
            {
                if (exportFormatsInfo == null)
                    exportFormatsInfo = CreateExportFormatsInfo();
                return exportFormatsInfo;
            }
        }

        static IDictionary Context { get { return System.Web.HttpContext.Current.Items; } }

        static Dictionary<GridViewExportFormat, GridViewExportMethod> CreateExportFormatsInfo()
        {
            return new Dictionary<GridViewExportFormat, GridViewExportMethod> {
                { GridViewExportFormat.Pdf, GridViewExtension.ExportToPdf },
                {
                    GridViewExportFormat.Xls,
                    (settings, data) => GridViewExtension.ExportToXls(settings, data, new XlsExportOptionsEx { ExportType = DevExpress.Export.ExportType.DataAware })
                },
                {
                    GridViewExportFormat.Xlsx,
                    (settings, data) => GridViewExtension.ExportToXlsx(settings, data, new XlsxExportOptionsEx { ExportType = DevExpress.Export.ExportType.WYSIWYG })
                },
                { GridViewExportFormat.Rtf, GridViewExtension.ExportToRtf },
                {
                    GridViewExportFormat.Csv,
                    (settings, data) => GridViewExtension.ExportToCsv(settings, data, new CsvExportOptionsEx { ExportType = DevExpress.Export.ExportType.WYSIWYG })
                }
            };
        }

        static Dictionary<GridViewExportFormat, GridViewExportMethod> dataAwareExportFormatsInfo;
        public static Dictionary<GridViewExportFormat, GridViewExportMethod> DataAwareExportFormatsInfo
        {
            get
            {
                if (dataAwareExportFormatsInfo == null)
                    dataAwareExportFormatsInfo = CreateDataAwareExportFormatsInfo();
                return dataAwareExportFormatsInfo;
            }
        }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> CreateDataAwareExportFormatsInfo()
        {
            return new Dictionary<GridViewExportFormat, GridViewExportMethod> {
                { GridViewExportFormat.Xls, GridViewExtension.ExportToXls },
                { GridViewExportFormat.Xlsx, GridViewExtension.ExportToXlsx },
                { GridViewExportFormat.Csv, GridViewExtension.ExportToCsv }
            };
        }

        static Dictionary<GridViewExportFormat, GridViewExportMethod> formatConditionsExportFormatsInfo;
        public static Dictionary<GridViewExportFormat, GridViewExportMethod> FormatConditionsExportFormatsInfo
        {
            get
            {
                if (formatConditionsExportFormatsInfo == null)
                    formatConditionsExportFormatsInfo = CreateFormatConditionsExportFormatsInfo();
                return formatConditionsExportFormatsInfo;
            }
        }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> CreateFormatConditionsExportFormatsInfo()
        {
            return new Dictionary<GridViewExportFormat, GridViewExportMethod> {
                { GridViewExportFormat.Pdf, GridViewExtension.ExportToPdf},
                { GridViewExportFormat.Xls, (settings, data) => GridViewExtension.ExportToXls(settings, data) },
                { GridViewExportFormat.Xlsx,
                    (settings, data) => GridViewExtension.ExportToXlsx(settings, data, new XlsxExportOptionsEx {ExportType = DevExpress.Export.ExportType.WYSIWYG})
                },
                { GridViewExportFormat.Rtf, GridViewExtension.ExportToRtf }
            };
        }

        static Dictionary<GridViewExportFormat, GridViewExportMethod> advancedBandsExportFormatsInfo;
        public static Dictionary<GridViewExportFormat, GridViewExportMethod> AdvancedBandsExportFormatsInfo
        {
            get
            {
                if (advancedBandsExportFormatsInfo == null)
                    advancedBandsExportFormatsInfo = CreateAdvancedBandsExportFormatsInfo();
                return advancedBandsExportFormatsInfo;
            }
        }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> CreateAdvancedBandsExportFormatsInfo()
        {
            return new Dictionary<GridViewExportFormat, GridViewExportMethod> {
                { GridViewExportFormat.Pdf, GridViewExtension.ExportToPdf },
                { GridViewExportFormat.Xls, (settings, data) => GridViewExtension.ExportToXls(settings, data, new XlsExportOptionsEx {ExportType = DevExpress.Export.ExportType.WYSIWYG}) },
                { GridViewExportFormat.Xlsx, (settings, data) => GridViewExtension.ExportToXlsx(settings, data, new XlsxExportOptionsEx {ExportType = DevExpress.Export.ExportType.WYSIWYG}) },
                { GridViewExportFormat.Rtf, GridViewExtension.ExportToRtf }
            };
        }

        public static string GetExportButtonTitle(GridViewExportFormat format)
        {
            if (format == GridViewExportFormat.None)
                return string.Empty;
            return string.Format("Export to {0}", format.ToString().ToUpper());
        }
        
     

    }
}
