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
using System.Globalization;
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
    public class AllocationSKUController : Controller
    {

        SQLQueryAllocationSKU queryskus = new SQLQueryAllocationSKU();
        
        //To display and select all dropdown list
        public ActionResult Index()
        {
            try
            { 
            ModelState.Clear();
            AllocationSKUModel model = new AllocationSKUModel();
            
            model.ClubSKU = queryskus.GetClubSQL();

            var trucks = queryskus.GetTruckcategory();
            var trucklist = new List<SelectListItem>();
            foreach (var i in trucks)
            {
                trucklist.Add(new SelectListItem() { Text = i.CategoryName, Value = i.Code.ToString() });
            }
            

            var configs = queryskus.GetDConfig();
            var configlist = new List<SelectListItem>();
            foreach (var i in configs)
            {
                configlist.Add(new SelectListItem() { Text = i.Dconfig, Value = i.Code.ToString() });
            }
            ViewBag.Config = configlist;
            ViewBag.TruckLoadCategory = trucklist;
            return View(model);
            }

            catch (Exception ex)
            {
                return Json(ex, JsonRequestBehavior.AllowGet);
            }
        }

        //To search SKU inputed in SKU searchbox
        [HttpGet]
        public ActionResult SearchSKU(long id)
        {
            var verify = queryskus.CheckSKUId(id);
            if (verify != 0)
            {
                var result = queryskus.SelectSKU(id);
                var listclub = queryskus.GetClubSKU(id);
                

                AllocationSKUModel skus = new AllocationSKUModel()
                {
                    SKU = id,
                    DConfig = (int)result.DConfig,
                    // DConfigQty = (int)result.DConfigQty,
                    Category = (int)result.Category,
                    Description = result.Description,
                    Status = result.Status,
                    ISTDPK = (double)result.ISTDPK,
                    IVPLHI = (double)result.IVPLHI,
                    IVPLTI = (double)result.IVPLTI,
                    BuildToDF = result.BuildToDF,
                    //ClubSKU = clubs,
                    ClubSKU = listclub
                    //ClubInval = listinval



                };
                ViewBag.Description = skus;
                return Json(skus, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = $"SKU Not Found" }, JsonRequestBehavior.AllowGet);
            }

           
        }

       

        //To insert new SKU when not exist if exist update
        [HttpPost]
        public ActionResult InsertSKU(List<AllocationSKUModel> allocationSKUModels)
        {
          // string userName = "Junes";
            string userName = Session["Username"].ToString();
            DateTime date = DateTime.Now;
            string todaysDate = date.ToString("MM-dd-yyyy,H:mm");
           // string CreatedDate = date.ToString("yyyy-MM-dd");
            int sku = 0;
            int clubcode = 0;
            int  minimum, multiplier, leadtime,dconfig,category,roundedclubneeds,department,subdepartment,classname,subclass,vendor;
            string buildtodf;
            double buildto,triggers,clubneeds,ave,neededorder,ohit;
            int dconfigqty;
            AllocationSKUModel skumodel = new AllocationSKUModel();
            try
            {
                
                var skuModel = allocationSKUModels.Select(x => x.SKU).FirstOrDefault();
                var clubCodeModel = allocationSKUModels.Select(x => x.ClubCode).FirstOrDefault();
               
                foreach (var item in allocationSKUModels)
                {
                    sku = (int)item.SKU;
                    clubcode = (int)item.ClubCode;
                    ave = (double)item.AveSalesPerDay;
                    minimum = (int)item.Minimum;
                    multiplier = (int)item.Multiplier;
                    leadtime = (int)item.LeadTime;
                    buildto = (double)item.BuildTo;
                    triggers = (double)item.Triggers;
                    ohit = (double)item.OHITsum;
                    clubneeds = (double)item.ClubNeeds;
                    
                    dconfig = (int)item.DConfig;
                    category = (int)item.Category;
                    roundedclubneeds = (int)item.RoundedClubNeeds;
                    department = (int)item.Department;
                    subdepartment = (int)item.SubDepartment;
                    classname = (int)item.ClassName;
                    subclass = (int)item.SubClass;
                    vendor = (int)item.Vendor;
                    buildtodf = item.BuildToDF;
                    neededorder = (double)item.NeededOrder;
                    dconfigqty = (int)item.DConfigQty;
                    

                    bool insertSKUbool = queryskus.VerifyClubInsert(sku, clubcode);
                    if (insertSKUbool == true)
                    {

                        queryskus.UpdateSKU(sku, clubcode, ave,minimum, multiplier, leadtime, buildto, triggers, ohit,clubneeds, dconfig, category, roundedclubneeds, department, subdepartment, classname, subclass, vendor, buildtodf,neededorder,dconfigqty, userName, todaysDate);
                    }
                    else if (insertSKUbool == false)
                    {
                        insertSKUbool = queryskus.InsertSKU(sku, clubcode,ave, minimum, multiplier, leadtime, buildto, triggers, ohit, clubneeds, dconfig, category, roundedclubneeds, department, subdepartment, classname, subclass, vendor, buildtodf, neededorder, dconfigqty, userName,todaysDate);
                    }


                }

                

                    return Json(new { success = true, message = "Successfully Saved!" }, JsonRequestBehavior.AllowGet);

            }
            
            catch (Exception)
            {
                return Json(new { success = false, message = $"There is an empty column for SKU: {sku} and Club Code : {clubcode}" }, JsonRequestBehavior.AllowGet);
            }
        }

        //To verify if SKU is existing in MMS
        [HttpPost]
        public ActionResult InsertBatchCon(List<AllocationSKUModel> allocationSKUModels)
        {


            int sku = 0;

            AllocationSKUModel skumodel = new AllocationSKUModel();

            try
            {

                var skuModel = allocationSKUModels.GroupBy(x => x.SKU).ToList();




                foreach (var item in skuModel)
                {



                    sku = (int)item.Key;


                    var checkSKU = queryskus.CheckSKUId(sku);
                    if (checkSKU != 0)
                    {
                        sku = (int)item.Key;
                    }
                    else
                    {
                        return Json(new { success = false, message = $"The SKU:{sku} is not found" }, JsonRequestBehavior.AllowGet);
                    }



                }
                return Json(new { success = true, message = "All SKU successfully validated. Do you want to upload this data?" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {

                return Json(new { success = false, message = $"The SKU:{sku} is not found" }, JsonRequestBehavior.AllowGet);


            }


        }
        //To insert new SKU using batch upload when not exist if exist update
        [HttpPost]
        public ActionResult InsertBatch(List<AllocationSKUModel> allocationSKUModels)
        {
            string userName = Session["Username"].ToString();
           // string userName = "junes";
            DateTime date = DateTime.Now;
            string todaysDate = date.ToString("MM-dd-yyyy,H:mm");
            int sku = 0;
            int clubcode = 0;
            int minimum, multiplier, leadtime,category,dconfig;
         
            string ConfigExcel;
            string CategoryExcel, buildtodf;
            
            AllocationSKUModel skumodel = new AllocationSKUModel();
            
                try
                {
               
                    var skuModel = allocationSKUModels.Select(x => x.SKU).FirstOrDefault();
                    var clubCodeModel = allocationSKUModels.Select(x => x.ClubCode).FirstOrDefault();
                    var configCodeModel = allocationSKUModels.Select(x => x.ConfigExcel).FirstOrDefault();
                    var categoryCodeModel = allocationSKUModels.Select(x => x.CategoryExcel).FirstOrDefault();



                    foreach (var item in allocationSKUModels) 
                    {




                        sku = (int)item.SKU;
                        clubcode = (int)item.ClubCode;
                        minimum = (int)item.Minimum;
                        multiplier = (int)item.Multiplier;
                        leadtime = (int)item.LeadTime;
                        dconfig = (int)item.DConfig;
                        //dconfigqty = (double)item.DConfigQty;
                        CategoryExcel = item.CategoryExcel;
                        ConfigExcel = item.ConfigExcel;
                        
                        category = (int)item.Category;
                        buildtodf = item.BuildToDF;

                    
                    switch (ConfigExcel)
                        {
                            case "PALLET":
                            case "Pallet":
                            case "pallet":
                                dconfig = 1;
                                break;
                            case "LAYER":
                            case "Layer":
                            case "layer":
                                dconfig = 2;
                                break;
                            case "CASE":
                            case "Case":
                            case "case":
                                dconfig = 3;
                                break;
                            default:
                                dconfig = 0;
                                break;
                        }

                        switch (CategoryExcel)
                        {
                            case "TOP ":
                           case "TOP":
                           case "Top":
                            case "top":
                                category = 1;
                                break;
                            case "BASE":
                            case "Base":
                            case "base":
                                category = 2;
                                break;
                            case "FRAGILE":
                            case "Fragile":
                            case "fragile":
                                category = 3;
                                break;
                            case "OFFSIZE":
                            case "Offsize":
                            case "offsize":
                                category = 4;
                                break;
                            default:
                                category = 0;

                                return Json(new { success = false, message = $"There is an invalid data of column Category for  SKU: {sku} and Club Code : {clubcode}" }, JsonRequestBehavior.AllowGet);


                        }




                    bool insertSKUbool = queryskus.VerifyBatchInsert(sku, clubcode);

                    if (insertSKUbool == true)
                    {


                        queryskus.UpdateBatch( minimum, multiplier, leadtime, sku, clubcode, dconfig, category, buildtodf, userName, todaysDate);



                    }
                    else if (insertSKUbool == false)
                    {


                        insertSKUbool = queryskus.InsertBatch( minimum, multiplier, leadtime, sku, clubcode, dconfig, category, buildtodf, userName, todaysDate);
                    }

                }
                
               
                return Json(new { success = true, message = "Successfully Uploaded!" }, JsonRequestBehavior.AllowGet);

               
                }
                catch (Exception)
                {
                    
                    return Json(new { success = false, message = $"There is an invalid data for SKU: {sku} and Club Code : {clubcode}" }, JsonRequestBehavior.AllowGet);

                
                }
            
            
        }



        


    }
}