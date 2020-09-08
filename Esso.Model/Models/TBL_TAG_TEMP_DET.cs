using System;
using System.ComponentModel.DataAnnotations;

namespace Esso.Models
{
    public class TBL_TAG_TEMP_DET
    {
        [Key]
        public int ID { get; set; }
        

        [Required]
        public int TAG_ID{ get; set; }

        [Required]
        public int TEMPLATE_ID { get; set; }

        [Required]
        public int INV_NO { get; set; }
        
        [StringLength(10, ErrorMessage = "Maximum length is {1}")]
        public string ADDRESS { get; set; }

        public bool IS_DELETED { get; set; }

        [StringLength(128, ErrorMessage = "Maximum length is {1}")]
        public string UPDATE_USER { get; set; }


        
        public DateTime? CREATED_DATE { get; set; }

        public DateTime? UPDATED_DATE { get; set; }

        public TBL_TAG_TEMP_DET()
        {
            CREATED_DATE = DateTime.Now;
            IS_DELETED = false;
        }
    }
}
