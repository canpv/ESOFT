using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Esso.Web.ViewModels
{
    public class TBL_PR_MONTH_DTO
    {
        public List<TBL_PR_MONTH> listPR { get; set; }
        public List<float> listTarget { get; set; }
        public float toplamEnerji { get; set; }
        public float toplamIsinim { get; set; }

        public float toplamPR { get; set; }
        public float ortalamaPR { get; set; }
        public float ortalamaEnerji { get; set; }
        public float ortalamaIsinim { get; set; }
        public float anlikPR { get; set; }
        public float anlikEnerji { get; set; }

        public float _target { get; set; }

    }
    public class TBL_OZET_MONTH
    {

        public int month { get; set; }
        public float? h2_wp_minus { get; set; }
        public float? h2_wp_plus { get; set; }


    }
    public class TBL_PR_MONTH
    {

        public int month { get; set; }
        public float? energy { get; set; }
        public float? irradiationSum { get; set; }
        public float? pr { get; set; }
        public float? target { get; set; }
        


    }
    public class TBL_PR_YEARLY
    {

        public int year { get; set; }
        public float? energy { get; set; }
        public float? pr { get; set; }
        public float? target { get; set; }

    }

}