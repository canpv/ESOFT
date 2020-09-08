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

    public partial class TBL_OZET
    {

    [Key]
    public int Id { get; set; }
        public int STATION_ID { get; set; }
        public System.DateTime tarih { get; set; }
        public Nullable<float> Enerji { get; set; }
        public Nullable<float> aylikEnerji { get; set; }
        public Nullable<float> yillikEnerji { get; set; }
        public Nullable<float> toplamEnerji { get; set; }
        public Nullable<float> isinim { get; set; }
        public Nullable<double> sicaklik { get; set; }
        public Nullable<float> gunlukUretim { get; set; }
        public Nullable<float> hucreSicakligi { get; set; }
        public Nullable<float> ruzgarHizi { get; set; }
        public Nullable<float> H3_Ie { get; set; }
        public Nullable<float> H3_Ia { get; set; }
        public Nullable<float> H3_Ib { get; set; }
        public Nullable<float> H3_Ic { get; set; }
        public Nullable<float> H2_Vbn { get; set; }
        public Nullable<float> H2_Van { get; set; }
        public Nullable<float> H2_Vcn { get; set; }
        public Nullable<float> H2_Vab { get; set; }
        public Nullable<float> H2_Vbc { get; set; }
        public Nullable<float> H2_Vac { get; set; }
        public Nullable<float> H2_F { get; set; }
        public Nullable<float> H2_THD_Va_ab { get; set; }
        public Nullable<float> H2_THD_Vb_bc { get; set; }
        public Nullable<float> H2_Q { get; set; }
        public Nullable<float> H2_P { get; set; }
        public Nullable<float> H2_THD_Ic { get; set; }
        public Nullable<float> H2_THD_Ib { get; set; }
        public Nullable<float> H2_THD_Ia { get; set; }
        public Nullable<float> H2_Ic { get; set; }
        public Nullable<float> H2_Ib { get; set; }
        public Nullable<float> H2_Ia { get; set; }
        public Nullable<float> H2_THD_Vc_ca { get; set; }
        public Nullable<float> H2_S { get; set; }
        public Nullable<float> H2_WQ_minus { get; set; }
        public Nullable<float> H2_WQ_plus { get; set; }
        public Nullable<float> H2_WP_minus { get; set; }
        public Nullable<float> H2_TDD_IL3 { get; set; }
        public Nullable<float> H2_CosFi { get; set; }
        public Nullable<float> H2_PF { get; set; }
        public Nullable<float> H2_Plt_IL1 { get; set; }
        public Nullable<float> H2_Plt_IL2 { get; set; }
        public Nullable<float> H2_Plt_IL3 { get; set; }
        public Nullable<float> H2_Pst_IL1 { get; set; }
        public Nullable<float> H2_Pst_IL2 { get; set; }
        public Nullable<float> H2_Pst_IL3 { get; set; }
        public Nullable<float> H2_TDD_IL1 { get; set; }
        public Nullable<float> H2_TDD_IL2 { get; set; }
        public Nullable<float> H2_WP_plus { get; set; }
        public Nullable<float> H2_Real_energy_L1 { get; set; }
        public Nullable<float> H2_Real_energy_L2 { get; set; }
        public Nullable<float> H2_Real_energy_L3 { get; set; }
        public Nullable<float> H2_Apparent_energy_L1 { get; set; }
        public Nullable<float> H2_Apparent_energy_L2 { get; set; }
        public Nullable<float> H2_Apparent_energy_L3 { get; set; }
        public Nullable<float> H2_Reaktive_energy_L1 { get; set; }
        public Nullable<float> H2_Reaktive_energy_L2 { get; set; }
        public Nullable<float> H2_Reaktive_energy_L3 { get; set; }
        public Nullable<float> H2_Reactive_eng_ind_L1 { get; set; }
        public Nullable<float> H2_Reactive_eng_ind_L2 { get; set; }
        public Nullable<float> H2_Reactive_eng_ind_L3 { get; set; }
        public Nullable<float> H2_Reactive_eng_cap_L1 { get; set; }
        public Nullable<float> H2_Reactive_eng_cap_L2 { get; set; }
        public Nullable<float> H2_Reactive_eng_cap_L3 { get; set; }
        public Nullable<float> Dc_Guc { get; set; }
        public Nullable<float> PR { get; set; }
        public Nullable<float> ISINIM_ORT { get; set; }
        public Nullable<float> PYRANOMETER { get; set; }
        public Nullable<float> MEAN_WIND_DIRECTION_1 { get; set; }

        public Nullable<float> AIR_TEMPERATURE_1 { get; set; }

        public Nullable<float> RELATIVE_HUMIDITY_1 { get; set; }

        public Nullable<float> ABSOLUTE_HUMIDITY_1 { get; set; }

        public Nullable<float> ABSOLUTE_AIR_PRESSURE_1 { get; set; }

        public Nullable<float> MEAN_WIND_DIRECTION_2 { get; set; }

        public Nullable<float> AIR_TEMPERATURE_2 { get; set; }

        public Nullable<float> RELATIVE_HUMIDITY_2 { get; set; }

        public Nullable<float> ABSOLUTE_HUMIDITY_2 { get; set; }

        public Nullable<float> ABSOLUTE_AIR_PRESSURE_2 { get; set; }

        public Nullable<float> HUCRESICAKLIGI_2 { get; set; }

        public Nullable<double> SICAKLIK_2 { get; set; }

        public Nullable<float> ISINIM_2 { get; set; }

        public Nullable<float> RUZGARHIZI_2 { get; set; }

        public Nullable<float> PYRANOMETER_2 { get; set; }

        public Nullable<float> YAGIS_MIKTARI { get; set; }
    }
}