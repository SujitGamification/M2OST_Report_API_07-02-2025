using ReportM2OST.Models;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Security;

namespace ReportM2OST.Controllers
{
    [EnableCorsAttribute(origins: "*", headers: "*", methods: "*")]
    public class MonthlytoppersreportController : ApiController
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

        [Route("~/api/MonthlyTopper")]
        [AllowAnonymous]
        [HttpPost]
        [HttpOptions]
        public IHttpActionResult MonthlyTopper(MonthlyTopperRequest request)
        {
            try
            {
                if (request.orgid == 0)
                {
                    return BadRequest("Invalid id_organization");
                }

                CommonFunction employeeService = new CommonFunction();


                //string[] month;
                string[] rolewise;
                string[] id_assessment;
                string[] roleId;
                string[] region;
                string[] user_designation;


                var orgid = request.orgid;
                var month = request.month;
                var id_assessment_1 = request.Id_assessment;
                var roleId_1 = request.rolewise;
                var region_1 = request.regionwise;
                var user_designation1 = request.user_designation;




                id_assessment = new string[id_assessment_1.Count]; // Initialize the category array

                for (int i = 0; i < id_assessment_1.Count; i++)
                {
                    id_assessment[i] = id_assessment_1[i];
                }

                roleId = new string[roleId_1.Count]; // Initialize the category array

                for (int i = 0; i < roleId_1.Count; i++)
                {
                    roleId[i] = roleId_1[i];
                }

                region = new string[region_1.Count]; // Initialize the category array

                for (int i = 0; i < region_1.Count; i++)
                {
                    region[i] = region_1[i];
                }


                user_designation = new string[user_designation1.Count]; // Initialize the category array

                for (int i = 0; i < user_designation1.Count; i++)
                {
                    user_designation[i] = user_designation1[i];
                }


                var employeeInfo = employeeService.GetMonthlyReport(orgid, month, id_assessment, roleId, region, user_designation);


                if (employeeInfo == null || !employeeInfo.Any())
                {
                    return BadRequest("Invalid id_organization");
                }

                return Ok(employeeInfo);
            }
            catch (Exception ex)
            {
                // Log the exception (ex) details here for further investigation
                return InternalServerError(new Exception("An error occurred while processing your request"));
            }
        }


        [Route("~/api/GetAssessmentlist")]
        [AllowAnonymous]
        [HttpGet]
        [HttpOptions]
        public HttpResponseMessage GetAssessmentlist(int orgid)
        {
            try
            {
                if (orgid == 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid id_organization");
                }

                CommonFunction employeeService = new CommonFunction();
                var employeeInfo = employeeService.tbl_assessmentlist(orgid);


                if (employeeInfo == null)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, "Invalid id_organization");
                }

                return Request.CreateResponse(HttpStatusCode.OK, employeeInfo);
            }
            catch (Exception ex)
            {
                // Log the exception (ex) details here for further investigation
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred while processing your request");
            }
        }



        [Route("~/api/GetRegionlist")]
        [AllowAnonymous]
        [HttpPost]
        [HttpOptions]
        public HttpResponseMessage GetRegionlist(MonthlyTopperRequest request1)
        {
            try
            {
                string[] roleId;

                var orgid = request1.orgid;
             
                var roleId_1 = request1.rolewise;
             
                CommonFunction employeeService = new CommonFunction();


                roleId = new string[roleId_1.Count]; // Initialize the category array

                for (int i = 0; i < roleId_1.Count; i++)
                {
                    roleId[i] = roleId_1[i];
                }

               
                var employeeInfo = employeeService.GetRegionlist(orgid, roleId);


                if (employeeInfo == null)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, "Invalid id_organization");
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
