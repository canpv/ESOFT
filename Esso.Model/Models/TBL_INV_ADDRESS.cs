using System;
using System.ComponentModel.DataAnnotations;

namespace Esso.Models
{
    public class TBL_INV_ADDRESS
    {
        [Key]
        public int ID { get; set; }
        
        [Required]
        public int TAG_ID{ get; set; }

        public int STATION_ID { get; set; }

        [Required]
        public int INV_ID { get; set; }
        
        [StringLength(10, ErrorMessage = "Maximum length is {1}")]
        public string ADDRESS { get; set; }

        public int IS_DELETED { get; set; }

        [StringLength(128, ErrorMessage = "Maximum length is {1}")]
        public string UPDATE_USER { get; set; }

        public DateTime? CREATED_DATE { get; set; }

        public DateTime? UPDATED_DATE { get; set; }

        public TBL_INV_ADDRESS()
        {
            CREATED_DATE = DateTime.Now;
            IS_DELETED = 0;
        }
    }
}
