using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SNRWMSPortal.Models
{
    public class AllocationClub
    {

        public int Id { get; set; }
        public int STRNUM { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [Display(Name = "SKU")]
        public long SKU { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [Display(Name = "Code")]
        public int CLubCode { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [Display(Name = "SKU MMS")]
        public int STSDAT { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [Display(Name = "Club")]
        public string STRNAM { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [Display(Name = "Club Name")]
        public string ClubName { get; set; }

        public string Description { get; set; }

        public int Reason { get; set; }

        public string Reasons { get; set; }

        public string Remarks { get; set; }

        public string RequestedDate { get; set; }

       


        public string ApprovedDate { get; set; }

        public int Pallet { get; set; }

        public int Layer { get; set; }


        //public string PConfig { get { return Pallet + "_" + Layer +"_" + ISTDPK; } }

        [Display(Name = "Pallet Config")]
        public string PConfig { get; set; }

        public double DCMOH { get; set; }

        [Display(Name = "Distribution Config")]
        public int DConfig { get; set; }

        [Display(Name = "DConfig")]
        public string DConfigName { get; set; }

        public string ReasonName { get; set; }

        public string Status { get; set; }

        [Display(Name = "Requested Qty(Units)")]
        public int Quantity { get; set; }



       



        [Display(Name = "Pallet Qty")]
        public int PQty { get; set; }

        [Display(Name = "Store OH")]
        public double StoreOH { get; set; }

        public double InTransit { get; set; }

        public double AveSales { get; set; }


        [Display(Name = "Store WS")]
        public double StoreWS { get; set; }

        public double ISTDPK { get; set; }
        public int IVPLTI { get; set; }
        public int IVPLHI { get; set; }

        public int Prio { get; set; }



        //[Required(ErrorMessage = "This field is required")]
        //[Display(Name = "Average Sales")]
        //public double AveSales { get; set; }

        public List<AllocationClub> Clubs { get; set; }

        public List<AllocationClub> ClubReq { get; set; }





    }
}