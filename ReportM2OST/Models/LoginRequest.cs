using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReportM2OST.Models
{
    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class UserLoginStatusRequest
    {
        public int orgid { get; set; }
        public List<string> Id_department { get; set; }
        public List<string> rolewise { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public List<string> regionwise { get; set; }
        public List<string> user_designation { get; set; }
        public List<string> usercity { get; set; }
        public List<string> daycount { get; set; }
    }

}