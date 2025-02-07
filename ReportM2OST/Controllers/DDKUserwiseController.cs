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
    public class DDKUserwiseController : ApiController
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

        [Route("~/api/DDKUser")]
        [AllowAnonymous]
        [HttpPost]
        [HttpOptions]
        public HttpResponseMessage DDKUser(DDKRequest request)
        {
            try
            {
                if (request.orgid_userdetails == 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid orgid request");
                }

                // var idAssessmentList = orgid.Split(',')
                //.Select(id => int.TryParse(id, out var parsedId) ? parsedId : (int?)null)
                //.Where(id => id.HasValue)
                //.Select(id => id.Value)
                //.ToList();

                string[] Id_deparment = new string[0];
                string[] id_learning_academy_brief = new string[0];
                string[] orgIdcoroebus_List = new string[0];
                string[] rolewise_List = new string[0];
                string[] regionwise_List = new string[0];
                string[] usercity_List = new string[0];
                string[] user_designation_List = new string[0];

                CommonFunction employeeService = new CommonFunction();

                if (request.Id_deparment != null && request.Id_deparment.Count != 0)
                {
                    Id_deparment = request.Id_deparment.Select(id => id.ToString()).ToArray();
                }

                // comment out
                //var orgIdcoroebus = employeeService.Getorgidcoroebus(Id_deparment);

                //orgIdcoroebus_List = orgIdcoroebus.Select(id => id.ToString()).ToArray();
               //



                if (request.id_learning_academy_brief != null && request.id_learning_academy_brief.Count != 0)
                {
                    id_learning_academy_brief = request.id_learning_academy_brief.Select(id => id.ToString()).ToArray();
                }

                if (request.rolewise != null && request.rolewise.Count != 0)
                {
                    rolewise_List = request.rolewise.Select(id => id.ToString()).ToArray();
                }

                if (request.regionwise != null && request.regionwise.Count != 0)
                {
                    regionwise_List = request.regionwise.Select(id => id.ToString()).ToArray();
                }

                if (request.usercity != null && request.usercity.Count != 0)
                {
                    usercity_List = request.usercity.Select(id => id.ToString()).ToArray();
                }

                if (request.user_designation != null && request.user_designation.Count != 0)
                {
                    user_designation_List = request.user_designation.Select(id => id.ToString()).ToArray();
                }



             // var employeeInfo1 = employeeService.DDKUser1(request.orgid_userdetails,orgIdcoroebus_List, id_learning_academy_brief, Id_deparment, rolewise_List, regionwise_List, usercity_List, user_designation_List,request.startdate_coroebus,request.enddate_coroebus);

               var employeeInfo1 = employeeService.DDKUsernotcorobus(request.orgid_userdetails, id_learning_academy_brief, Id_deparment, rolewise_List, regionwise_List, usercity_List, user_designation_List,request.startdate_coroebus,request.enddate_coroebus);
               // var employeeInfo1 = "";

                if (employeeInfo1 == null)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, "Invalid username or password");
                }

                return Request.CreateResponse(HttpStatusCode.OK, employeeInfo1);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred while processing your request");
            }
        }

        [Route("~/api/DDKtitle")]
        [AllowAnonymous]
        [HttpGet]
        [HttpOptions]
        public HttpResponseMessage DDKtitle(string orgId, string monthwise)
        {
            try
            {
                string year = string.Empty;
                string date = monthwise;
                string[] parts = date.Split('/');

                string month = parts[0];
                year = parts[1];

                if (orgId == "0")
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid id_organization");
                }

                CommonFunction employeeService = new CommonFunction();

                var employeeInfo1 = employeeService.DDKtitle(orgId, year, month);

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
