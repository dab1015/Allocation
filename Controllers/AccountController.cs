using SNRWMSPortal.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Web.Helpers;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Reflection;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using SNRWMSPortal.Common;
using System.Data.Entity.Infrastructure;
using Microsoft.AspNet.Identity.Owin;
using System.Net;

namespace SNRWMSPortal.Controllers
{
    public class AccountController : Controller
    {
        public string saltiness = ConfigurationManager.AppSettings["Asin"];
        public string nitertaions = ConfigurationManager.AppSettings["NIterations"];

        // GET: Account

        [AllowAnonymous]
        public ActionResult Index()

        {
            if (Session["NotLoggedIn"] != null)
            {
                string message = Session["NotLoggedIn"].ToString();
                ViewBag.NotLoggedIn = message;
                Session["NotLoggedIn"] = null;

                return View();
            }

            //if (Session["ForReset"] != null)
            //{
            //    ViewBag.ForResetMessage = Session["ForReset"];
            //    Session["ForReset"] = null;
            //    return View();
            //}

            if (Session["IsUpdated"] != null)
            {
                Session["ForReset"] = null;
                ViewBag.SuccessResetPass = "Successfully Updated Password";
                Session["IsUpdated"] = null;
                return View();
            }

            else if (User.Identity.IsAuthenticated || Request.Cookies["SNRPortalCookie"] != null)
            {

                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View();
            }







        }

        [AuthorizeRoles(Role.PickingSupervisor, Role.LetdownSupervisor, Role.Auditor, Role.LossPrevention, Role.SystemAdministrator)]
        public ActionResult ResetPassword()
        {
            ClaimsIdentity claimsIdentity = User.Identity as ClaimsIdentity;
            if (claimsIdentity != null)
            {
                IEnumerable<Claim> claims = claimsIdentity.Claims;
                ViewBag.ResetStatus = claims.FirstOrDefault(x => x.Type == "IsReset").Value;
                ViewBag.FirstName = claimsIdentity.Name;

                if (ViewBag.ResetStatus == "0")
                {
                    return RedirectToAction("Index", "Home");
                }
                return View();

            }
            else
            {
                return RedirectToAction("Index", "Account");
            }


        }

        public ActionResult ResetUserPassword(ResetPasswordModel resetPasswordModel)
        {
            ClaimsIdentity claimsIdentity = User.Identity as ClaimsIdentity;
            IEnumerable<Claim> claims = claimsIdentity.Claims;

            string resetUser = claimsIdentity.Name;

            int numberofSalt = Convert.ToInt32(saltiness);
            int numberofiterations = Convert.ToInt32(nitertaions);

            if (ModelState.IsValid)
            {

                DataAccess.SQLConnection sqlCon = new DataAccess.SQLConnection();

                var checkifCurrentPasswordisCorrect = sqlCon.VerifyUserTempPassword(resetUser, resetPasswordModel.currentPassword);

                if (checkifCurrentPasswordisCorrect)
                {
                    string newsalt = SecurityHelper.GenerateSalt(numberofSalt);
                    string oldpwdHashed = SecurityHelper.HashPassword(resetPasswordModel.currentPassword, newsalt, numberofiterations, numberofSalt);
                    string newpwdHashed = SecurityHelper.HashPassword(resetPasswordModel.newPassword, newsalt, numberofiterations, numberofSalt);

                    if (oldpwdHashed == newpwdHashed)
                    {
                        ViewBag.SamewithCurrentPass = "New Password must not be the same with the Current Password.";
                        return View("ResetPassword");
                    }

                    sqlCon.ResetUserPassword(resetUser, newsalt, newpwdHashed);

                    Session["IsUpdated"] = "1";

                    return RedirectToAction("Index", "Account");

                }
                else
                {
                    ViewBag.ErrorCurrentPass = "Current Password is not correct, kindly input the password provided by IT.";
                    return View("ResetPassword");
                }
            }
            else
            {
                ViewBag.error = "Model is invalid";
                return View("ResetPassword");
            }
        }


        //public ActionResult Login(LoginModels loginModels, string returnUrl)
        //{

