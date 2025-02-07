using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReportM2OST.Models
{
    public class PYPRequest
    {
        public int orgid { get; set; }
        public List<string> category { get; set; }
        //public List<string> assessment_id { get; set; }
        public List<string> rolewise { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public List<string> regionwise { get; set; }
        public List<string> user_designation { get; set; }
        public List<string> usercity { get; set; }
        public List<string> categoryText { get; set; }
    }
}