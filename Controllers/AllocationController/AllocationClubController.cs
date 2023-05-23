using Microsoft.AspNet.Identity;
using SNRWMSPortal.Common;
using SNRWMSPortal.DataAccess;
using SNRWMSPortal.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SNRWMSPortal.Controllers
{
    [AuthorizeRoles(Role.Allocation, Role.SystemAdministrator)]
    public class AllocationClubController : Controller
    {

        SQLQueryAllocationSKU queryskus = new SQLQueryAllocationSKU();
        public ActionResult Index()
        {
            try
            {
            ModelState.Clear();
            AllocationSKUModel model = new AllocationSKUModel();
            model.ClubSKU = queryskus.GetClubMMS();
            return View(model);
            }

            catch (Exception ex)
            {
                return Json(ex, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult EditClub(int id)
        {

            var equipment = queryskus.GetClubProv(id).Where(w => w.STRNUM == id).FirstOrDefault();

            return Json(equipment, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult InsertClub(List<AllocationSKUModel> allocationSKUModels)
        {
            int clubcode;
            string clubname;
            AllocationSKUModel skumodel = new AllocationSKUModel();

            
            
            
            // bool insertSKUbool = false;
            try
            {
                var clubCodeModel = allocationSKUModels.Select(x => x.ClubCode).FirstOrDefault();
                
                foreach (var item in allocationSKUModels)
                {
                    clubcode = (int)item.ClubCode;
                    clubname = (string)item.ClubName;
                    bool insertSKUbool = queryskus.VerifyInsertClub(clubcode);

                    
                    if (insertSKUbool == true)
                    {
                        
                        queryskus.UpdateClub(clubcode, clubname);
                    }
                    else if(insertSKUbool == false)
                    {
                        
                        insertSKUbool = queryskus.InsertClub(clubcode, clubname);

                    }
                }
                    // TempData["Success"] = "Successfully Updated!";
                
                

                return Json("");
            }
            catch
            {
                throw;
            }
        }


        [HttpPost]
        public ActionResult UpdateProvince(List<AllocationSKUModel> allocationClubModels)
        {

            int clubcode = 0;

            int status, isprovince;



            AllocationSKUModel skumodel = new AllocationSKUModel();
            try
            {

                
                var clubCodeModel = allocationClubModels.Select(x => x.ClubCode).FirstOrDefault();


                foreach (var item in allocationClubModels)
                {

                    clubcode = (int)item.ClubCode;
                    status = (int)item.ClubStatus;
                    isprovince = (int)item.IsProvince;

                    queryskus.UpdateClubProvince(clubcode,status,isprovince);

                }
                return Json(new { success = true, message = "Successfully Updated!" }, JsonRequestBehavior.AllowGet);

            }

            catch (Exception)
            {
                return Json(new { success = false, message = $"Please try again empty data cannot be update!" }, JsonRequestBehavior.AllowGet);
            }
        }


    }
}