using System;
using System.ComponentModel;

namespace Esso.Web.ViewModels
{
    public class RoleUserViewModel
    {

        public int ID { get; set; }

        public string ROLE_ID { get; set; }
        
        [DisplayName("Station Name")]
        public string ROLE_NAME { get; set; }

        [DisplayName("Auth.")]
        public bool AUTH { get; set; }

        public RoleUserViewModel()
        {
            AUTH = false;
        }
    }
}
