using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using System.Web.Security;

namespace SNRWMSPortal.Common
{
    public class AuthorizeRolesAttribute : AuthorizeAttribute
    {
        public AuthorizeRolesAttribute(params string[] roles) : base()
        {
            Roles = string.Join(",", roles);
        }
    }


    public static class Role
    {
        public const string PickingSupervisor = "Picking Supervisor";
        public const string LetdownSupervisor = "Letdown Supervisor";
        public const string Auditor = "Auditor";
        public const string LossPrevention = "Loss Prevention";
        public const string CycleCountManager = "Cycle Count Manager";
        public const string SystemAdministrator = "System Administrator";
        public const string Allocation = "Allocation";
        public const string Fulfillment = "Fulfillment";

    }

    
}