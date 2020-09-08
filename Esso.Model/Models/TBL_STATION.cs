using System;
using System.ComponentModel.DataAnnotations;

namespace Esso.Models
{
    public class TBL_STATION
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public int COMPANY_ID { get; set; }

        [StringLength(250, ErrorMessage = "Maximum length is {1}")]
        public string NAME { get; set; }

        [StringLength(250, ErrorMessage = "Maximum length is {1}")]
        public string DEMO_NAME { get; set; }


        [StringLength(150, ErrorMessage = "Maximum length is {1}")]
        public string PANEL_TYPE { get; set; }



        [StringLength(150, ErrorMessage = "Maximum length is {1}")]
        public string PANEL_BRAND { get; set; }

        public int SIZE { get; set; }
        public float? AC_INSTALLED_POWER { get; set; }
        public float? DC_INSTALLED_POWER { get; set; }

        public float? INSTALLED_POWER { get; set; }

        [StringLength(150, ErrorMessage = "Maximum length is {1}")]
        public string COORDINATE_INFORMATION { get; set; }

        [StringLength(20, ErrorMessage = "Maximum length is {1}")]
        public string WEATHER_LOCATION { get; set; }
        public float EXCHANGE_RATE { get; set; }
        public float? TAX { get; set; }
        public int GROUP_ID { get; set; }

        [StringLength(10, ErrorMessage = "Maxi" +
            "" +
            "" +
            "mum length is {1}")]
        public string PHOTO_PATH { get; set; }

        [StringLength(20, ErrorMessage = "Maximum length is {1}")]
        public string IP_ADDRESS { get; set; }
        public int? PORT { get; set; }
        public int? EXE_NUMBER { get; set; }

        public int? STATION_TYPE { get; set; }

        public bool IS_ACTIVE { get; set; }

        public bool? IS_CENTRAL_INV { get; set; }
        public bool IS_LOCKED { get; set; }
        [StringLength(500, ErrorMessage = "Maximum length is {1}")]
        public string ADDRESS { get; set; }
        [StringLength(500, ErrorMessage = "Maximum length is {1}")]
        public string DESCRIPTION { get; set; }
        public int? ALARM_TEMP_ID { get; set; }
        [StringLength(100, ErrorMessage = "Maximum length is {1}")]
        public string INVERTER_MODEL { get; set; }
        public int? ORIENTATION { get; set; }
        public DateTime? START_DATE { get; set; }        
        public int? METEROROLOGY_PLANT { get; set; }
        public int? PYRANOMETER_PLANT { get; set; }
        public bool? IS_EKK { get; set; }
        public bool? IS_METEOROLOGY { get; set; }
        public bool? IS_PYRANOMETER { get; set; }
        public bool? IS_STRING { get; set; }
        public bool? IS_STRING_DC { get; set; }
        public bool? OSOS { get; set; }
        public string PITCH_DETAIL { get; set; }
        public int? INVERTER_TYPE { get; set; }
        public int? PITCH { get; set; }
        public int? TAG_TEMP_ID { get; set; }
        public bool IS_DELETED { get; set; }
        [StringLength(128, ErrorMessage = "Maximum length is {1}")]
        public string UPDATE_USER { get; set; }
        public DateTime? INSTALL_DATE { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public DateTime? UPDATED_DATE { get; set; }
        public float? IRRADIATION_SCALE { get; set; }
        [StringLength(500, ErrorMessage = "Maximum length is {1}")]
        public string PLC_INTERFACE { get; set; }
        [StringLength(500, ErrorMessage = "Maximum length is {1}")]
        public string EKK_INTERFACE { get; set; }
        //SingleLine tipi için ARD_ID
        public string ARD_ID { get; set; }
        public TBL_STATION()
        {
            CREATED_DATE = DateTime.Now;
            IS_DELETED = false;
            IS_ACTIVE = true;
            //INSTALLED_POWER = 0;
            IS_LOCKED = false;
            SIZE = 0;
        }
    }
}