        //    if (!string.IsNullOrWhiteSpace(returnUrl))
        //    {
        //        return Redirect(returnUrl);
        //    }

        //    string userFirstName = "";
        //    string userLastName = "";
        //    string userProfile = "";
        //    string isAuthenticated = "";
        //    string isProfileAdmin = "";

        //    if (ModelState.IsValid)
        //    {
        //        DataAccess.SQLConnection sqlCon = new DataAccess.SQLConnection();
        //        //var hash = Crypto.SHA256(loginModels.Password);
        //        var result = sqlCon.VerifyUser(loginModels.Username, loginModels.Password);
        //        IEnumerable<UserInfosDetail> userinfos = sqlCon.getUserInfos(loginModels.Username);


        //        foreach (UserInfosDetail userinfo in userinfos)
        //        {
        //            userFirstName = userinfo.userFirstName;
        //            userLastName = userinfo.userLastName;
        //            userProfile = userinfo.userProfile;
        //            isProfileAdmin = userinfo.isProfileAdmin;
        //        }
        //        int isUserforReset = sqlCon.VerifyUserIfReset(loginModels.Username);

        //        if (isProfileAdmin == "8" && isUserforReset == 0 && result == true)
        //        {
        //            isAuthenticated = "1";
        //            var storeUser = SystemApplicationUser.Init.GetAppUser(loginModels.Username, this.ControllerContext, userFirstName, userLastName, userProfile, isUserforReset, isAuthenticated);

        //            this.SignInUser(loginModels.Username, userProfile, isUserforReset, storeUser, false, isAuthenticated);
        //            return RedirectToAction("Index", "Home");
        //            //return RedirectToAction("Index", "RegisterPeople");

        //        }
        //        else if (result == true && isUserforReset == 1)
        //        {
        //            isAuthenticated = "1";
        //            var storeUser = SystemApplicationUser.Init.GetAppUser(loginModels.Username, this.ControllerContext, userFirstName, userLastName, userProfile, isUserforReset, isAuthenticated);

        //            this.SignInUser(loginModels.Username, userProfile, isUserforReset, storeUser, false, isAuthenticated);
        //            return RedirectToAction("ResetPassword", "Account");

        //        }
        //        else if (result == true && isUserforReset == 0)
        //        {
        //            isAuthenticated = "1";
        //            var storeUser = SystemApplicationUser.Init.GetAppUser(loginModels.Username, this.ControllerContext, userFirstName, userLastName, userProfile, isUserforReset, isAuthenticated);

        //            this.SignInUser(loginModels.Username, userProfile, isUserforReset, storeUser, false, isAuthenticated);
        //            return RedirectToAction("Index", "Home");
        //        }
        //        else
        //        {
        //            ViewBag.error = "Login failed.";
        //        }

        //        return View("Index");


        //    }
        //    else
        //    {
        //        ViewBag.error = "Model is not valid";
        //        return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
        //    }
        //}

        //public void SignInUser(string username, string AccessRole, int isReset, SystemApplicationUser user, bool isPersistent, string isAuthenticated)
        //{
        //    string[] accessProfile = AccessRole.Split(',');
        //    int lengthOfProfiles = accessProfile.Length;
        //    var claims = new List<Claim>();
        //    claims.Add(new Claim(ClaimTypes.Name, username));
        //    claims.Add(new Claim("isAuthenticated", isAuthenticated));
        //    claims.Add(new Claim("IsReset", isReset.ToString()));


        //    for (int i = 0; i < lengthOfProfiles; i++)
        //    {
        //        claims.Add(new Claim(ClaimTypes.Role, accessProfile[i]));
        //    }


        //    var identity = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);


        //    var ctx = Request.GetOwinContext();

        //    var authenticationManager = ctx.Authentication;

        //    authenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = true }, identity);
        //}




