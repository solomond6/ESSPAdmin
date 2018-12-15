using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq.Dynamic;
using ESPAdmin.Models;
using DataTables.Mvc;
using ESPAdmin.Utility;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace ESPAdmin.Controllers
{
    public class ReturnedScheduleController : Controller
    {
        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            var application = sender as HttpApplication;
            if (application != null && application.Context != null)
            {
                application.Context.Response.Headers.Remove("Server");
            }

            Response.AddHeader("x-frame-options", "DENY");
        }

        [Authorize]
        public ActionResult Index()
        {
            string LoginUser = (string)Session["LoginSAPID"];
            string EmployerId = (string)Session["EMPLOYER_ID"];
            string WebUserID = (string)Session["WebUserID"];
            string RoleID = (string)Session["RoleID"];
            ViewBag.WebuserID = WebUserID;
            return View();
        }


        [Authorize]
        public ActionResult Create()
        {
            string LoginUser = (string)Session["LoginSAPID"];
            string EmployerId = (string)Session["EMPLOYER_ID"];
            string WebUserID = (string)Session["WebUserID"];
            string RoleID = (string)Session["RoleID"];
            ViewBag.WebuserID = WebUserID;
            return View();
        }

        [Authorize]
        public ActionResult MyRequest()
        {
            string LoginUser = (string)Session["LoginSAPID"];
            string EmployerId = (string)Session["EMPLOYER_ID"];
            string WebUserID = (string)Session["WebUserID"];
            string RoleID = (string)Session["RoleID"];
            ViewBag.WebuserID = WebUserID;
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string EmployerCode, string requestCategory, string Comment, string convertedFile, string convertedFile1, string convertedFile2, string classification, List<string> reasons, string narration, string AmountSchedule, string AmountReturned)
        {
            string UploadStatus = "";
            string UploadStatus1 = "";
            string UploadStatus2 = "";
            //convertedFile = "";
            //convertedFile1 = "";
            //convertedFile2 = "";

            string ipaddress = Request.UserHostAddress;
            string Agent = Request.UserAgent;
            string BrowserUsed = Request.Browser.Browser;
            string SessionID = HttpContext.Session.SessionID;

            string userkey = ConfigurationManager.AppSettings["userkey"];
            string uid = ConfigurationManager.AppSettings["uid"];

            string LoginUser = (string)Session["LoginSAPID"];
            string EmployerId = (string)Session["EMPLOYER_ID"];
            string WebUserID = (string)Session["WebUserID"];
            string RoleID = (string)Session["RoleID"];

            try
            {
                Employer.Employer employer = new Employer.Employer();
                var requestStatus = employer.CreateRequest(WebUserID, RoleID, requestCategory, "Initiated", Comment, userkey, uid);
                var requestDetails = requestStatus.Split('~');
                
                if (requestDetails[0] != "01")
                {
                    var file = Request.Files;

                    if (file[0].ContentLength > 0)
                    {
                        string _FileName = file[0].FileName;
                        string Docext = Path.GetExtension(file[0].FileName);
                        string Doc = convertedFile;
                        UploadStatus = employer.LogRequestDoc(requestDetails[1], "", Doc, Docext, _FileName, "Y", userkey, uid);
                        var uploadDetails = UploadStatus.Split('~');
                    }
                    if (file[1].ContentLength > 0)
                    {
                        string _FileName1 = file[1].FileName;
                        string Docext1 = Path.GetExtension(file[1].FileName);
                        string Doc1 = convertedFile1;
                        UploadStatus1 = employer.LogRequestDoc(requestDetails[1], "", Doc1, Docext1, _FileName1, "Y", userkey, uid);
                        //var uploadDetails1 = UploadStatus1.Split('~');
                    }
                    if (file[2].ContentLength > 0)
                    {
                        string _FileName2 = file[2].FileName;
                        string Docext2 = Path.GetExtension(file[2].FileName);
                        string Doc2 = convertedFile2;
                        UploadStatus2 = employer.LogRequestDoc(requestDetails[1], "", Doc2, Docext2, _FileName2, "Y", userkey, uid);
                        //uploadDetails2 = UploadStatus2.Split('~');
                    }

                    int ReasonCount = reasons.Count();

                    var appendedScheduleStatus = employer.AppendScheduleRequest(EmployerCode, classification, AmountReturned, AmountSchedule, narration, requestDetails[1], "", userkey, uid);


                    for (int i = 0; i < ReasonCount; i++)
                    {
                        var appendedReasonStatus = employer.AppendScheduleRequestError(requestDetails[1], "", reasons[i], userkey, uid);
                    }

                    if (UploadStatus == "00~Your documents have been logged successfully" || UploadStatus1 == "00~Your documents have been logged successfully" || UploadStatus2 == "00~Your documents have been logged successfully")
                    {
                        employer.NotifyIncident(requestDetails[1], userkey, uid);
                        string status = "00~Your documents have been logged successfully";
                        var statusDetails = status.Split('~');
                        TempData["msg"] = "Your request and " + statusDetails[1];
                        //employer
                        if (RoleID == "1" || RoleID == "2")
                        {
                            TempData["msg"] = requestDetails[2];
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            TempData["msg"] = requestDetails[2];
                            return RedirectToAction("MyRequest");
                        }
                    }
                    else
                    {
                        
                    }

                    if (RoleID == "1" || RoleID == "2")
                    {
                        TempData["msg"] = requestDetails[2];
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["msg"] = requestDetails[2];
                        return RedirectToAction("MyRequest");
                    }
                }
                else
                {
                    TempData["error"] = requestDetails[2];
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                LogError logerror = new LogError();
                logerror.ErrorLog("", LoginUser, "", "ReturnedSchedule/Create", "ReturnedSchedule", "Create", "CreateRequest Error", ex.InnerException.Message.ToString(), 0);
                TempData["error"] = "An error processing complaints";
                return View();
            }
        }


        [Authorize]
        public JsonResult GetRequests([ModelBinder(typeof(DataTablesBinder))] IDataTablesRequest requestModel, String EmployerId)
        {

            string userkey = ConfigurationManager.AppSettings["userkey"];
            string uid = ConfigurationManager.AppSettings["uid"];
            string LoginUser = (string)Session["LoginSAPID"];
            string _access_key = ConfigurationManager.AppSettings["Salt"];

            try
            {
                Employer.Employer employer = new Employer.Employer();
                DataTable dt = employer.FetchIncidents("1", "N/A", "N/A", userkey, uid);

                //DataTable dts = employer.FetchAssignmentHistory("ESSP-0000000003", userkey, uid);

                dt.TableName = "Requests";
                dt.Columns.ToString();
                List<Requests> requests = new List<Requests>();
                int startRec = requestModel.Start;
                int pageSize = requestModel.Length;

                List<Requests> requestCount = (from DataRow dr in dt.Rows
                                               where dr["Cat"].ToString() == "Returned Schedule"
                                               select new Requests()
                                               {
                                                   RequestID = dr["RequestID"].ToString()
                                               }).ToList();

               requests = (from DataRow dr in dt.Rows
                           where dr["Cat"].ToString() == "Returned Schedule"
                           orderby dr["Datecreated"] descending
                            select new Requests()
                            {
                                RequestID = dr["RequestID"].ToString(),
                                Creator = dr["Creator"].ToString(),
                                Category = dr["Cat"].ToString(),
                                Comment = dr["Comment"].ToString(),
                                Datecreated = Convert.ToDateTime(dr["Datecreated"]).ToString("dd-MMM-yyyy hh:mm"),
                                CurrentStatus = dr["CurrentStatus"].ToString(),
                                CurrentAssignedToName = dr["CurrentAssignedToName"].ToString(),
                                CurrentAssignedToID = dr["CurrentAssignedToID"].ToString()
                            }).Skip(startRec).Take(pageSize).ToList();

                var totalCount = requestCount.Count();
                var filteredCount = requests.Count();

                if (requestModel.Search.Value != string.Empty)
                {
                    var value = requestModel.Search.Value.ToLower().Trim();

                    requestCount = (from DataRow dr in dt.Rows
                                    where dr["Cat"].ToString() == "Returned Schedule"
                                    where dr["RequestID"].ToString().ToLower().Contains(value) || dr["Cat"].ToString().ToLower().Contains(value)
                                    || dr["Creator"].ToString().ToLower().Contains(value) || Convert.ToDateTime(dr["Datecreated"]).ToString("dd-MMM-yyyy hh:mm").ToLower().Contains(value) 
                                    || dr["CurrentStatus"].ToString().ToLower().Contains(value) || dr["CurrentAssignedToName"].ToString().ToLower().Contains(value)
                                    select new Requests()
                                    {
                                        RequestID = dr["RequestID"].ToString()
                                    }).ToList();

                    requests = (from DataRow dr in dt.Rows
                                orderby dr["Datecreated"] descending
                                where dr["Cat"].ToString() == "Returned Schedule"
                                where dr["RequestID"].ToString().ToLower().Contains(value) || dr["Cat"].ToString().ToLower().Contains(value)
                                    || dr["Creator"].ToString().ToLower().Contains(value) || Convert.ToDateTime(dr["Datecreated"]).ToString("dd-MMM-yyyy hh:mm").ToLower().Contains(value)
                                    || dr["CurrentStatus"].ToString().ToLower().Contains(value) || dr["CurrentAssignedToName"].ToString().ToLower().Contains(value)
                                select new Requests()
                                {
                                    RequestID = dr["RequestID"].ToString(),
                                    Category = dr["Cat"].ToString(),
                                    Creator = dr["Creator"].ToString(),
                                    Comment = dr["Comment"].ToString(),
                                    Datecreated = Convert.ToDateTime(dr["Datecreated"]).ToString("dd-MMM-yyyy hh:mm"),
                                    CurrentStatus = dr["CurrentStatus"].ToString(),
                                    CurrentAssignedToName = dr["CurrentAssignedToName"].ToString(),
                                    CurrentAssignedToID = dr["CurrentAssignedToID"].ToString()
                                }).Skip(startRec).Take(pageSize).ToList();

                    totalCount = requestCount.Count();
                    filteredCount = requests.Count();
                }

                var sortedColumns = requestModel.Columns.GetSortedColumns();
                var orderByString = String.Empty;

                foreach (var column in sortedColumns)
                {
                    orderByString += orderByString != String.Empty ? "," : "";
                    orderByString += (column.Data) + (column.SortDirection == Column.OrderDirection.Ascendant ? " asc" : " desc");
                }

                //requests = requests.OrderBy(orderByString == string.Empty ? "asc" : orderByString);

                var data = requests.Select(emList => new
                {
                    RequestID = emList.RequestID,
                    Category = emList.Category,
                    Creator = emList.Creator,
                    Comment = emList.Comment,
                    Datecreated = emList.Datecreated,
                    CurrentStatus = emList.CurrentStatus,
                    CurrentAssignedToName = emList.CurrentAssignedToName,
                    CurrentAssignedToID = emList.CurrentAssignedToID
                }).ToList();

                return Json(new DataTablesResponse(requestModel.Draw, data, totalCount, totalCount), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogError logerror = new LogError();
                logerror.ErrorLog("", LoginUser, "", "ReturnedSchedule/GetRequests", "ReturnedSchedule", "GetRequests", "FetchIncidents Error", ex.Message.ToString(), 0);
                return Json(new { data = "Error has occured" }, JsonRequestBehavior.AllowGet);
            }
        }


        [Authorize]
        public JsonResult GetMyRequests([ModelBinder(typeof(DataTablesBinder))] IDataTablesRequest requestModel, String EmployerId)
        {

            string userkey = ConfigurationManager.AppSettings["userkey"];
            string uid = ConfigurationManager.AppSettings["uid"];
            string LoginUser = (string)Session["LoginSAPID"];
            string _access_key = ConfigurationManager.AppSettings["Salt"];
            string WebUserID = (string)Session["WebUserID"];
            string RoleID = (string)Session["RoleID"];

            try
            {
                Employer.Employer employer = new Employer.Employer();
                DataTable dt = employer.FetchIncidents("3", WebUserID, RoleID, userkey, uid);

                //DataTable dts = employer.FetchAssignmentHistory("ESSP-0000000003", userkey, uid);

                dt.TableName = "Requests";
                dt.Columns.ToString();
                List<Requests> requests = new List<Requests>();
                int startRec = requestModel.Start;
                int pageSize = requestModel.Length;

                List<Requests> requestCount = (from DataRow dr in dt.Rows
                                               where dr["Cat"].ToString() == "Returned Schedule"
                                               select new Requests()
                                               {
                                                   RequestID = dr["RequestID"].ToString()
                                               }).ToList();

                requests = (from DataRow dr in dt.Rows
                            where dr["Cat"].ToString() == "Returned Schedule"
                            orderby dr["Datecreated"] descending
                            select new Requests()
                            {
                                RequestID = dr["RequestID"].ToString(),
                                Creator = dr["Creator"].ToString(),
                                Category = dr["Cat"].ToString(),
                                Comment = dr["Comment"].ToString(),
                                Datecreated = Convert.ToDateTime(dr["Datecreated"]).ToString("dd-MMM-yyyy hh:mm"),
                                CurrentStatus = dr["CurrentStatus"].ToString(),
                                CurrentAssignedToID = dr["CurrentAssignedToID"].ToString()
                            }).Skip(startRec).Take(pageSize).ToList();

                var totalCount = requestCount.Count();
                var filteredCount = requests.Count();

                if (requestModel.Search.Value != string.Empty)
                {
                    var value = requestModel.Search.Value.ToLower().Trim();

                    requestCount = (from DataRow dr in dt.Rows
                                    where dr["Cat"].ToString() == "Returned Schedule"
                                    where dr["RequestID"].ToString().ToLower().Contains(value) || dr["Cat"].ToString().ToLower().Contains(value)
                                    || dr["Creator"].ToString().ToLower().Contains(value) || Convert.ToDateTime(dr["Datecreated"]).ToString("dd-MMM-yyyy hh:mm").ToLower().Contains(value)
                                    || dr["CurrentStatus"].ToString().ToLower().Contains(value)
                                    select new Requests()
                                    {
                                        RequestID = dr["RequestID"].ToString()
                                    }).ToList();

                    requests = (from DataRow dr in dt.Rows
                                where dr["Cat"].ToString() == "Returned Schedule"
                                orderby dr["Datecreated"] descending
                                where dr["RequestID"].ToString().ToLower().Contains(value) || dr["Cat"].ToString().ToLower().Contains(value)
                                    || dr["Creator"].ToString().ToLower().Contains(value) || Convert.ToDateTime(dr["Datecreated"]).ToString("dd-MMM-yyyy hh:mm").ToLower().Contains(value)
                                    || dr["CurrentStatus"].ToString().ToLower().Contains(value)
                                select new Requests()
                                {
                                    RequestID = dr["RequestID"].ToString(),
                                    Category = dr["Cat"].ToString(),
                                    Creator = dr["Creator"].ToString(),
                                    Comment = dr["Comment"].ToString(),
                                    Datecreated = Convert.ToDateTime(dr["Datecreated"]).ToString("dd-MMM-yyyy hh:mm"),
                                    CurrentStatus = dr["CurrentStatus"].ToString(),
                                    CurrentAssignedToID = dr["CurrentAssignedToID"].ToString()
                                }).Skip(startRec).Take(pageSize).ToList();

                    totalCount = requestCount.Count();
                    filteredCount = requests.Count();
                }

                var sortedColumns = requestModel.Columns.GetSortedColumns();
                var orderByString = String.Empty;

                foreach (var column in sortedColumns)
                {
                    orderByString += orderByString != String.Empty ? "," : "";
                    orderByString += (column.Data) + (column.SortDirection == Column.OrderDirection.Ascendant ? " asc" : " desc");
                }

                //requests = requests.OrderBy(orderByString == string.Empty ? "asc" : orderByString);

                var data = requests.Select(emList => new
                {
                    RequestID = emList.RequestID,
                    Category = emList.Category,
                    Creator = emList.Creator,
                    Comment = emList.Comment,
                    Datecreated = emList.Datecreated,
                    CurrentStatus = emList.CurrentStatus,
                    CurrentAssignedToID = emList.CurrentAssignedToID
                }).ToList();

                return Json(new DataTablesResponse(requestModel.Draw, data, totalCount, totalCount), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogError logerror = new LogError();
                logerror.ErrorLog("", LoginUser, "", "ReturnedSchedule/GetMyRequests", "ReturnedSchedule", "GetMyRequests", "FetchIncidents Error", ex.Message.ToString(), 0);
                return Json(new { data = "Error has occured" }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize]
        public JsonResult GetUnAssignRequests([ModelBinder(typeof(DataTablesBinder))] IDataTablesRequest requestModel, String EmployerId)
        {

            string userkey = ConfigurationManager.AppSettings["userkey"];
            string uid = ConfigurationManager.AppSettings["uid"];
            string LoginUser = (string)Session["LoginSAPID"];
            string _access_key = ConfigurationManager.AppSettings["Salt"];
            string WebUserID = (string)Session["WebUserID"];
            string RoleID = (string)Session["RoleID"];

            try
            {
                Employer.Employer employer = new Employer.Employer();
                DataTable dt = employer.FetchIncidents("2", "N/A", "N/A", userkey, uid);

                //DataTable dts = employer.FetchAssignmentHistory("ESSP-0000000003", userkey, uid);

                dt.TableName = "Requests";
                dt.Columns.ToString();
                List<Requests> requests = new List<Requests>();
                int startRec = requestModel.Start;
                int pageSize = requestModel.Length;

                List<Requests> requestCount = (from DataRow dr in dt.Rows
                                               where dr["Cat"].ToString() == "Returned Schedule"
                                               select new Requests()
                                               {
                                                   RequestID = dr["RequestID"].ToString()
                                               }).ToList();

                requests = (from DataRow dr in dt.Rows
                            where dr["Cat"].ToString() == "Returned Schedule"
                            orderby dr["Datecreated"] descending
                            select new Requests()
                            {
                                RequestID = dr["RequestID"].ToString(),
                                Creator = dr["Creator"].ToString(),
                                Category = dr["Cat"].ToString(),
                                Comment = dr["Comment"].ToString(),
                                Datecreated = Convert.ToDateTime(dr["Datecreated"]).ToString("dd-MMM-yyyy hh:mm"),
                                CurrentStatus = dr["CurrentStatus"].ToString(),
                                CurrentAssignedToID = dr["CurrentAssignedToID"].ToString()
                            }).Skip(startRec).Take(pageSize).ToList();

                var totalCount = requestCount.Count();
                var filteredCount = requests.Count();

                if (requestModel.Search.Value != string.Empty)
                {
                    var value = requestModel.Search.Value.ToLower().Trim();

                    requestCount = (from DataRow dr in dt.Rows
                                    where dr["Cat"].ToString() == "Returned Schedule"
                                    where dr["RequestID"].ToString().ToLower().Contains(value) || dr["Cat"].ToString().ToLower().Contains(value)
                                    || dr["Creator"].ToString().ToLower().Contains(value) || Convert.ToDateTime(dr["Datecreated"]).ToString("dd-MMM-yyyy hh:mm").ToLower().Contains(value)
                                    || dr["CurrentStatus"].ToString().ToLower().Contains(value)
                                    select new Requests()
                                    {
                                        RequestID = dr["RequestID"].ToString()
                                    }).ToList();

                    requests = (from DataRow dr in dt.Rows
                                orderby dr["Datecreated"] descending
                                where dr["Cat"].ToString() == "Returned Schedule"
                                where dr["RequestID"].ToString().ToLower().Contains(value) || dr["Cat"].ToString().ToLower().Contains(value)
                                    || dr["Creator"].ToString().ToLower().Contains(value) || Convert.ToDateTime(dr["Datecreated"]).ToString("dd-MMM-yyyy hh:mm").ToLower().Contains(value)
                                    || dr["CurrentStatus"].ToString().ToLower().Contains(value)
                                select new Requests()
                                {
                                    RequestID = dr["RequestID"].ToString(),
                                    Category = dr["Cat"].ToString(),
                                    Creator = dr["Creator"].ToString(),
                                    Comment = dr["Comment"].ToString(),
                                    Datecreated = Convert.ToDateTime(dr["Datecreated"]).ToString("dd-MMM-yyyy hh:mm"),
                                    CurrentStatus = dr["CurrentStatus"].ToString(),
                                    CurrentAssignedToID = dr["CurrentAssignedToID"].ToString()
                                }).Skip(startRec).Take(pageSize).ToList();

                    totalCount = requestCount.Count();
                    filteredCount = requests.Count();
                }

                var sortedColumns = requestModel.Columns.GetSortedColumns();
                var orderByString = String.Empty;

                foreach (var column in sortedColumns)
                {
                    orderByString += orderByString != String.Empty ? "," : "";
                    orderByString += (column.Data) + (column.SortDirection == Column.OrderDirection.Ascendant ? " asc" : " desc");
                }

                //requests = requests.OrderBy(orderByString == string.Empty ? "asc" : orderByString);

                var data = requests.Select(emList => new
                {
                    RequestID = emList.RequestID,
                    Category = emList.Category,
                    Creator = emList.Creator,
                    Comment = emList.Comment,
                    Datecreated = emList.Datecreated,
                    CurrentStatus = emList.CurrentStatus,
                    CurrentAssignedToID = emList.CurrentAssignedToID
                }).ToList();

                return Json(new DataTablesResponse(requestModel.Draw, data, totalCount, totalCount), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogError logerror = new LogError();
                logerror.ErrorLog("", LoginUser, "", "ReturnedSchedule/GetMyRequests", "ReturnedSchedule", "GetMyRequests", "FetchIncidents Error", ex.Message.ToString(), 0);
                return Json(new { data = "Error has occured" }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize]
        public JsonResult GetRoleRequests([ModelBinder(typeof(DataTablesBinder))] IDataTablesRequest requestModel, String EmployerId)
        {

            string userkey = ConfigurationManager.AppSettings["userkey"];
            string uid = ConfigurationManager.AppSettings["uid"];
            string LoginUser = (string)Session["LoginSAPID"];
            string RoleID = (string)Session["RoleID"];
            string _access_key = ConfigurationManager.AppSettings["Salt"];

            try
            {
                Employer.Employer employer = new Employer.Employer();
                DataTable dt = employer.FetchIncidents("1", "N/A", "N/A", userkey, uid);

                //DataTable dts = employer.FetchAssignmentHistory("ESSP-0000000003", userkey, uid);

                dt.TableName = "Requests";
                dt.Columns.ToString();
                List<Requests> requests = new List<Requests>();
                int startRec = requestModel.Start;
                int pageSize = requestModel.Length;

                List<Requests> requestCount = (from DataRow dr in dt.Rows
                                               where dr["Cat"].ToString() == "Returned Schedule"
                                               where dr["CurrentAssignedRoleID"].ToString() == RoleID || dr["GroupOwner"].ToString() == RoleID
                                               select new Requests()
                                               {
                                                   RequestID = dr["RequestID"].ToString()
                                               }).ToList();

                requests = (from DataRow dr in dt.Rows
                            where dr["Cat"].ToString() == "Returned Schedule"
                            where dr["CurrentAssignedRoleID"].ToString() == RoleID || dr["GroupOwner"].ToString() == RoleID
                            orderby dr["Datecreated"] descending
                            select new Requests()
                            {
                                RequestID = dr["RequestID"].ToString(),
                                Creator = dr["Creator"].ToString(),
                                Category = dr["Cat"].ToString(),
                                Comment = dr["Comment"].ToString(),
                                Datecreated = Convert.ToDateTime(dr["Datecreated"]).ToString("dd-MMM-yyyy hh:mm"),
                                CurrentStatus = dr["CurrentStatus"].ToString(),
                                GroupOwner = dr["GroupOwner"].ToString(),
                                CurrentAssignedToName = dr["CurrentAssignedToName"].ToString(),
                                CurrentAssignedToID = dr["CurrentAssignedToID"].ToString()
                            }).Skip(startRec).Take(pageSize).ToList();

                var totalCount = requestCount.Count();
                var filteredCount = requests.Count();

                if (requestModel.Search.Value != string.Empty)
                {
                    var value = requestModel.Search.Value.ToLower().Trim();

                    requestCount = (from DataRow dr in dt.Rows
                                    where dr["Cat"].ToString() == "Returned Schedule"
                                    where dr["CurrentAssignedRoleID"].ToString() == RoleID || dr["GroupOwner"].ToString() == RoleID
                                    where dr["RequestID"].ToString().ToLower().Contains(value) || dr["Cat"].ToString().ToLower().Contains(value)
                                    || dr["Creator"].ToString().ToLower().Contains(value) || Convert.ToDateTime(dr["Datecreated"]).ToString("dd-MMM-yyyy hh:mm").ToLower().Contains(value)
                                    || dr["CurrentStatus"].ToString().ToLower().Contains(value) || dr["CurrentAssignedToName"].ToString().ToLower().Contains(value)
                                    select new Requests()
                                    {
                                        RequestID = dr["RequestID"].ToString()
                                    }).ToList();

                    requests = (from DataRow dr in dt.Rows
                                orderby dr["Datecreated"] descending
                                where dr["Cat"].ToString() == "Returned Schedule"
                                where dr["CurrentAssignedRoleID"].ToString() == RoleID || dr["GroupOwner"].ToString() == RoleID
                                where dr["RequestID"].ToString().ToLower().Contains(value) || dr["Cat"].ToString().ToLower().Contains(value)
                                    || dr["Creator"].ToString().ToLower().Contains(value) || Convert.ToDateTime(dr["Datecreated"]).ToString("dd-MMM-yyyy hh:mm").ToLower().Contains(value)
                                    || dr["CurrentStatus"].ToString().ToLower().Contains(value) || dr["CurrentAssignedToName"].ToString().ToLower().Contains(value)
                                select new Requests()
                                {
                                    RequestID = dr["RequestID"].ToString(),
                                    Category = dr["Cat"].ToString(),
                                    Creator = dr["Creator"].ToString(),
                                    Comment = dr["Comment"].ToString(),
                                    Datecreated = Convert.ToDateTime(dr["Datecreated"]).ToString("dd-MMM-yyyy hh:mm"),
                                    CurrentStatus = dr["CurrentStatus"].ToString(),
                                    CurrentAssignedToName = dr["CurrentAssignedToName"].ToString(),
                                    CurrentAssignedToID = dr["CurrentAssignedToID"].ToString()
                                }).Skip(startRec).Take(pageSize).ToList();

                    totalCount = requestCount.Count();
                    filteredCount = requests.Count();
                }

                var sortedColumns = requestModel.Columns.GetSortedColumns();
                var orderByString = String.Empty;

                foreach (var column in sortedColumns)
                {
                    orderByString += orderByString != String.Empty ? "," : "";
                    orderByString += (column.Data) + (column.SortDirection == Column.OrderDirection.Ascendant ? " asc" : " desc");
                }

                //requests = requests.OrderBy(orderByString == string.Empty ? "asc" : orderByString);

                var data = requests.Select(emList => new
                {
                    RequestID = emList.RequestID,
                    Category = emList.Category,
                    Creator = emList.Creator,
                    Comment = emList.Comment,
                    Datecreated = emList.Datecreated,
                    CurrentStatus = emList.CurrentStatus,
                    CurrentAssignedToName = emList.CurrentAssignedToName,
                    CurrentAssignedToID = emList.CurrentAssignedToID
                }).OrderBy(orderByString == string.Empty ? "RequestID asc" : orderByString).ToList();

                return Json(new DataTablesResponse(requestModel.Draw, data, totalCount, totalCount), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogError logerror = new LogError();
                logerror.ErrorLog("", LoginUser, "", "ReturnedSchedule/GetRequests", "ReturnedSchedule", "GetRequests", "FetchIncidents Error", ex.Message.ToString(), 0);
                return Json(new { data = "Error has occured" }, JsonRequestBehavior.AllowGet);
            }
        }


        [Authorize]
        public JsonResult GetMyRoleRequests([ModelBinder(typeof(DataTablesBinder))] IDataTablesRequest requestModel, String EmployerId)
        {

            string userkey = ConfigurationManager.AppSettings["userkey"];
            string uid = ConfigurationManager.AppSettings["uid"];
            string LoginUser = (string)Session["LoginSAPID"];
            string _access_key = ConfigurationManager.AppSettings["Salt"];
            string WebUserID = (string)Session["WebUserID"];
            string RoleID = (string)Session["RoleID"];

            try
            {
                Employer.Employer employer = new Employer.Employer();
                DataTable dt = employer.FetchIncidents("3", WebUserID, RoleID, userkey, uid);

                //DataTable dts = employer.FetchAssignmentHistory("ESSP-0000000003", userkey, uid);

                dt.TableName = "Requests";
                dt.Columns.ToString();
                List<Requests> requests = new List<Requests>();
                int startRec = requestModel.Start;
                int pageSize = requestModel.Length;

                List<Requests> requestCount = (from DataRow dr in dt.Rows
                                               where dr["Cat"].ToString() == "Returned Schedule"
                                               //where dr["GroupOwner"].ToString() == RoleID
                                               select new Requests()
                                               {
                                                   RequestID = dr["RequestID"].ToString()
                                               }).ToList();

                requests = (from DataRow dr in dt.Rows
                                //where dr["GroupOwner"].ToString() == RoleID
                            where dr["Cat"].ToString() == "Returned Schedule"
                            orderby dr["Datecreated"] descending
                            select new Requests()
                            {
                                RequestID = dr["RequestID"].ToString(),
                                Creator = dr["Creator"].ToString(),
                                Category = dr["Cat"].ToString(),
                                Comment = dr["Comment"].ToString(),
                                Datecreated = Convert.ToDateTime(dr["Datecreated"]).ToString("dd-MMM-yyyy hh:mm"),
                                CurrentStatus = dr["CurrentStatus"].ToString(),
                                CurrentAssignedToID = dr["CurrentAssignedToID"].ToString()
                            }).Skip(startRec).Take(pageSize).ToList();

                var totalCount = requestCount.Count();
                var filteredCount = requests.Count();

                if (requestModel.Search.Value != string.Empty)
                {
                    var value = requestModel.Search.Value.ToLower().Trim();

                    requestCount = (from DataRow dr in dt.Rows
                                    where dr["Cat"].ToString() == "Returned Schedule"
                                    //where dr["GroupOwner"].ToString() == RoleID
                                    where dr["RequestID"].ToString().ToLower().Contains(value) || dr["Cat"].ToString().ToLower().Contains(value)
                                    || dr["Creator"].ToString().ToLower().Contains(value) || Convert.ToDateTime(dr["Datecreated"]).ToString("dd-MMM-yyyy hh:mm").ToLower().Contains(value)
                                    || dr["CurrentStatus"].ToString().ToLower().Contains(value)
                                    select new Requests()
                                    {
                                        RequestID = dr["RequestID"].ToString()
                                    }).ToList();

                    requests = (from DataRow dr in dt.Rows
                                orderby dr["Datecreated"] descending
                                where dr["Cat"].ToString() == "Returned Schedule"
                                //where dr["GroupOwner"].ToString() == RoleID
                                where dr["RequestID"].ToString().ToLower().Contains(value) || dr["Cat"].ToString().ToLower().Contains(value)
                                    || dr["Creator"].ToString().ToLower().Contains(value) || Convert.ToDateTime(dr["Datecreated"]).ToString("dd-MMM-yyyy hh:mm").ToLower().Contains(value)
                                    || dr["CurrentStatus"].ToString().ToLower().Contains(value)
                                select new Requests()
                                {
                                    RequestID = dr["RequestID"].ToString(),
                                    Category = dr["Cat"].ToString(),
                                    Creator = dr["Creator"].ToString(),
                                    Comment = dr["Comment"].ToString(),
                                    Datecreated = Convert.ToDateTime(dr["Datecreated"]).ToString("dd-MMM-yyyy hh:mm"),
                                    CurrentStatus = dr["CurrentStatus"].ToString(),
                                    CurrentAssignedToID = dr["CurrentAssignedToID"].ToString()
                                }).Skip(startRec).Take(pageSize).ToList();

                    totalCount = requestCount.Count();
                    filteredCount = requests.Count();
                }

                var sortedColumns = requestModel.Columns.GetSortedColumns();
                var orderByString = String.Empty;

                foreach (var column in sortedColumns)
                {
                    orderByString += orderByString != String.Empty ? "," : "";
                    orderByString += (column.Data) + (column.SortDirection == Column.OrderDirection.Ascendant ? " asc" : " desc");
                }

                //requests = requests.OrderBy(orderByString == string.Empty ? "asc" : orderByString);

                var data = requests.Select(emList => new
                {
                    RequestID = emList.RequestID,
                    Category = emList.Category,
                    Creator = emList.Creator,
                    Comment = emList.Comment,
                    Datecreated = emList.Datecreated,
                    CurrentStatus = emList.CurrentStatus,
                    CurrentAssignedToID = emList.CurrentAssignedToID
                }).OrderBy(orderByString == string.Empty ? "RequestID asc" : orderByString).ToList();

                return Json(new DataTablesResponse(requestModel.Draw, data, totalCount, totalCount), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogError logerror = new LogError();
                logerror.ErrorLog("", LoginUser, "", "ReturnedSchedule/GetMyRequests", "ReturnedSchedule", "GetMyRequests", "FetchIncidents Error", ex.Message.ToString(), 0);
                return Json(new { data = "Error has occured" }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize]
        public JsonResult GetMyAssignedRequests([ModelBinder(typeof(DataTablesBinder))] IDataTablesRequest requestModel, String EmployerId)
        {

            string userkey = ConfigurationManager.AppSettings["userkey"];
            string uid = ConfigurationManager.AppSettings["uid"];
            string LoginUser = (string)Session["LoginSAPID"];
            string _access_key = ConfigurationManager.AppSettings["Salt"];
            string WebUserID = (string)Session["WebUserID"];
            string RoleID = (string)Session["RoleID"];

            try
            {
                Employer.Employer employer = new Employer.Employer();
                DataTable dt = employer.FetchIncidents("7", WebUserID, RoleID, userkey, uid);

                //DataTable dts = employer.FetchAssignmentHistory("ESSP-0000000003", userkey, uid);

                dt.TableName = "Requests";
                dt.Columns.ToString();
                List<Requests> requests = new List<Requests>();
                int startRec = requestModel.Start;
                int pageSize = requestModel.Length;

                List<Requests> requestCount = (from DataRow dr in dt.Rows
                                               where dr["Cat"].ToString() == "Returned Schedule"
                                               //where dr["GroupOwner"].ToString() == RoleID
                                               select new Requests()
                                               {
                                                   RequestID = dr["RequestID"].ToString()
                                               }).ToList();

                requests = (from DataRow dr in dt.Rows
                                //where dr["GroupOwner"].ToString() == RoleID
                            where dr["Cat"].ToString() == "Returned Schedule"
                            orderby dr["Datecreated"] descending
                            select new Requests()
                            {
                                RequestID = dr["RequestID"].ToString(),
                                Creator = dr["Creator"].ToString(),
                                Category = dr["Cat"].ToString(),
                                Comment = dr["Comment"].ToString(),
                                Datecreated = Convert.ToDateTime(dr["Datecreated"]).ToString("dd-MMM-yyyy hh:mm"),
                                CurrentStatus = dr["CurrentStatus"].ToString(),
                                CurrentAssignedToID = dr["CurrentAssignedToID"].ToString()
                            }).Skip(startRec).Take(pageSize).ToList();

                var totalCount = requestCount.Count();
                var filteredCount = requests.Count();

                if (requestModel.Search.Value != string.Empty)
                {
                    var value = requestModel.Search.Value.ToLower().Trim();

                    requestCount = (from DataRow dr in dt.Rows
                                        //where dr["GroupOwner"].ToString() == RoleID
                                    where dr["Cat"].ToString() == "Returned Schedule"
                                    where dr["RequestID"].ToString().ToLower().Contains(value) || dr["Cat"].ToString().ToLower().Contains(value)
                                    || dr["Creator"].ToString().ToLower().Contains(value) || Convert.ToDateTime(dr["Datecreated"]).ToString("dd-MMM-yyyy hh:mm").ToLower().Contains(value)
                                    || dr["CurrentStatus"].ToString().ToLower().Contains(value)
                                    select new Requests()
                                    {
                                        RequestID = dr["RequestID"].ToString()
                                    }).ToList();

                    requests = (from DataRow dr in dt.Rows
                                orderby dr["Datecreated"] descending
                                where dr["Cat"].ToString() == "Returned Schedule"
                                //where dr["GroupOwner"].ToString() == RoleID
                                where dr["RequestID"].ToString().ToLower().Contains(value) || dr["Cat"].ToString().ToLower().Contains(value)
                                    || dr["Creator"].ToString().ToLower().Contains(value) || Convert.ToDateTime(dr["Datecreated"]).ToString("dd-MMM-yyyy hh:mm").ToLower().Contains(value)
                                    || dr["CurrentStatus"].ToString().ToLower().Contains(value)
                                select new Requests()
                                {
                                    RequestID = dr["RequestID"].ToString(),
                                    Category = dr["Cat"].ToString(),
                                    Creator = dr["Creator"].ToString(),
                                    Comment = dr["Comment"].ToString(),
                                    Datecreated = Convert.ToDateTime(dr["Datecreated"]).ToString("dd-MMM-yyyy hh:mm"),
                                    CurrentStatus = dr["CurrentStatus"].ToString(),
                                    CurrentAssignedToID = dr["CurrentAssignedToID"].ToString()
                                }).Skip(startRec).Take(pageSize).ToList();

                    totalCount = requestCount.Count();
                    filteredCount = requests.Count();
                }

                var sortedColumns = requestModel.Columns.GetSortedColumns();
                var orderByString = String.Empty;

                foreach (var column in sortedColumns)
                {
                    orderByString += orderByString != String.Empty ? "," : "";
                    orderByString += (column.Data) + (column.SortDirection == Column.OrderDirection.Ascendant ? " asc" : " desc");
                }

                //requests = requests.OrderBy(orderByString == string.Empty ? "asc" : orderByString);

                var data = requests.Select(emList => new
                {
                    RequestID = emList.RequestID,
                    Category = emList.Category,
                    Creator = emList.Creator,
                    Comment = emList.Comment,
                    Datecreated = emList.Datecreated,
                    CurrentStatus = emList.CurrentStatus,
                    CurrentAssignedToID = emList.CurrentAssignedToID
                }).OrderBy(orderByString == string.Empty ? "RequestID asc" : orderByString).ToList();

                return Json(new DataTablesResponse(requestModel.Draw, data, totalCount, totalCount), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogError logerror = new LogError();
                logerror.ErrorLog("", LoginUser, "", "ReturnedSchedule/GetMyRequests", "ReturnedSchedule", "GetMyRequests", "FetchIncidents Error", ex.Message.ToString(), 0);
                return Json(new { data = "Error has occured" }, JsonRequestBehavior.AllowGet);
            }
        }


        [Authorize]
        public JsonResult GetRoleUnAssignRequests([ModelBinder(typeof(DataTablesBinder))] IDataTablesRequest requestModel, String EmployerId)
        {

            string userkey = ConfigurationManager.AppSettings["userkey"];
            string uid = ConfigurationManager.AppSettings["uid"];
            string LoginUser = (string)Session["LoginSAPID"];
            string _access_key = ConfigurationManager.AppSettings["Salt"];
            string WebUserID = (string)Session["WebUserID"];
            string RoleID = (string)Session["RoleID"];

            try
            {
                Employer.Employer employer = new Employer.Employer();
                DataTable dt = employer.FetchIncidents("2", "N/A", "N/A", userkey, uid);

                //DataTable dts = employer.FetchAssignmentHistory("ESSP-0000000003", userkey, uid);

                dt.TableName = "Requests";
                dt.Columns.ToString();
                List<Requests> requests = new List<Requests>();
                int startRec = requestModel.Start;
                int pageSize = requestModel.Length;

                List<Requests> requestCount = (from DataRow dr in dt.Rows
                                               where dr["Cat"].ToString() == "Returned Schedule"
                                               where dr["CurrentAssignedRoleID"].ToString() == RoleID || dr["GroupOwner"].ToString() == RoleID
                                               select new Requests()
                                               {
                                                   RequestID = dr["RequestID"].ToString()
                                               }).ToList();

                requests = (from DataRow dr in dt.Rows
                            where dr["Cat"].ToString() == "Returned Schedule"
                            where dr["CurrentAssignedRoleID"].ToString() == RoleID || dr["GroupOwner"].ToString() == RoleID
                            orderby dr["Datecreated"] descending
                            select new Requests()
                            {
                                RequestID = dr["RequestID"].ToString(),
                                Creator = dr["Creator"].ToString(),
                                Category = dr["Cat"].ToString(),
                                Comment = dr["Comment"].ToString(),
                                GroupOwner = dr["CurrentAssignedRoleID"].ToString(),
                                Datecreated = Convert.ToDateTime(dr["Datecreated"]).ToString("dd-MMM-yyyy hh:mm"),
                                CurrentStatus = dr["CurrentStatus"].ToString(),
                                CurrentAssignedToID = dr["CurrentAssignedToID"].ToString()
                            }).Skip(startRec).Take(pageSize).ToList();

                var totalCount = requestCount.Count();
                var filteredCount = requests.Count();

                if (requestModel.Search.Value != string.Empty)
                {
                    var value = requestModel.Search.Value.ToLower().Trim();

                    requestCount = (from DataRow dr in dt.Rows
                                    where dr["Cat"].ToString() == "Returned Schedule"
                                    where dr["CurrentAssignedRoleID"].ToString() == RoleID || dr["GroupOwner"].ToString() == RoleID
                                    where dr["RequestID"].ToString().ToLower().Contains(value) || dr["Cat"].ToString().ToLower().Contains(value)
                                    || dr["Creator"].ToString().ToLower().Contains(value) || Convert.ToDateTime(dr["Datecreated"]).ToString("dd-MMM-yyyy hh:mm").ToLower().Contains(value)
                                    || dr["CurrentStatus"].ToString().ToLower().Contains(value)
                                    select new Requests()
                                    {
                                        RequestID = dr["RequestID"].ToString()
                                    }).ToList();

                    requests = (from DataRow dr in dt.Rows
                                orderby dr["Datecreated"] descending
                                where dr["Cat"].ToString() == "Returned Schedule"
                                where dr["CurrentAssignedRoleID"].ToString() == RoleID || dr["GroupOwner"].ToString() == RoleID
                                where dr["RequestID"].ToString().ToLower().Contains(value) || dr["Cat"].ToString().ToLower().Contains(value)
                                    || dr["Creator"].ToString().ToLower().Contains(value) || Convert.ToDateTime(dr["Datecreated"]).ToString("dd-MMM-yyyy hh:mm").ToLower().Contains(value)
                                    || dr["CurrentStatus"].ToString().ToLower().Contains(value)
                                select new Requests()
                                {
                                    RequestID = dr["RequestID"].ToString(),
                                    Category = dr["Cat"].ToString(),
                                    Creator = dr["Creator"].ToString(),
                                    Comment = dr["Comment"].ToString(),
                                    Datecreated = Convert.ToDateTime(dr["Datecreated"]).ToString("dd-MMM-yyyy hh:mm"),
                                    CurrentStatus = dr["CurrentStatus"].ToString(),
                                    CurrentAssignedToID = dr["CurrentAssignedToID"].ToString()
                                }).Skip(startRec).Take(pageSize).ToList();

                    totalCount = requestCount.Count();
                    filteredCount = requests.Count();
                }

                var sortedColumns = requestModel.Columns.GetSortedColumns();
                var orderByString = String.Empty;

                foreach (var column in sortedColumns)
                {
                    orderByString += orderByString != String.Empty ? "," : "";
                    orderByString += (column.Data) + (column.SortDirection == Column.OrderDirection.Ascendant ? " asc" : " desc");
                }

                //requests = requests.OrderBy(orderByString == string.Empty ? "asc" : orderByString);

                var data = requests.Select(emList => new
                {
                    RequestID = emList.RequestID,
                    Category = emList.Category,
                    Creator = emList.Creator,
                    Comment = emList.Comment,
                    Datecreated = emList.Datecreated,
                    CurrentStatus = emList.CurrentStatus,
                    CurrentAssignedToID = emList.CurrentAssignedToID
                }).OrderBy(orderByString == string.Empty ? "RequestID asc" : orderByString).ToList();

                return Json(new DataTablesResponse(requestModel.Draw, data, totalCount, totalCount), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogError logerror = new LogError();
                logerror.ErrorLog("", LoginUser, "", "ReturnedSchedule/GetMyRequests", "ReturnedSchedule", "GetMyRequests", "FetchIncidents Error", ex.Message.ToString(), 0);
                return Json(new { data = "Error has occured" }, JsonRequestBehavior.AllowGet);
            }
        }


        [Authorize]
        public ActionResult Details(string RequestId)
        {
            string userkey = ConfigurationManager.AppSettings["userkey"];
            string uid = ConfigurationManager.AppSettings["uid"];
            string LoginUser = (string)Session["LoginSAPID"];
            string EmployerId = (string)Session["EMPLOYER_ID"];
            string WebUserID = (string)Session["WebUserID"];
            string RoleID = (string)Session["RoleID"];
            ViewBag.WebuserID = WebUserID;
            ViewBag.RequestId = RequestId;
            return View();
        }


        [Authorize]
        public JsonResult GetRequestCategory()
        {
            string LoginUser = (string)Session["LoginSAPID"];
            string ROLE_ID = (string)Session["RoleID"];
            try
            {
                string userkey = ConfigurationManager.AppSettings["userkey"];
                string uid = ConfigurationManager.AppSettings["uid"];

                Employer.Employer employer = new Employer.Employer();
                DataTable dt = employer.FetchRequestCategories(ROLE_ID, userkey, uid);
                dt.TableName = "Fetchrequestcategory";
                dt.Columns.ToString();
                var requestCategory = (from DataRow dr in dt.Rows
                                       where dr["IsAES"].ToString() == "R"
                                       select new
                                       {
                                           ID = dr["ID"].ToString(),
                                           Descr = dr["Descr"].ToString(),
                                           Mail = dr["Mail"].ToString(),
                                           Dynamictext = dr["dynamictext"].ToString(),
                                           Dynamicurl = dr["dynamicurls"].ToString()
                                       }).ToList();

                return Json(new { data = requestCategory }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogError logerror = new LogError();
                logerror.ErrorLog("", LoginUser, "", "ReturnedSchedule/GetRequestCategory", "ReturnedSchedule", "GetRequestCategory", "FetchRequestCategories Error", ex.Message.ToString(), 0);
                return Json(new { data = "Error has occured" }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public JsonResult GetEmployerCode(string Prefix)
        {
            //Note : you can bind same list from database  
            Employer.Employer employer = new Employer.Employer();
            DataTable dt = employer.FetchCompanies("All", "");
            dt.TableName = "Employer";
            
            var employerList = (from DataRow dr in dt.Rows
                                where dr["Company Name"].ToString().ToLower().StartsWith(Prefix.ToLower()) || dr["EmployerID"].ToString().ToLower().StartsWith(Prefix.ToLower())
                                select new Employers()
                                {
                                    EmployerID = dr["EmployerID"].ToString(),
                                    CompanyName = dr["Company Name"].ToString(),
                                }).ToList();

            return Json(employerList, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        /*public JsonResult GetRequestsHistory([ModelBinder(typeof(DataTablesBinder))] IDataTablesRequest requestModel, String RequestId)
        {

            string userkey = ConfigurationManager.AppSettings["userkey"];
            string uid = ConfigurationManager.AppSettings["uid"];
            string LoginUser = (string)Session["LoginSAPID"];
            string _access_key = ConfigurationManager.AppSettings["Salt"];

            try
            {
                Employer.Employer employer = new Employer.Employer();
                DataTable dt = employer.FetchAssignmentHistory(RequestId, userkey, uid);
                dt.TableName = "RequestsHistory";

                DataTable dts = employer.FetchSchedule_addendum(RequestId, "N/A", userkey, uid);
                dts.TableName = "ScheduleAddendum";
                //dts.Merge(dt, True, MissingSchemaAction.Ignore);
                //dt.Rows.Add(dts);

                dt.Columns.ToString();

                List<RequestsHistory> requestsHistory = new List<RequestsHistory>();
                int startRec = requestModel.Start;
                int pageSize = requestModel.Length;


                List<RequestsHistory> requestCount = (from DataRow dr in dt.Rows
                                                       select new RequestsHistory()
                                                       {
                                                           RequestID = dr["RequestID"].ToString()
                                                       }).ToList();

                requestsHistory = (from DataRow dr in dt.Rows
                                   orderby dr["AssignDate"] descending
                                   select new RequestsHistory()
                                    {
                                        RequestID = dr["RequestID"].ToString(),
                                        HistoryID = dr["HistoryID"].ToString(),
                                        Comment = dr["Comment"].ToString(),
                                        Assignee = dr["Assignee"].ToString(),
                                        Assignor = dr["Assignor"].ToString(),
                                        AssignDate = dr["AssignDate"].ToString(),
                                        AssignStatus = dr["AssignStatus"].ToString()
                                    }).Skip(startRec).Take(pageSize).ToList();

                var totalCount = requestCount.Count();
                var filteredCount = requestsHistory.Count();

                if (requestModel.Search.Value != string.Empty)
                {
                    var value = requestModel.Search.Value.ToLower().Trim();

                    requestCount = (from DataRow dr in dt.Rows
                                    where dr["RequestID"].ToString().ToString().Contains(value) || dr["Assignee"].ToString().Contains(value)
                                       || dr["Assignor"].ToString().Contains(value) || dr["AssignDate"].ToString().Contains(value) 
                                       || dr["Assignee"].ToString().Contains(value) || dr["AssignStatus"].ToString().Contains(value)
                                    select new RequestsHistory()
                                    {
                                        RequestID = dr["RequestID"].ToString()
                                    }).ToList();

                    requestsHistory = (from DataRow dr in dt.Rows
                                       orderby dr["AssignDate"] descending
                                       where dr["RequestID"].ToString().ToString().Contains(value) || dr["Assignee"].ToString().Contains(value)
                                               || dr["Assignor"].ToString().Contains(value) || dr["AssignDate"].ToString().Contains(value)
                                               || dr["Assignee"].ToString().Contains(value) || dr["AssignStatus"].ToString().Contains(value)
                                        select new RequestsHistory()
                                        {
                                            RequestID = dr["RequestID"].ToString(),
                                            HistoryID = dr["HistoryID"].ToString(),
                                            Comment = dr["Comment"].ToString(),
                                            Assignee = dr["Assignee"].ToString(),
                                            Assignor = dr["Assignor"].ToString(),
                                            AssignDate = dr["AssignDate"].ToString(),
                                            AssignStatus = dr["AssignStatus"].ToString()
                                        }).Skip(startRec).Take(pageSize).ToList();

                    totalCount = requestCount.Count();
                    filteredCount = requestsHistory.Count();
                }

                var sortedColumns = requestModel.Columns.GetSortedColumns();
                var orderByString = String.Empty;

                foreach (var column in sortedColumns)
                {
                    orderByString += orderByString != String.Empty ? "," : "";
                    orderByString += (column.Data) + (column.SortDirection == Column.OrderDirection.Ascendant ? " asc" : " desc");
                }

                var data = requestsHistory.Select(emList => new
                {
                    RequestID = emList.RequestID,
                    HistoryID = emList.HistoryID,
                    Comment = emList.Comment,
                    Assignee = emList.Assignee,
                    Assignor = emList.Assignor,
                    AssignDate = emList.AssignDate,
                    AssignStatus = emList.AssignStatus
                }).OrderBy(orderByString == string.Empty ? "RequestID asc" : orderByString).ToList();

                return Json(new DataTablesResponse(requestModel.Draw, data, totalCount, totalCount), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogError logerror = new LogError();
                logerror.ErrorLog("", LoginUser, "", "ReturnedSchedule/GetRequests", "ReturnedSchedule", "GetRequests", "FetchIncidents Error", ex.Message.ToString(), 0);
                return Json(new { data = "Error has occured" }, JsonRequestBehavior.AllowGet);
            }
        }*/

        [Authorize]
        public JsonResult GetRequestsHistory(String RequestId)
        {
            string userkey = ConfigurationManager.AppSettings["userkey"];
            string uid = ConfigurationManager.AppSettings["uid"];
            string LoginUser = (string)Session["LoginSAPID"];
            string _access_key = ConfigurationManager.AppSettings["Salt"];

            try
            {

                Employer.Employer employer = new Employer.Employer();
                DataTable dt = employer.FetchAssignmentHistory(RequestId, userkey, uid);
                dt.TableName = "RequestsHistory";
                dt.Columns.ToString();

                DataTable dts = employer.FetchSchedule_addendum(RequestId, "N/A", userkey, uid);
                dts.TableName = "ScheduleAddendum";
                dts.Columns.ToString();

                var result = (from DataRow dr in dt.Rows
                              join DataRow drs in dts.Rows
                              on dr["HistoryID"].ToString() equals drs["HistoryID"].ToString()
                              where drs["HistoryID"].ToString() != ""
                              select new
                             {
                                 RequestID = dr["RequestID"].ToString(),
                                 HistoryID = dr["HistoryID"].ToString(),
                                 Comment = dr["Comment"].ToString(),
                                 Assignee = dr["Assignee"].ToString(),
                                 Assignor = dr["Assignor"].ToString(),
                                 AssignDate = dr["AssignDate"].ToString(),
                                 AssignStatus = dr["AssignStatus"].ToString(),
                                 EmployerCode = drs["EmployerCode"].ToString(),
                                 ReturnDate = drs["ReturnDate"].ToString(),
                                 Classification = drs["Classification"].ToString(),
                                 Amount_Returned = drs["Amount_Returned"].ToString(),
                                 Amount_on_Schedule = drs["Amount_on_Schedule"].ToString(),
                                 Narration = drs["Narration"].ToString()
                             }).ToList();

                return Json(new { data = result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogError logerror = new LogError();
                logerror.ErrorLog("", LoginUser, "", "ReturnedSchedule/GetRequests", "ReturnedSchedule", "GetRequests", "FetchIncidents Error", ex.Message.ToString(), 0);
                return Json(new { data = "Error has occured" }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize]
        public JsonResult GetRequestsAddendum(String RequestId)
        {
            string userkey = ConfigurationManager.AppSettings["userkey"];
            string uid = ConfigurationManager.AppSettings["uid"];
            string LoginUser = (string)Session["LoginSAPID"];
            string _access_key = ConfigurationManager.AppSettings["Salt"];

            try
            {

                Employer.Employer employer = new Employer.Employer();
                DataTable dt = employer.FetchSchedule_addendum(RequestId, "N/A", userkey, uid);
                dt.TableName = "ScheduleAddendum";
                dt.Columns.ToString();

                var returnSchedule = (from DataRow dr in dt.Rows
                                      where dr["HistoryID"].ToString() == ""
                                      select new
                                        {
                                            EmployerName = dr["coyname"].ToString(),
                                            EmployerCode = dr["EmployerCode"].ToString(),
                                            ReturnDate = dr["ReturnDate"].ToString(),
                                            Classification = dr["Classification"].ToString(),
                                            Amount_Returned = dr["Amount_Returned"].ToString(),
                                            Amount_on_Schedule = dr["Amount_on_Schedule"].ToString(),
                                            Narration = dr["Narration"].ToString(),
                                            RequestID = dr["RequestID"].ToString(),
                                            HistoryID = dr["HistoryID"].ToString()
                                        }).ToList();

                return Json(new { data = returnSchedule }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogError logerror = new LogError();
                logerror.ErrorLog("", LoginUser, "", "ReturnedSchedule/GetRequestsAddendum", "Requests", "GetRequestsAddendum", "FetchSchedule_addendum Error", ex.Message.ToString(), 0);
                return Json(new { data = "Error has occured" }, JsonRequestBehavior.AllowGet);
            }
        }


        [Authorize]
        public JsonResult GetRequestsAddendumError(String RequestId, string HistoryId)
        {
            string userkey = ConfigurationManager.AppSettings["userkey"];
            string uid = ConfigurationManager.AppSettings["uid"];
            string LoginUser = (string)Session["LoginSAPID"];
            string _access_key = ConfigurationManager.AppSettings["Salt"];

            try
            {

                Employer.Employer employer = new Employer.Employer();
                DataTable dt = employer.FetchSchedule_addendumError(RequestId, HistoryId, userkey, uid);
                dt.TableName = "ScheduleAddendumError";
                dt.Columns.ToString();

                var returnScheduleError = (from DataRow dr in dt.Rows
                                          select new
                                          {
                                              RequestID = dr["RequestID"].ToString(),
                                              HistoryID = dr["HistoryID"].ToString(),
                                              ErrorName = dr["ErrorName"].ToString(),
                                          }).ToList();

                return Json(new { data = returnScheduleError }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogError logerror = new LogError();
                logerror.ErrorLog("", LoginUser, "", "ReturnedSchedule/GetRequestsAddendumError", "ReturnedSchedule", "GetRequestsAddendumError", "FetchSchedule_addendumError Error", ex.Message.ToString(), 0);
                return Json(new { data = "Error has occured" }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize]
        public JsonResult GetInternalUsers()
        {
            string LoginUser = (string)Session["LoginSAPID"];
            try
            {

                string userkey = ConfigurationManager.AppSettings["userkey"];
                string uid = ConfigurationManager.AppSettings["uid"];

                Employer.Employer employer = new Employer.Employer();
                DataTable dt = employer.FetchInternalUsers(userkey, uid);
                dt.TableName = "InternalUsers";
                dt.Columns.ToString();

                var users = (from DataRow dr in dt.Rows
                             select new Models.InternalUsers()
                             {
                                 ID = dr["ID"].ToString(),
                                 FULLNAME = dr["FULLNAME"].ToString(),
                                 ROLE_ID = dr["ROLE_ID"].ToString()
                             }).ToList();

                return Json(new { data = users }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogError logerror = new LogError();
                logerror.ErrorLog("", LoginUser, "", "ReturnedSchedule/GetInternalUsers", "ReturnedSchedule", "GetInternalUsers", "FetchInternalUsers Error", ex.Message.ToString(), 0);
                return Json(new { data = "Error has occured" }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize]
        public JsonResult GetExternalUsers()
        {
            string LoginUser = (string)Session["LoginSAPID"];
            try
            {

                string userkey = ConfigurationManager.AppSettings["userkey"];
                string uid = ConfigurationManager.AppSettings["uid"];

                Employer.Employer employer = new Employer.Employer();
                DataTable dt = employer.FetchExternalUsers(false,"1002",userkey, uid);
                dt.TableName = "ExternalUsers";
                dt.Columns.ToString();
                
                var users = (from DataRow dr in dt.Rows
                             select new ExternalUsers()
                             {
                                 ID = dr["ID"].ToString(),
                                 FULLNAME = dr["FULLNAME"].ToString(),
                                 ROLE_ID = dr["ROLE_ID"].ToString(),
                                 CompanyName = dr["coyname"].ToString()
                             }).ToList();

                return Json(new { data = users }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogError logerror = new LogError();
                logerror.ErrorLog("", LoginUser, "", "ReturnedSchedule/GetExternalUsers", "ReturnedSchedule", "GetExternalUsers", "FetchExternalUsers Error", ex.Message.ToString(), 0);
                return Json(new { data = "Error has occured" }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize]
        public JsonResult GetRequestDocs(string RequestId)
        {
            string LoginUser = (string)Session["LoginSAPID"];
            try
            {

                string userkey = ConfigurationManager.AppSettings["userkey"];
                string uid = ConfigurationManager.AppSettings["uid"];

                Employer.Employer employer = new Employer.Employer();
                DataTable dt = employer.FetchIncidentDoc("1", RequestId, userkey, uid);
                dt.TableName = "RequestsHistoryDoc";
                dt.Columns.ToString();

                var requestDocs = (from DataRow dr in dt.Rows
                                   select new
                                   {
                                       RequestID = dr["RequestID"].ToString(),
                                       Doc = dr["Doc"].ToString(),
                                       Docext = dr["Docext"].ToString(),
                                       Docname = dr["Docname"].ToString(),
                                       Dateupload = dr["Dateuploaded"].ToString(),
                                   }).ToList();

                return Json(new { data = requestDocs }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogError logerror = new LogError();
                logerror.ErrorLog("", LoginUser, "", "ReturnedSchedule/GetExternalUsers", "ReturnedSchedule", "GetExternalUsers", "FetchExternalUsers Error", ex.Message.ToString(), 0);
                return Json(new { data = "Error has occured" }, JsonRequestBehavior.AllowGet);
            }
        }


        [Authorize]
        public JsonResult GetRequestHistoryDocs(string historyId)
        {
            string LoginUser = (string)Session["LoginSAPID"];
            try
            {

                string userkey = ConfigurationManager.AppSettings["userkey"];
                string uid = ConfigurationManager.AppSettings["uid"];

                Employer.Employer employer = new Employer.Employer();
                DataTable dt = employer.FetchIncidentDoc("2", historyId, userkey, uid);
                dt.TableName = "RequestsHistoryDoc";
                dt.Columns.ToString();

                var requestDocs = (from DataRow dr in dt.Rows
                                    select new
                                    {
                                        RequestID = dr["RequestID"].ToString(),
                                        Doc = dr["Doc"].ToString(),
                                        Docext = dr["Docext"].ToString(),
                                        Docname = dr["Docname"].ToString(),
                                        Dateupload = dr["Dateuploaded"].ToString(),
                                    }).ToList();


                return Json(new { data = requestDocs }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogError logerror = new LogError();
                logerror.ErrorLog("", LoginUser, "", "ReturnedSchedule/GetExternalUsers", "ReturnedSchedule", "GetExternalUsers", "FetchExternalUsers Error", ex.Message.ToString(), 0);
                return Json(new { data = "Error has occured" }, JsonRequestBehavior.AllowGet);
            }
        }


        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateRequest(string requestId, string statusUpdate, string internalUser, string externalUser, string custodianUser, string Comment, string convertedFile, string convertedFile1, string convertedFile2, string EmployerCode, string requestCategory, string classification, List<string> reasons, string narration, string AmountSchedule, string AmountReturned)
        {
            string UploadStatus = "";
            string UploadStatus1 = "";
            string UploadStatus2 = "";
            string assignedToId = "";
            string roleId = "";
            string assignedToRoleId = "";
            //convertedFile = "";
            //convertedFile1 = "";
            //convertedFile2 = "";

            string ipaddress = Request.UserHostAddress;
            string Agent = Request.UserAgent;
            string BrowserUsed = Request.Browser.Browser;
            string SessionID = HttpContext.Session.SessionID;

            string userkey = ConfigurationManager.AppSettings["userkey"];
            string uid = ConfigurationManager.AppSettings["uid"];

            string LoginUser = (string)Session["LoginSAPID"];
            string EmployerId = (string)Session["EMPLOYER_ID"];
            string AssignByID = (string)Session["WebUserID"];
            string AssignByRoleId = (string)Session["RoleID"];
            string RoleID = (string)Session["RoleID"];

            try
            {
                Employer.Employer employer = new Employer.Employer();

                if (internalUser != "")
                {
                    var value = internalUser.Split('|');
                    assignedToId = value[0].Trim();
                    assignedToRoleId = value[1].Trim();
                }
                else if(externalUser != ""){
                    assignedToId = externalUser;
                    roleId = RoleID;
                }
                else if (custodianUser != "")
                {
                    assignedToId = externalUser;
                    roleId = RoleID;
                }
                string requestStatus;
                if (statusUpdate == "Assigned")
                {
                    requestStatus = employer.AssignRequest(requestId, Comment, AssignByID, AssignByRoleId, assignedToRoleId, assignedToId, statusUpdate, userkey, uid);
                }
                else
                {
                    requestStatus = employer.AssignRequest(requestId, Comment, AssignByID, AssignByRoleId, AssignByRoleId, AssignByID, statusUpdate, userkey, uid);
                }
                //var requestStatus = employer.CreateRequest(WebUserID, "3", requestCategory, "Initiated", Comment, userkey, uid);
                var requestDetails = requestStatus.Split('~');

                if (requestDetails[0] != "01")
                {
                    var file = Request.Files;

                    if (file[0].ContentLength > 0)
                    {
                        string _FileName = file[0].FileName;
                        string Docext = Path.GetExtension(file[0].FileName);
                        string Doc = convertedFile;
                        UploadStatus = employer.LogRequestDoc(requestId, requestDetails[1], Doc, Docext, _FileName, "Y", userkey, uid);
                        var uploadDetails = UploadStatus.Split('~');
                    }
                    if (file[1].ContentLength > 0)
                    {
                        string _FileName1 = file[1].FileName;
                        string Docext1 = Path.GetExtension(file[1].FileName);
                        string Doc1 = convertedFile1;
                        UploadStatus1 = employer.LogRequestDoc(requestId, requestDetails[1], Doc1, Docext1, _FileName1, "Y", userkey, uid);
                        //var uploadDetails1 = UploadStatus1.Split('~');
                    }
                    if (file[2].ContentLength > 0)
                    {
                        string _FileName2 = file[2].FileName;
                        string Docext2 = Path.GetExtension(file[2].FileName);
                        string Doc2 = convertedFile2;
                        UploadStatus2 = employer.LogRequestDoc(requestId, requestDetails[1], Doc2, Docext2, _FileName2, "Y", userkey, uid);
                        //uploadDetails2 = UploadStatus2.Split('~');
                    }

                    var appendedScheduleStatus = employer.AppendScheduleRequest(EmployerCode, classification, AmountReturned, AmountSchedule, narration, requestId, requestDetails[1], userkey, uid);

                    if (reasons != null)
                    {
                        int ReasonCount = reasons.Count();
                        for (int i = 0; i < ReasonCount; i++)
                        {
                            var appendedReasonStatus = employer.AppendScheduleRequestError(requestId, requestDetails[1], reasons[i], userkey, uid);
                        }
                    }

                    if (UploadStatus == "00~Your documents have been logged successfully" || UploadStatus1 == "00~Your documents have been logged successfully" || UploadStatus2 == "00~Your documents have been logged successfully")
                    {
                        employer.NotifyIncident(requestDetails[1], userkey, uid);
                        string status = "00~Your documents have been logged successfully";
                        var statusDetails = status.Split('~');
                        TempData["error"] = "Your request and " + statusDetails[1];
                        //employer
                        if (RoleID == "1" || RoleID == "2")
                        {
                            TempData["msg"] = requestDetails[2];
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            TempData["msg"] = requestDetails[2];
                            return RedirectToAction("MyRequest");
                        }
                    }
                    else
                    {

                    }

                    if (RoleID == "1" || RoleID == "2")
                    {
                        TempData["msg"] = requestDetails[2];
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["msg"] = requestDetails[2];
                        return RedirectToAction("MyRequest");
                    }
                }
                else
                {
                    TempData["error"] = requestDetails[2];
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                LogError logerror = new LogError();
                logerror.ErrorLog("", LoginUser, "", "ReturnedSchedule/Create", "ReturnedSchedule", "Create", "CreateRequest Error", ex.InnerException.Message.ToString(), 0);
                TempData["error"] = "An error processing complaints";
                return View();
            }
        }
    }
}
