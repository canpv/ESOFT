using System;
using System.ComponentModel.DataAnnotations;

namespace Esso.Models
{
    public class TBL_ALARM_DEF
    {
        [Key]
        public int ID { get; set; }
        public int TEMPLATE_ID { get; set; }
        public int TAG_ID { get; set; }
        public string OP { get; set; }
        public float VALUE { get; set; }
        public int ALARM_TYPE { get; set; }//0 = basit, 1 dallı

        public int STATUS_NO { get; set; }

        [StringLength(250, ErrorMessage = "Maximum length is {1}")]
        public string DESC { get; set; }
        public bool IS_DELETED { get; set; }

        public bool IS_ALARM { get; set; }

        [StringLength(128, ErrorMessage = "Maximum length is {1}")]
        public string UPDATE_USER { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public DateTime? UPDATED_DATE { get; set; }
        [StringLength(1000, ErrorMessage = "Maximum length is {1}")]
        public string ALARM_MESSAGE { get; set; }

        public TBL_ALARM_DEF()
        {
            CREATED_DATE = DateTime.Now;
            IS_DELETED = false;
            IS_ALARM = false;
        }
    }
}
