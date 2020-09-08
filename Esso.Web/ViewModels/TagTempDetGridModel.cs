using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Esso.ViewModels
{
    public class TagTempDetGridModel
    {
        [Key]
        public int ID { get; set; }
        
        [Required]
        public int TAG_ID { get; set; }

        [Required]
        public int TEMPLATE_ID { get; set; }
        

        [Required]
        public int INV_NO { get; set; }

        [Required]
        public string TAG_NAME { get; set; }

        [StringLength(10, ErrorMessage = "Maximum length is {1}")]
        public string ADDRESS { get; set; }

        public bool IS_DIGITAL { get; set; }

        //[Required]
        //[StringLength(100, ErrorMessage = "Maximum length is {1}")]
        //[DisplayName("Group Name")]
        //public string NAME { get; set; }
        //public bool IS_DELETED { get; set; }
        //[Required]
        //public string UPDATE_USER { get; set; }
        //public DateTime? CREATED_DATE { get; set; }
        //public DateTime? UPDATED_DATE { get; set; }

        public TagTempDetGridModel()
        {
            //CREATED_DATE = DateTime.Now;
            //IS_DELETED = false;
        }
    }
}
