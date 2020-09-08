using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Esso.Web.ViewModels
{
    public class TagDTO
    {
        public TagDTO()
        {
            IS_INV_TAG = false;
            IS_STRING = false;
            IS_DIGITAL = false;
        }
        public int ID { get; set; }
        
        public string NAME { get; set; }

        public Nullable<bool> IS_INV_TAG { get; set; }
        public Nullable<bool> IS_STRING { get; set; }

        public DateTime? INSERT_DATE { get; set; }

        public Nullable<bool> IS_DIGITAL { get; set; }
        

  
    }
}