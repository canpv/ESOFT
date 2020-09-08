using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Esso.Web.ViewModels
{
    public class RainGraph_DTO
    {
        public DateTime date { get; set; }
        public float? yagis_miktari { get; set; }
    }
}