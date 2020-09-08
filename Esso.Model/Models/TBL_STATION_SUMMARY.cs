using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Esso.Model.Models
{
    public class TBL_STATION_SUMMARY
    {
        [Key]
        public int ID { get; set; }
        public int COMPANY_ID { get; set; }
        public int GROUP_ID { get; set; }
        public int STATION_ID { get; set; }
        public DateTime? DATE { get; set; }
        public Nullable<long> TARIH_NUMBER { get; set; }
        public float? DC_INSTALLED_POWER { get; set; }
        public Nullable<float> DAILY_PRODUCTION { get; set; }
        public Nullable<float> ENERGY { get; set; }
        public Nullable<float> IRRADIATION { get; set; }
        public Nullable<float> SPESIFIC_YIELD { get; set; }
        public int? INV_COUNT { get; set; }
        public int? PASIVE_INV_COUNT { get; set; }
        public Nullable<float> FINANCIAL_INCOME { get; set; }
        public Nullable<float> PR { get; set; }
        public bool? COMMINCATION { get; set; }
        public int? ALARM_COUNT { get; set; }

    }
}
