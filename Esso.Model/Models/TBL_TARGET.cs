using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Esso.Models
{
    public class TBL_TARGET
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public int STATION_ID { get; set; }
        public float? JAN_PRODUCTION { get; set; }
        public float? FEB_PRODUCTION { get; set; }
        public float? MARCH_PRODUCTION { get; set; }
        public float? APRIL_PRODUCTION { get; set; }
        public float? MAY_PRODUCTION { get; set; }
        public float? JUNE_PRODUCTION { get; set; }
        public float? JULY_PRODUCTION { get; set; }
        public float? AUGUST_PRODUCTION { get; set; }
        public float? SEP_PRODUCTION { get; set; }
        public float? OKT_PRODUCTION { get; set; }
        public float? NOV_PRODUCTION { get; set; }
        public float? DEC_PRODUCTION { get; set; }
        public float? YEAR_PRODUCTION { get; set; }
        public float? JAN_IRRADIATION { get; set; }
        public float? FEB_IRRADIATION { get; set; }
        public float? MARCH_IRRADIATION { get; set; }
        public float? APRIL_IRRADIATION { get; set; }
        public float? MAY_IRRADIATION { get; set; }
        public float? JUNE_IRRADIATION { get; set; }
        public float? JULY_IRRADIATION { get; set; }
        public float? AUGUST_IRRADIATION { get; set; }
        public float? SEP_IRRADIATION { get; set; }
        public float? OKT_IRRADIATION { get; set; }
        public float? NOV_IRRADIATION { get; set; }
        public float? DEC_IRRADIATION { get; set; }
        public float? YEAR_IRRADIATION { get; set; }
        public bool IS_DELETED { get; set; }
        [StringLength(128, ErrorMessage = "Maximum length is {1}")]
        public string UPDATE_USER { get; set; }
        public DateTime? INSTALL_DATE { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public DateTime? UPDATED_DATE { get; set; }

        public TBL_TARGET()
        {
            CREATED_DATE = DateTime.Now;
            IS_DELETED = false;
        }
    }
}
