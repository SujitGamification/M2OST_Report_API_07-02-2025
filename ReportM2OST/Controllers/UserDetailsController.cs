using Org.BouncyCastle.Asn1.Ocsp;
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

    public class UserDetailsController : ApiController
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

        [Route("~/api/GetUserDetailsStatusReportList")]
        [AllowAnonymous]
        [HttpPost]
        [HttpOptions]
        public IHttpActionResult GetUserDetails(Getuserdetails reuqest)
        {
            string Id_deparment = reuqest.Id_deparment != null && reuqest.Id_deparment.Count != 0
                                     ? string.Join(",", reuqest.Id_deparment)
                                     : string.Empty;

            CommonFunction employeeService = new CommonFunction();

          


         
            var employeeInfo1 = employeeService.GetUserDetails(reuqest.orgid_userdetails, Id_deparment, string.Join(",", reuqest.rolewise),
        string.Join(",", reuqest.regionwise),
        string.Join(",", reuqest.usercity),
        string.Join(",", reuqest.user_designation), reuqest.startdate, reuqest.enddate,reuqest.Status).Result;
            return Ok(employeeInfo1);
        }

        // trial 
        //public class tbl_learning_user_log
        //{
        //    public int IdLearningUserLog { get; set; }           // id_learning_user_log (Primary Key)
        //    public int CategoryId { get; set; }          // id_learning_category
        //    public int SubCategoryId { get; set; }       // id_learning_sub_category
        //    public int QuestionId { get; set; }          // id_learning_question
        //    public int orgid { get; set; }              // id_organization
        //    public int userid { get; set; }                      // id_user
        //    public int? QuestionAnswerId { get; set; }   // id_learning_question_answer (nullable)
        //    public int? Point { get; set; }                      // point (nullable)
        //    public string IsCorrectAnswer { get; set; }          // is_correct_answer (varchar)
        //    public int? Attempt { get; set; }                    // attempt (nullable)
        //    public string Status { get; set; }
        //    public DateTime CreatedDateTime { get; set; }
        //}

        //[Route("api/Trivia/SubmitUserLog")]
        //[AllowAnonymous]
        //[HttpPost]
        //[HttpOptions]
        //public IHttpActionResult SubmitUserLog(tbl_learning_user_log user_Log)
        //{
        //    var UserAttempt = "yes";

        //    if (UserAttempt == null || !UserAttempt.Any())
        //    {
        //        return Content(HttpStatusCode.NotFound, "No data available");
        //    }
        //    return Ok(UserAttempt);
        //}
    }
}
