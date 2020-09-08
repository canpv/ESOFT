using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Esso.Models
{
    public class TBL_STATUS_LOG
    {
        [Key]
        public int ID { get; set; }
        
        public int TAG_ID { get; set; }

        public int STATION_ID { get; set; }
        public int INV_ID { get; set; }

        [StringLength(250, ErrorMessage = "Maximum length is {1}")]
        public string DESC { get; set; }

        public bool IS_DELETED { get; set; }

        public DateTime? CREATED_DATE { get; set; }
        

        public TBL_STATUS_LOG()
        {
            CREATED_DATE = DateTime.Now;
            IS_DELETED = false;
        }
    }
}
