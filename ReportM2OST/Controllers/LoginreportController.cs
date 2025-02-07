using Org.BouncyCastle.Asn1.Ocsp;
using ReportM2OST.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace ReportM2OST.Controllers
{
    [EnableCorsAttribute(origins: "*", headers: "*", methods: "*")]
    public class LoginreportController : ApiController
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

        [Route("~/api/GetLoginreport")]
        [AllowAnonymous]
        [HttpPost]
        [HttpOptions]
        public async Task<HttpResponseMessage> GetUserLoginStatus(UserLoginStatusRequest loginStatusRequest)
        {
            try
            {
                if (loginStatusRequest.orgid == 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid organization request");
                }
                CommonFunction employeeService = new CommonFunction();

                string[] rolewise;
                string[] Id_department;
                string[] regionwise;
                string[] user_designation;
                string[] usercity;

                var orgid = loginStatusRequest.orgid;
                var rolewise1 = loginStatusRequest.rolewise;
                var startDate = loginStatusRequest.StartDate;
                var endDate = loginStatusRequest.EndDate;
                var regionwise1 = loginStatusRequest.regionwise;
                var user_designation1 = loginStatusRequest.user_designation;
                var usercity1 = loginStatusRequest.usercity;
                var Id_department1 = loginStatusRequest.Id_department;

                rolewise = rolewise1.ToArray();
                regionwise = regionwise1.ToArray();
                user_designation = user_designation1.ToArray();
                usercity = usercity1.ToArray();
                Id_department = Id_department1.ToArray();

                // var employeeInfo = await employeeService.GetUserLoginStatus(userLogin.orgid, userLogin.startDate, userLogin.endDate,userLogin.rolewise);
                var employeeInfo = await employeeService.GetLoginReport(orgid, Id_department, rolewise, startDate, endDate, regionwise, user_designation, usercity);

                if (employeeInfo == null || !employeeInfo.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, "Invalid organization");
                }

                return Request.CreateResponse(HttpStatusCode.OK, employeeInfo);
            }
            catch (Exception ex)
            {
                // Log the exception (ex) details here for further investigation
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred while processing your request");
            }
        }

        [Route("~/api/GetDepartment")]
        [AllowAnonymous]
        [HttpGet]
        [HttpOptions]
        public async Task<HttpResponseMessage> GetDepartment(int orgid)
        {
            try
            {
                if (orgid == 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid organization request");
                }
                CommonFunction employeeService = new CommonFunction();
                var employeeInfo = await employeeService.GetDepartemntList(orgid);

                if (employeeInfo == null || !employeeInfo.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, "Invalid organization");
                }

                return Request.CreateResponse(HttpStatusCode.OK, employeeInfo);
            }
            catch (Exception ex)
            {
                // Log the exception (ex) details here for further investigation
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred while processing your request");
            }
        }

        [Route("~/api/GetRoleWise")]
        [AllowAnonymous]
        [HttpGet]
        [HttpOptions]
        public HttpResponseMessage GetRoleWise(int orgid,string id_deprt)
        {
            try
            {
                if (orgid == 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid id_organization");
                }

                CommonFunction employeeService = new CommonFunction();
                var employeeInfo = employeeService.tbl_csst_role(orgid,id_deprt);
              

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
