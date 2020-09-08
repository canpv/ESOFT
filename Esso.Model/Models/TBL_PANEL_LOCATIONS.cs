using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Esso.Model.Models
{
    public class TBL_PANEL_LOCATIONS
    {
        [Key]
        public int ID { get; set; }
        public int STATION_ID { get; set; }
        public int STRING_ID { get; set; }
        public string LOCATIONSX { get; set; }
        public string LOCATIONSY { get; set; }
        public int? RESIZE_WIDTH { get; set; }
        public int? RESIZE_HEIGHT { get; set; }
        public DateTime? UPDATE_DATE { get; set; }

    }
}
