using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Esso.Web.ViewModels
{
    public class AlarmStatusDTO
    {

        public int ID { get; set; }

        public int? INVERTER_ID { get; set; }

        public int? STATION_ID { get; set; }

        public string ERROR_NUMBER { get; set; }

        public int? STATUS { get; set; }

        public DateTime? START_DATE { get; set; }
        public DateTime? END_DATE { get; set; }

        public string INV_NAME { get; set; }

        public string ERROR_NUMBER_NAME { get; set; }

        public string STATION_NAME { get; set; }
        public int? TYPE { get; set; }
        public int? ALARM_DEF_ID { get; set; }
        public int? ALARM_VALUE { get; set; }
        public string ALARM_MESSAGE { get; set; }
    }

    public class AlarmStatusExportDTO
    {
        public int ID { get; set; }
        public string DEVICE { get; set; }
        public string ERROR_DEFINITION { get; set; }
        public string ALERT_START_DATE { get; set; }
        public string ALERT_END_DATE { get; set; }


    }

    public class CompanyAlarmStatusExportDTO
    {
        public int ID { get; set; }
        public string STATION_NAME { get; set; }
        public string DEVICE { get; set; }
        public string ERROR_DEFINITION { get; set; }
        public string ALERT_START_DATE { get; set; }
        public string ALERT_END_DATE { get; set; }


    }
}
