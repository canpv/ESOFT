using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Esso.Web.ViewModels
{
    public class AlarmListDTO
    {
        [Key]
        public int ID { get; set; }
        public int TAG_ID { get; set; }
        public string OP { get; set; }
        public float VALUE { get; set; }
        public byte ALARM_TYPE { get; set; }//0 = basit, 1 dallı

        public int STATUS_NO { get; set; }

        [StringLength(250, ErrorMessage = "Maximum length is {1}")]
        public string DESC { get; set; }
    }
}