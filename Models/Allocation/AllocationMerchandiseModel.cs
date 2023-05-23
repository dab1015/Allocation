using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SNRWMSPortal.Models
{
    public class AllocationMerchandiseModel
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

        
        [Display(Name = "DCM On Hand")]
        public int DCMOH { get; set; }


        [Display(Name = "DCL On Hand")]
        public int DCLOH { get; set; }

        [Display(Name = "DCP On Hand")]
        public int DCPOH { get; set; }

        [Display(Name = "DCB On Hand")]
        public int DCBOH { get; set; }

        
        [Display(Name = "PIC")]
        public string PIC { get; set; }

        public int Pieces { get; set; }

        public int STRNUM { get; set; }
        public int STSDAT { get; set; }

        public long INUMBR { get; set; }

        public string Sequence { get; set; }


        public string STRNAM { get; set; }

        public string ClubName{ get; set; }
        public int ClubCode { get; set; }

        public int RequestedQty { get; set; }
        public string Club { get; set; }
        public double ISTDPK { get; set; }

        public int IVPLHI { get; set; }

        public double NoPallets { get; set; }

        public double QtyIni { get; set; }

        public int NoCases { get; set; }

        public string Remarks { get; set; }

        public int QtyPerPallet { get; set; }

        public int IVPLTI { get; set; }

        public double IWGHT { get; set; }

        public int Checked { get; set; }

        public int Inputs { get; set; }


        public string IDESCR { get; set; }

        [Display(Name = "Staging Area")]
        public string StagingArea { get; set; }

        [Display(Name = "Batch Code")]
        public string BatchCode { get; set; }

        [Display(Name = "Door No.")]
        public int DoorNo { get; set; }

        public int QtyToPick { get; set; }
        public string Team { get; set; }

        public string ReasonName { get; set; }

        public string SlotLoc { get; set; }

        public string LPN { get; set; }

        public int TopCount { get; set; }


        [Display(Name = "DConfig")]
        public string DConfigName { get; set; }
        public string Reasons { get; set; }

        public string PrioritizationName { get; set; }

        public string StatusName { get; set; }
        public int Prioritization { get; set; }

        [Display(Name = "DConfig")]
        public int DConfig { get; set; }
        public int Status { get; set; }

        public long INUMBR1 { get; set; }
        public string LPN1 { get; set; }
        public string SlotLoc1 { get; set; }


        public List<AllocationMerchandiseModel> MerchandiseSKU { get; set; }

        public List<AllocationMerchandiseModel> MerchandiseWMS { get; set; }

        public List<AllocationMerchandiseModel> MerchandiseWMSCase { get; set; }





    }
}