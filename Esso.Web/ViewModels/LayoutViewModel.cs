using System;
using System.Collections.Generic;

namespace Esso.ViewModels
{
    public class LayoutViewModel
    {
        public LayoutViewModel()
        {
            STATIONS = new List<StationViewModel>();
        }

        public int COMPANY_ID { get; set; }
        public string COMPANY_NAME { get; set; }
        public string DEMO_COMPANY_NAME { get; set; }
        public List<StationViewModel> STATIONS { get; set; }
    }

    public class StationViewModel
    {
        public string STATION_NAME { get; set; }
        public string DEMO_STATION_NAME { get; set; }
        public string PHOTO_PATH { get; set; }
        public int STATION_ID { get; set; }
        public string COORDINANT { get; set; }
        public float Enerji { get; set; }
        public float Income { get; set; }
        public DateTime Tarih { get; set; }
        public bool CON_STATUS { get; set; }
        public float P_AC { get; set; }
        public float P_DC { get; set; }
        public float PR { get; set; }
        public float IRRADIATION { get; set; }
		public int ActiveInvCount { get; set; }
	}
}
