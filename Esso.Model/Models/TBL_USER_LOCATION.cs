using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Esso.Model.Models
{
    class TBL_USER_LOCATION
    {
        [Key]
        public int ID { get; set; }
        public System.DateTime DATE { get; set; }
        [Required]
        [StringLength(128, ErrorMessage = "Maximum length is {1}")]
        public string USER_ID { get; set; }

    }
}
