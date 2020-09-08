using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace Esso.Model
{

    public partial class TBL_UNIT_OZET
    {
        [Key]
        public int ID { get; set; }
        public DateTime INSERT_DATE { get; set; }
        public Nullable<long> DATE_NUMBER { get; set; }
        public int STATION_ID { get; set; }
        public int UNIT_ID { get; set; }
        public Nullable<float> WP_MINUS_DAILY { get; set; }
        public Nullable<float> WP_MINUS_TOTAL { get; set; }
        public Nullable<float> WP_PLUS_DAILY { get; set; }
        public Nullable<float> WP_PLUS_TOTAL { get; set; }
        public Nullable<float> REACTIVE_POWER { get; set; }
        public Nullable<float> VISIBLE_POWER { get; set; }
        public Nullable<float> FREQUENCY { get; set; }
        public Nullable<float> IA { get; set; }
        public Nullable<float> IB { get; set; }
        public Nullable<float> IC { get; set; }
        public Nullable<float> TEMPERATURE1 { get; set; }
        public Nullable<float> TEMPERATURE2 { get; set; }
        public Nullable<float> TEMPERATURE3 { get; set; }
        public Nullable<float> TEMPERATURE4 { get; set; }
        public Nullable<float> TEMPERATURE5 { get; set; }
        public Nullable<float> TEMPERATURE6 { get; set; }
        public Nullable<float> GEN_TEMPERATURE1 { get; set; }
        public Nullable<float> GEN_TEMPERATURE2 { get; set; }
        public Nullable<float> GEN_TEMPERATURE3 { get; set; }
        public Nullable<float> GEN_TEMPERATURE4 { get; set; }
        public Nullable<float> GEN_TEMPERATURE5 { get; set; }
        public Nullable<float> GEN_TEMPERATURE6 { get; set; }
        public Nullable<float> SPEED { get; set; }
        public Nullable<float> VAB { get; set; }
        public Nullable<float> VBC { get; set; }
        public Nullable<float> VCA { get; set; }
        public Nullable<float> ACTIVE_POWER { get; set; }

    }
}
