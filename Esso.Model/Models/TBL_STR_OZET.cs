using System;
using System.ComponentModel.DataAnnotations;

namespace Esso.Models
{
    public class TBL_STR_OZET
    {
        [Key]
        public int ID { get; set; }
        
        public int STATION_ID { get; set; }
        public int STRING_ID { get; set; }

        public float VALUE { get; set; }
        public bool IS_DELETED { get; set; }

        [StringLength(128, ErrorMessage = "Maximum length is {1}")]
        public string UPDATE_USER { get; set; }
        public DateTime? INSERT_DATE { get; set; }
        public long? TARIH_NUMBER { get; set; }
        public TBL_STR_OZET()
        {
            INSERT_DATE = DateTime.Now;
            IS_DELETED = false;
        }
    }
}
