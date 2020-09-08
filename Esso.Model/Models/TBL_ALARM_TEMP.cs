using System;
using System.ComponentModel.DataAnnotations;

namespace Esso.Models
{
    public class TBL_ALARM_TEMP
    {
        [Key]
        public int ID { get; set; }

        [StringLength(50, ErrorMessage = "Maximum length is {1}")]
        public string NAME { get; set; }

        [StringLength(250, ErrorMessage = "Maximum length is {1}")]
        public string DESC { get; set; }
        public bool IS_DELETED { get; set; }

        [StringLength(128, ErrorMessage = "Maximum length is {1}")]
        public string UPDATE_USER { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public DateTime? UPDATED_DATE { get; set; }

        public TBL_ALARM_TEMP()
        {
            CREATED_DATE = DateTime.Now;
            IS_DELETED = false;
        }
    }
}
