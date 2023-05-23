using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SNRWMSPortal.Models
{
    public class AllocationNearExpiryModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [Display(Name = "SKU")]
        public long SKU { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [Display(Name = "Description")]
        public string Description { get; set; }

        
        [Display(Name = "Reason")]
        public int Reason { get; set; }

       

        public int OneToThirty { get; set; }

        public int ThirtyoneToSixty { get; set; }

        public int SixtyoneAbove { get; set; }

        public int STRNUM { get; set; }
        public int STSDAT { get; set; }

        public long INUMBR { get; set; }

       
        public string STRNAM { get; set; }

        public string ClubName{ get; set; }
        public int ClubCode { get; set; }

        public int RequestedQty { get; set; }
        public string Club { get; set; }
        public double ISTDPK { get; set; }

        public string IDESCR { get; set; }

        [Display(Name = "Staging Area")]
        public string StagingArea { get; set; }

        [Display(Name = "Batch Code")]
        public string BatchCode { get; set; }

        [Display(Name = "Door No.")]
        public int DoorNo { get; set; }
        public string Team { get; set; }


        public string Reasons { get; set; }

        public string PrioritizationName { get; set; }

        public string StatusName { get; set; }
        public int Prioritization { get; set; }

        [Display(Name = "DConfig")]
        public int DConfig { get; set; }
        public int Status { get; set; }
        public List<AllocationNearExpiryModel> MerchandiseSKU { get; set; }

        


    }
}