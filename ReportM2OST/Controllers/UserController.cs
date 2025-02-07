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
    public class UserController : ApiController
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

        [Route("~/api/GetUserStatusReportList")]
        [AllowAnonymous]
        [HttpGet]
        [HttpOptions]
        public IHttpActionResult GetUserReportList(int orgid,string Id_deparment,string role_id,string region_name)
        {
            CommonFunction employeeService = new CommonFunction();
            var employeeInfo1 = employeeService.GetUserReportList(orgid, Id_deparment, role_id, region_name).Result;
            return Ok(employeeInfo1);
        }
    }
}
