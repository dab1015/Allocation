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
using System.Web.UI.WebControls;

namespace SNRWMSPortal.Controllers
{
    [AuthorizeRoles(Role.Allocation, Role.SystemAdministrator)]
    public class AllocationClubAppController : Controller
    {

        SQLQueryAllocationClubReq queryskus = new SQLQueryAllocationClubReq();
        public ActionResult Index()
        {
            
            try
            { 
            var clubs = queryskus.GetClubSQL();
            var clublist = new List<SelectListItem>();
            foreach (var i in clubs)
            {
                clublist.Add(new SelectListItem() { Text = i.ClubName, Value = i.CLubCode.ToString() });
            }
                //var configs = queryskus.GetDConfig();
                //var configlist = new List<SelectListItem>();
                //foreach (var i in configs)
                //{
                //    configlist.Add(new SelectListItem() { Text = i.Dconfig, Value = i.Code.ToString() });
                //}
                //ViewBag.Config = configlist;
                ViewBag.ClubRequest = clublist;

                var reasons = queryskus.GetReason();
                var reasonlist = new List<SelectListItem>();
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
        public ActionResult SearchClubReq(int clubcode,string status)
        {

            var result = queryskus.GetClubRequest(clubcode,status);
            


            AllocationClub skus = new AllocationClub()
            {
                    
                    Clubs = result
                    
                   

                };
                ViewBag.Description = skus;
            return new JsonResult() { Data = skus, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
        }

        [HttpGet]
        public ActionResult SearchSKU(long skuid,int clubcode)
        {
            var verify = queryskus.CheckSKUId(skuid);
            if (verify != 0)
            {
                var result = queryskus.SKUDes(skuid,clubcode);
                var result1 = queryskus.SKUConfig(skuid,clubcode);
                var result2 = queryskus.SKUClubQunatity(skuid,clubcode);
                var result3 = queryskus.SKUWS(skuid, clubcode);
                var result4 = queryskus.SKUWS_Ave(skuid, clubcode);
                var dcmoh = queryskus.SKUOnHandDCM(skuid);

                AllocationClub skus = new AllocationClub()
                {
                    SKU = skuid,
                    Description = result.Description,
                    ISTDPK = (double)result.ISTDPK,
                    IVPLHI = (int)result.IVPLHI,
                    IVPLTI = (int)result.IVPLTI,
                    Pallet = (int)result.Pallet,
                    Layer = (int)result.Layer, 
                    PConfig = result.PConfig,
                    InTransit = (double)result3.InTransit,
                    AveSales = (double)result4.AveSales,
                    DConfigName = result1.DConfigName,
                    StoreOH = (double)result.StoreOH,
                    Reason = (int)result2.Reason,
                    ReasonName = result2.ReasonName,
                    Status = result2.Status,
                    Quantity = (int)result2.Quantity,
                    RequestedDate = result2.RequestedDate,
                    Remarks = result2.Remarks,
                    DCMOH = dcmoh.DCMOH
                    
                };
                ViewBag.Description = skus;
                return new JsonResult() { Data = skus, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
            }
            else
            {
                return Json(new { success = false, message = $"SKU Not Found" }, JsonRequestBehavior.AllowGet);
            }


        }
        [HttpGet]
        public JsonResult EditSKU(int id)
        {

            var equipment = queryskus.GetSKU(id).Where(w=>w.SKU == id).FirstOrDefault();
            
            return Json(equipment, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult InsertRequest(List<AllocationClub> allocationClubModels)
        {
           // string userName = "Junes";
            string userName = Session["Username"].ToString();
            int skucode = 0;
            int clubcode = 0;
            int reason, quantity,pqty;
            
            string description,requesteddate,remarks,pconfig, dconfig;


            AllocationClub skumodel = new AllocationClub();
            try
            {

                var deptModel = allocationClubModels.Select(x => x.SKU).FirstOrDefault();
                var clubCodeModel = allocationClubModels.Select(x => x.CLubCode).FirstOrDefault();


                foreach (var item in allocationClubModels)
                {

                    clubcode = (int)item.CLubCode;
                    skucode = (int)item.SKU;
                    description = item.Description;
                    reason = (int)item.Reason;
                    
                    quantity = (int)item.Quantity;
                    requesteddate = item.RequestedDate;
                    remarks = item.Remarks;
                    pqty = (int)item.PQty;
                    pconfig = item.PConfig;
                    dconfig = item.DConfigName;
                   

                    bool insertDeptbool = queryskus.VerifyInsert(skucode,clubcode);
                   
                    
                    

                    if (insertDeptbool == true)
                    {
                        if(remarks == null)
                        {
                            remarks = "";
                        }

                        queryskus.UpdateRequest(skucode,clubcode,description,reason,quantity,pqty,requesteddate,remarks,pconfig, userName,dconfig);


                    }

                    else
                    {
                        if (remarks == null)
                        {
                            remarks = "";
                        }
                        queryskus.InsertRequest(skucode,clubcode,description, reason, "PENDING", quantity,pqty,requesteddate,remarks,pconfig, userName,dconfig);
                    }

                }
                return Json(new { success = true, message = "Successfully Saved!" }, JsonRequestBehavior.AllowGet);

            }

            catch (Exception)
            {
                return Json(new { success = false, message = $"Please try again empty data cannot be save!" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult ApprovedRequest(List<AllocationClub> allocationClubModels)
        {

            DateTime date = DateTime.Now;
            string todaysDate = date.ToString("MM-dd-yyyy,H:mm");
            int skucode = 0;
            int clubcode = 0;
            string approveddate;
            int quantity = 0;
            string dconfig,reasonname;
            double dcmoh;
            int prio = 5;
            int totals = 0;
            AllocationClub skumodel = new AllocationClub();
            try
            {

                var deptModel = allocationClubModels.Select(x => x.SKU).FirstOrDefault();
                var clubCodeModel = allocationClubModels.Select(x => x.CLubCode).FirstOrDefault();


                foreach (var item in allocationClubModels)
                {

                    clubcode = (int)item.CLubCode;
                    skucode = (int)item.SKU;
                    approveddate = item.ApprovedDate;
                    quantity = (int)item.Quantity;
                    dconfig = item.DConfigName;
                    reasonname = item.ReasonName;
                    dcmoh = (double)item.DCMOH;
                    bool insertDeptbool = queryskus.VerifyInsert(skucode, clubcode);

                    bool insertAllocbool = queryskus.VerifyInsertAllocation(skucode, clubcode);
                    int Curr = queryskus.CurrentQtyAllocation(skucode, clubcode, prio);


                    totals = Curr + quantity;
                    if (insertDeptbool == true)
                    {
                        
                        queryskus.UpdateApprovedRequest(skucode, clubcode, "APPROVED",  approveddate);
                       // queryskus.InsertAllocation(skucode, clubcode, quantity, 5, dconfig, reasonname, dcmoh, todaysDate);

                    }
                    if(insertAllocbool == true)
                    {
                        queryskus.UpdateApprovedAllocation(skucode, clubcode, totals, reasonname, todaysDate);
                    }
                    else if(insertAllocbool == false)
                    {
                        queryskus.InsertAllocation(skucode, clubcode, totals, 5,5, dconfig, reasonname, dcmoh, todaysDate);
                    }

                   

                }
                return Json(new { success = true, message = "Successfully Approved!" }, JsonRequestBehavior.AllowGet);

            }

            catch (Exception)
            {
                return Json(new { success = false, message = $"Please try again empty data cannot be save!" }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public ActionResult DisApprovedRequest(List<AllocationClub> allocationClubModels)
        {


            int skucode = 0;
            int clubcode = 0;
           




            AllocationClub skumodel = new AllocationClub();
            try
            {

                var deptModel = allocationClubModels.Select(x => x.SKU).FirstOrDefault();
                var clubCodeModel = allocationClubModels.Select(x => x.CLubCode).FirstOrDefault();


                foreach (var item in allocationClubModels)
                {

                    clubcode = (int)item.CLubCode;
                    skucode = (int)item.SKU;
                   



                    bool insertDeptbool = queryskus.VerifyInsert(skucode, clubcode);




                    if (insertDeptbool == true)
                    {

                        queryskus.UpdateDisApprovedRequest(skucode, clubcode, "DISAPPROVED");


                    }



                }
                return Json(new { success = true, message = "Successfully DISAPPROVED!" }, JsonRequestBehavior.AllowGet);

            }

            catch (Exception)
            {
                return Json(new { success = false, message = $"Please try again empty data cannot be save!" }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}