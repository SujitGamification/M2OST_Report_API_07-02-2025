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

    public class CoursecompletionanalyticsController : ApiController
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

        [Route("~/api/GetCourseAnalytics")]
        [AllowAnonymous]
        [HttpPost]
        [HttpOptions]
        public IHttpActionResult GetCourseAnalytics(GetCourceanalytics reuqest)
        {
            string Id_deparment = reuqest.Id_deparment != null && reuqest.Id_deparment.Count != 0
                                    ? string.Join(",", reuqest.Id_deparment)
                                    : string.Empty;
            string rolewise = reuqest.rolewise != null && reuqest.rolewise.Count != 0
                                    ? string.Join(",", reuqest.rolewise)
                                    : string.Empty;
            string ID_category = reuqest.ID_category != null && reuqest.ID_category.Count != 0
                                    ? string.Join(",", reuqest.ID_category)
                                    : string.Empty;
            string Id_assessment = reuqest.Id_assessment != null && reuqest.Id_assessment.Count != 0
                                     ? string.Join(",", reuqest.Id_assessment)
                                     : string.Empty;

            CommonFunction employeeService = new CommonFunction();





            var employeeInfo1 = employeeService.GetCourseAnalyticsChangequery(reuqest.orgid, Id_deparment, rolewise,
        string.Join(",", reuqest.regionwise), ID_category, Id_assessment, reuqest.startdate, reuqest.enddate).Result;
            return Ok(employeeInfo1);

        }
    }
}
