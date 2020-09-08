using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Esso.Models
{
   public class TBL_MODBUS_DATA
    {
        [Key]
        public int ID { get; set; }
        public int STATION_ID { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public System.DateTime? UPDATED_DATE { get; set; }
        public int ADDRESS { get; set; }
        public int VALUE { get; set; }



    }
}
