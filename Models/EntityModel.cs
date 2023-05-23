namespace SNRWMSPortal.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Web.UI.WebControls;

    public partial class EntityModel : DbContext
    {
        public EntityModel()
            : base("name=SQLConnect")
        {
        }

        public virtual DbSet<AllocationSKUModel> AllocationSKUs { get; set; }


        
        public object Type { get; internal set; }


    }
}
