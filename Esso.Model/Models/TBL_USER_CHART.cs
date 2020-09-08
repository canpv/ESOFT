using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Esso.Models
{
	public class TBL_USER_CHART
	{
		[Key]
		public int ID { get; set; }
		public string USER_ID { get; set; }
		public string CHART_NAME { get; set; }

        public int TYPE { get; set; }
    }
}
