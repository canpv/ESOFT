using System;
using System.ComponentModel.DataAnnotations;

namespace Esso.Models
{
    public class TBL_ALARM_TEMP_DET
    {
        [Key]
        public int ID { get; set; }
        public int TEMPLATE_ID { get; set; }
        public int TAG_ID { get; set; }
        public string OP { get; set; }
        public float VALUE { get; set; }
        public byte ALARM_TYPE { get; set; }//0 = basit, 1 dallı

        public int STATUS_NO { get; set; }

        [StringLength(250, ErrorMessage = "Maximum length is {1}")]
        public string DESC { get; set; }
        public bool IS_DELETED { get; set; }

        [StringLength(128, ErrorMessage = "Maximum length is {1}")]
        public string UPDATE_USER { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public DateTime? UPDATED_DATE { get; set; }

        public TBL_ALARM_TEMP_DET()
        {
            CREATED_DATE = DateTime.Now;
            IS_DELETED = false;
        }
    }
}
