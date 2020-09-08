using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Esso.Web.Models
{
    public class STRING_INV_DTO
    {
        public STRING_INV_DTO()
        {
            new List<CELL_DTO>();
        }
        public string INV_NO { get; set; }
        public string NAME { get; set; }
        public List<CELL_DTO> listCell { get; set; }
        public float? MAX_VALUE { get; set; }
        public float? MIN_VALUE { get; set; }
        public float? AVG_VALUE { get; set; }
    }

    public class CELL_DTO
    {
        public int ID { get; set; }
        public string INV_NO { get; set; }
        public string NAME { get; set; }
        public float? VALUE { get; set; }

    }
}