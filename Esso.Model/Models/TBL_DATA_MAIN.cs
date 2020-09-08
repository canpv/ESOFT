using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Esso.Models
{
    public class TBL_DATA_MAIN
    {
        [Key]
        public int ID { get; set; }
        public System.DateTime date { get; set; }
        [StringLength(15, ErrorMessage = "Maximum length is {1}")]
        public string Address { get; set; }
        public Nullable<float> Value { get; set; }
    }
}
