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

namespace ESPAdmin.Controllers
{
    public class AdminController : Controller
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
        public JsonResult GetInternalUsers([ModelBinder(typeof(DataTablesBinder))] IDataTablesRequest requestModel)
        {

            string userkey = ConfigurationManager.AppSettings["userkey"];
            string uid = ConfigurationManager.AppSettings["uid"];
            string LoginUser = (string)Session["LoginSAPID"];

            try
            {
                Employer.Employer employer = new Employer.Employer();
                DataTable dt = employer.FetchInternalUsers(userkey, uid);
                dt.TableName = "InternalUsers";

                dt.Columns.ToString();

                List<InternalUsers> userList = new List<InternalUsers>();
                int startRec = requestModel.Start;
                int pageSize = requestModel.Length;

                List<InternalUsers> usersCount = (from DataRow dr in dt.Rows
                                                 select new InternalUsers()
                                                 {
                                                     ID = dr["ID"].ToString()
                                                 }).ToList();

                userList = (from DataRow dr in dt.Rows
                                select new InternalUsers()
                                {
                                    ID = dr["ID"].ToString(),
                                    USER_NAME = dr["USER_NAME"].ToString(),
                                    FULLNAME = dr["FULLNAME"].ToString(),
                                    EMAIL = dr["EMAIL"].ToString(),
                                    ROLE_ID = dr["RoleName"].ToString(),
                                    LOCKED = dr["LOCKED"].ToString(),
                                    STATUS = dr["STATUS"].ToString(),
                                    DATE_LAST_MODIFIED = dr["DATE_LAST_MODIFIED"].ToString()
                                }).Skip(startRec).Take(pageSize).ToList();

                var totalCount = usersCount.Count();
                var filteredCount = userList.Count();

                if (requestModel.Search.Value != string.Empty)
                {
                    var value = requestModel.Search.Value.ToLower().Trim();

                    usersCount = (from DataRow dr in dt.Rows
                                     where dr["ID"].ToString().ToLower().Contains(value) || dr["USER_NAME"].ToString().ToLower().Contains(value)
                                        || dr["FULLNAME"].ToString().ToLower().Contains(value) || dr["EMAIL"].ToString().ToLower().Contains(value)
                                     select new InternalUsers()
                                     {
                                         ID = dr["ID"].ToString()
                                     }).ToList();

                    userList = (from DataRow dr in dt.Rows
                                where dr["ID"].ToString().ToLower().Contains(value) || dr["USER_NAME"].ToString().ToLower().Contains(value)
                                    || dr["FULLNAME"].ToString().ToLower().Contains(value) || dr["EMAIL"].ToString().ToLower().Contains(value)
                                select new InternalUsers()
                                {
                                    ID = dr["ID"].ToString(),
                                    USER_NAME = dr["USER_NAME"].ToString(),
                                    FULLNAME = dr["FULLNAME"].ToString(),
                                    EMAIL = dr["EMAIL"].ToString(),
                                    LOCKED = dr["LOCKED"].ToString(),
                                    ROLE_ID = dr["RoleName"].ToString(),
                                    STATUS = dr["STATUS"].ToString(),
                                    DATE_LAST_MODIFIED = dr["DATE_LAST_MODIFIED"].ToString()
                                }).Skip(startRec).Take(pageSize).ToList();

                    totalCount = usersCount.Count();
                    filteredCount = userList.Count();
                }

                var sortedColumns = requestModel.Columns.GetSortedColumns();
                var orderByString = String.Empty;

                foreach (var column in sortedColumns)
                {
                    orderByString += orderByString != String.Empty ? "," : "";
                    orderByString += (column.Data) + (column.SortDirection == Column.OrderDirection.Ascendant ? " asc" : " desc");
                }

                var data = userList.Select(emList => new
                {
                    ID = emList.ID,
                    USER_NAME = emList.USER_NAME,
                    FULLNAME = emList.FULLNAME,
                    EMAIL = emList.EMAIL,
                    ROLE_ID = emList.ROLE_ID,
                    LOCKED = emList.LOCKED,
                    STATUS = emList.STATUS,
                    DATE_LAST_MODIFIED = emList.DATE_LAST_MODIFIED
                }).OrderBy(orderByString == string.Empty ? "ID asc" : orderByString).ToList();

                return Json(new DataTablesResponse(requestModel.Draw, data, totalCount, totalCount), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogError logerror = new LogError();
                logerror.ErrorLog("", LoginUser, "", "Admin/GetInternalUsers", "Admin", "GetInternalUsers", "FetchInternalUsers Error", ex.Message.ToString(), 0);
                throw new Exception(ex.Message.ToString());
            }
        }


        [Authorize]
        public JsonResult GetExternalUsers([ModelBinder(typeof(DataTablesBinder))] IDataTablesRequest requestModel)
        {

            string userkey = ConfigurationManager.AppSettings["userkey"];
            string uid = ConfigurationManager.AppSettings["uid"];
            string LoginUser = (string)Session["LoginSAPID"];

            try
            {
                Employer.Employer employer = new Employer.Employer();
                DataTable dt = employer.FetchExternalUsers(false, "", userkey, uid);
                
                dt.TableName = "ExternalUsers";
                dt.Columns.ToString();

                List<ExternalUsers> userList = new List<ExternalUsers>();
                int startRec = requestModel.Start;
                int pageSize = requestModel.Length;

                List<ExternalUsers> usersCount = (from DataRow dr in dt.Rows
                                                  select new ExternalUsers()
                                                  {
                                                      ID = dr["ID"].ToString()
                                                  }).ToList();

                userList = (from DataRow dr in dt.Rows
                            select new ExternalUsers()
                            {
                                ID = dr["ID"].ToString(),
                                EMAIL = dr["EMAIL"].ToString(),
                                PHONE = dr["PHONE"].ToString(),
                                FULLNAME = dr["FULLNAME"].ToString(),
                                LOCKED = dr["LOCKED"].ToString(),
                                STATUS = dr["STATUS"].ToString(),
                                PCODE = dr["PCODE"].ToString(),
                                /*STATUS = dr["EMPLOYER_ID"].ToString(),
                                STATUS = dr["APPROVAL_STATUS"].ToString(),
                                STATUS = dr["CREATED_BY"].ToString(),
                                STATUS = dr["DATE_CREATED"].ToString(),
                                STATUS = dr["APPROVED_BY"].ToString(),
                                STATUS = dr["STATUS"].ToString(),
                                DATE_LAST_MODIFIED = dr["DATE_APPROVED"].ToString()*/
                            }).Skip(startRec).Take(pageSize).ToList();

                var totalCount = usersCount.Count();
                var filteredCount = userList.Count();

                if (requestModel.Search.Value != string.Empty)
                {
                    var value = requestModel.Search.Value.Trim();

                    usersCount = (from DataRow dr in dt.Rows
                                  where dr["ID"].ToString().Contains(value) || dr["EMAIL"].ToString().Contains(value)
                                     || dr["FULLNAME"].ToString().Contains(value) || dr["PHONE"].ToString().Contains(value)
                                  select new ExternalUsers()
                                  {
                                      ID = dr["ID"].ToString()
                                  }).ToList();

                    userList = (from DataRow dr in dt.Rows
                                where dr["ID"].ToString().Contains(value) || dr["EMAIL"].ToString().Contains(value)
                                     || dr["FULLNAME"].ToString().Contains(value) || dr["PHONE"].ToString().Contains(value)
                                select new ExternalUsers()
                                {
                                    ID = dr["ID"].ToString(),
                                    EMAIL = dr["EMAIL"].ToString(),
                                    PHONE = dr["PHONE"].ToString(),
                                    FULLNAME = dr["FULLNAME"].ToString(),
                                    LOCKED = dr["LOCKED"].ToString(),
                                    STATUS = dr["STATUS"].ToString(),
                                    PCODE = dr["PCODE"].ToString(),
                                    /*STATUS = dr["EMPLOYER_ID"].ToString(),
                                    STATUS = dr["APPROVAL_STATUS"].ToString(),
                                    STATUS = dr["CREATED_BY"].ToString(),
                                    STATUS = dr["DATE_CREATED"].ToString(),
                                    STATUS = dr["APPROVED_BY"].ToString(),
                                    STATUS = dr["STATUS"].ToString(),*/
                                    DATE_LAST_MODIFIED = dr["DATE_APPROVED"].ToString()
                                }).Skip(startRec).Take(pageSize).ToList();

                    totalCount = usersCount.Count();
                    filteredCount = userList.Count();
                }

                var sortedColumns = requestModel.Columns.GetSortedColumns();
                var orderByString = String.Empty;

                foreach (var column in sortedColumns)
                {
                    orderByString += orderByString != String.Empty ? "," : "";
                    orderByString += (column.Data) + (column.SortDirection == Column.OrderDirection.Ascendant ? " asc" : " desc");
                }

                var data = userList.Select(emList => new
                {
                    ID = emList.ID,
                    PHONE = emList.PHONE,
                    FULLNAME = emList.FULLNAME,
                    EMAIL = emList.EMAIL,
                    LOCKED = emList.LOCKED,
                    STATUS = emList.STATUS,
                    DATE_LAST_MODIFIED = emList.DATE_LAST_MODIFIED
                }).ToList();

                return Json(new DataTablesResponse(requestModel.Draw, data, totalCount, totalCount), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogError logerror = new LogError();
                logerror.ErrorLog("", LoginUser, "", "Admin/GetInternalUsers", "Admin", "GetExternalUsers", "FetchExternalUsers Error", ex.Message.ToString(), 0);
                throw new Exception(ex.Message.ToString());
            }
        }

        [Authorize]
        public ActionResult UnlockUser(string StaffID)
        {
            string userkey = ConfigurationManager.AppSettings["userkey"];
            string uid = ConfigurationManager.AppSettings["uid"];
            string LoginUser = (string)Session["LoginSAPID"];
            try
            {
                Employer.Employer employer = new Employer.Employer();
                var updateStatus= employer.AdministerInternalUser(StaffID, LoginUser, "1", "2", userkey, uid);
                var updateDetails = updateStatus.Split('~');

                if (updateDetails[0] != "01")
                {
                    TempData["smessage"] = updateDetails[1];
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["message"] = updateDetails[1];
                    return RedirectToAction("Index");
                }
            }
            catch(Exception ex)
            {
                LogError logerror = new LogError();
                logerror.ErrorLog("", LoginUser, "", "Admin/UpdateUsers", "Admin", "UpdateUser", "AdministerInternalUser Error", ex.Message.ToString(), 0);
                TempData["error"] = ex.Message.ToString();
                ViewBag.Error = TempData["error"];
                return RedirectToAction("Index");
            }
        }

        
        [Authorize]
        public ActionResult ActivateUser(string StaffID)
        {
            string userkey = ConfigurationManager.AppSettings["userkey"];
            string uid = ConfigurationManager.AppSettings["uid"];
            string LoginUser = (string)Session["LoginSAPID"];
            try
            {
                Employer.Employer employer = new Employer.Employer();
                var updateStatus = employer.AdministerInternalUser(StaffID, LoginUser, "1", "4", userkey, uid);
                var updateDetails = updateStatus.Split('~');

                if (updateDetails[0] != "01")
                {
                    TempData["smessage"] = updateDetails[1];
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["message"] = updateDetails[1];
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                LogError logerror = new LogError();
                logerror.ErrorLog("", LoginUser, "", "Admin/UpdateUsers", "Admin", "ActivateUser", "AdministerInternalUser Error", ex.Message.ToString(), 0);
                TempData["error"] = ex.Message.ToString();
                ViewBag.Error = TempData["error"];
                return RedirectToAction("Index");
            }
        }


        [Authorize]
        public ActionResult DeactivateUser(string StaffID)
        {
            string userkey = ConfigurationManager.AppSettings["userkey"];
            string uid = ConfigurationManager.AppSettings["uid"];
            string LoginUser = (string)Session["LoginSAPID"];
            try
            {
                Employer.Employer employer = new Employer.Employer();
                var updateStatus = employer.AdministerInternalUser(StaffID, LoginUser, "1", "3", userkey, uid);
                var updateDetails = updateStatus.Split('~');

                if (updateDetails[0] != "01")
                {
                    TempData["smessage"] = updateDetails[1];
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["message"] = updateDetails[1];
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                LogError logerror = new LogError();
                logerror.ErrorLog("", LoginUser, "", "Admin/UpdateUsers", "Admin", "ActivateUser", "AdministerInternalUser Error", ex.Message.ToString(), 0);
                TempData["error"] = ex.Message.ToString();
                ViewBag.Error = TempData["error"];
                return RedirectToAction("Index");
            }
        }

        [Authorize]
        public ActionResult EditRole(string StaffID, string Role)
        {
            string userkey = ConfigurationManager.AppSettings["userkey"];
            string uid = ConfigurationManager.AppSettings["uid"];
            string LoginUser = (string)Session["LoginSAPID"];
            try
            {
                Employer.Employer employer = new Employer.Employer();
                var updateRole = employer.UpdateRole(Role, StaffID, userkey, uid);
                var updateDetails = updateRole.Split('~');

                if (updateDetails[0] != "01")
                {
                    TempData["smessage"] = updateDetails[1];
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["message"] = updateDetails[1];
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                LogError logerror = new LogError();
                logerror.ErrorLog("", LoginUser, "", "Admin/UpdateUsers", "Admin", "UpdateUser", "AdministerInternalUser Error", ex.Message.ToString(), 0);
                TempData["error"] = ex.Message.ToString();
                ViewBag.Error = TempData["error"];
                return RedirectToAction("Index");
            }
        }

        [Authorize]
        public ActionResult Index()
        {
            if (TempData["message"] != null)
            {
                ViewBag.Message = TempData["message"].ToString();
            }
            if (TempData["smessage"] != null)
            {
                ViewBag.sMessage = TempData["smessage"].ToString();
            }
            return View();
        }

        [Authorize]
        public ActionResult ExternalUsers()
        {
            if (TempData["message"] != null)
            {
                ViewBag.Message = TempData["message"].ToString();
            }
            if (TempData["smessage"] != null)
            {
                ViewBag.sMessage = TempData["smessage"].ToString();
            }
            return View();
        }

        [Authorize]
        public ActionResult Create()
        {
            return View();
        }


        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AdminUsers admin)
        {
            string LoginUser = (string)Session["LoginSAPID"];
            try
            {
                string userkey = ConfigurationManager.AppSettings["userkey"];
                string uid = ConfigurationManager.AppSettings["uid"];
                
                Employer.Employer employer = new Employer.Employer();

                var createAdmin = employer.AdministerInternalUser(admin.SAPID, LoginUser, admin.Role, "1", userkey, uid);

                var createStatus = createAdmin.Split('~');
                if (createStatus[0] != "01"){

                    TempData["error"] = createStatus[1];
                    ViewBag.Error = TempData["error"];
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["error"] = createStatus[1];
                    ViewBag.Error = TempData["error"];
                    return View();
                }
            }
            catch (Exception ex)
            {
                LogError logerror = new LogError();
                logerror.ErrorLog("", LoginUser, "", "Admin/Create", "Admin", "Create", "AdministerInternalUser Error", ex.Message.ToString(), 0);
                TempData["error"] = ex.Message.ToString();
                ViewBag.Error = TempData["error"];
                return View();
            }
        }

        [Authorize]
        public ActionResult Roles()
        {
            if (TempData["message"] != null)
            {
                ViewBag.Message = TempData["message"].ToString();
            }
            if (TempData["smessage"] != null)
            {
                ViewBag.sMessage = TempData["smessage"].ToString();
            }
            return View();
        }

        [Authorize]
        public ActionResult CreateRole()
        {
            return View();
        }


        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateRole(UserRoles role)
        {
            string LoginUser = (string)Session["LoginSAPID"];
            try
            {
                string userkey = ConfigurationManager.AppSettings["userkey"];
                string uid = ConfigurationManager.AppSettings["uid"];

                Employer.Employer employer = new Employer.Employer();

                var createRole = employer.CreateRole(role.Name, LoginUser, userkey, uid);

                var createStatus = createRole.Split('~');
                if (createStatus[0] != "01")
                {

                    TempData["error"] = createStatus[1];
                    ViewBag.Error = TempData["error"];
                    //return RedirectToAction("Roles");
                    return View();
                }
                else
                {
                    TempData["error"] = createStatus[1];
                    ViewBag.Error = TempData["error"];
                    return View();
                }
            }
            catch (Exception ex)
            {
                LogError logerror = new LogError();
                logerror.ErrorLog("", LoginUser, "", "Admin/CreateRole", "Admin", "CreateRole", "CreateRole Error", ex.Message.ToString(), 0);
                TempData["error"] = ex.Message.ToString();
                ViewBag.Error = TempData["error"];
                return View();
            }
        }

        [Authorize]
        public JsonResult GetRoles()
        {
            string LoginUser = (string)Session["LoginSAPID"];
            try
            {
                string userkey = ConfigurationManager.AppSettings["userkey"];
                string uid = ConfigurationManager.AppSettings["uid"];

                Employer.Employer employer = new Employer.Employer();
                DataTable dt = employer.FetchAllRoles();
                dt.TableName = "FetchRoles";
                var roles = (from DataRow dr in dt.Rows
                             where dr["isexternal"].ToString().Trim() == "Y"
                             select new Models.UserRoles()
                             {
                                 ID = dr["ID"].ToString(),
                                 Name = dr["Name"].ToString(),
                                 Code = dr["Code"].ToString()
                             }).ToList();

                return Json(new { data = roles }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex){
                LogError logerror = new LogError();
                logerror.ErrorLog("", LoginUser, "", "Admin/GetRoles", "Admin", "GetRoles", "FetchAllRoles Error", ex.Message.ToString(), 0);
                return Json(new { data = "Error has occured" }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetAdminRoles()
        {
            string LoginUser = (string)Session["LoginSAPID"];
            try
            {
                string userkey = ConfigurationManager.AppSettings["userkey"];
                string uid = ConfigurationManager.AppSettings["uid"];

                Employer.Employer employer = new Employer.Employer();
                DataTable dt = employer.FetchAllRoles();
                dt.TableName = "FetchRoles";
                var roles = (from DataRow dr in dt.Rows
                             where dr["isexternal"].ToString().Trim() != "Y"
                             select new Models.UserRoles()
                             {
                                 ID = dr["ID"].ToString(),
                                 Name = dr["Name"].ToString(),
                                 Code = dr["Code"].ToString()
                             }).ToList();

                return Json(new { data = roles }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogError logerror = new LogError();
                logerror.ErrorLog("", LoginUser, "", "Admin/GetRoles", "Admin", "GetRoles", "FetchAllRoles Error", ex.Message.ToString(), 0);
                return Json(new { data = "Error has occured" }, JsonRequestBehavior.AllowGet);
            }
        }


        [Authorize]
        /*public JsonResult GetInternalUsers()
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
                                 FULLNAME = dr["Name"].ToString()
                             }).ToList();

                return Json(new { data = users }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogError logerror = new LogError();
                logerror.ErrorLog("", LoginUser, "", "Admin/GetUsers", "Admin", "GetRoles", "FetchInternalUsers Error", ex.Message.ToString(), 0);
                return Json(new { data = "Error has occured" }, JsonRequestBehavior.AllowGet);
            }
        }*/


        [Authorize]
        public ActionResult Audits()
        {
            return View();
        }


        [Authorize]
        public JsonResult GetAudits([ModelBinder(typeof(DataTablesBinder))] IDataTablesRequest requestModel)
        {

            string userkey = ConfigurationManager.AppSettings["userkey"];
            string uid = ConfigurationManager.AppSettings["uid"];
            string LoginUser = (string)Session["LoginSAPID"];
            try
            {
                Employer.Employer employer = new Employer.Employer();
                DataTable dt = employer.Getaudittrail(LoginUser, userkey, uid);
                dt.TableName = "Audit";
                dt.Columns.ToString();

                List<Audits> audits = new List<Audits>();
                int startRec = requestModel.Start;
                int pageSize = requestModel.Length;


                List<Audits> auditCount = (from DataRow dr in dt.Rows
                                             select new Audits()
                                             {
                                                 ID = dr["ID"].ToString()
                                             }).ToList();

                audits = (from DataRow dr in dt.Rows
                          orderby dr["ID"] descending
                          select new Audits()
                         {
                             ID = dr["ID"].ToString(),
                             TABLE_NAME = dr["TABLE_NAME"].ToString(),
                             COLUMN_NAME = dr["COLUMN_NAME"].ToString(),
                             OLD_VALUE = dr["OLD_VALUE"].ToString(),
                             NEW_VALUE = dr["NEW_VALUE"].ToString(),
                             RECORD_ID = dr["RECORD_ID"].ToString(),
                             OLD_VALUE2 = dr["OLD_VALUE2"].ToString(),
                             NEW_VALUE2 = dr["NEW_VALUE2"].ToString(),
                             RECORD_ID2 = dr["RECORD_ID2"].ToString(),
                             RECORD_COLUMN = dr["RECORD_COLUMN"].ToString(),
                             CHANGED_BY = dr["CHANGED_BY"].ToString(),
                             DATE_CHANGED = Convert.ToDateTime(dr["DATE_CHANGED"]).ToString("dd-MMM-yyyy hh:mm"),
                             APPROVED_BY = dr["APPROVED_BY"].ToString(),
                             DATE_APPROVED = Convert.ToDateTime(dr["DATE_APPROVED"]).ToString("dd-MMM-yyyy hh:mm"),
                             STATUS = dr["STATUS"].ToString(),
                             CHANGE_TYPE = dr["CHANGE_TYPE"].ToString(),
                             COLUMN_TYPE = dr["COLUMN_TYPE"].ToString(),
                             RECORD_COLUMN_TYPE = dr["RECORD_COLUMN_TYPE"].ToString(),
                             NOTE = dr["NOTE"].ToString()
                         }).Skip(startRec).Take(pageSize).ToList();

                var totalCount = auditCount.Count();
                var filteredCount = audits.Count();

                var sortedColumns = requestModel.Columns.GetSortedColumns();
                var orderByString = String.Empty;

                foreach (var column in sortedColumns)
                {
                    orderByString += orderByString != String.Empty ? "," : "";
                    orderByString += (column.Data) + (column.SortDirection == Column.OrderDirection.Ascendant ? " asc" : " desc");
                }

                var data = audits.Select(emList => new
                {
                    ID = emList.ID,
                    TABLE_NAME = emList.TABLE_NAME,
                    COLUMN_NAME = emList.COLUMN_NAME,
                    OLD_VALUE = emList.OLD_VALUE,
                    NEW_VALUE = emList.NEW_VALUE,
                    RECORD_ID = emList.RECORD_ID,
                    OLD_VALUE2 = emList.OLD_VALUE2,
                    NEW_VALUE2 = emList.NEW_VALUE2,
                    RECORD_ID2 = emList.RECORD_ID2,
                    RECORD_COLUMN = emList.RECORD_COLUMN,
                    CHANGED_BY = emList.CHANGED_BY,
                    DATE_CHANGED = emList.DATE_CHANGED,
                    APPROVED_BY = emList.APPROVED_BY,
                    DATE_APPROVED = emList.DATE_APPROVED,
                    STATUS = emList.STATUS,
                    CHANGE_TYPE = emList.CHANGE_TYPE,
                    COLUMN_TYPE = emList.COLUMN_TYPE,
                    RECORD_COLUMN_TYPE = emList.RECORD_COLUMN_TYPE,
                    NOTE = emList.NOTE
                }).ToList();

                return Json(new DataTablesResponse(requestModel.Draw, data, totalCount, totalCount), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogError logerror = new LogError();
                logerror.ErrorLog("", LoginUser, "", "AdminController/GetAudits", "AdminController", "GetAudits", "Getaudittrail Error", ex.Message.ToString(), 0);
                throw new Exception(ex.Message.ToString());
            }
        }


        

        [Authorize]
        public ActionResult CreateEmployerContact()
        {
            if (TempData["message"] != null)
            {
                ViewBag.Message = TempData["message"].ToString();
            }
            if (TempData["smessage"] != null)
            {
                ViewBag.sMessage = TempData["smessage"].ToString();
            }
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateEmployerContact(EmployerContacts contact, String EmployerId)
        {
            string LoginUser = (string)Session["LoginSAPID"];
            string userkey = ConfigurationManager.AppSettings["userkey"];
            string uid = ConfigurationManager.AppSettings["uid"];

            try
            {
                Employer.Employer employer = new Employer.Employer();

                var createEmContact = employer.AdministerExternalUser(contact.EmployerID, "", contact.Email, contact.Fullname, contact.PhoneNo, LoginUser, contact.Role, "1", userkey, uid);

                var createStatus = createEmContact.Split('~');

                if (createStatus[0] != "01")
                {

                    TempData["error"] = createStatus[1];
                    ViewBag.Error = TempData["error"];
                    return View();
                }
                else
                {
                    TempData["error"] = createStatus[1];
                    ViewBag.Error = TempData["error"];
                    return View();
                }
            }
            catch (Exception ex)
            {
                LogError logerror = new LogError();
                logerror.ErrorLog("", LoginUser, "", "Admin/CreateRole", "Admin", "CreateRole", "CreateRole Error", ex.Message.ToString(), 0);
                TempData["error"] = ex.Message.ToString();
                ViewBag.Error = TempData["error"];
                return View();
            }
        }

        [Authorize]
        [HttpPost]
        public JsonResult GetEmployersList(string Prefix)
        {
            string userkey = ConfigurationManager.AppSettings["userkey"];
            string uid = ConfigurationManager.AppSettings["uid"];
            string LoginUser = (string)Session["LoginSAPID"];
            string RoleID = (string)Session["RoleID"];
            string WebUserID = (string)Session["WebUserID"];
            try
            {
                if (RoleID == "1")
                {
                    Employer.Employer employer = new Employer.Employer();
                    DataTable dt = employer.FetchCompanies("Yes", LoginUser);
                    dt.TableName = "Employer";

                    //Note : you can bind same list from database  
                    List<Employers> employers = (from DataRow dr in dt.Rows
                                                 select new Employers()
                                                 {
                                                     EmployerID = dr["EmployerID"].ToString().ToUpper(),
                                                     CompanyName = dr["Company Name"].ToString().ToUpper(),
                                                 }).ToList();

                    //Searching records from list using LINQ query  
                    var employersList = (from DataRow dr in dt.Rows
                                         where dr["Company Name"].ToString().ToUpper().StartsWith(Prefix.ToUpper())
                                         select new Employers()
                                         {
                                             EmployerID = dr["EmployerID"].ToString().ToUpper(),
                                             CompanyName = dr["Company Name"].ToString().ToUpper(),
                                         }).ToList();
                    return Json(new { data = employersList }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    Employer.Employer employer = new Employer.Employer();
                    DataTable dt = employer.FetchCompanies("All", "");
                    dt.TableName = "Employer";

                    //Note : you can bind same list from database  
                    List<Employers> employers = (from DataRow dr in dt.Rows
                                                 select new Employers()
                                                 {
                                                     EmployerID = dr["EmployerID"].ToString().ToUpper(),
                                                     CompanyName = dr["Company Name"].ToString().ToUpper(),
                                                 }).ToList();

                    //Searching records from list using LINQ query  
                    var employersList = (from DataRow dr in dt.Rows
                                         where dr["Company Name"].ToString().ToUpper().StartsWith(Prefix.ToUpper())
                                         select new Employers()
                                         {
                                             EmployerID = dr["EmployerID"].ToString().ToUpper(),
                                             CompanyName = dr["Company Name"].ToString().ToUpper(),
                                         }).ToList();
                    return Json(new { data = employersList }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogError logerror = new LogError();
                logerror.ErrorLog("", LoginUser, "", "Employer/GetEmployers", "Employer", "GetEmployers", "GetEmployers Error", ex.Message.ToString(), 0);
                throw new Exception(ex.Message.ToString());
            }
        }

        [Authorize]
        public JsonResult GetValidSapId(string SAPID)
        {
            string userkey = ConfigurationManager.AppSettings["userkey"];
            string uid = ConfigurationManager.AppSettings["uid"];
            string LoginUser = (string)Session["LoginSAPID"];
            try
            {
                string ipaddress = Request.UserHostAddress;
                string BrowserUsed = Request.UserAgent;

                Employer.Employer employer = new Employer.Employer();
                var validDetails = employer.ADAuth(SAPID, BrowserUsed, ipaddress, "", userkey, uid);
                var sapDetails = validDetails.Split('~');
                return Json(new { data = sapDetails }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogError logerror = new LogError();
                logerror.ErrorLog("", LoginUser, "", "Admin/GetValidSapId", "Admin", "GetValidSapId", "ADAuth Error", ex.Message.ToString(), 0);
                throw new Exception(ex.Message.ToString());
            }
        }
    }
}