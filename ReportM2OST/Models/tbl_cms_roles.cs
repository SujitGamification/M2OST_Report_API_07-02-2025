using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReportM2OST.Models
{
    public class tbl_cms_roles
    {
        public int ID_ROLE { get; set; }
        public int ID_ORGANIZATION { get; set; }
        public string ROLENAME { get; set; }
        public string DESCRIPTION { get; set; }
        public char STATUS { get; set; }
        public string UPDATED_DATE_TIME { get; set; }
    }
}