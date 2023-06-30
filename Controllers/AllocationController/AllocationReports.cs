using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json.Linq;
using SNRWMSPortal.Common;
using SNRWMSPortal.DataAccess;
using SNRWMSPortal.Models;
using SNRWMSPortal.Reports;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.ConstrainedExecution;
using System.Security.Cryptography.Pkcs;
using System.Threading;
using System.Web;
using System.Web.Helpers;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.UI;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Security.Cryptography;
using System.Globalization;

namespace SNRWMSPortal.Controllers
{
  //  [AuthorizeRoles(Role.Allocation, Role.SystemAdministrator)]
    public class AllocationReportsController : Controller
    {

        SQLQueryAllocationReport queryskus = new SQLQueryAllocationReport();
        public ActionResult Index()
        {
            try { 
                return View();
            }

            catch (Exception ex)
            {
                return Json(ex, JsonRequestBehavior.AllowGet);
            }

        }


        [HttpGet]
        public ActionResult SearchSKU(string from,string to,int report)
        {
           
            DateTime dateTimeFrom = DateTime.ParseExact(from, "yyMMdd", CultureInfo.InvariantCulture);
            string formattedDateTimeFrom = dateTimeFrom.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

            DateTime dateTimeTo = DateTime.ParseExact(to, "yyMMdd", CultureInfo.InvariantCulture);
            string formattedDateTimeTo = dateTimeTo.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            if (report == 1)
            {
                List<AllocationSKUModel> results = queryskus.Daily_Service_Level(formattedDateTimeFrom, formattedDateTimeTo);

                //Name of File  
                string fileName = "DailyServiceLevel.xlsx";
                using (XLWorkbook wb = new XLWorkbook())
                {

                    var ws = wb.Worksheets.Add("DailyServiceLevel");
                    var currentrow = 5;
                    ws.Cell(1, 1).Value = "Report Name : Daily Service Level Report";
                    ws.Cell(2, 1).Value = "Date and Time: " + DateTime.Now.ToString("dd MMM yyyy hh:mm:ss tt");
                    ws.Cell(4, 3).Value = "OVERALL NO. OF SKU";
                    ws.Cell(4, 7).Value = "OVERALL NO. OF QUANTITY";
                    ws.Cell(currentrow, 1).Value = "CLUBCODE";
                    ws.Cell(currentrow, 2).Value = "ALLOCATED";
                    ws.Cell(currentrow, 3).Value = "SERVED";
                    ws.Cell(currentrow, 4).Value = "UNSERVED";
                    ws.Cell(currentrow, 5).Value = "SERVED %";
                    ws.Cell(currentrow, 6).Value = "ALLOCATED";
                    ws.Cell(currentrow, 7).Value = "SERVED";
                    ws.Cell(currentrow, 8).Value = "SERVED %";
                    ws.Cell(currentrow, 9).Value = "NO. OF TRUCKS";


                    ws.Row(4).Style.Font.Bold = true;
                    ws.Row(currentrow).Style.Font.Bold = true;
                    ws.Row(currentrow).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    ws.Row(4).Style.Font.FontColor = XLColor.Yellow;
                    ws.Row(currentrow).Style.Font.FontColor = XLColor.Yellow;
                    //ws.Columns().AdjustToContents();

                    ws.Cell(currentrow, 1).WorksheetColumn().Width = 10;
                    ws.Cell(currentrow, 2).WorksheetColumn().Width = 15;
                    ws.Cell(currentrow, 3).WorksheetColumn().Width = 15;
                    ws.Cell(currentrow, 4).WorksheetColumn().Width = 15;
                    ws.Cell(currentrow, 5).WorksheetColumn().Width = 15;
                    ws.Cell(currentrow, 6).WorksheetColumn().Width = 15;
                    ws.Cell(currentrow, 7).WorksheetColumn().Width = 15;
                    ws.Cell(currentrow, 8).WorksheetColumn().Width = 15;
                    ws.Cell(currentrow, 9).WorksheetColumn().Width = 15;






                    // ws.Row(currentrow).Style.Fill.BackgroundColor = XLColor.Blue;
                    ws.Range("A4", "I4").Style.Fill.BackgroundColor = XLColor.DarkBlue;
                    ws.Range("A5", "I5").Style.Fill.BackgroundColor = XLColor.DarkBlue;
                    //  wb.Worksheets.Add(ds);
                    foreach (var rep in results)
                    {
                        currentrow++;
                        ws.Cell(currentrow, 1).Value = rep.ClubCode;
                        ws.Cell(currentrow, 2).Value = rep.Allocated;
                        ws.Cell(currentrow, 3).Value = rep.Served;
                        ws.Cell(currentrow, 4).Value = rep.Unserved;
                        ws.Cell(currentrow, 5).Value = rep.SKUPercentage;
                        ws.Cell(currentrow, 6).Value = rep.AllocatedQty;
                        ws.Cell(currentrow, 7).Value = rep.ServedQty;
                        ws.Cell(currentrow, 8).Value = rep.QtyPercentage;
                        ws.Cell(currentrow, 9).Value = rep.TruckNo;


                        ws.Cell(currentrow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        ws.Cell(currentrow, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        ws.Cell(currentrow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        ws.Cell(currentrow, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        ws.Cell(currentrow, 5).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        ws.Cell(currentrow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        ws.Cell(currentrow, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        ws.Cell(currentrow, 8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        ws.Cell(currentrow, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);




                    }


                    using (MemoryStream stream = new MemoryStream())
                    {
                        wb.SaveAs(stream);

                        //Return xlsx Excel File  
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                    }

                }
            }

            else if (report == 2)
            {
                List<AllocationSKUModel> results = queryskus.Summary_Service_Level(formattedDateTimeFrom, formattedDateTimeTo);

                //Name of File  
                string fileName = "SummaryServiceLevel.xlsx";
                using (XLWorkbook wb = new XLWorkbook())
                {

                    var ws = wb.Worksheets.Add("SummaryServiceLevel");
                    var currentrow = 4;
                    ws.Cell(1, 1).Value = "Report Name : Summary Service Level Report";
                    ws.Cell(2, 1).Value = "Date and Time: " + DateTime.Now.ToString("dd MMM yyyy hh:mm:ss tt");
                   
                    ws.Cell(currentrow, 1).Value = "DATE";
                    ws.Cell(currentrow, 2).Value = "SL % BASED ON NO.OF SKU";
                    ws.Cell(currentrow, 3).Value = "SL % BASED ON NO.OF QTY";
                    ws.Cell(currentrow, 4).Value = "# OF TRUCKS";
                    ws.Cell(currentrow, 5).Value = "REMARKS";
                    


                    ws.Row(4).Style.Font.Bold = true;
                    ws.Row(currentrow).Style.Font.Bold = true;
                    ws.Row(currentrow).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    ws.Row(4).Style.Font.FontColor = XLColor.Yellow;
                    ws.Row(currentrow).Style.Font.FontColor = XLColor.Yellow;
                    //ws.Columns().AdjustToContents();

                    ws.Cell(currentrow, 1).WorksheetColumn().Width = 15;
                    ws.Cell(currentrow, 2).WorksheetColumn().Width = 25;
                    ws.Cell(currentrow, 3).WorksheetColumn().Width = 25;
                    ws.Cell(currentrow, 4).WorksheetColumn().Width = 15;
                    ws.Cell(currentrow, 5).WorksheetColumn().Width = 15;
                    
                    // ws.Row(currentrow).Style.Fill.BackgroundColor = XLColor.Blue;
                    ws.Range("A4", "E4").Style.Fill.BackgroundColor = XLColor.DarkBlue;
                   
                    //  wb.Worksheets.Add(ds);
                    foreach (var rep in results)
                    {
                        currentrow++;
                        ws.Cell(currentrow, 1).Value = rep.DateCreated;
                        ws.Cell(currentrow, 2).Value = rep.SKUPercentage;
                        ws.Cell(currentrow, 3).Value = rep.QtyPercentage;
                        ws.Cell(currentrow, 4).Value = rep.TruckNo;
                        ws.Cell(currentrow, 5).Value = rep.Remarks;
                        
                        ws.Cell(currentrow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        ws.Cell(currentrow, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        ws.Cell(currentrow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        ws.Cell(currentrow, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        ws.Cell(currentrow, 5).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        

                    }


                    using (MemoryStream stream = new MemoryStream())
                    {
                        wb.SaveAs(stream);

                        //Return xlsx Excel File  
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                    }

                }
            }

            else if (report == 3)
            {
                List<AllocationSKUModel> results = queryskus.Unserved_SKU(formattedDateTimeFrom, formattedDateTimeTo);

                //Name of File  
                string fileName = "UnservedSKU.xlsx";
                using (XLWorkbook wb = new XLWorkbook())
                {

                    var ws = wb.Worksheets.Add("UnservedSKU");
                    var currentrow = 4;
                    ws.Cell(1, 1).Value = "Report Name : Unserved SKUs";
                    ws.Cell(2, 1).Value = "Date and Time: " + DateTime.Now.ToString("dd MMM yyyy hh:mm:ss tt");
                    ws.Cell(currentrow, 1).Value = "CLUBCODE";
                    ws.Cell(currentrow, 2).Value = "SKU";
                    ws.Cell(currentrow, 3).Value = "DESCRIPTION";
                    ws.Cell(currentrow, 4).Value = "DEPARTMENT";
                    ws.Cell(currentrow, 5).Value = "SUB-DEPARTMENT";
                    ws.Cell(currentrow, 6).Value = "STATUS";
                    ws.Cell(currentrow, 7).Value = "L/I";
                    ws.Cell(currentrow, 8).Value = "UNSERVED DATE";
                    ws.Cell(currentrow, 9).Value = "ALLOCATED QTY";
                    ws.Cell(currentrow, 10).Value = "UNSERVED QTY";
                    ws.Cell(currentrow, 11).Value = "REMARKS";
                    ws.Row(currentrow).Style.Font.Bold = true;
                    ws.Row(currentrow).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    ws.Row(currentrow).Style.Font.FontColor = XLColor.Yellow;
                    //ws.Columns().AdjustToContents();

                    ws.Cell(currentrow, 1).WorksheetColumn().Width = 10;
                    ws.Cell(currentrow, 2).WorksheetColumn().Width = 10;
                    ws.Cell(currentrow, 3).WorksheetColumn().Width = 30;
                    ws.Cell(currentrow, 4).WorksheetColumn().Width = 30;
                    ws.Cell(currentrow, 5).WorksheetColumn().Width = 30;
                    ws.Cell(currentrow, 6).WorksheetColumn().Width = 8;
                    ws.Cell(currentrow, 7).WorksheetColumn().Width = 8;
                    ws.Cell(currentrow, 8).WorksheetColumn().Width = 13;
                    ws.Cell(currentrow, 9).WorksheetColumn().Width = 13;
                    ws.Cell(currentrow, 10).WorksheetColumn().Width = 13;
                    ws.Cell(currentrow, 11).WorksheetColumn().Width = 15;




                    // ws.Row(currentrow).Style.Fill.BackgroundColor = XLColor.Blue;
                    ws.Range("A4", "K4").Style.Fill.BackgroundColor = XLColor.DarkBlue;
                    //  wb.Worksheets.Add(ds);
                    foreach (var rep in results)
                    {
                        currentrow++;
                        ws.Cell(currentrow, 1).Value = rep.ClubCode;
                        ws.Cell(currentrow, 2).Value = rep.SKU;
                        ws.Cell(currentrow, 3).Value = rep.Description;
                        ws.Cell(currentrow, 4).Value = rep.DepartementMMS;
                        ws.Cell(currentrow, 5).Value = rep.SubDepartementMMS;
                        ws.Cell(currentrow, 6).Value = rep.Status;
                        ws.Cell(currentrow, 7).Value = rep.IATRB1;
                        ws.Cell(currentrow, 8).Value = rep.UnservedDAte;
                        ws.Cell(currentrow, 9).Value = rep.AllocatedQty;
                        ws.Cell(currentrow, 10).Value = rep.UnservedQty;
                        ws.Cell(currentrow, 11).Value = rep.Remarks;
                        ws.Cell(currentrow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        ws.Cell(currentrow, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        ws.Cell(currentrow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        ws.Cell(currentrow, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        ws.Cell(currentrow, 5).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        ws.Cell(currentrow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        ws.Cell(currentrow, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        ws.Cell(currentrow, 8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        ws.Cell(currentrow, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        ws.Cell(currentrow, 10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        ws.Cell(currentrow, 11).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);


                    }


                    using (MemoryStream stream = new MemoryStream())
                    {
                        wb.SaveAs(stream);

                        //Return xlsx Excel File  
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                    }
                }
            }
            else if (report == 4)
            {
                List<AllocationSKUModel> results = queryskus.GetNewSKUDate(from, to);

                //Name of File  
                string fileName = "NewSKU.xlsx";
                using (XLWorkbook wb = new XLWorkbook())
                {

                    var ws = wb.Worksheets.Add("NewSKU");
                    var currentrow = 4;
                    ws.Cell(1, 1).Value = "Report Name : New SKU for Set-up";
                    ws.Cell(2, 1).Value = "Date and Time: " + DateTime.Now.ToString("dd MMM yyyy hh:mm:ss tt");
                    ws.Cell(currentrow, 1).Value = "SKU";
                    ws.Cell(currentrow, 2).Value = "DESCRIPTION";
                    ws.Cell(currentrow, 3).Value = "DEPARTMENT";
                    ws.Cell(currentrow, 4).Value = "SUB-DEPARTMENT";
                    ws.Cell(currentrow, 5).Value = "STATUS";
                    ws.Cell(currentrow, 6).Value = "L/I";
                    ws.Cell(currentrow, 7).Value = "DATE OF RECIEPT";
                    ws.Cell(currentrow, 8).Value = "DC INVENTORY";
                    ws.Cell(currentrow, 9).Value = "UNIT PER CASE";
                    ws.Cell(currentrow, 10).Value = "UNIT PER LAYER";
                    ws.Cell(currentrow, 11).Value = "UNIT PER PALLET";
                    ws.Row(currentrow).Style.Font.Bold = true;
                    ws.Row(currentrow).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    ws.Row(currentrow).Style.Font.FontColor = XLColor.Yellow;
                    //ws.Columns().AdjustToContents();

                    ws.Cell(currentrow, 1).WorksheetColumn().Width = 10;
                    ws.Cell(currentrow, 2).WorksheetColumn().Width = 40;
                    ws.Cell(currentrow, 3).WorksheetColumn().Width = 28;
                    ws.Cell(currentrow, 4).WorksheetColumn().Width = 25;
                    ws.Cell(currentrow, 7).WorksheetColumn().Width = 15;
                    ws.Cell(currentrow, 8).WorksheetColumn().Width = 15;
                    ws.Cell(currentrow, 9).WorksheetColumn().Width = 13;
                    ws.Cell(currentrow, 10).WorksheetColumn().Width = 13;
                    ws.Cell(currentrow, 11).WorksheetColumn().Width = 13;




                    // ws.Row(currentrow).Style.Fill.BackgroundColor = XLColor.Blue;
                    ws.Range("A4", "K4").Style.Fill.BackgroundColor = XLColor.DarkBlue;
                    //  wb.Worksheets.Add(ds);
                    foreach (var rep in results)
                    {
                        currentrow++;
                        ws.Cell(currentrow, 1).Value = rep.SKU;
                        ws.Cell(currentrow, 2).Value = rep.Description;
                        ws.Cell(currentrow, 3).Value = rep.DepartementMMS;
                        ws.Cell(currentrow, 4).Value = rep.SubDepartementMMS;
                        ws.Cell(currentrow, 5).Value = rep.Status;
                        ws.Cell(currentrow, 6).Value = rep.IATRB1;
                        ws.Cell(currentrow, 7).Value = rep.IMDATE;
                        ws.Cell(currentrow, 8).Value = rep.DCInv;
                        ws.Cell(currentrow, 9).Value = rep.Case;
                        ws.Cell(currentrow, 10).Value = rep.Layer;
                        ws.Cell(currentrow, 11).Value = rep.Pallet;
                        ws.Cell(currentrow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        ws.Cell(currentrow, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        ws.Cell(currentrow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        ws.Cell(currentrow, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        ws.Cell(currentrow, 5).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        ws.Cell(currentrow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        ws.Cell(currentrow, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        ws.Cell(currentrow, 8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        ws.Cell(currentrow, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        ws.Cell(currentrow, 10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        ws.Cell(currentrow, 11).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);


                    }


                    using (MemoryStream stream = new MemoryStream())
                    {
                        wb.SaveAs(stream);

                        //Return xlsx Excel File  
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                    }

                }


            }
            else if (report == 5)
            {
                List<AllocationSKUModel> results = queryskus.SKU_ClubSetup(formattedDateTimeFrom, formattedDateTimeTo);

                //Name of File  
                string fileName = "SKU_ClubSetup.xlsx";
                using (XLWorkbook wb = new XLWorkbook())
                {

                    var ws = wb.Worksheets.Add("SKUClubSetup");
                    var currentrow = 4;
                    ws.Cell(1, 1).Value = "Report Name : SKU Club Setup";
                    ws.Cell(2, 1).Value = "Date and Time: " + DateTime.Now.ToString("dd MMM yyyy hh:mm:ss tt");
                    ws.Cell(currentrow, 1).Value = "CLUBCODE";
                    ws.Cell(currentrow, 2).Value = "SKU";
                    ws.Cell(currentrow, 3).Value = "DESCRIPTION";
                    ws.Cell(currentrow, 4).Value = "STATUS";
                    ws.Cell(currentrow, 5).Value = "AVERAGE SALES";
                    ws.Cell(currentrow, 6).Value = "DCONFIG";
                    ws.Cell(currentrow, 7).Value = "FORMULA";
                    ws.Cell(currentrow, 8).Value = "NEEDED ORDER";
                    ws.Cell(currentrow, 9).Value = "MINIMUM";
                    ws.Cell(currentrow, 10).Value = "MULTIPLIER";
                    ws.Cell(currentrow, 11).Value = "MULTIPLIER OVERIDE";
                    ws.Cell(currentrow, 12).Value = "LEAD TIME";
                    ws.Cell(currentrow, 13).Value = "TRIGGER";
                    ws.Cell(currentrow, 14).Value = "BUILD TO";
                    ws.Cell(currentrow, 15).Value = "CATEGORY";
                    ws.Cell(currentrow, 16).Value = "USERNAME";
                    ws.Cell(currentrow, 17).Value = "DATE CREATED";
                    ws.Row(currentrow).Style.Font.Bold = true;
                    ws.Row(currentrow).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    ws.Row(currentrow).Style.Font.FontColor = XLColor.Yellow;
                    //ws.Columns().AdjustToContents();

                    ws.Cell(currentrow, 1).WorksheetColumn().Width = 10;
                    ws.Cell(currentrow, 2).WorksheetColumn().Width = 10;
                    ws.Cell(currentrow, 3).WorksheetColumn().Width = 30;
                    ws.Cell(currentrow, 4).WorksheetColumn().Width = 10;
                    ws.Cell(currentrow, 5).WorksheetColumn().Width = 15;
                    ws.Cell(currentrow, 6).WorksheetColumn().Width = 10;
                    ws.Cell(currentrow, 7).WorksheetColumn().Width = 15;
                    ws.Cell(currentrow, 8).WorksheetColumn().Width = 15;
                    ws.Cell(currentrow, 9).WorksheetColumn().Width = 15;
                    ws.Cell(currentrow, 10).WorksheetColumn().Width = 15;
                    ws.Cell(currentrow, 11).WorksheetColumn().Width = 17;
                    ws.Cell(currentrow, 12).WorksheetColumn().Width = 15;
                    ws.Cell(currentrow, 13).WorksheetColumn().Width = 15;
                    ws.Cell(currentrow, 14).WorksheetColumn().Width = 17;
                    ws.Cell(currentrow, 15).WorksheetColumn().Width = 17;
                    ws.Cell(currentrow, 16).WorksheetColumn().Width = 17;
                    ws.Cell(currentrow, 17).WorksheetColumn().Width = 17;




                    // ws.Row(currentrow).Style.Fill.BackgroundColor = XLColor.Blue;
                    ws.Range("A4", "Q4").Style.Fill.BackgroundColor = XLColor.DarkBlue;
                    //  wb.Worksheets.Add(ds);
                    foreach (var rep in results)
                    {
                        currentrow++;
                        ws.Cell(currentrow, 1).Value = rep.ClubCode;
                        ws.Cell(currentrow, 2).Value = rep.SKU;
                        ws.Cell(currentrow, 3).Value = rep.Description;
                        ws.Cell(currentrow, 4).Value = rep.Status;
                        ws.Cell(currentrow, 5).Value = rep.AveSalesPerDay;
                        ws.Cell(currentrow, 6).Value = rep.DConfigName;
                        ws.Cell(currentrow, 7).Value = rep.BuildToDF;
                        ws.Cell(currentrow, 8).Value = rep.NeededOrder;
                        ws.Cell(currentrow, 9).Value = rep.Minimum;
                        ws.Cell(currentrow, 10).Value = rep.Multiplier;
                        ws.Cell(currentrow, 11).Value = rep.CurrentMultiplier;
                        ws.Cell(currentrow, 12).Value = rep.LeadTime;
                        ws.Cell(currentrow, 13).Value = rep.Triggers;
                        ws.Cell(currentrow, 14).Value = rep.BuildTo;
                        ws.Cell(currentrow, 15).Value = rep.CategoryName;
                        ws.Cell(currentrow, 16).Value = rep.Username;
                        ws.Cell(currentrow, 17).Value = rep.DateCreated;
                        ws.Cell(currentrow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        ws.Cell(currentrow, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        ws.Cell(currentrow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        ws.Cell(currentrow, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        ws.Cell(currentrow, 5).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        ws.Cell(currentrow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        ws.Cell(currentrow, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        ws.Cell(currentrow, 8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        ws.Cell(currentrow, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        ws.Cell(currentrow, 10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        ws.Cell(currentrow, 11).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        ws.Cell(currentrow, 12).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        ws.Cell(currentrow, 13).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        ws.Cell(currentrow, 14).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        ws.Cell(currentrow, 15).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        ws.Cell(currentrow, 16).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        ws.Cell(currentrow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);


                    }


                    using (MemoryStream stream = new MemoryStream())
                    {
                        wb.SaveAs(stream);

                        //Return xlsx Excel File  
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                    }
                }
            }
            else if (report == 6)
            {
                List<AllocationSKUModel> results = queryskus.Club_Request(from, to);

                //Name of File  
                string fileName = "Summary of Club Request.xlsx";
                using (XLWorkbook wb = new XLWorkbook())
                {

                    var ws = wb.Worksheets.Add("ClubRequest");
                    var currentrow = 4;
                    ws.Cell(1, 1).Value = "Report Name : Summary of Club Request";
                    ws.Cell(2, 1).Value = "Date and Time: " + DateTime.Now.ToString("dd MMM yyyy hh:mm:ss tt");
                    ws.Cell(currentrow, 1).Value = "DATE";
                    ws.Cell(currentrow, 2).Value = "CLUBCODE";
                    ws.Cell(currentrow, 3).Value = "SKU";
                    ws.Cell(currentrow, 4).Value = "DESCRIPTION";
                    ws.Cell(currentrow, 5).Value = "QUANTITY";
                    ws.Cell(currentrow, 6).Value = "REASON";
                    ws.Cell(currentrow, 7).Value = "REMARKS";
                    ws.Cell(currentrow, 8).Value = "USERNAME";

                    ws.Row(currentrow).Style.Font.Bold = true;
                    ws.Row(currentrow).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    ws.Row(currentrow).Style.Font.FontColor = XLColor.Yellow;
                    //ws.Columns().AdjustToContents();

                    ws.Cell(currentrow, 1).WorksheetColumn().Width = 17;
                    ws.Cell(currentrow, 2).WorksheetColumn().Width = 12;
                    ws.Cell(currentrow, 3).WorksheetColumn().Width = 12;
                    ws.Cell(currentrow, 4).WorksheetColumn().Width = 35;
                    ws.Cell(currentrow, 5).WorksheetColumn().Width = 12;
                    ws.Cell(currentrow, 6).WorksheetColumn().Width = 35;
                    ws.Cell(currentrow, 7).WorksheetColumn().Width = 20;
                    ws.Cell(currentrow, 8).WorksheetColumn().Width = 18;





                    // ws.Row(currentrow).Style.Fill.BackgroundColor = XLColor.Blue;
                    ws.Range("A4", "H4").Style.Fill.BackgroundColor = XLColor.DarkBlue;
                    //  wb.Worksheets.Add(ds);
                    foreach (var rep in results)
                    {
                        currentrow++;
                        ws.Cell(currentrow, 1).Value = rep.DateCreated;
                        ws.Cell(currentrow, 2).Value = rep.ClubCode;
                        ws.Cell(currentrow, 3).Value = rep.SKU;
                        ws.Cell(currentrow, 4).Value = rep.Description;
                        ws.Cell(currentrow, 5).Value = rep.Quantity;
                        ws.Cell(currentrow, 6).Value = rep.Reason;
                        ws.Cell(currentrow, 7).Value = rep.Remarks;
                        ws.Cell(currentrow, 8).Value = rep.Username;

                        ws.Cell(currentrow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        ws.Cell(currentrow, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        ws.Cell(currentrow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        ws.Cell(currentrow, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        ws.Cell(currentrow, 5).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        ws.Cell(currentrow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        ws.Cell(currentrow, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        ws.Cell(currentrow, 8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);



                    }


                    using (MemoryStream stream = new MemoryStream())
                    {
                        wb.SaveAs(stream);

                        //Return xlsx Excel File  
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                    }

                }
            }
            else if (report == 7)
            {
                List<AllocationSKUModel> results = queryskus.Allocation_Request(from, to);

                //Name of File  
                string fileName = "Summary of Allocation Request.xlsx";
                using (XLWorkbook wb = new XLWorkbook())
                {

                    var ws = wb.Worksheets.Add("AllocationRequest");
                    var currentrow = 4;
                    ws.Cell(1, 1).Value = "Report Name : Summary of Allocation Request";
                    ws.Cell(2, 1).Value = "Date and Time: " + DateTime.Now.ToString("dd MMM yyyy hh:mm:ss tt");
                    ws.Cell(currentrow, 1).Value = "DATE";
                    ws.Cell(currentrow, 2).Value = "CLUBCODE";
                    ws.Cell(currentrow, 3).Value = "SKU";
                    ws.Cell(currentrow, 4).Value = "DESCRIPTION";
                    ws.Cell(currentrow, 5).Value = "QUANTITY";
                    ws.Cell(currentrow, 6).Value = "REASON";
                    ws.Cell(currentrow, 7).Value = "USERNAME";
                    

                    ws.Row(currentrow).Style.Font.Bold = true;
                    ws.Row(currentrow).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    ws.Row(currentrow).Style.Font.FontColor = XLColor.Yellow;
                    //ws.Columns().AdjustToContents();

                    ws.Cell(currentrow, 1).WorksheetColumn().Width = 17;
                    ws.Cell(currentrow, 2).WorksheetColumn().Width = 12;
                    ws.Cell(currentrow, 3).WorksheetColumn().Width = 12;
                    ws.Cell(currentrow, 4).WorksheetColumn().Width = 35;
                    ws.Cell(currentrow, 5).WorksheetColumn().Width = 12;
                    ws.Cell(currentrow, 6).WorksheetColumn().Width = 35;
                    ws.Cell(currentrow, 7).WorksheetColumn().Width = 20;
                    





                    // ws.Row(currentrow).Style.Fill.BackgroundColor = XLColor.Blue;
                    ws.Range("A4", "G4").Style.Fill.BackgroundColor = XLColor.DarkBlue;
                    //  wb.Worksheets.Add(ds);
                    foreach (var rep in results)
                    {
                        currentrow++;
                        ws.Cell(currentrow, 1).Value = rep.DateCreated;
                        ws.Cell(currentrow, 2).Value = rep.ClubCode;
                        ws.Cell(currentrow, 3).Value = rep.SKU;
                        ws.Cell(currentrow, 4).Value = rep.Description;
                        ws.Cell(currentrow, 5).Value = rep.Quantity;
                        ws.Cell(currentrow, 6).Value = rep.Reason;
                        ws.Cell(currentrow, 7).Value = rep.Username;
                        

                        ws.Cell(currentrow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        ws.Cell(currentrow, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        ws.Cell(currentrow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        ws.Cell(currentrow, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        ws.Cell(currentrow, 5).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        ws.Cell(currentrow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        ws.Cell(currentrow, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                       



                    }


                    using (MemoryStream stream = new MemoryStream())
                    {
                        wb.SaveAs(stream);

                        //Return xlsx Excel File  
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                    }

                }
            }
            else if (report == 8)
            {
                List<AllocationSKUModel> results = queryskus.Merchandise_Request(from, to);

                //Name of File  
                string fileName = "Summary of Merchandise Request.xlsx";
                using (XLWorkbook wb = new XLWorkbook())
                {

                    var ws = wb.Worksheets.Add("MerchandiseRequest");
                    var currentrow = 4;
                    ws.Cell(1, 1).Value = "Report Name : Summary of Merchandise Request";
                    ws.Cell(2, 1).Value = "Date and Time: " + DateTime.Now.ToString("dd MMM yyyy hh:mm:ss tt");
                    ws.Cell(currentrow, 1).Value = "DATE";
                    ws.Cell(currentrow, 2).Value = "CLUBCODE";
                    ws.Cell(currentrow, 3).Value = "SKU";
                    ws.Cell(currentrow, 4).Value = "DESCRIPTION";
                    ws.Cell(currentrow, 5).Value = "QUANTITY";
                    ws.Cell(currentrow, 6).Value = "REASON";
                    ws.Cell(currentrow, 7).Value = "USERNAME";


                    ws.Row(currentrow).Style.Font.Bold = true;
                    ws.Row(currentrow).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    ws.Row(currentrow).Style.Font.FontColor = XLColor.Yellow;
                    //ws.Columns().AdjustToContents();

                    ws.Cell(currentrow, 1).WorksheetColumn().Width = 17;
                    ws.Cell(currentrow, 2).WorksheetColumn().Width = 12;
                    ws.Cell(currentrow, 3).WorksheetColumn().Width = 12;
                    ws.Cell(currentrow, 4).WorksheetColumn().Width = 35;
                    ws.Cell(currentrow, 5).WorksheetColumn().Width = 12;
                    ws.Cell(currentrow, 6).WorksheetColumn().Width = 35;
                    ws.Cell(currentrow, 7).WorksheetColumn().Width = 20;






                    // ws.Row(currentrow).Style.Fill.BackgroundColor = XLColor.Blue;
                    ws.Range("A4", "G4").Style.Fill.BackgroundColor = XLColor.DarkBlue;
                    //  wb.Worksheets.Add(ds);
                    foreach (var rep in results)
                    {
                        currentrow++;
                        ws.Cell(currentrow, 1).Value = rep.DateCreated;
                        ws.Cell(currentrow, 2).Value = rep.ClubCode;
                        ws.Cell(currentrow, 3).Value = rep.SKU;
                        ws.Cell(currentrow, 4).Value = rep.Description;
                        ws.Cell(currentrow, 5).Value = rep.Quantity;
                        ws.Cell(currentrow, 6).Value = rep.Reason;
                        ws.Cell(currentrow, 7).Value = rep.Username;


                        ws.Cell(currentrow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        ws.Cell(currentrow, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        ws.Cell(currentrow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        ws.Cell(currentrow, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        ws.Cell(currentrow, 5).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        ws.Cell(currentrow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        ws.Cell(currentrow, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);




                    }


                    using (MemoryStream stream = new MemoryStream())
                    {
                        wb.SaveAs(stream);

                        //Return xlsx Excel File  
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                    }

                }
            }
            return View();

            //ReportDocument rd = new ReportDocument();
            //rd.Load(Path.Combine(Server.MapPath("~/Reports"), "NewSKU.rpt"));
            //rd.SetDataSource(ds);
            //rd.SetDataSource(ds);

            //Response.Buffer = false;
            //Response.ClearContent();
            //Response.ClearHeaders();

            //try
            //{

            //    Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            //    stream.Seek(0, SeekOrigin.Begin);
            //    return File(stream, "application/pdf", "NewSKU.pdf");
            //    //ExcelFormatOptions excelOptions = new ExcelFormatOptions();
            //    //excelOptions.ExcelUseConstantColumnWidth = false;
            //    //excelOptions.ExcelTabHasColumnHeadings = true;
            //    //rd.ExportToDisk(ExportFormatType.Excel, "NewSKU.XLSX");
            //    //return View();
            //}
            //catch (Exception)
            //{
            //    throw;
            //}


        }







        public DataSet GeneratePdf(string from,string to)
        {
            DS_NewSKU reportDS = new DS_NewSKU();

            try
            {
                AllocationSKUModel model = new AllocationSKUModel();

                List<AllocationSKUModel> results = queryskus.GetNewSKUDate(from, to);


                foreach (AllocationSKUModel inv in results)
                {
                    reportDS.NewSKU.AddNewSKURow(inv.SKU.ToString(), inv.Description,
                        inv.IDEPT, inv.ISDEPT, inv.Status, inv.IATRB1, inv.IMDATE,
                        inv.DCInv, inv.Case, inv.Layer, inv.Pallet);

                    //reportDS.NewSKU.AddNewSKURow(100066, "desx", 9, 1, "A", "S", "112233", 3, 5, 6, 4);
                    //reportDS.NewSKU.AddNewSKURow(100066, "desx", 9, 1, "A", "S", "112233", 3, 5, 6, 4);
                    //reportDS.NewSKU.AddNewSKURow(100066, "desx", 9, 1, "A", "S", "112233", 3, 5, 6, 4);


                }
            }
            catch (Exception)
            {

                throw;
            }

            //SQLAccess.Dispose();


            var ds = new DataSet();

            return reportDS;
        }

        





        public DataTable GetReportDataTable(string from, string to)
        {
            var dt = new DataTable();

            dt.Columns.AddRange(new DataColumn[11] { new DataColumn("SKU"),
                                            new DataColumn("Description"),
                                            new DataColumn("Department"),
                                            new DataColumn("SubDepartment"),
                                            new DataColumn("Status"),
                                            new DataColumn("IATRB1"),
                                            new DataColumn("IMDATE"),
                                            new DataColumn("DCInv"),
                                            new DataColumn("Case"),
                                            new DataColumn("Layer"),
                                            new DataColumn("Pallet")

            });

            List<AllocationSKUModel> results = queryskus.GetNewSKUDate(from, to);
            foreach (var inv in results)
            {
                dt.Rows.Add(inv.SKU, inv.Description,
                        inv.IDEPT, inv.ISDEPT, inv.Status, inv.IATRB1, inv.IMDATE,
                        inv.DCInv, inv.Case, inv.Layer, inv.Pallet);


                
            }

            dt.AcceptChanges();

            return dt;
        }

    }

}