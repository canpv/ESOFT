using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Esso.Models
{
    public class VW_STR_OZET_DAILY
    {
        [Key]
        public int STATION_ID { get; set; }
        public int STRING_ID { get; set; }

        [StringLength(250, ErrorMessage = "Maximum length is {1}")]
        public string DISPLAY_NAME { get; set; }
        public float VALUE { get; set; }
        [StringLength(250, ErrorMessage = "Maximum length is {1}")]
        public string INSERT_DATE { get; set; }

    }
}