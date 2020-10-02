namespace HardwareStoreEF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Orders
    {
        [Key]
        public int OrderID { get; set; }

        public int UserID { get; set; }

        public int Amount { get; set; }

        public int ProductID { get; set; }

        public virtual Products Products { get; set; }

        public virtual Users Users { get; set; }
    }
}
