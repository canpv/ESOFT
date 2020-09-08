using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Esso.Web.Models.HES_MODEL
{
    public class ReportTablePageView
    {
        public string _SqlMaster { get; set; } = "";
        public string _SqlDetail { get; set; } = "";
        public List<GridViewFieldItem> _MasterFieldList { get; set; } = new List<GridViewFieldItem>();
        public List<GridViewFieldItem> _DetailFieldList { get; set; } = new List<GridViewFieldItem>();
        public List<List<string>> _ListTblOzet { get; set; } = new List<List<string>>();
        public List<List<string>> _ListInvOzet { get; set; } = new List<List<string>>();
        public List<List<string>> _ListStrOzet { get; set; } = new List<List<string>>();
        public string ErrorMessage { get; set; } = "";
    }
}