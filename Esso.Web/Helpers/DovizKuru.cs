using Esso.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Web;
using System.Xml;

namespace Esso.Web.Helpers
{
    public class DovizKuru
    {
        
        public static double GetXMLBuyingUSD()
        {
            double _kur = 0;
            try
            {         
                Thread.CurrentThread.CurrentCulture = new CultureInfo("tr-TR");

                // Bugün (en son iş gününe) e ait döviz kurları için
                string today = "http://www.tcmb.gov.tr/kurlar/today.xml";
                //// 14 Şubat 2013 e ait döviz kurları için
                //string anyDays = "http://www.tcmb.gov.tr/kurlar/201302/14022013.xml";

                var xmlDoc = new XmlDocument();
                xmlDoc.Load(today);

                //DateTime exchangeDate = Convert.ToDateTime(xmlDoc.SelectSingleNode("//Tarih_Date").Attributes["Tarih"].Value);

                string USD = xmlDoc.SelectSingleNode("Tarih_Date/Currency[@Kod='USD']/ForexBuying").InnerXml;
                _kur = Convert.ToDouble(USD.Replace(".", ","));
                //Console.WriteLine(string.Format("Tarih {0} USD   : {1}", exchangeDate.ToShortDateString(), USD));
            }
            catch (Exception ex)
            {
                _kur = 0;
            }
             
            return _kur; 
        }
        public static double GetEndDataBuyingUSD()
        {
            double _kur = 0;
            try
            {
                EssoEntities DB = new EssoEntities();
                _kur = DB.exchange.OrderByDescending(a => a.EXCHANGE_DATE).FirstOrDefault().BUYING_VALUE.Value;
            }
            catch (Exception ex)
            {
                _kur = 0;
            }

            return Math.Round(_kur,4);
        }

        public static decimal GetJsonBuyingUSD()
        {
            Dovizcom _kur = null;
            try
            {
                using (WebClient client = new WebClient())
                {
                    //var json = client.DownloadString("http://www.doviz.com/api/v1/currencies/all/latest");
                    var json = client.DownloadString("https://www.doviz.com/api/v1/currencies/USD/latest");
                    _kur = JsonConvert.DeserializeObject<Dovizcom>(json);
                }
            }
            catch (Exception ex)
            {
                _kur.buying = 0;
            }
            
            return _kur.buying;
        }
        public class Dovizcom
        {

            public string code { get; set; }
            public string name { get; set; }
            public string full_name { get; set; }
            public decimal selling { get; set; }
            public decimal buying { get; set; }
            public decimal currency { get; set; }
            public decimal change_rate { get; set; }
            public int update_date { get; set; }
        }
    }
}