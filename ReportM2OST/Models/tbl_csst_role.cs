using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReportM2OST.Models
{
    public class tbl_csst_role
    {
        public int id_csst_role { get; set; }
        public string csst_role { get; set; }
        public int orgid { get; set; }
        public int id_deprt { get; set; }
        public string status { get; set; }

    }
}