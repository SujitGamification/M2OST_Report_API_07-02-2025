using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReportM2OST.Models
{
    public class Getuserdetails
    {
        public int orgid_userdetails { get; set; }
        public List<string> Id_deparment { get; set; }
        public List<string> rolewise { get; set; }
        public List<string> regionwise { get; set; }
        public List<string> usercity { get; set; }
        public List<string> user_designation { get; set; }
        public string startdate { get; set; }
        public string enddate { get; set; }
        public string Status { get; set; }
    }
}