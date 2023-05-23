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
    public class AllocationRequestController : Controller
    {

        SQLQueryAllocationRequest queryskus = new SQLQueryAllocationRequest();
        public ActionResult Index()
        {
            try
            { 
            var reasons = queryskus.GetReason();
            var reasonlist = new List<SelectListItem>();
                //var configs = queryskus.GetDConfig();
                //var configlist = new List<SelectListItem>();
                //foreach (var i in configs)
                //{
                //    configlist.Add(new SelectListItem() { Text = i.Dconfig, Value = i.Code.ToString() });
                //}
                //ViewBag.Config = configlist;
                foreach (var i in reasons)
            {
                reasonlist.Add(new SelectListItem() { Text = i.Reasons, Value = i.Reason.ToString() });
            }
            ViewBag.Reason = reasonlist;
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
                var onhanddcm = queryskus.SKUOnHandDCM(id);
                var onhnaddcl = queryskus.SKUOnHandDCL(id);
                var onhanddcp = queryskus.SKUOnHandDCP(id);
                var onhanddcb = queryskus.SKUOnHandDCB(id);
                // var clubs = queryskus.GetClub();
                // result = queryskus.SelectSKUDistribution(id);

                AllocationMerchandiseModel skus = new AllocationMerchandiseModel()
                {
                    SKU = id,
                    Reason = (int)result.Reason,
                    
                    Description = result.Description,
                    DConfigName = result.DConfigName,
                    DCMOH = (int)onhanddcm.DCMOH,
                    DCLOH = (int)onhnaddcl.DCLOH,
                    DCPOH = (int)onhanddcp.DCPOH,
                    DCBOH = (int)onhanddcb.DCBOH,
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
        public ActionResult InsertMerchandise(List<AllocationMerchandiseModel> allocationOverModels)
        {
          //  string userName = "Junes";
            string userName = Session["Username"].ToString();
            DateTime date = DateTime.Now;
            string todaysDate = date.ToString("MM-dd-yyyy,H:mm");
            int skucode = 0;
            int clubcode = 0;
            int units,reason;
            string description;
            int prio = 6;
            string dconfig,reasonname;
            double dcmoh;
            int totals = 0;

            AllocationMerchandiseModel skumodel = new AllocationMerchandiseModel();
            try
            {

                var deptModel = allocationOverModels.Select(x => x.SKU).FirstOrDefault();
                var clubCodeModel = allocationOverModels.Select(x => x.ClubCode).FirstOrDefault();
                


                foreach (var item in allocationOverModels)
                {
                    
                    units = (int)item.Inputs;
                    skucode = (int)item.SKU;
                    clubcode = (int)item.ClubCode;
                    reason = (int)item.Reason;
                    description = item.Description;
                    dconfig = item.DConfigName;
                    reasonname = item.ReasonName;
                    dcmoh = (double)item.DCMOH;
                    bool insertDeptbool = queryskus.VerifyMerchan(skucode,clubcode);
                    bool insertDeptbool1 = queryskus.VerifyMerchanAllocation(skucode, clubcode,prio);

                    int Curr = queryskus.CurrentQtyAllocation(skucode, clubcode, prio);

                    totals = Curr + units;

                    if (insertDeptbool == true)
                    {

                        queryskus.UpdateMerchan(skucode,clubcode,totals,reason,"ALLOCATION",userName,todaysDate,dconfig,description);
                        
                    }

                    else
                    {
                        
                        queryskus.InsertMerchan(skucode,clubcode,totals,reason,"ALLOCATION",userName,todaysDate,dconfig,description);
                        
                    }
                    if (insertDeptbool1 == true)
                    {

                        

                        

                        queryskus.UpdateMerchanAllocation(skucode, clubcode, totals,reasonname, 6,dcmoh, todaysDate);
                        queryskus.DeleteMerchanAllocation(skucode, clubcode, 6);
                    }

                    else
                    {

                        if (units != 0)
                        {
                            queryskus.InsertMerchanAllocation(skucode, clubcode, totals,reasonname, 6,dcmoh, todaysDate,dconfig);
                        }
                        
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