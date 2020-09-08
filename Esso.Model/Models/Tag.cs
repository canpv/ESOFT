using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Esso.Model.Models
{
    public class Tag
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [StringLength(250, ErrorMessage = "Maximum length is {1}")]
        public string NAME { get; set; }
        public bool IS_DELETED { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public DateTime? UPDATED_DATE { get; set; }

        public Tag()
        {
            CREATED_DATE = DateTime.Now;
            IS_DELETED = false;
        }
    }
}
