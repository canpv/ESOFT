using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Esso.Web.ViewModels
{
    public class Inv_States_DTO
    {
        public DateTime _tarih { get; set; }
        public int _invId { get; set; }
        public int _globalState { get; set; }
        public int _alarmState { get; set; }
        public int _DcDcConverterState { get; set; }
        public int _DcAcConverterState { get; set; }
        public int _deratingState { get; set; }
        public float? _HEARTBEAT { get; set; }
        public float? _INVERTER_MAIN_STATUS { get; set; }
        public float? _REACTIVE_POWER { get; set; }
        public float? _GRID_VOLTAGE_VRMS { get; set; }
        public float? _GRID_FREQUENCY { get; set; }
        public float? _POWER_FACTOR { get; set; }
        public float? _CODE_OF_THE_ACTIVE_FAULT { get; set; }
        public float? _GRID_CURRENT { get; set; }
        public float? _DC_BUS_VOLTAGE { get; set; }
        public float? _GROUNDING_CURRENT { get; set; }
        public float? _SOLATION_RESISTANCE { get; set; }
        public float? _AMBIENT_TEMPERATURE { get; set; }
        public float? _HIGHEST_IGBT_TEMPERATURE_PU1 { get; set; }
        public float? _HIGHEST_IGBT_TEMPERATURE_PU2 { get; set; }
        public float? _HIGHEST_IGBT_TEMPERATURE_PU3 { get; set; }
        public float? _HIGHEST_IGBT_TEMPERATURE_PU4 { get; set; }
        public float? _CONTROL_SECTION_TEMPERATURE { get; set; }
        public float? _DAILY_KVAH_SUPPLIED { get; set; }
        public float? _TOTAL_KVAH_SUPPLIED { get; set; }
        public float? _IGBT_1_T1 { get; set; }
        public float? _IGBT_1_T2 { get; set; }
        public float? _IGBT_1_T3 { get; set; }
        public float? _IGBT_2_T1 { get; set; }
        public float? _IGBT_2_T2 { get; set; }
        public float? _IGBT_2_T3 { get; set; }
        public float? _IGBT_3_T1 { get; set; }
        public float? _IGBT_3_T2 { get; set; }
        public float? _IGBT_3_T3 { get; set; }
        public float? _IGBT_4_T1 { get; set; }
        public float? _IGBT_4_T2 { get; set; }
        public float? _IGBT_4_T3 { get; set; }

    }
}