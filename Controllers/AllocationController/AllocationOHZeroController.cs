using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json.Linq;
using SNRWMSPortal.Common;
using SNRWMSPortal.DataAccess;
using SNRWMSPortal.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Security.Cryptography.Pkcs;
using System.Web;
using System.Web.Helpers;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.UI;

namespace SNRWMSPortal.Controllers
{
    [AuthorizeRoles(Role.Allocation, Role.SystemAdministrator)]
    public class AllocationOHZeroController : Controller
    {

        SQLQueryAllocationReport queryskus = new SQLQueryAllocationReport();
        public ActionResult Index()
        {
            try
            { 
            ModelState.Clear();
            AllocationSKUModel model = new AllocationSKUModel();
            model.ClubSKU = queryskus.ZeroOnHand();
          

            return View(model);
            }

            catch (Exception ex)
            {
                return Json(ex, JsonRequestBehavior.AllowGet);
            }
        }

    }
}