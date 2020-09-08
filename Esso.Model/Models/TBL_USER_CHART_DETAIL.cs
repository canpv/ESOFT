using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Esso.Models
{
	public class TBL_USER_CHART_DETAIL
	{
		[Key]
		public int ID { get; set; }
		public int STATION_ID { get; set; }
		public int INVERTER_ID { get; set; }
		public string VALUE_TYPE { get; set; }

		public int CHART_ID { get; set; }





	}
}
