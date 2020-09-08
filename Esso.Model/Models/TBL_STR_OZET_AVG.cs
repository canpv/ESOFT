using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Esso.Model.Models
{
    public class TBL_STR_OZET_AVG
    {
        [Key]
        public int ID { get; set; }
        public int STATION_ID { get; set; }
        public int STRING_ID { get; set; }
        public float VALUE { get; set; }
        public long? TARIH_NUMBER { get; set; }

    }
}
