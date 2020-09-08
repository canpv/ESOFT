using Esso.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Esso.Web.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [StringLength(256, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 3)]
        [DisplayName("Username")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [DisplayName("Password")]
        public string Password { get; set; }

        [DisplayName("Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        public RegisterViewModel()
        {
            PUSH_NOT = false;
            //SHOW_MONEY_NAME = (SHOW_MONEY == null ? "SHOW" : SHOW_MONEY == 1 ? "NOT SHOW" : "Unknown");
        }

        public string ID { get; set; }

        [Required]
        [EmailAddress]
        [DisplayName("Email")]
        public string Email { get; set; }


        [Required]
        [StringLength(256, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 3)]
        [DisplayName("User Name")]
        public string UserName { get; set; }

        [DisplayName("Password")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 3)]
        public string Password { get; set; }

        [DisplayName("Push Notification")]
        public bool? PUSH_NOT { get; set; }


        public int? SHOW_MONEY { get; set; }
        public int? SEND_MAIL { get; set; }
        //public string SHOW_MONEY_NAME { get; set; }
        [DisplayName("Demo User")]
        public bool? IS_DEMO { get; set; }
        public DateTime? CREATED_DATE { get; set; }

        [DisplayName("Company")]
        List<int> CompanyIds { get; set; }
    }




}
