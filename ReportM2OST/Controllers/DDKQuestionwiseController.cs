using Google.Protobuf.WellKnownTypes;
using Org.BouncyCastle.Asn1.Ocsp;
using ReportM2OST.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;

namespace ReportM2OST.Controllers
{
    [EnableCorsAttribute(origins: "*", headers: "*", methods: "*")]
    public class DDKQuestionwiseController : ApiController
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

        [Route("~/api/DDKorgid")]
        [AllowAnonymous]
        [HttpGet]
        [HttpOptions]
        public HttpResponseMessage DDKorgid(int orgid)
        {
            try
            {
                if (orgid == 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid orgid request");
                }

                CommonFunction employeeService = new CommonFunction();
                var employeeInfo = employeeService.GetDDKorgid(orgid);

                if (employeeInfo == null)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, "Invalid username or password");
                }

                return Request.CreateResponse(HttpStatusCode.OK, employeeInfo);
            }
            catch (Exception ex)
            {
                // Log the exception (ex) details here for further investigation
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred while processing your request");
            }
        }



        [Route("~/api/DDKQuestionan")]
        [AllowAnonymous]
        [HttpPost]
        [HttpOptions]
        public async Task<HttpResponseMessage> DDKQuestionan(DDKRequest request)
        {

            try
            {
                // Check if orgid_userdetails is valid
                if (request.orgid_userdetails == null || request.orgid_userdetails == 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid orgid request");
                }

                // Initialize variables
                string[] Id_deparment = new string[0];
                string[] id_learning_academy_brief = new string[0];
                string[] orgIdcoroebus_List = new string[0];

                //string[] rolewise_List = new string[0];
                //string[] regionwise_List = new string[0];
                //string[] usercity_List = new string[0];

                var orgid_userdetails = request.orgid_userdetails;

                IEnumerable<dynamic> employeeInfo1 = null;

                CommonFunction employeeService = new CommonFunction();

                // Process Id_deparment and id_learning_academy_brief if they are provided
                if (request.Id_deparment != null && request.Id_deparment.Count != 0)
                {
                    Id_deparment = request.Id_deparment.Select(id => id.ToString()).ToArray();
                }


                //comment out
                //var orgIdcoroebus = employeeService.Getorgidcoroebus(Id_deparment);

                // orgIdcoroebus_List = orgIdcoroebus.Select(id => id.ToString()).ToArray();
                //

                if (request.id_learning_academy_brief != null && request.id_learning_academy_brief.Count != 0)
                {
                    id_learning_academy_brief = request.id_learning_academy_brief.Select(id => id.ToString()).ToArray();
                }

                //if (request.rolewise != null && request.rolewise.Count != 0)
                //{
                //    rolewise_List = request.rolewise.Select(id => id.ToString()).ToArray();
                //}

                //if (request.regionwise != null && request.regionwise.Count != 0)
                //{
                //    rolewise_List = request.regionwise.Select(id => id.ToString()).ToArray();
                //}

                //if (request.usercity != null && request.usercity.Count != 0)
                //{
                //    usercity_List = request.regionwise.Select(id => id.ToString()).ToArray();
                //}

                //if (request.usercity != null && request.usercity.Count != 0)
                //{
                //    usercity_List = request.regionwise.Select(id => id.ToString()).ToArray();
                //}

                //employeeInfo1 = await employeeService.DDKQuestionan(orgIdcoroebus_List, id_learning_academy_brief);

                employeeInfo1 = await employeeService.DDKQuestionannotincorbus(Id_deparment, id_learning_academy_brief);


                // Check if employeeInfo1 is null
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



        [Route("~/api/DDKcategoryname")]
        [AllowAnonymous]
        [HttpGet]
        [HttpOptions]
        public HttpResponseMessage DDKcategory_name(string Id_deparment)
        {
            try
            {
                if (Id_deparment == "0")
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid orgid request");
                }

             

                CommonFunction employeeService = new CommonFunction();

                // comment for cobus
                //var orgIds = employeeService.Getorgidcoroebus1(Id_deparment);
                //var orgAppend = new StringBuilder();
                //foreach (var orgId in orgIds)
                //{
                //    if (orgAppend.Length > 0)
                //    {
                //        orgAppend.Append(",");
                //    }
                //    orgAppend.Append(orgId);
                //}

                //var orgIdsString = orgAppend.ToString();
                //var employeeInfo = employeeService.GetDDKcategory_name(orgIdsString);
                //
                var employeeInfo = employeeService.GetDDKcategory_name1(Id_deparment);

                if (employeeInfo == null)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, "Invalid username or password");
                }

                return Request.CreateResponse(HttpStatusCode.OK, employeeInfo);
            }
            catch (Exception ex)
            {
                // Log the exception (ex) details here for further investigation
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred while processing your request");
            }
        }


        [Route("~/api/DDKsubcategory")]
        [AllowAnonymous]
        [HttpGet]
        [HttpOptions]
        public HttpResponseMessage DDKsubcategory(string Id_deparment, string subcategoryId)
        {
            try
            {
                if (subcategoryId == "0")
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid orgid request");
                }

                CommonFunction employeeService = new CommonFunction();

                //comment for corubus
                //var orgIds = employeeService.Getorgidcoroebus1(Id_deparment);

                //var orgAppend = new StringBuilder();
                //foreach (var orgId in orgIds)
                //{
                //    if (orgAppend.Length > 0)
                //    {
                //        orgAppend.Append(",");
                //    }
                //    orgAppend.Append(orgId);
                //}

                //var orgIdsString = orgAppend.ToString();
                //
                //var employeeInfo = employeeService.DDKsubcategory(orgIdsString, subcategoryId);
                var employeeInfo = employeeService.DDKsubcategorynotincrobus(Id_deparment, subcategoryId);

                if (employeeInfo == null)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, "Invalid username or password");
                }

                return Request.CreateResponse(HttpStatusCode.OK, employeeInfo);
            }
            catch (Exception ex)
            {
                // Log the exception (ex) details here for further investigation
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred while processing your request");
            }
        }


        [Route("~/api/DDKacademybrief")]
        [AllowAnonymous]
        [HttpGet]
        [HttpOptions]
        public HttpResponseMessage DDKacademybrief(string Id_deparment, string id_learning_academybrief)
        {
            try
            {
                if (Id_deparment == "0")
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid orgid request");
                }

                CommonFunction employeeService = new CommonFunction();
                //comment out
                //var orgIds = employeeService.Getorgidcoroebus1(Id_deparment);
                //var orgAppend = new StringBuilder();
                //foreach (var orgId in orgIds)
                //{
                //    if (orgAppend.Length > 0)
                //    {
                //        orgAppend.Append(",");
                //    }
                //    orgAppend.Append(orgId);
                //}

                //var orgIdsString = orgAppend.ToString();
                //
                //var employeeInfo = employeeService.DDKacademybrief(orgIdsString, id_learning_academybrief);
                var employeeInfo = employeeService.DDKacademybriefnotincrobus(Id_deparment, id_learning_academybrief);

                if (employeeInfo == null)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, "Invalid username or password");
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
