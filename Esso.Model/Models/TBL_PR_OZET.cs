//------------------------------------------------------------------------------
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

public partial class TBL_PR_OZET
    {
        [Key]
        public int id { get; set; }
        public int STATION_ID { get; set; }
        public Nullable<System.DateTime> date { get; set; }
        public Nullable<float> enerji { get; set; }
        public Nullable<float> isinim_ortalama { get; set; }
        public Nullable<float> pr { get; set; }

        public Nullable<float> isinim { get; set; }
        public Nullable<float> gunlukUretim { get; set; }
    }
}
