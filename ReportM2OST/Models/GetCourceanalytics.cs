using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReportM2OST.Models
{
    public class GetCourceanalytics
    {
        public List<string> Id_deparment { get; set; }
        public List<string> regionwise { get; set; }
        public List<string> rolewise { get; set; }
        public List<string> ID_category { get; set; }
        public List<string> Id_assessment { get; set; }
        public string startdate { get; set; }
        public string enddate { get; set; }
        public int orgid { get; set; }
    }
}