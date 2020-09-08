using System;
using System.ComponentModel.DataAnnotations;

namespace Esso.Models
{
    public class TBL_COMPANY
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [StringLength(250, ErrorMessage = "Maximum length is {1}")]
        public string NAME { get; set; }
        [StringLength(250, ErrorMessage = "Maximum length is {1}")]
        public string DEMO_NAME { get; set; }
        public bool IS_DELETED { get; set; }

        [StringLength(128, ErrorMessage = "Maximum length is {1}")]
        public string UPDATE_USER { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public DateTime? UPDATED_DATE { get; set; }

        public TBL_COMPANY()
        {
            CREATED_DATE = DateTime.Now;
            IS_DELETED = false;
        }
    }
}
