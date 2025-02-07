using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReportM2OST.Models
{
    public class MonthlyTopperRequest
    { 
        public int orgid { get; set; }
        public int month { get; set; }
        public List<string> Id_assessment { get; set; }
        public List<string> rolewise { get; set; }
        public List<string> regionwise { get; set; }
        public List<string> user_designation { get; set; }

    }
}