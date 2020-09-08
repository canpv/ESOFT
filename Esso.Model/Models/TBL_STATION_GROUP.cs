using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Esso.Models
{
    public class TBL_STATION_GROUP
    {
        [Key]
        public int ID { get; set; }

        [Required]      
        public int COMPANY_ID { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Maximum length is {1}")]
        [DisplayName("Group Name")]
        public string NAME { get; set; }
        [StringLength(100, ErrorMessage = "Maximum length is {1}")]
        public string DEMO_NAME { get; set; }

        public bool IS_DELETED { get; set; }

        [Required]
        public string UPDATE_USER { get; set; }

        public DateTime? CREATED_DATE { get; set; }

        public DateTime? UPDATED_DATE { get; set; }

        public TBL_STATION_GROUP()
        {
            CREATED_DATE = DateTime.Now;
            IS_DELETED = false;
        }
    }
}
