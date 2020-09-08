using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Esso.ViewModels
{
    public class StationGroupViewModel
    {
        [Key]
        public int ID { get; set; }

        
        [Required]
        [HiddenInput(DisplayValue = false)]
        public int COMPANY_ID { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Maximum length is {1}")]
        [DisplayName("Group Name")]

        public string NAME { get; set; }
        public bool IS_DELETED { get; set; }
        [Required]
        public string UPDATE_USER { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public DateTime? UPDATED_DATE { get; set; }


        public StationGroupViewModel()
        {
            CREATED_DATE = DateTime.Now;
            IS_DELETED = false;
        }
    }
}
