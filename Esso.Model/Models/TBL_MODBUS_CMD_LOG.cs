using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Esso.Models
{
   public partial class TBL_MODBUS_CMD_LOG
    {
        [Key]
        public int ID { get; set; }
        public int? STATION_ID { get; set; }
        public string USER_ID { get; set; }
        public int ADDRESS { get; set; }
        public int? OLD_VALUE { get; set; }
        public int VALUE { get; set; }
        public System.DateTime INSERT_DATE { get; set; }
    }
}
