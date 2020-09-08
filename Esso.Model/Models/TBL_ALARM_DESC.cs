using System;
using System.ComponentModel.DataAnnotations;

namespace Esso.Models
{
    public class TBL_ALARM_DESC
    {
        [Key]
        public int ID { get; set; }

        [StringLength(250, ErrorMessage = "Maximum length is {1}")]
        public string ERROR_NUMBER { get; set; }
        [StringLength(250, ErrorMessage = "Maximum length is {1}")]
        public string ERROR_DESC { get; set; }
        public int? TYPE { get; set; }

    }
}
