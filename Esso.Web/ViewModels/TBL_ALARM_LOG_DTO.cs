using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Esso.Models
{
	public class TBL_ALARM_LOG
	{
		[Key]
		public int ID { get; set; }
		public int STATION_ID { get; set; }
		public Nullable<System.DateTime> Tarih { get; set; }
		public int INV_ID { get; set; }
		public int TAG_ID { get; set; }

		[StringLength(250, ErrorMessage = "Maximum length is {1}")]
		public string DESC { get; set; }

		[StringLength(128, ErrorMessage = "Maximum length is {1}")]
		public string ASSIGNER_ID { get; set; }

		[StringLength(128, ErrorMessage = "Maximum length is {1}")]
		public string ASSIGNEE_ID { get; set; }

		public bool? PUSH_STAT { get; set; }

		public int STATE { get; set; }

		public bool IS_DELETED { get; set; }

		[StringLength(128, ErrorMessage = "Maximum length is {1}")]
		public string UPDATE_USER { get; set; }

		public DateTime? CREATED_DATE { get; set; }

		public DateTime? UPDATED_DATE { get; set; }

		public TBL_ALARM_LOG()
		{
			CREATED_DATE = DateTime.Now;
			IS_DELETED = false;
		}
	}
}
