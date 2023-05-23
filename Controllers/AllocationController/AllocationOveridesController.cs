using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json.Linq;
using SNRWMSPortal.Common;
using SNRWMSPortal.DataAccess;
using SNRWMSPortal.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.Pkcs;
using System.Web;
using System.Web.Helpers;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace SNRWMSPortal.Controllers
{
    [AuthorizeRoles(Role.Allocation, Role.SystemAdministrator)]
    public class AllocationOveridesController : Controller
    {

        SQLQueryAllocationOverRides queryskus = new SQLQueryAllocationOverRides();
        public ActionResult Index()
        {
            try
            { 
            int deptcode = 0;
            int subcode = 0;
            int classcode = 0;
           
                var department = queryskus.GetDepartment();
                var departmentlist = new List<SelectListItem>();
                foreach (var i in department)
                {
                    departmentlist.Add(new SelectListItem() { Text = i.DeptName, Value = i.Code.ToString() });
                }


                var subdept = queryskus.GetDepartmentSub(deptcode);
                var subdeptlist = new List<SelectListItem>();
                foreach (var i in subdept)
                {
                    subdeptlist.Add(new SelectListItem() { Text = i.SubDeptName, Value = i.SubCode.ToString() });
                }
                ViewBag.SubDepartments = subdeptlist;
                ViewBag.Departments = departmentlist;

                var classlist = queryskus.GetClass(deptcode, subcode);
                var classeslist = new List<SelectListItem>();
                foreach (var i in classlist)
                {
                    classeslist.Add(new SelectListItem() { Text = i.ClassName, Value = i.ClassCode.ToString() });
                }


                var subclasslsit = queryskus.GetSubClass(deptcode, subcode, classcode);
                var subclasseslist = new List<SelectListItem>();
                foreach (var i in subclasslsit)
                {
                    subclasseslist.Add(new SelectListItem() { Text = i.SubClassName, Value = i.SubClassCode.ToString() });
                }
                ViewBag.SubClasses = subclasseslist;
                ViewBag.Classes = classeslist;

                return View();
            }

            catch (Exception ex)
            {
                return Json(ex, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpGet]
        public ActionResult SearchSubDept(int deptcode)
        {
            
            var result = queryskus.GetDepartmentSub(deptcode);



            var subdeptlist = new List<SelectListItem>();
            foreach (var i in result)
            {
                subdeptlist.Add(new SelectListItem() { Text = i.SubDeptName, Value = i.SubCode.ToString() });
            }

            return Json(subdeptlist, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public ActionResult SearchClass(int deptcode, int subcode)
        {

            var result = queryskus.GetClass(deptcode, subcode);



            var subdeptlist = new List<SelectListItem>();
            foreach (var i in result)
            {
                subdeptlist.Add(new SelectListItem() { Text = i.ClassName, Value = i.ClassCode.ToString() });
            }

            return Json(subdeptlist, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public ActionResult SearchSubClass(int deptcode, int subcode, int classcode)
        {

            var result = queryskus.GetSubClass(deptcode, subcode, classcode);



            var subdeptlist = new List<SelectListItem>();
            foreach (var i in result)
            {
                subdeptlist.Add(new SelectListItem() { Text = i.SubClassName, Value = i.SubClassCode.ToString() });
            }

            return Json(subdeptlist, JsonRequestBehavior.AllowGet);

        }


        [HttpGet]
        public ActionResult SearchSKU(long id)
        {




            var result = queryskus.SelectSKU(id);
            var verify = queryskus.SKUAvailable(id);
            if (verify == 1)
            {
                var listskus = queryskus.GetClubSKU(id);

                AllocationOverRides skus = new AllocationOverRides()
                {
                    SKU = id,
                    Description = result.Description,
                    OverRides = listskus


                };
                ViewBag.Description = skus;
                return Json(skus, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = $"SKU Not Found in SKU setup with Multiplier" }, JsonRequestBehavior.AllowGet);
            }


        }

        [HttpGet]
        public ActionResult SearchAllCateg(int allcode)
        {
            
            
                var result = queryskus.SearchAll(allcode);
                AllocationOverRides skus = new AllocationOverRides()
                {
                   
                    OverRides = result

                };
                ViewBag.OverRides = skus;
                return Json(skus, JsonRequestBehavior.AllowGet);
            
        }


        [HttpGet]
        public ActionResult SearchVendorCode(int vcode)
        {
            var mmsres = queryskus.SelectVendorName(vcode);
            var verify = queryskus.VendorAvailable(vcode);
            if (verify == 1)
            {
                var result = queryskus.SearchVCode(vcode);
                AllocationOverRides skus = new AllocationOverRides()
                {
                    VendorCode = vcode,
                    VendorName = mmsres.VendorName,
                    
                    OverRides = result

                };
                ViewBag.OverRides = skus;
                return Json(skus, JsonRequestBehavior.AllowGet);
            }
            //return new JsonResult() { Data = skus, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
            else
            {
                return Json(new { success = false, message = $"Vendor Not Found in SKU setup with Multiplier" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult SearchDeptCode(int deptcode)
        {

           
                var listskus = queryskus.GetClubDeptCode(deptcode);
                AllocationOverRides skus = new AllocationOverRides()
                {
                    OverRides = listskus


                };
                ViewBag.Description = skus;
                return Json(skus, JsonRequestBehavior.AllowGet);
            
        }
        [HttpGet]
        public ActionResult SearchSubDeptCode(int deptcode,int subdeptcode)
        {


            var listskus = queryskus.GetClubSubDeptCode(deptcode,subdeptcode);
            AllocationOverRides skus = new AllocationOverRides()
            {
                OverRides = listskus


            };
            ViewBag.Description = skus;
            return Json(skus, JsonRequestBehavior.AllowGet);

        }
        [HttpGet]
        public ActionResult SearchClassCode(int deptcode, int subdeptcode,int classcode)
        {


            var listskus = queryskus.GetClubClassCode(deptcode, subdeptcode,classcode);
            AllocationOverRides skus = new AllocationOverRides()
            {
                OverRides = listskus


            };
            ViewBag.Description = skus;
            return Json(skus, JsonRequestBehavior.AllowGet);

        }
        [HttpGet]
        public ActionResult SearchSubClassCode(int deptcode, int subdeptcode, int classcode,int subclasscode)
        {


            var listskus = queryskus.GetClubSubClassCode(deptcode, subdeptcode, classcode,subclasscode);
            AllocationOverRides skus = new AllocationOverRides()
            {
                OverRides = listskus


            };
            ViewBag.Description = skus;
            return Json(skus, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult InsertOverRides(List<AllocationOverRides> allocationOverModels)
        {
            int sku = 0;
            int clubcode = 0;
            int multiplieradjustment;
            string datefrom, dateto;

           
            AllocationOverRides skumodel = new AllocationOverRides();
            try
            {

                var skuModel = allocationOverModels.Select(x => x.SKU).FirstOrDefault();
                var clubCodeModel = allocationOverModels.Select(x => x.ClubCode).FirstOrDefault();

                foreach (var item in allocationOverModels)
                {
                    
                    clubcode = (int)item.ClubCode;
                   
                    multiplieradjustment = (int)item.MultiplierAdjustment;
                    sku = (int)item.SKU;
                    datefrom = (string)item.DateFrom;
                    dateto = (string)item.DateTo;


                    bool insertSKUbool = queryskus.VerifyOverInsert(sku, clubcode);
                    var verify = queryskus.SKUAvailable(sku);
                    DateTime from;
                    DateTime to;

                    bool chValidityfrom = DateTime.TryParseExact(
                    datefrom,
                    "MM-dd-yyyy",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out from);
                    bool chValidityto = DateTime.TryParseExact(
                    dateto,
                    "MM-dd-yyyy",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out to);
                    var parameterDatefrom = DateTime.ParseExact(datefrom, "MM-dd-yyyy", CultureInfo.InvariantCulture);
                    var parameterDateto = DateTime.ParseExact(dateto, "MM-dd-yyyy", CultureInfo.InvariantCulture);
                    var todaysDate = DateTime.Today;

                    if (parameterDatefrom < todaysDate)
                    {
                        return Json(new { success = false, message = $"There is an invalid DateFrom for SKU: {sku} and Club Code : {clubcode}. You cannot set date on DateFrom that later than today's date." }, JsonRequestBehavior.AllowGet);
                    }

                    if (parameterDateto < todaysDate || parameterDateto < parameterDatefrom)
                    {
                        return Json(new { success = false, message = $"There is an invalid DateFrom for SKU: {sku} and Club Code : {clubcode}. You cannot set date on DateTo that later than today's date or the DateTo is less than to the DateFrom ." }, JsonRequestBehavior.AllowGet);
                    }
                    if (chValidityfrom == false)
                    {
                        return Json(new { success = false, message = $"There is an invalid DateFrom for SKU: {sku} and Club Code : {clubcode}. The Date format should be MM-dd-yyyy" }, JsonRequestBehavior.AllowGet);
                    }
                    if (chValidityto == false)
                    {
                        return Json(new { success = false, message = $"There is an invalid DateTo for SKU: {sku} and Club Code : {clubcode}. The Date format should be MM-dd-yyyy" }, JsonRequestBehavior.AllowGet);
                    }
                    if (verify == 0)
                    {
                        return Json(new { success = false, message = $"SKU Not Found in SKU setup with Multiplier! Please setup first on SKU Setup" }, JsonRequestBehavior.AllowGet);
                    }
                    if (insertSKUbool == true)
                    {

                        queryskus.UpdateOverSKU(clubcode, multiplieradjustment,sku,datefrom,dateto);
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
        public ActionResult InsertAllCateg(List<AllocationOverRides> allocationOverModels)
        {
            string userName = Session["Username"].ToString();
            DateTime date = DateTime.Now;
            string todaysDate = date.ToString("MM-dd-yyyy,H:mm");
            int searchall = 0;
            
            int clubcode, multiplieradjustment, department, subdepartment, classcode, subclasscode,vendorcode;
            string datefrom, dateto;

            AllocationOverRides skumodel = new AllocationOverRides();
            try
            {

                var allsearchModel = allocationOverModels.Select(x => x.AllSearch).FirstOrDefault();
                var clubCodeModel = allocationOverModels.Select(x => x.ClubCode).FirstOrDefault();

                foreach (var item in allocationOverModels)
                {


                    clubcode = (int)item.ClubCode;
                    
                    multiplieradjustment = (int)item.MultiplierAdjustment;
                    searchall = (int)item.AllSearch;
                    department = (int)item.Department;
                    subdepartment = (int)item.SubDepartment;
                    classcode = (int)item.ClassCode;
                    subclasscode = (int)item.SubClassCode;
                    vendorcode = (int)item.VendorCode;
                    datefrom = (string)item.DateFrom;
                    dateto = (string)item.DateTo;



                    bool insertSKUbool = queryskus.VerifyOverInsertAllSearch(clubcode,searchall);


                    

                    if (insertSKUbool == true)
                    {
                        queryskus.UpdateOverSearchAll(clubcode,  multiplieradjustment, searchall,datefrom,dateto,userName,todaysDate);
                        queryskus.UpdateOverAll(clubcode,  multiplieradjustment,datefrom,dateto,userName,todaysDate);
                        queryskus.UpdateSKUAll(clubcode,  multiplieradjustment,datefrom,dateto);
                    }
                    else if (insertSKUbool == false)
                    {

                        queryskus.InsertOverSearchAll(clubcode,  multiplieradjustment, searchall,department, subdepartment, classcode, subclasscode, vendorcode,datefrom,dateto, userName, todaysDate);
                        queryskus.UpdateOverAll(clubcode,  multiplieradjustment,datefrom,dateto,userName, todaysDate);
                        queryskus.UpdateSKUAll(clubcode,  multiplieradjustment,datefrom,dateto);
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
        public ActionResult InsertOverRidesVendor(List<AllocationOverRides> allocationOverModels)
        {
            string userName = Session["Username"].ToString();
            DateTime date = DateTime.Now;
            string todaysDates = date.ToString("MM-dd-yyyy,H:mm");
            int vendorcode = 0;
            int clubcode = 0;
            int multiplieradjustment,department,subdepartment,classcode,subclasscode,searchall;
            string datefrom, dateto;

            

            AllocationOverRides skumodel = new AllocationOverRides();
            try
            {

                var vendorModel = allocationOverModels.Select(x => x.VendorCode).FirstOrDefault();
                var clubCodeModel = allocationOverModels.Select(x => x.ClubCode).FirstOrDefault();

                foreach (var item in allocationOverModels)
                {

                    clubcode = (int)item.ClubCode;
                    
                    multiplieradjustment = (int)item.MultiplierAdjustment;
                    vendorcode = (int)item.VendorCode;
                    department = (int)item.Department;
                    subdepartment = (int)item.SubDepartment;
                    classcode = (int)item.ClassCode;
                    subclasscode = (int)item.SubClassCode;
                    searchall = (int)item.AllSearch;
                    datefrom = (string)item.DateFrom;
                    dateto = (string)item.DateTo;

                    bool insertSKUboolSKU = queryskus.VerifySKUInsertVendor(vendorcode, clubcode);
                    bool insertSKUbool = queryskus.VerifyOverInsertVendor(vendorcode, clubcode);
                    var verify = queryskus.VendorAvailable(vendorcode);
                    DateTime from;
                    DateTime to;

                    bool chValidityfrom = DateTime.TryParseExact(
                    datefrom,
                    "MM-dd-yyyy",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out from);
                    bool chValidityto = DateTime.TryParseExact(
                    dateto,
                    "MM-dd-yyyy",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out to);
                    var parameterDatefrom = DateTime.ParseExact(datefrom, "MM-dd-yyyy", CultureInfo.InvariantCulture);
                    var parameterDateto = DateTime.ParseExact(dateto, "MM-dd-yyyy", CultureInfo.InvariantCulture);
                    var todaysDate = DateTime.Today;

                    if (parameterDatefrom < todaysDate)
                    {
                        return Json(new { success = false, message = $"There is an invalid DateFrom for Vendor: {vendorcode} and Club Code : {clubcode}. You cannot set date on DateFrom that later than today's date." }, JsonRequestBehavior.AllowGet);
                    }

                    if (parameterDateto < todaysDate || parameterDateto < parameterDatefrom)
                    {
                        return Json(new { success = false, message = $"There is an invalid DateFrom for Vendor: {vendorcode} and Club Code : {clubcode}. You cannot set date on DateTo that later than today's date or the DateTo is less than to the DateFrom ." }, JsonRequestBehavior.AllowGet);
                    }

                    if (chValidityfrom == false)
                    {
                        return Json(new { success = false, message = $"There is an invalid DateFrom for Vendor: {vendorcode} and Club Code : {clubcode}. The Date format should be MM-dd-yyyy" }, JsonRequestBehavior.AllowGet);
                    }
                    if (chValidityto == false)
                    {
                        return Json(new { success = false, message = $"There is an invalid DateTo for Vendor: {vendorcode} and Club Code : {clubcode}. The Date format should be MM-dd-yyyy" }, JsonRequestBehavior.AllowGet);
                    }
                    if (verify == 0)
                    {
                        return Json(new { success = false, message = $"Vendor Not Found in SKU setup with Multiplier! Please setup first on SKU Setup" }, JsonRequestBehavior.AllowGet);
                    }
                    if (insertSKUboolSKU == true )
                    {

                        queryskus.UpdateSKUVendor(clubcode, multiplieradjustment, vendorcode,datefrom,dateto);

                        
                    }
                    if(insertSKUbool == true)
                    {
                        queryskus.UpdateOverVendor(clubcode, multiplieradjustment, vendorcode,0, 0, 0, 0,datefrom,dateto,userName,todaysDates);
                    }
                    else if(insertSKUbool == false)
                    {
                        
                        queryskus.InsertOverVendor(clubcode, multiplieradjustment, vendorcode, department, subdepartment, classcode, subclasscode,searchall,datefrom,dateto, userName, todaysDates);
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
        public ActionResult InsertOverDept(List<AllocationOverRides> allocationOverModels)
        {

            string userName = Session["Username"].ToString();
            DateTime date = DateTime.Now;
            string todaysDates = date.ToString("MM-dd-yyyy,H:mm");
            int deptcode = 0;
            int clubcode = 0;
            int  multiplieradjustment, subdepartment, classcode, subclasscode, vendor, searchall;
            string datefrom,dateto;


            AllocationOverRides skumodel = new AllocationOverRides();
            try
            {

                var deptModel = allocationOverModels.Select(x => x.Department).FirstOrDefault();
                var clubCodeModel = allocationOverModels.Select(x => x.ClubCode).FirstOrDefault();
                

                foreach (var item in allocationOverModels)
                {

                    clubcode = (int)item.ClubCode;
                    deptcode = (int)item.Department;
                    
                    multiplieradjustment = (int)item.MultiplierAdjustment;
                    subdepartment = (int)item.SubDepartment;
                    classcode = (int)item.ClassCode;    
                    subclasscode = (int)item.SubClassCode;
                    vendor = (int)item.VendorCode;
                    searchall = (int)item.AllSearch;
                    datefrom = (string)item.DateFrom;
                    dateto = (string)item.DateTo;

                    bool insertDeptbool = queryskus.VerifyOverInsertDept(clubcode,deptcode);
                    bool insertSKUboolSKUDept = queryskus.VerifySKUInsertDept(clubcode,deptcode);

                    var verify = queryskus.DeptAvailable(deptcode);
                    if (verify == 0)
                    {
                        return Json(new { success = false, message = $"Department Not Found in SKU setup with Multiplier! Please setup first on SKU Setup" }, JsonRequestBehavior.AllowGet);
                    }

                        if (insertSKUboolSKUDept == true)
                    {

                        queryskus.UpdateSKUDept(clubcode,  multiplieradjustment, deptcode,datefrom,dateto);


                    }

                    if (insertDeptbool == true)
                    {

                        queryskus.UpdateOverDept(clubcode,  multiplieradjustment,deptcode,datefrom,dateto,userName,todaysDates);

                    }
                    
                    else 
                    {

                        queryskus.InsertOverDept(clubcode,  multiplieradjustment, deptcode, subdepartment, classcode, subclasscode,vendor,searchall,datefrom,dateto,userName,todaysDates);
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
        public ActionResult InsertOverSubDept(List<AllocationOverRides> allocationOverModels)
        {
            string userName = Session["Username"].ToString();
            DateTime date = DateTime.Now;
            string todaysDates = date.ToString("MM-dd-yyyy,H:mm");
            int deptcode = 0;
            int clubcode = 0;
            int subdepartment = 0;
            int  multiplieradjustment,classcode, subclasscode,vendor, searchall;
            string datefrom, dateto;

            AllocationOverRides skumodel = new AllocationOverRides();
            try
            {

                var deptModel = allocationOverModels.Select(x => x.Department).FirstOrDefault();
                var clubCodeModel = allocationOverModels.Select(x => x.ClubCode).FirstOrDefault();
                var subdeptCodeModel = allocationOverModels.Select(x => x.SubDepartment).FirstOrDefault();
                //var classCodeModel = allocationOverModels.Select(x => x.ClassCode).FirstOrDefault();
                //var subclassCodeModel = allocationOverModels.Select(x => x.SubClassCode).FirstOrDefault();

                foreach (var item in allocationOverModels)
                {

                    clubcode = (int)item.ClubCode;
                    
                    multiplieradjustment = (int)item.MultiplierAdjustment;
                    deptcode = (int)item.Department;
                    subdepartment = (int)item.SubDepartment;
                    classcode = (int)item.ClassCode;
                    subclasscode = (int)item.SubClassCode;
                    vendor = (int)item.VendorCode;
                    searchall = (int)item.AllSearch;
                    datefrom = (string)item.DateFrom;
                    dateto = (string)item.DateTo;

                    bool insertSKUboolSKUSubDept = queryskus.VerifySKUInsertSubDept(clubcode, deptcode,subdepartment);
                    bool insertDeptbool = queryskus.VerifyOverInsertSubDept(clubcode, deptcode,subdepartment);

                    var verify = queryskus.SubDeptAvailable(deptcode,subdepartment);
                    if (verify == 0)
                    {
                        return Json(new { success = false, message = $"Sub-Department Not Found in SKU setup with Multiplier! Please setup first on SKU Setup" }, JsonRequestBehavior.AllowGet);
                    }

                    if (insertSKUboolSKUSubDept == true)
                    {

                        queryskus.UpdateSKUSubDept(clubcode,  multiplieradjustment, deptcode,subdepartment,datefrom,dateto);


                    }
                    if (insertDeptbool == true)
                    {

                        queryskus.UpdateOverSubDept(clubcode,  multiplieradjustment, deptcode, subdepartment,datefrom,dateto,userName,todaysDates);

                    }

                    else if (insertDeptbool == false)
                    {

                        queryskus.InsertOverSubDept(clubcode,  multiplieradjustment, deptcode, subdepartment,classcode,subclasscode,vendor,searchall,datefrom,dateto,userName,todaysDates);
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
        public ActionResult InsertOverClass(List<AllocationOverRides> allocationOverModels)
        {
            string userName = Session["Username"].ToString();
            DateTime date = DateTime.Now;
            string todaysDates = date.ToString("MM-dd-yyyy,H:mm");
            int deptcode = 0;
            int clubcode = 0;
            int subdepartment = 0;
            int classcode = 0;
            int  multiplieradjustment,subclasscode,vendor,searchall;
            string datefrom, dateto;

            AllocationOverRides skumodel = new AllocationOverRides();
            try
            {

                var deptModel = allocationOverModels.Select(x => x.Department).FirstOrDefault();
                var clubCodeModel = allocationOverModels.Select(x => x.ClubCode).FirstOrDefault();
                var subdeptCodeModel = allocationOverModels.Select(x => x.SubDepartment).FirstOrDefault();
                var classCodeModel = allocationOverModels.Select(x => x.ClassCode).FirstOrDefault();
                //var subclassCodeModel = allocationOverModels.Select(x => x.SubClassCode).FirstOrDefault();

                foreach (var item in allocationOverModels)
                {

                    clubcode = (int)item.ClubCode;
                    
                    multiplieradjustment = (int)item.MultiplierAdjustment;
                    deptcode = (int)item.Department;
                    subdepartment = (int)item.SubDepartment;
                    classcode = (int)item.ClassCode;
                    subclasscode = (int)item.SubClassCode;
                    vendor = (int)item.VendorCode;
                    searchall = (int)item.AllSearch;
                    datefrom = (string)item.DateFrom;
                    dateto = (string)item.DateTo;

                    bool insertSKUboolSKUClass = queryskus.VerifySKUInsertClass(clubcode, deptcode, subdepartment,classcode);
                    bool insertDeptbool = queryskus.VerifyOverInsertClass(clubcode, deptcode, subdepartment,classcode);
                    var verify = queryskus.ClassAvailable(deptcode, subdepartment,classcode);
                    if (verify == 0)
                    {
                        return Json(new { success = false, message = $"Class Not Found in SKU setup with Multiplier! Please setup first on SKU Setup" }, JsonRequestBehavior.AllowGet);
                    }

                    if (insertSKUboolSKUClass == true)
                    {

                        queryskus.UpdateSKUClass(clubcode,  multiplieradjustment, deptcode, subdepartment,classcode,datefrom,dateto);


                    }
                    if (insertDeptbool == true)
                    {

                        queryskus.UpdateOverClass(clubcode,  multiplieradjustment, deptcode, subdepartment, classcode,datefrom,dateto,userName,todaysDates);

                    }

                    else if (insertDeptbool == false)
                    {

                        queryskus.InsertOverClass(clubcode,  multiplieradjustment, deptcode, subdepartment, classcode, subclasscode,vendor,searchall,datefrom,dateto,userName,todaysDates);
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
        public ActionResult InsertOverSubClass(List<AllocationOverRides> allocationOverModels)
        {
            string userName = Session["Username"].ToString();
            DateTime date = DateTime.Now;
            string todaysDates = date.ToString("MM-dd-yyyy,H:mm");
            int deptcode = 0;
            int clubcode = 0;
            int subdepartment = 0;
            int classcode = 0;
            int subclasscode = 0;
            int  multiplieradjustment,vendor, searchall;
            string datefrom, dateto;

            AllocationOverRides skumodel = new AllocationOverRides();
            try
            {

                var deptModel = allocationOverModels.Select(x => x.Department).FirstOrDefault();
                var clubCodeModel = allocationOverModels.Select(x => x.ClubCode).FirstOrDefault();
                var subdeptCodeModel = allocationOverModels.Select(x => x.SubDepartment).FirstOrDefault();
                var classCodeModel = allocationOverModels.Select(x => x.ClassCode).FirstOrDefault();
                var subclassCodeModel = allocationOverModels.Select(x => x.SubClassCode).FirstOrDefault();

                foreach (var item in allocationOverModels)
                {

                    clubcode = (int)item.ClubCode;
                   
                    multiplieradjustment = (int)item.MultiplierAdjustment;
                    deptcode = (int)item.Department;
                    subdepartment = (int)item.SubDepartment;
                    classcode = (int)item.ClassCode;
                    subclasscode = (int)item.SubClassCode;
                    vendor = (int)item.VendorCode;
                    searchall = (int)item.AllSearch;
                    datefrom = (string)item.DateFrom;
                    dateto = (string)item.DateTo;

                    bool insertSKUboolSKUSubClass = queryskus.VerifySKUInsertSubClass(clubcode, deptcode, subdepartment, classcode,subclasscode);
                    bool insertDeptbool = queryskus.VerifyOverInsertSubClass(clubcode, deptcode, subdepartment, classcode,subclasscode);
                    var verify = queryskus.SubClassAvailable(deptcode, subdepartment,classcode,subclasscode);
                    if (verify == 0)
                    {
                        return Json(new { success = false, message = $"Sub-Class Not Found in SKU setup with Multiplier! Please setup first on SKU Setup" }, JsonRequestBehavior.AllowGet);
                    }
                    if (insertSKUboolSKUSubClass == true)
                    {

                        queryskus.UpdateSKUSubClass(clubcode,  multiplieradjustment, deptcode, subdepartment, classcode,subclasscode,datefrom,dateto);


                    }
                    if (insertDeptbool == true)
                    {

                        queryskus.UpdateOverSubClass(clubcode,  multiplieradjustment, deptcode, subdepartment, classcode, subclasscode,datefrom,dateto,userName,todaysDates);

                    }

                    else if (insertDeptbool == false)
                    {

                        queryskus.InsertOverSubClass(clubcode,  multiplieradjustment, deptcode, subdepartment, classcode, subclasscode,vendor, searchall,datefrom, dateto, userName, todaysDates);
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
        public ActionResult InsertBatchDept(List<AllocationOverRides> allocationOverModels)
        {
            string userName = Session["Username"].ToString();
            DateTime date = DateTime.Now;
            string todaysDates = date.ToString("MM-dd-yyyy,H:mm");
            int deptcode = 0;
            int clubcode = 0;
            int subdepartment = 0;
            int classcode = 0;
            int subclasscode = 0;
            int multiplieradjustment, vendor, searchall;
            string datefrom, dateto;

            AllocationOverRides skumodel = new AllocationOverRides();
            try
            {

                var deptModel = allocationOverModels.Select(x => x.Department).FirstOrDefault();
                var clubCodeModel = allocationOverModels.Select(x => x.ClubCode).FirstOrDefault();
                var subdeptCodeModel = allocationOverModels.Select(x => x.SubDepartment).FirstOrDefault();
                var classCodeModel = allocationOverModels.Select(x => x.ClassCode).FirstOrDefault();
                var subclassCodeModel = allocationOverModels.Select(x => x.SubClassCode).FirstOrDefault();

                foreach (var item in allocationOverModels)
                {

                    clubcode = (int)item.ClubCode;

                    multiplieradjustment = (int)item.MultiplierAdjustment;
                    deptcode = (int)item.Department;
                    subdepartment = (int)item.SubDepartment;
                    classcode = (int)item.ClassCode;
                    subclasscode = (int)item.SubClassCode;
                    vendor = (int)item.VendorCode;
                    searchall = (int)item.AllSearch;
                    datefrom = (string)item.DateFrom;
                    dateto = (string)item.DateTo;

                    bool insertSKUboolSKUDept = queryskus.VerifySKUInsertDept(clubcode, deptcode);
                    bool insertDeptbool = queryskus.VerifyOverInsertDept(clubcode, deptcode);
                    var verify = queryskus.DeptAvailable(deptcode);

                    bool insertSKUboolSKUSubDept = queryskus.VerifySKUInsertSubDept(clubcode, deptcode, subdepartment);
                    bool insertDeptboolsub = queryskus.VerifyOverInsertSubDept(clubcode, deptcode, subdepartment);

                    bool insertSKUboolSKUClass = queryskus.VerifySKUInsertClass(clubcode, deptcode, subdepartment, classcode);
                    bool insertDeptboolclass = queryskus.VerifyOverInsertClass(clubcode, deptcode, subdepartment, classcode);
                    var verifyclass = queryskus.ClassAvailable(deptcode, subdepartment, classcode);

                    bool insertSKUboolSKUSubClass = queryskus.VerifySKUInsertSubClass(clubcode, deptcode, subdepartment, classcode, subclasscode);
                    bool insertDeptboolsubclass = queryskus.VerifyOverInsertSubClass(clubcode, deptcode, subdepartment, classcode, subclasscode);
                    var verifysubclass = queryskus.SubClassAvailable(deptcode, subdepartment, classcode, subclasscode);

                    var verifysub = queryskus.SubDeptAvailable(deptcode, subdepartment);
                    DateTime from;
                    DateTime to;

                    bool chValidityfrom = DateTime.TryParseExact(
                    datefrom,
                    "MM-dd-yyyy",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out from);
                    bool chValidityto = DateTime.TryParseExact(
                    dateto,
                    "MM-dd-yyyy",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out to);
                    var parameterDatefrom = DateTime.ParseExact(datefrom, "MM-dd-yyyy", CultureInfo.InvariantCulture);
                    var parameterDateto = DateTime.ParseExact(dateto, "MM-dd-yyyy", CultureInfo.InvariantCulture);
                    var todaysDate = DateTime.Today;

                   
                    if (deptcode != 0 && subdepartment == 0 && classcode == 0 && subclasscode == 0)
                    {
                        if (parameterDatefrom < todaysDate)
                        {
                            return Json(new { success = false, message = $"There is an invalid DateFrom for Club Code : {clubcode}. You cannot set date on DateFrom that later than today's date." }, JsonRequestBehavior.AllowGet);
                        }

                        if (parameterDateto < todaysDate || parameterDateto < parameterDatefrom)
                        {
                            return Json(new { success = false, message = $"There is an invalid DateFrom for Club Code : {clubcode}. You cannot set date on DateTo that later than today's date or the DateTo is less than to the DateFrom ." }, JsonRequestBehavior.AllowGet);
                        }
                        if (chValidityfrom == false)
                        {
                            return Json(new { success = false, message = $"There is an invalid Date From for Club Code : {clubcode}. The Date format should be MM-dd-yyyy" }, JsonRequestBehavior.AllowGet);
                        }
                        if (chValidityto == false)
                        {
                            return Json(new { success = false, message = $"There is an invalid Date To for Club Code : {clubcode}. The Date format should be MM-dd-yyyy" }, JsonRequestBehavior.AllowGet);
                        }
                        if (verify == 0)
                        {
                            return Json(new { success = false, message = $"Department Not Found in SKU setup with Multiplier! Please setup first on SKU Setup" }, JsonRequestBehavior.AllowGet);
                        }

                        if (insertSKUboolSKUDept == true)
                        {

                            queryskus.UpdateSKUDept(clubcode, multiplieradjustment, deptcode, datefrom, dateto);


                        }
                        if (insertDeptbool == true)
                        {

                            queryskus.UpdateOverDept(clubcode, multiplieradjustment, deptcode, datefrom, dateto,userName,todaysDates);

                        }

                        else if (insertDeptbool == false)
                        {

                            queryskus.InsertOverDept(clubcode, multiplieradjustment, deptcode, subdepartment, classcode, subclasscode, 0, 0, datefrom, dateto,userName,todaysDates);
                        }

                    }

                    else if (deptcode != 0 && subdepartment != 0 && classcode == 0 && subclasscode == 0)
                    {
                        if (parameterDatefrom < todaysDate)
                        {
                            return Json(new { success = false, message = $"There is an invalid DateFrom for Club Code : {clubcode}. You cannot set date on DateFrom that later than today's date." }, JsonRequestBehavior.AllowGet);
                        }

                        if (parameterDateto < todaysDate || parameterDateto < parameterDatefrom)
                        {
                            return Json(new { success = false, message = $"There is an invalid DateFrom for Club Code : {clubcode}. You cannot set date on DateTo that later than today's date or the DateTo is less than to the DateFrom ." }, JsonRequestBehavior.AllowGet);
                        }
                        if (chValidityfrom == false)
                        {
                            return Json(new { success = false, message = $"There is an invalid Date From for Club Code : {clubcode}. The Date format should be MM-dd-yyyy" }, JsonRequestBehavior.AllowGet);
                        }
                        if (chValidityto == false)
                        {
                            return Json(new { success = false, message = $"There is an invalid Date To for Club Code : {clubcode}. The Date format should be MM-dd-yyyy" }, JsonRequestBehavior.AllowGet);
                        }
                        if (verifysub == 0)
                        {
                            return Json(new { success = false, message = $"Sub-Department Not Found in SKU setup with Multiplier! Please setup first on SKU Setup" }, JsonRequestBehavior.AllowGet);
                        }

                        if (insertSKUboolSKUSubDept == true)
                        {

                            queryskus.UpdateSKUSubDept(clubcode, multiplieradjustment, deptcode, subdepartment, datefrom, dateto);


                        }
                        if (insertDeptboolsub == true)
                        {

                            queryskus.UpdateOverSubDept(clubcode, multiplieradjustment, deptcode, subdepartment, datefrom, dateto, userName, todaysDates);

                        }

                        else if (insertDeptboolsub == false)
                        {

                            queryskus.InsertOverSubDept(clubcode, multiplieradjustment, deptcode, subdepartment, classcode, subclasscode, 0, 0, datefrom, dateto, userName, todaysDates);
                        }

                    }
                    else if (deptcode != 0 && subdepartment != 0 && classcode != 0 && subclasscode == 0)
                    {
                        if (parameterDatefrom < todaysDate)
                        {
                            return Json(new { success = false, message = $"There is an invalid DateFrom for Club Code : {clubcode}. You cannot set date on DateFrom that later than today's date." }, JsonRequestBehavior.AllowGet);
                        }

                        if (parameterDateto < todaysDate || parameterDateto < parameterDatefrom)
                        {
                            return Json(new { success = false, message = $"There is an invalid DateFrom for Club Code : {clubcode}. You cannot set date on DateTo that later than today's date or the DateTo is less than to the DateFrom ." }, JsonRequestBehavior.AllowGet);
                        }
                        if (chValidityfrom == false)
                        {
                            return Json(new { success = false, message = $"There is an invalid Date From for Club Code : {clubcode}. The Date format should be MM-dd-yyyy" }, JsonRequestBehavior.AllowGet);
                        }
                        if (chValidityto == false)
                        {
                            return Json(new { success = false, message = $"There is an invalid Date To for Club Code : {clubcode}. The Date format should be MM-dd-yyyy" }, JsonRequestBehavior.AllowGet);
                        }
                        if (verifyclass == 0)
                        {
                            return Json(new { success = false, message = $"Class Not Found in SKU setup with Multiplier! Please setup first on SKU Setup" }, JsonRequestBehavior.AllowGet);
                        }

                        if (insertSKUboolSKUClass == true)
                        {

                            queryskus.UpdateSKUClass(clubcode, multiplieradjustment, deptcode, subdepartment, classcode, datefrom, dateto);


                        }
                        if (insertDeptboolclass == true)
                        {

                            queryskus.UpdateOverClass(clubcode, multiplieradjustment, deptcode, subdepartment, classcode, datefrom, dateto, userName, todaysDates);

                        }

                        else if (insertDeptboolclass == false)
                        {

                            queryskus.InsertOverClass(clubcode, multiplieradjustment, deptcode, subdepartment, classcode, subclasscode, 0, 0, datefrom, dateto, userName, todaysDates);
                        }
                    }
                    else if (deptcode != 0 && subdepartment != 0 && classcode != 0 && subclasscode != 0)
                    {
                        if (parameterDatefrom < todaysDate)
                        {
                            return Json(new { success = false, message = $"There is an invalid DateFrom for Club Code : {clubcode}. You cannot set date on DateFrom that later than today's date." }, JsonRequestBehavior.AllowGet);
                        }

                        if (parameterDateto < todaysDate || parameterDateto < parameterDatefrom)
                        {
                            return Json(new { success = false, message = $"There is an invalid DateFrom for Club Code : {clubcode}. You cannot set date on DateTo that later than today's date or the DateTo is less than to the DateFrom ." }, JsonRequestBehavior.AllowGet);
                        }
                        if (chValidityfrom == false)
                        {
                            return Json(new { success = false, message = $"There is an invalid Date From for Club Code : {clubcode}. The Date format should be MM-dd-yyyy" }, JsonRequestBehavior.AllowGet);
                        }
                        if (chValidityto == false)
                        {
                            return Json(new { success = false, message = $"There is an invalid Date To for Club Code : {clubcode}. The Date format should be MM-dd-yyyy" }, JsonRequestBehavior.AllowGet);
                        }
                        if (verifysubclass == 0)
                        {
                            return Json(new { success = false, message = $"Sub-Class Not Found in SKU setup with Multiplier! Please setup first on SKU Setup" }, JsonRequestBehavior.AllowGet);
                        }
                        if (insertSKUboolSKUSubClass == true)
                        {

                            queryskus.UpdateSKUSubClass(clubcode, multiplieradjustment, deptcode, subdepartment, classcode, subclasscode, datefrom, dateto);


                        }
                        if (insertDeptboolsubclass == true)
                        {

                            queryskus.UpdateOverSubClass(clubcode, multiplieradjustment, deptcode, subdepartment, classcode, subclasscode, datefrom, dateto, userName, todaysDates);

                        }

                        else if (insertDeptboolsubclass == false)
                        {

                            queryskus.InsertOverSubClass(clubcode, multiplieradjustment, deptcode, subdepartment, classcode, subclasscode, 0, 0, datefrom, dateto, userName, todaysDates);
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