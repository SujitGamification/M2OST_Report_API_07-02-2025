using ReportM2OST.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace ReportM2OST.Controllers
{

    [EnableCorsAttribute(origins: "*", headers: "*", methods: "*")]
    public class PYPController : ApiController
    {
        [HttpOptions]
        public HttpResponseMessage Options()
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Headers.Add("Access-Control-Allow-Origin", "*");
            response.Headers.Add("Access-Control-Allow-Methods", "GET, POST");
            response.Headers.Add("Access-Control-Allow-Headers", "Content-Type"); // Specify allowed headers here
            return response;
        }



        [Route("~/api/PYPRequest")]
        [AllowAnonymous]
        [HttpPost]
        [HttpOptions]
        public HttpResponseMessage PYPRequest(PYPRequest request)
        {
            try
            {
                if (request.orgid == 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid orgid request");
                }

                CommonFunction employeeService = new CommonFunction();


                string[] category;
                string[] rolewise;
                string[] assessment_id;
                string[] regionwise;
                string[] user_designation;
                string[] usercity;
                string[] ID_category;

                var orgid = request.orgid;
                var category1 = request.category;
                var rolewise1 = request.rolewise;
               // var assessment_id1 = request.assessment_id;
                var startDate = request.StartDate;
                var endDate = request.EndDate;
                var regionwise1 = request.regionwise;
                var user_designation1 = request.user_designation;
                var usercity1 = request.usercity;
                var ID_category1 = request.categoryText;

                category = new string[category1.Count]; // Initialize the category array

                for (int i = 0; i < category1.Count; i++)
                {
                    category[i] = category1[i];
                }

                rolewise = new string[rolewise1.Count]; // Initialize the category array

                for (int i = 0; i < rolewise1.Count; i++)
                {
                    rolewise[i] = rolewise1[i];
                }

                //assessment_id = new string[assessment_id1.Count]; // Initialize the category array

                //for (int i = 0; i < assessment_id1.Count; i++)
                //{
                //    assessment_id[i] = assessment_id1[i];
                //}

                regionwise = new string[regionwise1.Count]; // Initialize the category array

                for (int i = 0; i < regionwise1.Count; i++)
                {
                    regionwise[i] = regionwise1[i];
                }

                user_designation = new string[user_designation1.Count]; // Initialize the category array

                for (int i = 0; i < user_designation1.Count; i++)
                {
                    user_designation[i] = user_designation1[i];
                }

                usercity = new string[usercity1.Count]; // Initialize the category array

                for (int i = 0; i < usercity1.Count; i++)
                {
                    usercity[i] = usercity1[i];
                }

                ID_category = new string[ID_category1.Count]; // Initialize the category array

                for (int i = 0; i < ID_category1.Count; i++)
                {
                    ID_category[i] = ID_category1[i];
                }


                var employeeInfo1 = employeeService.PYPCourse(orgid, category, rolewise, startDate, endDate, regionwise, user_designation, usercity, ID_category).Result;

                if (employeeInfo1 == null)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, "Invalid username or password");
                }

                return Request.CreateResponse(HttpStatusCode.OK, employeeInfo1);
            }
            catch (Exception ex)
            {
                // Log the exception (ex) details here for further investigation
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred while processing your request");
            }
        }


        [Route("~/api/Getcoursepyp")]
        [AllowAnonymous]
        [HttpGet]
        [HttpOptions]
        public HttpResponseMessage Getcourse(int orgid, string id_category_heading, string monthwise)
        {
            try
            {
                string year = string.Empty;
                string date = monthwise;
                string[] parts = date.Split('/');

                string month = parts[0];
                year = parts[1];
                if (orgid == 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid orgid request");
                }

                CommonFunction employeeService = new CommonFunction();
                var employeeInfo = employeeService.tbl_categorypyp(orgid, id_category_heading, year, month);

                if (employeeInfo == null)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, "Invalid");
                }

                return Request.CreateResponse(HttpStatusCode.OK, employeeInfo);
            }
            catch (Exception ex)
            {
                // Log the exception (ex) details here for further investigation
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred while processing your request");
            }
        }
    }
}
