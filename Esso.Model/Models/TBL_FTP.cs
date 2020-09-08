using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Esso.Models
{
    public class TBL_FTP
    {
        [Key]
        public int ID { get; set; }

        
        public int? STATION_ID { get; set; }
		
		public string IP_ADDRESS { get; set; }

		public string PORT_NO { get; set; }

		public string USER_NAME { get; set; }
		public string PASSWORD { get; set; }

		
		

	}
}
