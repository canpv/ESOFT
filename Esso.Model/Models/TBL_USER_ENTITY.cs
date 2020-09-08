using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Esso.Models
{
	public class TBL_USER_ENTITY
	{
		[Key]
		public int ID { get; set; }
		public string USER_ID { get; set; }
		public int? SHOW_MONEY { get; set; }
        public int? SEND_MAIL { get; set; }
		public int? DEVELOPER { get; set; }

	}
}
