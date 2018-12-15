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
    public class EmployersController : Controller
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

        // GET: Employer
        [System.Xml.Serialization.XmlInclude(typeof(Object[]))]

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
        public JsonResult GetEmployers([ModelBinder(typeof(DataTablesBinder))] IDataTablesRequest requestModel)
        {
            
            string userkey = ConfigurationManager.AppSettings["userkey"];
            string uid = ConfigurationManager.AppSettings["uid"];
            string LoginUser = (string)Session["LoginSAPID"];
            string RoleID = (string)Session["RoleID"];
            string WebUserID = (string)Session["WebUserID"];

            try
            {
                
                if (RoleID == "1") {
                    Employer.Employer employer = new Employer.Employer();
                    DataTable dt = employer.FetchCompanies("YES", LoginUser);
                    dt.TableName = "Employer";

                    List<Employers> employerList = new List<Employers>();
                    int startRec = requestModel.Start;
                    int pageSize = requestModel.Length;


                    List<Employers> employerCount = (from DataRow dr in dt.Rows
                                                     select new Employers()
                                                     {
                                                         EmployerID = dr["EmployerID"].ToString(),
                                                         CompanyName = dr["Company Name"].ToString(),
                                                     }).ToList();

                    employerList = (from DataRow dr in dt.Rows
                                    select new Employers()
                                    {
                                        EmployerID = dr["EmployerID"].ToString(),
                                        CompanyName = dr["Company Name"].ToString(),
                                        Address1 = dr["Address1"].ToString(),
                                        Address2 = dr["Address2"].ToString(),
                                        State = dr["State"].ToString(),
                                        Email = dr["Email"].ToString(),
                                        RelationshipManager = dr["Relationship Manager"].ToString(),
                                        RMName = dr["RM Name"].ToString()
                                    }).Skip(startRec).Take(pageSize).ToList();

                    var totalCount = employerCount.Count();
                    var filteredCount = employerList.Count();

                    if (requestModel.Search.Value != string.Empty)
                    {
                        var value = requestModel.Search.Value.ToLower().Trim();

                        employerCount = (from DataRow dr in dt.Rows
                                         where dr["EmployerID"].ToString().ToLower().Contains(value) || dr["Company Name"].ToString().ToLower().Contains(value)
                                            || dr["Email"].ToString().ToLower().Contains(value) || dr["Relationship Manager"].ToString().ToLower().Contains(value)
                                            || dr["RM Name"].ToString().ToLower().Contains(value)
                                         select new Employers()
                                         {
                                             EmployerID = dr["EmployerID"].ToString(),
                                             CompanyName = dr["Company Name"].ToString(),
                                         }).ToList();

                        employerList = (from DataRow dr in dt.Rows
                                        where dr["EmployerID"].ToString().ToLower().Contains(value) || dr["Company Name"].ToString().ToLower().Contains(value)
                                            || dr["Email"].ToString().ToLower().Contains(value) || dr["Relationship Manager"].ToString().ToLower().Contains(value)
                                            || dr["RM Name"].ToString().ToLower().Contains(value)
                                        select new Employers()
                                        {
                                            EmployerID = dr["EmployerID"].ToString(),
                                            CompanyName = dr["Company Name"].ToString(),
                                            Address1 = dr["Address1"].ToString(),
                                            Address2 = dr["Address2"].ToString(),
                                            State = dr["State"].ToString(),
                                            Email = dr["Email"].ToString(),
                                            RelationshipManager = dr["Relationship Manager"].ToString(),
                                            RMName = dr["RM Name"].ToString()
                                        }).Skip(startRec).Take(pageSize).ToList();

                        totalCount = employerCount.Count();
                        filteredCount = employerList.Count();

                    }

                    var sortedColumns = requestModel.Columns.GetSortedColumns();
                    var orderByString = String.Empty;

                    foreach (var column in sortedColumns)
                    {
                        orderByString += orderByString != String.Empty ? "," : "";
                        orderByString += (column.Data) + (column.SortDirection == Column.OrderDirection.Ascendant ? " asc" : " desc");
                    }

                    var data = employerList.Select(emList => new
                    {
                        EmployerID = emList.EmployerID,
                        CompanyName = emList.CompanyName,
                        Address1 = emList.Address1,
                        Address2 = emList.Address2,
                        State = emList.State,
                        Email = emList.Email,
                        RelationshipManager = emList.RelationshipManager,
                        RMName = emList.RMName
                    }).OrderBy(orderByString == string.Empty ? "EmployerID asc" : orderByString).ToList();

                    return Json(new DataTablesResponse(requestModel.Draw, data, totalCount, totalCount), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    Employer.Employer employer = new Employer.Employer();
                    DataTable dt = employer.FetchCompanies("All", "");
                    dt.TableName = "Employer";

                    List<Employers> employerList = new List<Employers>();
                    int startRec = requestModel.Start;
                    int pageSize = requestModel.Length;


                    List<Employers> employerCount = (from DataRow dr in dt.Rows
                                                     select new Employers()
                                                     {
                                                         EmployerID = dr["EmployerID"].ToString(),
                                                         CompanyName = dr["Company Name"].ToString(),
                                                     }).ToList();

                    employerList = (from DataRow dr in dt.Rows
                                    select new Employers()
                                    {
                                        EmployerID = dr["EmployerID"].ToString(),
                                        CompanyName = dr["Company Name"].ToString(),
                                        Address1 = dr["Address1"].ToString(),
                                        Address2 = dr["Address2"].ToString(),
                                        State = dr["State"].ToString(),
                                        Email = dr["Email"].ToString(),
                                        RelationshipManager = dr["Relationship Manager"].ToString(),
                                        RMName = dr["RM Name"].ToString()
                                    }).Skip(startRec).Take(pageSize).ToList();

                    var totalCount = employerCount.Count();
                    var filteredCount = employerList.Count();

                    if (requestModel.Search.Value != string.Empty)
                    {
                        var value = requestModel.Search.Value.ToLower().Trim();

                        employerCount = (from DataRow dr in dt.Rows
                                         where dr["EmployerID"].ToString().ToLower().Contains(value) || dr["Company Name"].ToString().ToLower().Contains(value)
                                            || dr["Email"].ToString().ToLower().Contains(value) || dr["Relationship Manager"].ToString().ToLower().Contains(value)
                                            || dr["RM Name"].ToString().ToLower().Contains(value)
                                         select new Employers()
                                         {
                                             EmployerID = dr["EmployerID"].ToString(),
                                             CompanyName = dr["Company Name"].ToString(),
                                         }).ToList();

                        employerList = (from DataRow dr in dt.Rows
                                        where dr["EmployerID"].ToString().ToLower().Contains(value) || dr["Company Name"].ToString().ToLower().Contains(value)
                                            || dr["Email"].ToString().ToLower().Contains(value) || dr["Relationship Manager"].ToString().ToLower().Contains(value)
                                            || dr["RM Name"].ToString().ToLower().Contains(value)
                                        select new Employers()
                                        {
                                            EmployerID = dr["EmployerID"].ToString(),
                                            CompanyName = dr["Company Name"].ToString(),
                                            Address1 = dr["Address1"].ToString(),
                                            Address2 = dr["Address2"].ToString(),
                                            State = dr["State"].ToString(),
                                            Email = dr["Email"].ToString(),
                                            RelationshipManager = dr["Relationship Manager"].ToString(),
                                            RMName = dr["RM Name"].ToString()
                                        }).Skip(startRec).Take(pageSize).ToList();

                        totalCount = employerCount.Count();
                        filteredCount = employerList.Count();

                    }

                    var sortedColumns = requestModel.Columns.GetSortedColumns();
                    var orderByString = String.Empty;

                    foreach (var column in sortedColumns)
                    {
                        orderByString += orderByString != String.Empty ? "," : "";
                        orderByString += (column.Data) + (column.SortDirection == Column.OrderDirection.Ascendant ? " asc" : " desc");
                    }

                    var data = employerList.Select(emList => new
                    {
                        EmployerID = emList.EmployerID,
                        CompanyName = emList.CompanyName,
                        Address1 = emList.Address1,
                        Address2 = emList.Address2,
                        State = emList.State,
                        Email = emList.Email,
                        RelationshipManager = emList.RelationshipManager,
                        RMName = emList.RMName
                    }).OrderBy(orderByString == string.Empty ? "EmployerID asc" : orderByString).ToList();

                    return Json(new DataTablesResponse(requestModel.Draw, data, totalCount, totalCount), JsonRequestBehavior.AllowGet);
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
        public ActionResult ViewStaffs(String EmployerId)
        {
            if (EmployerId == null)
            {
                TempData["error"] = "EmployerID was not found";
                ViewBag.Error = TempData["error"];
                return RedirectToAction("Index");
            }
            return View();
        }


        [Authorize]
        public JsonResult GetCompanyEmployee([ModelBinder(typeof(DataTablesBinder))] IDataTablesRequest requestModel, String EmployerId)
        {

            string userkey = ConfigurationManager.AppSettings["userkey"];
            string uid = ConfigurationManager.AppSettings["uid"];
            string LoginUser = (string)Session["LoginSAPID"];
            try
            {
                Employer.Employer employer = new Employer.Employer();
                DataTable dt = employer.FetchCompanYEmployees(EmployerId, userkey, uid);
                dt.TableName = "CompanyEmployees";
                dt.Columns.ToString();

                List<CompanyEmployee> companyEmployee = new List<CompanyEmployee>();
                int startRec = requestModel.Start;
                int pageSize = requestModel.Length;


                List<CompanyEmployee> employeeCount = (from DataRow dr in dt.Rows
                                                         select new CompanyEmployee()
                                                         {
                                                             PIN = dr["P_I_N"].ToString()
                                                         }).ToList();

                companyEmployee = (from DataRow dr in dt.Rows
                                    select new CompanyEmployee()
                                    {
                                        PIN = dr["P_I_N"].ToString(),
                                        FirstName = dr["First Name"].ToString(),
                                        LastName = dr["Last Name"].ToString(),
                                        MiddleName = dr["Middle Names"].ToString(),
                                        PhoneNo = dr["Mobile 1"].ToString(),
                                        DateOfBirth = dr["Date Of Birth"].ToString(),
                                        DateOfEmployment = dr["Date Of Employment"].ToString(),
                                        Email = dr["E-mail"].ToString(),
                                    }).Skip(startRec).Take(pageSize).ToList();

                var totalCount = employeeCount.Count();
                var filteredCount = companyEmployee.Count();

                if (requestModel.Search.Value != string.Empty)
                {
                    var value = requestModel.Search.Value.ToLower().Trim();

                    employeeCount = (from DataRow dr in dt.Rows
                                     where dr["P_I_N"].ToString().ToString().ToLower().Contains(value) || dr["First Name"].ToString().ToLower().Contains(value)
                                        || dr["Last Name"].ToString().ToLower().Contains(value) || dr["Mobile 1"].ToString().ToLower().Contains(value)
                                        || dr["E-mail"].ToString().ToLower().Contains(value)
                                     select new CompanyEmployee()
                                     {
                                         PIN = dr["P_I_N"].ToString()
                                     }).ToList();

                    companyEmployee = (from DataRow dr in dt.Rows
                                       where dr["P_I_N"].ToString().ToString().ToLower().Contains(value) || dr["First Name"].ToString().ToLower().Contains(value)
                                        || dr["Last Name"].ToString().ToLower().Contains(value) || dr["Mobile 1"].ToString().ToLower().Contains(value)
                                        || dr["E-mail"].ToString().ToLower().Contains(value)
                                       select new CompanyEmployee()
                                        {
                                            PIN = dr["P_I_N"].ToString(),
                                            FirstName = dr["First Name"].ToString(),
                                            LastName = dr["Last Name"].ToString(),
                                            MiddleName = dr["Middle Names"].ToString(),
                                            PhoneNo = dr["Mobile 1"].ToString(),
                                            DateOfBirth = dr["Date Of Birth"].ToString(),
                                            DateOfEmployment = dr["Date Of Employment"].ToString(),
                                            Email = dr["E-mail"].ToString(),
                                       }).Skip(startRec).Take(pageSize).ToList();

                    totalCount = employeeCount.Count();
                    filteredCount = companyEmployee.Count();
                }

                var sortedColumns = requestModel.Columns.GetSortedColumns();
                var orderByString = String.Empty;

                foreach (var column in sortedColumns)
                {
                    orderByString += orderByString != String.Empty ? "," : "";
                    orderByString += (column.Data) + (column.SortDirection == Column.OrderDirection.Ascendant ? " asc" : " desc");
                }

                var data = companyEmployee.Select(emList => new
                {
                    PIN = emList.PIN,
                    FirstName = emList.FirstName,
                    LastName = emList.LastName,
                    MiddleName = emList.MiddleName,
                    PhoneNo = emList.PhoneNo,
                    DateOfBirth = emList.DateOfBirth,
                    DateOfEmployment = emList.DateOfEmployment,
                    Email = emList.Email,
                }).OrderBy(orderByString == string.Empty ? "PIN asc" : orderByString).ToList();

                return Json(new DataTablesResponse(requestModel.Draw, data, totalCount, totalCount), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogError logerror = new LogError();
                logerror.ErrorLog("", LoginUser, "", "Employer/GetCompanyEmployee", "Employer", "GetCompanyEmployee", "GetCompanyEmployee Error", ex.Message.ToString(), 0);
                throw new Exception(ex.Message.ToString());
            }
        }

        [Authorize]
        public ActionResult Details(int id)
        {
            return View();
        }

        [Authorize]
        public ActionResult AllEmployerContact()
        {
            return View();
        }

        [Authorize]
        public ActionResult CreateEmployerContact()
        {
            return View();
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
                var updateStatus = employer.AdministerInternalUser(StaffID, LoginUser, "1", "2", userkey, uid);
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

                    TempData["Msg"] = createStatus[1];
                    ViewBag.sMessage = TempData["error"];
                    return RedirectToAction("Index");
                    //return View();
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
        public ActionResult ViewContacts(String EmployerId)
        {
            if (EmployerId == null)
            {
                TempData["error"] = "EmployerID was not found";
                ViewBag.Error = TempData["error"];
                return RedirectToAction("Index");
            }
            return View();
        }

        [Authorize]
        public JsonResult GetEmployerContacts([ModelBinder(typeof(DataTablesBinder))] IDataTablesRequest requestModel, string EmployerId)
        {

            string userkey = ConfigurationManager.AppSettings["userkey"];
            string uid = ConfigurationManager.AppSettings["uid"];
            string LoginUser = (string)Session["LoginSAPID"];

            try
            {
                Employer.Employer employer = new Employer.Employer();
                DataTable dt = employer.FetchExternalUsers(true, EmployerId, userkey, uid);

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
                                ROLE_ID = dr["ROLE_ID"].ToString(),
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
                    var value = requestModel.Search.Value.ToLower().Trim();

                    usersCount = (from DataRow dr in dt.Rows
                                  where dr["ID"].ToString().ToLower().Contains(value) || dr["EMAIL"].ToString().ToLower().Contains(value)
                                     || dr["FULLNAME"].ToString().ToLower().Contains(value) || dr["PHONE"].ToString().ToLower().Contains(value)
                                  select new ExternalUsers()
                                  {
                                      ID = dr["ID"].ToString()
                                  }).ToList();

                    userList = (from DataRow dr in dt.Rows
                                where dr["ID"].ToString().ToLower().Contains(value) || dr["EMAIL"].ToString().ToLower().Contains(value)
                                     || dr["FULLNAME"].ToString().ToLower().Contains(value) || dr["PHONE"].ToString().ToLower().Contains(value)
                                select new ExternalUsers()
                                {
                                    ID = dr["ID"].ToString(),
                                    EMAIL = dr["EMAIL"].ToString(),
                                    PHONE = dr["PHONE"].ToString(),
                                    FULLNAME = dr["FULLNAME"].ToString(),
                                    LOCKED = dr["LOCKED"].ToString(),
                                    STATUS = dr["STATUS"].ToString(),
                                    PCODE = dr["PCODE"].ToString(),
                                    ROLE_ID = dr["ROLE_ID"].ToString(),
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
                    ROLE_ID = emList.ROLE_ID,
                    DATE_LAST_MODIFIED = emList.DATE_LAST_MODIFIED
                }).OrderBy(orderByString == string.Empty ? "ID asc" : orderByString).ToList();

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
        public ActionResult EditEmployerContact()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditEmployerContact(EmployerContacts contact, String EmployerId)
        {
            string LoginUser = (string)Session["LoginSAPID"];
            string userkey = ConfigurationManager.AppSettings["userkey"];
            string uid = ConfigurationManager.AppSettings["uid"];

            try
            {
                Employer.Employer employer = new Employer.Employer();
                var createEmContact = employer.ModifyExternalUser(contact.EmployerID, "", contact.Email, contact.Fullname, contact.PhoneNo, LoginUser, contact.Role, contact.ID, userkey, uid);
                var createStatus = createEmContact.Split('~');

                if (createStatus[0] != "01")
                {

                    TempData["Msg"] = createStatus[1];
                    ViewBag.sMessage = TempData["Msg"];
                    return RedirectToAction("Index");
                    //return View();
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

       
    }
}
