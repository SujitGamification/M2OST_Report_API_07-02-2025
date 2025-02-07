using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReportM2OST.Models
{
    public class UserLogin
    {
        public int orgid { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public int Id_department { get; set; }
        public string rolewise { get; set; }
    }
}