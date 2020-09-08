using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Esso.Web.ViewModels
{
    public class RoleGridModel
    {
        public string Id { get; set; }
                
        [Required]
        [DisplayName("Role")]
        public string Name { get; set; }

    }
}
