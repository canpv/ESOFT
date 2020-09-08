using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Esso.Model.Models
{
    public class StringModel
    {
        public StringModel()
        {
            this.series = new List<Values>();
            this.StringList = new List<string>();
            this.Hours = new List<string>();
        }
        public List<Values> series { get; set; }
        public List<string> StringList { get; set; }
        public List<string> Hours { get; set; }
    }
    public class StringPerformanceView
    {
        public StringModel strModel { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class StringValues
    {
        public StringValues()
        {
            this.values = new List<float?>();
        }
        public List<float?> values { get; set; }
    }
}
