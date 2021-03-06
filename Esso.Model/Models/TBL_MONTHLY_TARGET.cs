﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Esso.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class TBL_MONTHLY_TARGET
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public int STATION_ID { get; set; }

        public int MONTH { get; set; }
        public float? VALUE { get; set; }
        public bool IS_DELETED { get; set; }
        [StringLength(128, ErrorMessage = "Maximum length is {1}")]
        public string UPDATE_USER { get; set; }
        public DateTime? INSTALL_DATE { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public DateTime? UPDATED_DATE { get; set; }
        
        public TBL_MONTHLY_TARGET()
        {
            CREATED_DATE = DateTime.Now;
            IS_DELETED = false;
        }
    }
}
