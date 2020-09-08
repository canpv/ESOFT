using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Esso.Web.ViewModels
{
    public class CompanyUserViewModel
    {

        public int ID { get; set; }

        public int COMPANY_ID { get; set; }
        

        [DisplayName("Cmmpany Name")]
        public string COMPANY_NAME { get; set; }
        
        [DisplayName("Auth.")]
        public bool AUTH { get; set; }

        public CompanyUserViewModel()
        {
            AUTH = false;
        }
    }
}
