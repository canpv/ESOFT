using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Xml;
using Esso.ViewModels;
using Esso.Web.ViewModels;

namespace Esso.Web.Helpers
{
    public class WeatherAPI
    {
        private const string APIKEY = "b7204938043031ab1c7e0eb6df69baef";
        private string CurrentURL;
        public XmlDocument xmlDocument;

        private void SetCurrentURL(string location)
        {
            CurrentURL = "http://api.openweathermap.org/data/2.5/forecast?q="
                + location + "&mode=xml&units=metric&APPID=" + APIKEY;
        }
        public WeatherAPI(string city)
        {
            SetCurrentURL(city);
            xmlDocument = GetXML(CurrentURL);
        }
        private XmlDocument GetXML(string CurrentURL)
        {
            using (WebClient client = new WebClient())
            {
                string xmlContent = client.DownloadString(CurrentURL);
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(xmlContent);
                return xmlDocument;
            }
        }

        public WeatherData GetMeteoData()
        {

            WeatherData wdata = new WeatherData();
            XmlNode temp_node = xmlDocument.SelectSingleNode("//sun");
            XmlAttribute temp_value = temp_node.Attributes["rise"];
            XmlAttribute temp_value2 = temp_node.Attributes["set"];
            wdata._sunrise = Convert.ToDateTime(temp_value.Value).AddHours(3);
            wdata._sunset = Convert.ToDateTime(temp_value2.Value).AddHours(3);
            return wdata;
        }

    }
    
}