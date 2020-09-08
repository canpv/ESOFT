using Esso.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Esso.Web.ViewModels
{   
    public class AlarmListVDTO
    {

        public int ID { get; set; }

        //[Required]
        //[StringLength(256, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 3)]
        public string INV_NAME { get; set; }

        public string DESC { get; set; }
        public string ASSIGNER_ID { get; set; }
        public string ASSIGNEE_ID { get; set; }


        public DateTime ALARM_DATE { get; set; }
    }

  
}
