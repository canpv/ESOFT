using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Esso.Models
{
    public class TBL_DIGITAL_LOG_LIVE
    {
        [Key]
        public int ID { get; set; }

        public int STATION_ID { get; set; }
        public int TAG_ID { get; set; }

        public Nullable<short> DESC { get; set; }
        //public int DESC { get; set; }
        public bool IS_DELETED { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public DateTime? UPDATED_DATE { get; set; }
        public string ADRESS { get; set; }
        public TBL_DIGITAL_LOG_LIVE()
        {
            CREATED_DATE = DateTime.Now;
            IS_DELETED = false;
        }
    }
}
