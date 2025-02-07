using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReportM2OST.Models
{
    public class tbl_cms_users
    {
        public int ID_USER { get; set; }
        public int ID_ROLE { get; set; }
        public string USERNAME { get; set; }
        public string PASSWORD { get; set; }
        public char STATUS { get; set; }
        public string UPDATED_DATE_TIME { get; set; }
        public int cmd_user_type { get; set; }
        public string employee_id { get; set; }
        public string employee_name { get; set; }
    }
}