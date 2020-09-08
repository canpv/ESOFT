using System;
using System.ComponentModel.DataAnnotations;

namespace Esso.Models
{
    public class TBL_STATION_STRING
    {
        [Key]
        public int ID { get; set; }
        
        public int STATION_ID { get; set; }
        public int STRING_ID { get; set; }
        [StringLength(250, ErrorMessage = "Maximum length is {1}")]
        public string DISPLAY_NAME { get; set; }
        public bool IS_DELETED { get; set; }

        [StringLength(128, ErrorMessage = "Maximum length is {1}")]
        public string UPDATE_USER { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public DateTime? UPDATED_DATE { get; set; }

        public TBL_STATION_STRING()
        {
            CREATED_DATE = DateTime.Now;
            IS_DELETED = false;
        }
    }
}
