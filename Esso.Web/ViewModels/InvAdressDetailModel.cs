using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Esso.ViewModels
{
    public class InvAdressDetailModel
    {
        public int ID { get; set; }
        public string ADDRESS { get; set; }
        public string TAG_NAME { get; set; }
    }
}
