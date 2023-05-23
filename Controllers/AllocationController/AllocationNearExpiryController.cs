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
    public class AllocationNearExpiryController : Controller
    {

        SQLQueryAllocationNearExpiry queryskus = new SQLQueryAllocationNearExpiry();
        public ActionResult Index()
        {
            try
            { 
            
            return View();
              }

            catch (Exception ex)
            {
                return Json(ex, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpGet]
        public ActionResult SearchSKU(long id)
        {
            var verify = queryskus.CheckSKUId(id);
            if (verify != 0)
            {
                var result = queryskus.SelectSKU(id);
                var listclub = queryskus.GetClubSKU(id);
               

                AllocationNearExpiryModel skus = new AllocationNearExpiryModel()
                {
                    SKU = id,
                    Reason = (int)result.Reason,
                    DConfig = (int)result.DConfig,
                    Description = result.Description,
                    


                    MerchandiseSKU = listclub
                   



                };
                ViewBag.Description = skus;
                return Json(skus, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = $"SKU Not Found" }, JsonRequestBehavior.AllowGet);
            }

           
        }


        [HttpPost]
        public ActionResult InsertNearExpiry(List<AllocationNearExpiryModel> allocationOverModels)
        {

           // string userName = Session["Username"].ToString();
           // DateTime date = DateTime.Now;
           // string todaysDate = date.ToString("MM-dd-yyyy,H:mm");
            int skucode = 0;
            int clubcode = 0;
            int one,two,three;
            

            AllocationNearExpiryModel skumodel = new AllocationNearExpiryModel();
            try
            {

                var deptModel = allocationOverModels.Select(x => x.SKU).FirstOrDefault();
                var clubCodeModel = allocationOverModels.Select(x => x.ClubCode).FirstOrDefault();
                


                foreach (var item in allocationOverModels)
                {

                    clubcode = (int)item.ClubCode;
                    one = (int)item.OneToThirty;
                    two = (int)item.ThirtyoneToSixty;
                    three = (int)item.SixtyoneAbove;
                    skucode = (int)item.SKU;
                   
                    
                    
                   

                    bool insertDeptbool = queryskus.VerifyNearExpiry(skucode,clubcode);
                   


                    if (insertDeptbool == true)
                    {

                        queryskus.UpdateNearExpiry(clubcode,one,two,three,skucode);
                        
                    }

                    else
                    {
                        
                        queryskus.InsertNearExpiry(clubcode, one, two, three, skucode);
                        
                    }
                    

                }
                return Json(new { success = true, message = "Successfully Saved!" }, JsonRequestBehavior.AllowGet);

            }

            catch (Exception)
            {
                return Json(new { success = false, message = $"Please try again empty data cannot be save!" }, JsonRequestBehavior.AllowGet);
            }
        }




    }
}