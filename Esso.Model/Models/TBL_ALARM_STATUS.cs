using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Esso.Models
{
    public class TBL_ALARM_STATUS
    {
        [Key]
        public int ID { get; set; }
		
        public int? INVERTER_ID { get; set; }

		public int? STATION_ID { get; set; }

		public string ERROR_NUMBER { get; set; }

		public int? STATUS { get; set; }

		public DateTime? START_DATE { get; set; }

		public DateTime? END_DATE { get; set; }

        public int? PROCESS_STEP { get; set; }

        public string USER_ID { get; set; }
    }
}
