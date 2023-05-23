using CrystalDecisions.ReportAppServer;
using SNRWMSPortal.Reports;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SNRWMSPortal.Models
{
    public class AllocationSKUModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [Display(Name = "SKU")]
        public long SKU { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [Display(Name = "Description")]
        public string Description { get; set; }

        
        [Display(Name = "Status")]
        public string Status { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [Display(Name = "Distribution Config")]
        public int DConfig { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [Display(Name = "Distribution Quantity")]
        public int DConfigQty { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [Display(Name = "Truck Load Category")]
        public int Category { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [Display(Name = "Average Sales Per Day")]
        public double AveSalesPerDay { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [Display(Name = "Minimum Club Display")]
        public int? Minimum { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [Display(Name = "Minimum Club Display")]
        public int? Multiplier { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [Display(Name = "Lead Time")]
        public int? LeadTime { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [Display(Name = "Trigger")]
        public double Triggers { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [Display(Name = "Build To")]
        public double BuildTo { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [Display(Name = "On-Hand")]
        public int OnHand { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [Display(Name = "In-Transit")]
        public int InTransit { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [Display(Name = "OHIT")]
        public int OHIT { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [Display(Name = "ClubNeeds")]
        public double ClubNeeds { get; set; }

        public double NeededOrder { get; set; }

        public int RoundedClubNeeds { get; set; }

        public int ClubStatus { get; set; }

        public int CurrentMultiplier { get; set; }

        public int IsProvince { get; set; }

        //  public int? ClubId { get; set; }

        public string BuildToDF { get; set; }

        public string Username { get; set; }

        public int STRNUM { get; set; }
        public int STSDAT { get; set; }

        public long INUMBR { get; set; }

        public int ISTORE { get; set; }

        public double IBHAND { get; set; }

        public double IBINTQ { get; set; }

        public double OHITsum { get; set; }

        public double ISTDPK { get; set; }
        public double IVPLTI { get; set; }
        public double IVPLHI { get; set; }

        public string STRNAM { get; set; }

        public string ClubName{ get; set; }
        public int ClubCode { get; set; }

        public string ConfigExcel { get; set; }

        public string CategoryExcel { get; set; }
        
        public int IDEPT { get; set; }
        public int ISDEPT  { get; set; }

        public int ICLAS { get; set; }
        public int ISCLAS { get; set; }
        public string IATRB1 { get; set; }
        
        public string IMDATE { get; set; }

        public double Pallet { get; set; }
        public double Layer { get; set; }
        public double Case { get; set; }

        public int Department { get; set; }
        public int SubDepartment { get; set; }
        public int ClassName { get; set; }
        public int SubClass { get; set; }
        public int Vendor { get; set; }

        public int No { get; set; }


        public string DepartementMMS { get; set; }


        public string SubDepartementMMS { get; set; }

        public double DCInv { get; set; }

        public int Allocated { get; set; }
        public int Served { get; set; }

        public int Unserved { get; set; }

        public string DateCreated { get; set; }

        public int AllocatedQty { get; set; }

        public int ServedQty { get; set; }

        public int SKUPercentage { get; set; }

        public int QtyPercentage { get; set; }

        public int TruckNo { get; set; }

        public int UnservedQty { get; set; }

        public string Remarks { get; set; }

        public string UnservedDAte { get; set; }

        public string DConfigName { get; set; }

        public string CategoryName { get; set; }

        public string DateModified { get; set; }

        public string Club { get; set; }

        public int DateReceipt { get; set; }

        public string Reason { get; set; }

        public int Quantity { get; set; }


        public List<AllocationSKUModel> ClubSKU { get; set; }

        public List<AllocationSKUModel> ClubInval { get; set; }


        

        
    }
}