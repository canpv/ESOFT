using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Esso.Web.ViewModels
{
    public class Layout_Station_DTO
    {
        public int STATION_ID { get; set; }
        public string STATION_NAME { get; set; }
        public string DEMO_STATION_NAME { get; set; }
        public int GROUP_ID { get; set; }
        public int COMPANY_ID { get; set; }
        public int? STATION_TYPE { get; set; }
    }
    public class Layout_Company_DTO
    {
        public Layout_Company_DTO()
        {
            listGroup = new List<Layout_Group_DTO>();
        }
        public int COMPANY_ID { get; set; }
        public int GROUP_ID { get; set; }
        public string COMPANY_NAME { get; set; }
        public string DEMO_COMPANY_NAME { get; set; }
        public List<Layout_Group_DTO> listGroup { get; set; }
    }
    public class Layout_Group_DTO
    {
        public Layout_Group_DTO()
        {
            listStation = new List<Layout_Station_DTO>();
        }
        public int GROUP_ID { get; set; }
        public string GROUP_NAME { get; set; }
        public string DEMO_GROUP_NAME { get; set; }
        public int COMPANY_ID { get; set; }
        public List<Layout_Station_DTO> listStation { get; set; }
    }
}
       
         