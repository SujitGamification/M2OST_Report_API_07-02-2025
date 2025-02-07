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
    public class LoginController : ApiController
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

        [Route("~/api/Login")]
        [AllowAnonymous]
        [HttpPost]
        [HttpOptions]
        public HttpResponseMessage Login(LoginRequest loginRequest)
        {
            try
            {
                if (loginRequest.Username == null || string.IsNullOrEmpty(loginRequest.Username) || string.IsNullOrEmpty(loginRequest.Password))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid login request");
                }

                CommonFunction employeeService = new CommonFunction();
                var employeeInfo = employeeService.GetLoginInfo(loginRequest.Username, loginRequest.Password);

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
