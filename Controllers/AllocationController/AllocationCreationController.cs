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
 //  [AuthorizeRoles(Role.Allocation, Role.SystemAdministrator)]
    public class AllocationCreationController : Controller
    {

        SQLQueryAllocationMerchandise queryskus = new SQLQueryAllocationMerchandise();
        

        //To select dropdown Clublist
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
                
                ViewBag.ClubRequest = clublist;
                return View();
            }

            catch (Exception ex)
            {
                return Json(ex, JsonRequestBehavior.AllowGet);
            }
        }


       
        //To search Alocated data on selected club,to delete Status = 3 which batchcode is null, to delete already posted in picklist table
        [HttpGet]
        public ActionResult SearchAllocationClubCode(int clubcode)
        {

            queryskus.DeleteStatus3andNullSlotLoc(clubcode);
            queryskus.DeleteAlreadyPosted();
            var res1 = queryskus.GetAllocationClubCode(clubcode);
            


            AllocationMerchandiseModel skus = new AllocationMerchandiseModel()
            {


                MerchandiseSKU = res1
               




            };
            ViewBag.Description = skus;
            
            return new JsonResult() { Data = skus, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };

        }

        //To display the picked clubcode
        [HttpGet]
        public ActionResult SearchAllocationClubCodeToPick(int clubcode)
        {

            
            var res1 = queryskus.GetAllocationClubCodeToPick(clubcode);



            AllocationMerchandiseModel skus = new AllocationMerchandiseModel()
            {


                MerchandiseSKU = res1

            };
            ViewBag.Description = skus;
            return new JsonResult() { Data = skus, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };

        }

        //To display the picked clubcode with slotloc and lpn ready
        [HttpGet]
        public ActionResult SearchAllocationClubCodeToPickReady(int clubcode)
        {

            var res1 = queryskus.GetAllocationClubCodeToPickReady(clubcode);
            AllocationMerchandiseModel skus = new AllocationMerchandiseModel()
            {


                MerchandiseSKU = res1

            };
            ViewBag.Description = skus;
            return new JsonResult() { Data = skus, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };

        }



        //To search Pallet Config in WMS
        [HttpGet]
        public ActionResult SearchAllocationSKU(int clubcode)
        {

            var res = queryskus.GetAllocationSKU(clubcode);


            AllocationMerchandiseModel skus = new AllocationMerchandiseModel()
            {

                MerchandiseWMS = res

            };
            ViewBag.Description = skus;
            return new JsonResult() { Data = skus, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };

        }
        //To search Case config in WMS
        [HttpGet]
        public ActionResult SearchAllocationSKUCase(int clubcode)
        {

            
            var res = queryskus.GetAllocationSKUCase(clubcode);


            AllocationMerchandiseModel skus = new AllocationMerchandiseModel()
            {



                MerchandiseWMSCase = res


            };
            ViewBag.Description = skus;
            return new JsonResult() { Data = skus, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };

        }

        //To insert data into picklist table with batchcode 
        [HttpPost]
        public ActionResult InsertPicklist(List<AllocationMerchandiseModel> allocationOverModels)
        {
          //  string userName = "Junes";
            string userName = Session["Username"].ToString();
            DateTime date = DateTime.Now;
            string todaysDate = date.ToString("MM-dd-yyyy,H:mm");
            int skucode = 0;
            int clubcode = 0;
            int reqty,qtytopick,prio ;
            string description, prioname, dconfig,lpn,slotloc,batchcode,sequence;
            int qtypicked = 0;
            double stdpk;
            int status = 1;


            AllocationMerchandiseModel skumodel = new AllocationMerchandiseModel();
            try
            {

                var deptModel = allocationOverModels.Select(x => x.INUMBR).FirstOrDefault();
                var clubCodeModel = allocationOverModels.Select(x => x.ClubCode).FirstOrDefault();
                var prioModel = allocationOverModels.Select(x => x.Prioritization).FirstOrDefault();


                foreach (var item in allocationOverModels)
                {
                    batchcode = item.ClubName;
                    clubcode = (int)item.ClubCode;
                    skucode = (int)item.INUMBR;
                    description = item.IDESCR;
                    reqty = (int)item.RequestedQty;
                    
                    prioname = item.FinalPrio;
                    slotloc = item.SlotLoc;
                    lpn = item.LPN;
                    qtytopick = (int)item.QtyToPick;
                    dconfig = item.DConfigName;
                    stdpk = (double)item.ISTDPK;
                    prio = (int)item.Prioritization;
                    sequence = item.Sequence;


                    if(dconfig == "LAYER")
                    {
                        dconfig = "CASE";
                    }
                    else
                    {
                        dconfig = item.DConfigName;
                    }
                    
                        queryskus.InsertPicklist(batchcode, clubcode, skucode, description, reqty, prioname,prio, slotloc, lpn, qtytopick, qtypicked, dconfig, stdpk, status, sequence, userName, todaysDate);

                       // queryskus.InsertAllocationPicklist(batchcode, clubcode, skucode, reqty, prio, slotloc, lpn, dconfig, todaysDate);

                        queryskus.InsertBatchCode(batchcode, clubcode, todaysDate);
                    


                }
                return Json(new { success = true, message = "Successfully Saved!" }, JsonRequestBehavior.AllowGet);

            }

            catch (Exception)
            {
                return Json(new { success = false, message = $"Please try again empty data cannot be save!" }, JsonRequestBehavior.AllowGet);
            }
        }


        
        
        
        //To populate the number of entries based on the number of pallets
        [HttpPost]
        public ActionResult InsertPicklistReady(List<AllocationMerchandiseModel> allocationOverModels)
        {

            DateTime date = DateTime.Now;
            string todaysDate = date.ToString("MM-dd-yyyy,H:mm");
            int skucode = 0;
            int clubcode = 0;
            int reqty, prio;
            string dconfig;
            int status = 3;
            string remarks;
            
            int nopallet = 0;
            double weightpersku = 0;
            int perpallet = 0;
            int checkeds = 0;

            AllocationMerchandiseModel skumodel = new AllocationMerchandiseModel();
            try
            {
                foreach (var item in allocationOverModels)
                {
                    checkeds = (int)item.Checked;
                    clubcode = (int)item.ClubCode;
                    skucode = (int)item.INUMBR;
                    reqty = (int)item.RequestedQty;
                    dconfig = item.DConfigName;
                    prio = (int)item.Prioritization;
                    nopallet = (int)item.NoPallets;
                    perpallet = (int)item.QtyPerPallet;
                    weightpersku = (double)item.IWGHT;
                    remarks = item.Remarks;

                    

                    if (nopallet > 0 && checkeds == 1 && dconfig == "PALLET")
                    {
                        int break_pallet = 0;
                        //var line_pallet = allocationOverModels.Where(a => a.NoPallets > 0 && a.Checked == 1 && a.DConfigName == "PALLET").Count();
                        break_pallet = reqty / nopallet;
                        for (double innopallet = 1; innopallet <= nopallet;)
                        {
                            
                            queryskus.InsertCreateAllocation(clubcode, skucode, break_pallet, status, dconfig, prio,prio,remarks, todaysDate);

                            innopallet++;

                        }
                    }

                    else if (checkeds == 1 && dconfig == "CASE")
                    {

                        
                            queryskus.InsertCreateAllocation(clubcode, skucode, reqty, status, dconfig, prio,prio, remarks, todaysDate);

                    }
                    else if (checkeds == 1 && dconfig == "LAYER")
                    {


                        queryskus.InsertCreateAllocation(clubcode, skucode, reqty, status, dconfig, prio,prio, remarks, todaysDate);

                    }

                }
                return Json(new { success = true, message = "Successfully Saved!" }, JsonRequestBehavior.AllowGet);

            }

            catch (Exception)
            {
                return Json(new { success = false, message = $"Please try again empty data cannot be save!" }, JsonRequestBehavior.AllowGet);
            }
        }


        //To update Allocated Status
        [HttpPost]
        public ActionResult UpdateAllocatedStatus(List<AllocationMerchandiseModel> allocationOverModels)
        {
           
            int skucode = 0;
            int clubcode = 0;
            int  prio = 0;
            string batchcode;


            AllocationMerchandiseModel skumodel = new AllocationMerchandiseModel();
            try
            {

                var deptModel = allocationOverModels.Select(x => x.INUMBR).FirstOrDefault();
                var clubCodeModel = allocationOverModels.Select(x => x.ClubCode).FirstOrDefault();
                var prioModel = allocationOverModels.Select(x => x.Prioritization).FirstOrDefault();


                foreach (var item in allocationOverModels)
                {

                    batchcode = item.ClubName;
                    clubcode = (int)item.ClubCode;
                    skucode = (int)item.INUMBR;
                    
                    prio = (int)item.Prioritization;

                    bool insertDeptbool = queryskus.VerifyInsertPiclist(skucode, clubcode,prio);



                    if (insertDeptbool == true)
                    {

                        queryskus.UpdateAllocatedStatus(batchcode,clubcode, skucode,prio);
                        queryskus.DeleteStatusNull(clubcode,skucode);
                        
                       

                    }


                }
                return Json(new { success = true, message = "Successfully Saved!" }, JsonRequestBehavior.AllowGet);

            }

            catch (Exception)
            {
                return Json(new { success = false, message = $"Please try again empty data cannot be save!" }, JsonRequestBehavior.AllowGet);
            }
        }


        //To update Slotlocation from null
        [HttpPost]
        public ActionResult UpdateSlotLoc(List<AllocationMerchandiseModel> allocationOverModels)
        {
            
            int skucode = 0;
            int clubcode = 0;
            string lpn, slotloc;

            AllocationMerchandiseModel skumodel = new AllocationMerchandiseModel();
            try
            {

                var deptModel = allocationOverModels.Select(x => x.INUMBR).FirstOrDefault();
                var clubCodeModel = allocationOverModels.Select(x => x.ClubCode).FirstOrDefault();
                


                foreach (var item in allocationOverModels)
                {

                    clubcode = (int)item.ClubCode;
                    skucode = (int)item.INUMBR;
                    lpn = item.LPN;
                    slotloc = item.SlotLoc;

                    bool insertDeptbool = queryskus.VerifyUpdateSlotLoc(skucode, clubcode);



                    if (insertDeptbool == true)
                    {
                        
                        queryskus.UpdateSlotLoc(clubcode,skucode, lpn, slotloc);

                    }

                }
                return Json(new { success = true, message = "Successfully Saved!" }, JsonRequestBehavior.AllowGet);

            }

            catch (Exception)
            {
                return Json(new { success = false, message = $"No available Slot-Location!" }, JsonRequestBehavior.AllowGet);
            }
        }

        //To prevent updating slotlocation with value
        [HttpPost]
        public ActionResult UpdateSlotLocStop(List<AllocationMerchandiseModel> allocationOverModels)
        {

            int skucode = 0;
            int clubcode = 0;
            int prio = 0;
            int status,reqty;
          
            AllocationMerchandiseModel skumodel = new AllocationMerchandiseModel();
            try
            {

                var deptModel = allocationOverModels.Select(x => x.INUMBR).FirstOrDefault();
                var clubCodeModel = allocationOverModels.Select(x => x.ClubCode).FirstOrDefault();
                var prioModel = allocationOverModels.Select(x => x.Prioritization).FirstOrDefault();
                var statModel = allocationOverModels.Select(x => x.Status).FirstOrDefault();
                
                foreach (var item in allocationOverModels)
                {

                    clubcode = (int)item.ClubCode;
                    skucode = (int)item.INUMBR;
                    reqty = (int)item.RequestedQty;
                    prio = (int)item.Prioritization;
                    status = (int)item.Status;

                    queryskus.DeleteSlotLocStop(clubcode, skucode, prio);


                }
                return Json(new { success = true, message = "Successfully Saved!" }, JsonRequestBehavior.AllowGet);

            }

            catch (Exception)
            {
                return Json(new { success = false, message = $"Missing data detected!" }, JsonRequestBehavior.AllowGet);
            }
        }




    }
}