using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Esso.Models
{
    public class TBL_INVERTER
    {
        [Key]
        public int ID { get; set; }

        [Required]        
        public int STATION_ID { get; set; }

        //[Required]
        //[StringLength(250, ErrorMessage = "Maximum length is {1}")]
        public string NAME { get; set; }
	
		public bool IS_DELETED { get; set; }    
        public Nullable<float> INV_DC_GUC { get; set; }
        public string UPDATE_USER { get; set; }
        public DateTime? INSTALL_DATE { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public DateTime? UPDATED_DATE { get; set; }

        public TBL_INVERTER()
        {
            CREATED_DATE = DateTime.Now;
            IS_DELETED = false;
        }
    }
}
