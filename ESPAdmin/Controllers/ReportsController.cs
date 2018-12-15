using ESPAdmin.Models;
using ESPAdmin.Utility;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ESPAdmin.Controllers
{
    public class ReportsController : Controller
    {
        // GET: Reports
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetSexRatio()
        {
            string userkey = ConfigurationManager.AppSettings["userkey"];
            string uid = ConfigurationManager.AppSettings["uid"];
            string LoginUser = (string)Session["LoginSAPID"];

            try
            {

                Employer.Employer employer = new Employer.Employer();
                DataTable dt = employer.FetchCompanYEmployees("1002", userkey, uid);
                dt.TableName = "ExternalUsers";

                dt.Columns.ToString();

                var employeeCount = (from DataRow dr in dt.Rows
                                    select new
                                    {
                                        PIN = dr["P_I_N"].ToString()
                                    }).ToList();

                var totalCount = employeeCount.Count();

                var sexCount = (from DataRow dr in dt.Rows
                                group dr by dr["SEX"] into g
                                select new  
                                {
                                    Mid = g.Key,
                                    Count = g.Count()
                                }).ToList();

                var stateCount = (from DataRow dr in dt.Rows
                                group dr by dr["State of Origin"] into g
                                select new
                                {
                                    Mid = g.Key,
                                    Count = g.Count()
                                }).ToList();

                var religionCount = (from DataRow dr in dt.Rows
                                      group dr by dr["Religion"] into g
                                      select new
                                      {
                                          Mid = g.Key,
                                          Count = g.Count()
                                      }).ToList();

                var maritalStatus = (from DataRow dr in dt.Rows
                                       group dr by dr["Marital Status"] into g
                                       select new
                                       {
                                           Mid = g.Key,
                                           Count = g.Count()
                                       }).ToList();

                var statementOption = (from DataRow dr in dt.Rows
                                        group dr by dr["Statement Option"] into g
                                        select new
                                        {
                                            Mid = g.Key,
                                            Count = g.Count()
                                        }).ToList();

                return Json(new { sex = sexCount, state = stateCount, religion = religionCount, statement = statementOption, marital = maritalStatus }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogError logerror = new LogError();
                logerror.ErrorLog("", LoginUser, "", "Requests/GetRequestCategory", "Requests", "GetRequestCategory", "FetchRequestCategories Error", ex.Message.ToString(), 0);
                return Json(new { data = "Error has occured" }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetMaritalStatusRatio()
        {
            string userkey = ConfigurationManager.AppSettings["userkey"];
            string uid = ConfigurationManager.AppSettings["uid"];
            string LoginUser = (string)Session["LoginSAPID"];

            try
            {

                Employer.Employer employer = new Employer.Employer();
                DataTable dt = employer.FetchCompanYEmployees("1002", userkey, uid);
                dt.TableName = "ExternalUsers";

                dt.Columns.ToString();
                var requestCategory = (from DataRow dr in dt.Rows
                                           //where dr["eid"].ToString() == "113"
                                       group dr by dr["Marital Status"] into g
                                       select new
                                       {
                                           Mid = g.Key,
                                           Count = g.Count()
                                       }).ToList();

                return Json(new { data = requestCategory }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogError logerror = new LogError();
                logerror.ErrorLog("", LoginUser, "", "Requests/GetRequestCategory", "Requests", "GetRequestCategory", "FetchRequestCategories Error", ex.Message.ToString(), 0);
                return Json(new { data = "Error has occured" }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetStateOriginRatio()
        {
            string userkey = ConfigurationManager.AppSettings["userkey"];
            string uid = ConfigurationManager.AppSettings["uid"];
            string LoginUser = (string)Session["LoginSAPID"];

            try
            {

                Employer.Employer employer = new Employer.Employer();
                DataTable dt = employer.FetchCompanYEmployees("1002", userkey, uid);
                dt.TableName = "ExternalUsers";

                dt.Columns.ToString();
                var requestCategory = (from DataRow dr in dt.Rows
                                           //where dr["eid"].ToString() == "113"
                                       group dr by dr["State of Origin"] into g
                                       select new
                                       {
                                           Mid = g.Key,
                                           Count = g.Count()
                                       }).ToList();

                return Json(new { data = requestCategory }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogError logerror = new LogError();
                logerror.ErrorLog("", LoginUser, "", "Requests/GetRequestCategory", "Requests", "GetRequestCategory", "FetchRequestCategories Error", ex.Message.ToString(), 0);
                return Json(new { data = "Error has occured" }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}