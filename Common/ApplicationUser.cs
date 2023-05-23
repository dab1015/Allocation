using Newtonsoft.Json;
using SNRWMSPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SNRWMSPortal.Common
{
    public sealed class SystemApplicationUser
    {
        public Models.ApplicationUser ApplicationUserIdentity { get; set; }

        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string AccessRole { get; set; }
        public string isAuthenticatedLogin { get; set; }

        public int IsReset { get; set; }    
        public static SystemApplicationUser Init { get; } = new SystemApplicationUser();
        private HttpCookie appCookie;
        private ControllerContext controllerContext;
        
        private static readonly string ApplicationUserCookie = "SNRPortalCookie";

        public SystemApplicationUser GetAppUser(string username, ControllerContext controllerContext, string userFirstName, string userLastName, string userProfile, int isUserforReset,string isAuthenticated)
        {
            this.controllerContext = controllerContext;
            if (!controllerContext.HttpContext.Request.Cookies.AllKeys.Contains(ApplicationUserCookie))
            {
                //var dbContext = new WMS_OutboundEntities();
                //var user = dbContext.Users.FirstOrDefault(f => f.Username == username);
                Username = username;
                FirstName = userFirstName;
                LastName = userLastName;
                AccessRole = userProfile;
                IsReset = isUserforReset;
                isAuthenticatedLogin = isAuthenticated;
                SetApplicationCookie(1);
            }

            return GetApplicationUserCookie();
        }

        private void SetApplicationCookie(int expiry)
        {
            if (expiry > 0)
            {
                appCookie = new HttpCookie(ApplicationUserCookie);
                var appusr = JsonConvert.SerializeObject(Init);
                appCookie.Value = appusr;
                appCookie.Path = "/";
            }
            else
            {
                appCookie = new HttpCookie(ApplicationUserCookie);
                var appusr = JsonConvert.SerializeObject(Init);
                appCookie.Value = appusr;
                appCookie.Expires = DateTime.Now.AddDays(expiry);
            }

            controllerContext.HttpContext.Response.Cookies.Add(appCookie);
        }

        private SystemApplicationUser GetApplicationUserCookie()
        {
            if (controllerContext.HttpContext.Request.Cookies[ApplicationUserCookie] == null)
                return null;

            var cookie = controllerContext.HttpContext.Request.Cookies[ApplicationUserCookie].Value;
            var appCookie = JsonConvert.DeserializeObject<Common.SystemApplicationUser>(cookie);
            Init.Username = appCookie.Username;
            Init.FullName = appCookie.FullName;
            Init.AccessRole = appCookie.AccessRole;
            return Init;
        }

        public SystemApplicationUser GetAppUser(ControllerContext controllerContext)
        {
            this.controllerContext = controllerContext;
            return GetApplicationUserCookie();
        }



        private void ClearCookie(ControllerContext controllerContext)
        {
            this.controllerContext = controllerContext;
            if (controllerContext.HttpContext.Request.Cookies.AllKeys.Contains(ApplicationUserCookie))

                SetApplicationCookie(-1);
        }

        public void ClearAppUser(ControllerContext controllerContext)
        {
            ClearCookie(controllerContext);
            Init.Username = string.Empty;
            Init.FullName = string.Empty;
            Init.AccessRole = "";
            Init.ApplicationUserIdentity = null;
        }
    }
    
}