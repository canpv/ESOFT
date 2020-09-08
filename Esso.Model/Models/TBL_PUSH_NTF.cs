using System;
using System.ComponentModel.DataAnnotations;

namespace Esso.Models
{
    public class TBL_PUSH_NTF
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [StringLength(128, ErrorMessage = "Maximum length is {1}")]
        public string USER_ID { get; set; }

        [Required]
        public string USER_TOKEN { get; set; }

        [Required]
        public string PUSH_TOKEN { get; set; }

        [Required]
        public bool IS_IOS { get; set; }

        public bool EXPRIRED { get; set; }
        

        [StringLength(128, ErrorMessage = "Maximum length is {1}")]
        public string UPDATE_USER { get; set; }
        public DateTime? INSERT_DATE { get; set; }
        
    }
}
