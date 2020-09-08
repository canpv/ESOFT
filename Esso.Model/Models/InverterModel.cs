using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Esso.Model.Models
{
    public class InverterModel
    {
        public InverterModel()
        {
            this.series = new List<Values>();
            this.InverterList = new List<string>();
            this.Hours = new List<string>();
        }
        public List<Values> series { get; set; }
        public List<string> InverterList { get; set; }
        public List<string> Hours { get; set; }
    }
    public class InverterPerformanceView
    {
        public  InverterModel  invModel{ get;set;}
        public string ErrorMessage { get; set; }
    }
        public class Values
    {
        public Values()
        {
            this.values = new List<float?>();
        }
        public List<float?> values { get; set; }
    }

    public class InverterProductionView
    {
        public MontlyInverterProduction invModel { get; set; }
        public string ErrorMessage { get; set; }
    }
    public class MontlyInverterProduction
    {
        public MontlyInverterProduction()
        {
            listDay = new List<int>();
            listInvName = new List<string>();
            listInvValue = new List<InvMonthlyValue>();
        }
        public List<int> listDay { get; set; }
        public List<string> listInvName { get; set; }
        public List<InvMonthlyValue> listInvValue { get; set; }
    }

    public class InvMonthlyValue
    {
        public int day { get; set; }
        public int inv { get; set; }
        public float? energy { get; set; }
        public string name { get; set; }
    }

}
