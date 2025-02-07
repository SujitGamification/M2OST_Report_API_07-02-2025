using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReportM2OST.Models
{
    public class tbl_organization
    {
        public string ID_ORGANIZATION {  get; set; }   
        public string ORGANIZATION_NAME {  get; set; }

       
    }
    public class tbl_assessment
    {
        public string Id_assessment { get; set; }
        public string Assessment_title { get; set; }


    }

    public class tbl_assessment_title
    {
        public string Id_assessment { get; set; }
        public string Assessment_title { get; set; }
        public string id_category { get; set; }


    }
    public class tbl_assessment_org
    {
        public int orgid { get; set; }
        public string ORGANIZATION_NAME { get; set; }


    }

    public class tbl_category
    {
        public int ID_CATEGORY { get; set; }

        public string CATEGORYNAME { get; set; }
        public string Text { get; set; }


    }

    public class tbl_categorypyp
    {
     
        public string CATEGORYNAME { get; set; }
        public string Text { get; set; }


    }


    public class course
    {
        public int orgid { get; set; } 
        public string category { get; set; } 
        public string assessment_id { get; set; } 
        public string rolewise { get; set; }

        public string startDate { get; set; }
        public string endDate { get; set; }

    }

  
}