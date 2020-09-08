using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Esso.Models
{
    public class TBL_COMPANY_USER
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [StringLength(128, ErrorMessage = "Maximum length is {1}")]
        public string USER_ID { get; set; }

        [Required]
        public int COMPANY_ID { get; set; }

        public bool IS_DELETED { get; set; }

        [StringLength(128, ErrorMessage = "Maximum length is {1}")]
        public string UPDATE_USER { get; set; }

        public DateTime? CREATED_DATE { get; set; }

        public DateTime? UPDATED_DATE { get; set; }

        public TBL_COMPANY_USER()
        {
            CREATED_DATE = DateTime.Now;
            UPDATED_DATE = DateTime.Now;
            IS_DELETED = false;
        }
    }
}
