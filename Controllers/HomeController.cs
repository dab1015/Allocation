using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Claims;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using SNRWMSPortal.Common;
using SNRWMSPortal.Controllers;
using SNRWMSPortal.Models;

namespace EOM.Controllers
{

    public class HomeController : Controller
    {
        [AuthorizeRoles(Role.Allocation, Role.SystemAdministrator)]
        public ActionResult Index()
        {

            SNRWMSPortal.Common.SystemApplicationUser.Init.GetAppUser(this.ControllerContext);

            var cookieUsername = SNRWMSPortal.Common.SystemApplicationUser.Init.Username;
            var cookieAuth = SNRWMSPortal.Common.SystemApplicationUser.Init.isAuthenticatedLogin;
            var cookieisReset = SNRWMSPortal.Common.SystemApplicationUser.Init.IsReset;
            var cookieisRole = SNRWMSPortal.Common.SystemApplicationUser.Init.AccessRole;

            //if (cookieisRole == null || Request.Cookies["SNRPortalCookie"] == null)
            //{
            //    Session["NotLoggedIn"] = "You are not logged in";
            //    return RedirectToAction("Index", "Account");
            //}


            ClaimsIdentity claimsIdentity = User.Identity as ClaimsIdentity;
            if (!claimsIdentity.Claims.Any())
            {
                if (cookieAuth == null)
                {
                    cookieAuth = "0";
                }
                else
                {
                    cookieAuth = "1";
                }
                var claimsCookie = new List<Claim>();

                string[] accessProfile = cookieisRole.Split(',');
                int lengthOfProfiles = accessProfile.Length;

                claimsCookie.Add(new Claim(ClaimTypes.Name, cookieUsername));
                claimsCookie.Add(new Claim("isAuthenticated", cookieAuth));
                claimsCookie.Add(new Claim("IsReset", cookieisReset.ToString()));


                for (int i = 0; i < lengthOfProfiles; i++)
                {
                    claimsCookie.Add(new Claim(ClaimTypes.Role, accessProfile[i]));
                }


                var identity = new ClaimsIdentity(claimsCookie, DefaultAuthenticationTypes.ApplicationCookie);
                var ctx = Request.GetOwinContext();

                var authenticationManager = ctx.Authentication;

                authenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = true }, identity);
            }


            //IEnumerable<Claim> claims = claimsIdentity.Claims;


            //ViewBag.Username = claimsIdentity.Name;
            ViewBag.Username = cookieUsername;
            //ViewData["User"] = claimsIdentity.Name;
            Session["Username"] = cookieUsername;
            int isUserforReset = cookieisReset;
            if (cookieUsername != null)
            {
                //var identity = User.Identity as ClaimsIdentity;
                //ViewBag.IdentityUser = identity;
                //ViewBag.ClaimsIdentity = Thread.CurrentPrincipal.Identity;
                //var claimsRolevalue = identity.FindAll(ClaimTypes.Role).Select(x => x.Value);
                //var peopleProfile = identity.FindFirst(ClaimTypes.Role)?.Value;
                //string claimsRolevalueToString = "";
                //claimsRolevalueToString = string.Join<string>(",", claimsRolevalue);
                var model = new UserInfosDetail()
                {
                    Username = cookieUsername,
                    userProfile = cookieisRole
                };
                //ViewBag.ClaimsIdentity = Thread.CurrentPrincipal.Identity;
                return View(model);

            }
            else
            {
                ViewBag.NotLoggedIn = "You are not logged in.";
                AccountController account = new AccountController();
                return account.Logout();
                //string message = "You need to login here then reset your password first.";
                //Session["ForReset"] = message;
                //ViewBag.ClaimsIdentity = Thread.CurrentPrincipal.Identity;
                //return RedirectToAction("Index", "Account");
            }


        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult OutboundPicking()
        {
            

            return View();
        }

        public ActionResult ClubRequest()
        {


            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        //public ActionResult removeClaims()
        //{
        //    if (User.Identity.IsAuthenticated)
        //    {
        //        string tru = "true";
        //    }
        //    HttpContext.GetOwinContext().Authentication.SignOut();

        //    if (User.Identity.IsAuthenticated)
        //    {
        //        string fa = "false";
        //    }
        //    return RedirectToAction("Index", "Account");
        //}


        //public JsonResult _GetStaggingCodes()
        //{
        //    SNRWMSPortal.DataAccess.SQLConnection SQL = new SNRWMSPortal.DataAccess.SQLConnection();

        //    var result = SQL.GetStaggingCodes();

        //    return Json(result,JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult _ShowStaggingData(string StagingCode)
        //{
        //    SNRWMSPortal.DataAccess.SQLConnection SQL = new SNRWMSPortal.DataAccess.SQLConnection();
        //    var result = SQL.ShowStaggingData(StagingCode);

        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}








    }
}