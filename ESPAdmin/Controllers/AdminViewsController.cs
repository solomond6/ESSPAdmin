using ESPAdmin.Models;
using ESPAdmin.Utility;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;
using System.IO;
using DataTables.Mvc;

namespace ESPAdmin.Controllers
{
    public class AdminViewsController : Controller
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
        // GET: Login
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

        public JsonResult GetAdminViews([ModelBinder(typeof(DataTablesBinder))] IDataTablesRequest requestModel)
        {

            string userkey = ConfigurationManager.AppSettings["userkey"];
            string uid = ConfigurationManager.AppSettings["uid"];
            string LoginUser = (string)Session["LoginSAPID"];
            try
            {
                Employer.Employer employer = new Employer.Employer();
                DataTable dt = employer.FetchViews(userkey, uid);
                dt.TableName = "AdminViews";
                dt.Columns.ToString();
                List<AdminViews> adminViews = new List<AdminViews>();
                int startRec = requestModel.Start;
                int pageSize = requestModel.Length;


                List<AdminViews> viewsCount = (from DataRow dr in dt.Rows
                                                select new AdminViews()
                                                {
                                                    MenuName = dr["MenuName"].ToString()
                                                }).ToList();

                adminViews = (from DataRow dr in dt.Rows
                                   select new AdminViews()
                                   {
                                       MenuName = dr["MenuName"].ToString(),
                                       ControllerName = dr["Controller"].ToString(),
                                       ActionName = dr["Action"].ToString(),
                                   }).Skip(startRec).Take(pageSize).ToList();

                var totalCount = viewsCount.Count();
                var filteredCount = adminViews.Count();

                /*if (requestModel.Search.Value != string.Empty)
                {
                    var value = requestModel.Search.Value.Trim();

                    employeeCount = (from DataRow dr in dt.Rows
                                     where dr["P_I_N"].ToString().ToString().Contains(value) || dr["First Name"].ToString().Contains(value)
                                        || dr["Last Name"].ToString().Contains(value) || dr["Phone No"].ToString().Contains(value)
                                        || dr["E-mail"].ToString().Contains(value)
                                     select new CompanyEmployee()
                                     {
                                         PIN = dr["P_I_N"].ToString()
                                     }).ToList();

                    companyEmployee = (from DataRow dr in dt.Rows
                                       where dr["P_I_N"].ToString().ToString().Contains(value) || dr["First Name"].ToString().Contains(value)
                                        || dr["Last Name"].ToString().Contains(value) || dr["Phone No"].ToString().Contains(value)
                                        || dr["E-mail"].ToString().Contains(value)
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
                }*/

                var sortedColumns = requestModel.Columns.GetSortedColumns();
                var orderByString = String.Empty;

                foreach (var column in sortedColumns)
                {
                    orderByString += orderByString != String.Empty ? "," : "";
                    orderByString += (column.Data) + (column.SortDirection == Column.OrderDirection.Ascendant ? " asc" : " desc");
                }

                var data = adminViews.Select(emList => new
                {
                    MenuName = emList.MenuName,
                    ControllerName = emList.ControllerName,
                    ActionName = emList.ActionName
                }).ToList();

                return Json(new DataTablesResponse(requestModel.Draw, data, totalCount, totalCount), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogError logerror = new LogError();
                logerror.ErrorLog("", LoginUser, "", "AdminViewsController/GetAdminViews", "AdminViewsController", "GetAdminViews", "FetchViews Error", ex.Message.ToString(), 0);
                throw new Exception(ex.Message.ToString());
            }
        }

        public ActionResult Create()
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AdminViews views)
        {
            string userkey = ConfigurationManager.AppSettings["userkey"];
            string uid = ConfigurationManager.AppSettings["uid"];
            string LoginUser = (string)Session["LoginSAPID"];

            try
            {
                Employer.Employer employer = new Employer.Employer();
                var defineRole = employer.DefineView(views.MenuName, views.ControllerName, views.ActionName, "INP211",userkey, uid);

                var roleDetails = defineRole.Split('~');

                if (roleDetails[0] != "01")
                {
                    TempData["msg"] = roleDetails[1];
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["error"] = roleDetails[1];
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                LogError logerror = new LogError();
                logerror.ErrorLog("", LoginUser, "", "AdminViews/Create", "AdminViews", "Create", "DefineView Error", ex.Message.ToString(), 0);
                TempData["error"] = ex.Message.ToString();
                ViewBag.Error = TempData["error"];
                return View();
            }
        }

        public ActionResult MapViewsToRole()
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

        public JsonResult GetAllRoles([ModelBinder(typeof(DataTablesBinder))] IDataTablesRequest requestModel)
        {

            string userkey = ConfigurationManager.AppSettings["userkey"];
            string uid = ConfigurationManager.AppSettings["uid"];
            string LoginUser = (string)Session["LoginSAPID"];
            try
            {
                Employer.Employer employer = new Employer.Employer();
                DataTable dt = employer.FetchAllRoles();
                dt.TableName = "Roles";

                List<UserRoles> roles = new List<UserRoles>();
                int startRec = requestModel.Start;
                int pageSize = requestModel.Length;


                List<UserRoles> roleCount = (from DataRow dr in dt.Rows
                                               select new UserRoles()
                                               {
                                                   ID = dr["ID"].ToString()
                                               }).ToList();

                roles = (from DataRow dr in dt.Rows
                              select new UserRoles()
                              {
                                  ID = dr["ID"].ToString(),
                                  Name = dr["NAME"].ToString(),
                                  Code = dr["CODE"].ToString(),
                              }).Skip(startRec).Take(pageSize).ToList();

                var totalCount = roleCount.Count();
                var filteredCount = roleCount.Count();

                var sortedColumns = requestModel.Columns.GetSortedColumns();
                var orderByString = String.Empty;

                foreach (var column in sortedColumns)
                {
                    orderByString += orderByString != String.Empty ? "," : "";
                    orderByString += (column.Data) + (column.SortDirection == Column.OrderDirection.Ascendant ? " asc" : " desc");
                }

                var data = roles.Select(emList => new
                {
                    ID = emList.ID,
                    Name = emList.Name,
                    Code = emList.Code
                }).ToList();

                return Json(new DataTablesResponse(requestModel.Draw, data, totalCount, totalCount), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogError logerror = new LogError();
                logerror.ErrorLog("", LoginUser, "", "AdminViewsController/GetAllRoles", "AdminViewsController", "GetAllRoles", "FetchAllRoles Error", ex.Message.ToString(), 0);
                throw new Exception(ex.Message.ToString());
            }
        }

        public ActionResult EditRole(string Id, string Name)
        {
            string userkey = ConfigurationManager.AppSettings["userkey"];
            string uid = ConfigurationManager.AppSettings["uid"];
            string LoginUser = (string)Session["LoginSAPID"];
            try
            {
                Employer.Employer employer = new Employer.Employer();
                DataTable dt = employer.FetchViews(userkey, uid);
                dt.TableName = "AdminViews";
                dt.Columns.ToString();


                var adminViews = (from DataRow dr in dt.Rows
                                  select new AdminViews()
                                  {
                                      ID = dr["MenuID"].ToString(),
                                      MenuName = dr["MenuName"].ToString(),
                                      ControllerName = dr["Controller"].ToString(),
                                      ActionName = dr["Action"].ToString(),
                                  }).ToList();

                ViewData.Model = adminViews;

                ViewBag.ParentId = adminViews;

                return View();
            }
            catch (Exception ex)
            {
                LogError logerror = new LogError();
                logerror.ErrorLog("", LoginUser, "", "AdminViews/EditRole", "AdminViews", "EditRole", "FetchViews Error", ex.Message.ToString(), 0);
                TempData["error"] = ex.Message.ToString();
                ViewBag.Error = TempData["error"];
                return View();
            }
        }

        [Authorize]
        public ActionResult RoleDetails(string roleId)
        {
            string userkey = ConfigurationManager.AppSettings["userkey"];
            string uid = ConfigurationManager.AppSettings["uid"];
            string LoginUser = (string)Session["LoginSAPID"];

            try
            {
                Employer.Employer employer = new Employer.Employer();
                DataTable dt = employer.FetchRoleView(roleId, userkey, uid);
                dt.TableName = "FetchRolesView";
                dt.Columns.ToString();

                List<AdminViews> adminViews = (from DataRow dr in dt.Rows
                                               select new AdminViews()
                                               {
                                                   ID = dr["ID"].ToString(),
                                                   MenuID = dr["MenuID"].ToString(),
                                                   ParentMenuID = dr["ParentMenuID"].ToString(),
                                                   ParentMenuName = dr["parentmenyname"].ToString(),
                                                   MenuName = dr["MenuName"].ToString(),

                                               }).ToList();
                ViewData.Model = adminViews;
                return View();
            }
            catch (Exception ex)
            {
                LogError logerror = new LogError();
                logerror.ErrorLog("", LoginUser, "", "AdminViews/RoleDetails", "AdminViews", "RoleDetails", "FetchRoleView Error", ex.Message.ToString(), 0);
                TempData["error"] = ex.Message.ToString();
                ViewBag.Error = TempData["error"];
                return View();
            }
        }

        public ActionResult UnMapView(string viewId)
        {
            string userkey = ConfigurationManager.AppSettings["userkey"];
            string uid = ConfigurationManager.AppSettings["uid"];
            string LoginUser = (string)Session["LoginSAPID"];

            try
            {
                Employer.Employer employer = new Employer.Employer();
                var unMap = employer.UnMapRoleView(viewId, LoginUser, userkey, uid);
                var unmapStatus = unMap.Split('~');

                if (unmapStatus[0] != "01")
                {
                    TempData["error"] = unmapStatus[1];
                    ViewBag.Error = TempData["error"];
                    return RedirectToAction("MapViewsToRole");
                }
                else
                {
                    TempData["error"] = unmapStatus[1];
                    ViewBag.Error = TempData["error"];
                    return RedirectToAction("MapViewsToRole");
                }
            }
            catch (Exception ex)
            {
                LogError logerror = new LogError();
                logerror.ErrorLog("", LoginUser, "", "AdminViews/UnMapView", "AdminViews", "UnMapView", "UnMapRoleView Error", ex.Message.ToString(), 0);
                TempData["error"] = ex.Message.ToString();
                ViewBag.Error = TempData["error"];
                return View(); ;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MapRole(string menuId, string parentId, string roleId)
        {
            string userkey = ConfigurationManager.AppSettings["userkey"];
            string uid = ConfigurationManager.AppSettings["uid"];
            string LoginUser = (string)Session["LoginSAPID"];

            try
            {

                Employer.Employer employer = new Employer.Employer();
                var  mapResult = employer.MapRoleView(menuId, parentId, roleId, "INP211", userkey, uid);

                var mapDetails = mapResult.Split('~');

                if (mapDetails[0] != "01")
                {
                    TempData["msg"] = mapDetails[1];
                    return RedirectToAction("EditRole");
                }
                else
                {
                    TempData["error"] = mapDetails[1];
                    return RedirectToAction("EditRole");
                }
            }
            catch (Exception ex)
            {
                LogError logerror = new LogError();
                logerror.ErrorLog("", LoginUser, "", "AdminViews/UnMapView", "AdminViews", "UnMapView", "UnMapRoleView Error", ex.Message.ToString(), 0);
                TempData["error"] = ex.Message.ToString();
                ViewBag.Error = TempData["error"];
                return RedirectToAction("EditRole");
            }
        }

        public JsonResult GetAllViews()
        {
            string userkey = ConfigurationManager.AppSettings["userkey"];
            string uid = ConfigurationManager.AppSettings["uid"];
            string LoginUser = (string)Session["LoginSAPID"];

            Employer.Employer employer = new Employer.Employer();
            DataTable dt = employer.FetchViews(userkey, uid);
            dt.TableName = "AdminViews";
            dt.Columns.ToString();

            var adminViews = (from DataRow dr in dt.Rows
                          select new AdminViews()
                          {
                              ID = dr["MenuID"].ToString(),
                              MenuName = dr["MenuName"].ToString(),
                              ControllerName = dr["Controller"].ToString(),
                              ActionName = dr["Action"].ToString(),
                          }).ToList();

            return Json(new { data = adminViews }, JsonRequestBehavior.AllowGet);
        }

        
    }
}