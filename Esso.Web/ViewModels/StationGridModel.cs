using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Esso.ViewModels
{
    public class StationGridModel
    {
        public int ID { get; set; }

        [Required]        
        public int COMPANY_ID { get; set; }

        [Required]
        [StringLength(250, ErrorMessage = "Maximum length is {1}")]
        public string NAME { get; set; }
        [StringLength(250, ErrorMessage = "Maximum length is {1}")]
        public string DEMO_NAME { get; set; }
        public byte[] PHOTO { get; set; }
        public int SIZE { get; set; }
        public int GROUP_ID { get; set; }
        [StringLength(500, ErrorMessage = "Maximum length is {1}")]
		[Required]
		public string ADDRESS { get; set; }
        [StringLength(20, ErrorMessage = "Maximum length is {1}")]
		[Required]
		public string IP_ADDRESS { get; set; }
		[Required]
		public float PORT { get; set; }
        public int? EXE_NUMBER { get; set; }
        public int? STATION_TYPE { get; set; }

        [StringLength(150, ErrorMessage = "Maximum length is {1}")]
		[Required]
		public string PANEL_TYPE { get; set; }


        public bool? IS_CENTRAL_INV { get; set; }

        [StringLength(150, ErrorMessage = "Maximum length is {1}")]
		[Required]
		public string PANEL_BRAND { get; set; }
		[Required]
		public float? AC_INSTALLED_POWER { get; set; }
		[Required]
		public float? DC_INSTALLED_POWER { get; set; }

        public DateTime? START_DATE { get; set; }

        [StringLength(100, ErrorMessage = "Maximum length is {1}")]
		[Required]
		public string INVERTER_MODEL { get; set; }

        public float INSTALLED_POWER { get; set; }
		
		public int? METEROROLOGY_PLANT { get; set; }

        public int? ORIENTATION { get; set; }

        public int? PITCH { get; set; }

        public int? PYRANOMETER_PLANT { get; set; }
        public bool? IS_EKK { get; set; }
        public bool? IS_METEOROLOGY { get; set; }
        public bool? IS_PYRANOMETER { get; set; }
        public bool? IS_STRING { get; set; }
        public bool? IS_STRING_DC { get; set; }
        public bool? OSOS { get; set; }
        public string PITCH_DETAIL { get; set; }

        [StringLength(150, ErrorMessage = "Maximum length is {1}")]
		[Required]
		public string COORDINATE_INFORMATION { get; set; }

        [StringLength(20, ErrorMessage = "Maximum length is {1}")]
		[Required]
		public string WEATHER_LOCATION { get; set; }
        public float EXCHANGE_RATE { get; set; }
        public float? TAX { get; set; }
		[Required]
		public float? INVERTER_TYPE { get; set; }


        [StringLength(500, ErrorMessage = "Maximum length is {1}")]
        public string DESCRIPTION { get; set; }
        public bool IS_ACTIVE { get; set; }
        public bool IS_LOCKED { get; set; }
        public bool IS_DELETED { get; set; }
        public string[] USERS { get; set; }
        [StringLength(128, ErrorMessage = "Maximum length is {1}")]
        public string UPDATE_USER { get; set; }
        public DateTime? INSTALL_DATE { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public DateTime? UPDATED_DATE { get; set; }
		[Required]
		public int? ALARM_TEMP_ID { get; set; }
		[Required]
		public int? TAG_TEMP_ID { get; set; }
        public float? IRRADIATION_SCALE { get; set; }
        [StringLength(500, ErrorMessage = "Maximum length is {1}")]
        public string PLC_INTERFACE { get; set; }
        [StringLength(500, ErrorMessage = "Maximum length is {1}")]
        public string EKK_INTERFACE { get; set; }
        public StationGridModel()
        {
            CREATED_DATE = DateTime.Now;
            IS_DELETED = false;
            IS_ACTIVE = true;
            IS_LOCKED = true;
            SIZE = 0;
        }

		public float? JAN_PRODUCTION { get; set; }
		public float? JAN_IRRADIATION { get; set; }
		public float? FEB_PRODUCTION { get; set; }
		public float? FEB_IRRADIATION { get; set; }
		public float? MARCH_PRODUCTION { get; set; }
		public float? MARCH_IRRADIATION { get; set; }
		public float? APRIL_PRODUCTION { get; set; }
		public float? APRIL_IRRADIATION { get; set; }
		public float? MAY_PRODUCTION { get; set; }
		public float? MAY_IRRADIATION { get; set; }
		public float? JUNE_PRODUCTION { get; set; }
		public float? JUNE_IRRADIATION { get; set; }
		public float? JULY_PRODUCTION { get; set; }
		public float? JULY_IRRADIATION { get; set; }
		public float? AUGUST_PRODUCTION { get; set; }
		public float? AUGUST_IRRADIATION { get; set; }
		public float? SEP_PRODUCTION { get; set; }
		public float? SEP_IRRADIATION { get; set; }
		public float? OKT_PRODUCTION { get; set; }
		public float? OKT_IRRADIATION { get; set; }
		public float? NOV_PRODUCTION { get; set; }
		public float? NOV_IRRADIATION { get; set; }
		public float? DEC_PRODUCTION { get; set; }
		public float? DEC_IRRADIATION { get; set; }
		public float? YEAR_PRODUCTION { get; set; }
		public float? YEAR_IRRADIATION { get; set; }
		


	}
}
