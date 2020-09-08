using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Esso.Web.ViewModels
{
    public class STATION_GRUP_COMPANY
    {
        public int COMPANY_ID { get; set; }
        public string COMPANY_NAME { get; set; }
        public string DEMO_COMPANY_NAME { get; set; }
        public int GROUP_ID { get; set; }
        public string GROUP_NAME { get; set; }
        public string DEMO_GROUP_NAME { get; set; }
        public int STATION_ID { get; set; }
        public string STATION_NAME { get; set; }
        public string DEMO_STATION_NAME { get; set; }
        public string COMPANY_GROUP_NAME { get; set; }
        public float? IRRADIATION { get; set; }
        public float? ENERGY { get; set; }
        public float? DAILY_PRODUCTION { get; set; }
        public float? PR { get; set; }
        public float? SPECIFIC_YIELD { get; set; }
        public bool? IS_MONEY { get; set; }
        public int? ACTIVE_INV_COUNT { get; set; }
        public int? STATION_TYPE { get; set; }
        public float? DC_INSTALLED_POWER { get; set; }
        public int? INVERTER_COUNT { get; set; }
        public int? INVERTER_ACTIVE_COUNT { get; set; }
        public string INV_SUM_ACTIVE_COUNT { get; set; }
        public bool? CON_STATUS { get; set; }
        public float? FINANCIAL_USD { get; set; }
        public float? FINANCIAL_TL { get; set; }
        public float? EXCHANGE_RATE { get; set; }
        public bool? IS_METEOROLOGY { get; set; }
        public bool? INV_ERROR { get; set; }
        public bool? IS_ALARM { get; set; }
        public int? ALARM_COUNT { get; set; }
        public DateTime DATE { get; set; }
        public long? TARIH_NUMBER { get; set; }
        public string COORDINANT { get; set; }
    }
}