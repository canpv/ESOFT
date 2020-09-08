using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Esso.Models
{
    public class TBL_PROVIENCE
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [StringLength(250, ErrorMessage = "Maximum length is {1}")]
        public string NAME { get; set; }

    }
}
