using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Esso.ViewModels
{
    public class LayoutGroupViewModel
    {
        public LayoutGroupViewModel()
        {
            GROUPS = new List<GroupViewModel2>();
        }

        public int COMPANY_ID { get; set; }
        public string COMPANY_NAME { get; set; }
        public string DEMO_COMPANY_NAME { get; set; }
        public List<GroupViewModel2> GROUPS { get; set; }

    }
    public class GroupViewModel2
    {
        public GroupViewModel2()
        {
            STATIONS = new List<StationViewModel2>();
        }
        public string GROUP_NAME { get; set; }
        public string DEMO_GROUP_NAME { get; set; }
        public int GROUP_ID { get; set; }
        public List<StationViewModel2> STATIONS { get; set; }
    }

    public class StationViewModel2
    {
        public string STATION_NAME { get; set; }
        public string PHOTO_PATH { get; set; }
        public int STATION_ID { get; set; }
        public float ENERJI { get; set; }
        public float PR { get; set; }
        public float FINANCIAL { get; set; }
        public float EXCHANGE_RATE { get; set; }
        public bool CON_STATUS { get; set; }
		public int? TAG_TEMP_ID { get; set; }

		public bool isString { get; set; }
        public bool isEkk { get; set; }
        public bool isMeteorology { get; set; }
        public string Description { get; set; }
        public string DEMO_NAME { get; set; }
        public float specificYield { get; set; }
        public int? isMoney { get; set; }
		public int ActiveInvCount { get; set; }
        public int? station_type { get; set; }
    }
}
