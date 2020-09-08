using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Esso.Models
{
   public class TBL_MODBUS_TAG
    {
        [Key]
        public int ID { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public System.DateTime? UPDATED_DATE { get; set; }
        public int ADDRESS { get; set; }  
        public string NAME { get; set; }
        public int ADDRESS_TYPE { get; set; }
        public int IS_DELETED { get; set; }

    }
}
