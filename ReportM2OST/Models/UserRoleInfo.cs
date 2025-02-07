using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReportM2OST.Models
{
    public class UserRoleInfo
    {
        public int ID_ROLE { get; set; }
        public int ID_ORGANIZATION { get; set; }
        public string ROLENAME { get; set; }
        public string DESCRIPTION { get; set; }

        public int cmd_user_type { get; set; }
        public string employee_id { get; set; }
        public string employee_name { get; set; }
    }
}