using MySql.Data.MySqlClient;
using Mysqlx.Crud;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Utilities.Zlib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Drawing;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Filters;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI.WebControls;
using System.Reflection;
using System.Data.SqlClient;
using MySqlX.XDevAPI.Common;
using Org.BouncyCastle.Tls.Crypto;
using System.Security.Cryptography.X509Certificates;

namespace ReportM2OST.Models
{
    public class CommonFunction
    {
        private readonly MySqlConnection _connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["dbconnectionstring"].ConnectionString);
        private readonly MySqlConnection _connectionpyp = new MySqlConnection(ConfigurationManager.ConnectionStrings["dbconnectionstringpyp"].ConnectionString);
        private readonly MySqlConnection _connectionNgage = new MySqlConnection(ConfigurationManager.ConnectionStrings["db_tgc_gameEntities"].ConnectionString);
        private readonly MySqlConnection _connectionCoroebus = new MySqlConnection(ConfigurationManager.ConnectionStrings["db_tgc_corobus"].ConnectionString);

        public UserRoleInfo GetLoginInfo(string username, string password)
        {
            int idrole = 0;

            string query = @"
            SELECT ID_ROLE
            FROM tbl_cms_users
            WHERE Username = @Username AND Password = @Password";
            using (var connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["dbconnectionstring"].ConnectionString))
            {
                connection.Open();

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Password", password);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            idrole = reader.IsDBNull(reader.GetOrdinal("ID_ROLE")) ? 0 : reader.GetInt32("ID_ROLE");
                        }
                        else
                        {
                            return null;
                        }
                    }
                }

                if (idrole != 0)
                {
                    string query1 = @"
            SELECT *
            FROM tbl_cms_users u
            JOIN tbl_cms_roles r ON u.ID_ROLE = r.ID_ROLE
            WHERE u.Username = @Username AND u.Password = @Password AND u.ID_ROLE = @ID_ROLE";

                    using (var command2 = new MySqlCommand(query1, connection))
                    {
                        command2.Parameters.AddWithValue("@Username", username);
                        command2.Parameters.AddWithValue("@Password", password);
                        command2.Parameters.AddWithValue("@ID_ROLE", idrole);

                        using (var reader = command2.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new UserRoleInfo
                                {
                                    ID_ROLE = reader.IsDBNull(reader.GetOrdinal("ID_ROLE")) ? 0 : reader.GetInt32("ID_ROLE"),
                                    ID_ORGANIZATION = reader.IsDBNull(reader.GetOrdinal("ID_ORGANIZATION")) ? 0 : reader.GetInt32("ID_ORGANIZATION"),
                                    ROLENAME = reader.IsDBNull(reader.GetOrdinal("ROLENAME")) ? null : reader.GetString("ROLENAME"),
                                    DESCRIPTION = reader.IsDBNull(reader.GetOrdinal("DESCRIPTION")) ? null : reader.GetString("DESCRIPTION"),
                                    cmd_user_type = reader.IsDBNull(reader.GetOrdinal("cmd_user_type")) ? 0 : reader.GetInt32("cmd_user_type"),
                                    employee_id = reader.IsDBNull(reader.GetOrdinal("employee_id")) ? null : reader.GetString("employee_id"),
                                    employee_name = reader.IsDBNull(reader.GetOrdinal("employee_name")) ? null : reader.GetString("employee_name")
                                };
                            }
                            else
                            {
                                return null;
                            }
                        }
                    }
                }
                else
                {
                    return null;
                }
            } // The connection will be closed automatically here

        }


        /// List
        public async Task<IEnumerable<dynamic>> GetUserLoginStatus(int orgId, DateTime startDate, DateTime endDate, string rolewise)
        {
            var results = new List<dynamic>();

            using (var connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["dbconnectionstring"].ConnectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand("GetUserLoginStatus", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@orgid", orgId);
                    cmd.Parameters.AddWithValue("@startDate", startDate);
                    cmd.Parameters.AddWithValue("@endDate", endDate);
                    cmd.Parameters.AddWithValue("@rolewise", rolewise);


                    await connection.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var result = new ExpandoObject() as IDictionary<string, object>;
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                result.Add(reader.GetName(i), reader.IsDBNull(i) ? null : reader.GetValue(i));
                            }
                            results.Add(result);
                        }
                    }
                }

                if (results.Count == 0)
                {

                    var noDataMessage = new ExpandoObject() as IDictionary<string, object>;
                    noDataMessage.Add("Message", "No data found for the provided.");

                    results.Add(noDataMessage);
                }
            }


            return results;
        }

        /// DropDown
        //      public List<SelectListItem> tbl_csst_role(int orgid,int id_deprt)
        //        {
        //            List<SelectListItem> list = new List<SelectListItem>();

        //            try
        //            {
        //              _connection.Open(); 

        //              using (MySqlCommand cmd = new MySqlCommand("SP_tbl_csst_role", _connection))
        //              {
        //                  cmd.CommandType = CommandType.StoredProcedure;
        //                  cmd.Parameters.AddWithValue("@orgid", orgid);
        //                 cmd.Parameters.AddWithValue("@id_deprt", id_deprt);


        //                  using (MySqlDataReader reader = cmd.ExecuteReader())
        //                  {
        //                      while (reader.Read())
        //                      {
        //                          SelectListItem orgItem = new SelectListItem
        //                          {
        //                              Value = reader["id_csst_role"].ToString(),
        //                              Text = reader["csst_role"].ToString()
        //                          };

        //                          list.Add(orgItem);
        //                      }
        //                  }
        //              }
        //            }
        //    catch (Exception ex)
        //    {
        //        // Handle the exception (log it, throw it, etc.)
        //    }
        //    finally
        //    {

        //              _connection.Close();

        //    }

        //    return list;
        //}

        public List<object> tbl_csst_role(int orgid, string id_deprt)
        {
            List<object> list = new List<object>();
            var id_deprts = id_deprt.Split(',').Select(p => p.Trim()).ToList();
            var id_deprtParams = string.Join(", ", id_deprts.Select((s, i) => $"@id_deprts{i}"));

            try
            {
                using (var connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["dbconnectionstring"].ConnectionString))
                {
                    connection.Open();
                    // _connection.Open();

                    string query = $@"SELECT id_csst_role, csst_role 
                          FROM tbl_csst_role 
                          WHERE id_organization = @orgId  
                          AND id_csst_role NOT LIKE '%404%'
                          AND id_csst_role NOT LIKE '%405%'
                          AND id_csst_role NOT LIKE '%406%'
                          AND id_csst_role NOT LIKE '%407%'
                          AND id_csst_role NOT LIKE '%408%'
                          AND id_csst_role NOT LIKE '%409%'
                          AND id_csst_role NOT LIKE '%419%'
                          AND id_csst_role NOT LIKE '%420%'
                          AND id_csst_role NOT LIKE '%427%'
                          AND id_csst_role NOT LIKE '%428%'
                          AND id_csst_role NOT LIKE '%485%'
                          AND id_csst_role NOT LIKE '%486%'
                          AND id_csst_role NOT LIKE '%487%'
                          AND id_csst_role NOT LIKE '%484%'
                          AND id_csst_role NOT LIKE '%BATA%'
                          AND Id_department IN ({id_deprtParams})";

                    //AND id_csst_role NOT LIKE '%527%'
                    //AND id_csst_role NOT LIKE '%528%'
                    //AND id_csst_role NOT LIKE '%466%'
                    //AND id_csst_role NOT LIKE '%483%'

                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@orgId", orgid);
                        for (int i = 0; i < id_deprts.Count; i++)
                        {
                            cmd.Parameters.AddWithValue($"@id_deprts{i}", id_deprts[i]);
                        }

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var item = new
                                {
                                    Text = reader["csst_role"].ToString(),
                                    Value = reader["id_csst_role"].ToString()
                                };

                                list.Add(item);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle the exception (log it, throw it, etc.)
                Console.WriteLine("An error occurred: " + ex.Message); // For example purposes
            }
            finally
            {
                //_connection.Close();
            }

            return list;
        }



        /// ALL list of Departemnt 
        public async Task<IEnumerable<dynamic>> GetDepartemntList(int Id_org)
        {
            var results = new List<dynamic>();

            string query = @"SELECT Id_department, Department_name FROM tbl_department WHERE Id_org = @Id_org";


            using (var connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["dbconnectionstring"].ConnectionString))
            {
                // Create a MySqlCommand with the query and connection
                using (var cmd = new MySqlCommand(query, connection))
                {

                    cmd.Parameters.AddWithValue("@Id_org", Id_org);
                    await connection.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var result = new ExpandoObject() as IDictionary<string, object>;

                            // Only add the required columns to the result
                            result.Add("Id_department", reader.IsDBNull(reader.GetOrdinal("Id_department")) ? null : reader.GetValue(reader.GetOrdinal("Id_department")));
                            result.Add("Department_name", reader.IsDBNull(reader.GetOrdinal("Department_name")) ? null : reader.GetValue(reader.GetOrdinal("Department_name")));

                            results.Add(result);
                        }
                    }
                }

                if (results.Count == 0)
                {

                    var noDataMessage = new ExpandoObject() as IDictionary<string, object>;
                    noDataMessage.Add("Message", "No data found for the provided.");

                    results.Add(noDataMessage);
                }
            }


            return results;
        }

        //tbl_Assessment skillmuni
        public List<tbl_assessment> tbl_assessmentlist(int orgid)
        {
            List<tbl_assessment> list = new List<tbl_assessment>();

            try
            {
                //_connection.Open();
                using (var connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["dbconnectionstring"].ConnectionString))
                {
                    connection.Open();
                    string query = @"select * from tbl_assessment where id_organization = @orgid and status ='A' ";

                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@orgid", orgid);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                tbl_assessment orgItem = new tbl_assessment
                                {
                                    Id_assessment = reader["Id_Assessment"].ToString(),
                                    Assessment_title = reader["Assessment_Title"].ToString()
                                };

                                list.Add(orgItem);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle the exception (log it, throw it, etc.)
            }
            finally
            {

                //_connection.Close();

            }

            return list;
        }

        //Login Report
        public async Task<IEnumerable<dynamic>> GetLoginReport(int orgId, string[] Id_department, string[] rolewise, string startDate, string endDate, string[] regionwise, string[] user_designation, string[] usercity)
        {
            var results = new List<dynamic>();

            // Constructing the list of parameters for IN clauses
            string Id_departmentParams = string.Join(", ", Id_department.Select((_, i) => $"@Id_department{i}"));
            string rolewiseParams = string.Join(", ", rolewise.Select((_, i) => $"@rolewise{i}"));
            string regionwiseParams = string.Join(", ", regionwise.Select((_, i) => $"@regionwise{i}"));
            string designationParams = string.Join(", ", user_designation.Select((_, i) => $"@designation{i}"));
            string cityParams = string.Join(", ", usercity.Select((_, i) => $"@city{i}"));

            using (var connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["dbconnectionstring"].ConnectionString))
            {
                await connection.OpenAsync();  // Use async Open to avoid blocking

                string query = $@"
        SELECT
            a.userid AS USERID,
            CONCAT(UPPER(LEFT(c.firstname, 1)), LOWER(SUBSTRING(c.firstname, 2)), ' ', UPPER(LEFT(c.lastname, 1)), LOWER(SUBSTRING(c.lastname, 2))) AS NAME,
            a.status AS STATUS,
            a.user_grade AS Store_code,
            c.Location AS Store_Name,
            a.user_function AS Store_Type,
            a.L4,
            a.L3,
            a.L2,
            a.L1,
            a.Spectator,
            c.OFFICE_ADDRESS AS Region,
            c.CITY AS City,
            c.gender AS Gender,
            d.csst_role AS Role,
            a.user_designation AS Designation,
            c.Date_of_joining,
            a.UPDATEDTIME AS 'Date_Of_Creation',
            IF(ISNULL(MAX(b.log_datetime)), 'NOT logged IN', 'Logged IN') AS Overall_Login_Status,
            IF(DATEDIFF(CURRENT_DATE(), MAX(b.log_datetime)) <= @daycount, 'Logged IN', 'NOT Logged IN') AS 'Login_Activity',
            IFNULL(MAX(b.log_datetime), '/N') AS 'Latest_Login'
        FROM
            tbl_user a
            LEFT JOIN tbl_report_login_log b ON a.id_user = b.id_user
            JOIN tbl_profile c ON a.id_user = c.id_user
            LEFT JOIN tbl_csst_role d ON a.id_role = d.id_csst_role 
            JOIN tbl_department e ON d.id_department = e.id_department
        WHERE
            a.id_organization = @orgid
            AND b.LOG_DATETIME BETWEEN @startDate AND @endDate
            AND e.id_department IN ({Id_departmentParams})
            AND d.id_csst_role IN ({rolewiseParams})
            AND c.city IN ({cityParams})
            AND c.office_address IN ({regionwiseParams})
            AND a.user_designation IN ({designationParams})
            AND a.STATUS = 'A'
            AND a.userid NOT LIKE '%-OLD%'
            AND a.userid NOT LIKE '%GS%'
            AND a.userid NOT LIKE '%IVABATA%'
            AND a.userid NOT LIKE '%Bata%'
            AND a.userid NOT LIKE '_SM'
            AND a.userid NOT LIKE '_DM'
            AND a.userid NOT LIKE '_RM'
        GROUP BY
            a.userid,
            c.firstname,
            c.lastname,
            d.csst_role
        ORDER BY
            a.userid";

                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    // Adding organization id
                    cmd.Parameters.AddWithValue("@orgid", orgId);

                    // Adding date range and daycount parameters
                    cmd.Parameters.AddWithValue("@startDate", startDate);
                    cmd.Parameters.AddWithValue("@endDate", endDate);
                    cmd.Parameters.AddWithValue("@daycount", 0);

                    // Adding the department, role, region, designation, and city parameters
                    for (int i = 0; i < Id_department.Length; i++)
                    {
                        cmd.Parameters.AddWithValue($"@Id_department{i}", Id_department[i]);
                    }

                    for (int i = 0; i < rolewise.Length; i++)
                    {
                        cmd.Parameters.AddWithValue($"@rolewise{i}", rolewise[i]);
                    }

                    for (int i = 0; i < regionwise.Length; i++)
                    {
                        cmd.Parameters.AddWithValue($"@regionwise{i}", regionwise[i]);
                    }

                    for (int i = 0; i < user_designation.Length; i++)
                    {
                        cmd.Parameters.AddWithValue($"@designation{i}", user_designation[i]);
                    }

                    for (int i = 0; i < usercity.Length; i++)
                    {
                        cmd.Parameters.AddWithValue($"@city{i}", usercity[i]);
                    }

                    try
                    {
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var result = new ExpandoObject() as IDictionary<string, object>;
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    result.Add(reader.GetName(i), reader.IsDBNull(i) ? null : reader.GetValue(i));
                                }
                                results.Add(result);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Handle or log any exceptions that occur during database read
                        Console.WriteLine($"Error executing SQL query: {ex.Message}");
                    }
                }

                if (results.Count == 0)
                {
                    var noDataMessage = new ExpandoObject() as IDictionary<string, object>;
                    noDataMessage.Add("Message", "No data found for the provided criteria.");
                    results.Add(noDataMessage);
                }
            }  // connection will automatically be closed here

            return results;
        }


        //Mounthly Report
        public IEnumerable<dynamic> GetMonthlyReport(int orgId, int month, string[] id_assessment, string[] roleId, string[] region, string[] user_designation)
        {
            var results = new List<dynamic>();
            try
            {
                // Check for null or empty id_assessment and roleId
                //var idAssessments = string.IsNullOrEmpty(id_assessment) ? new string[0] : id_assessment.Split(',').Select(id => id.Trim()).ToArray();
                //var roleIds = string.IsNullOrEmpty(roleId) ? new string[0] : roleId.Split(',').Select(id => id.Trim()).ToArray();
                //var regions = string.IsNullOrEmpty(region) ? new string[0] : region.Split(',').Select(id => id.Trim()).ToArray();

                using (var connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["dbconnectionstring"].ConnectionString))
                {
                    connection.Open();
                    //  string query = $@"SELECT 
                    //    a.userid,
                    //    a.id_user,
                    //    b.csst_role,
                    //    e.assessment_title,
                    //   -- g.CATEGORYNAME,
                    //    c.created_date,
                    //    d.firstname,
                    //    d.lastname,
                    //    d.OFFICE_ADDRESS,
                    //    a.L4,
                    //      a.L3,
                    //      a.L2,
                    //      a.L1,
                    //      a.Spectator
                    //FROM 
                    //    tbl_user a
                    //JOIN 
                    //    tbl_csst_role b ON a.id_role = b.id_csst_role
                    //JOIN 
                    //    tbl_user_kpi_data_log c ON a.id_user = c.id_user
                    //JOIN 
                    //    tbl_profile d ON a.id_user = d.id_user 
                    //JOIN 
                    //    tbl_assessment e ON c.Content_Assessment_ID = e.id_assessment
                    //-- JOIN 
                    //   -- tbl_assessment_categoty_mapping f ON e.id_assessment = f.id_assessment
                    //-- JOIN
                    //   --  tbl_category g ON f.id_category = g.id_category
                    //WHERE 
                    //    c.Content_Assessment_ID IN ({string.Join(", ", id_assessment.Select((s, i) => $"@IdAssessment{i}"))})
                    // AND a.id_role IN ({string.Join(", ", roleId.Select((s, i) => $"@roleId{i}"))})
                    //    -- AND f.id_category IN (2623)
                    //   AND MONTH(c.created_date) = @monthvalue
                    //    AND b.id_organization = @orgid
                    //LIMIT 10;
                    //";

                    //test
                    string query = $@"
                            SELECT 
                            a.userid AS USERID,  
                            CONCAT(d.firstname, ' ', d.lastname) AS NAME,
                            a.user_grade AS Store_Code,
                            d.LOCATION AS Store_Name,
                            d.OFFICE_ADDRESS AS Region,
                            MAX(c.created_date) AS Last_Created_Date, 
                            b.csst_role AS Role, 
                            a.user_function AS Store_Type, 
                            SUM(c.points_scored) AS Points,
                            b.csst_role 
                        FROM 
                            tbl_user a
                        JOIN 
                            tbl_csst_role b ON a.id_role = b.id_csst_role
                        JOIN 
                            tbl_user_kpi_data_log c ON a.id_user = c.id_user
                        JOIN 
                            tbl_profile d ON a.id_user = d.id_user
                        WHERE 
                              c.Content_Assessment_ID IN ({string.Join(", ", id_assessment.Select((s, i) => $"@IdAssessment{i}"))})
 
                            and b.id_organization = @orgid 
                           AND a.id_role IN ({string.Join(", ", roleId.Select((s, i) => $"@roleId{i}"))})
                            AND MONTH(c.created_date) = @monthvalue
                            AND d.OFFICE_ADDRESS IN  ({string.Join(", ", region.Select((s, i) => $"@region{i}"))})
                            AND a.user_designation IN ({string.Join(", ", user_designation.Select((s, i) => $"@user_designation{i}"))})
                        GROUP BY 
                            a.userid, a.ID_USER, d.firstname, d.lastname, d.OFFICE_ADDRESS, b.csst_role, a.user_function
                        HAVING 
                            Points > 0
                        ORDER BY 
                            Points DESC
                            limit 10;
                      ";
                    // Build the query with proper syntax
                //    string query = $@"
                //SELECT 
                //    a.userid AS USERID,  
                //    CONCAT(d.firstname, ' ', d.lastname) AS NAME,
                //    a.user_grade AS Store_Code,
                //    d.LOCATION AS Store_Name,
                //    d.OFFICE_ADDRESS AS Region,
                //    MAX(c.created_date) AS Last_Created_Date, 
                //    b.csst_role AS Role, 
                //    a.user_function AS Store_Type, 
                //    SUM(c.points_scored) AS Points
                //FROM 
                //    tbl_user a
                //JOIN 
                //    tbl_csst_role b ON a.id_role = b.id_csst_role
                //JOIN 
                //    tbl_user_kpi_data_log c ON a.id_user = c.id_user
                //JOIN 
                //    tbl_profile d ON a.id_user = d.id_user
                //WHERE 
                //        c.Content_Assessment_ID IN ({string.Join(", ", id_assessment.Select((s, i) => $"@IdAssessment{i}"))})
                //AND (
                //    b.id_organization = @orgid 
                //    AND b.csst_role IN ({string.Join(", ", roleId.Select((s, i) => $"@roleId{i}"))})
                //)
                //AND MONTH(c.created_date) = @monthvalue
                //AND d.OFFICE_ADDRESS IN ({string.Join(", ", region.Select((s, i) => $"@region{i}"))})
                //GROUP BY 
                //    a.userid, a.ID_USER, d.firstname, d.lastname, d.OFFICE_ADDRESS, b.csst_role, a.user_function
                //HAVING 
                //    Points > 0
                //ORDER BY 
                //    Points DESC;";

                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        // Add the fixed parameters
                        cmd.Parameters.AddWithValue("@orgid", orgId);
                        cmd.Parameters.AddWithValue("@monthvalue", month);


                        for (int i = 0; i < region.Length; i++)
                        {
                            cmd.Parameters.AddWithValue($"@region{i}", region[i]);
                        }

                        for (int i = 0; i < id_assessment.Length; i++)
                        {
                            cmd.Parameters.AddWithValue($"@IdAssessment{i}", id_assessment[i]);
                        }

                        for (int i = 0; i < roleId.Length; i++)
                        {
                            cmd.Parameters.AddWithValue($"@roleId{i}", roleId[i]);
                        }

                        for (int i = 0; i < user_designation.Length; i++)
                        {
                            cmd.Parameters.AddWithValue($"@user_designation{i}", user_designation[i]);
                        }

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var result = new ExpandoObject() as IDictionary<string, object>;
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    result.Add(reader.GetName(i), reader.IsDBNull(i) ? null : reader.GetValue(i));
                                }
                                results.Add(result);
                            }
                        }

                        if (results.Count == 0)
                        {
                            var noDataMessage = new ExpandoObject() as IDictionary<string, object>;
                            noDataMessage.Add("Message", "No data found for the provided criteria.");
                            results.Add(noDataMessage);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception (consider using a logging framework)
                Console.WriteLine($"An error occurred: {ex.Message}");
                // Optionally, re-throw or handle the exception as appropriate
            }

            return results;
        }

        //Mounthly Report Region
        public List<object> GetRegionlist(int orgid, string[] roleId)
        {
            List<object> list = new List<object>();

            try
            {
                //_connection.Open();
                using (var connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["dbconnectionstring"].ConnectionString))
                {
                    connection.Open();

                    //string query = @"SELECT * from tbl_region where orgid = " + orgid + " ";
                    //string query = @"SELECT DISTINCT tbl_profile.OFFICE_ADDRESS 
                    //     FROM tbl_profile
                    //     INNER JOIN tbl_user ON tbl_user.ID_USER = tbl_profile.ID_USER
                    //     WHERE tbl_user.ID_ORGANIZATION = @OrgId 
                    //        and status ='A' and USERID not like ""%Bata%""";

                    string query = $@"SELECT DISTINCT
                     p.OFFICE_ADDRESS
                 FROM
                     tbl_profile p
                 INNER JOIN
                     tbl_user u ON u.id_user = p.id_user
                 INNER JOIN
                     tbl_csst_role r ON u.id_role = r.id_csst_role
                 WHERE
                     u.id_organization = @OrgId 
                     AND u.status = 'A'
                     AND r.id_csst_role IN ({string.Join(", ", roleId.Select((s, i) => $"@roleId{i}"))})
                     AND u.USERID NOT LIKE '%Bata%'";

                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@OrgId", orgid);

                        for (int i = 0; i < roleId.Length; i++)
                        {
                            cmd.Parameters.AddWithValue($"@roleId{i}", roleId[i]);
                        }

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var item = new
                                {
                                    //Text = reader["Region_name"].ToString(),
                                    //Value = reader["Region_name"].ToString()
                                    Text = reader["OFFICE_ADDRESS"].ToString(),
                                    Value = reader["OFFICE_ADDRESS"].ToString()
                                };

                                list.Add(item);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle the exception (log it, throw it, etc.)
                Console.WriteLine("An error occurred: " + ex.Message); // For example purposes
            }
            finally
            {
                //_connection.Close();
            }

            return list;
        }

        //Ngage Tbl_orgination Dropdown
        public List<tbl_organization> tbl_organization()
        {
            List<tbl_organization> list = new List<tbl_organization>();

            try
            {
                //  _connectionNgage.Open();
                using (var connectionNgage = new MySqlConnection(ConfigurationManager.ConnectionStrings["db_tgc_gameEntities"].ConnectionString))
                {
                    connectionNgage.Open();
                    string query = @"SELECT * from tbl_organization where ID_ORGANIZATION = 15 AND STATUS ='A'";

                    using (MySqlCommand cmd = new MySqlCommand(query, connectionNgage))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                tbl_organization orgItem = new tbl_organization
                                {
                                    ID_ORGANIZATION = reader["ID_ORGANIZATION"].ToString(),
                                    ORGANIZATION_NAME = reader["ORGANIZATION_NAME"].ToString()
                                };

                                list.Add(orgItem);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle the exception (log it, throw it, etc.)
            }
            finally
            {

               // _connectionNgage.Close();

            }

            return list;
        }

        //Ngage Tbl_assessment Dropdown
        public List<tbl_assessment> tbl_assessment(int orgid_ngage,string year,string month)
        {
            List<tbl_assessment> list = new List<tbl_assessment>();

            try
            {
                // _connectionNgage.Open();

                using (var connectionNgage = new MySqlConnection(ConfigurationManager.ConnectionStrings["db_tgc_gameEntities"].ConnectionString))
                {
                    connectionNgage.Open();
                    string query = @"SELECT distinct a.Assessment_Title ,a.Id_Assessment from tbl_assessment  a
join tbl_assessment_details_user_log b
on a.Id_Assessment = b.Id_Assessment
where a.ID_ORGANIZATION=@orgid AND a.IsActive ='A' AND  year(b.Updated_Date_Time) in (@year) and MONTH(b.Updated_Date_Time) IN(@month)
";
                    //string query = @"SELECT * from tbl_assessment where ID_ORGANIZATION=@orgid AND IsActive ='A' AND  year(Updated_Date_Time) in (@year) and MONTH(Updated_Date_Time) IN(@month)";

                    using (MySqlCommand cmd = new MySqlCommand(query, connectionNgage))
                    {
                        cmd.Parameters.AddWithValue("@orgid", orgid_ngage);
                        cmd.Parameters.AddWithValue("@year", year);
                        cmd.Parameters.AddWithValue("@month", month);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                tbl_assessment orgItem = new tbl_assessment
                                {
                                    Id_assessment = reader["Id_Assessment"].ToString(),
                                    Assessment_title = reader["Assessment_Title"].ToString()
                                };

                                list.Add(orgItem);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle the exception (log it, throw it, etc.)
            }
            finally
            {

               // _connectionNgage.Close();

            }

            return list;
        }


        //Ngage AssessmentReport 
        public async Task<IEnumerable<dynamic>> GetAssessmentReport1(int orgid_ngage, string Id_assessment_ngage)
        {
            var results = new List<dynamic>();

            var idAssessments = Id_assessment_ngage.Split(',').Select(id => id.Trim()).ToArray();
            var idAssessmentParams = string.Join(", ", idAssessments.Select((s, i) => $"@IdAssessment{i}"));

            try
            {
                //if (_connectionNgage.State != System.Data.ConnectionState.Open)
                //{
                //    await _connectionNgage.OpenAsync().ConfigureAwait(false);
                //}

                using (var connectionNgage = new MySqlConnection(ConfigurationManager.ConnectionStrings["db_tgc_gameEntities"].ConnectionString))
                {
                    await connectionNgage.OpenAsync().ConfigureAwait(false);

                    string query = $@"
SELECT
    b.email AS Userid, 
    b.Name,
    a.id_assessment,
    c.Assessment_Title AS 'Assessment_Title', 
    d.Assessment_Question AS 'Assessment_Question',
    e.Answer_Description AS 'Selected_Answer',
    IF(a.is_right = 1, 10, 0) AS Score, 
    IF(a.is_right = 1, 'Right Ans', 'Wrong Ans') AS 'Answer_Status'
FROM 
    tbl_assessment_details_user_log a 
JOIN 
    tbl_users b ON a.id_user = b.id_user
JOIN 
    tbl_assessment c ON a.id_game = c.id_game AND a.id_assessment = c.id_assessment
JOIN 
    tbl_assessment_question d ON a.id_question = d.Id_Assessment_question
JOIN 
    tbl_assessment_question_answers e ON e.Id_Assessment_question_ans = a.Id_Assessment_question_ans
JOIN (
    SELECT
        b.email,
        a.id_question,
        MIN(a.id_log) AS min_id_log
    FROM 
        tbl_assessment_details_user_log a 
    JOIN 
        tbl_users b ON a.id_user = b.id_user
    WHERE 
        a.id_organization = @OrgIdNgage
        AND b.email NOT LIKE '%BATA%'
        AND b.email  NOT LIKE '%-OLD%'
            AND b.email  NOT LIKE '%GS%'
            AND b.email  NOT LIKE '%IVABATA%'
            AND b.email  NOT LIKE '%Bata%'
            AND b.email  NOT LIKE '_SM'
            AND b.email NOT LIKE '_DM'
            AND b.email  NOT LIKE '_RM'
        AND a.id_assessment IN ({idAssessmentParams})
    GROUP BY
        b.email, a.id_question
) sub ON a.id_log = sub.min_id_log
WHERE 
    a.id_organization = @OrgIdNgage
    AND b.email NOT LIKE '%BATA%'
    AND a.id_assessment IN ({idAssessmentParams})
";

                    using (var cmd = new MySqlCommand(query, connectionNgage))
                    {
                        cmd.Parameters.AddWithValue("@OrgIdNgage", orgid_ngage);

                        for (int i = 0; i < idAssessments.Length; i++)
                        {
                            cmd.Parameters.AddWithValue($"@IdAssessment{i}", idAssessments[i]);
                        }

                        using (var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                        {
                            while (await reader.ReadAsync().ConfigureAwait(false))
                            {
                                var result = new ExpandoObject() as IDictionary<string, object>;
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    result.Add(reader.GetName(i), reader.IsDBNull(i) ? null : reader.GetValue(i));
                                }
                                results.Add(result);
                            }
                        }
                    }

                    if (results.Count == 0)
                    {
                        var noDataMessage = new ExpandoObject() as IDictionary<string, object>;
                        noDataMessage.Add("Message", "No data found for the provided.");
                        results.Add(noDataMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exception (log it, rethrow it, or return a custom error message)
                throw new Exception("An error occurred while fetching the assessment report.", ex);
            }
            finally
            {
                //if (_connectionNgage.State == System.Data.ConnectionState.Open)
                //{
                //    await _connectionNgage.CloseAsync().ConfigureAwait(false);
                //}
            }

            return results;
        }


        //test Ngage  AssessmentReport userwiser
        public async Task<IEnumerable<dynamic>> GetAssessmentReport(int orgid_ngage,string[] Id_assessment_ngage, int orgid_userdetails ,string[] Id_deparment,string[] rolewise, string[] regionwise, string[] usercity,string startdate_ngage,string enddate_ngage, string[] user_designation)
        {
            DataTable results = new DataTable();
            DataTable results1 = new DataTable();
            DataTable results3 = new DataTable();
            var error = new List<dynamic>();
             var combinedResults = new List<dynamic>();

            //var idAssessments = Id_assessment_ngage.Split(',').Select(id => id.Trim()).ToArray();
            //var idAssessmentParams = string.Join(", ", idAssessments.Select((s, i) => $"@IdAssessment{i}"));

            var idAssessmentParams = string.Join(", ", Id_assessment_ngage.Select((id, index) => $"@IdAssessment{index}"));


            try
            {
                //if (_connectionNgage.State != System.Data.ConnectionState.Open)
                //{
                //    await _connectionNgage.OpenAsync().ConfigureAwait(false);
                //}

                using (var connectionNgage = new MySqlConnection(ConfigurationManager.ConnectionStrings["db_tgc_gameEntities"].ConnectionString))
                {
                    await connectionNgage.OpenAsync().ConfigureAwait(false);

                    string query = @"
            SELECT
                b.email AS UserId, 
                b.Name,
                a.id_assessment,
                c.Assessment_Title AS Assessment_Title, 
                d.Assessment_Question AS Assessment_Question,
                e.Answer_Description AS Selected_Answer,
                IF(a.is_right = 1, 10, 0) AS Score, 
                IF(a.is_right = 1, 'Right Ans', 'Wrong Ans') AS Answer_Status
            FROM 
                tbl_assessment_details_user_log a 
            JOIN 
                tbl_users b ON a.id_user = b.id_user
            JOIN 
                tbl_assessment c ON a.id_game = c.id_game AND a.id_assessment = c.id_assessment
            JOIN 
                tbl_assessment_question d ON a.id_question = d.Id_Assessment_question
            JOIN 
                tbl_assessment_question_answers e ON e.Id_Assessment_question_ans = a.Id_Assessment_question_ans
            JOIN (
                SELECT
                    b.email,
                    a.id_question,
                    MIN(a.id_log) AS min_id_log
                FROM 
                    tbl_assessment_details_user_log a 
                JOIN 
                    tbl_users b ON a.id_user = b.id_user
                WHERE 
                    a.id_organization = @OrgIdNgage
                    AND b.email NOT LIKE '%BATA%'
                    AND b.email NOT LIKE '%-OLD%'
                    AND b.email NOT LIKE '%GS%'
                    AND b.email NOT LIKE '%IVABATA%'
                    AND b.email NOT LIKE '%Bata%'
                    AND b.email NOT LIKE '_SM'
                    AND b.email NOT LIKE '_DM'
                    AND b.email NOT LIKE '_RM'
                    AND a.updated_date_time BETWEEN @startdate_ngage AND @enddate_ngage
                    AND a.id_assessment IN (" + idAssessmentParams + @")
                GROUP BY
                    b.email, a.id_question
            ) sub ON a.id_log = sub.min_id_log
            WHERE 
                a.id_organization = @OrgIdNgage
                AND b.email NOT LIKE '%BATA%'
                AND a.id_assessment IN (" + idAssessmentParams + @")";

                    using (var cmd = new MySqlCommand(query, connectionNgage))
                    {
                        cmd.Parameters.AddWithValue("@OrgIdNgage", orgid_ngage);
                        cmd.Parameters.AddWithValue("@startdate_ngage", startdate_ngage);
                        cmd.Parameters.AddWithValue("@enddate_ngage", enddate_ngage);

                        for (int i = 0; i < Id_assessment_ngage.Length; i++)
                        {
                            cmd.Parameters.AddWithValue($"@IdAssessment{i}", Id_assessment_ngage[i]);
                        }

                        using (var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                        {
                            results.Load(reader);



                            //if (results.Rows.Count == 0)
                            //{
                            //    var noDataMessage = new ExpandoObject() as IDictionary<string, object>;
                            //    noDataMessage.Add("Message", "No data found for the provided.");
                            //    error.Add(noDataMessage);
                            //}
                        }
                    }
                }

                if (results.Rows.Count == 0)
                {
                    var noDataMessage = new ExpandoObject() as IDictionary<string, object>;
                    noDataMessage.Add("Message", "No data found for the provided.");
                    error.Add(noDataMessage);
                    return error;
                }
                else
                {
                    var userDetails = await UserDetails1(orgid_userdetails, Id_deparment, rolewise, regionwise, usercity, user_designation).ConfigureAwait(false);
                    results1 = userDetails;


                    //// Combine results where UserId matches in both results and results1
                    //var combinedResults = new List<dynamic>();

                    //foreach (DataRow row in results.Rows)
                    //{
                    //    var userId = row["UserId"].ToString();
                    //    var matchedRows = results1.Select($"UserId = '{userId}'");

                    //    foreach (DataRow matchedRow in matchedRows)
                    //    {
                    //        var combinedResult = new ExpandoObject() as IDictionary<string, object>;
                    //        foreach (DataColumn column in results.Columns)
                    //        {
                    //            combinedResult.Add(column.ColumnName, row[column.ColumnName]);
                    //        }
                    //        foreach (DataColumn column in results1.Columns)
                    //        {
                    //            combinedResult.Add(column.ColumnName, matchedRow[column.ColumnName]);
                    //        }
                    //        combinedResults.Add(combinedResult);
                    //    }
                    //}

                    //return combinedResults;
                    var userDetailsDict = results1.AsEnumerable()
                                           .GroupBy(row => row["userid"].ToString())
                                           .ToDictionary(group => group.Key, group => group.First());


                   

                    foreach (DataRow row in results.Rows)
                    {
                        var userId = row["UserId"].ToString();
                        if (userDetailsDict.TryGetValue(userId, out DataRow matchedRow))
                        {
                            var combinedResult = new ExpandoObject() as IDictionary<string, object>;
                            foreach (DataColumn column in results.Columns)
                            {
                                combinedResult.Add(column.ColumnName, row[column.ColumnName]);
                            }
                            foreach (DataColumn column in results1.Columns)
                            {
                                combinedResult.Add(column.ColumnName, matchedRow[column.ColumnName]);
                            }
                            combinedResults.Add(combinedResult);
                        }
                    }

                    //return combinedResults;
                    //var json = JsonConvert.SerializeObject(combinedResults);
                    //var buffer = Encoding.UTF8.GetBytes(json);

                    if (combinedResults.Count == 0)
                    {
                        var noDataMessage = new ExpandoObject() as IDictionary<string, object>;
                        noDataMessage.Add("Message", "No data found for the provided.");
                        error.Add(noDataMessage);
                        return error;
                    }

                    return combinedResults;


                    //var response = new HttpResponseMessage(HttpStatusCode.OK)
                    //{
                    //    Content = new StringContent(json, Encoding.UTF8, "application/json")
                    //};

                    //return response;
                    // return combinedResults;


                }
             
            }
            catch (Exception ex)
            {
               
                var noDataMessage = new ExpandoObject() as IDictionary<string, object>;
                noDataMessage.Add("Message", $"Error in GetAssessmentReport: {ex.Message}");
                error.Add(noDataMessage);
                return error;
            }
            finally
            {
                if (_connectionNgage.State == System.Data.ConnectionState.Open)
                {
                    await _connectionNgage.CloseAsync().ConfigureAwait(false);
                }
            }
        }


        //Ngage AssessmentReport question
        public async Task<IEnumerable<dynamic>> AssessmentReportQuestion1(int orgIdNgage, string idAssessmentNgage , string startdate)
        {
            var results = new List<dynamic>();

            // Split idAssessmentNgage into individual IDs if it's a comma-separated string
            var idAssessments = idAssessmentNgage.Split(',').Select(id => id.Trim()).ToArray();
            var idAssessmentParams = string.Join(", ", idAssessments.Select((s, i) => $"@IdAssessment{i}"));

            try
            {
                //if (_connectionNgage.State != System.Data.ConnectionState.Open)
                //{
                //    await _connectionNgage.OpenAsync().ConfigureAwait(false);
                //}
                using (var connectionNgage = new MySqlConnection(ConfigurationManager.ConnectionStrings["db_tgc_gameEntities"].ConnectionString))
                {
                    await connectionNgage.OpenAsync().ConfigureAwait(false);

                    string query = $@"
        SELECT 
            id_assessment,
            Assessment_Title AS 'Assessment_Title',
            id_question,
            attempt_no,
            Assessment_Question AS 'Assessment_Question',
            COUNT(CASE WHEN Answer_Status = 'Right Ans' THEN 1 END) AS Right_Ans,
            COUNT(CASE WHEN Answer_Status = 'Wrong Ans' THEN 1 END) AS Wrong_Ans,
            COUNT(Answer_Status) AS Total_Ans
        FROM 
            (SELECT 
                b.email,
                a.id_game, 
                a.id_assessment,
                c.Assessment_Title,
                a.id_question,
                d.Assessment_Question,
                a.attempt_no,
                e.Answer_Description AS Selected_Answer,
                IF(a.is_right = 1, 'Right Ans', 'Wrong Ans') AS Answer_Status
            FROM 
                tbl_assessment_details_user_log a 
            JOIN 
                tbl_users b ON a.id_user = b.id_user
            JOIN 
                tbl_assessment c ON a.id_game = c.id_game AND a.id_assessment = c.id_assessment
            INNER JOIN 
                tbl_assessment_question d ON a.id_question = d.Id_Assessment_question
            INNER JOIN 
                tbl_assessment_question_answers e ON e.Id_Assessment_question_ans = a.Id_Assessment_question_ans
            join 
                tbl_assessment_details_user_log a ON b.id_user = a.id_user
            WHERE 
                a.id_organization = @OrgIdNgage
                AND b.email NOT LIKE '%BATA%'
               AND b.email  NOT LIKE '%-OLD%'
            AND b.email  NOT LIKE '%GS%'
            AND b.email  NOT LIKE '%IVABATA%'
            AND b.email  NOT LIKE '%Bata%'
            AND b.email  NOT LIKE '_SM'
            AND b.email NOT LIKE '_DM'
            AND b.email  NOT LIKE '_RM'
            AND b.id_user
                AND a.id_assessment IN ({idAssessmentParams})
            ) AS tbl101 
        GROUP BY 
            id_assessment,
            id_question,
            attempt_no
        ORDER BY 
            id_assessment,
            id_question,
            attempt_no;
        ";

                    using (var cmd = new MySqlCommand(query, connectionNgage))
                    {
                        cmd.Parameters.AddWithValue("@OrgIdNgage", orgIdNgage);
                        for (int i = 0; i < idAssessments.Length; i++)
                        {
                            cmd.Parameters.AddWithValue($"@IdAssessment{i}", idAssessments[i]);
                        }

                        using (var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                        {
                            while (await reader.ReadAsync().ConfigureAwait(false))
                            {
                                var result = new ExpandoObject() as IDictionary<string, object>;
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    result.Add(reader.GetName(i), reader.IsDBNull(i) ? null : reader.GetValue(i));
                                }
                                results.Add(result);
                            }
                        }
                    }

                    if (results.Count == 0)
                    {
                        var noDataMessage = new ExpandoObject() as IDictionary<string, object>;
                        noDataMessage.Add("Message", "No data found for the provided assessment.");
                        results.Add(noDataMessage);
                    }
                }
            }
            catch (FormatException ex)
            {
                // Handle format exception
                throw new Exception("Invalid input format.", ex);
            }
            catch (Exception ex)
            {
                // Handle general exceptions
                throw new Exception("An error occurred while fetching the assessment report.", ex);
            }
            finally
            {
                //if (_connectionNgage.State == System.Data.ConnectionState.Open)
                //{
                //    await _connectionNgage.CloseAsync().ConfigureAwait(false);
                //}
            }

            return results;
        }

        //test Ngage AssessmentReport question 
        public async Task<IEnumerable<dynamic>> AssessmentReportQuestion(int orgIdNgage, string[] idAssessmentNgage, int orgid_userdetails, string[] Id_deparment, string[] rolewise,string[] regionwise,string[] usercity,string startdate_ngage, string enddate_ngage, string[] user_designation)
        {
            DataTable results = new DataTable();
            DataTable results1 = new DataTable();
            DataTable results3 = new DataTable();
            var error = new List<dynamic>();

            // Split idAssessmentNgage into individual IDs if it's a comma-separated string
            //var idAssessments = idAssessmentNgage.Split(',').Select(id => id.Trim()).ToArray();
            //var idAssessmentParams = string.Join(", ", idAssessments.Select((s, i) => $"@IdAssessment{i}"));


            var idAssessmentParams = string.Join(", ", idAssessmentNgage.Select((id, index) => $"@IdAssessment{index}"));


            try
            {
                //if (_connectionNgage.State != System.Data.ConnectionState.Open)
                //{
                //    await _connectionNgage.OpenAsync().ConfigureAwait(false);
                //}
                using (var connectionNgage = new MySqlConnection(ConfigurationManager.ConnectionStrings["db_tgc_gameEntities"].ConnectionString))
                {
                    await connectionNgage.OpenAsync().ConfigureAwait(false);

                    string query = $@"
SELECT 
    email as UserId,
    id_assessment,
    Assessment_Title AS ""Assessment_Title"",
    id_question,
    Assessment_Question AS ""Assessment_Question"",
    COUNT(CASE WHEN Answer_Status = 'Right Ans' THEN 1 END) AS Right_Ans,
    COUNT(CASE WHEN Answer_Status = 'Wrong Ans' THEN 1 END) AS Wrong_Ans,
    COUNT(Answer_Status) AS Total_Ans
FROM 
    (SELECT 
        b.email,
        a.id_game, 
        a.id_assessment,
        c.Assessment_Title,
        a.id_question,
        d.Assessment_Question,
        a.updated_date_time,
        e.Answer_Description AS Selected_Answer,
        IF(a.is_right = 1, 'Right Ans', 'Wrong Ans') AS Answer_Status
    FROM 
        tbl_assessment_details_user_log a 
    JOIN 
        tbl_users b ON a.id_user = b.id_user
    JOIN 
        tbl_assessment c ON a.id_game = c.id_game AND a.id_assessment = c.id_assessment
    INNER JOIN 
        tbl_assessment_question d ON a.id_question = d.Id_Assessment_question
    INNER JOIN 
        tbl_assessment_question_answers e ON e.Id_Assessment_question_ans = a.Id_Assessment_question_ans
    WHERE 
        a.id_organization = @OrgIdNgage
        AND b.email NOT LIKE '%BATA%'
         AND a.id_assessment IN ({idAssessmentParams})
        -- AND a.attempt_no <= 3
       AND DATE(a.updated_date_time) BETWEEN @startdate_ngage AND @enddate_ngage
    ) AS tbl101 
GROUP BY 
    id_assessment,
    id_question
ORDER BY 
    id_assessment,
    id_question;
";


                    //        string query = $@"
                    //    SELECT 
                    //    UserId,
                    //    id_assessment,
                    //    Assessment_Title AS 'Assessment_Title',
                    //    id_question,
                    //    attempt_no,
                    //    Assessment_Question AS 'Assessment_Question',
                    //    COUNT(CASE WHEN Answer_Status = 'Right Ans' THEN 1 END) AS Right_Ans,
                    //    COUNT(CASE WHEN Answer_Status = 'Wrong Ans' THEN 1 END) AS Wrong_Ans,
                    //    COUNT(Answer_Status) AS Total_Ans
                    //FROM 
                    //    (SELECT 
                    //         b.email as UserId,
                    //        a.id_game, 
                    //        a.id_assessment,
                    //        c.Assessment_Title,
                    //        a.id_question,
                    //        d.Assessment_Question,
                    //        a.attempt_no,
                    //        e.Answer_Description AS Selected_Answer,
                    //        IF(a.is_right = 1, 'Right Ans', 'Wrong Ans') AS Answer_Status
                    //    FROM 
                    //        tbl_assessment_details_user_log a 
                    //    JOIN 
                    //        tbl_users b ON a.id_user = b.id_user
                    //    JOIN 
                    //        tbl_assessment c ON a.id_game = c.id_game AND a.id_assessment = c.id_assessment
                    //    INNER JOIN 
                    //        tbl_assessment_question d ON a.id_question = d.Id_Assessment_question
                    //    INNER JOIN 
                    //        tbl_assessment_question_answers e ON e.Id_Assessment_question_ans = a.Id_Assessment_question_ans
                    //     join 
                    //        tbl_assessment_details_user_log a ON b.id_user = a.id_user
                    //    WHERE 
                    //        a.id_organization = @OrgIdNgage
                    //        AND b.email NOT LIKE '%BATA%'
                    //        AND b.email  NOT LIKE '%-OLD%'
                    //        AND b.email  NOT LIKE '%GS%'
                    //        AND b.email  NOT LIKE '%IVABATA%'
                    //        AND b.email  NOT LIKE '%Bata%'
                    //        AND b.email  NOT LIKE '_SM'
                    //        AND b.email NOT LIKE '_DM'
                    //        AND b.email  NOT LIKE '_RM'
                    //        AND a.id_assessment IN ({idAssessmentParams})
                    //        AND a.updated_date_time BETWEEN @startdate_ngage AND @enddate_ngage
                    //    ) AS tbl101 
                    //GROUP BY 
                    //    id_assessment,
                    //    id_question,
                    //    attempt_no
                    //ORDER BY 
                    //    id_assessment,
                    //    id_question,
                    //    attempt_no;
                    //";

                    using (var cmd = new MySqlCommand(query, connectionNgage))
                    {
                        cmd.Parameters.AddWithValue("@OrgIdNgage", orgIdNgage);
                        cmd.Parameters.AddWithValue("@startdate_ngage", startdate_ngage);
                        cmd.Parameters.AddWithValue("@enddate_ngage", enddate_ngage);

                        for (int i = 0; i < idAssessmentNgage.Length; i++)
                        {
                            cmd.Parameters.AddWithValue($"@IdAssessment{i}", idAssessmentNgage[i]);
                        }

                        using (var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                        {
                            results.Load(reader);

                            //if (results.Rows.Count == 0)
                            //{
                            //    var noDataMessage = new ExpandoObject() as IDictionary<string, object>;
                            //    noDataMessage.Add("Message", "No data found for the provided.");
                            //    results.Rows.Add(noDataMessage);
                            //}
                        }
                    }
                }

                if (results.Rows.Count == 0)
                {
                    var noDataMessage = new ExpandoObject() as IDictionary<string, object>;
                    noDataMessage.Add("Message", "No data found for the provided.");
                    error.Add(noDataMessage);
                    return error;
                }
                else
                {
                    var userDetails = await UserDetailsQ(orgid_userdetails, Id_deparment, rolewise, regionwise, usercity, user_designation).ConfigureAwait(false);
                    results1 = userDetails;

                    // Combine results where UserId matches in both results and results1
                    var combinedResults = new List<dynamic>();

                    var userDetailsDict = results1.AsEnumerable()
                                         .GroupBy(row => row["userid"].ToString())
                                         .ToDictionary(group => group.Key, group => group.First());




                    foreach (DataRow row in results.Rows)
                    {
                        var userId = row["UserId"].ToString();
                        if (userDetailsDict.TryGetValue(userId, out DataRow matchedRow))
                        {
                            var combinedResult = new ExpandoObject() as IDictionary<string, object>;
                            foreach (DataColumn column in results.Columns)
                            {
                                combinedResult.Add(column.ColumnName, row[column.ColumnName]);
                            }
                            foreach (DataColumn column in results1.Columns)
                            {
                                combinedResult.Add(column.ColumnName, matchedRow[column.ColumnName]);
                            }
                            combinedResults.Add(combinedResult);
                        }
                    }

                    //foreach (DataRow row in results.Rows)
                    //{
                    //    var userId = row["UserId"].ToString();
                    //    var matchedRows = results1.Select($"userid = '{userId}'");

                    //    foreach (DataRow matchedRow in matchedRows)
                    //    {
                    //        var combinedResult = new ExpandoObject() as IDictionary<string, object>;
                    //        foreach (DataColumn column in results.Columns)
                    //        {
                    //            combinedResult.Add(column.ColumnName, row[column.ColumnName]);
                    //        }
                    //        foreach (DataColumn column in results1.Columns)
                    //        {
                    //            combinedResult.Add(column.ColumnName, matchedRow[column.ColumnName]);
                    //        }
                    //        combinedResults.Add(combinedResult);
                    //    }
                    //}

                    if (combinedResults.Count == 0)
                    {
                        var noDataMessage = new ExpandoObject() as IDictionary<string, object>;
                        noDataMessage.Add("Message", "No data found for the provided.");
                        error.Add(noDataMessage);
                        return error;
                    }

                        return combinedResults;


                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
                Console.WriteLine($"Error in GetAssessmentReport: {ex.Message}");
                throw;
            }
         
            finally
            {
                if (_connectionNgage.State == System.Data.ConnectionState.Open)
                {
                    await _connectionNgage.CloseAsync().ConfigureAwait(false);
                }
           
            }

         
        }


        //copy optimize freelance
        public async Task<IEnumerable<dynamic>> AssessmentReportQuestion1(int orgIdNgage, string[] idAssessmentNgage, int orgid_userdetails, string[] Id_deparment, string[] rolewise, string[] regionwise, string[] usercity, string startdate_ngage, string enddate_ngage, string[] user_designation)
        {
            DataTable results = new DataTable();
            DataTable results1 = new DataTable();
            var combinedResults = new List<dynamic>();
            var error = new List<dynamic>();

            // Join the idAssessmentNgage array into SQL placeholders
            var idAssessmentParams = string.Join(", ", idAssessmentNgage.Select((id, index) => $"@IdAssessment{index}"));

            try
            {
                // Get user details
                var userDetails = await UserDetailsAQ(orgid_userdetails, Id_deparment, rolewise, regionwise, usercity, user_designation).ConfigureAwait(false);
                results1 = userDetails;

                // Ensure the connection is open
                //if (_connectionNgage.State != System.Data.ConnectionState.Open)
                //{
                //    await _connectionNgage.OpenAsync().ConfigureAwait(false);
                //}

                using (var connectionNgage = new MySqlConnection(ConfigurationManager.ConnectionStrings["db_tgc_gameEntities"].ConnectionString))
                {
                    await connectionNgage.OpenAsync().ConfigureAwait(false);

                    // Extract email values from the DataTable
                    var emails = results1.AsEnumerable().Select(row => row.Field<string>("USERID")).ToArray();

                // Build the query to get user Ids based on emails
                string[] parameters = emails.Select((s, i) => $"@email{i}").ToArray();
                string query1 = $@"
                                    SELECT Id_User 
                                    FROM tbl_users 
                                    WHERE email IN ({string.Join(",", parameters)})
                                ";

                    using (var cmd1 = new MySqlCommand(query1, connectionNgage))
                    {
                        // Add email parameters
                        for (int i = 0; i < emails.Length; i++)
                        {
                            cmd1.Parameters.AddWithValue($"@email{i}", emails[i]);
                        }

                        // Execute the query and load the result
                        using (var reader = await cmd1.ExecuteReaderAsync().ConfigureAwait(false))
                        {
                            results.Load(reader);
                        }
                    }
                }

                // Reopen the connection if closed
                //if (_connectionNgage.State != System.Data.ConnectionState.Open)
                //{
                //    await _connectionNgage.OpenAsync().ConfigureAwait(false);
                //}

                using (var connectionNgage = new MySqlConnection(ConfigurationManager.ConnectionStrings["db_tgc_gameEntities"].ConnectionString))
                {
                    await connectionNgage.OpenAsync().ConfigureAwait(false);

                    // Prepare query for assessment report
                    string query2 = $@"
                SELECT
                    a.Assessment_Title AS assessment_title,
                    q.Assessment_Question AS assessment_question,
                    COUNT(CASE WHEN ar.any_right_attempt = 1 THEN 1 END) AS right_ans,
                    COUNT(CASE WHEN ar.any_right_attempt = 0 THEN 1 END) AS wrong_ans,
                    COUNT(DISTINCT ar.id_user) AS total_ans
                    FROM
                    (
                    SELECT
                    Id_Assessment,
                    id_question,
                    id_user,
                    MAX(CASE WHEN is_right = 1 THEN 1 ELSE 0 END) AS any_right_attempt
                    FROM
                    tbl_assessment_details_user_log
                    WHERE
                    ID_ORGANIZATION = @OrgIdNgage
                    AND Id_Assessment IN ({idAssessmentParams})
                    AND updated_date_time BETWEEN @startdate_ngage AND @enddate_ngage
                    AND id_user IN ({string.Join(",", results.AsEnumerable().Select(row => row.Field<int>("Id_User")).ToArray())})
                    GROUP BY
                    Id_Assessment,
                    id_question,
                    id_user

                    ) AS ar
                    JOIN tbl_assessment a ON ar.Id_Assessment = a.Id_Assessment
                    JOIN tbl_assessment_question q ON ar.id_question = q.Id_Assessment_question
                    WHERE q.isactive = 'A'
                    GROUP BY
                    a.Assessment_Title,
                    q.Assessment_Question
                    ";

                    using (var cmd2 = new MySqlCommand(query2, connectionNgage))
                    {
                        // Add parameters
                        cmd2.Parameters.AddWithValue("@OrgIdNgage", orgIdNgage);
                        cmd2.Parameters.AddWithValue("@startdate_ngage", startdate_ngage);
                        cmd2.Parameters.AddWithValue("@enddate_ngage", enddate_ngage);

                        for (int i = 0; i < idAssessmentNgage.Length; i++)
                        {
                            cmd2.Parameters.AddWithValue($"@IdAssessment{i}", idAssessmentNgage[i]);
                        }

                        // Execute the query and load results
                        using (var reader = await cmd2.ExecuteReaderAsync().ConfigureAwait(false))
                        {
                            while (reader.Read())
                            {
                                dynamic result = new ExpandoObject();
                                result.Assessment_Title = reader["Assessment_Title"];
                                result.Assessment_Question = reader["Assessment_Question"];
                                result.Right_Ans = reader["Right_Ans"];
                                result.Wrong_Ans = reader["Wrong_Ans"];
                                result.Total_Ans = reader["Total_Ans"];
                                combinedResults.Add(result);
                            }
                        }
                    }
                }

                // Return error if no data found
                if (combinedResults.Count == 0)
                {
                    var noDataMessage = new ExpandoObject() as IDictionary<string, object>;
                    noDataMessage.Add("Message", "No data found for the provided parameters.");
                    error.Add(noDataMessage);
                    return error;
                }

                return combinedResults;
            }
            catch (Exception ex)
            {
                // Handle exceptions
                Console.WriteLine($"Error in AssessmentReportQuestion1: {ex.Message}");
                throw;
            }
            finally
            {
                // Ensure the connection is closed
                if (_connectionNgage.State == System.Data.ConnectionState.Open)
                {
                    await _connectionNgage.CloseAsync().ConfigureAwait(false);
                }
            }
        }


        /// List
        public async Task<IEnumerable<dynamic>> GetUserReportList(int orgId, string Id_deparment, string role_id,string region_name)
        {
            var results = new List<dynamic>();

            var roleIds = string.IsNullOrEmpty(role_id) ? new string[0] : role_id.Split(',').Select(id => id.Trim()).ToArray();
            var Id_deparments = string.IsNullOrEmpty(Id_deparment) ? new string[0] : Id_deparment.Split(',').Select(id => id.Trim()).ToArray();
            var region_names = string.IsNullOrEmpty(region_name) ? new string[0] : region_name.Split(',').Select(id => id.Trim()).ToArray();


            try
            {
                // Ensure proper connection management
                using (var connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["dbconnectionstring"].ConnectionString))
                {
                    await connection.OpenAsync().ConfigureAwait(false);

                    string query = $@"
                            SELECT 
                            a.userid,
                            CONCAT(g.firstname, ' ', g.lastname) AS NAME,
                            g.date_of_joining,
                            a.user_grade AS Store_code,
                            a.L4,
                            a.L3,
                            a.L2,
                            a.L1,
                            a.Spectator,
                            h.Location AS Store_Name,
                            a.UPDATEDTIME,
                            a.user_department,
                            a.user_designation,
                            a.user_function,
                            g.gender,
                            g.city,
                            g.office_address,
                            o.Department_name,
                            a.status
                            
                        FROM 
                            tbl_user a
                        LEFT JOIN tbl_user b ON a.reporting_manager = b.id_user
                        LEFT JOIN tbl_csst_role c ON a.id_role = c.id_csst_role
                        LEFT JOIN tbl_profile g ON a.id_user = g.id_user
                        LEFT JOIN tbl_user d ON b.reporting_manager = d.id_user
                        LEFT JOIN tbl_profile h ON b.id_user = h.id_user
                        LEFT JOIN tbl_user e ON d.reporting_manager = e.id_user
                        LEFT JOIN tbl_profile i ON d.id_user = i.id_user
                        LEFT JOIN tbl_user f ON e.reporting_manager = f.id_user
                        LEFT JOIN tbl_profile j ON e.id_user = j.id_user
                        LEFT JOIN tbl_profile k ON f.id_user = k.id_user
                        JOIN tbl_department o ON c.id_department = o.id_department
                        WHERE 
                            a.id_organization = @orgId
                            AND g.OFFICE_ADDRESS IN ({string.Join(", ", region_names.Select((s, i) => $"@region_name{i}"))})
                            AND o.id_department IN ({string.Join(", ", Id_deparments.Select((s, i) => $"@Id_deparment{i}"))})
                            AND c.id_csst_role IN ({string.Join(", ", roleIds.Select((s, i) => $"@roleId{i}"))})
                            AND a.userid NOT LIKE '%-OLD%'
                                    AND a.userid NOT LIKE '%GS%'
                                    AND a.userid NOT LIKE '%IVABATA%'
                                    AND a.userid NOT LIKE '%Bata%'
                                    AND a.userid NOT LIKE '_SM'
                                    AND a.userid NOT LIKE '_DM'
                                    AND a.userid NOT LIKE '_RM'
           
                    ";

                    //string query = $@"
                    //    SELECT 
                    //        -- a.ID_USER,
                    //        a.userid,
                    //          a.user_grade AS Store_code,
                    //         h.Location AS Store_Name,
                    //     CONCAT(g.firstname, "" "", g.lastname) AS myName,
   
                    //         a.UPDATEDTIME,
                    //         g.date_of_joining,
                    //        a.user_department,
                    //          a.user_designation,
     
                    //            a.user_function,
                    //             g.gender,
  
                    //           g.city,
                    //        g.office_address,
    
                    //        -- c.csst_role,
                    //       -- b.ID_USER AS rm1iduser,
                    //        -- b.userid AS rm1UserID,
                    //        -- b.user_designation AS rm1UserDesignation,
                    //        -- CONCAT(h.firstname, "" "", h.lastname) AS rm1Name,
                    //           -- b.reporting_manager AS rm2_userid,
                    //        -- d.userid AS rm2UserID,
                    //        -- d.user_designation AS rm2UserDesignation,
                    //        -- CONCAT(i.firstname, "" "", i.lastname) AS rm2Name,
                    //        -- e.ID_USER AS RM3iduser,
                    //        -- e.userid AS RM3UserID,
                    //        -- e.user_designation AS RM3UserDesignation,
                    //        -- CONCAT(j.firstname, "" "", j.lastname) AS RM3Name,
                    //        -- f.userid AS rm4UserID,
                    //        -- f.user_designation AS rm4UserDesignation,
                    //        -- CONCAT(k.firstname, "" "", k.lastname) AS rm4Name,
                    //        a.status
                    //    FROM 
                    //        tbl_user a
                    //    LEFT JOIN tbl_user b ON a.reporting_manager = b.id_user
                    //    LEFT JOIN tbl_csst_role c ON a.id_role = c.id_csst_role
                    //    LEFT JOIN tbl_profile g ON a.id_user = g.id_user
                    //    LEFT JOIN tbl_user d ON b.reporting_manager = d.id_user
                    //    LEFT JOIN tbl_profile h ON b.id_user = h.id_user
                    //    LEFT JOIN tbl_user e ON d.reporting_manager = e.id_user
                    //    LEFT JOIN tbl_profile i ON d.id_user = i.id_user
                    //    LEFT JOIN tbl_user f ON e.reporting_manager = f.id_user
                    //    LEFT JOIN tbl_profile j ON e.id_user = j.id_user
                    //    LEFT JOIN tbl_profile k ON f.id_user = k.id_user
                    //    WHERE 
                    //        a.id_organization = @orgId
                    //     AND c.id_csst_role in ({string.Join(", ", roleIds.Select((s, i) => $"@roleId{i}"))})
                    //        -- AND a.userid not LIKE ""%Bata%""
                    //     AND  a.userid NOT LIKE ""%-old%""
                    //    ";

                    using (var command = new MySqlCommand(query, connection))
                    {

                        command.Parameters.AddWithValue("@orgId", orgId);

                       
                        for (int i = 0; i < Id_deparments.Length; i++)
                        {
                            command.Parameters.AddWithValue($"@Id_deparment{i}", Id_deparments[i]);
                        }
                        for (int i = 0; i < roleIds.Length; i++)
                        {
                            command.Parameters.AddWithValue($"@roleId{i}", roleIds[i]);
                        }
                        for (int i = 0; i < region_names.Length; i++)
                        {
                            command.Parameters.AddWithValue($"@region_name{i}", region_names[i]);
                        }
                        using (var reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
                        {
                            while (await reader.ReadAsync().ConfigureAwait(false))
                            {
                                var result = new ExpandoObject() as IDictionary<string, object>;
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    string columnName = reader.GetName(i);
                                    int duplicateCount = 0;
                                    string uniqueColumnName = columnName;

                                    while (result.ContainsKey(uniqueColumnName))
                                    {
                                        duplicateCount++;
                                        uniqueColumnName = $"{columnName}_{duplicateCount}";
                                    }

                                    result.Add(uniqueColumnName, reader.IsDBNull(i) ? null : reader.GetValue(i));
                                }
                                results.Add(result);
                            }

                            if (results.Count == 0)
                            {

                                var noDataMessage = new ExpandoObject() as IDictionary<string, object>;
                                noDataMessage.Add("Message", "No data found for the provided.");

                                results.Add(noDataMessage);
                            }

                        }

                        await connection.CloseAsync().ConfigureAwait(false);

                        return results;

                    }


                }

            }
            catch (Exception ex)
            {
                // Log the exception (you can replace this with your logging mechanism)
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw; // Rethrow the exception to handle it further up the stack if necessary
            }



        }

        // userList
        public async Task<IEnumerable<dynamic>> GetUserDetails(int orgId, string Id_deparment, string role_id, string region_name,string usercity,string user_designation,string startdate,string enddate,string Status)
        {
            var results = new List<dynamic>();

            var roleIds = string.IsNullOrEmpty(role_id) ? new string[0] : role_id.Split(',').Select(id => id.Trim()).ToArray();
            var Id_deparments = string.IsNullOrEmpty(Id_deparment) ? new string[0] : Id_deparment.Split(',').Select(id => id.Trim()).ToArray();
            var region_names = string.IsNullOrEmpty(region_name) ? new string[0] : region_name.Split(',').Select(id => id.Trim()).ToArray();
            var designation = string.IsNullOrEmpty(user_designation) ? new string[0] : user_designation.Split(',').Select(id => id.Trim()).ToArray();
            var city = string.IsNullOrEmpty(usercity) ? new string[0] : usercity.Split(',').Select(id => id.Trim()).ToArray();

            try
            {
                // Ensure proper connection management
                using (var connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["dbconnectionstring"].ConnectionString))
                {
                    await connection.OpenAsync().ConfigureAwait(false);

                    string query = $@"
                SELECT 
                    a.userid,
                    a.user_grade AS Store_code,
                    a.L4,
                    a.L3,
                    a.L2,
                    a.L1,
                    a.Spectator,
                    h.Location AS Store_Name,
                    CONCAT(g.firstname, ' ', g.lastname) AS NAME,
                    a.UPDATEDTIME,
                    g.date_of_joining,
                    a.user_department,
                    a.user_designation,
                    a.user_function,
                    g.gender,
                    g.city,
                    g.office_address,
                    c.csst_role,
                    a.status
                FROM 
                    tbl_user a
                LEFT JOIN tbl_user b ON a.reporting_manager = b.id_user
                LEFT JOIN tbl_csst_role c ON a.id_role = c.id_csst_role
                LEFT JOIN tbl_profile g ON a.id_user = g.id_user
                LEFT JOIN tbl_profile h ON b.id_user = h.id_user
                LEFT JOIN tbl_user d ON b.reporting_manager = d.id_user
                LEFT JOIN tbl_profile i ON d.id_user = i.id_user
                LEFT JOIN tbl_user e ON d.reporting_manager = e.id_user
                LEFT JOIN tbl_profile j ON e.id_user = j.id_user
                LEFT JOIN tbl_user f ON e.reporting_manager = f.id_user
                LEFT JOIN tbl_profile k ON f.id_user = k.id_user
                JOIN tbl_department o ON c.id_department = o.id_department
                WHERE 
                    a.id_organization = @orgId 
                    AND a.userid NOT LIKE '%Bata%'
                    AND a.userid NOT LIKE '%-old%'
                    AND g.city IN ({string.Join(", ", city.Select((_, i) => $"@usercity{i}"))})
                    AND g.office_address IN ({string.Join(", ", region_names.Select((_, i) => $"@region_name{i}"))})
                    AND c.id_csst_role IN ({string.Join(", ", roleIds.Select((_, i) => $"@roleId{i}"))})
                    AND a.user_designation IN ({string.Join(", ", designation.Select((_, i) => $"@user_designation{i}"))})
                    AND o.id_department IN ({string.Join(", ", Id_deparments.Select((_, i) => $"@Id_deparment{i}"))})
                    and a.STATUS=@Status;
                   
            ";
//-- AND a.UPDATEDTIME BETWEEN @startdate AND @enddate;
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@orgId", orgId);
                        command.Parameters.AddWithValue("@Status", Status);
                        //command.Parameters.AddWithValue("@startdate", startdate);
                        //command.Parameters.AddWithValue("@enddate", enddate);

                        for (int i = 0; i < Id_deparments.Length; i++)
                        {
                            command.Parameters.AddWithValue($"@Id_deparment{i}", Id_deparments[i]);
                        }
                        for (int i = 0; i < roleIds.Length; i++)
                        {
                            command.Parameters.AddWithValue($"@roleId{i}", roleIds[i]);
                        }
                        for (int i = 0; i < region_names.Length; i++)
                        {
                            command.Parameters.AddWithValue($"@region_name{i}", region_names[i]);
                        }
                        for (int i = 0; i < designation.Length; i++)
                        {
                            command.Parameters.AddWithValue($"@user_designation{i}", designation[i]);
                        }
                        for (int i = 0; i < city.Length; i++)
                        {
                            command.Parameters.AddWithValue($"@usercity{i}", city[i]);
                        }

                        using (var reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
                        {
                            while (await reader.ReadAsync().ConfigureAwait(false))
                            {
                                var result = new ExpandoObject() as IDictionary<string, object>;
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    result.Add(reader.GetName(i), reader.IsDBNull(i) ? null : reader.GetValue(i));
                                }
                                results.Add(result);
                            }
                        }

                        if (results.Count == 0)
                        {
                            var noDataMessage = new ExpandoObject() as IDictionary<string, object>;
                            noDataMessage.Add("Message", "No data found for the provided.");
                            results.Add(noDataMessage);
                        }

                        await connection.CloseAsync().ConfigureAwait(false);
                        return results;
                    }


                   


                }

            }
            catch (Exception ex)
            {
                // Log the exception (you can replace this with your logging mechanism)
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw; // Rethrow the exception to handle it further up the stack if necessary
            }



        }

        //Course Analytics
        public async Task<IEnumerable<dynamic>> GetCourseAnalytics( string Id_department, string region_name, string ID_category, string Id_assessment, string startdate, string enddate)
        {
            var results = new List<dynamic>();
            DataTable dt = new DataTable();
            DataTable dt2 = new DataTable();

            var ID_categories = string.IsNullOrEmpty(ID_category) ? new string[0] : ID_category.Split(',').Select(id => id.Trim()).ToArray();
            var Id_departments = string.IsNullOrEmpty(Id_department) ? new string[0] : Id_department.Split(',').Select(id => id.Trim()).ToArray();
            var region_names = string.IsNullOrEmpty(region_name) ? new string[0] : region_name.Split(',').Select(id => id.Trim()).ToArray();
            var Id_assessments = string.IsNullOrEmpty(Id_assessment) ? new string[0] : Id_assessment.Split(',').Select(id => id.Trim()).ToArray();

            string formattedValues = "'" + string.Join("', '", region_names) + "'";
            string Id_assessmentsValues = "'" + string.Join("', '", Id_assessments) + "'";

            try
            {
                using (var connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["dbconnectionstring"].ConnectionString))
                {
                    await connection.OpenAsync().ConfigureAwait(false);

                    string query1 = $@"
    SELECT DISTINCT 
        a.id_category, 
        qna.id_assessment, 
        p.OFFICE_ADDRESS, 
        COUNT(DISTINCT qna.id_user) AS unique_user_count
    FROM 
        tbl_rs_type_qna qna
    JOIN 
        tbl_user u ON qna.id_user = u.id_user
    JOIN 
        tbl_profile p ON u.id_user = p.id_user
    JOIN 
        tbl_csst_role r ON u.id_role = r.id_csst_role
    JOIN 
        tbl_department d ON r.id_department = d.id_department
    JOIN 
        tbl_assessment_user_assignment a ON qna.id_assessment = a.id_assessment
    JOIN 
        tbl_assessment b ON a.id_assessment = b.id_assessment
    WHERE 
        qna.id_assessment IN ({string.Join(", ", Id_assessments.Select((_, i) => $"@Id_assessment{i}"))})
        AND r.id_csst_role NOT IN (466, 483, 527, 528)
        AND u.USERID NOT LIKE '%_SM_BA%'
        AND qna.updated_date_time >= @startdate
        AND qna.updated_date_time < DATE_ADD(@enddate, INTERVAL 1 DAY)
        AND qna.result_in_percentage >= 80
        AND d.id_department IN ({string.Join(", ", Id_departments.Select((_, i) => $"@Id_department{i}"))})
        AND p.OFFICE_ADDRESS IN ({string.Join(", ", region_names.Select((_, i) => $"@region_name{i}"))})
        AND u.STATUS = 'A'
    GROUP BY 
        qna.id_assessment, a.id_assessment, p.OFFICE_ADDRESS";


                    //string query1 = $"SELECT DISTINCT "+
                    //        " a.id_category,"+
                    //        " qna.id_assessment,"+
                    //        " p.OFFICE_ADDRESS,"+
                    //        " COUNT(DISTINCT qna.id_user) AS unique_user_count" +
                    //    " FROM "+
                    //        "tbl_rs_type_qna qna"+
                    //    " JOIN "+
                    //        "tbl_user u ON qna.id_user = u.id_user"+
                    //    " JOIN "+
                    //       "tbl_profile p ON u.id_user = p.id_user"+
                    //    " JOIN "+
                    //       " tbl_csst_role r ON u.id_role = r.id_csst_role"+
                    //   " JOIN "+
                    //       " tbl_department d ON r.id_department = d.id_department"+
                    //    " JOIN "+
                    //      " tbl_assessment_user_assignment a ON qna.id_assessment = a.id_assessment"+
                    //   " JOIN "+
                    //       " tbl_assessment b ON a.id_assessment = b.id_assessment"+
                    //   " WHERE "+
                    //     " qna.id_assessment IN ( " + Id_assessmentsValues + ")" +
                    //       " and   r.id_csst_role not in (466,483,527,528)"+
                    //        " and   u.USERID not like '%_SM_BA%'"+
                    //         " AND qna.updated_date_time >= '" + startdate + "' AND qna.updated_date_time < DATE_ADD( '" + enddate + "', INTERVAL 1 DAY)" +
                    //       " AND   qna.result_in_percentage >= 80" +
                    //       " AND   d.id_department IN ("+ Id_department + ") "+
                    //       " AND   p.OFFICE_ADDRESS IN ("+ formattedValues + ")"+
                    //       " AND   u.STATUS = 'A' "+
                    //        " GROUP BY " +
                    //       " qna.id_assessment, a.id_assessment, p.OFFICE_ADDRESS";


                    string query2 = $@"
                    SELECT 
                        p.OFFICE_ADDRESS as region,
                        d.Department_name as Department,
                        cpm.id_category, 
                        cat.CATEGORYNAME,
                        COUNT(DISTINCT cpm.id_user) AS assigned_users,
                        MIN(cpm.start_date) AS first_start_date
                    FROM 
                        tbl_content_program_mapping cpm
                    JOIN 
                        tbl_user u ON cpm.id_user = u.id_user
                    JOIN 
                        tbl_category cat ON cpm.id_category = cat.id_category
                    JOIN 
                        tbl_profile p ON u.id_user = p.id_user
                    JOIN 
                        tbl_csst_role r ON u.id_role = r.id_csst_role
                    JOIN 
                        tbl_department d ON r.id_department = d.id_department
                    WHERE 
                        cpm.id_category IN ({string.Join(", ", ID_categories.Select((_, i) => $"@ID_category{i}"))})
                        AND u.USERID NOT LIKE '%Bata%'
                        AND d.id_department IN ({string.Join(", ", Id_departments.Select((_, i) => $"@Id_department{i}"))})
                        AND p.OFFICE_ADDRESS IN ({string.Join(", ", region_names.Select((_, i) => $"@region_name{i}"))})
                        AND u.STATUS = 'A'
                    GROUP BY 
                        cpm.id_category, cat.CATEGORYNAME,p.OFFICE_ADDRESS;
                    ";


                    using (var command1 = new MySqlCommand(query1, connection))
                    {
                        for (int i = 0; i < Id_assessments.Length; i++)
                        {
                            command1.Parameters.AddWithValue($"@Id_assessment{i}", Id_assessments[i]);
                        }
                        for (int i = 0; i < Id_departments.Length; i++)
                        {
                            command1.Parameters.AddWithValue($"@Id_department{i}", Id_departments[i]);
                        }
                        for (int i = 0; i < region_names.Length; i++)
                        {
                            command1.Parameters.AddWithValue($"@region_name{i}", region_names[i]);
                        }
                        command1.Parameters.AddWithValue("@startdate", startdate);
                        command1.Parameters.AddWithValue("@enddate", enddate);

                        using (var reader = await command1.ExecuteReaderAsync().ConfigureAwait(false))
                        {
                            dt.Load(reader);
                        }
                    }


                    //using (var command1 = new MySqlCommand(query1, connection))
                    //{
                    //    using (var reader = await command1.ExecuteReaderAsync().ConfigureAwait(false))
                    //    {
                    //        dt.Load(reader);
                    //    }

                    //}



                    // Second query

                    using (var command2= new MySqlCommand(query2, connection))
                    {
                        for (int i = 0; i < ID_categories.Length; i++)
                        {
                            command2.Parameters.AddWithValue($"@ID_category{i}", ID_categories[i]);
                        }
                        for (int i = 0; i < Id_departments.Length; i++)
                        {
                            command2.Parameters.AddWithValue($"@Id_department{i}", Id_departments[i]);
                        }
                        for (int i = 0; i < region_names.Length; i++)
                        {
                            command2.Parameters.AddWithValue($"@region_name{i}", region_names[i]);
                        }

                        using (var reader = await command2.ExecuteReaderAsync().ConfigureAwait(false))
                        {
                            dt2.Load(reader);
                        }

                    }

                    // Process and combine results from dt and dt2
                    foreach (DataRow row in dt2.Rows)
                    {
                        dynamic result = new ExpandoObject();
                        result.region = row["region"];
                        result.Department = row["Department"];
                        result.id_category = row["id_category"];
                        result.CATEGORYNAME = row["CATEGORYNAME"];
                        result.assigned_users = row["assigned_users"];
                        result.first_start_date = row["first_start_date"];

                        var matchingRows = dt.AsEnumerable().Where(r => r["id_category"].ToString() == row["id_category"].ToString());

                        // If there are matches, add the additional information
                        if (matchingRows.Any())
                        {
                            result.unique_user_count = matchingRows.First()["unique_user_count"];
                        }
                        else
                        {
                            // Handle case where there is no matching data in dt2
                            result.unique_user_count = 0; // Or some other default value
                        }

                        results.Add(result);
                    }

                    // Check if results are empty
                    if (results.Count == 0)
                    {
                        var noDataMessage = new ExpandoObject() as IDictionary<string, object>;
                        noDataMessage.Add("Message", "No data found for the provided criteria.");
                        results.Add(noDataMessage);
                    }

                    await connection.CloseAsync().ConfigureAwait(false);
                    return results;
                }
            }
            catch (Exception ex)
            {
                // Log the exception (you can replace this with your logging mechanism)
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw; // Rethrow the exception to handle it further up the stack if necessary
            }
        }

        public async Task<IEnumerable<dynamic>> GetCourseAnalyticsChangequery(int orgid, string Id_department,string rolewise, string region_name, string ID_category, string Id_assessment, string startdate, string enddate)
        {
            var results = new List<dynamic>();
            DataTable dt = new DataTable();
            DataTable dt2 = new DataTable();
            DataTable dt3 = new DataTable();
            DataTable dt4 = new DataTable();

            var ID_categories = string.IsNullOrEmpty(ID_category) ? new string[0] : ID_category.Split(',').Select(id => id.Trim()).ToArray();
            var Id_departments = string.IsNullOrEmpty(Id_department) ? new string[0] : Id_department.Split(',').Select(id => id.Trim()).ToArray();
            var rolewises = string.IsNullOrEmpty(rolewise) ? new string[0] : rolewise.Split(',').Select(id => id.Trim()).ToArray();
            var region_names = string.IsNullOrEmpty(region_name) ? new string[0] : region_name.Split(',').Select(id => id.Trim()).ToArray();
            var Id_assessments = string.IsNullOrEmpty(Id_assessment) ? new string[0] : Id_assessment.Split(',').Select(id => id.Trim()).ToArray();

            string formattedValues = "'" + string.Join("', '", region_names) + "'";
            string Id_assessmentsValues = "'" + string.Join("', '", Id_assessments) + "'";

            try
            {
                using (var connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["dbconnectionstring"].ConnectionString))
                {
                    await connection.OpenAsync().ConfigureAwait(false);

                    string query1 = $@"
                        -- First query: Fetch completed users
                        SELECT 
                            d.id_department,
                            d.department_name,  
                            p.OFFICE_ADDRESS,   
                            q.id_assessment,    
                            COUNT(DISTINCT q.id_user) AS completed_user  
                        FROM 
                            tbl_rs_type_qna q
                        JOIN 
                            tbl_user u ON q.id_user = u.id_user  
                        JOIN 
                            tbl_profile p ON u.id_user = p.id_user  
                        JOIN 
                            tbl_csst_role r ON u.id_role = r.id_csst_role  
                        JOIN 
                            tbl_department d ON r.id_department = d.id_department  
                        WHERE 
                            d.id_org = @orgId 
                            AND u.STATUS = 'A' 
                            AND q.id_assessment IN ({string.Join(", ", Id_assessments.Select((_, i) => $"@Id_assessment{i}"))})  
                            AND q.result_in_percentage >= 80  
                            AND q.updated_date_time BETWEEN @startdate AND @enddate  
                            AND d.id_department IN ({string.Join(", ", Id_departments.Select((_, i) => $"@Id_department{i}"))}) 
                            AND p.OFFICE_ADDRESS IN  ({string.Join(", ", region_names.Select((_, i) => $"@region_name{i}"))})  
                            AND u.USERID NOT REGEXP 'Bata|_SM_BA|_DM_BA|_RM_BA|_SM_HP'
                            AND r.id_csst_role IN ({string.Join(", ", rolewises.Select((_, i) => $"@rolewise{i}"))}) 
                        GROUP BY 
                            d.id_department,
                            d.department_name,
                            p.OFFICE_ADDRESS,
                            q.id_assessment 
                        ORDER BY 
                            q.id_assessment ASC,       
                            d.department_name ASC,   
                            p.OFFICE_ADDRESS ASC;
                           ";
                    using (var command1 = new MySqlCommand(query1, connection))
                    {
                        for (int i = 0; i < Id_assessments.Length; i++)
                        {
                            command1.Parameters.AddWithValue($"@Id_assessment{i}", Id_assessments[i]);
                        }
                        for (int i = 0; i < Id_departments.Length; i++)
                        {
                            command1.Parameters.AddWithValue($"@Id_department{i}", Id_departments[i]);
                        }

                        for (int i = 0; i < rolewises.Length; i++)
                        {
                            command1.Parameters.AddWithValue($"@rolewise{i}", rolewises[i]);
                        }

                        for (int i = 0; i < region_names.Length; i++)
                        {
                            command1.Parameters.AddWithValue($"@region_name{i}", region_names[i]);
                        }
                        command1.Parameters.AddWithValue("@startdate", startdate);
                        command1.Parameters.AddWithValue("@enddate", enddate);
                        command1.Parameters.AddWithValue("@orgId", orgid);

                        using (var reader = await command1.ExecuteReaderAsync().ConfigureAwait(false))
                        {
                            dt.Load(reader);
                        }
                    }





                    //Second query: Fetch assigned users
                    string query2 = $@"
                        SELECT 
                            d.id_department,
                            d.department_name,
                            p.OFFICE_ADDRESS,   
                            a.id_assessment,    
                            MIN(a.start_date) AS first_start_date,  -- Get the earliest start date
                            COUNT(DISTINCT u.id_user) AS Assigned_users  -- Count of unique assigned users
                        FROM 
                            tbl_assessment_user_assignment a
                        JOIN 
                            tbl_user u ON a.id_user = u.id_user  
                        JOIN 
                            tbl_csst_role r ON u.id_role = r.id_csst_role 
                        JOIN 
                            tbl_department d ON r.id_department = d.id_department  
                        JOIN 
                            tbl_profile p ON u.id_user = p.id_user 
                        WHERE 
                            d.id_org = @orgId  
                            AND u.STATUS = 'A' 
                         AND a.id_assessment IN ({string.Join(", ", Id_assessments.Select((_, i) => $"@Id_assessment{i}"))})  -- Filter by assessments
                         AND d.id_department IN ({string.Join(", ", Id_departments.Select((_, i) => $"@Id_department{i}"))})  -- Filter by department
                         AND p.OFFICE_ADDRESS IN ({string.Join(", ", region_names.Select((_, i) => $"@region_name{i}"))})  -- Filter by office address
                        
                         AND u.USERID NOT REGEXP 'Bata|_SM_BA|_DM_BA|_RM_BA|_SM_HP'
                         AND a.start_date BETWEEN @startdate AND @enddate  
                        
                          AND r.id_csst_role IN ({string.Join(", ", rolewises.Select((_, i) => $"@rolewise{i}"))})  
                        GROUP BY 
                            d.id_department,
                            d.department_name,
                            p.OFFICE_ADDRESS,
                            a.id_assessment 
                        ORDER BY 
                            a.id_assessment ASC,      
                            d.department_name ASC,   
                            p.OFFICE_ADDRESS ASC; ";

                    using (var command2 = new MySqlCommand(query2, connection))
                    {
                        for (int i = 0; i < Id_assessments.Length; i++)
                        {
                            command2.Parameters.AddWithValue($"@Id_assessment{i}", Id_assessments[i]);
                        }
                        for (int i = 0; i < Id_departments.Length; i++)
                        {
                            command2.Parameters.AddWithValue($"@Id_department{i}", Id_departments[i]);
                        }
                        for (int i = 0; i < rolewises.Length; i++)
                        {
                            command2.Parameters.AddWithValue($"@rolewise{i}", rolewises[i]);
                        }
                        for (int i = 0; i < region_names.Length; i++)
                        {
                            command2.Parameters.AddWithValue($"@region_name{i}", region_names[i]);
                        }
                        command2.Parameters.AddWithValue("@startdate", startdate);
                        command2.Parameters.AddWithValue("@enddate", enddate);
                        command2.Parameters.AddWithValue("@orgId", orgid);

                        using (var reader = await command2.ExecuteReaderAsync().ConfigureAwait(false))
                        {
                            dt2.Load(reader);
                        }
                    }



                    // Initialize dt3 with columns from both dt2 and dt
                    if (dt3.Columns.Count == 0) // Only add columns once
                    {
                        // Add columns from dt2 to dt3
                        foreach (DataColumn col in dt2.Columns)
                        {
                            if (!dt3.Columns.Contains(col.ColumnName))
                            {
                                dt3.Columns.Add(col.ColumnName, col.DataType);
                            }
                        }

                        // Add columns from dt to dt3
                        foreach (DataColumn col in dt.Columns)
                        {
                            if (!dt3.Columns.Contains(col.ColumnName))
                            {
                                dt3.Columns.Add(col.ColumnName, col.DataType);
                            }
                        }
                    }

                    foreach (DataRow row in dt2.Rows)
                    {
                        // Find matching rows in dt based on 'id_department' and 'id_assessment'
                        var matchingRows = dt.AsEnumerable().Where(r =>
                            r["id_department"].ToString() == row["id_department"].ToString() &&
                            r["department_name"].ToString() == row["department_name"].ToString() &&
                            r["OFFICE_ADDRESS"].ToString() == row["OFFICE_ADDRESS"].ToString() &&
                            r["id_assessment"].ToString() == row["id_assessment"].ToString());

                        // If there are matching rows, add fields from both dt and dt2 dynamically to dt3
                        if (matchingRows.Any())
                        {
                            foreach (var match in matchingRows)
                            {
                                DataRow newRow = dt3.NewRow();

                                // Add columns from dt2 (current row) to dt3
                                foreach (DataColumn col in dt2.Columns)
                                {
                                    if (row.Table.Columns.Contains(col.ColumnName)) // Ensure the column exists
                                    {
                                        newRow[col.ColumnName] = row[col.ColumnName];
                                    }
                                }

                                // Add columns from dt (matching row) to dt3
                                foreach (DataColumn col in dt.Columns)
                                {
                                    if (match.Table.Columns.Contains(col.ColumnName)) // Ensure the column exists
                                    {
                                        if (!newRow.Table.Columns.Contains(col.ColumnName))
                                        {
                                            newRow.Table.Columns.Add(col.ColumnName, col.DataType); // Ensure column exists in dt3
                                        }
                                        newRow[col.ColumnName] = match[col.ColumnName];
                                    }
                                }

                                dt3.Rows.Add(newRow); // Add the populated row to dt3
                            }
                        }
                        else
                        {
                            // If no matching rows are found, optionally handle adding a default row to dt3
                            DataRow newRow = dt3.NewRow();

                            // Add columns from dt2 to dt3 (no match case)
                            foreach (DataColumn col in dt2.Columns)
                            {
                                newRow[col.ColumnName] = row[col.ColumnName];
                            }

                            // Optionally add default values for unmatched columns
                            //newRow["unique_user_count"] = 0; // Or some other default value

                            dt3.Rows.Add(newRow); // Add the default row to dt3
                        }
                    }


                    string query3 = $@"
                            SELECT distinct id_assessment,CATEGORYNAME
                            FROM tbl_assessment_user_assignment a
                            JOIN tbl_category c ON a.ID_category = c.ID_category
                            WHERE a.id_assessment IN ({string.Join(", ", Id_assessments.Select((_, i) => $"@Id_assessment{i}"))})
                            AND c.ID_category IN ({string.Join(", ", ID_categories.Select((_, i) => $"@ID_category{i}"))});
                    ";

                    //SELECT
                    //    p.OFFICE_ADDRESS as region,
                    //    d.Department_name as Department,
                    //    cpm.id_category, 
                    //    cat.CATEGORYNAME,
                    //    COUNT(DISTINCT cpm.id_user) AS assigned_users,
                    //    MIN(cpm.start_date) AS first_start_date
                    //FROM
                    //    tbl_content_program_mapping cpm
                    //JOIN
                    //    tbl_user u ON cpm.id_user = u.id_user
                    //JOIN
                    //    tbl_category cat ON cpm.id_category = cat.id_category
                    //JOIN
                    //    tbl_profile p ON u.id_user = p.id_user
                    //JOIN
                    //    tbl_csst_role r ON u.id_role = r.id_csst_role
                    //JOIN
                    //    tbl_department d ON r.id_department = d.id_department
                    //WHERE
                    //    cpm.id_category IN({ string.Join(", ", ID_categories.Select((_, i) => $"@ID_category{i}"))})
                    //    AND u.USERID NOT LIKE '%Bata%'
                    //    AND d.id_department IN({ string.Join(", ", Id_departments.Select((_, i) => $"@Id_department{i}"))})
                    //    AND p.OFFICE_ADDRESS IN({ string.Join(", ", region_names.Select((_, i) => $"@region_name{i}"))})
                    //    AND u.STATUS = 'A'
                    //GROUP BY
                    //    cpm.id_category, cat.CATEGORYNAME,p.OFFICE_ADDRESS;




                    using (var command4 = new MySqlCommand(query3, connection))
                    {
                        for (int i = 0; i < ID_categories.Length; i++)
                        {
                            command4.Parameters.AddWithValue($"@ID_category{i}", ID_categories[i]);
                        }

                        for (int i = 0; i < Id_assessments.Length; i++)
                        {
                            command4.Parameters.AddWithValue($"@Id_assessment{i}", Id_assessments[i]);
                        }

                        using (var reader = await command4.ExecuteReaderAsync().ConfigureAwait(false))
                        {
                            dt4.Load(reader);
                        }

                    }

                    foreach (DataRow row in dt3.Rows)
                    {
                        dynamic result = new ExpandoObject();
                        result.region = row["OFFICE_ADDRESS"];
                        result.Department = row["department_name"];
                        result.id_assessment = row["id_assessment"];
                        result.completed_user = row["completed_user"];
                        result.assigned_users = row["assigned_users"];
                        result.first_start_date = row["first_start_date"];

                        var matchingRows = dt4.AsEnumerable().Where(r => r["id_assessment"].ToString() == row["id_assessment"].ToString());

                        if (matchingRows.Any())
                        {
                            var firstMatch = matchingRows.First();
                            //result.unique_user_count = firstMatch["unique_user_count"];
                            result.CATEGORYNAME = firstMatch["CATEGORYNAME"];  // Adding CATEGORYNAME from dt4
                        }
                        else
                        {
                            result.unique_user_count = 0;
                            result.CATEGORYNAME = "Unknown";  // Or assign some default value if no match found
                        }

                        results.Add(result);
                    }

                   
                    if (results.Count == 0)
                    {
                        var noDataMessage = new ExpandoObject() as IDictionary<string, object>;
                        noDataMessage.Add("Message", "No data found for the provided criteria.");
                        results.Add(noDataMessage);
                    }

                    await connection.CloseAsync().ConfigureAwait(false);
                    return results;
                }
            }
            catch (Exception ex)
            {
               
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw; 
            }
        }



        //DDK Drowndown
        public List<tbl_organization> GetDDKorgid(int orgId)
        {
            List<tbl_organization> list = new List<tbl_organization>();

            try
            {
                using (var connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["dbconnectionstring"].ConnectionString))
                {
                    connection.Open();
                    //_connection.Open();
                    string query = @"SELECT * from tbl_org_mapping where M2ost = @orgId";

                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@orgId", orgId);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string organizationName = reader["Organization_Name"].ToString();

                                if (!string.IsNullOrEmpty(organizationName))
                                {
                                    tbl_organization orgItem = new tbl_organization
                                    {
                                        ID_ORGANIZATION = reader["Coroebus_org_Id"].ToString(),
                                        ORGANIZATION_NAME = organizationName
                                    };

                                    list.Add(orgItem);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle the exception (log it, throw it, etc.)
            }
            finally
            {

               // _connectionNgage.Close();

            }

            return list;
        }

        //DDK Drowdown coroebus
        public List<object> GetDDKcategory_name(string orgid)
        {
            List<object> list = new List<object>();

            try
            {
                using (var connectionCoroebus = new MySqlConnection(ConfigurationManager.ConnectionStrings["db_tgc_corobus"].ConnectionString))
                {
                    connectionCoroebus.Open();
                    // _connectionCoroebus.Open();

                    // Split the orgids string into a list
                    var orgidList = orgid.Split(',').Select(o => o.Trim()).ToList();
                    // Create a string with parameter placeholders
                    var parameters = string.Join(",", orgidList.Select((_, index) => $"@OrgId{index}"));

                    string query = $@"SELECT id_learning_academy_category, category_name 
                          FROM db_coroebus_tgc.tbl_learning_academy_category 
                          WHERE id_coroebus_organization IN ({parameters}) order by id_learning_academy_category desc";

                    using (MySqlCommand cmd = new MySqlCommand(query, connectionCoroebus))
                    {
                        // Add parameters to the command
                        for (int i = 0; i < orgidList.Count; i++)
                        {
                            cmd.Parameters.AddWithValue($"@OrgId{i}", orgidList[i]);
                        }

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var item = new
                                {
                                    Text = reader["category_name"].ToString(),
                                    Value = reader["id_learning_academy_category"].ToString()
                                };

                                list.Add(item);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle the exception (log it, throw it, etc.)
                Console.WriteLine("An error occurred: " + ex.Message); // For example purposes
            }
            finally
            {
                _connectionCoroebus.Close();
            }

            return list;
        }

        //DDK not corobus
        public List<object> GetDDKcategory_name1(string orgid)
        {
            List<object> list = new List<object>();

            try
            {
                using (var connectionCoroebus = new MySqlConnection(ConfigurationManager.ConnectionStrings["dbconnectionstring"].ConnectionString))
                {
                    connectionCoroebus.Open();
                    // _connectionCoroebus.Open();

                    // Split the orgids string into a list
                    var orgidList = orgid.Split(',').Select(o => o.Trim()).ToList();
                    // Create a string with parameter placeholders
                    var parameters = string.Join(",", orgidList.Select((_, index) => $"@OrgId{index}"));

                    string query = $@"SELECT distinct lc.id_learning_category, lc.category_name
                                    FROM tbl_learning_category lc JOIN tbl_department d ON lc.id_organization = d.Id_org
                                    WHERE d.Id_department IN ({parameters})
                                    ORDER BY lc.id_learning_category DESC;";

                    using (MySqlCommand cmd = new MySqlCommand(query, connectionCoroebus))
                    {
                        // Add parameters to the command
                        for (int i = 0; i < orgidList.Count; i++)
                        {
                            cmd.Parameters.AddWithValue($"@OrgId{i}", orgidList[i]);
                        }

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var item = new
                                {
                                    Text = reader["category_name"].ToString(),
                                    Value = reader["id_learning_category"].ToString()
                                };

                                list.Add(item);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle the exception (log it, throw it, etc.)
                Console.WriteLine("An error occurred: " + ex.Message); // For example purposes
            }
            finally
            {
                _connectionCoroebus.Close();
            }

            return list;
        }


        //DDK Drowdown coroebus
        public List<object> DDKsubcategory(string orgid, string category)
        {
            List<object> list = new List<object>();

            try
            {
                using (var connectionCoroebus = new MySqlConnection(ConfigurationManager.ConnectionStrings["db_tgc_corobus"].ConnectionString))
                {
                    connectionCoroebus.Open();
                    // _connectionCoroebus.Open();

                    // Split the orgid and category strings into lists
                    var orgidList = orgid.Split(',').Select(o => o.Trim()).ToList();
                    var categoryList = category.Split(',').Select(c => c.Trim()).ToList();

                    // Create strings with parameter placeholders
                    var orgidParameters = string.Join(",", orgidList.Select((_, index) => $"@OrgId{index}"));
                    var categoryParameters = string.Join(",", categoryList.Select((_, index) => $"@CategoryId{index}"));

                    string query = $@"SELECT id_learning_academy_sub_category, sub_category_name, id_learning_academy_category 
                          FROM tbl_learning_academy_sub_category 
                          WHERE id_coroebus_organization IN ({orgidParameters})
                          AND id_learning_academy_category IN ({categoryParameters})";


                    using (MySqlCommand cmd = new MySqlCommand(query, connectionCoroebus))
                    {
                        // Add orgid parameters to the command
                        for (int i = 0; i < orgidList.Count; i++)
                        {
                            cmd.Parameters.AddWithValue($"@OrgId{i}", orgidList[i]);
                        }

                        // Add category parameters to the command
                        for (int i = 0; i < categoryList.Count; i++)
                        {
                            cmd.Parameters.AddWithValue($"@CategoryId{i}", categoryList[i]);
                        }

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var item = new
                                {
                                    Text = reader["sub_category_name"].ToString(),
                                    Value = reader["id_learning_academy_sub_category"].ToString()
                                };

                                list.Add(item);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle the exception (log it, throw it, etc.)
                Console.WriteLine("An error occurred: " + ex.Message); // For example purposes
            }
            finally
            {
                _connectionCoroebus.Close();
            }

            return list;
        }

        public List<object> DDKsubcategorynotincrobus(string orgid, string category)
        {
            List<object> list = new List<object>();

            try
            {
                using (var connectionCoroebus = new MySqlConnection(ConfigurationManager.ConnectionStrings["dbconnectionstring"].ConnectionString))
                {
                    connectionCoroebus.Open();
                    // _connectionCoroebus.Open();

                    // Split the orgid and category strings into lists
                    var orgidList = orgid.Split(',').Select(o => o.Trim()).ToList();
                    var categoryList = category.Split(',').Select(c => c.Trim()).ToList();

                    // Create strings with parameter placeholders
                    var orgidParameters = string.Join(",", orgidList.Select((_, index) => $"@OrgId{index}"));
                    var categoryParameters = string.Join(",", categoryList.Select((_, index) => $"@CategoryId{index}"));

                    string query = $@" SELECT distinct sc.id_learning_sub_category, sc.sub_category_name, sc.id_learning_category 
                          FROM tbl_learning_sub_category sc join  tbl_department d on d.Id_org = sc.id_organization
                          WHERE  d.Id_department IN ({orgidParameters}) AND sc.id_learning_category  IN ({categoryParameters});";


                    using (MySqlCommand cmd = new MySqlCommand(query, connectionCoroebus))
                    {
                        // Add orgid parameters to the command
                        for (int i = 0; i < orgidList.Count; i++)
                        {
                            cmd.Parameters.AddWithValue($"@OrgId{i}", orgidList[i]);
                        }

                        // Add category parameters to the command
                        for (int i = 0; i < categoryList.Count; i++)
                        {
                            cmd.Parameters.AddWithValue($"@CategoryId{i}", categoryList[i]);
                        }

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var item = new
                                {
                                    Text = reader["sub_category_name"].ToString(),
                                    Value = reader["id_learning_sub_category"].ToString()
                                };

                                list.Add(item);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle the exception (log it, throw it, etc.)
                Console.WriteLine("An error occurred: " + ex.Message); // For example purposes
            }
            finally
            {
                _connectionCoroebus.Close();
            }

            return list;
        }


        //DDK Drowdown coroebus
        public List<object> DDKacademybrief(string orgid, string id_learning_academybrief)
        {
            List<object> list = new List<object>();

            try
            {
                using (var connectionCoroebus = new MySqlConnection(ConfigurationManager.ConnectionStrings["db_tgc_corobus"].ConnectionString))
                {
                    connectionCoroebus.Open();
                    //  _connectionCoroebus.Open();

                    // Split the orgid and id_learning_academybrief strings into lists
                    var orgidList = orgid.Split(',').Select(o => o.Trim()).ToList();
                    var idLearningAcademybriefList = id_learning_academybrief.Split(',').Select(c => c.Trim()).ToList();

                    // Create strings with parameter placeholders
                    var orgidParameters = string.Join(",", orgidList.Select((_, index) => $"@OrgId{index}"));
                    var idLearningAcademybriefParameters = string.Join(",", idLearningAcademybriefList.Select((_, index) => $"@idLearningAcademybrief{index}"));

                    string query = $@"SELECT a.id_learning_academy_brief,b.brief_question,b.id_brief_question 
                          FROM tbl_learning_academy_brief a 
                          INNER JOIN tbl_learning_academy_brief_question b 
                          ON a.id_learning_academy_brief = b.id_learning_academy_brief
                          WHERE a.id_coroebus_organization IN ({orgidParameters}) 
                          AND a.status = 'A'
                          AND a.id_learning_academy_sub_category IN ({idLearningAcademybriefParameters})";

                    using (MySqlCommand cmd = new MySqlCommand(query, connectionCoroebus))
                    {
                        // Add orgid parameters to the command
                        for (int i = 0; i < orgidList.Count; i++)
                        {
                            cmd.Parameters.AddWithValue($"@OrgId{i}", orgidList[i]);
                        }

                        // Add category parameters to the command
                        for (int i = 0; i < idLearningAcademybriefList.Count; i++)
                        {
                            cmd.Parameters.AddWithValue($"@idLearningAcademybrief{i}", idLearningAcademybriefList[i]);
                        }

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var item = new
                                {
                                    //id_learning_academy_brief = reader["id_learning_academy_brief"].ToString(),
                                    Text = reader["brief_question"].ToString(), // Added this missing field
                                    Value = reader["id_brief_question"].ToString()
                                };

                                list.Add(item);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle the exception (log it, throw it, etc.)
                Console.WriteLine("An error occurred: " + ex.Message); // For example purposes
            }
            finally
            {
                _connectionCoroebus.Close();
            }

            return list;
        }

        //comment out
        public List<object> DDKacademybriefnotincrobus(string orgid, string id_learning_academybrief)
        {
            List<object> list = new List<object>();

            try
            {
                using (var connectionCoroebus = new MySqlConnection(ConfigurationManager.ConnectionStrings["dbconnectionstring"].ConnectionString))
                {
                    connectionCoroebus.Open();
                    //  _connectionCoroebus.Open();

                    // Split the orgid and id_learning_academybrief strings into lists
                    var orgidList = orgid.Split(',').Select(o => o.Trim()).ToList();
                    var idLearningAcademybriefList = id_learning_academybrief.Split(',').Select(c => c.Trim()).ToList();

                    // Create strings with parameter placeholders
                    var orgidParameters = string.Join(",", orgidList.Select((_, index) => $"@OrgId{index}"));
                    var idLearningAcademybriefParameters = string.Join(",", idLearningAcademybriefList.Select((_, index) => $"@idLearningAcademybrief{index}"));

                    string query = $@"SELECT distinct q.question,q.id_learning_question 
                                        FROM tbl_learning_question q
                                        JOIN tbl_department d 
                                        ON q.id_organization = d.Id_org
                                        WHERE d.Id_department IN ({orgidParameters})
                                         AND q.status = 'A'
                                        AND q.id_learning_sub_category IN ({idLearningAcademybriefParameters});";

                    using (MySqlCommand cmd = new MySqlCommand(query, connectionCoroebus))
                    {
                        // Add orgid parameters to the command
                        for (int i = 0; i < orgidList.Count; i++)
                        {
                            cmd.Parameters.AddWithValue($"@OrgId{i}", orgidList[i]);
                        }

                        // Add category parameters to the command
                        for (int i = 0; i < idLearningAcademybriefList.Count; i++)
                        {
                            cmd.Parameters.AddWithValue($"@idLearningAcademybrief{i}", idLearningAcademybriefList[i]);
                        }

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var item = new
                                {
                                    //id_learning_academy_brief = reader["id_learning_academy_brief"].ToString(),
                                    Text = reader["question"].ToString(), // Added this missing field
                                    Value = reader["id_learning_question"].ToString()
                                };

                                list.Add(item);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle the exception (log it, throw it, etc.)
                Console.WriteLine("An error occurred: " + ex.Message); // For example purposes
            }
            finally
            {
                _connectionCoroebus.Close();
            }

            return list;
        }






        //DDK coroebus
        public async Task<IEnumerable<dynamic>> DDKQuestionan(string[] orgIdcoroebus_List, string[] id_learning_academy_brief)
        {
            var results = new List<dynamic>();




            var orgid_coroebusParams = string.Join(", ", orgIdcoroebus_List.Select((id, index) => $"@orgid_coroebus{index}"));
            var id_learning_academy_briefParams = string.Join(", ", id_learning_academy_brief.Select((id, index) => $"@id_learning_academy_brief{index}"));

          
           // await _connectionCoroebus.OpenAsync().ConfigureAwait(false);
            using (var connectionCoroebus = new MySqlConnection(ConfigurationManager.ConnectionStrings["db_tgc_corobus"].ConnectionString))
            {
                await connectionCoroebus.OpenAsync().ConfigureAwait(false);
                string query = $@"
    SELECT DDK_Category AS ""DDK_Category"",
    id_learning_academy_brief AS ""DDK_Title_ID"",
    DDK_Title AS ""DDK_Title"",
    DDK_Question AS ""DDk_Question"",
  

   COUNT(CASE WHEN Answer_Status='Right Answer' THEN Answer_Status END) AS ""Correct_Answer_Count"",
   COUNT(CASE WHEN Answer_Status='Wrong Answer' THEN Answer_Status END) AS ""Incorrect_Answer_Count"",
   COUNT(Answer_Status) AS ""Total_Answer_Count""

     FROM (SELECT 
    -- a.id_coroebus_user,
    d.USERID,
    d.id_role,
    d.first_name AS NAME,
    -- d.id_role,
    -- d.id_coroebus_organization,
    -- c.id_coroebus_game,
    -- c.id_learning_academy_category,
    c.category_name AS DDK_Category,
     b.id_learning_academy_brief ,
    b.brief_title AS DDK_Title,
    e.id_brief_question,
    e.brief_question AS DDK_Question,
    -- f.id_brief_answer,
    f.option_answer AS DDK_Answer,
    -- a.score,
    -- a.is_correct_answer,
    CASE WHEN a.is_correct_answer =0 THEN ""Wrong Answer"" ELSE ""Right Answer"" END AS Answer_Status,
    a.created_at AS Taken_at
    FROM tbl_learning_academy_answer_log AS a 
    INNER JOIN tbl_learning_academy_brief AS b 
    ON a.id_learning_academy_brief=b.id_learning_academy_brief 
    INNER JOIN tbl_learning_academy_category AS c
    ON b.id_learning_academy_category=c.id_learning_academy_category
    INNER JOIN tbl_learning_academy_brief_question AS e
    ON e.id_learning_academy_brief=b.id_learning_academy_brief
    INNER JOIN tbl_learning_academy_brief_answer AS f
    ON f.id_brief_answer=a.id_brief_answer
    INNER JOIN tbl_coroebus_user AS d ON a.id_coroebus_user=d.id_coroebus_user
    WHERE  b.id_coroebus_organization IN ({orgid_coroebusParams})
    AND f.id_brief_question IN ({id_learning_academy_briefParams})
    AND d.userid NOT LIKE '%Bata%') tbl01
    GROUP BY id_brief_question";
                //                   SELECT 
                //    DDK_Category AS ""DDK_Category"",
                //    DDK_Title AS ""DDK_Title"",
                //    DDK_Question AS ""DDK_Question"",
                //    COUNT(CASE WHEN Answer_Status = 'Right Answer' THEN Answer_Status END) AS ""Correct_Answer_Count"",
                //    COUNT(CASE WHEN Answer_Status = 'Wrong Answer' THEN Answer_Status END) AS ""Incorrect_Answer_Count"",
                //    COUNT(Answer_Status) AS ""Total Answer Count""
                //FROM (
                //    SELECT 
                //        d.USERID,
                //        d.id_role,
                //        d.first_name AS NAME,
                //        c.category_name AS DDK_Category,
                //        b.id_learning_academy_brief,
                //        b.brief_title AS DDK_Title,
                //        e.id_brief_question,
                //        e.brief_question AS DDK_Question,
                //        f.option_answer AS DDK_Answer,
                //        CASE 
                //            WHEN a.is_correct_answer = 0 THEN 'Wrong_Answer' 
                //            ELSE 'Right Answer' 
                //        END AS Answer_Status,
                //        a.created_at AS Taken_at
                //    FROM 
                //        tbl_learning_academy_answer_log AS a 
                //    INNER JOIN 
                //        tbl_learning_academy_brief AS b ON a.id_learning_academy_brief = b.id_learning_academy_brief 
                //    INNER JOIN 
                //        tbl_learning_academy_category AS c ON b.id_learning_academy_category = c.id_learning_academy_category
                //    INNER JOIN 
                //        tbl_learning_academy_brief_question AS e ON e.id_learning_academy_brief = b.id_learning_academy_brief
                //    INNER JOIN 
                //        tbl_learning_academy_brief_answer AS f ON f.id_brief_answer = a.id_brief_answer
                //    INNER JOIN 
                //        tbl_coroebus_user AS d ON a.id_coroebus_user = d.id_coroebus_user
                //    INNER JOIN 
                //        tbl_coroebus_user AS d1 ON b.id_coroebus_organization = d1.id_coroebus_organization
                //    WHERE  
                //        b.id_coroebus_organization IN ({idAssessmentParams})
                //        AND b.id_learning_academy_brief IN (10284)
                //        AND d.USERID NOT LIKE '%Bata%'
                //) tbl01
                //GROUP BY 
                //    DDK_Category,
                //    DDK_Title,
                //    DDK_Question; ";
                //string query = $@"
                //             SELECT DDK_Category AS ""DDK Category"",
                //        id_learning_academy_brief AS ""DDK Title ID"",
                //        DDK_Title AS ""DDK Title"",
                //        DDK_Question AS ""DDk Question"",
                //        COUNT(CASE WHEN Answer_Status='Right Answer' THEN Answer_Status END) AS ""Correct Answer Count"",
                //        COUNT(CASE WHEN Answer_Status='Wrong Answer' THEN Answer_Status END) AS ""Incorrect Answer Count"",
                //        COUNT(Answer_Status) AS ""Total Answer Count""
                //         FROM (SELECT 
                //        -- a.id_coroebus_user,
                //        d.USERID,
                //        d.id_role,
                //        d.first_name AS NAME,
                //        -- d.id_role,
                //        -- d.id_coroebus_organization,
                //        -- c.id_coroebus_game,
                //        -- c.id_learning_academy_category,
                //        c.category_name AS DDK_Category,
                //         b.id_learning_academy_brief ,
                //        b.brief_title AS DDK_Title,
                //        e.id_brief_question,
                //        e.brief_question AS DDK_Question,
                //        -- f.id_brief_answer,
                //        f.option_answer AS DDK_Answer,
                //        -- a.score,
                //        -- a.is_correct_answer,
                //        CASE WHEN a.is_correct_answer =0 THEN ""Wrong Answer"" ELSE ""Right Answer"" END AS Answer_Status,
                //        a.created_at AS Taken_at
                //        FROM tbl_learning_academy_answer_log AS a 
                //        INNER JOIN tbl_learning_academy_brief AS b 
                //        ON a.id_learning_academy_brief=b.id_learning_academy_brief 
                //        INNER JOIN tbl_learning_academy_category AS c
                //        ON b.id_learning_academy_category=c.id_learning_academy_category
                //        INNER JOIN tbl_learning_academy_brief_question AS e
                //        ON e.id_learning_academy_brief=b.id_learning_academy_brief
                //        INNER JOIN tbl_learning_academy_brief_answer AS f
                //        ON f.id_brief_answer=a.id_brief_answer
                //        INNER JOIN tbl_coroebus_user AS d ON a.id_coroebus_user=d.id_coroebus_user
                //        WHERE  b.id_coroebus_organization IN ({idAssessmentParams})
                //        AND d.userid NOT LIKE '%Bata%') tbl01
                //        GROUP BY id_brief_question;
                //    ";
                //AND a.id_assessment IN ({idAssessmentParams}) 

                using (var cmd = new MySqlCommand(query, connectionCoroebus))
                {

                    for (int i = 0; i < orgIdcoroebus_List.Length; i++)
                    {
                        cmd.Parameters.AddWithValue($"@orgid_coroebus{i}", orgIdcoroebus_List[i]);
                    }

                    for (int i = 0; i < id_learning_academy_brief.Length; i++)
                    {
                        cmd.Parameters.AddWithValue($"@id_learning_academy_brief{i}", id_learning_academy_brief[i]);
                    }

                    using (var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        while (await reader.ReadAsync().ConfigureAwait(false))
                        {
                            var result = new ExpandoObject() as IDictionary<string, object>;
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                result.Add(reader.GetName(i), reader.IsDBNull(i) ? null : reader.GetValue(i));
                            }
                            results.Add(result);
                        }


                    }
                }



                if (results.Count == 0)
                {

                    var noDataMessage = new ExpandoObject() as IDictionary<string, object>;
                    noDataMessage.Add("Message", "No data found for the provided organization IDs.");
                    noDataMessage.Add("Detail", "The query executed successfully but no matching records were found.");
                    results.Add(noDataMessage);
                }

            }
           // await _connectionCoroebus.CloseAsync().ConfigureAwait(false);
            return results;
        }

        //comment out 
        public async Task<IEnumerable<dynamic>> DDKQuestionannotincorbus(string[] orgIdcoroebus_List, string[] id_learning_academy_brief)
        {
            var results = new List<dynamic>();




            var orgid_coroebusParams = string.Join(", ", orgIdcoroebus_List.Select((id, index) => $"@orgid_coroebus{index}"));
            var id_learning_academy_briefParams = string.Join(", ", id_learning_academy_brief.Select((id, index) => $"@id_learning_academy_brief{index}"));


            // await _connectionCoroebus.OpenAsync().ConfigureAwait(false);
            using (var connectionCoroebus = new MySqlConnection(ConfigurationManager.ConnectionStrings["dbconnectionstring"].ConnectionString))
            {
                await connectionCoroebus.OpenAsync().ConfigureAwait(false);
                string query = $@"
    SELECT 
    tbl01.DDK_Category AS ""DDK_Category"",
    tbl01.id_learning_question AS ""DDK_Title_ID"",
    tbl01.DDK_Title AS ""DDK_Title"",
    tbl01.DDK_Question AS ""DDK_Question"",
    COUNT(CASE WHEN tbl01.Answer_Status = 'Right Answer' THEN tbl01.Answer_Status END) AS ""Correct_Answer_Count"",
    COUNT(CASE WHEN tbl01.Answer_Status = 'Wrong Answer' THEN tbl01.Answer_Status END) AS ""Incorrect_Answer_Count"",
    COUNT(tbl01.Answer_Status) AS ""Total_Answer_Count""
FROM (
    SELECT 
        d.USERID,
        d.id_role,
        p.FIRSTNAME AS NAME,
        c.category_name AS DDK_Category,
        a.id_learning_question,
        e.title AS DDK_Title,
        e.question AS DDK_Question,
        f.option_answer AS DDK_Answer,
        CASE WHEN a.is_correct_answer = 0 THEN ""Wrong Answer"" ELSE ""Right Answer"" END AS Answer_Status,
        a.created_date_time AS Taken_at
    FROM tbl_learning_user_log AS a
    INNER JOIN tbl_learning_category AS c
        ON a.id_learning_category = c.id_learning_category
    INNER JOIN tbl_learning_question AS e
        ON e.id_learning_question = a.id_learning_question
    INNER JOIN tbl_learning_question_answer AS f
        ON f.id_learning_question_answer = a.id_learning_question_answer
    INNER JOIN tbl_user AS d
        ON a.ID_USER = d.ID_USER
    INNER JOIN tbl_profile AS p
        ON p.ID_USER = d.ID_USER
    INNER JOIN tbl_department AS b
        ON b.Id_org = a.id_organization
     WHERE 
	   b.Id_department IN ({orgid_coroebusParams})
	   AND  f.id_learning_question IN ({id_learning_academy_briefParams})
       AND d.userid NOT LIKE '%Bata%'
) tbl01
GROUP BY tbl01.id_learning_question;";
           

                using (var cmd = new MySqlCommand(query, connectionCoroebus))
                {

                    for (int i = 0; i < orgIdcoroebus_List.Length; i++)
                    {
                        cmd.Parameters.AddWithValue($"@orgid_coroebus{i}", orgIdcoroebus_List[i]);
                    }

                    for (int i = 0; i < id_learning_academy_brief.Length; i++)
                    {
                        cmd.Parameters.AddWithValue($"@id_learning_academy_brief{i}", id_learning_academy_brief[i]);
                    }

                    using (var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        while (await reader.ReadAsync().ConfigureAwait(false))
                        {
                            var result = new ExpandoObject() as IDictionary<string, object>;
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                result.Add(reader.GetName(i), reader.IsDBNull(i) ? null : reader.GetValue(i));
                            }
                            results.Add(result);
                        }


                    }
                }



                if (results.Count == 0)
                {

                    var noDataMessage = new ExpandoObject() as IDictionary<string, object>;
                    noDataMessage.Add("Message", "No data found for the provided organization IDs.");
                    noDataMessage.Add("Detail", "The query executed successfully but no matching records were found.");
                    results.Add(noDataMessage);
                }

            }
            // await _connectionCoroebus.CloseAsync().ConfigureAwait(false);
            return results;
        }


        //DDK coroebus
        public async Task<IEnumerable<dynamic>> DDKUser(int orgid_userdetails,string[] orgIdcoroebus_List, string[] id_learning_academy_brief, string[] Id_deparment, string[] rolewise_List, string[] regionwise_List, string[] usercity_List,string[] user_designation_List, string startdate_coroebus,string enddate_coroebus)
        {
            var results = new List<dynamic>();

            var orgid_coroebusParams = string.Join(", ", orgIdcoroebus_List.Select((id, index) => $"@orgid_coroebus{index}"));
            var id_learning_academy_briefParams = string.Join(", ", id_learning_academy_brief.Select((id, index) => $"@id_learning_academy_brief{index}"));
            //var rolewise_ListParams = string.Join(", ", rolewise_List.Select((id, index) => $"@rolewise_List{index}"));

            //var idAssessmentList = orgid_coroebus.ToList();
            //var idAssessmentParams = string.Join(", ", idAssessmentList.Select((s, i) => $"@orgid_coroebus{i}"));

            //await _connectionCoroebus.OpenAsync().ConfigureAwait(false);

            using (var connectionCoroebus = new MySqlConnection(ConfigurationManager.ConnectionStrings["db_tgc_corobus"].ConnectionString))
            {
                await connectionCoroebus.OpenAsync().ConfigureAwait(false);

                string query = $@"
                   SELECT 
                    d.USERID,
                    CONCAT(UPPER(LEFT(d.first_name, 1)), LOWER(SUBSTRING(d.first_name, 2))) AS NAME,
                    c.category_name AS DDK_Category,
                    b.id_learning_academy_brief AS ""DDK_Title_ID"",
                    b.brief_title AS DDK_Title,
                    e.brief_question AS DDK_Question,
                    f.id_brief_question,
                    f.option_answer AS DDK_Answer,
                    CASE WHEN a.is_correct_answer = 0 THEN 'Wrong Answer' ELSE 'Right Answer' END AS Answer_Status,
                    a.created_at AS Taken_at
                FROM tbl_learning_academy_answer_log AS a 
                INNER JOIN tbl_learning_academy_brief AS b 
                    ON a.id_learning_academy_brief = b.id_learning_academy_brief 
                INNER JOIN tbl_learning_academy_category AS c
                    ON b.id_learning_academy_category = c.id_learning_academy_category
                INNER JOIN tbl_learning_academy_brief_question AS e
                    ON e.id_learning_academy_brief = b.id_learning_academy_brief
                INNER JOIN tbl_learning_academy_brief_answer AS f
                    ON f.id_brief_answer = a.id_brief_answer
                INNER JOIN tbl_coroebus_user AS d 
                    ON a.id_coroebus_user = d.id_coroebus_user
                WHERE 
                    b.id_coroebus_organization IN ({orgid_coroebusParams})
                    
                    AND d.userid NOT LIKE '%Bata%'
                    -- AND MONTH(a.created_at) = 6
                    AND a.created_at BETWEEN '@startdate_coroebus' AND '@enddate_coroebus'
                    AND f.id_brief_question IN ({id_learning_academy_briefParams}); ";


                using (var cmd = new MySqlCommand(query, connectionCoroebus))
                {

                    for (int i = 0; i < orgIdcoroebus_List.Length; i++)
                    {
                        cmd.Parameters.AddWithValue($"@orgid_coroebusParams{i}", orgIdcoroebus_List[i]);
                    }

                    for (int i = 0; i < id_learning_academy_brief.Length; i++)
                    {
                        cmd.Parameters.AddWithValue($"@id_learning_academy_briefParams{i}", id_learning_academy_brief[i]);
                    }
                    cmd.Parameters.AddWithValue("@startdate_coroebus", startdate_coroebus);
                    cmd.Parameters.AddWithValue("@enddate_coroebus", enddate_coroebus);

                    using (var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        while (await reader.ReadAsync().ConfigureAwait(false))
                        {
                            var result = new ExpandoObject() as IDictionary<string, object>;
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                result.Add(reader.GetName(i), reader.IsDBNull(i) ? null : reader.GetValue(i));
                            }
                            results.Add(result);
                        }


                    }
                    if (results.Count == 0)
                    {

                        var noDataMessage = new ExpandoObject() as IDictionary<string, object>;
                        noDataMessage.Add("Message", "No data found for the provided organization IDs.");
                        noDataMessage.Add("Detail", "The query executed successfully but no matching records were found.");
                        results.Add(noDataMessage);
                    }

                }
            }
           // await _connectionCoroebus.CloseAsync().ConfigureAwait(false);
            return results;

        }
        //
        public async Task<IEnumerable<dynamic>> DDKUser1(int orgid_userdetails, string[] orgIdcoroebus_List, string[] id_learning_academy_brief, string[] Id_deparment, string[] rolewise_List, string[] regionwise_List, string[] usercity_List, string[] user_designation_List, string startdate_coroebus, string enddate_coroebus)
        {
            //var results = new List<dynamic>();
            DataTable results = new DataTable();
            DataTable results1 = new DataTable();
            var error = new List<dynamic>();
            var combinedResults = new List<dynamic>();


            var orgid_coroebusParams = string.Join(", ", orgIdcoroebus_List.Select((id, index) => $"@orgid_coroebus{index}"));
            var id_learning_academy_briefParams = string.Join(", ", id_learning_academy_brief.Select((id, index) => $"@id_learning_academy_brief{index}"));

            using (var connectionCoroebus = new MySqlConnection(ConfigurationManager.ConnectionStrings["db_tgc_corobus"].ConnectionString))
            {
                await connectionCoroebus.OpenAsync().ConfigureAwait(false);

                //await _connectionCoroebus.OpenAsync().ConfigureAwait(false);
                string query = $@"
        SELECT 
            d.USERID,
            CONCAT(UPPER(LEFT(d.first_name, 1)), LOWER(SUBSTRING(d.first_name, 2))) AS NAME,
            c.category_name AS DDK_Category,
            b.id_learning_academy_brief AS DDK_Title_ID,
            b.brief_title AS DDK_Title,
            e.brief_question AS DDK_Question,
            f.id_brief_question,
            f.option_answer AS DDK_Answer,
            CASE WHEN a.is_correct_answer = 0 THEN 'Wrong Answer' ELSE 'Right Answer' END AS Answer_Status,
            a.created_at AS Taken_at
        FROM tbl_learning_academy_answer_log AS a 
        INNER JOIN tbl_learning_academy_brief AS b 
            ON a.id_learning_academy_brief = b.id_learning_academy_brief 
        INNER JOIN tbl_learning_academy_category AS c
            ON b.id_learning_academy_category = c.id_learning_academy_category
        INNER JOIN tbl_learning_academy_brief_question AS e
            ON e.id_learning_academy_brief = b.id_learning_academy_brief
        INNER JOIN tbl_learning_academy_brief_answer AS f
            ON f.id_brief_answer = a.id_brief_answer
        INNER JOIN tbl_coroebus_user AS d 
            ON a.id_coroebus_user = d.id_coroebus_user
        WHERE 
            b.id_coroebus_organization IN ({orgid_coroebusParams})
            AND d.userid NOT LIKE '%Bata%'
            AND a.created_at BETWEEN @startdate_coroebus AND @enddate_coroebus
            AND f.id_brief_question IN ({id_learning_academy_briefParams}); ";

                using (var cmd = new MySqlCommand(query, connectionCoroebus))
                {
                    for (int i = 0; i < orgIdcoroebus_List.Length; i++)
                    {
                        cmd.Parameters.AddWithValue($"@orgid_coroebus{i}", orgIdcoroebus_List[i]);
                    }

                    for (int i = 0; i < id_learning_academy_brief.Length; i++)
                    {
                        cmd.Parameters.AddWithValue($"@id_learning_academy_brief{i}", id_learning_academy_brief[i]);
                    }

                    cmd.Parameters.AddWithValue("@startdate_coroebus", startdate_coroebus);
                    cmd.Parameters.AddWithValue("@enddate_coroebus", enddate_coroebus);

                    using (var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        //results.Load(reader);   
                        //while (await reader.ReadAsync().ConfigureAwait(false))
                        //{
                        //    var result = new Dictionary<string, object>();
                        //    for (int i = 0; i < reader.FieldCount; i++)
                        //    {
                        //        result.Add(reader.GetName(i), reader.IsDBNull(i) ? null : reader.GetValue(i));
                        //    }
                        //    results.Add(result);
                        //}
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            results.Columns.Add(reader.GetName(i), reader.GetFieldType(i));
                        }

                        // Load DataTable with rows
                        while (await reader.ReadAsync().ConfigureAwait(false))
                        {
                            var row = results.NewRow();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                row[i] = reader.IsDBNull(i) ? DBNull.Value : reader.GetValue(i);
                            }
                            results.Rows.Add(row);
                        }
                    }
                }

                //await _connectionCoroebus.CloseAsync().ConfigureAwait(false);

                if (results.Rows.Count == 0)
                {

                    var noDataMessage = new ExpandoObject() as IDictionary<string, object>;
                    noDataMessage.Add("Message", "No data found for the provided organization IDs.");
                    noDataMessage.Add("Detail", "The query executed successfully but no matching records were found.");
                    error.Add(noDataMessage);
                    return error;

                }
                else
                {
                    var userDetails = await UserDetails1(orgid_userdetails, Id_deparment, rolewise_List, regionwise_List, usercity_List, user_designation_List).ConfigureAwait(false);
                    results1 = userDetails;

                    var userDetailsDict = results1.AsEnumerable()
                                           .GroupBy(row => row["userid"].ToString())
                                           .ToDictionary(group => group.Key, group => group.First());

                    var finalResults = new List<dynamic>();
                    foreach (DataRow row in results.Rows)
                    {
                        var userId = row["UserId"].ToString();
                        if (userDetailsDict.TryGetValue(userId, out DataRow matchedRow))
                        {
                            var combinedResult = new ExpandoObject() as IDictionary<string, object>;
                            foreach (DataColumn column in results.Columns)
                            {
                                combinedResult.Add(column.ColumnName, row[column.ColumnName]);
                            }
                            foreach (DataColumn column in results1.Columns)
                            {
                                combinedResult.Add(column.ColumnName, matchedRow[column.ColumnName]);
                            }
                            combinedResults.Add(combinedResult);
                        }
                    }
                    if (combinedResults.Count == 0)
                    {
                        var noDataMessage = new ExpandoObject() as IDictionary<string, object>;
                        noDataMessage.Add("Message", "No data found for the provided parameters.");
                        error.Add(noDataMessage);
                        return error;
                    }

                    return combinedResults;



                }
            }
        }

        //not corbus
        public async Task<IEnumerable<dynamic>> DDKUsernotcorobus(int orgid_userdetails, string[] id_learning_academy_brief, string[] Id_deparment, string[] rolewise_List, string[] regionwise_List, string[] usercity_List, string[] user_designation_List, string startdate_coroebus, string enddate_coroebus)
        {
            //var results = new List<dynamic>();
            DataTable results = new DataTable();
            DataTable results1 = new DataTable();
            var error = new List<dynamic>();
            var combinedResults = new List<dynamic>();

            //comment out
            //var orgid_coroebusParams = string.Join(", ", orgIdcoroebus_List.Select((id, index) => $"@orgid_coroebus{index}"));

            var id_learning_academy_briefParams = string.Join(", ", id_learning_academy_brief.Select((id, index) => $"@id_learning_academy_brief{index}"));

            using (var connectionCoroebus = new MySqlConnection(ConfigurationManager.ConnectionStrings["dbconnectionstring"].ConnectionString))
            {
                await connectionCoroebus.OpenAsync().ConfigureAwait(false);

                //await _connectionCoroebus.OpenAsync().ConfigureAwait(false);
                string query = $@"
  SELECT 
       d.USERID,
      CONCAT(UPPER(LEFT(p.FIRSTNAME, 1)), LOWER(SUBSTRING(p.LASTNAME, 2))) AS NAME,
       c.category_name AS DDK_Category,
       e.id_learning_question as DDK_Title_ID,
       e.title AS DDK_Title,
       e.question AS DDK_Question,
       f.id_learning_question,
       f.option_answer AS DDK_Answer,
       CASE WHEN a.is_correct_answer = 0 THEN 'Wrong Answer' ELSE 'Right Answer' END AS Answer_Status,
       a.created_date_time AS Taken_at
   FROM tbl_learning_user_log AS a 
   INNER JOIN tbl_learning_category AS c
       ON a.id_learning_category = c.id_learning_category
   INNER JOIN tbl_learning_question AS e
       ON e.id_learning_question = a.id_learning_question
       INNER JOIN tbl_learning_question_answer AS f
       ON f.id_learning_question_answer = a.id_learning_question_answer
   INNER JOIN tbl_user AS d 
       ON a.ID_USER = d.ID_USER
       INNER JOIN tbl_profile AS p 
       ON p.ID_USER = d.ID_USER
   WHERE 
       d.userid NOT LIKE '%Bata%'
      AND a.created_date_time BETWEEN @startdate_coroebus AND @enddate_coroebus
       AND f.id_learning_question IN ({id_learning_academy_briefParams});";

                using (var cmd = new MySqlCommand(query, connectionCoroebus))
                {
                    //for (int i = 0; i < orgIdcoroebus_List.Length; i++)
                    //{
                    //    cmd.Parameters.AddWithValue($"@orgid_coroebus{i}", orgIdcoroebus_List[i]);
                    //}

                    for (int i = 0; i < id_learning_academy_brief.Length; i++)
                    {
                        cmd.Parameters.AddWithValue($"@id_learning_academy_brief{i}", id_learning_academy_brief[i]);
                    }

                    cmd.Parameters.AddWithValue("@startdate_coroebus", startdate_coroebus);
                    cmd.Parameters.AddWithValue("@enddate_coroebus", enddate_coroebus);

                    using (var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        //results.Load(reader);   
                        //while (await reader.ReadAsync().ConfigureAwait(false))
                        //{
                        //    var result = new Dictionary<string, object>();
                        //    for (int i = 0; i < reader.FieldCount; i++)
                        //    {
                        //        result.Add(reader.GetName(i), reader.IsDBNull(i) ? null : reader.GetValue(i));
                        //    }
                        //    results.Add(result);
                        //}
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            results.Columns.Add(reader.GetName(i), reader.GetFieldType(i));
                        }

                        // Load DataTable with rows
                        while (await reader.ReadAsync().ConfigureAwait(false))
                        {
                            var row = results.NewRow();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                row[i] = reader.IsDBNull(i) ? DBNull.Value : reader.GetValue(i);
                            }
                            results.Rows.Add(row);
                        }
                    }
                }

                //await _connectionCoroebus.CloseAsync().ConfigureAwait(false);

                if (results.Rows.Count == 0)
                {

                    var noDataMessage = new ExpandoObject() as IDictionary<string, object>;
                    noDataMessage.Add("Message", "No data found for the provided organization IDs.");
                    noDataMessage.Add("Detail", "The query executed successfully but no matching records were found.");
                    error.Add(noDataMessage);
                    return error;

                }
                else
                {
                    var userDetails = await UserDetails1(orgid_userdetails, Id_deparment, rolewise_List, regionwise_List, usercity_List, user_designation_List).ConfigureAwait(false);
                    results1 = userDetails;

                    var userDetailsDict = results1.AsEnumerable()
                                           .GroupBy(row => row["userid"].ToString())
                                           .ToDictionary(group => group.Key, group => group.First());

                    var finalResults = new List<dynamic>();
                    foreach (DataRow row in results.Rows)
                    {
                        var userId = row["UserId"].ToString();
                        if (userDetailsDict.TryGetValue(userId, out DataRow matchedRow))
                        {
                            var combinedResult = new ExpandoObject() as IDictionary<string, object>;
                            foreach (DataColumn column in results.Columns)
                            {
                                combinedResult.Add(column.ColumnName, row[column.ColumnName]);
                            }
                            foreach (DataColumn column in results1.Columns)
                            {
                                combinedResult.Add(column.ColumnName, matchedRow[column.ColumnName]);
                            }
                            combinedResults.Add(combinedResult);
                        }
                    }
                    if (combinedResults.Count == 0)
                    {
                        var noDataMessage = new ExpandoObject() as IDictionary<string, object>;
                        noDataMessage.Add("Message", "No data found for the provided parameters.");
                        error.Add(noDataMessage);
                        return error;
                    }

                    return combinedResults;



                }
            }
        }



        public async Task<IEnumerable<dynamic>> DDKUser1(IEnumerable<int> orgid_coroebus)
        {
            var results = new List<dynamic>();
            var idAssessmentList = orgid_coroebus.ToList();
            var idAssessmentParams = string.Join(", ", idAssessmentList.Select((s, i) => $"@orgid_coroebus{i}"));

            try
            {
                using (var connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["db_tgc_corobus"].ConnectionString))
                {
                    await connection.OpenAsync().ConfigureAwait(false);

                    string query = $@"
                    SELECT 
                    d.USERID,
                    d.first_name AS NAME,
                    d.user_function as REGION,
                    c.category_name AS DDK_Category,
                    b.id_learning_academy_brief AS `DDK Title ID`,
                    b.brief_title AS DDK_Title,
                    e.brief_question AS DDK_Question,
                    f.option_answer AS DDK_Answer,
                    CASE WHEN a.is_correct_answer = 0 THEN 'Wrong Answer' ELSE 'Right Answer' END AS Answer_Status,
                    a.created_at AS Taken_at
                FROM tbl_learning_academy_answer_log AS a 
                INNER JOIN tbl_learning_academy_brief AS b 
                    ON a.id_learning_academy_brief = b.id_learning_academy_brief 
                INNER JOIN tbl_learning_academy_category AS c
                    ON b.id_learning_academy_category = c.id_learning_academy_category
                INNER JOIN tbl_learning_academy_brief_question AS e
                    ON e.id_learning_academy_brief = b.id_learning_academy_brief
                INNER JOIN tbl_learning_academy_brief_answer AS f
                    ON f.id_brief_answer = a.id_brief_answer
                INNER JOIN tbl_coroebus_user AS d 
                    ON a.id_coroebus_user = d.id_coroebus_user
                WHERE b.id_coroebus_organization IN ({idAssessmentParams})
                    AND d.userid NOT LIKE '%Bata%';
            ";

                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        for (int i = 0; i < idAssessmentList.Count; i++)
                        {
                            cmd.Parameters.AddWithValue($"@orgid_coroebus{i}", idAssessmentList[i]);
                        }

                        using (var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                        {
                            while (await reader.ReadAsync().ConfigureAwait(false))
                            {
                                var result = new ExpandoObject() as IDictionary<string, object>;
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    result.Add(reader.GetName(i), reader.IsDBNull(i) ? null : reader.GetValue(i));
                                }
                                results.Add(result);
                            }
                        }

                        if (results.Count == 0)
                        {

                            var noDataMessage = new ExpandoObject() as IDictionary<string, object>;
                            noDataMessage.Add("Message", "No data found for the provided.");

                            results.Add(noDataMessage);
                        }

                    }
                }
            }
            catch (Exception ex)
            {

                var errorResponse = new ErrorResponse
                {
                    Message = "An unexpected error occurred.",
                    Detail = ex.Message
                };

                results.Add(errorResponse);
            }

            return results;
        }


        //Skillmuni Drodown
        public List<tbl_assessment_title> tbl_assessment_title(int orgid, string id_category_heading)
        {
            var id_category_headings = id_category_heading.Split(',').Select(id => id.Trim()).ToArray();
            var id_category_headingParams = string.Join(", ", id_category_headings.Select((_, i) => $"@id_category_heading{i}"));

            List<tbl_assessment_title> list = new List<tbl_assessment_title>();

            try
            {
                using (var connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["dbconnectionstring"].ConnectionString))
                {
                    connection.Open();
                    //_connection.Open();

                    string query = $@"SELECT DISTINCT a.id_assessment, b.assessment_title,a.id_category
                      FROM tbl_assessment_user_assignment a
                      JOIN tbl_assessment b ON a.id_assessment = b.id_assessment
                      WHERE a.id_organization = @orgid
                      AND b.status ='A'
                      AND a.id_category IN ({id_category_headingParams})
                      ORDER BY FIELD(a.id_category, {id_category_headingParams});";

                    //    string query = $@"
                    //SELECT DISTINCT c.id_assessment, c.assessment_title
                    //FROM tbl_content_program_mapping a
                    //JOIN tbl_assessment_categoty_mapping b ON a.ID_CATEGORY = b.id_category
                    //JOIN tbl_assessment c ON b.id_assessment = c.id_assessment
                    //WHERE a.ID_ORGANIZATION = @orgid 
                    //  AND a.STATUS = 'A'
                    //  AND a.id_category IN ({id_category_headingParams});";

                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@orgid", orgid);
                        for (int i = 0; i < id_category_headings.Length; i++)
                        {
                            cmd.Parameters.AddWithValue($"@id_category_heading{i}", id_category_headings[i]);
                        }

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                tbl_assessment_title orgItem = new tbl_assessment_title
                                {
                                    Id_assessment = reader["id_assessment"].ToString(),
                                    Assessment_title = reader["assessment_title"].ToString(),
                                    id_category = reader["id_category"].ToString()
                                };

                                list.Add(orgItem);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle the exception (log it, throw it, etc.)
                // Log the exception or rethrow it as needed
            }
            finally
            {
                _connection.Close();
            }

            return list;
        }



        //Skillmuni Drodown
        public List<tbl_category> tbl_category(int orgid, string id_category_heading,string year,string month)
        {
            var id_category_headings = id_category_heading.Split(',').Select(id => id.Trim()).ToArray();
            var id_category_headingParams = string.Join(", ", id_category_headings.Select((_, i) => $"@id_category_heading{i}"));

            List<tbl_category> list = new List<tbl_category>();

            try
            {
                using (var connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["dbconnectionstring"].ConnectionString))
                {
                    connection.Open();
                    //_connection.Open();

                    string query = $@"
            SELECT DISTINCT a.id_category, b.CATEGORYNAME, c.Heading_title, a.id_category_heading 
            FROM tbl_content_program_mapping a 
            LEFT JOIN tbl_category b ON a.ID_CATEGORY = b.ID_CATEGORY
            LEFT JOIN tbl_category_heading c ON a.id_category_heading = c.id_category_heading
            WHERE a.ID_ORGANIZATION = @orgid 
            AND a.STATUS = 'A' 
            AND c.id_category_heading IN ({id_category_headingParams}) AND  year(a.UPDATED_DATE_TIME) in (@year) and MONTH(a.UPDATED_DATE_TIME) IN(@month);";

                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@orgid", orgid);
                        cmd.Parameters.AddWithValue("@year", year);
                        cmd.Parameters.AddWithValue("@month", month);
                        for (int i = 0; i < id_category_headings.Length; i++)
                        {
                            cmd.Parameters.AddWithValue($"@id_category_heading{i}", id_category_headings[i]);
                        }

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                tbl_category orgItem = new tbl_category
                                {
                                    ID_CATEGORY = Convert.ToInt32(reader["id_category"]),
                                    CATEGORYNAME = reader["CATEGORYNAME"].ToString(),
                                    // Add more fields here if needed
                                };

                                list.Add(orgItem);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle the exception (log it, throw it, etc.)
                // You can log the exception or rethrow it
            }
            finally
            {
                _connection.Close();
            }

            return list;
        }





        //For PYP Dropdown
        public List<tbl_categorypyp> tbl_categorypyp(int orgid, string id_category_heading, string year, string month)
        {
            var id_category_headings = id_category_heading.Split(',').Select(id => id.Trim()).ToArray();
            var id_category_headingParams = string.Join(", ", id_category_headings.Select((_, i) => $"@id_category_heading{i}"));

            List<tbl_categorypyp> list = new List<tbl_categorypyp>();

            try
            {
                using (var connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["dbconnectionstring"].ConnectionString))
                {
                    connection.Open();
                    //_connection.Open();

                    string query = $@"
            SELECT DISTINCT a.id_category, b.CATEGORYNAME, c.Heading_title, a.id_category_heading 
            FROM tbl_content_program_mapping a 
            LEFT JOIN tbl_category b ON a.ID_CATEGORY = b.ID_CATEGORY
            LEFT JOIN tbl_category_heading c ON a.id_category_heading = c.id_category_heading
            WHERE a.ID_ORGANIZATION = @orgid 
            AND a.STATUS = 'A' 
            AND c.id_category_heading IN ({id_category_headingParams}) AND  year(a.UPDATED_DATE_TIME) in (@year) and MONTH(a.UPDATED_DATE_TIME) IN(@month);";

                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@orgid", orgid);
                        cmd.Parameters.AddWithValue("@year", year);
                        cmd.Parameters.AddWithValue("@month", month);
                        for (int i = 0; i < id_category_headings.Length; i++)
                        {
                            cmd.Parameters.AddWithValue($"@id_category_heading{i}", id_category_headings[i]);
                        }

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                tbl_categorypyp orgItempyp = new tbl_categorypyp
                                {
                                    Text = reader["CATEGORYNAME"].ToString(),
                                    CATEGORYNAME = reader["CATEGORYNAME"].ToString(),
                                    // Add more fields here if needed
                                };

                                list.Add(orgItempyp);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle the exception (log it, throw it, etc.)
                // You can log the exception or rethrow it
            }
            finally
            {
                _connection.Close();
            }

            return list;
        }




        //Skillmuni
        public async Task<IEnumerable<dynamic>> UserDetails(int orgid)
        {
            var results = new List<dynamic>();


            try
            {
                //if (_connection.State != System.Data.ConnectionState.Open)
                //{
                //    await _connection.OpenAsync().ConfigureAwait(false);
                //}

                using (var connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["dbconnectionstring"].ConnectionString))
                {
                    await connection.OpenAsync().ConfigureAwait(false);


                    string query = $@"
SELECT 
    a.userid,
    a.UPDATEDTIME,
    g.date_of_joining,
    a.user_department,
    a.user_designation,
    a.user_function,
    g.gender,
    g.city,
    g.office_address,
    a.user_grade AS Store_code,
    a.L4,
    a.L3,
    a.L2,
    a.L1,
    a.Spectator,
    h.Location AS Store_Name,
    a.status
FROM 
    tbl_user a
LEFT JOIN tbl_user b ON a.reporting_manager = b.id_user
LEFT JOIN tbl_csst_role c ON a.id_role = c.id_csst_role
LEFT JOIN tbl_profile g ON a.id_user = g.id_user
LEFT JOIN tbl_user d ON b.reporting_manager = d.id_user
LEFT JOIN tbl_profile h ON b.id_user = h.id_user
LEFT JOIN tbl_user e ON d.reporting_manager = e.id_user
LEFT JOIN tbl_profile i ON d.id_user = i.id_user
LEFT JOIN tbl_user f ON e.reporting_manager = f.id_user
LEFT JOIN tbl_profile j ON e.id_user = j.id_user
LEFT JOIN tbl_profile k ON f.id_user = k.id_user
WHERE 
    a.id_organization = @orgid 
    AND a.userid NOT LIKE '%-old%'
    AND b.userid NOT LIKE '%Bata%'
";

                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@orgid", orgid);


                        using (var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                        {
                            while (await reader.ReadAsync().ConfigureAwait(false))
                            {
                                var result = new ExpandoObject() as IDictionary<string, object>;
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    result.Add(reader.GetName(i), reader.IsDBNull(i) ? null : reader.GetValue(i));
                                }
                                results.Add(result);
                            }
                        }

                        if (results.Count == 0)
                        {
                            var noDataMessage = new ExpandoObject() as IDictionary<string, object>;
                            noDataMessage.Add("Message", "No data found for the provided.");
                            results.Add(noDataMessage);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exception (log it, rethrow it, or return a custom error message)
                throw new Exception("An error occurred while fetching the assessment report.", ex);
            }
            finally
            {
                //if (_connectionNgage.State == System.Data.ConnectionState.Open)
                //{
                //    await _connectionNgage.CloseAsync().ConfigureAwait(false);
                //}
            }

            return results;
        }


        //Skillmuni 
        public List<object> Getorgidcoroebus(string[] orgid_coroebus)
        {
            List<object> list = new List<object>();

            try
            {
                using (var connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["dbconnectionstring"].ConnectionString))
                {
                    connection.Open();
                    //_connection.Open();


                    // Create strings with parameter placeholders
                    var orgidParameters = string.Join(",", orgid_coroebus.Select((_, index) => $"@orgid_coroebus{index}"));

                    string query = $@"SELECT orgid_coroebus from tbl_department WHERE Id_department IN ({orgidParameters})";


                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        // Add orgid parameters to the command
                        for (int i = 0; i < orgid_coroebus.Length; i++)
                        {
                            cmd.Parameters.AddWithValue($"@orgid_coroebus{i}", orgid_coroebus[i]);
                        }

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                list.Add(reader["orgid_coroebus"].ToString());
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                // Handle the exception (log it, throw it, etc.)
                Console.WriteLine("An error occurred: " + ex.Message); // For example purposes
            }
            finally
            {
                _connection.Close(); // Ensure this is the correct connection variable
            }

            return list;
        }

        public List<object> Getorgidcoroebus1(string orgid_coroebus)
        {
            List<object> list = new List<object>();

            try
            {
                using (var connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["dbconnectionstring"].ConnectionString))
                {
                    connection.Open();
                    // _connection.Open();


                    // Create strings with parameter placeholders
                    var orgidParameters = string.Join(",", orgid_coroebus.Select((_, index) => $"@orgid_coroebus{index}"));

                    string query = $@"SELECT orgid_coroebus from tbl_department WHERE Id_department IN ({orgidParameters})";


                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        // Add orgid parameters to the command
                        for (int i = 0; i < orgid_coroebus.Length; i++)
                        {
                            cmd.Parameters.AddWithValue($"@orgid_coroebus{i}", orgid_coroebus[i]);
                        }

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                list.Add(reader["orgid_coroebus"].ToString());
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                // Handle the exception (log it, throw it, etc.)
                Console.WriteLine("An error occurred: " + ex.Message); // For example purposes
            }
            finally
            {
                _connection.Close(); // Ensure this is the correct connection variable
            }

            return list;
        }



        //Skillmuni
        //public async Task<DataTable> UserDetails1(int orgid,string[] Id_deparment, string[] rolewise,string[] regionwise,string[] usercity)
        //{
        //    DataTable results = new DataTable();

        //    var Id_deparments = Id_deparment.Split(',').Select(id => id.Trim()).ToArray();
        //    var Id_deparmentParams = string.Join(", ", Id_deparments.Select((s, i) => $"@Id_deparment{i}"));

        //    var rolewises = rolewise.Split(',').Select(id => id.Trim()).ToArray();
        //    var rolewiseParams = string.Join(", ", rolewises.Select((s, i) => $"@rolewise{i}"));

        //    string inputString = regionwise;

        //    // Split the string into a list of values
        //    string[] valuesArray = inputString.Split(',');

        //    // Join the values into a format suitable for the SQL query
        //    string regionwiseValues = "'" + string.Join("', '", valuesArray) + "'";

        //    string inputcity = usercity;

        //    // Split the string into a list of values
        //    string[] cityArray = inputcity.Split(',');

        //    // Join the values into a format suitable for the SQL query
        //    string cityValues1 = "'" + string.Join("', '", cityArray) + "'";

        //    try
        //    {
        //        if (_connection.State != System.Data.ConnectionState.Open)
        //        {
        //            await _connection.OpenAsync().ConfigureAwait(false);
        //        }



        //        string query = $@"
        //        SELECT 
        //        a.userid,
        //        a.UPDATEDTIME,
        //        g.date_of_joining,
        //        a.user_department,
        //        a.user_designation,
        //        a.user_function,
        //        g.gender,
        //        g.city,
        //        g.office_address,
        //        a.user_grade AS Store_code,
        //        h.Location AS Store_Name,
        //        a.status, 
        //        o.Department_name,
        //        c.csst_role AS Role
        //    FROM 
        //        tbl_user a
        //    LEFT JOIN tbl_user b ON a.reporting_manager = b.id_user
        //    LEFT JOIN tbl_csst_role c ON a.id_role = c.id_csst_role
        //    LEFT JOIN tbl_profile g ON a.id_user = g.id_user
        //    LEFT JOIN tbl_user d ON b.reporting_manager = d.id_user
        //    LEFT JOIN tbl_profile h ON b.id_user = h.id_user
        //    LEFT JOIN tbl_user e ON d.reporting_manager = e.id_user
        //    LEFT JOIN tbl_profile i ON d.id_user = i.id_user
        //    LEFT JOIN tbl_user f ON e.reporting_manager = f.id_user
        //    LEFT JOIN tbl_profile j ON e.id_user = j.id_user
        //    LEFT JOIN tbl_profile k ON f.id_user = k.id_user
        //    JOIN tbl_department o ON c.id_department = o.id_department
        //    WHERE 
        //        a.id_organization = @orgid 
        //        AND o.id_department IN ({Id_deparmentParams})
        //        AND c.id_csst_role IN ({rolewiseParams})
        //        AND j.OFFICE_ADDRESS IN ({regionwiseValues})
        //        AND j.CITY IN ({ cityValues1 })
        //        AND a.userid NOT LIKE '%-old%'
        //        AND b.userid NOT LIKE '%Bata%'
        //        ";

        //        using (var cmd = new MySqlCommand(query, _connection))
        //        {
        //            cmd.Parameters.AddWithValue("@orgid", orgid);

        //            for (int i = 0; i < Id_deparments.Length; i++)
        //            {
        //                cmd.Parameters.AddWithValue($"@Id_deparment{i}", Id_deparments[i]);
        //            }

        //            for (int i = 0; i < rolewises.Length; i++)
        //            {
        //                cmd.Parameters.AddWithValue($"@rolewise{i}", rolewises[i]);
        //            }

        //            using (var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
        //            {
        //                // Load data into DataTable directly from the reader
        //                results.Load(reader);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // Handle exception (log it, rethrow it, or return a custom error message)
        //        throw new Exception("An error occurred while fetching user details.", ex);
        //    }

        //    return results;
        //}

        //userwise
        public async Task<DataTable> UserDetails1(
          int orgid,
          string[] Id_department,
          string[] rolewise,
          string[] regionwise,
          string[] usercity,
          string[] user_designation)
        {
            DataTable results = new DataTable();

            // MySQL connection string
            string connectionString = ConfigurationManager.ConnectionStrings["dbconnectionstring"].ConnectionString;

            // Base SQL query with placeholders


            string query = @"SELECT 
    a.userid,
    a.UPDATEDTIME,
    g.date_of_joining,
    a.user_department,
    a.user_designation,
    a.user_function,
    g.gender,
    g.city,
    g.office_address,
    a.user_grade AS Store_code,
    a.L4,
    a.L3,
    a.L2,
    a.L1,
    a.Spectator,
    g.Location AS Store_Name,
    a.status, 
    o.Department_name,
    c.csst_role AS Role
FROM 
    tbl_user a
LEFT JOIN tbl_user b ON a.reporting_manager = b.id_user
LEFT JOIN tbl_csst_role c ON a.id_role = c.id_csst_role
LEFT JOIN tbl_profile g ON a.id_user = g.id_user
LEFT JOIN tbl_user d ON b.reporting_manager = d.id_user
LEFT JOIN tbl_profile h ON b.id_user = h.id_user
LEFT JOIN tbl_user e ON d.reporting_manager = e.id_user
LEFT JOIN tbl_profile i ON d.id_user = i.id_user
LEFT JOIN tbl_user f ON e.reporting_manager = f.id_user
LEFT JOIN tbl_profile j ON e.id_user = j.id_user 
    AND c.id_csst_role IN ({rolewiseParams})
    AND j.OFFICE_ADDRESS IN ({regionwiseParams})
    AND j.CITY  IN ({userCityParams})
LEFT JOIN tbl_profile k ON f.id_user = k.id_user
JOIN tbl_department o ON c.id_department = o.id_department
WHERE 
    a.id_organization = @orgid
    AND o.id_department IN ({Id_departmentParams})
    AND c.id_csst_role IN ({rolewiseParams})
    AND a.user_designation IN ({userDesignationParams})
    AND a.userid NOT LIKE '%-old%'
    AND b.userid NOT LIKE '%Bata%'
    AND b.user_status = 'A'";

            // Replace placeholders with dynamic parameter names
            query = query
                .Replace("{Id_departmentParams}", string.Join(", ", Id_department.Select((_, i) => $"@Id_department{i}")))
                .Replace("{rolewiseParams}", string.Join(", ", rolewise.Select((_, i) => $"@rolewise{i}")))
                .Replace("{regionwiseParams}", string.Join(", ", regionwise.Select((_, i) => $"@regionwise{i}")))
                .Replace("{userCityParams}", string.Join(", ", usercity.Select((_, i) => $"@userCity{i}")))
                .Replace("{userDesignationParams}", string.Join(", ", user_designation.Select((_, i) => $"@userDesignation{i}")));

            using (var connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (var cmd = new MySqlCommand(query, connection))
                {
                    // Add parameters dynamically
                    cmd.Parameters.AddWithValue("@orgid", orgid);

                    AddParameters(cmd, "Id_department", Id_department);
                    AddParameters(cmd, "rolewise", rolewise);
                    AddParameters(cmd, "regionwise", regionwise);
                    AddParameters(cmd, "userCity", usercity);
                    AddParameters(cmd, "userDesignation", user_designation);

                    // Execute and load results
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        results.Load(reader);
                    }
                }
            }

            return results;
        }

        // Helper method to add array parameters
        private void AddParameters(MySqlCommand cmd, string paramNameBase, string[] values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                cmd.Parameters.AddWithValue($"@{paramNameBase}{i}", values[i]);
            }
        }



        //        public async Task<DataTable> UserDetails1(int orgid, string[] Id_deparment, string[] rolewise, string[] regionwise, string[] usercity, string[] user_designation)
        //        {
        //            DataTable results = new DataTable();

        //            var Id_deparmentParams = string.Join(", ", Id_deparment.Select((id, i) => $"@Id_deparment{i}"));
        //            var rolewiseParams = string.Join(", ", rolewise.Select((id, i) => $"@rolewise{i}"));

        //            var regionwiseArray = regionwise.SelectMany(r => r.Split(',').Select(region => $"'{region.Trim()}'")).ToArray();

        //            string regionwiseArrayValues = "" + string.Join(",", regionwiseArray) + ""; //string

        //            var usercityArray = usercity.Select(city => $"'{city.Trim()}'").ToArray();

        //            string usercityArrayValues = "" + string.Join(",", usercityArray) + ""; //string


        //            var user_designationArray = user_designation.Select(userDesig => $"'{userDesig.Trim()}'").ToArray();

        //            string user_designationArrayValues = "" + string.Join(",", user_designationArray) + ""; //string


        //            try
        //            {
        //                //if (_connection.State != System.Data.ConnectionState.Open)
        //                //{
        //                //    await _connection.OpenAsync().ConfigureAwait(false);
        //                //}

        //                //test
        //                using (var connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["dbconnectionstring"].ConnectionString))
        //                {
        //                    await connection.OpenAsync().ConfigureAwait(false);

        //                    //            string query = $@"
        //                    //    SELECT 
        //                    //        a.userid,
        //                    //        a.UPDATEDTIME,
        //                    //        g.date_of_joining,
        //                    //        a.user_department,
        //                    //        a.user_designation,
        //                    //        a.user_function,
        //                    //        g.gender,
        //                    //        g.city,
        //                    //        g.office_address,
        //                    //        a.user_grade AS Store_code,
        //                    //        a.L4,
        //                    //        a.L3,
        //                    //        a.L2,
        //                    //        a.L1,
        //                    //        a.Spectator,
        //                    //        g.Location AS Store_Name,
        //                    //        a.status, 
        //                    //        o.Department_name,
        //                    //        c.csst_role AS Role
        //                    //    FROM 
        //                    //        tbl_user a
        //                    //    LEFT JOIN tbl_user b ON a.reporting_manager = b.id_user
        //                    //    LEFT JOIN tbl_csst_role c ON a.id_role = c.id_csst_role
        //                    //    LEFT JOIN tbl_profile g ON a.id_user = g.id_user
        //                    //    LEFT JOIN tbl_user d ON b.reporting_manager = d.id_user
        //                    //    LEFT JOIN tbl_profile h ON b.id_user = h.id_user
        //                    //    LEFT JOIN tbl_user e ON d.reporting_manager = e.id_user
        //                    //    LEFT JOIN tbl_profile i ON d.id_user = i.id_user
        //                    //    LEFT JOIN tbl_user f ON e.reporting_manager = f.id_user
        //                    //    LEFT JOIN tbl_profile j ON e.id_user = j.id_user
        //                    //            AND c.id_csst_role IN ({rolewiseParams})           
        //                    //            AND j.OFFICE_ADDRESS IN ({regionwiseArrayValues})
        //                    //    LEFT JOIN tbl_profile k ON f.id_user = k.id_user
        //                    //    JOIN tbl_department o ON c.id_department = o.id_department
        //                    //    WHERE 
        //                    //        a.id_organization = @orgid 
        //                    //        AND o.id_department IN ({Id_deparmentParams})
        //                    //        AND c.id_csst_role IN ({rolewiseParams})
        //                    //        AND j.OFFICE_ADDRESS IN ({regionwiseArrayValues})
        //                    //        AND j.CITY IN ({usercityArrayValues})

        //                    //       AND a.user_designation IN ({user_designationArrayValues})
        //                    //        AND a.userid NOT LIKE '%-old%'
        //                    //        AND b.userid NOT LIKE '%Bata%'
        //                    //        AND b.user_status ='A'
        //                    //";

        //                    string query = @"SELECT 
        //    a.userid,
        //    a.UPDATEDTIME,
        //    g.date_of_joining,
        //    a.user_department,
        //    a.user_designation,
        //    a.user_function,
        //    g.gender,
        //    g.city,
        //    g.office_address,
        //    a.user_grade AS Store_code,
        //    a.L4,
        //    a.L3,
        //    a.L2,
        //    a.L1,
        //    a.Spectator,
        //    g.Location AS Store_Name,
        //    a.status, 
        //    o.Department_name,
        //    c.csst_role AS Role
        //FROM 
        //    tbl_user a
        //LEFT JOIN tbl_user b ON a.reporting_manager = b.id_user
        //LEFT JOIN tbl_csst_role c ON a.id_role = c.id_csst_role
        //LEFT JOIN tbl_profile g ON a.id_user = g.id_user
        //LEFT JOIN tbl_user d ON b.reporting_manager = d.id_user
        //LEFT JOIN tbl_profile h ON b.id_user = h.id_user
        //LEFT JOIN tbl_user e ON d.reporting_manager = e.id_user
        //LEFT JOIN tbl_profile i ON d.id_user = i.id_user
        //LEFT JOIN tbl_user f ON e.reporting_manager = f.id_user
        //LEFT JOIN tbl_profile j ON e.id_user = j.id_user 
        //    AND c.id_csst_role IN ({rolewiseParams})
        //    AND j.OFFICE_ADDRESS IN ({regionwiseArrayValues})
        //LEFT JOIN tbl_profile k ON f.id_user = k.id_user
        //JOIN tbl_department o ON c.id_department = o.id_department
        //WHERE 
        //    a.id_organization =  @orgid  
        //    AND o.id_department IN ({Id_deparmentParams})
        //    AND c.id_csst_role  IN ({rolewiseParams})
        //AND a.user_designation IN ({user_designationArrayValues})
        //    AND a.userid NOT LIKE '%-old%'
        //    AND b.userid NOT LIKE '%Bata%'
        //    AND b.user_status = 'A'";

        //                    using (var cmd = new MySqlCommand(query, connection))
        //                    {
        //                        cmd.Parameters.AddWithValue("@orgid", orgid);

        //                        for (int i = 0; i < Id_deparment.Length; i++)
        //                        {
        //                            cmd.Parameters.AddWithValue($"@Id_deparment{i}", Id_deparment[i]);
        //                        }

        //                        for (int i = 0; i < rolewise.Length; i++)
        //                        {
        //                            cmd.Parameters.AddWithValue($"@rolewise{i}", rolewise[i]);
        //                        }

        //                        using (var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
        //                        {
        //                            // Load data into DataTable directly from the reader
        //                            results.Load(reader);
        //                        }
        //                    }
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                // Handle exception (log it, rethrow it, or return a custom error message)
        //                throw new Exception("An error occurred while fetching user details.", ex);
        //            }

        //            return results;
        //        }

        //Questionwise
        public async Task<DataTable> UserDetailsQ(int orgid, string[] Id_deparment, string[] rolewise, string[] regionwise, string[] usercity, string[] user_designation)
        {
            DataTable results = new DataTable();

            var Id_deparmentParams = string.Join(", ", Id_deparment.Select((id, i) => $"@Id_deparment{i}"));
            var rolewiseParams = string.Join(", ", rolewise.Select((id, i) => $"@rolewise{i}"));

            var regionwiseArray = regionwise.SelectMany(r => r.Split(',').Select(region => $"'{region.Trim()}'")).ToArray();

            string regionwiseArrayValues = "" + string.Join(",", regionwiseArray) + ""; //string

            var usercityArray = usercity.Select(city => $"'{city.Trim()}'").ToArray();

            string usercityArrayValues = "" + string.Join(",", usercityArray) + ""; //string


            var user_designationArray = user_designation.Select(userDesig => $"'{userDesig.Trim()}'").ToArray();

            string user_designationArrayValues = "" + string.Join(",", user_designationArray) + ""; //string


            try
            {
                //    if (_connection.State != System.Data.ConnectionState.Open)
                //    {
                //        await _connection.OpenAsync().ConfigureAwait(false);
                //    }
                using (var connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["dbconnectionstring"].ConnectionString))
                {
                    await connection.OpenAsync().ConfigureAwait(false);

                    string query = $@"
            SELECT 
                a.userid,
                a.UPDATEDTIME,
                g.date_of_joining,
                a.user_department,
                a.user_designation,
                a.user_function,
                g.gender,
                g.city,
                g.office_address,
                a.user_grade AS Store_code,
                a.L4,
                a.L3,
                a.L2,
                a.L1,
                a.Spectator,
                g.Location AS Store_Name,
                a.status, 
                o.Department_name,
                c.csst_role AS Role
            FROM 
                tbl_user a
            LEFT JOIN tbl_user b ON a.reporting_manager = b.id_user
            LEFT JOIN tbl_csst_role c ON a.id_role = c.id_csst_role
            LEFT JOIN tbl_profile g ON a.id_user = g.id_user
            LEFT JOIN tbl_user d ON b.reporting_manager = d.id_user
            LEFT JOIN tbl_profile h ON b.id_user = h.id_user
            LEFT JOIN tbl_user e ON d.reporting_manager = e.id_user
            LEFT JOIN tbl_profile i ON d.id_user = i.id_user
            LEFT JOIN tbl_user f ON e.reporting_manager = f.id_user
            LEFT JOIN tbl_profile j ON e.id_user = j.id_user
            LEFT JOIN tbl_profile k ON f.id_user = k.id_user
            JOIN tbl_department o ON c.id_department = o.id_department
            WHERE 
                a.id_organization = @orgid 
                And a.STATUS='A'
                AND o.id_department IN ({Id_deparmentParams})
                AND c.id_csst_role IN ({rolewiseParams})
              --  AND j.OFFICE_ADDRESS IN ({regionwiseArrayValues})
              --  AND j.CITY IN ({usercityArrayValues})
                
              --  AND a.user_designation IN ({user_designationArrayValues})
                AND a.userid NOT LIKE '%-old%'
                AND b.userid NOT LIKE '%Bata%'
            
                
        ";

                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@orgid", orgid);

                        for (int i = 0; i < Id_deparment.Length; i++)
                        {
                            cmd.Parameters.AddWithValue($"@Id_deparment{i}", Id_deparment[i]);
                        }

                        for (int i = 0; i < rolewise.Length; i++)
                        {
                            cmd.Parameters.AddWithValue($"@rolewise{i}", rolewise[i]);
                        }

                        using (var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                        {
                            // Load data into DataTable directly from the reader
                            results.Load(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exception (log it, rethrow it, or return a custom error message)
                throw new Exception("An error occurred while fetching user details.", ex);
            }

            return results;
        }

        //copy Questionwise
        public async Task<DataTable> UserDetailsAQ(int orgid, string[] Id_deparment, string[] rolewise, string[] regionwise, string[] usercity, string[] user_designation)
        {
            DataTable results = new DataTable();

            var Id_deparmentParams = string.Join(", ", Id_deparment.Select((id, i) => $"@Id_deparment{i}"));

            var rolewiseParams = string.Join(", ", rolewise.Select((id, i) => $"@rolewise{i}"));

            var regionwiseArray = regionwise.SelectMany(r => r.Split(',').Select(region => $"'{region.Trim()}'")).ToArray();

            string regionwiseArrayValues = "" + string.Join(",", regionwiseArray) + ""; //string

            var usercityArray = usercity.Select(city => $"'{city.Trim()}'").ToArray();

            string usercityArrayValues = "" + string.Join(",", usercityArray) + ""; //string

            var user_designationArray = user_designation.Select(userDesig => $"'{userDesig.Trim()}'").ToArray();

            string user_designationArrayValues = "" + string.Join(",", user_designationArray) + ""; //string


            try
            {
                //if (_connection.State != System.Data.ConnectionState.Open)
                //{
                //    await _connection.OpenAsync().ConfigureAwait(false);
                //}
                using (var connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["dbconnectionstring"].ConnectionString))
                {
                    await connection.OpenAsync().ConfigureAwait(false);

                    //string query = $@"
                    //        select USERID from tbl_user tu
                    //        join tbl_csst_role tcl on tcl.id_csst_role = tu.ID_ROLE
                    //        JOIN tbl_department o ON tcl.id_department = o.id_department
                    //        where tu.id_organization = @orgid 
                    //        AND tu.STATUS = 'A'
                    //        AND o.id_department IN ({Id_deparmentParams})
                    //        and tcl.id_csst_role in ({rolewiseParams})
                    //        AND tu.userid NOT LIKE '%-old%'
                    //        AND tu.userid NOT LIKE '%Bata%';  

                    //       ";

                    //AND j.OFFICE_ADDRESS IN({ regionwiseArrayValues})
                    //        AND j.CITY IN({ usercityArrayValues})
                
                    //       AND a.user_designation IN({ user_designationArrayValues})
                    string query = $@"  SELECT
                            a.userid                          
                        FROM
                            tbl_user a
                        LEFT JOIN tbl_user b ON a.reporting_manager = b.id_user
                        LEFT JOIN tbl_csst_role c ON a.id_role = c.id_csst_role
                        LEFT JOIN tbl_profile g ON a.id_user = g.id_user
                        LEFT JOIN tbl_user d ON b.reporting_manager = d.id_user
                        LEFT JOIN tbl_profile h ON b.id_user = h.id_user
                        LEFT JOIN tbl_user e ON d.reporting_manager = e.id_user
                        LEFT JOIN tbl_profile i ON d.id_user = i.id_user
                        LEFT JOIN tbl_user f ON e.reporting_manager = f.id_user
                        LEFT JOIN tbl_profile j ON e.id_user = j.id_user
                        LEFT JOIN tbl_profile k ON f.id_user = k.id_user
                        JOIN tbl_department o ON c.id_department = o.id_department
                        WHERE
                            a.id_organization = @orgid
                            AND o.id_department IN({Id_deparmentParams})
                            AND c.id_csst_role IN({rolewiseParams})
                            AND j.OFFICE_ADDRESS IN(0)
                            AND j.CITY IN(0)
                
                           AND a.user_designation IN(0)
                            AND a.userid NOT LIKE '%-old%'
                            AND b.userid NOT LIKE '%Bata%'
                            AND b.STATUS = 'A'";

                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@orgid", orgid);

                        for (int i = 0; i < Id_deparment.Length; i++)
                        {
                            cmd.Parameters.AddWithValue($"@Id_deparment{i}", Id_deparment[i]);
                        }

                        for (int i = 0; i < rolewise.Length; i++)
                        {
                            cmd.Parameters.AddWithValue($"@rolewise{i}", rolewise[i]);
                        }

                        using (var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                        {
                            // Load data into DataTable directly from the reader
                            results.Load(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exception (log it, rethrow it, or return a custom error message)
                throw new Exception("An error occurred while fetching user details.", ex);
            }

            return results;
        }


        //Skillmuni
        public async Task<IEnumerable<dynamic>> COECourse(int orgid, string[] category, string[] rolewise, string[] assessment_id, string startDate, string endDate, string[] regionwise,string[] user_designation,string[] usercity,string[] ID_category)
        {

           
            string categoryValues = "" + string.Join(",", category) + "";
            string assessment_idValues = "" + string.Join(",", assessment_id) + "";
            string rolewiseValues = "" + string.Join(",", rolewise) + "";
            string ID_categoryValues = "" + string.Join(",", ID_category) + "";
            //
            //string inputString = regionwise;

            //// Split the string into a list of values
            //string[] valuesArray = inputString.Split(',');

            // Join the values into a format suitable for the SQL query
            string formattedValues = "'" + string.Join("', '", regionwise) + "'";

            //string inputdesignation = user_designation;

            //// Split the string into a list of values
            //string[] designationArray = inputdesignation.Split(',');

            // Join the values into a format suitable for the SQL query
            string designationValues = "'" + string.Join("', '", user_designation) + "'";

            //string inputcity = usercity;

            //// Split the string into a list of values
            //string[] cityArray = inputcity.Split(',');

            // Join the values into a format suitable for the SQL query
            string cityValues = "'" + string.Join("', '", usercity) + "'";

          

            var results = new List<dynamic>();

            // Convert IEnumerable to List for indexing
            //var categoryList = category.ToList();
            //var assessmentIdList = assessment_id.ToList();
            //var rolewiseList = rolewise.ToList();

            //// Generate parameter placeholders for SQL IN clause
            //var categoryParams = string.Join(", ", categoryList.Select((s, i) => $"@category{i}"));
            //var assessmentIdParams = string.Join(", ", assessmentIdList.Select((s, i) => $"@assessment_id{i}"));
            //var rolewiseParams = string.Join(", ", rolewiseList.Select((s, i) => $"@rolewise{i}"));

            try
            {
                using (var connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["dbconnectionstring"].ConnectionString))
                {
                    await connection.OpenAsync().ConfigureAwait(false);

                    // await _connection.OpenAsync().ConfigureAwait(false);

                    //string query = "  SELECT" +
                    //    "  COALESCE(user_assigned.Region, '') AS Region," +
                    //    "   COALESCE(user_assigned.userid, content.userid) AS userid," +
                    //    "   COALESCE(user_assigned.csst_role, content.csst_role) AS csst_role," +
                    //    "   COALESCE(user_assigned.categoryname, content.Course) AS categoryname," +
                    //    "    user_assigned.start_date," +
                    //    "    CONCAT(COALESCE(user_assigned.Firstname, ''), ' ', COALESCE(user_assigned.Lastname, '')) AS Fullname," +
                    //    "    COALESCE(content.Course, user_assigned.categoryname) AS Course," +
                    //    "    COALESCE(content.Read_Count, 'NC') AS Read_Count," +
                    //    "    COALESCE(content.total_content_count, 'NC') AS total_content_count," +
                    //    "    CASE" +
                    //    "      WHEN content.total_content_count = 'NC' THEN '0'" +
                    //    "       ELSE COALESCE(content.Adoption_Percent, '0')" +
                    //    "   END AS Adoption_Percent," +
                    //    "    COALESCE(MAX(result_in_percentage), 0) AS max_result_in_percentage," +
                    //    "    CASE" +
                    //    "       WHEN COALESCE(content.Adoption_Percent, '0') = 1 AND COALESCE(MAX(result_in_percentage), 0) >= 80 THEN 1 " +
                    //    "        ELSE COALESCE(content.Adoption_Percent, '0') / 2" +
                    //    "   END AS completion_Percent," +
                    //    "    COALESCE(user_assigned.city, '') AS city," +
                    //    "   COALESCE(user_assigned.user_grade, '') AS Store_Code," +
                    //    "    COALESCE(assessment_data.attempt_number, '') AS Attempt_Number," +
                    //    "    COALESCE(user_assigned.DATE_OF_JOINING, '') AS DATE_OF_JOINING," +
                    //    "    COALESCE(user_assigned.GENDER, '') AS GENDER," +
                    //    "    COALESCE(user_assigned.STATUS, '') AS STATUS," +
                    //    "    COALESCE(user_assigned.user_designation, '') AS Designation," +
                    //     "  COALESCE(user_assigned.user_department, '') AS Department," +
                    //     "  COALESCE(user_assigned.Location, '') AS Store_name," +
                    //    "  COALESCE(user_assigned.user_function, '') AS StoreType," +
                    //    "  user_assigned.L4," +
                    //     "  user_assigned.L3," +
                    //     "  user_assigned.L2," +
                    //     "  user_assigned.L1," +
                    //     "  user_assigned.Spectator," +
                    //     "  COALESCE(user_assigned.Heading_title, '') AS Heading_title," +
                    //     "  COALESCE(user_assigned.updated_date_time, '') AS updated_date_time" +
                    //    "   FROM" +
                    //    "    (" +
                    //    "       SELECT" +
                    //    "           e.office_address AS Region," +
                    //    "            e.city," +
                    //    "           a.userid," +
                    //    "           a.user_grade," +
                    //    "           c.csst_role," +
                    //    "           d.categoryname," +
                    //    "          d.category_type," +
                    //    "           b.start_date," +
                    //    "           e.Firstname," +
                    //    "          a.STATUS," +
                    //    "          e.GENDER," +
                    //    "           e.DATE_OF_JOINING," +
                    //    "           e.Lastname," +
                    //    "     a.user_designation," +
                    //    "     a.user_department," +
                    //    "     e.LOCATION," +
                    //    "     a.user_function," +
                    //    "     h1.Heading_title," +
                    //    "     a.L4," +
                    //    "     a.L3," +
                    //    "     a.L2," +
                    //    "     a.L1," +
                    //    "     a.Spectator," +
                    //   "      Q1.updated_date_time" +
                    //    "        FROM  " +
                    //    "            tbl_user a" +
                    //    "      JOIN" +
                    //    "         tbl_profile e ON a.id_user = e.id_user" +
                    //    "       JOIN" +
                    //    "            tbl_csst_role c ON a.id_role = c.id_csst_role" +
                    //    "       LEFT JOIN" +
                    //    "            tbl_content_program_mapping b ON a.id_user = b.id_user AND b.status = 'A'" +
                    //    "       JOIN" +
                    //    "            tbl_category d ON b.id_category = d.id_category AND d.status = 'A'" +
                    //    "        JOIN tbl_category_heading h1 ON h1.id_category_heading = b.id_category_heading" +
                    //    "        JOIN tbl_rs_type_qna Q1 ON Q1.id_user = a.id_user" +
                    //    "       WHERE" +
                    //    "           a.id_organization = " + orgid + " AND a.STATUS = 'A'" +
                    //    "      And e.CITY IN (" + cityValues + ") " +
                    //    "     AND a.user_designation IN ( " + designationValues + ") " +
                    //    "     AND DATE(b.start_date) BETWEEN '" + startDate + "' AND '" + endDate + "'"+
                    //    "     AND b.id_category IN(" + categoryValues + ")" +
                    //    "     AND c.id_csst_role IN (" + rolewiseValues + ") " +
                    //    "     AND e.OFFICE_ADDRESS IN(" + formattedValues + ") " +
                    //    "     AND b.id_category_heading IN (" + ID_categoryValues + ") " +
                    //    "      AND a.userid NOT LIKE '%Bata%' " +
                    //    "   AND a.userid NOT LIKE '%_SM_BA%' " +
                    //    "    AND a.userid NOT LIKE '%_DM_BA%' " +
                    //    "    AND a.userid NOT LIKE '%_RM_BA%'" +
                    //    "       GROUP BY" +
                    //    "     a.id_user, categoryname" +
                    //    "   ) AS user_assigned" +
                    //    "   LEFT JOIN" +
                    //    "   (" +
                    //    "       SELECT" +
                    //    "           d.userid," +
                    //    "            d.id_role," +
                    //    "            f.csst_role," +
                    //    "           e.Firstname," +
                    //    "          e.Lastname," +
                    //    "          b.categoryname AS Course," +
                    //    "          COUNT(DISTINCT a.id_content) AS Read_Count," +
                    //    "            total_count.total_content_count," +
                    //    "           COUNT(DISTINCT a.id_content) / total_count.total_content_count AS Adoption_Percent" +
                    //    "       FROM" +
                    //    "          tbl_content_counters a" +
                    //    "       JOIN" +
                    //    "           tbl_content_organization_mapping c ON a.id_content = c.id_content" +
                    //    "       JOIN" +
                    //    "           tbl_category b ON b.id_category = c.id_category" +
                    //    "        JOIN" +
                    //    "            tbl_user d ON a.id_user = d.id_user" +
                    //    "        JOIN" +
                    //    "           tbl_profile e ON d.id_user = e.id_user" +
                    //    "       JOIN" +
                    //    "        tbl_csst_role f ON d.id_role = f.id_csst_role" +
                    //    "       JOIN" +
                    //    "         (" +
                    //    "                SELECT" +
                    //    "                   b.id_category," +
                    //    "                   COUNT(*) AS total_content_count" +
                    //    "              FROM" +
                    //    "                   tbl_category b" +
                    //    "               JOIN" +
                    //    "                   tbl_content_organization_mapping tcb ON b.id_category = tcb.id_category" +
                    //    "               JOIN" +
                    //    "                   tbl_content tcc ON tcb.id_content = tcc.id_content" +
                    //    "                WHERE" +
                    //    "                    b.id_organization = " + orgid + "" +
                    //    "                   AND tcc.status = 'A'" +
                    //    "               GROUP BY" +
                    //    "                    b.id_category" +
                    //    "           ) total_count ON b.id_category = total_count.id_category" +
                    //    "       WHERE" +
                    //    "              b.id_organization = " + orgid + "" +
                    //    "      AND b.status = 'A' " +
                    //    "    AND b.id_category IN (" + categoryValues + ")" +
                    //    "      AND d.status = 'A' " +
                    //    "      AND f.id_csst_role IN (" + rolewiseValues + ")" +
                    //    "      AND d.userid NOT LIKE '%Bata%' " +
                    //    "   AND d.userid NOT LIKE '%_SM_BA%' " +
                    //    "   AND d.userid NOT LIKE '%_DM_BA%' " +
                    //    "     AND d.userid NOT LIKE '%_RM_BA%' " +
                    //    "     GROUP BY" +
                    //    "            d.userid, b.categoryname, d.id_role" +
                    //    "   ) AS content ON user_assigned.userid = content.userid AND user_assigned.categoryname = content.Course" +
                    //    "   LEFT JOIN    (" +
                    //    "      SELECT" +
                    //    "           b.id_user," +
                    //    "           b.userid," +
                    //    "           c.firstname," +
                    //    "            c.lastname," +
                    //    "           c.office_address AS Region," +
                    //    "           c.city," +
                    //    "         d.csst_role AS Role," +
                    //    "            CONCAT(IFNULL(f.categoryname, ''), IF(f.categoryname IS NOT NULL AND i.categoryname IS NOT NULL, ',', ''), IFNULL(i.categoryname, '')) AS Catname," +
                    //    "           CONCAT(IFNULL(f.id_category, ''), IF(f.id_category IS NOT NULL AND i.id_category IS NOT NULL, ',', ''), IFNULL(i.id_category, '')) AS Catid," +
                    //    "           a.assessment_title," +
                    //    "          CASE" +
                    //    "                WHEN a.assessment_type = 1 THEN 'M2OST'" +
                    //    "              WHEN a.assessment_type = 3 THEN 'MT'" +
                    //    "               WHEN a.assessment_type = 4 THEN 'WB'" +
                    //    "            END AS TYPE," +
                    //    "            MAX(e.attempt_number) AS attempt_number," +
                    //    "           MAX(e.result_in_percentage) AS result_in_percentage," +
                    //    "           MAX(e.updated_date_time) AS updated_date_time" +
                    //    "        FROM" +
                    //    "            tbl_assessment a" +
                    //    "       JOIN tbl_user b ON b.status = 'A'" +
                    //    "       JOIN tbl_profile c ON b.id_user = c.id_user" +
                    //    "       JOIN tbl_csst_role d ON b.id_role = d.id_csst_role" +
                    //    "       JOIN tbl_rs_type_qna e ON a.id_assessment = e.id_assessment AND e.id_user = b.id_user" +
                    //    "        LEFT JOIN tbl_assessment_user_assignment g ON a.id_assessment = g.id_assessment AND g.id_user = b.id_user AND g.status = 'A'" +
                    //    "       LEFT JOIN tbl_assessment_categoty_mapping h ON a.id_assessment = h.id_assessment AND h.status = 'A'" +
                    //    "        LEFT JOIN tbl_category f ON g.id_category = f.id_category" +
                    //    "       LEFT JOIN tbl_category i ON h.id_category = i.id_category" +
                    //    "       WHERE" +
                    //    "            a.id_organization  = " + orgid + "" +
                    //    "     AND a.status = 'A' " +
                    //    "   AND d.id_csst_role IN(" + rolewiseValues + ")" +
                    //    "     AND a.id_assessment IN (" + assessment_idValues + ")" +
                    //    "    AND d.csst_role NOT LIKE '%Spectator_%' " +
                    //    "     AND b.userid NOT LIKE '%Bata%' " +
                    //    "      AND b.userid NOT LIKE '%_SM_BA%' " +
                    //    "      AND b.userid NOT LIKE '%_DM_BA%' " +
                    //    "     AND b.userid NOT LIKE '%_RM_BA%' " +
                    //    "     AND c.OFFICE_ADDRESS NOT LIKE 'Pune' " +
                    //    "        GROUP BY" +
                    //    "            Catname, userid" +
                    //    "    ) AS assessment_data ON user_assigned.userid = assessment_data.userid AND user_assigned.categoryname = assessment_data.Catname" +
                    //    "  LEFT JOIN" +
                    //    "   tbl_profile PROFILE ON user_assigned.userid = profile.id_user" +
                    //    "   LEFT JOIN" +
                    //    "    tbl_user USER ON user_assigned.userid = user.id_user" +
                    //    "  GROUP BY" +
                    //    "   user_assigned.Region," +
                    //    "   user_assigned.userid," +
                    //    "    user_assigned.csst_role," +
                    //    "    user_assigned.categoryname," +
                    //    "    user_assigned.category_type," +
                    //    "    user_assigned.start_date," +
                    //    "    Course," +
                    //    "    Read_Count," +
                    //    "    total_content_count," +
                    //    "    Adoption_Percent," +
                    //    "   profile.DATE_OF_JOINING," +
                    //    "    user.status,   user_assigned.GENDER";

                    //optimize 

                    string queryCondition = "";
                    if (int.TryParse(assessment_idValues, out int assessmentId) && assessmentId == 518)
                    {
                        queryCondition = "AND IFNULL(ad.Assessment_Completion, 0) = 100 THEN 1 ";
                    }
                    else
                    {
                        queryCondition = "AND IFNULL(ad.Assessment_Completion, 0) >= 80 THEN 1 ";
                    }

                    string query = "SELECT" +
                    " IFNULL(ua.Region, '') AS Region," +
                    "  IFNULL(ua.userid, c.userid) AS userid," +
                    " IFNULL(ua.csst_role, c.csst_role) AS csst_role," +
                    " IFNULL(ua.categoryname, c.Course) AS categoryname," +
                    " ua.start_date," +
                    " CONCAT(IFNULL(ua.Firstname, ''), ' ', IFNULL(ua.Lastname, '')) AS Fullname," +
                    " IFNULL(c.Course, ua.categoryname) AS Course," +
                    "  IFNULL(c.Read_Count, 'NC') AS Read_Count," +
                    "  IFNULL(c.total_content_count, 'NC') AS total_content_count," +
                    "  CASE " +
                    "  WHEN c.total_content_count = 'NC' THEN '0' " +
                    "   ELSE IFNULL(c.Adoption_Percent, '0') " +
                    "  END AS Adoption_Percent," +
                    " IFNULL(ad.Assessment_Completion, 0) AS Assessment_Completion," +
                    "  CASE " +
                    "   WHEN IFNULL(CAST(c.Adoption_Percent AS DECIMAL), 0) = 1 " +

                      queryCondition +


                    "   ELSE IFNULL(CAST(c.Adoption_Percent AS DECIMAL), 0) / 2 " +
                    " END AS completion_Percent," +
                    "  IFNULL(ua.city, '') AS city," +
                    "  IFNULL(ua.user_grade, '') AS Store_Code," +
                    "  IFNULL(ad.attempt_number, '') AS Attempt_Number," +
                    "  IFNULL(ua.DATE_OF_JOINING, '') AS DATE_OF_JOINING," +
                    "  IFNULL(ua.GENDER, '') AS GENDER," +
                    " IFNULL(ua.STATUS, '') AS STATUS," +
                    "  IFNULL(ua.user_designation, '') AS Designation," +
                    "  IFNULL(ua.user_department, '') AS Department," +
                    "  IFNULL(ua.Location, '') AS Store_name," +
                    " IFNULL(ua.user_function, '') AS StoreType," +
                    " ua.L4," +
                    " ua.L3," +
                    " ua.L2," +
                    " ua.L1," +
                    " ua.Spectator," +
                    "  IFNULL(ua.Heading_title, '') AS Heading_title," +
                    " IFNULL(ua.updated_date_time, '') AS updated_date_time" +
                    " FROM (   SELECT      e.office_address AS Region," +
                    "   e.city," +
                    "      a.userid," +
                    "     a.user_grade," +
                    "     c.csst_role," +
                    "      d.categoryname," +
                    "      b.start_date," +
                    "     e.Firstname," +
                    "      a.STATUS," +
                    "    e.GENDER," +
                    "      e.DATE_OF_JOINING," +
                    "    e.Lastname," +
                    "    a.user_designation," +
                    "     a.user_department," +
                    "     e.LOCATION," +
                    "     a.user_function," +
                    "     h1.Heading_title," +
                    "     a.L4," +
                    "     a.L3," +
                    "     a.L2," +
                    "     a.L1," +
                    "     a.Spectator," +
                    "      Q1.updated_date_time" +
                    "    FROM" +
                    "     tbl_user a" +
                    "     LEFT JOIN tbl_profile e ON a.id_user = e.id_user" +
                    "     LEFT JOIN tbl_csst_role c ON a.id_role = c.id_csst_role" +
                    "     LEFT JOIN tbl_content_program_mapping b ON a.id_user = b.id_user AND b.status = 'A'" +
                    "     LEFT JOIN tbl_category d ON b.id_category = d.id_category AND d.status = 'A'" +
                    "     LEFT JOIN tbl_category_heading h1 ON h1.id_category_heading = b.id_category_heading" +
                    "     LEFT JOIN tbl_rs_type_qna Q1 ON Q1.id_user = a.id_user" +
                    "   WHERE" +
                   "        a.id_organization = " + orgid + " AND a.STATUS = 'A'" +
                    "       And e.CITY IN (" + cityValues + ") " +
                    "        AND a.user_designation IN ( " + designationValues + ") " +
                    "        AND DATE(b.start_date) BETWEEN '" + startDate + "' AND '" + endDate + "'                                          " +
                    "         AND b.id_category IN (" + categoryValues + ")" +
                    "         AND c.id_csst_role IN (" + rolewiseValues + ") " +
                    "         AND e.OFFICE_ADDRESS IN (" + formattedValues + ") " +
                    "         AND b.id_category_heading IN (" + ID_categoryValues + ") " +
                    "      AND a.userid NOT REGEXP 'Bata|_SM_BA|_DM_BA|_RM_BA|_SM_HP') AS ua" +
                    " LEFT JOIN (  SELECT    d.userid,     f.csst_role,    e.Firstname,     e.Lastname,     b.categoryname AS Course," +
                    "      COUNT(DISTINCT a.id_content) AS Read_Count," +
                    "     total_count.total_content_count," +
                    "      COUNT(DISTINCT a.id_content) / total_count.total_content_count AS Adoption_Percent" +
                    "    FROM     tbl_content_counters a" +
                    "     LEFT JOIN tbl_content_organization_mapping c ON a.id_content = c.id_content" +
                    "     LEFT JOIN tbl_category b ON b.id_category = c.id_category" +
                    "     LEFT JOIN tbl_user d ON a.id_user = d.id_user" +
                    "     LEFT JOIN tbl_profile e ON d.id_user = e.id_user" +
                    "     LEFT JOIN tbl_csst_role f ON d.id_role = f.id_csst_role" +
                    "     LEFT JOIN (       SELECT         b.id_category,         COUNT(*) AS total_content_count      FROM" +
                    "         tbl_category b" +
                    "        JOIN tbl_content_organization_mapping tcb ON b.id_category = tcb.id_category" +
                    "       JOIN tbl_content tcc ON tcb.id_content = tcc.id_content" +
                    "        WHERE" +
                    "          b.id_organization = " + orgid + "" +
                    "         AND tcc.status = 'A'" +
                    "       GROUP BY" +
                    "         b.id_category     ) total_count ON b.id_category = total_count.id_category" +
                    "    WHERE" +
                    "      b.id_organization = " + orgid + "" +
                    "    AND b.status = 'A'" +
                    "    AND b.id_category IN (" + categoryValues + ")" +
                    "     AND d.status = 'A'" +
                    "      AND f.id_csst_role IN (" + rolewiseValues + ")" +
                    "      AND d.userid NOT REGEXP 'Bata|_SM_BA|_DM_BA|_RM_BA|_SM_HP'" +
                    "   GROUP BY     d.userid,    b.categoryname,      f.csst_role," +
                    "      e.Firstname,      e.Lastname,      total_count.total_content_count) AS c ON ua.userid = c.userid AND ua.categoryname = c.Course " +
                    " LEFT JOIN (   SELECT     b.userid,      CONCAT(IFNULL(f.categoryname, ''), IF(f.categoryname IS NOT NULL AND i.categoryname IS NOT NULL, ',', ''), IFNULL(i.categoryname, '')) AS Catname," +
                    "   MAX(e.attempt_number) AS attempt_number," +
                    "     MAX(e.result_in_percentage) AS result_in_percentage," +
                    "    MAX(e.result_in_percentage) AS Assessment_Completion" +
                    "    FROM      tbl_assessment a     JOIN tbl_user b ON b.status = 'A'" +
                    "    LEFT JOIN tbl_profile c ON b.id_user = c.id_user" +
                    "    LEFT JOIN tbl_csst_role d ON b.id_role = d.id_csst_role" +
                    "    LEFT JOIN tbl_rs_type_qna e ON a.id_assessment = e.id_assessment AND e.id_user = b.id_user" +
                    "    LEFT JOIN tbl_assessment_user_assignment g ON a.id_assessment = g.id_assessment AND g.id_user = b.id_user AND g.status = 'A'" +
                    "    LEFT JOIN tbl_assessment_categoty_mapping h ON a.id_assessment = h.id_assessment AND h.status = 'A'" +
                    "    LEFT JOIN tbl_category f ON g.id_category = f.id_category" +
                    "    LEFT JOIN tbl_category i ON h.id_category = i.id_category" +
                    "    WHERE    a.id_organization = " + orgid + "" +
                    "     AND a.status = 'A'" +
                    "     AND d.id_csst_role IN (" + rolewiseValues + ")" +
                    "     AND a.id_assessment IN (" + assessment_idValues + ")" +
                    "   AND d.csst_role NOT LIKE '%Spectator_%'" +
                    "     AND b.userid NOT REGEXP 'Bata|_SM_BA|_DM_BA|_RM_BA|_SM_HP'" +
                    "      AND c.OFFICE_ADDRESS NOT LIKE 'Pune'" +
                    "    GROUP BY" +
                    "    b.userid," +
                    "    CONCAT(IFNULL(f.categoryname, ''), IF(f.categoryname IS NOT NULL AND i.categoryname IS NOT NULL, ',', ''), IFNULL(i.categoryname, ''))) AS ad ON ua.userid = ad.userid AND ua.categoryname = ad.Catname" +
                    " LEFT JOIN tbl_profile p ON ua.userid = p.id_user" +
                    " LEFT JOIN tbl_user u ON ua.userid = u.id_user " +
                    " GROUP BY " +
                    " ua.Region, " +
                    " ua.userid, " +
                    " ua.csst_role, " +
                    "  ua.categoryname, " +
                    "  ua.Firstname, " +
                    "  ua.Lastname," +
                    "  c.Course," +
                    " c.Read_Count," +
                    "  c.total_content_count," +
                    "  c.Adoption_Percent," +
                    "  p.DATE_OF_JOINING," +
                    "  u.STATUS, " +
                    " ua.GENDER, " +
                    " ua.city," +
                    " ua.user_grade," +
                    " ad.attempt_number;";
                    //test 
                    //string query = "SELECT COALESCE(user_assigned.Region, '') AS Region," +
                    //    " COALESCE(user_assigned.userid, content.userid) AS userid," +
                    //    " COALESCE(user_assigned.csst_role, content.csst_role) AS csst_role," +
                    //    " COALESCE(user_assigned.categoryname, content.Course) AS categoryname," +
                    //    " user_assigned.start_date, " +
                    //    " CONCAT(COALESCE(user_assigned.Firstname, ''), ' ', COALESCE(user_assigned.Lastname, '')) AS Fullname," +
                    //    " COALESCE(content.Course, user_assigned.categoryname) AS Course," +
                    //    " COALESCE(content.Read_Count, 'NC') AS Read_Count," +
                    //    " COALESCE(content.total_content_count, 'NC') AS total_content_count," +
                    //    " CASE" +
                    //    " WHEN content.total_content_count = 'NC' THEN '0'" +
                    //    "      ELSE COALESCE(content.Adoption_Percent, '0')" +
                    //    "  END AS Adoption_Percent," +
                    //    " COALESCE(MAX(assessment_data.result_in_percentage), 0) AS Assessment_Completion," +
                    //    " CASE" +
                    //    " WHEN COALESCE(CAST(content.Adoption_Percent AS DECIMAL), 0) = 1" +
                    //    "   AND COALESCE(MAX(assessment_data.result_in_percentage), 0) >= 80" +
                    //    "  THEN 1" +
                    //    " ELSE COALESCE(CAST(content.Adoption_Percent AS DECIMAL), 0) / 2" +
                    //    " END AS completion_Percent," +
                    //    "   COALESCE(user_assigned.city, '') AS city," +
                    //    "  COALESCE(user_assigned.user_grade, '') AS Store_Code," +
                    //    "   COALESCE(assessment_data.attempt_number, '') AS Attempt_Number," +
                    //    "  COALESCE(user_assigned.DATE_OF_JOINING, '') AS DATE_OF_JOINING," +
                    //    "   COALESCE(user_assigned.GENDER, '') AS GENDER," +
                    //    "   COALESCE(user_assigned.STATUS, '') AS STATUS," +
                    //    "   COALESCE(user_assigned.user_designation, '') AS Designation," +
                    //    "   COALESCE(user_assigned.user_department, '') AS Deparment," +
                    //    "   COALESCE(user_assigned.Location, '') AS Store_name," +
                    //    "   COALESCE(user_assigned.user_function, '') AS StoreType," +
                    //    "   COALESCE(user_assigned.Heading_title, '') AS Heading_title," +
                    //    "   COALESCE(user_assigned.updated_date_time, '') AS updated_date_time" +
                    //    " FROM" +
                    //    "  (" +
                    //    "     SELECT" +
                    //    "          e.office_address AS Region," +
                    //    "           e.city," +
                    //    "            a.userid," +
                    //    "           a.user_grade," +
                    //    "           c.csst_role," +
                    //    "           d.categoryname," +
                    //    "          d.category_type," +
                    //    "          b.start_date," +
                    //    "          e.Firstname," +
                    //    "            a.STATUS," +
                    //    "           e.GENDER," +
                    //    "         e.DATE_OF_JOINING," +
                    //    "           e.Lastname, " +
                    //    "           a.user_designation," +
                    //    "            a.user_department," +
                    //    "          e.LOCATION," +
                    //    "            a.user_function," +
                    //    "           h1.Heading_title," +
                    //    "           Q1.updated_date_time" +
                    //    "      FROM " +
                    //    "            tbl_user a" +
                    //    "    JOIN" +
                    //    "           tbl_profile e ON a.id_user = e.id_user" +
                    //    "        JOIN" +
                    //    "         tbl_csst_role c ON a.id_role = c.id_csst_role" +
                    //    "       LEFT JOIN" +
                    //    "           tbl_content_program_mapping b ON a.id_user = b.id_user AND b.status = 'A'" +
                    //    "       JOIN" +
                    //    "      tbl_category d ON b.id_category = d.id_category AND d.status = 'A'" +
                    //    "  join" +
                    //    "   tbl_category_heading h1 ON h1.id_category_heading = b.id_category_heading" +
                    //    "   join tbl_rs_type_qna Q1 ON Q1.id_user = a.id_user" +
                    //    "       WHERE" +
                    //    "        a.id_organization = " + orgid + " AND a.STATUS = 'A'" +
                    //    "       And e.CITY IN (" + cityValues + ") " +
                    //    "        AND a.user_designation IN ( " + designationValues + ") " +
                    //    "        AND DATE(b.start_date) BETWEEN '" + startDate + "' AND '" + endDate + "'                                          " +
                    //    "         AND b.id_category IN (" + categoryValues + ")" +
                    //    "         AND c.id_csst_role IN (" + rolewiseValues + ") " +
                    //    "         AND e.OFFICE_ADDRESS IN (" + formattedValues + ") " +
                    //    "         AND b.id_category_heading IN (" + ID_categoryValues + ") " +
                    //    "         AND a.userid NOT LIKE '%Bata%'" +
                    //    "         AND a.userid NOT LIKE '%_SM_BA%'" +
                    //    "         AND a.userid NOT LIKE '%_DM_BA%'" +
                    //    "         AND a.userid NOT LIKE '%_RM_BA%'" +
                    //    "  ) AS user_assigned" +
                    //    " LEFT JOIN" +
                    //    "   (" +
                    //    "       SELECT" +
                    //    "           d.userid," +
                    //    "           d.id_role," +
                    //    "         f.csst_role," +
                    //    "          e.Firstname," +
                    //    "          e.Lastname," +
                    //    "         b.categoryname AS Course," +
                    //    "           COUNT(DISTINCT a.id_content) AS Read_Count," +
                    //    "           total_count.total_content_count," +
                    //    "           COUNT(DISTINCT a.id_content) / total_count.total_content_count AS Adoption_Percent" +
                    //    "     FROM" +
                    //    "           tbl_content_counters a" +
                    //    "     JOIN" +
                    //    "     tbl_content_organization_mapping c ON a.id_content = c.id_content" +
                    //    "       JOIN" +
                    //    "    tbl_category b ON b.id_category = c.id_category" +
                    //    "        JOIN" +
                    //    "            tbl_user d ON a.id_user = d.id_user" +
                    //    "        JOIN" +
                    //    "            tbl_profile e ON d.id_user = e.id_user" +
                    //    "        JOIN" +
                    //    "            tbl_csst_role f ON d.id_role = f.id_csst_role" +
                    //    "        JOIN" +
                    //    "            (" +
                    //    "                SELECT" +
                    //    "                   b.id_category," +
                    //    "                    COUNT(*) AS total_content_count" +
                    //    "               FROM" +
                    //    "                   tbl_category b" +
                    //    "               JOIN" +
                    //    "                   tbl_content_organization_mapping tcb ON b.id_category = tcb.id_category" +
                    //    "               JOIN" +
                    //    "                    tbl_content tcc ON tcb.id_content = tcc.id_content" +
                    //    "                WHERE" +
                    //    "                    b.id_organization =  " + orgid + "" +
                    //    "                   AND tcc.status = 'A'" +
                    //    "                GROUP BY" +
                    //    "                   b.id_category" +
                    //    "            ) total_count ON b.id_category = total_count.id_category" +
                    //    "       WHERE" +
                    //    "         b.id_organization =  " + orgid + "" +
                    //    "            AND b.status = 'A'" +
                    //    "            AND b.id_category IN (" + categoryValues + ")" +
                    //    "           AND d.status = 'A'" +
                    //    "            AND f.id_csst_role IN (" + rolewiseValues + ") " +
                    //    "            AND d.userid NOT LIKE '%Bata%'" +
                    //    "            AND d.userid NOT LIKE '%_SM_BA%'" +
                    //    "            AND d.userid NOT LIKE '%_DM_BA%'" +
                    //    "            AND d.userid NOT LIKE '%_RM_BA%'" +
                    //    "        GROUP BY" +
                    //    "            d.userid, b.categoryname, d.id_role, f.csst_role, e.Firstname, e.Lastname, total_count.total_content_count" +
                    //    "   ) AS content ON user_assigned.userid = content.userid AND user_assigned.categoryname = content.Course" +
                    //    " LEFT JOIN" +
                    //    "    (" +
                    //    "        SELECT" +
                    //    "          b.userid," +
                    //    "           CONCAT(IFNULL(f.categoryname, ''), IF(f.categoryname IS NOT NULL AND i.categoryname IS NOT NULL, ',', ''), IFNULL(i.categoryname, '')) AS Catname," +
                    //    "           MAX(e.attempt_number) AS attempt_number," +
                    //    "         MAX(e.result_in_percentage) AS result_in_percentage" +
                    //    "       FROM" +
                    //    "         tbl_assessment a" +
                    //    "       JOIN tbl_user b ON b.status = 'A'" +
                    //    "       JOIN tbl_profile c ON b.id_user = c.id_user" +
                    //    "      JOIN tbl_csst_role d ON b.id_role = d.id_csst_role" +
                    //    "       JOIN tbl_rs_type_qna e ON a.id_assessment = e.id_assessment AND e.id_user = b.id_user" +
                    //    "       LEFT JOIN tbl_assessment_user_assignment g ON a.id_assessment = g.id_assessment AND g.id_user = b.id_user AND g.status = 'A'" +
                    //    "       LEFT JOIN tbl_assessment_categoty_mapping h ON a.id_assessment = h.id_assessment AND h.status = 'A'" +
                    //    "        LEFT JOIN tbl_category f ON g.id_category = f.id_category" +
                    //    "        LEFT JOIN tbl_category i ON h.id_category = i.id_category" +
                    //    "        WHERE" +
                    //    "           a.id_organization =  " + orgid + " AND a.status = 'A'" +
                    //    "            AND d.id_csst_role IN (" + rolewiseValues + ") " +
                    //    "            AND a.id_assessment IN (" + assessment_idValues + ")" +
                    //    "            AND d.csst_role NOT LIKE '%Spectator_%'" +
                    //    "            AND b.userid NOT LIKE '%Bata%'" +
                    //    "            AND b.userid NOT LIKE '%_SM_BA%'" +
                    //    "            AND b.userid NOT LIKE '%_DM_BA%'" +
                    //    "            AND b.userid NOT LIKE '%_RM_BA%'" +
                    //    "            AND c.OFFICE_ADDRESS NOT LIKE 'Pune'" +
                    //    "        GROUP BY" +
                    //    "           b.userid, CONCAT(IFNULL(f.categoryname, ''), IF(f.categoryname IS NOT NULL AND i.categoryname IS NOT NULL, ',', ''), IFNULL(i.categoryname, ''))" +
                    //    "    ) AS assessment_data ON user_assigned.userid = assessment_data.userid AND user_assigned.categoryname = assessment_data.Catname" +
                    //    " LEFT JOIN" +
                    //    " tbl_profile profile ON user_assigned.userid = profile.id_user" +
                    //    " LEFT JOIN" +
                    //    " tbl_user user ON user_assigned.userid = user.id_user" +
                    //    " GROUP BY" +
                    //    "   user_assigned.Region," +
                    //    "    user_assigned.userid," +
                    //    "    user_assigned.csst_role," +
                    //    "    user_assigned.categoryname," +
                    //    "    user_assigned.Firstname," +
                    //    "   user_assigned.Lastname," +
                    //    "   content.Course," +
                    //    "   content.Read_Count," +
                    //    "    content.total_content_count," +
                    //    "    content.Adoption_Percent," +
                    //    "    profile.DATE_OF_JOINING," +
                    //    "    user.status," +
                    //    "   user_assigned.GENDER," +
                    //    "    user_assigned.city," +
                    //    "   user_assigned.user_grade," +
                    //    "   assessment_data.attempt_number;";


                    //        string query = " SELECT " +
                    //         " COALESCE(user_assigned.userid, content.userid) AS userid, " +
                    //             " CONCAT(COALESCE(user_assigned.Firstname, ''), ' ', COALESCE(user_assigned.Lastname, '')) AS Fullname, " +
                    //              " COALESCE(user_assigned.csst_role, content.csst_role) AS csst_role, " +
                    //           " COALESCE(user_assigned.Region, '') AS Region, " +
                    //              " COALESCE(user_assigned.city, '') AS city, " +
                    //           " COALESCE(user_assigned.user_grade, '') AS Store_Code, " +

                    //           " COALESCE(user_assigned.DATE_OF_JOINING, '') AS DATE_OF_JOINING, " +
                    //           " COALESCE(user_assigned.GENDER, '') AS GENDER, " +
                    //           " COALESCE(user_assigned.STATUS, '') AS STATUS, " +
                    //            " COALESCE(user_assigned.categoryname, content.Course) AS categoryname, " +
                    //             " user_assigned.start_date, " +
                    //           " COALESCE(content.Course, user_assigned.categoryname) AS Course, " +
                    //           " COALESCE(content.Read_Count, 'NC') AS Read_Count, " +
                    //           " COALESCE(content.total_content_count, 'NC') AS total_content_count, " +
                    //           " CASE " +
                    //           "    WHEN content.total_content_count = 'NC' THEN '0' " +
                    //           "    ELSE COALESCE(content.Adoption_Percent, '0') " +
                    //           " END AS Adoption_Percent ," +
                    //   " COALESCE(MAX(result_in_percentage), 0) AS max_result_in_percentage, " +
                    //           " CASE " +
                    //           "    WHEN COALESCE(content.Adoption_Percent, '0') = 1 AND COALESCE(MAX(result_in_percentage), 0) >= 80 THEN 1 " +
                    //           "    ELSE COALESCE(content.Adoption_Percent, '0') / 2 " +
                    //           " END AS completion_Percent, " +
                    //            " COALESCE(assessment_data.attempt_number, '') AS Attempt_Number " +

                    //           " FROM" +
                    //   "  (" +

                    //       " SELECT" +
                    //            " e.office_address AS Region," +
                    //            " e.city," +
                    //            " a.userid," +
                    //            " a.user_grade," +
                    //            " c.csst_role," +
                    //            " d.categoryname," +
                    //            " d.category_type," +
                    //            " b.start_date," +
                    //            " e.Firstname," +
                    //            " a.STATUS," +
                    //            " e.GENDER," +
                    //            " e.DATE_OF_JOINING," +
                    //            " e.Lastname" +
                    //        " FROM  " +
                    //            " tbl_user a JOIN tbl_profile e ON a.id_user = e.id_user JOIN tbl_csst_role c ON a.id_role = c.id_csst_role
                    //            LEFT JOIN tbl_content_program_mapping b ON a.id_user = b.id_user AND b.status = 'A'" +
                    //        " JOIN " +
                    //            " tbl_category d ON b.id_category = d.id_category AND d.status = 'A' WHERE a.id_organization = "+orgid+ " AND a.STATUS = 'A' AND DATE(b.start_date) BETWEEN '"+ startDate + "' AND '"+ endDate + "' AND b.id_category IN ("+ category+ ") AND c.id_csst_role IN ("+ rolewise + ")  AND a.userid NOT LIKE '%_SM_BA'" +
                    //        " GROUP BY a.id_user, categoryname) AS user_assigned LEFT JOIN (SELECT d.userid, d.id_role, f.csst_role,e.Firstname, e.Lastname, b.categoryname AS Course," +
                    //            " COUNT(DISTINCT a.id_content) AS Read_Count, total_count.total_content_count, COUNT(DISTINCT a.id_content) / total_count.total_content_count AS Adoption_Percent FROM tbl_content_counters a JOIN" +
                    //            " tbl_content_organization_mapping c ON a.id_content = c.id_content JOIN tbl_category b ON b.id_category = c.id_category JOIN  tbl_user d ON a.id_user = d.id_user JOIN" +
                    //            " tbl_profile e ON d.id_user = e.id_user JOIN tbl_csst_role f ON d.id_role = f.id_csst_role JOIN ( SELECT b.id_category, COUNT(*) AS total_content_count FROM" +
                    //                    " tbl_category b JOIN tbl_content_organization_mapping tcb ON b.id_category = tcb.id_category JOIN tbl_content tcc ON tcb.id_content = tcc.id_content WHERE" +
                    //                    " b.id_organization = "+orgid+"  AND tcc.status = 'A' GROUP BY b.id_category  ) total_count ON b.id_category = total_count.id_category" +
                    //               "  WHERE   b.id_organization = "+orgid+" AND b.status = 'A' AND b.id_category IN ("+category+") AND d.status = 'A' AND f.id_csst_role IN ("+ rolewise + ")  AND d.userid NOT LIKE '%_SM_BA' GROUP BY" +
                    //           " d.userid, b.categoryname, d.id_role ) AS content ON user_assigned.userid = content.userid AND user_assigned.categoryname = content.Course LEFT JOIN" +
                    //    " ( SELECT b.id_user, b.userid,  c.firstname, c.lastname, c.office_address AS Region, c.city, d.csst_role AS Role," +
                    //            " CONCAT(IFNULL(f.categoryname, ''), IF(f.categoryname IS NOT NULL AND i.categoryname IS NOT NULL, ',', ''), IFNULL(i.categoryname, '')) AS Catname," +
                    //            " CONCAT(IFNULL(f.id_category, ''), IF(f.id_category IS NOT NULL AND i.id_category IS NOT NULL, ',', ''), IFNULL(i.id_category, '')) AS Catid," +
                    //            " a.assessment_title,CASE WHEN a.assessment_type = 1 THEN 'M2OST' WHEN a.assessment_type = 3 THEN 'MT' WHEN a.assessment_type = 4 THEN 'WB'" +
                    //           " END AS TYPE, MAX(e.attempt_number) AS attempt_number,MAX(e.result_in_percentage) AS result_in_percentage,MAX(e.updated_date_time) AS updated_date_time" +
                    //        " FROM tbl_assessment a JOIN tbl_user b ON b.status = 'A' JOIN tbl_profile c ON b.id_user = c.id_user JOIN tbl_csst_role d ON b.id_role = d.id_csst_role" +
                    //        " JOIN tbl_rs_type_qna e ON a.id_assessment = e.id_assessment AND e.id_user = b.id_user  LEFT JOIN tbl_assessment_user_assignment g ON a.id_assessment = g.id_assessment AND g.id_user = b.id_user AND g.status = 'A'" +
                    //        " LEFT JOIN tbl_assessment_categoty_mapping h ON a.id_assessment = h.id_assessment AND h.status = 'A' LEFT JOIN tbl_category f ON g.id_category = f.id_category  LEFT JOIN tbl_category i ON h.id_category = i.id_category" +
                    //        " WHERE  a.id_organization = "+orgid+ "    AND a.status = 'A'  AND d.id_csst_role IN ("+ rolewise + ")  AND a.id_assessment IN (" + assessment_id + ")  AND d.csst_role NOT LIKE '%Spectator_%' AND b.USERID NOT LIKE '%FRN_%'" +
                    //          " AND b.userid NOT LIKE '%_SM_BA' AND c.OFFICE_ADDRESS NOT LIKE 'Pune' GROUP BY  Catname, userid ) AS assessment_data ON user_assigned.userid = assessment_data.userid AND user_assigned.categoryname = assessment_data.Catname" +
                    //" LEFT JOIN tbl_profile profile ON user_assigned.userid = profile.id_user LEFT JOIN tbl_user user ON user_assigned.userid = user.id_user  GROUP BY" +
                    //   " user_assigned.Region, user_assigned.userid,user_assigned.csst_role, user_assigned.categoryname, user_assigned.category_type, user_assigned.start_date, Course," +
                    //   " Read_Count, total_content_count, Adoption_Percent, profile.DATE_OF_JOINING, user.status, user_assigned.GENDER;";



                    using (var cmd = new MySqlCommand(query, connection))
                    {

                        using (var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                        {
                            while (await reader.ReadAsync().ConfigureAwait(false))
                            {
                                var result = new ExpandoObject() as IDictionary<string, object>;
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    result.Add(reader.GetName(i), reader.IsDBNull(i) ? null : reader.GetValue(i));
                                }
                                results.Add(result);
                            }
                        }

                        if (results.Count == 0)
                        {

                            var noDataMessage = new ExpandoObject() as IDictionary<string, object>;
                            noDataMessage.Add("Message", "No data found for the provided.");

                            results.Add(noDataMessage);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log or handle exception
                throw new Exception("An error occurred while processing the course data.", ex);
            }

            //  await _connection.CloseAsync().ConfigureAwait(false);
            return results;
        }


        //pyp query
        //public async Task<IEnumerable<dynamic>> PYPCourse(int orgid, string[] category, string[] rolewise, string startDate, string endDate, string[] regionwise, string[] user_designation, string[] usercity, string[] ID_category)
        //{
        //    DataTable results = new DataTable();
        //    DataTable results1 = new DataTable();
        //    var error = new List<dynamic>();
        //    var combinedResults = new List<dynamic>();


        //    string categoryValues = string.Join(",", category);
        //   // string assessment_idValues = string.Join(",", assessment_id);
        //    string rolewiseValues = string.Join(",", rolewise);
        //   // string ID_categoryValues = "'" + string.Join("', '", ID_category) + "'";  // Ensure IDs are in single quotes
        //    string ID_categoryValues =  string.Join(", ", ID_category) ;  // Ensure IDs are in single quotes
        //    string formattedValues = "'" + string.Join("', '", regionwise) + "'";
        //    string designationValues = "'" + string.Join("', '", user_designation) + "'";
        //    string cityValues = "'" + string.Join("', '", usercity) + "'";

        //    //List<string> searchTerms = new List<string> { ID_categoryValues };

        //    using (var connectionpyp = new MySqlConnection(ConfigurationManager.ConnectionStrings["dbconnectionstringpyp"].ConnectionString))
        //    {
        //        await connectionpyp.OpenAsync().ConfigureAwait(false);

        //        //await _connectionpyp.OpenAsync().ConfigureAwait(false);



        //        // Constructing the SQL query
        //        string query = "SELECT " +
        //            "given_by_user_id AS userid, " +
        //            "DATE(feedback_datetime) AS Submission_Date, " +
        //            "TIME(feedback_datetime) AS Submission_Time, " +
        //            "sub_type AS PYP_Category, " +
        //            "Well_Groomed, " +
        //            "Confidence_level, " +
        //            "subject_knowledge, " +
        //            "(Well_Groomed + Confidence_level + subject_knowledge) AS Score_Achieved, " +
        //            "15 AS Score_Total " +
        //            "FROM tbl_user_feedback " +
        //            "WHERE (";

        //        // Dynamically build the LIKE conditions for each search term
        //        for (int i = 0; i < ID_category.Length; i++)
        //        {
        //            if (i > 0)
        //            {
        //                query += " OR "; // Add 'OR' between multiple LIKE conditions
        //            }
        //            query += "sub_type LIKE @subType" + i;
        //        }

        //        // Add the date range condition
        //        query += ") AND DATE(feedback_datetime) BETWEEN '" + startDate + "' AND '" + endDate + "'";

        //        // Create the command and add parameters
        //        using (var cmd = new MySqlCommand(query, connectionpyp))
        //        {  // Add the search term parameters
        //            for (int i = 0; i < ID_category.Length; i++)
        //            {
        //                cmd.Parameters.AddWithValue("@subType" + i, "%" + ID_category[i] + "%");
        //            }




        //            using (var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
        //            {
        //                results.Load(reader);
        //            }
        //        }
            


        //        //await _connectionpyp.CloseAsync().ConfigureAwait(false);


        //        var userDetails = await UserDetailspyp(orgid, rolewise, regionwise, usercity, user_designation).ConfigureAwait(false);
        //        results1 = userDetails;

        //        var userDetailsDict = results1.AsEnumerable()
        //                .GroupBy(row => row["userid"].ToString())
        //                .ToDictionary(group => group.Key, group => group.First());

        //        var finalResults = new List<dynamic>();
        //        foreach (DataRow row in results.Rows)
        //        {
        //            var userId = row["userid"].ToString();

        //            // Try to get the matching row from results1 (userDetailsDict) by userid
        //            if (userDetailsDict.TryGetValue(userId, out DataRow matchedRow))
        //            {
        //                // Create a dynamic object to store the combined result
        //                var combinedResult = new ExpandoObject() as IDictionary<string, object>;

        //                // Add columns from the first result set (results)
        //                foreach (DataColumn col in results.Columns)
        //                {
        //                    combinedResult.Add(col.ColumnName, row[col]);
        //                }

        //                // Add columns from the second result set (results1)
        //                foreach (DataColumn col in results1.Columns)
        //                {
        //                    // Ensure no duplicate column names
        //                    if (!combinedResult.ContainsKey(col.ColumnName))
        //                    {
        //                        combinedResult.Add(col.ColumnName, matchedRow[col]);
        //                    }
        //                }
        //               combinedResults.Add(combinedResult);

     
                      
        //            }
        //        }
        //        if (combinedResults.Count == 0)
        //        {
        //            var noDataMessage = new ExpandoObject() as IDictionary<string, object>;
        //            noDataMessage.Add("Message", "No data found for the provided parameters.");
        //            error.Add(noDataMessage);
        //            return error;
        //        }

        //        return combinedResults;
        //    }

        //}
        public async Task<IEnumerable<dynamic>> PYPCourse(int orgid, string[] category, string[] rolewise, string startDate, string endDate, string[] regionwise, string[] user_designation, string[] usercity, string[] ID_category)
        {
            DataTable results = new DataTable();
            DataTable results1 = new DataTable();
            var error = new List<dynamic>();
            var combinedResults = new List<dynamic>();

            string ID_categoryValues = string.Join(", ", ID_category);  // IDs to be used in the query
            string formattedValues = "'" + string.Join("', '", regionwise) + "'";
            string designationValues = "'" + string.Join("', '", user_designation) + "'";
            string cityValues = "'" + string.Join("', '", usercity) + "'";

            try
            {
                using (var connectionpyp = new MySqlConnection(ConfigurationManager.ConnectionStrings["dbconnectionstringpyp"].ConnectionString))
                {
                    await connectionpyp.OpenAsync().ConfigureAwait(false);

                    // Constructing the SQL query
                    string query = "SELECT " +
                        "given_by_user_id AS userid, " +
                        "DATE(feedback_datetime) AS Submission_Date, " +
                        "TIME(feedback_datetime) AS Submission_Time, " +
                        "sub_type AS PYP_Category, " +
                        "Well_Groomed, " +
                        "Confidence_level, " +
                        "subject_knowledge, " +
                        "(Well_Groomed + Confidence_level + subject_knowledge) AS Score_Achieved, " +
                        "15 AS Score_Total " +
                        "FROM tbl_user_feedback " +
                        "WHERE (";

                    // Dynamically build the LIKE conditions for each search term
                    for (int i = 0; i < ID_category.Length; i++)
                    {
                        if (i > 0)
                        {
                            query += " OR "; // Add 'OR' between multiple LIKE conditions
                        }
                        query += "sub_type LIKE @subType" + i;
                    }

                    // Add the date range condition
                    query += ") AND DATE(feedback_datetime) BETWEEN @startDate AND @endDate";

                    // Create the command and add parameters
                    using (var cmd = new MySqlCommand(query, connectionpyp))
                    {
                        // Add the search term parameters
                        for (int i = 0; i < ID_category.Length; i++)
                        {
                            cmd.Parameters.AddWithValue("@subType" + i, "%" + ID_category[i] + "%");
                        }

                        // Add the date range parameters safely
                        cmd.Parameters.AddWithValue("@startDate", DateTime.Parse(startDate));
                        cmd.Parameters.AddWithValue("@endDate", DateTime.Parse(endDate));

                        using (var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                        {
                            results.Load(reader);
                        }
                    }

                    // Fetching the user details (UserDetailspyp)
                    var userDetails = await UserDetailspyp(orgid, rolewise, regionwise, usercity, user_designation).ConfigureAwait(false);
                    results1 = userDetails;

                    // Creating a dictionary for fast lookup by userid
                    var userDetailsDict = results1.AsEnumerable()
                        .GroupBy(row => row["userid"].ToString())
                        .ToDictionary(group => group.Key, group => group.First());

                    var finalResults = new List<dynamic>();
                    foreach (DataRow row in results.Rows)
                    {
                        var userId = row["userid"].ToString();

                        // Try to get the matching row from userDetails (results1)
                        if (userDetailsDict.TryGetValue(userId, out DataRow matchedRow))
                        {
                            // Create a dynamic object to store the combined result
                            var combinedResult = new ExpandoObject() as IDictionary<string, object>;

                            // Add columns from the first result set (results)
                            foreach (DataColumn col in results.Columns)
                            {
                                combinedResult.Add(col.ColumnName, row[col]);
                            }

                            // Add columns from the second result set (results1)
                            foreach (DataColumn col in results1.Columns)
                            {
                                if (!combinedResult.ContainsKey(col.ColumnName)) // Avoid duplicate column names
                                {
                                    combinedResult.Add(col.ColumnName, matchedRow[col]);
                                }
                            }

                            combinedResults.Add(combinedResult);
                        }
                    }

                    if (combinedResults.Count == 0)
                    {
                        var noDataMessage = new ExpandoObject() as IDictionary<string, object>;
                        noDataMessage.Add("Message", "No data found for the provided parameters.");
                        error.Add(noDataMessage);
                        return error;
                    }

                    return combinedResults;
                }
            }
            catch (Exception ex)
            {
                // Log or handle exception
                throw new Exception("An error occurred while processing the PYP course data.", ex);
            }
            finally
            {
                // Ensure the connection is closed
                if (_connectionpyp.State == ConnectionState.Open)
                {
                    _connectionpyp.Close(); // Explicitly close the connection
                }
                _connectionpyp.Dispose(); // Dispose of the connection object
            }
        }

        //pyp

        public async Task<DataTable> UserDetailspyp(int orgid, string[] rolewise, string[] regionwise, string[] usercity, string[] user_designation)
        {
            DataTable results = new DataTable();

          
            var rolewiseParams = string.Join(", ", rolewise.Select((role, i) => $"@rolewise{i}"));

            var regionwiseArrayValues = string.Join(", ", regionwise.Select(region => $"'{region.Trim()}'"));
            var usercityArrayValues = string.Join(", ", usercity.Select(city => $"'{city.Trim()}'"));
            var user_designationArrayValues = string.Join(", ", user_designation.Select(userDesig => $"'{userDesig.Trim()}'"));

            try
            {
                //if (_connection.State != ConnectionState.Open)
                //{
                //    await _connection.OpenAsync().ConfigureAwait(false);
                //}
                using (var connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["dbconnectionstring"].ConnectionString))
                {
                    await connection.OpenAsync().ConfigureAwait(false);

                    string query = $@"
                SELECT 
                 a.userid,
                 g.FIRSTNAME,
                 g.LASTNAME,
                 g.OFFICE_ADDRESS,
                 g.gender,
	             c.csst_role AS Role,
                 a.user_grade AS Store_code,
                 g.city,
                 g.DATE_OF_JOINING,
                 g.LOCATION as Store_Name,
                 a.user_department,
                 a.user_designation,
                 a.user_function as Store_type,
                 a.L3 as DM,
                 a.L2 as RM,
                 a.L1 as GM,
                 a.Spectator as RTM
                FROM 
                    tbl_user a
                LEFT JOIN tbl_user b ON a.reporting_manager = b.id_user
                LEFT JOIN tbl_csst_role c ON a.id_role = c.id_csst_role
                LEFT JOIN tbl_profile g ON a.id_user = g.id_user
                JOIN tbl_department o ON c.id_department = o.id_department
                WHERE 
                a.id_organization = @orgid 
            
                AND c.id_csst_role IN ({rolewiseParams})
                AND g.office_address IN ({regionwiseArrayValues})
                AND g.city IN ({usercityArrayValues})
                AND a.user_designation IN ({user_designationArrayValues})
                AND a.userid NOT LIKE '%-old%'
                AND b.userid NOT LIKE '%Bata%'
                AND b.user_status ='A'
        ";

                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@orgid", orgid);



                        for (int i = 0; i < rolewise.Length; i++)
                        {
                            cmd.Parameters.AddWithValue($"@rolewise{i}", rolewise[i]);
                        }

                        using (var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                        {
                            results.Load(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exception, log it, and provide a custom error message if needed
                throw new Exception("An error occurred while fetching user details.", ex);
            }


            return results;
        }


        //Skillmuni
        public List<object> categoryheading(int orgid ,string year, string monthwise)
        {
            //var monthwises = monthwise.Split(',').Select(id => id.Trim()).ToArray();
            //var monthwiseParams = string.Join(", ", monthwises.Select((s, i) => $"@monthwise{i}"));

            List<object> list = new List<object>();

            try
            {

                using (var connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["dbconnectionstring"].ConnectionString))
                {
                    connection.Open();

                    string query = $@"SELECT DISTINCT heading.id_category_heading,heading.Heading_title
                                FROM tbl_category_heading AS heading
                                JOIN tbl_content_program_mapping AS tiles
                                ON heading.id_category_heading = tiles.id_category_heading
                                WHERE tiles.id_organization = @OrgId
                                AND year(tiles.updated_date_time) in (@year) and MONTH(tiles.updated_date_time) IN(@monthwise)";

                    //string query = @"SELECT * from tbl_region where orgid = " + orgid + " ";
                    //string query = @"SELECT * FROM tbl_category_heading AS heading
                    //                    JOIN tbl_category_tiles AS tiles
                    //                    ON heading.id_category_tiles = tiles.id_category_tiles
                    //                    WHERE tiles.id_organization = @OrgId;";

                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@OrgId", orgid);
                        cmd.Parameters.AddWithValue("@year", year);
                        cmd.Parameters.AddWithValue("@monthwise", monthwise);

                        //for (int i = 0; i < monthwises.Length; i++)
                        //{
                        //    cmd.Parameters.AddWithValue($"@monthwise{i}", monthwises[i]);
                        //}

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var item = new
                                {
                                    Text = reader["Heading_title"].ToString(),
                                    Value = reader["id_category_heading"].ToString()
                                };

                                list.Add(item);
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                // Handle the exception (log it, throw it, etc.)
                Console.WriteLine("An error occurred: " + ex.Message); // For example purposes
            }
            finally
            {
                _connection.Close();
            }


            return list;
        }



        //Skillmuni for userdesignation
        public List<object> Getuserdesignation(int orgid, string[] roleId)
        {
            List<object> list = new List<object>();

            try
            {
                using (var connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["dbconnectionstring"].ConnectionString))
                {
                    connection.Open();

                    //string query = @"SELECT * from tbl_region where orgid = " + orgid + " ";
                    string query = $@"SELECT DISTINCT user_designation 
                         FROM tbl_user u
                         INNER JOIN tbl_csst_role r ON u.id_role = r.id_csst_role
                         WHERE u.ID_ORGANIZATION = @OrgId
                         AND r.id_csst_role IN ({string.Join(", ", roleId.Select((s, i) => $"@roleId{i}"))})
                         AND u.user_designation NOT LIKE '%Content Creator%' 
                         AND u.user_designation NOT LIKE '%Developer%'
                         AND u.user_designation NOT IN ('Tester', 'Testing', 'HOD', 'Player', 'TL', 'Gamification Specialist', 'NA')
                         AND u.user_designation IS NOT NULL;";

                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@OrgId", orgid);

                        for (int i = 0; i < roleId.Length; i++)
                        {
                            cmd.Parameters.AddWithValue($"@roleId{i}", roleId[i]);
                        }

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var item = new
                                {
                                    Text = reader["user_designation"].ToString(),
                                    Value = reader["user_designation"].ToString()
                                };

                                list.Add(item);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle the exception (log it, throw it, etc.)
                Console.WriteLine("An error occurred: " + ex.Message); // For example purposes
            }
            finally
            {
                _connection.Close();
            }

            return list;
        }

        //Skillmuni for usercity
        public List<object> Getusercity(int orgid, string[] regionwise)
        {
            List<object> list = new List<object>();

            string formattedValues = "'" + string.Join("', '", regionwise) + "'";


            try
            {
                using (var connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["dbconnectionstring"].ConnectionString))
                {
                    connection.Open();

                    //string query = @"SELECT * from tbl_region where orgid = " + orgid + " ";
                    //string query = @"SELECT distinct tbl_profile.CITY
                    //                FROM tbl_profile
                    //                INNER JOIN tbl_user ON tbl_user.ID_USER = tbl_profile.ID_USER
                    //                WHERE tbl_user.ID_ORGANIZATION = @OrgId and status ='A' and USERID not like ""%Bata%""";

                    string query = $@"SELECT DISTINCT tbl_profile.CITY
                         FROM tbl_profile
                         INNER JOIN tbl_user u ON u.ID_USER = tbl_profile.ID_USER
                         INNER JOIN tbl_csst_role r ON u.id_role = r.id_csst_role
                         WHERE u.ID_ORGANIZATION = @OrgId 
                         AND u.status = 'A'
                        
                         AND tbl_profile.OFFICE_ADDRESS IN ({formattedValues})

                         
                         AND u.USERID NOT LIKE '%Bata%'";

                    //AND r.id_csst_role IN({ string.Join(", ", roleId.Select((s, i) => $"@roleId{i}"))})

                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@OrgId", orgid);



                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var item = new
                                {
                                    Text = reader["CITY"].ToString(),
                                    Value = reader["CITY"].ToString()
                                };

                                list.Add(item);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle the exception (log it, throw it, etc.)
                Console.WriteLine("An error occurred: " + ex.Message); // For example purposes
            }
            finally
            {
                _connection.Close();
            }

            return list;
        }

        //Negage 
        public List<object> AssessmentQuestionList(int orgid)
        {
            List<object> list = new List<object>();

            try
            {
                using (var connectionNgage = new MySqlConnection(ConfigurationManager.ConnectionStrings["db_tgc_gameEntities"].ConnectionString))
                {
                    connectionNgage.Open();


                    //string query = @"SELECT * from tbl_region where orgid = " + orgid + " ";
                    string query = @"Select * from tbl_assessment_question where ID_ORGANIZATION = @OrgId ";

                    using (MySqlCommand cmd = new MySqlCommand(query, connectionNgage))
                    {
                        cmd.Parameters.AddWithValue("@OrgId", orgid);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var item = new
                                {
                                    Text = reader["Assessment_Question"].ToString(),
                                    Value = reader["Id_Assessment_question"].ToString()
                                };

                                list.Add(item);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle the exception (log it, throw it, etc.)
                Console.WriteLine("An error occurred: " + ex.Message); // For example purposes
            }
            finally
            {
                _connection.Close();
            }

            return list;
        }

        public List<object> DDKtitle(string orgid,string year , string month)
        {
            List<object> list = new List<object>();

            try
            {
                using (var connectionCoroebus = new MySqlConnection(ConfigurationManager.ConnectionStrings["db_tgc_corobus"].ConnectionString))
                {
                    connectionCoroebus.Open();

                    //string query = @"SELECT * from tbl_region where orgid = " + orgid + " ";
                    string query = @" SELECT id_learning_academy_brief,brief_title
                                 FROM tbl_learning_academy_brief 
                                WHERE id_coroebus_organization IN (" + orgid + ") AND YEAR(created_date)IN (" + year + ")AND MONTH(created_date) IN (" + month + ")AND STATUS='A'";

                    using (MySqlCommand cmd = new MySqlCommand(query, connectionCoroebus))
                    {


                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var item = new
                                {
                                    Text = reader["brief_title"].ToString(),
                                    Value = reader["id_learning_academy_brief"].ToString()
                                };

                                list.Add(item);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle the exception (log it, throw it, etc.)
                Console.WriteLine("An error occurred: " + ex.Message); // For example purposes
            }
            finally
            {
                _connection.Close();
            }

            return list;
        }

    }
}
