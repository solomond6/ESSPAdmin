using ESPAdmin.Models;
using ESPAdmin.Utility;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace ESPAdmin.Controllers
{

    public class LoginController : Controller
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
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(Users users, string returnUrl)
        {
            try
            {
                string userkey = ConfigurationManager.AppSettings["userkey"];
                string uid = ConfigurationManager.AppSettings["uid"];

                string ipaddress = Request.UserHostAddress;
                string BrowserUsed = Request.UserAgent;

                Employer.Employer employer = new Employer.Employer();
                var LoginStatus = employer.LOGIN(users.Username, users.Password, BrowserUsed, ipaddress, "", userkey, uid);

                var LoginDetails = LoginStatus.Split('~');

                if (LoginDetails[0] != "01")
                {
                    DataTable dt = employer.FetchRoleView(LoginDetails[3], userkey, uid);
                    dt.TableName = "FetchRolesView";
                    dt.Columns.ToString();

                    List<AdminViews> AdminViews = (from DataRow dr in dt.Rows
                                                    select new AdminViews()
                                                    {
                                                        ID = dr["ID"].ToString(),
                                                        MenuID = dr["MenuID"].ToString(),
                                                        ParentMenuID = dr["ParentMenuID"].ToString(),
                                                        ParentMenuName = dr["parentmenyname"].ToString(),
                                                        MenuName = dr["MenuName"].ToString(),
                                                        ControllerName = dr["Controller"].ToString(),
                                                        ActionName = dr["Action"].ToString()
                                                    }).ToList();

                    var menuView = AdminViews.Cast<AdminViews>().GroupBy(tm => tm.ParentMenuID).Select(group => new { ID = group.Key, Items = group.ToArray() }).ToArray();
                    Session["menuView"] = AdminViews;

                    FormsAuthentication.SetAuthCookie(LoginDetails[1], false);
                    Session["LoginSAPID"] = users.Username;
                    Session["RoleID"] = LoginDetails[3];
                    Session["WebUserID"] = LoginDetails[4];
                    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/") && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    {
                        if (LoginDetails[3] == "1" || LoginDetails[3] == "2")
                        {
                            return Redirect(returnUrl);
                        }
                        else
                        {
                            return RedirectToAction("MyRequest", "Requests");
                        }
                    }
                    else
                    {
                        if (LoginDetails[3] == "1" || LoginDetails[3] == "2")
                        {
                            return RedirectToAction("Index", "Employers");
                        }
                        else
                        {
                            return RedirectToAction("MyRequest", "Requests");
                        }
                    }
                }
                else
                {
                    TempData["error"] = LoginDetails[1];
                    ViewBag.Error = TempData["error"];
                    return View();
                }
            }
            catch(Exception ex)
            {
                LogError logerror = new LogError();
                logerror.ErrorLog(users.Username, "2", "3", "4", "Login", "Index", "Login Error", ex.Message.ToString(), 0);
                TempData["error"] = ex.Message.ToString();
                ViewBag.Error = TempData["error"];
                return View();
            }
        }

        [Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            return RedirectToAction("Index", "Login");
        }

        [Authorize]
        public JsonResult GetMenuItems()
        {

            try
            {
                var menuItems = Session["menuView"];
                return Json(new { data = menuItems }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //LogError logerror = new LogError();
                //logerror.ErrorLog("", LoginUser, "", "Employer/GetCompanyEmployee", "Employer", "GetCompanyEmployee", "GetCompanyEmployee Error", ex.Message.ToString(), 0);
                throw new Exception(ex.Message.ToString());
            }
        }
    }
}