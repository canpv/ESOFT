using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Esso.Data;
using Esso.Models;

namespace Esso.ViewModels
{
    public class LayoutGroupViewModel3
    {
        public LayoutGroupViewModel3()
        {
            GROUPS = new List<GroupViewModel2>();
        }

        public int COMPANY_ID { get; set; }
        public string COMPANY_NAME { get; set; }
        public List<GroupViewModel2> GROUPS { get; set; }

    }
    public class GroupViewModel3
    {
        public GroupViewModel3()
        {
            STATIONS = new List<StationViewModel3>();
        }
        public string GROUP_NAME { get; set; }
        public int GROUP_ID { get; set; }
        public List<StationViewModel3> STATIONS { get; set; }
    }

    public class StationViewModel3
    {
        public string STATION_NAME { get; set; }
        public string PHOTO_PATH { get; set; }
        public int STATION_ID { get; set; }
        public float ENERJI { get; set; }
        public float FINANCIAL { get; set; }
        public float EXCHANGE_RATE { get; set; }
        public bool CON_STATUS { get; set; }

    }
}
