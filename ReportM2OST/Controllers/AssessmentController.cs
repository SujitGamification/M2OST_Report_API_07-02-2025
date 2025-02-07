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
    public class AssessmentController : ApiController
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

        [Route("~/api/UserAssessmentReport")]
        [AllowAnonymous]
        [HttpPost]
        [HttpOptions]
        public HttpResponseMessage AssessmentReport([FromBody] UserAssessmentRequest request)
        {
            try
            {
                if (request.orgId == 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid id_organization");
                }

                CommonFunction employeeService = new CommonFunction();

                string[] Id_assessment_ngage;
                string[] Id_deparment;
                string[] rolewise;

                string[] regionwise;
                string[] usercity;
                string[] user_designation;
               

                var orgid_ngage = request.orgId;
                var Id_assessment_1 = request.Id_assessment;
                var orgid_userdetails = request.orgid_userdetails;
                var Id_deparment_1 = request.Id_deparment;
                var rolewise_1 = request.rolewise;
                var regionwise_1 = request.regionwise;
                var usercity_1 = request.usercity;

                var user_designation_1 = request.user_designation;
                
                var startdate_ngage = request.startdate_ngage;
                var enddate_ngage = request.enddate_ngage;


                Id_assessment_ngage = new string[Id_assessment_1.Count]; 

                for (int i = 0; i < Id_assessment_1.Count; i++)
                {
                    Id_assessment_ngage[i] = Id_assessment_1[i];
                }

                Id_deparment = new string[Id_deparment_1.Count];

                for (int i = 0; i < Id_deparment_1.Count; i++)
                {
                    Id_deparment[i] = Id_deparment_1[i];
                }

                rolewise = new string[rolewise_1.Count];

                for (int i = 0; i < rolewise_1.Count; i++)
                {
                    rolewise[i] = rolewise_1[i];
                }

                regionwise = new string[regionwise_1.Count];

                for (int i = 0; i < regionwise_1.Count; i++)
                {
                    regionwise[i] = regionwise_1[i];
                }

                usercity = new string[usercity_1.Count];

                for (int i = 0; i < usercity_1.Count; i++)
                {
                    usercity[i] = usercity_1[i];
                }

                user_designation = new string[user_designation_1.Count];

                for (int i = 0; i < user_designation_1.Count; i++)
                {
                    user_designation[i] = user_designation_1[i];
                }



                var employeeInfo1 = employeeService.GetAssessmentReport(orgid_ngage, Id_assessment_ngage, orgid_userdetails, Id_deparment, rolewise, regionwise, usercity, startdate_ngage, enddate_ngage, user_designation);

                

                if (employeeInfo1 == null)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, "Invalid id_organization");
                }

                return Request.CreateResponse(HttpStatusCode.OK, employeeInfo1);
            }
            catch (Exception ex)
            {
                // Log the exception (ex) details here for further investigation
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred while processing your request");
            }
        }



        [Route("~/api/AssessmentReportQuestion")]
        [AllowAnonymous]
        [HttpPost]
        [HttpOptions]
        public HttpResponseMessage AssessmentReportQuestion([FromBody] QuestionAssessmentRequest request)
        {
            try
            {
                if (request.orgId == 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid id_organization");
                }

                CommonFunction employeeService = new CommonFunction();

                string[] Id_assessment_ngage;
                string[] Id_deparment;
                string[] rolewise;

                string[] regionwise;
                string[] usercity;
                string[] user_designation;


                var orgid_ngage = request.orgId;
                var Id_assessment_1 = request.Id_assessment;
                var orgid_userdetails = request.orgid_userdetails;
                var Id_deparment_1 = request.Id_deparment;
                var rolewise_1 = request.rolewise;
                var regionwise_1 = request.regionwise;
                var usercity_1 = request.usercity;

                var user_designation_1 = request.user_designation;

                var startdate_ngage = request.startdate_ngage;
                var enddate_ngage = request.enddate_ngage;


                Id_assessment_ngage = new string[Id_assessment_1.Count];

                for (int i = 0; i < Id_assessment_1.Count; i++)
                {
                    Id_assessment_ngage[i] = Id_assessment_1[i];
                }

                Id_deparment = new string[Id_deparment_1.Count];

                for (int i = 0; i < Id_deparment_1.Count; i++)
                {
                    Id_deparment[i] = Id_deparment_1[i];
                }

                rolewise = new string[rolewise_1.Count];

                for (int i = 0; i < rolewise_1.Count; i++)
                {
                    rolewise[i] = rolewise_1[i];
                }

                regionwise = new string[regionwise_1.Count];

                for (int i = 0; i < regionwise_1.Count; i++)
                {
                    regionwise[i] = regionwise_1[i];
                }

                usercity = new string[usercity_1.Count];

                for (int i = 0; i < usercity_1.Count; i++)
                {
                    usercity[i] = usercity_1[i];
                }

                user_designation = new string[user_designation_1.Count];

                for (int i = 0; i < user_designation_1.Count; i++)
                {
                    user_designation[i] = user_designation_1[i];
                }



                var employeeInfo1 =  employeeService.AssessmentReportQuestion1(orgid_ngage, Id_assessment_ngage, orgid_userdetails, Id_deparment, rolewise, regionwise, usercity, startdate_ngage, enddate_ngage, user_designation);
              
                if (employeeInfo1 == null)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, "Invalid id_organization");
                }

                return Request.CreateResponse(HttpStatusCode.OK, employeeInfo1);
            }
            catch (Exception ex)
            {
                // Log the exception (ex) details here for further investigation
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred while processing your request");
            }
        }



        [Route("~/api/Assessmentorgid")]
        [AllowAnonymous]
        [HttpGet]
        [HttpOptions]
        public HttpResponseMessage OrgID(int orgId)
        {
            try
            {
                if (orgId == 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid id_organization");
                }

                CommonFunction employeeService = new CommonFunction();

                var employeeInfo1 = employeeService.tbl_organization();

                if (employeeInfo1 == null)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, "Invalid id_organization");
                }

                return Request.CreateResponse(HttpStatusCode.OK, employeeInfo1);
            }
            catch (Exception ex)
            {
                // Log the exception (ex) details here for further investigation
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred while processing your request");
            }
        }


        [Route("~/api/Assessmentid")]
        [AllowAnonymous]
        [HttpGet]
        [HttpOptions]
        public HttpResponseMessage AssessmentID(int orgId, string monthwise)
        {
            try
            {
                string year = string.Empty;
                string date = monthwise;
                string[] parts = date.Split('/');

                string month = parts[0];
                year = parts[1];

                if (orgId == 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid id_organization");
                }

                CommonFunction employeeService = new CommonFunction();

                var employeeInfo1 = employeeService.tbl_assessment(orgId, year, month);

                if (employeeInfo1 == null)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, "Invalid id_organization");
                }

                return Request.CreateResponse(HttpStatusCode.OK, employeeInfo1);
            }
            catch (Exception ex)
            {
                // Log the exception (ex) details here for further investigation
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred while processing your request");
            }
        }


        [Route("~/api/UserDetails")]
        [AllowAnonymous]
        [HttpGet]
        [HttpOptions]
        public HttpResponseMessage UserDetails(int orgId)
        {
            
            try
            {
                if (orgId == 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid id_organization");
                }

                CommonFunction employeeService = new CommonFunction();


                var employeeInfo1 = employeeService.UserDetails(orgId);



                if (employeeInfo1 == null)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, "Invalid id_organization");
                }

                return Request.CreateResponse(HttpStatusCode.OK, employeeInfo1);
            }
            catch (Exception ex)
            {
                // Log the exception (ex) details here for further investigation
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred while processing your request");
            }
        }

        [Route("~/api/AssessmentQuestionList")]
        [AllowAnonymous]
        [HttpGet]
        [HttpOptions]
        public HttpResponseMessage AssessmentQuestionList(int orgId)
        {

            try
            {
                if (orgId == 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid id_organization");
                }

                CommonFunction employeeService = new CommonFunction();


                var employeeInfo1 = employeeService.AssessmentQuestionList(orgId);



                if (employeeInfo1 == null)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, "Invalid id_organization");
                }

                return Request.CreateResponse(HttpStatusCode.OK, employeeInfo1);
            }
            catch (Exception ex)
            {
                // Log the exception (ex) details here for further investigation
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred while processing your request");
            }
        }


    }
}