        public ActionResult Register(LoginModels loginModels)
        {
            int numberofSalt = Convert.ToInt32(saltiness);
            int numberofiterations = Convert.ToInt32(nitertaions);
            if (ModelState.IsValid)
            {
                DataAccess.SQLConnection sqlCon = new DataAccess.SQLConnection();
                var result = sqlCon.VerifyUserRegister(loginModels.Username);

                if (result)
                {
                    ViewBag.error = "User already exist";
                    return View("Index");
                    //User.Identity.IsAuthenticated
                    //return Json("Success",JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //Crypto.SHA256()
                    //var hash = Crypto.SHA256(loginModels.Password);
                    string salt = SecurityHelper.GenerateSalt(numberofSalt);
                    string pwdHashed = SecurityHelper.HashPassword(loginModels.Password, salt, numberofiterations, numberofSalt);


                    //var hash = Encrypt(loginModels.Password);
                    sqlCon.RegisterUser(loginModels.Username, salt, pwdHashed);

                    Session["UserSessionName"] = loginModels.Username;
                    ViewBag.UserSessionName = loginModels.Username;
                    return RedirectToAction("Index", "Home");

                }

            }
            else
            {
                ViewBag.error = "Username/Password is required.";
                return View("Index");
            }

        }

        public ActionResult Logout()
        {
            HttpContext.GetOwinContext().Authentication.SignOut();
            Common.SystemApplicationUser.Init.ClearAppUser(this.ControllerContext);
            Session.Clear();
            Session.RemoveAll();
            ViewData.Clear();
            if (Request.Cookies["SNRPortalCookie"] != null)
            {
                var c = new HttpCookie("SNRPortalCookie");
                c.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(c);
            }

            return Redirect("http://199.84.200.96:710");


        }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ClaimsIdentity claimsIdentity = User.Identity as ClaimsIdentity;

            if (Request.Cookies["SNRPortalCookie"] != null)
            {
                var cookie = Request.Cookies["SNRPortalCookie"].Value;



                var appCookie = Newtonsoft.Json.JsonConvert.DeserializeObject<SystemApplicationUser>(cookie);

                //default values testing
                //var appCookie = SystemApplicationUser.Init;
                ViewBag.Username = appCookie.Username;
                string[] accessProfile = appCookie.AccessRole.Split(',');
                int lengthOfProfiles = accessProfile.Length;
                if (appCookie.isAuthenticatedLogin == "0" || appCookie.isAuthenticatedLogin == null)
                {
                    HttpContext.GetOwinContext().Authentication.SignOut();
                    Common.SystemApplicationUser.Init.ClearAppUser(this.ControllerContext);
                    Session.Clear();
                    Session.RemoveAll();
                    return Redirect("http://199.84.200.96:710");
                }

                //var storeUser = SystemApplicationUser.Init.GetAppUser(appCookie.Username, this.ControllerContext);

                //this.SignInUserPortal(appCookie.Username, accessProfile, lengthOfProfiles, storeUser, false);

                var claims = new List<Claim>();

                claims.Add(new Claim(ClaimTypes.Name, value: appCookie.Username));
                for (int i = 0; i < lengthOfProfiles; i++)
                {
                    claims.Add(new Claim(ClaimTypes.Role, accessProfile[i]));
                }

                var identity = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);

                AuthenticationProperties authenticationProperties = new AuthenticationProperties()
                {
                    IsPersistent = false
                };

                HttpContext.GetOwinContext().Authentication.SignIn(
                    properties: authenticationProperties,
                    identities: identity);


                return RedirectToAction("Index", "Home");


            }

            return Redirect("http://199.84.200.96:710");

            //if (User.Identity.IsAuthenticated)
            //{

            //    return RedirectToAction("Index", "Home");
            //}
            //ViewBag.ReturnUrl = returnUrl;
            //return View();
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            //Common.SystemApplicationUser.Init.ClearAppUser(this.ControllerContext);
            //AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            //return RedirectToAction("Index", "Home");

            HttpContext.GetOwinContext().Authentication.SignOut();
            Common.SystemApplicationUser.Init.ClearAppUser(this.ControllerContext);
            Session.Clear();
            Session.RemoveAll();
            //AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return Redirect("http://199.84.200.96:710");

        }
    }
}