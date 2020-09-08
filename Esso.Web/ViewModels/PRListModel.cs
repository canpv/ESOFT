using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Esso.Web.ViewModels
{
    public class PRListModel
    {
        public DateTime Date { get; set; }
        public string StationName { get; set; }
        public float Energy { get; set; }
        public float PR { get; set; }

    }
}