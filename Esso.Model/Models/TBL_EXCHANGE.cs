using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Esso.Models
{
    public class TBL_EXCHANGE
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public int EXCHANGE_ID { get; set; }
        public DateTime EXCHANGE_DATE { get; set; }
        public Nullable<float> BUYING_VALUE { get; set; }

    }
}
