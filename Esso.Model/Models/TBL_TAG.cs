using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Esso.Models
{
    public class TBL_TAG
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [StringLength(250, ErrorMessage = "Maximum length is {1}")]
        public string NAME { get; set; }

        public bool IS_DELETED { get; set; }
        
        public bool IS_INV_TAG { get; set; }
        public Nullable<bool> IS_STRING { get; set; }

        [StringLength(128, ErrorMessage = "Maximum length is {1}")]
        public string UPDATE_USER { get; set; }

        public DateTime? CREATED_DATE { get; set; }

        public Nullable<bool> IS_DIGITAL { get; set; }
        

        public DateTime? UPDATED_DATE { get; set; }

        public int? TAG_TYPE { get; set; }
        public TBL_TAG()
        {
            CREATED_DATE = DateTime.Now;
            IS_DELETED = false;
            IS_INV_TAG = true;
        }
    }
}
